using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.UI;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.UserControls.PanelControls;

public class ParameterRowUserControl : UserControl
{
	private ColumnBulkHelper bulkHelper;

	private CustomFieldsSupport customFieldsSupport;

	private CustomFieldsCellsTypesSupport customFieldsCellsTypesSupport;

	private ProcedureDesigner procedureDesigner;

	private Action setRemoveButtonAvailability;

	private IContainer components;

	private GridControl gridControl1;

	private GridView parameterGridView;

	private GridColumn iconProcedureParametersGridColumn;

	private GridColumn ordinalPositionParameterGridColumn;

	private GridColumn nameParameterGridColumn;

	private GridColumn parameterModeGridColumn;

	private GridColumn datatypeGridColumn;

	private GridColumn descriptionParameterGridColumn;

	private RepositoryItemPictureEdit iconFunctionParamterRepositoryItemPictureEdit;

	private GridColumn dataLengthGridColumn;

	private RepositoryItemTextEdit nameRepositoryItemTextEdit;

	private RepositoryItemTextEdit dataTypeRepositoryItemTextEdit;

	private RepositoryItemTextEdit sizeRepositoryItemTextEdit;

	private SharedObjectTypeEnum.ObjectType objectType { get; set; }

	public ParameterRow FocusedRow => parameterGridView.GetFocusedRow() as ParameterRow;

	public event Action SetRemoveButtonAvailability
	{
		add
		{
			if (setRemoveButtonAvailability == null || !setRemoveButtonAvailability.GetInvocationList().Contains(value))
			{
				setRemoveButtonAvailability = (Action)Delegate.Combine(setRemoveButtonAvailability, value);
			}
		}
		remove
		{
			setRemoveButtonAvailability = (Action)Delegate.Remove(setRemoveButtonAvailability, value);
		}
	}

	public ParameterRowUserControl()
	{
		InitializeComponent();
		customFieldsCellsTypesSupport = new CustomFieldsCellsTypesSupport(isForSummaryTable: false);
		parameterGridView.FocusedRowChanged += ParameterGridView_FocusedRowChanged;
		parameterGridView.ShownEditor += ParameterGrid_ShownEditor;
		parameterGridView.CellValueChanged += ParameterGridView_CellValueChanged;
		parameterGridView.CustomDrawCell += ParameterGridView_CustomDrawCell;
		parameterGridView.ShowingEditor += ParameterGridView_ShowingEditor;
	}

	private void ParameterGridView_ShowingEditor(object sender, CancelEventArgs e)
	{
		if (ColumnsHelper.IsCellBlocked(parameterGridView.GetFocusedRow() as ParameterRow, parameterGridView.FocusedColumn.FieldName))
		{
			e.Cancel = true;
		}
	}

