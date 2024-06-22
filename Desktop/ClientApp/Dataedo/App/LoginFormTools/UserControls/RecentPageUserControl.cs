using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.LoginFormTools.Tools;
using Dataedo.App.LoginFormTools.Tools.CustomEventArgs;
using Dataedo.App.LoginFormTools.Tools.Enums;
using Dataedo.App.LoginFormTools.Tools.Recent;
using Dataedo.App.LoginFormTools.UserControls.Base;
using Dataedo.App.LoginFormTools.UserControls.Common;
using Dataedo.App.Properties;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.App.Tools.UI;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using RecentProjectsLibrary;

namespace Dataedo.App.LoginFormTools.UserControls;

public class RecentPageUserControl : BasePageUserControl
{
	private readonly DoubleClickSupport tileViewDoubleClickSupport;

	private readonly RecentSupport recentSupport;

	private List<RecentItemModel> recentProjects;

	private int? lastFocusedRowHandle;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private EmptySpaceItem logoPictureEditTopSeparatorEmptySpaceItem;

	private EmptySpaceItem logoPictureEditRightSeparatorEmptySpaceItem;

	private LabelControl description1LabelControl;

	private LayoutControlItem description1LabelControlLayoutControlItem;

	private SeparatorControl separatorControl;

	private LayoutControlItem separatorControlLayoutControlItem;

	private EmptySpaceItem buttonsEmptySpaceItem;

	private SimpleButton newConnectionSimpleButton;

	private LayoutControlItem connectToRepositorySimpleButtonLayoutControlItem;

	private SimpleButton newRepositorySimpleButton;

	private LayoutControlItem createNewRepositorySimpleButtonLayoutControlItem;

	private SeparatorControl topSeparatorControl;

	private LayoutControlItem topSeparatorControlLayoutControlItem;

	private ImageCollection recentMenuImageCollection;

	private PopupMenu popupMenu;

	private ToolTipController toolTipController;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private BarButtonItem removeBarButtonItem;

	private GridControl gridControl;

	private LayoutControlItem gridControlLayoutControlItem;

	private TileView tileView;

	private TileViewColumn displayNameTileViewColumn;

	private TileViewColumn iconTileViewColumn;

	private BarButtonItem connectBarButtonItem;

	private SmallLogoUserControl smallLogoUserControl;

	private LayoutControlItem smallLogoUserControlLayoutControlItem;

	private SimpleButton connectSimpleButton;

	private LayoutControlItem nextSimpleButtonLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	private TileViewColumn legacyTileViewColumn;

	public RecentPageUserControl()
	{
		InitializeComponent();
		tileViewDoubleClickSupport = new DoubleClickSupport(tileView);
		tileViewDoubleClickSupport.DoubleClick += TileViewDoubleClickSupport_DoubleClick;
		base.LoaderContext = gridControl;
		recentSupport = new RecentSupport(base.ParentForm);
	}

	internal override void SetParameter(object parameter, bool isCalledAsPrevious)
	{
		base.SetParameter(parameter, isCalledAsPrevious);
		tileViewDoubleClickSupport.SetParameters();
	}

	internal override async Task<bool> Navigated()
	{
		await base.Navigated();
		SetNextButtonAvailability();
		try
		{
			OnTimeConsumingOperationStopped(this);
			ShowLoader();
			recentProjects = recentSupport.GetRecentProjects(allowLoadingFromPreviousVersion: true, allowLoadingFromPreviousVersionWithoutAsking: true);
			gridControl.BeginUpdate();
			gridControl.DataSource = recentProjects;
			gridControl.EndUpdate();
			if (lastFocusedRowHandle.HasValue && !base.ForceClean)
			{
				tileView.FocusedRowHandle = lastFocusedRowHandle.Value;
			}
			else
			{
				tileView.FocusedRowHandle = -1;
				tileView.FocusedRowHandle = 0;
				lastFocusedRowHandle = 0;
			}
			SetNextButtonAvailability();
			gridControl.Focus();
		}
		finally
		{
			HideLoader();
		}
		return true;
	}

