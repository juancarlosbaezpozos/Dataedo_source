using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DataProfiling.Enums;
using Dataedo.App.DataProfiling.Models;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;

namespace Dataedo.App.DataProfiling.Tools;

public static class DataProfilingTrackingHelper
{
	public const string All = "ALL";

	public const string Distribution = "DISTRIBUTION";

	public const string Values = "VALUES";

	public const string FAILED = "FAILED";

	public const string FINISHED = "FINISHED";

	public const string CANCELED = "CANCELED";

	public static string GetProfilingScope(ProfilingType profilingType)
	{
		return profilingType switch
		{
			ProfilingType.Full => "ALL", 
			ProfilingType.Distribution => "DISTRIBUTION", 
			ProfilingType.Values => "VALUES", 
			_ => null, 
		};
	}

	public static void TrackDataProfilingSingleTableOpen()
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingRepoParameters(), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.DataProfilingSingleTableOpen);
		});
	}

	public static void TrackDataProfilingMultipleTableOpen(int tables)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilderEventSpecificTables(new TrackingUserParameters(), new TrackingDataedoParameters(), new TrackingRepoParameters(), tables.ToString()), TrackingEventEnum.DataProfilingMultipleTablesOpen);
		});
	}

	public static void TrackDataProfilingStarted(DatabaseRow connectedDatabaseRow, string scope, int tablesToProfile, int columnsToProfile)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoConnectionBuilderEventSpecificScopeTablesColumns(new TrackingConnectionParameters(connectedDatabaseRow, connectedDatabaseRow.Type, SSLTypeHelper.GetSelectedSSLType(connectedDatabaseRow), ConnectionTypeService.GetConnectionType(connectedDatabaseRow)), new TrackingUserParameters(), new TrackingDataedoParameters(), new TrackingRepoParameters(), scope, tablesToProfile.ToString(), columnsToProfile.ToString()), TrackingEventEnum.DataProfilingStarted);
		});
	}

	public static void TrackDataProfilingFinished(string status, int profiledTables, int profiledColumns, int successfullyProfiledTables, int successfullyProfiledColumns, string datatypes, string duration)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilderEventSpecificStatusTablesColumnsDatatypesTime(new TrackingUserParameters(), new TrackingDataedoParameters(), new TrackingRepoParameters(), status, profiledTables.ToString(), profiledColumns.ToString(), successfullyProfiledTables.ToString(), successfullyProfiledColumns.ToString(), datatypes, duration.ToString()), TrackingEventEnum.DataProfilingFinished);
		});
	}

	public static void TrackDataProfilingSaved(string scope, string duration)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilderEventSpecificScopeTime(new TrackingUserParameters(), new TrackingDataedoParameters(), new TrackingRepoParameters(), scope, duration.ToString()), TrackingEventEnum.DataProfilingSaved);
		});
	}

	public static void TrackDataProfilingCleared(string scope, string duration)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilderEventSpecificScopeTime(new TrackingUserParameters(), new TrackingDataedoParameters(), new TrackingRepoParameters(), scope, duration.ToString()), TrackingEventEnum.DataProfilingCleared);
		});
	}

	public static void TrackSampleData(DatabaseRow connectedDatabaseRow)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoConnectionBuilder(new TrackingConnectionParameters(connectedDatabaseRow, connectedDatabaseRow.Type, SSLTypeHelper.GetSelectedSSLType(connectedDatabaseRow.Type, connectedDatabaseRow.SSLType, connectedDatabaseRow.SSLSettings), ConnectionTypeService.GetConnectionType(connectedDatabaseRow.Type, connectedDatabaseRow.ConnectionType, connectedDatabaseRow.SelectedAuthenticationType, connectedDatabaseRow.GeneralConnectionType)), new TrackingUserParameters(), new TrackingDataedoParameters(), new TrackingRepoParameters()), TrackingEventEnum.DataProfilingSampleData);
		});
	}

	public static string GetProfliledColumnsDatatypesJson(IEnumerable<ColumnNavigationObject> columnsToProfile)
	{
		IEnumerable<IGrouping<string, ColumnNavigationObject>> enumerable = columnsToProfile?.GroupBy((ColumnNavigationObject c) => c.DataType);
		if (enumerable == null)
		{
			return null;
		}
		List<ProfiledTypeCounter> list = new List<ProfiledTypeCounter>();
		foreach (IGrouping<string, ColumnNavigationObject> item in enumerable)
		{
			list.Add(new ProfiledTypeCounter
			{
				Datatype = item.Key,
				Counter = item.Count()
			});
		}
		return JsonSerializer.Serialize(list);
	}
}
