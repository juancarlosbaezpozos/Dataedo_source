using System;

namespace Dataedo.App.Tools.Export.Universal.ProgressTrackers;

internal class BackgroundWorkerTracker : IProgressTracker
{
	private IBackgroundProcessingWorkerReporting worker;

	public BackgroundWorkerTracker(IBackgroundProcessingWorkerReporting worker)
	{
		this.worker = worker;
	}

	public void OnProgress(int percentage)
	{
		worker.ReportTotalProgress(percentage);
	}

	public void OnSubProgress(int percentage)
	{
		worker.ReportProgress(percentage);
	}

	public void Log(string log)
	{
		worker.ReportProgress(log);
		SubLog(string.Empty);
	}

	public void SubLog(string log)
	{
		worker.SetCurrentObject(log);
	}

	public bool ShouldCancel()
	{
		return worker.IsCancelled;
	}

	public void ThrowIfCanceled()
	{
		if (ShouldCancel())
		{
			throw new OperationCanceledException();
		}
	}

	public void Started()
	{
		worker.ReportTotalProgress(0);
		worker.ReportProgress(0);
	}

	public void Done()
	{
		worker.ReportTotalProgress(0);
		worker.ReportProgress(100);
		worker.HasResult = true;
	}
}
