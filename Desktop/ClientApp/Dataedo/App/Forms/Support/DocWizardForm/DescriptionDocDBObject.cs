using Dataedo.Shared.Enums;

namespace Dataedo.App.Forms.Support.DocWizardForm;

public class DescriptionDocDBObject : DocDBObject
{
	public override string Name => GetName(base.ObjectType.ObjectType, base.ObjectType.ObjectSubtype);

	public DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType objectType, SharedObjectTypeEnum.ObjectType objectSubtype, int parentId = 0, bool isCheckedByDefault = true)
		: base(objectType, objectSubtype, parentId, isCheckedByDefault, null, true)
	{
	}

	private string GetName(SharedObjectTypeEnum.ObjectType? type, SharedObjectTypeEnum.ObjectType? subtype)
	{
		if (type != SharedObjectTypeEnum.ObjectType.UnresolvedEntity)
		{
			return $"{subtype} descriptions";
		}
		return $"{subtype}s";
	}
}
