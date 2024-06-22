using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.LoginFormTools.Tools.Common;
using Dataedo.App.LoginFormTools.Tools.CustomEventArgs;
using Dataedo.App.LoginFormTools.Tools.Enums;
using Dataedo.App.LoginFormTools.UserControls.Base;
using Dataedo.App.LoginFormTools.UserControls.Common;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.LoginFormTools.UserControls;

public class GettingStartedPageUserControl : BasePageUserControl
{
    private IContainer components;

    private NonCustomizableLayoutControl mainLayoutControl;

    private LayoutControlGroup mainLayoutControlGroup;

    private EmptySpaceItem logoPictureEditTopSeparatorEmptySpaceItem;

    private EmptySpaceItem logoPictureEditRightSeparatorEmptySpaceItem;

    private LabelControl headerLabelControl;

    private LayoutControlItem headerLabelControlLayoutControlItem;

    private PictureEdit rightImagePictureEdit;

    private LayoutControlItem rightImagePictureEditLayoutControlItem;

    private LabelControl description2LabelControl;

    private LabelControl description1LabelControl;

    private LayoutControlItem description1LabelControlLayoutControlItem;

    private LayoutControlItem description2LabelControlLayoutControlItem;

    private EmptySpaceItem imageSeparatorEmptySpaceItem;

    private SimpleButton createNewRepositorySimpleButton;

    private SimpleButton connectToRepositorySimpleButton;

    private LayoutControlItem connectToRepositorySimpleButtonLayoutControlItem;

    private LayoutControlItem createNewRepositorySimpleButtonLayoutControlItem;

    private SmallLogoUserControl smallLogoUserControl;

    private LayoutControlItem smallLogoUserControlLayoutControlItem;

    private EmptySpaceItem separatorEmptySpaceItem;

    public GettingStartedPageUserControl()
    {
        InitializeComponent();
    }

    internal override async Task<bool> Navigated()
    {
        bool result = await base.Navigated();
        createNewRepositorySimpleButton.Focus();
        return result;
    }

    private void GettingStartedPageUserControl_Load(object sender, EventArgs e)
    {
        if (!base.DesignMode && base.ParentForm != null)
        {
            rightImagePictureEdit.BackColor = base.ParentForm.BackColor;
        }
    }

    private void LogoPictureEdit_MouseClick(object sender, MouseEventArgs e)
    {
        OpeningLinks.OpenDataedoLink(e);
    }

