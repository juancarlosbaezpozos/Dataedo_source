using System.ComponentModel;

namespace Dataedo.App.UserControls.ImportFilter;

public class FilterObjectTypeEnum
{
	public enum FilterObjectType
	{
		[Description("All classes")]
		Any = 0,
		[Description("Table")]
		Table = 1,
		[Description("View")]
		View = 2,
		[Description("Procedure")]
		Procedure = 3,
		[Description("Function")]
		Function = 4
	}

	public static FilterObjectType StringToType(string type)
	{
		return type switch
		{
			"TABLE" => FilterObjectType.Table, 
			"VIEW" => FilterObjectType.View, 
			"FUNCTION" => FilterObjectType.Function, 
			"PROCEDURE" => FilterObjectType.Procedure, 
			"TRIGGER" => FilterObjectType.Any, 
			_ => FilterObjectType.Any, 
		};
	}
}
