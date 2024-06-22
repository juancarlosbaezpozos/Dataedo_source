using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.ImportDescriptions.Processing.CheckingBeforeSaving;
using Dataedo.App.ImportDescriptions.Tools;
using Dataedo.App.ImportDescriptions.Tools.Fields;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Model.Data.Common.CustomFieldsBase;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.ImportDescriptions.UserControls;

public class CheckingBeforeSavingUserControl : BaseUserControl
{
	private readonly int maxColumnWidth = 600;

	private List<FieldDefinition> fieldDefinitions;

	private IImportProcessorBase importProcessor;

	private CustomFieldsCellsTypesSupport customFieldsCellsTypesSupportForGrids;

	private RepositoryItemButtonEdit emptyEditor;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private GridControl dataGridControl;

	private LayoutControlItem dataGridControlLayoutControlItem;

	private BandedGridView dataBandedGridView;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ToolTipController toolTipController;

	private RepositoryItemCheckEdit repositoryItemCheckEdit;

	private CheckingBeforeSavingSummaryUserControl checkingBeforeSavingSummaryUserControl;

	private LayoutControlItem layoutControlItem1;

	private InfoUserControl ignoredInfoUserControl;

	private LayoutControlItem ignoredRowsLayoutControlItem;

	public CheckingBeforeSavingUserControl()
	{
		InitializeComponent();
		emptyEditor = new RepositoryItemButtonEdit();
		emptyEditor.Buttons.Clear();
		emptyEditor.TextEditStyle = TextEditStyles.HideTextEditor;
		dataGridControl.RepositoryItems.Add(emptyEditor);
	}

	public void Initialize(SharedObjectTypeEnum.ObjectType objectType, List<FieldDefinition> fieldDefinitions, List<ImportDataModel> modelsGeneral, int ignoredCounter)
	{
		this.fieldDefinitions = fieldDefinitions;
		customFieldsCellsTypesSupportForGrids = new CustomFieldsCellsTypesSupport(isForSummaryTable: false);
		if (ignoredCounter > 0)
		{
			ignoredRowsLayoutControlItem.Visibility = LayoutVisibility.Always;
			bool flag = ignoredCounter == 1;
			string arg = "";
			switch (objectType)
			{
			case SharedObjectTypeEnum.ObjectType.Table:
				arg = (flag ? "table" : "tables");
				break;
			case SharedObjectTypeEnum.ObjectType.Column:
				arg = (flag ? "column" : "columns");
				break;
			}
			ignoredInfoUserControl.Description = $"{ignoredCounter} {arg} " + (flag ? "was" : "were") + " ignored because of lack of changes.";
		}
		modelsGeneral?.ForEach(delegate(ImportDataModel x)
		{
			x.SelectAllNew();
		});
		switch (objectType)
		{
		case SharedObjectTypeEnum.ObjectType.Table:
			importProcessor = new ImportTablesProcessor(this.fieldDefinitions, dataBandedGridView, repositoryItemCheckEdit);
			break;
		case SharedObjectTypeEnum.ObjectType.Column:
			importProcessor = new ImportColumnsProcessor(this.fieldDefinitions, dataBandedGridView, repositoryItemCheckEdit);
			break;
		default:
			throw new ArgumentException($"Provided type ({objectType}) is not valid argument.");
		}
		importProcessor.PrepareColumns();
		dataGridControl.BeginUpdate();
		switch (objectType)
		{
		case SharedObjectTypeEnum.ObjectType.Table:
			dataGridControl.DataSource = modelsGeneral.Cast<TableImportDataModel>().ToList();
			break;
		case SharedObjectTypeEnum.ObjectType.Column:
			dataGridControl.DataSource = modelsGeneral.Cast<ColumnImportDataModel>().ToList();
			break;
		}
		dataBandedGridView.OptionsView.BestFitMaxRowCount = 1000;
		dataBandedGridView.BestFitColumns();
		dataGridControl.EndUpdate();
		SetCustomFieldsDataSource();
		foreach (GridBand band in dataBandedGridView.Bands)
		{
			SetValidColumnsWidthsForIndividualColumns(band);
		}
		checkingBeforeSavingSummaryUserControl.Initialize(dataGridControl, fieldDefinitions, modelsGeneral);
		checkingBeforeSavingSummaryUserControl.BackColor = SkinColors.ControlColorFromSystemColors;
	}

