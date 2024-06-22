using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Dataedo.App.API.Models;
using Dataedo.App.API.Services;
using Dataedo.App.LoginFormTools.Tools;
using Dataedo.App.LoginFormTools.Tools.Common;
using Dataedo.App.LoginFormTools.Tools.CustomEventArgs;
using Dataedo.App.LoginFormTools.Tools.Enums;
using Dataedo.App.LoginFormTools.Tools.Licenses;
using Dataedo.App.LoginFormTools.UserControls.Base;
using Dataedo.App.LoginFormTools.UserControls.Common;
using Dataedo.App.LoginFormTools.UserControls.Subcontrols;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.LoginFormTools.UserControls;

public class LicensesPageUserControl : BasePageUserControl
{
    private readonly DoubleClickSupport tileViewDoubleClickSupport;

    private readonly RecentListUserControl recentListUserControl;

    private readonly NoLicensesUserControl noLicensesUserControl;

    private LoginDataModel loginDataModel;

    private ProfileResult profile;

    private List<LicenseDataResult> licenses;

    private string lastLicenseId;

    private LicenseDataResult selectedLicense;

    private string email;

    private bool isCachedListLoaded;

    private IContainer components;

    private NonCustomizableLayoutControl mainLayoutControl;

    private LayoutControlGroup mainLayoutControlGroup;

    private EmptySpaceItem logoPictureEditRightSeparatorEmptySpaceItem;

    private LabelControl headerLabelControl;

    private LayoutControlItem headerLabelControlLayoutControlItem;

    private LabelControl description1LabelControl;

    private LayoutControlItem description1LabelControlLayoutControlItem;

    private SimpleButton backSimpleButton;

    private LayoutControlItem backSimpleButtonLayoutControlItem;

    private EmptySpaceItem buttonsEmptySpaceItem;

    private SimpleButton nextSimpleButton;

    private LayoutControlItem nextSimpleButtonLayoutControlItem;

    private SmallLogoUserControl smallLogoUserControl;

    private LayoutControlItem smallLogoUserControlLayoutControlItem;

    private ToolTipController toolTipController;

    private SimpleButton refreshSimpleButton;

    private LayoutControlItem refreshSimpleButtonLayoutControlItem;

    private PanelControl panelControl;

    private LayoutControlItem panelControlLayoutControlItem;

    private EmptySpaceItem separatorEmptySpaceItem;

    private LabelControl logIntoAccountLabelControl;

    private LayoutControlItem layoutControlItem1;

    private EmptySpaceItem emptySpaceItem1;

    private EmptySpaceItem emptySpaceItem2;

    private LabelControl convertLegacyKeyLabelControl;

    private LayoutControlItem convertLegacyKeyLabelControlLayoutControlItem;

    private InfoUserControl infoUserControl;

    private LayoutControlItem layoutControlItem2;

    private string Token => loginDataModel?.Token;

    public LicensesPageUserControl()
    {
        InitializeComponent();
        recentListUserControl = new RecentListUserControl();
        recentListUserControl.Dock = DockStyle.Fill;
        noLicensesUserControl = new NoLicensesUserControl();
        noLicensesUserControl.Dock = DockStyle.Fill;
        recentListUserControl.TileViewKeyDown += TileView_KeyDown;
        recentListUserControl.FocusedRowChanged += TileView_FocusedRowChanged;
        tileViewDoubleClickSupport = new DoubleClickSupport(recentListUserControl.TileView);
        tileViewDoubleClickSupport.DoubleClick += TileViewDoubleClickSupport_DoubleClick;
        panelControl.Controls.Clear();
        panelControl.Controls.Add(recentListUserControl);
        infoUserControl.SetShouldLoadColorsAfterLoad(shouldLoadColorsAfterLoad: false);
    }

    private void LicensesPageUserControl_Load(object sender, EventArgs e)
    {
        infoUserControl.SetDeletedObjectProperties(isControlVisible: false);
    }

