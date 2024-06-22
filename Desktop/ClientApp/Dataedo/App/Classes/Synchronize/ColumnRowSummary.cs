using Dataedo.App.Enums;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools.CustomFields;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class ColumnRowSummary : ColumnRow
{
	public string ParentString
	{
		get
		{
			if (!string.IsNullOrEmpty(ParentTitle))
			{
				return ParentNameWithSchema + " (" + ParentTitle + ")";
			}
			return ParentNameWithSchema;
		}
	}

	public string ParentGoToString
	{
		get
		{
			if (!string.IsNullOrEmpty(base.Title))
			{
				return ParentNameWithSchema + " (" + ParentTitle + ")";
			}
			return ParentNameWithSchema;
		}
	}

	public string SingleParentType => SharedObjectSubtypeEnum.TypeToStringForSingle(ParentType, ParentSubtype);

	public string ParentNameWithSchema => DBTreeMenu.SetNameOnlySchema(base.TableSchema, base.TableName, DatabaseRow.GetShowSchema(DatabaseShowSchema, DatabaseShowSchemaOverride));

	public SharedObjectTypeEnum.ObjectType? ParentType { get; private set; }

	public string ParentTitle { get; private set; }

	public SharedObjectSubtypeEnum.ObjectSubtype ParentSubtype { get; private set; }

	public int? DatabaseId { get; private set; }

	public SharedDatabaseTypeEnum.DatabaseType? DatabaseType { get; internal set; }

	public string DatabaseTypeText => DatabaseTypeEnum.TypeToStringForDisplay(DatabaseType);

	public string DatabaseTitle { get; private set; }

	public bool? DatabaseShowSchema { get; private set; }

	public bool? DatabaseShowSchemaOverride { get; private set; }

	public ColumnRowSummary()
	{
	}

	public ColumnRowSummary(ColumnViewObject row, CustomFieldsSupport customFieldsSupport)
	{
		base.Id = row.ColumnId;
		base.Name = row.Name;
		base.Title = row.Title;
		base.Position = row.OrdinalPosition;
		base.Type = row.ItemType;
		base.Description = row.Description;
		base.IsPk = row.PrimaryKey;
		base.DataType = row.DatatypeLen;
		base.Description = row.Description;
		SetStatus(row);
		base.Nullable = row.Nullable;
		base.IsIdentity = row.IsIdentity;
		base.IsComputed = row.IsComputed;
		base.Source = UserTypeEnum.ObjectToType(row.Source) ?? UserTypeEnum.UserType.DBMS;
		base.Sort = row.Sort;
		base.DataLength = row.DataLength;
		base.CreationDateString = row.CreationDate.ToString();
		base.CreatedBy = row.CreatedBy;
		base.LastModificationDateString = row.LastModificationDate.ToString();
		base.ModifiedBy = row.ModifiedBy;
		DatabaseId = row.DatabaseId;
		DatabaseTitle = row.DatabaseTitle;
		DatabaseType = DatabaseTypeEnum.StringToType(row.DatabaseType);
		DatabaseShowSchema = row.DatabaseShowSchema;
		DatabaseShowSchemaOverride = row.DatabaseShowSchemaOverride;
		base.TableId = row.TableId;
		ParentType = SharedObjectTypeEnum.StringToType(row.ObjectType);
		base.TableName = row.TableName;
		base.TableSchema = row.Schema;
		ParentTitle = row.ParentTitle;
		ParentSubtype = SharedObjectSubtypeEnum.StringToType(base.ObjectType, row.Subtype);
		base.Path = row.Path;
		base.Level = row.Level;
		base.ParentId = row.ParentId;
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
			base.CustomFields.RetrieveCustomFields(row);
			base.CustomFields.SetObjectId(base.Id);
			base.CustomFields.SetObjectType(SharedObjectTypeEnum.ObjectType.Column);
		}
	}
}
