using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoBuilderEventSpecificFileSizeConnector : ParametersWithUserDataedoBuilder
{
	private readonly string fileSize;

	private readonly string connector;

	public ParametersWithUserDataedoBuilderEventSpecificFileSizeConnector(ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, string fileSize, string connector)
		: base(trackingUserParameters, trackingDataedoParameters)
	{
		this.fileSize = fileSize;
		this.connector = connector;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetFileSize(nameValueCollection, fileSize);
		SpecificParametersMapper.GetConnector(nameValueCollection, connector);
		return this;
	}
}
