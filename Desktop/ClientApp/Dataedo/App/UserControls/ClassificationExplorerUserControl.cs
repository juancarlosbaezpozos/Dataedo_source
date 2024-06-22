using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Licences;
using Dataedo.App.Tools.ClassificationExplorer;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.PanelControls;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.DataProcessing.Synchronize.Classes;
using Dataedo.Model.Data.CustomFields;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls;

public class ClassificationExplorerUserControl : BaseUserControl
{
	private CustomFieldsSupport support;

	private List<GridColumn> additionalColumns;

	private List<CustomFieldRow> dbCustomFields;

	private BandedGridCreator creator;

	private IContainer components;

	private LayoutControlGroup layoutControlGroup1;

	private GridControl gridControl;

	private LayoutControlItem layoutControlItem3;

	private GridPanelUserControl gridPanelUserControl;

	private LayoutControlItem layoutControlItem4;

	private ToolTipController toolTipController;

	private NonCustomizableLayoutControl layoutControl1;

	private CustomBandedGridView gridView;

	private BandedGridColumn tableGridColumn;

	private BandedGridColumn columnGridColumn;

	private BandedGridColumn dataTypeGridColumn;

	private BandedGridColumn typeGridColumn;

	private BandedGridColumn documentationGridColumn;

	private BandedGridColumn descriptionGridColumn;

	private BandedGridColumn nullableGridColumn;

	private BandedGridColumn defaultValueGridColumn;

	private BandedGridColumn identityGridColumn;

	private BandedGridColumn computedGridColumn;

	private BandedGridColumn computedFormulaGridColumn;

	private BandedGridColumn creationDateGridColumn;

	private BandedGridColumn createdByGridColumn;

	private BandedGridColumn lastModificationgridColumn;

	private BandedGridColumn modifiedByGridColumn;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private RepositoryItemCustomTextEdit columnRepositoryItemCustomTextEdit;

	private GridBand columnsGridBand;

	public GridPanelUserControl GridPanel => gridPanelUserControl;

	public List<ClassificatorExplorerColumnRow> DataSource { get; private set; }

	public ClassificationExplorerUserControl()
	{
		InitializeComponent();
		additionalColumns = new List<GridColumn>();
		DataSource = new List<ClassificatorExplorerColumnRow>();
		creator = new BandedGridCreator();
		gridView.AddColumnToFilterMapping(columnGridColumn, "ColumnDisplayName");
		columnGridColumn.SortMode = ColumnSortMode.Custom;
	}

	public void SetParameters(CustomFieldsSupport support, string gridNameToExport)
	{
		this.support = support;
		SetCustomFields();
		DisableCustomFieldColumns();
		HideColumns();
		dbCustomFields = (from x in DB.CustomField.GetCustomFields(null, false)
			select new CustomFieldRow(x)).ToList();
		SetGridPanelControl(gridNameToExport);
	}

	private void SetGridPanelControl(string gridNameToExport)
	{
		gridPanelUserControl.Initialize(gridNameToExport);
		gridPanelUserControl.HideButtons();
	}

	public void SetColumns(ClassificationExplorerCustomFieldsItem item)
	{
		HidePreviousColumns();
		additionalColumns.Clear();
		if (gridView.Bands.Any((GridBand x) => x.Tag?.Equals("Classification") ?? false))
		{
			gridView.Bands.RemoveAt(1);
		}
		if (DataSource == null || !DataSource.Any())
		{
			return;
		}
		GridBand band = creator.GetBand(item.DisplayName);
		band.Tag = "Classification";
		foreach (int? id in item.CustomFields)
		{
			if ((!dbCustomFields.Any((CustomFieldRow x) => x.CustomFieldId == id.Value) || dbCustomFields.FirstOrDefault((CustomFieldRow x) => x.CustomFieldId == id.Value).OrdinalPosition <= Licence.GetCustomFieldsLimit()) && dbCustomFields.FirstOrDefault((CustomFieldRow x) => x.CustomFieldId == id.Value) != null)
			{
				CustomFieldRowExtended field = support.Fields.FirstOrDefault((CustomFieldRowExtended x) => x.CustomFieldId == id);
				GridColumn gridColumn = gridView.Columns.FirstOrDefault((GridColumn x) => x.FieldName.Equals(field.FieldName));
				gridColumn.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
				gridColumn.OptionsColumn.AllowEdit = false;
				gridColumn.OptionsColumn.ReadOnly = true;
				gridColumn.FieldName = field.FieldName;
				gridColumn.Visible = true;
				gridColumn.OptionsFilter.AutoFilterCondition = AutoFilterCondition.Contains;
				gridColumn.BestFit();
				additionalColumns.Add(gridColumn);
			}
		}
		additionalColumns.ForEach(delegate(GridColumn x)
		{
			band.Columns.Add(x as BandedGridColumn);
		});
		band.Width = additionalColumns.Sum((GridColumn x) => x.Width);
		gridView.Bands.Add(band);
	}

