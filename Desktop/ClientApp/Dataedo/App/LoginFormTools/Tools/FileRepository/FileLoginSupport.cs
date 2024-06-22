using System;
using System.Data.SqlServerCe;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.LoginFormTools.Tools.Common;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.Data.Commands;
using Dataedo.Data.Commands.Enums;
using Dataedo.Data.Factories;
using Dataedo.LicenseHelperLibrary.Repository;
using RecentProjectsLibrary;

namespace Dataedo.App.LoginFormTools.Tools.FileRepository;

internal class FileLoginSupport
{
	protected readonly Form ParentWindow;

	public FileLoginSupport(Form parentWindow)
	{
		ParentWindow = parentWindow;
	}

	public ConnectionResult TryLoginByPath(string fullPath)
	{
		return TryLogin(GetConnectionString(fullPath));
	}

	public ConnectionResult TryLogin(string connectionString)
	{
		bool? flag = null;
		bool? isConnectionDataValid = null;
		try
		{
			flag = CheckLicense();
			if (flag == false)
			{
				return new ConnectionResult(flag, isConnectionDataValid);
			}
			if (!TryLoginByConnectionString(connectionString))
			{
				return new ConnectionResult(flag, isConnectionDataValid);
			}
			isConnectionDataValid = true;
			try
			{
				SqlCeConnectionStringBuilder sqlCeConnectionStringBuilder = new SqlCeConnectionStringBuilder(StaticData.DataedoConnectionString);
				StaticData.Database = sqlCeConnectionStringBuilder.DataSource;
				LastConnectionInfo.SetFileProjectPath(sqlCeConnectionStringBuilder.DataSource);
				RecentProjectsHelper.Add(new RecentProject(sqlCeConnectionStringBuilder.DataSource));
			}
			catch
			{
			}
			return new ConnectionResult(flag, isConnectionDataValid);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, ParentWindow);
			return new ConnectionResult(flag, isConnectionDataValid);
		}
	}

	private static void SetOldConnectionString(DatabaseType? repositoryType, string connectionString)
	{
		if (repositoryType.HasValue && !string.IsNullOrEmpty(connectionString))
		{
			StaticData.Initialize(repositoryType.Value, connectionString);
			DB.ReloadClasses();
			StaticData.DataedoConnectionString = connectionString;
			StaticData.RepositoryType = repositoryType.Value;
			Dataedo.App.Tools.Licenses.Initialize(StaticData.Commands);
		}
	}

	public static string GetConnectionString(string path)
	{
		return new SqlCeConnectionStringBuilder
		{
			DataSource = path,
			MaxDatabaseSize = 4091
		}.ConnectionString;
	}

	private bool CheckLicense()
	{
		return true;
	}

	private bool TryLoginByConnectionString(string newConnectionString, Form owner = null)
	{
		CommandsFactory.GetCommands(DatabaseType.SqlServerCe, newConnectionString).Select.Repository.GetAllVersions();
		string dataedoLastValidConnectionString = StaticData.DataedoLastValidConnectionString;
		DatabaseType? lastValidRepositoryType = StaticData.LastValidRepositoryType;
		StaticData.Initialize(DatabaseType.SqlServerCe, newConnectionString);
		DB.ReloadClasses();
		StaticData.DataedoConnectionString = newConnectionString;
		StaticData.Host = string.Empty;
		StaticData.RepositoryType = DatabaseType.SqlServerCe;
		Dataedo.App.Tools.Licenses.Initialize(StaticData.Commands);
		if (!CheckRepositoryVersion(StaticData.Commands, owner))
		{
			SetOldConnectionString(lastValidRepositoryType, dataedoLastValidConnectionString);
			return false;
		}
		StaticData.DataedoLastValidConnectionString = newConnectionString;
		StaticData.LastValidRepositoryType = DatabaseType.SqlServerCe;
		return true;
	}

	private bool CheckRepositoryVersion(CommandsSetBase commands, Form owner = null)
	{
		if (!LoginHelper.GetRepositoryVersion(out var repoVersion, commands, owner))
		{
			return false;
		}
		repoVersion.GetMatchingRepositoryVersion(ProgramVersion.Major, ProgramVersion.Minor, ProgramVersion.Build, ProgramVersion.ReleaseType);
		RepositoryVersionEnum.RepositoryVersion versionMatch = StaticData.LicenseHelper.CompareRepositoryVersion(StaticData.DataedoConnectionString, ProgramVersion.Major, ProgramVersion.Minor, ProgramVersion.Build);
		if (!repoVersion.Stable)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("Repository is unstable and may cause errors. It is recommended to restore it from backup before starting work with Dataedo. Do you want to continue anyway?", "Error", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, ParentWindow);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
			{
				return false;
			}
		}
		if (versionMatch == RepositoryVersionEnum.RepositoryVersion.OLDER)
		{
			UpgradeForm upgradeForm = new UpgradeForm(repoVersion, isFile: true, null);
			try
			{
				bool upgradeResult = false;
				upgradeForm.UpgradeFileClicked += delegate
				{
					LoginHelper.UpgradeFileRepository(ref repoVersion, upgradeForm, ref upgradeResult);
				};
				upgradeForm.ShowDialog(ParentWindow);
				return upgradeResult;
			}
			finally
			{
				if (upgradeForm != null)
				{
					((IDisposable)upgradeForm).Dispose();
				}
			}
		}
		if (!StaticData.LicenseHelper.CheckRepositoryVersion(StaticData.DataedoConnectionString, ProgramVersion.Major, ProgramVersion.Minor, ProgramVersion.Build, isFile: true, out versionMatch))
		{
			return false;
		}
		return true;
	}
}
