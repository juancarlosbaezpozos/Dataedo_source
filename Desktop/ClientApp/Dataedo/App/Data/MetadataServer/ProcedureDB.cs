using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.FeedbackWidgetData;
using Dataedo.Model.Data.Modules;
using Dataedo.Model.Data.Procedures.Procedures;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer;

internal class ProcedureDB : CommonDBSupport
{
	public ProcedureDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public BaseDataObject GetDataByName(int databaseId, string name, string schema, string subtype)
	{
		return commands.Select.Procedures.GetObjectsByName(databaseId, name, schema, subtype);
	}

	public int GetIdByName(int databaseId, string name, string schema, string subtype)
	{
		return GetDataByName(databaseId, name, schema, subtype)?.Id ?? (-1);
	}

	public List<ObjectDeletedFromDatabaseObject> GetDataDeletedFromDBMS(int databaseId, SharedObjectTypeEnum.ObjectType objectType, string filter, bool updateEntireDocumentation)
	{
		if (updateEntireDocumentation)
		{
			return commands.Select.Procedures.GetObjectsDeletedFromDatabase(databaseId, SharedObjectTypeEnum.TypeToString(objectType), filter);
		}
		return commands.Select.Procedures.GetObjectsDeletedFromDatabaseWithoutFilter(databaseId, SharedObjectTypeEnum.TypeToString(objectType), filter);
	}

	public ProcedureObject GetDataById(int procedureId)
	{
		return commands.Select.Procedures.GetObject(procedureId);
	}

	public List<int> GetProcedureModules(int procedureId)
	{
		return commands.Select.Procedures.GetObjectModules(procedureId);
	}

