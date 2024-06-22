using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Enums;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Data.Base.Commands.Results;
using Dataedo.Model.Data.Erd;
using Dataedo.Model.Data.Tables.Relations;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;

namespace Dataedo.App.Data.MetadataServer;

internal class RelationsDB : CommonDBSupport
{
	public RelationsDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public List<RelationWithColumnAndUniqueConstraint> GetDataByTableWithColumnsAndUniqueConstraints(int tableId, bool notDeletedOnly = false)
	{
		return commands.Select.Relations.GetRelationsWithColumnsAndUniqueConstraints(tableId, GetNotStatusValue(notDeletedOnly));
	}

	public List<RelationLinkObject> GetDataBySavedNodesInModule(int moduleId)
	{
		return commands.Select.Erd.GetDataBySavedNodesInModule(moduleId);
	}

	public List<RelationLinkObject> GetDataByModule(int moduleId)
	{
		return commands.Select.Erd.GetDataByModule(moduleId);
	}

	public List<RelationLinkObject> GetDataByRelationsWithOtherModule(int moduleId)
	{
		return commands.Select.Erd.GetDataByRelationsWithOtherModule(moduleId);
	}

	public List<RelationKeyDataObject> GetKeyByTableDoc(int tableId, bool isPk, bool notDeletedOnly = false)
	{
		string tableIdAlias = (isPk ? "fk_table_id" : "pk_table_id");
		return commands.Select.Relations.GetRelationKeyData(tableId, tableIdAlias, GetNotStatusValue(notDeletedOnly));
	}

	public string GetJoinForPDF(bool contextShowSchema, int relationTable, RelationKeyDataObject row, bool getPkRelations)
	{
		return GetJoinForPDF(contextShowSchema, getPkRelations, relationTable, row.FkTableObject.DatabaseId, DatabaseRow.GetShowSchema(row.FkTableObject.DatabaseShowSchema, row.FkTableObject.DatabaseShowSchemaOverride), row.FkTableObject.DatabaseTitle, row.FkTableObject.Id, row.FkTableObject.Schema, row.FkTableObject.Name, row.ColumnFkPath, row.ColumnFkName, row.PkTableObject.DatabaseId, DatabaseRow.GetShowSchema(row.PkTableObject.DatabaseShowSchema, row.PkTableObject.DatabaseShowSchemaOverride), row.PkTableObject.DatabaseTitle, row.PkTableObject.Id, row.PkTableObject.Schema, row.PkTableObject.Name, row.ColumnPkPath, row.ColumnPkName, boldedSchemaAndTable: true);
	}

	public string GetJoinForPDF(bool contextShowSchema, bool getPkRelations, int? relationTable, int? fkDatabaseId, bool? useFkSchema, string fkDatabase, int? fkTableId, string fkSchema, string fkTableName, string fkColumnPath, string fkColumnName, int? pkDatabaseId, bool? usePkSchema, string pkDatabase, int? pkTableId, string pkSchema, string pkTableName, string pkColumnPath, string pkColumnName, bool boldedSchemaAndTable = false)
	{
		string tableObjectName = ObjectNames.GetTableObjectName(contextShowSchema, relationTable, pkDatabaseId, usePkSchema.GetValueOrDefault(), pkDatabase, pkTableId.GetValueOrDefault(), pkSchema, pkTableName, fkDatabaseId, useDatabaseName: true);
		string tableObjectName2 = ObjectNames.GetTableObjectName(contextShowSchema, relationTable, fkDatabaseId, useFkSchema.GetValueOrDefault(), fkDatabase, fkTableId.GetValueOrDefault(), fkSchema, fkTableName, pkDatabaseId, useDatabaseName: true);
		if (getPkRelations)
		{
			return (boldedSchemaAndTable ? "<b>" : string.Empty) + tableObjectName2 + (boldedSchemaAndTable ? "</b>" : string.Empty) + ColumnNames.GetFullNameFormattedForHtml(fkColumnPath, fkColumnName, addDotBeforePath: true) + " = " + tableObjectName + ColumnNames.GetFullNameFormattedForHtml(pkColumnPath, pkColumnName, addDotBeforePath: true);
		}
		return (boldedSchemaAndTable ? "<b>" : string.Empty) + tableObjectName + (boldedSchemaAndTable ? "</b>" : string.Empty) + ColumnNames.GetFullNameFormattedForHtml(pkColumnPath, pkColumnName, addDotBeforePath: true) + " = " + tableObjectName2 + ColumnNames.GetFullNameFormattedForHtml(fkColumnPath, fkColumnName, addDotBeforePath: true);
	}

