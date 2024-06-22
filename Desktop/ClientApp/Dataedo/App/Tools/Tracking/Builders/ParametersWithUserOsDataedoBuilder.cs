using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;
using Dataedo.App.Tools.Tracking.Models;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserOsDataedoBuilder : TrackingParametersBuilder
{
	private readonly DataedoParameterMappings dataedoParameterMappings;

	private readonly OSParameterMappings osParameterMappings;

	private readonly UserParameterMappings userParameterMappings;

	private readonly TrackingOSParameters trackingOSParameters;

	private readonly ITrackingDataedoParameters trackingDataedoParameters;

	private readonly ITrackingUserParameters trackingUserParameters;

	public ParametersWithUserOsDataedoBuilder(TrackingOSParameters trackingOSParameters, ITrackingDataedoParameters trackingDataedoParameters, ITrackingUserParameters trackingUserParameters)
	{
		this.trackingOSParameters = trackingOSParameters;
		this.trackingDataedoParameters = trackingDataedoParameters;
		this.trackingUserParameters = trackingUserParameters;
		dataedoParameterMappings = new DataedoParameterMappings();
		osParameterMappings = new OSParameterMappings();
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

	public override TrackingParametersBuilder BuildOSParameters()
	{
		osParameterMappings.MapParameters(nameValueCollection, trackingOSParameters);
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

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		return this;
	}
}
