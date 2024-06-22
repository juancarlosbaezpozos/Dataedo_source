using System.Collections.Generic;
using Dataedo.App.API.Models;
using Dataedo.App.Tools;
using Dataedo.CustomMessageBox;
using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.LoginFormTools.Tools.Licenses;

public static class SessionDataHelper
{
	public static void Save(string token)
	{
		LastConnectionInfo.LOGIN_INFO.SessionData = XmlFileSimpleAesService.EncryptData(new SessionData
		{
			Token = token,
			Profile = null,
			Licenses = null
		});
		LastConnectionInfo.LOGIN_INFO.LicenseFileData = null;
		StaticData.Token = token;
		StaticData.Profile = null;
		StaticData.Licenses = null;
		LastConnectionInfo.Save();
	}

	public static void Save(string token, ProfileResult profile, List<LicenseDataResult> licenses, LicenseDataResult license)
	{
		LastConnectionInfo.LOGIN_INFO.SessionData = XmlFileSimpleAesService.EncryptData(new SessionData
		{
			Token = token,
			Profile = profile,
			Licenses = licenses,
			LastLicenseId = license.Id
		});
		LastConnectionInfo.LOGIN_INFO.LicenseFileData = null;
		StaticData.Token = token;
		StaticData.Profile = profile;
		CustomMessageBoxForm.Email = profile?.Email;
		CustomFormattedMessageBoxForm.Email = profile?.Email;
		StaticData.Licenses = licenses;
		StaticData.License = new AppLicense(license);
		StaticData.LicenseEnum = LicenseEnum.DataedoAccount;
		LastConnectionInfo.Save();
	}

	public static void Save()
	{
		LastConnectionInfo.LOGIN_INFO.SessionData = XmlFileSimpleAesService.EncryptData(new SessionData
		{
			Token = StaticData.Token,
			Profile = StaticData.Profile,
			Licenses = StaticData.Licenses,
			LastLicenseId = StaticData.License?.Id
		});
		LastConnectionInfo.LOGIN_INFO.LicenseFileData = null;
		LastConnectionInfo.Save();
	}

	public static bool Load()
	{
		SessionData storedSessionData = GetStoredSessionData();
		StaticData.Token = storedSessionData?.Token;
		StaticData.Profile = storedSessionData?.Profile;
		CustomMessageBoxForm.Email = storedSessionData?.Profile?.Email;
		CustomFormattedMessageBoxForm.Email = storedSessionData?.Profile?.Email;
		StaticData.Licenses = storedSessionData?.Licenses;
		StaticData.LastLicenseId = storedSessionData?.LastLicenseId;
		return storedSessionData != null;
	}

	public static SessionData GetStoredSessionData()
	{
		return XmlFileSimpleAesService.DecryptData<SessionData>(LastConnectionInfo.LOGIN_INFO.SessionData);
	}

	public static string GetStoredToken()
	{
		return GetStoredSessionData()?.Token;
	}

	public static string GetLastLicenseId()
	{
		return GetStoredSessionData()?.LastLicenseId;
	}
}
