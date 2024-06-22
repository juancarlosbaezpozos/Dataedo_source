using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dataedo.App.DataRepository.Models;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class Trigger
{
	internal ITrigger Model;

	[JsonProperty("name")]
	public string Name => Model.Name;

	[JsonProperty("description")]
	public string Description => Model.Description;

	[JsonProperty("action")]
	public string Action => GetAction();

	[JsonProperty("script", NullValueHandling = NullValueHandling.Ignore)]
	public string Script => Model.Script;

	[JsonProperty("custom_fields", NullValueHandling = NullValueHandling.Ignore)]
	public Dictionary<string, CustomField> CustomFields => Model.CustomFields?.Where((ICustomField x) => CustomField.HasValue(x))?.ToDictionary((ICustomField x) => x.Name, (ICustomField x) => new CustomField(x));

	public Trigger(ITrigger model)
	{
		Model = model;
	}

	private string GetAction()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (Model.Before)
		{
			stringBuilder.Append("Before ");
		}
		if (Model.After)
		{
			stringBuilder.Append("After ");
		}
		if (Model.InsteadOf)
		{
			stringBuilder.Append("Instead Of ");
		}
		if (Model.OnInsert)
		{
			stringBuilder.Append("Insert ");
		}
		if (Model.OnUpdate)
		{
			stringBuilder.Append("Update ");
		}
		if (Model.OnDelete)
		{
			stringBuilder.Append("Delete ");
		}
		return stringBuilder.ToString();
	}
}
