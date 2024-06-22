using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.LoginFormTools.Tools;
using Dataedo.App.LoginFormTools.Tools.CallingApi;
using Dataedo.App.LoginFormTools.Tools.Common;
using Dataedo.App.LoginFormTools.Tools.CustomEventArgs;
using Dataedo.App.LoginFormTools.Tools.Diagnostics;
using Dataedo.App.LoginFormTools.Tools.Licenses;
using Dataedo.App.LoginFormTools.Tools.Recent;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using Dataedo.LicenseHelperLibrary.Repository;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSplashScreen;
using RecentProjectsLibrary;

namespace Dataedo.App.LoginFormTools;

public class LoginFormNew : BaseRibbonForm
{
	private bool isInitialStart;

	private bool suppressNavigationOnShown;

	private readonly NavigationManager navigationManager;

	private readonly PagesManager pagesManager;

	private readonly SuccessfulLoginSupport successfulLoginSupport;

	private OverlayWindowOptions overlayWindowOptions;

	private IOverlaySplashScreenHandle overlaySplashScreenHandle;

	private readonly StartForm startForm;

	private IContainer components;

	private RibbonControl mainRibbonControl;

	private BarButtonItem userBarButtonItem;

	private ToolTipController toolTipController;

	private PanelControl mainPanelControl;

	private PopupControlContainer userPopupControlContainer;

	private UserPanelUserControl userPanelUserControl;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private BarAndDockingController barAndDockingController;

	public static string FileName { get; private set; }

	public LoginFormNew(StartForm startForm)
	{
		isInitialStart = true;
		this.startForm = startForm;
		base.ShowInTaskbar = true;
		SkinsManager.SetSkin();
		EnvironmentInformation.WriteEnvironmentInformation();
		InitializeComponent();
		pagesManager = new PagesManager();
		pagesManager.TimeConsumingOperationStarted += PagesManager_TimeConsumingOperationStarted;
		pagesManager.TimeConsumingOperationStopped += PagesManager_TimeConsumingOperationStopped;
		navigationManager = new NavigationManager(mainPanelControl, pagesManager);
		navigationManager.Connected += NavigationManager_Connected;
		navigationManager.OnRequiresAction += NavigationManager_OnRequiresAction;
		successfulLoginSupport = new SuccessfulLoginSupport();
		Text = ProgramVersion.DisplayNameWithFullVersion;
		userPanelUserControl.OnChangeLicense += UserPanelUserControl_OnChangeLicense;
		userPanelUserControl.OnLicenseDetails += UserPanelUserControl_OnLicenseDetails;
		userPanelUserControl.OnSignOut += UserPanelUserControl_OnSignOut;
		userPanelUserControl.OnSignWithDifferentAccount += UserPanelUserControl_OnSignWithDifferentAccount;
	}

	private void UserPanelUserControl_OnSignWithDifferentAccount(object sender, EventArgs e)
	{
		userPopupControlContainer.HidePopup();
		SignInWithDifferentAccount(processRepositoryConnecting: true);
	}

	private void UserPanelUserControl_OnSignOut(object sender, EventArgs e)
	{
		userPopupControlContainer.HidePopup();
		SignOutHelper.SignOut();
		SignInAfterSignOut();
	}

	private void UserPanelUserControl_OnLicenseDetails(object sender, EventArgs e)
	{
		userPopupControlContainer.HidePopup();
		LicenseDetailsForm.ShowInParentForm(this);
	}

	private void UserPanelUserControl_OnChangeLicense(object sender, EventArgs e)
	{
		userPopupControlContainer.HidePopup();
		ChangeLicense(true, processRepositoryConnecting: true);
	}

	public LoginFormNew(StartForm startForm, string fileName)
		: this(startForm)
	{
		FileName = fileName;
	}

	public async void SetStartPageAfterSignIn()
	{
		await navigationManager.SetStartPageAfterSignIn();
	}

