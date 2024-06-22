using System.Drawing;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Support;

public class ConnectionTypeItem
{
	public string Key { get; set; }

	public string Name { get; set; }

	public string Description { get; set; }

	public Bitmap Icon { get; set; }

	public ConnectionTypeItem(string key, string name, Bitmap icon)
	{
		Key = key;
		Name = name;
		Icon = icon;
	}

	public ConnectionTypeItem(string key, string name, string description, Bitmap icon)
		: this(key, name, icon)
	{
		Description = description;
	}
}
