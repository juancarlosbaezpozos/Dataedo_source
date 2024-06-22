using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoRepoBuilderEventSpecificColumnsCount : ParametersWithUserDataedoRepoBuilder
{
	private readonly int columnsCount;

	public ParametersWithUserDataedoRepoBuilderEventSpecificColumnsCount(ITrackingRepoParameters trackingRepoParameters, ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, int columnsCount)
		: base(trackingRepoParameters, trackingDataedoParameters, trackingUserParameters)
	{
		this.columnsCount = columnsCount;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetColumnsCount(nameValueCollection, columnsCount.ToString());
		return this;
	}
}
