using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json;

internal interface IObject
{
	[JsonProperty("object_id")]
	string ObjectId { get; }
}
