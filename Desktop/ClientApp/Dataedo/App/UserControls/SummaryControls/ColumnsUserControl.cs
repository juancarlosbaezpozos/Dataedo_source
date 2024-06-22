using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.PanelControls;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.History;
using Dataedo.Shared.Enums;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.SummaryControls;

public class ColumnsUserControl : BaseUserControl
{
	private string currentColumnName;

	private CustomFieldsSupport customFieldsSupport;

	private Dictionary<int, Dictionary<string, string>> customFieldsColumnForHistory = new Dictionary<int, Dictionary<string, string>>();

	private Dictionary<int, ObjectWithTitleAndDescription> titleAndDescriptionParametersForHistory = new Dictionary<int, ObjectWithTitleAndDescription>();

	private IContainer components;

	private LayoutControlGroup layoutControlGroup1;

	private GridControl columnsGridControl;

	private BulkCopyGridUserControl columnsGridView;

	private LayoutControlItem girdLayoutControlItem;

	private GridColumn schemaGridColumn;

	private GridColumn objectNameGridColumn;

	private GridColumn nameGridColumn;

	private GridColumn titleGridColumn;

	private GridColumn dataTypeGridColumn;

	private GridColumn nullableGridColumn;

	private GridColumn identityGridColumn;

	private GridColumn defaultGridColumn;

	private RepositoryItemPictureEdit keyRepositoryItemPictureEdit;

	private RepositoryItemCustomTextEdit titleRepositoryItemCustomTextEdit;

	private RepositoryItemAutoHeightMemoEdit referencesRepositoryItemAutoHeightMemoEdit;

	private RepositoryItemAutoHeightMemoEdit descriptionRepositoryItemAutoHeightMemoEdit;

	private GridColumn nullableTableColumnsGridColumn;

	private RepositoryItemCheckEdit identityRepositoryItemCheckEdit;

	private GridColumn defaultComputedTableColumnsGridColumn;

	private GridPanelUserControl gridPanelUserControl;

	private GridColumn iconGridColumn;

	private RepositoryItemPictureEdit iconRepositoryItemPictureEdit;

	private BindingSource columnRowSummaryBindingSource;

	private GridColumn descriptionGridColumn;

	private GridColumn descriptionTableRelationsGridColumn;

	private GridColumn createdGridColumn;

	private GridColumn createdByGridColumn;

	private GridColumn lastUpdatedGridColumn;

	private GridColumn lastUpdatedByGridColumn;

	private GridColumn objectGridColumn;

	private LayoutControlItem filterPanelLayoutControlItem;

	private GridColumn documentationGridColumn;

	private RepositoryItemTextEdit nameRepositoryItemTextEdit;

	private GridColumn parentTypeGridColumn;

	private NonCustomizableLayoutControl layoutControl;

	private ToolTipController toolTipController;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	public bool IsChanged { get; private set; }

	public IEnumerable<ColumnRow> ColumnRows => columnsGridControl.DataSource as List<ColumnRowSummary>;

	public ColumnRow VisibleRow { get; set; }

	public ColumnsUserControl()
	{
		InitializeComponent();
		SetMetadataTooltips();
		LengthValidation.SetTitleOrNameLengthLimit(titleRepositoryItemCustomTextEdit);
		LengthValidation.SetTitleOrNameLengthLimit(nameRepositoryItemTextEdit);
		columnsGridView.AddColumnToFilterMapping(nameGridColumn, "FullName");
	}

	private void columnsGridControl_Load(object sender, EventArgs e)
	{
		CommonFunctionsPanels.AddEventForAutoFilterRow(columnsGridView);
		CommonFunctionsPanels.AddEventForColoringDeletedRows(columnsGridView, isObject: true);
	}

