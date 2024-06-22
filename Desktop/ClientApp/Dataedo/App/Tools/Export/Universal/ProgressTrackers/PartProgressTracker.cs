using System;

namespace Dataedo.App.Tools.Export.Universal.ProgressTrackers;

internal class PartProgressTracker : IProgressTracker
{
	private IProgressTracker progress;

	private int from;

	private int to;

	public PartProgressTracker(IProgressTracker progress = null, int from = 0, int to = 0)
	{
		this.progress = progress;
		this.from = from;
		this.to = to;
	}

	private int SubPercentage(int percentage)
	{
		return (int)Math.Floor((float)from + (float)(to - from) * ((float)percentage / 100f));
	}

	public void Done()
	{
		progress?.OnProgress(SubPercentage(100));
		progress?.OnSubProgress(100);
	}

	public void OnProgress(int percentage)
	{
		progress?.OnProgress(SubPercentage(percentage));
	}

	public void OnSubProgress(int percentage)
	{
		progress?.OnSubProgress(percentage);
	}

	public void Log(string log)
	{
		progress?.Log(log);
	}

	public void SubLog(string log)
	{
		progress?.SubLog(log);
	}

	public bool ShouldCancel()
	{
		return progress?.ShouldCancel() ?? false;
	}

	public void ThrowIfCanceled()
	{
		progress?.ThrowIfCanceled();
	}

	public void Started()
	{
		progress?.OnProgress(SubPercentage(0));
		progress?.OnSubProgress(0);
	}
}
