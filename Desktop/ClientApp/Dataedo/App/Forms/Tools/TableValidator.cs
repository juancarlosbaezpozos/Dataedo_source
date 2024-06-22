using System.Linq;
using Dataedo.App.Data.MetadataServer;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Forms.Tools;

public class TableValidator
{
	public int DatabaseId { get; private set; }

	public int TableId { get; private set; }

	public SharedObjectTypeEnum.ObjectType ObjectType { get; set; }

	public TableValidator(int databaseId, int tableId, SharedObjectTypeEnum.ObjectType objectType)
	{
		DatabaseId = databaseId;
		TableId = tableId;
		ObjectType = objectType;
	}

	public bool Exists(string schema, string name)
	{
		return (from x in DB.Table.GetTablesByDatabase(DatabaseId, ObjectType)
			where x.Id != TableId
			select x).Any((TableByDatabaseIdObject x) => (x.Schema?.ToLower() + "." + x.Name.ToLower()).Equals(schema?.ToLower() + "." + name.ToLower()));
	}
}
