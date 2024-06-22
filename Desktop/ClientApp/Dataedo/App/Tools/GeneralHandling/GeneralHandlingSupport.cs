using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Tools.Exceptions;

namespace Dataedo.App.Tools.GeneralHandling;

internal static class GeneralHandlingSupport
{
	private static List<HandlingResult> storedHandlingResults = new List<HandlingResult>();

	public static HandlingMethodEnumeration.HandlingMethod? OverrideHandlingMethod { get; set; } = null;


	public static List<HandlingResult> StoredHandlingResults => storedHandlingResults;

	public static List<HandlingResult> StoredHandlingExceptionResults => storedHandlingResults.Where((HandlingResult x) => x.IsException).ToList();

	public static void ResetOverrideHandlingMethod()
	{
		OverrideHandlingMethod = null;
	}

	public static void ClearStoredHandlingResults()
	{
		storedHandlingResults.Clear();
	}

	public static void StoreResult(HandlingResult result)
	{
		if (!result.IsStored)
		{
			storedHandlingResults.Add(result);
			result.IsStored = true;
		}
	}
}
