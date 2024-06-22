using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.UserControls;
using Dataedo.CustomMessageBox;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Interfaces;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Tools;

public class BulkCopy
{
	public bool IsCopying { get; set; }

	protected virtual void SetValues(GridView grid, string value, IEnumerable<GridCell> editableSelectedCells, bool hasSingleValue)
	{
		grid.BeginDataUpdate();
		foreach (GridCell editableSelectedCell in editableSelectedCells)
		{
			IModifiableBasic modifiableBasic = grid.GetRow(editableSelectedCell.RowHandle) as IModifiableBasic;
			if (IsValueProper(value, editableSelectedCell))
			{
				if (modifiableBasic != null && modifiableBasic.Id > 0)
				{
					modifiableBasic?.SetModified();
				}
				if (hasSingleValue)
				{
					grid.SetRowCellValue(editableSelectedCell.RowHandle, editableSelectedCell.Column, GetProperValue(value, editableSelectedCell.Column));
				}
				if (editableSelectedCell.Column.Tag is CustomFieldRowExtended customFieldRowExtended && customFieldRowExtended.IsOpenDefinitionType)
				{
					customFieldRowExtended.UpdateAddedDefinitionSingleValue(value);
					CustomFieldsRepositoryItems.RefreshEditOpenValues(editableSelectedCell.Column.ColumnEdit, customFieldRowExtended);
				}
			}
		}
		grid.EndDataUpdate();
	}

	public virtual void SetValues(GridView grid, string value, int rowHandle, GridColumn column)
	{
		IModifiableBasic modifiableBasic = grid.GetRow(rowHandle) as IModifiableBasic;
		if (IsValueProper(value, column))
		{
			if (modifiableBasic != null && modifiableBasic.Id > 0)
			{
				modifiableBasic?.SetModified();
			}
			if (column.Tag is CustomFieldRowExtended customFieldRowExtended && customFieldRowExtended.IsOpenDefinitionType)
			{
				customFieldRowExtended.UpdateAddedDefinitionSingleValue(value);
				CustomFieldsRepositoryItems.RefreshEditOpenValues(column.ColumnEdit, customFieldRowExtended);
			}
		}
	}

	public bool ShowDialog(int changedCellsCount, string value)
	{
		IsCopying = true;
		if (changedCellsCount > 1)
		{
			return CustomMessageBoxForm.Show(string.IsNullOrEmpty(value) ? $"Are you sure you want to delete the text of {changedCellsCount} selected cells?" : $"Are you sure you want to set value of {changedCellsCount} cells?", "Paste", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) == DialogResult.Yes;
		}
		return true;
	}

	public virtual bool IsValueProper(string value, GridColumn column)
	{
		if (column.ColumnType.IsPrimitive || column.ColumnType.IsEnum || column.ColumnType.Equals(typeof(string)) || column.ColumnType.Equals(typeof(decimal)))
		{
			if (column.Tag is CustomFieldRowExtended)
			{
				CustomFieldRowExtended customFieldRowExtended = column.Tag as CustomFieldRowExtended;
				if (customFieldRowExtended.IsDomainValueType)
				{
					return customFieldRowExtended.IsValueProperForDomainValuesType(value);
				}
			}
			return true;
		}
		return false;
	}

	public virtual bool IsValueProper(string value, GridCell cell)
	{
		if (cell.Column.ColumnType.IsPrimitive || cell.Column.ColumnType.IsEnum || cell.Column.ColumnType.Equals(typeof(string)) || cell.Column.ColumnType.Equals(typeof(decimal)))
		{
			if (cell.Column.Tag is CustomFieldRowExtended)
			{
				CustomFieldRowExtended customFieldRowExtended = cell.Column.Tag as CustomFieldRowExtended;
				if (customFieldRowExtended.IsDomainValueType)
				{
					return customFieldRowExtended.IsValueProperForDomainValuesType(value);
				}
			}
			return true;
		}
		return false;
	}

	public virtual object GetProperValue(string value, GridColumn column)
	{
		object empty = string.Empty;
		if (string.IsNullOrEmpty(value))
		{
			return empty;
		}
		empty = ((column.RealColumnEdit is RepositoryItemAutoHeightMemoEdit) ? value.Replace("\n", Environment.NewLine).Replace("\"", "") : ((!(column.RealColumnEdit is RepositoryItemTextEdit)) ? value : ((!column.FieldName.ToLower().Equals("title") || value.Length <= 80) ? GetMultilineValues(value) : GetShortenedValue(value, 79))));
		if (column.Tag is CustomFieldRowExtended)
		{
			CustomFieldRowExtended customFieldRowExtended = column.Tag as CustomFieldRowExtended;
			if (customFieldRowExtended.IsDomainValueType)
			{
				empty = customFieldRowExtended.GetPreparedValueForDomainValuesType(empty);
			}
		}
		return empty;
	}

	protected object GetShortenedValue(string value, int length)
	{
		if (value.Contains(Environment.NewLine))
		{
			return value.Substring(0, length).Replace(Environment.NewLine, " ");
		}
		return value.Substring(0, length);
	}

	public string GetMultilineValues(string value)
	{
		if (value.Contains(Environment.NewLine))
		{
			return value.Replace(Environment.NewLine, " ");
		}
		return value.Replace("\n", " ");
	}
}
