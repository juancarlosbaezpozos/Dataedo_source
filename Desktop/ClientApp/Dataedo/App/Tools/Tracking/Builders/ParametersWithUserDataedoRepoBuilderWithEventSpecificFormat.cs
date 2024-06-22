using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoRepoBuilderWithEventSpecificFormat : ParametersWithUserDataedoRepoBuilder
{
	private readonly string format;

	public ParametersWithUserDataedoRepoBuilderWithEventSpecificFormat(ITrackingRepoParameters trackingRepoParameters, ITrackingDataedoParameters trackingDataedoParameters, ITrackingUserParameters trackingUserParameters, string format)
		: base(trackingRepoParameters, trackingDataedoParameters, trackingUserParameters)
	{
		this.format = format;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetFormat(nameValueCollection, format);
		return this;
	}
}
