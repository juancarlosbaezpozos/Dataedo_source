using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;
using Dataedo.App.Tools.Tracking.Models;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoConnectionBuilderEventSpecificTime : ParametersWithUserDataedoConnectionBuilder
{
	private readonly string time;

	public ParametersWithUserDataedoConnectionBuilderEventSpecificTime(TrackingConnectionParameters trackingConnectionParameters, ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, string time)
		: base(trackingConnectionParameters, trackingUserParameters, trackingDataedoParameters)
	{
		this.time = time;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetTime(nameValueCollection, time);
		return this;
	}
}
