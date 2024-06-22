using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.LoginFormTools.Tools.CustomEventArgs;
using Dataedo.App.LoginFormTools.Tools.Enums;
using Dataedo.App.LoginFormTools.Tools.Repository;
using Dataedo.App.LoginFormTools.Tools.Repository.RepositoryCreator;
using Dataedo.App.LoginFormTools.Tools.Repository.RepositoryCreator.Data;
using Dataedo.App.LoginFormTools.UserControls.Base;
using Dataedo.App.LoginFormTools.UserControls.Common;
using Dataedo.App.Properties;
using Dataedo.CustomControls;
using Dataedo.LicenseHelperLibrary.Repository;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.LoginFormTools.UserControls;

public class UpgradeServerRepositoryConnectedUsersPageUserControl : BasePageUserControl
{
	private readonly string defaultDescription1;

	private RepositoryOperation repositoryOperation;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private EmptySpaceItem logoPictureEditTopSeparatorEmptySpaceItem;

	private EmptySpaceItem logoPictureEditRightSeparatorEmptySpaceItem;

	private LabelControl headerLabelControl;

	private LayoutControlItem headerLabelControlLayoutControlItem;

	private LabelControl description1LabelControl;

	private LayoutControlItem description1LabelControlLayoutControlItem;

	private SimpleButton backSimpleButton;

	private LayoutControlItem backSimpleButtonLayoutControlItem;

	private EmptySpaceItem buttonsEmptySpaceItem;

	private SimpleButton upgradeRepositorySimpleButton;

	private LayoutControlItem connectToRepositorySimpleButtonLayoutControlItem;

	private SmallLogoUserControl smallLogoUserControl;

	private LayoutControlItem smallLogoUserControlLayoutControlItem;

	private ToolTipController toolTipController;

	private LabelControl connectedUsersLabelControl;

	private LayoutControlItem connectedUsersLabelControlLayoutControlItem;

	private LayoutControlGroup Root;

	private GridControl connectedUsersGrid;

	private GridView upgradeGridView;

	private GridColumn hostUpgradeGridColumn;

	private GridColumn loginUpgradeGridColumn;

	private GridColumn applicationUpgradeGridColumn;

	private LayoutControlItem connectedUsersGridLayoutControlItem;

	private SimpleButton refreshSimpleButton;

	private LayoutControlItem refreshSimpleButtonLayoutControlItem;

	private EmptySpaceItem separatorEmptySpaceItem;

	public UpgradeServerRepositoryConnectedUsersPageUserControl()
	{
		InitializeComponent();
		defaultDescription1 = description1LabelControl.Text;
	}

	internal override void SetParameter(object parameter, bool isCalledAsPrevious)
	{
		base.SetParameter(parameter, isCalledAsPrevious);
		description1LabelControl.Text = defaultDescription1;
		connectedUsersGrid.BeginUpdate();
		connectedUsersGrid.DataSource = null;
		connectedUsersGrid.EndUpdate();
		connectedUsersGrid.Refresh();
		repositoryOperation = parameter as RepositoryOperation;
	}

