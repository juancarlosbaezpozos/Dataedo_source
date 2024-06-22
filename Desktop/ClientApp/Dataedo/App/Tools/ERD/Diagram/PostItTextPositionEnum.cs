namespace Dataedo.App.Tools.ERD.Diagram;

public class PostItTextPositionEnum
{
	public enum PostItTextPosition
	{
		TopLeft = 0,
		CenterCenter = 1
	}

	public static PostItTextPosition StringToType(string postItTextPositionString)
	{
		if (!(postItTextPositionString == "TOP_LEFT"))
		{
			if (!(postItTextPositionString == "CENTER_CENTER"))
			{
			}
			return PostItTextPosition.CenterCenter;
		}
		return PostItTextPosition.TopLeft;
	}

	public static string TypeToString(PostItTextPosition postItTextPosition)
	{
		return postItTextPosition switch
		{
			PostItTextPosition.TopLeft => "TOP_LEFT", 
			_ => "CENTER_CENTER", 
		};
	}
}
