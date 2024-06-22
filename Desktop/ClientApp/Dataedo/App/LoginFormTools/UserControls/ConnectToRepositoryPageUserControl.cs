using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.LoginFormTools.Tools.CustomEventArgs;
using Dataedo.App.LoginFormTools.Tools.Enums;
using Dataedo.App.LoginFormTools.UserControls.Base;
using Dataedo.App.LoginFormTools.UserControls.Common;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.App.Tools.UI;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.LoginFormTools.UserControls;

public class ConnectToRepositoryPageUserControl : BasePageUserControl
{
	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private EmptySpaceItem logoPictureEditTopSeparatorEmptySpaceItem;

	private EmptySpaceItem logoPictureEditRightSeparatorEmptySpaceItem;

	private LabelControl headerLabelControl;

	private LayoutControlItem headerLabelControlLayoutControlItem;

	private LabelControl description1LabelControl;

	private LayoutControlItem description1LabelControlLayoutControlItem;

	private OptionButtonUserControl openFileRepositoryOptionButtonUserControl;

	private OptionButtonUserControl connectToServerRepositoryOptionButtonUserControl;

	private LayoutControlItem connectToServerRepositoryOptionButtonUserControlLayoutControlItem;

	private LayoutControlItem openFileRepositoryOptionButtonUserControlLayoutControlItem;

	private EmptySpaceItem buttonsSeparatorEmptySpaceItem;

	private EmptySpaceItem buttonsLeftSeparatorEmptySpaceItem;

	private EmptySpaceItem buttonsRightSeparatorEmptySpaceItem;

	private SimpleButton backSimpleButton;

	private LayoutControlItem backSimpleButtonLayoutControlItem;

	private EmptySpaceItem buttonsEmptySpaceItem;

	private SmallLogoUserControl smallLogoUserControl;

	private LayoutControlItem smallLogoUserControlLayoutControlItem;

	private EmptySpaceItem separatorEmptySpaceItem;

	private LabelControl legacyTextLabelControl;

	public ConnectToRepositoryPageUserControl()
	{
		InitializeComponent();
		legacyTextLabelControl.BackColor = SkinsManager.CurrentSkin.LegacyTextExportBackColor;
		legacyTextLabelControl.ForeColor = SkinsManager.CurrentSkin.LegacyTextExportForeColor;
	}

	internal override async Task<bool> Navigated()
	{
		bool result = await base.Navigated();
		connectToServerRepositoryOptionButtonUserControl.Focus();
		backSimpleButtonLayoutControlItem.Visibility = ((!base.BackButtonVisibility) ? LayoutVisibility.Never : LayoutVisibility.Always);
		return result;
	}

