namespace Dataedo.App.Tools.Tracking.Interfaces;

public interface ITrackingRepoParameters
{
	string RepoType { get; }

	string RepoGuid { get; }

	string RepoVersion { get; }

	string RepoDBVersion { get; }

	string RepoDBEdition { get; }

	string ServerCollation { get; }

	string RepoCollation { get; }

	string CFCount { get; }
}
