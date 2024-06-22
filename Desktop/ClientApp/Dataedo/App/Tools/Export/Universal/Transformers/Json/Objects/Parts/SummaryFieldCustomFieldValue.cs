using Dataedo.App.DataRepository.Models;
using Dataedo.Shared.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class SummaryFieldCustomFieldValue : IJsonSerializable
{
	[JsonProperty("_type")]
	public const string Type = "custom_field";

	[JsonProperty("value")]
	public string FieldValue;

	[JsonProperty("type")]
	public string FieldType;

	public SummaryFieldCustomFieldValue(ICustomField customField)
	{
		FieldValue = customField.Value;
		FieldType = CustomFieldTypeEnum.TypeToString(customField.Type);
	}

	public JToken ToJson()
	{
		return JToken.FromObject(this);
	}
}
