using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base.HierarchyModel;

public class DirectoryItem : Item
{
	public DirectoryItem(string name)
		: base(name)
	{
	}

	public override List<Item> GetChildItems()
	{
		List<Item> list = new List<Item>();
		try
		{
			if (Directory.Exists(base.FullName))
			{
				string[] directories = Directory.GetDirectories(base.FullName);
				list.AddRange(from x in directories.Where(delegate(string x)
					{
						try
						{
							return (File.GetAttributes(x) & FileAttributes.Hidden) != FileAttributes.Hidden;
						}
						catch
						{
							return false;
						}
					})
					select new DirectoryItem(x));
				return list;
			}
			return list;
		}
		catch
		{
			return list;
		}
	}

	protected override string GetName(string fullName)
	{
		return Path.GetFileName(fullName);
	}
}
