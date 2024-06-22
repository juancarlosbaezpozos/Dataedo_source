using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoBuilderEventSpecificHistorySize : ParametersWithUserDataedoBuilder
{
	private readonly string size;

	public ParametersWithUserDataedoBuilderEventSpecificHistorySize(ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, string size)
		: base(trackingUserParameters, trackingDataedoParameters)
	{
		this.size = size;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetHistorySize(nameValueCollection, size);
		return this;
	}
}
