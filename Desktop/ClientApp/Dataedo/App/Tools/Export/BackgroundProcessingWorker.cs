using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;

namespace Dataedo.App.Tools.Export;

public class BackgroundProcessingWorker : IBackgroundProcessingWorkerReporting
{
	private Stack<float> currentProgressSteps;

	private Stack<float> totalProgressSteps;

	private BackgroundWorker worker;

	private Action startAction;

	private Action progressAction;

	private Action finishAction;

	private DateTime exportStartTime;

	private Form owner;

	public string CurrentStage { get; set; }

	public string CurrentObject { get; set; }

	public float CurrentProgress { get; set; }

	public float CurrentProgressStep { get; private set; }

	public float TotalProgress { get; set; }

	public float TotalProgressStep { get; private set; }

	public bool IsCancelled { get; private set; }

	public bool HasResult { get; set; }

	public bool HasError { get; set; }

	public BackgroundProcessingWorker(Form owner = null)
	{
		worker = new BackgroundWorker();
		worker.DoWork += worker_DoWork;
		worker.ProgressChanged += worker_ProgressChanged;
		worker.RunWorkerCompleted += worker_RunWorkerCompleted;
		worker.WorkerReportsProgress = true;
		worker.WorkerSupportsCancellation = true;
		currentProgressSteps = new Stack<float>();
		totalProgressSteps = new Stack<float>();
		this.owner = owner;
	}

	public void SetEventsHandling(Action startAction, Action progressAction, Action finishAction)
	{
		this.startAction = startAction;
		this.progressAction = progressAction;
		this.finishAction = finishAction;
	}

	public void Start()
	{
		worker.RunWorkerAsync();
	}

	public void CancelAsync()
	{
		worker.CancelAsync();
	}

	public void SetProgressStep(float step)
	{
		currentProgressSteps.Push(CurrentProgressStep);
		CurrentProgressStep = step;
	}

	public void SetTotalProgressStep(float step)
	{
		totalProgressSteps.Push(CurrentProgressStep);
		TotalProgressStep = step;
	}

	public void DivideProgressStep(float divider, bool withTotal = true)
	{
		currentProgressSteps.Push(CurrentProgressStep);
		CurrentProgressStep /= divider;
		if (withTotal)
		{
			totalProgressSteps.Push(TotalProgressStep);
			TotalProgressStep /= divider;
		}
	}

	public void SetProgressStepByPercent(float percent, bool withTotal = true)
	{
		currentProgressSteps.Push(CurrentProgressStep);
		CurrentProgressStep *= percent / 100f;
		if (withTotal)
		{
			totalProgressSteps.Push(TotalProgressStep);
			TotalProgressStep *= percent / 100f;
		}
	}

	public void SetPercentCurrentProgressStep(float percent)
	{
		currentProgressSteps.Push(CurrentProgressStep);
		CurrentProgressStep /= 100f / percent;
	}

	public void DivideTotalProgressStep(float divider)
	{
		totalProgressSteps.Push(TotalProgressStep);
		TotalProgressStep /= divider;
	}

	public void SetPercentTotalProgressStep(float percent)
	{
		totalProgressSteps.Push(TotalProgressStep);
		TotalProgressStep /= 100f / percent;
	}

	public void SetTotalProgressStepByPercent(float percent)
	{
		totalProgressSteps.Push(TotalProgressStep);
		TotalProgressStep *= percent / 100f;
	}

	public void RevertProgressStep(bool withTotal = true)
	{
		CurrentProgressStep = currentProgressSteps.Pop();
		if (withTotal)
		{
			RevertTotalProgressStep();
		}
	}

	public void RevertTotalProgressStep(int count = 1)
	{
		for (int i = 1; i <= count; i++)
		{
			if (i == count)
			{
				TotalProgressStep = totalProgressSteps.Pop();
			}
			else
			{
				totalProgressSteps.Pop();
			}
		}
	}

	public void ReportProgress(string stage)
	{
		CurrentStage = stage;
		CommonReportProgress();
	}

	public void ReportProgress(string stage, int step)
	{
		CurrentStage = stage;
		CurrentProgress = step;
		CommonReportProgress();
	}

	public void ReportProgress(int step)
	{
		CurrentProgress = step;
		CommonReportProgress();
	}

	public void ReportTotalProgress(int step)
	{
		TotalProgress = step;
		CommonReportProgress();
	}

	public void SetCurrentObject(string currentObject)
	{
		CurrentObject = currentObject;
		CommonReportProgress();
	}

	public void IncreaseProgress()
	{
		CurrentProgress += CurrentProgressStep;
		if (CurrentProgress > 100f)
		{
			CurrentProgress = 100f;
		}
		TotalProgress += TotalProgressStep;
		if (TotalProgress > 100f)
		{
			TotalProgress = 100f;
		}
		CommonReportProgress();
	}

	public void Cancel()
	{
		if (worker.IsBusy)
		{
			IsCancelled = true;
			CurrentStage = "Canceling...";
			SetCurrentObject(string.Empty);
			worker.ReportProgress(0);
			worker.CancelAsync();
		}
	}

	private void CommonReportProgress()
	{
		if (!worker.CancellationPending)
		{
			worker.ReportProgress(0);
			return;
		}
		throw new OperationCanceledException();
	}

	private void worker_DoWork(object sender, DoWorkEventArgs e)
	{
		BackgroundWorker obj = sender as BackgroundWorker;
		exportStartTime = DateTime.UtcNow;
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingRepoParameters(), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.ExportRun);
		});
		startAction?.Invoke();
		if (obj.CancellationPending)
		{
			e.Cancel = true;
		}
	}

	private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		progressAction?.Invoke();
	}

	private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		if (e.Error != null)
		{
			GeneralExceptionHandling.Handle(e.Error, "Unable to create documentation", owner);
		}
		double duration = (DateTime.UtcNow - exportStartTime).TotalSeconds;
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilderWithEventSpecificTime(new TrackingRepoParameters(), new TrackingDataedoParameters(), new TrackingUserParameters(), duration.ToString()), (e.Error == null && !e.Cancelled) ? TrackingEventEnum.ExportFinished : TrackingEventEnum.ExportFailed);
		});
		finishAction?.Invoke();
	}
}
