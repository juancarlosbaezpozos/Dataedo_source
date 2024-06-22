using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;
using Dataedo.App.Tools.Tracking.Models;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoConnectionBuilderEventSpecificObjectsCount : ParametersWithUserDataedoConnectionBuilder
{
	private readonly string objectsCount;

	public ParametersWithUserDataedoConnectionBuilderEventSpecificObjectsCount(TrackingConnectionParameters trackingConnectionParameters, ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, string objectsCount)
		: base(trackingConnectionParameters, trackingUserParameters, trackingDataedoParameters)
	{
		this.objectsCount = objectsCount;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetObjectCounts(nameValueCollection, objectsCount);
		return this;
	}
}
