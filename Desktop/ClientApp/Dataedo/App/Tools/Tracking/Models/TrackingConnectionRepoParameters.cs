using Dataedo.App.Tools.Tracking.Interfaces;

namespace Dataedo.App.Tools.Tracking.Models;

public class TrackingConnectionRepoParameters : ITrackingRepoParameters
{
	private string repoType;

	public string RepoType => repoType;

	public string RepoGuid { get; }

	public string RepoVersion { get; }

	public string RepoDBVersion { get; }

	public string RepoDBEdition { get; }

	public string ServerCollation { get; }

	public string RepoCollation { get; }

	public string CFCount { get; }

	public TrackingConnectionRepoParameters(bool isFile)
	{
		repoType = (isFile ? "FILE" : "DATABASE");
	}
}
