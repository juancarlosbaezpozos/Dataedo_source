using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.Shared.Enums;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class Column
{
	internal IColumn Model;

	internal IDocumentation ContextDocumentation;

	[JsonProperty("id")]
	public string Id => $"column-{Model.Id}";

	[JsonProperty("object_id")]
	public string ObjectId => Id;

	[JsonProperty("name")]
	public string Name => Model.DisplayName;

	[JsonProperty("name_without_path")]
	public string NameWithoutPath => Model.DisplayNameWithoutPath;

	[JsonProperty("description")]
	public string Description => Model.Description;

	[JsonProperty("is_pk")]
	public bool IsPK => Model.IsPrimaryKey;

	[JsonProperty("is_identity")]
	public bool? IsIdentity => Model.IsIdentity;

	[JsonProperty("data_type")]
	public string DataType => Model.DataType;

	[JsonProperty("data_length")]
	public string DataLength => Model.DataLength;

	[JsonProperty("is_nullable")]
	public bool? IsNullable => Model.IsNullable;

	[JsonProperty("computed_formula")]
	public string ComputedFormula => Model.ComputedFormula;

	[JsonProperty("default_value")]
	public string DefaultValue => Model.DefaultValue;

	[JsonProperty("path")]
	public string Path => Model.Path;

	[JsonProperty("level")]
	public int Level => Model.Level;

	[JsonProperty("item_type")]
	public string ItemType => Model.ItemType;

	[JsonProperty("is_user_defined")]
	public bool IsUserDefined => Model.Source == UserTypeEnum.UserType.USER;

	[JsonProperty("children", NullValueHandling = NullValueHandling.Ignore)]
	public IEnumerable<Column> Children { get; private set; }

	[JsonProperty("custom_fields", NullValueHandling = NullValueHandling.Ignore)]
	public Dictionary<string, CustomField> CustomFields => Model.CustomFields?.Where((ICustomField x) => CustomField.HasValue(x))?.ToDictionary((ICustomField x) => x.Name, (ICustomField x) => new CustomField(x));

	[JsonProperty("linked_terms")]
	public IEnumerable<LinkedTerm> LinkedTerms { get; private set; }

	[JsonProperty("references")]
	public IEnumerable<ColumnReference> References => Model.References.Select((IRelation x) => new ColumnReference(x, ContextDocumentation));

	public Column(IColumn model, IDocumentation contextDocumentation)
	{
		Model = model;
		ContextDocumentation = contextDocumentation;
		LinkedTerms = Model.LinkedTerms?.Select((ITerm x) => new LinkedTerm(x));
		Children = model.Children?.Select((IColumn x) => new Column(x, contextDocumentation));
	}
}
