using Dataedo.App.Enums;

namespace Dataedo.App.Documentation.Template;

public class TemplateTypeEnum
{
	public enum TemplateType
	{
		Undefined = 0,
		PDF = 1,
		HTML = 2,
		Excel = 3
	}

	public static TemplateType StringToType(string value)
	{
		if (value == "PDF")
		{
			return TemplateType.PDF;
		}
		return TemplateType.Undefined;
	}

	public static DocFormatEnum.DocFormat? TypeToDocFormat(TemplateType type)
	{
		return type switch
		{
			TemplateType.Undefined => null, 
			TemplateType.PDF => DocFormatEnum.DocFormat.PDF, 
			TemplateType.HTML => DocFormatEnum.DocFormat.HTML, 
			TemplateType.Excel => DocFormatEnum.DocFormat.Excel, 
			_ => null, 
		};
	}
}
