using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoBuilderEventSpecificObjectTypeColumnsCount : ParametersWithUserDataedoBuilderEventSpecificObjectType
{
	private readonly int columnsCount;

	public ParametersWithUserDataedoBuilderEventSpecificObjectTypeColumnsCount(ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, string objectType, int columnsCount)
		: base(trackingUserParameters, trackingDataedoParameters, objectType)
	{
		this.columnsCount = columnsCount;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		base.BuildEventSpecificParameters();
		SpecificParametersMapper.GetColumnsCount(nameValueCollection, columnsCount.ToString());
		return this;
	}
}
