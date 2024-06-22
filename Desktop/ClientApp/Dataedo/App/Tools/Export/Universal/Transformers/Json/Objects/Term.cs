using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Naming;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects;

internal class Term : IObject
{
	internal Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Term MenuTreeItem;

	internal ITerm Model;

	public string ObjectId => MenuTreeItem.ObjectId;

	[JsonProperty("name")]
	public string Name => NameBuilder.For(Model).WithSchema().WithTitle()
		.Build();

	[JsonProperty("type")]
	public string Type => "term";

	[JsonProperty("subtype")]
	public string Subtype => BusinessGlossarySupport.GetTermIconName(Model.TypeIconId, string.Empty);

	public string DisplayType => char.ToUpper(Subtype[0]) + Subtype.Substring(1);

	[JsonProperty("description")]
	public string Description => Model.Description;

	[JsonProperty("summary")]
	public IList<SummaryField> Summary { get; private set; }

	[JsonProperty("related_terms")]
	public IEnumerable<TermRelation> RelatedTerms { get; private set; }

	[JsonProperty("data_links")]
	public IEnumerable<TermDataLink> DataLinks { get; private set; }

	public Term(Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree.Term menuTreeItem)
	{
		MenuTreeItem = menuTreeItem;
		Model = menuTreeItem.Model;
		SetSummaryTable();
		RelatedTerms = Model?.RelatedTerms?.Select((ITermRelation x) => new TermRelation(x));
		DataLinks = Model?.DataLinks?.Select((ITermDataLink x) => new TermDataLink(x));
	}

	private void SetSummaryTable()
	{
		Summary = new List<SummaryField>
		{
			new SummaryField("Glossary", new SummaryFieldLinkValue(LinkGenerator.ToBusinessGlossaryObjectId(Model.BusinessGlossary.Id), NameBuilder.For(Model.BusinessGlossary).Full().Build()))
		};
		if (!string.IsNullOrEmpty(Model.Title))
		{
			Summary.Add(new SummaryField("Title", Model.Title));
		}
		Summary.Add(new SummaryField("Type", DisplayType));
		foreach (ICustomField item in Model.CustomFields.Where((ICustomField x) => CustomField.HasValue(x)))
		{
			SummaryFieldCustomFieldValue customField = new SummaryFieldCustomFieldValue(item);
			Summary.Add(new SummaryField(item.Name, customField));
		}
	}
}
