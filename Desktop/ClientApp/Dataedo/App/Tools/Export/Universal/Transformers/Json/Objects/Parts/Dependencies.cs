using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class Dependencies
{
	internal IDependencies Model;

	[JsonProperty("uses", NullValueHandling = NullValueHandling.Ignore)]
	public IEnumerable<Dependency> Uses { get; private set; }

	[JsonProperty("used_by", NullValueHandling = NullValueHandling.Ignore)]
	public IEnumerable<Dependency> UsedBy { get; private set; }

	public Dependencies(IDependencies model)
	{
		Model = model;
		Uses = model?.Uses?.Select((IDependency x) => new Dependency(x));
		UsedBy = model?.UsedBy?.Select((IDependency x) => new Dependency(x));
	}
}
