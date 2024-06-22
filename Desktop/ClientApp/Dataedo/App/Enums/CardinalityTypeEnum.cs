using System.Drawing;
using Dataedo.App.Properties;

namespace Dataedo.App.Enums;

public class CardinalityTypeEnum
{
	public enum CardinalityType
	{
		Many = 1,
		ZeroOrMany = 2,
		OneOrMany = 3,
		One = 4,
		ZeroOrOne = 5,
		ExactlyOne = 6
	}

	public static Image TypeToImage(CardinalityType? cardinalityType)
	{
		return cardinalityType switch
		{
			CardinalityType.Many => Resources.relation_mx_1x_16, 
			CardinalityType.One => Resources.relation_1x_1x_16, 
			_ => null, 
		};
	}

	public static CardinalityType? ToTypeGeneral(CardinalityType? cardinalityType)
	{
		return cardinalityType switch
		{
			CardinalityType.Many => CardinalityType.Many, 
			CardinalityType.ZeroOrMany => CardinalityType.ZeroOrMany, 
			CardinalityType.OneOrMany => CardinalityType.OneOrMany, 
			CardinalityType.One => CardinalityType.One, 
			CardinalityType.ZeroOrOne => CardinalityType.ZeroOrOne, 
			CardinalityType.ExactlyOne => CardinalityType.ExactlyOne, 
			_ => null, 
		};
	}

	public static CardinalityType? StringToType(string cardinalityTypeString)
	{
		return cardinalityTypeString switch
		{
			"MANY" => CardinalityType.Many, 
			"ZERO_MANY" => CardinalityType.ZeroOrMany, 
			"ONE_MANY" => CardinalityType.OneOrMany, 
			"ONE" => CardinalityType.One, 
			"ZERO_ONE" => CardinalityType.ZeroOrOne, 
			"EXACTLY_ONE" => CardinalityType.ExactlyOne, 
			_ => null, 
		};
	}

	public static string TypeToString(CardinalityType? cardinalityType)
	{
		return cardinalityType switch
		{
			CardinalityType.Many => "MANY", 
			CardinalityType.ZeroOrMany => "ZERO_MANY", 
			CardinalityType.OneOrMany => "ONE_MANY", 
			CardinalityType.One => "ONE", 
			CardinalityType.ZeroOrOne => "ZERO_ONE", 
			CardinalityType.ExactlyOne => "EXACTLY_ONE", 
			_ => null, 
		};
	}

	public static string TypeToStringForDisplay(CardinalityType? cardinalityType)
	{
		return cardinalityType switch
		{
			CardinalityType.Many => "Many", 
			CardinalityType.ZeroOrMany => "Zero or many", 
			CardinalityType.OneOrMany => "One or many", 
			CardinalityType.One => "One", 
			CardinalityType.ZeroOrOne => "Zero or one", 
			CardinalityType.ExactlyOne => "Exactly one", 
			_ => null, 
		};
	}

	public static string TypeToId(CardinalityType? cardinalityType)
	{
		return cardinalityType switch
		{
			CardinalityType.Many => "mx", 
			CardinalityType.ZeroOrMany => "m0", 
			CardinalityType.OneOrMany => "m1", 
			CardinalityType.One => "1x", 
			CardinalityType.ZeroOrOne => "10", 
			CardinalityType.ExactlyOne => "1e", 
			_ => null, 
		};
	}
}
