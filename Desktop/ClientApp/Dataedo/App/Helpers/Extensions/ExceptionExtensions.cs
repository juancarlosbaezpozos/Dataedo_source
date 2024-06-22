using System;
using System.Collections.Generic;
using System.Text;

namespace Dataedo.App.Helpers.Extensions;

public static class ExceptionExtensions
{
	public static IEnumerable<Exception> GetInnerExceptions(this Exception ex)
	{
		if (ex == null)
		{
			throw new ArgumentNullException("ex");
		}
		Exception innerException = ex;
		do
		{
			yield return innerException;
			innerException = innerException.InnerException;
		}
		while (innerException != null);
	}

	public static string GetInnerExceptionsMessages(this Exception ex)
	{
		IEnumerable<Exception> innerExceptions = ex.GetInnerExceptions();
		if (innerExceptions == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (Exception item in innerExceptions)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine(item.Message);
		}
		return stringBuilder.ToString();
	}
}