	public string GetJoin(bool contextDatabaseShowSchema, bool getPkRelations, int? relationTable, int? fkDatabaseId, SharedDatabaseTypeEnum.DatabaseType? fkDatabaseType, bool fkShowSchema, string fkDatabase, int? fkTableId, string fkSchema, string fkTableName, string fkColumnName, int? pkDatabaseId, SharedDatabaseTypeEnum.DatabaseType? pkDatabaseType, bool pkShowSchema, string pkDatabase, int? pkTableId, string pkSchema, string pkTableName, string pkColumnName, bool boldedSchemaAndTable = false)
	{
		string tableObjectName = ObjectNames.GetTableObjectName(relationTable, pkDatabaseId, pkDatabase, pkShowSchema, pkTableId.GetValueOrDefault(), pkSchema, pkTableName, fkDatabaseId, contextDatabaseShowSchema, useDatabaseName: true);
		string tableObjectName2 = ObjectNames.GetTableObjectName(relationTable, fkDatabaseId, fkDatabase, fkShowSchema, fkTableId.GetValueOrDefault(), fkSchema, fkTableName, pkDatabaseId, contextDatabaseShowSchema, useDatabaseName: true);
		if (getPkRelations)
		{
			return (boldedSchemaAndTable ? "<b>" : string.Empty) + tableObjectName2 + "." + (boldedSchemaAndTable ? "</b>" : string.Empty) + fkColumnName + " = " + tableObjectName + "." + pkColumnName;
		}
		return (boldedSchemaAndTable ? "<b>" : string.Empty) + tableObjectName + "." + (boldedSchemaAndTable ? "</b>" : string.Empty) + pkColumnName + " = " + tableObjectName2 + "." + fkColumnName;
	}

	public BindingList<RelationRow> GetDataObjectByTableId(int tableId, bool contextShowSchema, CustomFieldsSupport customFieldsSupport = null)
	{
		return new BindingList<RelationRow>(GroupRelations(from RelationWithColumnAndUniqueConstraint relation in commands.Select.Relations.GetRelationsWithColumnsAndUniqueConstraints(tableId)
			select new RelationRow(relation, customFieldsSupport, contextShowSchema)
			{
				CurrentTableId = tableId
			}).ToList());
	}

	public static IEnumerable<RelationRow> GroupRelations(IEnumerable<RelationRow> relations)
	{
		return (from x in relations
			group x by x.Id).Select(delegate(IGrouping<int, RelationRow> x)
		{
			RelationRow relationRow = x.First();
			if (x.Count() == 1)
			{
				return relationRow;
			}
			relationRow.Columns = new BindingList<RelationColumnRow>(RelationColumnDB.GroupRelationColumns(x.SelectMany((RelationRow y) => y.Columns)).ToList());
			return relationRow;
		}).ToList();
	}

	public bool Update(RelationRow relationRow, Form owner = null)
	{
		try
		{
			List<ObjectIdColumnsIds> list = new List<ObjectIdColumnsIds>();
			foreach (RelationColumnRow column in relationRow.Columns)
			{
				if (column.ColumnPkId != -1 && column.ColumnFkId != -1)
				{
					list.Add(new ObjectIdColumnsIds
					{
						BaseId = relationRow.Id,
						ColumnId = column.ColumnPkId,
						ColumnFkId = column.ColumnFkId,
						OrdinalPosition = column.OrdinalPosition
					});
				}
			}
			ObjectRelation objectRelation = ConvertRowToItem(relationRow, list);
			if (objectRelation.IsUserDefined)
			{
				commands.Manipulation.Relations.UpdateUserRelationAndColumns(objectRelation);
			}
			else
			{
				commands.Manipulation.Relations.UpdateRelationAndColumns(objectRelation);
			}
			relationRow.Id = objectRelation.RelationId;
			if (relationRow?.CustomFields != null)
			{
				relationRow.CustomFields.SetObjectId(relationRow.Id);
				relationRow.CustomFields.SetObjectType(SharedObjectTypeEnum.ObjectType.Relation);
				relationRow.CustomFields.UpdateCustomFieldDefinitionValues(DB.CustomField.UpdateCustomFieldValues);
			}
			relationRow.SetUnchanged();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the relationship:", owner);
			return false;
		}
		return true;
	}

	public bool Insert(RelationRow relationRow, Form owner = null)
	{
		try
		{
			List<ObjectIdColumnsIds> list = new List<ObjectIdColumnsIds>();
			foreach (RelationColumnRow column in relationRow.Columns)
			{
				if (column.ColumnPkId != -1 && column.ColumnFkId != -1)
				{
					list.Add(new ObjectIdColumnsIds
					{
						BaseId = relationRow.Id,
						ColumnId = column.ColumnPkId,
						ColumnFkId = column.ColumnFkId,
						OrdinalPosition = column.OrdinalPosition
					});
				}
			}
			ObjectRelation objectRelation = ConvertRowToItem(relationRow, list);
			commands.Manipulation.Relations.InsertRelationAndColumns(objectRelation);
			relationRow.Id = objectRelation.RelationId;
			relationRow.SetUnchanged();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while inserting the relationship:", owner);
			return false;
		}
		return true;
	}

