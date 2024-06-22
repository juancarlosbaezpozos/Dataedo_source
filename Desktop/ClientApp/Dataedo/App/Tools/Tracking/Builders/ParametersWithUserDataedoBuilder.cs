using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoBuilder : TrackingParametersBuilder
{
	private readonly ITrackingUserParameters trackingUserParameters;

	private readonly ITrackingDataedoParameters trackingDataedoParameters;

	private readonly DataedoParameterMappings dataedoParameterMappings;

	private readonly UserParameterMappings userParameterMappings;

	public ParametersWithUserDataedoBuilder(ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters)
	{
		this.trackingUserParameters = trackingUserParameters;
		this.trackingDataedoParameters = trackingDataedoParameters;
		dataedoParameterMappings = new DataedoParameterMappings();
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
		return this;
	}

	public override TrackingParametersBuilder BuildUserParameters()
	{
		userParameterMappings.MapParameters(nameValueCollection, trackingUserParameters);
		return this;
	}
}
