using Newtonsoft.Json;

namespace Dataedo.App.API.Models;

internal class SessionDataResult
{
	[JsonProperty("user_id")]
	public int UserId { get; set; }

	[JsonProperty("token")]
	public string Token { get; set; }

	[JsonProperty("expires_at")]
	public int ExpiresAt { get; set; }
}
