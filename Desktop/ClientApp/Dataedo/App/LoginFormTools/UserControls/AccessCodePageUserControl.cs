using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.API.Models;
using Dataedo.App.API.Services;
using Dataedo.App.LoginFormTools.Tools.Common;
using Dataedo.App.LoginFormTools.Tools.CustomEventArgs;
using Dataedo.App.LoginFormTools.Tools.Enums;
using Dataedo.App.LoginFormTools.UserControls.Base;
using Dataedo.App.LoginFormTools.UserControls.Common;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.LoginFormTools.UserControls;

public class AccessCodePageUserControl : BasePageUserControl
{
	private readonly string defaultDescription1;

	private string email;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private EmptySpaceItem logoPictureEditTopSeparatorEmptySpaceItem;

	private EmptySpaceItem logoPictureEditRightSeparatorEmptySpaceItem;

	private LabelControl headerLabelControl;

	private LayoutControlItem headerLabelControlLayoutControlItem;

	private PictureEdit rightImagePictureEdit;

	private LayoutControlItem rightImagePictureEditLayoutControlItem;

	private LabelControl description1LabelControl;

	private LayoutControlItem description1LabelControlLayoutControlItem;

	private EmptySpaceItem imageSeparatorEmptySpaceItem;

	private SimpleButton signInKeySimpleButton;

	private LayoutControlItem signInKeySimpleButtonLayoutControlItem;

	private ToolTipController toolTipController;

	private SmallLogoUserControl smallLogoUserControl;

	private LayoutControlItem smallLogoUserControlLayoutControlItem;

	private TextEdit accessCodeTextEdit;

	private LayoutControlItem accessCodeTextEditLayoutControlItem;

	private LabelControl emailTextEditLabelControl;

	private LayoutControlItem layoutControlItem1;

	private DXErrorProvider dxErrorProvider;

	private EmptySpaceItem separatorEmptySpaceItem;

	private HyperLinkEdit backHyperLinkEdit;

	private LayoutControlItem backHyperLinkEditLayoutControlItem;

	public AccessCodePageUserControl()
	{
		InitializeComponent();
		defaultDescription1 = description1LabelControl.Text;
	}

	internal override void SetParameter(object parameter, bool isCalledAsPrevious)
	{
		base.SetParameter(parameter, isCalledAsPrevious);
		email = null;
		accessCodeTextEdit.Text = null;
		signInKeySimpleButton.Enabled = false;
		dxErrorProvider.ClearErrors();
		if (parameter is string text)
		{
			email = text;
			description1LabelControl.Text = defaultDescription1 + " <b>" + email + "</b>";
		}
		else
		{
			description1LabelControl.Text = defaultDescription1.Replace(":", ".");
		}
		accessCodeTextEdit.Focus();
	}

