using System.Collections.Generic;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Naming;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;

internal class Function : IMenuTreeItem
{
	internal IFunction Model;

	public IMenuTreeItem Parent { get; private set; }

	public string Id
	{
		get
		{
			if (!(Parent is Module))
			{
				return $"f{Model.Id}";
			}
			return $"{Parent.Id}f{Model.Id}";
		}
	}

	public string ObjectId => $"f{Model.Id}";

	public string MenuType
	{
		get
		{
			if (!(Parent is Module))
			{
				return "function";
			}
			return "module_function";
		}
	}

	public string MenuSubtype => SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Function, Model.Subtype);

	public bool IsUserDefined => Model.Source == UserTypeEnum.UserType.USER;

	public string MenuName => NameBuilder.For(Model).Full().Build(null, (Parent is Module) ? (Parent as Module).Model.Documentation.ShowSchemaEffective : (Parent as Documentation).Model.ShowSchemaEffective);

	public IEnumerable<string> ColumnNames => null;

	public IEnumerable<IMenuTreeItem> MenuChildren => null;

	public Function(IFunction model, IMenuTreeItem parent)
	{
		Model = model;
		Parent = parent;
	}
}
