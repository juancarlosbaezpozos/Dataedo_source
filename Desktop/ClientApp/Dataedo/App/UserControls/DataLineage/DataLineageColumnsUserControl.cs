using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.App.Enums;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Model.Enums;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.DataLineage;

public class DataLineageColumnsUserControl : BaseUserControl
{
	private const string ShowAllDestinationsCaption = "Show all destinations";

	private const string ShowAllInputsCaption = "Show all inputs";

	public List<DataLineageColumnsFlowRow> DeletedColumnRows;

	public int[] selectedRowHandles;

	private int inflowsCount;

	private int outflowCount;

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private GridControl columnsGridControl;

	private GridView columnsGridView;

	private LayoutControlItem mainLayoutControlItem;

	private GridColumn inflowColumnIconGridColumn;

	private GridColumn inflowColumnGridColumn;

	private GridColumn outflowColumnIconGridColumn;

	private GridColumn outflowColumnGridColumn;

	private RepositoryItemGridLookUpEdit inflowColumnRepositoryItemGridLookUpEdit;

	private GridView inflowColumnRepositoryItemGridLookUpEditView;

	private RepositoryItemGridLookUpEdit outflowColumnRepositoryItemGridLookUpEdit;

	private GridView outflowColumnRepositoryItemGridLookUpEditView;

	private RepositoryItemPictureEdit inflowColumnRepositoryItemPictureEdit;

	private RepositoryItemPictureEdit outflowColumnRepositoryItemPictureEdit;

	private GridColumn inflowColumnIconPopupGridColumn;

	private GridColumn inflowColumnNamePopupGridColumn;

	private GridColumn outflowColumnIconPopupGridColumn;

	private GridColumn outflowColumnNamePopupGridColumn;

	private RepositoryItemPictureEdit inflowColumnNamePopupRepositoryItemPictureEdit;

	private RepositoryItemCustomTextEdit inflowColumnNamePopupRepositoryItemCustomTextEdit;

	private RepositoryItemPictureEdit outflowColumnImagePopupRepositoryItemRepositoryItemPictureEdit;

	private RepositoryItemCustomTextEdit outflowColumnNamePopupRepositoryItemCustomTextEdit;

	private GridColumn inflowObjectIconGridColumn;

	private RepositoryItemPictureEdit inflowObjectRepositoryItemPictureEdit;

	private GridColumn inflowObjectGridColumn;

	private GridColumn outflowObjectIconGridColumn;

	private RepositoryItemPictureEdit outflowObjectRepositoryItemPictureEdit;

	private GridColumn outflowObjectGridColumn;

	private GridColumn inflowObjectNamePopupGridColumn;

	private GridColumn inflowObjectIconPopupGridColumn;

	private RepositoryItemPictureEdit inflowObjectIconRepositoryItemPictureEdit;

	private GridColumn outflowObjectIconPopupGridColumn;

	private RepositoryItemPictureEdit outflowObjectIconRepositoryItemPictureEdit;

	private GridColumn outflowObjectNamePopupGridColumn;

	private PopupMenu columnsGridControlPopupMenu;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private BarButtonItem deleteColumnBarButtonItem;

	private Bar bar1;

	private BarButtonItem duplicateBarButtonItem;

	private BarButtonItem autofillRowsBarButtonItem;

	private BarButtonItem clearBarButtonItem;

	private GridColumn arrowGridColumn;

	private RepositoryItemPictureEdit arrowRepositoryItemPictureEdit;

	private BarButtonItem showAllColumnsBarButtonItem;

	public DataProcessRow CurrentProcessRow { get; private set; }

	public AllDataFlowsContainer AllDataFlowsContainer { get; private set; }

	private List<DataLineageColumnsFlowRow> Columns
	{
		get
		{
			if (CurrentProcessRow == null)
			{
				return AllDataFlowsContainer.Columns;
			}
			return CurrentProcessRow?.Columns;
		}
	}

	public CriteriaOperator CurrentInflowPopupCriteria { get; private set; }

	public CriteriaOperator CurrentOutflowPopupCriteria { get; private set; }

	[Browsable(true)]
	public event DataLineageUserControl.DataLineageEditedHandler DataLineageEdited;

	public DataLineageColumnsUserControl()
	{
		InitializeComponent();
	}

	public void SetParameters(DataProcessRow dataProcessRow, AllDataFlowsContainer allDataFlowsContainer)
	{
		if (dataProcessRow == null && allDataFlowsContainer == null)
		{
			CurrentProcessRow = dataProcessRow;
			RefreshColumnsGrid();
			return;
		}
		columnsGridView.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
		columnsGridControl.BeginUpdate();
		AllDataFlowsContainer = allDataFlowsContainer;
		CurrentProcessRow = dataProcessRow;
		columnsGridControl.DataSource = Columns;
		BindingList<DataFlowRow> bindingList = ((CurrentProcessRow != null) ? CurrentProcessRow.InflowRows : AllDataFlowsContainer.InflowRows);
		BindingList<DataFlowRow> bindingList2 = ((CurrentProcessRow != null) ? CurrentProcessRow.OutflowRows : AllDataFlowsContainer.OutflowRows);
		inflowsCount = bindingList.Count;
		outflowCount = bindingList2.Count;
		SetColumnsRepositoryLookups(bindingList.ToList(), bindingList2.ToList());
		LoadAllColumns(FlowDirectionEnum.Direction.IN);
		RefreshColumnsGrid();
		SetShowAllColumnsButtonFunctionality(setShowAllDestinations: true);
		columnsGridControl.EndUpdate();
	}

	private void SetColumnsRepositoryLookups(List<DataFlowRow> inflowRows, List<DataFlowRow> outflowRows)
	{
		if (inflowRows != null)
		{
			inflowColumnRepositoryItemGridLookUpEdit.DataSource = null;
			foreach (DataFlowRow inflowRow in inflowRows)
			{
				RefeshDropdownColumnsGrid(inflowRow, loadInflowColumns: true);
			}
		}
		if (outflowRows == null)
		{
			return;
		}
		outflowColumnRepositoryItemGridLookUpEdit.DataSource = null;
		foreach (DataFlowRow outflowRow in outflowRows)
		{
			RefeshDropdownColumnsGrid(outflowRow, loadInflowColumns: false);
		}
	}

	private void LoadAllColumns(FlowDirectionEnum.Direction direction)
	{
		if (CurrentProcessRow == null && AllDataFlowsContainer != null)
		{
			AllDataFlowsContainer.Processes.ForEach(delegate(DataProcessRow p)
			{
				p.Columns.RemoveAll(delegate(DataLineageColumnsFlowRow x)
				{
					if (x.IsRowComplete)
					{
						DataFlowRow inflowRow2 = x.InflowRow;
						if (inflowRow2 == null || inflowRow2.RowState != ManagingRowsEnum.ManagingRows.Deleted)
						{
							DataFlowRow outflowRow2 = x.OutflowRow;
							if (outflowRow2 == null)
							{
								return false;
							}
							return outflowRow2.RowState == ManagingRowsEnum.ManagingRows.Deleted;
						}
					}
					return true;
				});
			});
		}
		Columns.RemoveAll(delegate(DataLineageColumnsFlowRow x)
		{
			if (x.IsRowComplete)
			{
				DataFlowRow inflowRow = x.InflowRow;
				if (inflowRow == null || inflowRow.RowState != ManagingRowsEnum.ManagingRows.Deleted)
				{
					DataFlowRow outflowRow = x.OutflowRow;
					if (outflowRow == null)
					{
						return false;
					}
					return outflowRow.RowState == ManagingRowsEnum.ManagingRows.Deleted;
				}
			}
			return true;
		});
		columnsGridControl.DataSource = Columns;
		if (!(((direction == FlowDirectionEnum.Direction.IN) ? inflowColumnRepositoryItemGridLookUpEdit : outflowColumnRepositoryItemGridLookUpEdit).DataSource is List<ColumnForDataLineageColumnsDropdown> list))
		{
			return;
		}
		foreach (ColumnForDataLineageColumnsDropdown column in list)
		{
			DataLineageColumnsFlowRow dataLineageColumnsFlowRow = null;
			dataLineageColumnsFlowRow = ((direction != FlowDirectionEnum.Direction.IN) ? Columns.FirstOrDefault((DataLineageColumnsFlowRow x) => x.OutflowColumnId == column.Id && x.OutflowRow == column.DataFlowRow) : Columns.FirstOrDefault((DataLineageColumnsFlowRow x) => x.InflowColumnId == column.Id && x.InflowRow == column.DataFlowRow));
			if (dataLineageColumnsFlowRow == null)
			{
				dataLineageColumnsFlowRow = new DataLineageColumnsFlowRow();
				SetColumnProperties(dataLineageColumnsFlowRow, column, direction == FlowDirectionEnum.Direction.IN);
				AddNewColumnToSource(dataLineageColumnsFlowRow);
			}
		}
		columnsGridView.FocusedRowHandle = int.MinValue;
		RefreshSelectedRows(null);
	}

