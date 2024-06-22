namespace Dataedo.App.UserControls.SchemaImportsAndChanges.Model;

public class SchemaChangeTypeEnum
{
	public enum SchemaChangeType
	{
		NotSet = 0,
		Added = 1,
		Updated = 2,
		Deleted = 3
	}

	public static SchemaChangeType StringToType(string schemaChangeChangeTypeString)
	{
		schemaChangeChangeTypeString = schemaChangeChangeTypeString?.ToLower();
		if (schemaChangeChangeTypeString != null && schemaChangeChangeTypeString.Contains("added"))
		{
			return SchemaChangeType.Added;
		}
		if (schemaChangeChangeTypeString != null && schemaChangeChangeTypeString.Contains("updated"))
		{
			return SchemaChangeType.Updated;
		}
		if (schemaChangeChangeTypeString != null && schemaChangeChangeTypeString.Contains("deleted"))
		{
			return SchemaChangeType.Deleted;
		}
		return SchemaChangeType.NotSet;
	}

	public static string TypeToLowerString(SchemaChangeType schemaChangeChangeType)
	{
		return schemaChangeChangeType switch
		{
			SchemaChangeType.NotSet => string.Empty, 
			SchemaChangeType.Added => "added", 
			SchemaChangeType.Updated => "updated", 
			SchemaChangeType.Deleted => "deleted", 
			_ => string.Empty, 
		};
	}

	public static string TypeToStringForIcon(SchemaChangeType schemaChangeChangeType)
	{
		if (schemaChangeChangeType == SchemaChangeType.Added)
		{
			return "new";
		}
		return TypeToLowerString(schemaChangeChangeType);
	}
}
