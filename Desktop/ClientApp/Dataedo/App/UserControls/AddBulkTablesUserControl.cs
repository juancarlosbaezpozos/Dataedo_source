using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Tools;
using Dataedo.App.UserControls.Base;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Dataedo.App.UserControls;

public class AddBulkTablesUserControl : BaseUserControl
{
	private BulkHelper bulkHelper;

	private SharedObjectTypeEnum.ObjectType objectType;

	private IContainer components;

	private GridControl AddColumnsGridControl;

	private CustomGridUserControl AddTablesGridView;

	private GridColumn schemaGridColumn;

	private GridColumn tableNameGridColumn;

	private GridColumn columnNameGridColumn;

	private RepositoryItemTextEdit nameRepositoryItemTextEdit;

	private GridColumn titleGridColumn;

	private RepositoryItemTextEdit titleRepositoryItemTextEdit;

	private GridColumn dataTypeGridColumn;

	private RepositoryItemTextEdit dataTypeRepositoryItemTextEdit;

	private GridColumn sizeGridColumn;

	private RepositoryItemTextEdit sizeRepositoryItemTextEdit;

	private GridColumn nullableGridColumn;

	private GridColumn defaultValueGridColumn;

	private GridColumn computedFormulaGridColumn;

	private GridColumn identityGridColumn;

	private GridColumn descriptionGridColumn;

	private RepositoryItemPictureEdit repositoryItemPictureEdit1;

	private RepositoryItemTextEdit schemaRepositoryItemTextEdit;

	private RepositoryItemTextEdit tableNameRepositoryItemTextEdit;

	private ToolTipController toolTipController;

	private GridColumn statusGridColumn;

	public bool IsChanged { get; set; }

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public List<BulkTableModel> ColumnsList { get; set; }

	public AddBulkTablesUserControl()
	{
		InitializeComponent();
		LengthValidation.SetDataSize(sizeRepositoryItemTextEdit);
		LengthValidation.SetDataTypeLength(dataTypeRepositoryItemTextEdit);
		LengthValidation.SetTitleOrNameLengthLimit(nameRepositoryItemTextEdit);
		LengthValidation.SetTitleOrNameLengthLimit(tableNameRepositoryItemTextEdit);
		LengthValidation.SetTitleOrNameLengthLimit(schemaRepositoryItemTextEdit);
		LengthValidation.SetTitleOrNameLengthLimit(titleRepositoryItemTextEdit);
	}

	public void Initialize(SharedObjectTypeEnum.ObjectType objectType)
	{
		this.objectType = objectType;
		switch (objectType)
		{
		case SharedObjectTypeEnum.ObjectType.Table:
			bulkHelper = new BulkTablesHelper();
			break;
		case SharedObjectTypeEnum.ObjectType.View:
			bulkHelper = new BulkViewsHelper();
			tableNameGridColumn.Caption = "View name";
			break;
		default:
		{
			bulkHelper = new BulkObjectsHelper();
			GridColumn gridColumn = schemaGridColumn;
			OptionsColumn optionsColumn = schemaGridColumn.OptionsColumn;
			GridColumn gridColumn2 = defaultValueGridColumn;
			OptionsColumn optionsColumn2 = defaultValueGridColumn.OptionsColumn;
			GridColumn gridColumn3 = computedFormulaGridColumn;
			OptionsColumn optionsColumn3 = computedFormulaGridColumn.OptionsColumn;
			GridColumn gridColumn4 = identityGridColumn;
			bool flag2 = (identityGridColumn.OptionsColumn.ShowInCustomizationForm = false);
			bool flag4 = (gridColumn4.Visible = flag2);
			bool flag6 = (optionsColumn3.ShowInCustomizationForm = flag4);
			bool flag8 = (gridColumn3.Visible = flag6);
			bool flag10 = (optionsColumn2.ShowInCustomizationForm = flag8);
			bool flag12 = (gridColumn2.Visible = flag10);
			bool visible = (optionsColumn.ShowInCustomizationForm = flag12);
			gridColumn.Visible = visible;
			break;
		}
		}
		ColumnsList = new List<BulkTableModel>();
	}

