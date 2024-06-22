using System;
using System.Data;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;

namespace Dataedo.App.Data.StaticData;

internal class SnowflakeAuth
{
	public static DataTable authenticationDataSource;

	public static void CreateAuthenticationDataSource()
	{
		authenticationDataSource = new DataTable();
		authenticationDataSource.Columns.Add("id", typeof(int));
		authenticationDataSource.Columns.Add("type", typeof(string));
		authenticationDataSource.Rows.Add(1, "Password");
		authenticationDataSource.Rows.Add(2, "SSO (browser)");
		authenticationDataSource.Rows.Add(3, "JWT (private key)");
	}

	public static void SetAuthenticationDataSource(LookUpEdit authLookUpEdit)
	{
		authLookUpEdit.Properties.DataSource = authenticationDataSource;
		authLookUpEdit.Properties.DropDownRows = authenticationDataSource.Rows.Count;
		authLookUpEdit.EditValue = 1;
	}

	public static bool IsLoginPassword(LookUpEdit authLookUpEdit)
	{
		if (authLookUpEdit.EditValue != null)
		{
			return Convert.ToInt32(authLookUpEdit.EditValue) == 1;
		}
		return false;
	}

	public static bool IsLoginPassword(SnowflakeAuthMethod.SnowflakeAuthMethodEnum type)
	{
		return type == SnowflakeAuthMethod.SnowflakeAuthMethodEnum.LoginPassword;
	}

	public static bool IsLoginPassword(string authenticationType)
	{
		return IsLoginPassword(SnowflakeAuthMethod.StringToType(authenticationType));
	}

	public static bool isJWT(LookUpEdit authLookUpEdit)
	{
		if (authLookUpEdit.EditValue != null)
		{
			return Convert.ToInt32(authLookUpEdit.EditValue) == 3;
		}
		return false;
	}

	public static bool isJWT(SnowflakeAuthMethod.SnowflakeAuthMethodEnum type)
	{
		return type == SnowflakeAuthMethod.SnowflakeAuthMethodEnum.JWT_PrivateKey;
	}

	public static bool isJWT(string authenticationType)
	{
		return SnowflakeAuthMethod.StringToType(authenticationType) == SnowflakeAuthMethod.SnowflakeAuthMethodEnum.JWT_PrivateKey;
	}

	public static int GetIndex(LookUpEdit authLookUpEdit)
	{
		return Convert.ToInt32(authLookUpEdit.EditValue);
	}
}
