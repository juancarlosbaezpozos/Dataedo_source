using System;
using System.Collections.Generic;

namespace Dataedo.App.Data.EventArgsDef;

public class BackgroundWorkerProgressEventArgs : EventArgs
{
	public int Progress { get; set; }

	public List<string> Messages { get; set; }

	public double Max { get; set; }

	public int ProgressCalculated { get; set; }

	public int LastProgressCalculated { get; set; }

	public BackgroundWorkerProgressEventArgs(int progress, List<string> messages)
	{
		Progress = progress;
		Messages = messages;
	}

	public BackgroundWorkerProgressEventArgs()
	{
		Progress = 0;
	}

	private void CalculateProgress()
	{
		LastProgressCalculated = ProgressCalculated;
		ProgressCalculated = ((Max != 0.0) ? ((int)((double)(Progress * 100) / Max)) : 0);
	}

	public bool IncrementProgress()
	{
		CalculateProgress();
		Progress++;
		return ProgressCalculated != LastProgressCalculated;
	}

	public bool IncrementProgress(int steps)
	{
		CalculateProgress();
		Progress += steps;
		return ProgressCalculated != LastProgressCalculated;
	}

	public List<string> SetMessage(List<string> messages)
	{
		Messages = messages;
		return Messages;
	}

	public List<string> SetMessage(string message)
	{
		Messages = new List<string> { message };
		return Messages;
	}

	public void Clear()
	{
		int num2 = (LastProgressCalculated = 0);
		int num5 = (Progress = (ProgressCalculated = num2));
		Max = 0.0;
		Messages = new List<string>
		{
			string.Empty,
			string.Empty
		};
	}
}
