using Dataedo.App.DataRepository.Models;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class TermRelation
{
	internal ITermRelation Model;

	[JsonProperty("id")]
	public string Id => LinkGenerator.ToTermObjectId(Model.Id);

	[JsonProperty("name")]
	public string Name => Model.RelatedTerm.Title;

	[JsonProperty("type")]
	public string Type => "term";

	[JsonProperty("subtype")]
	public string Subtype => BusinessGlossarySupport.GetTermIconName(Model.RelatedTerm.TypeIconId, string.Empty);

	[JsonProperty("relationship")]
	public string Relationship => Model.Relationship;

	[JsonProperty("relationship_description")]
	public string RelationshipDescription => Model.Description;

	public TermRelation(ITermRelation model)
	{
		Model = model;
	}
}
