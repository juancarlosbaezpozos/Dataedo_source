using System.Collections.Generic;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Naming;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;

internal class Procedure : IMenuTreeItem
{
	internal IProcedure Model;

	public IMenuTreeItem Parent { get; private set; }

	public string Id
	{
		get
		{
			if (!(Parent is Module))
			{
				return $"p{Model.Id}";
			}
			return $"{Parent.Id}p{Model.Id}";
		}
	}

	public string ObjectId => $"p{Model.Id}";

	public string MenuType
	{
		get
		{
			if (!(Parent is Module))
			{
				return "procedure";
			}
			return "module_procedure";
		}
	}

	public string MenuSubtype => SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Procedure, Model.Subtype);

	public bool IsUserDefined => Model.Source == UserTypeEnum.UserType.USER;

	public string MenuName => NameBuilder.For(Model).Full().Build(null, (Parent is Module) ? (Parent as Module).Model.Documentation.ShowSchemaEffective : (Parent as Documentation).Model.ShowSchemaEffective);

	public IEnumerable<string> ColumnNames => null;

	public IEnumerable<IMenuTreeItem> MenuChildren => null;

	public Procedure(IProcedure model, IMenuTreeItem parent)
	{
		Model = model;
		Parent = parent;
	}
}
