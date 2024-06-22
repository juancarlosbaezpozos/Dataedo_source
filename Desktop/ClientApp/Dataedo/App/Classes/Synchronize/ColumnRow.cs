using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using Dataedo.App.Classes.Synchronize.Tools;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;
using DevExpress.XtraEditors.DXErrorProvider;

namespace Dataedo.App.Classes.Synchronize;

[DebuggerDisplay("{FullName} ({UniqueParentId}.{UniqueId}) (Level: {Level})")]
public class ColumnRow : BasicRow, IDXDataErrorInfo
{
	public static int UniqueIdSource;

	private string _tableSchema;

	public int UniqueId { get; set; }

	public int? UniqueParentId => ParentColumn?.UniqueId;

	public int NodeLevelPosition { get; set; }

	public int? DocumentationId { get; set; }

	public string DocumentationTitle { get; set; }

	public bool IsSortChanged { get; set; }

	public string LastModificationDateString { get; set; }

	public string CreationDateString { get; set; }

	public string CreatedBy { get; set; }

	public string ModifiedBy { get; set; }

	public string DatabaseName { get; set; }

	public string TableName { get; set; }

	public string TableSchema
	{
		get
		{
			return _tableSchema;
		}
		set
		{
			_tableSchema = ((value == null) ? string.Empty : value);
		}
	}

	public int? TableId { get; set; }

	public string TableType { get; set; }

	public string DataType { get; set; }

	public string DataTypeWithoutLength { get; set; }

	public int? Position { get; set; }

	public string ConstraintType { get; set; }

	public string DataLength { get; set; }

	public bool Nullable { get; set; }

	public string DefaultValue { get; set; }

	public bool IsIdentity { get; set; }

	public bool IsComputed { get; set; }

	public string ComputedFormula { get; set; }

	public bool IsPk { get; set; }

	public string DefaultDef { get; set; }

	public string IdentityDef { get; set; }

	public bool AlreadyExists { get; set; }

	public bool IsNameEmpty { get; set; }

	public int Sort { get; set; }

	public int? ParentId { get; set; }

	public string Path { get; set; }

	public string PathFormatted => ColumnNames.GetFullNameFormatted(Path, null);

	public int Level { get; set; } = 1;


	public string Type { get; set; } = "COLUMN";


	public string SparklineRowDistributionText { get; set; } = "";


	public SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype => SharedObjectSubtypeEnum.StringToType(base.ObjectType, Type);

	public ColumnReferenceDataContainer ReferencesDataContainer { get; set; }

	public ColumnUniqueConstraintWithFkDataContainer UniqueConstraintsDataContainer { get; set; }

	public DataLinkDataContainer DataLinksDataContainer { get; set; }

	public string ReferencesString => ReferencesDataContainer?.ReferencesString;

	public string ReferencesStringCommaDelimited => ReferencesDataContainer?.ReferencesStringCommaDelimited;

	public string DataLinksString => DataLinksDataContainer?.DataLinksString;

	public Image UniqueConstraintIcon => UniqueConstraintsDataContainer?.FirstItemKeysIcon;

	public Image UniqueConstraintOrFkIcon => UniqueConstraintsDataContainer?.FirstItemIcon;

	public ColumnRow ParentColumn { get; set; }

	public BindingList<ColumnRow> Subcolumns { get; set; }

	public string DisplayPosition
	{
		get
		{
			if (Level != 1 && !string.IsNullOrEmpty(ParentColumn?.DisplayPosition))
			{
				if (string.IsNullOrEmpty(ParentColumn?.DisplayPosition) || !Position.HasValue)
				{
					return null;
				}
				return $"{ParentColumn?.DisplayPosition}.{Position}";
			}
			return Position.ToString();
		}
	}

	public int SubcolumnsCount => Subcolumns?.Count ?? 0;

	public string FullName => ColumnNames.GetFullName(Path, base.Name);

	public string FullNameWithTitle => ColumnNames.GetFullName(Path, base.Name, base.Title);

	public string FullNameFormatted => ColumnNames.GetFullNameFormatted(Path, base.Name);

	public string FullNameFormattedWithTitle => ColumnNames.GetFullNameFormatted(Path, base.Name, base.Title);

	public new string Description { get; set; }

	public new bool ObjectIsAddedImportHistory { get; set; }

	public Bitmap Icon => IconsSupport.GetObjectIcon(base.ObjectType, ObjectSubtype, base.Source);

