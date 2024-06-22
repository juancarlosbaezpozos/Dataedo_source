using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.Erd;
using Dataedo.Model.Data.FeedbackWidgetData;
using Dataedo.Model.Data.Modules;
using Dataedo.Model.Data.SuggestedDescription;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer;

internal class TableDB : CommonDBSupport
{
	public TableDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public BaseDataObject GetBaseDataByName(int databaseId, string name, string schema, string subtype)
	{
		return commands.Select.Tables.GetObjectsByName(databaseId, name, schema, subtype);
	}

	public int? GetObjectIdByName(int databaseId, string schema, string name, string objectType)
	{
		return commands.Select.Tables.GetObjectIdByName(databaseId, schema, name, objectType);
	}

	public bool HasMultipleLevelColumns(int tableId)
	{
		return commands.Select.Tables.HasMultipleLevelColumns(tableId);
	}

	public IdAndColumnsCountByByName GetIdAndColumnsCountByName(int databaseId, string name, string schema, SharedObjectTypeEnum.ObjectType objectType)
	{
		return commands.Select.Tables.GetIdAndColumnsCountByName(databaseId, name, schema, SharedObjectTypeEnum.TypeToString(objectType)).FirstOrDefault();
	}

	public int GetIdByName(int databaseId, string name, string schema, string subtype)
	{
		return GetBaseDataByName(databaseId, name, schema, subtype).Id;
	}

	public TableIdAndColumnsCountModel GetTableIdAndColumnsCountModel(int databaseId, string name, string schema, SharedObjectTypeEnum.ObjectType objectType)
	{
		IdAndColumnsCountByByName idAndColumnsCountByName = GetIdAndColumnsCountByName(databaseId, name, schema, objectType);
		return new TableIdAndColumnsCountModel
		{
			TableId = idAndColumnsCountByName.TableId,
			ColumnsCount = idAndColumnsCountByName.Count.GetValueOrDefault()
		};
	}

	public List<ErdTableObject> GetAllDocsTablesAndViewsOutsideModuleOnErd(List<int> tableIds)
	{
		return commands.Select.Erd.GetAllDocsTablesAndViewsOutsideModuleOnErd((tableIds != null && tableIds.Count == 0) ? null : tableIds);
	}

	public List<ErdTableObject> GetTablesAndViewsOutsideModuleOnErd(int databaseId, List<int> tableIds)
	{
		return commands.Select.Erd.GetTablesAndViewsOutsideModuleOnErd(databaseId, null, (tableIds != null && tableIds.Count == 0) ? null : tableIds);
	}

	public ErdTableObject GetTableAsErdTableObject(int databaseId, int tableId)
	{
		return commands.Select.Erd.GetTablesAndViewsOutsideModuleOnErd(databaseId, tableId, null)?[0];
	}

	public List<ErdTableObject> GetTablesWithIntermoduleRelation(int moduleId, List<int> tableIds)
	{
		if (tableIds.Count == 0)
		{
			return new List<ErdTableObject>();
		}
		return commands.Select.Erd.GetTablesWithIntermoduleRelation(moduleId, tableIds);
	}

	public List<ErdTableWithModuleObject> GetTablesAndViewsIgnoredOnERD(int moduleId, List<int> tableIds)
	{
		return commands.Select.Erd.GetTablesAndViewsIgroredOnErd(moduleId, (tableIds != null && tableIds.Count == 0) ? null : tableIds);
	}

	public List<ObjectDeletedFromDatabaseObject> GetDataDeletedFromDBMS(int databaseId, SharedObjectTypeEnum.ObjectType objectType, string filter, bool updateEntireDocumentation)
	{
		if (updateEntireDocumentation)
		{
			return commands.Select.Tables.GetObjectsDeletedFromDatabase(databaseId, SharedObjectTypeEnum.TypeToString(objectType), filter);
		}
		return commands.Select.Tables.GetObjectsDeletedFromDatabaseWithoutFilter(databaseId, SharedObjectTypeEnum.TypeToString(objectType), filter);
	}

