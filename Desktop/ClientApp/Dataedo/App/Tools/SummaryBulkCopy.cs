using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.UserControls.SummaryControls;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Shared.Enums;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Tools;

public class SummaryBulkCopy : BulkCopy, IBulkCopy
{
	private readonly SharedObjectTypeEnum.ObjectType objectType;

	private readonly BaseSummaryUserControl baseSummaryUserControl;

	public CustomFieldsSupport CustomFieldsSupport { get; set; }

	public SummaryBulkCopy(SharedObjectTypeEnum.ObjectType objectType)
	{
		this.objectType = objectType;
	}

	public SummaryBulkCopy(SharedObjectTypeEnum.ObjectType objectType, BaseSummaryUserControl baseSummaryUserControl)
	{
		this.objectType = objectType;
		this.baseSummaryUserControl = baseSummaryUserControl;
	}

	public void SetValueForSelectedColumns(GridView grid, string value, bool hasSingleValue, KeyEventArgs e)
	{
		GridCell[] selectedCells;
		IEnumerable<GridCell> editableCells = GetEditableCells(grid, value, out selectedCells);
		if (hasSingleValue)
		{
			int num = editableCells.Count();
			if (num == 0)
			{
				return;
			}
			CellSelector.SelectEditableCells(selectedCells, editableCells, grid);
			if (!ShowDialog(num, value))
			{
				CellSelector.SelectCells(selectedCells, grid);
				e.SuppressKeyPress = true;
				e.Handled = true;
				return;
			}
			SetValues(grid, value, editableCells, hasSingleValue);
			BulkCopySummaryControlHelper.UpdateSummaryControl(CustomFieldsSupport, grid, GetKeyValuePair(editableCells, value), objectType, baseSummaryUserControl);
		}
		else
		{
			grid.PasteFromClipboard();
			foreach (GridCell item in editableCells.Where((GridCell x) => x.Column.FieldName.Equals("Title")))
			{
				BulkCopySummaryControlHelper.UpdateTitle(grid, item.RowHandle, objectType);
			}
		}
		base.IsCopying = false;
		e.SuppressKeyPress = true;
		e.Handled = true;
	}

	private IEnumerable<GridCell> GetEditableCells(GridView grid, string value, out GridCell[] selectedCells)
	{
		new List<GridCell>();
		selectedCells = grid.GetSelectedCells();
		return from x in selectedCells
			where !x.Column.ReadOnly && x.Column.OptionsColumn.AllowEdit && !x.Column.FieldName.Equals("ModulesId")
			where IsValueProper(value, x)
			select x;
	}

	public void BaseSetValues(GridView grid, string value, IEnumerable<GridCell> editableSelectedCells, bool isDeleting)
	{
		base.SetValues(grid, value, editableSelectedCells, isDeleting);
	}

	protected override void SetValues(GridView grid, string value, IEnumerable<GridCell> editableSelectedCells, bool isDeleting)
	{
		grid.BeginDataUpdate();
		foreach (GridCell editableSelectedCell in editableSelectedCells)
		{
			BasicRow basicRow = grid.GetRow(editableSelectedCell.RowHandle) as BasicRow;
			if (IsValueProper(value, editableSelectedCell))
			{
				if (isDeleting)
				{
					grid.SetRowCellValue(editableSelectedCell.RowHandle, editableSelectedCell.Column, GetProperValue(value, editableSelectedCell.Column));
				}
				basicRow?.SetModified();
			}
		}
		foreach (GridCell item in editableSelectedCells.Where((GridCell x) => x.Column.FieldName.Equals("Title")))
		{
			BulkCopySummaryControlHelper.UpdateTitle(grid, item.RowHandle, objectType);
		}
		grid.EndDataUpdate();
	}

	public Dictionary<string, object> GetKeyValuePair(IEnumerable<GridCell> selectedCells, string value)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (GridCell selectedCell in selectedCells)
		{
			if (!dictionary.ContainsKey(selectedCell.Column.FieldName) && !dictionary.ContainsKey(selectedCell.Column.FieldName.ToLower()))
			{
				dictionary.Add(selectedCell.Column.FieldName.ToLower(), GetProperValue(value, selectedCell.Column));
			}
		}
		return dictionary;
	}

	public void UpdateSummaryControl(GridView grid, Dictionary<string, object> keyValuePairs, int rowHandle, IEnumerable<string> columnCaptions)
	{
		foreach (CustomFieldRowExtended customField in CustomFieldsSupport.Fields.Where((CustomFieldRowExtended x) => x.IsOpenDefinitionType && keyValuePairs.ContainsKey(x.FieldPropertyName.ToLower())))
		{
			GridColumn gridColumn = grid.Columns.FirstOrDefault((GridColumn x) => x.Caption.Equals(customField.Title));
			string value = keyValuePairs[customField.FieldPropertyName.ToLower()]?.ToString();
			grid.SetRowCellValue(rowHandle, gridColumn, value);
			customField.UpdateAddedDefinitionSingleValue(value);
			CustomFieldsRepositoryItems.RefreshEditOpenValues(gridColumn.ColumnEdit, customField);
		}
		BulkCopySummaryControlHelper.UpdateSummaryControl(CustomFieldsSupport, grid, rowHandle, columnCaptions, keyValuePairs, objectType, baseSummaryUserControl);
	}
}
