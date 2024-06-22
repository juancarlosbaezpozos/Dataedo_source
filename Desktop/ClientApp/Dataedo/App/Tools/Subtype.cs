using System.Drawing;
using Dataedo.App.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools;

public class Subtype
{
	public string DisplayName => SharedObjectSubtypeEnum.TypeToStringForSingle(Type, SubType);

	public SharedObjectTypeEnum.ObjectType Type { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype SubType { get; set; }

	public Image Image => ObjectSubtypeEnum.TypeToImage(Type, SubType);

	public Subtype(SharedObjectTypeEnum.ObjectType type, SharedObjectSubtypeEnum.ObjectSubtype subType)
	{
		Type = type;
		SubType = subType;
	}
}
