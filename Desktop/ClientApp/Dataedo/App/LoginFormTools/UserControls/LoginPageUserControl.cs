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
using Dataedo.App.LoginFormTools.Tools.Licenses;
using Dataedo.App.LoginFormTools.UserControls.Base;
using Dataedo.App.LoginFormTools.UserControls.Common;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Tracking;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.CustomControls;
using Dataedo.Data.Commands.Enums;
using Dataedo.LicenseFile;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.LoginFormTools.UserControls;

public class LoginPageUserControl : BasePageUserControl
{
    private LoginType loginPageType;

    private LicenseFileReader LicenseFileReader;

    private string emailBeforeSwitch;

    private string filePathBeforeSwitch;

    private const string ChangeLicenseTypeLabelDataedoAccountText = "<href=>Login with offline license file</href>";

    private const string ChangeLicenseTypeLabelLicenseFileText = "<href=>Login with Dataedo Account</href>";

    private const string DescriptionOnDataedoAccountText = "Please login with your email.";

    private const string DescriptionOnLicenseFileText = "Please login with an offline license file.";

    private IContainer components;

    private NonCustomizableLayoutControl mainLayoutControl;

    private LayoutControlGroup mainLayoutControlGroup;

    private EmptySpaceItem logoPictureEditTopSeparatorEmptySpaceItem;

    private EmptySpaceItem logoPictureEditRightSeparatorEmptySpaceItem;

    private LabelControl headerLabelControl;

    private LayoutControlItem headerLabelControlLayoutControlItem;

    private PictureEdit rightImagePictureEdit;

    private LayoutControlItem rightImagePictureEditLayoutControlItem;

    private LabelControl privacyPolicyCheckEditLabelControl;

    private LabelControl description1LabelControl;

    private LayoutControlItem description1LabelControlLayoutControlItem;

    private LayoutControlItem privacyPolicyCheckEditLabelControlLayoutControlItem;

    private EmptySpaceItem imageSeparatorEmptySpaceItem;

    private SimpleButton nextSimpleButton;

    private LayoutControlItem nextSimpleButtonLayoutControlItem;

    private ToolTipController toolTipController;

    private SmallLogoUserControl smallLogoUserControl;

    private LayoutControlItem smallLogoUserControlLayoutControlItem;

    private CheckEdit privacyPolicyCheckEdit;

    private LayoutControlItem privacyPolicyCheckEditLayoutControlItem;

    private TextEdit emailTextEdit;

    private LayoutControlItem emailTextEditLayoutControlItem;

    private LabelControl emailTextEditLabelControl;

    private LayoutControlItem emailTextEditLabelControlLayoutControlItem;

    private DXErrorProvider dxErrorProvider;

    private EmptySpaceItem separatorEmptySpaceItem;

    private SimpleButton browseSimpleButton;

    private LayoutControlItem browseButtonLayoutControlItem;

    private LabelControl moreInfoLabelControl;

    private LayoutControlItem moreInfoLayoutControlItem;

    private LabelControl changeLicenseTypeLabel;

    private EmptySpaceItem emptySpaceItem1;

    private LayoutControlItem layoutControlItem1;

    public LoginPageUserControl()
    {
        InitializeComponent();
        browseButtonLayoutControlItem.Visibility = LayoutVisibility.Never;
        LicenseFileReader = new LicenseFileReader();
    }

    internal override void SetParameter(object parameter, bool isCalledAsPrevious)
    {
        TrackingRunner.Track(delegate
        {
            TrackingService.MakeAsyncRequest(new ParametersWithOsDataedoBuilder(new TrackingOSParameters(), new TrackingDataedoParameters()), TrackingEventEnum.WelcomePage);
        });
        base.SetParameter(parameter, isCalledAsPrevious);
        emailTextEdit.Text = null;
        privacyPolicyCheckEdit.Checked = false;
        nextSimpleButton.Enabled = false;
        loginPageType = ((StaticData.LicenseEnum == LicenseEnum.LocalFile) ? LoginType.LicenseFile : LoginType.DataedoAccount);
        dxErrorProvider.ClearErrors();
        if (parameter is string text)
        {
            emailTextEdit.Text = text;
            privacyPolicyCheckEdit.Checked = true;
            nextSimpleButton.Focus();
        }
        else if (parameter is LoginDataModel loginDataModel)
        {
            emailTextEdit.Text = loginDataModel.Email;
            privacyPolicyCheckEdit.Checked = true;
            nextSimpleButton.Focus();
        }
        else if (parameter is LicenseFileDataModel licenseFileDataModel)
        {
            loginPageType = LoginType.LicenseFile;
            emailTextEdit.Text = licenseFileDataModel.Path;
        }
        else
        {
            emailTextEdit.Focus();
        }
        SetLoginPageView();
    }

