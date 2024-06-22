using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using Dataedo.App.Enums;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.UI;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Search;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.Search;

internal class SearchSupport
{
	public static string HighlightHtmlColor = SkinsManager.CurrentSkin.SearchHighlightHtmlColor;

	public static string HighlightHtmlForeColor = SkinsManager.CurrentSkin.SearchHighlightHtmlForeColor;

	public static Color HighlightColor = SkinsManager.CurrentSkin.SearchHighlightColor;

	public static Color HighlightForeColor = SkinsManager.CurrentSkin.SearchHighlightForeColor;

	public static Color HighlightColorUnderSelectionColorNormal = SkinsManager.CurrentSkin.SearchHighlightColorUnderSelectionColorNormal;

	public static Color HighlightColorUnderSelectionForeColorNormal = SkinsManager.CurrentSkin.SearchHighlightColorUnderSelectionForeColorNormal;

	public static Color TabForeColorHighlighted = SkinsManager.CurrentSkin.SearchTabForeColorHighlighted;

	public static Color TabForeColorNotHighlighted = SkinColors.ControlForeColorFromSystemColors;

	public string[] SearchWords { get; set; }

	public List<CustomFieldSearchItem> CustomFieldSearchItems { get; set; }

	public string[] RegexEscapedSearchWords { get; set; }

	public List<string> Types { get; set; }

	public bool SearchInDocumentationObject { get; set; }

	public bool SearchInModuleObject { get; set; }

	public bool SearchInBusinessGlossaryObject { get; set; }

