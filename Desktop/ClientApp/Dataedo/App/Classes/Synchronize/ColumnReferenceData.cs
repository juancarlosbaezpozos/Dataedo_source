using System.Drawing;
using Dataedo.App.Classes.Synchronize.Tools;
using Dataedo.App.Enums;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;

namespace Dataedo.App.Classes.Synchronize;

public class ColumnReferenceData
{
	public bool ContextShowSchema { get; private set; }

	public ObjectRow ObjectRow { get; set; }

	public int? RelationId { get; set; }

	public UserTypeEnum.UserType? RelationSource { get; set; }

	public int? PkDatabaseId { get; set; }

	public int? FkDatabaseId { get; set; }

	public SharedDatabaseTypeEnum.DatabaseType FkDatabaseType { get; set; }

	public string FkDatabaseName { get; set; }

	public int? ObjectId { get; set; }

	public SharedDatabaseTypeEnum.DatabaseType? PkDatabaseType { get; set; }

	public string PkDatabaseName { get; set; }

	public bool? PkDatabaseMultipleSchemas { get; set; }

	public bool PKDatabaseShowSchema { get; set; }

	public string PkDatabaseIdString => IdStrings.GetDocumentationIdString(PkDatabaseName, PkDatabaseId);

	public int? PkObjectId { get; set; }

	public SharedObjectTypeEnum.ObjectType? PkObjectType { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype? PkObjectSubtype { get; set; }

	public UserTypeEnum.UserType PkObjectSource { get; set; }

	public string PkObjectSchema { get; set; }

	public string PkObjectName { get; set; }

	public string PkObjectTitle { get; set; }

	public int? PkColumnId { get; set; }

	public string PkColumnName { get; set; }

	public string ReferenceString => ObjectNames.GetTableObjectName(ContextShowSchema, ObjectRow?.ObjectId, PkDatabaseId, PKDatabaseShowSchema, PkDatabaseName, PkObjectId.GetValueOrDefault(), PkObjectSchema, PkObjectName, PkObjectTitle, ObjectRow?.DatabaseId, useDatabaseName: true);

	public string ReferenceStringWithColumnName => ReferenceString + "." + PkColumnName;

	public bool IsSameDatabase
	{
		get
		{
			ObjectRow objectRow = ObjectRow;
			if (objectRow != null && objectRow.DatabaseId.HasValue && PkDatabaseId.HasValue)
			{
				return ObjectRow.DatabaseId == PkDatabaseId;
			}
			return false;
		}
	}

	public Image ReferenceImage
	{
		get
		{
			if (RelationSource != UserTypeEnum.UserType.DBMS)
			{
				return Resources.relation_mx_1x_user_16;
			}
			return Resources.relation_mx_1x_16;
		}
	}

	public Image ObjectImage => IconsSupport.GetObjectIcon(PkObjectType, PkObjectSubtype, PkObjectSource);

	public string ObjectIdString => IdStrings.GetObjectIdString(PkDatabaseType, PkDatabaseMultipleSchemas, PkObjectSchema, PkObjectName, PkObjectId);

	public ColumnReferenceData(RelationRow relation, string pkColumnName, int pkColumnId)
	{
		ObjectRow = new ObjectRow
		{
			ObjectId = relation.FKTableId,
			DatabaseId = relation.FKTableDatabaseId,
			DocumentationType = relation.FKTableDatabaseType,
			DocumentationTitle = relation.FKTableDatabaseName
		};
		RelationId = relation.Id;
		RelationSource = relation.Source;
		PkDatabaseId = relation.PKTableDatabaseId;
		PkDatabaseType = relation.PKTableDatabaseType;
		PkDatabaseName = relation.PKTableDatabaseName;
		PkDatabaseMultipleSchemas = relation.PKTableDatabaseMultipleSchemas;
		PKDatabaseShowSchema = relation.PKTableDatabaseShowSchema;
		PkObjectId = relation.PKTableId;
		PkObjectType = relation.PKObjectType;
		PkObjectSubtype = relation.PKObjectSubtype;
		PkObjectSource = relation.PKObjectSource;
		PkObjectSchema = relation.PKTableSchema;
		PkObjectName = relation.PKTableName;
		PkObjectTitle = relation.PKTableTitle;
		PkColumnId = pkColumnId;
		PkColumnName = pkColumnName;
	}

	public ColumnReferenceData(bool contextShowSchema, ObjectRow objectRow, ColumnDocObject row)
	{
		ContextShowSchema = contextShowSchema;
		ObjectRow = objectRow;
		RelationId = row.RelationId;
		RelationSource = UserTypeEnum.ObjectToType(row.RelationSource);
		PkDatabaseId = row.PkDatabaseId;
		PkDatabaseType = DatabaseTypeEnum.StringToType(row.PkDatabaseType);
		PkDatabaseName = row.PkDatabaseName;
		PkDatabaseMultipleSchemas = row.PkDatabaseMultipleSchemas;
		PKDatabaseShowSchema = DatabaseRow.GetShowSchema(row.PkDatabaseDatabaseShowSchema, row.PkDatabaseShowSchemaOverride);
		PkObjectId = row.PkObjectId;
		PkObjectType = SharedObjectTypeEnum.StringToType(row.PkObjectType);
		PkObjectSubtype = SharedObjectSubtypeEnum.StringToType(PkObjectType, row.PkObjectSubtype);
		PkObjectSource = UserTypeEnum.ObjectToTypeOrDbms(row.PkObjectSource);
		PkObjectSchema = row.PkObjectSchema;
		PkObjectName = row.PkObjectName;
		PkObjectTitle = row.PkObjectTitle;
		PkColumnId = row.PkColumnId;
		PkColumnName = row.PkColumnName;
	}

	public ColumnReferenceData(ObjectRow objectRow, ColumnDocObject row)
	{
		ObjectRow = objectRow;
		RelationId = row.RelationId;
		RelationSource = UserTypeEnum.ObjectToType(row.RelationSource);
		PkDatabaseId = row.PkDatabaseId;
		PkDatabaseType = DatabaseTypeEnum.StringToType(row.PkDatabaseType);
		PkDatabaseName = row.PkDatabaseName;
		PkDatabaseMultipleSchemas = row.PkDatabaseMultipleSchemas;
		PKDatabaseShowSchema = DatabaseRow.GetShowSchema(row.PkDatabaseDatabaseShowSchema, row.PkDatabaseShowSchemaOverride);
		PkObjectId = row.PkObjectId;
		PkObjectType = SharedObjectTypeEnum.StringToType(row.PkObjectType);
		PkObjectSubtype = SharedObjectSubtypeEnum.StringToType(PkObjectType, row.PkObjectSubtype);
		PkObjectSource = UserTypeEnum.ObjectToTypeOrDbms(row.PkObjectSource);
		PkObjectSchema = row.PkObjectSchema;
		PkObjectName = row.PkObjectName;
		PkObjectTitle = row.PkObjectTitle;
		PkColumnId = row.PkColumnId;
		PkColumnName = row.PkColumnName;
	}
}
