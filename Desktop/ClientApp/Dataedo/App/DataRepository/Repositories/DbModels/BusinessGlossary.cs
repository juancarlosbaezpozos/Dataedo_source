using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.App.Tools.Export;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.BusinessGlossary;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class BusinessGlossary : IBusinessGlossary, IModel, ICloneable, ICustomFieldsContainer
{
	private DbRepository repository;

	private Lazy<IList<ICustomField>> customFields;

	private Lazy<IList<ITerm>> terms;

	public int Id { get; private set; }

	public int DocumentationId { get; private set; }

	public string Title { get; private set; }

	public string Description { get; private set; }

	public IList<ICustomField> CustomFields => customFields.Value;

	public IList<ITerm> Terms => terms.Value;

	public BusinessGlossary(DbRepository repository, BusinessGlossaryObject row)
	{
		BusinessGlossary businessGlossary = this;
		this.repository = repository;
		Id = row.DocumentationId.Value;
		Title = row.Title;
		Description = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Description) ? row.Description : null);
		customFields = new Lazy<IList<ICustomField>>(() => ((IEnumerable<ICustomField>)(from x in businessGlossary.repository.GetCustomFields()
			where x.DocumentationVisibility
			select new CustomField(businessGlossary.repository, row, x))).ToList());
		terms = new Lazy<IList<ITerm>>(() => businessGlossary.repository.GetRootTerms(businessGlossary.Id));
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
