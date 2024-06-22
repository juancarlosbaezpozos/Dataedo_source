using System.Collections.Generic;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base.HierarchyModel;

public class RootItem : Item
{
	public RootItem()
		: base("Root")
	{
	}

	public override List<Item> GetChildItems()
	{
		return new List<Item>
		{
			new ComputerItem("Server")
		};
	}
}
