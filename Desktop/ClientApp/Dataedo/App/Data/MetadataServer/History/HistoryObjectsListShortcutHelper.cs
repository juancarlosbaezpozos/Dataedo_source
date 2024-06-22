using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes;
using Dataedo.App.UserControls.SummaryControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.Modules;
using Dataedo.Shared.Enums;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Data.MetadataServer.History;

internal static class HistoryObjectsListShortcutHelper
{
	internal static void InsertHistoryCustomFieldsOnTermSummary(GridView gridView, BaseSummaryUserControl baseSummaryUserControl, int? row, Dictionary<string, object> keyValuePairs, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (!DB.History.SavingEnabled || baseSummaryUserControl == null || gridView == null)
		{
			return;
		}
		if (row.HasValue)
		{
			TermObject termObject = gridView.GetRow(row.Value) as TermObject;
			SaveShortCutTerms(gridView, baseSummaryUserControl, objectType, termObject, keyValuePairs);
			return;
		}
		int[] selectedRows = gridView.GetSelectedRows();
		foreach (int rowHandle in selectedRows)
		{
			if (gridView.GetRow(rowHandle) is TermObject termObject2 && !(termObject2.DatabaseId < 0) && termObject2.DatabaseId.HasValue && !(termObject2.TermId <= 0) && termObject2.TermId.HasValue)
			{
				SaveShortCutTerms(gridView, baseSummaryUserControl, objectType, termObject2, keyValuePairs);
			}
		}
	}

	internal static void InsertHistoryCustomFieldsOnTableAndProceduresSummary(GridView gridView, BaseSummaryUserControl baseSummaryUserControl, int? row, Dictionary<string, object> keyValuePairs, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (!DB.History.SavingEnabled || baseSummaryUserControl == null || gridView == null)
		{
			return;
		}
		if (row.HasValue)
		{
			ObjectWithModulesObject objectWithModulesObject = gridView.GetRow(row.Value) as ObjectWithModulesObject;
			SaveShortCutTables(gridView, baseSummaryUserControl, objectType, objectWithModulesObject, keyValuePairs);
			return;
		}
		int[] selectedRows = gridView.GetSelectedRows();
		foreach (int rowHandle in selectedRows)
		{
			if (gridView.GetRow(rowHandle) is ObjectWithModulesObject objectWithModulesObject2 && objectWithModulesObject2.DatabaseId >= 0)
			{
				_ = objectWithModulesObject2.DatabaseId;
				if (objectWithModulesObject2.Id > 0)
				{
					_ = objectWithModulesObject2.Id;
					SaveShortCutTables(gridView, baseSummaryUserControl, objectType, objectWithModulesObject2, keyValuePairs);
				}
			}
		}
	}

	internal static void InsertHistoryCustomFieldsOnModuleSummary(GridView gridView, BaseSummaryUserControl baseSummaryUserControl, int? row, Dictionary<string, object> keyValuePairs, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (!DB.History.SavingEnabled || baseSummaryUserControl == null || gridView == null)
		{
			return;
		}
		if (row.HasValue)
		{
			ModuleWithoutDescriptionObject moduleWithoutDescriptionObject = gridView.GetRow(row.Value) as ModuleWithoutDescriptionObject;
			SaveShortCutModules(gridView, baseSummaryUserControl, objectType, moduleWithoutDescriptionObject, keyValuePairs);
			return;
		}
		int[] selectedRows = gridView.GetSelectedRows();
		foreach (int rowHandle in selectedRows)
		{
			if (gridView.GetRow(rowHandle) is ModuleWithoutDescriptionObject moduleWithoutDescriptionObject2 && moduleWithoutDescriptionObject2.DatabaseId >= 0)
			{
				_ = moduleWithoutDescriptionObject2.DatabaseId;
				if (moduleWithoutDescriptionObject2.Id > 0)
				{
					_ = moduleWithoutDescriptionObject2.Id;
					SaveShortCutModules(gridView, baseSummaryUserControl, objectType, moduleWithoutDescriptionObject2, keyValuePairs);
				}
			}
		}
	}

