namespace Dataedo.App.Tools.ERD.Diagram;

public class PostItLayerEnum
{
	public enum PostItLayer
	{
		Front = 0,
		Back = 1
	}

	public static PostItLayer IntToType(int value)
	{
		return value switch
		{
			0 => PostItLayer.Back, 
			_ => PostItLayer.Front, 
		};
	}

	public static int TypeToInt(PostItLayer value)
	{
		if (value != 0 && value == PostItLayer.Back)
		{
			return 0;
		}
		return 100;
	}
}
