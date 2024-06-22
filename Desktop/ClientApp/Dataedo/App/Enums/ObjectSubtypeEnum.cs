using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Enums;

public class ObjectSubtypeEnum : SharedObjectSubtypeEnum
{
	public static Image TypeToImage(SharedObjectTypeEnum.ObjectType type, ObjectSubtype subtype, bool isUserDefined = true)
	{
		string name = (isUserDefined ? (SharedObjectSubtypeEnum.TypeToString(type, subtype).ToLower() + "_user_16") : (SharedObjectSubtypeEnum.TypeToString(type, subtype).ToLower() + "_16"));
		if (!(Resources.ResourceManager.GetObject(name) is Bitmap))
		{
			name = (isUserDefined ? (SharedObjectSubtypeEnum.TypeToString(type, null).ToLower() + "_user_16") : (SharedObjectSubtypeEnum.TypeToString(type, null).ToLower() + "_16"));
		}
		return Resources.ResourceManager.GetObject(name) as Bitmap;
	}

	public new static List<Subtype> GetSubtypes(SharedObjectTypeEnum.ObjectType type)
	{
		List<Subtype> list = new List<Subtype>();
		if (SharedObjectSubtypeEnum.ClassTypeMap.TryGetValue(type, out var value))
		{
			foreach (ObjectSubtype item in value)
			{
				list.Add(new Subtype(type, item));
			}
		}
		else
		{
			list.Add(new Subtype(type, SharedObjectSubtypeEnum.GetDefaultByMainType(type)));
		}
		return list.OrderBy((Subtype x) => x.DisplayName).ToList();
	}
}
