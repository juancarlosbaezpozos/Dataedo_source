using System.Collections.Generic;
using Dataedo.App.DataRepository.Models;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;

internal class Module : IMenuTreeItem
{
	internal IModule Model;

	public IMenuTreeItem Parent { get; private set; }

	public string Id => $"m{Model.Id}";

	public string ObjectId => Id;

	public string MenuType => "module";

	public string MenuSubtype => null;

	public bool IsUserDefined => true;

	public string MenuName => Model.Title;

	public IEnumerable<string> ColumnNames => null;

	public IEnumerable<IMenuTreeItem> MenuChildren => Children();

	public Module(IModule model, IMenuTreeItem parent)
	{
		Model = model;
		Parent = parent;
	}

	private IEnumerable<IMenuTreeItem> Children()
	{
		List<IMenuTreeItem> list = new List<IMenuTreeItem>();
		if (Model.Tables != null && Model.Tables.Count > 0)
		{
			Tables tables = new Tables(this)
			{
				Id = Id + "t",
				ObjectId = Id + "t",
				MenuType = "module_tables",
				MenuName = "Tables",
				ColumnNames = null
			};
			tables.MenuChildren = DataConverter.ToMenuTree(Model.Tables, this);
			list.Add(tables);
		}
		if (Model.Views != null && Model.Views.Count > 0)
		{
			Views views = new Views(this)
			{
				Id = Id + "v",
				ObjectId = Id + "v",
				MenuType = "module_views",
				MenuName = "Views",
				ColumnNames = null
			};
			views.MenuChildren = DataConverter.ToMenuTree(Model.Views, this);
			list.Add(views);
		}
		if (Model.Procedures != null && Model.Procedures.Count > 0)
		{
			Procedures procedures = new Procedures(this)
			{
				Id = Id + "p",
				ObjectId = Id + "p",
				MenuType = "module_procedures",
				MenuName = "Procedures",
				ColumnNames = null
			};
			procedures.MenuChildren = DataConverter.ToMenuTree(Model.Procedures, this);
			list.Add(procedures);
		}
		if (Model.Functions != null && Model.Functions.Count > 0)
		{
			Functions functions = new Functions(this)
			{
				Id = Id + "f",
				ObjectId = Id + "f",
				MenuType = "module_functions",
				MenuName = "Functions",
				ColumnNames = null
			};
			functions.MenuChildren = DataConverter.ToMenuTree(Model.Functions, this);
			list.Add(functions);
		}
		if (Model.Structures != null && Model.Structures.Count > 0)
		{
			Structures structures = new Structures(this)
			{
				Id = Id + "s",
				ObjectId = Id + "s",
				MenuType = "module_structures",
				MenuName = "Structures",
				ColumnNames = null
			};
			structures.MenuChildren = DataConverter.ToMenuTree(Model.Structures, this);
			list.Add(structures);
		}
		return list;
	}
}
