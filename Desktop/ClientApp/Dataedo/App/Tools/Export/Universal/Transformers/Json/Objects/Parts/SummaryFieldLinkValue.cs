using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class SummaryFieldLinkValue : IJsonSerializable
{
	[JsonProperty("_type")]
	public const string Type = "link";

	[JsonProperty("name")]
	public string Name { get; private set; }

	[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
	public string Id { get; private set; }

	public SummaryFieldLinkValue(string id, string name)
	{
		Id = id;
		Name = name;
	}

	public JToken ToJson()
	{
		return JToken.FromObject(this);
	}
}
