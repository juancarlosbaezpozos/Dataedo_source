using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Naming;
using Dataedo.App.Enums;
using Dataedo.Shared.Enums;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class Relation
{
	internal IRelation Model;

	internal IDocumentation ContextDocumentation;

	[JsonProperty("name")]
	public string Name => Model.Name;

	[JsonProperty("title")]
	public string Title => Model.Title;

	[JsonProperty("description")]
	public string Description => Model.Description;

	[JsonProperty("is_user_defined")]
	public bool IsUserDefined => Model.Source == UserTypeEnum.UserType.USER;

	[JsonProperty("foreign_table")]
	public string ForeignTable => NameBuilder.For(Model.ForeignModel).WithSchema().Build();

	[JsonProperty("foreign_table_show_schema")]
	public string ForeignTableShowSchema => NameBuilder.For(Model.ForeignModel).WithSchema().Build(null, showSchema: true);

	[JsonProperty("foreign_table_verbose")]
	public string ForeignTableVerbose => NameBuilder.For(Model.ForeignModel).Full().Build(Model.Documentation);

	[JsonProperty("foreign_table_verbose_show_schema")]
	public string ForeignTableVerboseShowSchema => NameBuilder.For(Model.ForeignModel).Full().Build(Model.Documentation, showSchema: true);

	[JsonProperty("foreign_table_object_id", NullValueHandling = NullValueHandling.Ignore)]
	public string ForeignTableId => LinkGenerator.ToObjectId(Model.ForeignModelType, Model.ForeignModel.Id);

	[JsonProperty("primary_table")]
	public string PrimaryTable => NameBuilder.For(Model.PrimaryModel).WithSchema().Build();

	[JsonProperty("primary_table_show_schema")]
	public string PrimaryTableShowSchema => NameBuilder.For(Model.PrimaryModel).WithSchema().Build(null, showSchema: true);

	[JsonProperty("primary_table_verbose")]
	public string PrimaryTableVerbose => NameBuilder.For(Model.PrimaryModel).Full().Build(Model.Documentation);

	[JsonProperty("primary_table_verbose_show_schema")]
	public string PrimaryTableVerboseShowSchema => NameBuilder.For(Model.PrimaryModel).Full().Build(Model.Documentation, showSchema: true);

	[JsonProperty("primary_table_object_id", NullValueHandling = NullValueHandling.Ignore)]
	public string PrimaryTableId => LinkGenerator.ToObjectId(Model.PrimaryModelType, Model.PrimaryModel.Id);

	[JsonProperty("pk_cardinality", NullValueHandling = NullValueHandling.Ignore)]
	public string PkCardinality => CardinalityTypeEnum.TypeToId(Model.PrimaryCardinality);

	[JsonProperty("fk_cardinality", NullValueHandling = NullValueHandling.Ignore)]
	public string FkCardinality => CardinalityTypeEnum.TypeToId(Model.ForeignCardinality);

	[JsonProperty("constraints")]
	public IEnumerable<RelationConstraint> Constraints { get; private set; }

	[JsonProperty("custom_fields", NullValueHandling = NullValueHandling.Ignore)]
	public Dictionary<string, CustomField> CustomFields => Model.CustomFields?.Where((ICustomField x) => CustomField.HasValue(x))?.ToDictionary((ICustomField x) => x.Name, (ICustomField x) => new CustomField(x));

	public Relation(IRelation model)
	{
		Model = model;
		Constraints = model.Constraints.Select((IRelationConstraint x) => new RelationConstraint(x));
	}

	public Relation(IRelation model, IDocumentation contextDocumentation)
		: this(model)
	{
		ContextDocumentation = contextDocumentation;
	}
}
