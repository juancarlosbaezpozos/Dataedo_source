using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;

namespace Dataedo.App.Tools.Tracking.Services;

public class TrackingService
{
    //private const string Url = "https://dataedo.com/api/app/tracking";

    //private static Uri Uri = new Uri(Url);

    //private static readonly List<TrackingEventEnum> completedTrackedOnlyFirstInSessionEvents = new List<TrackingEventEnum>();

    public static void MakeAsyncRequest(TrackingParametersBuilder parametersBuilder, TrackingEventEnum trackingEventEnum, bool trackOnlyFirstInSession = false)
    {
        /*if (!completedTrackedOnlyFirstInSessionEvents.Contains(trackingEventEnum))
		{
			NameValueCollection data = parametersBuilder.Build(trackingEventEnum);
			using (WebClient webClient = new WebClient())
			{
				webClient.UploadValuesAsync(Uri, data);
			}
			if (trackOnlyFirstInSession)
			{
				completedTrackedOnlyFirstInSessionEvents.Add(trackingEventEnum);
			}
		}*/
    }
}