	private void AddNewColumnToSource(DataLineageColumnsFlowRow columnToAdd)
	{
		if (CurrentProcessRow == null && AllDataFlowsContainer != null)
		{
			AllDataFlowsContainer.Processes.FirstOrDefault((DataProcessRow p) => p.InflowRows.Any((DataFlowRow i) => i == columnToAdd.InflowRow) || p.OutflowRows.Any((DataFlowRow o) => o == columnToAdd.OutflowRow))?.Columns.Add(columnToAdd);
		}
		else
		{
			Columns.Add(columnToAdd);
		}
		columnsGridControl.DataSource = Columns;
	}

	private void RefeshDropdownColumnsGrid(DataFlowRow dataFlowRow, bool loadInflowColumns)
	{
		if (dataFlowRow != null)
		{
			ControlsUtils.LoadColumns(loadInflowColumns ? inflowColumnRepositoryItemGridLookUpEdit : outflowColumnRepositoryItemGridLookUpEdit, dataFlowRow, base.ParentForm);
		}
	}

	private void RefreshSelectedRows(DataLineageColumnsFlowRow dataLineageColumnsFlowRow)
	{
		if (Columns == null || dataLineageColumnsFlowRow == null || string.IsNullOrWhiteSpace(dataLineageColumnsFlowRow.InflowColumnName))
		{
			columnsGridView.ClearSelection();
			selectedRowHandles = Array.Empty<int>();
			RefreshColumnsGrid();
			return;
		}
		IEnumerable<DataLineageColumnsFlowRow> enumerable = Columns.Where((DataLineageColumnsFlowRow x) => x.InflowColumnName == dataLineageColumnsFlowRow.InflowColumnName);
		if (columnsGridView.DataSource is List<DataLineageColumnsFlowRow>)
		{
			List<int> list = new List<int>();
			foreach (DataLineageColumnsFlowRow item in enumerable)
			{
				list.Add(columnsGridView.FindRow(item));
			}
			selectedRowHandles = list.ToArray();
		}
		RefreshColumnsGrid();
	}

	private void HandleColumnChange(object sender, bool isInflowColumn)
	{
		if (!(sender is GridLookUpEdit gridLookUpEdit) || !(gridLookUpEdit.GetSelectedDataRow() is ColumnForDataLineageColumnsDropdown columnForDataLineageColumnsDropdown))
		{
			return;
		}
		bool flag = false;
		DataLineageColumnsFlowRow dataLineageColumnsFlowRow = columnsGridView.GetFocusedRow() as DataLineageColumnsFlowRow;
		if (dataLineageColumnsFlowRow == null)
		{
			dataLineageColumnsFlowRow = new DataLineageColumnsFlowRow();
			flag = true;
		}
		SetColumnProperties(dataLineageColumnsFlowRow, columnForDataLineageColumnsDropdown, isInflowColumn);
		if (dataLineageColumnsFlowRow.DataColumnsFlowId >= 0)
		{
			columnForDataLineageColumnsDropdown.RowState = ManagingRowsEnum.ManagingRows.Updated;
		}
		else
		{
			dataLineageColumnsFlowRow.RowState = ManagingRowsEnum.ManagingRows.ForAdding;
			if (flag)
			{
				AddNewColumnToSource(dataLineageColumnsFlowRow);
			}
		}
		RefreshSelectedRows(dataLineageColumnsFlowRow);
		this.DataLineageEdited?.Invoke();
	}

	private void RefreshColumnsGrid()
	{
		columnsGridView.RefreshRow(columnsGridView.FocusedRowHandle);
		columnsGridView.RefreshData();
	}

