using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;
using Dataedo.App.Tools.Tracking.Models;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoRepoConnectionBuilder : TrackingParametersBuilder
{
	private readonly UserParameterMappings userParameterMappings;

	private readonly DataedoParameterMappings dataedoParameterMappings;

	private readonly RepoParameterMappings repoParameterMappings;

	private readonly ConnectionParameterMappings connectionParameterMappings;

	private readonly ITrackingUserParameters trackingUserParameters;

	private readonly ITrackingDataedoParameters trackingDataedoParameters;

	private readonly ITrackingRepoParameters trackingRepoParameters;

	private readonly TrackingConnectionParameters trackingConnectionParameters;

	public ParametersWithUserDataedoRepoConnectionBuilder(TrackingConnectionParameters trackingConnectionParameters, ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, ITrackingRepoParameters trackingRepoParameters)
	{
		this.trackingConnectionParameters = trackingConnectionParameters;
		this.trackingUserParameters = trackingUserParameters;
		this.trackingDataedoParameters = trackingDataedoParameters;
		this.trackingRepoParameters = trackingRepoParameters;
		connectionParameterMappings = new ConnectionParameterMappings();
		dataedoParameterMappings = new DataedoParameterMappings();
		userParameterMappings = new UserParameterMappings();
		repoParameterMappings = new RepoParameterMappings();
	}

	public override TrackingParametersBuilder BuildConnectionParameters()
	{
		connectionParameterMappings.MapParameters(nameValueCollection, trackingConnectionParameters);
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