	public async void SignInWithDifferentAccount(bool processRepositoryConnecting = false)
	{
		suppressNavigationOnShown = true;
		await navigationManager.SignInWithDifferentAccount(processRepositoryConnecting);
	}

	public async void SignInAfterSignOut()
	{
		suppressNavigationOnShown = true;
		await navigationManager.SignInAfterSignOut();
	}

	public async void ChangeLicense(bool? suppressNextAction = null, bool processRepositoryConnecting = false)
	{
		suppressNavigationOnShown = true;
		await navigationManager.ChangeLicense(suppressNextAction, processRepositoryConnecting);
	}

	public async Task RefreshCurrentLicenseAsync()
	{
		suppressNavigationOnShown = false;
		await navigationManager.ChangeLicense(false);
	}

	public async void OpenRecentPage(bool forceClean = false)
	{
		await navigationManager.OpenRecentPage(forceClean);
	}

	public async void OpenCreateRepositoryPage()
	{
		suppressNavigationOnShown = true;
		await navigationManager.OpenCreateRepositoryPage();
	}

	public async void OpenConnectToRepositoryPage()
	{
		suppressNavigationOnShown = true;
		await navigationManager.OpenConnectToRepositoryPage();
	}

	public bool OpenRecent(RecentProject project)
	{
		return navigationManager.ProcessRecent(new RecentItemModel(project));
	}

	public void SetWindowLocation(Point point)
	{
		base.Location = point;
		ScreenHelper.Center(this);
	}

	protected override void Dispose(bool disposing)
	{
		overlaySplashScreenHandle?.Dispose();
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Escape && startForm?.MainForm != null)
		{
			Close();
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void LoginFormNew_Load(object sender, EventArgs e)
	{
		overlayWindowOptions = LoaderTools.GetOverlayWindowOptions(this);
	}

	private async void LoginFormNew_Shown(object sender, EventArgs e)
	{
		try
		{
			navigationManager.CurrentPageControl?.CheckLoaderVisibility();
		}
		catch (Exception)
		{
		}
		base.TopMost = false;
		bool startPage = isInitialStart;
		isInitialStart = false;
		if (!suppressNavigationOnShown)
		{
			await navigationManager.SetStartPage(startPage);
		}
		suppressNavigationOnShown = false;
	}

	private async void NavigationManager_Connected(object sender, ActionEventArgs e)
	{
		bool loginOpenedFromMainForm = startForm?.MainForm != null;
		try
		{
			successfulLoginSupport.CallUrl();
			if (startForm?.MainForm != null)
			{
				startForm.MainForm.TopMost = true;
			}
			Hide();
			await navigationManager.SetStartPageAfterSignIn();
			navigationManager.HideLoaders();
			HideLoader();
			if (loginOpenedFromMainForm)
			{
				startForm.MainForm.TopMost = false;
				startForm.MainForm.SetWaitFormVisibility(visible: true);
			}
			else
			{
				StaticData.ShowSplashScreen();
				startForm.CreateMainForm();
			}
			startForm.MainForm.SetFunctionality();
			startForm.MainForm.SetUserObjectsButtonsEnabled();
			startForm.MainForm.SetSuggestedDescriptionsByLicense();
			DB.Session.InsertSessionLog(isLoginSuccess: true, this);
			InsertUpdateLicenseRecord();
			StaticData.RefreshLicenseId();
			if (loginOpenedFromMainForm)
			{
				startForm.MainForm.RefreshDataSource();
				startForm.MainForm.LoadUpdateBars(hideWaitForm: false);
				startForm.MainForm.SetRecentDropDownMenu();
				startForm.MainForm.SetWaitFormVisibility(visible: false);
				return;
			}
			if (StaticData.SignInAfterAfterSignOutInProgress)
			{
				StaticData.SignInAfterAfterSignOutInProgress = false;
			}
			StaticData.CloseSplashScreen();
			startForm.ShowMainForm();
		}
		catch (Exception exception)
		{
			startForm.MainForm?.SetWaitFormVisibility(visible: false);
			StaticData.CloseSplashScreen();
			StaticData.ClearRepositoryData();
			GeneralExceptionHandling.Handle(exception, this);
			if (startForm?.MainForm != null)
			{
				startForm.MainForm.CloseAfterError();
			}
			else
			{
				Show();
			}
		}
	}

	private void InsertUpdateLicenseRecord()
	{
		try
		{
			if (!StaticData.IsProjectFile)
			{
				if (!StaticData.License.IsFileLicense && !string.IsNullOrEmpty(StaticData.Profile?.Email))
				{
					LicenseHelper.InsertUpdateLicenseRecord(StaticData.DataedoConnectionString, StaticData.Profile.Email, LastConnectionInfo.LOGIN_INFO.DataedoRealLogin, isOffline: false);
				}
				else
				{
					LicenseHelper.InsertUpdateLicenseRecord(StaticData.DataedoConnectionString, LastConnectionInfo.LOGIN_INFO.DataedoRealLogin, LastConnectionInfo.LOGIN_INFO.DataedoRealLogin, isOffline: true);
				}
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, this);
		}
	}

	private void NavigationManager_OnRequiresAction(object sender, EventArgs e)
	{
		Show();
	}

	private void PagesManager_TimeConsumingOperationStarted(object sender, EventArgs e)
	{
		ShowLoader();
	}

	private void PagesManager_TimeConsumingOperationStopped(object sender, EventArgs e)
	{
		HideLoader();
	}

	private void LoginFormNew_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (!navigationManager.CurrentPageControl.AllowClosing)
		{
			e.Cancel = true;
		}
		else if (startForm.MainForm != null)
		{
			startForm.MainForm.TopMost = true;
			navigationManager.HideLoaders();
			base.DialogResult = DialogResult.OK;
			Hide();
			e.Cancel = true;
		}
	}

