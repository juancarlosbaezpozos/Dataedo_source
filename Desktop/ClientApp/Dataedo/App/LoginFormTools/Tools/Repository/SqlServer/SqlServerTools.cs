using System.Windows.Forms;
using Dataedo.App.Enums;
using Dataedo.App.LoginFormTools.Tools.Repository.RepositoryCreator;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.LicenseHelperLibrary.Repository;

namespace Dataedo.App.LoginFormTools.Tools.Repository.SqlServer;

internal class SqlServerTools
{
	internal static bool CheckIfServerIsValid(string serverConnectionString, string database, bool shouldExist, ref bool? isServerOnAzure, Form owner)
	{
		RepositoryExistsStatusEnum repositoryExistsStatusEnum = CheckRepositoryStatus(serverConnectionString, database, owner);
		isServerOnAzure = DatabaseHelper.IsDatabaseOnAzure(isProjectFile: false, serverConnectionString);
		switch (repositoryExistsStatusEnum)
		{
		case RepositoryExistsStatusEnum.NotExists:
		case RepositoryExistsStatusEnum.ExistsEmpty:
		{
			if (!LicenseHelper.HasLoginPermissions(serverConnectionString, "CREATE ANY DATABASE"))
			{
				GeneralMessageBoxesHandling.Show("User account has to have <b>CREATE ANY DATABASE</b> permission on the server.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
				return false;
			}
			string sQLServerProductVersion = DatabaseHelper.GetSQLServerProductVersion(serverConnectionString);
			if (string.IsNullOrWhiteSpace(sQLServerProductVersion))
			{
				GeneralMessageBoxesHandling.Show("Unable to determine version of SQL Server.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
				return false;
			}
			int num = int.Parse(sQLServerProductVersion.Split('.')[0]);
			if (num <= 8)
			{
				GeneralMessageBoxesHandling.Show("This version of SQL Server(" + sQLServerProductVersion + ") is not supported.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
				return false;
			}
			if (num > 15)
			{
				GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("This version of SQL Server(" + sQLServerProductVersion + ") is not supported, but you can try to use it. Do you want to continue?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, owner);
				if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
				{
					return false;
				}
			}
			if (isServerOnAzure == true)
			{
				switch (DatabaseHelper.CheckIfDatabaseExists(serverConnectionString, database))
				{
				case RepositoryExistsStatusEnum.NotExists:
					GeneralMessageBoxesHandling.Show("Dataedo can't create a database in that location, as it might cause unexpected costs! Please create a database manually, and point to it.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, owner);
					return false;
				case RepositoryExistsStatusEnum.Exists:
					break;
				default:
					return false;
				}
				if (!DatabaseHelper.IsDatabaseEmpty(serverConnectionString, database))
				{
					GeneralMessageBoxesHandling.Show("Can't create repository with this name, database already exists and is not empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return false;
				}
			}
			if (repositoryExistsStatusEnum == RepositoryExistsStatusEnum.NotExists || (isServerOnAzure == true && repositoryExistsStatusEnum == RepositoryExistsStatusEnum.ExistsEmpty))
			{
				return CheckConnectionToServer(serverConnectionString, owner);
			}
			break;
		}
		case RepositoryExistsStatusEnum.Exists:
			if (shouldExist)
			{
				return true;
			}
			if (!DatabaseHelper.IsDatabaseEmpty(serverConnectionString, database))
			{
				GeneralMessageBoxesHandling.Show("Can't create repository with this name, database already exists and is not empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			break;
		}
		return false;
	}

	internal static RepositoryExistsStatusEnum CheckRepositoryStatus(string serverConnectionString, string database, Form owner)
	{
		string messageWhenNoDbOwnerRole = "Database '<b>" + database + "</b>' exists. User has to have <b>db_owner</b> role in the database to continue.";
		return DatabaseHelper.CheckIfRepositoryExists(serverConnectionString, database, checkIfDbOwner: true, messageWhenNoDbOwnerRole, owner);
	}

	internal static bool CheckConnectionToServer(string serverConnectionString, Form owner)
	{
		return DatabaseHelper.TryConnection(serverConnectionString, showException: true, owner);
	}
}
