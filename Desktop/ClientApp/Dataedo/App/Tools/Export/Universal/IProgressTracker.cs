namespace Dataedo.App.Tools.Export.Universal;

public interface IProgressTracker
{
	void Started();

	void OnProgress(int percentage);

	void OnSubProgress(int percentage);

	void Log(string log);

	void SubLog(string log);

	bool ShouldCancel();

	void ThrowIfCanceled();

	void Done();
}
