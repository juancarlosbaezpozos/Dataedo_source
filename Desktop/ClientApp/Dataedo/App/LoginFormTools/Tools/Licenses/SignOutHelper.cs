using Dataedo.App.Tools;

namespace Dataedo.App.LoginFormTools.Tools.Licenses;

public static class SignOutHelper
{
	public static void SignOut()
	{
		LastConnectionInfo.LOGIN_INFO.SessionData = null;
		LastConnectionInfo.LOGIN_INFO.LicenseFileData = null;
		StaticData.Token = null;
		StaticData.License = null;
		StaticData.Licenses = null;
		StaticData.Profile = null;
		LastConnectionInfo.Save();
	}
}
