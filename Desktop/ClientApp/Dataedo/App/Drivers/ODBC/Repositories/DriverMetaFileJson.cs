using Newtonsoft.Json;

namespace Dataedo.App.Drivers.ODBC.Repositories;

public class DriverMetaFileJson
{
	[JsonProperty("uid")]
	public string UID { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("version")]
	public string Version { get; set; }
}
