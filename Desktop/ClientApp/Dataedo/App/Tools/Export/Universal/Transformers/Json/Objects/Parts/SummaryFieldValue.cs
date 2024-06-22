using Newtonsoft.Json.Linq;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class SummaryFieldValue : IJsonSerializable
{
	private string text;

	public SummaryFieldValue(string text)
	{
		this.text = text;
	}

	public JToken ToJson()
	{
		return new JValue(text);
	}
}
