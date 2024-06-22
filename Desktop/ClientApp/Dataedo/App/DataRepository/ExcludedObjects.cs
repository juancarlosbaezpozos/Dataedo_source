using System.Collections.Generic;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository;

internal class ExcludedObjects
{
	private IList<SharedObjectTypeEnum.ObjectType> types;

	private IDictionary<SharedObjectTypeEnum.ObjectType, IList<SharedObjectTypeEnum.ObjectType>> subtypes;

	private IDictionary<(SharedObjectTypeEnum.ObjectType, SharedObjectTypeEnum.ObjectType), IList<SharedObjectTypeEnum.ObjectType>> subsubtypes;

	private IList<CustomExcludedTypeEnum.CustomExcludedType> customTypes;

	public ExcludedObjects()
	{
		types = new List<SharedObjectTypeEnum.ObjectType>();
		subtypes = new Dictionary<SharedObjectTypeEnum.ObjectType, IList<SharedObjectTypeEnum.ObjectType>>();
		subsubtypes = new Dictionary<(SharedObjectTypeEnum.ObjectType, SharedObjectTypeEnum.ObjectType), IList<SharedObjectTypeEnum.ObjectType>>();
		customTypes = new List<CustomExcludedTypeEnum.CustomExcludedType>();
	}

	public void ExcludeType(SharedObjectTypeEnum.ObjectType type)
	{
		if (!IsExluded(type))
		{
			types.Add(type);
		}
	}

	public void ExcludeType(SharedObjectTypeEnum.ObjectType type, SharedObjectTypeEnum.ObjectType subtype, SharedObjectTypeEnum.ObjectType? subsubtype)
	{
		if (!subsubtype.HasValue)
		{
			ExcludeType(type, subtype);
			return;
		}
		if (!subsubtypes.ContainsKey((type, subtype)))
		{
			subsubtypes.Add((type, subtype), new List<SharedObjectTypeEnum.ObjectType>());
		}
		if (!IsExluded(type, subtype, subsubtype.Value))
		{
			subsubtypes[(type, subtype)].Add(subsubtype.Value);
		}
	}

	public void ExcludeType(SharedObjectTypeEnum.ObjectType type, SharedObjectTypeEnum.ObjectType subtype)
	{
		if (type == subtype)
		{
			ExcludeType(type);
			return;
		}
		if (!subtypes.ContainsKey(type))
		{
			subtypes.Add(type, new List<SharedObjectTypeEnum.ObjectType>());
		}
		if (!IsExluded(type, subtype))
		{
			subtypes[type].Add(subtype);
		}
	}

	public void ExcludeType(CustomExcludedTypeEnum.CustomExcludedType type)
	{
		if (!IsExluded(type))
		{
			customTypes.Add(type);
		}
	}

	public bool IsExluded(SharedObjectTypeEnum.ObjectType type)
	{
		return types.Contains(type);
	}

	public bool IsExluded(SharedObjectTypeEnum.ObjectType type, SharedObjectTypeEnum.ObjectType subtype)
	{
		if (IsExluded(type))
		{
			return true;
		}
		if (subtypes.ContainsKey(type))
		{
			return subtypes[type].Contains(subtype);
		}
		return false;
	}

	public bool IsExluded(SharedObjectTypeEnum.ObjectType type, SharedObjectTypeEnum.ObjectType subtype, SharedObjectTypeEnum.ObjectType subsubtype)
	{
		if (IsExluded(type))
		{
			return true;
		}
		if (IsExluded(type, subtype))
		{
			return true;
		}
		if (subsubtypes.ContainsKey((type, subtype)))
		{
			return subsubtypes[(type, subtype)].Contains(subsubtype);
		}
		return false;
	}

	public bool IsExluded(CustomExcludedTypeEnum.CustomExcludedType type)
	{
		return customTypes.Contains(type);
	}
}
