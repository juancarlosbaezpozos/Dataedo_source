using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Enums;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.UserControls.SummaryControls;
using Dataedo.Data.Base.Commands.Results.Base;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Common.Interfaces;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.Interfaces;
using Dataedo.Model.Data.Modules;
using Dataedo.Shared.Enums;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Tools;

public class BulkCopySummaryControlHelper
{
	public static void UpdateSummaryControl(CustomFieldsSupport customFieldsSupport, GridView grid, int rowHandle, IEnumerable<string> columnCaptions, Dictionary<string, object> keyValuePairs, SharedObjectTypeEnum.ObjectType objectType, BaseSummaryUserControl baseSummaryUserControl)
	{
		switch (objectType)
		{
		case SharedObjectTypeEnum.ObjectType.Function:
		case SharedObjectTypeEnum.ObjectType.Procedure:
		case SharedObjectTypeEnum.ObjectType.Table:
		case SharedObjectTypeEnum.ObjectType.View:
		case SharedObjectTypeEnum.ObjectType.Structure:
		case SharedObjectTypeEnum.ObjectType.Module:
		{
			switch (objectType)
			{
			case SharedObjectTypeEnum.ObjectType.Table:
			case SharedObjectTypeEnum.ObjectType.View:
			case SharedObjectTypeEnum.ObjectType.Structure:
				UpdateTable(grid, rowHandle, keyValuePairs);
				HistoryObjectsListShortcutHelper.InsertHistoryCustomFieldsOnTableAndProceduresSummary(grid, baseSummaryUserControl, rowHandle, keyValuePairs, objectType);
				break;
			case SharedObjectTypeEnum.ObjectType.Function:
			case SharedObjectTypeEnum.ObjectType.Procedure:
				UpdateProcedure(grid, rowHandle, keyValuePairs);
				HistoryObjectsListShortcutHelper.InsertHistoryCustomFieldsOnTableAndProceduresSummary(grid, baseSummaryUserControl, rowHandle, keyValuePairs, objectType);
				break;
			case SharedObjectTypeEnum.ObjectType.Module:
				UpdateModule(grid, rowHandle, keyValuePairs);
				HistoryObjectsListShortcutHelper.InsertHistoryCustomFieldsOnModuleSummary(grid, baseSummaryUserControl, rowHandle, keyValuePairs, objectType);
				break;
			}
			IId id2 = grid.GetRow(rowHandle) as IId;
			int id3 = id2.Id;
			CustomFieldContainer customFieldContainer2 = new CustomFieldContainer(objectType, id3, customFieldsSupport);
			customFieldContainer2.RetrieveCustomFields(id2 as ICustomFieldsData);
			customFieldContainer2.UpdateCustomFieldDefinitionValues(DB.CustomField.UpdateCustomFieldValues);
			if (baseSummaryUserControl.MainControl.ShowProgress && ProgressColumnHelper.IsProgressColumnChanged(columnCaptions, baseSummaryUserControl.MainControl.ProgressType.ColumnName))
			{
				baseSummaryUserControl.MainControl.RefreshObjectProgress(showWaitForm: false, id3, objectType);
			}
			break;
		}
		case SharedObjectTypeEnum.ObjectType.Term:
		{
			UpdateTerm(grid, rowHandle, keyValuePairs);
			IBasicData basicData = grid.GetRow(rowHandle) as IBasicData;
			int? id = basicData.Id;
			if (id.HasValue)
			{
				CustomFieldContainer customFieldContainer = new CustomFieldContainer(objectType, id, customFieldsSupport);
				customFieldContainer.RetrieveCustomFields(basicData as CustomFieldsData);
				customFieldContainer.UpdateCustomFieldDefinitionValues(DB.CustomField.UpdateCustomFieldValues);
				if (baseSummaryUserControl != null && baseSummaryUserControl.MainControl?.ShowProgress == true && ProgressColumnHelper.IsProgressColumnChanged(columnCaptions, baseSummaryUserControl.MainControl.ProgressType.ColumnName))
				{
					baseSummaryUserControl.MainControl.RefreshObjectProgress(showWaitForm: false, id ?? (-1), objectType);
				}
			}
			HistoryObjectsListShortcutHelper.InsertHistoryCustomFieldsOnTermSummary(grid, baseSummaryUserControl, rowHandle, keyValuePairs, objectType);
			break;
		}
		}
	}

