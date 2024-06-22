using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class Column : Dataedo.App.DataRepository.Models.IColumn, IModel, ICloneable, ICustomFieldsContainer, IHasLinkedTerms
{
	private DbRepository repository;

	private Lazy<IList<ICustomField>> customFields;

	private Lazy<IList<ITerm>> linkedTerms;

	public int Id { get; private set; }

	public int TableId { get; private set; }

	public string Name { get; private set; }

	public string NameWithoutPath { get; private set; }

	public string Title { get; private set; }

	public string DisplayName
	{
		get
		{
			if (string.IsNullOrEmpty(Title))
			{
				return Name;
			}
			return Name + " (" + Title + ")";
		}
	}

	public string DisplayNameWithoutPath
	{
		get
		{
			if (string.IsNullOrEmpty(Title))
			{
				return NameWithoutPath;
			}
			return NameWithoutPath + " (" + Title + ")";
		}
	}

	public string Description { get; private set; }

	public int? OrdinalPosition { get; private set; }

	public string DataType { get; private set; }

	public string DataLength { get; private set; }

	public string DisplayDataType
	{
		get
		{
			if (string.IsNullOrEmpty(DataLength))
			{
				return DataType;
			}
			return DataType + "(" + DataLength + ")";
		}
	}

	public bool IsPrimaryKey { get; private set; }

	public bool? IsNullable { get; private set; }

	public bool? IsIdentity { get; private set; }

	public string ComputedFormula { get; private set; }

	public string DefaultValue { get; private set; }

	public UserTypeEnum.UserType Source { get; private set; }

	public int? ParentId { get; set; }

	public string Path { get; set; }

	public int Level { get; set; }

	public string ItemType { get; set; }

	public IEnumerable<IKey> Keys { get; private set; }

	public IEnumerable<IRelation> References { get; private set; }

	public IEnumerable<Dataedo.App.DataRepository.Models.IColumn> Children { get; private set; }

	public IList<ICustomField> CustomFields => customFields.Value;

	public IList<ITerm> LinkedTerms => linkedTerms.Value;

	public Column(DbRepository repository, ColumnObjectExtended<ColumnObject> row, IList<IKey> keys, IList<Relation> references)
	{
		Column column = this;
		this.repository = repository;
		Id = row.ColumnObject.ColumnId;
		TableId = row.ColumnObject.TableId;
		Name = ColumnNames.GetFullName(row.ColumnObject.Path, row.ColumnObject.Name);
		NameWithoutPath = row.ColumnObject.Name;
		Title = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Title) ? row.ColumnObject.Title : null);
		Description = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Description) ? row.ColumnObject.Description : null);
		OrdinalPosition = row.ColumnObject.OrdinalPosition;
		DataType = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.DataType) ? row.ColumnObject.Datatype : null);
		DataLength = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.DataType) ? row.ColumnObject.DataLength : null);
		IsPrimaryKey = keys != null && row.ColumnObject.PrimaryKey;
		IsNullable = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Nullable) ? new bool?(row.ColumnObject.Nullable) : null);
		IsIdentity = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Identity) ? new bool?(row.ColumnObject.IsIdentity) : null);
		ComputedFormula = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.DefaultComputed) ? row.ColumnObject.ComputedFormula : null);
		DefaultValue = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.DefaultComputed) ? row.ColumnObject.DefaultValue : null);
		Source = UserTypeEnum.ObjectToType(row.ColumnObject.Source).GetValueOrDefault();
		ParentId = row.ColumnObject.ParentId;
		Path = row.ColumnObject.Path;
		Level = row.ColumnObject.Level;
		ItemType = row.ColumnObject.ItemType;
		if (ItemType == SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Column, SharedObjectSubtypeEnum.ObjectSubtype.Dimension) || ItemType == SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Column, SharedObjectSubtypeEnum.ObjectSubtype.Object))
		{
			ItemType = "COLUMN_" + ItemType;
		}
		Keys = keys?.Where((IKey x) => x.Columns.Any((PathName y) => y.Name == column.Name)) ?? new IKey[0];
		References = references?.Where((Relation x) => x.ForeignModel.Id == column.TableId && x.Constraints.Where((IRelationConstraint y) => ColumnNames.GetFullName(y.ForeignColumnPath, y.ForeignColumnName) == column.Name).Count() > 0) ?? new Relation[0];
		Children = row.Subcolumns?.Select((ColumnObjectExtended<ColumnObject> x) => new Column(repository, x, keys, references));
		customFields = new Lazy<IList<ICustomField>>(() => ((IEnumerable<ICustomField>)(from x in column.repository.GetCustomFields()
			where x.ColumnVisibility
			select new CustomField(column.repository, row.ColumnObject, x))).ToList());
		linkedTerms = new Lazy<IList<ITerm>>(() => column.repository.GetColumnLinkedTerms(column.TableId, column.Id));
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
