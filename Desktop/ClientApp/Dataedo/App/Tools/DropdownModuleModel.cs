using Dataedo.Model.Data.Modules;

namespace Dataedo.App.Tools;

public class DropdownModuleModel
{
	public string Name { get; set; }

	public string DatabaseName { get; set; }

	public string DisplayName { get; private set; }

	public int? DatabaseId { get; set; }

	public int? ModuleId { get; set; }

	public DropdownModuleModel(ModuleWithoutDescriptionObject row)
	{
		Name = row.Title;
		DatabaseName = row.DatabaseTitle;
		DatabaseId = row.DatabaseId;
		ModuleId = row.ModuleId;
	}

	public void SetDisplayName(int selectedNodeDatabaseId)
	{
		DisplayName = ((DatabaseId != selectedNodeDatabaseId) ? (DatabaseName + "." + Name) : Name);
	}
}
