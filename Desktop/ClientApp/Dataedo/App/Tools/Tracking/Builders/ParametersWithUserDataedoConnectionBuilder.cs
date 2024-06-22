using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;
using Dataedo.App.Tools.Tracking.Models;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoConnectionBuilder : TrackingParametersBuilder
{
	private readonly TrackingConnectionParameters trackingConnectionParameters;

	private readonly ITrackingUserParameters trackingUserParameters;

	private readonly ITrackingDataedoParameters trackingDataedoParameters;

	private readonly DataedoParameterMappings dataedoParameterMappings;

	private readonly UserParameterMappings userParameterMappings;

	private readonly ConnectionParameterMappings connectionParameterMappings;

	public ParametersWithUserDataedoConnectionBuilder(TrackingConnectionParameters trackingConnectionParameters, ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters)
	{
		this.trackingConnectionParameters = trackingConnectionParameters;
		this.trackingUserParameters = trackingUserParameters;
		this.trackingDataedoParameters = trackingDataedoParameters;
		connectionParameterMappings = new ConnectionParameterMappings();
		dataedoParameterMappings = new DataedoParameterMappings();
		userParameterMappings = new UserParameterMappings();
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
		return this;
	}

	public override TrackingParametersBuilder BuildUserParameters()
	{
		userParameterMappings.MapParameters(nameValueCollection, trackingUserParameters);
		return this;
	}
}
