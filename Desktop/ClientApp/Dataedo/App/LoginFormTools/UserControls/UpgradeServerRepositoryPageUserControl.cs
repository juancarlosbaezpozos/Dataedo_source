using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.LoginFormTools.Tools.CustomEventArgs;
using Dataedo.App.LoginFormTools.Tools.Enums;
using Dataedo.App.LoginFormTools.Tools.Recent;
using Dataedo.App.LoginFormTools.Tools.Repository;
using Dataedo.App.LoginFormTools.Tools.Repository.SqlServer;
using Dataedo.App.LoginFormTools.UserControls.Base;
using Dataedo.App.LoginFormTools.UserControls.Common;
using Dataedo.App.LoginFormTools.UserControls.Subcontrols;
using Dataedo.App.Properties;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.LoginFormTools.UserControls;

public class UpgradeServerRepositoryPageUserControl : BasePageUserControl
{
	private readonly UpgradeSqlServerRepositoryUserControl upgradeControl;

	private UpgradeDataModel upgradeDataModel;

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

	private LayoutControlItem createRepositorySimpleButtonLayoutControlItem;

	private PanelControl contentPanelControl;

	private LayoutControlItem contentPanelControlLayoutControlItem;

	private XtraScrollableControl xtraScrollableControl;

	private SmallLogoUserControl smallLogoUserControl;

	private LayoutControlItem smallLogoUserControlLayoutControlItem;

	private ToolTipController toolTipController;

	private EmptySpaceItem separatorEmptySpaceItem;

	public UpgradeServerRepositoryPageUserControl()
	{
		InitializeComponent();
		upgradeControl = new UpgradeSqlServerRepositoryUserControl();
		upgradeControl.Dock = DockStyle.Fill;
		xtraScrollableControl.SuspendLayout();
		xtraScrollableControl.Controls.Clear();
		xtraScrollableControl.Controls.Add(upgradeControl);
		xtraScrollableControl.ResumeLayout(performLayout: false);
	}

	internal override void SetParameter(object parameter, bool isCalledAsPrevious)
	{
		base.SetParameter(parameter, isCalledAsPrevious);
		if (!base.IsCalledAsPrevious || parameter != null)
		{
			upgradeDataModel = parameter as UpgradeDataModel;
		}
	}

	internal override async Task<bool> Navigated()
	{
		try
		{
			await base.Navigated();
			ShowLoader();
			if (!base.IsCalledAsPrevious)
			{
				upgradeControl.SetRecentData(upgradeDataModel.RecentItemModel);
			}
			if (upgradeDataModel.RecentItemModel.Data.HasCompleteConnectionInfo)
			{
				upgradeRepositorySimpleButton.Focus();
			}
		}
		finally
		{
			HideLoader();
		}
		return true;
	}

	internal RecentItemModel GetRecentProject()
	{
		return new RecentItemModel(RecentSupport.GetRecentProject(upgradeControl));
	}

