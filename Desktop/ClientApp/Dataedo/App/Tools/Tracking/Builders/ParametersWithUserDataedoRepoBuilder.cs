using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoRepoBuilder : TrackingParametersBuilder
{
	private readonly DataedoParameterMappings dataedoParameterMappings;

	private readonly RepoParameterMappings repoParameterMappings;

	private readonly UserParameterMappings userParameterMappings;

	private readonly ITrackingRepoParameters trackingRepoParameters;

	private readonly ITrackingDataedoParameters trackingDataedoParameters;

	private readonly ITrackingUserParameters trackingUserParameters;

	public ParametersWithUserDataedoRepoBuilder(ITrackingRepoParameters trackingRepoParameters, ITrackingDataedoParameters trackingDataedoParameters, ITrackingUserParameters trackingUserParameters)
	{
		this.trackingRepoParameters = trackingRepoParameters;
		this.trackingDataedoParameters = trackingDataedoParameters;
		this.trackingUserParameters = trackingUserParameters;
		dataedoParameterMappings = new DataedoParameterMappings();
		repoParameterMappings = new RepoParameterMappings();
		userParameterMappings = new UserParameterMappings();
	}

	public override TrackingParametersBuilder BuildConnectionParameters()
	{
		return this;
	}

	public override TrackingParametersBuilder BuildDataedoParameters()
	{
		dataedoParameterMappings.MapParameters(nameValueCollection, trackingDataedoParameters);
		return this;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		return this;
	}

	public override TrackingParametersBuilder BuildOSParameters()
	{
		return this;
	}

	public override TrackingParametersBuilder BuildRepoParameters()
	{
		repoParameterMappings.MapParameters(nameValueCollection, trackingRepoParameters);
		return this;
	}

	public override TrackingParametersBuilder BuildUserParameters()
	{
		userParameterMappings.MapParameters(nameValueCollection, trackingUserParameters);
		return this;
	}
}
