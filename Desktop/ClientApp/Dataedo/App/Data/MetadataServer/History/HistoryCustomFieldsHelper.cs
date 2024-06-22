using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.ImportDescriptions.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.SummaryControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Common.Interfaces;
using Dataedo.Model.Data.History;
using Dataedo.Shared.Enums;
using DevExpress.XtraGrid.Columns;

namespace Dataedo.App.Data.MetadataServer.History;

internal static class HistoryCustomFieldsHelper
{
	internal static void SaveCustomFieldsOnImportDescription(int databaseId, TableImportDataModel x)
	{
		if (!DB.History.SavingEnabled || x == null || databaseId == -1 || x == null || !x.TableId.HasValue)
		{
			return;
		}
		List<HistoryModel> historyModelListToBulkInsert = new List<HistoryModel>();
		foreach (FieldData item in x?.Fields?.Where((FieldData f) => f != null && f.IsSelected && f.CurrentValue != f.OverwriteValue))
		{
			HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, x?.TableId.Value, null, item?.OverwriteValue, null, saveTitle: false, saveDescription: false, saveCustomfield: true, SharedObjectTypeEnum.ObjectType.Table, "tables", $"field{Array.IndexOf(x?.Fields, item) + 1}");
		}
		HistoryBulkInserterHelper.BulkInsertHistoryModels(historyModelListToBulkInsert);
	}

	internal static void InsertHistoryCustomFieldsOnTablePanel(int? tableRowId, Dictionary<string, string> customFieldsForHistory, CustomFieldContainer customFieldContainer, int? databaseId, SharedObjectTypeEnum.ObjectType? objectType)
	{
		if (!DB.History.SavingEnabled)
		{
			return;
		}
		List<HistoryModel> historyModelListToBulkInsert = new List<HistoryModel>();
		foreach (KeyValuePair<string, string> customField in customFieldsForHistory)
		{
			CustomFieldDefinition customFieldDefinition = customFieldContainer?.CustomFieldsData.Where((CustomFieldDefinition d) => customField.Key == d?.CustomField?.FieldName && customField.Value != d?.FieldValue)?.FirstOrDefault();
			if (customFieldDefinition != null)
			{
				HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, tableRowId, null, customFieldDefinition?.FieldValue, null, saveTitle: false, saveDescription: false, saveCustomfield: true, objectType, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(objectType), customFieldDefinition?.CustomField?.FieldName);
			}
		}
		HistoryBulkInserterHelper.BulkInsertHistoryModels(historyModelListToBulkInsert);
	}

	internal static void InsertHistoryCustomFieldsOnTableSummary(int id, int databaseId, Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> customFields, Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> customFieldCopyForHistory, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (!DB.History.SavingEnabled || customFields == null || customFieldCopyForHistory == null)
		{
			return;
		}
		List<HistoryModel> historyModelListToBulkInsert = new List<HistoryModel>();
		foreach (KeyValuePair<string, BaseWithCustomFields.CustomFieldWithValue> customFieldss in customFieldCopyForHistory)
		{
			KeyValuePair<string, BaseWithCustomFields.CustomFieldWithValue>? keyValuePair = customFields?.Where((KeyValuePair<string, BaseWithCustomFields.CustomFieldWithValue> d) => customFieldss.Key == d.Key && customFieldss.Value?.Value != d.Value?.Value)?.FirstOrDefault();
			if (keyValuePair.HasValue && keyValuePair.Value.Key != null)
			{
				HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, id, null, keyValuePair.Value.Value?.Value, null, saveTitle: false, saveDescription: false, saveCustomfield: true, objectType, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(objectType), keyValuePair.Value.Value.CustomField?.FieldName);
			}
		}
		HistoryBulkInserterHelper.BulkInsertHistoryModels(historyModelListToBulkInsert);
	}

	internal static void InsertHistoryCustomFieldsWhenDBIsNotAddedOnImportChanges(int? id, CustomFieldContainer tableCustomFields, int? databaseId, SharedObjectTypeEnum.ObjectType? objectType, CustomFieldContainer customFields)
	{
		if (!DB.History.SavingEnabled || tableCustomFields == null || customFields == null || customFields?.CustomFieldsData == null)
		{
			return;
		}
		List<HistoryModel> historyModelListToBulkInsert = new List<HistoryModel>();
		CustomFieldDefinition[] array = customFields?.CustomFieldsData;
		foreach (CustomFieldDefinition customField in array)
		{
			CustomFieldDefinition customFieldDefinition = tableCustomFields?.CustomFieldsData.Where((CustomFieldDefinition d) => customField.FieldValue != d.FieldValue && string.IsNullOrEmpty(customField.FieldValue) && customField.CustomField.FieldName == d?.CustomField.FieldName && (!string.IsNullOrEmpty(customField.FieldValue) || !string.IsNullOrEmpty(d.FieldValue)))?.FirstOrDefault();
			if (customFieldDefinition != null)
			{
				HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, id, null, customFieldDefinition.FieldValue, null, saveTitle: false, saveDescription: false, saveCustomfield: true, objectType, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(objectType), customFieldDefinition.CustomField.FieldName);
			}
		}
		HistoryBulkInserterHelper.BulkInsertHistoryModels(historyModelListToBulkInsert);
	}

	internal static void InsertCustomFieldsObjectWhenDBIsAddedOnImportChanges(int? id, CustomFieldContainer objectCustomFieldContainer, int databaseId, SharedObjectTypeEnum.ObjectType? objectType)
	{
		if (DB.History.SavingEnabled && objectCustomFieldContainer?.CustomFieldsData != null && objectCustomFieldContainer != null)
		{
			List<HistoryModel> historyModelListToBulkInsert = new List<HistoryModel>();
			CustomFieldDefinition[] array = objectCustomFieldContainer?.CustomFieldsData;
			foreach (CustomFieldDefinition customFieldDefinition in array)
			{
				HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, id, null, customFieldDefinition.FieldValue, null, saveTitle: false, saveDescription: false, !string.IsNullOrEmpty(customFieldDefinition.FieldValue), objectType, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(objectType), customFieldDefinition.CustomField.FieldName);
			}
			HistoryBulkInserterHelper.BulkInsertHistoryModels(historyModelListToBulkInsert);
		}
	}

	internal static void InsertHistoryCustomFieldsOnColumnSummary(int id, int? databaseId, CustomFieldDefinition[] newCustomFields, Dictionary<string, string> oldCustomFields, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (!DB.History.SavingEnabled || newCustomFields == null || oldCustomFields == null || !databaseId.HasValue || databaseId < 0)
		{
			return;
		}
		List<HistoryModel> historyModelListToBulkInsert = new List<HistoryModel>();
		foreach (KeyValuePair<string, string> oldCustomField in oldCustomFields)
		{
			CustomFieldDefinition customFieldDefinition = newCustomFields?.Where((CustomFieldDefinition d) => oldCustomField.Key == d?.CustomField?.FieldName && oldCustomField.Value != d.FieldValue)?.FirstOrDefault();
			if (customFieldDefinition != null && customFieldDefinition.CustomField.FieldName != null)
			{
				HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, id, null, customFieldDefinition.FieldValue, null, saveTitle: false, saveDescription: false, saveCustomfield: true, objectType, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(objectType), customFieldDefinition.CustomField?.FieldName);
			}
		}
		HistoryBulkInserterHelper.BulkInsertHistoryModels(historyModelListToBulkInsert);
	}

	internal static void InsertHistoryCustomFieldsOnTermSummary(BulkCopyGridUserControl gridView, BaseSummaryUserControl baseSummaryUserControl, TermObject termObject, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (DB.History.SavingEnabled && termObject != null && gridView != null && termObject.DatabaseId.HasValue && !(termObject.DatabaseId < 0) && termObject.TermId.HasValue && !(termObject.TermId < 0))
		{
			Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> dictionary = gridView.Columns.Where((GridColumn x) => x.FieldName.Contains("Field")).ToDictionary((GridColumn x) => x.FieldName, (GridColumn y) => BaseCustomFieldDB.GetCustomFieldWithValue(baseSummaryUserControl.CustomFieldsSupport.GetField(y.FieldName), termObject.GetField(y.FieldName)?.ToString()));
			Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> value = null;
			CommonFunctionsPanels.customFieldsForHistory.TryGetValue(termObject.TermId.Value, out value);
			if (value != null)
			{
				InsertHistoryCustomFieldsOnTableSummary(termObject.TermId.Value, termObject.DatabaseId.Value, dictionary, value, objectType);
				CommonFunctionsPanels.customFieldsForHistory[termObject.TermId.Value] = dictionary;
			}
		}
	}

	internal static void PrepareCustomFieldsForImport(ObjectRow objectRow, int databaseId, CustomFieldsSupport customFieldsSupport, ICustomFieldsDataAccess customFieldsDataAccess)
	{
		if (objectRow != null && customFieldsSupport != null && customFieldsDataAccess != null)
		{
			CustomFieldContainer customFieldContainer = new CustomFieldContainer(objectRow.ObjectTypeValue.Value, objectRow.ObjectId, customFieldsSupport);
			customFieldContainer.RetrieveCustomFields(customFieldsDataAccess);
			customFieldContainer.ClearAddedDefinitionValues(null);
			InsertHistoryCustomFieldsWhenDBIsNotAddedOnImportChanges(objectRow.ObjectId, objectRow.CustomFields, databaseId, objectRow.ObjectTypeValue, customFieldContainer);
		}
	}

	internal static Dictionary<string, string> GetOldCustomFieldsInObjectUserControl(CustomFieldContainer customFields)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (customFields == null || customFields.CustomFieldsData == null)
		{
			return dictionary;
		}
		CustomFieldDefinition[] customFieldsData = customFields.CustomFieldsData;
		foreach (CustomFieldDefinition customFieldDefinition in customFieldsData)
		{
			if (customFieldDefinition.CustomField != null && customFieldDefinition.CustomField.FieldName != null)
			{
				dictionary.Add(customFieldDefinition.CustomField.FieldName, customFieldDefinition.FieldValue);
			}
		}
		return dictionary;
	}
}
