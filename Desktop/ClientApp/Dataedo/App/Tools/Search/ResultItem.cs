using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Enums;
using Dataedo.Model.Data.Search;
using Dataedo.Shared.Enums;
using DevExpress.XtraTreeList;

namespace Dataedo.App.Tools.Search;

public class ResultItem : TreeList.IVirtualTreeListData
{
	private static int BASE_FOUND_IN_NAME_RANK = 8;

	private static int BASE_FOUND_IN_TITLE_RANK = 4;

	private static int BASE_FOUND_IN_CUSTOM_FIELD = 4;

	private static int BASE_FOUND_IN_SPECIFIC_CUSTOM_FIELD = 8;

	private static int BASE_FOUND_IN_DESCRIPTION_RANK = 2;

	private static int BASE_FOUND_IN_DEFITNITION_RANK = 2;

	public ResultItem ParentNode { get; set; }

	public List<ResultItem> Nodes { get; set; }

	public SharedObjectTypeEnum.ObjectType ObjectType { get; set; }

	public int? ObjectTypeId { get; set; }

	public string CustomObjectType { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype { get; set; }

	public UserTypeEnum.UserType Source { get; set; }

	public UserTypeEnum.UserType ObjectSource { get; set; }

	public CardinalityTypeEnum.CardinalityType? FkType { get; set; }

	public bool IsPrimaryKey { get; set; }

	public SharedObjectTypeEnum.ObjectType? ElementType { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype ElementSubtype { get; set; }

	public SharedDatabaseTypeEnum.DatabaseType? DocumentationType { get; internal set; }

	public int Rank { get; set; }

	public int TreeRank { get; set; }

	public string OtherName { get; set; }

	public int DatabaseId { get; set; }

	public string DatabaseName { get; set; }

	public string DatabaseTitle { get; set; }

	public string DatabaseNameNoHighlight { get; set; }

	public string DatabaseTitleNoHighlight { get; set; }

	public bool DatabaseShowSchema { get; internal set; }

	public int? ModuleId { get; set; }

	public string ModuleTitle { get; set; }

	public int? ModuleDatabaseId { get; set; }

	public string ModuleDatabaseName { get; set; }

	public string ModuleDatabaseTitle { get; set; }

	public string ModuleDatabaseNameNoHighlight { get; set; }

	public string ModuleDatabaseTitleNoHighlight { get; set; }

	public bool ModuleDatabaseShowSchema { get; internal set; }

	public int? ObjectId { get; set; }

	public string ObjectName { get; set; }

	public string ObjectNameWithSchema { get; set; }

	public string ObjectTitle { get; set; }

	public int? ElementId { get; set; }

	public string ElementName { get; set; }

	public string ElementTitle { get; set; }

	public bool IsNotResult { get; set; }

	public bool IsProperResult { get; set; }

	public bool FoundInDefinition { get; set; }

	public int Id
	{
		get
		{
			if (ElementId.HasValue)
			{
				return ElementId.Value;
			}
			if (ObjectId.HasValue)
			{
				return ObjectId.Value;
			}
			if (ModuleId.HasValue)
			{
				return ModuleId.Value;
			}
			return DatabaseId;
		}
	}

	public string Name
	{
		get
		{
			if (ObjectType == SharedObjectTypeEnum.ObjectType.Database)
			{
				return DatabaseName;
			}
			if (ObjectType == SharedObjectTypeEnum.ObjectType.Module)
			{
				return ModuleTitle;
			}
			return ObjectName;
		}
	}

	public string Title
	{
		get
		{
			if (ObjectType == SharedObjectTypeEnum.ObjectType.Database)
			{
				return DatabaseTitle;
			}
			if (ObjectType == SharedObjectTypeEnum.ObjectType.Module)
			{
				return ModuleTitle;
			}
			return ObjectTitle;
		}
	}

	public string NameWithTitle
	{
		get
		{
			if (ObjectType == SharedObjectTypeEnum.ObjectType.Database || ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
			{
				return DatabaseTitle;
			}
			if (ObjectType == SharedObjectTypeEnum.ObjectType.Module)
			{
				return ModuleTitle;
			}
			if (ElementId.HasValue)
			{
				return ElementName + ((!string.IsNullOrEmpty(ElementTitle)) ? (" (" + ElementTitle + ")") : "");
			}
			if (ObjectType == SharedObjectTypeEnum.ObjectType.Term)
			{
				return ObjectTitle;
			}
			return ObjectName + ((!string.IsNullOrEmpty(ObjectTitle)) ? (" (" + ObjectTitle + ")") : "");
		}
	}

	public string NameWithTitleForObjects
	{
		get
		{
			if (ObjectType == SharedObjectTypeEnum.ObjectType.Database)
			{
				return DatabaseName + ((!string.IsNullOrEmpty(DatabaseTitle)) ? (" (" + DatabaseTitle + ")") : "");
			}
			if (ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
			{
				return DatabaseTitle;
			}
			if (ObjectType == SharedObjectTypeEnum.ObjectType.Module)
			{
				return ModuleTitle;
			}
			return ObjectName + ((!string.IsNullOrEmpty(ObjectTitle)) ? (" (" + ObjectTitle + ")") : "");
		}
	}

	public SharedObjectTypeEnum.ObjectType Type
	{
		get
		{
			if (!ElementType.HasValue)
			{
				return ObjectType;
			}
			return ElementType.Value;
		}
	}

	public string DocumentationNameWithTitle => DatabaseName + ((!string.IsNullOrEmpty(DatabaseTitle)) ? (" (" + DatabaseTitle + ")") : "");

	public string DocumentationTitleNoHighlight => DatabaseTitleNoHighlight;

	public string Status { get; internal set; }

	public string ObjectStatus { get; internal set; }

	public override string ToString()
	{
		return NameWithTitle;
	}

	public void ComputeRank(bool areAllFoundInName, bool areAllFoundInTitle, List<SearchResultItem.FoundCustomFieldItem> foundInCustomFields, List<SearchResultItem.FoundCustomFieldItem> foundInSpecificCustomFields, bool areAllFoundInDescription, bool areAllFoundInDefinition)
	{
		Rank = 0;
		if (ElementType.HasValue)
		{
			Rank += (areAllFoundInName ? BASE_FOUND_IN_NAME_RANK : 0);
			Rank += (areAllFoundInTitle ? BASE_FOUND_IN_TITLE_RANK : 0);
			Rank += foundInCustomFields.Where((SearchResultItem.FoundCustomFieldItem x) => x.AreAllWordsFound).Count() * BASE_FOUND_IN_CUSTOM_FIELD;
			Rank += foundInSpecificCustomFields.Where((SearchResultItem.FoundCustomFieldItem x) => x.AreAllWordsFound).Count() * BASE_FOUND_IN_SPECIFIC_CUSTOM_FIELD;
			Rank += (areAllFoundInDescription ? BASE_FOUND_IN_DESCRIPTION_RANK : 0);
			Rank += (areAllFoundInDefinition ? BASE_FOUND_IN_DEFITNITION_RANK : 0);
		}
		else if (ObjectType == SharedObjectTypeEnum.ObjectType.Database)
		{
			Rank += (areAllFoundInName ? (BASE_FOUND_IN_NAME_RANK * 4) : 0);
			Rank += (areAllFoundInTitle ? (BASE_FOUND_IN_TITLE_RANK * 4) : 0);
			Rank += foundInCustomFields.Where((SearchResultItem.FoundCustomFieldItem x) => x.AreAllWordsFound).Count() * BASE_FOUND_IN_CUSTOM_FIELD * 4;
			Rank += foundInSpecificCustomFields.Where((SearchResultItem.FoundCustomFieldItem x) => x.AreAllWordsFound).Count() * BASE_FOUND_IN_SPECIFIC_CUSTOM_FIELD * 4;
			Rank += (areAllFoundInDescription ? (BASE_FOUND_IN_DESCRIPTION_RANK * 4) : 0);
		}
		else if (ObjectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			Rank += (areAllFoundInName ? (BASE_FOUND_IN_NAME_RANK * 3) : 0);
			Rank += (areAllFoundInTitle ? (BASE_FOUND_IN_TITLE_RANK * 3) : 0);
			Rank += foundInCustomFields.Where((SearchResultItem.FoundCustomFieldItem x) => x.AreAllWordsFound).Count() * BASE_FOUND_IN_CUSTOM_FIELD * 3;
			Rank += foundInSpecificCustomFields.Where((SearchResultItem.FoundCustomFieldItem x) => x.AreAllWordsFound).Count() * BASE_FOUND_IN_SPECIFIC_CUSTOM_FIELD * 3;
			Rank += (areAllFoundInDescription ? (BASE_FOUND_IN_DESCRIPTION_RANK * 3) : 0);
		}
		else
		{
			Rank += (areAllFoundInName ? (BASE_FOUND_IN_NAME_RANK * 2) : 0);
			Rank += (areAllFoundInTitle ? (BASE_FOUND_IN_TITLE_RANK * 2) : 0);
			Rank += foundInCustomFields.Where((SearchResultItem.FoundCustomFieldItem x) => x.AreAllWordsFound).Count() * BASE_FOUND_IN_CUSTOM_FIELD * 2;
			Rank += foundInSpecificCustomFields.Where((SearchResultItem.FoundCustomFieldItem x) => x.AreAllWordsFound).Count() * BASE_FOUND_IN_SPECIFIC_CUSTOM_FIELD * 2;
			Rank += (areAllFoundInDescription ? (BASE_FOUND_IN_DESCRIPTION_RANK * 2) : 0);
			Rank += (areAllFoundInDefinition ? (BASE_FOUND_IN_DEFITNITION_RANK * 2) : 0);
		}
		TreeRank = Rank;
	}

	public ResultItem GetDocumentationItem(bool isNotResult, bool mayBeWithModule)
	{
		if (mayBeWithModule)
		{
			return new ResultItem
			{
				ObjectType = ((ObjectType != SharedObjectTypeEnum.ObjectType.BusinessGlossary && ObjectType != SharedObjectTypeEnum.ObjectType.Term) ? SharedObjectTypeEnum.ObjectType.Database : SharedObjectTypeEnum.ObjectType.BusinessGlossary),
				DocumentationType = DocumentationType,
				ElementType = null,
				Rank = ((!isNotResult) ? Rank : 0),
				TreeRank = ((!isNotResult) ? TreeRank : 0),
				DatabaseId = DatabaseId,
				DatabaseName = DatabaseName,
				DatabaseTitle = DatabaseTitle,
				DatabaseNameNoHighlight = DatabaseNameNoHighlight,
				DatabaseTitleNoHighlight = DatabaseTitleNoHighlight,
				IsNotResult = IsNotResult
			};
		}
		return new ResultItem
		{
			ObjectType = ((ObjectType != SharedObjectTypeEnum.ObjectType.BusinessGlossary && ObjectType != SharedObjectTypeEnum.ObjectType.Term) ? SharedObjectTypeEnum.ObjectType.Database : SharedObjectTypeEnum.ObjectType.BusinessGlossary),
			DocumentationType = DocumentationType,
			ElementType = null,
			Rank = ((!isNotResult) ? Rank : 0),
			TreeRank = ((!isNotResult) ? TreeRank : 0),
			DatabaseId = ModuleDatabaseId.Value,
			DatabaseName = ModuleDatabaseName,
			DatabaseTitle = ModuleDatabaseTitle,
			DatabaseNameNoHighlight = ModuleDatabaseNameNoHighlight,
			DatabaseTitleNoHighlight = ModuleDatabaseTitleNoHighlight,
			IsNotResult = IsNotResult
		};
	}

	public ResultItem GetModuleItem(bool isNotResult, bool withModule = true)
	{
		ResultItem documentationItem = GetDocumentationItem(isNotResult, mayBeWithModule: true);
		if (withModule && ModuleDatabaseId.HasValue && ModuleDatabaseId != DatabaseId)
		{
			documentationItem.DatabaseId = ModuleDatabaseId.Value;
			documentationItem.ModuleDatabaseShowSchema = ModuleDatabaseShowSchema;
		}
		documentationItem.ObjectType = SharedObjectTypeEnum.ObjectType.Module;
		documentationItem.ModuleId = ((!withModule) ? (-1) : (ModuleId ?? (-1)));
		documentationItem.ModuleTitle = ((!withModule) ? "Other" : (ModuleId.HasValue ? ModuleTitle : "Other"));
		if (!isNotResult)
		{
			documentationItem.IsProperResult = IsProperResult;
		}
		return documentationItem;
	}

	public ResultItem GetObjectItem(bool isNotResult, bool withModule = true)
	{
		ResultItem moduleItem = GetModuleItem(isNotResult, withModule);
		moduleItem.ObjectType = ObjectType;
		moduleItem.ObjectTypeId = ObjectTypeId;
		moduleItem.CustomObjectType = CustomObjectType;
		moduleItem.ObjectSubtype = ObjectSubtype;
		moduleItem.Source = Source;
		moduleItem.FkType = FkType;
		moduleItem.IsPrimaryKey = IsPrimaryKey;
		moduleItem.Status = Status;
		moduleItem.ObjectStatus = ObjectStatus;
		moduleItem.ObjectSource = ObjectSource;
		moduleItem.ObjectId = ObjectId;
		if (withModule && (moduleItem.DatabaseShowSchema || moduleItem.ModuleDatabaseShowSchema))
		{
			moduleItem.ObjectName = ObjectNameWithSchema;
		}
		else
		{
			moduleItem.ObjectName = ObjectName;
		}
		moduleItem.ObjectTitle = ObjectTitle;
		moduleItem.FoundInDefinition = FoundInDefinition;
		if (!isNotResult)
		{
			moduleItem.IsProperResult = IsProperResult;
		}
		return moduleItem;
	}

	public ResultItem GetElementItem(bool isNotResult, bool withModule = true)
	{
		ResultItem objectItem = GetObjectItem(isNotResult, withModule);
		objectItem.ObjectType = ObjectType;
		objectItem.ObjectTypeId = ObjectTypeId;
		objectItem.CustomObjectType = CustomObjectType;
		objectItem.ObjectSubtype = ObjectSubtype;
		objectItem.Source = Source;
		objectItem.IsPrimaryKey = IsPrimaryKey;
		objectItem.FkType = FkType;
		objectItem.Status = Status;
		objectItem.ObjectStatus = ObjectStatus;
		objectItem.ObjectSource = ObjectSource;
		objectItem.ElementType = ElementType;
		objectItem.ElementSubtype = ElementSubtype;
		objectItem.ElementId = ElementId;
		objectItem.ElementName = ElementName;
		objectItem.ElementTitle = ElementTitle;
		if (!isNotResult)
		{
			objectItem.IsProperResult = IsProperResult;
		}
		return objectItem;
	}

	public List<ResultItem> GetObjectNodes(bool withoutModuleOnly)
	{
		List<ResultItem> list = new List<ResultItem>();
		int num;
		if (withoutModuleOnly)
		{
			if (!withoutModuleOnly)
			{
				num = 0;
				goto IL_0083;
			}
			if (ModuleId != -1 && ModuleId.HasValue)
			{
				num = ((ObjectType == SharedObjectTypeEnum.ObjectType.Module) ? 1 : 0);
				if (num == 0)
				{
					goto IL_0083;
				}
			}
			else
			{
				num = 1;
			}
		}
		else
		{
			num = 1;
		}
		if (!IsNotResult || (ObjectId.HasValue && Nodes != null && Nodes.Count > 0))
		{
			list.Add(this);
		}
		goto IL_0083;
		IL_0083:
		if (num != 0 && !ObjectId.HasValue && Nodes != null)
		{
			foreach (ResultItem node in Nodes)
			{
				list.AddRange(node.GetObjectNodes(withoutModuleOnly));
			}
		}
		if (list.Count > 1)
		{
			return (from x in list
				group x by x.Id into x
				select x.First()).ToList();
		}
		return list;
	}

	public List<ResultItem> GetNodes()
	{
		List<ResultItem> list = new List<ResultItem>();
		if (!IsNotResult)
		{
			list.Add(this);
		}
		if (Nodes != null)
		{
			foreach (ResultItem node in Nodes)
			{
				list.AddRange(node.GetNodes());
			}
			return list;
		}
		return list;
	}

	public void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
	{
		if (info.Column.FieldName == "NameWithTitle")
		{
			info.CellData = NameWithTitle;
		}
	}

	public void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
	{
		info.Children = Nodes;
	}

	public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
	{
		throw new NotImplementedException();
	}
}