	public static void UpdateSummaryControl(CustomFieldsSupport customFieldsSupport, GridView grid, Dictionary<string, object> keyValuePairs, SharedObjectTypeEnum.ObjectType objectType, BaseSummaryUserControl baseSummaryUserControl)
	{
		switch (objectType)
		{
		case SharedObjectTypeEnum.ObjectType.Function:
		case SharedObjectTypeEnum.ObjectType.Procedure:
		case SharedObjectTypeEnum.ObjectType.Table:
		case SharedObjectTypeEnum.ObjectType.View:
		case SharedObjectTypeEnum.ObjectType.Structure:
		case SharedObjectTypeEnum.ObjectType.Module:
		{
			switch (objectType)
			{
			case SharedObjectTypeEnum.ObjectType.Table:
			case SharedObjectTypeEnum.ObjectType.View:
			case SharedObjectTypeEnum.ObjectType.Structure:
				UpdateTables(grid, keyValuePairs);
				HistoryObjectsListShortcutHelper.InsertHistoryCustomFieldsOnTableAndProceduresSummary(grid, baseSummaryUserControl, null, keyValuePairs, objectType);
				break;
			case SharedObjectTypeEnum.ObjectType.Function:
			case SharedObjectTypeEnum.ObjectType.Procedure:
				UpdateProcedures(grid, keyValuePairs);
				HistoryObjectsListShortcutHelper.InsertHistoryCustomFieldsOnTableAndProceduresSummary(grid, baseSummaryUserControl, null, keyValuePairs, objectType);
				break;
			case SharedObjectTypeEnum.ObjectType.Module:
				UpdateModules(grid, keyValuePairs);
				HistoryObjectsListShortcutHelper.InsertHistoryCustomFieldsOnModuleSummary(grid, baseSummaryUserControl, null, keyValuePairs, objectType);
				break;
			}
			int[] selectedRows = grid.GetSelectedRows();
			foreach (int rowHandle2 in selectedRows)
			{
				IId id2 = grid.GetRow(rowHandle2) as IId;
				int id3 = id2.Id;
				CustomFieldContainer customFieldContainer2 = new CustomFieldContainer(objectType, id3, customFieldsSupport);
				customFieldContainer2.RetrieveCustomFields(id2 as ICustomFieldsData);
				customFieldContainer2.UpdateCustomFieldDefinitionValues(DB.CustomField.UpdateCustomFieldValues);
				if (baseSummaryUserControl.MainControl.ShowProgress && ProgressColumnHelper.IsProgressColumnChanged(grid.GetSelectedCells(), baseSummaryUserControl.MainControl.ProgressType.ColumnName))
				{
					baseSummaryUserControl.MainControl.RefreshObjectProgress(showWaitForm: false, id3, objectType);
				}
			}
			break;
		}
		case SharedObjectTypeEnum.ObjectType.Term:
		{
			UpdateTerms(grid, keyValuePairs);
			int[] selectedRows = grid.GetSelectedRows();
			foreach (int rowHandle in selectedRows)
			{
				IBasicData basicData = grid.GetRow(rowHandle) as IBasicData;
				int? id = basicData.Id;
				if (id.HasValue)
				{
					CustomFieldContainer customFieldContainer = new CustomFieldContainer(objectType, id, customFieldsSupport);
					customFieldContainer.RetrieveCustomFields(basicData as CustomFieldsData);
					customFieldContainer.UpdateCustomFieldDefinitionValues(DB.CustomField.UpdateCustomFieldValues);
					if (baseSummaryUserControl != null && baseSummaryUserControl.MainControl?.ShowProgress == true && ProgressColumnHelper.IsProgressColumnChanged(grid.GetSelectedCells(), baseSummaryUserControl.MainControl.ProgressType.ColumnName))
					{
						baseSummaryUserControl.MainControl.RefreshObjectProgress(showWaitForm: false, id ?? (-1), objectType);
					}
				}
			}
			HistoryObjectsListShortcutHelper.InsertHistoryCustomFieldsOnTermSummary(grid, baseSummaryUserControl, null, keyValuePairs, objectType);
			break;
		}
		}
	}

	public static void UpdateTables(GridView grid, Dictionary<string, object> keyValuePairs)
	{
		Form owner = grid?.GridControl?.FindForm();
		foreach (KeyValuePair<string, object> keyValuePair in keyValuePairs)
		{
			DB.Table.BulkCopyTableUpdate(GetSelectedObjectIds(grid, keyValuePair.Key), keyValuePair.Value?.ToString(), keyValuePair.Key, owner);
		}
	}

	public static void UpdateTable(GridView grid, int rowHandle, Dictionary<string, object> keyValuePairs)
	{
		DB.Table.BulkCopyTableUpdate(GetObjectId(grid, rowHandle).GetValueOrDefault(), keyValuePairs);
	}

