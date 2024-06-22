using Dataedo.App.Import.CloudStorage;
using Dataedo.App.Import.DataLake;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.Tracking.Helpers;

public static class ImportFileTrackingHelper
{
	public static void TrackImportFileShowForm()
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilder(new TrackingUserParameters(), new TrackingDataedoParameters()), TrackingEventEnum.ImportFileShowForm);
		});
	}

	public static void TrackImportFileConnectorSelected(DataLakeTypeEnum.DataLakeType? dataLakeType)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilderEventSpecificConnector(new TrackingUserParameters(), new TrackingDataedoParameters(), dataLakeType?.ToString()), TrackingEventEnum.ImportFileConnectorSelected);
		});
	}

	public static void TrackImportFileConnectorSelected(SharedDatabaseTypeEnum.DatabaseType? databaseType)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilderEventSpecificConnector(new TrackingUserParameters(), new TrackingDataedoParameters(), databaseType?.ToString()), TrackingEventEnum.ImportFileConnectorSelected);
		});
	}

	public static void TrackImportFileConnectorSelected(FileImportTypeEnum.FileImportType? fileImportType)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilderEventSpecificConnector(new TrackingUserParameters(), new TrackingDataedoParameters(), fileImportType?.ToString()), TrackingEventEnum.ImportFileConnectorSelected);
		});
	}

	public static void TrackImportFileReadSuccess(int objectsCount, string fileSize, DataLakeTypeEnum.DataLakeType? dataLakeType)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilderEventSpecificObjectsCountFileSizeConnector(new TrackingUserParameters(), new TrackingDataedoParameters(), objectsCount.ToString(), fileSize, dataLakeType?.ToString()), TrackingEventEnum.ImportFileReadSuccess);
		});
	}

	public static void TrackImportFileReadFailed(string fileSize, DataLakeTypeEnum.DataLakeType? dataLakeType)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilderEventSpecificFileSizeConnector(new TrackingUserParameters(), new TrackingDataedoParameters(), fileSize, dataLakeType?.ToString()), TrackingEventEnum.ImportFileReadFailed);
		});
	}

	public static void TrackImportFileSaveSuccess(int objectsCount, DataLakeTypeEnum.DataLakeType? dataLakeType)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilderEventSpecificObjectsCountConnector(new TrackingUserParameters(), new TrackingDataedoParameters(), objectsCount.ToString(), dataLakeType?.ToString()), TrackingEventEnum.ImportFileSaveSuccess);
		});
	}
}
