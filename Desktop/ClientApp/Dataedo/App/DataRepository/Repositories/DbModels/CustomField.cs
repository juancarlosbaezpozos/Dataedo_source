using System;
using Dataedo.App.DataRepository.Models;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Common.CustomFields;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class CustomField : ICustomField, IModel, ICloneable
{
	private DbRepository repository;

	public int Id { get; private set; }

	public string Name { get; private set; }

	public string Value { get; private set; }

	public CustomFieldTypeEnum.CustomFieldType? Type { get; private set; }

	public CustomField(DbRepository repository, CustomFieldsData row, CustomFieldRowExtended field)
	{
		this.repository = repository;
		Id = field.CustomFieldId;
		Name = field.Title ?? field.Code;
		Value = PrepareValue.ToString(row.GetField(field.FieldName));
		Type = field.Type;
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
