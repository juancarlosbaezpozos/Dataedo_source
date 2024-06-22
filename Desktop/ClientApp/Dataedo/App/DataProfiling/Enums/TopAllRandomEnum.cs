namespace Dataedo.App.DataProfiling.Enums;

public static class TopAllRandomEnum
{
	public enum TopAllRandom
	{
		Top = 0,
		All = 1,
		Random = 2
	}

	public const string TOP = "T";

	public const string ALL = "R";

	public const string RANDOM = "S";

	public static TopAllRandom? GetEnumValue(string value)
	{
		return value switch
		{
			"T" => TopAllRandom.Top, 
			"R" => TopAllRandom.All, 
			"S" => TopAllRandom.Random, 
			_ => null, 
		};
	}

	public static string GetStringValue(TopAllRandom? enumValue)
	{
		return enumValue switch
		{
			TopAllRandom.Top => "T", 
			TopAllRandom.All => "R", 
			TopAllRandom.Random => "S", 
			_ => null, 
		};
	}

	public static string GetDisplayText(string valuesListMode)
	{
		return valuesListMode switch
		{
			"T" => "Top", 
			"R" => "All", 
			"S" => "Random", 
			_ => string.Empty, 
		};
	}
}
