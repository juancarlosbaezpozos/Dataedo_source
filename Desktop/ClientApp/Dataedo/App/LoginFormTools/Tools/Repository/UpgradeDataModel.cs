using Dataedo.App.LoginFormTools.Tools.Recent;
using Dataedo.LicenseHelperLibrary.Repository;

namespace Dataedo.App.LoginFormTools.Tools.Repository;

public class UpgradeDataModel
{
	public RecentItemModel RecentItemModel { get; set; }

	public RepositoryVersion RepositoryVersion { get; set; }

	public string ConnectionString { get; set; }

	public RepositoryOperation RepositoryOperation { get; set; }

	public bool DetachWebCatalog { get; set; }

	public UpgradeDataModel(RecentItemModel recentItemModel, RepositoryVersion repositoryVersion, string connectionString, bool detachWebCatalog = false)
	{
		RecentItemModel = recentItemModel;
		RepositoryVersion = repositoryVersion;
		ConnectionString = connectionString;
		DetachWebCatalog = detachWebCatalog;
	}

	public UpgradeDataModel(RecentItemModel recentItemModel, RepositoryVersion repositoryVersion, string connectionString, RepositoryOperation repositoryOperation, bool detachWebCatalog = false)
	{
		RecentItemModel = recentItemModel;
		RepositoryVersion = repositoryVersion;
		ConnectionString = connectionString;
		RepositoryOperation = repositoryOperation;
		DetachWebCatalog = detachWebCatalog;
	}
}