    private string PrepareLicenseText(ProfileResult profileResult)
    {
        email = string.Empty;
        StringBuilder stringBuilder = new StringBuilder();
        if (profileResult != null && (!string.IsNullOrEmpty(profileResult.FullName) || !string.IsNullOrEmpty(profileResult.Email)))
        {
            email = profileResult.Email;
            stringBuilder.Append("Account: <b>");
            if (!string.IsNullOrEmpty(profileResult.FullName) && !string.IsNullOrEmpty(profileResult.Email))
            {
                stringBuilder.Append(profileResult.FullName + " (" + profileResult.Email + ")");
            }
            else if (!string.IsNullOrEmpty(profileResult.FullName))
            {
                stringBuilder.Append(profileResult.FullName ?? "");
            }
            else if (!string.IsNullOrEmpty(profileResult.Email))
            {
                stringBuilder.Append(profileResult.Email ?? "");
            }
            stringBuilder.Append("</b> " + Environment.NewLine);
        }
        return stringBuilder.ToString();
    }

    internal override void SetParameter(object parameter, bool isCalledAsPrevious)
    {
        base.SetParameter(parameter, isCalledAsPrevious);
        description1LabelControl.Text = PrepareLicenseText(null);
        SetBannerVisibility(isVisible: false);
        tileViewDoubleClickSupport.SetParameters();
        this.loginDataModel = null;
        profile = null;
        licenses = null;
        if (parameter is LoginDataModel loginDataModel)
        {
            this.loginDataModel = loginDataModel;
        }
        recentListUserControl.GridControl.BeginUpdate();
        recentListUserControl.GridControl.DataSource = null;
        recentListUserControl.GridControl.EndUpdate();
        backSimpleButtonLayoutControlItem.Visibility = ((!base.BackButtonVisibility) ? LayoutVisibility.Never : LayoutVisibility.Always);
        SetNextButtonAvailability();
    }

    private void InfoUserControl_Load(object sender, EventArgs e)
    {
        infoUserControl.SetDeletedObjectProperties(isControlVisible: false);
    }

