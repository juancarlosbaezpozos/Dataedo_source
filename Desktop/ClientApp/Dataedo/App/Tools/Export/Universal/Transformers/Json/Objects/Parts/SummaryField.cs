using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class SummaryField
{
	private IJsonSerializable value;

	[JsonProperty("field")]
	public string Field { get; private set; }

	[JsonProperty("value")]
	public JToken Value => value.ToJson();

	public SummaryField(string field, string text)
	{
		Field = field;
		value = new SummaryFieldValue(text);
	}

	public SummaryField(string field, SummaryFieldLinkValue link)
	{
		Field = field;
		value = link;
	}

	public SummaryField(string field, SummaryFieldArrayValue array)
	{
		Field = field;
		value = array;
	}

	public SummaryField(string field, SummaryFieldCustomFieldValue customField)
	{
		Field = field;
		value = customField;
	}

	public SummaryField(string field, SummaryFieldLinkedTermsValue linkedTerms)
	{
		Field = field;
		value = linkedTerms;
	}
}