	private static void SaveShortCutModules(GridView gridView, BaseSummaryUserControl baseSummaryUserControl, SharedObjectTypeEnum.ObjectType objectType, ModuleWithoutDescriptionObject moduleWithoutDescriptionObject, Dictionary<string, object> keyValuePairs)
	{
		foreach (KeyValuePair<string, object> keyValuePair in keyValuePairs)
		{
			_ = keyValuePair;
			if (keyValuePairs.ContainsKey("title") && CommonFunctionsPanels.summaryObjectTitleHistory.ContainsKey(moduleWithoutDescriptionObject.Id))
			{
				moduleWithoutDescriptionObject.Title = keyValuePairs["title"].ToString();
				DB.History.InsertHistoryRow(moduleWithoutDescriptionObject.DatabaseId, moduleWithoutDescriptionObject.Id, moduleWithoutDescriptionObject?.Title, null, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(objectType), HistoryGeneralHelper.CheckAreValuesDiffrent(CommonFunctionsPanels.summaryObjectTitleHistory[moduleWithoutDescriptionObject.Id], moduleWithoutDescriptionObject.Title), saveDescription: false, objectType);
				if (CommonFunctionsPanels.summaryObjectTitleHistory.ContainsKey(moduleWithoutDescriptionObject.Id))
				{
					CommonFunctionsPanels.summaryObjectTitleHistory[moduleWithoutDescriptionObject.Id] = moduleWithoutDescriptionObject.Title;
				}
			}
		}
		Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> dictionary = gridView.Columns.Where((GridColumn x) => x.FieldName.Contains("Field")).ToDictionary((GridColumn x) => x.FieldName, (GridColumn y) => BaseCustomFieldDB.GetCustomFieldWithValue(baseSummaryUserControl.CustomFieldsSupport.GetField(y.FieldName), moduleWithoutDescriptionObject.GetField(y.FieldName)?.ToString()));
		Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> value = null;
		CommonFunctionsPanels.customFieldsForHistory.TryGetValue(moduleWithoutDescriptionObject.Id, out value);
		if (value == null)
		{
			return;
		}
		foreach (KeyValuePair<string, object> keyValuePair2 in keyValuePairs)
		{
			string text = HistoryGeneralHelper.UppercaseFirst(keyValuePair2.Key);
			if (text == null)
			{
				return;
			}
			if (dictionary.ContainsKey(text))
			{
				dictionary[text].Value = keyValuePair2.Value?.ToString();
			}
		}
		HistoryCustomFieldsHelper.InsertHistoryCustomFieldsOnTableSummary(moduleWithoutDescriptionObject.Id, moduleWithoutDescriptionObject.DatabaseId, dictionary, value, objectType);
		DB.History.InsertHistoryRow(moduleWithoutDescriptionObject.DatabaseId, moduleWithoutDescriptionObject.Id, moduleWithoutDescriptionObject?.Title, null, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(objectType), HistoryGeneralHelper.CheckAreValuesDiffrent(CommonFunctionsPanels.summaryObjectTitleHistory[moduleWithoutDescriptionObject.Id], moduleWithoutDescriptionObject.Title), saveDescription: false, objectType);
		if (CommonFunctionsPanels.summaryObjectTitleHistory.ContainsKey(moduleWithoutDescriptionObject.Id))
		{
			CommonFunctionsPanels.summaryObjectTitleHistory[moduleWithoutDescriptionObject.Id] = moduleWithoutDescriptionObject.Title;
		}
		if (CommonFunctionsPanels.customFieldsForHistory.ContainsKey(moduleWithoutDescriptionObject.Id))
		{
			CommonFunctionsPanels.customFieldsForHistory[moduleWithoutDescriptionObject.Id] = dictionary;
		}
	}

