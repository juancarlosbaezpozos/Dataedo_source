namespace Dataedo.App.Tools;

public class BasicEdit
{
	public virtual bool IsEdited { get; set; }

	public void SetValue(bool isEdited)
	{
		IsEdited = isEdited;
	}

	public void SetEdited()
	{
		SetValue(isEdited: true);
	}

	public void SetUnchanged()
	{
		SetValue(isEdited: false);
	}
}
