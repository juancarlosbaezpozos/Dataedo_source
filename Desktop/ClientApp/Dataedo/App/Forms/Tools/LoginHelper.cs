using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Enums;
using Dataedo.App.LoginFormTools.Tools.Repository.RepositoryCreator;
using Dataedo.App.Tools;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Upgrade;
using Dataedo.Data.Commands;
using Dataedo.Data.Commands.Enums;
using Dataedo.LicenseHelperLibrary.Repository;

namespace Dataedo.App.Forms.Tools;

public static class LoginHelper
{
	public static RepositoryExistsStatusEnum CheckIfRepositoryExists(string connectionString, string database, Form owner = null)
	{
		Exception exception = null;
		return CheckIfRepositoryExists(connectionString, database, ref exception, showMessage: true, owner);
	}

	public static async Task<RepositoryExistsStatusEnum> CheckIfRepositoryExistsAsync(string connectionString, string database, bool showMessage, CancellationToken cancellationToken)
	{
		RepositoryExistsStatusEnum repositoryExistsStatusEnum = await DatabaseHelper.CheckIfRepositoryExistsAsync(connectionString, database, cancellationToken);
		if (showMessage && repositoryExistsStatusEnum == RepositoryExistsStatusEnum.NotExists)
		{
			GeneralMessageBoxesHandling.Show("Unable to connect to repository." + Environment.NewLine + "Specified database doesn't exist or you don't have access to it." + Environment.NewLine + "To grant access to existing repository please use <href=" + Paths.GetAdminConsolePath() + ">Administration Console</href>.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, null, "Unable to connect to repository." + Environment.NewLine + "Specified database doesn't exist or you don't have access to it." + Environment.NewLine + "To grant access to existing repository please use Administration Console: " + Paths.GetAdminConsolePath() + ".");
			return repositoryExistsStatusEnum;
		}
		return repositoryExistsStatusEnum;
	}

	public static RepositoryExistsStatusEnum CheckIfRepositoryExists(string connectionString, string database, ref Exception exception, bool showMessage = true, Form owner = null)
	{
		RepositoryExistsStatusEnum repositoryExistsStatusEnum = DatabaseHelper.CheckIfRepositoryExists(connectionString, database, ref exception);
		if (showMessage && repositoryExistsStatusEnum == RepositoryExistsStatusEnum.NotExists)
		{
			string message = "Unable to connect to repository." + Environment.NewLine + "Specified database doesn't exist or you don't have access to it." + Environment.NewLine + "To grant access to existing repository please use <href=" + Paths.GetAdminConsolePath() + ">Administration Console</href>.";
			string messageSimple = "Unable to connect to repository." + Environment.NewLine + "Specified database doesn't exist or you don't have access to it." + Environment.NewLine + "To grant access to existing repository please use Administration Console: " + Paths.GetAdminConsolePath() + ".";
			GeneralMessageBoxesHandling.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner, messageSimple);
			return repositoryExistsStatusEnum;
		}
		return repositoryExistsStatusEnum;
	}

	public static bool GetRepositoryVersion(out RepositoryVersion repositoryVersion, Form owner = null)
	{
		return GetRepositoryVersion(out repositoryVersion, StaticData.DataedoConnectionString, owner);
	}

	public static bool GetRepositoryVersion(out RepositoryVersion repositoryVersion, string connectionString, Form owner = null)
	{
		repositoryVersion = StaticData.LicenseHelper.GetRepositoryVersion(connectionString);
		return ProcessCheckingRepositoryVersion(repositoryVersion, owner);
	}

	public static bool GetRepositoryVersion(out RepositoryVersion repositoryVersion, CommandsSetBase commands, Form owner = null)
	{
		repositoryVersion = StaticData.LicenseHelper.GetRepositoryVersion(commands);
		return ProcessCheckingRepositoryVersion(repositoryVersion, owner);
	}

