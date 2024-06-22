using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Naming;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;

internal class View : IMenuTreeItem
{
	internal IView Model;

	public IMenuTreeItem Parent { get; private set; }

	public string Id
	{
		get
		{
			if (!(Parent is Module))
			{
				return $"v{Model.Id}";
			}
			return $"{Parent.Id}v{Model.Id}";
		}
	}

	public string ObjectId => $"v{Model.Id}";

	public string MenuType
	{
		get
		{
			if (!(Parent is Module))
			{
				return "view";
			}
			return "module_view";
		}
	}

	public string MenuSubtype => SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.View, Model.Subtype);

	public bool IsUserDefined => Model.Source == UserTypeEnum.UserType.USER;

	public string MenuName => NameBuilder.For(Model).Full().Build(null, (Parent is Module) ? (Parent as Module).Model.Documentation.ShowSchemaEffective : (Parent as Documentation).Model.ShowSchemaEffective);

	public IEnumerable<string> ColumnNames => (from x in ObjectTools.FlattenColumns(Model?.Columns)
		select x.DisplayNameWithoutPath).Distinct();

	public IEnumerable<IMenuTreeItem> MenuChildren => null;

	public View(IView model, IMenuTreeItem parent)
	{
		Model = model;
		Parent = parent;
	}
}
