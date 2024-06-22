using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;

namespace Dataedo.App.Tools.Tracking.Helpers;

public static class ClassificationTrackingHelper
{
	public static void TrackClassificationAdded()
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilder(new TrackingUserParameters(), new TrackingDataedoParameters()), TrackingEventEnum.ClassificationAdded);
		});
	}

	public static void TrackClassificationRun(int databasesCount, int columnsCount)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilderEventSpecificDatabasesCountColumnsCount(new TrackingRepoParameters(), new TrackingUserParameters(), new TrackingDataedoParameters(), databasesCount, columnsCount), TrackingEventEnum.ClassificationRun);
		});
	}

	public static void TrackClassificationFailed(int databasesCount, int columnsCount)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilderEventSpecificDatabasesCountColumnsCount(new TrackingUserParameters(), new TrackingDataedoParameters(), databasesCount, columnsCount), TrackingEventEnum.ClassificationFailed);
		});
	}

	public static void TrackClassificationCanceled(int databasesCount, int columnsCount)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilderEventSpecificDatabasesCountColumnsCount(new TrackingUserParameters(), new TrackingDataedoParameters(), databasesCount, columnsCount), TrackingEventEnum.ClassificationCanceled);
		});
	}

	public static void TrackClassificationSave(int columnsCount)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilderEventSpecificColumnsCount(new TrackingUserParameters(), new TrackingDataedoParameters(), columnsCount), TrackingEventEnum.ClassificationSave);
		});
	}

	public static void TrackClassificationSavingCanceled(int columnsCount)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilderEventSpecificColumnsCount(new TrackingUserParameters(), new TrackingDataedoParameters(), columnsCount), TrackingEventEnum.ClassificationSavingCanceled);
		});
	}
}
