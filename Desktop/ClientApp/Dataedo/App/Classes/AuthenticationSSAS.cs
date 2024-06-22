using System;
using System.Data;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;

namespace Dataedo.App.Classes;

public static class AuthenticationSSAS
{
	public static DataTable authenticationDataSource;

	public static void CreateAuthenticationDataSource()
	{
		authenticationDataSource = new DataTable();
		authenticationDataSource.Columns.Add("id", typeof(int));
		authenticationDataSource.Columns.Add("type", typeof(string));
		authenticationDataSource.Rows.Add(1, "Windows Authentication");
		authenticationDataSource.Rows.Add(3, "Azure Active Directory - Password");
		authenticationDataSource.Rows.Add(4, "Azure Active Directory - Integrated");
	}

	public static void SetAuthenticationDataSource(LookUpEdit authenticationSsasServerLookUpEdit)
	{
		authenticationSsasServerLookUpEdit.Properties.DataSource = authenticationDataSource;
		authenticationSsasServerLookUpEdit.Properties.DropDownRows = authenticationDataSource.Rows.Count;
	}

	public static bool IsWindowsAuthentication(LookUpEdit authenticationSsasServerLookUpEdit)
	{
		if (authenticationSsasServerLookUpEdit.EditValue != null)
		{
			return Convert.ToInt32(authenticationSsasServerLookUpEdit.EditValue) == 1;
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

	public static bool IsActiveDirectoryIntegrated(LookUpEdit authenticationSsasServerLookUpEdit)
	{
		if (authenticationSsasServerLookUpEdit.EditValue != null)
		{
			return Convert.ToInt32(authenticationSsasServerLookUpEdit.EditValue) == 4;
		}
		return false;
	}

	public static bool IsActiveDirectoryIntegrated(string authenticationType)
	{
		return AuthenticationType.StringToType(authenticationType) == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryIntegrated;
	}

	public static int GetIndex(LookUpEdit authenticationSsasServerLookUpEdit)
	{
		return Convert.ToInt32(authenticationSsasServerLookUpEdit.EditValue);
	}
}