	private void ParameterGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		ParameterRow row = parameterGridView.GetRow(e.RowHandle) as ParameterRow;
		if (e.Column.FieldName == "ParameterMode" || e.Column.FieldName == "Position")
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridDisabledGridRowForeColor;
		}
		else if (ColumnsHelper.IsCellBlocked(row, e.Column.FieldName) && !parameterGridView.GetSelectedCells(e.RowHandle).Any((GridColumn x) => x == e.Column))
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridDisabledGridRowForeColor;
		}
	}

	private void ParameterGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		setRemoveButtonAvailability?.Invoke();
	}

	public void Initialize(SharedObjectTypeEnum.ObjectType objectType, CustomFieldsSupport customFieldsSupport)
	{
		this.objectType = objectType;
		this.customFieldsSupport = customFieldsSupport;
		LengthValidation.SetTitleOrNameLengthLimit(nameRepositoryItemTextEdit);
		LengthValidation.SetDataTypeLength(dataTypeRepositoryItemTextEdit);
		LengthValidation.SetDataSize(sizeRepositoryItemTextEdit);
		customFieldsCellsTypesSupport.SetCustomFields(this.customFieldsSupport, new Dictionary<SharedObjectTypeEnum.ObjectType, GridView> { [objectType] = parameterGridView });
	}

	private void ParameterGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (procedureDesigner == null)
		{
			return;
		}
		if (e.Column != null && e.Column.FieldName.Equals("Name"))
		{
			IsNotValid();
		}
		procedureDesigner.IsChanged = true;
		if (!(parameterGridView.GetRow(e.RowHandle) is ParameterRow parameterRow))
		{
			return;
		}
		if (e.Column.FieldName == "Mode")
		{
			if (parameterRow.Mode.HasValue)
			{
				parameterRow.ParameterMode = ParameterRow.GetModeText(parameterRow.Mode.Value);
			}
			else
			{
				parameterRow.ParameterMode = null;
			}
		}
		if (parameterRow.RowState == ManagingRowsEnum.ManagingRows.Unchanged)
		{
			parameterRow.RowState = ManagingRowsEnum.ManagingRows.Updated;
		}
	}

	public void SetParameters(ProcedureDesigner procedureDesigner)
	{
		this.procedureDesigner = procedureDesigner;
		gridControl1.DataSource = this.procedureDesigner.DataSourceParameterRows;
		customFieldsCellsTypesSupport.SetCustomColumns(parameterGridView, this.procedureDesigner.CustomFieldsSupport, this.procedureDesigner.ObjectTypeValue);
		SetColumnsWidth();
	}

	private void SetColumnsWidth()
	{
		foreach (GridColumn column in parameterGridView.Columns)
		{
			if (!column.FieldName.Equals("iconProcedureParametersGridColumn") && !column.FieldName.Equals("Position"))
			{
				column.BestFit();
				if (column.Width > 400)
				{
					column.Width = 400;
				}
			}
		}
		parameterGridView.BestFitColumns();
	}

	private void ParameterGrid_ShownEditor(object sender, EventArgs e)
	{
		TextBoxMaskBox textBoxMaskBox = ((sender as GridView)?.ActiveEditor as TextEdit)?.MaskBox;
		if (textBoxMaskBox != null)
		{
			if (parameterGridView.FocusedColumn.Equals(datatypeGridColumn))
			{
				textBoxMaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
				textBoxMaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
				textBoxMaskBox.AutoCompleteCustomSource = ColumnsHelper.DataTypes;
			}
			else
			{
				textBoxMaskBox.AutoCompleteMode = AutoCompleteMode.None;
			}
		}
	}

	public void AddNewInParameter(string name = null)
	{
		AddNewParameter(ParameterRow.ModeEnum.In, name);
	}

	public void AddNewOutParameter(string name = null)
	{
		AddNewParameter(ParameterRow.ModeEnum.Out, name);
	}

	public void AddNewInOutParameter(string name = null)
	{
		AddNewParameter(ParameterRow.ModeEnum.InOut, name);
	}

	private void AddNewParameter(ParameterRow.ModeEnum parameterMode, string name = null)
	{
		ParameterRow parameterRow = new ParameterRow(customFieldsSupport)
		{
			Mode = parameterMode,
			ParameterMode = ParameterRow.GetModeText(parameterMode),
			RowState = ManagingRowsEnum.ManagingRows.ForAdding,
			Source = UserTypeEnum.UserType.USER,
			Name = name
		};
		ProcedureDesigner obj = procedureDesigner;
		if (obj != null && obj.Type == SharedObjectSubtypeEnum.ObjectSubtype.Function)
		{
			parameterRow.Position = procedureDesigner.DataSourceParameterRows.Count();
		}
		else
		{
			parameterRow.Position = procedureDesigner.DataSourceParameterRows.Count() + 1;
		}
		procedureDesigner.DataSourceParameterRows.Add(parameterRow);
		parameterGridView.RefreshData();
	}

	private void parameterGridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		Icons.SetIcon(e, iconProcedureParametersGridColumn, SharedObjectTypeEnum.ObjectType.Parameter);
	}

	public bool MoveUp()
	{
		bool result = Move(moveUp: true);
		parameterGridView.RefreshData();
		return result;
	}

	public bool MoveDown()
	{
		bool result = Move(moveUp: false);
		parameterGridView.RefreshData();
		return result;
	}

	public void MoveToTop()
	{
		try
		{
			parameterGridView.BeginUpdate();
			while (Move(moveUp: true))
			{
			}
		}
		finally
		{
			parameterGridView.EndUpdate();
			parameterGridView.RefreshData();
		}
	}

	public void MoveToBottom()
	{
		try
		{
			parameterGridView.BeginUpdate();
			while (Move(moveUp: false))
			{
			}
		}
		finally
		{
			parameterGridView.EndUpdate();
			parameterGridView.RefreshData();
		}
	}

	public new bool Move(bool moveUp)
	{
		try
		{
			parameterGridView.BeginUpdate();
			CloseEditor();
			if (parameterGridView.GetSelectedRows().FirstOrDefault() < 0)
			{
				return false;
			}
			ParameterRow selectedRow = parameterGridView.GetFocusedRow() as ParameterRow;
			if (selectedRow == null)
			{
				return false;
			}
			ParameterRow parameterRow = null;
			parameterRow = ((!moveUp) ? procedureDesigner.DataSourceParameterRows.FirstOrDefault((ParameterRow r) => r.Position == selectedRow.Position + 1) : procedureDesigner.DataSourceParameterRows.FirstOrDefault((ParameterRow r) => r.Position == selectedRow.Position - 1));
			if (parameterRow == null)
			{
				return false;
			}
			int position = selectedRow.Position;
			selectedRow.Position = parameterRow.Position;
			parameterRow.Position = position;
			if (selectedRow.RowState == ManagingRowsEnum.ManagingRows.Unchanged)
			{
				selectedRow.RowState = ManagingRowsEnum.ManagingRows.Updated;
			}
			if (parameterRow.RowState == ManagingRowsEnum.ManagingRows.Unchanged)
			{
				parameterRow.RowState = ManagingRowsEnum.ManagingRows.Updated;
			}
		}
		finally
		{
			parameterGridView.EndUpdate();
		}
		return true;
	}

	public void CloseEditor()
	{
		parameterGridView.CloseEditor();
	}

	public void RemoveParameter()
	{
		int[] selectedRows = parameterGridView.GetSelectedRows();
		List<ParameterRow> list = new List<ParameterRow>();
		int[] array = selectedRows;
		foreach (int rowHandle in array)
		{
			if (parameterGridView.GetRow(rowHandle) is ParameterRow item)
			{
				list.Add(item);
			}
		}
		procedureDesigner.Remove(list.ToArray());
		RecalculatePositions();
		parameterGridView.ClearSelection();
	}

	private void RecalculatePositions()
	{
		int num = 1;
		foreach (ParameterRow item in procedureDesigner.DataSourceParameterRows.OrderBy((ParameterRow p) => p.Position).ToList())
		{
			item.Position = num;
			num++;
		}
		parameterGridView.RefreshData();
	}

	internal bool IsNotValid()
	{
		parameterGridView.ClearColumnErrors();
		parameterGridView.RefreshData();
		return procedureDesigner.HasDuplicatesOrEmptyNames();
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
		this.gridControl1 = new DevExpress.XtraGrid.GridControl();
		this.parameterGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.iconProcedureParametersGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconFunctionParamterRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.ordinalPositionParameterGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.parameterModeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameParameterGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.datatypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.dataTypeRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.dataLengthGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.sizeRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.descriptionParameterGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.parameterGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconFunctionParamterRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataTypeRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sizeRepositoryItemTextEdit).BeginInit();
		base.SuspendLayout();
		this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gridControl1.Location = new System.Drawing.Point(0, 0);
		this.gridControl1.MainView = this.parameterGridView;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.nameRepositoryItemTextEdit, this.dataTypeRepositoryItemTextEdit, this.sizeRepositoryItemTextEdit });
		this.gridControl1.Size = new System.Drawing.Size(772, 417);
		this.gridControl1.TabIndex = 4;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.parameterGridView });
		this.parameterGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.iconProcedureParametersGridColumn, this.ordinalPositionParameterGridColumn, this.parameterModeGridColumn, this.nameParameterGridColumn, this.datatypeGridColumn, this.dataLengthGridColumn, this.descriptionParameterGridColumn });
		this.parameterGridView.GridControl = this.gridControl1;
		this.parameterGridView.Name = "parameterGridView";
		this.parameterGridView.OptionsCustomization.AllowGroup = false;
		this.parameterGridView.OptionsCustomization.AllowSort = false;
		this.parameterGridView.OptionsSelection.MultiSelect = true;
		this.parameterGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.parameterGridView.OptionsView.ColumnAutoWidth = false;
		this.parameterGridView.OptionsView.ShowGroupPanel = false;
		this.parameterGridView.OptionsView.ShowIndicator = false;
		this.parameterGridView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.ordinalPositionParameterGridColumn, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.parameterGridView.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(parameterGridView_CustomUnboundColumnData);
		this.iconProcedureParametersGridColumn.Caption = " ";
		this.iconProcedureParametersGridColumn.ColumnEdit = this.iconFunctionParamterRepositoryItemPictureEdit;
		this.iconProcedureParametersGridColumn.FieldName = "iconProcedureParametersGridColumn";
		this.iconProcedureParametersGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.iconProcedureParametersGridColumn.MaxWidth = 21;
		this.iconProcedureParametersGridColumn.MinWidth = 21;
		this.iconProcedureParametersGridColumn.Name = "iconProcedureParametersGridColumn";
		this.iconProcedureParametersGridColumn.OptionsColumn.AllowEdit = false;
		this.iconProcedureParametersGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.iconProcedureParametersGridColumn.OptionsColumn.ReadOnly = true;
		this.iconProcedureParametersGridColumn.OptionsFilter.AllowAutoFilter = false;
		this.iconProcedureParametersGridColumn.OptionsFilter.AllowFilter = false;
		this.iconProcedureParametersGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.iconProcedureParametersGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.iconProcedureParametersGridColumn.Visible = true;
		this.iconProcedureParametersGridColumn.VisibleIndex = 0;
		this.iconProcedureParametersGridColumn.Width = 21;
		this.iconFunctionParamterRepositoryItemPictureEdit.AllowFocused = false;
		this.iconFunctionParamterRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.iconFunctionParamterRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconFunctionParamterRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconFunctionParamterRepositoryItemPictureEdit.Name = "iconFunctionParamterRepositoryItemPictureEdit";
		this.iconFunctionParamterRepositoryItemPictureEdit.ShowMenu = false;
		this.ordinalPositionParameterGridColumn.Caption = "#";
		this.ordinalPositionParameterGridColumn.FieldName = "Position";
		this.ordinalPositionParameterGridColumn.Name = "ordinalPositionParameterGridColumn";
		this.ordinalPositionParameterGridColumn.OptionsColumn.AllowEdit = false;
		this.ordinalPositionParameterGridColumn.OptionsColumn.ReadOnly = true;
		this.ordinalPositionParameterGridColumn.Visible = true;
		this.ordinalPositionParameterGridColumn.VisibleIndex = 1;
		this.ordinalPositionParameterGridColumn.Width = 46;
		this.parameterModeGridColumn.Caption = "Mode";
		this.parameterModeGridColumn.FieldName = "Mode";
		this.parameterModeGridColumn.Name = "parameterModeGridColumn";
		this.parameterModeGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.parameterModeGridColumn.Visible = true;
		this.parameterModeGridColumn.VisibleIndex = 3;
		this.parameterModeGridColumn.Width = 64;
		this.nameParameterGridColumn.Caption = "Parameter";
		this.nameParameterGridColumn.ColumnEdit = this.nameRepositoryItemTextEdit;
		this.nameParameterGridColumn.FieldName = "Name";
		this.nameParameterGridColumn.Name = "nameParameterGridColumn";
		this.nameParameterGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.nameParameterGridColumn.Visible = true;
		this.nameParameterGridColumn.VisibleIndex = 2;
		this.nameParameterGridColumn.Width = 146;
		this.nameRepositoryItemTextEdit.AutoHeight = false;
		this.nameRepositoryItemTextEdit.Name = "nameRepositoryItemTextEdit";
		this.datatypeGridColumn.Caption = "Data type";
		this.datatypeGridColumn.ColumnEdit = this.dataTypeRepositoryItemTextEdit;
		this.datatypeGridColumn.FieldName = "DataType";
		this.datatypeGridColumn.Name = "datatypeGridColumn";
		this.datatypeGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.datatypeGridColumn.Visible = true;
		this.datatypeGridColumn.VisibleIndex = 4;
		this.datatypeGridColumn.Width = 97;
		this.dataTypeRepositoryItemTextEdit.AutoHeight = false;
		this.dataTypeRepositoryItemTextEdit.Name = "dataTypeRepositoryItemTextEdit";
		this.dataLengthGridColumn.Caption = "Size";
		this.dataLengthGridColumn.ColumnEdit = this.sizeRepositoryItemTextEdit;
		this.dataLengthGridColumn.FieldName = "DataLength";
		this.dataLengthGridColumn.Name = "dataLengthGridColumn";
		this.dataLengthGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.dataLengthGridColumn.Visible = true;
		this.dataLengthGridColumn.VisibleIndex = 5;
		this.dataLengthGridColumn.Width = 67;
		this.sizeRepositoryItemTextEdit.AutoHeight = false;
		this.sizeRepositoryItemTextEdit.Name = "sizeRepositoryItemTextEdit";
		this.descriptionParameterGridColumn.Caption = "Description";
		this.descriptionParameterGridColumn.FieldName = "Description";
		this.descriptionParameterGridColumn.Name = "descriptionParameterGridColumn";
		this.descriptionParameterGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.descriptionParameterGridColumn.Visible = true;
		this.descriptionParameterGridColumn.VisibleIndex = 6;
		this.descriptionParameterGridColumn.Width = 302;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.gridControl1);
		base.Name = "ParameterRowUserControl";
		base.Size = new System.Drawing.Size(772, 417);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.parameterGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconFunctionParamterRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataTypeRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sizeRepositoryItemTextEdit).EndInit();
		base.ResumeLayout(false);
	}
}
