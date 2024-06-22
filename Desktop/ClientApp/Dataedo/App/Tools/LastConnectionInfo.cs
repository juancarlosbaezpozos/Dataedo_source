using System;
using System.Drawing;
using Dataedo.App.Classes;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.DataProcessing.Classes;
using Dataedo.Shared.Enums;
using RecentProjectsLibrary;

namespace Dataedo.App.Tools;

public class LastConnectionInfo
{
	public static LoginInfo LOGIN_INFO;

	public static string MyDocumentsPath => Environment.GetFolderPath(Environment.SpecialFolder.Personal);

	public static string Password
	{
		set
		{
			try
			{
				SimpleAES simpleAES = new SimpleAES();
				LOGIN_INFO.DataedoPasswordEncrypted = simpleAES.EncryptToString(value);
			}
			catch
			{
				LOGIN_INFO.DataedoPasswordEncrypted = "";
			}
		}
	}

	public static string PasswordEncrypted
	{
		get
		{
			return LOGIN_INFO.DataedoPasswordEncrypted;
		}
		set
		{
			LOGIN_INFO.DataedoPasswordEncrypted = value;
		}
	}

	public static RecentProject GetRecentProject()
	{
		return new RecentProject(LOGIN_INFO.DataedoDatabase, LOGIN_INFO.DataedoHost, LOGIN_INFO.DataedoLogin, LOGIN_INFO.AuthenticationType, LOGIN_INFO.DataedoConnectionMode, PrepareValue.ToInt(LOGIN_INFO.Port), null);
	}

	public static void Save()
	{
		ConfigurationFileHelper.CreateConfFile(LOGIN_INFO, !StaticData.IsCmdImport);
	}

	public static void SetValues(string host, string database, string login, AuthenticationType.AuthenticationTypeEnum authenticationType, string connectionMode, int? port)
	{
		LOGIN_INFO.DataedoHost = host;
		LOGIN_INFO.DataedoDatabase = database;
		if (!Authentication.IsWindowsAuthentication(authenticationType))
		{
			LOGIN_INFO.DataedoLogin = login;
		}
		LOGIN_INFO.AuthenticationType = AuthenticationType.TypeToString(authenticationType);
		LOGIN_INFO.DataedoConnectionMode = connectionMode;
		LOGIN_INFO.Port = port.ToString();
		ConfigurationFileHelper.CreateConfFile(LOGIN_INFO, !StaticData.IsCmdImport);
	}

	public static void SetDocumentationPath(string documentationPath)
	{
		LOGIN_INFO.DocumentationPath = documentationPath;
		ConfigurationFileHelper.CreateConfFile(LOGIN_INFO, !StaticData.IsCmdImport);
	}

	public static void SetFileRepositoryPath(string fileRepositoryPath)
	{
		LOGIN_INFO.FileRepositoryPath = fileRepositoryPath;
		ConfigurationFileHelper.CreateConfFile(LOGIN_INFO, !StaticData.IsCmdImport);
	}

	public static void SetFileProjectPath(string projectFilePath)
	{
		LOGIN_INFO.ProjectFilePath = projectFilePath;
		ConfigurationFileHelper.CreateConfFile(LOGIN_INFO, !StaticData.IsCmdImport);
	}

	public static void SetFileProjectLicenseKey(string licenseKey)
	{
		LOGIN_INFO.ProjectFileLicenseKey = licenseKey;
		ConfigurationFileHelper.CreateConfFile(LOGIN_INFO, !StaticData.IsCmdImport);
	}

	public static void SetTrialUsed(DateTime trialExpirationDate)
	{
		LOGIN_INFO.TrialUsed = true;
		LOGIN_INFO.TrialUsedExpirationDate = trialExpirationDate;
		ConfigurationFileHelper.CreateConfFile(LOGIN_INFO, !StaticData.IsCmdImport);
	}

	public static void SetWindowLocation(Point point)
	{
		LOGIN_INFO.WindowLocation = point;
		ConfigurationFileHelper.CreateConfFile(LOGIN_INFO, !StaticData.IsCmdImport);
	}

	public static void SetWindowSize(Size size)
	{
		LOGIN_INFO.WindowSize = size;
		ConfigurationFileHelper.CreateConfFile(LOGIN_INFO, !StaticData.IsCmdImport);
	}

	public static void SetAutoAddToModule(bool autoAddToModule)
	{
		LOGIN_INFO.AutoAddToModule = autoAddToModule;
		ConfigurationFileHelper.CreateConfFile(LOGIN_INFO, !StaticData.IsCmdImport);
	}

	public static void SetAutosave(bool autosave)
	{
		LOGIN_INFO.Autosave = autosave;
		ConfigurationFileHelper.CreateConfFile(LOGIN_INFO, !StaticData.IsCmdImport);
	}

	public static void SetEnableTracking(bool enableTracking)
	{
		LOGIN_INFO.DisableTracking = enableTracking;
		ConfigurationFileHelper.CreateConfFile(LOGIN_INFO, !StaticData.IsCmdImport);
	}

	public static void SetConnectionTimeout(int connectionTimeout)
	{
		if (LOGIN_INFO != null)
		{
			LOGIN_INFO.ConnectionTimeout = connectionTimeout;
			ConfigurationFileHelper.CreateConfFile(LOGIN_INFO, !StaticData.IsCmdImport);
		}
	}

	public static void SetRepositoryTimeout(int repositoryTimeout)
	{
		if (LOGIN_INFO != null)
		{
			LOGIN_INFO.RepositoryTimeout = repositoryTimeout;
			ConfigurationFileHelper.CreateConfFile(LOGIN_INFO, !StaticData.IsCmdImport);
		}
	}
}