	private void SetCustomFieldsDataSource()
	{
		customFieldsCellsTypesSupportForGrids.SetCustomColumnsForExistingColumns(dataBandedGridView);
	}

	private void dataBandedGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (!e.Column.FieldName.Contains(".") || !(sender is BandedGridView bandedGridView) || e.RowHandle == -2147483646)
		{
			return;
		}
		ImportDataModel importDataModel = bandedGridView.GetRow(e.RowHandle) as ImportDataModel;
		string fieldName = NestedProperty.GetFieldName(e.Column.FieldName);
		if (importDataModel.GetType().GetProperty(fieldName).GetValue(importDataModel, null) is FieldData fieldData)
		{
			if (fieldData.Change == ChangeEnum.Change.NoChange)
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.ImportDescriptionsNoChangeBackColor;
			}
			else if (fieldData.Change == ChangeEnum.Change.New && !fieldData.IsSelected)
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.ImportDescriptionsNewBackColor;
			}
			else if (fieldData.Change == ChangeEnum.Change.Update && fieldData.IsSelected)
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.ImportDescriptionsUpdateBackColor;
			}
			else if (fieldData.Change == ChangeEnum.Change.Erase && fieldData.IsSelected)
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.ImportDescriptionsEraseBackColor;
			}
			if (e.Column.FieldName.EndsWith(".ChangeDescription") || fieldData.Change == ChangeEnum.Change.NoChange)
			{
				e.Appearance.ForeColor = ChangeEnum.GetTextColor(fieldData.Change, fieldData.IsSelected);
			}
		}
	}

	private void dataBandedGridView_ShowingEditor(object sender, CancelEventArgs e)
	{
		if (sender is BandedGridView bandedGridView)
		{
			GridColumn focusedColumn = bandedGridView.FocusedColumn;
			string fieldName = NestedProperty.GetFieldName(focusedColumn.FieldName);
			if (focusedColumn.FieldName == "Schema" || focusedColumn.FieldName == "TableName" || focusedColumn.FieldName == "ColumnName" || focusedColumn.FieldName == fieldName + ".CurrentValue" || focusedColumn.FieldName == fieldName + ".ChangeDescription")
			{
				e.Cancel = true;
			}
		}
	}

	private void dataBandedGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageBandedGridViewPopup(e);
		if (e.Menu != null)
		{
			e.Menu.Hide(GridStringId.MenuColumnBandCustomization);
			e.Menu.Hide(GridStringId.MenuColumnRemoveColumn);
		}
	}

	private void dataBandedGridView_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		if (sender is BandedGridView bandedGridView && e.RowHandle != -2147483646 && bandedGridView.GetRow(e.RowHandle) is ImportDataModel importDataModel)
		{
			string fieldName = NestedProperty.GetFieldName(e.Column.FieldName);
			if (importDataModel.GetType().GetProperty(fieldName).GetValue(importDataModel, null) is FieldData fieldData && !fieldData.IsChanged && e.Column.FieldName == fieldName + ".IsSelected")
			{
				fieldData.IsSelected = false;
				e.RepositoryItem = emptyEditor;
			}
		}
	}

	private void dataBandedGridView_ColumnWidthChanged(object sender, ColumnEventArgs e)
	{
		e.Column.Width = Math.Min(e.Column.Width, maxColumnWidth);
	}

	private void dataBandedGridView_BandWidthChanged(object sender, BandEventArgs e)
	{
		SetValidColumnsWidths(e.Band);
	}

	private void SetValidColumnsWidths(GridBand gridBand)
	{
		try
		{
			int num = gridBand.Width;
			int num2 = gridBand.Columns.Count * maxColumnWidth;
			HashSet<int> hashSet = new HashSet<int>();
			int num3 = Math.Max(0, num - num2);
			int num4 = 0;
			while ((hashSet.Count <= gridBand.Columns.Count && num3 > 0) || IsColumnTooLong(gridBand.Columns))
			{
				num4 = num3 / (gridBand.Columns.Count - hashSet.Count);
				for (int i = 0; i < gridBand.Columns.Count; i++)
				{
					GridColumn gridColumn = gridBand.Columns[i];
					int num5 = gridColumn.Width;
					if (gridColumn.Width < maxColumnWidth)
					{
						gridColumn.Width = Math.Min(gridColumn.Width + num4, maxColumnWidth);
						num3 -= num5 - gridColumn.Width;
						continue;
					}
					gridColumn.Width = Math.Min(gridColumn.Width, maxColumnWidth);
					if (!hashSet.Contains(i))
					{
						hashSet.Add(i);
					}
				}
			}
			gridBand.Width = Math.Min(gridBand.Width, gridBand.Columns.Count * maxColumnWidth);
		}
		catch
		{
		}
	}

	private bool IsColumnTooLong(GridBandColumnCollection gridBandColumnCollection)
	{
		for (int i = 0; i < gridBandColumnCollection.Count; i++)
		{
			if (gridBandColumnCollection[i].Width > maxColumnWidth)
			{
				return true;
			}
		}
		return false;
	}

	private void SetValidColumnsWidthsForIndividualColumns(GridBand gridBand)
	{
		for (int i = 0; i < gridBand.Columns.Count; i++)
		{
			BandedGridColumn bandedGridColumn = gridBand.Columns[i];
			bandedGridColumn.Width = Math.Min(bandedGridColumn.Width, maxColumnWidth);
		}
	}

	private void RepositoryItemCheckEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (dataBandedGridView.FocusedRowHandle != -2147483646)
		{
			dataBandedGridView.CloseEditor();
		}
	}

	private void DataGridControl_MouseDown(object sender, MouseEventArgs e)
	{
		BandedGridHitInfo bandedGridHitInfo = dataBandedGridView.CalcHitInfo(e.Location);
		if (bandedGridHitInfo.InRow && bandedGridHitInfo.Column.RealColumnEdit is RepositoryItemCheckEdit)
		{
			dataBandedGridView.FocusedColumn = bandedGridHitInfo.Column;
			dataBandedGridView.FocusedRowHandle = bandedGridHitInfo.RowHandle;
			dataBandedGridView.ShowEditor();
			if (dataBandedGridView.ActiveEditor is CheckEdit checkEdit)
			{
				checkEdit.Toggle();
				(e as DXMouseEventArgs).Handled = true;
				dataBandedGridView.PostEditor();
				dataBandedGridView.CloseEditor();
				checkingBeforeSavingSummaryUserControl.SetInfo();
			}
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
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.ignoredInfoUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.checkingBeforeSavingSummaryUserControl = new Dataedo.App.ImportDescriptions.UserControls.CheckingBeforeSavingSummaryUserControl();
		this.dataGridControl = new DevExpress.XtraGrid.GridControl();
		this.dataBandedGridView = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridView();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.repositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.dataGridControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.ignoredRowsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataBandedGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataGridControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ignoredRowsLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.ignoredInfoUserControl);
		this.mainLayoutControl.Controls.Add(this.checkingBeforeSavingSummaryUserControl);
		this.mainLayoutControl.Controls.Add(this.dataGridControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Margin = new System.Windows.Forms.Padding(4);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(1363, 846);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.ignoredInfoUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.ignoredInfoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.ignoredInfoUserControl.Description = "Ignored info";
		this.ignoredInfoUserControl.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40);
		this.ignoredInfoUserControl.Image = Dataedo.App.Properties.Resources.about_16;
		this.ignoredInfoUserControl.Location = new System.Drawing.Point(2, 675);
		this.ignoredInfoUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.ignoredInfoUserControl.MaximumSize = new System.Drawing.Size(0, 39);
		this.ignoredInfoUserControl.MinimumSize = new System.Drawing.Size(658, 39);
		this.ignoredInfoUserControl.Name = "ignoredInfoUserControl";
		this.ignoredInfoUserControl.Size = new System.Drawing.Size(1359, 39);
		this.ignoredInfoUserControl.TabIndex = 12;
		this.checkingBeforeSavingSummaryUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.checkingBeforeSavingSummaryUserControl.Location = new System.Drawing.Point(0, 716);
		this.checkingBeforeSavingSummaryUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.checkingBeforeSavingSummaryUserControl.Name = "checkingBeforeSavingSummaryUserControl";
		this.checkingBeforeSavingSummaryUserControl.Size = new System.Drawing.Size(1363, 130);
		this.checkingBeforeSavingSummaryUserControl.TabIndex = 5;
		this.dataGridControl.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4);
		this.dataGridControl.Location = new System.Drawing.Point(0, 0);
		this.dataGridControl.MainView = this.dataBandedGridView;
		this.dataGridControl.Margin = new System.Windows.Forms.Padding(4);
		this.dataGridControl.MenuManager = this.barManager;
		this.dataGridControl.Name = "dataGridControl";
		this.dataGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemCheckEdit });
		this.dataGridControl.ShowOnlyPredefinedDetails = true;
		this.dataGridControl.Size = new System.Drawing.Size(1363, 673);
		this.dataGridControl.TabIndex = 4;
		this.dataGridControl.ToolTipController = this.toolTipController;
		this.dataGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.dataBandedGridView });
		this.dataGridControl.MouseDown += new System.Windows.Forms.MouseEventHandler(DataGridControl_MouseDown);
		this.dataBandedGridView.DetailHeight = 431;
		this.dataBandedGridView.GridControl = this.dataGridControl;
		this.dataBandedGridView.Name = "dataBandedGridView";
		this.dataBandedGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
		this.dataBandedGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.dataBandedGridView.OptionsCustomization.AllowQuickHideColumns = false;
		this.dataBandedGridView.OptionsFilter.AllowFilterEditor = false;
		this.dataBandedGridView.OptionsSelection.MultiSelect = true;
		this.dataBandedGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.dataBandedGridView.OptionsView.ColumnAutoWidth = false;
		this.dataBandedGridView.OptionsView.RowAutoHeight = true;
		this.dataBandedGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.dataBandedGridView.OptionsView.ShowGroupPanel = false;
		this.dataBandedGridView.OptionsView.ShowIndicator = false;
		this.dataBandedGridView.BandWidthChanged += new DevExpress.XtraGrid.Views.BandedGrid.BandEventHandler(dataBandedGridView_BandWidthChanged);
		this.dataBandedGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(dataBandedGridView_CustomDrawCell);
		this.dataBandedGridView.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(dataBandedGridView_CustomRowCellEdit);
		this.dataBandedGridView.ColumnWidthChanged += new DevExpress.XtraGrid.Views.Base.ColumnEventHandler(dataBandedGridView_ColumnWidthChanged);
		this.dataBandedGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(dataBandedGridView_PopupMenuShowing);
		this.dataBandedGridView.ShowingEditor += new System.ComponentModel.CancelEventHandler(dataBandedGridView_ShowingEditor);
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Margin = new System.Windows.Forms.Padding(4);
		this.barDockControlTop.Size = new System.Drawing.Size(1363, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 846);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(4);
		this.barDockControlBottom.Size = new System.Drawing.Size(1363, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(4);
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 846);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1363, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Margin = new System.Windows.Forms.Padding(4);
		this.barDockControlRight.Size = new System.Drawing.Size(0, 846);
		this.repositoryItemCheckEdit.AutoHeight = false;
		this.repositoryItemCheckEdit.Name = "repositoryItemCheckEdit";
		this.repositoryItemCheckEdit.EditValueChanged += new System.EventHandler(RepositoryItemCheckEdit_EditValueChanged);
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.dataGridControlLayoutControlItem, this.layoutControlItem1, this.ignoredRowsLayoutControlItem });
		this.mainLayoutControlGroup.Name = "mainLayoutControlGroup";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(1363, 846);
		this.mainLayoutControlGroup.TextVisible = false;
		this.dataGridControlLayoutControlItem.Control = this.dataGridControl;
		this.dataGridControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.dataGridControlLayoutControlItem.Name = "dataGridControlLayoutControlItem";
		this.dataGridControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.dataGridControlLayoutControlItem.Size = new System.Drawing.Size(1363, 673);
		this.dataGridControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.dataGridControlLayoutControlItem.TextVisible = false;
		this.layoutControlItem1.Control = this.checkingBeforeSavingSummaryUserControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 716);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 130);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(100, 130);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem1.Size = new System.Drawing.Size(1363, 130);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.ignoredRowsLayoutControlItem.Control = this.ignoredInfoUserControl;
		this.ignoredRowsLayoutControlItem.Location = new System.Drawing.Point(0, 673);
		this.ignoredRowsLayoutControlItem.Name = "ignoredRowsLayoutControlItem";
		this.ignoredRowsLayoutControlItem.Size = new System.Drawing.Size(1363, 43);
		this.ignoredRowsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.ignoredRowsLayoutControlItem.TextVisible = false;
		this.ignoredRowsLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Margin = new System.Windows.Forms.Padding(4);
		base.Name = "CheckingBeforeSavingUserControl";
		base.Size = new System.Drawing.Size(1363, 846);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dataGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataBandedGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataGridControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ignoredRowsLayoutControlItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
