namespace Dataedo.App.Enums;

public class FlowDirectionEnum
{
	public enum Direction
	{
		IN = 1,
		OUT = 2
	}

	public static string TypeToString(Direction? flowDirection)
	{
		if (!flowDirection.HasValue)
		{
			return null;
		}
		return flowDirection.Value switch
		{
			Direction.IN => "IN", 
			Direction.OUT => "OUT", 
			_ => null, 
		};
	}

	public static Direction? StringToType(string flowDirection)
	{
		if (string.IsNullOrEmpty(flowDirection))
		{
			return null;
		}
		if (!(flowDirection == "IN"))
		{
			if (flowDirection == "OUT")
			{
				return Direction.OUT;
			}
			return null;
		}
		return Direction.IN;
	}
}
