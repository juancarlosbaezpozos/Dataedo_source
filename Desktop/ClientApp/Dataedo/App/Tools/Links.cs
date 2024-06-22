using System;
using System.Diagnostics;
using System.Net;
using System.Web;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.Tools.Exceptions;

namespace Dataedo.App.Tools;

public static class Links
{
	public static readonly string EnterKeyLink = "enter_key";

	public static readonly string ChangeKeyLink = "change_key";

	public static readonly string UpgradeToPro = "upgrade";

	public static readonly string UpgradeTrialToEnterprise = "upgrade_trial_enterprise";

	public static readonly string EnableSchemaImportsAndChanges = "enable_schema_change_tracking";

	public static readonly string UpgradeTrialAndEnableSchemaImportsAndChanges = "upgrade_trial_enable_schema_change_tracking";

	private const string GAString = "?utm_source=App&utm_medium=App";

	private const string GAStringExistingParameters = "&utm_source=App&utm_medium=App";

	public static readonly string DataedoPricing = "https://dataedo.com/pricing?utm_source=App&utm_medium=App";

	public static readonly string DataedoBuy = "https://dataedo.com/app_buy?utm_source=App&utm_medium=App";

	public static readonly string HowToCustomizeHTML = "https://dataedo.com/docs/customize-html-export?utm_source=App&utm_medium=App";

	public static readonly string SupportedDatabases = "https://dataedo.com/docs/supported-databases?utm_source=App&utm_medium=App";

	public const string DataedoVersions = "https://dataedo.com/updates";

	public const string HomePageXml = "https://dataedo.com/updates";

	public static readonly string MariaDBPackage = "https://jira.mariadb.org/browse/MDEV-18092";

	public static readonly string DataedoBaseUrl = "https://dataedo.com";

	public const string DataedoAccount = "https://account.dataedo.com";

	public static readonly string SupportContactMailAddress = "support@dataedo.com";

	public static readonly string SupportContact = "mailto:" + SupportContactMailAddress;

	public static readonly string SalesContactMailAddress = "sales@dataedo.com";

	public static readonly string SalesContact = "mailto:" + SalesContactMailAddress;

	public static readonly string WhyUpgradeToPro = "https://dataedo.com/why-upgrade-to-pro?utm_source=App&utm_medium=App";

	public static readonly string Dataedo = "https://dataedo.com/?utm_source=App&utm_medium=App";

	public static readonly string LogicSystems = "https://logicsystems.com.pl/english.html?utm_source=App&utm_medium=App";

	public static readonly string CrashReports = "https://dataedo.com/crashreports/crashes/report?utm_source=App&utm_medium=App";

	private const string Trial = "https://dataedo.com/free-trial";

	private const string UpgradeTrial = "https://dataedo.com/upgrade-trial";

	public static readonly string ManageAccounts = "https://account.dataedo.com?utm_source=App&utm_medium=App";

	public static readonly string DataedoPricingBuy = "https://dataedo.com/pricing?buy";

	public static readonly string WebCatalog = "https://dataedo.com/product/web-catalog?utm_source=App&utm_medium=App";

	public static readonly string DocumentationSharing = "https://dataedo.com/docs/documentation-sharing?utm_source=App&utm_medium=App";

	public static readonly string ExportDescriptions = "https://dataedo.com/docs/export-descriptions-to-database?utm_source=App&utm_medium=App";

	public static readonly string CopyDescriptions = "https://dataedo.com/docs/copy-descriptions-between-documentations?utm_source=App&utm_medium=App";

	public static readonly string RepositorySchema = "https://dataedo.com/repository-schema";

	public static readonly string WelcomePageUrl = "https://dataedo.com/repo_creator_1_welcome?utm_source=App&utm_medium=App";

	public static readonly string ProgressPageUrl = "https://dataedo.com/repo_creator_2_creating?utm_source=App&utm_medium=App";

	public static readonly string CompletionPageUrl = "https://dataedo.com/repo_creator_4_connected?utm_source=App&utm_medium=App";

	public static readonly string GeneralDocumentationUrl = "https://dataedo.com/support?utm_source=App&utm_medium=App";

	public static readonly string OracleConnectionRequirementsUrl = "https://dataedo.com/docs/oracle-connection-requirements?utm_source=App&utm_medium=App#32-vs-64-bit?utm_source=App&utm_medium=App";

	public static readonly string InvalidPdfTemplateSupportUrl = "https://dataedo.com/docs?utm_source=App&utm_medium=App";

	public static readonly string CommandLineTrial = "https://dataedo.com/free-trial";

	public static readonly string CommandLineDataedoStore = "https://dataedo.com/store";

	public static readonly string CommandLineSupportUrl = "https://dataedo.com/docs/run-dataedo-from-command-line";

	public static readonly string SupportDocumentation = "https://dataedo.com/docs?utm_source=App&utm_medium=App";

	public static readonly string SupportTutorials = "https://dataedo.com/tutorials?utm_source=App&utm_medium=App";

	public static readonly string SupportSupport = "https://dataedo.com/support?utm_source=App&utm_medium=App";

	public static readonly string SupportCreateSupportTicket = "https://dataedo.com/add-ticket?utm_source=App&utm_medium=App";

	public static readonly string Community = "https://support.dataedo.com?utm_source=App&utm_medium=App";

	public static readonly string RibbonSupportProblem = "https://support.dataedo.com/s1-general/problems?utm_source=App&utm_medium=App";

	public static readonly string RibbonSupportQuestion = "https://support.dataedo.com/s1-general/questions?utm_source=App&utm_medium=App";