	private void AddBulkColumns(string data)
	{
		string[] array = data.Split('\r', '\t');
		if (!IsEmptyLine(array) && !string.IsNullOrEmpty(data))
		{
			int num = -1;
			BulkTableModel bulkTableModel = new BulkTableModel();
			if (objectType == SharedObjectTypeEnum.ObjectType.Table || objectType == SharedObjectTypeEnum.ObjectType.View)
			{
				bulkTableModel.Schema = ShortenedString(array[++num], 80);
			}
			bulkTableModel.TableName = ((++num < array.Length) ? ShortenedString(array[num], 80) : string.Empty);
			bulkTableModel.Column.Name = ((++num < array.Length) ? ShortenedString(array[num], 80) : string.Empty);
			bulkTableModel.Column.DataType = ((++num < array.Length) ? ShortenedString(array[num], 250) : null);
			if (bulkTableModel.Column.DataType != null && bulkTableModel.Column.DataType.Equals(string.Empty))
			{
				bulkTableModel.Column.DataType = null;
			}
			bulkTableModel.Column.DataTypeWithoutLength = bulkTableModel.Column.DataType;
			bulkTableModel.Column.DataLength = ((++num >= array.Length) ? null : (string.IsNullOrEmpty(array[num]) ? null : ShortenedString(array[num], 50)));
			bulkTableModel.Column.Nullable = ++num < array.Length && CheckCheckBoxColumnsValues(array[num]);
			if (objectType == SharedObjectTypeEnum.ObjectType.Table || objectType == SharedObjectTypeEnum.ObjectType.View)
			{
				bulkTableModel.Column.DefaultValue = ((++num < array.Length) ? array[num] : string.Empty);
				bulkTableModel.Column.ComputedFormula = ((++num < array.Length) ? array[num] : string.Empty);
				bulkTableModel.Column.IsIdentity = ++num < array.Length && CheckCheckBoxColumnsValues(array[num]);
			}
			bulkTableModel.Column.Title = ((++num < array.Length) ? ShortenedString(array[num], 80) : string.Empty);
			bulkTableModel.Column.Description = ((++num < array.Length) ? array[num] : string.Empty);
			bulkTableModel.Column.Source = UserTypeEnum.UserType.USER;
			bulkTableModel.Column.RowState = ManagingRowsEnum.ManagingRows.ForAdding;
			ColumnsList.Add(bulkTableModel);
		}
	}

	public bool IsNotValid()
	{
		AddTablesGridView.ClearColumnErrors();
		AddTablesGridView.RefreshData();
		return ColumnsList.Any((BulkTableModel x) => x.ColumnEmptyName || x.HasEmptyName);
	}

	public void SetAsDuplicated()
	{
		foreach (IGrouping<string, BulkTableModel> item in ColumnsList.ToLookup((BulkTableModel x) => x.FullName + "." + x.ColumnName))
		{
			foreach (BulkTableModel item2 in item)
			{
				if (!string.IsNullOrEmpty(item2.Schema) && !string.IsNullOrEmpty(item2.TableName) && !string.IsNullOrEmpty(item2.ColumnName))
				{
					item2.IsColumnDuplicated = true;
				}
			}
			item.LastOrDefault().IsColumnDuplicated = false;
		}
	}

	public void CopyTemplate()
	{
		bulkHelper.CopyTemplate();
	}

	public void Paste()
	{
		AddTablesGridView.BeginUpdate();
		if (!string.IsNullOrEmpty(bulkHelper.ClipboardData))
		{
			List<string> list = bulkHelper.ClipboardData.Split('\n').ToList();
			if (list.FirstOrDefault().Contains(bulkHelper.Template))
			{
				list.Remove(list.FirstOrDefault());
			}
			foreach (string item in list)
			{
				AddBulkColumns(item);
			}
			UpdateDataSource();
			IsChanged = ColumnsList.Count > 0;
			foreach (BulkTableModel columns in ColumnsList)
			{
				columns.CheckIfNameIsEmpty();
			}
			SetAsDuplicated();
		}
		AddTablesGridView.RefreshData();
		AddTablesGridView.EndUpdate();
	}