	internal override async Task<bool> Navigated()
	{
		try
		{
			await base.Navigated();
			ShowLoader();
			Dataedo.LicenseHelperLibrary.Repository.RepositoryVersion repositoryVersion = repositoryOperation.RepositoryVersion;
			string text = (repositoryVersion.Stable ? string.Empty : " <b>(UNSTABLE)</b>");
			description1LabelControl.Text = "This repository is in version " + $"{repositoryVersion.Version}.{repositoryVersion.Update}.{repositoryVersion.Build}" + text + " which is <b>incompatible</b> with the program version." + Environment.NewLine + "Do you want to <b>upgrade repository?</b>" + Environment.NewLine + Environment.NewLine + "It is recommended to <b>backup repository first</b>.";
			if (RefreshConnectedUsersList())
			{
				ProcessUpgrade();
			}
		}
		finally
		{
			HideLoader();
		}
		return true;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void BackSimpleButton_Click(object sender, EventArgs e)
	{
		OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.Back, repositoryOperation.RecentItemModel));
	}

	private void UpgradeRepositorySimpleButton_Click(object sender, EventArgs e)
	{
		ProcessUpgrade();
	}

	private void ProcessUpgrade()
	{
		try
		{
			OnTimeConsumingOperationStarted(this);
			OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.UpgradeRepositoryProgress, repositoryOperation));
		}
		finally
		{
			OnTimeConsumingOperationStopped(this);
		}
	}

	private void RefreshSimpleButton_Click(object sender, EventArgs e)
	{
		try
		{
			ShowLoader();
			RefreshConnectedUsersList();
		}
		finally
		{
			HideLoader();
		}
	}

	private bool RefreshConnectedUsersList()
	{
		List<ConnectedUser> connectedUsersList = GetConnectedUsersList();
		connectedUsersGrid.BeginUpdate();
		connectedUsersGrid.DataSource = connectedUsersList;
		connectedUsersGrid.EndUpdate();
		connectedUsersGrid.Refresh();
		return connectedUsersList.Count == 0;
	}

	private List<ConnectedUser> GetConnectedUsersList()
	{
		SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(repositoryOperation.ConnectionString);
		return DatabaseHelper.GetConnectedUsers(repositoryOperation.ConnectionString, sqlConnectionStringBuilder.InitialCatalog, sqlConnectionStringBuilder.DataSource, sqlConnectionStringBuilder.ApplicationName);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.refreshSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.connectedUsersGrid = new DevExpress.XtraGrid.GridControl();
		this.upgradeGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.hostUpgradeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.loginUpgradeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.applicationUpgradeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.connectedUsersLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.smallLogoUserControl = new Dataedo.App.LoginFormTools.UserControls.Common.SmallLogoUserControl();
		this.upgradeRepositorySimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.backSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.description1LabelControl = new DevExpress.XtraEditors.LabelControl();
		this.headerLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.logoPictureEditTopSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.logoPictureEditRightSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.headerLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.description1LabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.backSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.buttonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.connectToRepositorySimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.smallLogoUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.connectedUsersLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.connectedUsersGridLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.refreshSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.separatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.connectedUsersGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.upgradeGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.backSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectToRepositorySimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectedUsersLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectedUsersGridLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.refreshSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.refreshSimpleButton);
		this.mainLayoutControl.Controls.Add(this.connectedUsersGrid);
		this.mainLayoutControl.Controls.Add(this.connectedUsersLabelControl);
		this.mainLayoutControl.Controls.Add(this.smallLogoUserControl);
		this.mainLayoutControl.Controls.Add(this.upgradeRepositorySimpleButton);
		this.mainLayoutControl.Controls.Add(this.backSimpleButton);
		this.mainLayoutControl.Controls.Add(this.description1LabelControl);
		this.mainLayoutControl.Controls.Add(this.headerLabelControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3543, 251, 855, 685);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.mainLayoutControl.ToolTipController = this.toolTipController;
		this.refreshSimpleButton.ImageOptions.Image = Dataedo.App.Properties.Resources.refresh_16;
		this.refreshSimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
		this.refreshSimpleButton.Location = new System.Drawing.Point(603, 366);
		this.refreshSimpleButton.MaximumSize = new System.Drawing.Size(70, 29);
		this.refreshSimpleButton.MinimumSize = new System.Drawing.Size(70, 29);
		this.refreshSimpleButton.Name = "refreshSimpleButton";
		this.refreshSimpleButton.Size = new System.Drawing.Size(70, 29);
		this.refreshSimpleButton.StyleController = this.mainLayoutControl;
		this.refreshSimpleButton.TabIndex = 27;
		this.refreshSimpleButton.Text = "Refresh";
		this.refreshSimpleButton.Click += new System.EventHandler(RefreshSimpleButton_Click);
		this.connectedUsersGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.connectedUsersGrid.Location = new System.Drawing.Point(27, 212);
		this.connectedUsersGrid.MainView = this.upgradeGridView;
		this.connectedUsersGrid.Name = "connectedUsersGrid";
		this.connectedUsersGrid.Size = new System.Drawing.Size(646, 150);
		this.connectedUsersGrid.TabIndex = 26;
		this.connectedUsersGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.upgradeGridView });
		this.upgradeGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.hostUpgradeGridColumn, this.loginUpgradeGridColumn, this.applicationUpgradeGridColumn });
		this.upgradeGridView.GridControl = this.connectedUsersGrid;
		this.upgradeGridView.Name = "upgradeGridView";
		this.upgradeGridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
		this.upgradeGridView.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
		this.upgradeGridView.OptionsBehavior.Editable = false;
		this.upgradeGridView.OptionsBehavior.ReadOnly = true;
		this.upgradeGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.upgradeGridView.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.upgradeGridView.OptionsView.ShowGroupPanel = false;
		this.upgradeGridView.OptionsView.ShowIndicator = false;
		this.hostUpgradeGridColumn.Caption = "Server name";
		this.hostUpgradeGridColumn.FieldName = "Host";
		this.hostUpgradeGridColumn.Name = "hostUpgradeGridColumn";
		this.hostUpgradeGridColumn.Visible = true;
		this.hostUpgradeGridColumn.VisibleIndex = 0;
		this.hostUpgradeGridColumn.Width = 91;
		this.loginUpgradeGridColumn.Caption = "Login";
		this.loginUpgradeGridColumn.FieldName = "Login";
		this.loginUpgradeGridColumn.Name = "loginUpgradeGridColumn";
		this.loginUpgradeGridColumn.Visible = true;
		this.loginUpgradeGridColumn.VisibleIndex = 1;
		this.loginUpgradeGridColumn.Width = 117;
		this.applicationUpgradeGridColumn.Caption = "Application";
		this.applicationUpgradeGridColumn.FieldName = "Application";
		this.applicationUpgradeGridColumn.Name = "applicationUpgradeGridColumn";
		this.applicationUpgradeGridColumn.Visible = true;
		this.applicationUpgradeGridColumn.VisibleIndex = 2;
		this.applicationUpgradeGridColumn.Width = 254;
		this.connectedUsersLabelControl.AllowHtmlString = true;
		this.connectedUsersLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.connectedUsersLabelControl.Appearance.Options.UseFont = true;
		this.connectedUsersLabelControl.Appearance.Options.UseTextOptions = true;
		this.connectedUsersLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
		this.connectedUsersLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.connectedUsersLabelControl.Location = new System.Drawing.Point(27, 176);
		this.connectedUsersLabelControl.Name = "connectedUsersLabelControl";
		this.connectedUsersLabelControl.Size = new System.Drawing.Size(646, 32);
		this.connectedUsersLabelControl.StyleController = this.mainLayoutControl;
		this.connectedUsersLabelControl.TabIndex = 24;
		this.connectedUsersLabelControl.Text = "<b>Users connected to repository</b>\r\nWe advise that all users listed below disconnect before proceeding.";
		this.smallLogoUserControl.Location = new System.Drawing.Point(27, 419);
		this.smallLogoUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.smallLogoUserControl.MaximumSize = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.MinimumSize = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.Name = "smallLogoUserControl";
		this.smallLogoUserControl.Size = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.TabIndex = 22;
		this.upgradeRepositorySimpleButton.AllowFocus = false;
		this.upgradeRepositorySimpleButton.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.upgradeRepositorySimpleButton.Appearance.Options.UseFont = true;
		this.upgradeRepositorySimpleButton.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_top_green_16;
		this.upgradeRepositorySimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
		this.upgradeRepositorySimpleButton.Location = new System.Drawing.Point(590, 416);
		this.upgradeRepositorySimpleButton.MaximumSize = new System.Drawing.Size(85, 29);
		this.upgradeRepositorySimpleButton.MinimumSize = new System.Drawing.Size(85, 29);
		this.upgradeRepositorySimpleButton.Name = "upgradeRepositorySimpleButton";
		this.upgradeRepositorySimpleButton.Size = new System.Drawing.Size(85, 29);
		this.upgradeRepositorySimpleButton.StyleController = this.mainLayoutControl;
		this.upgradeRepositorySimpleButton.TabIndex = 20;
		this.upgradeRepositorySimpleButton.Text = "Upgrade";
		this.upgradeRepositorySimpleButton.Click += new System.EventHandler(UpgradeRepositorySimpleButton_Click);
		this.backSimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
		this.backSimpleButton.Location = new System.Drawing.Point(510, 416);
		this.backSimpleButton.MaximumSize = new System.Drawing.Size(70, 29);
		this.backSimpleButton.MinimumSize = new System.Drawing.Size(70, 29);
		this.backSimpleButton.Name = "backSimpleButton";
		this.backSimpleButton.Size = new System.Drawing.Size(70, 29);
		this.backSimpleButton.StyleController = this.mainLayoutControl;
		this.backSimpleButton.TabIndex = 18;
		this.backSimpleButton.Text = "Back";
		this.backSimpleButton.Click += new System.EventHandler(BackSimpleButton_Click);
		this.description1LabelControl.AllowHtmlString = true;
		this.description1LabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.description1LabelControl.Appearance.Options.UseFont = true;
		this.description1LabelControl.Appearance.Options.UseTextOptions = true;
		this.description1LabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
		this.description1LabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.description1LabelControl.Location = new System.Drawing.Point(27, 82);
		this.description1LabelControl.Name = "description1LabelControl";
		this.description1LabelControl.Size = new System.Drawing.Size(646, 64);
		this.description1LabelControl.StyleController = this.mainLayoutControl;
		this.description1LabelControl.TabIndex = 10;
		this.description1LabelControl.Text = "This repository is in version which is <b>incompatible</b> with the program version.\r\nDo you want to <b>upgrade repository</b>?\r\n\r\nIt is recommended to <b>backup repository database</b> first.";
		this.headerLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 24f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.headerLabelControl.Appearance.Options.UseFont = true;
		this.headerLabelControl.AutoEllipsis = true;
		this.headerLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.headerLabelControl.Location = new System.Drawing.Point(27, 25);
		this.headerLabelControl.Name = "headerLabelControl";
		this.headerLabelControl.Size = new System.Drawing.Size(646, 39);
		this.headerLabelControl.StyleController = this.mainLayoutControl;
		this.headerLabelControl.TabIndex = 8;
		this.headerLabelControl.Text = "Repository upgrade required";
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[12]
		{
			this.logoPictureEditTopSeparatorEmptySpaceItem, this.logoPictureEditRightSeparatorEmptySpaceItem, this.headerLabelControlLayoutControlItem, this.description1LabelControlLayoutControlItem, this.backSimpleButtonLayoutControlItem, this.buttonsEmptySpaceItem, this.connectToRepositorySimpleButtonLayoutControlItem, this.smallLogoUserControlLayoutControlItem, this.connectedUsersLabelControlLayoutControlItem, this.connectedUsersGridLayoutControlItem,
			this.refreshSimpleButtonLayoutControlItem, this.separatorEmptySpaceItem
		});
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 25, 25);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControlGroup.TextVisible = false;
		this.logoPictureEditTopSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.logoPictureEditTopSeparatorEmptySpaceItem.Location = new System.Drawing.Point(0, 372);
		this.logoPictureEditTopSeparatorEmptySpaceItem.MinSize = new System.Drawing.Size(1, 10);
		this.logoPictureEditTopSeparatorEmptySpaceItem.Name = "logoPictureEditTopSeparatorEmptySpaceItem";
		this.logoPictureEditTopSeparatorEmptySpaceItem.Size = new System.Drawing.Size(97, 20);
		this.logoPictureEditTopSeparatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.logoPictureEditTopSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.logoPictureEditRightSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.logoPictureEditRightSeparatorEmptySpaceItem.Location = new System.Drawing.Point(97, 372);
		this.logoPictureEditRightSeparatorEmptySpaceItem.Name = "logoPictureEditRightSeparatorEmptySpaceItem";
		this.logoPictureEditRightSeparatorEmptySpaceItem.Size = new System.Drawing.Size(388, 48);
		this.logoPictureEditRightSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.headerLabelControlLayoutControlItem.Control = this.headerLabelControl;
		this.headerLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.headerLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 55);
		this.headerLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 55);
		this.headerLabelControlLayoutControlItem.Name = "headerLabelControlLayoutControlItem";
		this.headerLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 16);
		this.headerLabelControlLayoutControlItem.Size = new System.Drawing.Size(650, 55);
		this.headerLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.headerLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.headerLabelControlLayoutControlItem.TextVisible = false;
		this.description1LabelControlLayoutControlItem.Control = this.description1LabelControl;
		this.description1LabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 55);
		this.description1LabelControlLayoutControlItem.Name = "description1LabelControlLayoutControlItem";
		this.description1LabelControlLayoutControlItem.Size = new System.Drawing.Size(650, 68);
		this.description1LabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.description1LabelControlLayoutControlItem.TextVisible = false;
		this.backSimpleButtonLayoutControlItem.Control = this.backSimpleButton;
		this.backSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(485, 391);
		this.backSimpleButtonLayoutControlItem.Name = "backSimpleButtonLayoutControlItem";
		this.backSimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.backSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(70, 29);
		this.backSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.backSimpleButtonLayoutControlItem.TextVisible = false;
		this.buttonsEmptySpaceItem.AllowHotTrack = false;
		this.buttonsEmptySpaceItem.Location = new System.Drawing.Point(485, 372);
		this.buttonsEmptySpaceItem.Name = "buttonsEmptySpaceItem";
		this.buttonsEmptySpaceItem.Size = new System.Drawing.Size(165, 19);
		this.buttonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.connectToRepositorySimpleButtonLayoutControlItem.Control = this.upgradeRepositorySimpleButton;
		this.connectToRepositorySimpleButtonLayoutControlItem.Location = new System.Drawing.Point(555, 391);
		this.connectToRepositorySimpleButtonLayoutControlItem.Name = "connectToRepositorySimpleButtonLayoutControlItem";
		this.connectToRepositorySimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 0, 0, 0);
		this.connectToRepositorySimpleButtonLayoutControlItem.Size = new System.Drawing.Size(95, 29);
		this.connectToRepositorySimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.connectToRepositorySimpleButtonLayoutControlItem.TextVisible = false;
		this.smallLogoUserControlLayoutControlItem.Control = this.smallLogoUserControl;
		this.smallLogoUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 392);
		this.smallLogoUserControlLayoutControlItem.MaxSize = new System.Drawing.Size(97, 28);
		this.smallLogoUserControlLayoutControlItem.MinSize = new System.Drawing.Size(97, 28);
		this.smallLogoUserControlLayoutControlItem.Name = "smallLogoUserControlLayoutControlItem";
		this.smallLogoUserControlLayoutControlItem.Size = new System.Drawing.Size(97, 28);
		this.smallLogoUserControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.smallLogoUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.smallLogoUserControlLayoutControlItem.TextVisible = false;
		this.connectedUsersLabelControlLayoutControlItem.Control = this.connectedUsersLabelControl;
		this.connectedUsersLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 149);
		this.connectedUsersLabelControlLayoutControlItem.Name = "connectedUsersLabelControlLayoutControlItem";
		this.connectedUsersLabelControlLayoutControlItem.Size = new System.Drawing.Size(650, 36);
		this.connectedUsersLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.connectedUsersLabelControlLayoutControlItem.TextVisible = false;
		this.connectedUsersGridLayoutControlItem.Control = this.connectedUsersGrid;
		this.connectedUsersGridLayoutControlItem.Location = new System.Drawing.Point(0, 185);
		this.connectedUsersGridLayoutControlItem.MaxSize = new System.Drawing.Size(650, 196);
		this.connectedUsersGridLayoutControlItem.MinSize = new System.Drawing.Size(1, 1);
		this.connectedUsersGridLayoutControlItem.Name = "connectedUsersGridLayoutControlItem";
		this.connectedUsersGridLayoutControlItem.Size = new System.Drawing.Size(650, 154);
		this.connectedUsersGridLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.connectedUsersGridLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.connectedUsersGridLayoutControlItem.TextVisible = false;
		this.refreshSimpleButtonLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.refreshSimpleButtonLayoutControlItem.Control = this.refreshSimpleButton;
		this.refreshSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(0, 339);
		this.refreshSimpleButtonLayoutControlItem.Name = "refreshSimpleButtonLayoutControlItem";
		this.refreshSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(650, 33);
		this.refreshSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.refreshSimpleButtonLayoutControlItem.TextVisible = false;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Location = new System.Drawing.Point(0, 0);
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(180, 120);
		this.Root.TextVisible = false;
		this.separatorEmptySpaceItem.AllowHotTrack = false;
		this.separatorEmptySpaceItem.Location = new System.Drawing.Point(0, 123);
		this.separatorEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 26);
		this.separatorEmptySpaceItem.MinSize = new System.Drawing.Size(104, 26);
		this.separatorEmptySpaceItem.Name = "separatorEmptySpaceItem";
		this.separatorEmptySpaceItem.Size = new System.Drawing.Size(650, 26);
		this.separatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.separatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Name = "UpgradeServerRepositoryConnectedUsersPageUserControl";
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.connectedUsersGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.upgradeGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.backSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectToRepositorySimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectedUsersLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectedUsersGridLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.refreshSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).EndInit();
		base.ResumeLayout(false);
	}
}
