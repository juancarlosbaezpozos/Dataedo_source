using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.Model.Data.Modules;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Tools.Helpers;

public static class ModuleSummaryHelper
{
	public static void MoveModuleToTopOnGrid(GridView moduleSummaryGridView, MetadataEditorUserControl metadataEditorUserControl, Form owner)
	{
		MoveModuleToTopOrButomGrid(moveToTheTop: true, moduleSummaryGridView, metadataEditorUserControl, owner);
	}

	public static void MoveModuleToBottomOnGrid(GridView moduleSummaryGridView, MetadataEditorUserControl metadataEditorUserControl, Form owner)
	{
		MoveModuleToTopOrButomGrid(moveToTheTop: false, moduleSummaryGridView, metadataEditorUserControl, owner);
	}

	private static void MoveModuleToTopOrButomGrid(bool moveToTheTop, GridView moduleSummaryGridView, MetadataEditorUserControl metadataEditorUserControl, Form owner)
	{
		try
		{
			if (moduleSummaryGridView == null || metadataEditorUserControl == null)
			{
				return;
			}
			int[] array = moduleSummaryGridView.GetSelectedRows();
			if (!array.Any())
			{
				return;
			}
			List<ModuleWithoutDescriptionObject> list = new List<ModuleWithoutDescriptionObject>();
			if (moveToTheTop)
			{
				array = array.OrderByDescending((int x) => x).ToArray();
			}
			int[] array2 = array;
			foreach (int rowHandle in array2)
			{
				ModuleWithoutDescriptionObject item = moduleSummaryGridView.GetRow(rowHandle) as ModuleWithoutDescriptionObject;
				list.Add(item);
			}
			foreach (ModuleWithoutDescriptionObject item2 in list)
			{
				if (moveToTheTop)
				{
					metadataEditorUserControl.MoveModuleToTheTop(item2.Id, reloadModulesSummaryRows: false, item2.Id == list.Last().Id);
				}
				else
				{
					metadataEditorUserControl.MoveModuleToTheBottom(item2.Id, reloadModulesSummaryRows: false, item2.Id == list.Last().Id);
				}
			}
			metadataEditorUserControl.ReloadModuleSummaryControlRows();
			if (moveToTheTop)
			{
				moduleSummaryGridView.SelectRows(0, list.Count - 1);
			}
			else
			{
				moduleSummaryGridView.SelectRows(moduleSummaryGridView.DataRowCount - list.Count, moduleSummaryGridView.DataRowCount - 1);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, owner);
		}
	}

	public static void MoveModuleUpOnGridView(GridView moduleSummaryGridView, MetadataEditorUserControl metadataEditorUserControl, Form owner = null)
	{
		try
		{
			if (moduleSummaryGridView == null || metadataEditorUserControl == null)
			{
				return;
			}
			IEnumerable<int> enumerable = GridObjects.MoveUp<ModuleWithoutDescriptionObject>(moduleSummaryGridView);
			int[] selectedRows = moduleSummaryGridView.GetSelectedRows();
			foreach (int num in selectedRows)
			{
				if (enumerable.Contains(num))
				{
					ModuleWithoutDescriptionObject moduleWithoutDescriptionObject = moduleSummaryGridView.GetRow(num) as ModuleWithoutDescriptionObject;
					metadataEditorUserControl.MoveUpNode(moduleWithoutDescriptionObject.Id);
				}
			}
			UpdateOrdinalPositions(enumerable, moduleSummaryGridView, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, owner);
		}
	}

	public static void MoveModuleDownOnGridView(GridView moduleSummaryGridView, MetadataEditorUserControl metadataEditorUserControl, Form owner = null)
	{
		try
		{
			if (moduleSummaryGridView == null || metadataEditorUserControl == null)
			{
				return;
			}
			IEnumerable<int> enumerable = GridObjects.MoveDown<ModuleWithoutDescriptionObject>(moduleSummaryGridView);
			foreach (int item in from x in moduleSummaryGridView.GetSelectedRows()
				orderby x descending
				select x)
			{
				if (enumerable.Contains(item))
				{
					ModuleWithoutDescriptionObject moduleWithoutDescriptionObject = moduleSummaryGridView.GetRow(item) as ModuleWithoutDescriptionObject;
					metadataEditorUserControl.MoveDownNode(moduleWithoutDescriptionObject.Id);
				}
			}
			UpdateOrdinalPositions(enumerable, moduleSummaryGridView, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, owner);
		}
	}

	private static void UpdateOrdinalPositions(IEnumerable<int> touchedRowHandles, GridView moduleSummaryGridView, Form owner = null)
	{
		if (moduleSummaryGridView == null)
		{
			return;
		}
		foreach (int touchedRowHandle in touchedRowHandles)
		{
			ModuleWithoutDescriptionObject moduleWithoutDescriptionObject = moduleSummaryGridView.GetRow(touchedRowHandle) as ModuleWithoutDescriptionObject;
			DB.Module.UpdateOrdinalPosition(moduleWithoutDescriptionObject.Id, touchedRowHandle, owner);
		}
	}
}