	public bool ContinueAfterPossibleChanges()
	{
		if (!IsChanged)
		{
			return true;
		}
		bool result;
		switch (GeneralMessageBoxesHandling.Show("There were changes. Do you want to save them?", "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, FindForm())?.DialogResult)
		{
		case DialogResult.None:
		case DialogResult.Cancel:
			result = false;
			break;
		case DialogResult.Yes:
			result = Save();
			break;
		case DialogResult.No:
			IsChanged = false;
			result = true;
			SetDataSourceByRow();
			break;
		default:
			result = true;
			break;
		}
		return result;
	}

	public void SetParameters(CustomFieldsSupport customFieldsSupport, Action editCustomFields, string columnName)
	{
		currentColumnName = columnName;
		gridPanelUserControl.SetRemoveButtonVisibility(value: false);
		gridPanelUserControl.CustomFields += delegate
		{
			EditCustomFields(editCustomFields);
		};
		SetCustomFields(customFieldsSupport);
		gridPanelUserControl.Initialize(SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Column), isSummaryControlOnModule: true, isColumnsForm: true);
		columnsGridView.SetRowCellValue(-2147483646, "iconGridColumn", Resources.blank_16);
	}

	public void EditCustomFields(Action editCustomFields)
	{
		columnsGridView.CloseEditor();
		if (ContinueAfterPossibleChanges())
		{
			editCustomFields?.Invoke();
			RefreshCustomFields();
			gridPanelUserControl.Initialize(SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Column), isSummaryControlOnModule: false, isColumnsForm: true);
		}
	}

	public void RefreshCustomFields()
	{
		SetCustomFields(customFieldsSupport);
		SetDataSourceByRow();
	}

	public void SetGridControlVisibility(bool value)
	{
		LayoutVisibility layoutVisibility3 = (girdLayoutControlItem.Visibility = (filterPanelLayoutControlItem.Visibility = ((!value) ? LayoutVisibility.Never : LayoutVisibility.Always)));
	}

	public void SetDataSourceByRow()
	{
		columnsGridControl.DataSource = DB.Column.GetColumnsByName(currentColumnName, customFieldsSupport);
		columnsGridView.Columns.FirstOrDefault((GridColumn x) => x.FieldName.Equals("Title")).Width = (ColumnRows.Any((ColumnRow x) => !string.IsNullOrEmpty(x.Title)) ? 140 : 70);
		columnsGridView.BestFitColumns();
		SaveOldCopyOfCustomFields();
		BasicRow[] elements = ColumnRows?.ToArray();
		HistoryColumnsHelper.SaveColumnsOrParametrsOfOldCustomFields(elements, customFieldsColumnForHistory, titleAndDescriptionParametersForHistory);
	}

	public bool Save()
	{
		try
		{
			bool flag = false;
			if (!Licenses.CheckRepositoryVersionAfterLogin())
			{
				flag = true;
			}
			else
			{
				UpdateColumns();
			}
			return !flag;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
			return false;
		}
	}

	private void SetCustomFields(CustomFieldsSupport customFieldsSupport)
	{
		CustomFieldsCellsTypesSupport customFieldsCellsTypesSupport = new CustomFieldsCellsTypesSupport(isForSummaryTable: true);
		this.customFieldsSupport = customFieldsSupport;
		customFieldsCellsTypesSupport.SetCustomColumns(columnsGridView, customFieldsSupport, SharedObjectTypeEnum.ObjectType.Column);
	}

	private bool UpdateColumns()
	{
		columnsGridView.CloseEditor();
		SetCustomFields(customFieldsSupport);
		bool result = DB.Column.UpdateColumns(ColumnRows, customFieldsColumnForHistory, titleAndDescriptionParametersForHistory, null, FindForm());
		CustomFieldContainer.UpdateDefinitionValues(ColumnRows.SelectMany((ColumnRow x) => x.CustomFields.CustomFieldsData));
		return result;
	}

	private void SetMetadataTooltips()
	{
		MetadataToolTip.SetColumnToolTip(createdGridColumn, "first_imported");
		MetadataToolTip.SetColumnToolTip(createdByGridColumn, "first_imported_by");
		MetadataToolTip.SetColumnToolTip(lastUpdatedGridColumn, "last_updated");
		MetadataToolTip.SetColumnToolTip(lastUpdatedByGridColumn, "last_updated_by");
	}

	private void ModifyVisibleRow(ColumnRow columnRow)
	{
		if (VisibleRow.Id == columnRow.Id)
		{
			VisibleRow.SetModified();
		}
	}

