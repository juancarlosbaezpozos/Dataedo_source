using System.Collections.Specialized;

namespace Dataedo.App.Tools.Tracking.Mappings;

public class SpecificParametersMapper
{
	private const string objectsCountParameterName = "objects_count";

	private const string timeParameterName = "time";

	private const string importedObjectsParameterName = "imported_objects";

	private const string importedTablesParameterName = "imported_tables";

	private const string formatParameterName = "format";

	private const string connectorParameterName = "connector";

	private const string fileSizeParameterName = "file_size";

	private const string descriptionParameterName = "description";

	private const string profiledScopeParameterName = "scope";

	private const string profiledTablesParameterName = "tables";

	private const string profiledColumnsParameterName = "columns";

	private const string profiledTablesSuccessParameterName = "tables_success";

	private const string profiledColumnsSuccessParameterName = "columns_success";

	private const string profiledStatusParameterName = "status";

	private const string profiledDatatypesParameterName = "datatypes";

	private const string objectTypeParameterName = "object_type";

	private const string draggedObjectTypeParameterName = "dragged_object_type";

	private const string viewParameterName = "view";

	private const string flowsCountParameterName = "flows_count";

	private const string columnsCountParameterName = "columns_count";

	private const string historySizeParameterName = "size";

	private const string databasesCountParameterName = "db_count";

	public static NameValueCollection GetObjectCounts(NameValueCollection nameValueCollection, string objectCounts)
	{
		nameValueCollection.Add("objects_count", objectCounts);
		return nameValueCollection;
	}

	public static NameValueCollection GetTime(NameValueCollection nameValueCollection, string time)
	{
		nameValueCollection.Add("time", time);
		return nameValueCollection;
	}

	public static NameValueCollection GetImportedObjects(NameValueCollection nameValueCollection, string importedObjects)
	{
		nameValueCollection.Add("imported_objects", importedObjects);
		return nameValueCollection;
	}

	public static NameValueCollection GetImportedTables(NameValueCollection nameValueCollection, string importedTables)
	{
		nameValueCollection.Add("imported_tables", importedTables);
		return nameValueCollection;
	}

	public static NameValueCollection GetFormat(NameValueCollection nameValueCollection, string format)
	{
		nameValueCollection.Add("format", format);
		return nameValueCollection;
	}

	public static NameValueCollection GetConnector(NameValueCollection nameValueCollection, string connector)
	{
		nameValueCollection.Add("connector", connector);
		return nameValueCollection;
	}

	public static NameValueCollection GetFileSize(NameValueCollection nameValueCollection, string fileSize)
	{
		nameValueCollection.Add("file_size", fileSize);
		return nameValueCollection;
	}

	public static NameValueCollection GetScope(NameValueCollection nameValueCollection, string scope)
	{
		nameValueCollection.Add("scope", scope);
		return nameValueCollection;
	}

	public static NameValueCollection GetTables(NameValueCollection nameValueCollection, string tables)
	{
		nameValueCollection.Add("tables", tables);
		return nameValueCollection;
	}

	public static NameValueCollection GetColumns(NameValueCollection nameValueCollection, string columns)
	{
		nameValueCollection.Add("columns", columns);
		return nameValueCollection;
	}

	public static NameValueCollection GetTablesSuccess(NameValueCollection nameValueCollection, string tablesSuccess)
	{
		nameValueCollection.Add("tables_success", tablesSuccess);
		return nameValueCollection;
	}

	public static NameValueCollection GetColumnsSuccess(NameValueCollection nameValueCollection, string columnsSuccess)
	{
		nameValueCollection.Add("columns_success", columnsSuccess);
		return nameValueCollection;
	}

	public static NameValueCollection GetDatatypes(NameValueCollection nameValueCollection, string datatypes)
	{
		nameValueCollection.Add("datatypes", datatypes);
		return nameValueCollection;
	}

	public static NameValueCollection GetStatus(NameValueCollection nameValueCollection, string status)
	{
		nameValueCollection.Add("status", status);
		return nameValueCollection;
	}

	public static NameValueCollection GetObjectType(NameValueCollection nameValueCollection, string objectType)
	{
		nameValueCollection.Add("object_type", objectType);
		return nameValueCollection;
	}

	public static NameValueCollection GetDraggedObjectType(NameValueCollection nameValueCollection, string draggedObjectType)
	{
		nameValueCollection.Add("dragged_object_type", draggedObjectType);
		return nameValueCollection;
	}

	public static NameValueCollection GetView(NameValueCollection nameValueCollection, string view)
	{
		nameValueCollection.Add("view", view);
		return nameValueCollection;
	}

	public static NameValueCollection GetFlowsCount(NameValueCollection nameValueCollection, string flowsCount)
	{
		nameValueCollection.Add("flows_count", flowsCount);
		return nameValueCollection;
	}

	public static NameValueCollection GetDescription(NameValueCollection nameValueCollection, string description)
	{
		nameValueCollection.Add("description", description);
		return nameValueCollection;
	}

	public static NameValueCollection GetColumnsCount(NameValueCollection nameValueCollection, string columnsCount)
	{
		nameValueCollection.Add("columns_count", columnsCount);
		return nameValueCollection;
	}

	public static NameValueCollection GetHistorySize(NameValueCollection nameValueCollection, string historySize)
	{
		nameValueCollection.Add("size", historySize);
		return nameValueCollection;
	}

	public static NameValueCollection GetDatabasesCount(NameValueCollection nameValueCollection, string databasesCount)
	{
		nameValueCollection.Add("db_count", databasesCount);
		return nameValueCollection;
	}
}
