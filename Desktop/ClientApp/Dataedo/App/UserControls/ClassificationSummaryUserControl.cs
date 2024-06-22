using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Forms;
using Dataedo.App.Licences;
using Dataedo.App.Properties;
using Dataedo.App.Tools.ClassificationSummary;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.PanelControls;
using Dataedo.CustomControls;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls;

public class ClassificationSummaryUserControl : BaseUserControl
{
	private IEnumerable<int> databasesIds;

	private IContainer components;

	private LayoutControlGroup layoutControlGroup1;

	private GridControl gridControl;

	private LayoutControlItem layoutControlItem1;

	private LabelControl noneLabelControl;

	private LabelControl allLabelControl;

	private LayoutControlItem selectAllLayoutControlItem;

	private LayoutControlItem selectNonelayoutControlItemL;

	private EmptySpaceItem emptySpaceItem2;

	private CustomBandedGridView bandedGridView;

	private BandedGridColumn documentationGridColumn;

	private BandedGridColumn tableGridColumn;

	private BandedGridColumn columnGridColumn;

	private BandedGridColumn dataTypeGridColumn;

	private NonCustomizableLayoutControl layoutControl;

	private InfoUserControl infoUserControl;

	private LayoutControlItem infoUserControlLayoutControlItem;

	private ToolTipController toolTipController;

	private ClassificatorGridPanelUserControl gridPanelUserControl;

	private LayoutControlItem layoutControlItem2;

	private LabelControl selectSuggestedLabelControl;

	private LayoutControlItem selectEmptyLayoutControlItem;

	private BandedGridColumn columnNameBandedGridColumn;

	private PopupMenu popupMenu;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private RepositoryItemCustomTextEdit columnRepositoryItemCustomTextEdit;

	private BandedGridColumn descriptionBandedGridColumn;

	private ToggleSwitch showSuggestionsToggleSwitch;

	private ToggleSwitch organizeToggleSwitch;

	private LayoutControlItem organizeToggleLayoutControlItem;

	private LayoutControlItem showSuggestionsLayoutControlItem;

	private BandedGridColumn isCheckedBandedGridColumn;

	private GridBand gridBand1;

	public bool IsFilterPopupMenuShown { get; set; }

	public Classificator ColumnOperations { get; set; }

	public bool HasChanges { get; private set; }

	public ClassificationSummaryUserControl()
	{
		InitializeComponent();
		bandedGridView.PopupMenuShowing += delegate(object s, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
		{
			BandedGridView obj = s as BandedGridView;
			if (obj.IsFilterRow(obj.FocusedRowHandle))
			{
				IsFilterPopupMenuShown = true;
			}
		};
		bandedGridView.AddColumnToFilterMapping(columnGridColumn, "ColumnFormatted");
		SkinsManager.SetToggleSwitchTheme(organizeToggleSwitch);
		SkinsManager.SetToggleSwitchTheme(showSuggestionsToggleSwitch);
	}

	public void SetParameters(IEnumerable<int> databasesIds)
	{
		this.databasesIds = databasesIds;
	}

	public async Task GetClassificationsAsync(CancellationToken cancellationToken)
	{
		await ColumnOperations.GetClassificationsAsync(databasesIds, cancellationToken);
	}

	public void SetInfoBarVisibility()
	{
		bool flag = !Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataClassification);
		infoUserControlLayoutControlItem.Visibility = ((!flag) ? LayoutVisibility.Never : LayoutVisibility.Always);
	}

	public void SetGridPanel()
	{
		gridPanelUserControl.Initialize("Classification");
		gridPanelUserControl.HideButtons();
	}

