using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Naming;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;

internal class Table : IMenuTreeItem
{
	internal ITable Model;

	public IMenuTreeItem Parent { get; private set; }

	public string Id
	{
		get
		{
			if (!(Parent is Module))
			{
				return $"t{Model.Id}";
			}
			return $"{Parent.Id}t{Model.Id}";
		}
	}

	public string ObjectId => $"t{Model.Id}";

	public string MenuType
	{
		get
		{
			if (!(Parent is Module))
			{
				return "table";
			}
			return "module_table";
		}
	}

	public string MenuSubtype => SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Table, Model.Subtype);

	public bool IsUserDefined => Model.Source == UserTypeEnum.UserType.USER;

	public string MenuName => NameBuilder.For(Model).Full().Build(null, (Parent is Module) ? (Parent as Module).Model.Documentation.ShowSchemaEffective : (Parent as Documentation).Model.ShowSchemaEffective);

	public IEnumerable<string> ColumnNames => (from x in ObjectTools.FlattenColumns(Model?.Columns)
		select x.DisplayNameWithoutPath).Distinct();

	public IEnumerable<IMenuTreeItem> MenuChildren => null;

	public Table(ITable model, IMenuTreeItem parent)
	{
		Model = model;
		Parent = parent;
	}
}