	public static void UpdateProcedures(GridView grid, Dictionary<string, object> keyValuePairs)
	{
		Form owner = grid?.GridControl?.FindForm();
		foreach (KeyValuePair<string, object> keyValuePair in keyValuePairs)
		{
			DB.Procedure.BulkCopyProcedureUpdate(GetSelectedObjectIds(grid, keyValuePair.Key), keyValuePair.Value?.ToString(), keyValuePair.Key, owner);
		}
	}

	public static void UpdateProcedure(GridView grid, int rowHandle, Dictionary<string, object> keyValuePairs)
	{
		DB.Procedure.BulkCopyProcedureUpdate(GetObjectId(grid, rowHandle).GetValueOrDefault(), keyValuePairs);
	}

	public static void UpdateModules(GridView grid, Dictionary<string, object> keyValuePairs)
	{
		Form owner = grid?.GridControl?.FindForm();
		foreach (KeyValuePair<string, object> keyValuePair in keyValuePairs)
		{
			DB.Module.BulkCopyModulesUpdate(GetSelectedObjectIds(grid, keyValuePair.Key), keyValuePair.Value?.ToString(), keyValuePair.Key, owner);
		}
	}

	public static void UpdateModule(GridView grid, int rowHandle, Dictionary<string, object> keyValuePairs)
	{
		DB.Module.BulkCopyModulesUpdate(GetObjectId(grid, rowHandle).GetValueOrDefault(), keyValuePairs);
	}

	public static void UpdateTerms(GridView grid, Dictionary<string, object> keyValuePairs)
	{
		Form owner = grid?.GridControl?.FindForm();
		foreach (KeyValuePair<string, object> keyValuePair in keyValuePairs)
		{
			DB.BusinessGlossary.BulkCopyTermsUpdate(GetSelectedBasicDataObjectId(grid, keyValuePair.Key), keyValuePair.Value?.ToString(), keyValuePair.Key, owner);
		}
	}

	public static void UpdateTerm(GridView grid, int rowHandle, Dictionary<string, object> keyValuePairs)
	{
		DB.BusinessGlossary.BulkCopyTermsUpdateSingleTerm(GetObjectId(grid, rowHandle).GetValueOrDefault(), keyValuePairs);
	}

	public static void UpdateTitle(GridView gridView, int rowHandle, SharedObjectTypeEnum.ObjectType objectType)
	{
		object row = gridView.GetRow(rowHandle);
		if (objectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			UpdateModuleTitle(objectType, row as ModuleObject);
		}
		else
		{
			UpdateObjectTitle(objectType, row as BaseDataObjectWithCustomFields);
		}
	}

	private static void UpdateModuleTitle(SharedObjectTypeEnum.ObjectType objectType, ModuleObject row)
	{
		DBTreeMenu.RefeshNodeTitle(PrepareValue.ToInt(row.ModuleId) ?? (-1), PrepareValue.ToString(row.Title), objectType);
	}

	private static void UpdateObjectTitle(SharedObjectTypeEnum.ObjectType objectType, BaseDataObjectWithCustomFields row)
	{
		DBTreeMenu.RefeshNodeTitle(row.DatabaseId, PrepareValue.ToString(row.Title), objectType, PrepareValue.ToString(row.Name), PrepareValue.ToString(row.Schema), DatabaseTypeEnum.StringToType(PrepareValue.ToString(row.DatabaseType)), PrepareValue.ToBool(row.DatabaseMultipleSchemas));
	}

	private static List<int> GetSelectedObjectIds(GridView gridView, string fieldColumnName)
	{
		List<int> list = new List<int>();
		int[] selectedRows = gridView.GetSelectedRows();
		foreach (int rowHandle in selectedRows)
		{
			if ((from x in gridView.GetSelectedCells(rowHandle)
				select x.FieldName.ToLower()).Contains(fieldColumnName))
			{
				list.Add(GetObjectId(gridView, rowHandle).GetValueOrDefault());
			}
		}
		return list;
	}

	private static int? GetObjectId(GridView gridView, int rowHandle)
	{
		return (gridView.GetRow(rowHandle) as IId).Id;
	}

	private static List<int> GetSelectedBasicDataObjectId(GridView gridView, string fieldColumnName)
	{
		List<int> list = new List<int>();
		int[] selectedRows = gridView.GetSelectedRows();
		foreach (int rowHandle in selectedRows)
		{
			if ((from x in gridView.GetSelectedCells(rowHandle)
				select x.FieldName.ToLower()).Contains(fieldColumnName))
			{
				list.Add(GetBasicDataObjectId(gridView, rowHandle).GetValueOrDefault());
			}
		}
		return list;
	}

	private static int? GetBasicDataObjectId(GridView gridView, int rowHandle)
	{
		return (gridView.GetRow(rowHandle) as IBasicData).Id;
	}
}
