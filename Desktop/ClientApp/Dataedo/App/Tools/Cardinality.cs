using System.Drawing;
using Dataedo.App.Enums;

namespace Dataedo.App.Tools;

public class Cardinality
{
	public CardinalityTypeEnum.CardinalityType? Type { get; set; }

	public string DisplayName => CardinalityTypeEnum.TypeToStringForDisplay(Type);

	public string Code => CardinalityTypeEnum.TypeToString(Type);

	public string Id => CardinalityTypeEnum.TypeToId(Type);

	public Image Image => CardinalityTypeEnum.TypeToImage(Type);

	public Cardinality()
	{
	}

	public Cardinality(CardinalityTypeEnum.CardinalityType? cardinalityType)
	{
		Type = cardinalityType;
	}

	public Cardinality(bool isPk)
	{
		Type = ((!isPk) ? CardinalityTypeEnum.CardinalityType.Many : CardinalityTypeEnum.CardinalityType.One);
	}
}
