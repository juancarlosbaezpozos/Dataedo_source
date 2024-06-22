using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classification.UserControls.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Helpers.Controls;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Model.Data.Classificator;
using Dataedo.Model.Data.Documentations;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;

namespace Dataedo.App.Classification.UserControls;

public class DatabasesSelectorUserControl : BaseUserControl
{
	public delegate void DatabasesSelectionChangedHandler();

	private List<ClassificationStats> classificatorStats;

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private GridControl gridControl;

	private BandedGridView bandedGridView;

	private LayoutControlItem gridControlLayoutControlItem;

	private BandedGridColumn databasesSelectedColumn;

	private RepositoryItemCheckEdit repositoryItemCheckEdit;

	private BandedGridColumn databaseIconColumn;

	private BandedGridColumn databaseNameColumn;

	private BandedGridColumn classifiedFieldsNumberColumn;

	private BandedGridColumn classifiedFieldsProgressColumn;

	private GridBand databasesBand;

	private GridBand classifiedFieldsBand;

	private HyperlinkLabelControl selectNoneHyperlinkLabelControl;

	private HyperlinkLabelControl selectAllHyperlinkLabelControl;

	private LayoutControlItem layoutControlItem1;

	private LayoutControlItem layoutControlItem2;

	private EmptySpaceItem selectLinksEmptySpaceItem;

	private LabelControl selectionCounterLabelControl;

	private LayoutControlItem selectionCounterLabelControlLayoutControlItem;

	private ToolTipController toolTipController;

	private List<DatabaseSelectorRow> DataSource => gridControl?.DataSource as List<DatabaseSelectorRow>;

	public IEnumerable<int> SelectedDatabasesIds => DataSource?.Where((DatabaseSelectorRow x) => x.IsSelected)?.Select((DatabaseSelectorRow x) => x.DatabaseId);

	public int NumberOfColumnsInSelectedDatabases => classificatorStats?.Where((ClassificationStats x) => SelectedDatabasesIds.Contains(x.DatabaseId)).Sum((ClassificationStats x) => x.CustomFieldValueCounter) ?? 0;

	[Browsable(true)]
	public event DatabasesSelectionChangedHandler DatabasesSelectionChanged;

	public DatabasesSelectorUserControl()
	{
		InitializeComponent();
		bandedGridView.CustomDrawCell += BandedGridView_CustomDrawCell;
	}

