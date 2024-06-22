using Newtonsoft.Json;

namespace Dataedo.App.API.Models;

internal class SessionResult
{
	[JsonProperty("data")]
	public SessionDataResult Data { get; set; }
}
