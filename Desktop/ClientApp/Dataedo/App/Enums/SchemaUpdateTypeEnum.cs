namespace Dataedo.App.Enums;

public class SchemaUpdateTypeEnum
{
	public enum SchemaUpdateType
	{
		Import = 0,
		Update = 1,
		Edit = 2
	}

	public static string TypeToString(SchemaUpdateType type)
	{
		return type switch
		{
			SchemaUpdateType.Import => "IMPORT", 
			SchemaUpdateType.Update => "UPDATE", 
			_ => "EDIT", 
		};
	}

	public static SchemaUpdateType StringToType(string type)
	{
		return type switch
		{
			"IMPORT" => SchemaUpdateType.Import, 
			"UPDATE" => SchemaUpdateType.Update, 
			_ => SchemaUpdateType.Edit, 
		};
	}
}
