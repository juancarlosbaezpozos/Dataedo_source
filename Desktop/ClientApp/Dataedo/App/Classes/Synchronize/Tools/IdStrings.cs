using Dataedo.App.Enums;
using Dataedo.App.Tools;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize.Tools;

public static class IdStrings
{
	public static string GetDocumentationIdString(string name, int? id)
	{
		return Paths.RemoveInvalidFilePathCharacters($"{name.Replace(' ', '_')}_{id}", "_");
	}

	public static string GetObjectIdString(SharedDatabaseTypeEnum.DatabaseType? databaseType, bool? isMultipleSchemasDatabase, string schema, string name, int? id)
	{
		bool? flag = DatabaseTypeEnum.IsSchemaType(databaseType);
		string text = null;
		text = ((flag == true || isMultipleSchemasDatabase == true) ? (schema + "_" + name) : ((flag != false) ? (string.IsNullOrEmpty(schema) ? name : (schema + "_" + name)) : (name ?? string.Empty)));
		return Paths.EncodeInvalidPathCharacters(text) + $"_{id ?? (-1)}";
	}
}