	public ColumnRow(ColumnWithReferenceObject row)
		: base(row)
	{
		base.Id = row.ColumnId;
		base.Name = row.Name;
		base.Title = row.Title;
		Position = row.OrdinalPosition;
		Description = row.Description;
		IsPk = row.PrimaryKey;
		DataLength = row.DataLength;
		DataType = (string.IsNullOrEmpty(DataLength) ? row.Datatype : row.DatatypeLen);
		DataTypeWithoutLength = row.Datatype;
		LastModificationDateString = PrepareValue.SetDateTimeWithFormatting(row.LastModificationDate);
		ModifiedBy = row.ModifiedBy;
		CreationDateString = PrepareValue.SetDateTimeWithFormatting(row.CreationDate);
		CreatedBy = row.CreatedBy;
		Nullable = row.Nullable;
		DefaultValue = row.DefaultValue;
		IsIdentity = row.IsIdentity;
		IsComputed = row.IsComputed;
		ComputedFormula = row.ComputedFormula;
		base.Source = UserTypeEnum.ObjectToType(row.Source) ?? UserTypeEnum.UserType.DBMS;
		Sort = row.Sort;
		ParentId = row.ParentId;
		Path = row.Path;
		Level = row.Level;
		Type = row.ItemType;
		ReferencesDataContainer = new ColumnReferenceDataContainer();
		UniqueConstraintsDataContainer = new ColumnUniqueConstraintWithFkDataContainer(new ColumnUniqueConstraintWithFkData(row));
		DataLinksDataContainer = new DataLinkDataContainer(new DataLinkData(row));
		ReferencesDataContainer = new ColumnReferenceDataContainer();
		UniqueConstraintsDataContainer = new ColumnUniqueConstraintWithFkDataContainer(new ColumnUniqueConstraintWithFkData(row));
		DataLinksDataContainer = new DataLinkDataContainer(new DataLinkData(row));
		base.ObjectType = SharedObjectTypeEnum.ObjectType.Column;
	}

	public ColumnRow()
	{
		base.ObjectType = SharedObjectTypeEnum.ObjectType.Column;
	}

