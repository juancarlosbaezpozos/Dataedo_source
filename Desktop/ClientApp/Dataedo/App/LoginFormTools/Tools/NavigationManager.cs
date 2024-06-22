using System;
using System.Threading.Tasks;
using Dataedo.App.Forms.Tools;
using Dataedo.App.LoginFormTools.Tools.Common;
using Dataedo.App.LoginFormTools.Tools.CustomEventArgs;
using Dataedo.App.LoginFormTools.Tools.Enums;
using Dataedo.App.LoginFormTools.Tools.FileRepository;
using Dataedo.App.LoginFormTools.Tools.Licenses;
using Dataedo.App.LoginFormTools.Tools.Recent;
using Dataedo.App.LoginFormTools.UserControls;
using Dataedo.App.LoginFormTools.UserControls.Base;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.App.Tools.UI;
using Dataedo.Data.Commands.Enums;
using DevExpress.XtraEditors;
using RecentProjectsLibrary;

namespace Dataedo.App.LoginFormTools.Tools;

internal class NavigationManager
{
    private readonly PanelControl pagesPanel;

    private readonly PagesManager pagesManager;

    private LoginFormPageEnum.LoginFormPage currentPage;

    private LoginFormPageEnum.LoginFormPage lastStartPage;

    private LoginFormPageEnum.LoginFormPage lastStartPageAfterSignIn;

    private bool processRepositoryConnecting;

    public BasePageUserControl CurrentPageControl => pagesManager.GetPage(currentPage);

    public event BasePageUserControl.ActionEventHandler Connected;

    public event EventHandler OnRequiresAction;

    public NavigationManager(PanelControl pagesPanel, PagesManager pagesManager)
    {
        this.pagesPanel = pagesPanel ?? throw new ArgumentNullException("pagesPanel");
        this.pagesManager = pagesManager ?? throw new ArgumentNullException("pagesManager");
    }

    public async Task SetStartPage()
    {
        await ChangePage(GetStartPage());
    }

    public async Task SetStartPage(bool isInitialStart)
    {
        processRepositoryConnecting = isInitialStart;
        if (isInitialStart)
        {
            LoginFormPageEnum.LoginFormPage startPage = GetStartPage();
            await ChangePage((LoginFormPageEnum.LoginFormPage?)startPage, (object)isInitialStart);
        }
        else
        {
            await SetStartPageAfterSignIn();
        }
    }

    public async Task SetStartPage(string fileName)
    {
        processRepositoryConnecting = true;
        BasePageUserControl obj = await ChangePage(LoginFormPageEnum.LoginFormPage.ConnectToRepository);
        obj.CalledFrom = GetStartPage();
        if (!(obj is ConnectToRepositoryPageUserControl connectToRepositoryPageUserControl))
        {
            await SetStartPage();
        }
        else
        {
            connectToRepositoryPageUserControl.OpenFileRepository(fileName);
        }
    }

    public async Task SetStartPageAfterSignIn()
    {
        processRepositoryConnecting = false;
        await ChangePage(GetStartPageAfterSignIn());
    }

    public async Task SignInWithDifferentAccount(bool processRepositoryConnecting = false)
    {
        this.processRepositoryConnecting = processRepositoryConnecting;
        await ChangePage(GetStartPage(), null, isCalledAsPrevious: false, true);
    }

    public async Task SignInAfterSignOut()
    {
        processRepositoryConnecting = true;
        await ChangePage(GetStartPage(), null, isCalledAsPrevious: false, false);
    }