	public void SetBandedGridView()
	{
		ColumnOperations.SetBandedGridView(bandedGridView);
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == (Keys.C | Keys.Control))
		{
			if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataClassification))
			{
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void PopupMenu_BeforePopup(object sender, CancelEventArgs e)
	{
		popupMenu.ItemLinks.Clear();
		GridHitInfo info = Grids.GetBeforePopupHitInfo(sender);
		if (!Grids.GetBeforePopupIsRowClicked(sender) || Grids.GetBeforePopupShouldCancel(sender))
		{
			e.Cancel = true;
			return;
		}
		if (info.InGroupRow)
		{
			e.Cancel = true;
			return;
		}
		if (bandedGridView.GetSelectedCells().ToList().TrueForAll((GridCell x) => x.Column.Tag?.Equals(info?.Column?.Tag) ?? false))
		{
			CreateItem(info?.Column, "Check all", SelectAll_Click);
			CreateItem(info?.Column, "Uncheck all", UnselectAll_Click);
		}
		if (bandedGridView.GetSelectedCells().ToList().Any(delegate(GridCell x)
		{
			GridColumn column = x.Column;
			return column != null && column.Tag?.Equals("IsChecked") == true;
		}))
		{
			CreateItem("Check selected", CheckSelected_Click);
			CreateItem("Uncheck selected", UncheckSelected_Click);
			e.Cancel = false;
		}
		else
		{
			e.Cancel = true;
		}
	}

	private void CreateItem(string title, ItemClickEventHandler action)
	{
		BarButtonItem barButtonItem = new BarButtonItem(barManager, title);
		barButtonItem.ItemClick += action;
		popupMenu.ItemLinks.Add(barButtonItem);
	}

	private void CreateItem(GridColumn column, string title, ItemClickEventHandler action)
	{
		BarButtonItem barButtonItem = new BarButtonItem(barManager, title);
		barButtonItem.ItemClick += action;
		barButtonItem.Tag = column;
		popupMenu.ItemLinks.Add(barButtonItem);
	}

	private void ManageSelected(bool value)
	{
		bandedGridView.BeginUpdate();
		GridCell[] selectedCells = bandedGridView.GetSelectedCells();
		foreach (GridCell gridCell in selectedCells)
		{
			if (bandedGridView.GetRow(gridCell.RowHandle) is ClassificatorDataModel classificatorDataModel)
			{
				classificatorDataModel.IsChecked = value;
			}
		}
		bandedGridView.EndUpdate();
		bandedGridView.RefreshData();
	}

	private void ManageAllFiltered(bool value)
	{
		bandedGridView.BeginUpdate();
		for (int i = 0; i < bandedGridView.DataRowCount; i++)
		{
			if (bandedGridView.GetRow(i) is ClassificatorDataModel classificatorDataModel)
			{
				classificatorDataModel.IsChecked = value;
			}
		}
		bandedGridView.EndUpdate();
		bandedGridView.RefreshData();
	}

	private void SelectAllUnprocessed()
	{
		bandedGridView.BeginUpdate();
		for (int i = 0; i < bandedGridView.DataRowCount; i++)
		{
			if (bandedGridView.GetRow(i) is ClassificatorDataModel classificatorDataModel)
			{
				classificatorDataModel.IsChecked = !classificatorDataModel.IsProcessed;
			}
		}
		bandedGridView.EndUpdate();
		bandedGridView.RefreshData();
	}

	private void CheckSelected_Click(object sender, ItemClickEventArgs e)
	{
		ManageSelected(value: true);
	}

	private void UncheckSelected_Click(object sender, ItemClickEventArgs e)
	{
		ManageSelected(value: false);
	}

	private void SelectAll_Click(object sender, ItemClickEventArgs e)
	{
		ManageAllFiltered(value: true);
	}

	private void UnselectAll_Click(object sender, ItemClickEventArgs e)
	{
		ManageAllFiltered(value: false);
	}

	public void Save()
	{
		bandedGridView.PostEditor();
		ColumnOperations.Save();
	}

	public void AfterSaving(bool savingSuccessful)
	{
		bandedGridView.RefreshData();
		if (savingSuccessful)
		{
			HasChanges = false;
		}
		(FindForm() as ClassificationSummaryForm)?.AfterSaving();
	}

	private void AllLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			bandedGridView.CloseEditor();
			ManageAllFiltered(value: true);
			bandedGridView.RefreshData();
		}
	}

	private void NoneLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			bandedGridView.CloseEditor();
			ManageAllFiltered(value: false);
			bandedGridView.RefreshData();
		}
	}

	private void SelectSuggestedLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			bandedGridView.CloseEditor();
			SelectAllUnprocessed();
			bandedGridView.RefreshData();
		}
	}

	private void BandedGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageBandedGridViewPopup(e);
		popupMenu.ShowPopupMenu(sender, e, inRowCellOnly: false);
	}

	private void BandedGridView_MouseDown(object sender, MouseEventArgs e)
	{
		GridHitInfo gridHitInfo = bandedGridView.CalcHitInfo(e.Location);
		if (gridHitInfo.InRow && gridHitInfo?.Column?.RealColumnEdit is RepositoryItemCheckEdit)
		{
			bandedGridView.FocusedColumn = gridHitInfo.Column;
			bandedGridView.FocusedRowHandle = gridHitInfo.RowHandle;
			bandedGridView.ShowEditor();
		}
	}

	private void BandedGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		HasChanges = true;
	}

	private void BandedGridView_RowStyle(object sender, RowStyleEventArgs e)
	{
		if (bandedGridView.IsGroupRow(e.RowHandle))
		{
			e.Appearance.BackColor = bandedGridView.PaintAppearance.GroupRow.BackColor;
			e.HighPriority = true;
			return;
		}
		ClassificatorDataModel obj = bandedGridView.GetRow(e.RowHandle) as ClassificatorDataModel;
		if (obj != null && obj.IsProcessed)
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
		}
	}

	private void BandedGridView_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
	{
		e.Appearance.BackColor = SkinColors.GridViewBandBackColor;
		e.Appearance.ForeColor = SkinColors.GridViewBandForeColor;
	}

	private void OrganizeToggleSwitch_Toggled(object sender, EventArgs e)
	{
		if (organizeToggleSwitch.IsOn)
		{
			columnNameBandedGridColumn.GroupIndex = -1;
			tableGridColumn.GroupIndex = 0;
		}
		else
		{
			tableGridColumn.GroupIndex = -1;
			columnNameBandedGridColumn.GroupIndex = 0;
		}
	}

	private void ShowSuggestionsToggleSwitch_Toggled(object sender, EventArgs e)
	{
		if (showSuggestionsToggleSwitch.IsOn)
		{
			bandedGridView.ActiveFilter.NonColumnFilter = new BinaryOperator("IsUnprocessed", value: true, BinaryOperatorType.Equal).ToString();
		}
		else
		{
			bandedGridView.ActiveFilter.NonColumnFilter = string.Empty;
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
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.showSuggestionsToggleSwitch = new DevExpress.XtraEditors.ToggleSwitch();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.organizeToggleSwitch = new DevExpress.XtraEditors.ToggleSwitch();
		this.selectSuggestedLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.infoUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.noneLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.allLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.gridControl = new DevExpress.XtraGrid.GridControl();
		this.bandedGridView = new Dataedo.App.UserControls.CustomBandedGridView();
		this.gridBand1 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.isCheckedBandedGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.columnGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.columnRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.tableGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.documentationGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.dataTypeGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.descriptionBandedGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.columnNameBandedGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.gridPanelUserControl = new Dataedo.App.UserControls.PanelControls.ClassificatorGridPanelUserControl();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.infoUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.organizeToggleLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.selectAllLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.selectEmptyLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.selectNonelayoutControlItemL = new DevExpress.XtraLayout.LayoutControlItem();
		this.showSuggestionsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.popupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.showSuggestionsToggleSwitch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.organizeToggleSwitch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bandedGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.infoUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.organizeToggleLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.selectAllLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.selectEmptyLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.selectNonelayoutControlItemL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.showSuggestionsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.popupMenu).BeginInit();
		base.SuspendLayout();
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl.Controls.Add(this.showSuggestionsToggleSwitch);
		this.layoutControl.Controls.Add(this.organizeToggleSwitch);
		this.layoutControl.Controls.Add(this.selectSuggestedLabelControl);
		this.layoutControl.Controls.Add(this.infoUserControl);
		this.layoutControl.Controls.Add(this.noneLabelControl);
		this.layoutControl.Controls.Add(this.allLabelControl);
		this.layoutControl.Controls.Add(this.gridControl);
		this.layoutControl.Controls.Add(this.gridPanelUserControl);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2837, 271, 250, 350);
		this.layoutControl.Root = this.layoutControlGroup1;
		this.layoutControl.Size = new System.Drawing.Size(1071, 498);
		this.layoutControl.TabIndex = 0;
		this.layoutControl.Text = "layoutControl1";
		this.showSuggestionsToggleSwitch.Location = new System.Drawing.Point(163, 70);
		this.showSuggestionsToggleSwitch.MenuManager = this.barManager;
		this.showSuggestionsToggleSwitch.Name = "showSuggestionsToggleSwitch";
		this.showSuggestionsToggleSwitch.Properties.AllowFocused = false;
		this.showSuggestionsToggleSwitch.Properties.OffText = "Show all suggestions";
		this.showSuggestionsToggleSwitch.Properties.OnText = "Show only unprocessed suggestions";
		this.showSuggestionsToggleSwitch.Size = new System.Drawing.Size(906, 19);
		this.showSuggestionsToggleSwitch.StyleController = this.layoutControl;
		this.showSuggestionsToggleSwitch.TabIndex = 39;
		this.showSuggestionsToggleSwitch.Toggled += new System.EventHandler(ShowSuggestionsToggleSwitch_Toggled);
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(1071, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 498);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(1071, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 498);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1071, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 498);
		this.organizeToggleSwitch.Location = new System.Drawing.Point(2, 70);
		this.organizeToggleSwitch.MenuManager = this.barManager;
		this.organizeToggleSwitch.Name = "organizeToggleSwitch";
		this.organizeToggleSwitch.Properties.AllowFocused = false;
		this.organizeToggleSwitch.Properties.OffText = "Organize by column";
		this.organizeToggleSwitch.Properties.OnText = "Organize by table";
		this.organizeToggleSwitch.Size = new System.Drawing.Size(157, 19);
		this.organizeToggleSwitch.StyleController = this.layoutControl;
		this.organizeToggleSwitch.TabIndex = 38;
		this.organizeToggleSwitch.Toggled += new System.EventHandler(OrganizeToggleSwitch_Toggled);
		this.selectSuggestedLabelControl.AllowHtmlString = true;
		this.selectSuggestedLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.selectSuggestedLabelControl.Location = new System.Drawing.Point(55, 93);
		this.selectSuggestedLabelControl.Name = "selectSuggestedLabelControl";
		this.selectSuggestedLabelControl.Padding = new System.Windows.Forms.Padding(3);
		this.selectSuggestedLabelControl.Size = new System.Drawing.Size(67, 19);
		this.selectSuggestedLabelControl.StyleController = this.layoutControl;
		this.selectSuggestedLabelControl.TabIndex = 34;
		this.selectSuggestedLabelControl.Text = "<href>Select empty</href>";
		this.selectSuggestedLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(SelectSuggestedLabelControl_HyperlinkClick);
		this.infoUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.infoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.infoUserControl.Description = "Upgrade to Enterprise to edit and save classifications";
		this.infoUserControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.infoUserControl.Image = Dataedo.App.Properties.Resources.about_16;
		this.infoUserControl.Location = new System.Drawing.Point(2, 2);
		this.infoUserControl.MaximumSize = new System.Drawing.Size(0, 32);
		this.infoUserControl.MinimumSize = new System.Drawing.Size(564, 32);
		this.infoUserControl.Name = "infoUserControl";
		this.infoUserControl.Size = new System.Drawing.Size(1067, 32);
		this.infoUserControl.TabIndex = 32;
		this.noneLabelControl.AllowHtmlString = true;
		this.noneLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.noneLabelControl.Location = new System.Drawing.Point(126, 93);
		this.noneLabelControl.Name = "noneLabelControl";
		this.noneLabelControl.Padding = new System.Windows.Forms.Padding(3);
		this.noneLabelControl.Size = new System.Drawing.Size(63, 19);
		this.noneLabelControl.StyleController = this.layoutControl;
		this.noneLabelControl.TabIndex = 9;
		this.noneLabelControl.Text = "<href>Select none</href>";
		this.noneLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(NoneLabelControl_HyperlinkClick);
		this.allLabelControl.AllowHtmlString = true;
		this.allLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.allLabelControl.Location = new System.Drawing.Point(2, 93);
		this.allLabelControl.Name = "allLabelControl";
		this.allLabelControl.Padding = new System.Windows.Forms.Padding(3);
		this.allLabelControl.Size = new System.Drawing.Size(49, 19);
		this.allLabelControl.StyleController = this.layoutControl;
		this.allLabelControl.TabIndex = 7;
		this.allLabelControl.Text = "<href>Select all</href>";
		this.allLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(AllLabelControl_HyperlinkClick);
		this.gridControl.Location = new System.Drawing.Point(2, 116);
		this.gridControl.MainView = this.bandedGridView;
		this.gridControl.MenuManager = this.barManager;
		this.gridControl.Name = "gridControl";
		this.gridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.columnRepositoryItemCustomTextEdit });
		this.gridControl.Size = new System.Drawing.Size(1067, 380);
		this.gridControl.TabIndex = 4;
		this.gridControl.ToolTipController = this.toolTipController;
		this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.bandedGridView });
		this.bandedGridView.Bands.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[1] { this.gridBand1 });
		this.bandedGridView.Columns.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn[7] { this.documentationGridColumn, this.tableGridColumn, this.columnGridColumn, this.dataTypeGridColumn, this.columnNameBandedGridColumn, this.descriptionBandedGridColumn, this.isCheckedBandedGridColumn });
		this.bandedGridView.CustomizationFormBounds = new System.Drawing.Rectangle(866, 364, 252, 242);
		this.bandedGridView.GridControl = this.gridControl;
		this.bandedGridView.GroupCount = 1;
		this.bandedGridView.Name = "bandedGridView";
		this.bandedGridView.OptionsBehavior.AutoExpandAllGroups = true;
		this.bandedGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
		this.bandedGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.bandedGridView.OptionsDetail.EnableMasterViewMode = false;
		this.bandedGridView.OptionsFilter.AllowFilterEditor = false;
		this.bandedGridView.OptionsFilter.ShowAllTableValuesInFilterPopup = true;
		this.bandedGridView.OptionsFilter.UseNewCustomFilterDialog = true;
		this.bandedGridView.OptionsFind.AllowFindPanel = false;
		this.bandedGridView.OptionsNavigation.AutoMoveRowFocus = false;
		this.bandedGridView.OptionsSelection.MultiSelect = true;
		this.bandedGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.bandedGridView.OptionsView.ColumnAutoWidth = false;
		this.bandedGridView.OptionsView.RowAutoHeight = true;
		this.bandedGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.bandedGridView.OptionsView.ShowGroupPanel = false;
		this.bandedGridView.OptionsView.ShowIndicator = false;
		this.bandedGridView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[2]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.columnNameBandedGridColumn, DevExpress.Data.ColumnSortOrder.Ascending),
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.dataTypeGridColumn, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.bandedGridView.CustomDrawGroupRow += new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(BandedGridView_CustomDrawGroupRow);
		this.bandedGridView.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(BandedGridView_RowStyle);
		this.bandedGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(BandedGridView_PopupMenuShowing);
		this.bandedGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(BandedGridView_CellValueChanged);
		this.bandedGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(BandedGridView_MouseDown);
		this.gridBand1.Caption = "gridBand1";
		this.gridBand1.Columns.Add(this.isCheckedBandedGridColumn);
		this.gridBand1.Columns.Add(this.columnGridColumn);
		this.gridBand1.Columns.Add(this.tableGridColumn);
		this.gridBand1.Columns.Add(this.documentationGridColumn);
		this.gridBand1.Columns.Add(this.dataTypeGridColumn);
		this.gridBand1.Columns.Add(this.descriptionBandedGridColumn);
		this.gridBand1.Name = "gridBand1";
		this.gridBand1.OptionsBand.AllowMove = false;
		this.gridBand1.VisibleIndex = 0;
		this.gridBand1.Width = 830;
		this.isCheckedBandedGridColumn.Caption = " ";
		this.isCheckedBandedGridColumn.FieldName = "IsChecked";
		this.isCheckedBandedGridColumn.Name = "isCheckedBandedGridColumn";
		this.isCheckedBandedGridColumn.OptionsColumn.AllowMove = false;
		this.isCheckedBandedGridColumn.OptionsColumn.AllowSize = false;
		this.isCheckedBandedGridColumn.Visible = true;
		this.isCheckedBandedGridColumn.Width = 20;
		this.columnGridColumn.Caption = "Column";
		this.columnGridColumn.ColumnEdit = this.columnRepositoryItemCustomTextEdit;
		this.columnGridColumn.FieldName = "ColumnFormatted";
		this.columnGridColumn.Name = "columnGridColumn";
		this.columnGridColumn.OptionsColumn.AllowEdit = false;
		this.columnGridColumn.OptionsColumn.AllowMove = false;
		this.columnGridColumn.OptionsColumn.ReadOnly = true;
		this.columnGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.columnGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.columnGridColumn.Visible = true;
		this.columnGridColumn.Width = 120;
		this.columnRepositoryItemCustomTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.columnRepositoryItemCustomTextEdit.AutoHeight = false;
		this.columnRepositoryItemCustomTextEdit.Name = "columnRepositoryItemCustomTextEdit";
		this.tableGridColumn.Caption = "Table";
		this.tableGridColumn.FieldName = "Table";
		this.tableGridColumn.Name = "tableGridColumn";
		this.tableGridColumn.OptionsColumn.AllowEdit = false;
		this.tableGridColumn.OptionsColumn.AllowMove = false;
		this.tableGridColumn.OptionsColumn.ReadOnly = true;
		this.tableGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.tableGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.tableGridColumn.Visible = true;
		this.tableGridColumn.Width = 120;
		this.documentationGridColumn.Caption = "Database";
		this.documentationGridColumn.FieldName = "Documentation";
		this.documentationGridColumn.Name = "documentationGridColumn";
		this.documentationGridColumn.OptionsColumn.AllowEdit = false;
		this.documentationGridColumn.OptionsColumn.AllowMove = false;
		this.documentationGridColumn.OptionsColumn.ReadOnly = true;
		this.documentationGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.documentationGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.documentationGridColumn.Visible = true;
		this.documentationGridColumn.Width = 120;
		this.dataTypeGridColumn.Caption = "Data type";
		this.dataTypeGridColumn.FieldName = "DataType";
		this.dataTypeGridColumn.Name = "dataTypeGridColumn";
		this.dataTypeGridColumn.OptionsColumn.AllowEdit = false;
		this.dataTypeGridColumn.OptionsColumn.AllowMove = false;
		this.dataTypeGridColumn.OptionsColumn.ReadOnly = true;
		this.dataTypeGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.dataTypeGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.dataTypeGridColumn.Visible = true;
		this.dataTypeGridColumn.Width = 100;
		this.descriptionBandedGridColumn.Caption = "Description";
		this.descriptionBandedGridColumn.FieldName = "Description";
		this.descriptionBandedGridColumn.Name = "descriptionBandedGridColumn";
		this.descriptionBandedGridColumn.OptionsColumn.AllowEdit = false;
		this.descriptionBandedGridColumn.OptionsColumn.ReadOnly = true;
		this.descriptionBandedGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.descriptionBandedGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.descriptionBandedGridColumn.Visible = true;
		this.descriptionBandedGridColumn.Width = 350;
		this.columnNameBandedGridColumn.Caption = "Column name";
		this.columnNameBandedGridColumn.FieldName = "ColumnName";
		this.columnNameBandedGridColumn.Name = "columnNameBandedGridColumn";
		this.columnNameBandedGridColumn.OptionsColumn.AllowEdit = false;
		this.columnNameBandedGridColumn.OptionsColumn.ReadOnly = true;
		this.columnNameBandedGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.columnNameBandedGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.columnNameBandedGridColumn.Visible = true;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.gridPanelUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.gridPanelUserControl.GridView = this.bandedGridView;
		this.gridPanelUserControl.Location = new System.Drawing.Point(2, 38);
		this.gridPanelUserControl.MaximumSize = new System.Drawing.Size(0, 28);
		this.gridPanelUserControl.MinimumSize = new System.Drawing.Size(0, 28);
		this.gridPanelUserControl.Name = "gridPanelUserControl";
		this.gridPanelUserControl.Size = new System.Drawing.Size(1067, 28);
		this.gridPanelUserControl.TabIndex = 33;
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[9] { this.layoutControlItem1, this.layoutControlItem2, this.infoUserControlLayoutControlItem, this.organizeToggleLayoutControlItem, this.emptySpaceItem2, this.selectAllLayoutControlItem, this.selectEmptyLayoutControlItem, this.selectNonelayoutControlItemL, this.showSuggestionsLayoutControlItem });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(1071, 498);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem1.Control = this.gridControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 114);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(1071, 384);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem2.Control = this.gridPanelUserControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 36);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(0, 32);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(24, 32);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(1071, 32);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.infoUserControlLayoutControlItem.Control = this.infoUserControl;
		this.infoUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.infoUserControlLayoutControlItem.Name = "infoUserControlLayoutControlItem";
		this.infoUserControlLayoutControlItem.Size = new System.Drawing.Size(1071, 36);
		this.infoUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.infoUserControlLayoutControlItem.TextVisible = false;
		this.organizeToggleLayoutControlItem.Control = this.organizeToggleSwitch;
		this.organizeToggleLayoutControlItem.Location = new System.Drawing.Point(0, 68);
		this.organizeToggleLayoutControlItem.MaxSize = new System.Drawing.Size(161, 23);
		this.organizeToggleLayoutControlItem.MinSize = new System.Drawing.Size(161, 23);
		this.organizeToggleLayoutControlItem.Name = "organizeToggleLayoutControlItem";
		this.organizeToggleLayoutControlItem.Size = new System.Drawing.Size(161, 23);
		this.organizeToggleLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.organizeToggleLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.organizeToggleLayoutControlItem.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(191, 91);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(880, 23);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.selectAllLayoutControlItem.Control = this.allLabelControl;
		this.selectAllLayoutControlItem.Location = new System.Drawing.Point(0, 91);
		this.selectAllLayoutControlItem.Name = "selectAllLayoutControlItem";
		this.selectAllLayoutControlItem.Size = new System.Drawing.Size(53, 23);
		this.selectAllLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.selectAllLayoutControlItem.TextVisible = false;
		this.selectEmptyLayoutControlItem.Control = this.selectSuggestedLabelControl;
		this.selectEmptyLayoutControlItem.Location = new System.Drawing.Point(53, 91);
		this.selectEmptyLayoutControlItem.Name = "selectEmptyLayoutControlItem";
		this.selectEmptyLayoutControlItem.Size = new System.Drawing.Size(71, 23);
		this.selectEmptyLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.selectEmptyLayoutControlItem.TextVisible = false;
		this.selectNonelayoutControlItemL.Control = this.noneLabelControl;
		this.selectNonelayoutControlItemL.Location = new System.Drawing.Point(124, 91);
		this.selectNonelayoutControlItemL.Name = "selectNonelayoutControlItemL";
		this.selectNonelayoutControlItemL.Size = new System.Drawing.Size(67, 23);
		this.selectNonelayoutControlItemL.TextSize = new System.Drawing.Size(0, 0);
		this.selectNonelayoutControlItemL.TextVisible = false;
		this.showSuggestionsLayoutControlItem.Control = this.showSuggestionsToggleSwitch;
		this.showSuggestionsLayoutControlItem.Location = new System.Drawing.Point(161, 68);
		this.showSuggestionsLayoutControlItem.Name = "showSuggestionsLayoutControlItem";
		this.showSuggestionsLayoutControlItem.Size = new System.Drawing.Size(910, 23);
		this.showSuggestionsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.showSuggestionsLayoutControlItem.TextVisible = false;
		this.popupMenu.Manager = this.barManager;
		this.popupMenu.Name = "popupMenu";
		this.popupMenu.BeforePopup += new System.ComponentModel.CancelEventHandler(PopupMenu_BeforePopup);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.layoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "ClassificationSummaryUserControl";
		base.Size = new System.Drawing.Size(1071, 498);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.showSuggestionsToggleSwitch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.organizeToggleSwitch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bandedGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.infoUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.organizeToggleLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.selectAllLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.selectEmptyLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.selectNonelayoutControlItemL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.showSuggestionsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.popupMenu).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
