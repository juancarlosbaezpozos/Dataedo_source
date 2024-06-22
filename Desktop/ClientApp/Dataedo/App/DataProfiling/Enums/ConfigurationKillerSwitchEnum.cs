namespace Dataedo.App.DataProfiling.Enums;

public class ConfigurationKillerSwitchEnum
{
	public enum EnabledNoSaveEnum
	{
		ENABLED = 0,
		NOSAVE = 1
	}

	public const string ENABLED = "ENABLED";

	public const string NOSAVE = "NO_SAVE";

	public const string DISABLED = "DISABLED";

	public static EnabledNoSaveEnum? GetEnumValue(string value)
	{
		if (!(value == "ENABLED"))
		{
			if (value == "NO_SAVE")
			{
				return EnabledNoSaveEnum.NOSAVE;
			}
			return null;
		}
		return EnabledNoSaveEnum.ENABLED;
	}

	public static string GetValue(EnabledNoSaveEnum? enumValue)
	{
		return enumValue switch
		{
			EnabledNoSaveEnum.ENABLED => "ENABLED", 
			EnabledNoSaveEnum.NOSAVE => "NO_SAVE", 
			_ => null, 
		};
	}
}
