using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Naming;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;
using Dataedo.Shared.Enums;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects;

internal class Procedure : IObject
{
	internal Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Procedure MenuTreeItem;

	internal IProcedure Model;

	[JsonProperty("parameters_custom_fields")]
	public IList<string> ParametersCustomFields;

	public string ObjectId => MenuTreeItem.ObjectId;

	[JsonProperty("name")]
	public string Name => NameBuilder.For(Model).WithSchema().WithTitle()
		.Build();

	[JsonProperty("subtype")]
	public string Subtype => SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Procedure, Model.Subtype);

	[JsonProperty("is_user_defined")]
	public bool IsUserDefined => Model.Source == UserTypeEnum.UserType.USER;

	[JsonProperty("description")]
	public string Description => Model.Description;

	[JsonProperty("summary")]
	public IList<SummaryField> Summary { get; private set; }

	[JsonProperty("script")]
	public string Script => Model.Script;

	[JsonProperty("parameters")]
	public Parameter[] Parameters { get; private set; }

	[JsonProperty("dependencies")]
	public Dependencies Dependencies { get; private set; }

	[JsonProperty("imported_at")]
	public string ImportedAt => Model.ImportedAt?.ToString("yyyy-MM-dd HH:mm");

	public Procedure(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Procedure menuTreeItem)
	{
		MenuTreeItem = menuTreeItem;
		Model = menuTreeItem.Model;
		SetSummaryTable();
		Parameters = Model.Parameters?.Select((IParameter x) => new Parameter(x)).ToArray();
		Dependencies = ((Model.Dependencies != null) ? new Dependencies(Model.Dependencies) : null);
		ParametersCustomFields = CustomField.GetUniqueNamesWithValue(Parameters, (object x) => ((Parameter)x).Model.CustomFields);
	}

	private void SetSummaryTable()
	{
		Summary = new List<SummaryField>
		{
			new SummaryField("Documentation", new SummaryFieldLinkValue($"d{Model.Documentation.Id}", NameBuilder.For(Model.Documentation).Full().Build())),
			new SummaryField("Schema", Model.Schema),
			new SummaryField("Name", Model.Name),
			new SummaryField("Type", SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Procedure, Model.Subtype))
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
	}
}
