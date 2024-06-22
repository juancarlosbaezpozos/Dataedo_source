namespace Dataedo.App.Documentation.Template;

public class TemplatePDFSubtypeEnum
{
	public enum TemplatePDFSubtype
	{
		Undefined = 0,
		DetailedPrinterFriendly = 1,
		Detailed = 2,
		Overview = 3
	}

	public static TemplatePDFSubtype StringToType(string value)
	{
		return value switch
		{
			"DetailedPrinterFriendly" => TemplatePDFSubtype.DetailedPrinterFriendly, 
			"Detailed" => TemplatePDFSubtype.Detailed, 
			"Overview" => TemplatePDFSubtype.Overview, 
			_ => TemplatePDFSubtype.Undefined, 
		};
	}
}