    internal override async Task<bool> Navigated()
    {
        await base.Navigated();
        bool loadDataResult = await LoadData();
        if (!base.SuppressNextAction && loadDataResult && recentListUserControl.TileView.SelectedRowsCount > 0 && selectedLicense != null)
        {
            await ProcessNextAction(GetSelectedItem());
        }
        return loadDataResult;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        Control control = FindFocusedControl();
        if (keyData == Keys.Return)
        {
            ProcessNextAction(GetSelectedItem());
        }
        control?.Focus();
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private async Task<bool> LoadData()
    {
        try
        {
            OnTimeConsumingOperationStopped(this);
            ShowLoader();
            isCachedListLoaded = false;
            SetBannerVisibility(isVisible: false);
            LoginService loginService = new LoginService();
            ResultWithData<ProfileResult> profileResult = await loginService.GetProfile(Token);
            ResultWithData<LicensesResult> resultWithData = await loginService.GetLicenses(Token);
            description1LabelControl.Text = PrepareLicenseText(profileResult.IsOK ? profileResult.Data : null);
            if (profileResult.IsOK && resultWithData.IsOK)
            {
                profile = profileResult.Data;
                licenses = resultWithData.Data.Licenses;
                TrackingRunner.Track(delegate
                {
                    TrackingService.MakeAsyncRequest(new ParametersWithUserOsDataedoBuilder(new TrackingOSParameters(), new TrackingDataedoParameters(), new TrackingLicensesListedUserParameters(profile.Email)), TrackingEventEnum.LicensesListed);
                });
            }
            else
            {
                if (!profileResult.IsNotUnauthorized || !resultWithData.IsNotUnauthorized)
                {
                    GeneralMessageBoxesHandling.Show("Your session expired.", "Choose license", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
                    OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.Back));
                    return false;
                }
                SessionData storedSessionData = SessionDataHelper.GetStoredSessionData();
                if (storedSessionData == null || storedSessionData == null || storedSessionData.Profile?.Email == null || storedSessionData.Licenses == null)
                {
                    GeneralMessageBoxesHandling.Show("Unable to get licenses.", "Licenses", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
                    SetNextButtonAvailability();
                    return false;
                }
                profile = storedSessionData.Profile;
                licenses = storedSessionData.Licenses;
            }
            recentListUserControl.GridControl.BeginUpdate();
            recentListUserControl.GridControl.DataSource = licenses;
            recentListUserControl.GridControl.EndUpdate();
            SetNextButtonAvailability();
            recentListUserControl.GridControl.ForceInitialize();
            if (licenses.Count() > 0)
            {
                if (!panelControl.Controls.Contains(recentListUserControl))
                {
                    panelControl.Controls.Clear();
                    panelControl.Controls.Add(recentListUserControl);
                }
                selectedLicense = licenses.FirstOrDefault((LicenseDataResult x) => x.Id == SessionDataHelper.GetLastLicenseId());
                if (selectedLicense != null)
                {
                    int focusedRowHandle = recentListUserControl.TileView.FindRow(selectedLicense);
                    recentListUserControl.TileView.FocusedRowHandle = focusedRowHandle;
                }
            }
            else if (!panelControl.Controls.Contains(noLicensesUserControl))
            {
                panelControl.Controls.Clear();
                noLicensesUserControl.SetEmail(email);
                panelControl.Controls.Add(noLicensesUserControl);
            }
        }
        catch (HttpRequestException)
        {
            HideLoader();
            LoadCachedLicenses();
            GeneralMessageBoxesHandling.Show("Could not connect to dataedo.com and check available licenses." + Environment.NewLine + Environment.NewLine + "Please make sure you are connected to the Internet.", "Licenses", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
            return false;
        }
        catch (AggregateException ex)
        {
            if (ex.Flatten().InnerExceptions.Any((Exception e) => e is HttpRequestException))
            {
                HideLoader();
                LoadCachedLicenses();
                GeneralMessageBoxesHandling.Show("Could not connect to dataedo.com and check available licenses." + Environment.NewLine + Environment.NewLine + "Please make sure you are connected to the Internet.", "Licenses", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
                return false;
            }
            HideLoader();
            GeneralExceptionHandling.Handle(ex, "Error while loading licenses:", base.ParentForm);
            return false;
        }
        catch (Exception exception)
        {
            HideLoader();
            GeneralExceptionHandling.Handle(exception, "Error while loading licenses:", base.ParentForm);
            return false;
        }
        finally
        {
            HideLoader();
        }
        SetNextButtonAvailability();
        return true;
    }

    private void LoadCachedLicenses()
    {
        SessionData storedSessionData = SessionDataHelper.GetStoredSessionData();
        if (storedSessionData != null && storedSessionData.Licenses?.Any() == true)
        {
            SetBannerVisibility(isVisible: true);
        }
        infoUserControl.SetDeletedObjectProperties();
        isCachedListLoaded = true;
        profile = storedSessionData?.Profile;
        licenses = storedSessionData?.Licenses;
        recentListUserControl.GridControl.BeginUpdate();
        recentListUserControl.GridControl.DataSource = licenses;
        recentListUserControl.GridControl.EndUpdate();
        SetNextButtonAvailability();
    }

    private void SetBannerVisibility(bool isVisible)
    {
        LayoutControlItem itemByControl = mainLayoutControl.GetItemByControl(infoUserControl);
        if (itemByControl != null)
        {
            if (isVisible)
            {
                itemByControl.Visibility = LayoutVisibility.Always;
            }
            else
            {
                itemByControl.Visibility = LayoutVisibility.Never;
            }
        }
    }

    private void SetNextButtonAvailability()
    {
        nextSimpleButton.Enabled = GetSelectedItem()?.IsValid ?? false;
    }

    private async Task ProcessNextAction(LicenseDataResult licenseDataResult)
    {
        try
        {
            ShowLoader();
            if (licenseDataResult == null || !licenseDataResult.IsValid || !nextSimpleButton.Enabled)
            {
                return;
            }
            LoginService loginService = new LoginService();
            Result result = new Result(HttpStatusCode.OK, isValid: false);
            try
            {
                result = await loginService.UseLicense(Token, licenseDataResult);
            }
            catch
            {
            }
            if (result.IsOK || licenseDataResult.Id == SessionDataHelper.GetLastLicenseId() || (isCachedListLoaded && licenseDataResult.IsValid))
            {
                SessionDataHelper.Save(Token, profile, licenses, licenseDataResult);
                TrackingRunner.Track(delegate
                {
                    TrackingService.MakeAsyncRequest(new ParametersWithUserOsDataedoBuilder(new TrackingOSParameters(), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.LicenseSelected);
                });
                OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.Next));
            }
            else if (result.HasErrors)
            {
                GeneralMessageBoxesHandling.Show(result.Errors.Message + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, result.Errors.Errors.SelectMany((KeyValuePair<string, string[]> x) => x.Value)), "Choose license", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
            }
            else if (result.ShouldProposeTryAgain)
            {
                GeneralMessageBoxesHandling.Show("Unable to use license in at this time.Please try again in a few minutes.", "Licenses", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
            }
            else
            {
                GeneralMessageBoxesHandling.Show("Unable to use license.", "Licenses", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
            }
        }
        finally
        {
            HideLoader();
        }
    }

    private async void TileViewDoubleClickSupport_DoubleClick(object sender, EventArgs e)
    {
        await ProcessNextAction(GetSelectedItem());
    }

    private async void TileView_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Return)
        {
            await ProcessNextAction(GetSelectedItem());
        }
    }

    private void TileView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
    {
        TileView tileView = sender as TileView;
        LicenseDataResult selectedItem = GetSelectedItem();
        if (selectedItem == null || !selectedItem.IsValid)
        {
            tileView?.UnselectRow(e.FocusedRowHandle);
        }
        SetNextButtonAvailability();
    }

    private void BackSimpleButton_Click(object sender, EventArgs e)
    {
        OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.Back, loginDataModel));
    }

