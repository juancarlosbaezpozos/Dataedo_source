using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class SummaryFieldArrayValue : IJsonSerializable
{
	public IList<IJsonSerializable> Items;

	public SummaryFieldArrayValue()
	{
		Items = new List<IJsonSerializable>();
	}

	public void Add(string text)
	{
		Items.Add(new SummaryFieldValue(text));
	}

	public void Add(SummaryFieldLinkValue link)
	{
		Items.Add(link);
	}

	public int Count()
	{
		return Items.Count();
	}

	public JToken ToJson()
	{
		return JToken.FromObject(Items);
	}
}
