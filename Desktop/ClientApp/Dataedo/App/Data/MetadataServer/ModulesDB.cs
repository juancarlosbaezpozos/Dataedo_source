using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.MenuTree;
using Dataedo.App.Synchronization.Tools;
using Dataedo.App.Tools.ERD;
using Dataedo.App.Tools.ERD.Diagram;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Data.Base.Commands.Results;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.DataProcessing.MetadataServer;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.Erd;
using Dataedo.Model.Data.History;
using Dataedo.Model.Data.Modules;
using Dataedo.Shared.Enums;
using DevExpress.Utils.Extensions;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Data.MetadataServer;

internal class ModulesDB : BaseCommonDBSupport
{
	public ModulesDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public List<ModuleObject> GetDataByDatabase(int databaseId)
	{
		return commands.Select.Modules.GetModules(databaseId, null);
	}

	public List<int?> GetDataIdsByDatabase(int databaseId)
	{
		return commands.Select.Modules.GetModulesIds(databaseId);
	}

	public List<int> GetModulesIdByObjectId(int objectId, string tableName, string columnName)
	{
		List<int?> modulesIdByObjectId = commands.Select.Modules.GetModulesIdByObjectId(objectId, tableName, columnName);
		List<int> list = new List<int>();
		foreach (int? item in modulesIdByObjectId)
		{
			list.Add(item ?? (-1));
		}
		return list;
	}

	public bool IsModuleContainsTable(int moduleId, int tableId)
	{
		return commands.Select.Modules.IsModuleContainsTable(moduleId, tableId);
	}

	public void BulkCopyModulesUpdate(int id, Dictionary<string, object> keyValuePairs)
	{
		try
		{
			commands.Manipulation.Modules.BulkCopyModulesUpdateSingleModule(id, keyValuePairs);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating modules");
		}
	}