    private async void NextSimpleButton_Click(object sender, EventArgs e)
    {
        await ProcessNextAction(GetSelectedItem());
    }

    private void TileView_ItemCustomize(object sender, TileViewItemCustomizeEventArgs e)
    {
        if (base.ParentForm != null)
        {
            e.Item.AppearanceItem.Normal.BackColor = base.ParentForm.BackColor;
        }
        e.Item.AppearanceItem.Normal.Font = new Font(e.Item.AppearanceItem.Normal.Font.FontFamily, 12f);
        e.Item.AppearanceItem.Focused.BackColor = SkinsManager.CurrentSkin.LoginFormFocusedItemBackColor;
        LicenseDataResult obj = (sender as TileView)?.GetRow(e.RowHandle) as LicenseDataResult;
        if (obj != null && !obj.IsValid)
        {
            e.Item.AppearanceItem.Normal.ForeColor = SkinsManager.CurrentSkin.DisabledForeColor;
            e.Item.AppearanceItem.Focused.BackColor = e.Item.AppearanceItem.Normal.BackColor;
        }
        else if (base.ParentForm != null)
        {
            e.Item.AppearanceItem.Normal.ForeColor = base.ParentForm.ForeColor;
        }
    }

    private LicenseDataResult GetSelectedItem()
    {
        return recentListUserControl.TileView.GetRow(recentListUserControl.TileView.FocusedRowHandle) as LicenseDataResult;
    }

    private async void RefreshSimpleButton_Click(object sender, EventArgs e)
    {
        await LoadData();
    }

    private void Description1LabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
    {
        OpeningLinks.OpenLink(e);
    }

    private void logIntoAccountLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
    {
        e.Link = "https://account.dataedo.com";
        OpeningLinks.OpenLinkWithOptionalEmail(e, email, '?');
    }

