using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Naming;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;
using Dataedo.Shared.Enums;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects;

internal class View : IObject
{
	internal Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.View MenuTreeItem;

	internal IView Model;

	[JsonProperty("columns_custom_fields")]
	public IList<string> ColumnsCustomFields;

	[JsonProperty("relations_custom_fields")]
	public IList<string> RelationsCustomFields;

	[JsonProperty("triggers_custom_fields")]
	public IList<string> TriggersCustomFields;

	public string ObjectId => MenuTreeItem.ObjectId;

	[JsonProperty("name")]
	public string Name => NameBuilder.For(Model).WithSchema().WithTitle()
		.Build();

	[JsonProperty("subtype")]
	public string Subtype => SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.View, Model.Subtype);

	[JsonProperty("is_user_defined")]
	public bool IsUserDefined => Model.Source == UserTypeEnum.UserType.USER;

	[JsonProperty("description")]
	public string Description => Model.Description;

	[JsonProperty("script")]
	public string Script => Model.Script;

	[JsonProperty("summary")]
	public IList<SummaryField> Summary { get; private set; }

	[JsonProperty("columns")]
	public IEnumerable<Column> Columns { get; private set; }

	[JsonProperty("relations")]
	public IEnumerable<Relation> Relations { get; private set; }

	[JsonProperty("triggers")]
	public IEnumerable<Trigger> Triggers { get; private set; }

	[JsonProperty("unique_keys")]
	public IEnumerable<Key> Keys { get; private set; }

	[JsonProperty("dependencies")]
	public Dependencies Dependencies { get; private set; }

	[JsonProperty("imported_at")]
	public string ImportedAt => Model.ImportedAt?.ToString("yyyy-MM-dd HH:mm");

	public View(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.View menuTreeItem)
	{
		IDocumentation contextDocumentation = ((menuTreeItem.Parent is Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Module) ? (menuTreeItem.Parent as Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Module).Model.Documentation : (menuTreeItem.Parent as Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Documentation).Model);
		MenuTreeItem = menuTreeItem;
		Model = menuTreeItem.Model;
		SetSummaryTable();
		Columns = Model.Columns?.Select((IColumn x) => new Column(x, contextDocumentation));
		Relations = Model.Relations?.Select((IRelation x) => new Relation(x, contextDocumentation));
		Keys = Model.Keys?.Select((IKey x) => new Key(x));
		Triggers = Model.Triggers?.Select((ITrigger x) => new Trigger(x));
		Dependencies = ((Model.Dependencies != null) ? new Dependencies(Model.Dependencies) : null);
		ColumnsCustomFields = CustomField.GetUniqueNamesWithValue(ObjectTools.FlattenColumns(Columns), (object x) => ((Column)x).Model.CustomFields);
		RelationsCustomFields = CustomField.GetUniqueNamesWithValue(Relations, (object x) => ((Relation)x).Model.CustomFields);
		TriggersCustomFields = CustomField.GetUniqueNamesWithValue(Triggers, (object x) => ((Trigger)x).Model.CustomFields);
	}

	private void SetSummaryTable()
	{
		Summary = new List<SummaryField>
		{
			new SummaryField("Documentation", new SummaryFieldLinkValue($"d{Model.Documentation.Id}", NameBuilder.For(Model.Documentation).Full().Build())),
			new SummaryField("Schema", Model.Schema),
			new SummaryField("Name", Model.Name),
			new SummaryField("Type", SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.View, Model.Subtype))
		};
		if (!string.IsNullOrEmpty(Model.Title))
		{
			Summary.Add(new SummaryField("Title", Model.Title));
		}
		SummaryFieldArrayValue summaryFieldArrayValue = new SummaryFieldArrayValue();
		foreach (IModule module in Model.Modules)
		{
			summaryFieldArrayValue.Add(new SummaryFieldLinkValue(LinkGenerator.ToModuleObjectId(module.Id), module.Title));
		}
		if (summaryFieldArrayValue.Count() > 0)
		{
			Summary.Add(new SummaryField("Subject Area", summaryFieldArrayValue));
		}
		foreach (ICustomField item in Model.CustomFields.Where((ICustomField x) => CustomField.HasValue(x)))
		{
			SummaryFieldCustomFieldValue customField = new SummaryFieldCustomFieldValue(item);
			Summary.Add(new SummaryField(item.Name, customField));
		}
		IEnumerable<LinkedTerm> enumerable = Model.LinkedTerms?.Select((ITerm x) => new LinkedTerm(x));
		if (enumerable != null && enumerable.Count() > 0)
		{
			SummaryFieldLinkedTermsValue linkedTerms = new SummaryFieldLinkedTermsValue(enumerable);
			Summary.Add(new SummaryField("Linked terms", linkedTerms));
		}
	}
}
