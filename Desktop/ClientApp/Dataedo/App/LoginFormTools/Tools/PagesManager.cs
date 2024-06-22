using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.LoginFormTools.Tools.Enums;
using Dataedo.App.LoginFormTools.UserControls;
using Dataedo.App.LoginFormTools.UserControls.Base;

namespace Dataedo.App.LoginFormTools.Tools;

internal class PagesManager
{
	private readonly Dictionary<LoginFormPageEnum.LoginFormPage, BasePageUserControl> pages;

	public event EventHandler TimeConsumingOperationStarted;

	public event EventHandler TimeConsumingOperationStopped;

	public PagesManager()
	{
		pages = new Dictionary<LoginFormPageEnum.LoginFormPage, BasePageUserControl>();
	}

	public BasePageUserControl GetPage(LoginFormPageEnum.LoginFormPage pageKey)
	{
		if (pages.ContainsKey(pageKey))
		{
			return pages[pageKey];
		}
		BasePageUserControl basePageUserControl = CreatePage(pageKey);
		pages.Add(pageKey, basePageUserControl);
		return basePageUserControl;
	}

	public void OnTimeConsumingOperationStarted(object sender)
	{
		this.TimeConsumingOperationStarted?.Invoke(sender, EventArgs.Empty);
	}

	public void OnTimeConsumingOperationStarted(object sender, EventArgs e)
	{
		this.TimeConsumingOperationStarted?.Invoke(sender, e);
	}

	public void OnTimeConsumingOperationStopped(object sender)
	{
		this.TimeConsumingOperationStopped?.Invoke(sender, EventArgs.Empty);
	}

	public void OnTimeConsumingOperationStopped(object sender, EventArgs e)
	{
		this.TimeConsumingOperationStopped?.Invoke(sender, e);
	}

	public IEnumerable<BasePageUserControl> GetCreatedPages()
	{
		return pages.Select((KeyValuePair<LoginFormPageEnum.LoginFormPage, BasePageUserControl> x) => x.Value);
	}

	private BasePageUserControl CreatePage(LoginFormPageEnum.LoginFormPage pageKey)
	{
		return PreparePage(pageKey switch
		{
			LoginFormPageEnum.LoginFormPage.SignIn => new LoginPageUserControl(), 
			LoginFormPageEnum.LoginFormPage.AccessCode => new AccessCodePageUserControl(), 
			LoginFormPageEnum.LoginFormPage.Licenses => new LicensesPageUserControl(), 
			LoginFormPageEnum.LoginFormPage.FileLicenses => new FileLicensesPageUserControl(), 
			LoginFormPageEnum.LoginFormPage.GettingStarted => new GettingStartedPageUserControl(), 
			LoginFormPageEnum.LoginFormPage.ConnectToRepository => new ConnectToRepositoryPageUserControl(), 
			LoginFormPageEnum.LoginFormPage.CreateNewRepository => new CreateNewRepositoryPageUserControl(), 
			LoginFormPageEnum.LoginFormPage.Recent => new RecentPageUserControl(), 
			LoginFormPageEnum.LoginFormPage.ConnectToServerRepository => new ConnectToServerRepositoryPageUserControl(), 
			LoginFormPageEnum.LoginFormPage.CreateNewServerRepository => new CreateNewServerRepositoryPageUserControl(), 
			LoginFormPageEnum.LoginFormPage.UpgradeServerRepository => new UpgradeServerRepositoryPageUserControl(), 
			LoginFormPageEnum.LoginFormPage.UpgradeServerRepositoryConnectedUsers => new UpgradeServerRepositoryConnectedUsersPageUserControl(), 
			LoginFormPageEnum.LoginFormPage.UpgradeRepositoryProgress => new UpgradeRepositoryProgressPageUserControl(), 
			LoginFormPageEnum.LoginFormPage.CreateRepositoryProgress => new CreateRepositoryProgressPageUserControl(), 
			LoginFormPageEnum.LoginFormPage.OperationCompletedSuccessfully => new OperationCompletedSuccessfullyPageUserControl(), 
			LoginFormPageEnum.LoginFormPage.OperationCompletedNotSuccessfully => new OperationCompletedNotSuccessfullyPageUserControl(), 
			_ => throw new ArgumentException($"Provided page ({pageKey}) is not supported."), 
		});
	}

	private BasePageUserControl PreparePage(BasePageUserControl pageControl)
	{
		pageControl.Dock = DockStyle.Fill;
		pageControl.Location = new Point(0, 0);
		pageControl.MaximumSize = new Size(700, 470);
		pageControl.MinimumSize = new Size(700, 470);
		pageControl.Size = new Size(700, 470);
		pageControl.TabIndex = 0;
		pageControl.TimeConsumingOperationStarted += PageControl_TimeConsumingOperationStarted;
		pageControl.TimeConsumingOperationStopped += PageControl_TimeConsumingOperationStopped;
		return pageControl;
	}

	private void PageControl_TimeConsumingOperationStarted(object sender, EventArgs e)
	{
		this.TimeConsumingOperationStarted?.Invoke(sender, e);
	}

	private void PageControl_TimeConsumingOperationStopped(object sender, EventArgs e)
	{
		this.TimeConsumingOperationStopped?.Invoke(sender, e);
	}
}
