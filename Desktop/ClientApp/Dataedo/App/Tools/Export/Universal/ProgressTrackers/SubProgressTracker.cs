using System;

namespace Dataedo.App.Tools.Export.Universal.ProgressTrackers;

internal class SubProgressTracker : IProgressTracker
{
	private IProgressTracker progress;

	private int from;

	private int to;

	public SubProgressTracker(IProgressTracker progress = null, int from = 0, int to = 0)
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
	}

	public void OnProgress(int percentage)
	{
		progress?.OnProgress(SubPercentage(percentage));
		progress?.OnSubProgress(percentage);
	}

	public void OnSubProgress(int percentage)
	{
		throw new Exception("Sub progress tracker supports only one level of progress reporting.");
	}

	public void Log(string text)
	{
		progress?.SubLog(text);
	}

	public void SubLog(string log)
	{
		throw new Exception("Sub progress tracker supports only one level of progress reporting.");
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
	}
}
