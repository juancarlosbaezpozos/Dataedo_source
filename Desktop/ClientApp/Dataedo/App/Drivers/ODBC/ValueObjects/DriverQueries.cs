using System;
using Newtonsoft.Json;

namespace Dataedo.App.Drivers.ODBC.ValueObjects;

internal class DriverQueries : ICloneable
{
	[JsonProperty("count_tables_query")]
	public string CountTablesQuery { get; set; }

	[JsonProperty("count_views_query")]
	public string CountViewsQuery { get; set; }

	[JsonProperty("count_procedures_query")]
	public string CountProceduresQuery { get; set; }

	[JsonProperty("count_functions_query")]
	public string CountFunctionsQuery { get; set; }

	[JsonProperty("get_tables_query")]
	public string GetTablesQuery { get; set; }

	[JsonProperty("get_views_query")]
	public string GetViewsQuery { get; set; }

	[JsonProperty("get_procedures_query")]
	public string GetProceduresQuery { get; set; }

	[JsonProperty("get_functions_query")]
	public string GetFunctionsQuery { get; set; }

	[JsonProperty("relations_query")]
	public string RelationsQuery { get; set; }

	[JsonProperty("columns_query")]
	public string ColumnsQuery { get; set; }

	[JsonProperty("triggers_query")]
	public string TriggersQuery { get; set; }

	[JsonProperty("unique_constraints_query")]
	public string UniqueConstraintsQuery { get; set; }

	[JsonProperty("parameters_query")]
	public string ParametersQuery { get; set; }

	[JsonProperty("dependencies_query")]
	public string DependenciesQuery { get; set; }

	public object Clone()
	{
		return MemberwiseClone();
	}
}
