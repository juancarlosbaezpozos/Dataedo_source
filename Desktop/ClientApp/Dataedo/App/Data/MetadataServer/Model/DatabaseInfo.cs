using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer.Model;

public class DatabaseInfo
{
	private IEnumerable<string> schemas;

	public SharedDatabaseTypeEnum.DatabaseType? DatabaseType { get; private set; }

	public int DocumentationId { get; private set; }

	public string Server { get; private set; }

	public string Database { get; private set; }

	public string Schema { get; private set; }

	public bool? HasMultipleSchemas { get; private set; }

	public bool? ShowSchema { get; set; }

	public bool? ShowSchemaOverride { get; set; }

	public string Title { get; private set; }

	public string ServerLower => Server?.ToLower();

	public string DatabaseLower => Database?.ToLower();

	public string DatabaseOrSchema
	{
		get
		{
			if (HasMultipleSchemas == true)
			{
				return Schema;
			}
			return Database;
		}
	}

	public string DatabaseOrSchemaLower => DatabaseOrSchema?.ToLower();

	public virtual string DatabaseIdString => Paths.RemoveInvalidFilePathCharacters($"{Title?.Replace(' ', '_')}_{DocumentationId}", "_");

	public DatabaseInfo(int documentationId, SharedDatabaseTypeEnum.DatabaseType? databaseType, string server, string database, string title, bool? hasMultipleSchemas, bool? showSchema, bool? showSchemaOverride, string schema)
	{
		DocumentationId = documentationId;
		DatabaseType = databaseType;
		Server = server;
		Database = database;
		Title = title;
		Schema = schema;
		HasMultipleSchemas = hasMultipleSchemas;
		ShowSchema = showSchema;
		ShowSchemaOverride = showSchemaOverride;
		if (HasMultipleSchemas != true)
		{
			schemas = new List<string>();
		}
		else
		{
			schemas = DatabaseRow.GetSchemasNames(Database);
		}
	}

	public bool IsCurrentDocumentation(string name)
	{
		if (name != null)
		{
			string nameLower = name.ToLower();
			if (HasMultipleSchemas != true)
			{
				return DatabaseOrSchemaLower == nameLower;
			}
			return schemas.Any((string x) => x.ToLower() == nameLower);
		}
		return false;
	}
}
