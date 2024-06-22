using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;
using Dataedo.App.Tools.Tracking.Models;

namespace Dataedo.App.Tools.Tracking;

public class ParametersWithOsDataedoBuilder : TrackingParametersBuilder
{
	private readonly DataedoParameterMappings dataedoParameterMappings;

	private readonly OSParameterMappings osParameterMappings;

	private readonly TrackingOSParameters trackingOSParameters;

	private readonly ITrackingDataedoParameters trackingDataedoParameters;

	public ParametersWithOsDataedoBuilder(TrackingOSParameters trackingOSParameters, ITrackingDataedoParameters trackingDataedoParameters)
	{
		this.trackingOSParameters = trackingOSParameters;
		this.trackingDataedoParameters = trackingDataedoParameters;
		dataedoParameterMappings = new DataedoParameterMappings();
		osParameterMappings = new OSParameterMappings();
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
		return this;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		return this;
	}
}
