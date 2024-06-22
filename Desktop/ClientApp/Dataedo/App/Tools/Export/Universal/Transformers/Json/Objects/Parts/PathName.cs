using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

public class PathName
{
	[JsonProperty("path")]
	public string Path { get; private set; }

	[JsonProperty("name_without_path")]
	public string NameWithoutPath { get; private set; }

	[JsonProperty("name")]
	public string Name { get; private set; }

	public PathName(string path, string nameWithoutPath, string name)
	{
		Path = path;
		NameWithoutPath = nameWithoutPath;
		Name = name;
	}
}