	public List<ObjectForMenuObject> GetByModuleForMenu(int moduleId, SharedObjectTypeEnum.ObjectType objectType)
	{
		return commands.Select.Tables.GetObjectsByModuleForMenu(moduleId, objectType);
	}

	public List<ObjectForMenuObjectWithModuleId> GetObjectsForMenuByDatabaseForModules(int databaseId, SharedObjectTypeEnum.ObjectType objectType)
	{
		return commands.Select.Tables.GetObjectsForMenuByDatabaseForModules(databaseId, objectType);
	}

	public List<ObjectForMenuObject> GetByDatabaseForMenu(int databaseId, SharedObjectTypeEnum.ObjectType objectType)
	{
		return commands.Select.Tables.GetObjectsByDatabaseForMenu(databaseId, objectType);
	}

	public List<TableByDatabaseIdObject> GetTablesByDatabase(int databaseId, SharedObjectTypeEnum.ObjectType? objectType, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsByDatabase(databaseId, objectType, GetNotStatusValue(notDeletedOnly));
	}

	public List<int> GetTablesIdsByDatabase(int databaseId, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsIdsByDatabase(databaseId, SharedObjectTypeEnum.ObjectType.Table, GetNotStatusValue(notDeletedOnly));
	}

	public List<TableByDatabaseIdObject> GetViewsByDatabase(int databaseId, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsByDatabase(databaseId, SharedObjectTypeEnum.ObjectType.View, GetNotStatusValue(notDeletedOnly));
	}

	public List<int> GetViewsIdsByDatabase(int databaseId, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsIdsByDatabase(databaseId, SharedObjectTypeEnum.ObjectType.View, GetNotStatusValue(notDeletedOnly));
	}

	public List<TableByDatabaseIdObject> GetStructuresByDatabase(int databaseId, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsByDatabase(databaseId, SharedObjectTypeEnum.ObjectType.Structure, GetNotStatusValue(notDeletedOnly));
	}

	public List<int> GetStructuresIdsByDatabase(int databaseId, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsIdsByDatabase(databaseId, SharedObjectTypeEnum.ObjectType.Structure, GetNotStatusValue(notDeletedOnly));
	}

	public List<ObjectByModuleObject> GetTablesByModule(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsByModule(moduleId, SharedObjectTypeEnum.ObjectType.Table, GetNotStatusValue(notDeletedOnly));
	}

	public List<int> GetTablesIdsByModule(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsIdsByModule(moduleId, SharedObjectTypeEnum.ObjectType.Table, GetNotStatusValue(notDeletedOnly));
	}

	public List<ObjectByModuleObject> GetTablesWithoutModule(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsWithoutModule(moduleId, SharedObjectTypeEnum.ObjectType.Table);
	}

	public List<ObjectByModuleObject> GetViewsByModule(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsByModule(moduleId, SharedObjectTypeEnum.ObjectType.View, GetNotStatusValue(notDeletedOnly));
	}

	public List<int> GetViewsIdsByModule(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsIdsByModule(moduleId, SharedObjectTypeEnum.ObjectType.View, GetNotStatusValue(notDeletedOnly));
	}

	public List<ObjectByModuleObject> GetViewsWithoutModule(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsWithoutModule(moduleId, SharedObjectTypeEnum.ObjectType.View);
	}

	public List<ObjectByModuleObject> GetStructuresByModule(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsByModule(moduleId, SharedObjectTypeEnum.ObjectType.Structure, GetNotStatusValue(notDeletedOnly));
	}

	public List<int> GetStructuresIdsByModule(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsIdsByModule(moduleId, SharedObjectTypeEnum.ObjectType.Structure, GetNotStatusValue(notDeletedOnly));
	}

	public List<ObjectByModuleObject> GetStructuresWithoutModule(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsWithoutModule(moduleId, SharedObjectTypeEnum.ObjectType.Structure);
	}

