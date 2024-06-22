using System.Collections.Specialized;
using Dataedo.App.Tools.Tracking.Interfaces;

namespace Dataedo.App.Tools.Tracking.Mappings;

public class RepoParameterMappings
{
	private const string repoTypeParameterName = "repo_type";

	private const string repoVersionParameterName = "repo_version";

	private const string repoGuidParameterName = "repo_guid";

	private const string repoDBVersionParameterName = "repo_db_version";

	private const string repoDBEditionParameterName = "repo_db_edition";

	private const string repoCollationParameterName = "repo_collation";

	private const string repoServerCollationParameterName = "repo_server_collation";

	private const string repoCFCountParameterName = "repo_cf_count";

	public NameValueCollection MapParameters(NameValueCollection nameValueCollection, ITrackingRepoParameters trackingRepoParameters)
	{
		nameValueCollection.Add("repo_type", trackingRepoParameters?.RepoType);
		nameValueCollection.Add("repo_version", trackingRepoParameters?.RepoVersion);
		nameValueCollection.Add("repo_guid", trackingRepoParameters?.RepoGuid);
		nameValueCollection.Add("repo_db_version", trackingRepoParameters?.RepoDBVersion);
		nameValueCollection.Add("repo_db_edition", trackingRepoParameters?.RepoDBEdition);
		nameValueCollection.Add("repo_collation", trackingRepoParameters?.RepoCollation);
		nameValueCollection.Add("repo_server_collation", trackingRepoParameters?.ServerCollation);
		nameValueCollection.Add("repo_cf_count", trackingRepoParameters?.CFCount);
		return nameValueCollection;
	}
}
