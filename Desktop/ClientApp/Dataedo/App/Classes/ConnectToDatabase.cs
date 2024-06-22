using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Common.WaitFormCanceling;
using Dataedo.App.Data;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.General;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Log.Execution;

namespace Dataedo.App.Classes;

public class ConnectToDatabase
{
	private BackgroundWorker databaseConnectBackgroundWorker;

	private BackgroundWorkerManager backgroundWorkerManager;

	private Locker connectingLocker;

	private DatabaseRow databaseRow;

	public bool IsBusy => databaseConnectBackgroundWorker.IsBusy;

	public event EventHandler UpdateProgressEvent;

	public event EventHandler FinishedEvent;

	public ConnectToDatabase()
	{
		databaseConnectBackgroundWorker = new BackgroundWorker();
		backgroundWorkerManager = new BackgroundWorkerManager(databaseConnectBackgroundWorker);
		databaseConnectBackgroundWorker.DoWork += databaseConnectBackgroundWorker_DoWork;
		databaseConnectBackgroundWorker.RunWorkerCompleted += databaseConnectBackgroundWorker_RunWorkerCompleted;
		databaseConnectBackgroundWorker.ProgressChanged += databaseConnectBackgroundWorker_ProgressChanged;
		databaseConnectBackgroundWorker.WorkerReportsProgress = true;
		databaseConnectBackgroundWorker.WorkerSupportsCancellation = true;
		connectingLocker = new Locker();
	}

	private void databaseConnectBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		this.UpdateProgressEvent?.Invoke(null, new BackgroundWorkerProgressEventArgs(e.ProgressPercentage, e.UserState as List<string>));
	}

	public void Cancel()
	{
		if (databaseConnectBackgroundWorker.IsBusy)
		{
			backgroundWorkerManager.ReportProgress("Canceling");
			connectingLocker.IsCanceled = true;
		}
	}

	public void TryConnectAndRead(ref DatabaseRow databaseRow, bool fullImport, bool updateEntireDocumentation, DocumentationCustomFieldRow[] customFields, bool isDbAdded, Form owner)
	{
		this.databaseRow = databaseRow;
		ConnectionParameters argument = new ConnectionParameters
		{
			DatabaseRow = databaseRow,
			DbConnectionLocker = connectingLocker,
			FullImport = fullImport,
			UpdateEntireDocumentation = updateEntireDocumentation,
			CustomFields = customFields,
			IsDbAdded = isDbAdded,
			Owner = owner
		};
		backgroundWorkerManager.Clear();
		databaseConnectBackgroundWorker.RunWorkerAsync(argument);
	}

	private void databaseConnectBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
	{
		backgroundWorkerManager.ReportProgress("Counting objects to import");
		ConnectionParameters connectionParameters = e.Argument as ConnectionParameters;
		connectionParameters.DatabaseRow.tableRows = new ObservableCollection<ObjectRow>();
		SynchronizeParameters synchronizeParameters = new SynchronizeParameters
		{
			DatabaseRow = connectionParameters.DatabaseRow,
			DbSynchLocker = connectionParameters.DbConnectionLocker,
			FullImport = connectionParameters.FullImport,
			UpdateEntireDocumentation = connectionParameters.UpdateEntireDocumentation,
			CustomFields = connectionParameters.CustomFields,
			IsDbAdded = connectionParameters.IsDbAdded,
			Owner = connectionParameters.Owner
		};
		SynchronizeDatabase synchronizeModel = DatabaseSupportFactory.GetDatabaseSupport(connectionParameters.DatabaseRow.Type).GetSynchronizeModel(synchronizeParameters);
		synchronizeModel.SetBackgroundWorkerManager(backgroundWorkerManager);
		List<SynchronizeDatabase.ImportQuery> list = synchronizeModel.CountObjectsQuery();
		bool flag = true;
		QueryViewer.View("Count objects query", synchronizeModel.CombineQueries(list));
		synchronizeModel.ObjectsCounter = new ObjectsCounter();
		Stopwatch stopwatch = ExecutionLog.StartStopwatch();
		foreach (SynchronizeDatabase.ImportQuery item in list)
		{
			if (!synchronizeModel.CountObjects(item, backgroundWorkerManager, connectionParameters?.Owner))
			{
				flag = false;
				SetProgressLength(synchronizeModel.ObjectsCounter);
			}
		}
		ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP SYNCHRONIZATION CountObjects", null, null, 2, 1);
		if (flag)
		{
			SetProgressLength(synchronizeModel.ObjectsCounter);
			connectionParameters.DatabaseRow.ConnectAndSynchronizeState = synchronizeModel.SetObjects(backgroundWorkerManager);
		}
		else
		{
			connectionParameters.DatabaseRow.ConnectAndSynchronizeState = SynchConnectStateEnum.SynchConnectStateType.Error;
		}
		e.Result = connectionParameters;
	}

	private void SetProgressLength(ObjectsCounter objectsCounter)
	{
		if (objectsCounter != null)
		{
			backgroundWorkerManager.SetMaxProgress(objectsCounter.CountMaxForConnecting(databaseRow.Type));
		}
	}

	private void databaseConnectBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		ConnectionParameters connectionParameters = e.Result as ConnectionParameters;
		bool isSuccessful = false;
		if (e.Cancelled)
		{
			connectionParameters.DatabaseRow.ConnectAndSynchronizeState = SynchConnectStateEnum.SynchConnectStateType.Canceled;
		}
		else if (e.Error != null)
		{
			GeneralExceptionHandling.Handle(e.Error, "Database connection error", connectionParameters.Owner);
			connectionParameters.DatabaseRow.ConnectAndSynchronizeState = SynchConnectStateEnum.SynchConnectStateType.Error;
		}
		else if (connectionParameters.DatabaseRow.ConnectAndSynchronizeState != SynchConnectStateEnum.SynchConnectStateType.Error && connectionParameters.DatabaseRow.ConnectAndSynchronizeState != SynchConnectStateEnum.SynchConnectStateType.Canceled)
		{
			isSuccessful = true;
		}
		this.FinishedEvent?.Invoke(null, new ConnectionStatusEventArgs(isSuccessful));
	}
}