	public List<ObjectWithModulesObject> GetObjectsByDatabaseWithoutDescription(int databaseId, SharedObjectTypeEnum.ObjectType objectType, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsByDatabaseWithModulesWithoutDescription(databaseId, objectType, GetNotStatusValue(notDeletedOnly));
	}

	public List<TableWithSchemaByDatabaseObject> GetTablesAndViewsWithSchemaByDatabase(int databaseId, bool? contextShowSchema)
	{
		return commands.Select.Tables.GetObjectsWithSchemaByDatabase(databaseId, null, contextShowSchema);
	}

	public FeedbackWidgetDataObject GetTableFeedbackWidgetData(int tableId)
	{
		return commands.Select.Tables.GetObjectsFeedbackWidgetData(tableId);
	}

	public List<ObjectWithModulesObject> GetObjectsByModuleWithoutDescription(int moduleId, SharedObjectTypeEnum.ObjectType objectType, bool notDeletedOnly = false)
	{
		return commands.Select.Tables.GetObjectsByModuleWithModulesWithoutDescription(moduleId, objectType, GetNotStatusValue(notDeletedOnly));
	}

	public List<SuggestedDescriptionContainer> GetSuggestedDescriptionsByColumnName(int objecId, string fieldName, string tableName, string objectIdName, string joinTableName, string joinColumnName)
	{
		List<SuggestedDescriptionObject> suggestedDescriptionByColumnName = commands.Select.Tables.GetSuggestedDescriptionByColumnName(objecId, fieldName, tableName, objectIdName, joinTableName, joinColumnName);
		List<SuggestedDescriptionContainer> list = ConvertDataTableToSuggestedDescriptionContainer(suggestedDescriptionByColumnName, fieldName);
		if (Dataedo.App.StaticData.IsProjectFile)
		{
			List<SuggestedDescriptionContainer> list2 = new List<SuggestedDescriptionContainer>();
			{
				foreach (IGrouping<string, SuggestedDescriptionContainer> item in list.ToLookup((SuggestedDescriptionContainer x) => x.ColumnName))
				{
					IEnumerable<SuggestedDescriptionContainer> enumerable2;
					if (item.Count() <= 10)
					{
						IEnumerable<SuggestedDescriptionContainer> enumerable = item;
						enumerable2 = enumerable;
					}
					else
					{
						enumerable2 = item.Take(10);
					}
					IEnumerable<SuggestedDescriptionContainer> collection = enumerable2;
					list2.AddRange(collection);
				}
				return list2;
			}
		}
		return list;
	}

	public TableObject GetDataById(int tableId)
	{
		return commands.Select.Tables.GetObject(tableId);
	}

	public TableObject GetDataByName(int databaseId, string schema, string name, string objectType)
	{
		return commands.Select.Tables.GetObjectByName(databaseId, schema, name, objectType);
	}

	public List<int> GetTableModules(int tableId)
	{
		return commands.Select.Tables.GetObjectModules(tableId);
	}

	public List<ObjectDocObject> GetTablesByModuleDoc(int moduleId)
	{
		return commands.Select.Tables.GetObjectsByModuleDoc(moduleId, SharedObjectTypeEnum.ObjectType.Table);
	}

	public List<ObjectDocObject> GetTablesWithoutModuleDoc(int databaseId)
	{
		return commands.Select.Tables.GetObjectsWithoutModuleDoc(databaseId, SharedObjectTypeEnum.ObjectType.Table);
	}

	public List<ObjectDocObject> GetViewsByModuleDoc(int moduleId)
	{
		return commands.Select.Tables.GetObjectsByModuleDoc(moduleId, SharedObjectTypeEnum.ObjectType.View);
	}

	public List<ObjectDocObject> GetViewsWithoutModuleDoc(int databaseId)
	{
		return commands.Select.Tables.GetObjectsWithoutModuleDoc(databaseId, SharedObjectTypeEnum.ObjectType.View);
	}

	public List<ObjectDocObject> GetStructuresByModuleDoc(int moduleId)
	{
		return commands.Select.Tables.GetObjectsByModuleDoc(moduleId, SharedObjectTypeEnum.ObjectType.Structure);
	}

