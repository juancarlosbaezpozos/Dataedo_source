using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Naming;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class ColumnReference
{
	internal IRelation Model;

	internal IDocumentation ContextDocumentation;

	[JsonProperty("id")]
	public string ObjectId => LinkGenerator.ToObjectId(Model.PrimaryModelType, Model.PrimaryModel.Id);

	[JsonProperty("name")]
	public string Name => NameBuilder.For(Model.PrimaryModel).Full().Build(ContextDocumentation);

	[JsonProperty("name_show_schema")]
	public string NameShowSchema => NameBuilder.For(Model.PrimaryModel).Full().Build(null, showSchema: true);

	public ColumnReference(IRelation model, IDocumentation contextDocumentation)
	{
		Model = model;
		ContextDocumentation = contextDocumentation;
	}
}
