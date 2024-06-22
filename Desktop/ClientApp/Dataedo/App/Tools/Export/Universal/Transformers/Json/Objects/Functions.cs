using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;
using Dataedo.Shared.Enums;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects;

internal class Functions : IObject
{
	internal Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Functions MenuTreeItem;

	[JsonProperty("title_prefix")]
	public string TitlePrefix;

	[JsonProperty("custom_fields")]
	public IList<string> CustomFields;

	[JsonProperty("functions")]
	public IEnumerable<ItemsTableRow> Items;

	public string ObjectId => MenuTreeItem.ObjectId;

	public Functions(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Functions menuTreeItem)
	{
		MenuTreeItem = menuTreeItem;
		IEnumerable<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Function> enumerable = MenuTreeItem.MenuChildren.Cast<Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Function>();
		if (menuTreeItem.Parent is Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Module)
		{
			TitlePrefix = menuTreeItem.Parent.MenuName;
		}
		CustomFields = CustomField.GetUniqueNamesWithValue(enumerable, (object x) => ((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Function)x).Model.CustomFields);
		Items = enumerable.Select((Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Function x) => new ItemsTableRow
		{
			Id = x.Id,
			Name = x.MenuName,
			Type = SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Function),
			Subtype = SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Function, x.Model.Subtype),
			IsUserDefined = (x.Model.Source == UserTypeEnum.UserType.USER),
			CustomFields = x.Model.CustomFields.ToDictionary((ICustomField m) => m.Name, (ICustomField m) => new CustomField(m))
		});
	}
}
