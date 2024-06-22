using System;

namespace Dataedo.App.Tools.ERD.Canvas;

public struct RectangularObjectKey
{
	public int Key { get; set; }

	public RectangularObjectType Type { get; set; }

	public RectangularObjectKey(int id, RectangularObjectType type = RectangularObjectType.Node)
	{
		Key = id;
		Type = type;
	}

	public override bool Equals(object obj)
	{
		if (obj is RectangularObjectKey rectangularObjectKey)
		{
			if (Key == rectangularObjectKey.Key)
			{
				return Type == rectangularObjectKey.Type;
			}
			return false;
		}
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Key.GetHashCode(), Type.GetHashCode());
	}

	public static bool operator ==(RectangularObjectKey left, RectangularObjectKey right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(RectangularObjectKey left, RectangularObjectKey right)
	{
		return !(left == right);
	}
}
