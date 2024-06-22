using Dataedo.App.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class SchemaUpdateRow
{
	public SchemaUpdateTypeEnum.SchemaUpdateType Type { get; set; } = SchemaUpdateTypeEnum.SchemaUpdateType.Edit;


	public int? DatabaseId { get; set; }

	public string ConnectionDatabaseType { get; set; }

	public string ConnectionHost { get; set; }

	public string ConnectionUser { get; set; }

	public int? ConnectionPort { get; set; }

	public string ConnectionServiceName { get; set; }

	public string ConnectionDatabaseName { get; set; }

	public string ConnectionDBMSVersion { get; set; }
}
