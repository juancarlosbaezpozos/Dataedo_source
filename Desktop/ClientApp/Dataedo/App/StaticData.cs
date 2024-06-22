using System.Collections.Generic;
using System.ComponentModel;
using Dataedo.App.API.Models;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Tools;
using Dataedo.Data.Commands;
using Dataedo.Data.Commands.Enums;
using Dataedo.Data.Factories;
using Dataedo.LicenseHelperLibrary.Repository;
using Dataedo.Model.Data.Repository;
using Dataedo.Shared.Enums;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App;

public static class StaticData
{
	private static AppLicense license;

	public static SplashScreenManager splashScreenManager = new SplashScreenManager(typeof(DefaultWaitForm), new SplashFormProperties());

	public static DBSplashScreen SplashScreen;

	private static bool isSplashScreenShown = false;

	public static bool SignInAfterAfterSignOutInProgress = false;

	public static bool ClosingMainFormAfterErrorInProgress = false;

	public static string DataedoConnectionString;

	public static DatabaseType RepositoryType;

	public static LicenseEnum LicenseEnum;

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static string DataedoLastValidConnectionString;

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static DatabaseType? LastValidRepositoryType;

	public static bool IsCmdImport = false;

	public static string RepositoryTypeString => RepositoryType.ToString();

	public static bool IsProjectFile => RepositoryType == DatabaseType.SqlServerCe;

	public static CommandsSetBase Commands { get; private set; }

	public static LicenseHelper LicenseHelper { get; private set; }

	public static SharedDatabaseTypeEnum.DatabaseType? CrashedDatabaseType { get; set; }

	public static string CrashedDatabaseTypeString => CrashedDatabaseType.ToString();

	public static string CrashedDBMSVersion { get; internal set; }

	public static string Host { get; internal set; }

	public static string Database { get; internal set; }

	public static string Edition { get; set; }

	public static string ProductVersion { get; set; }

	public static string RepositoryCollation { get; set; }

	public static string RepositoryDbVersion { get; internal set; }

	public static string ServerCollation { get; set; }

	public static int? CFCount { get; set; }

	public static string Guid { get; set; }

	public static string RepositoryTypeSource
	{
		get
		{
			if (!IsProjectFile)
			{
				return "DATABASE";
			}
			return "FILE";
		}
	}

	public static string AppType => "DESKTOP";

	public static string Token { get; set; }

	public static ProfileResult Profile { get; set; }

	public static List<LicenseDataResult> Licenses { get; set; }

	public static string LastLicenseId { get; set; }

	public static int CurrentLicenseId { get; internal set; }

	public static AppLicense License
	{
		get
		{
			return license;
		}
		set
		{
			license = value;
		}
	}

	public static void Initialize(DatabaseType databaseType, string connectionString)
	{
		Commands = CommandsFactory.GetCommands(databaseType, connectionString);
		LicenseHelper = new LicenseHelper();
		LicenseHelper.Initialize(databaseType, connectionString);
	}

	public static void SetRepositoryData(RepositoryData repositoryData)
	{
		RepositoryDbVersion = repositoryData?.Version;
		Edition = repositoryData?.Edition;
		RepositoryCollation = repositoryData?.RepositoryCollation;
		ServerCollation = repositoryData?.ServerCollation;
		CFCount = repositoryData?.CFCount;
		Guid = repositoryData?.Guid;
		if (!string.IsNullOrEmpty(DataedoConnectionString))
		{
			LoginHelper.GetRepositoryVersion(out var repositoryVersion);
			ProductVersion = repositoryVersion?.ApplicationVersionString;
		}
	}

	public static void RefreshLicenseId()
	{
		string login = ((License.IsFileLicense || string.IsNullOrEmpty(Profile?.Email)) ? LastConnectionInfo.LOGIN_INFO.DataedoRealLogin : Profile?.Email);
		if (!IsProjectFile)
		{
			CurrentLicenseId = LicenseHelper.GetLicenseIdForLogin(DataedoConnectionString, login);
		}
		else
		{
			CurrentLicenseId = -1;
		}
	}

	public static void ClearDatabaseInfoForCrashes()
	{
		CrashedDatabaseType = null;
		CrashedDBMSVersion = null;
	}

	public static void ShowSplashScreen()
	{
		if (!isSplashScreenShown)
		{
			SplashScreen = new DBSplashScreen();
			isSplashScreenShown = true;
			SplashScreenManager.ShowForm(SplashScreen, typeof(DBSplashScreen));
		}
	}

	public static void CloseSplashScreen()
	{
		if (isSplashScreenShown)
		{
			isSplashScreenShown = false;
			SplashScreenManager.CloseForm();
			SplashScreen = null;
		}
	}

	public static void ClearRepositoryData()
	{
		DataedoConnectionString = null;
		Host = null;
		DataedoLastValidConnectionString = null;
		LastValidRepositoryType = null;
		Database = null;
	}
}
