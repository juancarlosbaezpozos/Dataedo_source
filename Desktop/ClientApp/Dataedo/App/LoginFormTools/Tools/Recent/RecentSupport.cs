using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Enums;
using Dataedo.App.LoginFormTools.UserControls.Subcontrols.Interfaces;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.UI;
using Dataedo.Shared.Enums;
using RecentProjectsLibrary;

namespace Dataedo.App.LoginFormTools.Tools.Recent;

internal class RecentSupport
{
	private readonly Form parentWindow;

	public RecentSupport(Form parentWindow)
	{
		this.parentWindow = parentWindow;
	}

	public static RecentProject GetRecentProject(IRecentProjectData recentProjectData)
	{
		return new RecentProject(recentProjectData.Database, recentProjectData.ServerName, recentProjectData.Login, AuthenticationType.TypeToString(recentProjectData.AuthenticationType), SqlServerConnectionModeEnum.TypeToString(recentProjectData.SqlServerConnectionMode), recentProjectData.Port, new SimpleAES().EncryptToString(recentProjectData.Password), recentProjectData.SavePassword);
	}

	public static RecentItemModel GetRecentItemModel(IRecentProjectData recentProjectData)
	{
		return new RecentItemModel(GetRecentProject(recentProjectData));
	}

	public List<RecentItemModel> GetRecentProjects(bool allowLoadingFromPreviousVersion, bool allowLoadingFromPreviousVersionWithoutAsking)
	{
		try
		{
			if (allowLoadingFromPreviousVersion && (!File.Exists(RecentProjectsHelper.GetRecentFilePath(ProgramVersion.Major)) || RecentProjectsHelper.GetList(ProgramVersion.Major, null).Count == 0) && RecentProjectsHelper.PreviousRecentFileExists() && (allowLoadingFromPreviousVersionWithoutAsking || GeneralMessageBoxesHandling.Show("Would you like to load recent repositories from previous version?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, null, 1, parentWindow).DialogResult == DialogResult.Yes))
			{
				RecentProjectsHelper.GetPreviousVersionList(SkinsManager.CurrentSkin.ListSelectionForeColor);
				RecentProjectsHelper.CreateRecentFromPreviousVersion();
			}
			return (from x in RecentProjectsHelper.GetList(ProgramVersion.Major, SkinsManager.CurrentSkin.ListSelectionForeColor)
				select new RecentItemModel(x)).ToList();
		}
		catch (Exception exception)
		{
			if (!allowLoadingFromPreviousVersionWithoutAsking)
			{
				GeneralExceptionHandling.Handle(exception, "Unable to load recent projects", parentWindow);
			}
		}
		return new List<RecentItemModel>();
	}
}
