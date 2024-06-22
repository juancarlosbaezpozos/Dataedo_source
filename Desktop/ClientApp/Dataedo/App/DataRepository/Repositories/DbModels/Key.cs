using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Tables.Constraints;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class Key : IKey, IModel, ICloneable, ICustomFieldsContainer
{
	private DbRepository repository;

	private Lazy<IList<ICustomField>> customFields;

	public int Id { get; private set; }

	public string Name { get; private set; }

	public string Description { get; private set; }

	public bool IsPk { get; private set; }

	public UserTypeEnum.UserType Source { get; private set; }

	public IEnumerable<PathName> Columns { get; private set; }

	public IList<ICustomField> CustomFields => customFields.Value;

	public Key(DbRepository repository, UniqueConstraintWithColumnObject row)
	{
		Key key = this;
		this.repository = repository;
		Id = row.UniqueConstraintId;
		Name = row.Name;
		Description = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Description) ? row.Description : null);
		IsPk = row.PrimaryKey;
		Source = UserTypeEnum.ObjectToType(row.Source).GetValueOrDefault();
		Columns = new PathName[1]
		{
			new PathName(row.ColumnPath, (string.IsNullOrEmpty(row.ColumnTitle) || !this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Title)) ? row.ColumnName : (row.ColumnName + " (" + row.ColumnTitle + ")"), ColumnNames.GetFullName(row.ColumnPath, row.ColumnName, row.ColumnTitle))
		};
		customFields = new Lazy<IList<ICustomField>>(() => ((IEnumerable<ICustomField>)(from x in key.repository.GetCustomFields()
			where x.KeyVisibility
			select new CustomField(key.repository, row, x))).ToList());
	}

	public void SetColumns(IEnumerable<IKey> keys)
	{
		Columns = keys.Where((IKey x) => x.Columns != null).SelectMany((IKey y) => y.Columns).ToArray();
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