	internal void OpenFileRepository(string fileName)
	{
		try
		{
			ShowLoader();
			OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.OpenFileRepository, fileName));
		}
		finally
		{
			HideLoader();
		}
	}

	internal void OnRequiresAction()
	{
		OnRequiresAction(this);
	}

	internal void OnInvalidPathProvided(string fileName)
	{
		openFileRepositoryOptionButtonUserControl.Focus();
		OnRequiresAction();
		OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.OpenFileRepositoryForInvalidFile, fileName));
	}

	private void BackSimpleButton_Click(object sender, EventArgs e)
	{
		OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.Back));
	}

	private void ConnectToServerRepositoryOptionButtonUserControl_Click(object sender, EventArgs e)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingConnectToRepositoryRepoParameters(isFile: false), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.CreatorExistingServer);
		});
		OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.ConnectToServerRepository, null));
	}

	private void OpenFileRepositoryOptionButtonUserControl_Click(object sender, EventArgs e)
	{
		try
		{
			ShowLoader();
			TrackingRunner.Track(delegate
			{
				TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingConnectionRepoParameters(isFile: true), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.CreatorExistingFile);
			});
			OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.OpenFileRepository, null));
		}
		finally
		{
			HideLoader();
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
		DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem = new DevExpress.Utils.ToolTipItem();
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.smallLogoUserControl = new Dataedo.App.LoginFormTools.UserControls.Common.SmallLogoUserControl();
		this.backSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.openFileRepositoryOptionButtonUserControl = new Dataedo.App.LoginFormTools.UserControls.Common.OptionButtonUserControl();
		this.connectToServerRepositoryOptionButtonUserControl = new Dataedo.App.LoginFormTools.UserControls.Common.OptionButtonUserControl();
		this.description1LabelControl = new DevExpress.XtraEditors.LabelControl();
		this.headerLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.logoPictureEditRightSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.headerLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.description1LabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.connectToServerRepositoryOptionButtonUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.openFileRepositoryOptionButtonUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.buttonsSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.buttonsLeftSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.buttonsRightSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.backSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.buttonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.logoPictureEditTopSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.smallLogoUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.separatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.legacyTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectToServerRepositoryOptionButtonUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.openFileRepositoryOptionButtonUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsLeftSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsRightSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.backSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.legacyTextLabelControl);
		this.mainLayoutControl.Controls.Add(this.smallLogoUserControl);
		this.mainLayoutControl.Controls.Add(this.backSimpleButton);
		this.mainLayoutControl.Controls.Add(this.openFileRepositoryOptionButtonUserControl);
		this.mainLayoutControl.Controls.Add(this.connectToServerRepositoryOptionButtonUserControl);
		this.mainLayoutControl.Controls.Add(this.description1LabelControl);
		this.mainLayoutControl.Controls.Add(this.headerLabelControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3579, 429, 855, 685);
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
		this.smallLogoUserControl.TabIndex = 19;
		this.backSimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
		this.backSimpleButton.Location = new System.Drawing.Point(605, 416);
		this.backSimpleButton.MaximumSize = new System.Drawing.Size(70, 29);
		this.backSimpleButton.MinimumSize = new System.Drawing.Size(70, 29);
		this.backSimpleButton.Name = "backSimpleButton";
		this.backSimpleButton.Size = new System.Drawing.Size(70, 29);
		this.backSimpleButton.StyleController = this.mainLayoutControl;
		this.backSimpleButton.TabIndex = 18;
		this.backSimpleButton.Text = "Back";
		this.backSimpleButton.Click += new System.EventHandler(BackSimpleButton_Click);
		this.openFileRepositoryOptionButtonUserControl.HintImage = Dataedo.App.Properties.Resources.question_16;
		this.openFileRepositoryOptionButtonUserControl.HintImageAutoPopDelay = 5000;
		this.openFileRepositoryOptionButtonUserControl.HintImageMaxToolTipWidth = 500;
		this.openFileRepositoryOptionButtonUserControl.HintImageToolTipHeader = null;
		this.openFileRepositoryOptionButtonUserControl.HintImageToolTipText = "Choose a Dataedo file to open.\r\nIf the file is stored on a network drive, to avoid performance issues create a copy on a local disk first.";
		this.openFileRepositoryOptionButtonUserControl.ImageOptions.Image = Dataedo.App.Properties.Resources.file_repository_connect_32;
		this.openFileRepositoryOptionButtonUserControl.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
		this.openFileRepositoryOptionButtonUserControl.ImageOptions.ImageToTextIndent = 20;
		this.openFileRepositoryOptionButtonUserControl.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter;
		this.openFileRepositoryOptionButtonUserControl.KeepWhileHovered = false;
		this.openFileRepositoryOptionButtonUserControl.Location = new System.Drawing.Point(364, 173);
		this.openFileRepositoryOptionButtonUserControl.MaximumSize = new System.Drawing.Size(260, 180);
		this.openFileRepositoryOptionButtonUserControl.MinimumSize = new System.Drawing.Size(260, 180);
		this.openFileRepositoryOptionButtonUserControl.Name = "openFileRepositoryOptionButtonUserControl";
		this.openFileRepositoryOptionButtonUserControl.Size = new System.Drawing.Size(260, 180);
		this.openFileRepositoryOptionButtonUserControl.StyleController = this.mainLayoutControl;
		this.openFileRepositoryOptionButtonUserControl.TabIndex = 16;
		this.openFileRepositoryOptionButtonUserControl.Text = "Open file\r\nrepository";
		this.openFileRepositoryOptionButtonUserControl.Click += new System.EventHandler(OpenFileRepositoryOptionButtonUserControl_Click);
		this.connectToServerRepositoryOptionButtonUserControl.HintImage = Dataedo.App.Properties.Resources.question_16;
		this.connectToServerRepositoryOptionButtonUserControl.HintImageAutoPopDelay = 5000;
		this.connectToServerRepositoryOptionButtonUserControl.HintImageMaxToolTipWidth = 500;
		this.connectToServerRepositoryOptionButtonUserControl.HintImageToolTipHeader = null;
		this.connectToServerRepositoryOptionButtonUserControl.HintImageToolTipText = "Choose this option if you already have access to Dataedo repository.\r\nThis is not an option to connect to the database you want to document!";
		this.connectToServerRepositoryOptionButtonUserControl.ImageOptions.Image = Dataedo.App.Properties.Resources.server_repository_connect_32;
		this.connectToServerRepositoryOptionButtonUserControl.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
		this.connectToServerRepositoryOptionButtonUserControl.ImageOptions.ImageToTextIndent = 20;
		this.connectToServerRepositoryOptionButtonUserControl.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter;
		this.connectToServerRepositoryOptionButtonUserControl.KeepWhileHovered = false;
		this.connectToServerRepositoryOptionButtonUserControl.Location = new System.Drawing.Point(76, 173);
		this.connectToServerRepositoryOptionButtonUserControl.MaximumSize = new System.Drawing.Size(260, 180);
		this.connectToServerRepositoryOptionButtonUserControl.MinimumSize = new System.Drawing.Size(260, 180);
		this.connectToServerRepositoryOptionButtonUserControl.Name = "connectToServerRepositoryOptionButtonUserControl";
		this.connectToServerRepositoryOptionButtonUserControl.Size = new System.Drawing.Size(260, 180);
		this.connectToServerRepositoryOptionButtonUserControl.StyleController = this.mainLayoutControl;
		this.connectToServerRepositoryOptionButtonUserControl.TabIndex = 15;
		this.connectToServerRepositoryOptionButtonUserControl.Text = "Connect to\r\nserver repository";
		this.connectToServerRepositoryOptionButtonUserControl.Click += new System.EventHandler(ConnectToServerRepositoryOptionButtonUserControl_Click);
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
		this.description1LabelControl.Text = "Is your Dataedo repository a file or server based (SQL Server database)?\r\n ";
		this.headerLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 24f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.headerLabelControl.Appearance.Options.UseFont = true;
		this.headerLabelControl.AutoEllipsis = true;
		this.headerLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.headerLabelControl.Location = new System.Drawing.Point(27, 25);
		this.headerLabelControl.Name = "headerLabelControl";
		this.headerLabelControl.Size = new System.Drawing.Size(646, 39);
		this.headerLabelControl.StyleController = this.mainLayoutControl;
		this.headerLabelControl.TabIndex = 8;
		this.headerLabelControl.Text = "Connect to repository";
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[13]
		{
			this.logoPictureEditRightSeparatorEmptySpaceItem, this.headerLabelControlLayoutControlItem, this.description1LabelControlLayoutControlItem, this.connectToServerRepositoryOptionButtonUserControlLayoutControlItem, this.openFileRepositoryOptionButtonUserControlLayoutControlItem, this.buttonsSeparatorEmptySpaceItem, this.buttonsLeftSeparatorEmptySpaceItem, this.buttonsRightSeparatorEmptySpaceItem, this.backSimpleButtonLayoutControlItem, this.buttonsEmptySpaceItem,
			this.logoPictureEditTopSeparatorEmptySpaceItem, this.smallLogoUserControlLayoutControlItem, this.separatorEmptySpaceItem
		});
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 25, 25);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControlGroup.TextVisible = false;
		this.logoPictureEditRightSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.logoPictureEditRightSeparatorEmptySpaceItem.Location = new System.Drawing.Point(97, 330);
		this.logoPictureEditRightSeparatorEmptySpaceItem.Name = "logoPictureEditRightSeparatorEmptySpaceItem";
		this.logoPictureEditRightSeparatorEmptySpaceItem.Size = new System.Drawing.Size(483, 90);
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
		this.connectToServerRepositoryOptionButtonUserControlLayoutControlItem.Control = this.connectToServerRepositoryOptionButtonUserControl;
		this.connectToServerRepositoryOptionButtonUserControlLayoutControlItem.Location = new System.Drawing.Point(49, 146);
		this.connectToServerRepositoryOptionButtonUserControlLayoutControlItem.Name = "connectToServerRepositoryOptionButtonUserControlLayoutControlItem";
		this.connectToServerRepositoryOptionButtonUserControlLayoutControlItem.Size = new System.Drawing.Size(264, 184);
		this.connectToServerRepositoryOptionButtonUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.connectToServerRepositoryOptionButtonUserControlLayoutControlItem.TextVisible = false;
		this.openFileRepositoryOptionButtonUserControlLayoutControlItem.Control = this.openFileRepositoryOptionButtonUserControl;
		this.openFileRepositoryOptionButtonUserControlLayoutControlItem.Location = new System.Drawing.Point(337, 146);
		this.openFileRepositoryOptionButtonUserControlLayoutControlItem.Name = "openFileRepositoryOptionButtonUserControlLayoutControlItem";
		this.openFileRepositoryOptionButtonUserControlLayoutControlItem.Size = new System.Drawing.Size(264, 184);
		this.openFileRepositoryOptionButtonUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.openFileRepositoryOptionButtonUserControlLayoutControlItem.TextVisible = false;
		this.buttonsSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.buttonsSeparatorEmptySpaceItem.Location = new System.Drawing.Point(313, 146);
		this.buttonsSeparatorEmptySpaceItem.MaxSize = new System.Drawing.Size(24, 0);
		this.buttonsSeparatorEmptySpaceItem.MinSize = new System.Drawing.Size(24, 24);
		this.buttonsSeparatorEmptySpaceItem.Name = "buttonsSeparatorEmptySpaceItem";
		this.buttonsSeparatorEmptySpaceItem.Size = new System.Drawing.Size(24, 184);
		this.buttonsSeparatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.buttonsSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.buttonsLeftSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.buttonsLeftSeparatorEmptySpaceItem.Location = new System.Drawing.Point(0, 146);
		this.buttonsLeftSeparatorEmptySpaceItem.MaxSize = new System.Drawing.Size(49, 24);
		this.buttonsLeftSeparatorEmptySpaceItem.MinSize = new System.Drawing.Size(49, 24);
		this.buttonsLeftSeparatorEmptySpaceItem.Name = "buttonsLeftSeparatorEmptySpaceItem";
		this.buttonsLeftSeparatorEmptySpaceItem.Size = new System.Drawing.Size(49, 184);
		this.buttonsLeftSeparatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.buttonsLeftSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.buttonsRightSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.buttonsRightSeparatorEmptySpaceItem.Location = new System.Drawing.Point(601, 146);
		this.buttonsRightSeparatorEmptySpaceItem.MaxSize = new System.Drawing.Size(49, 24);
		this.buttonsRightSeparatorEmptySpaceItem.MinSize = new System.Drawing.Size(49, 24);
		this.buttonsRightSeparatorEmptySpaceItem.Name = "buttonsRightSeparatorEmptySpaceItem";
		this.buttonsRightSeparatorEmptySpaceItem.Size = new System.Drawing.Size(49, 184);
		this.buttonsRightSeparatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.buttonsRightSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.backSimpleButtonLayoutControlItem.Control = this.backSimpleButton;
		this.backSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(580, 391);
		this.backSimpleButtonLayoutControlItem.Name = "backSimpleButtonLayoutControlItem";
		this.backSimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.backSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(70, 29);
		this.backSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.backSimpleButtonLayoutControlItem.TextVisible = false;
		this.buttonsEmptySpaceItem.AllowHotTrack = false;
		this.buttonsEmptySpaceItem.Location = new System.Drawing.Point(580, 330);
		this.buttonsEmptySpaceItem.Name = "buttonsEmptySpaceItem";
		this.buttonsEmptySpaceItem.Size = new System.Drawing.Size(70, 61);
		this.buttonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.logoPictureEditTopSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.logoPictureEditTopSeparatorEmptySpaceItem.Location = new System.Drawing.Point(0, 330);
		this.logoPictureEditTopSeparatorEmptySpaceItem.Name = "logoPictureEditTopSeparatorEmptySpaceItem";
		this.logoPictureEditTopSeparatorEmptySpaceItem.Size = new System.Drawing.Size(97, 62);
		this.logoPictureEditTopSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.smallLogoUserControlLayoutControlItem.Control = this.smallLogoUserControl;
		this.smallLogoUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 392);
		this.smallLogoUserControlLayoutControlItem.Name = "smallLogoUserControlLayoutControlItem";
		this.smallLogoUserControlLayoutControlItem.Size = new System.Drawing.Size(97, 28);
		this.smallLogoUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.smallLogoUserControlLayoutControlItem.TextVisible = false;
		this.separatorEmptySpaceItem.AllowHotTrack = false;
		this.separatorEmptySpaceItem.Location = new System.Drawing.Point(0, 91);
		this.separatorEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 55);
		this.separatorEmptySpaceItem.MinSize = new System.Drawing.Size(104, 55);
		this.separatorEmptySpaceItem.Name = "separatorEmptySpaceItem";
		this.separatorEmptySpaceItem.Size = new System.Drawing.Size(650, 55);
		this.separatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.separatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.legacyTextLabelControl.Appearance.BackColor = System.Drawing.Color.FromArgb(229, 229, 229);
		this.legacyTextLabelControl.Appearance.ForeColor = System.Drawing.Color.FromArgb(124, 124, 124);
		this.legacyTextLabelControl.Appearance.Options.UseBackColor = true;
		this.legacyTextLabelControl.Appearance.Options.UseForeColor = true;
		this.legacyTextLabelControl.Appearance.Options.UseTextOptions = true;
		this.legacyTextLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.legacyTextLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.legacyTextLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.RightCenter;
		this.legacyTextLabelControl.ImageOptions.Alignment = System.Drawing.ContentAlignment.MiddleLeft;
		this.legacyTextLabelControl.ImageOptions.Image = Dataedo.App.Properties.Resources.question_16;
		this.legacyTextLabelControl.Location = new System.Drawing.Point(459, 312);
		this.legacyTextLabelControl.MaximumSize = new System.Drawing.Size(70, 0);
		this.legacyTextLabelControl.MinimumSize = new System.Drawing.Size(70, 24);
		this.legacyTextLabelControl.Name = "legacyTextLabelControl";
		this.legacyTextLabelControl.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
		this.legacyTextLabelControl.Size = new System.Drawing.Size(70, 24);
		this.legacyTextLabelControl.StyleController = this.mainLayoutControl;
		toolTipItem.Text = "A file repository is about to be discontinued in the next releases.";
		superToolTip.Items.Add(toolTipItem);
		this.legacyTextLabelControl.SuperTip = superToolTip;
		this.legacyTextLabelControl.TabIndex = 8;
		this.legacyTextLabelControl.Text = "Legacy ";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Name = "ConnectToRepositoryPageUserControl";
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		this.mainLayoutControl.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectToServerRepositoryOptionButtonUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.openFileRepositoryOptionButtonUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsLeftSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsRightSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.backSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).EndInit();
		base.ResumeLayout(false);
	}
}
