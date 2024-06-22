using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoRepoBuilderEventSpecificScopeTime : ParametersWithUserDataedoRepoBuilder
{
	private readonly string scope;

	private readonly string time;

	public ParametersWithUserDataedoRepoBuilderEventSpecificScopeTime(ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, ITrackingRepoParameters trackingRepoParameters, string scope, string time)
		: base(trackingRepoParameters, trackingDataedoParameters, trackingUserParameters)
	{
		this.scope = scope;
		this.time = time;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetScope(nameValueCollection, scope);
		SpecificParametersMapper.GetTime(nameValueCollection, time);
		return this;
	}
}
