using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.DragDrop;

internal static class MatchingDragDropTypes
{
	public static bool IsValidTermDragDrop(SharedObjectTypeEnum.ObjectType? dragged, SharedObjectTypeEnum.ObjectType? target, SharedObjectTypeEnum.ObjectType? targetParent)
	{
		if (dragged == SharedObjectTypeEnum.ObjectType.Term)
		{
			if (target != SharedObjectTypeEnum.ObjectType.Term)
			{
				return targetParent == SharedObjectTypeEnum.ObjectType.BusinessGlossary;
			}
			return true;
		}
		return false;
	}

	public static bool IsValidTermToObjectDragDrop(SharedObjectTypeEnum.ObjectType? dragged, SharedObjectTypeEnum.ObjectType? target)
	{
		if (dragged == SharedObjectTypeEnum.ObjectType.Term)
		{
			return SharedObjectTypeEnum.IsSupportingBG(target, alsoSubojects: true);
		}
		return false;
	}

	public static bool IsValidObjectToTermDragDrop(SharedObjectTypeEnum.ObjectType? dragged, SharedObjectTypeEnum.ObjectType? target)
	{
		if (SharedObjectTypeEnum.IsSupportingBG(dragged, alsoSubojects: true))
		{
			return target == SharedObjectTypeEnum.ObjectType.Term;
		}
		return false;
	}

	public static bool IsValidObjectToTermDragDrop(SharedObjectTypeEnum.ObjectType? dragged)
	{
		return SharedObjectTypeEnum.IsSupportingBG(dragged, alsoSubojects: true);
	}
}