	public static readonly string RibbonSupportIdea = "https://support.dataedo.com/s1-general/ideas?utm_source=App&utm_medium=App";

	public static readonly string SuggestDataSource = "https://dataedo.com/suggest-data-source-redirect?utm_source=App&utm_medium=App";

	public static readonly string ODBCConnectionToDatabase = "https://dataedo.com/docs/connecting-to-database-with-odbc?utm_source=App&utm_medium=App";

	public static readonly string Chat = "https://dataedo.com/chat";

	public static readonly string CreatingServerRepository = "https://dataedo.com/docs/creating-server-repository?utm_source=App&utm_medium=App";

	public static readonly string SchemaImportsAndChanges = "https://dataedo.com/docs/schema-change-tracking?utm_source=App&utm_medium=App";

	public static readonly string HowToDocumentSQLServerDatabase = "https://dataedo.com/blog/how-to-document-sql-server-database-in-5-minutes-with-dataedo-free?utm_source=App&utm_medium=App#toc_2";

	public static readonly string BusinessGlossaryDocumentation = "https://dataedo.com/docs/business-glossary-introduction";

	public static readonly string DataClassificationDocumentation = "https://dataedo.com/docs/data-classification-introduction ";

	public static readonly string SchemaChangeTrackingDocumentation = "https://dataedo.com/docs/schema-change-tracking";

	public static readonly string DataProfilingDocumentation = "https://dataedo.com/docs/data-profiling-in-dataedo";

	public static readonly string DataProfilingConfiguration = "https://dataedo.com/docs/data-profiling-configuration?utm_source=App&utm_medium=App";

	public static readonly string ConnectionTroubleshooting = "https://dataedo.com/docs/desktop-signup-troubleshooting?utm_source=App&utm_medium=App";

	public static readonly string DataLineageDocumentation = "https://dataedo.com/docs/data-lineage-in-dataedo?utm_source=App&utm_medium=App";

	public const string DataProfilingSupportedSources = "https://dataedo.com/docs/supported-sources-data-profiling";

	public static readonly string ConfigureSQLServerSSLCertificate = "https://dataedo.com/docs/configuring-mssql-connection-encryption";

	public static readonly string PostgreSQLSSLErrors = "https://dataedo.com/docs/postgresql-ssl-errors";

	public static string AfterLoggedToRepositoryUrl(string programVersion)
	{
		return "https://dataedo.com/log_in_success/" + programVersion + "?utm_source=App&utm_medium=App";
	}

	public static string AfterRunUrl(string programVersion)
	{
		return "https://dataedo.com/run_application/" + programVersion + "?utm_source=App&utm_medium=App";
	}

	public static string GetSupportMailtoLink()
	{
		string versionWithBuild = ProgramVersion.VersionWithBuild;
		string text = (StaticData.IsProjectFile ? "file" : "server");
		return "mailto:support@dataedo.com?subject=Support request for Dataedo " + versionWithBuild + " " + text;
	}

	public static void OpenCTALink(string link, IWin32Window owner, bool afterLogin = true, Action afterLinkClick = null, Form ownerForm = null)
	{
		if (!EnterKeyLink.Equals(link) && !ChangeKeyLink.Equals(link) && !UpgradeToPro.Equals(link) && !UpgradeTrialToEnterprise.Equals(link))
		{
			if (EnableSchemaImportsAndChanges.Equals(link))
			{
				DB.SchemaImportsAndChanges.SetEnabled(enabled: true);
			}
			else if (UpgradeTrialAndEnableSchemaImportsAndChanges.Equals(link))
			{
				DB.SchemaImportsAndChanges.SetEnabled(enabled: true);
			}
			else
			{
				OpenLink(link, ownerForm);
			}
		}
		afterLinkClick?.Invoke();
	}

	public static void OpenLink(string link, Form owner = null)
	{
		try
		{
			if (link.StartsWith("mailto:"))
			{
				Process.Start(link);
				return;
			}
			Process process = new Process();
			process.StartInfo.FileName = link;
			process.StartInfo.Verb = "open";
			process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			process.Start();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to go to the link", owner);
		}
	}

	public static string GetTrialLink(TrialLocationEnum.TrialLocation location)
	{
		return "https://dataedo.com/free-trial?cta=app" + TrialLocationEnum.ValueToString(location) + "&utm_source=App&utm_medium=App";
	}

	public static string GetChatLink(string version)
	{
		return Chat + "?v=" + version + "&utm_source=App&utm_medium=App";
	}

	public static string GetDownloadLink(int version)
	{
		return string.Format("https://dataedo.com/download{0}{1}", version, "?utm_source=App&utm_medium=App");
	}

	public static void MakeGetRequestWithoutResponse(string url)
	{
		WebResponse webResponse = null;
		try
		{
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create(url);
			obj.Timeout = 5000;
			obj.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1312.57 Safari/537.17";
			webResponse = obj.GetResponse();
		}
		catch (Exception)
		{
		}
		finally
		{
			webResponse?.Close();
		}
	}

	public static string GetUpgradeTrialLink()
	{
		if (string.IsNullOrEmpty(StaticData.Profile?.Email) || string.IsNullOrEmpty(StaticData.License?.Token))
		{
			return DataedoPricing;
		}
		return "https://dataedo.com/upgrade-trial?email=" + HttpUtility.UrlEncode(StaticData.Profile?.Email) + "&token=" + HttpUtility.UrlEncode(StaticData.License?.Token);
	}
}
