using System;
using Dataedo.App.DataProfiling.Models;

namespace Dataedo.App.DataProfiling.Tools;

public class ProfilingProgress : Progress<(double, INavigationObject)>
{
	private int numberOfStepsInColumnProfiling;

	private int numberOfStepsInTableProfiling = 2;

	private int numberOfTablesToBeProfiled;

	private int numberOfColumnsToBeProfiled;

	private int numberOfProfiledTables;

	private int numberOfProfiledColumns;

	private double progressPerObject;

	private double currentProgress;

	private INavigationObject profiledObject;

	private DateTime? lastReportTime;

	private int numberOfProfiledObjects => numberOfProfiledTables + numberOfProfiledColumns;

	private double progressPerColumnProfilingStep => progressPerObject / (double)numberOfStepsInColumnProfiling;

	private double progressPerTableProfilingStep => progressPerObject / (double)numberOfStepsInTableProfiling;

	public void Init(int numberOfProfiledTables, int numberOfProfiledColumns)
	{
		numberOfTablesToBeProfiled = numberOfProfiledTables;
		numberOfColumnsToBeProfiled = numberOfProfiledColumns;
		progressPerObject = 100.0 / (double)(numberOfTablesToBeProfiled + numberOfColumnsToBeProfiled);
	}

	public void ObjectProfilingStarted(INavigationObject profiledObject)
	{
		this.profiledObject = profiledObject;
	}

	public void SetNumberOfStepsInColumnProfiling(int number)
	{
		numberOfStepsInColumnProfiling = number;
	}

	public void ObjectProfilingEnded()
	{
		if (profiledObject is TableNavigationObject)
		{
			numberOfProfiledTables++;
		}
		else if (profiledObject is ColumnNavigationObject)
		{
			numberOfProfiledColumns++;
		}
		RefreshCurrentProgress();
	}

	private void RefreshCurrentProgress()
	{
		currentProgress = (double)numberOfProfiledObjects * progressPerObject;
		ReportProgress(forceReport: true);
	}

	public void ObjectProfilingStepEnded()
	{
		if (profiledObject is TableNavigationObject)
		{
			currentProgress += progressPerTableProfilingStep;
		}
		else if (profiledObject is ColumnNavigationObject)
		{
			currentProgress += progressPerColumnProfilingStep;
		}
		ReportProgress();
	}

	public void SkipTableProfiling()
	{
		currentProgress += progressPerTableProfilingStep;
		numberOfProfiledTables++;
		ReportProgress();
	}

	public void SkipColumnsProfiling(int numberOfSkippedColumns)
	{
		currentProgress += progressPerColumnProfilingStep * (double)numberOfSkippedColumns;
		numberOfProfiledColumns += numberOfSkippedColumns;
		ReportProgress();
	}

	private void ReportProgress(bool forceReport = false)
	{
		DateTime now = DateTime.Now;
		if (forceReport || !lastReportTime.HasValue)
		{
			((IProgress<(double, INavigationObject)>)this).Report((currentProgress, profiledObject));
			lastReportTime = now;
		}
		else if (lastReportTime.HasValue && (now - lastReportTime.Value).TotalMilliseconds > 150.0)
		{
			((IProgress<(double, INavigationObject)>)this).Report((currentProgress, profiledObject));
			lastReportTime = now;
		}
	}
}
