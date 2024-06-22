using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoBuilderEventSpecificObjectsCountFileSizeConnector : ParametersWithUserDataedoBuilder
{
	private readonly string objectsCount;

	private readonly string fileSize;

	private readonly string connector;

	public ParametersWithUserDataedoBuilderEventSpecificObjectsCountFileSizeConnector(ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, string objectsCount, string fileSize, string connector)
		: base(trackingUserParameters, trackingDataedoParameters)
	{
		this.objectsCount = objectsCount;
		this.fileSize = fileSize;
		this.connector = connector;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetObjectCounts(nameValueCollection, objectsCount);
		SpecificParametersMapper.GetFileSize(nameValueCollection, fileSize);
		SpecificParametersMapper.GetConnector(nameValueCollection, connector);
		return this;
	}
}
