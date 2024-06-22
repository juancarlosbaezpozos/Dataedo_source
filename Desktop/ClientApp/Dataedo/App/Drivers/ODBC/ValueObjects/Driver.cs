namespace Dataedo.App.Drivers.ODBC.ValueObjects;

internal class Driver
{
	public DriverMetaFile MetaFile { get; protected set; }

	public DriverQueries Queries { get; protected set; }

	public Driver(DriverMetaFile metaFile, DriverQueries queries)
	{
		MetaFile = metaFile;
		Queries = queries;
	}

	public Driver MakeCopy()
	{
		return new Driver((DriverMetaFile)MetaFile.Clone(), (DriverQueries)Queries.Clone());
	}
}
