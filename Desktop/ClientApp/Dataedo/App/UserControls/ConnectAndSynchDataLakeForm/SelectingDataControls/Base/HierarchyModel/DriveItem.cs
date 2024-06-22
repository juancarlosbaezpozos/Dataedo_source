using System.IO;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base.HierarchyModel;

public class DriveItem : DirectoryItem
{
	public DriveItem(string name)
		: base(name)
	{
	}

	protected override string GetName(string fullName)
	{
		return fullName.Replace(Path.DirectorySeparatorChar.ToString(), string.Empty);
	}

	protected override string GetDisplayName(string fullName)
	{
		return base.FullName;
	}
}
