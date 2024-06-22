namespace Dataedo.App.Tools;

public class ModuleModel
{
	private int id;

	public bool IsSelected { get; set; }

	public string DisplayName { get; private set; }

	public ModuleModel(string displayName)
	{
		DisplayName = displayName;
	}
}
