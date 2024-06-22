using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;
using Dataedo.Shared.Enums;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects;

internal class Procedures : IObject
{
	internal Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Procedures MenuTreeItem;

	[JsonProperty("title_prefix")]
	public string TitlePrefix;

	[JsonProperty("custom_fields")]
	public IList<string> CustomFields;

	[JsonProperty("procedures")]
	public IEnumerable<ItemsTableRow> Items;

	public string ObjectId => MenuTreeItem.ObjectId;

	public Procedures(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Procedures menuTreeItem)
	{
		MenuTreeItem = menuTreeItem;
		IEnumerable<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Procedure> enumerable = MenuTreeItem.MenuChildren.Cast<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Procedure>();
		if (menuTreeItem.Parent is Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Module)
		{
			TitlePrefix = menuTreeItem.Parent.MenuName;
		}
		CustomFields = CustomField.GetUniqueNamesWithValue(enumerable, (object x) => ((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Procedure)x).Model.CustomFields);
		Items = enumerable.Select((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Procedure x) => new ItemsTableRow
		{
			Id = x.Id,
			Name = x.MenuName,
			Type = SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Procedure),
			Subtype = SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Procedure, x.Model.Subtype),
			IsUserDefined = (x.Model.Source == UserTypeEnum.UserType.USER),
			CustomFields = x.Model.CustomFields.ToDictionary((ICustomField m) => m.Name, (ICustomField m) => new CustomField(m))
		});
	}
}