	internal bool ProcessRecent(RecentItemModel recentProject)
	{
		if (recentProject.Type == RepositoryType.Repository)
		{
			OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.ConnectToServerRepository, recentProject));
			return true;
		}
		OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.OpenFileRepository, recentProject));
		return true;
	}

	private void GridControl_Load(object sender, EventArgs e)
	{
		if (!base.DesignMode && base.ParentForm != null)
		{
			tileView.Appearance.EmptySpace.BackColor = base.ParentForm.BackColor;
		}
	}

	private void ConnectToRepositorySimpleButton_Click(object sender, EventArgs e)
	{
		lastFocusedRowHandle = tileView.FocusedRowHandle;
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingConnectToRepositoryRepoParameters(isFile: false, forceEmptyRepoType: true), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.CreatorExisting);
		});
		OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.ConnectToRepository));
	}

	private void CreateNewRepositorySimpleButton_Click(object sender, EventArgs e)
	{
		lastFocusedRowHandle = tileView.FocusedRowHandle;
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilder(new TrackingUserParameters(), new TrackingDataedoParameters()), TrackingEventEnum.CreatorNew);
		});
		OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.CreateRepository));
	}

	private void TileViewDoubleClickSupport_DoubleClick(object sender, EventArgs e)
	{
		ProcessFocusedRecent(sender);
	}

	private void TileView_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			ProcessFocusedRecent(sender);
		}
	}

	private void ProcessFocusedRecent(object sender)
	{
		if (sender is TileView tileView)
		{
			lastFocusedRowHandle = this.tileView.FocusedRowHandle;
			RecentItemModel recentProject = tileView.GetRow(tileView.FocusedRowHandle) as RecentItemModel;
			ProcessRecent(recentProject);
		}
	}

	private void NextSimpleButton_Click(object sender, EventArgs e)
	{
		ProcessFocusedRecent(tileView);
	}

	private void TileView_ItemCustomize(object sender, TileViewItemCustomizeEventArgs e)
	{
		if (base.ParentForm != null)
		{
			e.Item.AppearanceItem.Normal.BackColor = base.ParentForm.BackColor;
		}
		e.Item.AppearanceItem.Normal.Font = new Font(e.Item.AppearanceItem.Normal.Font.FontFamily, 12f);
		e.Item.AppearanceItem.Focused.BackColor = SkinsManager.CurrentSkin.LoginFormFocusedItemBackColor;
	}

	private void GridControl_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right)
		{
			popupMenu.ShowPopup(barManager, Control.MousePosition);
		}
	}

	private void ToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		if (e.SelectedControl != gridControl)
		{
			return;
		}
		TileViewHitInfo tileViewHitInfo = tileView.CalcHitInfo(e.ControlMousePosition);
		if (tileViewHitInfo.HitTest != TileControlHitTest.Item)
		{
			return;
		}
		TileItemElementViewInfo tileItemElementViewInfo = tileViewHitInfo.ItemInfo.Elements[tileViewHitInfo.ItemInfo.Elements.Count - 1];
		if (tileItemElementViewInfo.Image != null && tileItemElementViewInfo.ItemContentBounds.Contains(e.ControlMousePosition))
		{
			e.Info = new ToolTipControlInfo(tileItemElementViewInfo, "A file repository is about to be discontinued in the next releases.")
			{
				ToolTipType = ToolTipType.SuperTip,
				SuperTip = new SuperToolTip()
			};
			return;
		}
		RecentItemModel selectedItem = GetSelectedItem();
		if (selectedItem.Type != RepositoryType.Repository && !string.IsNullOrWhiteSpace(selectedItem.Descrption))
		{
			ToolTipControlInfo toolTipControlInfo = new ToolTipControlInfo(tileViewHitInfo.ItemInfo, selectedItem.Descrption);
			toolTipControlInfo.ToolTipType = ToolTipType.SuperTip;
			toolTipControlInfo.SuperTip = new SuperToolTip();
			e.Info = toolTipControlInfo;
		}
	}

	private void RemoveSelectedRecent()
	{
		RecentItemModel selectedItem = GetSelectedItem();
		if (selectedItem != null)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("Do you want to delete this item?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, null, 1, FindForm());
			if (handlingDialogResult != null && handlingDialogResult.DialogResult == DialogResult.Yes)
			{
				RecentProjectsHelper.Remove(selectedItem.Data);
				gridControl.BeginUpdate();
				gridControl.DataSource = recentSupport.GetRecentProjects(allowLoadingFromPreviousVersion: false, allowLoadingFromPreviousVersionWithoutAsking: false);
				gridControl.EndUpdate();
			}
		}
	}

	private RecentItemModel GetSelectedItem()
	{
		return tileView.GetRow(tileView.FocusedRowHandle) as RecentItemModel;
	}

	private void ConnectBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ProcessRecent(GetSelectedItem());
	}

	private void RemoveBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			ShowLoader();
			RemoveSelectedRecent();
		}
		finally
		{
			HideLoader();
		}
	}

	private void TileView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		SetNextButtonAvailability();
	}

	private void SetNextButtonAvailability()
	{
		connectSimpleButton.Enabled = GetSelectedItem() != null;
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
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition2 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition3 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement2 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement3 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.LoginFormTools.UserControls.RecentPageUserControl));
		this.iconTileViewColumn = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.displayNameTileViewColumn = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.connectSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.smallLogoUserControl = new Dataedo.App.LoginFormTools.UserControls.Common.SmallLogoUserControl();
		this.gridControl = new DevExpress.XtraGrid.GridControl();
		this.tileView = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.removeBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.connectBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.topSeparatorControl = new DevExpress.XtraEditors.SeparatorControl();
		this.newRepositorySimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.newConnectionSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.separatorControl = new DevExpress.XtraEditors.SeparatorControl();
		this.description1LabelControl = new DevExpress.XtraEditors.LabelControl();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.logoPictureEditTopSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.logoPictureEditRightSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.description1LabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.separatorControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.buttonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.topSeparatorControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.gridControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.smallLogoUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.nextSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.connectToRepositorySimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.createNewRepositorySimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.recentMenuImageCollection = new DevExpress.Utils.ImageCollection(this.components);
		this.popupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.legacyTileViewColumn = new DevExpress.XtraGrid.Columns.TileViewColumn();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.topSeparatorControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.separatorControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.separatorControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.topSeparatorControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nextSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectToRepositorySimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.createNewRepositorySimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.recentMenuImageCollection).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.popupMenu).BeginInit();
		base.SuspendLayout();
		this.iconTileViewColumn.Caption = "Icon";
		this.iconTileViewColumn.FieldName = "Icon";
		this.iconTileViewColumn.Name = "iconTileViewColumn";
		this.iconTileViewColumn.Visible = true;
		this.iconTileViewColumn.VisibleIndex = 0;
		this.displayNameTileViewColumn.Caption = "Display name";
		this.displayNameTileViewColumn.FieldName = "DisplayName";
		this.displayNameTileViewColumn.Name = "displayNameTileViewColumn";
		this.displayNameTileViewColumn.Visible = true;
		this.displayNameTileViewColumn.VisibleIndex = 1;
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.connectSimpleButton);
		this.mainLayoutControl.Controls.Add(this.smallLogoUserControl);
		this.mainLayoutControl.Controls.Add(this.gridControl);
		this.mainLayoutControl.Controls.Add(this.topSeparatorControl);
		this.mainLayoutControl.Controls.Add(this.newRepositorySimpleButton);
		this.mainLayoutControl.Controls.Add(this.newConnectionSimpleButton);
		this.mainLayoutControl.Controls.Add(this.separatorControl);
		this.mainLayoutControl.Controls.Add(this.description1LabelControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(120, 526, 855, 685);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.connectSimpleButton.ImageOptions.Image = Dataedo.App.Properties.Resources.connect_16;
		this.connectSimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
		this.connectSimpleButton.Location = new System.Drawing.Point(590, 416);
		this.connectSimpleButton.MaximumSize = new System.Drawing.Size(85, 29);
		this.connectSimpleButton.MinimumSize = new System.Drawing.Size(85, 29);
		this.connectSimpleButton.Name = "connectSimpleButton";
		this.connectSimpleButton.Size = new System.Drawing.Size(85, 29);
		this.connectSimpleButton.StyleController = this.mainLayoutControl;
		this.connectSimpleButton.TabIndex = 25;
		this.connectSimpleButton.Text = "Connect";
		this.connectSimpleButton.Click += new System.EventHandler(NextSimpleButton_Click);
		this.smallLogoUserControl.Location = new System.Drawing.Point(27, 418);
		this.smallLogoUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.smallLogoUserControl.MaximumSize = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.MinimumSize = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.Name = "smallLogoUserControl";
		this.smallLogoUserControl.Size = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.TabIndex = 24;
		this.gridControl.Location = new System.Drawing.Point(27, 73);
		this.gridControl.MainView = this.tileView;
		this.gridControl.MenuManager = this.barManager;
		this.gridControl.Name = "gridControl";
		this.gridControl.Size = new System.Drawing.Size(646, 305);
		this.gridControl.TabIndex = 23;
		this.gridControl.ToolTipController = this.toolTipController;
		this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tileView });
		this.gridControl.Load += new System.EventHandler(GridControl_Load);
		this.gridControl.MouseClick += new System.Windows.Forms.MouseEventHandler(GridControl_MouseClick);
		this.tileView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.iconTileViewColumn, this.displayNameTileViewColumn, this.legacyTileViewColumn });
		this.tileView.GridControl = this.gridControl;
		this.tileView.Name = "tileView";
		this.tileView.OptionsList.DrawItemSeparators = DevExpress.XtraGrid.Views.Tile.DrawItemSeparatorsMode.None;
		this.tileView.OptionsTiles.GroupTextPadding = new System.Windows.Forms.Padding(13, 8, 12, 8);
		this.tileView.OptionsTiles.IndentBetweenGroups = 0;
		this.tileView.OptionsTiles.IndentBetweenItems = 0;
		this.tileView.OptionsTiles.ItemPadding = new System.Windows.Forms.Padding(8);
		this.tileView.OptionsTiles.ItemSize = new System.Drawing.Size(300, 50);
		this.tileView.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.tileView.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tileView.OptionsTiles.Padding = new System.Windows.Forms.Padding(0);
		this.tileView.OptionsTiles.RowCount = 0;
		tableColumnDefinition.Length.Type = DevExpress.XtraEditors.TableLayout.TableDefinitionLengthType.Pixel;
		tableColumnDefinition.Length.Value = 32.0;
		tableColumnDefinition2.Length.Value = 185.0;
		tableColumnDefinition3.Length.Type = DevExpress.XtraEditors.TableLayout.TableDefinitionLengthType.Pixel;
		tableColumnDefinition3.Length.Value = 80.0;
		this.tileView.TileColumns.Add(tableColumnDefinition);
		this.tileView.TileColumns.Add(tableColumnDefinition2);
		this.tileView.TileColumns.Add(tableColumnDefinition3);
		tableRowDefinition.Length.Type = DevExpress.XtraEditors.TableLayout.TableDefinitionLengthType.Pixel;
		tableRowDefinition.Length.Value = 32.0;
		this.tileView.TileRows.Add(tableRowDefinition);
		tileViewItemElement.Column = this.iconTileViewColumn;
		tileViewItemElement.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement.ImageOptions.ImageSize = new System.Drawing.Size(32, 32);
		tileViewItemElement.Text = "iconTileViewColumn";
		tileViewItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement.Width = 64;
		tileViewItemElement2.Column = this.displayNameTileViewColumn;
		tileViewItemElement2.ColumnIndex = 1;
		tileViewItemElement2.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement2.Text = "displayNameTileViewColumn";
		tileViewItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement2.TextLocation = new System.Drawing.Point(15, 0);
		tileViewItemElement3.Column = this.legacyTileViewColumn;
		tileViewItemElement3.ColumnIndex = 2;
		tileViewItemElement3.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement3.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement3.Text = "legacyTileViewColumn";
		tileViewItemElement3.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileView.TileTemplate.Add(tileViewItemElement);
		this.tileView.TileTemplate.Add(tileViewItemElement2);
		this.tileView.TileTemplate.Add(tileViewItemElement3);
		this.tileView.ItemCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewItemCustomizeEventHandler(TileView_ItemCustomize);
		this.tileView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(TileView_FocusedRowChanged);
		this.tileView.KeyDown += new System.Windows.Forms.KeyEventHandler(TileView_KeyDown);
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[2] { this.removeBarButtonItem, this.connectBarButtonItem });
		this.barManager.MaxItemId = 2;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(700, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 470);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(700, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 470);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(700, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 470);
		this.removeBarButtonItem.Caption = "Remove";
		this.removeBarButtonItem.Id = 0;
		this.removeBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeBarButtonItem.Name = "removeBarButtonItem";
		this.removeBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(RemoveBarButtonItem_ItemClick);
		this.connectBarButtonItem.Caption = "Connect";
		this.connectBarButtonItem.Id = 1;
		this.connectBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.connect_16;
		this.connectBarButtonItem.Name = "connectBarButtonItem";
		this.connectBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ConnectBarButtonItem_ItemClick);
		this.toolTipController.AllowHtmlText = true;
		this.toolTipController.Appearance.Options.UseTextOptions = true;
		this.toolTipController.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.toolTipController.AutoPopDelay = 32000;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.toolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(ToolTipController_GetActiveObjectInfo);
		this.topSeparatorControl.Location = new System.Drawing.Point(27, 47);
		this.topSeparatorControl.MaximumSize = new System.Drawing.Size(0, 22);
		this.topSeparatorControl.MinimumSize = new System.Drawing.Size(0, 22);
		this.topSeparatorControl.Name = "topSeparatorControl";
		this.topSeparatorControl.Size = new System.Drawing.Size(646, 22);
		this.topSeparatorControl.TabIndex = 22;
		this.newRepositorySimpleButton.ImageOptions.Image = Dataedo.App.Properties.Resources.add_16;
		this.newRepositorySimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
		this.newRepositorySimpleButton.Location = new System.Drawing.Point(252, 416);
		this.newRepositorySimpleButton.MaximumSize = new System.Drawing.Size(110, 29);
		this.newRepositorySimpleButton.MinimumSize = new System.Drawing.Size(110, 29);
		this.newRepositorySimpleButton.Name = "newRepositorySimpleButton";
		this.newRepositorySimpleButton.Size = new System.Drawing.Size(110, 29);
		this.newRepositorySimpleButton.StyleController = this.mainLayoutControl;
		this.newRepositorySimpleButton.TabIndex = 20;
		this.newRepositorySimpleButton.Text = "New repository";
		this.newRepositorySimpleButton.Click += new System.EventHandler(CreateNewRepositorySimpleButton_Click);
		this.newConnectionSimpleButton.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.newConnectionSimpleButton.Appearance.Options.UseFont = true;
		this.newConnectionSimpleButton.ImageOptions.Image = Dataedo.App.Properties.Resources.connect_new_16;
		this.newConnectionSimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
		this.newConnectionSimpleButton.Location = new System.Drawing.Point(132, 416);
		this.newConnectionSimpleButton.MaximumSize = new System.Drawing.Size(110, 29);
		this.newConnectionSimpleButton.MinimumSize = new System.Drawing.Size(110, 29);
		this.newConnectionSimpleButton.Name = "newConnectionSimpleButton";
		this.newConnectionSimpleButton.Size = new System.Drawing.Size(110, 29);
		this.newConnectionSimpleButton.StyleController = this.mainLayoutControl;
		this.newConnectionSimpleButton.TabIndex = 19;
		this.newConnectionSimpleButton.Text = "New connection";
		this.newConnectionSimpleButton.Click += new System.EventHandler(ConnectToRepositorySimpleButton_Click);
		this.separatorControl.Location = new System.Drawing.Point(27, 382);
		this.separatorControl.MaximumSize = new System.Drawing.Size(0, 22);
		this.separatorControl.MinimumSize = new System.Drawing.Size(0, 22);
		this.separatorControl.Name = "separatorControl";
		this.separatorControl.Size = new System.Drawing.Size(646, 22);
		this.separatorControl.TabIndex = 14;
		this.description1LabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.description1LabelControl.Appearance.Options.UseFont = true;
		this.description1LabelControl.Appearance.Options.UseTextOptions = true;
		this.description1LabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
		this.description1LabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.description1LabelControl.Location = new System.Drawing.Point(27, 27);
		this.description1LabelControl.Name = "description1LabelControl";
		this.description1LabelControl.Size = new System.Drawing.Size(646, 16);
		this.description1LabelControl.StyleController = this.mainLayoutControl;
		this.description1LabelControl.TabIndex = 10;
		this.description1LabelControl.Text = "Recent connections:";
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[12]
		{
			this.logoPictureEditTopSeparatorEmptySpaceItem, this.logoPictureEditRightSeparatorEmptySpaceItem, this.description1LabelControlLayoutControlItem, this.separatorControlLayoutControlItem, this.buttonsEmptySpaceItem, this.topSeparatorControlLayoutControlItem, this.gridControlLayoutControlItem, this.smallLogoUserControlLayoutControlItem, this.nextSimpleButtonLayoutControlItem, this.connectToRepositorySimpleButtonLayoutControlItem,
			this.createNewRepositorySimpleButtonLayoutControlItem, this.emptySpaceItem1
		});
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 25, 25);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControlGroup.TextVisible = false;
		this.logoPictureEditTopSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.logoPictureEditTopSeparatorEmptySpaceItem.Location = new System.Drawing.Point(0, 381);
		this.logoPictureEditTopSeparatorEmptySpaceItem.MaxSize = new System.Drawing.Size(267, 10);
		this.logoPictureEditTopSeparatorEmptySpaceItem.MinSize = new System.Drawing.Size(267, 10);
		this.logoPictureEditTopSeparatorEmptySpaceItem.Name = "logoPictureEditTopSeparatorEmptySpaceItem";
		this.logoPictureEditTopSeparatorEmptySpaceItem.Size = new System.Drawing.Size(337, 10);
		this.logoPictureEditTopSeparatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.logoPictureEditTopSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.logoPictureEditRightSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.logoPictureEditRightSeparatorEmptySpaceItem.Location = new System.Drawing.Point(337, 381);
		this.logoPictureEditRightSeparatorEmptySpaceItem.Name = "logoPictureEditRightSeparatorEmptySpaceItem";
		this.logoPictureEditRightSeparatorEmptySpaceItem.Size = new System.Drawing.Size(218, 39);
		this.logoPictureEditRightSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.description1LabelControlLayoutControlItem.Control = this.description1LabelControl;
		this.description1LabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.description1LabelControlLayoutControlItem.Name = "description1LabelControlLayoutControlItem";
		this.description1LabelControlLayoutControlItem.Size = new System.Drawing.Size(650, 20);
		this.description1LabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.description1LabelControlLayoutControlItem.TextVisible = false;
		this.separatorControlLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.separatorControlLayoutControlItem.Control = this.separatorControl;
		this.separatorControlLayoutControlItem.Location = new System.Drawing.Point(0, 355);
		this.separatorControlLayoutControlItem.Name = "separatorControlLayoutControlItem";
		this.separatorControlLayoutControlItem.Size = new System.Drawing.Size(650, 26);
		this.separatorControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.SupportHorzAlignment;
		this.separatorControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.separatorControlLayoutControlItem.TextVisible = false;
		this.buttonsEmptySpaceItem.AllowHotTrack = false;
		this.buttonsEmptySpaceItem.Location = new System.Drawing.Point(555, 381);
		this.buttonsEmptySpaceItem.Name = "buttonsEmptySpaceItem";
		this.buttonsEmptySpaceItem.Size = new System.Drawing.Size(95, 10);
		this.buttonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.topSeparatorControlLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.topSeparatorControlLayoutControlItem.Control = this.topSeparatorControl;
		this.topSeparatorControlLayoutControlItem.Location = new System.Drawing.Point(0, 20);
		this.topSeparatorControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 26);
		this.topSeparatorControlLayoutControlItem.MinSize = new System.Drawing.Size(24, 26);
		this.topSeparatorControlLayoutControlItem.Name = "topSeparatorControlLayoutControlItem";
		this.topSeparatorControlLayoutControlItem.Size = new System.Drawing.Size(650, 26);
		this.topSeparatorControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.topSeparatorControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.topSeparatorControlLayoutControlItem.TextVisible = false;
		this.gridControlLayoutControlItem.Control = this.gridControl;
		this.gridControlLayoutControlItem.Location = new System.Drawing.Point(0, 46);
		this.gridControlLayoutControlItem.Name = "gridControlLayoutControlItem";
		this.gridControlLayoutControlItem.Size = new System.Drawing.Size(650, 309);
		this.gridControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.gridControlLayoutControlItem.TextVisible = false;
		this.smallLogoUserControlLayoutControlItem.Control = this.smallLogoUserControl;
		this.smallLogoUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 391);
		this.smallLogoUserControlLayoutControlItem.Name = "smallLogoUserControlLayoutControlItem";
		this.smallLogoUserControlLayoutControlItem.Size = new System.Drawing.Size(97, 29);
		this.smallLogoUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.smallLogoUserControlLayoutControlItem.TextVisible = false;
		this.nextSimpleButtonLayoutControlItem.Control = this.connectSimpleButton;
		this.nextSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(555, 391);
		this.nextSimpleButtonLayoutControlItem.Name = "nextSimpleButtonLayoutControlItem";
		this.nextSimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 0, 0, 0);
		this.nextSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(95, 29);
		this.nextSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.nextSimpleButtonLayoutControlItem.TextVisible = false;
		this.connectToRepositorySimpleButtonLayoutControlItem.Control = this.newConnectionSimpleButton;
		this.connectToRepositorySimpleButtonLayoutControlItem.Location = new System.Drawing.Point(107, 391);
		this.connectToRepositorySimpleButtonLayoutControlItem.Name = "connectToRepositorySimpleButtonLayoutControlItem";
		this.connectToRepositorySimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.connectToRepositorySimpleButtonLayoutControlItem.Size = new System.Drawing.Size(110, 29);
		this.connectToRepositorySimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.connectToRepositorySimpleButtonLayoutControlItem.TextVisible = false;
		this.createNewRepositorySimpleButtonLayoutControlItem.Control = this.newRepositorySimpleButton;
		this.createNewRepositorySimpleButtonLayoutControlItem.Location = new System.Drawing.Point(217, 391);
		this.createNewRepositorySimpleButtonLayoutControlItem.Name = "createNewRepositorySimpleButtonLayoutControlItem";
		this.createNewRepositorySimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 0, 0, 0);
		this.createNewRepositorySimpleButtonLayoutControlItem.Size = new System.Drawing.Size(120, 29);
		this.createNewRepositorySimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.createNewRepositorySimpleButtonLayoutControlItem.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(97, 391);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(10, 29);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.recentMenuImageCollection.ImageSize = new System.Drawing.Size(32, 32);
		this.recentMenuImageCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("recentMenuImageCollection.ImageStream");
		this.recentMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.file_repository_32, "file_repository_32", typeof(Dataedo.App.Properties.Resources), 0);
		this.recentMenuImageCollection.Images.SetKeyName(0, "file_repository_32");
		this.recentMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.server_repository_32, "server_repository_32", typeof(Dataedo.App.Properties.Resources), 1);
		this.recentMenuImageCollection.Images.SetKeyName(1, "server_repository_32");
		this.recentMenuImageCollection.Images.SetKeyName(2, "connect_new_16.png");
		this.popupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.connectBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeBarButtonItem)
		});
		this.popupMenu.Manager = this.barManager;
		this.popupMenu.Name = "popupMenu";
		this.legacyTileViewColumn.Caption = "Legacy";
		this.legacyTileViewColumn.FieldName = "Legacy";
		this.legacyTileViewColumn.Name = "legacyTileViewColumn";
		this.legacyTileViewColumn.Visible = true;
		this.legacyTileViewColumn.VisibleIndex = 2;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "RecentPageUserControl";
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.topSeparatorControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.separatorControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.separatorControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.topSeparatorControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nextSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectToRepositorySimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.createNewRepositorySimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.recentMenuImageCollection).EndInit();
		((System.ComponentModel.ISupportInitialize)this.popupMenu).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
