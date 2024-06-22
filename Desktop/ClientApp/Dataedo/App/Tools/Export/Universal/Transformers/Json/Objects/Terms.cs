using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Dataedo.Shared.Enums;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects;

internal class Terms : IObject
{
	internal Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Terms MenuTreeItem;

	[JsonProperty("custom_fields")]
	public IList<string> CustomFields;

	[JsonProperty("terms")]
	public IEnumerable<ItemsTableRow> Items;

	public string ObjectId => MenuTreeItem.ObjectId;

	public Terms(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Terms menuTreeItem)
	{
		MenuTreeItem = menuTreeItem;
		IOrderedEnumerable<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Term> orderedEnumerable = from x in GetAllChildren<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Term>(MenuTreeItem)
			orderby x.MenuName
			select x;
		CustomFields = CustomField.GetUniqueNamesWithValue(orderedEnumerable, (object x) => ((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Term)x).Model.CustomFields);
		Items = orderedEnumerable.Select((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Term x) => new ItemsTableRow
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
