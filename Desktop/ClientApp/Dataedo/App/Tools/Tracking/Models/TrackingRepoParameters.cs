using Dataedo.App.Tools.Tracking.Interfaces;

namespace Dataedo.App.Tools.Tracking.Models;

public class TrackingRepoParameters : ITrackingRepoParameters
{
	public string RepoType => StaticData.RepositoryTypeSource;

	public string RepoGuid => StaticData.Guid;

	public string RepoVersion => StaticData.ProductVersion;

	public string RepoDBVersion => StaticData.RepositoryDbVersion;

	public string RepoDBEdition => StaticData.Edition;

	public string ServerCollation => StaticData.ServerCollation;

	public string RepoCollation => StaticData.RepositoryCollation;

	public string CFCount => StaticData.CFCount?.ToString();
}