	public void Remove()
	{
		if (AddTablesGridView.IsEditing)
		{
			return;
		}
		_ = from x in AddTablesGridView.GetSelectedRows()
			orderby x descending
			select x;
		foreach (int item in from x in AddTablesGridView.GetSelectedRows()
			orderby x descending
			select x)
		{
			AddTablesGridView.DeleteRow(item);
		}
		SetAsDuplicated();
		UpdateDataSource();
		IsChanged = ColumnsList.Count > 0;
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == (Keys.V | Keys.Control) && !AddTablesGridView.IsEditing)
		{
			Paste();
			return true;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void UpdateDataSource()
	{
		AddTablesGridView.BeginUpdate();
		AddColumnsGridControl.DataSource = ColumnsList;
		AddTablesGridView.EndUpdate();
	}

	private bool IsEmptyLine(string[] data)
	{
		return bulkHelper.IsEmptyLine(data);
	}

	private string ShortenedString(string value, int length)
	{
		return new string(value.Trim().Take(length).ToArray());
	}

	private bool CheckCheckBoxColumnsValues(string data)
	{
		return bulkHelper.CheckCheckBoxColumnsValues(data);
	}

	private void AddTablesGridView_MouseDown(object sender, MouseEventArgs e)
	{
		GridHitInfo gridHitInfo = AddTablesGridView.CalcHitInfo(e.Location);
		if (gridHitInfo.InRow && gridHitInfo.Column.RealColumnEdit is RepositoryItemCheckEdit)
		{
			AddTablesGridView.FocusedColumn = gridHitInfo.Column;
			AddTablesGridView.FocusedRowHandle = gridHitInfo.RowHandle;
			AddTablesGridView.ShowEditor();
			if (AddTablesGridView.ActiveEditor is CheckEdit checkEdit)
			{
				checkEdit.Toggle();
			}
		}
	}

	private void AddTablesGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (e.RowHandle < 0 || (!e.Column.FieldName.Equals("ColumnName") && !e.Column.FieldName.Equals("Schema") && !e.Column.FieldName.Equals("TableName")))
		{
			return;
		}
		foreach (BulkTableModel columns in ColumnsList)
		{
			columns.CheckIfNameIsEmpty();
		}
		SetAsDuplicated();
		AddTablesGridView.RefreshData();
	}

