using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json;

internal static class DataConverter
{
	private static IDictionary<Type, Type> menuTreeTypes = new Dictionary<Type, Type>
	{
		{
			typeof(IDocumentation),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Documentation)
		},
		{
			typeof(IModule),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Module)
		},
		{
			typeof(ITable),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Table)
		},
		{
			typeof(IView),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.View)
		},
		{
			typeof(IProcedure),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Procedure)
		},
		{
			typeof(IFunction),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Function)
		},
		{
			typeof(IStructure),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Structure)
		},
		{
			typeof(IBusinessGlossary),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.BusinessGlossary)
		},
		{
			typeof(ITerm),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Term)
		}
	};

	private static IDictionary<Type, Type> objectTypes = new Dictionary<Type, Type>
	{
		{
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Documentation),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Documentation)
		},
		{
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Modules),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Modules)
		},
		{
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Module),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Module)
		},
		{
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Tables),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Tables)
		},
		{
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Table),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Table)
		},
		{
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Views),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Views)
		},
		{
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.View),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.View)
		},
		{
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Procedures),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Procedures)
		},
		{
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Procedure),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Procedure)
		},
		{
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Functions),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Functions)
		},
		{
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Function),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Function)
		},
		{
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Structures),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Structures)
		},
		{
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Structure),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Structure)
		},
		{
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.BusinessGlossary),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.BusinessGlossary)
		},
		{
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Terms),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Terms)
		},
		{
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Term),
			typeof(Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Term)
		}
	};

	public static IEnumerable<IMenuTreeItem> ToMenuTree(IEnumerable<IModel> models, IMenuTreeItem parent = null)
	{
		return models.Select((IModel x) => ToMenuTree(x, parent));
	}

	public static IMenuTreeItem ToMenuTree(IModel model, IMenuTreeItem parent = null)
	{
		Type type = model.GetType();
		foreach (Type key in menuTreeTypes.Keys)
		{
			if (key.IsAssignableFrom(type))
			{
				Type type2 = menuTreeTypes[key];
				object[] args = new object[2] { model, parent };
				return (IMenuTreeItem)Activator.CreateInstance(type2, args);
			}
		}
		throw new Exception($"Cannot convert type '{model.GetType()}' to menu tree item.");
	}

	public static IEnumerable<IMenuTreeItem> FlattenMenuTree(IEnumerable<IMenuTreeItem> items)
	{
		if (items == null)
		{
			return new List<IMenuTreeItem>();
		}
		List<IMenuTreeItem> list = new List<IMenuTreeItem>(items);
		foreach (IEnumerable<IMenuTreeItem> item in items.Select((IMenuTreeItem x) => x.MenuChildren))
		{
			list.AddRange(FlattenMenuTree(item));
		}
		List<IMenuTreeItem> list2 = new List<IMenuTreeItem>();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (IMenuTreeItem item2 in list)
		{
			if (!hashSet.Contains(item2.ObjectId))
			{
				hashSet.Add(item2.ObjectId);
				list2.Add(item2);
			}
		}
		return list2;
	}

	public static IEnumerable<IMenuTreeItem> FlattenMenuTree(IMenuTreeItem item)
	{
		return new List<IMenuTreeItem> { item };
	}

	public static IObject ToObject(IMenuTreeItem model)
	{
		Type type = model.GetType();
		foreach (Type key in objectTypes.Keys)
		{
			if (key.IsAssignableFrom(type))
			{
				Type type2 = objectTypes[key];
				object[] args = new object[1] { model };
				return (IObject)Activator.CreateInstance(type2, args);
			}
		}
		throw new Exception($"Cannot convert type '{model.GetType()}' to object.");
	}
}
