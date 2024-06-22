using Dataedo.Data.Base.Tools;

namespace Dataedo.App.Tools;

public class DataedoLoginStrategy : ILoginStrategy
{
	public string GetUserLogin()
	{
		if (!StaticData.License.IsFileLicense && !string.IsNullOrEmpty(StaticData.Profile?.Email))
		{
			return StaticData.Profile.Email;
		}
		return LastConnectionInfo.LOGIN_INFO.DataedoRealLogin;
	}
}
