using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Common.WaitFormCanceling;
using Dataedo.App.Data;
using Dataedo.App.Data.General;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.Tools.CommandLine.Tools;
using Dataedo.App.Tools.CommandLine.Xml;
using Dataedo.App.Tools.CommandLine.Xml.Commands.Base;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ImportCommand.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.GeneralHandling;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;

namespace Dataedo.App.Tools.CommandLine.ImportCommand;

internal class ImportProcessor : ProcessorBase
{
	private string summary;

	public ImportProcessor(Dataedo.App.Tools.CommandLine.Tools.Log log, string commandsFilePath)
		: base(log, commandsFilePath)
	{
	}

	protected override int ProcessInternal(CommandBase command, Form owner = null)
	{
		int handlingResultsCount = 0;
		if (command is Dataedo.App.Tools.CommandLine.Xml.Import import)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			if (command.IsEnabled)
			{
				try
				{
					if (InitializeRepository(command as RepositoryDocumentationCommandBase))
					{
						Initialization(import, ref handlingResultsCount);
						if (handlingResultsCount == 0)
						{
							ProcessImport(import, ref handlingResultsCount, owner);
							handlingResultsCount += LogHandlingResults(GeneralHandlingSupport.StoredHandlingResults);
							GeneralHandlingSupport.ClearStoredHandlingResults();
						}
						else
						{
							handlingResultsCount++;
						}
					}
					else
					{
						handlingResultsCount++;
					}
				}
				catch (Exception exception)
				{
					GeneralHandlingSupport.StoreResult(GeneralExceptionHandling.Handle(exception, HandlingMethodEnumeration.HandlingMethod.NoAction, owner));
					handlingResultsCount++;
				}
				finally
				{
					handlingResultsCount += LogHandlingResults(GeneralHandlingSupport.StoredHandlingResults);
					GeneralHandlingSupport.ClearStoredHandlingResults();
					stopwatch.Stop();
					base.Log.WriteSimple("Elapsed time: " + stopwatch.Elapsed.ToString("hh\\:mm\\:ss") + ".");
					if (!string.IsNullOrEmpty(summary))
					{
						base.Log.WriteSimple(summary ?? "");
					}
					if (handlingResultsCount > 0)
					{
						base.Log.Write("Import failed.");
					}
					else
					{
						base.Log.Write("Import finished successfully.");
					}
				}
			}
			else
			{
				base.Log.Write("Command is not enabled.");
			}
		}
		else
		{
			base.Log.Write("Command is not valid.");
		}
		return handlingResultsCount;
	}

	private void Initialization(Dataedo.App.Tools.CommandLine.Xml.Import importCommand, ref int handlingResultsCount)
	{
		if (importCommand != null && importCommand.RepositoryDocumentationId.HasValue)
		{
			importCommand.RepositoryDatabaseRow = new DatabaseRow(DB.Database.GetDataById(importCommand.RepositoryDocumentationId.Value));
			DatabaseRow repositoryDatabaseRow = importCommand.RepositoryDatabaseRow;
			if (repositoryDatabaseRow != null && repositoryDatabaseRow.Id.HasValue)
			{
				base.Log.Write("Update: " + importCommand.SourceDatabaseConnection.GetDatabaseFull() + " => " + importCommand.RepositoryConnection.GetDatabaseFull());
				return;
			}
			base.Log.Write("Unable to update documentation.", "The documentation to update (ID: " + importCommand?.RepositoryDocumentationId + ") not found in " + importCommand.RepositoryConnection.GetDatabaseFull() + ".", "Import failed.");
			handlingResultsCount++;
		}
		else
		{
			base.Log.Write("Unable to update documentation.", "The documentation to update (\"RepositoryDocumentationId\" element in command file) is not set.");
			handlingResultsCount++;
		}
	}

	public void ProcessImport(Dataedo.App.Tools.CommandLine.Xml.Import command, ref int handlingResultsCount, Form owner = null)
	{
		if (command?.RepositoryDatabaseRow == null)
		{
			if (command.RepositoryDocumentationId.HasValue)
			{
				command.RepositoryDatabaseRow = new DatabaseRow(DB.Database.GetDataById(command.RepositoryDocumentationId.Value));
			}
			else
			{
				command.RepositoryDatabaseRow = new DatabaseRow();
			}
		}
		if (!command.SourceDatabaseConnection.SetDataInDatabaseRow(command.RepositoryDatabaseRow))
		{
			return;
		}
		if (command.SourceDatabaseConnection.Timeout.HasValue)
		{
			CommandsWithTimeout.Timeout = command.SourceDatabaseConnection.Timeout.Value;
		}
		else
		{
			CommandsWithTimeout.SetNotToUseTimeout();
		}
		if (command.FilterRules == null)
		{
			command.FilterRules = new FilterRulesCollection();
		}
		if (command.FilterRules.Rules.Count == 0)
		{
			command.FilterRules.Rules.Add(new FilterRule(FilterRuleType.Include, FilterObjectTypeEnum.FilterObjectType.Any, null, null));
		}
		command.RepositoryDatabaseRow.Filter = command.FilterRules;
		Locker dbConnectionLocker = new Locker();
		ConnectionParameters connectionParameters = new ConnectionParameters
		{
			DatabaseRow = command.RepositoryDatabaseRow,
			DbConnectionLocker = dbConnectionLocker,
			FullImport = command.FullReimport,
			UpdateEntireDocumentation = command.UpdateEntireDocumentation,
			IsDbAdded = false
		};
		DocumentationCustomFieldRow[] array = DB.CustomField.GetDocumentationCustomFields(connectionParameters.DatabaseRow?.Id).ToArray();
		DocumentationCustomFieldRow[] array2 = array;
		foreach (DocumentationCustomFieldRow obj in array2)
		{
			obj.ExtendedProperty = null;
			obj.IsSelected = false;
		}
		if (DatabaseSupportFactory.GetDatabaseSupport(command.RepositoryDatabaseRow.Type).CanImportToCustomFields)
		{
			ExtendedPropertiesImportBase extendedPropertiesImport = command.ExtendedPropertiesImport;
			if (extendedPropertiesImport != null && extendedPropertiesImport.ExtendedProperties?.Count > 0)
			{
				foreach (ExtendedPropertyModel destinationCustomFields in command.ExtendedPropertiesImport.ExtendedProperties)
				{
					DocumentationCustomFieldRow documentationCustomFieldRow = array.FirstOrDefault((DocumentationCustomFieldRow x) => x.CustomFieldId == destinationCustomFields.DestinationCustomFieldId);
					if (documentationCustomFieldRow == null)
					{
						base.Log.Write($"Destination custom field (ID: {destinationCustomFields.DestinationCustomFieldId}) is not defined in repository.");
						handlingResultsCount++;
					}
					else
					{
						documentationCustomFieldRow.ExtendedProperty = destinationCustomFields.SourceExtendedProperty;
						documentationCustomFieldRow.IsSelected = destinationCustomFields.IsEnabled;
					}
				}
			}
		}
		else
		{
			base.Log.Write("Import from extended properties applies only to SQL Server, Azure and Tableau.");
		}
		SynchronizeParameters synchronizeParameters = new SynchronizeParameters
		{
			DatabaseRow = connectionParameters.DatabaseRow,
			DbSynchLocker = connectionParameters.DbConnectionLocker,
			FullImport = connectionParameters.FullImport,
			UpdateEntireDocumentation = connectionParameters.UpdateEntireDocumentation,
			CustomFields = array,
			IsDbAdded = connectionParameters.IsDbAdded,
			Log = base.Log
		};
		ConnectionResult connectionResult = connectionParameters.DatabaseRow.TryConnection();
		if (connectionResult.Exception != null)
		{
			base.Log.Write("Unable to synchronize from " + connectionParameters.DatabaseRow.Host + "@" + connectionParameters.DatabaseRow.Name);
			throw connectionResult.Exception;
		}
		connectionParameters.DatabaseRow.Connection = connectionResult.Connection;
		SynchronizeDatabase synchronizeModel = DatabaseSupportFactory.GetDatabaseSupport(connectionParameters.DatabaseRow.Type).GetSynchronizeModel(synchronizeParameters);
		try
		{
			StaticData.RefreshLicenseId();
			if (SetObjects(connectionParameters, synchronizeModel, owner))
			{
				int updateId = 0;
				if (!StaticData.IsProjectFile)
				{
					updateId = DB.SchemaImportsAndChanges.InsertSchemaUpdateRow(synchronizeParameters.DatabaseRow, SchemaUpdateTypeEnum.SchemaUpdateType.Update, owner);
				}
				Synchronize(command.RepositoryDatabaseRow, command.FullReimport, command.UpdateEntireDocumentation, array, updateId);
			}
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			connectionParameters.DatabaseRow.CloseConnection();
		}
	}

	private bool SetObjects(ConnectionParameters connectionParameters, SynchronizeDatabase synchronizeModel, Form owner = null)
	{
		BackgroundWorkerManager backgroundWorkerManager = new BackgroundWorkerManager(new BackgroundWorker
		{
			WorkerReportsProgress = true
		});
		List<SynchronizeDatabase.ImportQuery> list = synchronizeModel.CountObjectsQuery();
		bool flag = true;
		flag &= synchronizeModel.PreCmdImportOperations(null);
		synchronizeModel.ObjectsCounter = new ObjectsCounter();
		foreach (SynchronizeDatabase.ImportQuery item in list)
		{
			if (!synchronizeModel.CountObjects(item, backgroundWorkerManager, owner))
			{
				flag = false;
			}
		}
		if (flag)
		{
			connectionParameters.DatabaseRow.ConnectAndSynchronizeState = synchronizeModel.SetObjects(backgroundWorkerManager);
			int count = connectionParameters.DatabaseRow.tableRows.Count;
			int count2 = connectionParameters.DatabaseRow.tableRows.Where((ObjectRow x) => x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Synchronized).Count();
			int count3 = connectionParameters.DatabaseRow.tableRows.Where((ObjectRow x) => x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Unsynchronized).Count();
			int count4 = connectionParameters.DatabaseRow.tableRows.Where((ObjectRow x) => x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.New).Count();
			int count5 = connectionParameters.DatabaseRow.tableRows.Where((ObjectRow x) => x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted).Count();
			int count6 = connectionParameters.DatabaseRow.tableRows.Where((ObjectRow x) => x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Ignored).Count();
			string text = ObjectString(count3) + " unsynchronized";
			if (synchronizeModel.synchronizeParameters.FullImport)
			{
				text += " (full reimport)";
			}
			base.Log.Write(ObjectString(count) + " found: " + ObjectString(count2) + " synchronized, " + text + ", " + ObjectString(count4) + " new, " + ObjectString(count5) + " removed, " + ObjectString(count6) + " ignored.");
			return true;
		}
		connectionParameters.DatabaseRow.ConnectAndSynchronizeState = SynchConnectStateEnum.SynchConnectStateType.Error;
		base.Log.Write("Unable to synchronize from " + connectionParameters.DatabaseRow.Host + "@" + connectionParameters.DatabaseRow.Name);
		return false;
	}

	private void Synchronize(DatabaseRow databaseRow, bool fullImport, bool updateEntireDocumentation, DocumentationCustomFieldRow[] customFields, int updateId)
	{
		BackgroundWorkerManager backgroundWorkerManager = new BackgroundWorkerManager(new BackgroundWorker
		{
			WorkerReportsProgress = true
		});
		SynchDatabase synchDatabase = new SynchDatabase();
		bool flag = DatabaseSupportFactory.GetDatabaseSupport(databaseRow.Type)?.CanImportDependencies ?? false;
		if (synchDatabase.Synchronize(databaseRow, fullImport, updateEntireDocumentation, isDbAdded: false, customFields, updateId, backgroundWorkerManager, flag && Program.IncludeDependenciesOnInport) == true)
		{
			backgroundWorkerManager.BackgroundWorkerRunCompletedResetEvent.WaitOne();
			int count = databaseRow.tableRows.Count;
			int count2 = databaseRow.tableRows.Where((ObjectRow x) => (x.StateBeforeSynchronization == SynchronizeStateEnum.SynchronizeState.New || x.StateBeforeSynchronization == SynchronizeStateEnum.SynchronizeState.Ignored || x.StateBeforeSynchronization == SynchronizeStateEnum.SynchronizeState.Deleted) && x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Synchronized).Count();
			int count3 = databaseRow.tableRows.Where((ObjectRow x) => x.StateBeforeSynchronization == SynchronizeStateEnum.SynchronizeState.Unsynchronized && x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Synchronized).Count();
			int count4 = databaseRow.tableRows.Where((ObjectRow x) => x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted).Count();
			int count5 = databaseRow.tableRows.Where((ObjectRow x) => x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Ignored).Count();
			summary = ObjectString(count) + " found: " + ObjectString(count2) + " imported, " + ObjectString(count3) + " updated, " + ObjectString(count4) + " removed, " + ObjectString(count5) + " ignored.";
		}
		else
		{
			base.Log.Write("Import not required.");
		}
	}

	private string ObjectString(int count)
	{
		return count + ((count != 1) ? " objects" : " object");
	}
}
