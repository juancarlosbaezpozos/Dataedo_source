namespace Dataedo.App.Tools;

public interface IEdit
{
	bool IsEdited { get; set; }

	void SetValue(bool isEdited);

	void SetUnchanged();

	void SetEdited();
}