	public ColumnRow(ColumnObject row, CustomFieldsSupport customFieldsSupport)
	{
		base.Id = row.ColumnId;
		base.Name = row.Name;
		base.Title = row.Title;
		Position = row.OrdinalPosition;
		Description = row.Description;
		IsPk = row.PrimaryKey;
		DataType = row.DatatypeLen;
		DataTypeWithoutLength = row.Datatype;
		DataLength = row.DataLength;
		LastModificationDateString = PrepareValue.SetDateTimeWithFormatting(row.LastModificationDate);
		ModifiedBy = row.ModifiedBy;
		CreationDateString = PrepareValue.SetDateTimeWithFormatting(row.CreationDate);
		CreatedBy = row.CreatedBy;
		Nullable = row.Nullable;
		DefaultValue = row.DefaultValue;
		IsIdentity = row.IsIdentity;
		IsComputed = row.IsComputed;
		ComputedFormula = row.ComputedFormula;
		base.Source = UserTypeEnum.ObjectToType(row.Source) ?? UserTypeEnum.UserType.DBMS;
		Sort = row.Sort;
		base.RowState = ManagingRowsEnum.ManagingRows.Added;
		ParentId = row.ParentId;
		Path = row.Path;
		Level = row.Level;
		Type = row.ItemType;
		base.ObjectType = SharedObjectTypeEnum.ObjectType.Column;
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
			base.CustomFields.RetrieveCustomFields(row);
		}
	}

	public ColumnRow(bool contextShowSchema, ObjectRow objectRow, ColumnWithReferenceObject row, CustomFieldsSupport customFieldsSupport)
		: this(row)
	{
		ReferencesDataContainer = new ColumnReferenceDataContainer(new ColumnReferenceData(contextShowSchema, objectRow, row));
		if (objectRow != null)
		{
			base.CustomFields = new CustomFieldContainer(SharedObjectTypeEnum.ObjectType.Column, base.Id, customFieldsSupport);
			TableId = objectRow.ObjectId;
			base.ParentObjectType = objectRow.ObjectTypeValueOrUnresolved;
		}
		else
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
		}
		base.CustomFields.RetrieveCustomFields(row);
		DocumentationId = objectRow.DatabaseId;
		DocumentationTitle = objectRow.DocumentationTitle;
	}

	public ColumnRow(ObjectRow objectRow, ColumnWithReferenceObject row, CustomFieldsSupport customFieldsSupport)
		: this(row)
	{
		ReferencesDataContainer = new ColumnReferenceDataContainer(new ColumnReferenceData(objectRow, row));
		if (objectRow != null)
		{
			base.CustomFields = new CustomFieldContainer(SharedObjectTypeEnum.ObjectType.Column, base.Id, customFieldsSupport);
			TableId = objectRow.ObjectId;
			base.ParentObjectType = objectRow.ObjectTypeValueOrUnresolved;
		}
		else
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
		}
		base.CustomFields.RetrieveCustomFields(row);
		DocumentationId = objectRow?.DatabaseId;
		DocumentationTitle = objectRow?.DocumentationTitle;
		Path = row.Path;
	}

	public ColumnRow(ColumnSynchronizationObject dataReader, CustomFieldsSupport customFieldsSupport)
	{
		base.Name = dataReader.Name;
		DatabaseName = dataReader.DatabaseName;
		TableName = dataReader.TableName;
		TableSchema = dataReader.TableSchema;
		DataType = dataReader.Datatype;
		Position = dataReader.Position;
		Description = dataReader.Description;
		ConstraintType = dataReader.ConstraintType;
		Nullable = dataReader.Nullable;
		DefaultValue = PrepareValue.ToStringWithoutBrackets(dataReader.DefaultValue);
		IsIdentity = dataReader.IsIdentity;
		IsComputed = dataReader.IsComputed;
		ComputedFormula = dataReader.ComputedFormula;
		DataLength = dataReader.DataLength;
		if (string.IsNullOrEmpty(DataLength))
		{
			DataLength = null;
		}
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
			base.CustomFields.RetrieveCustomFields(dataReader);
		}
		base.ObjectType = SharedObjectTypeEnum.ObjectType.Column;
	}

	public ColumnRow(ColumnSynchronizationForInterfaceTablesObject dataReader, CustomFieldsSupport customFieldsSupport)
		: this((ColumnSynchronizationObject)dataReader, customFieldsSupport)
	{
		Path = dataReader.Path;
		Level = dataReader.Level;
		Type = dataReader.Type;
	}

	public ColumnRow(ColumnSynchronizationForMongoDBObject dataReader, CustomFieldsSupport customFieldsSupport)
	{
		base.Name = dataReader.Name;
		DatabaseName = dataReader.DatabaseName;
		TableName = dataReader.TableName;
		TableSchema = dataReader.TableSchema;
		DataType = dataReader.Datatype;
		Position = dataReader.Position;
		Description = dataReader.Description;
		ConstraintType = dataReader.ConstraintType;
		Nullable = dataReader.Nullable;
		DefaultValue = PrepareValue.ToStringWithoutBrackets(dataReader.DefaultValue);
		IsIdentity = dataReader.IsIdentity;
		IsComputed = dataReader.IsComputed;
		ComputedFormula = dataReader.ComputedFormula;
		DataLength = dataReader.DataLength;
		if (string.IsNullOrEmpty(DataLength))
		{
			DataLength = null;
		}
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
			base.CustomFields.RetrieveCustomFields(dataReader);
		}
		base.ObjectType = SharedObjectTypeEnum.ObjectType.Column;
		Path = dataReader.Path;
		Level = dataReader.Level;
		Type = dataReader.Type;
	}

	public ColumnRow(ColumnSynchronizationForDynamoDBObject dataReader, CustomFieldsSupport customFieldsSupport)
	{
		base.Name = dataReader.Name;
		DatabaseName = dataReader.DatabaseName;
		TableName = dataReader.TableName;
		TableSchema = dataReader.TableSchema;
		DataType = dataReader.Datatype;
		Position = dataReader.Position;
		Description = dataReader.Description;
		ConstraintType = dataReader.ConstraintType;
		Nullable = dataReader.Nullable;
		DefaultValue = PrepareValue.ToStringWithoutBrackets(dataReader.DefaultValue);
		IsIdentity = dataReader.IsIdentity;
		IsComputed = dataReader.IsComputed;
		ComputedFormula = dataReader.ComputedFormula;
		DataLength = dataReader.DataLength;
		if (string.IsNullOrEmpty(DataLength))
		{
			DataLength = null;
		}
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
			base.CustomFields.RetrieveCustomFields(dataReader);
		}
		base.ObjectType = SharedObjectTypeEnum.ObjectType.Column;
		Path = dataReader.Path;
		Level = dataReader.Level;
		Type = dataReader.Type;
	}

	public ColumnRow(ColumnSynchronizationForElasticsearchObject dataReader, CustomFieldsSupport customFieldsSupport)
	{
		base.Name = dataReader.Name;
		DatabaseName = dataReader.DatabaseName;
		TableName = dataReader.TableName;
		TableSchema = dataReader.TableSchema;
		DataType = dataReader.Datatype;
		Position = dataReader.Position;
		Description = dataReader.Description;
		ConstraintType = dataReader.ConstraintType;
		Nullable = dataReader.Nullable;
		DefaultValue = PrepareValue.ToStringWithoutBrackets(dataReader.DefaultValue);
		IsIdentity = dataReader.IsIdentity;
		IsComputed = dataReader.IsComputed;
		ComputedFormula = dataReader.ComputedFormula;
		DataLength = dataReader.DataLength;
		if (string.IsNullOrEmpty(DataLength))
		{
			DataLength = null;
		}
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
			base.CustomFields.RetrieveCustomFields(dataReader);
		}
		base.ObjectType = SharedObjectTypeEnum.ObjectType.Column;
		Type = dataReader.Type;
		Path = dataReader.Path;
		Level = dataReader.Level;
	}

	public ColumnRow(ColumnSynchronizationForNeo4jObject dataReader, CustomFieldsSupport customFieldsSupport)
	{
		base.Name = dataReader.Name;
		DatabaseName = dataReader.DatabaseName;
		TableName = dataReader.TableName;
		TableSchema = dataReader.TableSchema;
		DataType = dataReader.Datatype;
		Position = dataReader.Position;
		Description = dataReader.Description;
		ConstraintType = dataReader.ConstraintType;
		Nullable = dataReader.Nullable;
		DefaultValue = PrepareValue.ToStringWithoutBrackets(dataReader.DefaultValue);
		IsIdentity = dataReader.IsIdentity;
		IsComputed = dataReader.IsComputed;
		ComputedFormula = dataReader.ComputedFormula;
		DataLength = dataReader.DataLength;
		if (string.IsNullOrEmpty(DataLength))
		{
			DataLength = null;
		}
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
			base.CustomFields.RetrieveCustomFields(dataReader);
		}
		base.ObjectType = SharedObjectTypeEnum.ObjectType.Column;
	}

	public ColumnRow(ColumnSynchronizationForSsasObject dataReader, CustomFieldsSupport customFieldsSupport)
		: this((ColumnSynchronizationObject)dataReader, customFieldsSupport)
	{
		Type = dataReader.Type;
	}

	public ColumnRow(ColumnSynchronizationForSalesforceObject dataReader, CustomFieldsSupport customFieldsSupport)
		: this((ColumnSynchronizationObject)dataReader, customFieldsSupport)
	{
		Type = dataReader.Type;
		base.Title = dataReader.Title;
	}

	public ColumnRow(ColumnSynchronizationForDataverseObject dataReader, CustomFieldsSupport customFieldsSupport)
		: this((ColumnSynchronizationObject)dataReader, customFieldsSupport)
	{
		base.Title = dataReader.Title;
	}

	public ColumnRow(ColumnWithTableTypeSynchronizationObject dataReader, CustomFieldsSupport customFieldsSupport)
		: this((ColumnSynchronizationObject)dataReader, customFieldsSupport)
	{
		TableType = dataReader.TableType;
	}

	public ColumnRow(int tableId)
	{
		base.Source = UserTypeEnum.UserType.USER;
		TableId = tableId;
		base.ObjectType = SharedObjectTypeEnum.ObjectType.Column;
	}

	public override string ToString()
	{
		return ReferencesDataContainer?.ReferencesStringWithColumnName;
	}

	public void GetPropertyError(string propertyName, ErrorInfo info)
	{
		if (!(propertyName != "Name"))
		{
			if (AlreadyExists)
			{
				info.ErrorText = "Columns names can not be duplicated";
				info.ErrorType = ErrorType.Critical;
			}
			else if (IsNameEmpty)
			{
				info.ErrorText = "Column name can not be empty";
				info.ErrorType = ErrorType.Critical;
			}
		}
	}

	public void GetError(ErrorInfo info)
	{
	}

	public ColumnRow GetRootObject(ColumnRow column)
	{
		if (!column.ParentId.HasValue)
		{
			return column;
		}
		return GetRootObject(column.ParentColumn);
	}
}