    public async Task ChangeLicense(bool? suppressNextAction = null, bool processRepositoryConnecting = false)
    {
        this.processRepositoryConnecting = processRepositoryConnecting;
        pagesManager.GetPage(LoginFormPageEnum.LoginFormPage.Licenses).CalledFrom = LoginFormPageEnum.LoginFormPage.MainForm;
        switch (StaticData.LicenseEnum)
        {
            case LicenseEnum.DataedoAccount:
                await ChangePage(LoginFormPageEnum.LoginFormPage.Licenses, new LoginDataModel(StaticData.Token, StaticData.Profile.Email), isCalledAsPrevious: false, suppressNextAction ?? true);
                break;
            case LicenseEnum.LocalFile:
                {
                    FileData fileData = new FileData();
                    fileData.Map(LicenseFileDataHelper.GetLicenseFileData().LicenseFile, LicenseFileDataHelper.GetLicenseFileData().LastSelectedLicense);
                    await ChangePage(LoginFormPageEnum.LoginFormPage.FileLicenses, new LicenseFileDataModel(string.Empty, fileData), isCalledAsPrevious: false, suppressNextAction ?? true);
                    break;
                }
        }
    }

    public async Task OpenRecentPage(bool forceClean = false)
    {
        pagesManager.GetPage(LoginFormPageEnum.LoginFormPage.Recent).CalledFrom = LoginFormPageEnum.LoginFormPage.MainForm;
        if (!forceClean)
        {
            await ChangePage(LoginFormPageEnum.LoginFormPage.Recent);
        }
        else
        {
            await ChangePageForceClear(LoginFormPageEnum.LoginFormPage.Recent);
        }
    }

    public async Task OpenCreateRepositoryPage()
    {
        pagesManager.GetPage(LoginFormPageEnum.LoginFormPage.CreateNewRepository).CalledFrom = LoginFormPageEnum.LoginFormPage.MainForm;
        await ChangePageForceClear(LoginFormPageEnum.LoginFormPage.CreateNewRepository, allowBackButton: false);
    }

    public async Task OpenConnectToRepositoryPage()
    {
        pagesManager.GetPage(LoginFormPageEnum.LoginFormPage.ConnectToRepository).CalledFrom = LoginFormPageEnum.LoginFormPage.MainForm;
        await ChangePageForceClear(LoginFormPageEnum.LoginFormPage.ConnectToRepository, allowBackButton: false);
    }

    public bool ProcessRecent(RecentItemModel recentProject)
    {
        return (pagesManager.GetPage(LoginFormPageEnum.LoginFormPage.Recent) as RecentPageUserControl).ProcessRecent(recentProject);
    }

    public async Task<BasePageUserControl> ChangePage(LoginFormPageEnum.LoginFormPage? pageKey, object parameter, bool isCalledAsPrevious, bool? suppressNextAction, bool forceClean = false)
    {
        if (!pageKey.HasValue)
        {
            return null;
        }
        LoginFormPageEnum.LoginFormPage pageKeyValue = pageKey.Value;
        BasePageUserControl pageControl = pagesManager.GetPage(pageKeyValue);
        pageControl.CalledFrom = ((!pageControl.CalledFrom.HasValue) ? new LoginFormPageEnum.LoginFormPage?(currentPage) : pageControl.CalledFrom);
        pageControl.BackButtonVisibility = pageControl.CalledFrom != LoginFormPageEnum.LoginFormPage.MainForm;
        if (pagesManager.GetPage(LoginFormPageEnum.LoginFormPage.ConnectToRepository).CalledFrom == LoginFormPageEnum.LoginFormPage.MainForm)
        {
            pagesManager.GetPage(LoginFormPageEnum.LoginFormPage.ConnectToServerRepository).CalledFrom = LoginFormPageEnum.LoginFormPage.ConnectToRepository;
        }
        else if (parameter is RecentItemModel)
        {
            pageControl.CalledFrom = LoginFormPageEnum.LoginFormPage.Recent;
        }
        pageControl.IsCalledAsPrevious = isCalledAsPrevious;
        pageControl.ForceClean = forceClean;
        try
        {
            DrawingControl.SuspendDrawing(pagesPanel);
            await ApplyEvents(pageKeyValue);
            pagesPanel.Controls.Clear();
            pagesPanel.Controls.Add(pageControl);
            currentPage = pageKeyValue;
            pageControl.SetParameter(parameter, isCalledAsPrevious);
            if (suppressNextAction.HasValue)
            {
                pageControl.SuppressNextAction = suppressNextAction.Value;
            }
            await TryNavigate(pageControl);
        }
        finally
        {
            DrawingControl.ResumeDrawing(pagesPanel);
            pageControl?.Focus();
        }
        return pageControl;
    }

