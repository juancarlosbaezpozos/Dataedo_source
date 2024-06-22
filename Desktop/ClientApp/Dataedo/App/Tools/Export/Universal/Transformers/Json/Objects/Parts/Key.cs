using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.Shared.Enums;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class Key
{
	internal IKey Model;

	[JsonProperty("name")]
	public string Name => Model.Name;

	[JsonProperty("description")]
	public string Description => Model.Description;

	[JsonProperty("is_pk")]
	public bool IsPk => Model.IsPk;

	[JsonProperty("is_user_defined")]
	public bool IsUserDefined => Model.Source == UserTypeEnum.UserType.USER;

	[JsonProperty("columns")]
	public IEnumerable<PathName> Columns => Model.Columns;

	[JsonProperty("custom_fields", NullValueHandling = NullValueHandling.Ignore)]
	public Dictionary<string, CustomField> CustomFields => Model.CustomFields?.Where((ICustomField x) => CustomField.HasValue(x))?.ToDictionary((ICustomField x) => x.Name, (ICustomField x) => new CustomField(x));

	public Key(IKey model)
	{
		Model = model;
	}
}