	private static bool ProcessCheckingRepositoryVersion(RepositoryVersion repositoryVersion, Form owner = null)
	{
		if (repositoryVersion == null)
		{
			GeneralMessageBoxesHandling.Show("Unable to check repository version. Database error occurred.\nPlease contact your administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			return false;
		}
		return true;
	}

	public static void UpgradeFileRepository(ref RepositoryVersion repoVersion, UpgradeForm upgradeForm, ref bool upgradeResult)
	{
		string text = (repoVersion.Stable ? string.Empty : (Environment.NewLine + Environment.NewLine + "<b>Repository is unstable and may cause errors. It is recommended to restore it from backup before upgrading. Do you want to continue anyway?</b>"));
		MessageBoxIcon messageBoxIcon = (repoVersion.Stable ? MessageBoxIcon.Question : MessageBoxIcon.Exclamation);
		GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("Do you want to <b>upgrade the file</b>?" + Environment.NewLine + Environment.NewLine + "It is recommended to backup file first." + text, "Upgrade", MessageBoxButtons.YesNo, messageBoxIcon, null, 1, upgradeForm);
		if (handlingDialogResult != null && handlingDialogResult.DialogResult == DialogResult.Yes)
		{
			upgradeResult = Upgrade(ref repoVersion);
		}
	}

	public static bool Upgrade(ref RepositoryVersion repoVersion, Form owner = null)
	{
		bool flag;
		if (repoVersion.Version == 5 && repoVersion.Update == 0)
		{
			flag = new V5_1(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 5 && repoVersion.Update == 1)
		{
			flag = new V5_2(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 5 && repoVersion.Update == 2)
		{
			flag = new V6_0(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 6 && repoVersion.Update == 0 && repoVersion.Build == 0)
		{
			flag = new V6_0_1(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 6 && repoVersion.Update == 0 && repoVersion.Build == 1)
		{
			flag = new V6_0_2(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 6 && repoVersion.Update == 0 && repoVersion.Build == 2)
		{
			flag = new V6_0_3(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 6 && repoVersion.Update == 0 && repoVersion.Build == 3)
		{
			flag = new V6_1(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 6 && repoVersion.Update == 1 && repoVersion.Build == 0)
		{
			flag = new V7_0(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 7 && repoVersion.Update == 0 && repoVersion.Build == 0)
		{
			flag = new V7_1(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 7 && repoVersion.Update == 1 && repoVersion.Build == 0)
		{
			flag = new V7_2(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 7 && repoVersion.Update == 2 && repoVersion.Build == 0)
		{
			flag = new V7_2_2(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 7 && repoVersion.Update == 2 && repoVersion.Build == 2)
		{
			flag = new V7_3(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 7 && repoVersion.Update == 3 && repoVersion.Build == 0)
		{
			flag = new V7_4(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 7 && repoVersion.Update == 4 && repoVersion.Build == 0)
		{
			flag = new V7_5(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 7 && repoVersion.Update == 5 && repoVersion.Build == 0)
		{
			flag = new V8_0(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 8 && repoVersion.Update == 0 && repoVersion.Build == 0)
		{
			flag = new V8_1(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 8 && repoVersion.Update == 0 && repoVersion.Build == 0)
		{
			flag = new V8_1(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && repoVersion.Version == 8 && repoVersion.Update == 1 && repoVersion.Build == 0)
		{
			flag = new V8_2(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		if (flag && repoVersion.Version == 8 && repoVersion.Update == 2 && repoVersion.Build == 0)
		{
			flag = new V9_1(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion, owner))
			{
				flag = false;
			}
		}
		if (flag && repoVersion.Version == 9 && repoVersion.Update == 1 && repoVersion.Build == 0)
		{
			flag = new V10_0(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion))
			{
				flag = false;
			}
		}
		if (flag && repoVersion.Version == 10 && repoVersion.Update == 0 && repoVersion.Build == 0)
		{
			flag = new V10_1(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion))
			{
				flag = false;
			}
		}
		if (flag && repoVersion.Version == 10 && repoVersion.Update == 1 && repoVersion.Build == 0)
		{
			flag = new V10_2(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion))
			{
				flag = false;
			}
		}
		if (flag && repoVersion.Version == 10 && repoVersion.Update == 2 && repoVersion.Build == 0)
		{
			flag = new V10_3(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
			if (!GetRepositoryVersion(out repoVersion))
			{
				flag = false;
			}
		}
		if (flag && repoVersion.Version == 10 && repoVersion.Update == 3 && repoVersion.Build == 0)
		{
			flag = new V10_3_2(DatabaseType.SqlServerCe, StaticData.DataedoConnectionString).Upgrade(owner);
		}
		if (flag)
		{
			GeneralMessageBoxesHandling.Show("File was successfully upgraded.", "Upgrade", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, owner);
		}
		return flag;
	}
}