	private void BandedGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (e.Column == classifiedFieldsProgressColumn)
		{
			GridViewHelpers.DrawBackgroundForProgressBar(e, Color.FromArgb(224, 234, 248));
		}
	}

	public void SetDatabases(int? focusedDatabaseId)
	{
		List<DatabaseSelectorRow> list = (from x in (from x in DB.Database.GetDataForMenu()
				where x.Class == "DATABASE"
				select x).ToList()
			select new DatabaseSelectorRow(x.DatabaseId, x.Title, x.Type, x.Class, 0)).ToList();
		if (focusedDatabaseId.HasValue && focusedDatabaseId != 0)
		{
			DatabaseSelectorRow databaseSelectorRow = list.Where((DatabaseSelectorRow x) => x.DatabaseId == focusedDatabaseId).FirstOrDefault();
			if (databaseSelectorRow != null)
			{
				databaseSelectorRow.IsSelected = true;
			}
		}
		gridControl.DataSource = list;
		this.DatabasesSelectionChanged?.Invoke();
		SetSelectionCounterLabel();
	}

	public void SetParameters(List<ClassificationStats> classificatorStats)
	{
		this.classificatorStats = classificatorStats;
		foreach (DatabaseSelectorRow db in DataSource)
		{
			db.ClassifiedFieldNumber = this.classificatorStats.Where((ClassificationStats y) => y.DatabaseId == db.DatabaseId && !string.IsNullOrEmpty(y.CustomFieldValue)).Sum((ClassificationStats y) => y.CustomFieldValueCounter);
		}
		gridControl.RefreshDataSource();
		GridViewHelpers.SetColumnBestWidth(classifiedFieldsNumberColumn);
	}

	private void BandedGridView_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		if (e.Column == classifiedFieldsProgressColumn)
		{
			DatabaseSelectorRow databaseRow = bandedGridView.GetRow(e.RowHandle) as DatabaseSelectorRow;
			int? num = classificatorStats?.Where((ClassificationStats x) => x.DatabaseId == databaseRow.DatabaseId).Sum((ClassificationStats x) => x.CustomFieldValueCounter);
			RepositoryItemProgressBar repositoryItemProgressBar = new RepositoryItemProgressBar
			{
				EndColor = Color.FromArgb(77, 130, 184),
				StartColor = Color.FromArgb(77, 130, 184),
				ProgressPadding = new Padding(1, 5, 4, 5),
				ProgressViewStyle = ProgressViewStyle.Solid,
				Maximum = num.GetValueOrDefault()
			};
			repositoryItemProgressBar.LookAndFeel.Style = LookAndFeelStyle.Flat;
			repositoryItemProgressBar.LookAndFeel.UseDefaultLookAndFeel = false;
			e.RepositoryItem = repositoryItemProgressBar;
		}
	}

	private void RepositoryItemCheckEdit_CheckedChanged(object sender, EventArgs e)
	{
		bandedGridView.PostEditor();
		this.DatabasesSelectionChanged?.Invoke();
		SetSelectionCounterLabel();
	}

	private void SelectNoneHyperlinkLabelControl_Click(object sender, EventArgs e)
	{
		gridControl.BeginUpdate();
		DataSource?.ForEach(delegate(DatabaseSelectorRow x)
		{
			x.IsSelected = false;
		});
		gridControl.EndUpdate();
		this.DatabasesSelectionChanged?.Invoke();
		SetSelectionCounterLabel();
	}

	private void SelectAllHyperlinkLabelControl_Click(object sender, EventArgs e)
	{
		gridControl.BeginUpdate();
		DataSource?.ForEach(delegate(DatabaseSelectorRow x)
		{
			x.IsSelected = true;
		});
		gridControl.EndUpdate();
		this.DatabasesSelectionChanged?.Invoke();
		SetSelectionCounterLabel();
	}

	private void SetSelectionCounterLabel()
	{
		int num = DataSource?.Count((DatabaseSelectorRow x) => x.IsSelected) ?? 0;
		if (num == 1)
		{
			selectionCounterLabelControl.Text = "1 database selected";
		}
		else
		{
			selectionCounterLabelControl.Text = $"{num} databases selected";
		}
	}

	private void BandedGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageBandedGridViewPopup(e);
	}

	private void GridControl_PaintEx(object sender, PaintExEventArgs e)
	{
		BandedGridViewHelpers.DrawVerticalLinesBetweenBands(bandedGridView, e);
	}

	private void ToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		if (!(e?.SelectedControl is GridControl gridControl) || !(gridControl.MainView is BandedGridView bandedGridView))
		{
			return;
		}
		BandedGridHitInfo bandedGridHitInfo = bandedGridView.CalcHitInfo(e.ControlMousePosition);
		if (!bandedGridHitInfo.InRowCell || bandedGridHitInfo.Column != classifiedFieldsProgressColumn)
		{
			return;
		}
		DatabaseSelectorRow row = bandedGridView.GetRow(bandedGridHitInfo.RowHandle) as DatabaseSelectorRow;
		if (row != null)
		{
			int num = classificatorStats?.Where((ClassificationStats x) => x.DatabaseId == row.DatabaseId).Sum((ClassificationStats x) => x.CustomFieldValueCounter) ?? 0;
			ToolTipControlInfo toolTipControlInfo = new ToolTipControlInfo
			{
				Object = row,
				SuperTip = new SuperToolTip()
			};
			toolTipControlInfo.SuperTip.Items.Add($"{row.ClassifiedFieldNumber} out of {num}" + " columns are classified with selected classification");
			e.Info = toolTipControlInfo;
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
		this.nonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.selectionCounterLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.selectNoneHyperlinkLabelControl = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.selectAllHyperlinkLabelControl = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.gridControl = new DevExpress.XtraGrid.GridControl();
		this.bandedGridView = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridView();
		this.databasesBand = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.databasesSelectedColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.repositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.databaseIconColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.databaseNameColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.classifiedFieldsBand = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.classifiedFieldsNumberColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.classifiedFieldsProgressColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.gridControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.selectLinksEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.selectionCounterLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bandedGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.selectLinksEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.selectionCounterLabelControlLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.selectionCounterLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.selectNoneHyperlinkLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.selectAllHyperlinkLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.gridControl);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(708, 212, 650, 400);
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(355, 504);
		this.nonCustomizableLayoutControl.TabIndex = 0;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl";
		this.selectionCounterLabelControl.Location = new System.Drawing.Point(242, 479);
		this.selectionCounterLabelControl.Name = "selectionCounterLabelControl";
		this.selectionCounterLabelControl.Size = new System.Drawing.Size(101, 13);
		this.selectionCounterLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.selectionCounterLabelControl.TabIndex = 9;
		this.selectionCounterLabelControl.Text = "0 databases selected";
		this.selectNoneHyperlinkLabelControl.Location = new System.Drawing.Point(65, 12);
		this.selectNoneHyperlinkLabelControl.Name = "selectNoneHyperlinkLabelControl";
		this.selectNoneHyperlinkLabelControl.Padding = new System.Windows.Forms.Padding(3);
		this.selectNoneHyperlinkLabelControl.Size = new System.Drawing.Size(63, 19);
		this.selectNoneHyperlinkLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.selectNoneHyperlinkLabelControl.TabIndex = 8;
		this.selectNoneHyperlinkLabelControl.Text = "Select none";
		this.selectNoneHyperlinkLabelControl.Click += new System.EventHandler(SelectNoneHyperlinkLabelControl_Click);
		this.selectAllHyperlinkLabelControl.Location = new System.Drawing.Point(12, 12);
		this.selectAllHyperlinkLabelControl.Name = "selectAllHyperlinkLabelControl";
		this.selectAllHyperlinkLabelControl.Padding = new System.Windows.Forms.Padding(3);
		this.selectAllHyperlinkLabelControl.Size = new System.Drawing.Size(49, 19);
		this.selectAllHyperlinkLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.selectAllHyperlinkLabelControl.TabIndex = 7;
		this.selectAllHyperlinkLabelControl.Text = "Select all";
		this.selectAllHyperlinkLabelControl.Click += new System.EventHandler(SelectAllHyperlinkLabelControl_Click);
		this.gridControl.Location = new System.Drawing.Point(12, 35);
		this.gridControl.MainView = this.bandedGridView;
		this.gridControl.Name = "gridControl";
		this.gridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemCheckEdit });
		this.gridControl.Size = new System.Drawing.Size(331, 440);
		this.gridControl.TabIndex = 6;
		this.gridControl.ToolTipController = this.toolTipController;
		this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.bandedGridView });
		this.gridControl.PaintEx += new DevExpress.XtraGrid.PaintExEventHandler(GridControl_PaintEx);
		this.bandedGridView.Appearance.FooterPanel.Options.UseFont = true;
		this.bandedGridView.Bands.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[2] { this.databasesBand, this.classifiedFieldsBand });
		this.bandedGridView.Columns.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn[5] { this.databasesSelectedColumn, this.databaseIconColumn, this.databaseNameColumn, this.classifiedFieldsNumberColumn, this.classifiedFieldsProgressColumn });
		this.bandedGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
		this.bandedGridView.GridControl = this.gridControl;
		this.bandedGridView.Name = "bandedGridView";
		this.bandedGridView.OptionsCustomization.AllowBandMoving = false;
		this.bandedGridView.OptionsCustomization.AllowBandResizing = false;
		this.bandedGridView.OptionsCustomization.AllowColumnMoving = false;
		this.bandedGridView.OptionsCustomization.AllowFilter = false;
		this.bandedGridView.OptionsCustomization.AllowGroup = false;
		this.bandedGridView.OptionsCustomization.AllowQuickHideColumns = false;
		this.bandedGridView.OptionsFind.AllowFindPanel = false;
		this.bandedGridView.OptionsMenu.EnableFooterMenu = false;
		this.bandedGridView.OptionsMenu.EnableGroupPanelMenu = false;
		this.bandedGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.bandedGridView.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;
		this.bandedGridView.OptionsView.ShowColumnHeaders = false;
		this.bandedGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.bandedGridView.OptionsView.ShowFooter = true;
		this.bandedGridView.OptionsView.ShowGroupPanel = false;
		this.bandedGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.bandedGridView.OptionsView.ShowIndicator = false;
		this.bandedGridView.OptionsView.ShowPreviewRowLines = DevExpress.Utils.DefaultBoolean.False;
		this.bandedGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.bandedGridView.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(BandedGridView_CustomRowCellEdit);
		this.bandedGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(BandedGridView_PopupMenuShowing);
		this.databasesBand.Caption = "Databases";
		this.databasesBand.Columns.Add(this.databasesSelectedColumn);
		this.databasesBand.Columns.Add(this.databaseIconColumn);
		this.databasesBand.Columns.Add(this.databaseNameColumn);
		this.databasesBand.Name = "databasesBand";
		this.databasesBand.VisibleIndex = 0;
		this.databasesBand.Width = 170;
		this.databasesSelectedColumn.Caption = "databasesSelectedColumn";
		this.databasesSelectedColumn.ColumnEdit = this.repositoryItemCheckEdit;
		this.databasesSelectedColumn.FieldName = "IsSelected";
		this.databasesSelectedColumn.Name = "databasesSelectedColumn";
		this.databasesSelectedColumn.OptionsColumn.FixedWidth = true;
		this.databasesSelectedColumn.Visible = true;
		this.databasesSelectedColumn.Width = 25;
		this.repositoryItemCheckEdit.AutoHeight = false;
		this.repositoryItemCheckEdit.Name = "repositoryItemCheckEdit";
		this.repositoryItemCheckEdit.CheckedChanged += new System.EventHandler(RepositoryItemCheckEdit_CheckedChanged);
		this.databaseIconColumn.Caption = "databaseIconColumn";
		this.databaseIconColumn.FieldName = "Icon";
		this.databaseIconColumn.Name = "databaseIconColumn";
		this.databaseIconColumn.OptionsColumn.AllowEdit = false;
		this.databaseIconColumn.OptionsColumn.FixedWidth = true;
		this.databaseIconColumn.Visible = true;
		this.databaseIconColumn.Width = 20;
		this.databaseNameColumn.Caption = "databaseNameColumn";
		this.databaseNameColumn.FieldName = "DatabaseName";
		this.databaseNameColumn.Name = "databaseNameColumn";
		this.databaseNameColumn.OptionsColumn.AllowEdit = false;
		this.databaseNameColumn.Visible = true;
		this.databaseNameColumn.Width = 125;
		this.classifiedFieldsBand.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.classifiedFieldsBand.AppearanceHeader.Options.UseFont = true;
		this.classifiedFieldsBand.AppearanceHeader.Options.UseTextOptions = true;
		this.classifiedFieldsBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.classifiedFieldsBand.Caption = "Classified fields";
		this.classifiedFieldsBand.Columns.Add(this.classifiedFieldsNumberColumn);
		this.classifiedFieldsBand.Columns.Add(this.classifiedFieldsProgressColumn);
		this.classifiedFieldsBand.Name = "classifiedFieldsBand";
		this.classifiedFieldsBand.OptionsBand.FixedWidth = true;
		this.classifiedFieldsBand.VisibleIndex = 1;
		this.classifiedFieldsBand.Width = 138;
		this.classifiedFieldsNumberColumn.Caption = "classifiedFieldsNumberColumn";
		this.classifiedFieldsNumberColumn.DisplayFormat.FormatString = "{0:N0}";
		this.classifiedFieldsNumberColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.classifiedFieldsNumberColumn.FieldName = "ClassifiedFieldNumber";
		this.classifiedFieldsNumberColumn.Name = "classifiedFieldsNumberColumn";
		this.classifiedFieldsNumberColumn.OptionsColumn.AllowEdit = false;
		this.classifiedFieldsNumberColumn.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "ClassifiedFieldNumber", "{0:N0}")
		});
		this.classifiedFieldsNumberColumn.Visible = true;
		this.classifiedFieldsNumberColumn.Width = 68;
		this.classifiedFieldsProgressColumn.Caption = "classifiedFieldsProgressColumn";
		this.classifiedFieldsProgressColumn.FieldName = "ClassifiedFieldNumber";
		this.classifiedFieldsProgressColumn.MinWidth = 70;
		this.classifiedFieldsProgressColumn.Name = "classifiedFieldsProgressColumn";
		this.classifiedFieldsProgressColumn.OptionsColumn.AllowEdit = false;
		this.classifiedFieldsProgressColumn.OptionsColumn.FixedWidth = true;
		this.classifiedFieldsProgressColumn.Visible = true;
		this.classifiedFieldsProgressColumn.Width = 70;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.gridControlLayoutControlItem, this.layoutControlItem1, this.layoutControlItem2, this.selectLinksEmptySpaceItem, this.selectionCounterLabelControlLayoutControlItem });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(355, 504);
		this.Root.TextVisible = false;
		this.gridControlLayoutControlItem.Control = this.gridControl;
		this.gridControlLayoutControlItem.Location = new System.Drawing.Point(0, 23);
		this.gridControlLayoutControlItem.Name = "gridControlLayoutControlItem";
		this.gridControlLayoutControlItem.Size = new System.Drawing.Size(335, 444);
		this.gridControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.gridControlLayoutControlItem.TextVisible = false;
		this.layoutControlItem1.Control = this.selectAllHyperlinkLabelControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(53, 23);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem2.Control = this.selectNoneHyperlinkLabelControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(53, 0);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(67, 23);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.selectLinksEmptySpaceItem.AllowHotTrack = false;
		this.selectLinksEmptySpaceItem.Location = new System.Drawing.Point(120, 0);
		this.selectLinksEmptySpaceItem.Name = "selectLinksEmptySpaceItem";
		this.selectLinksEmptySpaceItem.Size = new System.Drawing.Size(215, 23);
		this.selectLinksEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.selectionCounterLabelControlLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.selectionCounterLabelControlLayoutControlItem.Control = this.selectionCounterLabelControl;
		this.selectionCounterLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 467);
		this.selectionCounterLabelControlLayoutControlItem.Name = "selectionCounterLabelControlLayoutControlItem";
		this.selectionCounterLabelControlLayoutControlItem.Size = new System.Drawing.Size(335, 17);
		this.selectionCounterLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.selectionCounterLabelControlLayoutControlItem.TextVisible = false;
		this.toolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(ToolTipController_GetActiveObjectInfo);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.Name = "DatabasesSelectorUserControl";
		base.Size = new System.Drawing.Size(355, 504);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bandedGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.selectLinksEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.selectionCounterLabelControlLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
