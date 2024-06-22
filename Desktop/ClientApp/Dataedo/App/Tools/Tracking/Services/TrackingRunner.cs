using System;

namespace Dataedo.App.Tools.Tracking.Services;

public class TrackingRunner
{
	public static void Track(Action action)
	{
		if (!LastConnectionInfo.LOGIN_INFO.DisableTracking)
		{
			action?.Invoke();
		}
	}
}
