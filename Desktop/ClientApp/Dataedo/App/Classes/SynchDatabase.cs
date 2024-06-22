using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Common.WaitFormCanceling;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.Log.Execution;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes;

public class SynchDatabase
{
	private BackgroundWorker synchronizeBackgroundWorker;

	private BackgroundWorkerManager backgroundWorkerManager;

	private SynchronizeParameters synchronizeParameters;

	private Locker dbSynchLocker;

	private DateTime? startDate;

	public bool IsBusy => synchronizeBackgroundWorker?.IsBusy ?? false;

	public bool? IsSynchronizationRequired { get; set; }

	public event EventHandler UpdateProgressEvent;

	public event EventHandler SynchronizationFinishedEvent;

	private void InitializeObjects(BackgroundWorkerManager customBackgroundWorkerManager = null)
	{
		if (customBackgroundWorkerManager == null)
		{
			synchronizeBackgroundWorker = new BackgroundWorker();
			backgroundWorkerManager = new BackgroundWorkerManager(synchronizeBackgroundWorker);
		}
		else
		{
			backgroundWorkerManager = customBackgroundWorkerManager;
			synchronizeBackgroundWorker = backgroundWorkerManager.Worker;
		}
		synchronizeBackgroundWorker.DoWork += synchronizeBackgroundWorker_DoWork;
		synchronizeBackgroundWorker.RunWorkerCompleted += synchronizeBackgroundWorker_RunWorkerCompleted;
		synchronizeBackgroundWorker.ProgressChanged += synchronizeBackgroundWorker_ProgressChanged;
		synchronizeBackgroundWorker.WorkerReportsProgress = true;
		synchronizeBackgroundWorker.WorkerSupportsCancellation = true;
		synchronizeParameters = new SynchronizeParameters();
		dbSynchLocker = new Locker();
	}

	public void Cancel()
	{
		BackgroundWorker backgroundWorker = synchronizeBackgroundWorker;
		if (backgroundWorker != null && backgroundWorker.IsBusy)
		{
			backgroundWorkerManager.ReportProgress("Canceling");
			dbSynchLocker.IsCanceled = true;
		}
	}

	public bool? Synchronize(DatabaseRow selectedDatabase, bool fullImport, bool updateEntireDocumentation, bool isDbAdded, DocumentationCustomFieldRow[] customFields, int updateId, BackgroundWorkerManager customBackgroundWorkerManager = null, bool importDependencies = false)
	{
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		InitializeObjects(customBackgroundWorkerManager);
		if (!synchronizeBackgroundWorker.IsBusy)
		{
			synchronizeParameters.DatabaseRow = selectedDatabase;
			synchronizeParameters.FullImport = fullImport;
			synchronizeParameters.IsDbAdded = isDbAdded;
			synchronizeParameters.UpdateEntireDocumentation = updateEntireDocumentation;
			synchronizeParameters.CustomFields = customFields;
			synchronizeParameters.UpdateId = updateId;
			synchronizeParameters.ImportDependencies = importDependencies;
			if (synchronizeParameters.DatabaseRow.tableRows == null)
			{
				return false;
			}
			IEnumerable<ObjectRow> source = synchronizeParameters.DatabaseRow.tableRows?.Where((ObjectRow x) => x.IsNotSynchronized);
			synchronizeParameters.SynchObjectsCount = source.Where((ObjectRow x) => x.ToSynchronize).Count();
			IsSynchronizationRequired = synchronizeParameters.SynchObjectsCount > 0;
			synchronizeParameters.DbSynchLocker = dbSynchLocker;
			if (synchronizeParameters.SynchObjectsCount > 0)
			{
				backgroundWorkerManager.SetMaxProgress(synchronizeParameters.AllObjectsCount);
				synchronizeBackgroundWorker.RunWorkerAsync(synchronizeParameters);
			}
			return synchronizeParameters.SynchObjectsCount > 0;
		}
		ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP", null, null, 2, 1);
		return null;
	}

	private void synchronizeBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
	{
		startDate = DateTime.UtcNow;
		SynchronizeParameters synchronizeParameters = e.Argument as SynchronizeParameters;
		synchronizeParameters.DatabaseRow.ConnectAndSynchronizeState = SynchConnectStateEnum.SynchConnectStateType.Synchronize;
		DatabaseSupportFactory.GetDatabaseSupport(synchronizeParameters.DatabaseRow.Type).GetSynchronizeModel(synchronizeParameters).Synchronize(backgroundWorkerManager);
		backgroundWorkerManager.SetMessage("Rebuilding dictionaries");
		DB.CustomField.UpdateCustomFieldsAfterSynchronization(synchronizeParameters.CustomFields);
		if (synchronizeParameters.DatabaseRow.Id.HasValue)
		{
			DB.Database.UpdateDocumentationShowSchemaFlag(synchronizeParameters.DatabaseRow.Id.Value, synchronizeParameters?.Owner);
			DB.Community.InsertFollowingToRepository(SharedObjectTypeEnum.ObjectType.Database, synchronizeParameters.DatabaseRow.Id);
		}
		e.Result = synchronizeParameters;
	}

