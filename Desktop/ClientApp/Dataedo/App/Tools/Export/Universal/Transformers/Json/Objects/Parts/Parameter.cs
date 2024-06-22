using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class Parameter
{
	internal IParameter Model;

	[JsonProperty("name")]
	public string Name => Model.Name;

	[JsonProperty("description")]
	public string Description => Model.Description;

	[JsonProperty("mode")]
	public string Mode => Model.Mode;

	[JsonProperty("data_type")]
	public string DataType => Model.DataType;

	[JsonProperty("custom_fields", NullValueHandling = NullValueHandling.Ignore)]
	public Dictionary<string, CustomField> CustomFields => Model.CustomFields?.Where((ICustomField x) => CustomField.HasValue(x))?.ToDictionary((ICustomField x) => x.Name, (ICustomField x) => new CustomField(x));

	public Parameter(IParameter model)
	{
		Model = model;
	}
}
