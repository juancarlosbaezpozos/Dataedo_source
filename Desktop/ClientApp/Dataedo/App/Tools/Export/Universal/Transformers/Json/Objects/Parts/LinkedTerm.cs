using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Naming;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class LinkedTerm
{
	internal ITerm Model;

	[JsonProperty("id")]
	public string Id => LinkGenerator.ToTermObjectId(Model.Id);

	[JsonProperty("name")]
	public string Name => NameBuilder.For(Model).WithSchema().WithTitle()
		.Build();

	[JsonProperty("type")]
	public string Type => "term";

	[JsonProperty("subtype")]
	public string Subtype => BusinessGlossarySupport.GetTermIconName(Model.TypeIconId, string.Empty);

	public LinkedTerm(ITerm model)
	{
		Model = model;
	}
}