	internal void FilterColumns(DataFlowRow dataFlowRow)
	{
		if (dataFlowRow?.Direction == FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.IN))
		{
			columnsGridView.ActiveFilterCriteria = new GroupOperator(GroupOperatorType.Or, new BinaryOperator("InflowRow", dataFlowRow), new BinaryOperator("IsRowEmpty", value: true));
			CriteriaOperator criteriaOperator3 = (CurrentInflowPopupCriteria = (inflowColumnRepositoryItemGridLookUpEditView.ActiveFilterCriteria = new BinaryOperator("DataFlowRow.Guid", dataFlowRow.Guid)));
			criteriaOperator3 = (CurrentOutflowPopupCriteria = (outflowColumnRepositoryItemGridLookUpEditView.ActiveFilterCriteria = null));
		}
		else if (dataFlowRow?.Direction == FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.OUT))
		{
			columnsGridView.ActiveFilterCriteria = new GroupOperator(GroupOperatorType.Or, new BinaryOperator("OutflowRow", dataFlowRow), new BinaryOperator("IsRowEmpty", value: true));
			CriteriaOperator criteriaOperator3 = (CurrentOutflowPopupCriteria = (outflowColumnRepositoryItemGridLookUpEditView.ActiveFilterCriteria = new BinaryOperator("DataFlowRow.Guid", dataFlowRow.Guid)));
			criteriaOperator3 = (CurrentInflowPopupCriteria = (inflowColumnRepositoryItemGridLookUpEditView.ActiveFilterCriteria = null));
		}
		else
		{
			inflowColumnRepositoryItemGridLookUpEditView.ActiveFilterCriteria = null;
			CurrentInflowPopupCriteria = null;
			outflowColumnRepositoryItemGridLookUpEditView.ActiveFilterCriteria = null;
			CurrentOutflowPopupCriteria = null;
			columnsGridView.ActiveFilterCriteria = null;
		}
	}

	private void SetBestColumnsWidth(object popupView)
	{
		if (popupView is GridLookUpEdit gridLookUpEdit && gridLookUpEdit.Properties.View.Columns.Count >= 1)
		{
			GridColumn gridColumn = gridLookUpEdit.Properties.View.Columns[1];
			gridColumn.Width = gridLookUpEdit.Properties.View.CalcColumnBestWidth(gridColumn);
			int gridLookUpEditFilteredRowCount = ControlsUtils.GetGridLookUpEditFilteredRowCount<ColumnForDataLineageColumnsDropdown>(gridLookUpEdit);
			int maxElementsOnList = ((gridLookUpEditFilteredRowCount < 15) ? gridLookUpEditFilteredRowCount : 15);
			ControlsUtils.SetGridLookUpEditHeight(gridLookUpEdit, maxElementsOnList);
		}
	}

	private void DuplicateRow(DataLineageColumnsFlowRow dataLineageColumnsFlowRow, bool duplicateInputColumn)
	{
		if (Columns != null)
		{
			DataLineageColumnsFlowRow dataLineageColumnsFlowRow2 = new DataLineageColumnsFlowRow(dataLineageColumnsFlowRow, duplicateInputColumn);
			AddNewColumnToSource(dataLineageColumnsFlowRow2);
			RefreshSelectedRows(dataLineageColumnsFlowRow2);
			int num = columnsGridView.FindRow(dataLineageColumnsFlowRow2);
			columnsGridView.FocusedRowHandle = num;
			columnsGridView.ClearSelection();
			columnsGridView.SelectRow(num);
			this.DataLineageEdited?.Invoke();
		}
	}

	private void DeleteColumnRow(DataLineageColumnsFlowRow dataLineageColumnsFlowRow)
	{
		if (dataLineageColumnsFlowRow?.ProcessRow?.Columns != null)
		{
			if (DeletedColumnRows == null)
			{
				DeletedColumnRows = new List<DataLineageColumnsFlowRow>();
			}
			dataLineageColumnsFlowRow.ProcessRow.Columns.Remove(dataLineageColumnsFlowRow);
			if (dataLineageColumnsFlowRow.DataColumnsFlowId >= 0)
			{
				DeletedColumnRows.Add(dataLineageColumnsFlowRow);
			}
			this.DataLineageEdited?.Invoke();
		}
	}

	private void AutofillColumns()
	{
		bool autofillInflows = showAllColumnsBarButtonItem.Caption == "Show all inputs";
		RepositoryItemGridLookUpEdit repositoryItemGridLookUpEdit = (autofillInflows ? inflowColumnRepositoryItemGridLookUpEdit : outflowColumnRepositoryItemGridLookUpEdit);
		if (Columns == null || !(repositoryItemGridLookUpEdit.DataSource is List<ColumnForDataLineageColumnsDropdown> source))
		{
			return;
		}
		foreach (DataLineageColumnsFlowRow column in Columns.Where((DataLineageColumnsFlowRow c) => !c.IsRowComplete))
		{
			string columnNameToSearch = (autofillInflows ? column.OutflowColumnName : column.InflowColumnName);
			DataProcessRow columnProcess = ((!autofillInflows) ? column.InflowRow?.Process : column.OutflowRow?.Process);
			if (!string.IsNullOrEmpty(columnNameToSearch) && columnProcess != null)
			{
				IEnumerable<ColumnForDataLineageColumnsDropdown> source2 = source.Where((ColumnForDataLineageColumnsDropdown x) => x.Name == columnNameToSearch && x.DataFlowRow?.Process != null && x.DataFlowRow.Process == columnProcess);
				if (source2.Any() && source2.Count() <= 1 && !string.IsNullOrWhiteSpace(columnNameToSearch) && !Columns.Any((DataLineageColumnsFlowRow c) => c != column && c.ProcessRow == columnProcess && (autofillInflows ? c.OutflowColumnName : c.InflowColumnName) == columnNameToSearch))
				{
					SetColumnProperties(column, source2.FirstOrDefault(), autofillInflows);
				}
			}
		}
		RefreshColumnsGrid();
		this.DataLineageEdited?.Invoke();
	}

	private void SetColumnProperties(DataLineageColumnsFlowRow columnRowToFill, ColumnForDataLineageColumnsDropdown outflowColumnRow, bool setInflowColumnProperties)
	{
		if (columnRowToFill != null && outflowColumnRow != null)
		{
			columnRowToFill.ProcessRow = outflowColumnRow.DataFlowRow.Process;
			if (setInflowColumnProperties)
			{
				columnRowToFill.InflowRow = outflowColumnRow.DataFlowRow;
				columnRowToFill.InflowColumnId = outflowColumnRow.Id;
				columnRowToFill.InflowColumnImage = outflowColumnRow.Icon;
				columnRowToFill.InflowColumnName = outflowColumnRow.Name;
			}
			else
			{
				columnRowToFill.OutflowRow = outflowColumnRow.DataFlowRow;
				columnRowToFill.OutflowColumnId = outflowColumnRow.Id;
				columnRowToFill.OutflowColumnImage = outflowColumnRow.Icon;
				columnRowToFill.OutflowColumnName = outflowColumnRow.Name;
			}
			if (columnRowToFill.DataColumnsFlowId >= 0)
			{
				columnRowToFill.RowState = ManagingRowsEnum.ManagingRows.Updated;
			}
			else
			{
				columnRowToFill.RowState = ManagingRowsEnum.ManagingRows.ForAdding;
			}
		}
	}

	private void ClearColumnProperties(DataLineageColumnsFlowRow columnRowToFill, bool setInflowColumnProperties)
	{
		if (setInflowColumnProperties)
		{
			columnRowToFill.InflowColumnId = -1;
			columnRowToFill.InflowColumnImage = Resources.blank_16;
			columnRowToFill.InflowColumnName = null;
			columnRowToFill.InflowRow = null;
		}
		else
		{
			columnRowToFill.OutflowColumnId = -1;
			columnRowToFill.OutflowColumnImage = Resources.blank_16;
			columnRowToFill.OutflowColumnName = null;
			columnRowToFill.OutflowRow = null;
		}
		if (columnRowToFill.DataColumnsFlowId >= 0)
		{
			columnRowToFill.RowState = ManagingRowsEnum.ManagingRows.Updated;
		}
		else
		{
			columnRowToFill.RowState = ManagingRowsEnum.ManagingRows.ForAdding;
		}
	}

	private void SetShowAllColumnsButtonFunctionality(bool setShowAllDestinations)
	{
		if (setShowAllDestinations)
		{
			showAllColumnsBarButtonItem.Caption = "Show all destinations";
			showAllColumnsBarButtonItem.ImageOptions.Image = Resources.show_destination_columns_16;
			showAllColumnsBarButtonItem.Enabled = outflowCount > 0;
		}
		else
		{
			showAllColumnsBarButtonItem.Caption = "Show all inputs";
			showAllColumnsBarButtonItem.ImageOptions.Image = Resources.show_input_columns_16;
			showAllColumnsBarButtonItem.Enabled = inflowsCount > 0;
		}
	}

	private void SetPopupFilter(GridLookUpEdit gridLookUpEdit)
	{
		if (gridLookUpEdit == null)
		{
			return;
		}
		CriteriaOperator criteriaOperator = ((!(gridLookUpEdit.Properties.View.Name == "outflowColumnRepositoryItemGridLookUpEditView")) ? CurrentInflowPopupCriteria : CurrentOutflowPopupCriteria);
		if ((object)criteriaOperator != null)
		{
			return;
		}
		if (columnsGridView.GetFocusedRow() is DataLineageColumnsFlowRow dataLineageColumnsFlowRow && !dataLineageColumnsFlowRow.IsRowEmpty && dataLineageColumnsFlowRow.ProcessRow != null)
		{
			gridLookUpEdit.Properties.View.ActiveFilterCriteria = new GroupOperator(GroupOperatorType.And, new NotOperator(new NullOperator("DataFlowRow.Process")), new BinaryOperator("DataFlowRow.Process.Guid", dataLineageColumnsFlowRow.ProcessRow.Guid));
		}
		else if (AllDataFlowsContainer != null)
		{
			IEnumerable<Guid> enumerable = AllDataFlowsContainer.ReferencedInflows.Select((DataFlowRow x) => x.Guid).Concat(AllDataFlowsContainer.ReferencedOutflows.Select((DataFlowRow x) => x.Guid));
			if (enumerable.Any())
			{
				gridLookUpEdit.Properties.View.ActiveFilterCriteria = new NotOperator(new InOperator("DataFlowRow.Guid", enumerable));
			}
		}
	}

	private bool ShouldPopupBeOpen(GridLookUpEdit gridLookUpEdit)
	{
		if (gridLookUpEdit == null || ControlsUtils.GetGridLookUpEditFilteredRowCount<ColumnForDataLineageColumnsDropdown>(gridLookUpEdit) == 0)
		{
			return false;
		}
		return true;
	}

	private bool AreInflowsDisplayed()
	{
		return showAllColumnsBarButtonItem.Caption == "Show all destinations";
	}

	private bool? IsInflowColumn(GridColumn column)
	{
		if (column == null)
		{
			return null;
		}
		if (column == outflowColumnGridColumn || column == outflowColumnIconGridColumn || column == outflowObjectGridColumn || column == outflowObjectIconGridColumn)
		{
			return false;
		}
		if (column == inflowColumnGridColumn || column == inflowColumnIconGridColumn || column == inflowObjectGridColumn || column == inflowObjectIconGridColumn)
		{
			return true;
		}
		return null;
	}

	private void SetDestinationButton(GridColumn column, int selectedRowsCount)
	{
		bool? flag = IsInflowColumn(column);
		duplicateBarButtonItem.Enabled = true;
		if (flag == true)
		{
			duplicateBarButtonItem.Caption = ((selectedRowsCount > 1) ? "Duplicate input columns" : "Duplicate input column");
			return;
		}
		if (flag == false)
		{
			duplicateBarButtonItem.Caption = ((selectedRowsCount > 1) ? "Duplicate destination columns" : "Duplicate destination column");
			return;
		}
		duplicateBarButtonItem.Caption = "Duplicate";
		duplicateBarButtonItem.Enabled = false;
	}

	private void SetClearButton(GridColumn column, int selectedRowsCount)
	{
		bool? flag = IsInflowColumn(column);
		if (flag == true)
		{
			clearBarButtonItem.Enabled = true;
			clearBarButtonItem.Caption = ((selectedRowsCount > 1) ? "Clear input columns" : "Clear input column");
		}
		else if (flag == false)
		{
			clearBarButtonItem.Enabled = true;
			clearBarButtonItem.Caption = ((selectedRowsCount > 1) ? "Clear destination columns" : "Clear destination column");
		}
		else
		{
			clearBarButtonItem.Caption = "Clear";
			clearBarButtonItem.Enabled = false;
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			if (keyData == Keys.Delete)
			{
				DeleteSelectedColumns();
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void DeleteSelectedColumns()
	{
		int[] selectedRows = columnsGridView.GetSelectedRows();
		foreach (int rowHandle in selectedRows)
		{
			if (columnsGridView.GetRow(rowHandle) is DataLineageColumnsFlowRow dataLineageColumnsFlowRow)
			{
				DeleteColumnRow(dataLineageColumnsFlowRow);
			}
		}
		if (AllDataFlowsContainer != null && CurrentProcessRow == null)
		{
			columnsGridControl.DataSource = Columns;
		}
		else
		{
			RefreshColumnsGrid();
		}
	}

	private void InflowColumnRepositoryItemGridLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		HandleColumnChange(sender, isInflowColumn: true);
	}

	private void OutflowColumnRepositoryItemGridLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		HandleColumnChange(sender, isInflowColumn: false);
	}

	private void InflowColumnRepositoryItemGridLookUpEdit_BeforePopup(object sender, EventArgs e)
	{
		SetBestColumnsWidth(sender);
	}

	private void OutflowColumnRepositoryItemGridLookUpEdit_BeforePopup(object sender, EventArgs e)
	{
		SetBestColumnsWidth(sender);
	}

	private void ColumnsGridControl_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Right || columnsGridView.CalcHitInfo(e.Location).HitTest != GridHitTest.RowCell)
		{
			return;
		}
		List<DataLineageColumnsFlowRow> list = new List<DataLineageColumnsFlowRow>();
		int[] selectedRows = columnsGridView.GetSelectedRows();
		foreach (int rowHandle in selectedRows)
		{
			if (columnsGridView.GetRow(rowHandle) is DataLineageColumnsFlowRow item)
			{
				list.Add(item);
			}
		}
		columnsGridControlPopupMenu.Tag = list;
		GridColumn column = columnsGridView.CalcHitInfo(e.Location).Column;
		SetClearButton(column, list.Count);
		SetDestinationButton(column, list.Count);
		columnsGridControlPopupMenu.ShowPopup(barManager, columnsGridView.GridControl.PointToScreen(e.Location));
	}

	private void ColumnsGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		if (columnsGridView.GetRow(e.FocusedRowHandle) is DataLineageColumnsFlowRow dataLineageColumnsFlowRow)
		{
			RefreshSelectedRows(dataLineageColumnsFlowRow);
		}
	}

	private void ColumnsGridView_RowStyle(object sender, RowStyleEventArgs e)
	{
		int[] array = selectedRowHandles;
		if (array != null && array.Contains(e.RowHandle))
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridHighlightRowBackColor;
		}
		else
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridGridRowBackColor;
		}
	}

	private void DeleteColumnBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (!(columnsGridControlPopupMenu.Tag is List<DataLineageColumnsFlowRow> list))
		{
			return;
		}
		foreach (DataLineageColumnsFlowRow item in list)
		{
			DeleteColumnRow(item);
		}
		if (AllDataFlowsContainer != null && CurrentProcessRow == null)
		{
			columnsGridControl.DataSource = Columns;
		}
		else
		{
			RefreshColumnsGrid();
		}
	}

	private void DuplicateBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (!(columnsGridControlPopupMenu.Tag is List<DataLineageColumnsFlowRow> list))
		{
			return;
		}
		bool? flag = IsInflowColumn(columnsGridView.FocusedColumn);
		if (!flag.HasValue)
		{
			return;
		}
		foreach (DataLineageColumnsFlowRow item in list)
		{
			DuplicateRow(item, flag.Value);
		}
	}

	private void AutofillRowsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AutofillColumns();
	}

	private void ColumnsGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (!(columnsGridView.GetRow(e.RowHandle) is DataLineageColumnsFlowRow columnRowToFill))
		{
			return;
		}
		object value = e.Value;
		if (value is int && (int)value == 0)
		{
			if (e.Column == outflowColumnGridColumn)
			{
				ClearColumnProperties(columnRowToFill, setInflowColumnProperties: false);
			}
			else if (e.Column == inflowColumnGridColumn)
			{
				ClearColumnProperties(columnRowToFill, setInflowColumnProperties: true);
			}
		}
	}

	private void ClearBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (!(columnsGridControlPopupMenu.Tag is List<DataLineageColumnsFlowRow> list))
		{
			return;
		}
		bool? flag = IsInflowColumn(columnsGridView.FocusedColumn);
		if (flag == false)
		{
			foreach (DataLineageColumnsFlowRow item in list)
			{
				ClearColumnProperties(item, setInflowColumnProperties: false);
			}
		}
		else if (flag == true)
		{
			foreach (DataLineageColumnsFlowRow item2 in list)
			{
				ClearColumnProperties(item2, setInflowColumnProperties: true);
			}
		}
		RefreshColumnsGrid();
		this.DataLineageEdited?.Invoke();
	}

	private void ShowAllColumnsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (AreInflowsDisplayed())
		{
			LoadAllColumns(FlowDirectionEnum.Direction.OUT);
			SetShowAllColumnsButtonFunctionality(setShowAllDestinations: false);
		}
		else
		{
			LoadAllColumns(FlowDirectionEnum.Direction.IN);
			SetShowAllColumnsButtonFunctionality(setShowAllDestinations: true);
		}
		this.DataLineageEdited?.Invoke();
	}

	private void inflowColumnRepositoryItemGridLookUpEdit_QueryPopUp(object sender, CancelEventArgs e)
	{
		SetPopupFilter(sender as GridLookUpEdit);
		e.Cancel = !ShouldPopupBeOpen(sender as GridLookUpEdit);
	}

	private void outflowColumnRepositoryItemGridLookUpEdit_QueryPopUp(object sender, CancelEventArgs e)
	{
		SetPopupFilter(sender as GridLookUpEdit);
		e.Cancel = !ShouldPopupBeOpen(sender as GridLookUpEdit);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.DataLineage.DataLineageColumnsUserControl));
		this.nonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.columnsGridControl = new DevExpress.XtraGrid.GridControl();
		this.columnsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.inflowColumnIconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.inflowColumnRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.inflowColumnGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.inflowColumnRepositoryItemGridLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
		this.inflowColumnRepositoryItemGridLookUpEditView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.inflowColumnIconPopupGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.inflowColumnNamePopupRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.inflowColumnNamePopupGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.inflowColumnNamePopupRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.inflowObjectIconPopupGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.inflowObjectIconRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.inflowObjectNamePopupGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.inflowObjectIconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.inflowObjectRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.inflowObjectGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.arrowGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.arrowRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.outflowColumnIconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.outflowColumnRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.outflowColumnGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.outflowColumnRepositoryItemGridLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
		this.outflowColumnRepositoryItemGridLookUpEditView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.outflowColumnIconPopupGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.outflowColumnImagePopupRepositoryItemRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.outflowColumnNamePopupGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.outflowColumnNamePopupRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.outflowObjectIconPopupGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.outflowObjectIconRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.outflowObjectNamePopupGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.outflowObjectIconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.outflowObjectRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.outflowObjectGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.autofillRowsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.duplicateBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.deleteColumnBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.clearBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.showAllColumnsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.mainLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.columnsGridControlPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.bar1 = new DevExpress.XtraBars.Bar();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.columnsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.inflowColumnRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.inflowColumnRepositoryItemGridLookUpEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.inflowColumnRepositoryItemGridLookUpEditView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.inflowColumnNamePopupRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.inflowColumnNamePopupRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.inflowObjectIconRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.inflowObjectRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.arrowRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.outflowColumnRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.outflowColumnRepositoryItemGridLookUpEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.outflowColumnRepositoryItemGridLookUpEditView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.outflowColumnImagePopupRepositoryItemRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.outflowColumnNamePopupRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.outflowObjectIconRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.outflowObjectRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnsGridControlPopupMenu).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.columnsGridControl);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(788, 462);
		this.nonCustomizableLayoutControl.TabIndex = 0;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.columnsGridControl.Location = new System.Drawing.Point(2, 2);
		this.columnsGridControl.MainView = this.columnsGridView;
		this.columnsGridControl.MenuManager = this.barManager;
		this.columnsGridControl.Name = "columnsGridControl";
		this.columnsGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.inflowColumnRepositoryItemGridLookUpEdit, this.outflowColumnRepositoryItemGridLookUpEdit, this.inflowColumnRepositoryItemPictureEdit, this.outflowColumnRepositoryItemPictureEdit, this.inflowObjectRepositoryItemPictureEdit, this.outflowObjectRepositoryItemPictureEdit, this.arrowRepositoryItemPictureEdit });
		this.columnsGridControl.Size = new System.Drawing.Size(784, 458);
		this.columnsGridControl.TabIndex = 4;
		this.columnsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.columnsGridView });
		this.columnsGridControl.MouseClick += new System.Windows.Forms.MouseEventHandler(ColumnsGridControl_MouseClick);
		this.columnsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.inflowColumnIconGridColumn, this.inflowColumnGridColumn, this.inflowObjectIconGridColumn, this.inflowObjectGridColumn, this.arrowGridColumn, this.outflowColumnIconGridColumn, this.outflowColumnGridColumn, this.outflowObjectIconGridColumn, this.outflowObjectGridColumn });
		this.columnsGridView.GridControl = this.columnsGridControl;
		this.columnsGridView.Name = "columnsGridView";
		this.columnsGridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
		this.columnsGridView.OptionsBehavior.AutoPopulateColumns = false;
		this.columnsGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
		this.columnsGridView.OptionsBehavior.FocusLeaveOnTab = true;
		this.columnsGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.columnsGridView.OptionsCustomization.AllowColumnMoving = false;
		this.columnsGridView.OptionsCustomization.AllowFilter = false;
		this.columnsGridView.OptionsCustomization.AllowQuickHideColumns = false;
		this.columnsGridView.OptionsCustomization.AllowSort = false;
		this.columnsGridView.OptionsDetail.EnableMasterViewMode = false;
		this.columnsGridView.OptionsSelection.MultiSelect = true;
		this.columnsGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.columnsGridView.OptionsView.ShowGroupPanel = false;
		this.columnsGridView.OptionsView.ShowIndicator = false;
		this.columnsGridView.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(ColumnsGridView_RowStyle);
		this.columnsGridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(ColumnsGridView_FocusedRowChanged);
		this.columnsGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(ColumnsGridView_CellValueChanged);
		this.inflowColumnIconGridColumn.ColumnEdit = this.inflowColumnRepositoryItemPictureEdit;
		this.inflowColumnIconGridColumn.FieldName = "InflowColumnImage";
		this.inflowColumnIconGridColumn.ImageOptions.Image = Dataedo.App.Properties.Resources.blank_16;
		this.inflowColumnIconGridColumn.MaxWidth = 25;
		this.inflowColumnIconGridColumn.MinWidth = 25;
		this.inflowColumnIconGridColumn.Name = "inflowColumnIconGridColumn";
		this.inflowColumnIconGridColumn.OptionsColumn.AllowFocus = false;
		this.inflowColumnIconGridColumn.OptionsColumn.AllowShowHide = false;
		this.inflowColumnIconGridColumn.OptionsColumn.ReadOnly = true;
		this.inflowColumnIconGridColumn.OptionsColumn.ShowCaption = false;
		this.inflowColumnIconGridColumn.Visible = true;
		this.inflowColumnIconGridColumn.VisibleIndex = 0;
		this.inflowColumnIconGridColumn.Width = 25;
		this.inflowColumnRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.inflowColumnRepositoryItemPictureEdit.AllowScrollViaMouseDrag = true;
		this.inflowColumnRepositoryItemPictureEdit.Name = "inflowColumnRepositoryItemPictureEdit";
		this.inflowColumnRepositoryItemPictureEdit.NullText = " ";
		this.inflowColumnRepositoryItemPictureEdit.ReadOnly = true;
		this.inflowColumnRepositoryItemPictureEdit.ShowMenu = false;
		this.inflowColumnGridColumn.Caption = "Input column";
		this.inflowColumnGridColumn.ColumnEdit = this.inflowColumnRepositoryItemGridLookUpEdit;
		this.inflowColumnGridColumn.FieldName = "InflowColumnId";
		this.inflowColumnGridColumn.Name = "inflowColumnGridColumn";
		this.inflowColumnGridColumn.OptionsColumn.AllowShowHide = false;
		this.inflowColumnGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.inflowColumnGridColumn.Visible = true;
		this.inflowColumnGridColumn.VisibleIndex = 1;
		this.inflowColumnGridColumn.Width = 329;
		this.inflowColumnRepositoryItemGridLookUpEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.inflowColumnRepositoryItemGridLookUpEdit.AutoHeight = false;
		this.inflowColumnRepositoryItemGridLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.inflowColumnRepositoryItemGridLookUpEdit.DisplayMember = "FullNameWithTitle";
		this.inflowColumnRepositoryItemGridLookUpEdit.Name = "inflowColumnRepositoryItemGridLookUpEdit";
		this.inflowColumnRepositoryItemGridLookUpEdit.NullText = "";
		this.inflowColumnRepositoryItemGridLookUpEdit.PopupFormMinSize = new System.Drawing.Size(400, 0);
		this.inflowColumnRepositoryItemGridLookUpEdit.PopupFormSize = new System.Drawing.Size(400, 0);
		this.inflowColumnRepositoryItemGridLookUpEdit.PopupView = this.inflowColumnRepositoryItemGridLookUpEditView;
		this.inflowColumnRepositoryItemGridLookUpEdit.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.inflowColumnNamePopupRepositoryItemPictureEdit, this.inflowColumnNamePopupRepositoryItemCustomTextEdit, this.inflowObjectIconRepositoryItemPictureEdit });
		this.inflowColumnRepositoryItemGridLookUpEdit.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.None;
		this.inflowColumnRepositoryItemGridLookUpEdit.ShowFooter = false;
		this.inflowColumnRepositoryItemGridLookUpEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.inflowColumnRepositoryItemGridLookUpEdit.ValueMember = "Id";
		this.inflowColumnRepositoryItemGridLookUpEdit.QueryPopUp += new System.ComponentModel.CancelEventHandler(inflowColumnRepositoryItemGridLookUpEdit_QueryPopUp);
		this.inflowColumnRepositoryItemGridLookUpEdit.BeforePopup += new System.EventHandler(InflowColumnRepositoryItemGridLookUpEdit_BeforePopup);
		this.inflowColumnRepositoryItemGridLookUpEdit.EditValueChanged += new System.EventHandler(InflowColumnRepositoryItemGridLookUpEdit_EditValueChanged);
		this.inflowColumnRepositoryItemGridLookUpEditView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.inflowColumnIconPopupGridColumn, this.inflowColumnNamePopupGridColumn, this.inflowObjectIconPopupGridColumn, this.inflowObjectNamePopupGridColumn });
		this.inflowColumnRepositoryItemGridLookUpEditView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.inflowColumnRepositoryItemGridLookUpEditView.Name = "inflowColumnRepositoryItemGridLookUpEditView";
		this.inflowColumnRepositoryItemGridLookUpEditView.OptionsBehavior.AutoPopulateColumns = false;
		this.inflowColumnRepositoryItemGridLookUpEditView.OptionsBehavior.Editable = false;
		this.inflowColumnRepositoryItemGridLookUpEditView.OptionsBehavior.ReadOnly = true;
		this.inflowColumnRepositoryItemGridLookUpEditView.OptionsCustomization.AllowColumnMoving = false;
		this.inflowColumnRepositoryItemGridLookUpEditView.OptionsCustomization.AllowColumnResizing = false;
		this.inflowColumnRepositoryItemGridLookUpEditView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.inflowColumnRepositoryItemGridLookUpEditView.OptionsView.ShowColumnHeaders = false;
		this.inflowColumnRepositoryItemGridLookUpEditView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.inflowColumnRepositoryItemGridLookUpEditView.OptionsView.ShowGroupPanel = false;
		this.inflowColumnRepositoryItemGridLookUpEditView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.inflowColumnRepositoryItemGridLookUpEditView.OptionsView.ShowIndicator = false;
		this.inflowColumnRepositoryItemGridLookUpEditView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.inflowColumnIconPopupGridColumn.ColumnEdit = this.inflowColumnNamePopupRepositoryItemPictureEdit;
		this.inflowColumnIconPopupGridColumn.FieldName = "Icon";
		this.inflowColumnIconPopupGridColumn.MaxWidth = 20;
		this.inflowColumnIconPopupGridColumn.Name = "inflowColumnIconPopupGridColumn";
		this.inflowColumnIconPopupGridColumn.Visible = true;
		this.inflowColumnIconPopupGridColumn.VisibleIndex = 0;
		this.inflowColumnIconPopupGridColumn.Width = 20;
		this.inflowColumnNamePopupRepositoryItemPictureEdit.Name = "inflowColumnNamePopupRepositoryItemPictureEdit";
		this.inflowColumnNamePopupGridColumn.ColumnEdit = this.inflowColumnNamePopupRepositoryItemCustomTextEdit;
		this.inflowColumnNamePopupGridColumn.FieldName = "FullNameFormattedWithTitle";
		this.inflowColumnNamePopupGridColumn.Name = "inflowColumnNamePopupGridColumn";
		this.inflowColumnNamePopupGridColumn.Visible = true;
		this.inflowColumnNamePopupGridColumn.VisibleIndex = 1;
		this.inflowColumnNamePopupGridColumn.Width = 310;
		this.inflowColumnNamePopupRepositoryItemCustomTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.inflowColumnNamePopupRepositoryItemCustomTextEdit.AutoHeight = false;
		this.inflowColumnNamePopupRepositoryItemCustomTextEdit.Name = "inflowColumnNamePopupRepositoryItemCustomTextEdit";
		this.inflowObjectIconPopupGridColumn.Caption = "Object Icon";
		this.inflowObjectIconPopupGridColumn.ColumnEdit = this.inflowObjectIconRepositoryItemPictureEdit;
		this.inflowObjectIconPopupGridColumn.FieldName = "ParentObjectIcon";
		this.inflowObjectIconPopupGridColumn.MaxWidth = 20;
		this.inflowObjectIconPopupGridColumn.Name = "inflowObjectIconPopupGridColumn";
		this.inflowObjectIconPopupGridColumn.OptionsColumn.ReadOnly = true;
		this.inflowObjectIconPopupGridColumn.OptionsColumn.ShowCaption = false;
		this.inflowObjectIconPopupGridColumn.Visible = true;
		this.inflowObjectIconPopupGridColumn.VisibleIndex = 2;
		this.inflowObjectIconPopupGridColumn.Width = 20;
		this.inflowObjectIconRepositoryItemPictureEdit.Name = "inflowObjectIconRepositoryItemPictureEdit";
		this.inflowObjectNamePopupGridColumn.Caption = "Object Name";
		this.inflowObjectNamePopupGridColumn.FieldName = "ParentObjectFullName";
		this.inflowObjectNamePopupGridColumn.Name = "inflowObjectNamePopupGridColumn";
		this.inflowObjectNamePopupGridColumn.OptionsColumn.ReadOnly = true;
		this.inflowObjectNamePopupGridColumn.OptionsColumn.ShowCaption = false;
		this.inflowObjectNamePopupGridColumn.Visible = true;
		this.inflowObjectNamePopupGridColumn.VisibleIndex = 3;
		this.inflowObjectNamePopupGridColumn.Width = 54;
		this.inflowObjectIconGridColumn.Caption = "Object";
		this.inflowObjectIconGridColumn.ColumnEdit = this.inflowObjectRepositoryItemPictureEdit;
		this.inflowObjectIconGridColumn.FieldName = "InflowObjectImage";
		this.inflowObjectIconGridColumn.MaxWidth = 25;
		this.inflowObjectIconGridColumn.MinWidth = 25;
		this.inflowObjectIconGridColumn.Name = "inflowObjectIconGridColumn";
		this.inflowObjectIconGridColumn.OptionsColumn.AllowFocus = false;
		this.inflowObjectIconGridColumn.OptionsColumn.AllowShowHide = false;
		this.inflowObjectIconGridColumn.OptionsColumn.ReadOnly = true;
		this.inflowObjectIconGridColumn.OptionsColumn.ShowCaption = false;
		this.inflowObjectIconGridColumn.Visible = true;
		this.inflowObjectIconGridColumn.VisibleIndex = 2;
		this.inflowObjectIconGridColumn.Width = 25;
		this.inflowObjectRepositoryItemPictureEdit.AllowScrollViaMouseDrag = true;
		this.inflowObjectRepositoryItemPictureEdit.Name = "inflowObjectRepositoryItemPictureEdit";
		this.inflowObjectRepositoryItemPictureEdit.NullText = " ";
		this.inflowObjectRepositoryItemPictureEdit.ReadOnly = true;
		this.inflowObjectRepositoryItemPictureEdit.ShowMenu = false;
		this.inflowObjectGridColumn.Caption = "Object";
		this.inflowObjectGridColumn.FieldName = "InflowObjectName";
		this.inflowObjectGridColumn.Name = "inflowObjectGridColumn";
		this.inflowObjectGridColumn.OptionsColumn.AllowEdit = false;
		this.inflowObjectGridColumn.OptionsColumn.AllowShowHide = false;
		this.inflowObjectGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.inflowObjectGridColumn.OptionsColumn.ReadOnly = true;
		this.inflowObjectGridColumn.Visible = true;
		this.inflowObjectGridColumn.VisibleIndex = 3;
		this.inflowObjectGridColumn.Width = 361;
		this.arrowGridColumn.Caption = "Arrow Grid Column";
		this.arrowGridColumn.ColumnEdit = this.arrowRepositoryItemPictureEdit;
		this.arrowGridColumn.FieldName = "ConnectionIcon";
		this.arrowGridColumn.MaxWidth = 25;
		this.arrowGridColumn.MinWidth = 25;
		this.arrowGridColumn.Name = "arrowGridColumn";
		this.arrowGridColumn.OptionsColumn.AllowFocus = false;
		this.arrowGridColumn.OptionsColumn.AllowShowHide = false;
		this.arrowGridColumn.OptionsColumn.ReadOnly = true;
		this.arrowGridColumn.OptionsColumn.ShowCaption = false;
		this.arrowGridColumn.Visible = true;
		this.arrowGridColumn.VisibleIndex = 4;
		this.arrowGridColumn.Width = 25;
		this.arrowRepositoryItemPictureEdit.Name = "arrowRepositoryItemPictureEdit";
		this.arrowRepositoryItemPictureEdit.NullText = " ";
		this.arrowRepositoryItemPictureEdit.ReadOnly = true;
		this.arrowRepositoryItemPictureEdit.ShowMenu = false;
		this.outflowColumnIconGridColumn.ColumnEdit = this.outflowColumnRepositoryItemPictureEdit;
		this.outflowColumnIconGridColumn.FieldName = "OutflowColumnImage";
		this.outflowColumnIconGridColumn.MaxWidth = 25;
		this.outflowColumnIconGridColumn.MinWidth = 25;
		this.outflowColumnIconGridColumn.Name = "outflowColumnIconGridColumn";
		this.outflowColumnIconGridColumn.OptionsColumn.AllowFocus = false;
		this.outflowColumnIconGridColumn.OptionsColumn.AllowShowHide = false;
		this.outflowColumnIconGridColumn.OptionsColumn.ReadOnly = true;
		this.outflowColumnIconGridColumn.OptionsColumn.ShowCaption = false;
		this.outflowColumnIconGridColumn.Visible = true;
		this.outflowColumnIconGridColumn.VisibleIndex = 5;
		this.outflowColumnIconGridColumn.Width = 25;
		this.outflowColumnRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.outflowColumnRepositoryItemPictureEdit.AllowScrollViaMouseDrag = true;
		this.outflowColumnRepositoryItemPictureEdit.Name = "outflowColumnRepositoryItemPictureEdit";
		this.outflowColumnRepositoryItemPictureEdit.NullText = " ";
		this.outflowColumnRepositoryItemPictureEdit.ReadOnly = true;
		this.outflowColumnRepositoryItemPictureEdit.ShowMenu = false;
		this.outflowColumnGridColumn.Caption = "Destination column";
		this.outflowColumnGridColumn.ColumnEdit = this.outflowColumnRepositoryItemGridLookUpEdit;
		this.outflowColumnGridColumn.FieldName = "OutflowColumnId";
		this.outflowColumnGridColumn.Name = "outflowColumnGridColumn";
		this.outflowColumnGridColumn.OptionsColumn.AllowShowHide = false;
		this.outflowColumnGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.outflowColumnGridColumn.Visible = true;
		this.outflowColumnGridColumn.VisibleIndex = 6;
		this.outflowColumnGridColumn.Width = 426;
		this.outflowColumnRepositoryItemGridLookUpEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.outflowColumnRepositoryItemGridLookUpEdit.AutoHeight = false;
		this.outflowColumnRepositoryItemGridLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.outflowColumnRepositoryItemGridLookUpEdit.DisplayMember = "FullNameFormattedWithTitle";
		this.outflowColumnRepositoryItemGridLookUpEdit.Name = "outflowColumnRepositoryItemGridLookUpEdit";
		this.outflowColumnRepositoryItemGridLookUpEdit.NullText = "";
		this.outflowColumnRepositoryItemGridLookUpEdit.PopupFormMinSize = new System.Drawing.Size(400, 0);
		this.outflowColumnRepositoryItemGridLookUpEdit.PopupFormSize = new System.Drawing.Size(400, 0);
		this.outflowColumnRepositoryItemGridLookUpEdit.PopupView = this.outflowColumnRepositoryItemGridLookUpEditView;
		this.outflowColumnRepositoryItemGridLookUpEdit.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.outflowColumnImagePopupRepositoryItemRepositoryItemPictureEdit, this.outflowColumnNamePopupRepositoryItemCustomTextEdit, this.outflowObjectIconRepositoryItemPictureEdit });
		this.outflowColumnRepositoryItemGridLookUpEdit.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.None;
		this.outflowColumnRepositoryItemGridLookUpEdit.ShowFooter = false;
		this.outflowColumnRepositoryItemGridLookUpEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.outflowColumnRepositoryItemGridLookUpEdit.ValueMember = "Id";
		this.outflowColumnRepositoryItemGridLookUpEdit.QueryPopUp += new System.ComponentModel.CancelEventHandler(outflowColumnRepositoryItemGridLookUpEdit_QueryPopUp);
		this.outflowColumnRepositoryItemGridLookUpEdit.BeforePopup += new System.EventHandler(OutflowColumnRepositoryItemGridLookUpEdit_BeforePopup);
		this.outflowColumnRepositoryItemGridLookUpEdit.EditValueChanged += new System.EventHandler(OutflowColumnRepositoryItemGridLookUpEdit_EditValueChanged);
		this.outflowColumnRepositoryItemGridLookUpEditView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.outflowColumnIconPopupGridColumn, this.outflowColumnNamePopupGridColumn, this.outflowObjectIconPopupGridColumn, this.outflowObjectNamePopupGridColumn });
		this.outflowColumnRepositoryItemGridLookUpEditView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.outflowColumnRepositoryItemGridLookUpEditView.Name = "outflowColumnRepositoryItemGridLookUpEditView";
		this.outflowColumnRepositoryItemGridLookUpEditView.OptionsBehavior.AutoPopulateColumns = false;
		this.outflowColumnRepositoryItemGridLookUpEditView.OptionsBehavior.Editable = false;
		this.outflowColumnRepositoryItemGridLookUpEditView.OptionsBehavior.ReadOnly = true;
		this.outflowColumnRepositoryItemGridLookUpEditView.OptionsCustomization.AllowColumnMoving = false;
		this.outflowColumnRepositoryItemGridLookUpEditView.OptionsCustomization.AllowColumnResizing = false;
		this.outflowColumnRepositoryItemGridLookUpEditView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.outflowColumnRepositoryItemGridLookUpEditView.OptionsView.ShowColumnHeaders = false;
		this.outflowColumnRepositoryItemGridLookUpEditView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.outflowColumnRepositoryItemGridLookUpEditView.OptionsView.ShowGroupPanel = false;
		this.outflowColumnRepositoryItemGridLookUpEditView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.outflowColumnRepositoryItemGridLookUpEditView.OptionsView.ShowIndicator = false;
		this.outflowColumnRepositoryItemGridLookUpEditView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.outflowColumnIconPopupGridColumn.ColumnEdit = this.outflowColumnImagePopupRepositoryItemRepositoryItemPictureEdit;
		this.outflowColumnIconPopupGridColumn.FieldName = "Icon";
		this.outflowColumnIconPopupGridColumn.MaxWidth = 20;
		this.outflowColumnIconPopupGridColumn.Name = "outflowColumnIconPopupGridColumn";
		this.outflowColumnIconPopupGridColumn.Visible = true;
		this.outflowColumnIconPopupGridColumn.VisibleIndex = 0;
		this.outflowColumnIconPopupGridColumn.Width = 20;
		this.outflowColumnImagePopupRepositoryItemRepositoryItemPictureEdit.Name = "outflowColumnImagePopupRepositoryItemRepositoryItemPictureEdit";
		this.outflowColumnNamePopupGridColumn.ColumnEdit = this.outflowColumnNamePopupRepositoryItemCustomTextEdit;
		this.outflowColumnNamePopupGridColumn.FieldName = "FullNameFormattedWithTitle";
		this.outflowColumnNamePopupGridColumn.Name = "outflowColumnNamePopupGridColumn";
		this.outflowColumnNamePopupGridColumn.Visible = true;
		this.outflowColumnNamePopupGridColumn.VisibleIndex = 1;
		this.outflowColumnNamePopupGridColumn.Width = 310;
		this.outflowColumnNamePopupRepositoryItemCustomTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.outflowColumnNamePopupRepositoryItemCustomTextEdit.AutoHeight = false;
		this.outflowColumnNamePopupRepositoryItemCustomTextEdit.Name = "outflowColumnNamePopupRepositoryItemCustomTextEdit";
		this.outflowObjectIconPopupGridColumn.Caption = "Object Icon";
		this.outflowObjectIconPopupGridColumn.ColumnEdit = this.outflowObjectIconRepositoryItemPictureEdit;
		this.outflowObjectIconPopupGridColumn.FieldName = "ParentObjectIcon";
		this.outflowObjectIconPopupGridColumn.MaxWidth = 20;
		this.outflowObjectIconPopupGridColumn.Name = "outflowObjectIconPopupGridColumn";
		this.outflowObjectIconPopupGridColumn.OptionsColumn.ReadOnly = true;
		this.outflowObjectIconPopupGridColumn.OptionsColumn.ShowCaption = false;
		this.outflowObjectIconPopupGridColumn.Visible = true;
		this.outflowObjectIconPopupGridColumn.VisibleIndex = 2;
		this.outflowObjectIconPopupGridColumn.Width = 20;
		this.outflowObjectIconRepositoryItemPictureEdit.Name = "outflowObjectIconRepositoryItemPictureEdit";
		this.outflowObjectNamePopupGridColumn.Caption = "Object Name";
		this.outflowObjectNamePopupGridColumn.FieldName = "ParentObjectFullName";
		this.outflowObjectNamePopupGridColumn.Name = "outflowObjectNamePopupGridColumn";
		this.outflowObjectNamePopupGridColumn.OptionsColumn.ReadOnly = true;
		this.outflowObjectNamePopupGridColumn.OptionsColumn.ShowCaption = false;
		this.outflowObjectNamePopupGridColumn.Visible = true;
		this.outflowObjectNamePopupGridColumn.VisibleIndex = 3;
		this.outflowObjectNamePopupGridColumn.Width = 58;
		this.outflowObjectIconGridColumn.Caption = "Outflow Object Icon";
		this.outflowObjectIconGridColumn.ColumnEdit = this.outflowObjectRepositoryItemPictureEdit;
		this.outflowObjectIconGridColumn.FieldName = "OutflowObjectImage";
		this.outflowObjectIconGridColumn.MaxWidth = 25;
		this.outflowObjectIconGridColumn.MinWidth = 25;
		this.outflowObjectIconGridColumn.Name = "outflowObjectIconGridColumn";
		this.outflowObjectIconGridColumn.OptionsColumn.AllowFocus = false;
		this.outflowObjectIconGridColumn.OptionsColumn.AllowShowHide = false;
		this.outflowObjectIconGridColumn.OptionsColumn.ReadOnly = true;
		this.outflowObjectIconGridColumn.OptionsColumn.ShowCaption = false;
		this.outflowObjectIconGridColumn.Visible = true;
		this.outflowObjectIconGridColumn.VisibleIndex = 7;
		this.outflowObjectIconGridColumn.Width = 25;
		this.outflowObjectRepositoryItemPictureEdit.AllowScrollViaMouseDrag = true;
		this.outflowObjectRepositoryItemPictureEdit.Name = "outflowObjectRepositoryItemPictureEdit";
		this.outflowObjectRepositoryItemPictureEdit.NullText = " ";
		this.outflowObjectRepositoryItemPictureEdit.ReadOnly = true;
		this.outflowObjectRepositoryItemPictureEdit.ShowMenu = false;
		this.outflowObjectGridColumn.Caption = "Object";
		this.outflowObjectGridColumn.FieldName = "OutflowObjectName";
		this.outflowObjectGridColumn.Name = "outflowObjectGridColumn";
		this.outflowObjectGridColumn.OptionsColumn.AllowEdit = false;
		this.outflowObjectGridColumn.OptionsColumn.AllowShowHide = false;
		this.outflowObjectGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.outflowObjectGridColumn.OptionsColumn.ReadOnly = true;
		this.outflowObjectGridColumn.Visible = true;
		this.outflowObjectGridColumn.VisibleIndex = 8;
		this.outflowObjectGridColumn.Width = 371;
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[5] { this.autofillRowsBarButtonItem, this.duplicateBarButtonItem, this.deleteColumnBarButtonItem, this.clearBarButtonItem, this.showAllColumnsBarButtonItem });
		this.barManager.MaxItemId = 7;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(788, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 462);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(788, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 462);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(788, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 462);
		this.autofillRowsBarButtonItem.Caption = "Autofill rows";
		this.autofillRowsBarButtonItem.Id = 4;
		this.autofillRowsBarButtonItem.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("autofillRowsBarButtonItem.ImageOptions.Image");
		this.autofillRowsBarButtonItem.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("autofillRowsBarButtonItem.ImageOptions.LargeImage");
		this.autofillRowsBarButtonItem.Name = "autofillRowsBarButtonItem";
		this.autofillRowsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(AutofillRowsBarButtonItem_ItemClick);
		this.duplicateBarButtonItem.Caption = "Duplicate";
		this.duplicateBarButtonItem.Id = 3;
		this.duplicateBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.copy_16_alt;
		this.duplicateBarButtonItem.Name = "duplicateBarButtonItem";
		this.duplicateBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(DuplicateBarButtonItem_ItemClick);
		this.deleteColumnBarButtonItem.Caption = "Delete row";
		this.deleteColumnBarButtonItem.Id = 2;
		this.deleteColumnBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.deleteColumnBarButtonItem.Name = "deleteColumnBarButtonItem";
		this.deleteColumnBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(DeleteColumnBarButtonItem_ItemClick);
		this.clearBarButtonItem.Caption = "Clear";
		this.clearBarButtonItem.Id = 5;
		this.clearBarButtonItem.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("clearBarButtonItem.ImageOptions.Image");
		this.clearBarButtonItem.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("clearBarButtonItem.ImageOptions.LargeImage");
		this.clearBarButtonItem.Name = "clearBarButtonItem";
		this.clearBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ClearBarButtonItem_ItemClick);
		this.showAllColumnsBarButtonItem.Caption = "Show all destinations";
		this.showAllColumnsBarButtonItem.Id = 6;
		this.showAllColumnsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.show_destination_columns_16;
		this.showAllColumnsBarButtonItem.Name = "showAllColumnsBarButtonItem";
		this.showAllColumnsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ShowAllColumnsBarButtonItem_ItemClick);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.mainLayoutControlItem });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(788, 462);
		this.Root.TextVisible = false;
		this.mainLayoutControlItem.Control = this.columnsGridControl;
		this.mainLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControlItem.Name = "mainLayoutControlItem";
		this.mainLayoutControlItem.Size = new System.Drawing.Size(788, 462);
		this.mainLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.mainLayoutControlItem.TextVisible = false;
		this.columnsGridControlPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.autofillRowsBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.showAllColumnsBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.duplicateBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.clearBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.deleteColumnBarButtonItem)
		});
		this.columnsGridControlPopupMenu.Manager = this.barManager;
		this.columnsGridControlPopupMenu.Name = "columnsGridControlPopupMenu";
		this.bar1.BarName = "Custom 2";
		this.bar1.DockCol = 0;
		this.bar1.DockRow = 0;
		this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.bar1.Text = "Custom 2";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "DataLineageColumnsUserControl";
		base.Size = new System.Drawing.Size(788, 462);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.columnsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.inflowColumnRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.inflowColumnRepositoryItemGridLookUpEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.inflowColumnRepositoryItemGridLookUpEditView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.inflowColumnNamePopupRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.inflowColumnNamePopupRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.inflowObjectIconRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.inflowObjectRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.arrowRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.outflowColumnRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.outflowColumnRepositoryItemGridLookUpEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.outflowColumnRepositoryItemGridLookUpEditView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.outflowColumnImagePopupRepositoryItemRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.outflowColumnNamePopupRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.outflowObjectIconRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.outflowObjectRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnsGridControlPopupMenu).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
