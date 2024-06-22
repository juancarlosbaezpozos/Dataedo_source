using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class SummaryFieldLinkedTermsValue : IJsonSerializable
{
	[JsonProperty("_type")]
	public const string Type = "linked_terms";

	[JsonProperty("linked_terms")]
	public IEnumerable<LinkedTerm> FieldValue;

	public SummaryFieldLinkedTermsValue(IEnumerable<LinkedTerm> linkedTerms)
	{
		FieldValue = linkedTerms;
	}

	public JToken ToJson()
	{
		return JToken.FromObject(this);
	}
}
