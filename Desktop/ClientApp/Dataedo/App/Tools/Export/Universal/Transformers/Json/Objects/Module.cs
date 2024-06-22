using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;
using Dataedo.Shared.Enums;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects;

internal class Module : IObject
{
	internal Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Module MenuTreeItem;

	internal IModule Model;

	[JsonProperty("tables_custom_fields")]
	public IList<string> TablesCustomFields;

	[JsonProperty("tables")]
	public IEnumerable<ItemsTableRow> Tables;

	[JsonProperty("views_custom_fields")]
	public IList<string> ViewsCustomFields;

	[JsonProperty("views")]
	public IEnumerable<ItemsTableRow> Views;

	[JsonProperty("procedures_custom_fields")]
	public IList<string> ProceduresCustomFields;

	[JsonProperty("procedures")]
	public IEnumerable<ItemsTableRow> Procedures;

	[JsonProperty("functions_custom_fields")]
	public IList<string> FunctionsCustomFields;

	[JsonProperty("functions")]
	public IEnumerable<ItemsTableRow> Functions;

	[JsonProperty("structures_custom_fields")]
	public IList<string> StructuresCustomFields;

	[JsonProperty("structures")]
	public IEnumerable<ItemsTableRow> Structures;

	public string ObjectId => MenuTreeItem.ObjectId;

	[JsonProperty("name")]
	public string Name => Model.Title;

	[JsonProperty("description")]
	public string Description => Model.Description;

	[JsonProperty("summary")]
	public IList<SummaryField> Summary { get; private set; }

	[JsonProperty("erd")]
	public string Erd => Model.Erd;

	public Module(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Module menuTreeItem)
	{
		MenuTreeItem = menuTreeItem;
		Model = menuTreeItem.Model;
		SetSummaryTable();
		LoadTables();
		LoadViews();
		LoadProcedures();
		LoadFunctions();
		LoadStructures();
	}

	private void SetSummaryTable()
	{
		Summary = new List<SummaryField>();
		foreach (ICustomField item in Model.CustomFields.Where((ICustomField x) => CustomField.HasValue(x)))
		{
			SummaryFieldCustomFieldValue customField = new SummaryFieldCustomFieldValue(item);
			Summary.Add(new SummaryField(item.Name, customField));
		}
	}

	private void LoadTables()
	{
		IEnumerable<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Table> enumerable = GetChildrenOf<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Tables>().Cast<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Table>();
		TablesCustomFields = CustomField.GetUniqueNamesWithValue(enumerable, (object x) => ((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Table)x).Model.CustomFields);
		Tables = enumerable.Select((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Table x) => new ItemsTableRow
		{
			Id = x.Id,
			Name = x.MenuName,
			Type = SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Table),
			Subtype = SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Table, x.Model.Subtype),
			IsUserDefined = (x.Model.Source == UserTypeEnum.UserType.USER),
			CustomFields = x.Model.CustomFields.ToDictionary((ICustomField m) => m.Name, (ICustomField m) => new CustomField(m))
		});
	}

	private void LoadViews()
	{
		IEnumerable<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.View> enumerable = GetChildrenOf<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Views>().Cast<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.View>();
		ViewsCustomFields = CustomField.GetUniqueNamesWithValue(enumerable, (object x) => ((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.View)x).Model.CustomFields);
		Views = enumerable.Select((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.View x) => new ItemsTableRow
		{
			Id = x.Id,
			Name = x.MenuName,
			Type = SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.View),
			Subtype = SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.View, x.Model.Subtype),
			IsUserDefined = (x.Model.Source == UserTypeEnum.UserType.USER),
			CustomFields = x.Model.CustomFields.ToDictionary((ICustomField m) => m.Name, (ICustomField m) => new CustomField(m))
		});
	}

	private void LoadProcedures()
	{
		IEnumerable<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Procedure> enumerable = GetChildrenOf<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Procedures>().Cast<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Procedure>();
		ProceduresCustomFields = CustomField.GetUniqueNamesWithValue(enumerable, (object x) => ((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Procedure)x).Model.CustomFields);
		Procedures = enumerable.Select((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Procedure x) => new ItemsTableRow
		{
			Id = x.Id,
			Name = x.MenuName,
			Type = SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Procedure),
			Subtype = SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Procedure, x.Model.Subtype),
			IsUserDefined = (x.Model.Source == UserTypeEnum.UserType.USER),
			CustomFields = x.Model.CustomFields.ToDictionary((ICustomField m) => m.Name, (ICustomField m) => new CustomField(m))
		});
	}

	private void LoadFunctions()
	{
		IEnumerable<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Function> enumerable = GetChildrenOf<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Functions>().Cast<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Function>();
		FunctionsCustomFields = CustomField.GetUniqueNamesWithValue(enumerable, (object x) => ((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Function)x).Model.CustomFields);
		Functions = enumerable.Select((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Function x) => new ItemsTableRow
		{
			Id = x.Id,
			Name = x.MenuName,
			Type = SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Function),
			Subtype = SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Function, x.Model.Subtype),
			IsUserDefined = (x.Model.Source == UserTypeEnum.UserType.USER),
			CustomFields = x.Model.CustomFields.ToDictionary((ICustomField m) => m.Name, (ICustomField m) => new CustomField(m))
		});
	}

	private void LoadStructures()
	{
		IEnumerable<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Structure> enumerable = GetChildrenOf<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Structures>().Cast<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Structure>();
		StructuresCustomFields = CustomField.GetUniqueNamesWithValue(enumerable, (object x) => ((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Structure)x).Model.CustomFields);
		Structures = enumerable.Select((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Structure x) => new ItemsTableRow
		{
			Id = x.Id,
			Name = x.MenuName,
			Type = SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Structure),
			Subtype = SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Structure, x.Model.Subtype),
			IsUserDefined = (x.Model.Source == UserTypeEnum.UserType.USER),
			CustomFields = x.Model.CustomFields.ToDictionary((ICustomField m) => m.Name, (ICustomField m) => new CustomField(m))
		});
	}

	private IEnumerable<IMenuTreeItem> GetChildrenOf<T>() where T : IMenuTreeItem
	{
		List<IMenuTreeItem> list = new List<IMenuTreeItem>();
		foreach (IMenuTreeItem menuChild in MenuTreeItem.MenuChildren)
		{
			if (typeof(T).IsAssignableFrom(menuChild.GetType()))
			{
				list.AddRange(menuChild.MenuChildren);
			}
		}
		return list;
	}
}
