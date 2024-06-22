using System.Diagnostics;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Forms.Support.DocWizardForm;

[DebuggerDisplay("{ParentObjectTypeString}.{ObjectTypeString}.{ObjectSubtypeString}")]
public class ObjectTypeHierarchy
{
	public SharedObjectTypeEnum.ObjectType? ParentObjectType { get; set; }

	public SharedObjectTypeEnum.ObjectType? ObjectType { get; set; }

	public SharedObjectTypeEnum.ObjectType? ObjectSubtype { get; set; }

	public CustomExcludedTypeEnum.CustomExcludedType? CustomType { get; set; }

	public ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType? objectType, SharedObjectTypeEnum.ObjectType? objectSubtype, SharedObjectTypeEnum.ObjectType? parentObjectType = null)
	{
		ObjectType = objectType;
		ObjectSubtype = objectSubtype;
		ParentObjectType = parentObjectType;
		CustomType = null;
	}

	public ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType objectType)
		: this(objectType, objectType)
	{
	}

	public ObjectTypeHierarchy(CustomExcludedTypeEnum.CustomExcludedType customType)
		: this(null, null)
	{
		CustomType = customType;
	}

	public bool IsType(SharedObjectTypeEnum.ObjectType objectType)
	{
		if (CustomType.HasValue)
		{
			return false;
		}
		if (objectType == ObjectType)
		{
			return objectType == ObjectSubtype;
		}
		return false;
	}

	public bool IsType(SharedObjectTypeEnum.ObjectType objectType, SharedObjectTypeEnum.ObjectType objectSubtype)
	{
		if (CustomType.HasValue)
		{
			return false;
		}
		if (objectType == ObjectType)
		{
			return objectSubtype == ObjectSubtype;
		}
		return false;
	}

	public bool IsType(CustomExcludedTypeEnum.CustomExcludedType objectType)
	{
		if (!CustomType.HasValue)
		{
			return false;
		}
		return objectType == CustomType;
	}
}
