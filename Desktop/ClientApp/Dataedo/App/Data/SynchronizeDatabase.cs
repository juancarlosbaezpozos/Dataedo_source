using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Cassandra;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.App.Data.General;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
using Dataedo.App.Enums;
using Dataedo.App.Helpers.Extensions;
using Dataedo.App.Licences;
using Dataedo.App.Synchronization.Tools;
using Dataedo.App.Synchronization_Tools;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.DataLineage;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.Classes;
using Dataedo.DBTConnector.Models;
using Dataedo.Log.Execution;
using Dataedo.Model.Data.DdlScript;
using Dataedo.Model.Data.Dependencies;
using Dataedo.Model.Data.DynamoDB;
using Dataedo.Model.Data.MongoDB;
using Dataedo.Model.Data.Neo4j;
using Dataedo.Model.Data.Object;
using Dataedo.Model.Data.Procedures.Parameters;
using Dataedo.Model.Data.Salesforce;
using Dataedo.Model.Data.Snowlfake;
using Dataedo.Model.Data.SSAS;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Model.Data.Tables.Columns.Tools;
using Dataedo.Model.Data.Tables.Constraints;
using Dataedo.Model.Data.Tables.Relations;
using Dataedo.Model.Data.Tables.Triggers;
using Dataedo.Model.Data.UniqueConstraints;
using Dataedo.Model.Enums;
using Dataedo.Model.Extensions;
using Dataedo.SalesforceConnector.SalesforceObjects;
using Dataedo.Shared.Enums;
using Dataedo.SSISConnector.DtsxParser;
using Google.Cloud.BigQuery.V2;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace Dataedo.App.Data;

public abstract class SynchronizeDatabase
{
	public class ImportQuery
	{
		public FilterObjectTypeEnum.FilterObjectType ObjectType { get; private set; }

		public string Query { get; private set; }

		public ImportQuery(FilterObjectTypeEnum.FilterObjectType objectType, string query)
		{
			ObjectType = objectType;
			Query = query;
		}
	}

	public SynchronizeParameters synchronizeParameters;

	protected DateTime ServerTime;

	protected List<ObjectIdType> objectsIds;

	protected string ErrorMessageCaption;

	protected CustomFieldsSupport customFieldsSupport;

	public SynchConnectStateEnum.SynchConnectStateType SynchState
	{
		get
		{
			if (!synchronizeParameters.DbSynchLocker.IsCanceled)
			{
				return SynchConnectStateEnum.SynchConnectStateType.Connected;
			}
			return SynchConnectStateEnum.SynchConnectStateType.Canceled;
		}
	}

	public ObservableCollection<RelationRow> RelationRows { get; set; }

	public ObservableCollection<DependencyRow> DependenciesRows { get; set; }

	public ObservableCollection<ColumnRow> ColumnRows { get; set; }

	public ObservableCollection<TriggerRow> TriggerRows { get; set; }

	public ObservableCollection<UniqueConstraintRow> UniqueConstraintRows { get; set; }

	public ObservableCollection<ParameterRow> ParameterRows { get; set; }

	public Restrictions RestrictionsForTables { get; set; }

	public Restrictions RestrictionsForProcedures { get; set; }

	public ObjectsCounter ObjectsCounter { get; set; }

	public abstract IEnumerable<string> CountTablesQuery();

	public abstract IEnumerable<string> CountViewsQuery();

	public abstract IEnumerable<string> CountProceduresQuery();

	public abstract IEnumerable<string> CountFunctionsQuery();

	public abstract IEnumerable<string> GetTablesQuery();

	public abstract IEnumerable<string> GetViewsQuery();

	public abstract IEnumerable<string> GetProceduresQuery();

	public abstract IEnumerable<string> GetFunctionsQuery();

	public abstract IEnumerable<string> RelationsQuery();

	public abstract IEnumerable<string> ColumnsQuery(bool getComputedFormula = true);

	public abstract IEnumerable<string> TriggersQuery();

	public abstract IEnumerable<string> UniqueConstraintsQuery();

	public abstract IEnumerable<string> ParametersQuery(bool canReadParameters = true);

