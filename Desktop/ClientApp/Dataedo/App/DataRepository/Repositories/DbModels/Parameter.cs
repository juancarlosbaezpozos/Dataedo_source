using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.App.Tools.Export;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Procedures.Parameters;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class Parameter : IParameter, IModel, ICloneable, ICustomFieldsContainer
{
	private DbRepository repository;

	private Lazy<IList<ICustomField>> customFields;

	public int Id { get; private set; }

	public int ParentObjectId { get; private set; }

	public string Name { get; private set; }

	public string Description { get; private set; }

	public string Mode { get; private set; }

	public string DataType { get; private set; }

	public IList<ICustomField> CustomFields => customFields.Value;

	public Parameter(DbRepository repository, ParameterObject row)
	{
		Parameter parameter = this;
		this.repository = repository;
		Id = row.ParameterId;
		Name = row.Name;
		Description = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Description) ? row.Description : null);
		Mode = row.ParameterMode;
		DataType = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.DataType) ? row.Datatype : null);
		customFields = new Lazy<IList<ICustomField>>(() => ((IEnumerable<ICustomField>)(from x in parameter.repository.GetCustomFields()
			where x.ParameterVisibility
			select new CustomField(parameter.repository, row, x))).ToList());
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