    private void ConvertLegacyKeyLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
    {
        string text = HttpUtility.UrlEncode(this?.profile?.Email);
        e.Link = "https://account.dataedo.com/convert-legacy-key?email=" + text;
        OpeningLinks.OpenLink(e);
    }

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
        this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
        this.infoUserControl = new Dataedo.App.UserControls.InfoUserControl();
        this.convertLegacyKeyLabelControl = new DevExpress.XtraEditors.LabelControl();
        this.logIntoAccountLabelControl = new DevExpress.XtraEditors.LabelControl();
        this.panelControl = new DevExpress.XtraEditors.PanelControl();
        this.refreshSimpleButton = new DevExpress.XtraEditors.SimpleButton();
        this.smallLogoUserControl = new Dataedo.App.LoginFormTools.UserControls.Common.SmallLogoUserControl();
        this.nextSimpleButton = new DevExpress.XtraEditors.SimpleButton();
        this.backSimpleButton = new DevExpress.XtraEditors.SimpleButton();
        this.description1LabelControl = new DevExpress.XtraEditors.LabelControl();
        this.headerLabelControl = new DevExpress.XtraEditors.LabelControl();
        this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
        this.logoPictureEditRightSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
        this.headerLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.description1LabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.backSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.buttonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
        this.nextSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.smallLogoUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.refreshSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.panelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.separatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
        this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
        this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
        this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
        this.convertLegacyKeyLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
        ((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
        this.mainLayoutControl.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.panelControl).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.backSimpleButtonLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.nextSimpleButtonLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.refreshSimpleButtonLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.panelControlLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.convertLegacyKeyLabelControlLayoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
        base.SuspendLayout();
        this.mainLayoutControl.AllowCustomization = false;
        this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
        this.mainLayoutControl.Controls.Add(this.infoUserControl);
        this.mainLayoutControl.Controls.Add(this.convertLegacyKeyLabelControl);
        this.mainLayoutControl.Controls.Add(this.logIntoAccountLabelControl);
        this.mainLayoutControl.Controls.Add(this.panelControl);
        this.mainLayoutControl.Controls.Add(this.refreshSimpleButton);
        this.mainLayoutControl.Controls.Add(this.smallLogoUserControl);
        this.mainLayoutControl.Controls.Add(this.nextSimpleButton);
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
        this.infoUserControl.BackColor = System.Drawing.Color.LightYellow;
        this.infoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
        this.infoUserControl.Description = "Could not connect to dataedo.com and refresh licenses.";
        this.infoUserControl.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40);
        this.infoUserControl.Image = Dataedo.App.Properties.Resources.warning_16;
        this.infoUserControl.Location = new System.Drawing.Point(27, 27);
        this.infoUserControl.Name = "infoUserControl";
        this.infoUserControl.Size = new System.Drawing.Size(646, 32);
        this.infoUserControl.TabIndex = 32;
        this.infoUserControl.Visible = false;
        this.infoUserControl.Load += new System.EventHandler(InfoUserControl_Load);
        this.convertLegacyKeyLabelControl.AllowHtmlString = true;
        this.convertLegacyKeyLabelControl.Location = new System.Drawing.Point(159, 392);
        this.convertLegacyKeyLabelControl.Name = "convertLegacyKeyLabelControl";
        this.convertLegacyKeyLabelControl.Size = new System.Drawing.Size(93, 13);
        this.convertLegacyKeyLabelControl.StyleController = this.mainLayoutControl;
        this.convertLegacyKeyLabelControl.TabIndex = 31;
        this.convertLegacyKeyLabelControl.Text = "<href>Convert legacy key</href>";
        this.convertLegacyKeyLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(ConvertLegacyKeyLabelControl_HyperlinkClick);
        this.logIntoAccountLabelControl.AllowHtmlString = true;
        this.logIntoAccountLabelControl.Location = new System.Drawing.Point(27, 392);
        this.logIntoAccountLabelControl.Name = "logIntoAccountLabelControl";
        this.logIntoAccountLabelControl.Size = new System.Drawing.Size(124, 13);
        this.logIntoAccountLabelControl.StyleController = this.mainLayoutControl;
        this.logIntoAccountLabelControl.TabIndex = 1;
        this.logIntoAccountLabelControl.Text = "<href>Log into Dataedo Account</href>";
        this.logIntoAccountLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(logIntoAccountLabelControl_HyperlinkClick);
        this.panelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        this.panelControl.Location = new System.Drawing.Point(25, 147);
        this.panelControl.Margin = new System.Windows.Forms.Padding(0);
        this.panelControl.Name = "panelControl";
        this.panelControl.Size = new System.Drawing.Size(650, 233);
        this.panelControl.TabIndex = 30;
        this.refreshSimpleButton.ImageOptions.Image = Dataedo.App.Properties.Resources.refresh_16;
        this.refreshSimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
        this.refreshSimpleButton.Location = new System.Drawing.Point(410, 416);
        this.refreshSimpleButton.MaximumSize = new System.Drawing.Size(80, 29);
        this.refreshSimpleButton.MinimumSize = new System.Drawing.Size(80, 29);
        this.refreshSimpleButton.Name = "refreshSimpleButton";
        this.refreshSimpleButton.Size = new System.Drawing.Size(80, 29);
        this.refreshSimpleButton.StyleController = this.mainLayoutControl;
        this.refreshSimpleButton.TabIndex = 29;
        this.refreshSimpleButton.Text = "Refresh";
        this.refreshSimpleButton.Click += new System.EventHandler(RefreshSimpleButton_Click);
        this.smallLogoUserControl.Location = new System.Drawing.Point(27, 419);
        this.smallLogoUserControl.Margin = new System.Windows.Forms.Padding(0);
        this.smallLogoUserControl.MaximumSize = new System.Drawing.Size(93, 24);
        this.smallLogoUserControl.MinimumSize = new System.Drawing.Size(93, 24);
        this.smallLogoUserControl.Name = "smallLogoUserControl";
        this.smallLogoUserControl.Size = new System.Drawing.Size(93, 24);
        this.smallLogoUserControl.TabIndex = 22;
        this.nextSimpleButton.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
        this.nextSimpleButton.Appearance.Options.UseFont = true;
        this.nextSimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
        this.nextSimpleButton.Location = new System.Drawing.Point(590, 416);
        this.nextSimpleButton.MaximumSize = new System.Drawing.Size(85, 29);
        this.nextSimpleButton.MinimumSize = new System.Drawing.Size(85, 29);
        this.nextSimpleButton.Name = "nextSimpleButton";
        this.nextSimpleButton.Size = new System.Drawing.Size(85, 29);
        this.nextSimpleButton.StyleController = this.mainLayoutControl;
        this.nextSimpleButton.TabIndex = 20;
        this.nextSimpleButton.Text = "Next";
        this.nextSimpleButton.Click += new System.EventHandler(NextSimpleButton_Click);
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
        this.description1LabelControl.Location = new System.Drawing.Point(27, 118);
        this.description1LabelControl.Name = "description1LabelControl";
        this.description1LabelControl.Size = new System.Drawing.Size(646, 1);
        this.description1LabelControl.StyleController = this.mainLayoutControl;
        this.description1LabelControl.TabIndex = 10;
        this.description1LabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(Description1LabelControl_HyperlinkClick);
        this.headerLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 24f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
        this.headerLabelControl.Appearance.Options.UseFont = true;
        this.headerLabelControl.AutoEllipsis = true;
        this.headerLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
        this.headerLabelControl.Location = new System.Drawing.Point(27, 61);
        this.headerLabelControl.Name = "headerLabelControl";
        this.headerLabelControl.Size = new System.Drawing.Size(646, 39);
        this.headerLabelControl.StyleController = this.mainLayoutControl;
        this.headerLabelControl.TabIndex = 8;
        this.headerLabelControl.Text = "Available licenses";
        this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
        this.mainLayoutControlGroup.GroupBordersVisible = false;
        this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[15]
        {
            this.logoPictureEditRightSeparatorEmptySpaceItem, this.headerLabelControlLayoutControlItem, this.description1LabelControlLayoutControlItem, this.backSimpleButtonLayoutControlItem, this.buttonsEmptySpaceItem, this.nextSimpleButtonLayoutControlItem, this.smallLogoUserControlLayoutControlItem, this.refreshSimpleButtonLayoutControlItem, this.panelControlLayoutControlItem, this.separatorEmptySpaceItem,
            this.layoutControlItem1, this.emptySpaceItem2, this.emptySpaceItem1, this.convertLegacyKeyLabelControlLayoutControlItem, this.layoutControlItem2
        });
        this.mainLayoutControlGroup.Name = "Root";
        this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 25, 25);
        this.mainLayoutControlGroup.Size = new System.Drawing.Size(700, 470);
        this.mainLayoutControlGroup.TextVisible = false;
        this.logoPictureEditRightSeparatorEmptySpaceItem.AllowHotTrack = false;
        this.logoPictureEditRightSeparatorEmptySpaceItem.Location = new System.Drawing.Point(229, 365);
        this.logoPictureEditRightSeparatorEmptySpaceItem.Name = "logoPictureEditRightSeparatorEmptySpaceItem";
        this.logoPictureEditRightSeparatorEmptySpaceItem.Size = new System.Drawing.Size(156, 55);
        this.logoPictureEditRightSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
        this.headerLabelControlLayoutControlItem.Control = this.headerLabelControl;
        this.headerLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 36);
        this.headerLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 55);
        this.headerLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 55);
        this.headerLabelControlLayoutControlItem.Name = "headerLabelControlLayoutControlItem";
        this.headerLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 16);
        this.headerLabelControlLayoutControlItem.Size = new System.Drawing.Size(650, 55);
        this.headerLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.headerLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.headerLabelControlLayoutControlItem.TextVisible = false;
        this.description1LabelControlLayoutControlItem.Control = this.description1LabelControl;
        this.description1LabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 91);
        this.description1LabelControlLayoutControlItem.Name = "description1LabelControlLayoutControlItem";
        this.description1LabelControlLayoutControlItem.Size = new System.Drawing.Size(650, 5);
        this.description1LabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.description1LabelControlLayoutControlItem.TextVisible = false;
        this.backSimpleButtonLayoutControlItem.Control = this.backSimpleButton;
        this.backSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(465, 391);
        this.backSimpleButtonLayoutControlItem.Name = "backSimpleButtonLayoutControlItem";
        this.backSimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(20, 0, 0, 0);
        this.backSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(90, 29);
        this.backSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.backSimpleButtonLayoutControlItem.TextVisible = false;
        this.buttonsEmptySpaceItem.AllowHotTrack = false;
        this.buttonsEmptySpaceItem.Location = new System.Drawing.Point(385, 365);
        this.buttonsEmptySpaceItem.Name = "buttonsEmptySpaceItem";
        this.buttonsEmptySpaceItem.Size = new System.Drawing.Size(265, 26);
        this.buttonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
        this.nextSimpleButtonLayoutControlItem.Control = this.nextSimpleButton;
        this.nextSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(555, 391);
        this.nextSimpleButtonLayoutControlItem.Name = "nextSimpleButtonLayoutControlItem";
        this.nextSimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 0, 0, 0);
        this.nextSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(95, 29);
        this.nextSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.nextSimpleButtonLayoutControlItem.TextVisible = false;
        this.smallLogoUserControlLayoutControlItem.Control = this.smallLogoUserControl;
        this.smallLogoUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 392);
        this.smallLogoUserControlLayoutControlItem.MaxSize = new System.Drawing.Size(97, 28);
        this.smallLogoUserControlLayoutControlItem.MinSize = new System.Drawing.Size(97, 28);
        this.smallLogoUserControlLayoutControlItem.Name = "smallLogoUserControlLayoutControlItem";
        this.smallLogoUserControlLayoutControlItem.Size = new System.Drawing.Size(229, 28);
        this.smallLogoUserControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.smallLogoUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.smallLogoUserControlLayoutControlItem.TextVisible = false;
        this.refreshSimpleButtonLayoutControlItem.Control = this.refreshSimpleButton;
        this.refreshSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(385, 391);
        this.refreshSimpleButtonLayoutControlItem.Name = "refreshSimpleButtonLayoutControlItem";
        this.refreshSimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
        this.refreshSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(80, 29);
        this.refreshSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.refreshSimpleButtonLayoutControlItem.TextVisible = false;
        this.panelControlLayoutControlItem.Control = this.panelControl;
        this.panelControlLayoutControlItem.Location = new System.Drawing.Point(0, 122);
        this.panelControlLayoutControlItem.Name = "panelControlLayoutControlItem";
        this.panelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
        this.panelControlLayoutControlItem.Size = new System.Drawing.Size(650, 233);
        this.panelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.panelControlLayoutControlItem.TextVisible = false;
        this.separatorEmptySpaceItem.AllowHotTrack = false;
        this.separatorEmptySpaceItem.Location = new System.Drawing.Point(0, 96);
        this.separatorEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 26);
        this.separatorEmptySpaceItem.MinSize = new System.Drawing.Size(104, 26);
        this.separatorEmptySpaceItem.Name = "separatorEmptySpaceItem";
        this.separatorEmptySpaceItem.Size = new System.Drawing.Size(650, 26);
        this.separatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.separatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
        this.layoutControlItem1.Control = this.logIntoAccountLabelControl;
        this.layoutControlItem1.Location = new System.Drawing.Point(0, 365);
        this.layoutControlItem1.Name = "layoutControlItem1";
        this.layoutControlItem1.Size = new System.Drawing.Size(128, 17);
        this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
        this.layoutControlItem1.TextVisible = false;
        this.emptySpaceItem2.AllowHotTrack = false;
        this.emptySpaceItem2.Location = new System.Drawing.Point(0, 382);
        this.emptySpaceItem2.Name = "emptySpaceItem2";
        this.emptySpaceItem2.Size = new System.Drawing.Size(229, 10);
        this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
        this.emptySpaceItem1.AllowHotTrack = false;
        this.emptySpaceItem1.Location = new System.Drawing.Point(0, 355);
        this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 10);
        this.emptySpaceItem1.MinSize = new System.Drawing.Size(10, 10);
        this.emptySpaceItem1.Name = "emptySpaceItem1";
        this.emptySpaceItem1.Size = new System.Drawing.Size(650, 10);
        this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
        this.convertLegacyKeyLabelControlLayoutControlItem.Control = this.convertLegacyKeyLabelControl;
        this.convertLegacyKeyLabelControlLayoutControlItem.Location = new System.Drawing.Point(128, 365);
        this.convertLegacyKeyLabelControlLayoutControlItem.Name = "convertLegacyKeyLabelControlLayoutControlItem";
        this.convertLegacyKeyLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(6, 2, 2, 2);
        this.convertLegacyKeyLabelControlLayoutControlItem.Size = new System.Drawing.Size(101, 17);
        this.convertLegacyKeyLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
        this.convertLegacyKeyLabelControlLayoutControlItem.TextVisible = false;
        this.layoutControlItem2.Control = this.infoUserControl;
        this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
        this.layoutControlItem2.MaxSize = new System.Drawing.Size(0, 36);
        this.layoutControlItem2.MinSize = new System.Drawing.Size(104, 36);
        this.layoutControlItem2.Name = "layoutControlItem2";
        this.layoutControlItem2.Size = new System.Drawing.Size(650, 36);
        this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
        this.layoutControlItem2.TextVisible = false;
        base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        base.Controls.Add(this.mainLayoutControl);
        base.Name = "LicensesPageUserControl";
        base.Load += new System.EventHandler(LicensesPageUserControl_Load);
        ((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
        this.mainLayoutControl.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.panelControl).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.backSimpleButtonLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.nextSimpleButtonLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.refreshSimpleButtonLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.panelControlLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.convertLegacyKeyLabelControlLayoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
        base.ResumeLayout(false);
    }
}
