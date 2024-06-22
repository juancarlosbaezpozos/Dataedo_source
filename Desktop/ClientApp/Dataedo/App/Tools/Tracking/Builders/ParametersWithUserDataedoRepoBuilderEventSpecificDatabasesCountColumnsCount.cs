using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoRepoBuilderEventSpecificDatabasesCountColumnsCount : ParametersWithUserDataedoRepoBuilderEventSpecificColumnsCount
{
	private readonly int databasesCount;

	public ParametersWithUserDataedoRepoBuilderEventSpecificDatabasesCountColumnsCount(ITrackingRepoParameters trackingRepoParameters, ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, int databasesCount, int columnsCount)
		: base(trackingRepoParameters, trackingUserParameters, trackingDataedoParameters, columnsCount)
	{
		this.databasesCount = databasesCount;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		base.BuildEventSpecificParameters();
		SpecificParametersMapper.GetDatabasesCount(nameValueCollection, databasesCount.ToString());
		return this;
	}
}