	private ObjectRelation ConvertRowToItem(RelationRow row, List<ObjectIdColumnsIds> objectsToUse)
	{
		ObjectRelation objectRelation = new ObjectRelation
		{
			RelationId = row.Id,
			TablePkId = row.PKTableId,
			TableFkId = row.FKTableId,
			Name = row.NotEmptyName,
			Title = row.Title,
			Description = row.Description,
			Columns = objectsToUse.ToArray(),
			PKCardinality = CardinalityTypeEnum.TypeToString(row.PKCardinality.Type),
			FKCardinality = CardinalityTypeEnum.TypeToString(row.FKCardinality.Type),
			IsUserDefined = (row.Source == UserTypeEnum.UserType.USER)
		};
		SetCustomFields(objectRelation, row);
		return objectRelation;
	}

	public bool Delete(IEnumerable<int> relations, Form owner = null)
	{
		try
		{
			commands.Manipulation.Relations.DeleteRelations(relations.ToArray());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting the relationships:", owner);
			return false;
		}
		return true;
	}

	public bool Synchronize(IEnumerable<RelationRow> relations, IEnumerable<ObjectRow> updatedTables, DatabaseRow database, bool isDbAdded, int updateId, out int[] succeedTablesIds, Form owner = null)
	{
		try
		{
			ObjectNameSchemaId[] array = updatedTables.Select((ObjectRow r) => new ObjectNameSchemaId
			{
				BaseId = database.IdValue,
				BaseName = r.Name,
				BaseSchema = r.Schema
			}).ToArray();
			RelationForSynchronization[] array2 = relations.Select((RelationRow r) => ConvertRowToSynchronizationItem(r, database, updateId)).ToArray();
			if (array2.Length != 0)
			{
				IEnumerable<IGrouping<int?, IdSource>> source = from x in (from r in commands.Synchronization.Relations.SynchronizeRelationsByTablesAndByRelations(array, database.Host, array2, isDbAdded, updateId)
						where r.Result == 0
						select r).SelectMany((ValueWithDataArrayResult<int, IdSource> r) => r.Data.Where((IdSource x) => x.IsFromCurrentDatabase))
					group x by x.RelationGroupId;
				List<IdSource> list = source.Where((IGrouping<int?, IdSource> x) => !x.Key.HasValue).SelectMany((IGrouping<int?, IdSource> x) => x).ToList();
				list.AddRange((from x in source
					where x.Key.HasValue
					select x.First()).ToArray());
				succeedTablesIds = list.Select((IdSource x) => x.Id.Value).ToArray();
			}
			else if (isDbAdded)
			{
				succeedTablesIds = new int[0];
			}
			else
			{
				List<ValueWithDataArrayResult<int, int>> source2 = commands.Synchronization.Relations.SynchronizeRelationsByTables(array, isDbAdded);
				succeedTablesIds = source2.Where((ValueWithDataArrayResult<int, int> r) => r.Result == 0).SelectMany((ValueWithDataArrayResult<int, int> r) => r.Data).ToArray();
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while synchronizing the relationships:", owner);
			succeedTablesIds = new int[0];
			return false;
		}
		return true;
	}

	private RelationForSynchronization ConvertRowToSynchronizationItem(RelationRow row, DatabaseRow database, int updateId)
	{
		RelationForSynchronization relationForSynchronization = new RelationForSynchronization
		{
			DatabaseId = database.IdValue,
			SucceedTablesIds = string.Empty,
			Name = row.Name,
			IsPkFromCurrentDatabase = database.IsCurrentDocumentation(row.PKTableDatabaseName),
			PkTableDatabaseName = row.PKTableDatabaseName,
			PkTableSchema = row.PKTableSchema,
			PkTableName = row.PKTableName,
			IsFkFromCurrentDatabase = database.IsCurrentDocumentation(row.FKTableDatabaseName),
			FkTableDatabaseName = row.FKTableDatabaseName,
			FkTableSchema = row.FKTableSchema,
			FkTableName = row.FKTableName,
			Description = row.Description,
			UpdateRule = row.UpdateRule,
			DeleteRule = row.DeleteRule,
			UpdateId = updateId
		};
		SetCustomFields(relationForSynchronization, row);
		return relationForSynchronization;
	}

	public List<RelationTablesIdPair> GetRelationTablesIDsByObject(int tableId, Form owner = null)
	{
		try
		{
			return commands.Select.Relations.GetRelationTablesIDsByObject(tableId);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting the relations IDs:", owner);
			return null;
		}
	}
}
