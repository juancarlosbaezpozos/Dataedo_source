using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools;
using Dataedo.App.Tools.UI;
using Dataedo.Shared.Enums;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.UserControls;

public class BulkCopyGridUserControl : CustomGridUserControl
{
	private List<GridCell> selectedCells;

	private bool isCounting;

	private IBulkCopy copy;

	private int[] selectedRowsHandles;

	private List<GridColumn> columnsSelectedToPaste;

	public IBulkCopy Copy
	{
		get
		{
			if (copy == null)
			{
				return new DefaultBulkCopy();
			}
			return copy;
		}
		set
		{
			copy = value;
		}
	}

	public SplashScreenManager SplashScreenManager { get; set; }

	public new GridControl GridControl
	{
		get
		{
			return base.GridControl;
		}
		set
		{
			if (value != null)
			{
				if (base.GridControl != null)
				{
					base.GridControl.KeyPress -= GridControl_KeyPress;
					base.GridControl.ProcessGridKey -= GridControl_ProcessGridKey;
				}
				value.KeyPress += GridControl_KeyPress;
				value.ProcessGridKey += GridControl_ProcessGridKey;
				base.GridControl = value;
			}
		}
	}

	private string ClipboardData
	{
		get
		{
			try
			{
				IDataObject dataObject = Clipboard.GetDataObject();
				if (dataObject == null)
				{
					return string.Empty;
				}
				if (dataObject.GetDataPresent(DataFormats.UnicodeText))
				{
					return dataObject.GetData(DataFormats.UnicodeText)?.ToString();
				}
				return string.Empty;
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}
		set
		{
			ClipboardSupport.SetDataObject(value, GridControl?.FindForm());
		}
	}

	public BulkCopyGridUserControl()
	{
		selectedCells = new List<GridCell>();
		base.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
		base.OptionsSelection.MultiSelect = true;
		base.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
		base.OptionsBehavior.EditorShowMode = EditorShowMode.MouseDownFocused;
		base.OptionsClipboard.ClipboardMode = ClipboardMode.Formatted;
		base.OptionsClipboard.PasteMode = PasteMode.Update;
		base.OptionsClipboard.AllowExcelFormat = DefaultBoolean.True;
		base.OptionsClipboard.AllowRtfFormat = (base.OptionsClipboard.AllowHtmlFormat = (base.OptionsClipboard.AllowCsvFormat = DefaultBoolean.False));
		base.ClipboardRowPasting += BulkCopyGridUserControl_ClipboardRowPasting;
	}

	private void GridControl_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar == '\u0016')
		{
			e.Handled = true;
		}
	}

	private void GridControl_ProcessGridKey(object sender, KeyEventArgs e)
	{
		if (!((sender as GridControl).FocusedView is GridView))
		{
			return;
		}
		switch (e.KeyData)
		{
		case Keys.V | Keys.Control:
		{
			base.OptionsClipboard.ShowProgress = ProgressMode.Never;
			if (ActiveEditor != null && !(ActiveEditor is CheckedComboBoxEdit) && !(ActiveEditor is LookUpEdit))
			{
				break;
			}
			selectedRowsHandles = GetSelectedRows();
			columnsSelectedToPaste = (from c in GetSelectedCells()
				select c.Column)?.ToList();
			string text = Clipboard.GetText();
			if (!text.Contains(Environment.NewLine) && !text.Contains("\t"))
			{
				copy.IsCopying = true;
				copy.SetValueForSelectedColumns(this, text, hasSingleValue: true, e);
			}
			else
			{
				FocusTopLeftSelectedCell();
				isCounting = true;
				selectedCells.Clear();
				ClearSelection();
				BeginUpdate();
				PasteFromClipboard();
				base.OptionsClipboard.ShowProgress = ((!(Copy is SummaryBulkCopy)) ? ProgressMode.Always : ProgressMode.Never);
				CellSelector.UnselectCells(GetSelectedCells(), this);
				CellSelector.SelectCells(selectedCells, this);
				EndUpdate();
				if (Copy.ShowDialog(selectedCells.Count, ClipboardDataProvider.GetData()?.ToString()))
				{
					if (SplashScreenManager != null)
					{
						CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: true);
					}
					copy.IsCopying = true;
					isCounting = false;
					BeginUpdate();
					copy.SetValueForSelectedColumns(this, text, hasSingleValue: false, e);
					EndUpdate();
					Copy.IsCopying = false;
					if (SplashScreenManager != null)
					{
						CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: false);
					}
				}
				CellSelector.UnselectCells(GetSelectedCells(), this);
				CellSelector.SelectCells(selectedCells, this);
			}
			e.Handled = true;
			break;
		}
		case Keys.C | Keys.Control:
			CopyToClipboard();
			e.Handled = true;
			break;
		case Keys.Delete:
			if (ActiveEditor == null || ActiveEditor is CheckedComboBoxEdit || ActiveEditor is LookUpEdit)
			{
				Copy.SetValueForSelectedColumns(this, null, hasSingleValue: true, e);
			}
			break;
		}
	}

	private void FocusTopLeftSelectedCell()
	{
		GridCell[] array = GetSelectedCells();
		if (array != null && array.Length != 0)
		{
			base.FocusedRowHandle = array.Select((GridCell x) => x.RowHandle).Min();
			int focusedColumnsMinVisibleIndex = array.Select((GridCell x) => x.Column.VisibleIndex).Min();
			FocusedColumn = array.Select((GridCell x) => x.Column).FirstOrDefault((GridColumn y) => y.VisibleIndex == focusedColumnsMinVisibleIndex);
		}
	}

	private void InitializeComponent()
	{
		((ISupportInitialize)this).BeginInit();
		base.OptionsClipboard.ShowProgress = ProgressMode.Never;
		base.OptionsNavigation.AutoMoveRowFocus = false;
		((ISupportInitialize)this).EndInit();
	}

	private void BulkCopyGridUserControl_ClipboardRowPasting(object sender, ClipboardRowPastingEventArgs e)
	{
		if (e.RowHandle >= 0)
		{
			if (isCounting)
			{
				CountCells(sender, e);
			}
			else
			{
				Paste(sender, e);
			}
		}
	}

	private void CountCells(object sender, ClipboardRowPastingEventArgs e)
	{
		int num = e.Values.Keys.ToArray().Count();
		new List<string>();
		for (int i = 0; i < num; i++)
		{
			GridColumn gridColumn = e.Values.Keys.ToArray()[i];
			List<GridColumn> list = columnsSelectedToPaste;
			if ((list == null || list.Count <= 1 || columnsSelectedToPaste.Contains(gridColumn)) && copy.IsValueProper(e.Values[gridColumn]?.ToString(), gridColumn) && !gridColumn.ReadOnly && gridColumn.OptionsColumn.AllowEdit && !gridColumn.FieldName.Equals("ModulesId") && (!(copy is DesignTableBulkCopy) || (GetRow(e.RowHandle) as ColumnRow).Source != UserTypeEnum.UserType.DBMS || gridColumn.FieldName.Equals("Title") || gridColumn.FieldName.Equals("Description")))
			{
				SelectCell(e.RowHandle, gridColumn);
			}
		}
		int[] array = selectedRowsHandles;
		if (array != null && array.Length > 1)
		{
			selectedCells.AddRange(from x in GetSelectedCells()
				where x.RowHandle == e.RowHandle && selectedRowsHandles.Contains(x.RowHandle)
				select x);
		}
		else
		{
			selectedCells.AddRange(from x in GetSelectedCells()
				where x.RowHandle == e.RowHandle
				select x);
		}
		e.Values.Clear();
	}

	private void Paste(object sender, ClipboardRowPastingEventArgs e)
	{
		int num = e.Values.Keys.ToArray().Count();
		List<GridColumn> list = columnsSelectedToPaste;
		if (list != null && list.Count > 1)
		{
			List<GridColumn> list2 = new List<GridColumn>();
			list2.AddRange(from k in e.Values.Keys.ToArray()
				where !columnsSelectedToPaste.Contains(k)
				select k);
			foreach (GridColumn item in list2)
			{
				e.Values.Remove(item);
			}
			num = e.Values.Keys.ToArray().Count();
		}
		List<string> properColumnFieldNames = new List<string>();
		for (int i = 0; i < num; i++)
		{
			GridColumn gridColumn = e.Values.Keys.ToArray()[i];
			if (!copy.IsValueProper(e.Values[gridColumn]?.ToString(), gridColumn) || gridColumn.ReadOnly || !gridColumn.OptionsColumn.AllowEdit || gridColumn.FieldName.Equals("ModulesId") || (copy is DesignTableBulkCopy && (GetRow(e.RowHandle) as ColumnRow).Source == UserTypeEnum.UserType.DBMS && !gridColumn.FieldName.Equals("Title") && !gridColumn.FieldName.Equals("Description")))
			{
				if (copy is DesignTableBulkCopy && gridColumn.RealColumnEdit.EditorTypeName.Equals("CheckEdit"))
				{
					e.Values[gridColumn] = CheckboxValues.IsPositive(e.Values[gridColumn]?.ToString());
				}
				else
				{
					e.Values[gridColumn] = GetRowCellValue(e.RowHandle, gridColumn);
				}
			}
			else
			{
				e.Values[gridColumn] = Copy.GetProperValue(e.Values[gridColumn]?.ToString(), gridColumn);
				properColumnFieldNames.Add(gridColumn.FieldName);
			}
			if (!(Copy is DefaultBulkCopy))
			{
				continue;
			}
			DefaultBulkCopy defaultBulkCopy = Copy as DefaultBulkCopy;
			GridView grid = sender as GridView;
			int[] array = selectedRowsHandles;
			if (array != null && array.Length > 1)
			{
				if (selectedRowsHandles.Contains(e.RowHandle))
				{
					defaultBulkCopy.SetValues(grid, e.Values[gridColumn]?.ToString(), e.RowHandle, gridColumn);
				}
				else
				{
					e.Cancel = true;
				}
			}
			else
			{
				defaultBulkCopy.SetValues(grid, e.Values[gridColumn]?.ToString(), e.RowHandle, gridColumn);
			}
		}
		if (!(Copy is SummaryBulkCopy))
		{
			return;
		}
		SummaryBulkCopy summaryBulkCopy = Copy as SummaryBulkCopy;
		GridView grid2 = sender as GridView;
		Dictionary<string, object> source = e.Values.ToDictionary((KeyValuePair<GridColumn, object> x) => x.Key.FieldName, (KeyValuePair<GridColumn, object> x) => x.Value);
		IEnumerable<string> columnCaptions = e.Values.Keys.Select((GridColumn x) => x.Caption);
		source = source.Where((KeyValuePair<string, object> x) => properColumnFieldNames.Contains(x.Key)).ToDictionary((KeyValuePair<string, object> x) => x.Key.ToLower(), (KeyValuePair<string, object> x) => x.Value);
		int[] array2 = selectedRowsHandles;
		if (array2 != null && array2.Length > 1)
		{
			if (selectedRowsHandles.Contains(e.RowHandle))
			{
				summaryBulkCopy.UpdateSummaryControl(grid2, source, e.RowHandle, columnCaptions);
			}
			else
			{
				e.Cancel = true;
			}
		}
		else
		{
			summaryBulkCopy.UpdateSummaryControl(grid2, source, e.RowHandle, columnCaptions);
		}
	}
}
