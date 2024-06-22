using System;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.Tools.Export;
using Dataedo.Model.Data.BusinessGlossary;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class TermRelation : ITermRelation, IModel, ICloneable
{
	private DbRepository repository;

	private Lazy<ITerm> relatedTerm;

	public int Id { get; private set; }

	public string Relationship { get; private set; }

	public string Description { get; private set; }

	public ITerm RelatedTerm => relatedTerm.Value;

	public TermRelation(DbRepository repository, TermRelationshipObjectExtended row)
	{
		TermRelation termRelation = this;
		this.repository = repository;
		Id = row.ObjectId;
		Relationship = row.RelationshipTitle;
		Description = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Description) ? row.Description : null);
		relatedTerm = new Lazy<ITerm>(() => termRelation.repository.GetTerm(row.RelatedTermId));
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
