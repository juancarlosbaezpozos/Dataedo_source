using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.App.Tools.Export;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.BusinessGlossary;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class Term : ITerm, IModel, ICloneable, ICustomFieldsContainer
{
	private DbRepository repository;

	private Lazy<IBusinessGlossary> businessGlossary;

	private Lazy<IList<ICustomField>> customFields;

	private Lazy<IList<ITerm>> terms;

	private Lazy<IList<ITermRelation>> relatedTerms;

	private Lazy<IList<ITermDataLink>> dataLinks;

	public int Id { get; private set; }

	public int BusinessGlossaryId { get; private set; }

	public string Title { get; private set; }

	public string Description { get; private set; }

	public int? TypeIconId { get; private set; }

	public IBusinessGlossary BusinessGlossary => businessGlossary.Value;

	public IList<ICustomField> CustomFields => customFields.Value;

	public IList<ITerm> Terms => terms.Value;

	public IList<ITermRelation> RelatedTerms => relatedTerms.Value;

	public IList<ITermDataLink> DataLinks => dataLinks.Value;

	public Term(DbRepository repository, TermObject row)
	{
		Term term = this;
		this.repository = repository;
		Id = row.TermId.Value;
		BusinessGlossaryId = row.DatabaseId.Value;
		Title = row.Title;
		Description = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Description) ? row.Description : null);
		TypeIconId = row.TypeIconId;
		customFields = new Lazy<IList<ICustomField>>(() => ((IEnumerable<ICustomField>)(from x in term.repository.GetCustomFields()
			where x.TermVisibility
			select new CustomField(term.repository, row, x))).ToList());
		businessGlossary = new Lazy<IBusinessGlossary>(() => term.repository.GetBusinessGlossary(term.BusinessGlossaryId));
		terms = new Lazy<IList<ITerm>>(() => term.repository.GetChildrenTerms(term.Id));
		relatedTerms = new Lazy<IList<ITermRelation>>(() => term.repository.GetTermRelations(term.Id));
		dataLinks = new Lazy<IList<ITermDataLink>>(() => term.repository.GetTermDataLinks(term.Id));
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
