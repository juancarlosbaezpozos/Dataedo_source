using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class ObjectTypeCount
{
	public SharedObjectTypeEnum.ObjectType ObjectType { get; set; }

	public int Counter { get; set; }

	public ObjectTypeCount(SharedObjectTypeEnum.ObjectType objectType, int counter)
	{
		ObjectType = objectType;
		Counter = counter;
	}
}
