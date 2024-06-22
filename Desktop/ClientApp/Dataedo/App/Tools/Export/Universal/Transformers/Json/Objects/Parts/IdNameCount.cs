using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class IdNameCount : IdName
{
	[JsonProperty("count")]
	public int Count { get; private set; }

	public IdNameCount(string id, string name, int count)
		: base(id, name)
	{
		Count = count;
	}
}
