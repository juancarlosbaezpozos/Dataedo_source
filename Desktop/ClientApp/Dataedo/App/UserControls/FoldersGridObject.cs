using Dataedo.Shared.Enums;

namespace Dataedo.App.UserControls;

public class FoldersGridObject
{
	public string FolderName { get; set; }

	public SharedImportFolderEnum.ImportFolder? ImportFolder { get; set; }

	public bool IsAllSources { get; set; }
}