	private void ShowLoader()
	{
		HideLoader();
		try
		{
			PanelControl panelControl = mainPanelControl;
			if (panelControl.Visible && panelControl.IsHandleCreated)
			{
				overlaySplashScreenHandle = SplashScreenManager.ShowOverlayForm(panelControl, overlayWindowOptions);
			}
		}
		catch
		{
		}
	}

	private void HideLoader()
	{
		if (overlaySplashScreenHandle != null)
		{
			SplashScreenManager.CloseOverlayForm(overlaySplashScreenHandle);
		}
	}

	public static void ClearFileName()
	{
		FileName = string.Empty;
	}

	public void SetUserInfo()
	{
		if (StaticData.License == null || startForm.MainForm != null)
		{
			userBarButtonItem.Visibility = BarItemVisibility.Never;
			return;
		}
		userPanelUserControl.SetParameters();
		userBarButtonItem.Caption = UserDataService.GetUsernameText();
		userBarButtonItem.Visibility = BarItemVisibility.Always;
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.LoginFormTools.LoginFormNew));
		this.mainRibbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
		this.userBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.userPopupControlContainer = new DevExpress.XtraBars.PopupControlContainer(this.components);
		this.userPanelUserControl = new Dataedo.App.UserControls.UserPanelUserControl();
		this.barAndDockingController = new DevExpress.XtraBars.BarAndDockingController(this.components);
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.mainPanelControl = new DevExpress.XtraEditors.PanelControl();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		((System.ComponentModel.ISupportInitialize)this.mainRibbonControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.userPopupControlContainer).BeginInit();
		this.userPopupControlContainer.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.barAndDockingController).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainPanelControl).BeginInit();
		this.mainPanelControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		base.SuspendLayout();
		this.mainRibbonControl.CaptionBarItemLinks.Add(this.userBarButtonItem);
		this.mainRibbonControl.Controller = this.barAndDockingController;
		this.mainRibbonControl.ExpandCollapseItem.Id = 0;
		this.mainRibbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[3]
		{
			this.mainRibbonControl.SearchEditItem,
			this.mainRibbonControl.ExpandCollapseItem,
			this.userBarButtonItem
		});
		this.mainRibbonControl.Location = new System.Drawing.Point(0, 0);
		this.mainRibbonControl.MaxItemId = 2;
		this.mainRibbonControl.Name = "mainRibbonControl";
		this.mainRibbonControl.RibbonCaptionAlignment = DevExpress.XtraBars.Ribbon.RibbonCaptionAlignment.Left;
		this.mainRibbonControl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2019;
		this.mainRibbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
		this.mainRibbonControl.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
		this.mainRibbonControl.ShowItemCaptionsInCaptionBar = true;
		this.mainRibbonControl.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Show;
		this.mainRibbonControl.ShowToolbarCustomizeItem = false;
		this.mainRibbonControl.Size = new System.Drawing.Size(700, 30);
		this.mainRibbonControl.Toolbar.ShowCustomizeItem = false;
		this.mainRibbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
		this.mainRibbonControl.ToolTipController = this.toolTipController;
		this.userBarButtonItem.ActAsDropDown = true;
		this.userBarButtonItem.AllowDrawArrow = false;
		this.userBarButtonItem.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
		this.userBarButtonItem.Caption = "User";
		this.userBarButtonItem.DropDownControl = this.userPopupControlContainer;
		this.userBarButtonItem.Id = 1;
		this.userBarButtonItem.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("userBarButtonItem.ImageOptions.SvgImage");
		this.userBarButtonItem.Name = "userBarButtonItem";
		this.userPopupControlContainer.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.userPopupControlContainer.Controls.Add(this.userPanelUserControl);
		this.userPopupControlContainer.Location = new System.Drawing.Point(153, 97);
		this.userPopupControlContainer.Name = "userPopupControlContainer";
		this.userPopupControlContainer.Ribbon = this.mainRibbonControl;
		this.userPopupControlContainer.Size = new System.Drawing.Size(395, 251);
		this.userPopupControlContainer.TabIndex = 4;
		this.userPopupControlContainer.Visible = false;
		this.userPanelUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.userPanelUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.userPanelUserControl.Location = new System.Drawing.Point(0, 0);
		this.userPanelUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.userPanelUserControl.Name = "userPanelUserControl";
		this.userPanelUserControl.Size = new System.Drawing.Size(395, 251);
		this.userPanelUserControl.TabIndex = 0;
		this.barAndDockingController.PropertiesBar.AllowLinkLighting = false;
		this.mainPanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.mainPanelControl.Controls.Add(this.userPopupControlContainer);
		this.mainPanelControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainPanelControl.Location = new System.Drawing.Point(0, 30);
		this.mainPanelControl.Name = "mainPanelControl";
		this.mainPanelControl.Size = new System.Drawing.Size(700, 470);
		this.mainPanelControl.TabIndex = 11;
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barManager.ToolTipController = this.toolTipController;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(700, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 500);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(700, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 500);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(700, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 500);
		base.AllowFormGlass = DevExpress.Utils.DefaultBoolean.False;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(700, 500);
		base.Controls.Add(this.mainPanelControl);
		base.Controls.Add(this.mainRibbonControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_32;
		base.MaximizeBox = false;
		base.Name = "LoginFormNew";
		this.Ribbon = this.mainRibbonControl;
		base.RibbonVisibility = DevExpress.XtraBars.Ribbon.RibbonVisibility.Hidden;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Dataedo";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(LoginFormNew_FormClosing);
		base.Load += new System.EventHandler(LoginFormNew_Load);
		base.Shown += new System.EventHandler(LoginFormNew_Shown);
		((System.ComponentModel.ISupportInitialize)this.mainRibbonControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.userPopupControlContainer).EndInit();
		this.userPopupControlContainer.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.barAndDockingController).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainPanelControl).EndInit();
		this.mainPanelControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