    private void ConnectToRepositorySimpleButton_Click(object sender, EventArgs e)
    {
        TrackingRunner.Track(delegate
        {
            TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingConnectToRepositoryRepoParameters(isFile: false, forceEmptyRepoType: true), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.CreatorExisting);
        });
        OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.ConnectToRepository));
    }

    private void CreateNewRepositorySimpleButton_Click(object sender, EventArgs e)
    {
        TrackingRunner.Track(delegate
        {
            TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilder(new TrackingUserParameters(), new TrackingDataedoParameters()), TrackingEventEnum.CreatorNew);
        });
        OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.CreateRepository));
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
        this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
        this.smallLogoUserControl = new Dataedo.App.LoginFormTools.UserControls.Common.SmallLogoUserControl();
        this.createNewRepositorySimpleButton = new DevExpress.XtraEditors.SimpleButton();
        this.connectToRepositorySimpleButton = new DevExpress.XtraEditors.SimpleButton();
        this.description2LabelControl = new DevExpress.XtraEditors.LabelControl();
        this.description1LabelControl = new DevExpress.XtraEditors.LabelControl();
        this.rightImagePictureEdit = new DevExpress.XtraEditors.PictureEdit();
        this.headerLabelControl = new DevExpress.XtraEditors.LabelControl();
        this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
        this.logoPictureEditTopSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
        this.logoPictureEditRightSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
        this.headerLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.rightImagePictureEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.description1LabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.description2LabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.imageSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
        this.connectToRepositorySimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.createNewRepositorySimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.smallLogoUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.separatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
        ((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
        this.mainLayoutControl.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.rightImagePictureEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.rightImagePictureEditLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.description2LabelControlLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.imageSeparatorEmptySpaceItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.connectToRepositorySimpleButtonLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.createNewRepositorySimpleButtonLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).BeginInit();
        base.SuspendLayout();
        this.mainLayoutControl.AllowCustomization = false;
        this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
        this.mainLayoutControl.Controls.Add(this.smallLogoUserControl);
        this.mainLayoutControl.Controls.Add(this.createNewRepositorySimpleButton);
        this.mainLayoutControl.Controls.Add(this.connectToRepositorySimpleButton);
        this.mainLayoutControl.Controls.Add(this.description2LabelControl);
        this.mainLayoutControl.Controls.Add(this.description1LabelControl);
        this.mainLayoutControl.Controls.Add(this.rightImagePictureEdit);
        this.mainLayoutControl.Controls.Add(this.headerLabelControl);
        this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
        this.mainLayoutControl.Name = "mainLayoutControl";
        this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3512, 590, 855, 685);
        this.mainLayoutControl.Root = this.mainLayoutControlGroup;
        this.mainLayoutControl.Size = new System.Drawing.Size(700, 470);
        this.mainLayoutControl.TabIndex = 0;
        this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
        this.smallLogoUserControl.Location = new System.Drawing.Point(27, 419);
        this.smallLogoUserControl.Margin = new System.Windows.Forms.Padding(0);
        this.smallLogoUserControl.MaximumSize = new System.Drawing.Size(93, 24);
        this.smallLogoUserControl.MinimumSize = new System.Drawing.Size(93, 24);
        this.smallLogoUserControl.Name = "smallLogoUserControl";
        this.smallLogoUserControl.Size = new System.Drawing.Size(93, 24);
        this.smallLogoUserControl.TabIndex = 15;
        this.createNewRepositorySimpleButton.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
        this.createNewRepositorySimpleButton.Appearance.Options.UseFont = true;
        this.createNewRepositorySimpleButton.ImageOptions.Image = Dataedo.App.Properties.Resources.add_16;
        this.createNewRepositorySimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
        this.createNewRepositorySimpleButton.Location = new System.Drawing.Point(25, 301);
        this.createNewRepositorySimpleButton.MaximumSize = new System.Drawing.Size(160, 29);
        this.createNewRepositorySimpleButton.MinimumSize = new System.Drawing.Size(160, 29);
        this.createNewRepositorySimpleButton.Name = "createNewRepositorySimpleButton";
        this.createNewRepositorySimpleButton.Size = new System.Drawing.Size(160, 29);
        this.createNewRepositorySimpleButton.StyleController = this.mainLayoutControl;
        this.createNewRepositorySimpleButton.TabIndex = 12;
        this.createNewRepositorySimpleButton.Text = "Create new repository";
        this.createNewRepositorySimpleButton.Click += new System.EventHandler(CreateNewRepositorySimpleButton_Click);
        this.connectToRepositorySimpleButton.ImageOptions.Image = Dataedo.App.Properties.Resources.connect_16;
        this.connectToRepositorySimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
        this.connectToRepositorySimpleButton.Location = new System.Drawing.Point(25, 340);
        this.connectToRepositorySimpleButton.MaximumSize = new System.Drawing.Size(145, 29);
        this.connectToRepositorySimpleButton.MinimumSize = new System.Drawing.Size(145, 29);
        this.connectToRepositorySimpleButton.Name = "connectToRepositorySimpleButton";
        this.connectToRepositorySimpleButton.Size = new System.Drawing.Size(145, 29);
        this.connectToRepositorySimpleButton.StyleController = this.mainLayoutControl;
        this.connectToRepositorySimpleButton.TabIndex = 13;
        this.connectToRepositorySimpleButton.Text = "Connect to repository";
        this.connectToRepositorySimpleButton.Click += new System.EventHandler(ConnectToRepositorySimpleButton_Click);
        this.description2LabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
        this.description2LabelControl.Appearance.ForeColor = System.Drawing.SystemColors.GrayText;
        this.description2LabelControl.Appearance.Options.UseFont = true;
        this.description2LabelControl.Appearance.Options.UseForeColor = true;
        this.description2LabelControl.Appearance.Options.UseTextOptions = true;
        this.description2LabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
        this.description2LabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
        this.description2LabelControl.Location = new System.Drawing.Point(27, 218);
        this.description2LabelControl.Name = "description2LabelControl";
        this.description2LabelControl.Size = new System.Drawing.Size(340, 16);
        this.description2LabelControl.StyleController = this.mainLayoutControl;
        this.description2LabelControl.TabIndex = 11;
        this.description2LabelControl.Text = "You can create or connect to other repository at any time.";
        this.description1LabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
        this.description1LabelControl.Appearance.Options.UseFont = true;
        this.description1LabelControl.Appearance.Options.UseTextOptions = true;
        this.description1LabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
        this.description1LabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
        this.description1LabelControl.Location = new System.Drawing.Point(27, 137);
        this.description1LabelControl.Name = "description1LabelControl";
        this.description1LabelControl.Size = new System.Drawing.Size(340, 64);
        this.description1LabelControl.StyleController = this.mainLayoutControl;
        this.description1LabelControl.TabIndex = 10;
        this.description1LabelControl.Text = "Does your company already have a Dataedo repository? Connect to an existing one or create new if you're unsure or just you'd like to try application.\r\n\r\n";
        this.rightImagePictureEdit.EditValue = Dataedo.App.Properties.Resources.loginform_getting_started;
        this.rightImagePictureEdit.Location = new System.Drawing.Point(419, 25);
        this.rightImagePictureEdit.Margin = new System.Windows.Forms.Padding(0);
        this.rightImagePictureEdit.Name = "rightImagePictureEdit";
        this.rightImagePictureEdit.Properties.AllowFocused = false;
        this.rightImagePictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        this.rightImagePictureEdit.Properties.ReadOnly = true;
        this.rightImagePictureEdit.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
        this.rightImagePictureEdit.Properties.ShowMenu = false;
        this.rightImagePictureEdit.Size = new System.Drawing.Size(256, 420);
        this.rightImagePictureEdit.StyleController = this.mainLayoutControl;
        this.rightImagePictureEdit.TabIndex = 9;
        this.headerLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 24f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
        this.headerLabelControl.Appearance.Options.UseFont = true;
        this.headerLabelControl.Location = new System.Drawing.Point(27, 55);
        this.headerLabelControl.Name = "headerLabelControl";
        this.headerLabelControl.Size = new System.Drawing.Size(340, 40);
        this.headerLabelControl.StyleController = this.mainLayoutControl;
        this.headerLabelControl.TabIndex = 8;
        this.headerLabelControl.Text = "Getting started";
        this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
        this.mainLayoutControlGroup.GroupBordersVisible = false;
        this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[11]
        {
            this.logoPictureEditTopSeparatorEmptySpaceItem, this.logoPictureEditRightSeparatorEmptySpaceItem, this.headerLabelControlLayoutControlItem, this.rightImagePictureEditLayoutControlItem, this.description1LabelControlLayoutControlItem, this.description2LabelControlLayoutControlItem, this.imageSeparatorEmptySpaceItem, this.connectToRepositorySimpleButtonLayoutControlItem, this.smallLogoUserControlLayoutControlItem, this.separatorEmptySpaceItem,
            this.createNewRepositorySimpleButtonLayoutControlItem
        });
        this.mainLayoutControlGroup.Name = "Root";
        this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 25, 25);
        this.mainLayoutControlGroup.Size = new System.Drawing.Size(700, 470);
        this.mainLayoutControlGroup.TextVisible = false;
        this.logoPictureEditTopSeparatorEmptySpaceItem.AllowHotTrack = false;
        this.logoPictureEditTopSeparatorEmptySpaceItem.Location = new System.Drawing.Point(0, 354);
        this.logoPictureEditTopSeparatorEmptySpaceItem.Name = "logoPictureEditTopSeparatorEmptySpaceItem";
        this.logoPictureEditTopSeparatorEmptySpaceItem.Size = new System.Drawing.Size(97, 38);
        this.logoPictureEditTopSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
        this.logoPictureEditRightSeparatorEmptySpaceItem.AllowHotTrack = false;
        this.logoPictureEditRightSeparatorEmptySpaceItem.Location = new System.Drawing.Point(97, 354);
        this.logoPictureEditRightSeparatorEmptySpaceItem.Name = "logoPictureEditRightSeparatorEmptySpaceItem";
        this.logoPictureEditRightSeparatorEmptySpaceItem.Size = new System.Drawing.Size(247, 66);
        this.logoPictureEditRightSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
        this.headerLabelControlLayoutControlItem.Control = this.headerLabelControl;
        this.headerLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
        this.headerLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 110);
        this.headerLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 110);
        this.headerLabelControlLayoutControlItem.Name = "headerLabelControlLayoutControlItem";
        this.headerLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 30, 40);
        this.headerLabelControlLayoutControlItem.Size = new System.Drawing.Size(344, 110);
        this.headerLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.headerLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.headerLabelControlLayoutControlItem.TextVisible = false;
        this.rightImagePictureEditLayoutControlItem.Control = this.rightImagePictureEdit;
        this.rightImagePictureEditLayoutControlItem.Location = new System.Drawing.Point(394, 0);
        this.rightImagePictureEditLayoutControlItem.MaxSize = new System.Drawing.Size(256, 0);
        this.rightImagePictureEditLayoutControlItem.MinSize = new System.Drawing.Size(256, 1);
        this.rightImagePictureEditLayoutControlItem.Name = "rightImagePictureEditLayoutControlItem";
        this.rightImagePictureEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
        this.rightImagePictureEditLayoutControlItem.Size = new System.Drawing.Size(256, 420);
        this.rightImagePictureEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.rightImagePictureEditLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
        this.rightImagePictureEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.rightImagePictureEditLayoutControlItem.TextToControlDistance = 0;
        this.rightImagePictureEditLayoutControlItem.TextVisible = false;
        this.description1LabelControlLayoutControlItem.Control = this.description1LabelControl;
        this.description1LabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 110);
        this.description1LabelControlLayoutControlItem.Name = "description1LabelControlLayoutControlItem";
        this.description1LabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 15);
        this.description1LabelControlLayoutControlItem.Size = new System.Drawing.Size(344, 81);
        this.description1LabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.description1LabelControlLayoutControlItem.TextVisible = false;
        this.description2LabelControlLayoutControlItem.Control = this.description2LabelControl;
        this.description2LabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 191);
        this.description2LabelControlLayoutControlItem.Name = "description2LabelControlLayoutControlItem";
        this.description2LabelControlLayoutControlItem.Size = new System.Drawing.Size(344, 20);
        this.description2LabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.description2LabelControlLayoutControlItem.TextVisible = false;
        this.imageSeparatorEmptySpaceItem.AllowHotTrack = false;
        this.imageSeparatorEmptySpaceItem.Location = new System.Drawing.Point(344, 0);
        this.imageSeparatorEmptySpaceItem.MaxSize = new System.Drawing.Size(50, 0);
        this.imageSeparatorEmptySpaceItem.MinSize = new System.Drawing.Size(50, 24);
        this.imageSeparatorEmptySpaceItem.Name = "imageSeparatorEmptySpaceItem";
        this.imageSeparatorEmptySpaceItem.Size = new System.Drawing.Size(50, 420);
        this.imageSeparatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.imageSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
        this.connectToRepositorySimpleButtonLayoutControlItem.Control = this.connectToRepositorySimpleButton;
        this.connectToRepositorySimpleButtonLayoutControlItem.Location = new System.Drawing.Point(0, 315);
        this.connectToRepositorySimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(0, 39);
        this.connectToRepositorySimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(135, 39);
        this.connectToRepositorySimpleButtonLayoutControlItem.Name = "connectToRepositorySimpleButtonLayoutControlItem";
        this.connectToRepositorySimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 10);
        this.connectToRepositorySimpleButtonLayoutControlItem.Size = new System.Drawing.Size(344, 39);
        this.connectToRepositorySimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.connectToRepositorySimpleButtonLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
        this.connectToRepositorySimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.connectToRepositorySimpleButtonLayoutControlItem.TextToControlDistance = 0;
        this.connectToRepositorySimpleButtonLayoutControlItem.TextVisible = false;
        this.createNewRepositorySimpleButtonLayoutControlItem.Control = this.createNewRepositorySimpleButton;
        this.createNewRepositorySimpleButtonLayoutControlItem.Location = new System.Drawing.Point(0, 276);
        this.createNewRepositorySimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(0, 39);
        this.createNewRepositorySimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(138, 39);
        this.createNewRepositorySimpleButtonLayoutControlItem.Name = "createNewRepositorySimpleButtonLayoutControlItem";
        this.createNewRepositorySimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 10);
        this.createNewRepositorySimpleButtonLayoutControlItem.Size = new System.Drawing.Size(344, 39);
        this.createNewRepositorySimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.createNewRepositorySimpleButtonLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
        this.createNewRepositorySimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.createNewRepositorySimpleButtonLayoutControlItem.TextToControlDistance = 0;
        this.createNewRepositorySimpleButtonLayoutControlItem.TextVisible = false;
        this.smallLogoUserControlLayoutControlItem.Control = this.smallLogoUserControl;
        this.smallLogoUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 392);
        this.smallLogoUserControlLayoutControlItem.Name = "smallLogoUserControlLayoutControlItem";
        this.smallLogoUserControlLayoutControlItem.Size = new System.Drawing.Size(97, 28);
        this.smallLogoUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.smallLogoUserControlLayoutControlItem.TextVisible = false;
        this.separatorEmptySpaceItem.AllowHotTrack = false;
        this.separatorEmptySpaceItem.Location = new System.Drawing.Point(0, 211);
        this.separatorEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 65);
        this.separatorEmptySpaceItem.MinSize = new System.Drawing.Size(104, 65);
        this.separatorEmptySpaceItem.Name = "separatorEmptySpaceItem";
        this.separatorEmptySpaceItem.Size = new System.Drawing.Size(344, 65);
        this.separatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.separatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
        base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        base.Controls.Add(this.mainLayoutControl);
        base.Name = "GettingStartedPageUserControl";
        base.Load += new System.EventHandler(GettingStartedPageUserControl_Load);
        ((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
        this.mainLayoutControl.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.rightImagePictureEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.rightImagePictureEditLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.description2LabelControlLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.imageSeparatorEmptySpaceItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.connectToRepositorySimpleButtonLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.createNewRepositorySimpleButtonLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).EndInit();
        base.ResumeLayout(false);
    }
}