	private void columnsGridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		Icons.SetIcon(e, iconGridColumn, SharedObjectTypeEnum.ObjectType.Column);
	}

	private void columnsGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		ColumnRow columnRow = columnsGridView.GetFocusedRow() as ColumnRow;
		columnRow.SetModified();
		IsChanged = true;
		ModifyVisibleRow(columnRow);
	}

	private void columnsGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
	}

	private void columnsGridView_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		IsChanged = true;
	}

	private void columnsGridView_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		if (e.Column == nameGridColumn)
		{
			string fullName = (e.RowObject1 as ColumnRowSummary).FullName;
			string fullName2 = (e.RowObject2 as ColumnRowSummary).FullName;
			e.Result = fullName.CompareTo(fullName2);
			e.Handled = true;
		}
	}

	private void SaveOldCopyOfCustomFields()
	{
		customFieldsColumnForHistory.Clear();
		foreach (ColumnRow columnRow in ColumnRows)
		{
			if (columnRow == null || columnRow?.CustomFields == null || columnRow?.CustomFields?.CustomFieldsData == null || columnRow == null)
			{
				continue;
			}
			_ = columnRow.Id;
			if (false || (columnRow != null && columnRow.Id < 0))
			{
				continue;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			CustomFieldDefinition[] array = columnRow?.CustomFields?.CustomFieldsData;
			foreach (CustomFieldDefinition customFieldDefinition in array)
			{
				if (customFieldDefinition != null && customFieldDefinition.CustomField != null)
				{
					dictionary.Add(customFieldDefinition?.CustomField?.FieldName, customFieldDefinition?.FieldValue);
				}
			}
			if (dictionary.Count() == 0)
			{
				break;
			}
			customFieldsColumnForHistory.Add(columnRow.Id, dictionary);
		}
	}

	private void ColumnsGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if ((e.Column == objectGridColumn || e.Column == parentTypeGridColumn || e.Column == nameGridColumn || e.Column == dataTypeGridColumn || e.Column == documentationGridColumn || e.Column == nullableTableColumnsGridColumn || e.Column == identityGridColumn || e.Column == defaultComputedTableColumnsGridColumn || e.Column == schemaGridColumn || e.Column == objectNameGridColumn || e.Column == createdByGridColumn || e.Column == createdGridColumn || e.Column == lastUpdatedByGridColumn || e.Column == lastUpdatedGridColumn) && (!columnsGridView.IsFocusedView || !columnsGridView.GetSelectedRows().Contains(e.RowHandle)))
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridNonEditableColumnsBackColor;
		}
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
		Dataedo.App.Tools.DefaultBulkCopy defaultBulkCopy = new Dataedo.App.Tools.DefaultBulkCopy();
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.columnsGridControl = new DevExpress.XtraGrid.GridControl();
		this.columnRowSummaryBindingSource = new System.Windows.Forms.BindingSource(this.components);
		this.columnsGridView = new Dataedo.App.UserControls.BulkCopyGridUserControl();
		this.iconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.objectGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.parentTypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.titleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.titleRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.dataTypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.descriptionTableRelationsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.descriptionRepositoryItemAutoHeightMemoEdit = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEdit();
		this.documentationGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nullableTableColumnsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.identityGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.identityRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.defaultComputedTableColumnsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.schemaGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.objectNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.keyRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.referencesRepositoryItemAutoHeightMemoEdit = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEdit();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.gridPanelUserControl = new Dataedo.App.UserControls.PanelControls.GridPanelUserControl();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.girdLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.filterPanelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.columnsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnRowSummaryBindingSource).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionRepositoryItemAutoHeightMemoEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.identityRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.keyRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.referencesRepositoryItemAutoHeightMemoEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.girdLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.filterPanelLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl.Controls.Add(this.columnsGridControl);
		this.layoutControl.Controls.Add(this.gridPanelUserControl);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.Root = this.layoutControlGroup1;
		this.layoutControl.Size = new System.Drawing.Size(1538, 617);
		this.layoutControl.TabIndex = 0;
		this.layoutControl.Text = "layoutControl1";
		this.columnsGridControl.DataSource = this.columnRowSummaryBindingSource;
		this.columnsGridControl.Location = new System.Drawing.Point(2, 30);
		this.columnsGridControl.MainView = this.columnsGridView;
		this.columnsGridControl.Margin = new System.Windows.Forms.Padding(0);
		this.columnsGridControl.MenuManager = this.barManager;
		this.columnsGridControl.Name = "columnsGridControl";
		this.columnsGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.keyRepositoryItemPictureEdit, this.titleRepositoryItemCustomTextEdit, this.referencesRepositoryItemAutoHeightMemoEdit, this.descriptionRepositoryItemAutoHeightMemoEdit, this.identityRepositoryItemCheckEdit, this.iconRepositoryItemPictureEdit, this.nameRepositoryItemTextEdit });
		this.columnsGridControl.ShowOnlyPredefinedDetails = true;
		this.columnsGridControl.Size = new System.Drawing.Size(1534, 585);
		this.columnsGridControl.TabIndex = 4;
		this.columnsGridControl.ToolTipController = this.toolTipController;
		this.columnsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.columnsGridView });
		this.columnsGridControl.Load += new System.EventHandler(columnsGridControl_Load);
		this.columnsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[17]
		{
			this.iconGridColumn, this.objectGridColumn, this.parentTypeGridColumn, this.nameGridColumn, this.titleGridColumn, this.dataTypeGridColumn, this.descriptionTableRelationsGridColumn, this.documentationGridColumn, this.nullableTableColumnsGridColumn, this.identityGridColumn,
			this.defaultComputedTableColumnsGridColumn, this.schemaGridColumn, this.objectNameGridColumn, this.createdGridColumn, this.createdByGridColumn, this.lastUpdatedGridColumn, this.lastUpdatedByGridColumn
		});
		defaultBulkCopy.IsCopying = false;
		this.columnsGridView.Copy = defaultBulkCopy;
		this.columnsGridView.GridControl = this.columnsGridControl;
		this.columnsGridView.Name = "columnsGridView";
		this.columnsGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.columnsGridView.OptionsSelection.MultiSelect = true;
		this.columnsGridView.OptionsView.ColumnAutoWidth = false;
		this.columnsGridView.OptionsView.RowAutoHeight = true;
		this.columnsGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.columnsGridView.OptionsView.ShowGroupPanel = false;
		this.columnsGridView.OptionsView.ShowIndicator = false;
		this.columnsGridView.RowHighlightingIsEnabled = true;
		this.columnsGridView.SplashScreenManager = null;
		this.columnsGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(ColumnsGridView_CustomDrawCell);
		this.columnsGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(columnsGridView_PopupMenuShowing);
		this.columnsGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(columnsGridView_CellValueChanged);
		this.columnsGridView.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(columnsGridView_CellValueChanging);
		this.columnsGridView.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(columnsGridView_CustomColumnSort);
		this.columnsGridView.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(columnsGridView_CustomUnboundColumnData);
		this.iconGridColumn.Caption = " ";
		this.iconGridColumn.ColumnEdit = this.iconRepositoryItemPictureEdit;
		this.iconGridColumn.FieldName = "key";
		this.iconGridColumn.MaxWidth = 21;
		this.iconGridColumn.MinWidth = 21;
		this.iconGridColumn.Name = "iconGridColumn";
		this.iconGridColumn.OptionsColumn.AllowEdit = false;
		this.iconGridColumn.OptionsColumn.ReadOnly = true;
		this.iconGridColumn.OptionsFilter.AllowAutoFilter = false;
		this.iconGridColumn.OptionsFilter.AllowFilter = false;
		this.iconGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.iconGridColumn.Visible = true;
		this.iconGridColumn.VisibleIndex = 0;
		this.iconGridColumn.Width = 21;
		this.iconRepositoryItemPictureEdit.Name = "iconRepositoryItemPictureEdit";
		this.objectGridColumn.Caption = "Parent Object";
		this.objectGridColumn.FieldName = "ParentString";
		this.objectGridColumn.Name = "objectGridColumn";
		this.objectGridColumn.OptionsColumn.AllowEdit = false;
		this.objectGridColumn.OptionsColumn.ReadOnly = true;
		this.objectGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.objectGridColumn.Visible = true;
		this.objectGridColumn.VisibleIndex = 1;
		this.objectGridColumn.Width = 192;
		this.parentTypeGridColumn.Caption = "Parent type";
		this.parentTypeGridColumn.FieldName = "SingleParentType";
		this.parentTypeGridColumn.Name = "parentTypeGridColumn";
		this.parentTypeGridColumn.OptionsColumn.AllowEdit = false;
		this.parentTypeGridColumn.OptionsColumn.ReadOnly = true;
		this.parentTypeGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.parentTypeGridColumn.Visible = true;
		this.parentTypeGridColumn.VisibleIndex = 2;
		this.nameGridColumn.Caption = "Column name";
		this.nameGridColumn.ColumnEdit = this.nameRepositoryItemTextEdit;
		this.nameGridColumn.FieldName = "FullNameFormatted";
		this.nameGridColumn.Name = "nameGridColumn";
		this.nameGridColumn.OptionsColumn.AllowEdit = false;
		this.nameGridColumn.OptionsColumn.ReadOnly = true;
		this.nameGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.nameGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.nameGridColumn.Visible = true;
		this.nameGridColumn.VisibleIndex = 3;
		this.nameGridColumn.Width = 101;
		this.nameRepositoryItemTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.nameRepositoryItemTextEdit.AutoHeight = false;
		this.nameRepositoryItemTextEdit.Name = "nameRepositoryItemTextEdit";
		this.titleGridColumn.Caption = "Column title";
		this.titleGridColumn.ColumnEdit = this.titleRepositoryItemCustomTextEdit;
		this.titleGridColumn.FieldName = "Title";
		this.titleGridColumn.Name = "titleGridColumn";
		this.titleGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.titleGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.titleGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.titleGridColumn.Tag = "FIT_WIDTH";
		this.titleGridColumn.Visible = true;
		this.titleGridColumn.VisibleIndex = 4;
		this.titleGridColumn.Width = 70;
		this.titleRepositoryItemCustomTextEdit.AutoHeight = false;
		this.titleRepositoryItemCustomTextEdit.Name = "titleRepositoryItemCustomTextEdit";
		this.dataTypeGridColumn.Caption = "Data type";
		this.dataTypeGridColumn.FieldName = "DataType";
		this.dataTypeGridColumn.Name = "dataTypeGridColumn";
		this.dataTypeGridColumn.OptionsColumn.AllowEdit = false;
		this.dataTypeGridColumn.OptionsColumn.ReadOnly = true;
		this.dataTypeGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.dataTypeGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.dataTypeGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.dataTypeGridColumn.Tag = "FIT_WIDTH";
		this.dataTypeGridColumn.Visible = true;
		this.dataTypeGridColumn.VisibleIndex = 5;
		this.dataTypeGridColumn.Width = 100;
		this.descriptionTableRelationsGridColumn.AppearanceCell.Options.UseTextOptions = true;
		this.descriptionTableRelationsGridColumn.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.descriptionTableRelationsGridColumn.Caption = "Description";
		this.descriptionTableRelationsGridColumn.ColumnEdit = this.descriptionRepositoryItemAutoHeightMemoEdit;
		this.descriptionTableRelationsGridColumn.FieldName = "Description";
		this.descriptionTableRelationsGridColumn.MinWidth = 200;
		this.descriptionTableRelationsGridColumn.Name = "descriptionTableRelationsGridColumn";
		this.descriptionTableRelationsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.descriptionTableRelationsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.descriptionTableRelationsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.descriptionTableRelationsGridColumn.Tag = "FIT_WIDTH";
		this.descriptionTableRelationsGridColumn.Visible = true;
		this.descriptionTableRelationsGridColumn.VisibleIndex = 6;
		this.descriptionTableRelationsGridColumn.Width = 400;
		this.descriptionRepositoryItemAutoHeightMemoEdit.Name = "descriptionRepositoryItemAutoHeightMemoEdit";
		this.descriptionRepositoryItemAutoHeightMemoEdit.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.documentationGridColumn.Caption = "Database";
		this.documentationGridColumn.FieldName = "DatabaseTitle";
		this.documentationGridColumn.Name = "documentationGridColumn";
		this.documentationGridColumn.OptionsColumn.AllowEdit = false;
		this.documentationGridColumn.OptionsColumn.ReadOnly = true;
		this.documentationGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.documentationGridColumn.Visible = true;
		this.documentationGridColumn.VisibleIndex = 7;
		this.documentationGridColumn.Width = 191;
		this.nullableTableColumnsGridColumn.Caption = "Nullable";
		this.nullableTableColumnsGridColumn.FieldName = "Nullable";
		this.nullableTableColumnsGridColumn.Name = "nullableTableColumnsGridColumn";
		this.nullableTableColumnsGridColumn.OptionsColumn.AllowEdit = false;
		this.nullableTableColumnsGridColumn.OptionsColumn.ReadOnly = true;
		this.nullableTableColumnsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.nullableTableColumnsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.nullableTableColumnsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.nullableTableColumnsGridColumn.Visible = true;
		this.nullableTableColumnsGridColumn.VisibleIndex = 8;
		this.nullableTableColumnsGridColumn.Width = 51;
		this.identityGridColumn.Caption = "Identity";
		this.identityGridColumn.ColumnEdit = this.identityRepositoryItemCheckEdit;
		this.identityGridColumn.FieldName = "IsIdentity";
		this.identityGridColumn.Name = "identityGridColumn";
		this.identityGridColumn.OptionsColumn.AllowEdit = false;
		this.identityGridColumn.OptionsColumn.ReadOnly = true;
		this.identityGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.identityGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.identityGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.identityGridColumn.ToolTip = "Identity / Auto increment";
		this.identityGridColumn.Visible = true;
		this.identityGridColumn.VisibleIndex = 9;
		this.identityGridColumn.Width = 51;
		this.identityRepositoryItemCheckEdit.AutoHeight = false;
		this.identityRepositoryItemCheckEdit.Name = "identityRepositoryItemCheckEdit";
		this.defaultComputedTableColumnsGridColumn.Caption = "Default / Computed";
		this.defaultComputedTableColumnsGridColumn.FieldName = "IsComputed";
		this.defaultComputedTableColumnsGridColumn.Name = "defaultComputedTableColumnsGridColumn";
		this.defaultComputedTableColumnsGridColumn.OptionsColumn.AllowEdit = false;
		this.defaultComputedTableColumnsGridColumn.OptionsColumn.ReadOnly = true;
		this.defaultComputedTableColumnsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.defaultComputedTableColumnsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.defaultComputedTableColumnsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.defaultComputedTableColumnsGridColumn.Tag = "FIT_WIDTH";
		this.defaultComputedTableColumnsGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String;
		this.defaultComputedTableColumnsGridColumn.Visible = true;
		this.defaultComputedTableColumnsGridColumn.VisibleIndex = 10;
		this.defaultComputedTableColumnsGridColumn.Width = 110;
		this.schemaGridColumn.Caption = "Schema";
		this.schemaGridColumn.FieldName = "TableSchema";
		this.schemaGridColumn.Name = "schemaGridColumn";
		this.schemaGridColumn.OptionsColumn.AllowEdit = false;
		this.schemaGridColumn.OptionsColumn.ReadOnly = true;
		this.schemaGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.schemaGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.objectNameGridColumn.Caption = "Object name";
		this.objectNameGridColumn.FieldName = "TableName";
		this.objectNameGridColumn.Name = "objectNameGridColumn";
		this.objectNameGridColumn.OptionsColumn.AllowEdit = false;
		this.objectNameGridColumn.OptionsColumn.ReadOnly = true;
		this.objectNameGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.objectNameGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.objectNameGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.objectNameGridColumn.Tag = "FIT_WIDTH";
		this.objectNameGridColumn.Width = 140;
		this.createdGridColumn.Caption = "Created/first imported";
		this.createdGridColumn.FieldName = "CreationDateString";
		this.createdGridColumn.Name = "createdGridColumn";
		this.createdGridColumn.OptionsColumn.AllowEdit = false;
		this.createdGridColumn.OptionsColumn.ReadOnly = true;
		this.createdGridColumn.OptionsFilter.AllowFilter = false;
		this.createdGridColumn.Width = 150;
		this.createdByGridColumn.Caption = "Created/first imported by";
		this.createdByGridColumn.FieldName = "CreatedBy";
		this.createdByGridColumn.Name = "createdByGridColumn";
		this.createdByGridColumn.OptionsColumn.AllowEdit = false;
		this.createdByGridColumn.OptionsColumn.ReadOnly = true;
		this.createdByGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.createdByGridColumn.Width = 150;
		this.lastUpdatedGridColumn.Caption = "Last updated";
		this.lastUpdatedGridColumn.FieldName = "LastModificationDateString";
		this.lastUpdatedGridColumn.Name = "lastUpdatedGridColumn";
		this.lastUpdatedGridColumn.OptionsColumn.AllowEdit = false;
		this.lastUpdatedGridColumn.OptionsColumn.ReadOnly = true;
		this.lastUpdatedGridColumn.OptionsFilter.AllowFilter = false;
		this.lastUpdatedGridColumn.Width = 100;
		this.lastUpdatedByGridColumn.Caption = "Last updated by";
		this.lastUpdatedByGridColumn.FieldName = "ModifiedBy";
		this.lastUpdatedByGridColumn.Name = "lastUpdatedByGridColumn";
		this.lastUpdatedByGridColumn.OptionsColumn.AllowEdit = false;
		this.lastUpdatedByGridColumn.OptionsColumn.ReadOnly = true;
		this.lastUpdatedByGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.lastUpdatedByGridColumn.Width = 100;
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(1538, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 617);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(1538, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 617);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1538, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 617);
		this.keyRepositoryItemPictureEdit.Name = "keyRepositoryItemPictureEdit";
		this.referencesRepositoryItemAutoHeightMemoEdit.Name = "referencesRepositoryItemAutoHeightMemoEdit";
		this.referencesRepositoryItemAutoHeightMemoEdit.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.gridPanelUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.gridPanelUserControl.GridView = this.columnsGridView;
		this.gridPanelUserControl.Location = new System.Drawing.Point(2, 2);
		this.gridPanelUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.gridPanelUserControl.MaximumSize = new System.Drawing.Size(0, 28);
		this.gridPanelUserControl.MinimumSize = new System.Drawing.Size(0, 28);
		this.gridPanelUserControl.Name = "gridPanelUserControl";
		this.gridPanelUserControl.Size = new System.Drawing.Size(1534, 28);
		this.gridPanelUserControl.TabIndex = 5;
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.girdLayoutControlItem, this.filterPanelLayoutControlItem });
		this.layoutControlGroup1.Name = "layoutControlGroup1";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(1538, 617);
		this.layoutControlGroup1.TextVisible = false;
		this.girdLayoutControlItem.Control = this.columnsGridControl;
		this.girdLayoutControlItem.Location = new System.Drawing.Point(0, 28);
		this.girdLayoutControlItem.Name = "girdLayoutControlItem";
		this.girdLayoutControlItem.Size = new System.Drawing.Size(1538, 589);
		this.girdLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.girdLayoutControlItem.TextVisible = false;
		this.filterPanelLayoutControlItem.Control = this.gridPanelUserControl;
		this.filterPanelLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.filterPanelLayoutControlItem.MaxSize = new System.Drawing.Size(0, 28);
		this.filterPanelLayoutControlItem.MinSize = new System.Drawing.Size(24, 28);
		this.filterPanelLayoutControlItem.Name = "filterPanelLayoutControlItem";
		this.filterPanelLayoutControlItem.Size = new System.Drawing.Size(1538, 28);
		this.filterPanelLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.filterPanelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.filterPanelLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Controls.Add(this.layoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "ColumnsUserControl";
		base.Size = new System.Drawing.Size(1538, 617);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.columnsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnRowSummaryBindingSource).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionRepositoryItemAutoHeightMemoEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.identityRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.keyRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.referencesRepositoryItemAutoHeightMemoEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.girdLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.filterPanelLayoutControlItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
