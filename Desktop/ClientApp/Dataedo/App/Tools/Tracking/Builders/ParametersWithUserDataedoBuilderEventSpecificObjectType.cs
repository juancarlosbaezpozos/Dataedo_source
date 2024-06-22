using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoBuilderEventSpecificObjectType : ParametersWithUserDataedoBuilder
{
	private readonly string objectType;

	public ParametersWithUserDataedoBuilderEventSpecificObjectType(ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, string objectType)
		: base(trackingUserParameters, trackingDataedoParameters)
	{
		this.objectType = objectType;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetObjectType(nameValueCollection, objectType);
		return this;
	}
}
