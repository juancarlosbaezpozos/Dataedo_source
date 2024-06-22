using Dataedo.App.DataRepository.Models;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class RelationConstraint
{
	internal IRelationConstraint Model;

	[JsonProperty("primary_column_path")]
	public string PrimaryColumnPath => Model.PrimaryColumnPath;

	[JsonProperty("primary_column")]
	public string PrimaryColumnName => Model.PrimaryColumnName;

	[JsonProperty("foreign_column_path")]
	public string ForeignColumnPath => Model.ForeignColumnPath;

	[JsonProperty("foreign_column")]
	public string ForeignColumnName => Model.ForeignColumnName;

	public RelationConstraint(IRelationConstraint model)
	{
		Model = model;
	}
}