    internal override async Task<bool> Navigated()
    {
        _ = 1;
        try
        {
            await base.Navigated();
            ShowLoader();
            if (!base.SuppressNextAction)
            {
                await ContinueWithStoredToken();
            }
            ChangeControlsVisibility(visible: true);
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

    private async Task<bool> ContinueWithStoredToken()
    {
        try
        {
            string storedToken = SessionDataHelper.GetStoredToken();
            LicenseFileDataContainer licenseFileData = LicenseFileDataHelper.GetLicenseFileData();
            if (licenseFileData != null)
            {
                return LoadOfflineData(licenseFileData);
            }
            ResultWithData<ProfileResult> result = null;
            try
            {
                result = await LoadOnlineData(storedToken);
            }
            catch (Exception)
            {
            }
            if (result == null || !result.IsOK)
            {
                try
                {
                    return LoadCachedProfile(storedToken);
                }
                catch (Exception exception)
                {
                    GeneralExceptionHandling.Handle(exception, "Unable to login.", base.ParentForm);
                }
            }
            return true;
        }
        catch
        {
            GeneralMessageBoxesHandling.Show("Unable to load profile data." + Environment.NewLine + Environment.NewLine + "Make sure you have an Internet connection if you are logging in for the first time.", "Unable to login.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
            return false;
        }
    }

    private bool LoadOfflineData(LicenseFileDataContainer storedFileData)
    {
        try
        {
            if (storedFileData == null)
            {
                return false;
            }
            FileData fileData = new FileData();
            fileData.Map(storedFileData.LicenseFile, storedFileData.LastSelectedLicense);
            OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.FileLicenses, new LicenseFileDataModel(emailTextEdit.Text, fileData)));
        }
        catch
        {
            return false;
        }
        return true;
    }

    private async Task<ResultWithData<ProfileResult>> LoadOnlineData(string storedToken)
    {
        ResultWithData<ProfileResult> resultWithData = await new LoginService().GetProfile(storedToken);
		if (resultWithData != null && resultWithData.IsOK && resultWithData != null && resultWithData.IsNotUnauthorized && resultWithData?.Data != null)
		{
			OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.Licenses, new LoginDataModel(storedToken, resultWithData.Data.Email)));
		}
		return resultWithData;

        //ResultWithData<ProfileResult> resultWithData = new ResultWithData<ProfileResult>(System.Net.HttpStatusCode.OK, true);
        //resultWithData.Errors = null;
        //resultWithData.Data = new ProfileResult()
        //{
        //    Email = "",
        //    FirstName="Admin",
        //    LastName="Guy"
        //};

        //OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.Licenses, new LoginDataModel("DASREQDFSA", "admin@domain.com")));

