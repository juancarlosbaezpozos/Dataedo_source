namespace Dataedo.App.Tools.UI;

internal class Escaping
{
	public static string EscapeTextForUI(string value)
	{
		return value?.Replace("&", "&&");
	}
}
