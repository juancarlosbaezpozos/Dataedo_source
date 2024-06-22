using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.General;

public class ObjectsCounter
{
	public int TablesCount { get; set; }

	public int ViewsCount { get; set; }

	public int ProceduresCount { get; set; }

	public int FunctionsCount { get; set; }

	public ObjectsCounter()
	{
	}

	public ObjectsCounter(int tablesCount, int viewsCount, int proceduresCount, int functionsCount)
	{
		TablesCount = tablesCount;
		ViewsCount = viewsCount;
		ProceduresCount = proceduresCount;
		FunctionsCount = functionsCount;
	}

	public int CountMaxForConnecting(SharedDatabaseTypeEnum.DatabaseType? databaseTypeEnum)
	{
		int num = 0;
		num += TablesCount + ViewsCount + ProceduresCount + FunctionsCount;
		if (databaseTypeEnum == SharedDatabaseTypeEnum.DatabaseType.Oracle)
		{
			num += ProceduresCount + FunctionsCount;
		}
		return num;
	}
}