        //return resultWithData;
    }

    private bool LoadCachedProfile(string storedToken)
    {
        if (string.IsNullOrEmpty(storedToken))
        {
            return false;
        }
        SessionData storedSessionData = SessionDataHelper.GetStoredSessionData();
        if (string.IsNullOrEmpty(storedSessionData.Profile?.Email))
        {
            GeneralMessageBoxesHandling.Show("Unable to load profile data." + Environment.NewLine + Environment.NewLine + "Make sure you have an Internet connection if you are logging in for the first time.", "Unable to login.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
            return false;
        }
        OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.Licenses, new LoginDataModel(storedToken, storedSessionData.Profile?.Email)));
        return true;
    }

    private void SetNextButtonAvailability()
    {
        nextSimpleButton.Enabled = (privacyPolicyCheckEdit.Checked && !string.IsNullOrEmpty(emailTextEdit.Text) && loginPageType == LoginType.DataedoAccount) || (loginPageType == LoginType.LicenseFile && !string.IsNullOrEmpty(emailTextEdit.Text));
    }

    private async Task ProcessNextAction()
    {
        _ = 1;
        try
        {
            ShowLoader();
            if (!nextSimpleButton.Enabled)
            {
                return;
            }
            dxErrorProvider.ClearErrors();
            if (loginPageType == LoginType.DataedoAccount)
            {
                LoginService loginService = new LoginService();
                SessionData storedSessionData = SessionDataHelper.GetStoredSessionData();
                if (emailTextEdit.Text == storedSessionData?.Profile?.Email && !base.SuppressNextAction && await ContinueWithStoredToken())
                {
                    return;
                }
                ResultWithData<MessageResult> resultWithData;
                try
                {
                    resultWithData = await loginService.SignInAsync(emailTextEdit.Text);
                }
                catch (Exception ex)
                {
                    HideLoader();
                    string message = string.Empty;
                    bool flag = GeneralExceptionHandling.IsSockedAccessDenied(ex, ref message);
                    if (ex is HttpRequestException || ex is TaskCanceledException || flag)
                    {
                        if (!string.IsNullOrEmpty(message))
                        {
                            message = "<i>" + message + "</i> " + Environment.NewLine + Environment.NewLine;
                        }
                        GeneralMessageBoxesHandling.Show("Unable to login." + Environment.NewLine + Environment.NewLine + "Unable to connect to Dataedo server." + Environment.NewLine + Environment.NewLine + message + "Please check your internet connection and firewall settings. Read more about possible solutions in the<href=" + Links.ConnectionTroubleshooting + "> Troubleshooting</href> article.", "Sign in", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
                    }
                    else
                    {
                        GeneralExceptionHandling.Handle(ex, "Unable to login.", base.ParentForm);
                    }
                    return;
                }
                if (resultWithData.IsOK)
                {
                    OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.AccessCode, emailTextEdit.Text));
                }
                else if (resultWithData.HasErrors)
                {
                    string message2 = resultWithData.Errors.Message + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, resultWithData.Errors.Errors.SelectMany((KeyValuePair<string, string[]> x) => x.Value));
                    if (resultWithData.Errors.Errors.ContainsKey("email"))
                    {
                        dxErrorProvider.SetError(emailTextEdit, string.Join(Environment.NewLine, resultWithData.Errors.Errors["email"]));
                    }
                    GeneralMessageBoxesHandling.Show(message2, "Sign in", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
                }
                else if (resultWithData.ShouldProposeTryAgain)
                {
                    GeneralMessageBoxesHandling.Show("Unable to sign in at this time." + Environment.NewLine + "Please try again in a few minutes", "Sign in", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
                }
            }
            else
            {
                if (loginPageType != LoginType.LicenseFile)
                {
                    return;
                }
                try
                {
                    if (!LicenseFileReader.SetLicenseFileData(emailTextEdit.Text))
                    {
                        throw new Exception();
                    }
                    FileData fileData = new FileData();
                    fileData.Map(LicenseFileReader.LicenseFileModel);
                    OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.FileLicenses, new LicenseFileDataModel(emailTextEdit.Text, fileData)));
                    return;
                }
                catch (Exception)
                {
                    GeneralMessageBoxesHandling.Show("Incorrect license file provided.\nHow to get an <href=https://dataedo.com/docs/offline-licenses?utm_source=App&utm_medium=App>offline license</href> file.", "License file", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
                    return;
                }
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

    private void ChangeControlsVisibility(bool visible)
    {
        LayoutVisibility visibility = ((!visible) ? LayoutVisibility.Never : LayoutVisibility.Always);
        description1LabelControlLayoutControlItem.Visibility = visibility;
        emailTextEditLabelControlLayoutControlItem.Visibility = visibility;
        emailTextEditLayoutControlItem.Visibility = visibility;
        privacyPolicyCheckEditLayoutControlItem.Visibility = visibility;
        privacyPolicyCheckEditLabelControlLayoutControlItem.Visibility = visibility;
        nextSimpleButtonLayoutControlItem.Visibility = visibility;
    }

    private void Description2LabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
    {
        OpeningLinks.OpenLink(e);
    }

    private async void NextSimpleButton_Click(object sender, EventArgs e)
    {
        await ProcessNextAction();
    }

    private void PrivacyPolicyCheckEditLabelControl_Click(object sender, EventArgs e)
    {
        privacyPolicyCheckEdit.Checked = !privacyPolicyCheckEdit.Checked;
    }

    private void PrivacyPolicyCheckEdit_CheckedChanged(object sender, EventArgs e)
    {
        SetNextButtonAvailability();
    }

    private void EmailTextEdit_TextChanged(object sender, EventArgs e)
    {
        SetNextButtonAvailability();
    }

    private async void EmailTextEdit_KeyDown(object sender, KeyEventArgs e)
    {
        await ProcessNextActionOnKeyDown(sender, e);
    }

    private async void PrivacyPolicyCheckEdit_KeyDown(object sender, KeyEventArgs e)
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

    private void loginLicenseTypeLabel_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
    {
        if (loginPageType == LoginType.DataedoAccount)
        {
            loginPageType = LoginType.LicenseFile;
        }
        else
        {
            loginPageType = LoginType.DataedoAccount;
        }
        SetLoginPageView();
    }

    private void SetLoginPageView()
    {
        if (loginPageType == LoginType.DataedoAccount)
        {
            emailTextEditLabelControl.Text = "Email:";
            filePathBeforeSwitch = emailTextEdit.Text;
            emailTextEdit.Text = emailBeforeSwitch;
            changeLicenseTypeLabel.Text = "<href=>Login with offline license file</href>";
            description1LabelControl.Text = "Please login with your email.";
        }
        else
        {
            emailTextEditLabelControl.Text = "Select license file:";
            emailBeforeSwitch = emailTextEdit.Text;
            emailTextEdit.Text = filePathBeforeSwitch;
            changeLicenseTypeLabel.Text = "<href=>Login with Dataedo Account</href>";
            description1LabelControl.Text = "Please login with an offline license file.";
        }
        LayoutControlItem layoutControlItem = privacyPolicyCheckEditLabelControlLayoutControlItem;
        bool contentVisible = (privacyPolicyCheckEditLayoutControlItem.ContentVisible = loginPageType == LoginType.DataedoAccount);
        layoutControlItem.ContentVisible = contentVisible;
        LayoutVisibility layoutVisibility3 = (moreInfoLayoutControlItem.Visibility = (browseButtonLayoutControlItem.Visibility = ((loginPageType != LoginType.LicenseFile) ? LayoutVisibility.Never : LayoutVisibility.Always)));
        layoutVisibility3 = (privacyPolicyCheckEditLabelControlLayoutControlItem.Visibility = (privacyPolicyCheckEditLayoutControlItem.Visibility = ((loginPageType != 0) ? LayoutVisibility.Never : LayoutVisibility.Always)));
        SetNextButtonAvailability();
    }

    private void browseSimpleButton_Click(object sender, EventArgs e)
    {
        using OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.FileName = emailTextEdit.Text;
        openFileDialog.Filter = "Dataedo license (*.dkey)|*.dkey|All files (*.*)|*.*";
        if (openFileDialog.ShowDialog(this) == DialogResult.OK)
        {
            if (openFileDialog.FileNames.Length == 1)
            {
                emailTextEdit.Text = openFileDialog.FileName;
            }
            else
            {
                emailTextEdit.Text = "\"" + string.Join("\" \"", openFileDialog.FileNames) + "\"";
            }
            SetNextButtonAvailability();
        }
    }

    private void MoreInfoLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
    {
        OpeningLinks.OpenLink(e);
    }

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
        this.dxErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
        this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
        this.changeLicenseTypeLabel = new DevExpress.XtraEditors.LabelControl();
        this.moreInfoLabelControl = new DevExpress.XtraEditors.LabelControl();
        this.browseSimpleButton = new DevExpress.XtraEditors.SimpleButton();
        this.emailTextEditLabelControl = new DevExpress.XtraEditors.LabelControl();
        this.smallLogoUserControl = new Dataedo.App.LoginFormTools.UserControls.Common.SmallLogoUserControl();
        this.nextSimpleButton = new DevExpress.XtraEditors.SimpleButton();
        this.privacyPolicyCheckEditLabelControl = new DevExpress.XtraEditors.LabelControl();
        this.description1LabelControl = new DevExpress.XtraEditors.LabelControl();
        this.headerLabelControl = new DevExpress.XtraEditors.LabelControl();
        this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
        this.logoPictureEditTopSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
        this.logoPictureEditRightSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
        this.headerLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.description1LabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.privacyPolicyCheckEditLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.imageSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
        this.nextSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.smallLogoUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.emailTextEditLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.separatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
        this.browseButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
        this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
        this.moreInfoLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.emailTextEdit = new DevExpress.XtraEditors.TextEdit();
        this.privacyPolicyCheckEdit = new DevExpress.XtraEditors.CheckEdit();
        this.rightImagePictureEdit = new DevExpress.XtraEditors.PictureEdit();
        this.rightImagePictureEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.privacyPolicyCheckEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.emailTextEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        ((System.ComponentModel.ISupportInitialize)this.dxErrorProvider).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
        this.mainLayoutControl.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.privacyPolicyCheckEditLabelControlLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.imageSeparatorEmptySpaceItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.nextSimpleButtonLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.emailTextEditLabelControlLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.browseButtonLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.moreInfoLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.emailTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.privacyPolicyCheckEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.rightImagePictureEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.rightImagePictureEditLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.privacyPolicyCheckEditLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.emailTextEditLayoutControlItem).BeginInit();
        base.SuspendLayout();
        this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
        this.dxErrorProvider.ContainerControl = this;
        this.mainLayoutControl.AllowCustomization = false;
        this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
        this.mainLayoutControl.Controls.Add(this.changeLicenseTypeLabel);
        this.mainLayoutControl.Controls.Add(this.moreInfoLabelControl);
        this.mainLayoutControl.Controls.Add(this.browseSimpleButton);
        this.mainLayoutControl.Controls.Add(this.emailTextEditLabelControl);
        this.mainLayoutControl.Controls.Add(this.emailTextEdit);
        this.mainLayoutControl.Controls.Add(this.privacyPolicyCheckEdit);
        this.mainLayoutControl.Controls.Add(this.smallLogoUserControl);
        this.mainLayoutControl.Controls.Add(this.nextSimpleButton);
        this.mainLayoutControl.Controls.Add(this.privacyPolicyCheckEditLabelControl);
        this.mainLayoutControl.Controls.Add(this.description1LabelControl);
        this.mainLayoutControl.Controls.Add(this.rightImagePictureEdit);
        this.mainLayoutControl.Controls.Add(this.headerLabelControl);
        this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
        this.mainLayoutControl.Name = "mainLayoutControl";
        this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1863, 452, 855, 685);
        this.mainLayoutControl.Root = this.mainLayoutControlGroup;
        this.mainLayoutControl.Size = new System.Drawing.Size(700, 470);
        this.mainLayoutControl.TabIndex = 0;
        this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
        this.changeLicenseTypeLabel.AllowHtmlString = true;
        this.changeLicenseTypeLabel.Appearance.Options.UseTextOptions = true;
        this.changeLicenseTypeLabel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        this.changeLicenseTypeLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
        this.changeLicenseTypeLabel.Location = new System.Drawing.Point(27, 392);
        this.changeLicenseTypeLabel.Name = "changeLicenseTypeLabel";
        this.changeLicenseTypeLabel.Size = new System.Drawing.Size(316, 13);
        this.changeLicenseTypeLabel.StyleController = this.mainLayoutControl;
        this.changeLicenseTypeLabel.TabIndex = 23;
        this.changeLicenseTypeLabel.Text = "<href=>Login with offline license file</href>";
        this.changeLicenseTypeLabel.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(loginLicenseTypeLabel_HyperlinkClick);
        this.moreInfoLabelControl.AllowHtmlString = true;
        this.moreInfoLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
        this.moreInfoLabelControl.Appearance.Options.UseFont = true;
        this.moreInfoLabelControl.Appearance.Options.UseTextOptions = true;
        this.moreInfoLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
        this.moreInfoLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
        this.moreInfoLabelControl.Location = new System.Drawing.Point(27, 243);
        this.moreInfoLabelControl.Name = "moreInfoLabelControl";
        this.moreInfoLabelControl.Size = new System.Drawing.Size(316, 32);
        this.moreInfoLabelControl.StyleController = this.mainLayoutControl;
        this.moreInfoLabelControl.TabIndex = 22;
        this.moreInfoLabelControl.Text = "How to get an <href=https://dataedo.com/docs/offline-licenses?utm_source=App&utm_medium=App>offline license</href> file";
        this.moreInfoLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(MoreInfoLabelControl_HyperlinkClick);
        this.browseSimpleButton.Location = new System.Drawing.Point(320, 194);
        this.browseSimpleButton.Margin = new System.Windows.Forms.Padding(2);
        this.browseSimpleButton.MaximumSize = new System.Drawing.Size(0, 20);
        this.browseSimpleButton.MinimumSize = new System.Drawing.Size(0, 20);
        this.browseSimpleButton.Name = "browseSimpleButton";
        this.browseSimpleButton.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
        this.browseSimpleButton.Size = new System.Drawing.Size(23, 20);
        this.browseSimpleButton.StyleController = this.mainLayoutControl;
        this.browseSimpleButton.TabIndex = 21;
        this.browseSimpleButton.Text = "...";
        this.browseSimpleButton.Click += new System.EventHandler(browseSimpleButton_Click);
        this.emailTextEditLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
        this.emailTextEditLabelControl.Appearance.Options.UseFont = true;
        this.emailTextEditLabelControl.Appearance.Options.UseTextOptions = true;
        this.emailTextEditLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
        this.emailTextEditLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
        this.emailTextEditLabelControl.Location = new System.Drawing.Point(27, 174);
        this.emailTextEditLabelControl.Name = "emailTextEditLabelControl";
        this.emailTextEditLabelControl.Size = new System.Drawing.Size(316, 16);
        this.emailTextEditLabelControl.StyleController = this.mainLayoutControl;
        this.emailTextEditLabelControl.TabIndex = 19;
        this.emailTextEditLabelControl.Text = "Email:";
        this.smallLogoUserControl.Location = new System.Drawing.Point(27, 419);
        this.smallLogoUserControl.Margin = new System.Windows.Forms.Padding(0);
        this.smallLogoUserControl.MaximumSize = new System.Drawing.Size(93, 24);
        this.smallLogoUserControl.MinimumSize = new System.Drawing.Size(93, 24);
        this.smallLogoUserControl.Name = "smallLogoUserControl";
        this.smallLogoUserControl.Size = new System.Drawing.Size(93, 24);
        this.smallLogoUserControl.TabIndex = 15;
        this.nextSimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
        this.nextSimpleButton.Location = new System.Drawing.Point(113, 341);
        this.nextSimpleButton.MaximumSize = new System.Drawing.Size(145, 0);
        this.nextSimpleButton.MinimumSize = new System.Drawing.Size(145, 0);
        this.nextSimpleButton.Name = "nextSimpleButton";
        this.nextSimpleButton.Size = new System.Drawing.Size(145, 29);
        this.nextSimpleButton.StyleController = this.mainLayoutControl;
        this.nextSimpleButton.TabIndex = 13;
        this.nextSimpleButton.Text = "Next";
        this.nextSimpleButton.Click += new System.EventHandler(NextSimpleButton_Click);
        this.privacyPolicyCheckEditLabelControl.AllowHtmlString = true;
        this.privacyPolicyCheckEditLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
        this.privacyPolicyCheckEditLabelControl.Appearance.Options.UseFont = true;
        this.privacyPolicyCheckEditLabelControl.Appearance.Options.UseTextOptions = true;
        this.privacyPolicyCheckEditLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
        this.privacyPolicyCheckEditLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
        this.privacyPolicyCheckEditLabelControl.Location = new System.Drawing.Point(51, 281);
        this.privacyPolicyCheckEditLabelControl.Name = "privacyPolicyCheckEditLabelControl";
        this.privacyPolicyCheckEditLabelControl.Size = new System.Drawing.Size(292, 32);
        this.privacyPolicyCheckEditLabelControl.StyleController = this.mainLayoutControl;
        this.privacyPolicyCheckEditLabelControl.TabIndex = 11;
        this.privacyPolicyCheckEditLabelControl.Text = "I agree to the <href=https://dataedo.com/privacy?utm_source=App&utm_medium=App>Privacy Policy</href>";
        this.privacyPolicyCheckEditLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(Description2LabelControl_HyperlinkClick);
        this.privacyPolicyCheckEditLabelControl.Click += new System.EventHandler(PrivacyPolicyCheckEditLabelControl_Click);
        this.description1LabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
        this.description1LabelControl.Appearance.Options.UseFont = true;
        this.description1LabelControl.Appearance.Options.UseTextOptions = true;
        this.description1LabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
        this.description1LabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
        this.description1LabelControl.Location = new System.Drawing.Point(27, 136);
        this.description1LabelControl.Name = "description1LabelControl";
        this.description1LabelControl.Size = new System.Drawing.Size(316, 16);
        this.description1LabelControl.StyleController = this.mainLayoutControl;
        this.description1LabelControl.TabIndex = 10;
        this.description1LabelControl.Text = "Please login with your email.";
        this.headerLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 24f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
        this.headerLabelControl.Appearance.Options.UseFont = true;
        this.headerLabelControl.Location = new System.Drawing.Point(27, 55);
        this.headerLabelControl.Name = "headerLabelControl";
        this.headerLabelControl.Size = new System.Drawing.Size(316, 39);
        this.headerLabelControl.StyleController = this.mainLayoutControl;
        this.headerLabelControl.TabIndex = 8;
        this.headerLabelControl.Text = "Welcome to Dataedo";
        this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
        this.mainLayoutControlGroup.GroupBordersVisible = false;
        this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[17]
        {
            this.logoPictureEditTopSeparatorEmptySpaceItem, this.logoPictureEditRightSeparatorEmptySpaceItem, this.headerLabelControlLayoutControlItem, this.rightImagePictureEditLayoutControlItem, this.description1LabelControlLayoutControlItem, this.privacyPolicyCheckEditLabelControlLayoutControlItem, this.imageSeparatorEmptySpaceItem, this.nextSimpleButtonLayoutControlItem, this.smallLogoUserControlLayoutControlItem, this.privacyPolicyCheckEditLayoutControlItem,
            this.emailTextEditLayoutControlItem, this.emailTextEditLabelControlLayoutControlItem, this.separatorEmptySpaceItem, this.browseButtonLayoutControlItem, this.emptySpaceItem1, this.layoutControlItem1, this.moreInfoLayoutControlItem
        });
        this.mainLayoutControlGroup.Name = "Root";
        this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 25, 25);
        this.mainLayoutControlGroup.Size = new System.Drawing.Size(700, 470);
        this.mainLayoutControlGroup.TextVisible = false;
        this.logoPictureEditTopSeparatorEmptySpaceItem.AllowHotTrack = false;
        this.logoPictureEditTopSeparatorEmptySpaceItem.Location = new System.Drawing.Point(0, 382);
        this.logoPictureEditTopSeparatorEmptySpaceItem.Name = "logoPictureEditTopSeparatorEmptySpaceItem";
        this.logoPictureEditTopSeparatorEmptySpaceItem.Size = new System.Drawing.Size(97, 10);
        this.logoPictureEditTopSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
        this.logoPictureEditRightSeparatorEmptySpaceItem.AllowHotTrack = false;
        this.logoPictureEditRightSeparatorEmptySpaceItem.Location = new System.Drawing.Point(97, 382);
        this.logoPictureEditRightSeparatorEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 38);
        this.logoPictureEditRightSeparatorEmptySpaceItem.MinSize = new System.Drawing.Size(104, 38);
        this.logoPictureEditRightSeparatorEmptySpaceItem.Name = "logoPictureEditRightSeparatorEmptySpaceItem";
        this.logoPictureEditRightSeparatorEmptySpaceItem.Size = new System.Drawing.Size(223, 38);
        this.logoPictureEditRightSeparatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.logoPictureEditRightSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
        this.headerLabelControlLayoutControlItem.Control = this.headerLabelControl;
        this.headerLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
        this.headerLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 109);
        this.headerLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(300, 109);
        this.headerLabelControlLayoutControlItem.Name = "headerLabelControlLayoutControlItem";
        this.headerLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 30, 40);
        this.headerLabelControlLayoutControlItem.Size = new System.Drawing.Size(320, 109);
        this.headerLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.headerLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.headerLabelControlLayoutControlItem.TextVisible = false;
        this.description1LabelControlLayoutControlItem.Control = this.description1LabelControl;
        this.description1LabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 109);
        this.description1LabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 38);
        this.description1LabelControlLayoutControlItem.MinSize = new System.Drawing.Size(16, 38);
        this.description1LabelControlLayoutControlItem.Name = "description1LabelControlLayoutControlItem";
        this.description1LabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 20);
        this.description1LabelControlLayoutControlItem.Size = new System.Drawing.Size(320, 38);
        this.description1LabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.description1LabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.description1LabelControlLayoutControlItem.TextVisible = false;
        this.privacyPolicyCheckEditLabelControlLayoutControlItem.Control = this.privacyPolicyCheckEditLabelControl;
        this.privacyPolicyCheckEditLabelControlLayoutControlItem.Location = new System.Drawing.Point(24, 252);
        this.privacyPolicyCheckEditLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 38);
        this.privacyPolicyCheckEditLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(16, 38);
        this.privacyPolicyCheckEditLabelControlLayoutControlItem.Name = "privacyPolicyCheckEditLabelControlLayoutControlItem";
        this.privacyPolicyCheckEditLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 4, 2);
        this.privacyPolicyCheckEditLabelControlLayoutControlItem.Size = new System.Drawing.Size(296, 38);
        this.privacyPolicyCheckEditLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.privacyPolicyCheckEditLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.privacyPolicyCheckEditLabelControlLayoutControlItem.TextVisible = false;
        this.imageSeparatorEmptySpaceItem.AllowHotTrack = false;
        this.imageSeparatorEmptySpaceItem.Location = new System.Drawing.Point(320, 0);
        this.imageSeparatorEmptySpaceItem.MaxSize = new System.Drawing.Size(50, 420);
        this.imageSeparatorEmptySpaceItem.MinSize = new System.Drawing.Size(50, 420);
        this.imageSeparatorEmptySpaceItem.Name = "imageSeparatorEmptySpaceItem";
        this.imageSeparatorEmptySpaceItem.Size = new System.Drawing.Size(50, 420);
        this.imageSeparatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.imageSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
        this.nextSimpleButtonLayoutControlItem.Control = this.nextSimpleButton;
        this.nextSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(0, 316);
        this.nextSimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(0, 39);
        this.nextSimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(138, 39);
        this.nextSimpleButtonLayoutControlItem.Name = "nextSimpleButtonLayoutControlItem";
        this.nextSimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(88, 0, 0, 10);
        this.nextSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(320, 39);
        this.nextSimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.nextSimpleButtonLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
        this.nextSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.nextSimpleButtonLayoutControlItem.TextToControlDistance = 0;
        this.nextSimpleButtonLayoutControlItem.TextVisible = false;
        this.smallLogoUserControlLayoutControlItem.Control = this.smallLogoUserControl;
        this.smallLogoUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 392);
        this.smallLogoUserControlLayoutControlItem.Name = "smallLogoUserControlLayoutControlItem";
        this.smallLogoUserControlLayoutControlItem.Size = new System.Drawing.Size(97, 28);
        this.smallLogoUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.smallLogoUserControlLayoutControlItem.TextVisible = false;
        this.emailTextEditLabelControlLayoutControlItem.Control = this.emailTextEditLabelControl;
        this.emailTextEditLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 147);
        this.emailTextEditLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 20);
        this.emailTextEditLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(16, 20);
        this.emailTextEditLabelControlLayoutControlItem.Name = "emailTextEditLabelControlLayoutControlItem";
        this.emailTextEditLabelControlLayoutControlItem.Size = new System.Drawing.Size(320, 20);
        this.emailTextEditLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.emailTextEditLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.emailTextEditLabelControlLayoutControlItem.TextVisible = false;
        this.separatorEmptySpaceItem.AllowHotTrack = false;
        this.separatorEmptySpaceItem.Location = new System.Drawing.Point(0, 290);
        this.separatorEmptySpaceItem.MinSize = new System.Drawing.Size(104, 24);
        this.separatorEmptySpaceItem.Name = "separatorEmptySpaceItem";
        this.separatorEmptySpaceItem.Size = new System.Drawing.Size(320, 26);
        this.separatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.separatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
        this.browseButtonLayoutControlItem.Control = this.browseSimpleButton;
        this.browseButtonLayoutControlItem.Location = new System.Drawing.Point(293, 167);
        this.browseButtonLayoutControlItem.Name = "browseButtonLayoutControlItem";
        this.browseButtonLayoutControlItem.Size = new System.Drawing.Size(27, 47);
        this.browseButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.browseButtonLayoutControlItem.TextVisible = false;
        this.emptySpaceItem1.AllowHotTrack = false;
        this.emptySpaceItem1.Location = new System.Drawing.Point(0, 355);
        this.emptySpaceItem1.Name = "emptySpaceItem1";
        this.emptySpaceItem1.Size = new System.Drawing.Size(320, 10);
        this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
        this.layoutControlItem1.Control = this.changeLicenseTypeLabel;
        this.layoutControlItem1.Location = new System.Drawing.Point(0, 365);
        this.layoutControlItem1.Name = "layoutControlItem1";
        this.layoutControlItem1.Size = new System.Drawing.Size(320, 17);
        this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
        this.layoutControlItem1.TextVisible = false;
        this.moreInfoLayoutControlItem.Control = this.moreInfoLabelControl;
        this.moreInfoLayoutControlItem.Location = new System.Drawing.Point(0, 214);
        this.moreInfoLayoutControlItem.MaxSize = new System.Drawing.Size(0, 38);
        this.moreInfoLayoutControlItem.MinSize = new System.Drawing.Size(16, 38);
        this.moreInfoLayoutControlItem.Name = "moreInfoLayoutControlItem";
        this.moreInfoLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 4, 2);
        this.moreInfoLayoutControlItem.Size = new System.Drawing.Size(320, 38);
        this.moreInfoLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.moreInfoLayoutControlItem.TextLocation = DevExpress.Utils.Locations.Bottom;
        this.moreInfoLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.moreInfoLayoutControlItem.TextVisible = false;
        this.moreInfoLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        this.emailTextEdit.Location = new System.Drawing.Point(27, 194);
        this.emailTextEdit.MaximumSize = new System.Drawing.Size(0, 20);
        this.emailTextEdit.Name = "emailTextEdit";
        this.emailTextEdit.Size = new System.Drawing.Size(289, 20);
        this.emailTextEdit.StyleController = this.mainLayoutControl;
        this.emailTextEdit.TabIndex = 18;
        this.emailTextEdit.ToolTipController = this.toolTipController;
        this.emailTextEdit.TextChanged += new System.EventHandler(EmailTextEdit_TextChanged);
        this.emailTextEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(EmailTextEdit_KeyDown);
        this.privacyPolicyCheckEdit.Location = new System.Drawing.Point(27, 279);
        this.privacyPolicyCheckEdit.Name = "privacyPolicyCheckEdit";
        this.privacyPolicyCheckEdit.Properties.Caption = "";
        this.privacyPolicyCheckEdit.Size = new System.Drawing.Size(20, 20);
        this.privacyPolicyCheckEdit.StyleController = this.mainLayoutControl;
        this.privacyPolicyCheckEdit.TabIndex = 17;
        this.privacyPolicyCheckEdit.CheckedChanged += new System.EventHandler(PrivacyPolicyCheckEdit_CheckedChanged);
        this.privacyPolicyCheckEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(PrivacyPolicyCheckEdit_KeyDown);
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
        this.privacyPolicyCheckEditLayoutControlItem.Control = this.privacyPolicyCheckEdit;
        this.privacyPolicyCheckEditLayoutControlItem.Location = new System.Drawing.Point(0, 252);
        this.privacyPolicyCheckEditLayoutControlItem.MaxSize = new System.Drawing.Size(24, 36);
        this.privacyPolicyCheckEditLayoutControlItem.MinSize = new System.Drawing.Size(24, 36);
        this.privacyPolicyCheckEditLayoutControlItem.Name = "privacyPolicyCheckEditLayoutControlItem";
        this.privacyPolicyCheckEditLayoutControlItem.Size = new System.Drawing.Size(24, 38);
        this.privacyPolicyCheckEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.privacyPolicyCheckEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.privacyPolicyCheckEditLayoutControlItem.TextVisible = false;
        this.emailTextEditLayoutControlItem.Control = this.emailTextEdit;
        this.emailTextEditLayoutControlItem.Location = new System.Drawing.Point(0, 167);
        this.emailTextEditLayoutControlItem.MaxSize = new System.Drawing.Size(0, 47);
        this.emailTextEditLayoutControlItem.MinSize = new System.Drawing.Size(54, 47);
        this.emailTextEditLayoutControlItem.Name = "emailTextEditLayoutControlItem";
        this.emailTextEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 25);
        this.emailTextEditLayoutControlItem.Size = new System.Drawing.Size(293, 47);
        this.emailTextEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.emailTextEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.emailTextEditLayoutControlItem.TextVisible = false;
        base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        base.Controls.Add(this.mainLayoutControl);
        base.Name = "LoginPageUserControl";
        base.Load += new System.EventHandler(WelcomePageUserControl_Load);
        ((System.ComponentModel.ISupportInitialize)this.dxErrorProvider).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
        this.mainLayoutControl.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.privacyPolicyCheckEditLabelControlLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.imageSeparatorEmptySpaceItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.nextSimpleButtonLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.emailTextEditLabelControlLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.browseButtonLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.moreInfoLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.emailTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.privacyPolicyCheckEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.rightImagePictureEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.rightImagePictureEditLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.privacyPolicyCheckEditLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.emailTextEditLayoutControlItem).EndInit();
        base.ResumeLayout(false);
    }
}
