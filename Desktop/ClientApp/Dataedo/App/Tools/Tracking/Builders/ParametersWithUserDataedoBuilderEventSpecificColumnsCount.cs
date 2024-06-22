using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoBuilderEventSpecificColumnsCount : ParametersWithUserDataedoBuilder
{
	private readonly int columnsCount;

	public ParametersWithUserDataedoBuilderEventSpecificColumnsCount(ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, int columnsCount)
		: base(trackingUserParameters, trackingDataedoParameters)
	{
		this.columnsCount = columnsCount;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetColumnsCount(nameValueCollection, columnsCount.ToString());
		return this;
	}
}
