using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoBuilderEventSpecificConnector : ParametersWithUserDataedoBuilder
{
	private readonly string connector;

	public ParametersWithUserDataedoBuilderEventSpecificConnector(ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, string connector)
		: base(trackingUserParameters, trackingDataedoParameters)
	{
		this.connector = connector;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetConnector(nameValueCollection, connector);
		return this;
	}
}