	public void BulkCopyModulesUpdate(List<int> ids, string value, string fieldName, Form owner = null)
	{
		try
		{
			commands.Manipulation.Modules.BulkCopyModulesUpdate(ids, value, fieldName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating modules", owner);
		}
	}

	public int? CountModulesScalarQuery(int databaseId)
	{
		return commands.Select.Modules.GetCountOfModulesInDatabase(databaseId);
	}

	public List<string> GetDataByDatabaseNewTitle(int databaseId, string titleTextToCheck = null)
	{
		return commands.Select.Modules.GetTitleExistingNumbers(databaseId, titleTextToCheck);
	}

	public bool DoNewModuleTitleExists(int databaseId, string titleTextToCheck = null)
	{
		return commands.Select.Modules.CheckIfTitleExists(databaseId, titleTextToCheck);
	}

	public List<ModuleWithoutDescriptionObject> GetDataByDatabaseWithoutDescription(int? databaseId)
	{
		return commands.Select.Modules.GetModulesWithoutDescription(databaseId);
	}

	public List<ModuleWithoutDescriptionObject> GetDataByDatabaseWithoutDescription()
	{
		return commands.Select.Modules.GetAllModulesWithoutDescription();
	}

	public List<int?> GetModulesIdsByDatabase(int? database)
	{
		return commands.Select.Modules.GetModulesIdsByDatabase(database);
	}

	public List<ModuleForMenuObject> GetDataByDatabaseForMenu(int databaseId)
	{
		return commands.Select.Modules.GetModulesForMenu(databaseId);
	}

	public ModuleObject GetDataById(int moduleId)
	{
		List<ModuleObject> modules = commands.Select.Modules.GetModules(null, moduleId);
		if (modules.Count <= 0)
		{
			return null;
		}
		return modules[0];
	}

	public bool Update(int moduleId, string title, Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> customFields = null, string description = null, string descriptionSearch = null, string erdLinkStyle = "STRAIGHT", bool erdShowTypes = false, string displayDocumentationNameMode = "EXTERNAL_ENTITIES_ONLY", bool erdShowNullable = false, Form owner = null)
	{
		try
		{
			commands.Manipulation.Modules.UpdateModule(moduleId, title, description, descriptionSearch, erdLinkStyle, erdShowTypes, displayDocumentationNameMode, customFields, erdShowNullable);
			DB.Community.InsertFollowingToRepository(SharedObjectTypeEnum.ObjectType.Module, moduleId);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the module:", owner);
			return false;
		}
		return true;
	}

	public bool Update(ModuleSyncRow row, Form owner = null)
	{
		try
		{
			Module module = ConvertRowToItem(row);
			commands.Manipulation.Modules.UpdateModuleWithCustomFields(module);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the module:", owner);
			return false;
		}
		return true;
	}

	private Module ConvertRowToItem(ModuleSyncRow row)
	{
		Module module = new Module
		{
			Id = row.Id,
			Title = row.Title,
			Description = row.Description,
			DescriptionPlain = row.DescriptionPlain,
			ErdLinkStyle = row.ErdLinkStyle,
			ErdShowTypes = row.ErdShowTypes,
			DisplayDocumentationNameMode = row.DisplayDocumentationNameMode,
			ErdShowNullable = row.ErdShowNullable
		};
		SetCustomFields(module, row);
		return module;
	}

	public bool UpdateOrdinalPosition(int moduleId, int? ordinalPosition = null, Form owner = null)
	{
		if (ordinalPosition.HasValue)
		{
			try
			{
				commands.Manipulation.Modules.UpdateModuleOrdinalPosition(moduleId, ordinalPosition);
			}
			catch (Exception exception)
			{
				GeneralExceptionHandling.Handle(exception, "Error while updating the module:", owner);
				return false;
			}
		}
		return true;
	}

	public int Insert(DBTreeNode dbTreeNode, string title, string description = null, string descriptionSearch = null, string erdLinkStyle = "STRAIGHT", bool erdShowTypes = false, string displayDocumentationNameMode = "EXTERNAL_ENTITIES_ONLY", bool erdShowNullable = false, Form owner = null)
	{
		try
		{
			int? num = commands.Manipulation.Modules.InsertModule(dbTreeNode.DatabaseId, title, description, descriptionSearch, erdLinkStyle, erdShowTypes, displayDocumentationNameMode, erdShowNullable);
			dbTreeNode.Id = num.Value;
			dbTreeNode.InsertElement = DBTreeNode.InsertElementEnum.AlreadyInserted;
			dbTreeNode.UpdateIdForDirectories();
			return Convert.ToInt32(num);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while inserting the module:", owner);
			return -1;
		}
	}

	public int Insert(int databaseId, string title, string description = null, string descriptionSearch = null, string erdLinkStyle = "STRAIGHT", bool erdShowTypes = false, string displayDocumentationNameMode = "EXTERNAL_ENTITIES_ONLY", bool erdShowNullable = false, Form owner = null)
	{
		try
		{
			return Convert.ToInt32(commands.Manipulation.Modules.InsertModule(databaseId, title, description, descriptionSearch, erdLinkStyle, erdShowTypes, displayDocumentationNameMode, erdShowNullable));
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while inserting the module:", owner);
			return -1;
		}
	}

	public void InsertManualTable(int moduleId, int tableId)
	{
		commands.Manipulation.Modules.InsertModuleTable(moduleId, tableId);
	}

	public void InsertManualProcedure(int moduleId, int procedureId)
	{
		commands.Manipulation.Modules.InsertModuleProcedure(moduleId, procedureId);
	}

	public bool Delete(List<int> ids, Form owner = null)
	{
		try
		{
			if (Dataedo.App.StaticData.IsProjectFile)
			{
				commands.Manipulation.Modules.DeleteModulesCE(ids);
			}
			else
			{
				commands.Manipulation.Modules.DeleteModules(ids);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting the module:", owner);
			return false;
		}
		return true;
	}

	public bool InsertModuleObject(int moduleId, int objectId, SharedObjectTypeEnum.ObjectType type, Form owner = null)
	{
		try
		{
			switch (type)
			{
			case SharedObjectTypeEnum.ObjectType.Table:
			case SharedObjectTypeEnum.ObjectType.View:
			case SharedObjectTypeEnum.ObjectType.Structure:
				commands.Manipulation.Modules.InsertModuleTable(moduleId, objectId);
				break;
			case SharedObjectTypeEnum.ObjectType.Function:
			case SharedObjectTypeEnum.ObjectType.Procedure:
				commands.Manipulation.Modules.InsertModuleProcedure(moduleId, objectId);
				break;
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while inserting object to module", owner);
			return false;
		}
		return true;
	}

	public bool InsertModuleObjects(IEnumerable<Node> nodes, int moduleId, Form owner = null)
	{
		if (nodes == null || nodes.Count() == 0)
		{
			return true;
		}
		try
		{
			List<ObjectIdTypeForModule> list = ConvertingToTables.ReloadModuleObjectsTable(moduleId, nodes);
			commands.Manipulation.Modules.InsertModuleObjects(list.ToArray(), Dataedo.App.StaticData.IsProjectFile);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while adding objects to the module., handlingWindowOwner: owner");
			return false;
		}
		return true;
	}

	public bool AddLinks(int moduleId, List<DBTreeNode> objectNodes, out int[] succeedObjectsIds, Form owner = null)
	{
		try
		{
			List<ObjectIdTypeForModule> list = ConvertingToTables.ReloadModuleObjectsTable(moduleId, objectNodes);
			foreach (ObjectIdTypeForModule item in list)
			{
				item.ModuleId = moduleId;
			}
			List<ValueWithDataArrayResult<int, int>> source = commands.Manipulation.Modules.InsertModuleObjects(list.ToArray(), Dataedo.App.StaticData.IsProjectFile);
			succeedObjectsIds = source.Where((ValueWithDataArrayResult<int, int> r) => r.Result == 0).SelectMany((ValueWithDataArrayResult<int, int> r) => r.Data).ToArray();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while adding objects to the module:", owner);
			succeedObjectsIds = new int[0];
			return false;
		}
		return true;
	}

	public bool DeleteLinks(int? moduleId, GridView gridView, SharedObjectTypeEnum.ObjectType objectType, out int[] succeedObjectsIds)
	{
		try
		{
			List<ObjectIdTypeForModule> list = ConvertingToTables.ReloadModuleObjectsTable(gridView, objectType);
			foreach (ObjectIdTypeForModule item in list)
			{
				item.ModuleId = moduleId;
			}
			List<ValueWithDataArrayResult<int, int>> source = commands.Manipulation.Modules.DeleteModuleObjects(list.ToArray());
			succeedObjectsIds = source.Where((ValueWithDataArrayResult<int, int> r) => r.Result == 0).SelectMany((ValueWithDataArrayResult<int, int> r) => r.Data).ToArray();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting objects from the module:", gridView?.GridControl?.FindForm());
			succeedObjectsIds = new int[0];
			return false;
		}
		return true;
	}

	public bool DeleteLinks(DBTreeNode dbTreeNode, Form owner = null)
	{
		try
		{
			int id = dbTreeNode.ParentNode.Id;
			List<ObjectIdTypeForModule> list = ConvertingToTables.ReloadModuleObjectsTable(id, new List<DBTreeNode> { dbTreeNode });
			foreach (ObjectIdTypeForModule item in list)
			{
				item.ModuleId = id;
			}
			commands.Manipulation.Modules.DeleteModuleObjects(list.ToArray());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting objects from the module:", owner);
			return false;
		}
		return true;
	}

	public bool CopyModuleObjects(int sourceModuleId, int destinationModuleId, SharedObjectTypeEnum.ObjectType objectType, Form owner = null)
	{
		try
		{
			List<ObjectByModuleObject> list = null;
			switch (objectType)
			{
			case SharedObjectTypeEnum.ObjectType.Table:
				list = DB.Table.GetTablesByModule(sourceModuleId);
				break;
			case SharedObjectTypeEnum.ObjectType.Function:
				list = DB.Procedure.GetFunctionsByModule(sourceModuleId);
				break;
			case SharedObjectTypeEnum.ObjectType.Procedure:
				list = DB.Procedure.GetProceduresByModule(sourceModuleId);
				break;
			case SharedObjectTypeEnum.ObjectType.View:
				list = DB.Table.GetViewsByModule(sourceModuleId);
				break;
			case SharedObjectTypeEnum.ObjectType.Structure:
				list = DB.Table.GetStructuresByModule(sourceModuleId);
				break;
			}
			if (list != null)
			{
				foreach (ObjectByModuleObject item in list)
				{
					DB.Module.InsertModuleObject(destinationModuleId, item.Id, objectType);
				}
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while copying objects from the module:", owner);
			return false;
		}
		return true;
	}

	public bool CopyModuleDiagram(int sourceModuleId, int destinationModuleId, Form owner = null)
	{
		try
		{
			ErdNode[] array = DB.ErdNode.GetExistingErdNodesByModuleId(sourceModuleId).ToArray();
			List<ErdNodeColumn> erdColumnsOfModule = DB.NodeColumn.GetExistingErdColumnsByModuleId(sourceModuleId);
			ErdNodeColumn[][] array2 = array.Select((ErdNode x) => erdColumnsOfModule.Where((ErdNodeColumn y) => y.NodeId == x.Id).ToArray()).ToArray();
			ErdPostIt[] array3 = DB.ErdPostIt.GetPostItsByModuleId(sourceModuleId).ToArray();
			array.ForEach(delegate(ErdNode x)
			{
				x.Id = null;
				x.ModuleId = destinationModuleId;
			});
			array2.ForEach(delegate(ErdNodeColumn[] x)
			{
				x.ForEach(delegate(ErdNodeColumn y)
				{
					y.Id = null;
					y.NodeId = null;
					y.ModuleId = destinationModuleId;
				});
			});
			array3.ForEach(delegate(ErdPostIt x)
			{
				x.Id = -1;
				x.ModuleId = destinationModuleId;
			});
			DB.ErdNode.InsertOrUpdateNodes(array);
			for (int i = 0; i < array.Length; i++)
			{
				ErdNode node = array[i];
				array2[i].ForEach(delegate(ErdNodeColumn x)
				{
					x.NodeId = node.Id;
				});
			}
			DB.NodeColumn.InsertOrUpdateErdNodeColumns(array2.SelectMany((ErdNodeColumn[] n) => n).ToArray());
			DB.ErdPostIt.InsertOrUpdatePostIts(array3);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while copying module diagram:", owner);
			return false;
		}
		return true;
	}

	public bool CopyModuleRelations(int sourceModuleId, int destinationModuleId, Form owner = null)
	{
		try
		{
			List<LinkDB> list = new List<LinkDB>();
			foreach (RelationLinkObject item in DB.Relation.GetDataByModule(sourceModuleId))
			{
				if (item.LinkId.HasValue)
				{
					list.Add(new LinkDB
					{
						hidden = (item.Hidden ?? true),
						link_id = item.LinkId,
						link_style = item.LinkStyle,
						show_join_condition = (item.ShowJoinCondition ?? true),
						relation_id = item.TableRelationId,
						show_label = (item.ShowLabel ?? true),
						module_id = destinationModuleId
					});
				}
			}
			DB.ErdLink.InsertOrUpdateLinks(list);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while copying module relationships:", owner);
			return false;
		}
		return true;
	}

	public bool CopyModuleCustomFields(List<CustomFieldRowExtended> customFields, ModuleObject sourceModuleObject, int databaseId, int destinationModuleId, string destinationModuleTitle, string descriptionSearch, Form owner = null)
	{
		try
		{
			if (customFields == null || customFields.Count == 0 || sourceModuleObject == null)
			{
				return false;
			}
			Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> dictionary = new Dictionary<string, BaseWithCustomFields.CustomFieldWithValue>();
			List<HistoryModel> historyModelListToBulkInsert = new List<HistoryModel>();
			foreach (CustomFieldRowExtended customField in customFields)
			{
				object obj = sourceModuleObject.GetType()?.GetProperty(customField.FieldPropertyName)?.GetValue(sourceModuleObject, null);
				if (obj == null)
				{
					continue;
				}
				BaseWithCustomFields.CustomFieldWithValue customFieldWithValue = customField.GetCustomFieldWithValue(obj.ToString());
				if (customFieldWithValue != null)
				{
					dictionary.Add(customField.FieldName, customFieldWithValue);
					if (!string.IsNullOrEmpty(customFieldWithValue.Value))
					{
						HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, destinationModuleId, null, customFieldWithValue.Value, null, saveTitle: false, saveDescription: false, saveCustomfield: true, SharedObjectTypeEnum.ObjectType.Module, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(SharedObjectTypeEnum.ObjectType.Module), customField.FieldName);
					}
				}
			}
			DB.Module.Update(destinationModuleId, destinationModuleTitle, dictionary, sourceModuleObject.Description, descriptionSearch, sourceModuleObject.ErdLinkStyle, sourceModuleObject.ErdShowTypes.GetValueOrDefault(), sourceModuleObject.DisplayDocumentationNameMode);
			HistoryBulkInserterHelper.BulkInsertHistoryModels(historyModelListToBulkInsert);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while copying module custom fields:", owner);
			return false;
		}
		return true;
	}

	public bool UpdateModuleDatabase(int moduleID, int targetDatabaseId, Form owner)
	{
		return UpdateModulesDatabase(new List<int> { moduleID }, targetDatabaseId, owner);
	}

	public bool UpdateModulesDatabase(List<int> modulesIDs, int targetDatabaseId, Form owner)
	{
		try
		{
			commands.Manipulation.Modules.UpdateModulesDatabase(modulesIDs, targetDatabaseId);
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while moving the module to the database:", owner);
			return false;
		}
	}

	public bool RemoveDatabaseModulesOrdinalPosition(int databaseID, Form owner)
	{
		try
		{
			commands.Manipulation.Modules.RemoveDatabaseModulesOrdinalPosition(databaseID);
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while removing subject areas position:", owner);
			return false;
		}
	}
}
