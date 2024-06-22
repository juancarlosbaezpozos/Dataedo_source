using System;
using System.Globalization;
using System.Threading;

namespace Dataedo.App.DataProfiling.Tools;

public static class DataProfilingStringFormatter
{
	private static CultureInfo dataprofilingCulture;

	public static string FormatTopIntValues(long number)
	{
		return number.ToString("#,0", dataprofilingCulture);
	}

	public static string FormatFloat1Values(double? number)
	{
		if (!number.HasValue)
		{
			return string.Empty;
		}
		return number.GetValueOrDefault().ToString("#,0.0", dataprofilingCulture);
	}

	public static string FormatFloat2Values(double? number)
	{
		if (!number.HasValue)
		{
			return string.Empty;
		}
		return number.GetValueOrDefault().ToString("#,0.00", dataprofilingCulture);
	}

	public static string Format2FloatInteligentValues(double? number)
	{
		if (!number.HasValue)
		{
			return string.Empty;
		}
		string text = number.GetValueOrDefault().ToString("#,0.00", dataprofilingCulture);
		if (text.EndsWith("00", StringComparison.CurrentCulture))
		{
			return text = number.GetValueOrDefault().ToString("#,0", dataprofilingCulture);
		}
		if (text.EndsWith("0", StringComparison.CurrentCulture))
		{
			return text = number.GetValueOrDefault().ToString("#,0", dataprofilingCulture);
		}
		return text;
	}

	public static void SetDataProfilingCulture()
	{
		CultureInfo cultureInfo = new CultureInfo("EN-US");
		cultureInfo.DateTimeFormat.LongDatePattern = "yyyy-MM-dd HH:mm:ss";
		cultureInfo.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
		cultureInfo.NumberFormat.NumberGroupSeparator = ",";
		Thread.CurrentThread.CurrentCulture = cultureInfo;
		Thread.CurrentThread.CurrentUICulture = cultureInfo;
		dataprofilingCulture = cultureInfo;
	}

	public static string FormatDateToRepositoryFormat(DateTime? date, string datetimeType = "DATETIME")
	{
		if (!date.HasValue)
		{
			return string.Empty;
		}
		if (datetimeType == "DATETIME")
		{
			return date?.ToString("yyyy-MM-dd HH:mm:ss", dataprofilingCulture);
		}
		return date?.ToString("yyyy-MM-dd", dataprofilingCulture);
	}
}