	private void synchronizeBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		this.UpdateProgressEvent?.Invoke(null, new BackgroundWorkerProgressEventArgs(e.ProgressPercentage, e.UserState as List<string>));
	}

	private void synchronizeBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		SynchronizeParameters synchronizeParameters = e.Result as SynchronizeParameters;
		bool isSuccessful = false;
		if (e.Cancelled || synchronizeParameters.DbSynchLocker.IsCanceled)
		{
			synchronizeParameters.DatabaseRow.ConnectAndSynchronizeState = SynchConnectStateEnum.SynchConnectStateType.Canceled;
		}
		else if (e.Error != null)
		{
			GeneralExceptionHandling.Handle(e.Error, "Synchronization error", synchronizeParameters?.Owner);
			synchronizeParameters.DatabaseRow.ConnectAndSynchronizeState = SynchConnectStateEnum.SynchConnectStateType.Error;
		}
		else if (synchronizeParameters.DatabaseRow.ConnectAndSynchronizeState != SynchConnectStateEnum.SynchConnectStateType.Error && synchronizeParameters.DatabaseRow.ConnectAndSynchronizeState != SynchConnectStateEnum.SynchConnectStateType.Canceled)
		{
			isSuccessful = true;
			foreach (IGrouping<string, ObjectRow> item in synchronizeParameters.DatabaseRow.tableRows.Where((ObjectRow x) => x.StateBeforeSynchronization == SynchronizeStateEnum.SynchronizeState.New || (x.ToSynchronize && x.StateBeforeSynchronization == SynchronizeStateEnum.SynchronizeState.Ignored)).ToList().ToLookup((ObjectRow x) => x.ObjectName.ToLower()))
			{
				item.FirstOrDefault().AddNewObjectToTree();
			}
			synchronizeParameters.DatabaseRow.tableRows.Where((ObjectRow x) => !x.ToSynchronize && x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.New).ToList().ForEach(delegate(ObjectRow x)
			{
				x.SynchronizeState = SynchronizeStateEnum.SynchronizeState.Ignored;
			});
			synchronizeParameters.DatabaseRow.ConnectAndSynchronizeState = SynchConnectStateEnum.SynchConnectStateType.Synchronize;
		}
		synchronizeParameters.DbSynchLocker.IsCanceled = false;
		this.SynchronizationFinishedEvent?.Invoke(null, new ConnectionStatusEventArgs(isSuccessful));
		synchronizeBackgroundWorker.Dispose();
		backgroundWorkerManager.BackgroundWorkerRunCompletedResetEvent.Set();
		TrackingRunner.Track(delegate
		{
			Track(isSuccessful);
		});
		startDate = null;
	}

	private void Track(bool isSuccessful)
	{
		DateTime utcNow = DateTime.UtcNow;
		DateTime? dateTime = startDate;
		string text = (utcNow - dateTime)?.TotalSeconds.ToString();
		if (isSuccessful)
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoConnectionBuilderEventSpecificTimeImportedObjectsImportedTables(new TrackingConnectionParameters(synchronizeParameters.DatabaseRow, synchronizeParameters.DatabaseRow.Type, SSLTypeHelper.GetSelectedSSLType(synchronizeParameters.DatabaseRow), ConnectionTypeService.GetConnectionType(synchronizeParameters.DatabaseRow)), new TrackingUserParameters(), new TrackingDataedoParameters(), text.ToString(), synchronizeParameters.DatabaseRow.tableRows.Count((ObjectRow x) => x.ToSynchronize).ToString(), synchronizeParameters.DatabaseRow.tableRows.Count((ObjectRow x) => x.ObjectTypeValue == SharedObjectTypeEnum.ObjectType.Table && x.ToSynchronize).ToString()), synchronizeParameters.IsDbAdded ? TrackingEventEnum.ImportFinished : TrackingEventEnum.UpdateFinished);
		}
		else
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoConnectionBuilderEventSpecificTime(new TrackingConnectionParameters(synchronizeParameters.DatabaseRow, synchronizeParameters.DatabaseRow.Type, SSLTypeHelper.GetSelectedSSLType(synchronizeParameters.DatabaseRow), ConnectionTypeService.GetConnectionType(synchronizeParameters.DatabaseRow)), new TrackingUserParameters(), new TrackingDataedoParameters(), text.ToString()), synchronizeParameters.IsDbAdded ? TrackingEventEnum.ImportFailed : TrackingEventEnum.UpdateFailed);
		}
	}
}
