using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoRepoBuilderWithEventSpecificTime : ParametersWithUserDataedoRepoBuilder
{
	private readonly string time;

	public ParametersWithUserDataedoRepoBuilderWithEventSpecificTime(ITrackingRepoParameters trackingRepoParameters, ITrackingDataedoParameters trackingDataedoParameters, ITrackingUserParameters trackingUserParameters, string time)
		: base(trackingRepoParameters, trackingDataedoParameters, trackingUserParameters)
	{
		this.time = time;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetTime(nameValueCollection, time);
		return this;
	}
}
