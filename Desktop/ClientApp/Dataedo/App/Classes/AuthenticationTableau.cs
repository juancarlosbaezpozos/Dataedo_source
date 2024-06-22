using System;
using System.Data;
using DevExpress.XtraEditors;

namespace Dataedo.App.Classes;

public static class AuthenticationTableau
{
	public static DataTable authenticationDataSource;

	public static void CreateAuthenticationDataSource()
	{
		authenticationDataSource = new DataTable();
		authenticationDataSource.Columns.Add("id", typeof(int));
		authenticationDataSource.Columns.Add("type", typeof(string));
		authenticationDataSource.Rows.Add(2, "Username/password");
		authenticationDataSource.Rows.Add(6, "Personal Access Token");
	}

	public static void SetAuthenticationDataSource(LookUpEdit tableauAuthenticationLookUpEdit)
	{
		tableauAuthenticationLookUpEdit.Properties.DataSource = authenticationDataSource;
		tableauAuthenticationLookUpEdit.Properties.DropDownRows = authenticationDataSource.Rows.Count;
	}

	public static bool IsTokenAuthentication(LookUpEdit tableauAuthenticationLookUpEdit)
	{
		if (tableauAuthenticationLookUpEdit.EditValue != null)
		{
			return Convert.ToInt32(tableauAuthenticationLookUpEdit.EditValue) == 6;
		}
		return false;
	}

	public static int GetIndex(LookUpEdit tableauAuthenticationLookUpEdit)
	{
		return Convert.ToInt32(tableauAuthenticationLookUpEdit.EditValue);
	}
}