	public void BulkCopyProcedureUpdate(List<int> ids, string value, string fieldName, Form owner = null)
	{
		try
		{
			commands.Manipulation.Procedures.BulkCopyProcedureUpdate(ids, value, fieldName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the table", owner);
		}
	}

	public void BulkCopyProcedureUpdate(int id, Dictionary<string, object> keyValuePairs)
	{
		try
		{
			commands.Manipulation.Procedures.BulkCopyProcedureUpdateSingleProcedureUpdate(id, keyValuePairs);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the table");
		}
	}

	public List<ObjectForMenuObject> GetProceduresByDatabaseForMenu(int databaseId)
	{
		return commands.Select.Procedures.GetObjectsByDatabaseForMenu(databaseId, true);
	}

	public List<ObjectForMenuObject> GetProceduresByModuleForMenu(int moduleId)
	{
		return commands.Select.Procedures.GetObjectsByModuleForMenu(moduleId, true);
	}

	public List<ObjectForMenuObjectWithModuleId> GetObjectsForMenuByDatabaseForModules(int databaseId, SharedObjectTypeEnum.ObjectType objectType)
	{
		return commands.Select.Procedures.GetObjectsForMenuByDatabaseForModules(databaseId, objectType);
	}

	public List<ObjectForMenuObject> GetFunctionsByDatabaseForMenu(int databaseId)
	{
		return commands.Select.Procedures.GetObjectsByDatabaseForMenu(databaseId, false);
	}

	public List<ObjectForMenuObject> GetFunctionsByModuleForMenu(int moduleId)
	{
		return commands.Select.Procedures.GetObjectsByModuleForMenu(moduleId, false);
	}

	public List<ObjectDocObject> GetProceduresByModuleDoc(int moduleId)
	{
		return commands.Select.Procedures.GetObjectsByModuleDoc(moduleId, true);
	}

	public List<ObjectWithModulesObject> GetProceduresByModuleWithoutDescription(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Procedures.GetObjectsByModuleWithModulesWithoutDescription(moduleId, true, GetNotStatusValue(notDeletedOnly));
	}

	public List<ObjectDocObject> GetProceduresWithoutModuleDoc(int databaseId)
	{
		return commands.Select.Procedures.GetObjectsWithoutModuleDoc(databaseId, true);
	}

	public List<ObjectDocObject> GetFunctionsByModuleDoc(int moduleId)
	{
		return commands.Select.Procedures.GetObjectsByModuleDoc(moduleId, false);
	}

	public List<int> GetFunctionsIdsByDatabase(int databaseId, bool notDeletedOnly = false)
	{
		return commands.Select.Procedures.GetObjectsIdsByDatabase(databaseId, false, GetNotStatusValue(notDeletedOnly));
	}

	public List<int> GetProceduresIdsByDatabase(int databaseId, bool notDeletedOnly = false)
	{
		return commands.Select.Procedures.GetObjectsIdsByDatabase(databaseId, true, GetNotStatusValue(notDeletedOnly));
	}

	public List<ObjectByModuleObject> GetFunctionsByModule(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Procedures.GetObjectsByModule(moduleId, false, GetNotStatusValue(notDeletedOnly));
	}

	public List<int> GetFunctionsIdsByModule(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Procedures.GetObjectsIdsByModule(moduleId, false, GetNotStatusValue(notDeletedOnly));
	}

	public List<ObjectByModuleObject> GetFunctionsWithoutModule(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Procedures.GetObjectsWithoutModule(moduleId, false);
	}

	public List<ObjectByModuleObject> GetProceduresByModule(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Procedures.GetObjectsByModule(moduleId, true, GetNotStatusValue(notDeletedOnly));
	}

	public List<int> GetProceduresIdsByModule(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Procedures.GetObjectsIdsByModule(moduleId, true, GetNotStatusValue(notDeletedOnly));
	}

	public List<ObjectByModuleObject> GetProceduresWithoutModule(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Procedures.GetObjectsWithoutModule(moduleId, true);
	}

	public List<ObjectWithModulesObject> GetFunctionsByModuleWithoutDescription(int moduleId, bool notDeletedOnly = false)
	{
		return commands.Select.Procedures.GetObjectsByModuleWithModulesWithoutDescription(moduleId, false, GetNotStatusValue(notDeletedOnly));
	}

	public List<ObjectDocObject> GetFunctionsWithoutModuleDoc(int databaseId)
	{
		return commands.Select.Procedures.GetObjectsWithoutModuleDoc(databaseId, false);
	}

	public List<ObjectWithModulesObject> GetFunctionsByDatabaseWithoutDescription(int databaseId, bool notDeletedOnly = false)
	{
		string notWithStatus = null;
		if (notDeletedOnly)
		{
			notWithStatus = "D";
		}
		return commands.Select.Procedures.GetObjectsByDatabaseWithModulesWithoutDescription(databaseId, false, notWithStatus);
	}

	public List<ObjectWithModulesObject> GetProceduresByDatabaseWithoutDescription(int databaseId, bool notDeletedOnly = false)
	{
		string notWithStatus = null;
		if (notDeletedOnly)
		{
			notWithStatus = "D";
		}
		return commands.Select.Procedures.GetObjectsByDatabaseWithModulesWithoutDescription(databaseId, true, notWithStatus);
	}

	public bool AreObjectsWithoutModule(int databaseId)
	{
		return commands.Select.Procedures.GetCountOfObjectsWithoutModule(databaseId) > 0;
	}

	public bool Update(int databaseId, int id, SharedObjectTypeEnum.ObjectType objectType, string title, List<int> intModulesIds, Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> customFields, string description = null, string descriptionSearch = null, Form owner = null)
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
				commands.Manipulation.Procedures.UpdateProcedureCE(id, SharedObjectTypeEnum.TypeToString(objectType), title, description, descriptionSearch, intModulesIds.ToArray(), customFields);
			}
			else
			{
				FeedbackWidgetDataObject objectsFeedbackWidgetData = commands.Select.Procedures.GetObjectsFeedbackWidgetData(id);
				commands.Manipulation.Procedures.UpdateProcedureServer(id, SharedObjectTypeEnum.TypeToString(objectType), title, description, descriptionSearch, intModulesIds.ToArray(), customFields, objectsFeedbackWidgetData);
			}
			if (!CommonFunctionsPanels.summaryObjectTitleHistory.ContainsKey(id))
			{
				CommonFunctionsPanels.summaryObjectTitleHistory.Add(id, title);
			}
			DB.History.InsertHistoryRow(databaseId, id, title, null, null, "procedures", HistoryGeneralHelper.CheckAreValuesDiffrent(title, CommonFunctionsPanels.summaryObjectTitleHistory[id]), saveDescription: false, objectType);
			CommonFunctionsPanels.summaryObjectTitleHistory[id] = title;
			DB.Community.InsertFollowingToRepository(objectType, id);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the procedure:", owner);
			return false;
		}
		return true;
	}

	public bool Update(ProcedureRow row, Form owner = null)
	{
		try
		{
			Procedure procedure = ConvertRowToItem(row);
			if (Dataedo.App.StaticData.IsProjectFile)
			{
				commands.Manipulation.Procedures.UpdateProcedureWithCustomFieldsCE(procedure);
			}
			else
			{
				FeedbackWidgetDataObject objectsFeedbackWidgetData = commands.Select.Procedures.GetObjectsFeedbackWidgetData(procedure.Id);
				commands.Manipulation.Procedures.UpdateProcedureWithCustomFieldsServer(procedure, objectsFeedbackWidgetData);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the procedure:", owner);
			return false;
		}
		return true;
	}

	private Procedure ConvertRowToItem(ProcedureRow row)
	{
		Procedure procedure = new Procedure
		{
			Id = row.Id,
			ObjectType = SharedObjectTypeEnum.TypeToString(row.ObjectType),
			Title = row.Title,
			Description = row.Description,
			DescriptionPlain = row.DescriptionPlain,
			Modules = row.Modules
		};
		SetCustomFields(procedure, row);
		return procedure;
	}

	private ProcedureForSynchronization ConvertObjectRowToItem(ObjectRow row, int databaseId, int updateId)
	{
		ProcedureForSynchronization procedureForSynchronization = new ProcedureForSynchronization
		{
			Name = row.Name,
			Schema = row.Schema,
			DatabaseId = databaseId,
			ObjectType = row.TypeAsString,
			Description = row.Description,
			DescriptionPlain = row.DescriptionSearch,
			Definition = row.Definition,
			FunctionType = row.FunctionType,
			DbmsCreationDate = row.DbmsCreationDate,
			DbmsModificationDate = row.DbmsLastModificationDate,
			Subtype = SharedObjectSubtypeEnum.TypeToString(row.Type, row.Subtype),
			Language = row.Language,
			UpdateId = updateId
		};
		SetCustomFields(procedureForSynchronization, row);
		return procedureForSynchronization;
	}

	public bool Synchronize(ObjectRow procedure, int databaseId, bool isDbAdded, int updateId, CustomFieldsSupport customFieldsSupport, Form owner = null)
	{
		if (procedure == null)
		{
			return false;
		}
		try
		{
			bool saveDescription = false;
			bool saveTitle = false;
			ProcedureForSynchronization procedureForSynchronization = ConvertObjectRowToItem(procedure, databaseId, updateId);
			if (procedureForSynchronization == null)
			{
				return false;
			}
			bool flag = false;
			if (!isDbAdded)
			{
				procedure.ObjectId = GetIdByName(databaseId, procedure.Name, procedure.Schema, SharedObjectTypeEnum.TypeToString(procedure.ObjectTypeValue));
				flag = procedure.ObjectId < 0;
				ProcedureObject dataById = DB.Procedure.GetDataById(procedure.ObjectId);
				HistoryCustomFieldsHelper.PrepareCustomFieldsForImport(procedure, databaseId, customFieldsSupport, dataById);
				saveDescription = HistoryGeneralHelper.CheckAreValuesDiffrentAndFirstValueIsNullForImport(PrepareValue.GetHtmlText(dataById?.DescriptionPlain, dataById?.Description), PrepareValue.GetHtmlText(procedureForSynchronization.DescriptionPlain, procedureForSynchronization.Description));
				saveTitle = HistoryGeneralHelper.CheckAreValuesDiffrentAndFirstValueIsNullForImport(dataById?.Title, procedure?.Title);
			}
			procedure.ObjectId = commands.Synchronization.Procedures.SynchronizeProcedure(databaseId, procedureForSynchronization, isDbAdded, Dataedo.App.StaticData.IsProjectFile);
			if (isDbAdded)
			{
				HistoryCustomFieldsHelper.InsertCustomFieldsObjectWhenDBIsAddedOnImportChanges(procedure.ObjectId, procedure.CustomFields, databaseId, procedure.Type);
				DB.History.InsertHistoryRow(databaseId, procedure.ObjectId, procedure.Title, procedure.Description, procedure.DescriptionSearch, "procedures", !string.IsNullOrEmpty(procedure.Title), !string.IsNullOrEmpty(procedure.Description), procedure.ObjectTypeValue);
			}
			else
			{
				if (flag)
				{
					HistoryCustomFieldsHelper.InsertCustomFieldsObjectWhenDBIsAddedOnImportChanges(procedure.ObjectId, procedure.CustomFields, databaseId, procedure.ObjectTypeValue);
					saveTitle = !string.IsNullOrEmpty(procedure.Description);
					saveDescription = !string.IsNullOrEmpty(procedure.Description);
				}
				DB.History.InsertHistoryRow(databaseId, procedure.ObjectId, procedure.Title, procedure.Description, procedure.DescriptionSearch, "procedures", saveTitle, saveDescription, procedure.ObjectTypeValue);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while synchronizing the procedure:", owner);
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
				commands.Manipulation.Procedures.DeleteProceduresCE(ids, SharedObjectTypeEnum.TypeToString(objectType));
			}
			else
			{
				commands.Manipulation.Procedures.DeleteProcedures(ids, SharedObjectTypeEnum.TypeToString(objectType));
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting the procedure:", owner);
			return false;
		}
		return true;
	}

	public int? InsertManualProcedure(ObjectRow procedure, Form owner = null)
	{
		try
		{
			return commands.Manipulation.Procedures.InsertManualProcedure(ConvertObjectRowToManualProcedure(procedure), Dataedo.App.StaticData.IsProjectFile);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while inserting manual procedure", owner);
			return null;
		}
	}

	private ManualProcedure ConvertObjectRowToManualProcedure(ObjectRow table)
	{
		ManualProcedure manualProcedure = new ManualProcedure
		{
			DatabaseId = table.DatabaseId.Value,
			Schema = table.Schema,
			Name = table.Name,
			Title = table.Title,
			Description = table.Description,
			ObjectType = SharedObjectTypeEnum.TypeToString(table.ObjectTypeValue),
			Definition = table.Definition,
			Status = ObjectStatusEnum.StatusToShortString(table.Status),
			FunctionType = table.FunctionType,
			DescriptionSearch = table.DescriptionSearch,
			Subtype = SharedObjectSubtypeEnum.TypeToString(table.ObjectTypeValue, table.Subtype),
			Language = table.Language,
			Source = table.Source
		};
		if (table.ObjectId > 0)
		{
			manualProcedure.ProcedureId = table.ObjectId;
		}
		return manualProcedure;
	}

	public void UpdateManualProcedure(int procedureId, string schema, string name, string title, string type, string language, string definition, string source, Form owner = null)
	{
		try
		{
			commands.Manipulation.Procedures.UpdateManualProcedure(procedureId, schema, name, title, type, language, definition, source);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating manual procedure", owner);
		}
	}

	public bool UpdateProcedureDatabase(int procedureID, int targetDatabaseId, Form owner)
	{
		return UpdateProceduresDatabase(new List<int> { procedureID }, targetDatabaseId, owner);
	}

	public bool UpdateProceduresDatabase(List<int> proceduresIDs, int targetDatabaseId, Form owner)
	{
		try
		{
			commands.Manipulation.Procedures.UpdateProceduresDatabase(proceduresIDs, targetDatabaseId);
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while moving procedure to the database:", owner);
			return false;
		}
	}

	public bool CheckIfIsProcedureRecordAlreadyExists(int databaseId, string name, string schema, SharedObjectTypeEnum.ObjectType objectType, int procedureId = -1)
	{
		return commands.Select.Procedures.CheckIfProcedureRecordAlreadyExists(databaseId, name, schema, SharedObjectTypeEnum.TypeToString(objectType), procedureId);
	}

	public List<ModuleTitleWithDatabaseTitle> GetObjectsTop5ModulesNamesWithDatabase(int procedureId)
	{
		return commands.Select.Procedures.GetObjectsTop5ModulesNamesWithDatabase(procedureId);
	}
}