    public async Task<BasePageUserControl> ChangePage(LoginFormPageEnum.LoginFormPage? pageKey, object parameter)
    {
        return await ChangePage(pageKey, parameter, isCalledAsPrevious: false, null);
    }

    public async Task<BasePageUserControl> ChangePage(LoginFormPageEnum.LoginFormPage? pageKey, bool isCalledAsPrevious)
    {
        return await ChangePage(pageKey, null, isCalledAsPrevious, null);
    }

    public async Task<BasePageUserControl> ChangePage(LoginFormPageEnum.LoginFormPage? pageKey)
    {
        return await ChangePage(pageKey, null, isCalledAsPrevious: false, null);
    }

    public async Task<BasePageUserControl> ChangePageForceClear(LoginFormPageEnum.LoginFormPage? pageKey, bool allowBackButton = true)
    {
        return await ChangePage(pageKey, null, isCalledAsPrevious: false, null, forceClean: true);
    }

    public void HideLoaders()
    {
        pagesManager.OnTimeConsumingOperationStopped(this);
        foreach (BasePageUserControl createdPage in pagesManager.GetCreatedPages())
        {
            createdPage.HideLoader();
        }
    }

    private static async Task<bool> TryNavigate(BasePageUserControl pageControl)
    {
        bool result = default(bool);
        int num;
        try
        {
            await pageControl.Navigated();
            result = await Task.Run(() => true);
            return result;
        }
        catch
        {
            num = 1;
        }
        if (num != 1)
        {
            return result;
        }
        return await Task.Run(() => false);
    }

    private LoginFormPageEnum.LoginFormPage GetStartPage()
    {
        lastStartPage = LoginFormPageEnum.LoginFormPage.SignIn;
        return lastStartPage;
    }

    private LoginFormPageEnum.LoginFormPage GetStartPageAfterSignIn()
    {
        if (RecentProjectsHelper.HasRecentProjects(ProgramVersion.Major))
        {
            lastStartPageAfterSignIn = LoginFormPageEnum.LoginFormPage.Recent;
        }
        else
        {
            lastStartPageAfterSignIn = LoginFormPageEnum.LoginFormPage.GettingStarted;
        }
        return lastStartPageAfterSignIn;
    }

