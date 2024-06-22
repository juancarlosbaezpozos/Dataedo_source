using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.SummaryControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Modules;
using Dataedo.Shared.Enums;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Data.MetadataServer.History;

internal class HistoryModulesHelper
{
	internal static void InsertHistoryCustomFieldsOnModuleSummary(BulkCopyGridUserControl gridView, BaseSummaryUserControl baseSummaryUserControl, ModuleWithoutDescriptionObject moduleWithoutDescriptionObject, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (!DB.History.SavingEnabled || moduleWithoutDescriptionObject == null || gridView == null || baseSummaryUserControl == null)
		{
			return;
		}
		_ = moduleWithoutDescriptionObject.DatabaseId;
		if (moduleWithoutDescriptionObject.DatabaseId < 0)
		{
			return;
		}
		_ = moduleWithoutDescriptionObject.ModuleId;
		if (moduleWithoutDescriptionObject.ModuleId < 0)
		{
			return;
		}
		IEnumerable<GridColumn> enumerable = gridView?.Columns?.Where((GridColumn x) => x.FieldName.Contains("Field"));
		if (enumerable != null)
		{
			Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> dictionary = enumerable.ToDictionary((GridColumn x) => x.FieldName, (GridColumn y) => BaseCustomFieldDB.GetCustomFieldWithValue(baseSummaryUserControl.CustomFieldsSupport.GetField(y.FieldName), moduleWithoutDescriptionObject.GetField(y.FieldName)?.ToString()));
			Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> value = null;
			CommonFunctionsPanels.customFieldsForHistory.TryGetValue(moduleWithoutDescriptionObject.ModuleId, out value);
			if (value != null)
			{
				HistoryCustomFieldsHelper.InsertHistoryCustomFieldsOnTableSummary(moduleWithoutDescriptionObject.ModuleId, moduleWithoutDescriptionObject.DatabaseId, dictionary, value, objectType);
				CommonFunctionsPanels.customFieldsForHistory[moduleWithoutDescriptionObject.ModuleId] = dictionary;
			}
		}
	}

	internal static void InsertHistoryTitleOnModuleSummary(BulkCopyGridUserControl gridView, ModuleWithoutDescriptionObject moduleWithoutDescriptionObject, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (!DB.History.SavingEnabled || moduleWithoutDescriptionObject == null || gridView == null)
		{
			return;
		}
		_ = moduleWithoutDescriptionObject.DatabaseId;
		if (moduleWithoutDescriptionObject.DatabaseId >= 0)
		{
			_ = moduleWithoutDescriptionObject.ModuleId;
			if (moduleWithoutDescriptionObject.ModuleId >= 0 && CommonFunctionsPanels.summaryObjectTitleHistory.TryGetValue(moduleWithoutDescriptionObject.ModuleId, out var value))
			{
				bool saveTitle = HistoryGeneralHelper.CheckAreValuesDiffrent(value, moduleWithoutDescriptionObject.Title);
				DB.History.InsertHistoryRow(moduleWithoutDescriptionObject.DatabaseId, moduleWithoutDescriptionObject.ModuleId, moduleWithoutDescriptionObject.Title, null, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(objectType), saveTitle, saveDescription: false, objectType);
				CommonFunctionsPanels.summaryObjectTitleHistory[moduleWithoutDescriptionObject.ModuleId] = moduleWithoutDescriptionObject.Title;
			}
		}
	}

	internal static void SetHistoryOfNewModuleAddedToSummary(GridView gridView, CustomFieldsSupport customFieldsSupport, string newModuleName, int newModuleID, ModuleWithoutDescriptionObject newModule)
	{
		SaveOldTitleHistory(newModuleName, newModuleID);
		SaveOldCustomFieldsHistory(gridView, customFieldsSupport, newModuleID);
	}

	private static void SaveOldCustomFieldsHistory(GridView gridView, CustomFieldsSupport customFieldsSupport, int newModuleID)
	{
		if (CommonFunctionsPanels.customFieldsForHistory == null || CommonFunctionsPanels.customFieldsForHistory.ContainsKey(newModuleID) || gridView == null || gridView?.Columns == null)
		{
			return;
		}
		ModuleObject moduleRow = DB.Module.GetDataById(newModuleID);
		if (moduleRow == null)
		{
			return;
		}
		IEnumerable<GridColumn> enumerable = gridView?.Columns?.Where((GridColumn x) => x.FieldName.Contains("Field"));
		if (enumerable != null && (enumerable == null || enumerable.Count() != 0))
		{
			CommonFunctionsPanels.customFieldsForHistory.Add(newModuleID, enumerable.ToDictionary((GridColumn x) => x.FieldName, (GridColumn y) => BaseCustomFieldDB.GetCustomFieldWithValue(customFieldsSupport.GetField(y.FieldName), moduleRow.GetField(y.FieldName)?.ToString())));
		}
	}

	private static void SaveOldTitleHistory(string newModuleName, int newModuleID)
	{
		if (CommonFunctionsPanels.summaryObjectTitleHistory != null && !CommonFunctionsPanels.summaryObjectTitleHistory.ContainsKey(newModuleID))
		{
			CommonFunctionsPanels.summaryObjectTitleHistory.Add(newModuleID, newModuleName);
		}
	}
}
