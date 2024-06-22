using System.Data.Common;
using Dataedo.Model.Data.Interfaces.Initialization.DbDataReader;
using Dataedo.Model.Extensions;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes;

public class HiveDatabaseInfo : IInitializable
{
	public string DatabaseName { get; set; }

	public string DatabaseDesc { get; set; }

	public string CatalogName { get; set; }

	public string CatalogDesc { get; set; }

	public HiveDatabaseInfo()
	{
	}

	public HiveDatabaseInfo(DbDataReader reader)
	{
		Initialize(reader);
	}

	public void Initialize(DbDataReader data)
	{
		CatalogName = data.Field<string>("catalog_name");
		CatalogDesc = data.Field<string>("catalog_desc");
		DatabaseName = data.Field<string>("database_name");
		DatabaseDesc = data.Field<string>("database_desc");
	}

	public override string ToString()
	{
		return CatalogName + "." + DatabaseName;
	}
}