	public static string HighlightText(string[] regexEscapedSearchWords, string input, bool isHtmlSearch = false)
	{
		if (!string.IsNullOrEmpty(input) && regexEscapedSearchWords != null && regexEscapedSearchWords.Length != 0)
		{
			input = HttpUtility.HtmlEncode(input);
			foreach (string text in regexEscapedSearchWords)
			{
				if (!Regex.IsMatch(input, "(" + text + "{1,})", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
				{
					return input;
				}
			}
			string pattern = "(" + string.Join("|", regexEscapedSearchWords) + ")";
			input = Regex.Replace(input, pattern, "<span style=\"color: " + HighlightHtmlForeColor + "; background-color: " + HighlightHtmlColor + "\">$1</span>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		}
		return input;
	}

	private string HighlightText(string input)
	{
		return HighlightText(RegexEscapedSearchWords, input);
	}

	public List<ResultItem> Search(List<int> documentations, List<string> types, string searchText, List<CustomFieldSearchItem> customFieldsSearchItems, CustomFieldsSupport customFieldsSupport, Form owner = null)
	{
		List<SharedObjectTypeEnum.ObjectType?> list = types.Select((string x) => SharedObjectTypeEnum.StringToType(x)).ToList();
		SearchInDocumentationObject = list.Contains(SharedObjectTypeEnum.ObjectType.Database);
		SearchInModuleObject = list.Contains(SharedObjectTypeEnum.ObjectType.Module);
		SearchInBusinessGlossaryObject = list.Contains(SharedObjectTypeEnum.ObjectType.BusinessGlossary);
		Types = types;
		IEnumerable<string> source = searchText.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries).Distinct();
		SearchWords = source.ToArray();
		CustomFieldSearchItems = customFieldsSearchItems;
		RegexEscapedSearchWords = (from x in SearchWords
			orderby x.Length descending
			select Regex.Escape(HttpUtility.HtmlEncode(x))).ToArray();
		List<ResultItem> list2 = new List<ResultItem>();
		List<SearchResultItem> list3 = null;
		try
		{
			list3 = StaticData.Commands.Select.Search.Search(documentations, Types, customFieldsSupport.Fields.Select((CustomFieldRowExtended x) => new Dataedo.Model.Data.Search.CustomFieldSearchItem(x.FieldName)).ToList(), searchText, customFieldsSearchItems.Select((CustomFieldSearchItem x) => new Dataedo.Model.Data.Search.CustomFieldSearchItem(x.CustomField.FieldName, x.SearchWords)).ToList());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, owner);
		}
		if (list3 != null)
		{
			foreach (SearchResultItem item in list3)
			{
				SharedObjectTypeEnum.ObjectType? mainType = SharedObjectTypeEnum.StringToType(item.ElementType);
				SharedDatabaseTypeEnum.DatabaseType? documentationType = SharedDatabaseTypeEnum.StringToType(item.DocumentationType);
				SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(item.ObjectType);
				ResultItem resultItem = new ResultItem
				{
					ObjectType = (objectType ?? SharedObjectTypeEnum.ObjectType.UnresolvedEntity),
					ObjectTypeId = item.ObjectTypeId,
					CustomObjectType = ((objectType == SharedObjectTypeEnum.ObjectType.Term) ? item.ObjectSubtype : null),
					DocumentationType = documentationType,
					ObjectSubtype = SharedObjectSubtypeEnum.StringToType(objectType, item.ObjectSubtype),
					Source = (UserTypeEnum.ObjectToType(item.Source) ?? UserTypeEnum.UserType.DBMS),
					FkType = CardinalityTypeEnum.StringToType(item.FkType),
					IsPrimaryKey = item.IsPrimaryKey.Equals("True"),
					Status = item.Status,
					ObjectSource = (UserTypeEnum.ObjectToType(item.ObjectSource) ?? UserTypeEnum.UserType.DBMS),
					ObjectStatus = item.ObjectStatus,
					ElementType = SharedObjectTypeEnum.StringToType(item.ElementType),
					ElementSubtype = SharedObjectSubtypeEnum.StringToType(mainType, item.ElementSubtype),
					DatabaseId = Convert.ToInt32(item.DocumentationId),
					DatabaseName = (SearchInDocumentationObject ? HighlightText(item.DocumentationName) : item.DocumentationName),
					DatabaseTitle = ((SearchInDocumentationObject || SearchInBusinessGlossaryObject) ? HighlightText(item.DocumentationTitle) : item.DocumentationTitle),
					DatabaseNameNoHighlight = Convert.ToString(item.DocumentationName),
					DatabaseTitleNoHighlight = Convert.ToString(item.DocumentationTitle),
					DatabaseShowSchema = item.DocumentationShowSchemaEffective,
					ModuleId = item.ModuleId,
					ModuleTitle = (SearchInModuleObject ? HighlightText(item.ModuleTitle) : item.ModuleTitle),
					ModuleDatabaseId = item.ModuleDocumentationId,
					ModuleDatabaseName = (SearchInDocumentationObject ? HighlightText(item.ModuleDocumentationName) : item.ModuleDocumentationName),
					ModuleDatabaseTitle = (SearchInDocumentationObject ? HighlightText(item.ModuleDocumentationTitle) : item.ModuleDocumentationTitle),
					ModuleDatabaseNameNoHighlight = Convert.ToString(item.ModuleDocumentationName),
					ModuleDatabaseTitleNoHighlight = Convert.ToString(item.ModuleDocumentationTitle),
					ModuleDatabaseShowSchema = item.ModuleDocumentationShowSchemaEffective,
					ObjectId = item.ObjectId,
					ObjectName = ((item.ShowSchema && !string.IsNullOrEmpty(item.ObjectSchema)) ? HighlightText(item.ObjectSchema + "." + item.ObjectName) : HighlightText(item.ObjectName)),
					ObjectNameWithSchema = HighlightText(item.ObjectSchema + "." + item.ObjectName),
					ObjectTitle = HighlightText(item.ObjectTitle),
					ElementId = item.ElementId,
					ElementName = HighlightText(item.ElementName),
					ElementTitle = HighlightText(item.ElementTitle),
					IsNotResult = false,
					IsProperResult = item.IsAllFoundInAnyFieldAndAllSpecificCustomField,
					FoundInDefinition = item.FoundInDefinition.AreAllWordsFound
				};
				resultItem.ComputeRank(item.FoundInName.AreAllWordsFound, item.FoundInTitle.AreAllWordsFound, item.FoundInCustomFields, item.FoundInSpecificCustomFields, item.FoundInDescription.AreAllWordsFound, item.FoundInDefinition.AreAllWordsFound);
				list2.Add(resultItem);
			}
			return list2;
		}
		return list2;
	}

	public ResultItem PrepareTree(List<int> documentations, List<ResultItem> results)
	{
		ResultItem resultItem = new ResultItem
		{
			IsNotResult = true
		};
		resultItem.Nodes = new List<ResultItem>();
		var enumerable = from r in results
			group r by new { r.DatabaseId };
		_ = from r in results
			group r by new { r.DatabaseId, r.ModuleDatabaseId };
		foreach (var item in enumerable)
		{
			foreach (var item2 in from r in item
				group r by new { r.ModuleDatabaseId })
			{
				ProcessDocumentationResults(documentations, resultItem, item, item2, mayBeWithModule: true);
			}
		}
		resultItem.Nodes = resultItem.Nodes.OrderByDescending((ResultItem x) => x.TreeRank).ToList();
		return resultItem;
	}

	private void ProcessDocumentationResults(List<int> documentations, ResultItem root, IGrouping<object, ResultItem> documentationsGroup, IGrouping<object, ResultItem> documentationsModulesGroup, bool mayBeWithModule)
	{
		ResultItem documentation = documentationsModulesGroup.Where((ResultItem r) => r.ObjectType == SharedObjectTypeEnum.ObjectType.Database || r.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary).FirstOrDefault();
		if (documentation == null)
		{
			ResultItem resultItem = documentationsModulesGroup.First();
			documentation = resultItem.GetDocumentationItem(isNotResult: true, mayBeWithModule);
			documentation.IsNotResult = true;
			documentation.ParentNode = root;
			if (mayBeWithModule && resultItem.ModuleDatabaseId.HasValue && resultItem.ModuleDatabaseId != resultItem.DatabaseId)
			{
				ProcessDocumentationResults(documentations, root, documentationsGroup, documentationsModulesGroup, mayBeWithModule: false);
			}
			else if (!documentations.Contains(documentation.DatabaseId))
			{
				return;
			}
		}
		if (!documentations.Contains(documentation.DatabaseId) || root.Nodes.Any((ResultItem x) => x.DatabaseId == documentation.DatabaseId))
		{
			return;
		}
		documentation.Nodes = new List<ResultItem>();
		if (mayBeWithModule)
		{
			foreach (var item in from x in documentationsGroup
				group x by new { x.DatabaseId })
			{
				AddObjects(documentation, (from x in item
					where x.ObjectType != SharedObjectTypeEnum.ObjectType.Database && x.ObjectType != SharedObjectTypeEnum.ObjectType.BusinessGlossary
					select (x)).ToList(), withModule: false);
			}
		}
		List<ResultItem> list = (from r in (from r in documentationsGroup
				group r by r.ModuleId).Select(delegate(IGrouping<int?, ResultItem> modulesGroup)
			{
				ResultItem resultItem2 = modulesGroup.Where((ResultItem r) => r.ObjectType == SharedObjectTypeEnum.ObjectType.Module).FirstOrDefault();
				if (resultItem2 == null)
				{
					resultItem2 = modulesGroup.First().GetModuleItem(isNotResult: true);
					resultItem2.IsNotResult = true;
				}
				if (resultItem2.ModuleId != -1 && resultItem2.DatabaseId == documentation.DatabaseId)
				{
					resultItem2.ParentNode = documentation;
					resultItem2.Nodes = new List<ResultItem>();
					AddObjects(resultItem2, modulesGroup.OrderBy((ResultItem r) => SharedObjectTypeEnum.GetSortValue(r.ObjectType)).ToList());
					return resultItem2;
				}
				return null;
			})
			where r != null
			select r into x
			orderby x.TreeRank descending
			select x).ToList();
		if (list.Count > 0)
		{
			ResultItem moduleItem = documentation.GetModuleItem(isNotResult: true);
			moduleItem.ObjectName = "Subject Areas";
			moduleItem.ObjectType = SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database;
			moduleItem.ParentNode = documentation;
			moduleItem.Nodes = list;
			moduleItem.IsNotResult = true;
			documentation.Nodes.Insert(0, moduleItem);
		}
		root.Nodes.Add(documentation);
	}

	private static string[] GetCaseInsensitiveDistinctWords(IEnumerable<string> words)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (string word in words)
		{
			string text = word.ToLowerInvariant();
			if (!dictionary.ContainsKey(word))
			{
				dictionary.Add(text, text);
			}
		}
		return dictionary.Select((KeyValuePair<string, string> x) => x.Value).ToArray();
	}

	private void AddObjects(ResultItem resultElement, List<ResultItem> baseList, bool withModule = true)
	{
		foreach (IGrouping<SharedObjectTypeEnum.ObjectType, ResultItem> item in from g in baseList
			group g by g.ObjectType)
		{
			if (item.Key != SharedObjectTypeEnum.ObjectType.Module)
			{
				ResultItem resultItem = item.First();
				AddObjectsByType(resultElement, item.ToList(), resultItem.ObjectType, withModule);
			}
		}
	}

	private void AddObjectsByType(ResultItem resultElement, List<ResultItem> baseList, SharedObjectTypeEnum.ObjectType objectType, bool withModule = true)
	{
		ResultItem moduleItem = resultElement.GetModuleItem(isNotResult: true);
		moduleItem.ObjectName = SharedObjectTypeEnum.TypeToStringForMenu(objectType);
		moduleItem.ObjectType = SharedObjectTypeEnum.ObjectType.Folder_Database;
		moduleItem.ParentNode = resultElement;
		moduleItem.Nodes = new List<ResultItem>();
		moduleItem.IsNotResult = true;
		resultElement.Nodes.Add(moduleItem);
		foreach (IGrouping<int?, ResultItem> item in from g in baseList
			group g by g.ObjectId)
		{
			ResultItem objectItem = (from r in item
				where !r.ElementType.HasValue
				orderby r.Rank descending
				select r).FirstOrDefault()?.GetObjectItem(isNotResult: false, withModule);
			if (objectItem == null)
			{
				objectItem = item.First().GetObjectItem(isNotResult: true, withModule);
				objectItem.IsNotResult = true;
			}
			objectItem.Nodes = (from r in (from r in item
					where r.ElementType.HasValue
					select r into x
					group x by new { x.ElementId, x.ElementSubtype } into x
					select x.FirstOrDefault()?.GetElementItem(isNotResult: false, withModule)).Select(delegate(ResultItem r)
				{
					r.ParentNode = objectItem;
					objectItem.Rank += r.Rank;
					objectItem.TreeRank += r.TreeRank;
					return r;
				})
				orderby r.ElementType, r.Rank descending
				select r).ToList();
			objectItem.ParentNode = moduleItem;
			moduleItem.TreeRank += objectItem.TreeRank;
			moduleItem.Nodes.Add(objectItem);
		}
		moduleItem.Nodes = (from x in moduleItem.Nodes
			orderby x.TreeRank descending, x.ObjectType, x.ElementType, x.NameWithTitle
			select x).ToList();
		resultElement.TreeRank += moduleItem.TreeRank;
	}
}
