using System.Collections.Generic;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Naming;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;

internal class Documentation : IMenuTreeItem
{
	internal IDocumentation Model;

	public IMenuTreeItem Parent { get; private set; }

	public string Id => $"d{Model.Id}";

	public string ObjectId => Id;

	public string MenuType => "documentation";

	public string MenuSubtype => null;

	public bool IsUserDefined => false;

	public string MenuName => NameBuilder.For(Model).Full().Build();

	public IEnumerable<string> ColumnNames => null;

	public IEnumerable<IMenuTreeItem> MenuChildren => Children();

	public Documentation(IDocumentation model, IMenuTreeItem parent)
	{
		Model = model;
		Parent = parent;
	}

	private IEnumerable<IMenuTreeItem> Children()
	{
		List<IMenuTreeItem> list = new List<IMenuTreeItem>();
		if (Model.Modules != null && Model.Modules.Count > 0)
		{
			Modules modules = new Modules(this)
			{
				Id = Id + "m",
				ObjectId = Id + "m",
				MenuType = "modules",
				MenuName = "Subject Areas",
				ColumnNames = null
			};
			modules.MenuChildren = DataConverter.ToMenuTree(Model.Modules, this);
			list.Add(modules);
		}
		if (Model.Tables != null && Model.Tables.Count > 0)
		{
			Tables tables = new Tables(this)
			{
				Id = Id + "t",
				ObjectId = Id + "t",
				MenuType = "tables",
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
				MenuType = "views",
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
				MenuType = "procedures",
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
				MenuType = "functions",
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
				MenuType = "structures",
				MenuName = "Structures",
				ColumnNames = null
			};
			structures.MenuChildren = DataConverter.ToMenuTree(Model.Structures, this);
			list.Add(structures);
		}
		return list;
	}
}
