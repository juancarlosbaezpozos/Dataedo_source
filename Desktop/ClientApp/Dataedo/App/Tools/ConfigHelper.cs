using System.Configuration;

namespace Dataedo.App.Tools;

public static class ConfigHelper
{
	public const string ForceRepoUpgrade = "ForceRepoUpgrade";

	public const string SalesforceClientId = "SalesforceClientId";

	public const string SalesforceLoginRedirectPage = "SalesforceLoginRedirectPage";

	public static bool GetForceRepoUpgradeConfigValue()
	{
		bool result = false;
		if (ConfigurationManager.AppSettings["ForceRepoUpgrade"] != null && bool.TryParse(ConfigurationManager.AppSettings["ForceRepoUpgrade"], out var result2) && result2)
		{
			result = true;
		}
		return result;
	}

	public static string GetSalesforceClientIdConfigValue()
	{
		string result = null;
		try
		{
			if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["SalesforceClientId"]))
			{
				result = new SimpleAES().DecryptString(ConfigurationManager.AppSettings["SalesforceClientId"]);
				return result;
			}
			return result;
		}
		catch
		{
			return result;
		}
	}

	public static string GetSalesforceLoginRedirectPageConfigValue()
	{
		string result = null;
		if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["SalesforceLoginRedirectPage"]))
		{
			result = ConfigurationManager.AppSettings["SalesforceLoginRedirectPage"];
		}
		return result;
	}
}
