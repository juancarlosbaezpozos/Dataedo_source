using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoBuilderEventSpecificObjectsCountConnector : ParametersWithUserDataedoBuilder
{
	private readonly string objectsCount;

	private readonly string connector;

	public ParametersWithUserDataedoBuilderEventSpecificObjectsCountConnector(ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, string objectsCount, string connector)
		: base(trackingUserParameters, trackingDataedoParameters)
	{
		this.objectsCount = objectsCount;
		this.connector = connector;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetObjectCounts(nameValueCollection, objectsCount);
		SpecificParametersMapper.GetConnector(nameValueCollection, connector);
		return this;
	}
}
