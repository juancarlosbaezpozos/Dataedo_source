using Newtonsoft.Json.Linq;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json;

internal interface IJsonSerializable
{
	JToken ToJson();
}