    private async Task ApplyEvents(LoginFormPageEnum.LoginFormPage pageKey)
    {
        BasePageUserControl pageControl = pagesManager.GetPage(pageKey);
        if (pageControl?.HasEventsApplied ?? true)
        {
            return;
        }
        pageControl.RequiresAction += this.OnRequiresAction;
        switch (pageKey)
        {
            case LoginFormPageEnum.LoginFormPage.SignIn:
                pageControl.Action += async delegate (object s, ActionEventArgs e)
                {
                    if (e.ActionResult == ActionResultEnum.ActionResult.AccessCode)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.AccessCode, e.Parameter);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.Licenses)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.Licenses, e.Parameter);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.FileLicenses)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.FileLicenses, e.Parameter);
                    }
                };
                break;
            case LoginFormPageEnum.LoginFormPage.AccessCode:
                pageControl.Action += async delegate (object s, ActionEventArgs e)
                {
                    if (e.ActionResult == ActionResultEnum.ActionResult.Licenses)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.Licenses, e.Parameter);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.Back)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.SignIn, e.Parameter, isCalledAsPrevious: false, true);
                    }
                };
                break;
            case LoginFormPageEnum.LoginFormPage.Licenses:
            case LoginFormPageEnum.LoginFormPage.FileLicenses:
                pageControl.Action += async delegate (object s, ActionEventArgs e)
                {
                    if (e.ActionResult == ActionResultEnum.ActionResult.Next)
                    {
                        if (processRepositoryConnecting)
                        {
                            if (!string.IsNullOrWhiteSpace(LoginFormNew.FileName))
                            {
                                string fileName = LoginFormNew.FileName;
                                LoginFormNew.ClearFileName();
                                SetStartPage(fileName);
                            }
                            LoginFormPageEnum.LoginFormPage startPageAfterSignIn = GetStartPageAfterSignIn();
                            if (startPageAfterSignIn == LoginFormPageEnum.LoginFormPage.Recent)
                            {
                                await ChangePage((LoginFormPageEnum.LoginFormPage?)startPageAfterSignIn, (object)true);
                            }
                            else
                            {
                                await ChangePage(startPageAfterSignIn);
                            }
                        }
                        else
                        {
                            OnConnected(pageControl, new ActionEventArgs(ActionResultEnum.ActionResult.SignedIn), TrackingEventEnum.ServerRepoConnecting);
                        }
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.Back)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.SignIn, e.Parameter, isCalledAsPrevious: false, true);
                    }
                };
                break;
            case LoginFormPageEnum.LoginFormPage.GettingStarted:
                pageControl.Action += async delegate (object s, ActionEventArgs e)
                {
                    if (e.ActionResult == ActionResultEnum.ActionResult.CreateRepository)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.CreateNewRepository);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.ConnectToRepository)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.ConnectToRepository);
                    }
                };
                break;
            case LoginFormPageEnum.LoginFormPage.ConnectToRepository:
                pageControl.Action += async delegate (object s, ActionEventArgs e)
                {
                    if (e.ActionResult == ActionResultEnum.ActionResult.Back)
                    {
                        await ChangePage(CurrentPageControl.CalledFrom);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.ConnectToServerRepository)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.ConnectToServerRepository, e.Parameter, isCalledAsPrevious: false, true);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.OpenFileRepository)
                    {
                        if (e.Parameter == null)
                        {
                            OpenFileRepository(s, null);
                        }
                        else
                        {
                            string text = e.Parameter as string;
                            if (!OpenFileRepositoryFromPath(s, text).IsSuccess)
                            {
                                (s as ConnectToRepositoryPageUserControl)?.OnInvalidPathProvided(text);
                            }
                        }
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.OpenFileRepositoryForInvalidFile)
                    {
                        string fileNameToSet = e.Parameter as string;
                        if (!OpenFileRepository(s, fileNameToSet).IsSuccess)
                        {
                            await ChangePage(lastStartPage);
                        }
                    }
                };
                break;
            case LoginFormPageEnum.LoginFormPage.CreateNewRepository:
                pageControl.Action += async delegate (object s, ActionEventArgs e)
                {
                    if (e.ActionResult == ActionResultEnum.ActionResult.Back)
                    {
                        await ChangePage(CurrentPageControl.CalledFrom);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.CreateServerRepository)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.CreateNewServerRepository);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.CreateFileRepository)
                    {
                        CreateFileRepository(s);
                    }
                };
                break;
            case LoginFormPageEnum.LoginFormPage.Recent:
                pageControl.Action += async delegate (object s, ActionEventArgs e)
                {
                    if (e.ActionResult == ActionResultEnum.ActionResult.CreateRepository)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.CreateNewRepository);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.ConnectToRepository)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.ConnectToRepository);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.ConnectToServerRepository)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.ConnectToServerRepository, e.Parameter);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.OpenFileRepository)
                    {
                        OpenFileRepositoryFromPath(s, (e.Parameter as RecentItemModel).Database);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.Connected)
                    {
                        OnConnected(pageControl, e, TrackingEventEnum.ServerRepoConnected);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.SignedOut)
                    {
                        await SetStartPage();
                    }
                };
                break;
            case LoginFormPageEnum.LoginFormPage.ConnectToServerRepository:
                pageControl.Action += async delegate (object s, ActionEventArgs e)
                {
                    if (e.ActionResult == ActionResultEnum.ActionResult.Back)
                    {
                        await ChangePage(CurrentPageControl.CalledFrom);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.Connected)
                    {
                        OnConnected(pageControl, e, TrackingEventEnum.ServerRepoConnected);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.UpgradeServerRepository)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.UpgradeServerRepository, e.Parameter);
                    }
                };
                break;
            case LoginFormPageEnum.LoginFormPage.CreateNewServerRepository:
                pageControl.Action += async delegate (object s, ActionEventArgs e)
                {
                    if (e.ActionResult == ActionResultEnum.ActionResult.Back)
                    {
                        await ChangePage(CurrentPageControl.CalledFrom);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.CreateRepositoryProgress)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.CreateRepositoryProgress, e.Parameter);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.CreateFileRepository)
                    {
                        CreateFileRepository(s);
                    }
                };
                break;
            case LoginFormPageEnum.LoginFormPage.CreateRepositoryProgress:
                pageControl.Action += async delegate (object s, ActionEventArgs e)
                {
                    if (e.ActionResult == ActionResultEnum.ActionResult.Back)
                    {
                        await ChangePage(CurrentPageControl.CalledFrom);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.OperationCompletedSuccessfully)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.OperationCompletedSuccessfully, e.Parameter);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.OperationCompletedNotSuccessfully)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.OperationCompletedNotSuccessfully, e.Parameter);
                    }
                };
                break;
            case LoginFormPageEnum.LoginFormPage.UpgradeServerRepository:
                pageControl.Action += async delegate (object s, ActionEventArgs e)
                {
                    if (e.ActionResult == ActionResultEnum.ActionResult.UpgradeServerRepositoryConnectedUsers)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.UpgradeServerRepositoryConnectedUsers, e.Parameter);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.Back)
                    {
                        await ChangePage(CurrentPageControl.CalledFrom, isCalledAsPrevious: true);
                    }
                };
                break;
            case LoginFormPageEnum.LoginFormPage.UpgradeServerRepositoryConnectedUsers:
                pageControl.Action += async delegate (object s, ActionEventArgs e)
                {
                    if (e.ActionResult == ActionResultEnum.ActionResult.UpgradeRepositoryProgress)
                    {
                        (await ChangePage(LoginFormPageEnum.LoginFormPage.UpgradeRepositoryProgress, e.Parameter)).CalledFrom = lastStartPage;
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.Back)
                    {
                        await ChangePage(CurrentPageControl.CalledFrom, isCalledAsPrevious: true);
                    }
                };
                break;
            case LoginFormPageEnum.LoginFormPage.UpgradeRepositoryProgress:
                pageControl.Action += async delegate (object s, ActionEventArgs e)
                {
                    if (e.ActionResult == ActionResultEnum.ActionResult.Back)
                    {
                        await ChangePage(CurrentPageControl.CalledFrom);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.OperationCompletedSuccessfully)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.OperationCompletedSuccessfully, e.Parameter);
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.OperationCompletedNotSuccessfully)
                    {
                        await ChangePage(LoginFormPageEnum.LoginFormPage.OperationCompletedNotSuccessfully, e.Parameter);
                    }
                };
                break;
            case LoginFormPageEnum.LoginFormPage.OperationCompletedSuccessfully:
                pageControl.Action += async delegate (object s, ActionEventArgs e)
                {
                    if (e.ActionResult == ActionResultEnum.ActionResult.ConnectToServerRepository)
                    {
                        (await ChangePage(LoginFormPageEnum.LoginFormPage.ConnectToServerRepository, e.Parameter, isCalledAsPrevious: false, true)).CalledFrom = lastStartPage;
                    }
                    else if (e.ActionResult == ActionResultEnum.ActionResult.Back)
                    {
                        await SetStartPage();
                    }
                };
                break;
            case LoginFormPageEnum.LoginFormPage.OperationCompletedNotSuccessfully:
                pageControl.Action += async delegate (object s, ActionEventArgs e)
                {
                    if (e.ActionResult == ActionResultEnum.ActionResult.Back)
                    {
                        await SetStartPage();
                    }
                };
                break;
            default:
                throw new ArgumentException($"Provided page ({pageKey}) is not supported.");
        }
        pageControl.HasEventsApplied = true;
    }

    private void CreateFileRepository(object sender)
    {
        try
        {
            pagesManager.OnTimeConsumingOperationStarted(sender);
            using SaveFileSupport saveFileSupport = new SaveFileSupport(pagesPanel.FindForm());
            if (saveFileSupport.SaveFile())
            {
                OnConnected(sender, ActionEventArgs.Empty, TrackingEventEnum.CreatorNewFileCreated);
            }
        }
        finally
        {
            pagesManager.OnTimeConsumingOperationStopped(sender);
        }
    }

    private ConnectionResult OpenFileRepositoryFromPath(object sender, string path)
    {
        try
        {
            pagesManager.OnTimeConsumingOperationStarted(sender);
            using OpenFileSupport openFileSupport = new OpenFileSupport(pagesPanel.FindForm());
            StaticData.SetRepositoryData(RepositoriesDBHelper.GetRepositoryData(path, isProjectFile: true, FileLoginSupport.GetConnectionString(path)));
            TrackingRunner.Track(delegate
            {
                TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingConnectToRepositoryRepoParameters(isFile: true), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.FileRepoConnecting);
            });
            ConnectionResult connectionResult = openFileSupport.OpenProject(path);
            if (connectionResult.IsSuccess)
            {
                OnConnected(sender, ActionEventArgs.Empty, TrackingEventEnum.FileRepoConnected);
            }
            else
            {
                TrackingRunner.Track(delegate
                {
                    TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingConnectToRepositoryRepoParameters(isFile: true), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.FileRepoConnectionFailed);
                });
            }
            return connectionResult;
        }
        finally
        {
            pagesManager.OnTimeConsumingOperationStopped(sender);
        }
    }

    private ConnectionResult OpenFileRepository(object sender, string fileNameToSet)
    {
        try
        {
            pagesManager.OnTimeConsumingOperationStarted(sender);
            using OpenFileSupport openFileSupport = new OpenFileSupport(pagesPanel.FindForm(), fileNameToSet);
            ConnectionResult connectionResult = openFileSupport.OpenFile();
            TrackingRunner.Track(delegate
            {
                TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingConnectToRepositoryRepoParameters(isFile: true), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.FileRepoConnecting);
            });
            if (connectionResult.IsSuccess)
            {
                OnConnected(sender, ActionEventArgs.Empty, TrackingEventEnum.FileRepoConnected);
            }
            else
            {
                TrackingRunner.Track(delegate
                {
                    TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingConnectToRepositoryRepoParameters(isFile: true), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.FileRepoConnectionFailed);
                });
            }
            return connectionResult;
        }
        finally
        {
            pagesManager.OnTimeConsumingOperationStopped(sender);
        }
    }

    private void OnConnected(object sender, ActionEventArgs e, TrackingEventEnum eventEnum)
    {
        StaticData.SetRepositoryData(RepositoriesDBHelper.GetRepositoryData(StaticData.Database));
        TrackingRunner.Track(delegate
        {
            TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingRepoParameters(), new TrackingDataedoParameters(), new TrackingUserParameters()), eventEnum);
        });
        this.Connected?.Invoke(sender, e);
    }
}