	private void HidePreviousColumns()
	{
		foreach (GridColumn additionalColumn in additionalColumns)
		{
			additionalColumn.Visible = false;
		}
	}

	private void SetCustomFields()
	{
		new CustomFieldsCellsTypesSupport(isForSummaryTable: true).SetCustomColumns(gridView, support, SharedObjectTypeEnum.ObjectType.Column);
	}

	private void DisableCustomFieldColumns()
	{
		foreach (GridColumn column in gridView.Bands.FirstOrDefault().Columns)
		{
			column.OptionsColumn.AllowEdit = false;
			column.OptionsColumn.ReadOnly = true;
		}
	}

	public void SetDataSource(ClassificationExplorerCustomFieldsItem item)
	{
		DataSource.Clear();
		IEnumerable<CustomFieldRowExtended> source = support.Fields.Where((CustomFieldRowExtended x) => item.CustomFields.Any((int? y) => y == x.CustomFieldId));
		if (!source.Any())
		{
			return;
		}
		foreach (ColumnWithTableAndDatabaseObject item2 in DB.Column.GetColumnsWithTableAndDatabase(source.Select((CustomFieldRowExtended x) => x.FieldName)))
		{
			if (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataClassification))
			{
				DataSource.Add(new ClassificatorExplorerColumnRow(item2, support));
			}
			else
			{
				DataSource.Add(new ClassificatorExplorerColumnRow(item2));
			}
		}
		gridControl.Invoke((Action)delegate
		{
			gridControl.DataSource = DataSource;
		});
		gridControl.Invoke((Action)delegate
		{
			gridView.RefreshData();
		});
	}

	private void HideColumns()
	{
		foreach (GridColumn item in gridView.Columns.Where((GridColumn x) => x.AbsoluteIndex > 6))
		{
			item.Visible = false;
		}
	}

	private void GridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
	}

	private void GridView_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		if (e.Column == columnGridColumn)
		{
			string columnDisplayName = (e.RowObject1 as ClassificatorExplorerColumnRow).ColumnDisplayName;
			string columnDisplayName2 = (e.RowObject2 as ClassificatorExplorerColumnRow).ColumnDisplayName;
			e.Result = columnDisplayName.CompareTo(columnDisplayName2);
			e.Handled = true;
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
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.gridPanelUserControl = new Dataedo.App.UserControls.PanelControls.GridPanelUserControl();
		this.gridView = new Dataedo.App.UserControls.CustomBandedGridView();
		this.tableGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.columnGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.columnRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.dataTypeGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.typeGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.documentationGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.descriptionGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.nullableGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.defaultValueGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.identityGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.computedGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.computedFormulaGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.creationDateGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.createdByGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.lastModificationgridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.modifiedByGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.gridControl = new DevExpress.XtraGrid.GridControl();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.columnsGridBand = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		base.SuspendLayout();
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl1.Controls.Add(this.gridPanelUserControl);
		this.layoutControl1.Controls.Add(this.gridControl);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(943, 606);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl1";
		this.gridPanelUserControl.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.gridPanelUserControl.GridView = this.gridView;
		this.gridPanelUserControl.Location = new System.Drawing.Point(2, 2);
		this.gridPanelUserControl.Name = "gridPanelUserControl";
		this.gridPanelUserControl.Size = new System.Drawing.Size(939, 26);
		this.gridPanelUserControl.TabIndex = 7;
		this.gridView.Bands.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[1] { this.columnsGridBand });
		this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn[15]
		{
			this.tableGridColumn, this.columnGridColumn, this.dataTypeGridColumn, this.typeGridColumn, this.documentationGridColumn, this.descriptionGridColumn, this.nullableGridColumn, this.defaultValueGridColumn, this.identityGridColumn, this.computedGridColumn,
			this.computedFormulaGridColumn, this.creationDateGridColumn, this.createdByGridColumn, this.lastModificationgridColumn, this.modifiedByGridColumn
		});
		this.gridView.GridControl = this.gridControl;
		this.gridView.Name = "gridView";
		this.gridView.OptionsFilter.AllowFilterEditor = false;
		this.gridView.OptionsFind.AllowFindPanel = false;
		this.gridView.OptionsSelection.MultiSelect = true;
		this.gridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.gridView.OptionsView.ColumnAutoWidth = false;
		this.gridView.OptionsView.RowAutoHeight = true;
		this.gridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.gridView.OptionsView.ShowGroupPanel = false;
		this.gridView.OptionsView.ShowIndicator = false;
		this.gridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(GridView_PopupMenuShowing);
		this.gridView.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(GridView_CustomColumnSort);
		this.tableGridColumn.Caption = "Table";
		this.tableGridColumn.FieldName = "TableDisplayName";
		this.tableGridColumn.Name = "tableGridColumn";
		this.tableGridColumn.OptionsColumn.AllowEdit = false;
		this.tableGridColumn.OptionsColumn.ReadOnly = true;
		this.tableGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.tableGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.tableGridColumn.Visible = true;
		this.tableGridColumn.Width = 300;
		this.columnGridColumn.Caption = "Column";
		this.columnGridColumn.ColumnEdit = this.columnRepositoryItemCustomTextEdit;
		this.columnGridColumn.FieldName = "ColumnDisplayNameFormatted";
		this.columnGridColumn.Name = "columnGridColumn";
		this.columnGridColumn.OptionsColumn.AllowEdit = false;
		this.columnGridColumn.OptionsColumn.ReadOnly = true;
		this.columnGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.columnGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.columnGridColumn.Visible = true;
		this.columnGridColumn.Width = 210;
		this.columnRepositoryItemCustomTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.columnRepositoryItemCustomTextEdit.AutoHeight = false;
		this.columnRepositoryItemCustomTextEdit.Name = "columnRepositoryItemCustomTextEdit";
		this.dataTypeGridColumn.Caption = "Data type";
		this.dataTypeGridColumn.FieldName = "DataType";
		this.dataTypeGridColumn.Name = "dataTypeGridColumn";
		this.dataTypeGridColumn.OptionsColumn.AllowEdit = false;
		this.dataTypeGridColumn.OptionsColumn.ReadOnly = true;
		this.dataTypeGridColumn.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.dataTypeGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.dataTypeGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.dataTypeGridColumn.Width = 115;
		this.typeGridColumn.Caption = "Type";
		this.typeGridColumn.FieldName = "SingleParentType";
		this.typeGridColumn.Name = "typeGridColumn";
		this.typeGridColumn.OptionsColumn.AllowEdit = false;
		this.typeGridColumn.OptionsColumn.ReadOnly = true;
		this.typeGridColumn.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.typeGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.typeGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.typeGridColumn.Width = 73;
		this.documentationGridColumn.Caption = "Documentation";
		this.documentationGridColumn.FieldName = "DatabaseTitle";
		this.documentationGridColumn.Name = "documentationGridColumn";
		this.documentationGridColumn.OptionsColumn.AllowEdit = false;
		this.documentationGridColumn.OptionsColumn.ReadOnly = true;
		this.documentationGridColumn.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.documentationGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.documentationGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.documentationGridColumn.Visible = true;
		this.documentationGridColumn.Width = 163;
		this.descriptionGridColumn.Caption = "Description";
		this.descriptionGridColumn.FieldName = "Description";
		this.descriptionGridColumn.Name = "descriptionGridColumn";
		this.descriptionGridColumn.OptionsColumn.AllowEdit = false;
		this.descriptionGridColumn.OptionsColumn.ReadOnly = true;
		this.descriptionGridColumn.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.descriptionGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.descriptionGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.descriptionGridColumn.Visible = true;
		this.descriptionGridColumn.Width = 295;
		this.nullableGridColumn.Caption = "Nullable";
		this.nullableGridColumn.FieldName = "Nullable";
		this.nullableGridColumn.Name = "nullableGridColumn";
		this.nullableGridColumn.OptionsColumn.AllowEdit = false;
		this.nullableGridColumn.OptionsColumn.ReadOnly = true;
		this.nullableGridColumn.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.nullableGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.nullableGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.defaultValueGridColumn.Caption = "Default value";
		this.defaultValueGridColumn.FieldName = "DefaultValue";
		this.defaultValueGridColumn.Name = "defaultValueGridColumn";
		this.defaultValueGridColumn.OptionsColumn.AllowEdit = false;
		this.defaultValueGridColumn.OptionsColumn.ReadOnly = true;
		this.defaultValueGridColumn.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.defaultValueGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.defaultValueGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.identityGridColumn.Caption = "Identity";
		this.identityGridColumn.FieldName = "IsIdentity";
		this.identityGridColumn.Name = "identityGridColumn";
		this.identityGridColumn.OptionsColumn.AllowEdit = false;
		this.identityGridColumn.OptionsColumn.ReadOnly = true;
		this.identityGridColumn.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.identityGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.identityGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.computedGridColumn.Caption = "Computed";
		this.computedGridColumn.FieldName = "IsComputed";
		this.computedGridColumn.Name = "computedGridColumn";
		this.computedGridColumn.OptionsColumn.AllowEdit = false;
		this.computedGridColumn.OptionsColumn.ReadOnly = true;
		this.computedGridColumn.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.computedGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.computedGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.computedFormulaGridColumn.Caption = "Computed formula";
		this.computedFormulaGridColumn.FieldName = "ComputedFormula";
		this.computedFormulaGridColumn.Name = "computedFormulaGridColumn";
		this.computedFormulaGridColumn.OptionsColumn.AllowEdit = false;
		this.computedFormulaGridColumn.OptionsColumn.ReadOnly = true;
		this.computedFormulaGridColumn.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.computedFormulaGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.computedFormulaGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.creationDateGridColumn.Caption = "Creation date";
		this.creationDateGridColumn.FieldName = "CreationDateString";
		this.creationDateGridColumn.Name = "creationDateGridColumn";
		this.creationDateGridColumn.OptionsColumn.AllowEdit = false;
		this.creationDateGridColumn.OptionsColumn.ReadOnly = true;
		this.creationDateGridColumn.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.creationDateGridColumn.OptionsFilter.AllowFilter = false;
		this.creationDateGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.createdByGridColumn.Caption = "Created by";
		this.createdByGridColumn.FieldName = "CreatedBy";
		this.createdByGridColumn.Name = "createdByGridColumn";
		this.createdByGridColumn.OptionsColumn.AllowEdit = false;
		this.createdByGridColumn.OptionsColumn.ReadOnly = true;
		this.createdByGridColumn.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.createdByGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.createdByGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.lastModificationgridColumn.Caption = "Last modification";
		this.lastModificationgridColumn.FieldName = "LastModificationDateString";
		this.lastModificationgridColumn.Name = "lastModificationgridColumn";
		this.lastModificationgridColumn.OptionsColumn.AllowEdit = false;
		this.lastModificationgridColumn.OptionsColumn.ReadOnly = true;
		this.lastModificationgridColumn.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.lastModificationgridColumn.OptionsFilter.AllowFilter = false;
		this.lastModificationgridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.modifiedByGridColumn.Caption = "Modified by";
		this.modifiedByGridColumn.FieldName = "ModifiedBy";
		this.modifiedByGridColumn.Name = "modifiedByGridColumn";
		this.modifiedByGridColumn.OptionsColumn.AllowEdit = false;
		this.modifiedByGridColumn.OptionsColumn.ReadOnly = true;
		this.modifiedByGridColumn.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.modifiedByGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.modifiedByGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridControl.Location = new System.Drawing.Point(2, 32);
		this.gridControl.MainView = this.gridView;
		this.gridControl.MenuManager = this.barManager;
		this.gridControl.Name = "gridControl";
		this.gridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.columnRepositoryItemCustomTextEdit });
		this.gridControl.ShowOnlyPredefinedDetails = true;
		this.gridControl.Size = new System.Drawing.Size(939, 572);
		this.gridControl.TabIndex = 6;
		this.gridControl.ToolTipController = this.toolTipController;
		this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView });
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(943, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 606);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(943, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 606);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(943, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 606);
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.layoutControlItem3, this.layoutControlItem4 });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(943, 606);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem3.Control = this.gridControl;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 30);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(943, 576);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.layoutControlItem4.Control = this.gridPanelUserControl;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem4.MaxSize = new System.Drawing.Size(0, 30);
		this.layoutControlItem4.MinSize = new System.Drawing.Size(104, 30);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(943, 30);
		this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.columnsGridBand.Caption = "Columns";
		this.columnsGridBand.Columns.Add(this.tableGridColumn);
		this.columnsGridBand.Columns.Add(this.columnGridColumn);
		this.columnsGridBand.Columns.Add(this.dataTypeGridColumn);
		this.columnsGridBand.Columns.Add(this.typeGridColumn);
		this.columnsGridBand.Columns.Add(this.documentationGridColumn);
		this.columnsGridBand.Columns.Add(this.descriptionGridColumn);
		this.columnsGridBand.Columns.Add(this.nullableGridColumn);
		this.columnsGridBand.Columns.Add(this.defaultValueGridColumn);
		this.columnsGridBand.Columns.Add(this.identityGridColumn);
		this.columnsGridBand.Columns.Add(this.computedGridColumn);
		this.columnsGridBand.Columns.Add(this.computedFormulaGridColumn);
		this.columnsGridBand.Columns.Add(this.creationDateGridColumn);
		this.columnsGridBand.Columns.Add(this.createdByGridColumn);
		this.columnsGridBand.Columns.Add(this.lastModificationgridColumn);
		this.columnsGridBand.Columns.Add(this.modifiedByGridColumn);
		this.columnsGridBand.Name = "columnsGridBand";
		this.columnsGridBand.VisibleIndex = 0;
		this.columnsGridBand.Width = 968;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.layoutControl1);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "ClassificationExplorerUserControl";
		base.Size = new System.Drawing.Size(943, 606);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
