using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Dataedo.App.Data.EventArgsDef;

namespace Dataedo.App.Tools;

public class BackgroundWorkerManager
{
	private BackgroundWorker bw;

	private BackgroundWorkerProgressEventArgs backgroundWorkerProgressEventArgs;

	public BackgroundWorker Worker => bw;

	public AutoResetEvent BackgroundWorkerRunCompletedResetEvent { get; private set; }

	public BackgroundWorkerManager(BackgroundWorker bw)
	{
		this.bw = bw;
		backgroundWorkerProgressEventArgs = new BackgroundWorkerProgressEventArgs();
		BackgroundWorkerRunCompletedResetEvent = new AutoResetEvent(initialState: false);
	}

	public void SetMaxProgress(int max)
	{
		backgroundWorkerProgressEventArgs.Max = max;
	}

	public int GetMaxProgress()
	{
		return (int)backgroundWorkerProgressEventArgs.Max;
	}

	public void ReportProgress(List<string> messages)
	{
		backgroundWorkerProgressEventArgs.IncrementProgress();
		bw.ReportProgress(backgroundWorkerProgressEventArgs.ProgressCalculated, backgroundWorkerProgressEventArgs.SetMessage(messages));
	}

	public void ReportProgress(string message)
	{
		backgroundWorkerProgressEventArgs.IncrementProgress();
		bw.ReportProgress(backgroundWorkerProgressEventArgs.ProgressCalculated, backgroundWorkerProgressEventArgs.SetMessage(message));
	}

	public void ReportProgress(string message, int steps)
	{
		if (backgroundWorkerProgressEventArgs.IncrementProgress(steps))
		{
			bw.ReportProgress(backgroundWorkerProgressEventArgs.ProgressCalculated, backgroundWorkerProgressEventArgs.SetMessage(message));
		}
	}

	public void SetMessage(List<string> messages)
	{
		bw.ReportProgress(backgroundWorkerProgressEventArgs.ProgressCalculated, backgroundWorkerProgressEventArgs.SetMessage(messages));
	}

	public void SetMessage(string message)
	{
		bw.ReportProgress(backgroundWorkerProgressEventArgs.ProgressCalculated, backgroundWorkerProgressEventArgs.SetMessage(message));
	}

	public void IncrementProgress()
	{
		if (backgroundWorkerProgressEventArgs.IncrementProgress())
		{
			bw.ReportProgress(backgroundWorkerProgressEventArgs.ProgressCalculated, backgroundWorkerProgressEventArgs.Messages);
		}
	}

	public void IncrementProgress(int steps)
	{
		if (backgroundWorkerProgressEventArgs.IncrementProgress(steps))
		{
			bw.ReportProgress(backgroundWorkerProgressEventArgs.ProgressCalculated, backgroundWorkerProgressEventArgs.Messages);
		}
	}

	public void Clear()
	{
		backgroundWorkerProgressEventArgs.Clear();
	}
}