	public List<ObjectDocObject> GetStructuresWithoutModuleDoc(int databaseId)
	{
		return commands.Select.Tables.GetObjectsWithoutModuleDoc(databaseId, SharedObjectTypeEnum.ObjectType.Structure);
	}

	public bool AreObjectsWithoutModule(int databaseId)
	{
		return commands.Select.Tables.GetCountOfObjectsWithoutModule(databaseId) > 0;
	}

	public List<TableAndColumnForTree> GetTablesAndColumnsForTree(int databaseId)
	{
		return commands.Select.Tables.GetTablesAndColumnsForTree(databaseId);
	}

	public bool Update(int databaseId, int id, SharedObjectTypeEnum.ObjectType objectType, string title, string location, List<int> intModulesIds, Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> customFields, string description = null, string descriptionSearch = null, Form owner = null)
	{
		try
		{
			if (CommonFunctionsPanels.customFieldsForHistory.TryGetValue(id, out var value))
			{
				HistoryCustomFieldsHelper.InsertHistoryCustomFieldsOnTableSummary(id, databaseId, customFields, value, objectType);
				CommonFunctionsPanels.customFieldsForHistory[id] = customFields;
			}
			if (Dataedo.App.StaticData.IsProjectFile)
			{
				commands.Manipulation.Tables.UpdateTableCE(id, SharedObjectTypeEnum.TypeToString(objectType), title, location, description, descriptionSearch, intModulesIds.Select((int x) => new ObjectIdName
				{
					BaseId = x
				}).ToArray(), customFields);
			}
			else
			{
				FeedbackWidgetDataObject objectsFeedbackWidgetData = commands.Select.Tables.GetObjectsFeedbackWidgetData(id);
				commands.Manipulation.Tables.UpdateTableServer(id, SharedObjectTypeEnum.TypeToString(objectType), title, location, description, descriptionSearch, intModulesIds.Select((int x) => new ObjectIdName
				{
					BaseId = x
				}).ToArray(), customFields, objectsFeedbackWidgetData);
			}
			if (!CommonFunctionsPanels.summaryObjectTitleHistory.ContainsKey(id))
			{
				CommonFunctionsPanels.summaryObjectTitleHistory.Add(id, title);
			}
			DB.History.InsertHistoryRow(databaseId, id, title, null, null, "tables", HistoryGeneralHelper.CheckAreValuesDiffrent(title, CommonFunctionsPanels.summaryObjectTitleHistory[id]), saveDescription: false, objectType);
			CommonFunctionsPanels.summaryObjectTitleHistory[id] = title;
			DB.Community.InsertFollowingToRepository(objectType, id);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the table:", owner);
			return false;
		}
		return true;
	}