	private static void SaveShortCutTerms(GridView gridView, BaseSummaryUserControl baseSummaryUserControl, SharedObjectTypeEnum.ObjectType objectType, TermObject termObject, Dictionary<string, object> keyValuePairs)
	{
		Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> dictionary = gridView.Columns.Where((GridColumn x) => x.FieldName.Contains("Field")).ToDictionary((GridColumn x) => x.FieldName, (GridColumn y) => BaseCustomFieldDB.GetCustomFieldWithValue(baseSummaryUserControl.CustomFieldsSupport.GetField(y.FieldName), termObject.GetField(y.FieldName)?.ToString()));
		Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> value = null;
		CommonFunctionsPanels.customFieldsForHistory.TryGetValue(termObject.TermId.Value, out value);
		if (value == null)
		{
			return;
		}
		foreach (KeyValuePair<string, object> keyValuePair in keyValuePairs)
		{
			string text = HistoryGeneralHelper.UppercaseFirst(keyValuePair.Key);
			if (text == null)
			{
				return;
			}
			if (dictionary.ContainsKey(text))
			{
				dictionary[text].Value = keyValuePair.Value?.ToString();
			}
		}
		HistoryCustomFieldsHelper.InsertHistoryCustomFieldsOnTableSummary(termObject.TermId.Value, termObject.DatabaseId.Value, dictionary, value, objectType);
		if (CommonFunctionsPanels.customFieldsForHistory.ContainsKey(termObject.TermId.Value))
		{
			CommonFunctionsPanels.customFieldsForHistory[termObject.TermId.Value] = dictionary;
		}
	}

	private static void SaveShortCutTables(GridView gridView, BaseSummaryUserControl baseSummaryUserControl, SharedObjectTypeEnum.ObjectType objectType, ObjectWithModulesObject objectWithModulesObject, Dictionary<string, object> keyValuePairs)
	{
		foreach (KeyValuePair<string, object> keyValuePair in keyValuePairs)
		{
			_ = keyValuePair;
			if (keyValuePairs.ContainsKey("title") && CommonFunctionsPanels.summaryObjectTitleHistory.ContainsKey(objectWithModulesObject.Id))
			{
				objectWithModulesObject.Title = keyValuePairs["title"].ToString();
				DB.History.InsertHistoryRow(objectWithModulesObject.DatabaseId, objectWithModulesObject.Id, objectWithModulesObject?.Title, null, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(objectType), HistoryGeneralHelper.CheckAreValuesDiffrent(CommonFunctionsPanels.summaryObjectTitleHistory[objectWithModulesObject.Id], objectWithModulesObject.Title), saveDescription: false, objectType);
				if (CommonFunctionsPanels.summaryObjectTitleHistory.ContainsKey(objectWithModulesObject.Id))
				{
					CommonFunctionsPanels.summaryObjectTitleHistory[objectWithModulesObject.Id] = objectWithModulesObject.Title;
				}
			}
		}
		Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> dictionary = gridView.Columns.Where((GridColumn x) => x.FieldName.Contains("Field")).ToDictionary((GridColumn x) => x.FieldName, (GridColumn y) => BaseCustomFieldDB.GetCustomFieldWithValue(baseSummaryUserControl.CustomFieldsSupport.GetField(y.FieldName), objectWithModulesObject.GetField(y.FieldName)?.ToString()));
		Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> value = null;
		CommonFunctionsPanels.customFieldsForHistory.TryGetValue(objectWithModulesObject.Id, out value);
		if (value == null)
		{
			return;
		}
		foreach (KeyValuePair<string, object> keyValuePair2 in keyValuePairs)
		{
			string text = HistoryGeneralHelper.UppercaseFirst(keyValuePair2.Key);
			if (text == null)
			{
				return;
			}
			if (dictionary.ContainsKey(text))
			{
				dictionary[text].Value = keyValuePair2.Value?.ToString();
			}
		}
		HistoryCustomFieldsHelper.InsertHistoryCustomFieldsOnTableSummary(objectWithModulesObject.Id, objectWithModulesObject.DatabaseId, dictionary, value, objectType);
		DB.History.InsertHistoryRow(objectWithModulesObject.DatabaseId, objectWithModulesObject.Id, objectWithModulesObject?.Title, null, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(objectType), HistoryGeneralHelper.CheckAreValuesDiffrent(CommonFunctionsPanels.summaryObjectTitleHistory[objectWithModulesObject.Id], objectWithModulesObject.Title), saveDescription: false, objectType);
		if (CommonFunctionsPanels.customFieldsForHistory.ContainsKey(objectWithModulesObject.Id))
		{
			CommonFunctionsPanels.customFieldsForHistory[objectWithModulesObject.Id] = dictionary;
		}
		if (CommonFunctionsPanels.summaryObjectTitleHistory.ContainsKey(objectWithModulesObject.Id))
		{
			CommonFunctionsPanels.summaryObjectTitleHistory[objectWithModulesObject.Id] = objectWithModulesObject.Title;
		}
	}
}
