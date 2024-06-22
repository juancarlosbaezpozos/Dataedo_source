using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class ItemsTableRow
{
	[JsonProperty("id")]
	public string Id;

	[JsonProperty("name")]
	public string Name;

	[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
	public string Type;

	[JsonProperty("subtype", NullValueHandling = NullValueHandling.Ignore)]
	public string Subtype;

	[JsonProperty("is_user_defined", NullValueHandling = NullValueHandling.Ignore)]
	public bool? IsUserDefined;

	[JsonProperty("custom_fields", NullValueHandling = NullValueHandling.Ignore)]
	public Dictionary<string, CustomField> CustomFields;
}
