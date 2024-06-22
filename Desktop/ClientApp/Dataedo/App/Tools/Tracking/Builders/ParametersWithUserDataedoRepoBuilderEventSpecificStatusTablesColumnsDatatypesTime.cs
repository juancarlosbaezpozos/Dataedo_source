using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoRepoBuilderEventSpecificStatusTablesColumnsDatatypesTime : ParametersWithUserDataedoRepoBuilder
{
	private readonly string status;

	private readonly string tables;

	private readonly string tables_success;

	private readonly string columns;

	private readonly string columns_success;

	private readonly string datatypes;

	private readonly string time;

	public ParametersWithUserDataedoRepoBuilderEventSpecificStatusTablesColumnsDatatypesTime(ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, ITrackingRepoParameters trackingRepoParameters, string status, string tables, string columns, string tablesSuccess, string columnsSuccess, string datatypes, string time)
		: base(trackingRepoParameters, trackingDataedoParameters, trackingUserParameters)
	{
		this.status = status;
		this.tables = tables;
		this.columns = columns;
		tables_success = tablesSuccess;
		columns_success = columnsSuccess;
		this.datatypes = datatypes;
		this.time = time;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetStatus(nameValueCollection, status);
		SpecificParametersMapper.GetTables(nameValueCollection, tables);
		SpecificParametersMapper.GetColumns(nameValueCollection, columns);
		SpecificParametersMapper.GetTablesSuccess(nameValueCollection, tables_success);
		SpecificParametersMapper.GetColumnsSuccess(nameValueCollection, columns_success);
		SpecificParametersMapper.GetDatatypes(nameValueCollection, datatypes);
		SpecificParametersMapper.GetTime(nameValueCollection, time);
		return this;
	}
}
