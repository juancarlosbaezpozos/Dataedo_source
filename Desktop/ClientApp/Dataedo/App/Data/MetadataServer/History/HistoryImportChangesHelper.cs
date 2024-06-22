using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools.CustomFields;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.History;
using Dataedo.Model.Data.Procedures.Parameters;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Model.Data.Tables.Triggers;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer.History;

internal static class HistoryImportChangesHelper
{
	internal static void CheckColumnChangesInColumnDB(IEnumerable<ColumnRow> columns, int databaseId, int tableId, bool isDbAdded, bool savingEnabled, CustomFieldsSupport customFieldsSupport)
	{
		if (!savingEnabled || isDbAdded || tableId < 0)
		{
			return;
		}
		List<ColumnObject> dataByTable = DB.Column.GetDataByTable(tableId);
		foreach (ColumnRow column in columns)
		{
			ColumnObject columnObject = dataByTable?.Where((ColumnObject x) => x.Name == column.Name).FirstOrDefault();
			column.Id = columnObject?.ColumnId ?? (-1);
			column.ObjectIsAddedImportHistory = column.Id < 0;
			if (column.Id >= 0)
			{
				column.IsTitleChangedHistory = HistoryGeneralHelper.CheckAreValuesDiffrentAndFirstValueIsNullForImport(columnObject?.Title, column.Title);
				column.IsDescriptionChangedHistory = HistoryGeneralHelper.CheckAreValuesDiffrentAndFirstValueIsNullForImport(columnObject?.Description, column.Description);
				if (columnObject != null)
				{
					CustomFieldContainer customFieldContainer = new CustomFieldContainer(column.ObjectType, column.Id, customFieldsSupport);
					customFieldContainer.RetrieveCustomFields(columnObject);
					customFieldContainer.ClearAddedDefinitionValues(null);
					HistoryCustomFieldsHelper.InsertHistoryCustomFieldsWhenDBIsNotAddedOnImportChanges(column.Id, column.CustomFields, databaseId, column.ObjectType, customFieldContainer);
				}
			}
		}
	}