	private void toolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		GridHitInfo gridHitInfo = AddTablesGridView.CalcHitInfo(e.ControlMousePosition);
		if (gridHitInfo.RowHandle < 0)
		{
			return;
		}
		BulkTableModel bulkTableModel = AddTablesGridView.GetRow(gridHitInfo.RowHandle) as BulkTableModel;
		SuperToolTipSetupArgs superToolTipSetupArgs = new SuperToolTipSetupArgs();
		if (gridHitInfo.Column.FieldName.Equals("Image"))
		{
			if (bulkTableModel.IsColumnDuplicated)
			{
				superToolTipSetupArgs.Title.Text = "Column is duplicated";
			}
			e.Info = new ToolTipControlInfo();
			e.Info.Object = gridHitInfo.HitTest.ToString() + gridHitInfo.RowHandle;
			e.Info.ToolTipType = ToolTipType.SuperTip;
			e.Info.SuperTip = new SuperToolTip();
			e.Info.SuperTip.Setup(superToolTipSetupArgs);
		}
	}

	private void AddTablesGridView_RowCountChanged(object sender, EventArgs e)
	{
		foreach (BulkTableModel columns in ColumnsList)
		{
			columns.CheckIfNameIsEmpty();
		}
		SetAsDuplicated();
	}

	private void AddTablesGridView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
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
		this.AddColumnsGridControl = new DevExpress.XtraGrid.GridControl();
		this.AddTablesGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.statusGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.schemaGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.schemaRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.tableNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.tableNameRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.columnNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.titleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.titleRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.dataTypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.dataTypeRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.sizeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.sizeRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.nullableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.defaultValueGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.computedFormulaGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.identityGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.descriptionGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemPictureEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		((System.ComponentModel.ISupportInitialize)this.AddColumnsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.AddTablesGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.schemaRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableNameRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataTypeRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sizeRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemPictureEdit1).BeginInit();
		base.SuspendLayout();
		this.AddColumnsGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.AddColumnsGridControl.Location = new System.Drawing.Point(0, 0);
		this.AddColumnsGridControl.MainView = this.AddTablesGridView;
		this.AddColumnsGridControl.Name = "AddColumnsGridControl";
		this.AddColumnsGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.repositoryItemPictureEdit1, this.nameRepositoryItemTextEdit, this.dataTypeRepositoryItemTextEdit, this.sizeRepositoryItemTextEdit, this.titleRepositoryItemTextEdit, this.schemaRepositoryItemTextEdit, this.tableNameRepositoryItemTextEdit });
		this.AddColumnsGridControl.Size = new System.Drawing.Size(1062, 484);
		this.AddColumnsGridControl.TabIndex = 2;
		this.AddColumnsGridControl.ToolTipController = this.toolTipController;
		this.AddColumnsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.AddTablesGridView });
		this.AddTablesGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.statusGridColumn, this.schemaGridColumn, this.tableNameGridColumn, this.columnNameGridColumn, this.titleGridColumn, this.dataTypeGridColumn, this.sizeGridColumn, this.nullableGridColumn, this.defaultValueGridColumn, this.computedFormulaGridColumn,
			this.identityGridColumn, this.descriptionGridColumn
		});
		this.AddTablesGridView.GridControl = this.AddColumnsGridControl;
		this.AddTablesGridView.Name = "AddTablesGridView";
		this.AddTablesGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.AddTablesGridView.OptionsFilter.AllowFilterEditor = false;
		this.AddTablesGridView.OptionsSelection.MultiSelect = true;
		this.AddTablesGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.AddTablesGridView.OptionsView.ColumnAutoWidth = false;
		this.AddTablesGridView.OptionsView.RowAutoHeight = true;
		this.AddTablesGridView.OptionsView.ShowGroupPanel = false;
		this.AddTablesGridView.OptionsView.ShowIndicator = false;
		this.AddTablesGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(AddTablesGridView_PopupMenuShowing);
		this.AddTablesGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(AddTablesGridView_CellValueChanged);
		this.AddTablesGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(AddTablesGridView_MouseDown);
		this.AddTablesGridView.RowCountChanged += new System.EventHandler(AddTablesGridView_RowCountChanged);
		this.statusGridColumn.FieldName = "Image";
		this.statusGridColumn.Name = "statusGridColumn";
		this.statusGridColumn.OptionsColumn.ShowCaption = false;
		this.statusGridColumn.Visible = true;
		this.statusGridColumn.VisibleIndex = 0;
		this.statusGridColumn.Width = 20;
		this.schemaGridColumn.Caption = "Schema";
		this.schemaGridColumn.ColumnEdit = this.schemaRepositoryItemTextEdit;
		this.schemaGridColumn.FieldName = "Schema";
		this.schemaGridColumn.Name = "schemaGridColumn";
		this.schemaGridColumn.Visible = true;
		this.schemaGridColumn.VisibleIndex = 1;
		this.schemaRepositoryItemTextEdit.AutoHeight = false;
		this.schemaRepositoryItemTextEdit.Name = "schemaRepositoryItemTextEdit";
		this.tableNameGridColumn.Caption = "Table name";
		this.tableNameGridColumn.ColumnEdit = this.tableNameRepositoryItemTextEdit;
		this.tableNameGridColumn.FieldName = "TableName";
		this.tableNameGridColumn.Name = "tableNameGridColumn";
		this.tableNameGridColumn.Visible = true;
		this.tableNameGridColumn.VisibleIndex = 2;
		this.tableNameRepositoryItemTextEdit.AutoHeight = false;
		this.tableNameRepositoryItemTextEdit.Name = "tableNameRepositoryItemTextEdit";
		this.columnNameGridColumn.Caption = "Column name";
		this.columnNameGridColumn.ColumnEdit = this.nameRepositoryItemTextEdit;
		this.columnNameGridColumn.FieldName = "ColumnName";
		this.columnNameGridColumn.MinWidth = 97;
		this.columnNameGridColumn.Name = "columnNameGridColumn";
		this.columnNameGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.columnNameGridColumn.Visible = true;
		this.columnNameGridColumn.VisibleIndex = 3;
		this.columnNameGridColumn.Width = 97;
		this.nameRepositoryItemTextEdit.AutoHeight = false;
		this.nameRepositoryItemTextEdit.Name = "nameRepositoryItemTextEdit";
		this.titleGridColumn.Caption = "Title";
		this.titleGridColumn.ColumnEdit = this.titleRepositoryItemTextEdit;
		this.titleGridColumn.FieldName = "ColumnTitle";
		this.titleGridColumn.MinWidth = 64;
		this.titleGridColumn.Name = "titleGridColumn";
		this.titleGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.titleGridColumn.Visible = true;
		this.titleGridColumn.VisibleIndex = 10;
		this.titleGridColumn.Width = 64;
		this.titleRepositoryItemTextEdit.AutoHeight = false;
		this.titleRepositoryItemTextEdit.Name = "titleRepositoryItemTextEdit";
		this.dataTypeGridColumn.Caption = "Data type";
		this.dataTypeGridColumn.ColumnEdit = this.dataTypeRepositoryItemTextEdit;
		this.dataTypeGridColumn.FieldName = "DataType";
		this.dataTypeGridColumn.MinWidth = 70;
		this.dataTypeGridColumn.Name = "dataTypeGridColumn";
		this.dataTypeGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.dataTypeGridColumn.Visible = true;
		this.dataTypeGridColumn.VisibleIndex = 4;
		this.dataTypeGridColumn.Width = 70;
		this.dataTypeRepositoryItemTextEdit.AutoHeight = false;
		this.dataTypeRepositoryItemTextEdit.Name = "dataTypeRepositoryItemTextEdit";
		this.sizeGridColumn.Caption = "Size";
		this.sizeGridColumn.ColumnEdit = this.sizeRepositoryItemTextEdit;
		this.sizeGridColumn.FieldName = "Size";
		this.sizeGridColumn.MinWidth = 37;
		this.sizeGridColumn.Name = "sizeGridColumn";
		this.sizeGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.sizeGridColumn.Visible = true;
		this.sizeGridColumn.VisibleIndex = 5;
		this.sizeGridColumn.Width = 37;
		this.sizeRepositoryItemTextEdit.AutoHeight = false;
		this.sizeRepositoryItemTextEdit.Name = "sizeRepositoryItemTextEdit";
		this.nullableGridColumn.Caption = "Nullable";
		this.nullableGridColumn.FieldName = "Nullable";
		this.nullableGridColumn.MinWidth = 46;
		this.nullableGridColumn.Name = "nullableGridColumn";
		this.nullableGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.nullableGridColumn.Visible = true;
		this.nullableGridColumn.VisibleIndex = 6;
		this.nullableGridColumn.Width = 46;
		this.defaultValueGridColumn.Caption = "Default value";
		this.defaultValueGridColumn.FieldName = "DefaultValue";
		this.defaultValueGridColumn.MinWidth = 102;
		this.defaultValueGridColumn.Name = "defaultValueGridColumn";
		this.defaultValueGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.defaultValueGridColumn.Visible = true;
		this.defaultValueGridColumn.VisibleIndex = 7;
		this.defaultValueGridColumn.Width = 102;
		this.computedFormulaGridColumn.Caption = "Computed formula";
		this.computedFormulaGridColumn.FieldName = "ComputedFormula";
		this.computedFormulaGridColumn.MinWidth = 107;
		this.computedFormulaGridColumn.Name = "computedFormulaGridColumn";
		this.computedFormulaGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.computedFormulaGridColumn.Visible = true;
		this.computedFormulaGridColumn.VisibleIndex = 8;
		this.computedFormulaGridColumn.Width = 107;
		this.identityGridColumn.Caption = "Identity";
		this.identityGridColumn.FieldName = "Identity";
		this.identityGridColumn.MinWidth = 60;
		this.identityGridColumn.Name = "identityGridColumn";
		this.identityGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.identityGridColumn.Visible = true;
		this.identityGridColumn.VisibleIndex = 9;
		this.identityGridColumn.Width = 60;
		this.descriptionGridColumn.Caption = "Description";
		this.descriptionGridColumn.FieldName = "Description";
		this.descriptionGridColumn.Name = "descriptionGridColumn";
		this.descriptionGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.descriptionGridColumn.Visible = true;
		this.descriptionGridColumn.VisibleIndex = 11;
		this.descriptionGridColumn.Width = 300;
		this.repositoryItemPictureEdit1.Name = "repositoryItemPictureEdit1";
		this.toolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(toolTipController_GetActiveObjectInfo);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.AddColumnsGridControl);
		base.Name = "AddBulkTablesUserControl";
		base.Size = new System.Drawing.Size(1062, 484);
		((System.ComponentModel.ISupportInitialize)this.AddColumnsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.AddTablesGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.schemaRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableNameRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataTypeRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sizeRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemPictureEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