	internal override async Task<bool> Navigated()
	{
		try
		{
			await base.Navigated();
			ShowLoader();
			return true;
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

	private void SetSignInButtonAvailability()
	{
		signInKeySimpleButton.Enabled = !string.IsNullOrEmpty(signInKeySimpleButton.Text);
	}

	private async Task ProcessNextAction()
	{
		try
		{
			ShowLoader();
			if (!signInKeySimpleButton.Enabled)
			{
				return;
			}
			dxErrorProvider.ClearErrors();
			LoginService loginService = new LoginService();
			ResultWithData<SessionResult> resultWithData;
			try
			{
				resultWithData = await loginService.EnterAccessCodeAsync(email, accessCodeTextEdit.Text, keepSignedIn: true);
			}
			catch (HttpRequestException)
			{
				HideLoader();
				GeneralMessageBoxesHandling.Show("Unable to process access code." + Environment.NewLine + Environment.NewLine + "Unable to connect to Dataedo server.", "Access code", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
				return;
			}
			catch (Exception exception)
			{
				HideLoader();
				GeneralExceptionHandling.Handle(exception, "Unable to process access code.", base.ParentForm);
				return;
			}
			if (resultWithData.IsOK)
			{
				OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.Licenses, new LoginDataModel(resultWithData.Data.Data.Token, email)));
			}
			else if (resultWithData.HasErrors)
			{
				string message = resultWithData.Errors.Message + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, resultWithData.Errors.Errors.SelectMany((KeyValuePair<string, string[]> x) => x.Value));
				if (resultWithData.Errors.Errors.ContainsKey("code"))
				{
					dxErrorProvider.SetError(accessCodeTextEdit, string.Join(Environment.NewLine, resultWithData.Errors.Errors["code"]));
				}
				GeneralMessageBoxesHandling.Show(message, "Enter access code", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
			}
			else if (resultWithData.ShouldProposeTryAgain)
			{
				GeneralMessageBoxesHandling.Show("Unable to sign in at this time.Please try again in a few minutes", "Enter access code", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
			}
		}
		finally
		{
			HideLoader();
		}
	}

	private void WelcomePageUserControl_Load(object sender, EventArgs e)
	{
		if (!base.DesignMode && base.ParentForm != null)
		{
			rightImagePictureEdit.BackColor = base.ParentForm.BackColor;
		}
	}

	private async void SignInSimpleButton_Click(object sender, EventArgs e)
	{
		await ProcessNextAction();
	}

	private void BackHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.Back, email));
	}

	private void AccessCodeTextEdit_TextChanged(object sender, EventArgs e)
	{
		SetSignInButtonAvailability();
	}

	private async void AccessCodeTextEdit_KeyDown(object sender, KeyEventArgs e)
	{
		await ProcessNextActionOnKeyDown(sender, e);
	}

	private async Task ProcessNextActionOnKeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			await ProcessNextAction();
			(sender as Control)?.Focus();
		}
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.backHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.emailTextEditLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.accessCodeTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.smallLogoUserControl = new Dataedo.App.LoginFormTools.UserControls.Common.SmallLogoUserControl();
		this.signInKeySimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.description1LabelControl = new DevExpress.XtraEditors.LabelControl();
		this.rightImagePictureEdit = new DevExpress.XtraEditors.PictureEdit();
		this.headerLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.logoPictureEditTopSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.logoPictureEditRightSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.headerLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.rightImagePictureEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.description1LabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.imageSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.smallLogoUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.accessCodeTextEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.signInKeySimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.separatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.backHyperLinkEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.dxErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.backHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.accessCodeTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rightImagePictureEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rightImagePictureEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.imageSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.accessCodeTextEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.signInKeySimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.backHyperLinkEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dxErrorProvider).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.backHyperLinkEdit);
		this.mainLayoutControl.Controls.Add(this.emailTextEditLabelControl);
		this.mainLayoutControl.Controls.Add(this.accessCodeTextEdit);
		this.mainLayoutControl.Controls.Add(this.smallLogoUserControl);
		this.mainLayoutControl.Controls.Add(this.signInKeySimpleButton);
		this.mainLayoutControl.Controls.Add(this.description1LabelControl);
		this.mainLayoutControl.Controls.Add(this.rightImagePictureEdit);
		this.mainLayoutControl.Controls.Add(this.headerLabelControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3555, 378, 855, 685);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.backHyperLinkEdit.EditValue = "Back";
		this.backHyperLinkEdit.Location = new System.Drawing.Point(167, 365);
		this.backHyperLinkEdit.MaximumSize = new System.Drawing.Size(32, 0);
		this.backHyperLinkEdit.MinimumSize = new System.Drawing.Size(32, 0);
		this.backHyperLinkEdit.Name = "backHyperLinkEdit";
		this.backHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.backHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.backHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.backHyperLinkEdit.Size = new System.Drawing.Size(32, 18);
		this.backHyperLinkEdit.StyleController = this.mainLayoutControl;
		this.backHyperLinkEdit.TabIndex = 21;
		this.backHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(BackHyperLinkEdit_OpenLink);
		this.emailTextEditLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.emailTextEditLabelControl.Appearance.Options.UseFont = true;
		this.emailTextEditLabelControl.Appearance.Options.UseTextOptions = true;
		this.emailTextEditLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.emailTextEditLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
		this.emailTextEditLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.emailTextEditLabelControl.Location = new System.Drawing.Point(27, 191);
		this.emailTextEditLabelControl.Name = "emailTextEditLabelControl";
		this.emailTextEditLabelControl.Size = new System.Drawing.Size(316, 16);
		this.emailTextEditLabelControl.StyleController = this.mainLayoutControl;
		this.emailTextEditLabelControl.TabIndex = 19;
		this.emailTextEditLabelControl.Text = "Access code:";
		this.accessCodeTextEdit.EditValue = "";
		this.accessCodeTextEdit.Location = new System.Drawing.Point(112, 211);
		this.accessCodeTextEdit.MaximumSize = new System.Drawing.Size(145, 35);
		this.accessCodeTextEdit.Name = "accessCodeTextEdit";
		this.accessCodeTextEdit.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.accessCodeTextEdit.Properties.Appearance.Options.UseFont = true;
		this.accessCodeTextEdit.Properties.Appearance.Options.UseTextOptions = true;
		this.accessCodeTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.accessCodeTextEdit.Size = new System.Drawing.Size(145, 30);
		this.accessCodeTextEdit.StyleController = this.mainLayoutControl;
		this.accessCodeTextEdit.TabIndex = 18;
		this.accessCodeTextEdit.ToolTipController = this.toolTipController;
		this.accessCodeTextEdit.TextChanged += new System.EventHandler(AccessCodeTextEdit_TextChanged);
		this.accessCodeTextEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(AccessCodeTextEdit_KeyDown);
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.smallLogoUserControl.Location = new System.Drawing.Point(27, 419);
		this.smallLogoUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.smallLogoUserControl.MaximumSize = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.MinimumSize = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.Name = "smallLogoUserControl";
		this.smallLogoUserControl.Size = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.TabIndex = 15;
		this.signInKeySimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
		this.signInKeySimpleButton.Location = new System.Drawing.Point(113, 266);
		this.signInKeySimpleButton.MaximumSize = new System.Drawing.Size(145, 0);
		this.signInKeySimpleButton.MinimumSize = new System.Drawing.Size(145, 0);
		this.signInKeySimpleButton.Name = "signInKeySimpleButton";
		this.signInKeySimpleButton.Size = new System.Drawing.Size(145, 29);
		this.signInKeySimpleButton.StyleController = this.mainLayoutControl;
		this.signInKeySimpleButton.TabIndex = 13;
		this.signInKeySimpleButton.Text = "Sign in";
		this.signInKeySimpleButton.Click += new System.EventHandler(SignInSimpleButton_Click);
		this.description1LabelControl.AllowHtmlString = true;
		this.description1LabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.description1LabelControl.Appearance.Options.UseFont = true;
		this.description1LabelControl.Appearance.Options.UseTextOptions = true;
		this.description1LabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
		this.description1LabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.description1LabelControl.Location = new System.Drawing.Point(27, 137);
		this.description1LabelControl.Name = "description1LabelControl";
		this.description1LabelControl.Size = new System.Drawing.Size(316, 32);
		this.description1LabelControl.StyleController = this.mainLayoutControl;
		this.description1LabelControl.TabIndex = 10;
		this.description1LabelControl.Text = "We've sent one time access code to your email address:";
		this.rightImagePictureEdit.EditValue = Dataedo.App.Properties.Resources.loginform_welcome;
		this.rightImagePictureEdit.Location = new System.Drawing.Point(395, 25);
		this.rightImagePictureEdit.Margin = new System.Windows.Forms.Padding(0);
		this.rightImagePictureEdit.Name = "rightImagePictureEdit";
		this.rightImagePictureEdit.Properties.AllowFocused = false;
		this.rightImagePictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.rightImagePictureEdit.Properties.ReadOnly = true;
		this.rightImagePictureEdit.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
		this.rightImagePictureEdit.Properties.ShowMenu = false;
		this.rightImagePictureEdit.Size = new System.Drawing.Size(280, 420);
		this.rightImagePictureEdit.StyleController = this.mainLayoutControl;
		this.rightImagePictureEdit.TabIndex = 9;
		this.headerLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 24f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.headerLabelControl.Appearance.Options.UseFont = true;
		this.headerLabelControl.Location = new System.Drawing.Point(27, 55);
		this.headerLabelControl.Name = "headerLabelControl";
		this.headerLabelControl.Size = new System.Drawing.Size(316, 40);
		this.headerLabelControl.StyleController = this.mainLayoutControl;
		this.headerLabelControl.TabIndex = 8;
		this.headerLabelControl.Text = "Enter access code";
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[12]
		{
			this.logoPictureEditTopSeparatorEmptySpaceItem, this.logoPictureEditRightSeparatorEmptySpaceItem, this.headerLabelControlLayoutControlItem, this.rightImagePictureEditLayoutControlItem, this.description1LabelControlLayoutControlItem, this.imageSeparatorEmptySpaceItem, this.smallLogoUserControlLayoutControlItem, this.accessCodeTextEditLayoutControlItem, this.layoutControlItem1, this.signInKeySimpleButtonLayoutControlItem,
			this.separatorEmptySpaceItem, this.backHyperLinkEditLayoutControlItem
		});
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 25, 25);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControlGroup.TextVisible = false;
		this.logoPictureEditTopSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.logoPictureEditTopSeparatorEmptySpaceItem.Location = new System.Drawing.Point(0, 379);
		this.logoPictureEditTopSeparatorEmptySpaceItem.Name = "logoPictureEditTopSeparatorEmptySpaceItem";
		this.logoPictureEditTopSeparatorEmptySpaceItem.Size = new System.Drawing.Size(97, 13);
		this.logoPictureEditTopSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.logoPictureEditRightSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.logoPictureEditRightSeparatorEmptySpaceItem.Location = new System.Drawing.Point(97, 379);
		this.logoPictureEditRightSeparatorEmptySpaceItem.Name = "logoPictureEditRightSeparatorEmptySpaceItem";
		this.logoPictureEditRightSeparatorEmptySpaceItem.Size = new System.Drawing.Size(223, 41);
		this.logoPictureEditRightSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.headerLabelControlLayoutControlItem.Control = this.headerLabelControl;
		this.headerLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.headerLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 110);
		this.headerLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 110);
		this.headerLabelControlLayoutControlItem.Name = "headerLabelControlLayoutControlItem";
		this.headerLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 30, 40);
		this.headerLabelControlLayoutControlItem.Size = new System.Drawing.Size(320, 110);
		this.headerLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.headerLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.headerLabelControlLayoutControlItem.TextVisible = false;
		this.rightImagePictureEditLayoutControlItem.Control = this.rightImagePictureEdit;
		this.rightImagePictureEditLayoutControlItem.Location = new System.Drawing.Point(370, 0);
		this.rightImagePictureEditLayoutControlItem.MaxSize = new System.Drawing.Size(280, 0);
		this.rightImagePictureEditLayoutControlItem.MinSize = new System.Drawing.Size(280, 1);
		this.rightImagePictureEditLayoutControlItem.Name = "rightImagePictureEditLayoutControlItem";
		this.rightImagePictureEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.rightImagePictureEditLayoutControlItem.Size = new System.Drawing.Size(280, 420);
		this.rightImagePictureEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.rightImagePictureEditLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.rightImagePictureEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.rightImagePictureEditLayoutControlItem.TextToControlDistance = 0;
		this.rightImagePictureEditLayoutControlItem.TextVisible = false;
		this.description1LabelControlLayoutControlItem.Control = this.description1LabelControl;
		this.description1LabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 110);
		this.description1LabelControlLayoutControlItem.Name = "description1LabelControlLayoutControlItem";
		this.description1LabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 20);
		this.description1LabelControlLayoutControlItem.Size = new System.Drawing.Size(320, 54);
		this.description1LabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.description1LabelControlLayoutControlItem.TextVisible = false;
		this.imageSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.imageSeparatorEmptySpaceItem.Location = new System.Drawing.Point(320, 0);
		this.imageSeparatorEmptySpaceItem.MaxSize = new System.Drawing.Size(50, 0);
		this.imageSeparatorEmptySpaceItem.MinSize = new System.Drawing.Size(50, 24);
		this.imageSeparatorEmptySpaceItem.Name = "imageSeparatorEmptySpaceItem";
		this.imageSeparatorEmptySpaceItem.Size = new System.Drawing.Size(50, 420);
		this.imageSeparatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.imageSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.smallLogoUserControlLayoutControlItem.Control = this.smallLogoUserControl;
		this.smallLogoUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 392);
		this.smallLogoUserControlLayoutControlItem.Name = "smallLogoUserControlLayoutControlItem";
		this.smallLogoUserControlLayoutControlItem.Size = new System.Drawing.Size(97, 28);
		this.smallLogoUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.smallLogoUserControlLayoutControlItem.TextVisible = false;
		this.accessCodeTextEditLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.accessCodeTextEditLayoutControlItem.Control = this.accessCodeTextEdit;
		this.accessCodeTextEditLayoutControlItem.Location = new System.Drawing.Point(0, 184);
		this.accessCodeTextEditLayoutControlItem.Name = "accessCodeTextEditLayoutControlItem";
		this.accessCodeTextEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 25);
		this.accessCodeTextEditLayoutControlItem.Size = new System.Drawing.Size(320, 57);
		this.accessCodeTextEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.accessCodeTextEditLayoutControlItem.TextVisible = false;
		this.layoutControlItem1.Control = this.emailTextEditLabelControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 164);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(320, 20);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.signInKeySimpleButtonLayoutControlItem.Control = this.signInKeySimpleButton;
		this.signInKeySimpleButtonLayoutControlItem.Location = new System.Drawing.Point(0, 241);
		this.signInKeySimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(0, 39);
		this.signInKeySimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(138, 39);
		this.signInKeySimpleButtonLayoutControlItem.Name = "signInKeySimpleButtonLayoutControlItem";
		this.signInKeySimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(88, 0, 0, 10);
		this.signInKeySimpleButtonLayoutControlItem.Size = new System.Drawing.Size(320, 39);
		this.signInKeySimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.signInKeySimpleButtonLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.signInKeySimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.signInKeySimpleButtonLayoutControlItem.TextToControlDistance = 0;
		this.signInKeySimpleButtonLayoutControlItem.TextVisible = false;
		this.separatorEmptySpaceItem.AllowHotTrack = false;
		this.separatorEmptySpaceItem.Location = new System.Drawing.Point(0, 280);
		this.separatorEmptySpaceItem.MaxSize = new System.Drawing.Size(320, 60);
		this.separatorEmptySpaceItem.MinSize = new System.Drawing.Size(320, 60);
		this.separatorEmptySpaceItem.Name = "separatorEmptySpaceItem";
		this.separatorEmptySpaceItem.Size = new System.Drawing.Size(320, 60);
		this.separatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.separatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.backHyperLinkEditLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.backHyperLinkEditLayoutControlItem.Control = this.backHyperLinkEdit;
		this.backHyperLinkEditLayoutControlItem.Location = new System.Drawing.Point(0, 340);
		this.backHyperLinkEditLayoutControlItem.MaxSize = new System.Drawing.Size(0, 39);
		this.backHyperLinkEditLayoutControlItem.MinSize = new System.Drawing.Size(234, 39);
		this.backHyperLinkEditLayoutControlItem.Name = "backHyperLinkEditLayoutControlItem";
		this.backHyperLinkEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(142, 0, 0, 0);
		this.backHyperLinkEditLayoutControlItem.Size = new System.Drawing.Size(320, 39);
		this.backHyperLinkEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.backHyperLinkEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.backHyperLinkEditLayoutControlItem.TextVisible = false;
		this.dxErrorProvider.ContainerControl = this;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Name = "AccessCodePageUserControl";
		base.Load += new System.EventHandler(WelcomePageUserControl_Load);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.backHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.accessCodeTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rightImagePictureEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rightImagePictureEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.imageSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.accessCodeTextEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.signInKeySimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.backHyperLinkEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dxErrorProvider).EndInit();
		base.ResumeLayout(false);
	}
}
