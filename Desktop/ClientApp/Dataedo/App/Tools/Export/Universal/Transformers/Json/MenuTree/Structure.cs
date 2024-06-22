using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Naming;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;

internal class Structure : IMenuTreeItem
{
	internal IStructure Model;

	public IMenuTreeItem Parent { get; private set; }

	public string Id
	{
		get
		{
			if (!(Parent is Module))
			{
				return $"s{Model.Id}";
			}
			return $"{Parent.Id}s{Model.Id}";
		}
	}

	public string ObjectId => $"s{Model.Id}";

	public string MenuType
	{
		get
		{
			if (!(Parent is Module))
			{
				return "structure";
			}
			return "module_structure";
		}
	}

	public string MenuSubtype => SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Structure, Model.Subtype);

	public bool IsUserDefined => Model.Source == UserTypeEnum.UserType.USER;

	public string MenuName => NameBuilder.For(Model).Full().Build(null, (Parent is Module) ? (Parent as Module).Model.Documentation.ShowSchemaEffective : (Parent as Documentation).Model.ShowSchemaEffective);

	public IEnumerable<string> ColumnNames => (from x in ObjectTools.FlattenColumns(Model?.Columns)
		select x.DisplayNameWithoutPath).Distinct();

	public IEnumerable<IMenuTreeItem> MenuChildren => null;

	public Structure(IStructure model, IMenuTreeItem parent)
	{
		Model = model;
		Parent = parent;
	}
}