	internal static void InsertHistoryRowsInColumnDB(IEnumerable<ColumnRow> columns, int databaseId, int tableId, bool isDbAdded)
	{
		if (!DB.History.SavingEnabled || tableId < 0)
		{
			return;
		}
		List<ColumnObject> dataByTable = DB.Column.GetDataByTable(tableId);
		List<HistoryModel> historyModelListToBulkInsert = new List<HistoryModel>();
		foreach (ColumnRow column in columns)
		{
			if (column.Id < 1)
			{
				ColumnObject columnObject = dataByTable.Where((ColumnObject x) => x.Name == column.Name).FirstOrDefault();
				column.Id = columnObject?.ColumnId ?? (-1);
			}
			if (column.Id < 0)
			{
				continue;
			}
			if (isDbAdded)
			{
				HistoryCustomFieldsHelper.InsertCustomFieldsObjectWhenDBIsAddedOnImportChanges(column.Id, column.CustomFields, databaseId, column.ObjectType);
				column.IsDescriptionChangedHistory = !string.IsNullOrEmpty(column.Description);
				column.IsTitleChangedHistory = !string.IsNullOrEmpty(column.Title);
				HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, column.Id, column.Title, column.Description, null, column.IsTitleChangedHistory, column.IsDescriptionChangedHistory, saveCustomfield: false, column.ObjectType, "columns", null);
				continue;
			}
			if (column.ObjectIsAddedImportHistory)
			{
				HistoryCustomFieldsHelper.InsertCustomFieldsObjectWhenDBIsAddedOnImportChanges(column.Id, column.CustomFields, databaseId, column.ObjectType);
				column.IsDescriptionChangedHistory = !string.IsNullOrEmpty(column.Description);
				column.IsTitleChangedHistory = !string.IsNullOrEmpty(column.Title);
			}
			HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, column.Id, column.Title, column.Description, null, column.IsTitleChangedHistory, column.IsDescriptionChangedHistory, saveCustomfield: false, column.ObjectType, "columns", null);
		}
		HistoryBulkInserterHelper.BulkInsertHistoryModels(historyModelListToBulkInsert);
	}

	internal static void CheckParameterChangesInParameterDB(IEnumerable<ParameterRow> parameters, int databaseId, int procedureId, bool isDbAdded, bool savingEnabled, CustomFieldsSupport customFieldsSupport)
	{
		if (!savingEnabled || isDbAdded || procedureId < 0)
		{
			return;
		}
		List<ParameterObject> dataByProcedureId = DB.Parameter.GetDataByProcedureId(procedureId);
		foreach (ParameterRow parameter in parameters)
		{
			ParameterObject parameterObject = dataByProcedureId?.Where((ParameterObject x) => x.Name == parameter.Name).FirstOrDefault();
			parameter.Id = parameterObject?.ParameterId ?? (-1);
			parameter.ObjectIsAddedImportHistory = parameter.Id < 0;
			if (parameter.Id >= 0)
			{
				parameter.IsDescriptionChangedHistory = HistoryGeneralHelper.CheckAreValuesDiffrentAndFirstValueIsNullForImport(parameterObject?.Description, parameter.Description);
				if (parameterObject != null)
				{
					CustomFieldContainer customFieldContainer = new CustomFieldContainer(SharedObjectTypeEnum.ObjectType.Parameter, parameter.Id, customFieldsSupport);
					customFieldContainer.RetrieveCustomFields(parameterObject);
					customFieldContainer.ClearAddedDefinitionValues(null);
					HistoryCustomFieldsHelper.InsertHistoryCustomFieldsWhenDBIsNotAddedOnImportChanges(parameter.Id, parameter.CustomFields, databaseId, SharedObjectTypeEnum.ObjectType.Parameter, customFieldContainer);
				}
			}
		}
	}

	internal static void InsertHistoryRowsInParameterDB(IEnumerable<ParameterRow> parameters, int databaseId, int procedureId, bool isDbAdded)
	{
		if (!DB.History.SavingEnabled || procedureId < 0)
		{
			return;
		}
		List<ParameterObject> dataByProcedureId = DB.Parameter.GetDataByProcedureId(procedureId);
		List<HistoryModel> historyModelListToBulkInsert = new List<HistoryModel>();
		foreach (ParameterRow parameter in parameters)
		{
			if (parameter.Id < 1)
			{
				ParameterObject obj = dataByProcedureId?.Where((ParameterObject x) => x.Name == parameter.Name).FirstOrDefault();
				parameter.Id = obj?.ParameterId ?? (-1);
			}
			if (parameter.Id < 0)
			{
				continue;
			}
			if (isDbAdded)
			{
				HistoryCustomFieldsHelper.InsertCustomFieldsObjectWhenDBIsAddedOnImportChanges(parameter.Id, parameter.CustomFields, databaseId, SharedObjectTypeEnum.ObjectType.Parameter);
				parameter.IsDescriptionChangedHistory = !string.IsNullOrEmpty(parameter.Description);
				parameter.IsTitleChangedHistory = !string.IsNullOrEmpty(parameter.Title);
				HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, parameter.Id, parameter.Title, parameter.Description, null, parameter.IsTitleChangedHistory, parameter.IsDescriptionChangedHistory, saveCustomfield: false, SharedObjectTypeEnum.ObjectType.Parameter, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(SharedObjectTypeEnum.ObjectType.Parameter), null);
				continue;
			}
			if (parameter.ObjectIsAddedImportHistory)
			{
				HistoryCustomFieldsHelper.InsertCustomFieldsObjectWhenDBIsAddedOnImportChanges(parameter.Id, parameter.CustomFields, databaseId, SharedObjectTypeEnum.ObjectType.Parameter);
				parameter.IsDescriptionChangedHistory = !string.IsNullOrEmpty(parameter.Description);
				parameter.IsTitleChangedHistory = !string.IsNullOrEmpty(parameter.Title);
			}
			HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, parameter.Id, parameter.Title, parameter.Description, null, parameter.IsTitleChangedHistory, parameter.IsDescriptionChangedHistory, saveCustomfield: false, SharedObjectTypeEnum.ObjectType.Parameter, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(SharedObjectTypeEnum.ObjectType.Parameter), null);
		}
		HistoryBulkInserterHelper.BulkInsertHistoryModels(historyModelListToBulkInsert);
	}

	internal static void CheckTriggerChangesInTriggerDB(IEnumerable<TriggerRow> triggers, int databaseId, int tableId, bool isDbAdded, CustomFieldsSupport customFieldsSupport)
	{
		if (!DB.History.SavingEnabled || isDbAdded || tableId < 0)
		{
			return;
		}
		List<TriggerObject> dataByTable = DB.Trigger.GetDataByTable(tableId);
		foreach (TriggerRow trigger in triggers)
		{
			TriggerObject triggerObject = dataByTable?.Where((TriggerObject x) => x.Name == trigger.Name).FirstOrDefault();
			trigger.Id = triggerObject?.TriggerId ?? (-1);
			trigger.ObjectIsAddedImportHistory = trigger.Id < 0;
			if (trigger.Id >= 0)
			{
				trigger.IsDescriptionChangedHistory = HistoryGeneralHelper.CheckAreValuesDiffrentAndFirstValueIsNullForImport(triggerObject?.Description, trigger.Description);
				if (triggerObject != null)
				{
					CustomFieldContainer customFieldContainer = new CustomFieldContainer(trigger.ObjectType, trigger.Id, customFieldsSupport);
					customFieldContainer.RetrieveCustomFields(triggerObject);
					customFieldContainer.ClearAddedDefinitionValues(null);
					HistoryCustomFieldsHelper.InsertHistoryCustomFieldsWhenDBIsNotAddedOnImportChanges(trigger.Id, trigger.CustomFields, databaseId, trigger.ObjectType, customFieldContainer);
				}
			}
		}
	}

	internal static void InsertHistoryRowsInTriggerDB(IEnumerable<TriggerRow> triggers, int databaseId, int tableId, bool isDbAdded)
	{
		if (!DB.History.SavingEnabled || tableId < 0)
		{
			return;
		}
		List<TriggerObject> dataByTable = DB.Trigger.GetDataByTable(tableId);
		List<HistoryModel> historyModelListToBulkInsert = new List<HistoryModel>();
		foreach (TriggerRow trigger in triggers)
		{
			if (trigger.Id < 1)
			{
				TriggerObject triggerObject = dataByTable.Where((TriggerObject x) => x.Name == trigger.Name).FirstOrDefault();
				trigger.Id = triggerObject?.TriggerId ?? (-1);
			}
			if (trigger.Id < 0)
			{
				continue;
			}
			if (isDbAdded)
			{
				HistoryCustomFieldsHelper.InsertCustomFieldsObjectWhenDBIsAddedOnImportChanges(trigger.Id, trigger.CustomFields, databaseId, trigger.ObjectType);
				trigger.IsDescriptionChangedHistory = !string.IsNullOrEmpty(trigger.Description);
				trigger.IsTitleChangedHistory = !string.IsNullOrEmpty(trigger.Title);
				HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, trigger.Id, trigger.Title, trigger.Description, null, trigger.IsTitleChangedHistory, trigger.IsDescriptionChangedHistory, saveCustomfield: false, trigger.ObjectType, "triggers", null);
				continue;
			}
			if (trigger.ObjectIsAddedImportHistory)
			{
				HistoryCustomFieldsHelper.InsertCustomFieldsObjectWhenDBIsAddedOnImportChanges(trigger.Id, trigger.CustomFields, databaseId, trigger.ObjectType);
				trigger.IsDescriptionChangedHistory = !string.IsNullOrEmpty(trigger.Description);
				trigger.IsTitleChangedHistory = !string.IsNullOrEmpty(trigger.Title);
			}
			HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, trigger.Id, trigger.Title, trigger.Description, null, trigger.IsTitleChangedHistory, trigger.IsDescriptionChangedHistory, saveCustomfield: false, trigger.ObjectType, "triggers", null);
		}
		HistoryBulkInserterHelper.BulkInsertHistoryModels(historyModelListToBulkInsert);
	}
}
