using System.Collections.Generic;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.History;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer.History;

internal static class HistoryColumnsHelper
{
	internal static void InsertHistoryRowsInDataLakeImportProcessor(ObjectModel objectModel, List<ColumnRow> columnsToInsert, int databaseId)
	{
		if (!DB.History.SavingEnabled)
		{
			return;
		}
		List<HistoryModel> historyModelListToBulkInsert = new List<HistoryModel>();
		foreach (ColumnRow item in columnsToInsert)
		{
			HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, item.Id, item.Title, item.Description, null, item.IsTitleChangedHistory, item.IsDescriptionChangedHistory, saveCustomfield: false, objectModel?.ObjectType, "columns", null);
		}
		HistoryBulkInserterHelper.BulkInsertHistoryModels(historyModelListToBulkInsert);
	}

	internal static void InsertColumnsToHistory(ColumnRow[] columns, int? databaseId, SharedObjectTypeEnum.ObjectType? objectType)
	{
		if (DB.History.SavingEnabled)
		{
			List<HistoryModel> historyModelListToBulkInsert = new List<HistoryModel>();
			foreach (ColumnRow columnRow in columns)
			{
				databaseId = HistoryGeneralHelper.FindDatabaseId(databaseId, columnRow);
				HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, columnRow.Id, columnRow.Title, columnRow.Description, null, columnRow.IsTitleChangedHistory, columnRow.IsDescriptionChangedHistory, saveCustomfield: false, objectType, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(objectType), null);
				columnRow.IsTitleChangedHistory = false;
				columnRow.IsDescriptionChangedHistory = false;
			}
			HistoryBulkInserterHelper.BulkInsertHistoryModels(historyModelListToBulkInsert);
		}
	}

	internal static void CheckColumnsForHistory(ColumnRow column, SharedObjectTypeEnum.ObjectType? objectType)
	{
		if (DB.History.SavingEnabled && !HistoryGeneralHelper.IsNotAcceptedType(objectType))
		{
			ObjectWithTitleAndDescription objectWithTitleAndDescription = DB.History.SelectColumnTitleAndDescription(column.Id);
			column.IsDescriptionChangedHistory = HistoryGeneralHelper.CheckAreValuesDiffrent(objectWithTitleAndDescription?.Description, column.Description);
			column.IsTitleChangedHistory = HistoryGeneralHelper.CheckAreValuesDiffrent(objectWithTitleAndDescription?.Title, column.Title);
		}
	}

	public static void SaveTitleDescriptionCustomFieldsHistoryInUpdateColumns(Dictionary<int, Dictionary<string, string>> oldCustomFields, Dictionary<int, ObjectWithTitleAndDescription> titleAndDescriptionParametersForHistory, IEnumerable<BasicRow> basicRowsModifiedElements, SharedObjectTypeEnum.ObjectType objectType, int? databaseId = null)
	{
		List<HistoryModel> historyModelListToBulkInsert = new List<HistoryModel>();
		foreach (BasicRow basicRowsModifiedElement in basicRowsModifiedElements)
		{
			databaseId = HistoryGeneralHelper.FindDatabaseId(databaseId, basicRowsModifiedElement);
			if (!databaseId.HasValue || databaseId < 0 || basicRowsModifiedElement.Id < 0)
			{
				return;
			}
			if (titleAndDescriptionParametersForHistory.TryGetValue(basicRowsModifiedElement.Id, out var value))
			{
				if (basicRowsModifiedElement is ColumnRow columnRow)
				{
					basicRowsModifiedElement.Description = columnRow.Description;
				}
				else if (basicRowsModifiedElement is ParameterRow parameterRow)
				{
					basicRowsModifiedElement.Description = parameterRow.Description;
				}
				HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, basicRowsModifiedElement.Id, basicRowsModifiedElement.Title, basicRowsModifiedElement.Description, null, HistoryGeneralHelper.CheckAreValuesDiffrent(value.Title, basicRowsModifiedElement.Title), HistoryGeneralHelper.CheckAreValuesDiffrent(value.Description, basicRowsModifiedElement.Description), saveCustomfield: false, objectType, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(objectType), null);
				value.Title = basicRowsModifiedElement.Title;
				value.Description = basicRowsModifiedElement.Description;
			}
			oldCustomFields.TryGetValue(basicRowsModifiedElement.Id, out var value2);
			if (value2 == null || oldCustomFields == null || basicRowsModifiedElement.CustomFields == null)
			{
				return;
			}
			HistoryCustomFieldsHelper.InsertHistoryCustomFieldsOnColumnSummary(basicRowsModifiedElement.Id, databaseId, basicRowsModifiedElement.CustomFields.CustomFieldsData, value2, objectType);
			CustomFieldDefinition[] customFieldsData = basicRowsModifiedElement.CustomFields.CustomFieldsData;
			foreach (CustomFieldDefinition customFieldDefinition in customFieldsData)
			{
				if (customFieldDefinition != null && !string.IsNullOrEmpty(customFieldDefinition.CustomField.FieldName) && value2.ContainsKey(customFieldDefinition.CustomField.FieldName))
				{
					value2[customFieldDefinition.CustomField.FieldName] = customFieldDefinition.FieldValue;
				}
			}
		}
		HistoryBulkInserterHelper.BulkInsertHistoryModels(historyModelListToBulkInsert);
	}

	public static void SaveColumnsOrParametrsOfOldCustomFields(BasicRow[] elements, Dictionary<int, Dictionary<string, string>> customFieldsColumnForHistory, Dictionary<int, ObjectWithTitleAndDescription> titleAndDescriptionParametersForHistory)
	{
		if (elements == null || customFieldsColumnForHistory == null || titleAndDescriptionParametersForHistory == null)
		{
			return;
		}
		customFieldsColumnForHistory.Clear();
		titleAndDescriptionParametersForHistory.Clear();
		foreach (BasicRow basicRow in elements)
		{
			if (basicRow == null || basicRow.CustomFields == null || basicRow.CustomFields.CustomFieldsData == null)
			{
				continue;
			}
			if (basicRow is ColumnRow columnRow)
			{
				titleAndDescriptionParametersForHistory.Add(columnRow.Id, new ObjectWithTitleAndDescription
				{
					Title = columnRow.Title,
					Description = columnRow.Description
				});
			}
			else if (basicRow is ColumnRowSummary columnRowSummary)
			{
				titleAndDescriptionParametersForHistory.Add(columnRowSummary.Id, new ObjectWithTitleAndDescription
				{
					Title = columnRowSummary.Title,
					Description = columnRowSummary.Description
				});
			}
			else
			{
				titleAndDescriptionParametersForHistory.Add(basicRow.Id, new ObjectWithTitleAndDescription
				{
					Title = basicRow.Title,
					Description = basicRow.Description
				});
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			CustomFieldDefinition[] customFieldsData = basicRow.CustomFields.CustomFieldsData;
			foreach (CustomFieldDefinition customFieldDefinition in customFieldsData)
			{
				if (customFieldDefinition != null && customFieldDefinition.CustomField != null && customFieldDefinition.CustomField.FieldName != null)
				{
					dictionary.Add(customFieldDefinition.CustomField.FieldName, customFieldDefinition.FieldValue);
				}
			}
			customFieldsColumnForHistory.Add(basicRow.Id, dictionary);
		}
	}

	internal static HistoryModel[] GetClassificatorHistoryModels(ClassificatorUpdateModel classificatorUpdateModel)
	{
		List<HistoryModel> list = new List<HistoryModel>();
		if (classificatorUpdateModel == null || !DB.History.SavingEnabled)
		{
			return list.ToArray();
		}
		if (classificatorUpdateModel?.FieldsForSaving == null)
		{
			return list.ToArray();
		}
		foreach (CustomFieldModel item in classificatorUpdateModel?.FieldsForSaving)
		{
			HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(classificatorUpdateModel.DatabaseId, list, classificatorUpdateModel.ColumnId, null, item.FieldUpdateTo, null, saveTitle: false, saveDescription: false, saveCustomfield: true, SharedObjectTypeEnum.ObjectType.Column, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(SharedObjectTypeEnum.ObjectType.Column), item.FieldName);
		}
		return list.ToArray();
	}
}
