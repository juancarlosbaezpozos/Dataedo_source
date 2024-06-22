namespace Dataedo.App.Tools;

public static class CheckboxValues
{
	public static bool IsPositive(string data)
	{
		if (string.IsNullOrEmpty(data))
		{
			return false;
		}
		string text = data.ToLower();
		if (!text.Equals("true") && !text.Equals("checked") && !text.Equals("y") && !text.Equals("yes"))
		{
			return text.Equals("1");
		}
		return true;
	}
}
