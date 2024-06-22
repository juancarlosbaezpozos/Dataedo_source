using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class IdName
{
	[JsonProperty("id")]
	public string Id { get; private set; }

	[JsonProperty("name")]
	public string Name { get; private set; }

	public IdName(string id, string name)
	{
		Id = id;
		Name = name;
	}
}
