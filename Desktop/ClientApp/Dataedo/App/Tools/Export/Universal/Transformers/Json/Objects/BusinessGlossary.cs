using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Dataedo.Shared.Enums;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects;

internal class BusinessGlossary : IObject
{
	internal Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.BusinessGlossary MenuTreeItem;

	internal IBusinessGlossary Model;

	[JsonProperty("terms_custom_fields")]
	public IList<string> TermsCustomFields;

	[JsonProperty("terms")]
	public IEnumerable<ItemsTableRow> Terms;

	public string ObjectId => MenuTreeItem.ObjectId;

	[JsonProperty("name")]
	public string Name => Model.Title;

	[JsonProperty("description")]
	public string Description => Model.Description;

	[JsonProperty("type")]
	public string Type => "business-glossary";

	[JsonProperty("summary")]
	public IList<SummaryField> Summary { get; private set; }

	public BusinessGlossary(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.BusinessGlossary menuTreeItem)
	{
		MenuTreeItem = menuTreeItem;
		Model = menuTreeItem.Model;
		SetSummaryTable();
		LoadTerms();
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

	private void LoadTerms()
	{
		IOrderedEnumerable<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Term> orderedEnumerable = from x in GetAllChildren<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Term>(MenuTreeItem)
			orderby x.MenuName
			select x;
		TermsCustomFields = CustomField.GetUniqueNamesWithValue(orderedEnumerable, (object x) => ((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Term)x).Model.CustomFields);
		Terms = orderedEnumerable.Select((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Term x) => new ItemsTableRow
		{
			Id = x.Id,
			Name = x.MenuName,
			Type = SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Term),
			Subtype = BusinessGlossarySupport.GetTermIconName(x.Model.TypeIconId, string.Empty),
			IsUserDefined = false,
			CustomFields = x.Model.CustomFields.ToDictionary((ICustomField m) => m.Name, (ICustomField m) => new CustomField(m))
		});
	}

	private IEnumerable<T> GetAllChildren<T>(IMenuTreeItem menuItem) where T : IMenuTreeItem
	{
		List<T> list = new List<T>();
		foreach (IMenuTreeItem menuChild in menuItem.MenuChildren)
		{
			if (typeof(T).IsAssignableFrom(menuChild.GetType()))
			{
				list.Add((T)menuChild);
			}
			list.AddRange(GetAllChildren<T>(menuChild));
		}
		return list;
	}
}
