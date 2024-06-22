using System;

namespace Dataedo.App.LoginFormTools.Tools;

public static class EndDateSuffixService
{
	public static string GetEndDateSuffix(DateTime dateTime, DateTime? utcNow = null)
	{
		utcNow = utcNow ?? DateTime.UtcNow;
		TimeSpan timeSpan = dateTime - utcNow.Value;
		string arg = ((timeSpan.Days > 1 || timeSpan.Days < -1) ? "days" : "day");
		if (timeSpan.Days > 0)
		{
			return $"{timeSpan.Days} {arg} left";
		}
		if (timeSpan.Days == 0 && timeSpan.Ticks > 0)
		{
			return "expires today";
		}
		if (timeSpan.Days == 0 && timeSpan.Ticks < 0)
		{
			return "expired today";
		}
		return $"expired {Math.Abs(timeSpan.Days)} {arg} ago";
	}
}