	protected override void Dispose(bool disposing)
	{
		upgradeControl?.Dispose();
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void BackSimpleButton_Click(object sender, EventArgs e)
	{
		OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.Back));
	}

	private void UpgradeRepositorySimpleButton_Click(object sender, EventArgs e)
	{
		try
		{
			OnTimeConsumingOperationStarted(this);
			if (ProcessUpgrade())
			{
				OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.UpgradeServerRepositoryConnectedUsers, new RepositoryOperation(upgradeControl.Instructions, upgradeControl.ServerConnectionString, upgradeControl.Database, GetRecentProject(), upgradeDataModel.RepositoryVersion)));
			}
		}
		finally
		{
			OnTimeConsumingOperationStopped(this);
		}
	}

	private void Description1LabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.CreateFileRepository));
	}

	private bool ProcessUpgrade()
	{
		if (!upgradeControl.ValidateRequiredFields())
		{
			return false;
		}
		upgradeControl.PrepareServerConnectionStringBuilder();
		if (!upgradeControl.CheckConnectionToServer())
		{
			return false;
		}
		bool? isServerOnAzure = null;
		if (!SqlServerTools.CheckIfServerIsValid(upgradeControl.ServerConnectionString, upgradeControl.Database, shouldExist: true, ref isServerOnAzure, FindForm()) || !isServerOnAzure.HasValue)
		{
			return false;
		}
		if (!upgradeControl.PrepareInstructions(upgradeDataModel?.DetachWebCatalog ?? false))
		{
			return false;
		}
		return true;
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.smallLogoUserControl = new Dataedo.App.LoginFormTools.UserControls.Common.SmallLogoUserControl();
		this.contentPanelControl = new DevExpress.XtraEditors.PanelControl();
		this.xtraScrollableControl = new DevExpress.XtraEditors.XtraScrollableControl();
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
		this.createRepositorySimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.contentPanelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.smallLogoUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.separatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.contentPanelControl).BeginInit();
		this.contentPanelControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.backSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.createRepositorySimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.contentPanelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.smallLogoUserControl);
		this.mainLayoutControl.Controls.Add(this.contentPanelControl);
		this.mainLayoutControl.Controls.Add(this.upgradeRepositorySimpleButton);
		this.mainLayoutControl.Controls.Add(this.backSimpleButton);
		this.mainLayoutControl.Controls.Add(this.description1LabelControl);
		this.mainLayoutControl.Controls.Add(this.headerLabelControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(120, 526, 855, 685);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.mainLayoutControl.ToolTipController = this.toolTipController;
		this.smallLogoUserControl.Location = new System.Drawing.Point(27, 419);
		this.smallLogoUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.smallLogoUserControl.MaximumSize = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.MinimumSize = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.Name = "smallLogoUserControl";
		this.smallLogoUserControl.Size = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.TabIndex = 22;
		this.contentPanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.contentPanelControl.Controls.Add(this.xtraScrollableControl);
		this.contentPanelControl.Location = new System.Drawing.Point(27, 144);
		this.contentPanelControl.Name = "contentPanelControl";
		this.contentPanelControl.Size = new System.Drawing.Size(646, 243);
		this.contentPanelControl.TabIndex = 21;
		this.xtraScrollableControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraScrollableControl.Location = new System.Drawing.Point(0, 0);
		this.xtraScrollableControl.Name = "xtraScrollableControl";
		this.xtraScrollableControl.Size = new System.Drawing.Size(646, 243);
		this.xtraScrollableControl.TabIndex = 0;
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
		this.description1LabelControl.Size = new System.Drawing.Size(646, 32);
		this.description1LabelControl.StyleController = this.mainLayoutControl;
		this.description1LabelControl.TabIndex = 10;
		this.description1LabelControl.Text = "This wizard will upgrade your repository.\r\nIt is recommended to <b>backup repository</b> first.";
		this.description1LabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(Description1LabelControl_HyperlinkClick);
		this.headerLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 24f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.headerLabelControl.Appearance.Options.UseFont = true;
		this.headerLabelControl.AutoEllipsis = true;
		this.headerLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.headerLabelControl.Location = new System.Drawing.Point(27, 25);
		this.headerLabelControl.Name = "headerLabelControl";
		this.headerLabelControl.Size = new System.Drawing.Size(646, 39);
		this.headerLabelControl.StyleController = this.mainLayoutControl;
		this.headerLabelControl.TabIndex = 8;
		this.headerLabelControl.Text = "Upgrade repository";
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[10] { this.logoPictureEditTopSeparatorEmptySpaceItem, this.logoPictureEditRightSeparatorEmptySpaceItem, this.headerLabelControlLayoutControlItem, this.description1LabelControlLayoutControlItem, this.backSimpleButtonLayoutControlItem, this.buttonsEmptySpaceItem, this.createRepositorySimpleButtonLayoutControlItem, this.contentPanelControlLayoutControlItem, this.smallLogoUserControlLayoutControlItem, this.separatorEmptySpaceItem });
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 25, 25);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControlGroup.TextVisible = false;
		this.logoPictureEditTopSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.logoPictureEditTopSeparatorEmptySpaceItem.Location = new System.Drawing.Point(0, 364);
		this.logoPictureEditTopSeparatorEmptySpaceItem.MinSize = new System.Drawing.Size(1, 10);
		this.logoPictureEditTopSeparatorEmptySpaceItem.Name = "logoPictureEditTopSeparatorEmptySpaceItem";
		this.logoPictureEditTopSeparatorEmptySpaceItem.Size = new System.Drawing.Size(97, 28);
		this.logoPictureEditTopSeparatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.logoPictureEditTopSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.logoPictureEditRightSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.logoPictureEditRightSeparatorEmptySpaceItem.Location = new System.Drawing.Point(97, 364);
		this.logoPictureEditRightSeparatorEmptySpaceItem.Name = "logoPictureEditRightSeparatorEmptySpaceItem";
		this.logoPictureEditRightSeparatorEmptySpaceItem.Size = new System.Drawing.Size(388, 56);
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
		this.description1LabelControlLayoutControlItem.Size = new System.Drawing.Size(650, 36);
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
		this.buttonsEmptySpaceItem.Location = new System.Drawing.Point(485, 364);
		this.buttonsEmptySpaceItem.Name = "buttonsEmptySpaceItem";
		this.buttonsEmptySpaceItem.Size = new System.Drawing.Size(165, 27);
		this.buttonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.createRepositorySimpleButtonLayoutControlItem.Control = this.upgradeRepositorySimpleButton;
		this.createRepositorySimpleButtonLayoutControlItem.Location = new System.Drawing.Point(555, 391);
		this.createRepositorySimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(95, 29);
		this.createRepositorySimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(95, 29);
		this.createRepositorySimpleButtonLayoutControlItem.Name = "createRepositorySimpleButtonLayoutControlItem";
		this.createRepositorySimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 0, 0, 0);
		this.createRepositorySimpleButtonLayoutControlItem.Size = new System.Drawing.Size(95, 29);
		this.createRepositorySimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.createRepositorySimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.createRepositorySimpleButtonLayoutControlItem.TextVisible = false;
		this.contentPanelControlLayoutControlItem.Control = this.contentPanelControl;
		this.contentPanelControlLayoutControlItem.Location = new System.Drawing.Point(0, 117);
		this.contentPanelControlLayoutControlItem.Name = "contentPanelControlLayoutControlItem";
		this.contentPanelControlLayoutControlItem.Size = new System.Drawing.Size(650, 247);
		this.contentPanelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.contentPanelControlLayoutControlItem.TextVisible = false;
		this.smallLogoUserControlLayoutControlItem.Control = this.smallLogoUserControl;
		this.smallLogoUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 392);
		this.smallLogoUserControlLayoutControlItem.MaxSize = new System.Drawing.Size(97, 28);
		this.smallLogoUserControlLayoutControlItem.MinSize = new System.Drawing.Size(97, 28);
		this.smallLogoUserControlLayoutControlItem.Name = "smallLogoUserControlLayoutControlItem";
		this.smallLogoUserControlLayoutControlItem.Size = new System.Drawing.Size(97, 28);
		this.smallLogoUserControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.smallLogoUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.smallLogoUserControlLayoutControlItem.TextVisible = false;
		this.separatorEmptySpaceItem.AllowHotTrack = false;
		this.separatorEmptySpaceItem.Location = new System.Drawing.Point(0, 91);
		this.separatorEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 26);
		this.separatorEmptySpaceItem.MinSize = new System.Drawing.Size(104, 26);
		this.separatorEmptySpaceItem.Name = "separatorEmptySpaceItem";
		this.separatorEmptySpaceItem.Size = new System.Drawing.Size(650, 26);
		this.separatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.separatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Name = "UpgradeServerRepositoryPageUserControl";
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.contentPanelControl).EndInit();
		this.contentPanelControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.backSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.createRepositorySimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.contentPanelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).EndInit();
		base.ResumeLayout(false);
	}
}
