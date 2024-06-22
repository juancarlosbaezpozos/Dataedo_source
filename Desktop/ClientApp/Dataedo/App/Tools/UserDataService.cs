using System;
using Dataedo.App.API.Enums;
using Dataedo.App.API.Models;

namespace Dataedo.App.Tools;

public static class UserDataService
{
	public static string GetBaseLicenseText()
	{
		string result = string.Empty;
		switch (StaticData.License.LicenseTypeValue)
		{
		case LicenseTypeEnum.LicenseType.Trial:
			result = "<b>License: </b>" + StaticData.License.PackageName + ((!StaticData.License.End.HasValue) ? null : (Environment.NewLine + "<b>Expires on:</b> " + StaticData.License.EndUtcDateTime.Value.ToLocalTime().ToShortDateString()));
			break;
		case LicenseTypeEnum.LicenseType.Educational:
			result = "<b>License: </b>" + StaticData.License.PackageName + Environment.NewLine + "<b>University:</b> " + StaticData.License.University + ((!StaticData.License.End.HasValue) ? null : (Environment.NewLine + "<b>Expires on:</b> " + StaticData.License.EndUtcDateTime.Value.ToLocalTime().ToShortDateString()));
			break;
		case LicenseTypeEnum.LicenseType.Subscription:
			result = "<b>License: </b>" + StaticData.License.PackageName + " - " + GetAccountString() + ((!StaticData.License.End.HasValue) ? null : (Environment.NewLine + "<b>Expires on:</b> " + StaticData.License.EndUtcDateTime.Value.ToLocalTime().ToShortDateString()));
			break;
		}
		return result;
	}

	public static string GetUsernameText()
	{
		if (StaticData.License.IsFileLicense)
		{
			return GetAccountString();
		}
		if (!string.IsNullOrEmpty(StaticData.Profile?.FullName))
		{
			return StaticData.Profile?.FullName;
		}
		return StaticData.Profile?.Email;
	}

	public static string GetAccountString()
	{
		AppLicense license = StaticData.License;
		if (license != null && license.LicenseTypeValue == LicenseTypeEnum.LicenseType.Educational)
		{
			return StaticData.License.University;
		}
		return $"{StaticData.License?.AccountName} [{StaticData.License?.AccountId}]";
	}

	private static string GetAccountString(AppLicense appLicense)
	{
		if (!appLicense.IsFileLicense)
		{
			return Environment.NewLine + "<b>Account:</b> " + GetAccountString();
		}
		return string.Empty;
	}
}
