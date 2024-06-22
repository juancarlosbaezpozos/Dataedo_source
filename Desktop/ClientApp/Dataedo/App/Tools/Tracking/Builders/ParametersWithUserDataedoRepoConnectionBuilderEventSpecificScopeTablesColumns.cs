using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;
using Dataedo.App.Tools.Tracking.Models;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoRepoConnectionBuilderEventSpecificScopeTablesColumns : ParametersWithUserDataedoRepoConnectionBuilder
{
	private readonly string scope;

	private readonly string tables;

	private readonly string columns;

	public ParametersWithUserDataedoRepoConnectionBuilderEventSpecificScopeTablesColumns(TrackingConnectionParameters trackingConnectionParameters, ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, ITrackingRepoParameters trackingRepoParameters, string scope, string tables, string columns)
		: base(trackingConnectionParameters, trackingUserParameters, trackingDataedoParameters, trackingRepoParameters)
	{
		this.scope = scope;
		this.tables = tables;
		this.columns = columns;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetScope(nameValueCollection, scope);
		SpecificParametersMapper.GetTables(nameValueCollection, tables);
		SpecificParametersMapper.GetColumns(nameValueCollection, columns);
		return this;
	}
}
