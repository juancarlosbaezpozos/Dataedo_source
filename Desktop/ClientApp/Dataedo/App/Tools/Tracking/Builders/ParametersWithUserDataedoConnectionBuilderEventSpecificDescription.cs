using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;
using Dataedo.App.Tools.Tracking.Models;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoConnectionBuilderEventSpecificDescription : ParametersWithUserDataedoConnectionBuilder
{
	private readonly string description;

	public ParametersWithUserDataedoConnectionBuilderEventSpecificDescription(TrackingConnectionParameters trackingConnectionParameters, ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, string description)
		: base(trackingConnectionParameters, trackingUserParameters, trackingDataedoParameters)
	{
		this.description = description;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetDescription(nameValueCollection, description);
		return this;
	}
}
