using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base.HierarchyModel;

public class ComputerItem : Item
{
	public override Image Image
	{
		get
		{
			return null;
		}
		protected set
		{
		}
	}

	public ComputerItem(string name)
		: base(name)
	{
	}

	public override List<Item> GetChildItems()
	{
		List<Item> list = new List<Item>();
		list.Add(new DirectoryItem(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)));
		list.Add(new DirectoryItem(Environment.GetFolderPath(Environment.SpecialFolder.Personal)));
		list.Add(new DirectoryItem(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)));
		list.Add(new DirectoryItem(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)));
		list.Add(new DirectoryItem(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)));
		list.AddRange(from x in Environment.GetLogicalDrives()
			select new DriveItem(x));
		return list;
	}
}
