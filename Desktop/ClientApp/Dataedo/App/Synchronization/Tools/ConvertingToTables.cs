using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools.ERD.Diagram;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Synchronization.Tools;

public class ConvertingToTables
{
	public static List<IgnoredObject> ReloadIgnoredObjectsTable(int database_id, IEnumerable<ObjectRow> objects)
	{
		return objects.Select((ObjectRow x) => new IgnoredObject
		{
			DatabaseId = database_id,
			Schema = x.Schema,
			Name = x.Name,
			ObjectType = x.TypeAsString
		}).ToList();
	}

	public static List<IgnoredObject> ReloadIgnoredObjectsTable(int database_id, string schema, string name, string object_type)
	{
		return new List<IgnoredObject>
		{
			new IgnoredObject
			{
				DatabaseId = database_id,
				Schema = schema,
				Name = name,
				ObjectType = object_type
			}
		};
	}

	public static List<ObjectIdTypeForModule> ReloadModuleObjectsTable(int module_id, List<DBTreeNode> objectNodes)
	{
		return objectNodes.Select((DBTreeNode x) => new ObjectIdTypeForModule
		{
			ModuleId = module_id,
			BaseId = x.Id,
			Type = SharedObjectTypeEnum.TypeToStringShort(x.ObjectType)
		}).ToList();
	}

	public static List<ObjectIdTypeForModule> ReloadModuleObjectsTable(int module_id, IEnumerable<Node> objectNodes)
	{
		return objectNodes.Select((Node x) => new ObjectIdTypeForModule
		{
			ModuleId = module_id,
			BaseId = x.TableId,
			Type = SharedObjectTypeEnum.TypeToStringShort(x.ObjectType)
		}).ToList();
	}

	public static List<ObjectIdTypeForModule> ReloadModuleObjectsTable(GridView gridView, SharedObjectTypeEnum.ObjectType objectType)
	{
		List<ObjectIdTypeForModule> list = new List<ObjectIdTypeForModule>();
		string type = SharedObjectTypeEnum.TypeToStringShort(objectType);
		int[] selectedRows = gridView.GetSelectedRows();
		for (int num = selectedRows.Count() - 1; num >= 0; num--)
		{
			ObjectWithModulesObject objectWithModulesObject = gridView.GetRow(selectedRows[num]) as ObjectWithModulesObject;
			list.Add(new ObjectIdTypeForModule
			{
				BaseId = PrepareValue.ToInt(objectWithModulesObject.Id).Value,
				Type = type
			});
		}
		return list;
	}
}