	public abstract IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true);

	protected virtual string JoinObjectsNames(IEnumerable<ObjectRow> rows)
	{
		return RestrictionsForQueries.JoinObjectsNames(rows);
	}

	public virtual List<ImportQuery> CountObjectsQuery()
	{
		return GetQueries(CountQueryByType);
	}

	public IEnumerable<string> CountQueryByType(FilterObjectTypeEnum.FilterObjectType type)
	{
		return type switch
		{
			FilterObjectTypeEnum.FilterObjectType.Table => CountTablesQuery(), 
			FilterObjectTypeEnum.FilterObjectType.View => CountViewsQuery(), 
			FilterObjectTypeEnum.FilterObjectType.Procedure => CountProceduresQuery(), 
			FilterObjectTypeEnum.FilterObjectType.Function => CountFunctionsQuery(), 
			_ => new List<string> { string.Empty }, 
		};
	}

	public virtual List<ImportQuery> GetObjectsQuery()
	{
		return GetQueries(GetQueryByType);
	}

	public IEnumerable<string> GetQueryByType(FilterObjectTypeEnum.FilterObjectType type)
	{
		return type switch
		{
			FilterObjectTypeEnum.FilterObjectType.Table => GetTablesQuery(), 
			FilterObjectTypeEnum.FilterObjectType.View => GetViewsQuery(), 
			FilterObjectTypeEnum.FilterObjectType.Procedure => GetProceduresQuery(), 
			FilterObjectTypeEnum.FilterObjectType.Function => GetFunctionsQuery(), 
			_ => new List<string> { string.Empty }, 
		};
	}

	protected List<ImportQuery> GetQueries(Func<FilterObjectTypeEnum.FilterObjectType, IEnumerable<string>> queryAction)
	{
		List<ImportQuery> list = new List<ImportQuery>();
		foreach (FilterObjectTypeEnum.FilterObjectType includedType in synchronizeParameters.DatabaseRow.Filter.GetIncludedTypes(synchronizeParameters.DatabaseRow.Type))
		{
			foreach (string item in queryAction(includedType).Distinct())
			{
				list.Add(new ImportQuery(includedType, item));
			}
		}
		return list;
	}

	public string CombineQueries(List<string> queries)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < queries.Count; i++)
		{
			_ = queries[i];
			stringBuilder.AppendLine(queries[i]);
			if (i != queries.Count - 1)
			{
				stringBuilder.AppendLine();
			}
		}
		return stringBuilder.ToString();
	}

	public string CombineQueries(IEnumerable<string> queries)
	{
		return CombineQueries(queries.ToList());
	}

	public string CombineQueries(IEnumerable<ImportQuery> queries)
	{
		return CombineQueries(queries.Select((ImportQuery x) => x.Query).ToList());
	}

	private bool Like(string toSearch, string toFind)
	{
		return new Regex("\\A" + new Regex("\\.|\\$|\\^|\\{|\\[|\\(|\\||\\)|\\*|\\+|\\?|\\\\").Replace(toFind, (Match ch) => "\\" + ch).Replace('_', '.').Replace("%", ".*") + "\\z", RegexOptions.Singleline).IsMatch(toSearch);
	}

	protected bool IsObjectFiltered(string objectName, IEnumerable<FilterRule> filterRules)
	{
		bool result = false;
		foreach (FilterRule item in filterRules.Where((FilterRule rule) => rule.RuleType == FilterRuleType.Exclude))
		{
			if (Like(objectName.ToUpper(), item.PreparedName))
			{
				return true;
			}
		}
		foreach (FilterRule item2 in filterRules.Where((FilterRule rule) => rule.RuleType == FilterRuleType.Include))
		{
			result = true;
			if (Like(objectName.ToUpper(), item2.PreparedName))
			{
				return false;
			}
		}
		return result;
	}

	public SynchronizeDatabase(SynchronizeParameters synchronizeParameters)
	{
		customFieldsSupport = new CustomFieldsSupport();
		customFieldsSupport.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: false, synchronizeParameters.CustomFields.Select((DocumentationCustomFieldRow x) => x.CustomFieldId));
		this.synchronizeParameters = synchronizeParameters;
		objectsIds = new List<ObjectIdType>();
		RelationRows = new ObservableCollection<RelationRow>();
		DependenciesRows = new ObservableCollection<DependencyRow>();
		ColumnRows = new ObservableCollection<ColumnRow>();
		TriggerRows = new ObservableCollection<TriggerRow>();
		UniqueConstraintRows = new ObservableCollection<UniqueConstraintRow>();
		ParameterRows = new ObservableCollection<ParameterRow>();
		ErrorMessageCaption = "Error while synchronizing database " + this.synchronizeParameters.DatabaseRow.Name;
	}

	public virtual void SetBackgroundWorkerManager(BackgroundWorkerManager backgroundWorkerManager)
	{
	}

	public SynchConnectStateEnum.SynchConnectStateType SetObjects(BackgroundWorkerManager backgroundWorkerManager)
	{
		List<ImportQuery> objectsQuery = GetObjectsQuery();
		bool flag = true;
		ClearExistsFlag(backgroundWorkerManager, synchronizeParameters.IsDbAdded, synchronizeParameters.DatabaseRow.IdValue, synchronizeParameters?.Owner);
		QueryViewer.View("Get objects query", CombineQueries(objectsQuery));
		synchronizeParameters.DatabaseRow.tableRows = new ObservableCollection<ObjectRow>();
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		foreach (ImportQuery item in objectsQuery)
		{
			if (!GetObjects(item, backgroundWorkerManager, synchronizeParameters.Owner))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			AddDeletedObjects(synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager);
			synchronizeParameters.DatabaseRow.tableRows = new ObservableCollection<ObjectRow>(from i in synchronizeParameters.DatabaseRow.tableRows
				orderby i.SynchronizeState, i.Type, i.SubtypeDisplayString, i.Schema, i.Name
				select i);
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION SetObjects", null, null, 2, 1);
			return SynchState;
		}
		return SynchConnectStateEnum.SynchConnectStateType.Error;
	}

	public void Synchronize(BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		bool flag = SynchronizeObjects(synchronizeParameters.DatabaseRow.tableRows.Where((ObjectRow x) => x.IsNotSynchronized), backgroundWorkerManager, synchronizeParameters.ImportDependencies, owner);
		synchronizeParameters.DatabaseRow.ConnectAndSynchronizeState = (flag ? SynchState : SynchConnectStateEnum.SynchConnectStateType.Error);
	}

	private void SetSynchBeforeState(ObjectRow objectRow)
	{
		objectRow.SetSynchStateBeforeSynchronization();
		if (objectRow.SynchronizeState != SynchronizeStateEnum.SynchronizeState.Deleted)
		{
			objectRow.SynchronizeState = SynchronizeStateEnum.SynchronizeState.Synchronized;
		}
		objectsIds.Add(new ObjectIdType
		{
			BaseId = objectRow.ObjectId,
			Type = SharedObjectTypeEnum.TypeToStringShort(objectRow.Type)
		});
	}

	private bool SynchronizeObjects(IEnumerable<ObjectRow> rows, BackgroundWorkerManager backgroundWorkerManager, bool importDependencies, Form owner = null)
	{
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		double rowCount = 0.0;
		try
		{
			backgroundWorkerManager.ReportProgress("Importing");
			if (!GetServerTime(owner))
			{
				return false;
			}
			if (rows.Count() > 0 && !rows.First().DatabaseId.HasValue)
			{
				rows.ToList().ForEach(delegate(ObjectRow x)
				{
					x.DatabaseId = synchronizeParameters.DatabaseRow.Id;
				});
			}
			IEnumerable<ObjectRow> enumerable = rows.Where((ObjectRow x) => x.Type == SharedObjectTypeEnum.ObjectType.Table || x.Type == SharedObjectTypeEnum.ObjectType.View || x.Type == SharedObjectTypeEnum.ObjectType.Structure);
			SetRestrictionsForTables(enumerable);
			IEnumerable<ObjectRow> enumerable2 = rows.Where((ObjectRow x) => x.Type == SharedObjectTypeEnum.ObjectType.Procedure || x.Type == SharedObjectTypeEnum.ObjectType.Function || x.Subtype == SharedObjectSubtypeEnum.ObjectSubtype.Package);
			IEnumerable<ObjectRow> source = enumerable2.Where((ObjectRow x) => x.ToSynchronize);
			SetRestrictionsForProcedures(enumerable2);
			if (enumerable.Where((ObjectRow x) => x.ToSynchronize).Count() > 0)
			{
				GetObjectsDetails(backgroundWorkerManager, owner);
				if (!GetRelations(backgroundWorkerManager, owner))
				{
					return false;
				}
				if (!GetColumns(rows, backgroundWorkerManager, owner))
				{
					return false;
				}
				if (!GetTriggers(backgroundWorkerManager, owner))
				{
					return false;
				}
				if (!GetUniqueKeys(backgroundWorkerManager, owner))
				{
					return false;
				}
			}
			else
			{
				backgroundWorkerManager.IncrementProgress(4);
			}
			if (source.Count() > 0)
			{
				if (!GetParameters(backgroundWorkerManager, owner))
				{
					return false;
				}
			}
			else
			{
				backgroundWorkerManager.IncrementProgress(1);
			}
			objectsIds.Clear();
			backgroundWorkerManager.SetMessage("Updating ignored objects");
			if (!RemoveFromIgnored(rows, owner) || !AddToIgnored(rows, owner))
			{
				return false;
			}
			backgroundWorkerManager.IncrementProgress();
			IEnumerable<ObjectRow> enumerable3 = rows.Where((ObjectRow x) => x.ToSynchronize && (x.Type == SharedObjectTypeEnum.ObjectType.Table || x.Type == SharedObjectTypeEnum.ObjectType.View || x.Type == SharedObjectTypeEnum.ObjectType.Structure));
			int tableViewRowsCount = 0;
			int num = 0;
			if (ExecutionLog.IsLogEnabled)
			{
				tableViewRowsCount = enumerable3.Count();
			}
			foreach (ObjectRow item in enumerable3)
			{
				num++;
				if (!SynchronizeTable(item, backgroundWorkerManager, owner, num, tableViewRowsCount, ref rowCount))
				{
					return false;
				}
			}
			backgroundWorkerManager.SetMessage("Import relationships");
			if (!SynchronizeRelations(rows, owner))
			{
				return false;
			}
			backgroundWorkerManager.IncrementProgress();
			if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Oracle)
			{
				backgroundWorkerManager.SetMessage("Import columns constraints");
				DB.Column.UpdateColumnsConstraints(synchronizeParameters.DatabaseRow.Id.Value, owner);
			}
			backgroundWorkerManager.IncrementProgress();
			bool flag = false;
			Stopwatch stopwatch2 = ExecutionLog.StartStopwatch();
			try
			{
				flag = GetProcedureDefinitions(backgroundWorkerManager);
			}
			finally
			{
				ExecutionLog.WriteExecutionLog(stopwatch2, "DATAEDO.APP SYNCHRONIZATION GetProcedureDefinitions", null, null, 2, 1);
			}
			IDbConnection dbConnection = null;
			if (!flag)
			{
				PrepareQueriesForGettingProcedureDefinitions();
			}
			try
			{
				IEnumerable<ObjectRow> enumerable4 = rows.Where((ObjectRow x) => x.ToSynchronize && (x.Type == SharedObjectTypeEnum.ObjectType.Procedure || x.Type == SharedObjectTypeEnum.ObjectType.Function));
				int procedureFunctionRowsCount = 0;
				int num2 = 0;
				if (ExecutionLog.IsLogEnabled)
				{
					procedureFunctionRowsCount = enumerable4.Count();
				}
				foreach (ObjectRow item2 in enumerable4)
				{
					num2++;
					if (!SynchronizeProcedure(item2, backgroundWorkerManager, owner, flag, num2, procedureFunctionRowsCount, ref rowCount))
					{
						return false;
					}
				}
			}
			finally
			{
				dbConnection?.Close();
			}
			if (!GetDependencies(rows, importDependencies, backgroundWorkerManager, owner))
			{
				return false;
			}
			if (!SynchronizeDataLineage(backgroundWorkerManager, owner))
			{
				return false;
			}
			if (synchronizeParameters.UpdateId != 0)
			{
				DB.SchemaImportsAndChanges.UpdateObjectChanges(synchronizeParameters.UpdateId);
			}
			CreateAutomaticViewsDataLineage(backgroundWorkerManager);
			backgroundWorkerManager.SetMessage("Finalizing");
			if (DB.Database.SetSynchronized(objectsIds, ServerTime, owner))
			{
				backgroundWorkerManager.IncrementProgress();
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to synchronize objects", ErrorMessageCaption, owner);
			return false;
		}
		finally
		{
			string[] additionalData = new string[1] { "ROOT SYNCHRONIZATION METHOD" };
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION", null, null, 2, 1, additionalData);
		}
		return true;
	}

	private void SetRestrictionsForProcedures(IEnumerable<ObjectRow> procedureObjects)
	{
		IEnumerable<ObjectRow> enumerable = procedureObjects.Where((ObjectRow x) => x.ToSynchronize);
		if (enumerable.Count() >= procedureObjects.Count() / 2)
		{
			IEnumerable<ObjectRow> rows = procedureObjects.Where((ObjectRow x) => !x.ToSynchronize);
			RestrictionsForProcedures = new Restrictions
			{
				Type = RestrictionType.NotIn,
				Restriction = JoinObjectsNames(rows),
				DatabaseType = synchronizeParameters.DatabaseRow.Type
			};
		}
		else
		{
			RestrictionsForProcedures = new Restrictions
			{
				Type = RestrictionType.In,
				Restriction = JoinObjectsNames(enumerable),
				DatabaseType = synchronizeParameters.DatabaseRow.Type
			};
		}
	}

	private void SetRestrictionsForTables(IEnumerable<ObjectRow> tableObjects)
	{
		IEnumerable<ObjectRow> enumerable = tableObjects.Where((ObjectRow x) => x.ToSynchronize);
		if (enumerable.Count() >= tableObjects.Count() / 2)
		{
			IEnumerable<ObjectRow> rows = tableObjects.Where((ObjectRow x) => !x.ToSynchronize);
			RestrictionsForTables = new Restrictions
			{
				Type = RestrictionType.NotIn,
				Restriction = JoinObjectsNames(rows),
				DatabaseType = synchronizeParameters.DatabaseRow.Type
			};
		}
		else
		{
			RestrictionsForTables = new Restrictions
			{
				Type = RestrictionType.In,
				Restriction = JoinObjectsNames(enumerable),
				DatabaseType = synchronizeParameters.DatabaseRow.Type
			};
		}
	}

	private bool SynchronizeTable(ObjectRow tableView, BackgroundWorkerManager backgroundWorkerManager, Form owner, int tableViewRowsIndex, int tableViewRowsCount, ref double rowCount)
	{
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		backgroundWorkerManager.SetMessage(CommonFunctionsSynchronize.ReportProgressForObject(tableView, rowCount += 1.0, synchronizeParameters.SynchObjectsCount));
		int num = 0;
		if (tableView.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted)
		{
			tableView.SetId();
			num = ((!DB.Database.Delete(tableView.ObjectId, tableView.TypeAsString, ServerTime, synchronizeParameters.UpdateId, owner)) ? (-1) : 0);
			if (tableView.Type == SharedObjectTypeEnum.ObjectType.Table)
			{
				SetSynchBeforeState(tableView);
			}
		}
		else
		{
			num = ((!DB.Table.Synchronize(tableView, synchronizeParameters.DatabaseRow.Id.Value, synchronizeParameters.IsDbAdded, synchronizeParameters.UpdateId, customFieldsSupport, owner)) ? (-1) : 0);
			if (num == 0)
			{
				num = SynchronizeColumns(tableView.Name, tableView.Schema, tableView.ObjectId, tableView.TypeAsString, owner);
				if (num == 0 && (tableView.Type == SharedObjectTypeEnum.ObjectType.Table || tableView.Type == SharedObjectTypeEnum.ObjectType.View || tableView.Type == SharedObjectTypeEnum.ObjectType.Structure))
				{
					num += SynchronizeTriggers(tableView.Name, tableView.Schema, tableView.ObjectId, customFieldsSupport, owner);
					num += SynchronizeUniqueConstraints(tableView.Name, tableView.Schema, owner);
				}
			}
		}
		if (ExecutionLog.IsLogEnabled)
		{
			string[] additionalData = new string[3]
			{
				tableView.Name,
				tableViewRowsIndex.ToString(),
				tableViewRowsCount.ToString()
			};
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION Object", null, null, 2, 1, additionalData);
		}
		switch (num)
		{
		case 0:
			backgroundWorkerManager.IncrementProgress();
			if (tableView.Type == SharedObjectTypeEnum.ObjectType.View || tableView.Type == SharedObjectTypeEnum.ObjectType.Structure)
			{
				SetSynchBeforeState(tableView);
			}
			if (synchronizeParameters.DbSynchLocker.IsCanceled)
			{
				return false;
			}
			return true;
		case -2:
			GeneralMessageBoxesHandling.Show("Unable to synchronize table/view: " + tableView.Name + "." + Environment.NewLine + Environment.NewLine + "Table/view doesn't exist or it's name contains characters in other collation than repository collation.", ErrorMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			return false;
		default:
			GeneralMessageBoxesHandling.Show("Import " + tableView.Name + " failed.", ErrorMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			return false;
		}
	}

	private bool SynchronizeProcedure(ObjectRow procedureFunction, BackgroundWorkerManager backgroundWorkerManager, Form owner, bool areProcedureDefinitionsGet, int procedureFunctionRowsIndex, int procedureFunctionRowsCount, ref double rowCount)
	{
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		backgroundWorkerManager.SetMessage(CommonFunctionsSynchronize.ReportProgressForObject(procedureFunction, rowCount += 1.0, synchronizeParameters.SynchObjectsCount));
		int num = 0;
		if (procedureFunction.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted)
		{
			procedureFunction.SetId();
			num = ((!DB.Database.Delete(procedureFunction.ObjectId, procedureFunction.TypeAsString, ServerTime, synchronizeParameters.UpdateId, owner)) ? (-1) : 0);
		}
		else
		{
			if (!areProcedureDefinitionsGet)
			{
				Stopwatch stopwatch2 = ExecutionLog.StartStopwatch();
				try
				{
					GetDefinitionForProcedureObject(procedureFunction, backgroundWorkerManager);
				}
				finally
				{
					ExecutionLog.WriteExecutionLog(stopwatch2, "DATAEDO.APP SYNCHRONIZATION GetDefinitionForProcedureObject", null, null, 2, 1);
				}
			}
			num = ((!DB.Procedure.Synchronize(procedureFunction, synchronizeParameters.DatabaseRow.Id.Value, synchronizeParameters.IsDbAdded, synchronizeParameters.UpdateId, customFieldsSupport, owner)) ? (-1) : 0);
			procedureFunction.Definition = null;
			if (num == 0)
			{
				num = SynchronizeParameters(procedureFunction.Name, procedureFunction.Schema, procedureFunction.ObjectId, owner);
				switch (num)
				{
				case -1:
					GeneralMessageBoxesHandling.Show("Import parameters of " + procedureFunction.Name + " failed.", ErrorMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
					return false;
				case -2:
					GeneralMessageBoxesHandling.Show("Unable to synchronize parameters of procedure/function: " + procedureFunction.Name + "." + Environment.NewLine + Environment.NewLine + "Procedure/function doesn't exist or it's name contains characters in other collation than repository collation.", ErrorMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
					return false;
				}
			}
		}
		if (ExecutionLog.IsLogEnabled)
		{
			string[] additionalData = new string[3]
			{
				procedureFunction.Name,
				procedureFunctionRowsIndex.ToString(),
				procedureFunctionRowsCount.ToString()
			};
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION Object", null, null, 2, 1, additionalData);
		}
		if (num == 0)
		{
			backgroundWorkerManager.IncrementProgress();
			SetSynchBeforeState(procedureFunction);
			if (synchronizeParameters.DbSynchLocker.IsCanceled)
			{
				return false;
			}
			return true;
		}
		GeneralMessageBoxesHandling.Show("Import " + procedureFunction.Name + " failed.", ErrorMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		return false;
	}

	private bool GetDependencies(IEnumerable<ObjectRow> rows, bool importDependencies, BackgroundWorkerManager backgroundWorkerManager, Form owner)
	{
		bool canReadExternalDependencies = !DatabaseSupportFactory.GetDatabaseSupport(synchronizeParameters.DatabaseRow.Type).SupportsReadingExternalDependencies || CheckIfHasSelectPermissionForObject("sys.sql_expression_dependencies");
		backgroundWorkerManager.SetMessage("Retrieving dependencies");
		IEnumerable<string> enumerable = DependenciesQuery(canReadExternalDependencies);
		QueryViewer.View("Get dependencies query", CombineQueries(enumerable));
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		bool flag = true;
		if (importDependencies)
		{
			DependenciesRows = new ObservableCollection<DependencyRow>();
			foreach (string item in enumerable)
			{
				if (!GetDependencies(item, owner))
				{
					flag = false;
					break;
				}
			}
		}
		ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION GetDependencies", null, null, 2, 1);
		if (!flag)
		{
			return false;
		}
		Stopwatch stopwatch2 = ExecutionLog.StartStopwatch();
		if (!SynchronizeDependencies(rows, canReadExternalDependencies, owner))
		{
			return false;
		}
		backgroundWorkerManager.IncrementProgress();
		ExecutionLog.WriteExecutionLog(stopwatch2, "DATAEDO.APP SYNCHRONIZATION SynchronizeDependencies", null, null, 2, 1);
		return true;
	}

	private bool GetParameters(BackgroundWorkerManager backgroundWorkerManager, Form owner)
	{
		backgroundWorkerManager.ReportProgress("Retrieving parameters");
		IEnumerable<string> enumerable = ParametersQuery();
		QueryViewer.View("Get parameters query", CombineQueries(enumerable));
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		bool result = true;
		ParameterRows = new ObservableCollection<ParameterRow>();
		foreach (string item in enumerable)
		{
			if (!GetParameters(item, synchronizeParameters.DatabaseRow.SynchronizeParameters, owner))
			{
				result = false;
				break;
			}
		}
		ParameterRows.Where((ParameterRow x) => x.ParameterMode.Equals("OUT") && string.IsNullOrEmpty(x.Name)).ToList().ForEach(delegate(ParameterRow x)
		{
			x.Name = "Returns";
		});
		ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION GetParameters", null, null, 2, 1);
		return result;
	}

	private bool GetUniqueKeys(BackgroundWorkerManager backgroundWorkerManager, Form owner)
	{
		backgroundWorkerManager.ReportProgress("Retrieving unique keys");
		IEnumerable<string> enumerable = UniqueConstraintsQuery();
		QueryViewer.View("Get unique constraints query", CombineQueries(enumerable));
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		bool result = true;
		UniqueConstraintRows = new ObservableCollection<UniqueConstraintRow>();
		foreach (string item in enumerable)
		{
			if (!GetUniqueConstraints(item, owner))
			{
				result = false;
				break;
			}
		}
		if (synchronizeParameters.DatabaseRow.Type.HasValue && DatabaseTypeEnum.UsesOracleLibraries(synchronizeParameters.DatabaseRow.Type.Value))
		{
			foreach (UniqueConstraintRow item2 in UniqueConstraintRows.Where((UniqueConstraintRow x) => x.Columns.Count > 1))
			{
				IGrouping<string, UniqueConstraintColumnRow>[] array = (from x in item2.Columns
					group x by x.ColumnName).ToArray();
				item2.Columns.Clear();
				IGrouping<string, UniqueConstraintColumnRow>[] array2 = array;
				foreach (IGrouping<string, UniqueConstraintColumnRow> source in array2)
				{
					if (source.Count() > 1)
					{
						UniqueConstraintColumnRow uniqueConstraintColumnRow = source.FirstOrDefault((UniqueConstraintColumnRow x) => !x.IsDisabled);
						if (uniqueConstraintColumnRow != null)
						{
							item2.Columns.Add(uniqueConstraintColumnRow);
						}
						else
						{
							item2.Columns.Add(source.First());
						}
					}
					else
					{
						item2.Columns.Add(source.First());
					}
				}
			}
		}
		ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION GetUniqueConstraints", null, null, 2, 1);
		return result;
	}

	private bool GetTriggers(BackgroundWorkerManager backgroundWorkerManager, Form owner)
	{
		backgroundWorkerManager.ReportProgress("Retrieving triggers");
		IEnumerable<string> enumerable = TriggersQuery();
		QueryViewer.View("Get triggers query", CombineQueries(enumerable));
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		bool result = true;
		TriggerRows = new ObservableCollection<TriggerRow>();
		foreach (string item in enumerable)
		{
			if (!GetTriggers(item, owner))
			{
				result = false;
				break;
			}
		}
		ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION GetTriggers", null, null, 2, 1);
		return result;
	}

	private bool GetColumns(IEnumerable<ObjectRow> rows, BackgroundWorkerManager backgroundWorkerManager, Form owner)
	{
		backgroundWorkerManager.ReportProgress("Retrieving columns");
		IEnumerable<string> enumerable = ColumnsQuery(synchronizeParameters.DatabaseRow.SynchronizeColumnsComputedFormula);
		QueryViewer.View("Get columns query", CombineQueries(enumerable));
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		bool result = true;
		ColumnRows = new ObservableCollection<ColumnRow>();
		foreach (string item4 in enumerable)
		{
			if (!GetColumns(item4, owner))
			{
				result = false;
				break;
			}
		}
		SharedDatabaseTypeEnum.DatabaseType? type = synchronizeParameters.DatabaseRow.Type;
		if (type == SharedDatabaseTypeEnum.DatabaseType.Cassandra || type == SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra)
		{
			foreach (ObjectRow table in rows.Where((ObjectRow x) => x.Type == SharedObjectTypeEnum.ObjectType.Table || x.Type == SharedObjectTypeEnum.ObjectType.View))
			{
				List<ColumnRow> source = ColumnRows.Where((ColumnRow x) => x.TableName == table.Name && x.TableSchema == table.Schema && x.DatabaseName == table.DatabaseName).ToList();
				List<IOrderedEnumerable<ColumnRow>> list = new List<IOrderedEnumerable<ColumnRow>>();
				IOrderedEnumerable<ColumnRow> item = from x in source.Where((ColumnRow x) => x.ConstraintType == "partition_key").ToList()
					orderby x.Position
					select x;
				IOrderedEnumerable<ColumnRow> item2 = from x in source.Where((ColumnRow x) => x.ConstraintType == "clustering").ToList()
					orderby x.Position
					select x;
				IOrderedEnumerable<ColumnRow> item3 = from x in source.Where((ColumnRow x) => x.ConstraintType == "regular").ToList()
					orderby x.Name
					select x;
				list.Add(item);
				list.Add(item2);
				list.Add(item3);
				int num = 1;
				foreach (IOrderedEnumerable<ColumnRow> item5 in list)
				{
					foreach (ColumnRow item6 in item5)
					{
						item6.Position = num;
						num++;
					}
				}
			}
		}
		ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION GetColumns", null, null, 2, 1);
		return result;
	}

	private bool GetRelations(BackgroundWorkerManager backgroundWorkerManager, Form owner)
	{
		backgroundWorkerManager.ReportProgress("Retrieving relationships");
		IEnumerable<string> enumerable = RelationsQuery();
		QueryViewer.View("Get relationships query", CombineQueries(enumerable));
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		bool result = true;
		RelationRows = new ObservableCollection<RelationRow>();
		foreach (string item in enumerable)
		{
			if (!GetRelations(item, owner))
			{
				result = false;
				break;
			}
		}
		ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION GetRelations", null, null, 2, 1);
		return result;
	}

	private void CreateAutomaticViewsDataLineage(BackgroundWorkerManager backgroundWorkerManager)
	{
		if (Dataedo.App.StaticData.IsProjectFile)
		{
			return;
		}
		SynchronizeParameters obj = synchronizeParameters;
		if (obj != null)
		{
			DatabaseRow databaseRow = obj.DatabaseRow;
			if (databaseRow != null && databaseRow.Id.HasValue)
			{
				backgroundWorkerManager?.SetMessage("Creating automatic data lineage for views");
				DB.DataProcess.CreateAutomaticViewsDataLineage(synchronizeParameters.DatabaseRow.Id.Value);
			}
		}
	}

	public virtual bool PreCmdImportOperations(Form owner)
	{
		return true;
	}

	private bool CheckIfHasSelectPermissionForObject(string objectName)
	{
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		try
		{
			using SqlCommand sqlCommand = CommandsWithTimeout.SqlServer("SELECT HAS_PERMS_BY_NAME (@object_name, 'OBJECT', 'SELECT')", synchronizeParameters.DatabaseRow.Connection);
			sqlCommand.Parameters.AddWithValue("object_name", objectName);
			if (!int.TryParse(sqlCommand.ExecuteScalar()?.ToString(), out var result))
			{
				return false;
			}
			return result == 1;
		}
		finally
		{
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION", null, null, 2, 1);
		}
	}

	public bool RemoveFromIgnored(IEnumerable<ObjectRow> rows, Form owner = null)
	{
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		try
		{
			List<IgnoredObject> objects = ConvertingToTables.ReloadIgnoredObjectsTable(synchronizeParameters.DatabaseRow.Id.Value, rows.Where((ObjectRow x) => x.ToSynchronize && x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Ignored));
			return DB.IgnoredObjects.Delete(objects, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to remove object from ignored", ErrorMessageCaption, owner);
		}
		finally
		{
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION", null, null, 2, 1);
		}
		return false;
	}

	public bool AddToIgnored(IEnumerable<ObjectRow> rows, Form owner = null)
	{
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		try
		{
			List<IgnoredObject> objects = ConvertingToTables.ReloadIgnoredObjectsTable(synchronizeParameters.DatabaseRow.Id.Value, rows.Where((ObjectRow x) => !x.ToSynchronize && x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.New));
			return DB.IgnoredObjects.Insert(objects, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to add object to ignored", ErrorMessageCaption, owner);
		}
		finally
		{
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION", null, null, 2, 1);
		}
		return false;
	}

	private int SynchronizeColumns(string tableName, string schema, int tableId, string type, Form owner = null)
	{
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		try
		{
			IEnumerable<ColumnRow> enumerable = new List<ColumnRow>();
			SharedDatabaseTypeEnum.DatabaseType? databaseType = synchronizeParameters?.DatabaseRow?.Type;
			enumerable = ((!databaseType.HasValue || databaseType.GetValueOrDefault() != SharedDatabaseTypeEnum.DatabaseType.Oracle) ? ColumnRows.Where((ColumnRow x) => x.TableName.Equals(tableName) && x.TableSchema.Equals(schema)) : ColumnRows.Where((ColumnRow x) => x.TableName.Equals(tableName) && x.TableSchema.Equals(schema) && (x.TableType?.Equals(type) ?? false)));
			string type2 = type;
			return DB.Column.Synchronize(enumerable, tableName, schema, tableId, type2, synchronizeParameters.DatabaseRow.Id.Value, synchronizeParameters.IsDbAdded, synchronizeParameters.UpdateId, customFieldsSupport, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to synchronize column", ErrorMessageCaption, owner);
		}
		finally
		{
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION", null, null, 2, 1);
		}
		return -1;
	}

	public int SynchronizeTriggers(string tableName, string schema, int tableId, CustomFieldsSupport customFieldsSupport, Form owner = null)
	{
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		try
		{
			return DB.Trigger.Synchronize(TriggerRows.Where((TriggerRow x) => x.TableName.Equals(tableName) && x.TableSchema.Equals(schema)), tableName, schema, tableId, synchronizeParameters.DatabaseRow.Id.Value, synchronizeParameters.IsDbAdded, synchronizeParameters.UpdateId, customFieldsSupport, owner);
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
		}
		finally
		{
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION", null, null, 2, 1);
		}
		return -1;
	}

	public int SynchronizeUniqueConstraints(string tableName, string schema, Form owner = null)
	{
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		try
		{
			IEnumerable<UniqueConstraintRow> enumerable = UniqueConstraintRows.Where((UniqueConstraintRow x) => x.TableName.Equals(tableName) && x.TableSchema.Equals(schema));
			if (DB.Constraint.Synchronize(enumerable, tableName, schema, synchronizeParameters.DatabaseRow.Id.Value, synchronizeParameters.IsDbAdded, synchronizeParameters.UpdateId, owner) == 0)
			{
				return DB.ConstraintColumn.Synchronize(enumerable, tableName, schema, synchronizeParameters.DatabaseRow.Id.Value, synchronizeParameters.IsDbAdded, synchronizeParameters.UpdateId, owner);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to synchronize unique constraints", ErrorMessageCaption, owner);
		}
		finally
		{
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION", null, null, 2, 1);
		}
		return -1;
	}

	public int SynchronizeParameters(string procedureName, string schema, int procedureId, Form owner = null)
	{
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		try
		{
			if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Teradata)
			{
				IEnumerable<ParameterRow> parameters = ParameterRows.Where((ParameterRow x) => x.ProcedureName.Equals(procedureName, StringComparison.OrdinalIgnoreCase) && x.ProcedureSchema.Equals(schema));
				return DB.Parameter.Synchronize(parameters, procedureName, procedureId, schema, synchronizeParameters.DatabaseRow.Id.Value, synchronizeParameters.IsDbAdded, synchronizeParameters.UpdateId, customFieldsSupport, owner);
			}
			IEnumerable<ParameterRow> parameters2 = ParameterRows.Where((ParameterRow x) => x.ProcedureName.Equals(procedureName) && x.ProcedureSchema.Equals(schema));
			return DB.Parameter.Synchronize(parameters2, procedureName, procedureId, schema, synchronizeParameters.DatabaseRow.Id.Value, synchronizeParameters.IsDbAdded, synchronizeParameters.UpdateId, customFieldsSupport, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to synchronize parameters", ErrorMessageCaption, owner);
		}
		finally
		{
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION", null, null, 2, 1);
		}
		return -1;
	}

	public bool SynchronizeRelations(IEnumerable<ObjectRow> rows, Form owner = null)
	{
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		bool result = false;
		try
		{
			int[] succeedTablesIds = null;
			int[] succeedTablesIds2 = null;
			if (DB.Relation.Synchronize(RelationRows, rows.Where((ObjectRow x) => x.ToSynchronize && x.Type == SharedObjectTypeEnum.ObjectType.Table), synchronizeParameters.DatabaseRow, synchronizeParameters.IsDbAdded, synchronizeParameters.UpdateId, out succeedTablesIds, owner))
			{
				result = DB.RelationColumns.Synchronize(RelationRows, synchronizeParameters.DatabaseRow, synchronizeParameters.IsDbAdded, synchronizeParameters.UpdateId, out succeedTablesIds2, owner);
			}
			int numberOfColumns;
			foreach (ObjectRow table in rows.Where((ObjectRow x) => x.ToSynchronize && x.Type == SharedObjectTypeEnum.ObjectType.Table && x.SynchronizeState != SynchronizeStateEnum.SynchronizeState.Deleted).ToList())
			{
				Stopwatch stopwatch2 = ExecutionLog.StartStopwatch();
				try
				{
					numberOfColumns = 0;
					IEnumerable<RelationRow> source = RelationRows.Where((RelationRow x) => x.FKTableSchema.Equals(table.Schema) && x.FKTableName.Equals(table.Name));
					int num = source.Count();
					source.ToList().ForEach(delegate(RelationRow relation)
					{
						numberOfColumns += relation.Columns.Count();
					});
					source = RelationRows.Where((RelationRow x) => x.PKTableSchema.Equals(table.Schema) && x.PKTableName.Equals(table.Name));
					int num2 = num + source.Count();
					source.ToList().ForEach(delegate(RelationRow relation)
					{
						numberOfColumns += relation.Columns.Count();
					});
					int num3 = succeedTablesIds.Where((int x) => x == table.ObjectId).Count();
					int num4 = succeedTablesIds2.Where((int x) => x == table.ObjectId).Count();
					if (num2 == num3 && numberOfColumns == num4)
					{
						SetSynchBeforeState(table);
						continue;
					}
					GeneralMessageBoxesHandling.Show("Importing relationships for the table " + table.Name + " failed.", ErrorMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
					result = false;
					return result;
				}
				finally
				{
					ExecutionLog.WriteExecutionLog(stopwatch2, "DATAEDO.APP SYNCHRONIZATION Object for relationships", null, null, 2, 1);
				}
			}
			return result;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to synchronize relationships", ErrorMessageCaption, owner);
			return false;
		}
		finally
		{
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION", null, null, 2, 1);
		}
	}

	private bool SynchronizeDependencies(IEnumerable<ObjectRow> rows, bool canReadExternalDependencies = true, Form owner = null)
	{
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		bool flag = false;
		try
		{
			flag = DB.Dependency.Synchronize(DependenciesRows.ToList(), synchronizeParameters.DatabaseRow.Host, synchronizeParameters.DatabaseRow.Names, canReadExternalDependencies, synchronizeParameters.IsDbAdded, out var succeedTablesIds, owner);
			foreach (ObjectRow objectForSynchronizing in rows.Where((ObjectRow x) => x.ToSynchronize && x.SynchronizeState != SynchronizeStateEnum.SynchronizeState.Deleted).ToList())
			{
				Stopwatch stopwatch2 = ExecutionLog.StartStopwatch();
				try
				{
					int num = DependenciesRows.Where((DependencyRow x) => x.ReferencingName.Equals(objectForSynchronizing.Name) || x.ReferencedName.Equals(objectForSynchronizing.Name)).Count();
					int num2 = succeedTablesIds.Where((string x) => PrepareValue.ToInt(x) == objectForSynchronizing.ObjectId).Count();
					if (num == num2)
					{
						SetSynchBeforeState(objectForSynchronizing);
					}
				}
				finally
				{
					ExecutionLog.WriteExecutionLog(stopwatch2, "DATAEDO.APP SYNCHRONIZATION Object for dependencies", null, null, 2, 1);
				}
			}
			return flag;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to synchronize dependencies", ErrorMessageCaption, owner);
			return false;
		}
		finally
		{
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION", null, null, 2, 1);
		}
	}

	public bool SynchronizeDataLineage(BackgroundWorkerManager backgroundWorkerManager, Form owner)
	{
		backgroundWorkerManager.SetMessage("Retrieving data lineage");
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		try
		{
			List<DataProcessRow> dataLineage = GetDataLineage();
			if (dataLineage == null)
			{
				return true;
			}
			int num = 0;
			foreach (DataProcessRow item in dataLineage)
			{
				num++;
				backgroundWorkerManager.SetMessage($"Retrieving data process {num} of {dataLineage.Count}");
				int? num2 = FindParentIdForDataProcess(item);
				if (!num2.HasValue)
				{
					continue;
				}
				item.ParentId = num2.Value;
				item.DatabaseId = synchronizeParameters.DatabaseRow.Id.Value;
				if (!DB.DataProcess.Synchronize(item, owner).HasValue)
				{
					continue;
				}
				DataFlowHelper.SetProcessIdInProcessFlows(item);
				foreach (DataFlowRow item2 in item.InflowRows.Where((DataFlowRow x) => x.RowState == ManagingRowsEnum.ManagingRows.ForAdding).Concat(item.OutflowRows.Where((DataFlowRow x) => x.RowState == ManagingRowsEnum.ManagingRows.ForAdding)))
				{
					if (item2.ObjectId > 0)
					{
						if (!DataFlowHelper.SynchronizeDataFlow(item2, owner))
						{
							return false;
						}
						continue;
					}
					int? num3 = FindParentIdForDataFlow(item2);
					if (num3.HasValue)
					{
						item2.ObjectId = num3.Value;
						if (!DataFlowHelper.SynchronizeDataFlow(item2, owner))
						{
							return false;
						}
					}
				}
			}
			backgroundWorkerManager.IncrementProgress();
		}
		finally
		{
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION SynchronizeDataLineage", null, null, 2, 1);
		}
		return true;
	}

	private int? FindParentIdForDataProcess(DataProcessRow dataProcess)
	{
		ObjectRow objectRow = synchronizeParameters.DatabaseRow.tableRows.Where((ObjectRow x) => x.Schema == dataProcess.ParentSchema && x.Name == dataProcess.ParentName && x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Synchronized).FirstOrDefault();
		if (objectRow == null)
		{
			return null;
		}
		if (objectRow.ObjectId > 0)
		{
			return objectRow.ObjectId;
		}
		return DB.Table.GetObjectIdByName(synchronizeParameters.DatabaseRow.Id.Value, dataProcess.ParentSchema, dataProcess.ParentName, objectRow.TypeAsString);
	}

	private int? FindParentIdForDataFlow(DataFlowRow dataFlow)
	{
		ObjectRow objectRow = synchronizeParameters.DatabaseRow.tableRows.Where((ObjectRow x) => x.Schema == dataFlow.Schema && x.Name == dataFlow.Name && x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Synchronized).FirstOrDefault();
		if (objectRow == null)
		{
			return null;
		}
		if (objectRow.ObjectId > 0)
		{
			return objectRow.ObjectId;
		}
		return DB.Table.GetObjectIdByName(synchronizeParameters.DatabaseRow.Id.Value, objectRow.Schema, objectRow.Name, objectRow.TypeAsString);
	}

	public virtual IDbConnection GetConnection()
	{
		return null;
	}

	public abstract bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null);

	public abstract bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null);

	public virtual bool GetProcedureDefinitions(BackgroundWorkerManager backgroundWorkerManager)
	{
		return true;
	}

	internal virtual List<ObjectRow> GetProceduresAndFunctionsForSync()
	{
		return (from t in synchronizeParameters.DatabaseRow.tableRows
			where t.ToSynchronize
			select t into x
			where x.Type == SharedObjectTypeEnum.ObjectType.Procedure || x.Type == SharedObjectTypeEnum.ObjectType.Function
			select x).ToList();
	}

	public virtual void PrepareQueriesForGettingProcedureDefinitions()
	{
	}

	public virtual void GetDefinitionForProcedureObject(ObjectRow row, BackgroundWorkerManager backgroundWorkerManager)
	{
	}

	public virtual void GetObjectsDetails(BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
	}

	public abstract bool GetRelations(string query, Form owner = null);

	public abstract bool GetColumns(string query, Form owner = null);

	public abstract bool GetTriggers(string query, Form owner = null);

	public abstract bool GetUniqueConstraints(string query, Form owner = null);

	public abstract bool GetParameters(string query, bool canReadParameters = true, Form owner = null);

	public abstract bool GetDependencies(string query, Form owner = null);

	public virtual List<DataProcessRow> GetDataLineage()
	{
		return null;
	}

	public bool GetServerTime(Form owner = null)
	{
		DateTime? dateTime = ((synchronizeParameters.DatabaseRow.Type != SharedDatabaseTypeEnum.DatabaseType.GoogleBigQuery) ? GeneralQueries.GetServerTime(synchronizeParameters.DatabaseRow.Connection, synchronizeParameters.DatabaseRow.Type, owner) : new GoogleBigQuerySupport().GetServerTime(synchronizeParameters.DatabaseRow.Connection, synchronizeParameters.DatabaseRow.Host));
		if (dateTime.HasValue)
		{
			ServerTime = dateTime.Value;
		}
		return dateTime.HasValue;
	}

	public void AddDBObject(ObjectRow row, ObservableCollection<ObjectRow> rows, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress(new List<string> { "Checking objects' status", row.NameWithType });
		if (synchronizeParameters.DatabaseRow.Id.HasValue)
		{
			switch (DB.Database.CheckSynchronization(row.Name, row.Schema, row.TypeAsString, synchronizeParameters.DatabaseRow.Id.Value, row.DbmsLastModificationDate, owner))
			{
			case "I":
				row.SynchronizeState = SynchronizeStateEnum.SynchronizeState.Ignored;
				break;
			case "N":
				row.SynchronizeState = SynchronizeStateEnum.SynchronizeState.New;
				break;
			case "S":
				row.SynchronizeState = (synchronizeParameters.FullImport ? SynchronizeStateEnum.SynchronizeState.Unsynchronized : SynchronizeStateEnum.SynchronizeState.Synchronized);
				row.FullImport = true;
				break;
			case "U":
				row.SynchronizeState = SynchronizeStateEnum.SynchronizeState.Unsynchronized;
				break;
			}
		}
		else
		{
			row.SynchronizeState = SynchronizeStateEnum.SynchronizeState.New;
		}
		rows.Add(row);
	}

	public void AddDBObject(object dr, ObservableCollection<ObjectRow> rows, BackgroundWorkerManager backgroundWorkerManager, string script = null, DateTime? modifyDate = null, Form owner = null)
	{
		bool hasImportUsingCustomFields = DatabaseSupportFactory.GetDatabaseSupport(synchronizeParameters.DatabaseRow.Type).HasImportUsingCustomFields;
		ObjectSynchronizationObject objectSynchronizationObject2;
		switch (synchronizeParameters.DatabaseRow.Type)
		{
		case SharedDatabaseTypeEnum.DatabaseType.Oracle:
			objectSynchronizationObject2 = new ObjectSynchronizationForOracleObject(dr as DbDataReader, synchronizeParameters.DatabaseRow.Name);
			break;
		case SharedDatabaseTypeEnum.DatabaseType.Teradata:
		{
			DbDataReader data8 = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds10;
			if (!hasImportUsingCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds10 = enumerable;
			}
			else
			{
				fieldsToLoadIds10 = customFieldsSupport.GetFieldsIds();
			}
			objectSynchronizationObject2 = new ObjectSynchronizationObject(data8, fieldsToLoadIds10, script);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.MongoDB:
		case SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB:
			objectSynchronizationObject2 = new ObjectSynchronizationForMongoDBObject(dr as BsonTable, synchronizeParameters.DatabaseRow.Name);
			break;
		case SharedDatabaseTypeEnum.DatabaseType.Cassandra:
		case SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra:
		case SharedDatabaseTypeEnum.DatabaseType.AmazonKeyspaces:
		case SharedDatabaseTypeEnum.DatabaseType.Astra:
		{
			Row data3 = dr as Row;
			IEnumerable<int> fieldsToLoadIds3;
			if (!hasImportUsingCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds3 = enumerable;
			}
			else
			{
				fieldsToLoadIds3 = customFieldsSupport.GetFieldsIds();
			}
			objectSynchronizationObject2 = new ObjectSynchronizationObject(data3, fieldsToLoadIds3, synchronizeParameters.DatabaseRow.Type);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.GoogleBigQuery:
		{
			BigQueryRow table2 = dr as BigQueryRow;
			IEnumerable<int> fieldsToLoadIds9;
			if (!hasImportUsingCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds9 = enumerable;
			}
			else
			{
				fieldsToLoadIds9 = customFieldsSupport.GetFieldsIds();
			}
			objectSynchronizationObject2 = new ObjectSynchronizationObject(table2, fieldsToLoadIds9, modifyDate, hasImportUsingCustomFields ? synchronizeParameters.CustomFields.ToDictionary((DocumentationCustomFieldRow x) => x.OrdinalPosition, (DocumentationCustomFieldRow x) => x.ExtendedPropertyForQueries) : new Dictionary<int, string>());
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.Elasticsearch:
			objectSynchronizationObject2 = new ObjectSynchronizationForElasticsearchObject(dr as string, synchronizeParameters.DatabaseRow.Name);
			break;
		case SharedDatabaseTypeEnum.DatabaseType.Neo4j:
			objectSynchronizationObject2 = new ObjectSynchronizationForNeo4jObject(dr as Neo4jObject, synchronizeParameters.DatabaseRow.Name);
			break;
		case SharedDatabaseTypeEnum.DatabaseType.SsasTabular:
		case SharedDatabaseTypeEnum.DatabaseType.PowerBiDataset:
		{
			NamedMetadataObject data6 = dr as NamedMetadataObject;
			IEnumerable<int> fieldsToLoadIds7;
			if (!hasImportUsingCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds7 = enumerable;
			}
			else
			{
				fieldsToLoadIds7 = customFieldsSupport.GetFieldsIds();
			}
			objectSynchronizationObject2 = new ObjectSynchronizationForSsasTabularObject(data6, fieldsToLoadIds7, synchronizeParameters.DatabaseRow.Type);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.Tableau:
		{
			JObject data4 = dr as JObject;
			IEnumerable<int> fieldsToLoadIds4;
			if (!hasImportUsingCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds4 = enumerable;
			}
			else
			{
				fieldsToLoadIds4 = customFieldsSupport.GetFieldsIds();
			}
			objectSynchronizationObject2 = new ObjectSynchronizationForTableauObject(data4, fieldsToLoadIds4, synchronizeParameters.DatabaseRow.Name);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.CosmosDbSQL:
			objectSynchronizationObject2 = ((!(dr is ObjectSynchronizationForCosmosDBObject objectSynchronizationForCosmosDBObject)) ? new ObjectSynchronizationForCosmosDBObject(dr as BsonTable, synchronizeParameters.DatabaseRow.Name) : objectSynchronizationForCosmosDBObject);
			break;
		case SharedDatabaseTypeEnum.DatabaseType.Salesforce:
		{
			SObject data9 = dr as SObject;
			string name = synchronizeParameters.DatabaseRow.Name;
			IEnumerable<int> fieldsToLoadIds12;
			if (!hasImportUsingCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds12 = enumerable;
			}
			else
			{
				fieldsToLoadIds12 = customFieldsSupport.GetFieldsIds();
			}
			objectSynchronizationObject2 = new ObjectSynchronizationForSalesforceObject(data9, name, fieldsToLoadIds12);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.Athena:
			objectSynchronizationObject2 = new ObjectSynchronizationForAthenaObject(dr as Dictionary<string, string>, synchronizeParameters.DatabaseRow.Name);
			break;
		case SharedDatabaseTypeEnum.DatabaseType.InterfaceTables:
		{
			DbDataReader data5 = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds6;
			if (!hasImportUsingCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds6 = enumerable;
			}
			else
			{
				fieldsToLoadIds6 = customFieldsSupport.GetFieldsIds();
			}
			objectSynchronizationObject2 = new ObjectSynchronizationForInterfaceTableObject(data5, fieldsToLoadIds6, synchronizeParameters.DatabaseRow.Type);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.Dataverse:
		case SharedDatabaseTypeEnum.DatabaseType.Dynamics365:
			objectSynchronizationObject2 = new ObjectSynchronizationForDataverseObject(dr as DataverseObjectWrapper, synchronizeParameters.DatabaseRow.Name);
			break;
		case SharedDatabaseTypeEnum.DatabaseType.DynamoDB:
		{
			DynamoDBTable table = dr as DynamoDBTable;
			IEnumerable<int> fieldsToLoadIds5;
			if (!hasImportUsingCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds5 = enumerable;
			}
			else
			{
				fieldsToLoadIds5 = customFieldsSupport.GetFieldsIds();
			}
			objectSynchronizationObject2 = new ObjectSynchronizationForDynamoDBObject(table, fieldsToLoadIds5, synchronizeParameters.DatabaseRow.Type);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.NetSuite:
		{
			DbDataReader data10 = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds13;
			if (!hasImportUsingCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds13 = enumerable;
			}
			else
			{
				fieldsToLoadIds13 = customFieldsSupport.GetFieldsIds();
			}
			objectSynchronizationObject2 = new ObjectSynchronizationForNetSuiteObject(data10, fieldsToLoadIds13, synchronizeParameters.DatabaseRow.Name);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.DBT:
			objectSynchronizationObject2 = new ObjectSynchronizationForDBTObject(dr as Node, synchronizeParameters.DatabaseRow.Name);
			break;
		case SharedDatabaseTypeEnum.DatabaseType.SSIS:
		{
			Package package = dr as Package;
			IEnumerable<int> fieldsToLoadIds11;
			if (!hasImportUsingCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds11 = enumerable;
			}
			else
			{
				fieldsToLoadIds11 = customFieldsSupport.GetFieldsIds();
			}
			objectSynchronizationObject2 = new ObjectSynchronizationForSSISObject(package, fieldsToLoadIds11);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.SapHana:
		{
			DbDataReader data7 = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds8;
			if (!hasImportUsingCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds8 = enumerable;
			}
			else
			{
				fieldsToLoadIds8 = customFieldsSupport.GetFieldsIds();
			}
			objectSynchronizationObject2 = new ObjectSynchronizationForSapHanaObject(data7, fieldsToLoadIds8, synchronizeParameters.DatabaseRow.Name);
			break;
		}
		default:
		{
			ObjectSynchronizationObject objectSynchronizationObject;
			if (!string.IsNullOrEmpty(script))
			{
				DbDataReader data = dr as DbDataReader;
				IEnumerable<int> fieldsToLoadIds;
				if (!hasImportUsingCustomFields)
				{
					IEnumerable<int> enumerable = new List<int>();
					fieldsToLoadIds = enumerable;
				}
				else
				{
					fieldsToLoadIds = customFieldsSupport.GetFieldsIds();
				}
				objectSynchronizationObject = new ObjectSynchronizationObject(data, fieldsToLoadIds, script);
			}
			else
			{
				DbDataReader data2 = dr as DbDataReader;
				IEnumerable<int> fieldsToLoadIds2;
				if (!hasImportUsingCustomFields)
				{
					IEnumerable<int> enumerable = new List<int>();
					fieldsToLoadIds2 = enumerable;
				}
				else
				{
					fieldsToLoadIds2 = customFieldsSupport.GetFieldsIds();
				}
				objectSynchronizationObject = new ObjectSynchronizationObject(data2, fieldsToLoadIds2, synchronizeParameters.DatabaseRow.Type);
			}
			objectSynchronizationObject2 = objectSynchronizationObject;
			break;
		}
		}
		ObjectRow objectRow = new ObjectRow(objectSynchronizationObject2, synchronizeParameters.DatabaseRow.Id, synchronizeParameters.DatabaseRow.HasMultipleSchemas, synchronizeParameters.DatabaseRow.Type, hasImportUsingCustomFields ? customFieldsSupport : null);
		if (objectSynchronizationObject2 is ObjectSynchronizationForSalesforceObject objectSynchronizationForSalesforceObject)
		{
			objectRow.Title = objectSynchronizationForSalesforceObject.Title;
		}
		if (objectSynchronizationObject2 is ObjectSynchronizationForDataverseObject objectSynchronizationForDataverseObject)
		{
			objectRow.Title = objectSynchronizationForDataverseObject.Title;
		}
		if (objectSynchronizationObject2 is ObjectSynchronizationForSapHanaObject objectSynchronizationForSapHanaObject)
		{
			objectRow.Title = objectSynchronizationForSapHanaObject.Title;
		}
		backgroundWorkerManager.ReportProgress(new List<string> { "Checking objects' status", objectRow.NameWithType });
		if (synchronizeParameters.DatabaseRow.Id.HasValue)
		{
			string text = DB.Database.CheckSynchronization(objectRow.Name, objectRow.Schema, objectRow.TypeAsString, synchronizeParameters.DatabaseRow.Id.Value, objectRow.DbmsLastModificationDate, owner);
			_ = text != "I";
			switch (text)
			{
			case "I":
				objectRow.SynchronizeState = SynchronizeStateEnum.SynchronizeState.Ignored;
				break;
			case "N":
				objectRow.SynchronizeState = SynchronizeStateEnum.SynchronizeState.New;
				break;
			case "S":
				objectRow.SynchronizeState = (synchronizeParameters.FullImport ? SynchronizeStateEnum.SynchronizeState.Unsynchronized : SynchronizeStateEnum.SynchronizeState.Synchronized);
				objectRow.FullImport = true;
				break;
			case "U":
				objectRow.SynchronizeState = SynchronizeStateEnum.SynchronizeState.Unsynchronized;
				break;
			}
		}
		else
		{
			objectRow.SynchronizeState = SynchronizeStateEnum.SynchronizeState.New;
		}
		rows.Add(objectRow);
	}

	public void AddDeletedObjects(ObservableCollection<ObjectRow> rows, BackgroundWorkerManager backgroundWorkerManager)
	{
		if (synchronizeParameters.DatabaseRow.Id.HasValue)
		{
			string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString(FilterObjectTypeEnum.FilterObjectType.Table, "[tables].[schema]", "[tables].[name]");
			string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString(FilterObjectTypeEnum.FilterObjectType.View, "[tables].[schema]", "[tables].[name]");
			string filterString3 = synchronizeParameters.DatabaseRow.Filter.GetFilterString(FilterObjectTypeEnum.FilterObjectType.Procedure, "[procedures].[schema]", "[procedures].[name]");
			string filterString4 = synchronizeParameters.DatabaseRow.Filter.GetFilterString(FilterObjectTypeEnum.FilterObjectType.Function, "[procedures].[schema]", "[procedures].[name]");
			bool updateEntireDocumentation = synchronizeParameters.UpdateEntireDocumentation;
			CommonFunctionsSynchronize.AddDeletedObjects(rows, DB.Table.GetDataDeletedFromDBMS(synchronizeParameters.DatabaseRow.Id.Value, SharedObjectTypeEnum.ObjectType.Table, filterString, updateEntireDocumentation), synchronizeParameters, backgroundWorkerManager);
			CommonFunctionsSynchronize.AddDeletedObjects(rows, DB.Table.GetDataDeletedFromDBMS(synchronizeParameters.DatabaseRow.Id.Value, SharedObjectTypeEnum.ObjectType.View, filterString2, updateEntireDocumentation), synchronizeParameters, backgroundWorkerManager);
			CommonFunctionsSynchronize.AddDeletedObjects(rows, DB.Table.GetDataDeletedFromDBMS(synchronizeParameters.DatabaseRow.Id.Value, SharedObjectTypeEnum.ObjectType.Structure, null, updateEntireDocumentation), synchronizeParameters, backgroundWorkerManager);
			CommonFunctionsSynchronize.AddDeletedObjects(rows, DB.Procedure.GetDataDeletedFromDBMS(synchronizeParameters.DatabaseRow.Id.Value, SharedObjectTypeEnum.ObjectType.Procedure, filterString3, updateEntireDocumentation), synchronizeParameters, backgroundWorkerManager);
			CommonFunctionsSynchronize.AddDeletedObjects(rows, DB.Procedure.GetDataDeletedFromDBMS(synchronizeParameters.DatabaseRow.Id.Value, SharedObjectTypeEnum.ObjectType.Function, filterString4, updateEntireDocumentation), synchronizeParameters, backgroundWorkerManager);
		}
		CommonFunctionsSynchronize.SetToSynchronizeCheck(rows, synchronizeParameters.DbSynchLocker);
	}

	public bool ClearExistsFlag(BackgroundWorkerManager backgroundWorkerManager, bool isDbAdded, int databaseId, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Preparing repository");
		if (isDbAdded)
		{
			return true;
		}
		return DB.Database.ClearExistsFlag(databaseId, owner);
	}

	public void AddRelation(object dr, SharedDatabaseTypeEnum.DatabaseType databaseType, bool withCustomFields = false)
	{
		RelationRow relationRow = null;
		RelationColumnRow relationColumnRow = null;
		switch (databaseType)
		{
		case SharedDatabaseTypeEnum.DatabaseType.Oracle:
		{
			DbDataReader data10 = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds10;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds10 = enumerable;
			}
			else
			{
				fieldsToLoadIds10 = customFieldsSupport.GetFieldsIds();
			}
			relationRow = new RelationRow(new RelationSynchronizationObjectForOracle(data10, fieldsToLoadIds10), withCustomFields ? customFieldsSupport : null);
			relationColumnRow = new RelationColumnRow(dr as DbDataReader);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.Neo4j:
		{
			Neo4jRelation data6 = dr as Neo4jRelation;
			string name = synchronizeParameters.DatabaseRow.Name;
			IEnumerable<int> fieldsToLoadIds6;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds6 = enumerable;
			}
			else
			{
				fieldsToLoadIds6 = customFieldsSupport.GetFieldsIds();
			}
			relationRow = new RelationRow(new RelationSynchronizationObjectForNeo4j(data6, name, fieldsToLoadIds6), withCustomFields ? customFieldsSupport : null);
			relationColumnRow = new RelationColumnRow(dr as Neo4jRelation);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.SsasTabular:
		{
			SsasRelation data9 = dr as SsasRelation;
			IEnumerable<int> fieldsToLoadIds9;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds9 = enumerable;
			}
			else
			{
				fieldsToLoadIds9 = customFieldsSupport.GetFieldsIds();
			}
			relationRow = new RelationRow(new RelationSynchronizationObjectForSSAS(data9, fieldsToLoadIds9), withCustomFields ? customFieldsSupport : null);
			relationColumnRow = new RelationColumnRow(dr as SsasRelation);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.Salesforce:
		{
			SalesforceRelation data2 = dr as SalesforceRelation;
			IEnumerable<int> fieldsToLoadIds2;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds2 = enumerable;
			}
			else
			{
				fieldsToLoadIds2 = customFieldsSupport.GetFieldsIds();
			}
			relationRow = new RelationRow(new RelationSynchronizationObjectForSalesforce(data2, fieldsToLoadIds2), withCustomFields ? customFieldsSupport : null);
			relationColumnRow = new RelationColumnRow(dr as SalesforceRelation);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.DdlScript:
		{
			DdlScriptRelation data7 = dr as DdlScriptRelation;
			IEnumerable<int> fieldsToLoadIds7;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds7 = enumerable;
			}
			else
			{
				fieldsToLoadIds7 = customFieldsSupport.GetFieldsIds();
			}
			relationRow = new RelationRow(new RelationSynchronizationObjectForDdlScript(data7, fieldsToLoadIds7), withCustomFields ? customFieldsSupport : null);
			relationColumnRow = new RelationColumnRow(dr as DdlScriptRelation);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.InterfaceTables:
		{
			DbDataReader data4 = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds4;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds4 = enumerable;
			}
			else
			{
				fieldsToLoadIds4 = customFieldsSupport.GetFieldsIds();
			}
			RelationSynchronizationObject relationSynchronizationObject = new RelationSynchronizationObject(data4, fieldsToLoadIds4);
			if (string.IsNullOrEmpty(relationSynchronizationObject.Name))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("FK_");
				stringBuilder.Append(relationSynchronizationObject.FkTableName + "_");
				stringBuilder.Append(relationSynchronizationObject.RefTableName + "_");
				stringBuilder.Append(relationSynchronizationObject.FkColumn ?? "");
				relationSynchronizationObject.Name = stringBuilder.ToString();
			}
			relationRow = new RelationRow(relationSynchronizationObject, withCustomFields ? customFieldsSupport : null);
			relationColumnRow = new RelationColumnRow(dr as DbDataReader);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.Dataverse:
		case SharedDatabaseTypeEnum.DatabaseType.Dynamics365:
		{
			DataverseRelationWrapper data3 = dr as DataverseRelationWrapper;
			IEnumerable<int> fieldsToLoadIds3;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds3 = enumerable;
			}
			else
			{
				fieldsToLoadIds3 = customFieldsSupport.GetFieldsIds();
			}
			relationRow = new RelationRow(new RelationSynchronizationObjectForDataverse(data3, fieldsToLoadIds3), withCustomFields ? customFieldsSupport : null);
			relationColumnRow = new RelationColumnRow(dr as DataverseRelationWrapper);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.DBT:
		{
			Dataedo.DBTConnector.Models.Relationship data5 = dr as Dataedo.DBTConnector.Models.Relationship;
			IEnumerable<int> fieldsToLoadIds5;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds5 = enumerable;
			}
			else
			{
				fieldsToLoadIds5 = customFieldsSupport.GetFieldsIds();
			}
			relationRow = new RelationRow(new RelationSynchronizationObjectForDBT(data5, fieldsToLoadIds5), withCustomFields ? customFieldsSupport : null);
			relationColumnRow = new RelationColumnRow(dr as Dataedo.DBTConnector.Models.Relationship);
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.Snowflake:
		{
			DbDataReader data8 = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds8;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds8 = enumerable;
			}
			else
			{
				fieldsToLoadIds8 = customFieldsSupport.GetFieldsIds();
			}
			RelationSynchronizationObject relationSynchronizationObject2 = new RelationSynchronizationObjectForSnowflake(data8, fieldsToLoadIds8);
			relationRow = new RelationRow(relationSynchronizationObject2, withCustomFields ? customFieldsSupport : null);
			relationColumnRow = new RelationColumnRow(relationSynchronizationObject2);
			break;
		}
		default:
		{
			DbDataReader data = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds = enumerable;
			}
			else
			{
				fieldsToLoadIds = customFieldsSupport.GetFieldsIds();
			}
			relationRow = new RelationRow(new RelationSynchronizationObject(data, fieldsToLoadIds), withCustomFields ? customFieldsSupport : null);
			relationColumnRow = new RelationColumnRow(dr as DbDataReader);
			break;
		}
		}
		RelationRow relationRow2 = RelationRows.Where((RelationRow x) => x.Name.Equals(relationRow.Name) && x.FKTableSchema.Equals(relationRow.FKTableSchema) && x.FKTableName.Equals(relationRow.FKTableName) && x.PKTableSchema.Equals(relationRow.PKTableSchema) && x.PKTableName.Equals(relationRow.PKTableName)).FirstOrDefault();
		if (relationRow2 == null)
		{
			RelationRows.Add(relationRow);
			relationRow2 = relationRow;
		}
		relationRow2.Columns.Add(relationColumnRow);
	}

	public void AddColumn(ColumnRow row)
	{
		ColumnRows.Add(row);
	}

	public void AddColumn(object dr, bool withCustomFields = false, bool withTableTypes = false)
	{
		if (withTableTypes)
		{
			ObservableCollection<ColumnRow> columnRows = ColumnRows;
			DbDataReader data = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds = enumerable;
			}
			else
			{
				fieldsToLoadIds = customFieldsSupport.GetFieldsIds();
			}
			columnRows.Add(new ColumnRow(new ColumnWithTableTypeSynchronizationObject(data, fieldsToLoadIds), withCustomFields ? customFieldsSupport : null));
		}
		else if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.MongoDB || synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB)
		{
			ObservableCollection<ColumnRow> columnRows2 = ColumnRows;
			BsonColumn data2 = dr as BsonColumn;
			IEnumerable<int> fieldsToLoadIds2;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds2 = enumerable;
			}
			else
			{
				fieldsToLoadIds2 = customFieldsSupport.GetFieldsIds();
			}
			columnRows2.Add(new ColumnRow(new ColumnSynchronizationForMongoDBObject(data2, fieldsToLoadIds2), withCustomFields ? customFieldsSupport : null));
		}
		else if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Cassandra)
		{
			ObservableCollection<ColumnRow> columnRows3 = ColumnRows;
			Row data3 = dr as Row;
			IEnumerable<int> fieldsToLoadIds3;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds3 = enumerable;
			}
			else
			{
				fieldsToLoadIds3 = customFieldsSupport.GetFieldsIds();
			}
			columnRows3.Add(new ColumnRow(new ColumnSynchronizationObject(data3, fieldsToLoadIds3, synchronizeParameters.DatabaseRow.Type), withCustomFields ? customFieldsSupport : null));
		}
		else if (IsCassandraRowWithPositionCheck(synchronizeParameters.DatabaseRow.Type))
		{
			Row data4 = dr as Row;
			IEnumerable<int> fieldsToLoadIds4;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds4 = enumerable;
			}
			else
			{
				fieldsToLoadIds4 = customFieldsSupport.GetFieldsIds();
			}
			ColumnRow columRow = new ColumnRow(new ColumnSynchronizationObject(data4, fieldsToLoadIds4, synchronizeParameters.DatabaseRow.Type), withCustomFields ? customFieldsSupport : null);
			ColumnRow columnRow2 = ColumnRows.FirstOrDefault((ColumnRow c) => c.TableName == columRow.TableName && c.Position == columRow.Position);
			if (columRow.Position < 1 || columnRow2 != null)
			{
				List<int> second = ColumnRows.Where((ColumnRow c) => c.TableName == columRow.TableName && c.Position.HasValue)?.Select((ColumnRow c) => c.Position.Value).ToList();
				columRow.Position = Enumerable.Range(1, int.MaxValue).Except(second).FirstOrDefault();
			}
			ColumnRows.Add(columRow);
		}
		else if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Elasticsearch)
		{
			ObservableCollection<ColumnRow> columnRows4 = ColumnRows;
			ElasticsearchColumnData data5 = dr as ElasticsearchColumnData;
			IEnumerable<int> fieldsToLoadIds5;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds5 = enumerable;
			}
			else
			{
				fieldsToLoadIds5 = customFieldsSupport.GetFieldsIds();
			}
			columnRows4.Add(new ColumnRow(new ColumnSynchronizationForElasticsearchObject(data5, fieldsToLoadIds5, synchronizeParameters.DatabaseRow.Name), withCustomFields ? customFieldsSupport : null));
		}
		else if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Neo4j)
		{
			ObservableCollection<ColumnRow> columnRows5 = ColumnRows;
			Neo4jColumn data6 = dr as Neo4jColumn;
			string name = synchronizeParameters.DatabaseRow.Name;
			IEnumerable<int> fieldsToLoadIds6;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds6 = enumerable;
			}
			else
			{
				fieldsToLoadIds6 = customFieldsSupport.GetFieldsIds();
			}
			columnRows5.Add(new ColumnRow(new ColumnSynchronizationForNeo4jObject(data6, name, fieldsToLoadIds6), withCustomFields ? customFieldsSupport : null));
		}
		else if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.SsasTabular || synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.PowerBiDataset)
		{
			PerspectiveColumn perspectiveColumn = dr as PerspectiveColumn;
			if (perspectiveColumn != null)
			{
				PerspectiveColumn perspectiveColumn2 = perspectiveColumn;
				IEnumerable<int> fieldsToLoadIds7;
				if (!withCustomFields)
				{
					IEnumerable<int> enumerable = new List<int>();
					fieldsToLoadIds7 = enumerable;
				}
				else
				{
					fieldsToLoadIds7 = customFieldsSupport.GetFieldsIds();
				}
				ColumnRow columnRow3 = new ColumnRow(new ColumnSynchronizationForSsasObject(perspectiveColumn2, fieldsToLoadIds7), withCustomFields ? customFieldsSupport : null);
				if (!columnRow3.Position.HasValue && !string.IsNullOrEmpty(perspectiveColumn.PerspectiveTable?.Perspective?.Name))
				{
					columnRow3.Position = ColumnRows.Where((ColumnRow c) => c.TableName == perspectiveColumn.PerspectiveTable.Perspective.Name && c.TableType == SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.View)).Count() + 1;
				}
				ColumnRows.Add(columnRow3);
				return;
			}
			Measure measure = dr as Measure;
			if (measure != null)
			{
				Measure measure2 = measure;
				IEnumerable<int> fieldsToLoadIds8;
				if (!withCustomFields)
				{
					IEnumerable<int> enumerable = new List<int>();
					fieldsToLoadIds8 = enumerable;
				}
				else
				{
					fieldsToLoadIds8 = customFieldsSupport.GetFieldsIds();
				}
				ColumnRow columnRow4 = new ColumnRow(new ColumnSynchronizationForSsasObject(measure2, fieldsToLoadIds8), withCustomFields ? customFieldsSupport : null);
				if (!columnRow4.Position.HasValue && !string.IsNullOrEmpty(measure?.Table?.Name))
				{
					columnRow4.Position = ColumnRows.Where((ColumnRow c) => c.TableName == measure.Table.Name).Count();
				}
				ColumnRows.Add(columnRow4);
				return;
			}
			PerspectiveMeasure perspectiveMeasure = dr as PerspectiveMeasure;
			if (perspectiveMeasure != null)
			{
				PerspectiveMeasure measure3 = perspectiveMeasure;
				IEnumerable<int> fieldsToLoadIds9;
				if (!withCustomFields)
				{
					IEnumerable<int> enumerable = new List<int>();
					fieldsToLoadIds9 = enumerable;
				}
				else
				{
					fieldsToLoadIds9 = customFieldsSupport.GetFieldsIds();
				}
				ColumnRow columnRow5 = new ColumnRow(new ColumnSynchronizationForSsasObject(measure3, fieldsToLoadIds9), withCustomFields ? customFieldsSupport : null);
				if (!columnRow5.Position.HasValue && !string.IsNullOrEmpty(perspectiveMeasure?.PerspectiveTable?.Name))
				{
					columnRow5.Position = ColumnRows.Where((ColumnRow c) => c.TableName == perspectiveMeasure.PerspectiveTable.Name).Count();
				}
				ColumnRows.Add(columnRow5);
			}
			else
			{
				ObservableCollection<ColumnRow> columnRows6 = ColumnRows;
				Microsoft.AnalysisServices.Tabular.Column column = dr as Microsoft.AnalysisServices.Tabular.Column;
				IEnumerable<int> fieldsToLoadIds10;
				if (!withCustomFields)
				{
					IEnumerable<int> enumerable = new List<int>();
					fieldsToLoadIds10 = enumerable;
				}
				else
				{
					fieldsToLoadIds10 = customFieldsSupport.GetFieldsIds();
				}
				columnRows6.Add(new ColumnRow(new ColumnSynchronizationForSsasObject(column, fieldsToLoadIds10), withCustomFields ? customFieldsSupport : null));
			}
		}
		else if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Athena)
		{
			ObservableCollection<ColumnRow> columnRows7 = ColumnRows;
			Dictionary<string, string> data7 = dr as Dictionary<string, string>;
			IEnumerable<int> fieldsToLoadIds11;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds11 = enumerable;
			}
			else
			{
				fieldsToLoadIds11 = customFieldsSupport.GetFieldsIds();
			}
			columnRows7.Add(new ColumnRow(new ColumnSynchronizationForMongoDBObject(data7, fieldsToLoadIds11), withCustomFields ? customFieldsSupport : null));
		}
		else if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Salesforce)
		{
			ObservableCollection<ColumnRow> columnRows8 = ColumnRows;
			SObjectFieldWrapper wrapper = dr as SObjectFieldWrapper;
			IEnumerable<int> fieldsToLoadIds12;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds12 = enumerable;
			}
			else
			{
				fieldsToLoadIds12 = customFieldsSupport.GetFieldsIds();
			}
			columnRows8.Add(new ColumnRow(new ColumnSynchronizationForSalesforceObject(wrapper, fieldsToLoadIds12), withCustomFields ? customFieldsSupport : null));
		}
		else if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.CosmosDbSQL)
		{
			ObservableCollection<ColumnRow> columnRows9 = ColumnRows;
			BsonColumn data8 = dr as BsonColumn;
			IEnumerable<int> fieldsToLoadIds13;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds13 = enumerable;
			}
			else
			{
				fieldsToLoadIds13 = customFieldsSupport.GetFieldsIds();
			}
			columnRows9.Add(new ColumnRow(new ColumnSynchronizationForMongoDBObject(data8, fieldsToLoadIds13), withCustomFields ? customFieldsSupport : null));
		}
		else if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Tableau)
		{
			ObservableCollection<ColumnRow> columnRows10 = ColumnRows;
			JObject data9 = dr as JObject;
			IEnumerable<int> fieldsToLoadIds14;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds14 = enumerable;
			}
			else
			{
				fieldsToLoadIds14 = customFieldsSupport.GetFieldsIds();
			}
			columnRows10.Add(new ColumnRow(new ColumnSynchronizationForTableauObject(data9, fieldsToLoadIds14), withCustomFields ? customFieldsSupport : null));
		}
		else if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.InterfaceTables)
		{
			ObservableCollection<ColumnRow> columnRows11 = ColumnRows;
			DbDataReader data10 = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds15;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds15 = enumerable;
			}
			else
			{
				fieldsToLoadIds15 = customFieldsSupport.GetFieldsIds();
			}
			columnRows11.Add(new ColumnRow(new ColumnSynchronizationForInterfaceTablesObject(data10, fieldsToLoadIds15), withCustomFields ? customFieldsSupport : null));
		}
		else if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Dataverse || synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Dynamics365)
		{
			ObservableCollection<ColumnRow> columnRows12 = ColumnRows;
			DataverseColumnWrapper wrapper2 = dr as DataverseColumnWrapper;
			IEnumerable<int> fieldsToLoadIds16;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds16 = enumerable;
			}
			else
			{
				fieldsToLoadIds16 = customFieldsSupport.GetFieldsIds();
			}
			columnRows12.Add(new ColumnRow(new ColumnSynchronizationForDataverseObject(wrapper2, fieldsToLoadIds16), withCustomFields ? customFieldsSupport : null));
		}
		else if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.DynamoDB)
		{
			ObservableCollection<ColumnRow> columnRows13 = ColumnRows;
			DynamoDBColumn column2 = dr as DynamoDBColumn;
			IEnumerable<int> fieldsToLoadIds17;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds17 = enumerable;
			}
			else
			{
				fieldsToLoadIds17 = customFieldsSupport.GetFieldsIds();
			}
			columnRows13.Add(new ColumnRow(new ColumnSynchronizationForDynamoDBObject(column2, fieldsToLoadIds17), withCustomFields ? customFieldsSupport : null));
		}
		else if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.NetSuite)
		{
			DbDataReader column3 = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds18;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds18 = enumerable;
			}
			else
			{
				fieldsToLoadIds18 = customFieldsSupport.GetFieldsIds();
			}
			ColumnRow columnRow = new ColumnRow(new ColumnSynchronizationForNetSuiteObject(column3, fieldsToLoadIds18, synchronizeParameters.DatabaseRow.Name), withCustomFields ? customFieldsSupport : null);
			columnRow.Position = ColumnRows.Where((ColumnRow c) => c.TableName == columnRow.TableName).Count() + 1;
			ColumnRows.Add(columnRow);
		}
		else if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.GoogleBigQuery)
		{
			ObservableCollection<ColumnRow> columnRows14 = ColumnRows;
			BigQueryRow data11 = dr as BigQueryRow;
			IEnumerable<int> fieldsToLoadIds19;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds19 = enumerable;
			}
			else
			{
				fieldsToLoadIds19 = customFieldsSupport.GetFieldsIds();
			}
			columnRows14.Add(new ColumnRow(new ColumnSynchronizationForMongoDBObject(data11, fieldsToLoadIds19), withCustomFields ? customFieldsSupport : null));
		}
		else
		{
			ObservableCollection<ColumnRow> columnRows15 = ColumnRows;
			DbDataReader data12 = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds20;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds20 = enumerable;
			}
			else
			{
				fieldsToLoadIds20 = customFieldsSupport.GetFieldsIds();
			}
			columnRows15.Add(new ColumnRow(new ColumnSynchronizationObject(data12, fieldsToLoadIds20), withCustomFields ? customFieldsSupport : null));
		}
	}

	public void AddParameter(ParameterRow row)
	{
		ParameterRows.Add(row);
	}

	public void AddParameter(object dr, bool withCustomFields = false)
	{
		switch (synchronizeParameters.DatabaseRow.Type)
		{
		case SharedDatabaseTypeEnum.DatabaseType.Cassandra:
		case SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra:
		case SharedDatabaseTypeEnum.DatabaseType.AmazonKeyspaces:
		case SharedDatabaseTypeEnum.DatabaseType.Astra:
		{
			ObservableCollection<ParameterRow> parameterRows4 = ParameterRows;
			Row data5 = dr as Row;
			IEnumerable<int> fieldsToLoadIds5;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds5 = enumerable;
			}
			else
			{
				fieldsToLoadIds5 = customFieldsSupport.GetFieldsIds();
			}
			parameterRows4.Add(new ParameterRow(new ParameterSynchronizationObject(data5, fieldsToLoadIds5), withCustomFields ? customFieldsSupport : null));
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.GoogleBigQuery:
		{
			ObservableCollection<ParameterRow> parameterRows5 = ParameterRows;
			BigQueryRow data6 = dr as BigQueryRow;
			IEnumerable<int> fieldsToLoadIds6;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds6 = enumerable;
			}
			else
			{
				fieldsToLoadIds6 = customFieldsSupport.GetFieldsIds();
			}
			parameterRows5.Add(new ParameterRow(new ParameterSynchronizationObject(data6, fieldsToLoadIds6), withCustomFields ? customFieldsSupport : null));
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.Neo4j:
		{
			ObservableCollection<ParameterRow> parameterRows3 = ParameterRows;
			Neo4jParameter data4 = dr as Neo4jParameter;
			string name = synchronizeParameters.DatabaseRow.Name;
			IEnumerable<int> fieldsToLoadIds4;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds4 = enumerable;
			}
			else
			{
				fieldsToLoadIds4 = customFieldsSupport.GetFieldsIds();
			}
			parameterRows3.Add(new ParameterRow(new ParameterSynchronizationObjectForNeo4j(data4, name, fieldsToLoadIds4), withCustomFields ? customFieldsSupport : null));
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.InterfaceTables:
		{
			ObservableCollection<ParameterRow> parameterRows2 = ParameterRows;
			DbDataReader data3 = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds3;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds3 = enumerable;
			}
			else
			{
				fieldsToLoadIds3 = customFieldsSupport.GetFieldsIds();
			}
			parameterRows2.Add(new ParameterRow(new ParameterSynchronizationObjectForInterfaceTables(data3, fieldsToLoadIds3), withCustomFields ? customFieldsSupport : null));
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.Snowflake:
		{
			DbDataReader data2 = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds2;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds2 = enumerable;
			}
			else
			{
				fieldsToLoadIds2 = customFieldsSupport.GetFieldsIds();
			}
			ParameterSynchronizationObject parameterSynchronizationObject = new ParameterSynchronizationObject(data2, fieldsToLoadIds2);
			if (parameterSynchronizationObject.Name == "Arguments")
			{
				string datatype = parameterSynchronizationObject.Datatype;
				if (datatype != null && datatype.Length > 250)
				{
					IEnumerable<string> enumerable2 = parameterSynchronizationObject.Datatype.Split(250);
					int num = 1;
					{
						foreach (string item2 in enumerable2)
						{
							ParameterRow item = new ParameterRow(parameterSynchronizationObject, withCustomFields ? customFieldsSupport : null)
							{
								Name = $"Arguments ({num++})",
								DataType = item2
							};
							ParameterRows.Add(item);
						}
						break;
					}
				}
			}
			ParameterRows.Add(new ParameterRow(parameterSynchronizationObject, withCustomFields ? customFieldsSupport : null));
			break;
		}
		default:
		{
			ObservableCollection<ParameterRow> parameterRows = ParameterRows;
			DbDataReader data = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds = enumerable;
			}
			else
			{
				fieldsToLoadIds = customFieldsSupport.GetFieldsIds();
			}
			parameterRows.Add(new ParameterRow(new ParameterSynchronizationObject(data, fieldsToLoadIds), withCustomFields ? customFieldsSupport : null));
			break;
		}
		}
	}

	public void AddParameter(ParameterSynchronizationObject parameter)
	{
		ParameterRows.Add(new ParameterRow(parameter));
	}

	public void AddDependency(object dr)
	{
		if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.MongoDB || synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB)
		{
			DependenciesRows.Add(new DependencyRow(new DependencySynchronizationForMongoDBObject(dr as MongoDBDependency)));
		}
		else if (IsCassandraBasedType(synchronizeParameters.DatabaseRow.Type))
		{
			DependenciesRows.Add(new DependencyRow(new DependencySynchronizationObject(dr as Row)));
		}
		else if (synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Dataverse || synchronizeParameters.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Dynamics365)
		{
			DependenciesRows.Add(new DependencyRow(new DependencySynchronizationForDataverseObject(dr as DataverseDependencyWrapper)));
		}
		else
		{
			DependenciesRows.Add(new DependencyRow(new DependencySynchronizationObject(dr as DbDataReader)));
		}
	}

	public void AddTrigger(object dr, bool withCustomFields = false)
	{
		switch (synchronizeParameters.DatabaseRow.Type)
		{
		case SharedDatabaseTypeEnum.DatabaseType.Cassandra:
		case SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra:
		case SharedDatabaseTypeEnum.DatabaseType.AmazonKeyspaces:
		case SharedDatabaseTypeEnum.DatabaseType.Astra:
		{
			ObservableCollection<TriggerRow> triggerRows2 = TriggerRows;
			Row data2 = dr as Row;
			IEnumerable<int> fieldsToLoadIds2;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds2 = enumerable;
			}
			else
			{
				fieldsToLoadIds2 = customFieldsSupport.GetFieldsIds();
			}
			triggerRows2.Add(new TriggerRow(new TriggerSynchronizationObject(data2, fieldsToLoadIds2, synchronizeParameters.DatabaseRow.Type), withCustomFields ? customFieldsSupport : null));
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.CosmosDbSQL:
		case SharedDatabaseTypeEnum.DatabaseType.DdlScript:
			TriggerRows.Add(new TriggerRow(dr as TriggerSynchronizationObject, withCustomFields ? customFieldsSupport : null));
			break;
		case SharedDatabaseTypeEnum.DatabaseType.Salesforce:
		{
			ObservableCollection<TriggerRow> triggerRows4 = TriggerRows;
			STriggerObject data4 = dr as STriggerObject;
			IEnumerable<int> fieldsToLoadIds4;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds4 = enumerable;
			}
			else
			{
				fieldsToLoadIds4 = customFieldsSupport.GetFieldsIds();
			}
			triggerRows4.Add(new TriggerRow(new TriggerSynchronizationObject(data4, fieldsToLoadIds4, synchronizeParameters.DatabaseRow.Type), withCustomFields ? customFieldsSupport : null));
			break;
		}
		case SharedDatabaseTypeEnum.DatabaseType.InterfaceTables:
		{
			ObservableCollection<TriggerRow> triggerRows3 = TriggerRows;
			DbDataReader data3 = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds3;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds3 = enumerable;
			}
			else
			{
				fieldsToLoadIds3 = customFieldsSupport.GetFieldsIds();
			}
			triggerRows3.Add(new TriggerRow(new TriggerSynchronizationObjectForInterfaceTables(data3, fieldsToLoadIds3, synchronizeParameters.DatabaseRow.Type), withCustomFields ? customFieldsSupport : null));
			break;
		}
		default:
		{
			ObservableCollection<TriggerRow> triggerRows = TriggerRows;
			DbDataReader data = dr as DbDataReader;
			IEnumerable<int> fieldsToLoadIds;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds = enumerable;
			}
			else
			{
				fieldsToLoadIds = customFieldsSupport.GetFieldsIds();
			}
			triggerRows.Add(new TriggerRow(new TriggerSynchronizationObject(data, fieldsToLoadIds, synchronizeParameters.DatabaseRow.Type), withCustomFields ? customFieldsSupport : null));
			break;
		}
		}
	}

	public void AddUniqueConstraint(object dr, SharedDatabaseTypeEnum.DatabaseType databaseType, bool withCustomFields = false)
	{
		UniqueConstraintRow constraintRow;
		if (databaseType == SharedDatabaseTypeEnum.DatabaseType.MongoDB || databaseType == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB)
		{
			MongoDBConstraint data = dr as MongoDBConstraint;
			IEnumerable<int> fieldsToLoadIds;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds = enumerable;
			}
			else
			{
				fieldsToLoadIds = customFieldsSupport.GetFieldsIds();
			}
			constraintRow = new UniqueConstraintRow(new UniqueConstraintSynchronizationObjectForMongoDB(data, fieldsToLoadIds), withCustomFields ? customFieldsSupport : null);
		}
		else if (databaseType == SharedDatabaseTypeEnum.DatabaseType.Cassandra)
		{
			Row data2 = dr as Row;
			IEnumerable<int> fieldsToLoadIds2;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds2 = enumerable;
			}
			else
			{
				fieldsToLoadIds2 = customFieldsSupport.GetFieldsIds();
			}
			constraintRow = new UniqueConstraintRow(new UniqueConstraintSynchronizationObject(data2, fieldsToLoadIds2), withCustomFields ? customFieldsSupport : null);
		}
		else if (databaseType == SharedDatabaseTypeEnum.DatabaseType.Neo4j)
		{
			Neo4jConstraint data3 = dr as Neo4jConstraint;
			IEnumerable<int> fieldsToLoadIds3;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds3 = enumerable;
			}
			else
			{
				fieldsToLoadIds3 = customFieldsSupport.GetFieldsIds();
			}
			constraintRow = new UniqueConstraintRow(new UniqueConstraintSynchronizationObjectForNeo4j(data3, fieldsToLoadIds3), withCustomFields ? customFieldsSupport : null);
		}
		else if (databaseType == SharedDatabaseTypeEnum.DatabaseType.SsasTabular || databaseType == SharedDatabaseTypeEnum.DatabaseType.PowerBiDataset)
		{
			SsasConstraint data4 = dr as SsasConstraint;
			IEnumerable<int> fieldsToLoadIds4;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds4 = enumerable;
			}
			else
			{
				fieldsToLoadIds4 = customFieldsSupport.GetFieldsIds();
			}
			constraintRow = new UniqueConstraintRow(new UniqueConstraintSynchronizationObjectForSSAS(data4, fieldsToLoadIds4), withCustomFields ? customFieldsSupport : null);
		}
		else if (databaseType == SharedDatabaseTypeEnum.DatabaseType.CosmosDbSQL || IsCassandraBasedType(synchronizeParameters.DatabaseRow.Type))
		{
			UniqueConstraintBaseObject data5 = dr as UniqueConstraintBaseObject;
			IEnumerable<int> fieldsToLoadIds5;
			if (!withCustomFields)
			{
				IEnumerable<int> enumerable = new List<int>();
				fieldsToLoadIds5 = enumerable;
			}
			else
			{
				fieldsToLoadIds5 = customFieldsSupport.GetFieldsIds();
			}
			constraintRow = new UniqueConstraintRow(new UniqueConstraintSynchronizationObject(data5, fieldsToLoadIds5), withCustomFields ? customFieldsSupport : null);
		}
		else
		{
			switch (databaseType)
			{
			case SharedDatabaseTypeEnum.DatabaseType.Salesforce:
			{
				SalesforceConstraint data9 = dr as SalesforceConstraint;
				IEnumerable<int> fieldsToLoadIds10;
				if (!withCustomFields)
				{
					IEnumerable<int> enumerable = new List<int>();
					fieldsToLoadIds10 = enumerable;
				}
				else
				{
					fieldsToLoadIds10 = customFieldsSupport.GetFieldsIds();
				}
				constraintRow = new UniqueConstraintRow(new UniqueConstraintSynchronizationObjectForSalesforce(data9, fieldsToLoadIds10), withCustomFields ? customFieldsSupport : null);
				break;
			}
			case SharedDatabaseTypeEnum.DatabaseType.DdlScript:
			{
				DdlScriptConstraint data13 = dr as DdlScriptConstraint;
				IEnumerable<int> fieldsToLoadIds14;
				if (!withCustomFields)
				{
					IEnumerable<int> enumerable = new List<int>();
					fieldsToLoadIds14 = enumerable;
				}
				else
				{
					fieldsToLoadIds14 = customFieldsSupport.GetFieldsIds();
				}
				constraintRow = new UniqueConstraintRow(new UniqueConstraintSynchronizationObjectForDdlScript(data13, fieldsToLoadIds14), withCustomFields ? customFieldsSupport : null);
				break;
			}
			case SharedDatabaseTypeEnum.DatabaseType.InterfaceTables:
			{
				DbDataReader data7 = dr as DbDataReader;
				IEnumerable<int> fieldsToLoadIds8;
				if (!withCustomFields)
				{
					IEnumerable<int> enumerable = new List<int>();
					fieldsToLoadIds8 = enumerable;
				}
				else
				{
					fieldsToLoadIds8 = customFieldsSupport.GetFieldsIds();
				}
				UniqueConstraintSynchronizationObject uniqueConstraintSynchronizationObject = new UniqueConstraintSynchronizationObject(data7, fieldsToLoadIds8);
				if (string.IsNullOrEmpty(uniqueConstraintSynchronizationObject.Name))
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(uniqueConstraintSynchronizationObject.Type + "K_");
					stringBuilder.Append(uniqueConstraintSynchronizationObject.TableName + "_");
					stringBuilder.Append(uniqueConstraintSynchronizationObject.ColumnName);
					uniqueConstraintSynchronizationObject.Name = stringBuilder.ToString();
				}
				constraintRow = new UniqueConstraintRow(uniqueConstraintSynchronizationObject, withCustomFields ? customFieldsSupport : null);
				break;
			}
			case SharedDatabaseTypeEnum.DatabaseType.Dataverse:
			case SharedDatabaseTypeEnum.DatabaseType.Dynamics365:
			{
				UniqueConstraintDataverseWrapper data11 = dr as UniqueConstraintDataverseWrapper;
				IEnumerable<int> fieldsToLoadIds12;
				if (!withCustomFields)
				{
					IEnumerable<int> enumerable = new List<int>();
					fieldsToLoadIds12 = enumerable;
				}
				else
				{
					fieldsToLoadIds12 = customFieldsSupport.GetFieldsIds();
				}
				constraintRow = new UniqueConstraintRow(new UniqueConstraintSynchronizationObjectForDataverse(data11, fieldsToLoadIds12), withCustomFields ? customFieldsSupport : null);
				break;
			}
			case SharedDatabaseTypeEnum.DatabaseType.DynamoDB:
			{
				DynamoDBConstraint constraint = dr as DynamoDBConstraint;
				IEnumerable<int> fieldsToLoadIds7;
				if (!withCustomFields)
				{
					IEnumerable<int> enumerable = new List<int>();
					fieldsToLoadIds7 = enumerable;
				}
				else
				{
					fieldsToLoadIds7 = customFieldsSupport.GetFieldsIds();
				}
				constraintRow = new UniqueConstraintRow(new UniqueConstraintSynchronizationObjectForDynamoDB(constraint, fieldsToLoadIds7), withCustomFields ? customFieldsSupport : null);
				break;
			}
			case SharedDatabaseTypeEnum.DatabaseType.NetSuite:
			{
				DbDataReader data8 = dr as DbDataReader;
				IEnumerable<int> fieldsToLoadIds9;
				if (!withCustomFields)
				{
					IEnumerable<int> enumerable = new List<int>();
					fieldsToLoadIds9 = enumerable;
				}
				else
				{
					fieldsToLoadIds9 = customFieldsSupport.GetFieldsIds();
				}
				constraintRow = new UniqueConstraintRow(new UniqueConstraintSynchronizationObjectForNetSuite(data8, fieldsToLoadIds9, synchronizeParameters.DatabaseRow.Name), withCustomFields ? customFieldsSupport : null);
				break;
			}
			case SharedDatabaseTypeEnum.DatabaseType.Snowflake:
			{
				SnowflakeConstraint data10 = dr as SnowflakeConstraint;
				IEnumerable<int> fieldsToLoadIds11;
				if (!withCustomFields)
				{
					IEnumerable<int> enumerable = new List<int>();
					fieldsToLoadIds11 = enumerable;
				}
				else
				{
					fieldsToLoadIds11 = customFieldsSupport.GetFieldsIds();
				}
				constraintRow = new UniqueConstraintRow(new UniqueConstraintSynchronizationObjectForSnowflake(data10, fieldsToLoadIds11), withCustomFields ? customFieldsSupport : null);
				break;
			}
			case SharedDatabaseTypeEnum.DatabaseType.DBT:
			{
				UniqueColumnDbt data12 = dr as UniqueColumnDbt;
				IEnumerable<int> fieldsToLoadIds13;
				if (!withCustomFields)
				{
					IEnumerable<int> enumerable = new List<int>();
					fieldsToLoadIds13 = enumerable;
				}
				else
				{
					fieldsToLoadIds13 = customFieldsSupport.GetFieldsIds();
				}
				constraintRow = new UniqueConstraintRow(new UniqueConstraintSynchronizationObjectForDbt(data12, fieldsToLoadIds13), withCustomFields ? customFieldsSupport : null);
				break;
			}
			default:
			{
				DbDataReader data6 = dr as DbDataReader;
				IEnumerable<int> fieldsToLoadIds6;
				if (!withCustomFields)
				{
					IEnumerable<int> enumerable = new List<int>();
					fieldsToLoadIds6 = enumerable;
				}
				else
				{
					fieldsToLoadIds6 = customFieldsSupport.GetFieldsIds();
				}
				constraintRow = new UniqueConstraintRow(new UniqueConstraintSynchronizationObject(data6, fieldsToLoadIds6), withCustomFields ? customFieldsSupport : null);
				break;
			}
			}
		}
		UniqueConstraintColumnRow uniqueConstraintColumnRow = null;
		if (DatabaseTypeEnum.UsesOracleLibraries(databaseType))
		{
			uniqueConstraintColumnRow = new UniqueConstraintColumnRow();
			uniqueConstraintColumnRow.InitializeOracleData(new UniqueConstraintColumnSynchronizationObjectForOracle(dr as DbDataReader));
		}
		else if (databaseType == SharedDatabaseTypeEnum.DatabaseType.Redshift)
		{
			uniqueConstraintColumnRow = new UniqueConstraintColumnRow(dr as NpgsqlDataReader);
		}
		else if (databaseType == SharedDatabaseTypeEnum.DatabaseType.MongoDB || databaseType == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB)
		{
			uniqueConstraintColumnRow = new UniqueConstraintColumnRow(dr as MongoDBConstraint);
		}
		else if (databaseType == SharedDatabaseTypeEnum.DatabaseType.Cassandra)
		{
			uniqueConstraintColumnRow = new UniqueConstraintColumnRow();
			uniqueConstraintColumnRow.InitializeData(new UniqueConstraintColumnSynchronizationObject(dr as Row));
		}
		else if (databaseType == SharedDatabaseTypeEnum.DatabaseType.Neo4j)
		{
			uniqueConstraintColumnRow = new UniqueConstraintColumnRow(dr as Neo4jConstraint);
		}
		else if (databaseType == SharedDatabaseTypeEnum.DatabaseType.SsasTabular || databaseType == SharedDatabaseTypeEnum.DatabaseType.PowerBiDataset)
		{
			uniqueConstraintColumnRow = new UniqueConstraintColumnRow(dr as SsasConstraint);
		}
		else if (databaseType == SharedDatabaseTypeEnum.DatabaseType.CosmosDbSQL || IsCassandraBasedType(synchronizeParameters.DatabaseRow.Type))
		{
			uniqueConstraintColumnRow = new UniqueConstraintColumnRow(dr as UniqueConstraintBaseObject);
		}
		else
		{
			switch (databaseType)
			{
			case SharedDatabaseTypeEnum.DatabaseType.Salesforce:
				uniqueConstraintColumnRow = new UniqueConstraintColumnRow(dr as SalesforceConstraint);
				break;
			case SharedDatabaseTypeEnum.DatabaseType.DdlScript:
				uniqueConstraintColumnRow = new UniqueConstraintColumnRow(dr as DdlScriptConstraint);
				break;
			case SharedDatabaseTypeEnum.DatabaseType.InterfaceTables:
				uniqueConstraintColumnRow = new UniqueConstraintColumnRow();
				uniqueConstraintColumnRow.InitializeInterfaceTablesData(new UniqueConstraintColumnSynchronizationObjectForInterfaceTables(dr as DbDataReader));
				break;
			case SharedDatabaseTypeEnum.DatabaseType.Dataverse:
			case SharedDatabaseTypeEnum.DatabaseType.Dynamics365:
				uniqueConstraintColumnRow = new UniqueConstraintColumnRow(dr as UniqueConstraintDataverseWrapper);
				break;
			case SharedDatabaseTypeEnum.DatabaseType.DynamoDB:
				uniqueConstraintColumnRow = new UniqueConstraintColumnRow(dr as DynamoDBConstraint);
				break;
			case SharedDatabaseTypeEnum.DatabaseType.Snowflake:
				uniqueConstraintColumnRow = new UniqueConstraintColumnRow(dr as SnowflakeConstraint);
				break;
			case SharedDatabaseTypeEnum.DatabaseType.DBT:
				uniqueConstraintColumnRow = new UniqueConstraintColumnRow(dr as UniqueColumnDbt);
				break;
			default:
				uniqueConstraintColumnRow = new UniqueConstraintColumnRow();
				uniqueConstraintColumnRow.InitializeData(new UniqueConstraintColumnSynchronizationObject(dr as DbDataReader));
				break;
			}
		}
		UniqueConstraintRow uniqueConstraintRow = UniqueConstraintRows.Where((UniqueConstraintRow x) => x.DatabaseName.Equals(constraintRow.DatabaseName) && x.TableSchema.Equals(constraintRow.TableSchema) && x.TableName.Equals(constraintRow.TableName) && x.Name.Equals(constraintRow.Name) && x.Type.Equals(constraintRow.Type)).FirstOrDefault();
		if (uniqueConstraintRow == null)
		{
			UniqueConstraintRows.Add(constraintRow);
			uniqueConstraintRow = constraintRow;
		}
		uniqueConstraintRow.Columns.Add(uniqueConstraintColumnRow);
	}

	public void ReadCount(SharedObjectTypeEnum.ObjectType type, int count)
	{
		switch (type)
		{
		case SharedObjectTypeEnum.ObjectType.Table:
			ObjectsCounter.TablesCount += count;
			break;
		case SharedObjectTypeEnum.ObjectType.View:
			ObjectsCounter.ViewsCount += count;
			break;
		case SharedObjectTypeEnum.ObjectType.Procedure:
			ObjectsCounter.ProceduresCount += count;
			break;
		case SharedObjectTypeEnum.ObjectType.Function:
			ObjectsCounter.FunctionsCount += count;
			break;
		}
	}

	public void ReadCount(DbDataReader reader)
	{
		int num = Convert.ToInt32(reader["count"]);
		SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(PrepareValue.ToString(reader["object_type"]));
		if (objectType.HasValue)
		{
			switch (objectType.GetValueOrDefault())
			{
			case SharedObjectTypeEnum.ObjectType.Table:
				ObjectsCounter.TablesCount += num;
				break;
			case SharedObjectTypeEnum.ObjectType.View:
				ObjectsCounter.ViewsCount += num;
				break;
			case SharedObjectTypeEnum.ObjectType.Procedure:
				ObjectsCounter.ProceduresCount += num;
				break;
			case SharedObjectTypeEnum.ObjectType.Function:
				ObjectsCounter.FunctionsCount += num;
				break;
			}
		}
	}

	public void ReadCount(Row reader)
	{
		int num = Convert.ToInt32(reader["count"]);
		SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(PrepareValue.ToString(reader.Field<string>("object_type")));
		if (objectType.HasValue)
		{
			switch (objectType.GetValueOrDefault())
			{
			case SharedObjectTypeEnum.ObjectType.Table:
				ObjectsCounter.TablesCount += num;
				break;
			case SharedObjectTypeEnum.ObjectType.View:
				ObjectsCounter.ViewsCount += num;
				break;
			case SharedObjectTypeEnum.ObjectType.Procedure:
				ObjectsCounter.ProceduresCount += num;
				break;
			case SharedObjectTypeEnum.ObjectType.Function:
				ObjectsCounter.FunctionsCount += num;
				break;
			}
		}
	}

	private bool IsCassandraBasedType(SharedDatabaseTypeEnum.DatabaseType? type)
	{
		if (type == SharedDatabaseTypeEnum.DatabaseType.Cassandra || type == SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra || type == SharedDatabaseTypeEnum.DatabaseType.AmazonKeyspaces)
		{
			return true;
		}
		return false;
	}

	private bool IsCassandraRowWithPositionCheck(SharedDatabaseTypeEnum.DatabaseType? type)
	{
		if (type == SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra || type == SharedDatabaseTypeEnum.DatabaseType.AmazonKeyspaces || type == SharedDatabaseTypeEnum.DatabaseType.Astra)
		{
			return true;
		}
		return false;
	}
}
