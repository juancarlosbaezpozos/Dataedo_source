using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoRepoBuilderEventSpecificTables : ParametersWithUserDataedoRepoBuilder
{
	private readonly string tables;

	public ParametersWithUserDataedoRepoBuilderEventSpecificTables(ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, ITrackingRepoParameters trackingRepoParameters, string tables)
		: base(trackingRepoParameters, trackingDataedoParameters, trackingUserParameters)
	{
		this.tables = tables;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetTables(nameValueCollection, tables);
		return this;
	}
}
