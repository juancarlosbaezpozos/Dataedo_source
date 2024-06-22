using System;
using System.Data;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;

namespace Dataedo.App.Classes;

public static class Authentication
{
	public static DataTable authenticationDataSource;

	public static DataTable authenticationDataSourceWithout2FA;

	private static DataTable GetBaseAuthDataTabel()
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("id", typeof(int));
		dataTable.Columns.Add("type", typeof(string));
		dataTable.Rows.Add(1, "Windows Authentication");
		dataTable.Rows.Add(2, "SQL Server Authentication");
		dataTable.Rows.Add(3, "Azure Active Directory - Password");
		dataTable.Rows.Add(4, "Azure Active Directory - Integrated");
		return dataTable;
	}

	public static void CreateAuthenticationDataSource()
	{
		authenticationDataSource = GetBaseAuthDataTabel();
		authenticationDataSource.Rows.Add(5, "Azure Active Directory - Universal with MFA");
		authenticationDataSourceWithout2FA = GetBaseAuthDataTabel();
	}

	public static void SetAuthenticationDataSource(LookUpEdit authenticationSqlServerLookUpEdit, bool exlude2FA = false)
	{
		if (!exlude2FA)
		{
			authenticationSqlServerLookUpEdit.Properties.DataSource = authenticationDataSource;
			authenticationSqlServerLookUpEdit.Properties.DropDownRows = authenticationDataSource.Rows.Count;
		}
		else
		{
			authenticationSqlServerLookUpEdit.Properties.DataSource = authenticationDataSourceWithout2FA;
			authenticationSqlServerLookUpEdit.Properties.DropDownRows = authenticationDataSourceWithout2FA.Rows.Count;
		}
	}

	public static bool IsWindowsAuthentication(LookUpEdit authenticationSqlServerLookUpEdit)
	{
		if (authenticationSqlServerLookUpEdit.EditValue != null)
		{
			return Convert.ToInt32(authenticationSqlServerLookUpEdit.EditValue) == 1;
		}
		return false;
	}

	public static bool IsWindowsAuthentication(AuthenticationType.AuthenticationTypeEnum type)
	{
		return type == AuthenticationType.AuthenticationTypeEnum.WindowsAuthentication;
	}

	public static bool IsWindowsAuthentication(string authenticationType)
	{
		return IsWindowsAuthentication(AuthenticationType.StringToType(authenticationType));
	}

	public static bool IsActiveDirectoryIntegrated(LookUpEdit authenticationSqlServerLookUpEdit)
	{
		if (authenticationSqlServerLookUpEdit.EditValue != null)
		{
			return Convert.ToInt32(authenticationSqlServerLookUpEdit.EditValue) == 4;
		}
		return false;
	}

	public static bool IsActiveDirectoryIntegrated(string authenticationType)
	{
		return AuthenticationType.StringToType(authenticationType) == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryIntegrated;
	}

	public static bool IsActiveDirectoryInteractive(LookUpEdit authenticationSqlServerLookUpEdit)
	{
		if (authenticationSqlServerLookUpEdit.EditValue != null)
		{
			return Convert.ToInt32(authenticationSqlServerLookUpEdit.EditValue) == 5;
		}
		return false;
	}

	public static bool IsActiveDirectoryInteractive(string authenticationType)
	{
		return AuthenticationType.StringToType(authenticationType) == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryInteractive;
	}

	public static int GetIndex(LookUpEdit authenticationSqlServerLookUpEdit)
	{
		return Convert.ToInt32(authenticationSqlServerLookUpEdit.EditValue);
	}
}
