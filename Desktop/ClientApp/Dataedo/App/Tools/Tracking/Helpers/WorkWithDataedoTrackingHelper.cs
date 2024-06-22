using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;

namespace Dataedo.App.Tools.Tracking.Helpers;

public static class WorkWithDataedoTrackingHelper
{
	private static void TrackEventWithUserAndDataedoParameters(TrackingEventEnum trackingEvent, bool trackOnlyFirstInSession = false)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilder(new TrackingUserParameters(), new TrackingDataedoParameters()), trackingEvent, trackOnlyFirstInSession);
		});
	}

	public static void TrackNewSubjectAreaAdd()
	{
		TrackEventWithUserAndDataedoParameters(TrackingEventEnum.NewSubjectAreaAdd);
	}

	public static void TrackNewManualDatabaseAdd()
	{
		TrackEventWithUserAndDataedoParameters(TrackingEventEnum.NewManualDatabaseAdd);
	}

	public static void TrackCustomFieldsSave()
	{
		TrackEventWithUserAndDataedoParameters(TrackingEventEnum.CustomFieldsSave);
	}

	public static void TrackFirstInSessionSCTView()
	{
		TrackEventWithUserAndDataedoParameters(TrackingEventEnum.FirstInSessionSCTView, trackOnlyFirstInSession: true);
	}

	public static void TrackFirstInSessionERDiagramEdit()
	{
		TrackEventWithUserAndDataedoParameters(TrackingEventEnum.FirstInSessionERDiagramEdit, trackOnlyFirstInSession: true);
	}

	public static void TrackFirstInSessionERDiagramView()
	{
		TrackEventWithUserAndDataedoParameters(TrackingEventEnum.FirstInSessionERDiagramView, trackOnlyFirstInSession: true);
	}

	public static void TrackFirstInSessionTermView()
	{
		TrackEventWithUserAndDataedoParameters(TrackingEventEnum.FirstInSessionTermView, trackOnlyFirstInSession: true);
	}

	public static void TrackFirstInSessionTermEdit()
	{
		TrackEventWithUserAndDataedoParameters(TrackingEventEnum.FirstInSessionTermEdit, trackOnlyFirstInSession: true);
	}

	public static void TrackFirstInSessionTermAdd()
	{
		TrackEventWithUserAndDataedoParameters(TrackingEventEnum.FirstInSessionTermAdd, trackOnlyFirstInSession: true);
	}

	public static void TrackNewGlossaryAdd()
	{
		TrackEventWithUserAndDataedoParameters(TrackingEventEnum.NewGlossaryAdd);
	}

	public static void TrackFirstInSessionObjectsListView()
	{
		TrackEventWithUserAndDataedoParameters(TrackingEventEnum.FirstInSessionObjectsListView, trackOnlyFirstInSession: true);
	}

	public static void TrackFirstInSessionObjectView()
	{
		TrackEventWithUserAndDataedoParameters(TrackingEventEnum.FirstInSessionObjectView, trackOnlyFirstInSession: true);
	}

	public static void TrackFirstInSessionObjectEdit()
	{
		TrackEventWithUserAndDataedoParameters(TrackingEventEnum.FirstInSessionObjectEdit, trackOnlyFirstInSession: true);
	}

	public static void TrackFirstInSessionObjectAdd()
	{
		TrackEventWithUserAndDataedoParameters(TrackingEventEnum.FirstInSessionObjectAdd, trackOnlyFirstInSession: true);
	}

	public static void TrackFirstInSessionRelationSave()
	{
		TrackEventWithUserAndDataedoParameters(TrackingEventEnum.FirstInSessionRelationSave, trackOnlyFirstInSession: true);
	}

	public static void TrackFirstInSessionSearch()
	{
		TrackEventWithUserAndDataedoParameters(TrackingEventEnum.FirstInSessionSearch, trackOnlyFirstInSession: true);
	}
}
