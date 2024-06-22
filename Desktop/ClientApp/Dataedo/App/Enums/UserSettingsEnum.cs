namespace Dataedo.App.Enums;

public static class UserSettingsEnum
{
	public static string GetSettingsString(UserSettingsType type)
	{
		return type switch
		{
			UserSettingsType.Personal => "Personal", 
			UserSettingsType.Public => "Public", 
			_ => string.Empty, 
		};
	}
}
