using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.MenuTree;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Tools;

public static class BaseSummaryModuleHelper
{
	public static void AddObjectsToAssignedModule(IEnumerable<object> selectedObjects, int? moduleId, Form owner = null)
	{
		foreach (ObjectWithModulesObject selectedObject in selectedObjects)
		{
			if (!IsAlreadyInModule(moduleId, selectedObject))
			{
				SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(selectedObject.ObjectType);
				int id = selectedObject.Id;
				DB.Module.InsertModuleObject(moduleId.Value, id, objectType ?? SharedObjectTypeEnum.ObjectType.Table, owner);
				AddObjectNodeToAssignedModule(moduleId, selectedObject, id, objectType ?? SharedObjectTypeEnum.ObjectType.Table);
			}
		}
	}

	public static bool IsAlreadyInModule(int? moduleId, ObjectWithModulesObject selectedObject)
	{
		string modulesId = selectedObject.ModulesId;
		if (string.IsNullOrEmpty(modulesId))
		{
			return false;
		}
		return modulesId.Split(',').Contains(moduleId.ToString());
	}

	public static void AddToNewModule(GridView gridView, int moduleId)
	{
		new List<DataRow>();
		int[] selectedRows = gridView.GetSelectedRows();
		foreach (int rowHandle in selectedRows)
		{
			ObjectWithModulesObject objectWithModulesObject = gridView.GetRow(rowHandle) as ObjectWithModulesObject;
			SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(objectWithModulesObject.ObjectType);
			int id = objectWithModulesObject.Id;
			DB.Module.InsertModuleObject(moduleId, id, objectType ?? SharedObjectTypeEnum.ObjectType.Table, gridView?.GridControl?.FindForm());
			AddObjectNodeToAssignedModule(moduleId, objectWithModulesObject, id, objectType ?? SharedObjectTypeEnum.ObjectType.Table);
		}
	}

	private static void AddObjectNodeToAssignedModule(int? moduleId, ObjectWithModulesObject selectedObject, int objectId, SharedObjectTypeEnum.ObjectType type)
	{
		DBTreeMenu.AddObjectToModule(selectedObject.DatabaseId, schema: selectedObject.Schema, name: selectedObject.Name, title: selectedObject.Title, source: UserTypeEnum.ObjectToType(selectedObject.Source) ?? UserTypeEnum.UserType.DBMS, subtype: SharedObjectSubtypeEnum.StringToType(type, selectedObject.Subtype), tableId: objectId, moduleId: moduleId.Value, type: type);
		if (string.IsNullOrEmpty(selectedObject.ModulesId))
		{
			selectedObject.ModulesId = moduleId.ToString();
		}
		else
		{
			selectedObject.ModulesId = selectedObject.ModulesId + "," + moduleId;
		}
	}
}
