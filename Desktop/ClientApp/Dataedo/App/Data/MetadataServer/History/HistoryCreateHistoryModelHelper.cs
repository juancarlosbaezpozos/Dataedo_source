using System.Collections.Generic;
using Dataedo.Model.Data.History;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer.History;

internal static class HistoryCreateHistoryModelHelper
{
	internal static void ReturnCreatedHistoryModels(int? databaseId, List<HistoryModel> historyModelListToBulkInsert, int? objectId, string objectTitle, string objectValue, string objectValuePlain, bool saveTitle, bool saveDescription, bool saveCustomfield, SharedObjectTypeEnum.ObjectType? objectType, string objectTable, string customFieldName)
	{
		HistoryModel historyModel = ReturnHistoryModel(databaseId, objectId, objectTitle, null, objectTable, "title", saveTitle, objectType);
		HistoryModel historyModel2 = ReturnHistoryModel(databaseId, objectId, objectValue, objectValuePlain, objectTable, "description", saveDescription, objectType);
		HistoryModel historyModel3 = ReturnHistoryModel(databaseId, objectId, objectValue, null, objectTable, customFieldName, saveCustomfield, objectType);
		InsertHistoryModelToListOrRepo(historyModelListToBulkInsert, historyModel);
		InsertHistoryModelToListOrRepo(historyModelListToBulkInsert, historyModel2);
		InsertHistoryModelToListOrRepo(historyModelListToBulkInsert, historyModel3);
	}

	internal static void InsertHistoryModelToListOrRepo(List<HistoryModel> historyModelListToBulkInsert, HistoryModel historyModel)
	{
		if (historyModel != null)
		{
			if (Dataedo.App.StaticData.IsProjectFile)
			{
				DB.History.InsertHistoryModel(historyModel);
			}
			else
			{
				historyModelListToBulkInsert.Add(historyModel);
			}
		}
	}

	private static HistoryModel ReturnHistoryModel(int? databaseId, int? objectId, string value, string valuePlain, string objectTable, string objectColumn, bool saveToHistory, SharedObjectTypeEnum.ObjectType? objectType)
	{
		if (HistoryGeneralHelper.DontSaveHistory(databaseId, objectId, saveToHistory, objectType))
		{
			return null;
		}
		int? userId = (Dataedo.App.StaticData.IsProjectFile ? null : new int?(Dataedo.App.StaticData.CurrentLicenseId));
		return new HistoryModel(databaseId.Value, objectTable, objectColumn.ToLower(), objectId.Value, value, valuePlain, userId, "DESKTOP", Dataedo.App.StaticData.ProductVersion);
	}
}
