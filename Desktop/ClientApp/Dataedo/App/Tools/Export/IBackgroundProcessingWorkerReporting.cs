namespace Dataedo.App.Tools.Export;

public interface IBackgroundProcessingWorkerReporting
{
	bool HasResult { get; set; }

	bool HasError { get; set; }

	bool IsCancelled { get; }

	void SetProgressStep(float step);

	void SetTotalProgressStep(float step);

	void ReportProgress(string stage);

	void ReportProgress(string stage, int step);

	void ReportProgress(int step);

	void ReportTotalProgress(int step);

	void SetCurrentObject(string currentObject);

	void IncreaseProgress();

	void DivideProgressStep(float divider, bool withTotal = true);

	void SetProgressStepByPercent(float percent, bool withTotal = true);

	void SetPercentCurrentProgressStep(float percent);

	void DivideTotalProgressStep(float divider);

	void SetPercentTotalProgressStep(float percent);

	void SetTotalProgressStepByPercent(float percent);

	void RevertProgressStep(bool withTotal = true);

	void RevertTotalProgressStep(int count = 1);
}
