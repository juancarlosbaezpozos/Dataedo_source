using Amazon.Athena;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

public class AthenaConnectionWrapper
{
	private string database;

	private string workGroup;

	private string dataCatalog;

	private AmazonAthenaClient client;

	public string Database { get; set; }

	public string WorkGroup { get; set; }

	public string DataCatalog { get; set; }

	public AmazonAthenaClient Client { get; set; }
}
