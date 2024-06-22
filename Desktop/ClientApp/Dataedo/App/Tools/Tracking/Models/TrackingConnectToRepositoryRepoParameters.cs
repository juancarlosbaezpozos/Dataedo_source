using Dataedo.App.Tools.Tracking.Interfaces;

namespace Dataedo.App.Tools.Tracking.Models;

public class TrackingConnectToRepositoryRepoParameters : ITrackingRepoParameters
{
	private bool isFile;

	private readonly bool forceEmptyRepoType;

	public string RepoType
	{
		get
		{
			if (!forceEmptyRepoType)
			{
				if (!isFile)
				{
					return "DATABASE";
				}
				return "FILE";
			}
			return string.Empty;
		}
	}

	public string RepoGuid => StaticData.Guid;

	public string RepoVersion => StaticData.ProductVersion;

	public string RepoDBVersion => StaticData.RepositoryDbVersion;

	public string RepoDBEdition => StaticData.Edition;

	public string ServerCollation => StaticData.ServerCollation;

	public string RepoCollation => StaticData.RepositoryCollation;

	public string CFCount => StaticData.CFCount?.ToString();

	public TrackingConnectToRepositoryRepoParameters(bool isFile, bool forceEmptyRepoType = false)
	{
		this.isFile = isFile;
		this.forceEmptyRepoType = forceEmptyRepoType;
	}
}