	public void BulkCopyTableUpdate(List<int> ids, string value, string fieldName, Form owner = null)
	{
		try
		{
			commands.Manipulation.Tables.BulkCopyTableUpdate(ids, value, fieldName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the table", owner);
		}
	}

	public void BulkCopyTableUpdate(int id, Dictionary<string, object> keyValuePairs, Form owner = null)
	{
		try
		{
			commands.Manipulation.Tables.BulkCopyTableUpdateSingleTableUpdate(id, keyValuePairs);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the table", owner);
		}
	}

	public bool Update(TableRow row, Form owner = null)
	{
		try
		{
			Table table = ConvertRowToItem(row);
			if (Dataedo.App.StaticData.IsProjectFile)
			{
				commands.Manipulation.Tables.UpdateTableWithCustomFieldsCE(table);
			}
			else
			{
				FeedbackWidgetDataObject objectsFeedbackWidgetData = commands.Select.Tables.GetObjectsFeedbackWidgetData(table.Id);
				commands.Manipulation.Tables.UpdateTableWithCustomFieldsServer(table, objectsFeedbackWidgetData);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the table:", owner);
			return false;
		}
		return true;
	}

	public int? InsertManualTable(TableModel table, Form owner = null)
	{
		try
		{
			return commands.Manipulation.Tables.InsertManualTable(ConvertTableToManualTable(table), Dataedo.App.StaticData.IsProjectFile);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while inserting manual table", owner);
			return null;
		}
	}

	public void UpdateManualTable(int tableId, string schema, string name, string title, string location, string type, string definition, bool updateDefinition, bool updateLocation, string source, Form owner = null)
	{
		try
		{
			commands.Manipulation.Tables.UpdateManualTable(tableId, schema, name, title, location, type, definition, updateDefinition, updateLocation, source);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating manual table", owner);
		}
	}

	private List<SuggestedDescriptionContainer> ConvertDataTableToSuggestedDescriptionContainer(IEnumerable<SuggestedDescriptionObject> data, string fieldName)
	{
		List<SuggestedDescriptionContainer> list = new List<SuggestedDescriptionContainer>();
		foreach (SuggestedDescriptionObject datum in data)
		{
			SuggestedDescriptionContainer item = new SuggestedDescriptionContainer
			{
				Description = datum.Dscr,
				Count = datum.Cnt.GetValueOrDefault(),
				ColumnName = datum.Name,
				CommonTableName = datum.ObjName,
				ObjectName = datum.ObjectName,
				ObjectSchema = datum.ObjectSchema,
				ShowSchema = datum.ShowSchema,
				ShowSchemaOverride = datum.ShowSchemaOverride
			};
			list.Add(item);
		}
		return list;
	}

	private ManualTable ConvertTableToManualTable(TableModel table)
	{
		return new ManualTable
		{
			TableId = table.TableId,
			Title = table.Title,
			Location = table.Location,
			Name = table.Name,
			Schema = table.Schema,
			ObjectType = SharedObjectTypeEnum.TypeToString(table.ObjectType),
			DatabaseId = table.DatabaseId,
			Subtype = table.SubtypeString,
			Definition = table.Definition,
			Source = (table.Source ?? UserTypeEnum.UserType.USER)
		};
	}

	private Table ConvertRowToItem(TableRow row)
	{
		Table table = new Table
		{
			Id = row.Id,
			ObjectType = SharedObjectTypeEnum.TypeToString(row.ObjectType),
			Title = row.Title,
			Location = row.Location,
			Description = row.Description,
			DescriptionPlain = row.DescriptionPlain,
			Modules = row.Modules
		};
		SetCustomFields(table, row);
		return table;
	}

	private TableForSynchronization ConvertObjectRowToItem(ObjectRow row, int databaseId, int updateId)
	{
		TableForSynchronization tableForSynchronization = new TableForSynchronization
		{
			Name = row.Name,
			Schema = row.Schema,
			DatabaseId = databaseId,
			ObjectType = row.TypeAsString,
			Subtype = row.SubtypeAsString,
			Description = row.Description,
			DescriptionPlain = row.DescriptionSearch,
			Definition = row.Definition,
			Language = row.Language,
			DbmsCreationDate = row.DbmsCreationDate,
			DbmsModificationDate = row.DbmsLastModificationDate,
			UpdateId = updateId,
			Title = row.Title
		};
		SetCustomFields(tableForSynchronization, row);
		return tableForSynchronization;
	}

	public bool Synchronize(ObjectRow table, int databaseId, bool isDbAdded, int updateId, CustomFieldsSupport customFieldsSupport, Form owner = null)
	{
		if (table == null)
		{
			return false;
		}
		try
		{
			bool saveDescription = false;
			bool saveTitle = false;
			TableForSynchronization tableForSynchronization = ConvertObjectRowToItem(table, databaseId, updateId);
			if (tableForSynchronization == null)
			{
				return false;
			}
			bool flag = false;
			if (!isDbAdded)
			{
				table.ObjectId = GetObjectIdByName(databaseId, table.Schema, table.Name, SharedObjectTypeEnum.TypeToString(table.ObjectTypeValue)) ?? (-1);
				flag = table.ObjectId < 0;
				TableObject dataById = DB.Table.GetDataById(table.ObjectId);
				HistoryCustomFieldsHelper.PrepareCustomFieldsForImport(table, databaseId, customFieldsSupport, dataById);
				saveDescription = HistoryGeneralHelper.CheckAreValuesDiffrentAndFirstValueIsNullForImport(PrepareValue.GetHtmlText(dataById?.DescriptionPlain, dataById?.Description), PrepareValue.GetHtmlText(tableForSynchronization?.DescriptionPlain, tableForSynchronization?.Description));
				saveTitle = HistoryGeneralHelper.CheckAreValuesDiffrentAndFirstValueIsNullForImport(dataById?.Title, table.Title);
			}
			table.ObjectId = commands.Synchronization.Tables.SynchronizeTable(databaseId, tableForSynchronization, isDbAdded, Dataedo.App.StaticData.IsProjectFile);
			if (isDbAdded)
			{
				HistoryCustomFieldsHelper.InsertCustomFieldsObjectWhenDBIsAddedOnImportChanges(table.ObjectId, table.CustomFields, databaseId, table.Type);
				DB.History.InsertHistoryRow(databaseId, table.ObjectId, table.Title, table.Description, table.DescriptionSearch, "tables", !string.IsNullOrEmpty(table.Title), !string.IsNullOrEmpty(table.Description), table.ObjectTypeValue);
			}
			else
			{
				if (flag)
				{
					HistoryCustomFieldsHelper.InsertCustomFieldsObjectWhenDBIsAddedOnImportChanges(table.ObjectId, table.CustomFields, databaseId, table.ObjectTypeValue);
				}
				DB.History.InsertHistoryRow(databaseId, table.ObjectId, table.Title, table.Description, table.DescriptionSearch, "tables", saveTitle, saveDescription, table.ObjectTypeValue);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while synchronizing the table:", owner);
			return false;
		}
		return true;
	}

	public bool Delete(List<int> ids, SharedObjectTypeEnum.ObjectType objectType, Form owner = null)
	{
		try
		{
			if (Dataedo.App.StaticData.IsProjectFile)
			{
				commands.Manipulation.Tables.DeleteTablesCE(ids, SharedObjectTypeEnum.TypeToString(objectType));
			}
			else
			{
				commands.Manipulation.Tables.DeleteTables(ids, SharedObjectTypeEnum.TypeToString(objectType));
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting the table:", owner);
			return false;
		}
		return true;
	}

	public bool UpdateTableFromManualImport(Table table)
	{
		try
		{
			commands.Manipulation.Tables.UpdateTableFromManualImport(table);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the table:");
			return false;
		}
		return true;
	}

	public bool UpdateTablesFromManualImport(Table[] tables, Action<Table, DbTransaction> additionalAction = null)
	{
		try
		{
			commands.Manipulation.Tables.UpdateTablesFromManualImport(tables, null, additionalAction);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the tables:");
			return false;
		}
		return true;
	}

	public bool UpdateTableDatabase(int tableID, int targetDatabaseId, Form owner)
	{
		return UpdateTablesDatabase(new List<int> { tableID }, targetDatabaseId, owner);
	}

	public bool UpdateTablesDatabase(List<int> tablesIDs, int targetDatabaseId, Form owner)
	{
		try
		{
			commands.Manipulation.Tables.UpdateTablesDatabase(tablesIDs, targetDatabaseId);
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while moving table to the database:", owner);
			return false;
		}
	}

	public bool CheckIfTableRecordAlreadyExists(int databaseId, string name, string schema, SharedObjectTypeEnum.ObjectType objectType, int tableId = -1)
	{
		return commands.Select.Tables.CheckIfTableRecordAlreadyExists(databaseId, name, schema, SharedObjectTypeEnum.TypeToString(objectType), tableId);
	}

	public List<ModuleTitleWithDatabaseTitle> GetObjectsTop5ModulesNamesWithDatabase(int tableId)
	{
		return commands.Select.Tables.GetObjectsTop5ModulesNamesWithDatabase(tableId);
	}
}
