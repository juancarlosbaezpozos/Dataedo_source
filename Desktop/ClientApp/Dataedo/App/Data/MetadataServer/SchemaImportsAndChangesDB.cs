using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Enums;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.UserControls.SchemaImportsAndChanges.Model;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Model.Data.SchemaImportsAndChanges;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer;

internal class SchemaImportsAndChangesDB : CommonDBSupport
{
	public SchemaImportsAndChangesDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public bool CheckIfEnabled()
	{
		return commands.Select.SchemaImportsAndChanges.CheckIfEnabled();
	}

	public void SetEnabled(bool enabled)
	{
		commands.Manipulation.SchemaImportsAndChanges.SetEnabled(enabled);
	}

	public List<SchemaImportsAndChangesObjectModel> GetDocumentationReport(int? documentationId, bool showSchema, bool showAllImports, string importsId)
	{
		return GetDocumentationReport(documentationId, showSchema, null, null, null, null, SharedObjectTypeEnum.ObjectType.Database, showAllImports, importsId);
	}

	public List<SchemaImportsAndChangesObjectModel> GetModuleReport(int? documentationId, int? moduleId, bool showSchema, bool showAllImports, string importsId)
	{
		return GetDocumentationReport(documentationId, showSchema, moduleId, null, null, null, SharedObjectTypeEnum.ObjectType.Module, showAllImports, importsId);
	}

	public List<SchemaImportsAndChangesObjectModel> GetObjectReport(int? documentationId, bool showSchema, string schema, string name, SharedObjectTypeEnum.ObjectType? objectType, bool showAllImports, string importsId)
	{
		return GetDocumentationReport(documentationId, showSchema, null, null, schema, name, objectType, showAllImports, importsId);
	}

	public void UpdateComments(IEnumerable<SchemaImportsAndChangesObjectModel> schemaImportsAndChangesObjects)
	{
		if (schemaImportsAndChangesObjects == null)
		{
			return;
		}
		foreach (SchemaImportsAndChangesObjectModel schemaImportsAndChangesObject in schemaImportsAndChangesObjects)
		{
			string descriptionBy = null;
			DateTime? descriptionDate = null;
			if (schemaImportsAndChangesObject.Level == SchemaChangeLevelEnum.SchemaChangeLevel.Object)
			{
				if (schemaImportsAndChangesObject.ObjectType == SharedObjectTypeEnum.ObjectType.Table || schemaImportsAndChangesObject.ObjectType == SharedObjectTypeEnum.ObjectType.View)
				{
					commands.Manipulation.SchemaImportsAndChanges.UpdateTableDescription(schemaImportsAndChangesObject.Data.ObjectChangeId, schemaImportsAndChangesObject.Data.ObjectDescription, out descriptionBy, out descriptionDate);
				}
				else if (schemaImportsAndChangesObject.ObjectType == SharedObjectTypeEnum.ObjectType.Procedure || schemaImportsAndChangesObject.ObjectType == SharedObjectTypeEnum.ObjectType.Function)
				{
					commands.Manipulation.SchemaImportsAndChanges.UpdateProcedureDescription(schemaImportsAndChangesObject.Data.ObjectChangeId, schemaImportsAndChangesObject.Data.ObjectDescription, out descriptionBy, out descriptionDate);
				}
				schemaImportsAndChangesObject.CommentedBy = descriptionBy;
				schemaImportsAndChangesObject.CommentDate = descriptionDate;
				schemaImportsAndChangesObject.IsCommentModified = false;
			}
			else if (schemaImportsAndChangesObject.Level == SchemaChangeLevelEnum.SchemaChangeLevel.Element && schemaImportsAndChangesObject.Data.ElementChangeId.HasValue)
			{
				if (schemaImportsAndChangesObject.ElementType == SharedObjectTypeEnum.ObjectType.Column)
				{
					commands.Manipulation.SchemaImportsAndChanges.UpdateColumnDescription(schemaImportsAndChangesObject.Data.ElementChangeId.Value, schemaImportsAndChangesObject.Data.ElementDescription, out descriptionBy, out descriptionDate);
				}
				else if (schemaImportsAndChangesObject.ElementType == SharedObjectTypeEnum.ObjectType.Relation)
				{
					commands.Manipulation.SchemaImportsAndChanges.UpdateRelationDescription(schemaImportsAndChangesObject.Data.ElementChangeId.Value, schemaImportsAndChangesObject.Data.ElementDescription, out descriptionBy, out descriptionDate);
				}
				else if (schemaImportsAndChangesObject.ElementType == SharedObjectTypeEnum.ObjectType.Key)
				{
					commands.Manipulation.SchemaImportsAndChanges.UpdateUniqueConstraintDescription(schemaImportsAndChangesObject.Data.ElementChangeId.Value, schemaImportsAndChangesObject.Data.ElementDescription, out descriptionBy, out descriptionDate);
				}
				else if (schemaImportsAndChangesObject.ElementType == SharedObjectTypeEnum.ObjectType.Trigger)
				{
					commands.Manipulation.SchemaImportsAndChanges.UpdateTriggerDescription(schemaImportsAndChangesObject.Data.ElementChangeId.Value, schemaImportsAndChangesObject.Data.ElementDescription, out descriptionBy, out descriptionDate);
				}
				else if (schemaImportsAndChangesObject.ElementType == SharedObjectTypeEnum.ObjectType.Parameter)
				{
					commands.Manipulation.SchemaImportsAndChanges.UpdateParameterDescription(schemaImportsAndChangesObject.Data.ElementChangeId.Value, schemaImportsAndChangesObject.Data.ElementDescription, out descriptionBy, out descriptionDate);
				}
				schemaImportsAndChangesObject.CommentedBy = descriptionBy;
				schemaImportsAndChangesObject.CommentDate = descriptionDate;
				schemaImportsAndChangesObject.IsCommentModified = false;
			}
			else if (schemaImportsAndChangesObject.Level == SchemaChangeLevelEnum.SchemaChangeLevel.Date)
			{
				if ((schemaImportsAndChangesObject.ObjectType == SharedObjectTypeEnum.ObjectType.Table || schemaImportsAndChangesObject.ObjectType == SharedObjectTypeEnum.ObjectType.View) && schemaImportsAndChangesObject.Data.ChangesId.HasValue)
				{
					commands.Manipulation.SchemaImportsAndChanges.UpdateTableDescription(schemaImportsAndChangesObject.Data.ChangesId.Value, schemaImportsAndChangesObject.Data.ObjectDescription, out descriptionBy, out descriptionDate);
				}
				else if ((schemaImportsAndChangesObject.ObjectType == SharedObjectTypeEnum.ObjectType.Procedure || schemaImportsAndChangesObject.ObjectType == SharedObjectTypeEnum.ObjectType.Function) && schemaImportsAndChangesObject.Data.ChangesId.HasValue)
				{
					commands.Manipulation.SchemaImportsAndChanges.UpdateProcedureDescription(schemaImportsAndChangesObject.Data.ChangesId.Value, schemaImportsAndChangesObject.Data.ObjectDescription, out descriptionBy, out descriptionDate);
				}
				else if (schemaImportsAndChangesObject.Data.UpdateId.HasValue)
				{
					commands.Manipulation.SchemaImportsAndChanges.UpdateSchemaUpdateDescription(schemaImportsAndChangesObject.Data.UpdateId.Value, schemaImportsAndChangesObject.Data.SchemaUpdateDescription, out descriptionBy, out descriptionDate);
				}
				schemaImportsAndChangesObject.CommentedBy = descriptionBy;
				schemaImportsAndChangesObject.CommentDate = descriptionDate;
				schemaImportsAndChangesObject.IsCommentModified = false;
			}
		}
	}

	public int InsertSchemaUpdateRow(DatabaseRow databaseRow, SchemaUpdateTypeEnum.SchemaUpdateType updateType = SchemaUpdateTypeEnum.SchemaUpdateType.Edit, Form owner = null)
	{
		SchemaUpdateRow row = new SchemaUpdateRow
		{
			Type = updateType,
			DatabaseId = databaseRow.Id,
			ConnectionDatabaseType = DatabaseTypeEnum.TypeToString(databaseRow.Type),
			ConnectionHost = databaseRow.Host,
			ConnectionUser = databaseRow.User,
			ConnectionPort = databaseRow.Port,
			ConnectionServiceName = databaseRow.ServiceName,
			ConnectionDatabaseName = databaseRow.Name,
			ConnectionDBMSVersion = databaseRow.DbmsVersion
		};
		return InsertSchemaUpdateRow(row, owner);
	}

	public void UpdateObjectChanges(int updateId)
	{
		commands.Manipulation.SchemaImportsAndChanges.UpdateObjectChanges(updateId);
	}

	private List<SchemaImportsAndChangesObjectModel> GetDocumentationReport(int? documentationId, bool showSchema, int? moduleId, int? objectId, string objectSchema, string objectName, SharedObjectTypeEnum.ObjectType? objectType, bool showAllImports, string importsId)
	{
		List<SchemaImportsAndChangesObjectModel> result = new List<SchemaImportsAndChangesObjectModel>();
		if (importsId == null)
		{
			return LoadImports(documentationId, showSchema, moduleId, objectId, objectSchema, objectName, objectType, showAllImports);
		}
		return LoadNodeData(documentationId, showSchema, moduleId, objectId, objectSchema, objectName, objectType, importsId, result);
	}

	private List<SchemaImportsAndChangesObjectModel> LoadImports(int? documentationId, bool showSchema, int? moduleId, int? objectId, string objectSchema, string objectName, SharedObjectTypeEnum.ObjectType? objectType, bool showAllImports)
	{
		string objectType2 = null;
		List<SchemaImportsAndChangesImportObject> imports;
		if (objectType == SharedObjectTypeEnum.ObjectType.Database)
		{
			imports = commands.Select.SchemaImportsAndChanges.GetDocumentationImports(documentationId);
		}
		else if (objectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			imports = commands.Select.SchemaImportsAndChanges.GetModuleImports(documentationId, moduleId);
		}
		else
		{
			objectType2 = SharedObjectTypeEnum.TypeToString(objectType);
			imports = commands.Select.SchemaImportsAndChanges.GetObjectImports(documentationId, objectType2, objectId, objectSchema, objectName);
		}
		return CreateImportChangesList(showSchema, imports, showAllImports, objectType2);
	}

	private static List<SchemaImportsAndChangesObjectModel> CreateImportChangesList(bool showSchema, List<SchemaImportsAndChangesImportObject> imports, bool showAllImports, string objectType = null)
	{
		List<SchemaImportsAndChangesObjectModel> list = new List<SchemaImportsAndChangesObjectModel>();
		if (imports.Count < 1)
		{
			SchemaImportsAndChangesObjectModel item = new SchemaImportsAndChangesObjectModel(SchemaChangeLevelEnum.SchemaChangeLevel.NoResults, null, null, showSchema);
			list.Add(item);
		}
		foreach (SchemaImportsAndChangesImportObject import in imports)
		{
			if (import != null)
			{
				SchemaImportsAndChangesObject schemaImportsAndChangesObject = new SchemaImportsAndChangesObject(import);
				SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel = new SchemaImportsAndChangesObjectModel(SchemaChangeLevelEnum.SchemaChangeLevel.Date, schemaImportsAndChangesObject, null, showSchema);
				bool flag = schemaImportsAndChangesObjectModel == null || schemaImportsAndChangesObjectModel.Data.AddedCount != 0 || schemaImportsAndChangesObjectModel == null || schemaImportsAndChangesObjectModel.Data.UpdatedCount != 0 || schemaImportsAndChangesObjectModel == null || schemaImportsAndChangesObjectModel.Data.DeletedCount != 0;
				if (objectType != null && flag)
				{
					schemaImportsAndChangesObjectModel.IsObjectImportDateLevel = true;
					schemaImportsAndChangesObject.ObjectType = objectType;
				}
				if (showAllImports || flag)
				{
					list.Add(schemaImportsAndChangesObjectModel);
				}
			}
		}
		return list;
	}

	private List<SchemaImportsAndChangesObjectModel> LoadNodeData(int? documentationId, bool showSchema, int? moduleId, int? objectId, string objectSchema, string objectName, SharedObjectTypeEnum.ObjectType? objectType, string importsId, List<SchemaImportsAndChangesObjectModel> result)
	{
		List<SchemaImportsAndChangesObject> list = new List<SchemaImportsAndChangesObject>();
		list = ((objectType == SharedObjectTypeEnum.ObjectType.Database) ? commands.Select.SchemaImportsAndChanges.GetImportChanges(documentationId, null, null, null, null, null, importsId) : ((objectType != SharedObjectTypeEnum.ObjectType.Module) ? commands.Select.SchemaImportsAndChanges.GetImportChanges(documentationId, null, SharedObjectTypeEnum.TypeToString(objectType), objectId, objectSchema, objectName, importsId) : commands.Select.SchemaImportsAndChanges.GetImportChanges(null, moduleId, null, null, null, null, importsId)));
		if (list.Count < 1)
		{
			SchemaImportsAndChangesObjectModel item = new SchemaImportsAndChangesObjectModel(SchemaChangeLevelEnum.SchemaChangeLevel.NoResults, null, null, showSchema);
			result.Add(item);
		}
		else
		{
			bool forObject = objectId.HasValue || objectSchema != null || objectName != null;
			AddDateLevelObjects(result, list, forObject, showSchema);
		}
		return result;
	}

	private void AddDateLevelObjects(List<SchemaImportsAndChangesObjectModel> result, List<SchemaImportsAndChangesObject> data, bool forObject, bool showSchema)
	{
		if (data.Count <= 0)
		{
			return;
		}
		foreach (IGrouping<DateTime, SchemaImportsAndChangesObject> item in from x in data
			group x by x.UpdateDate into x
			orderby x.Key descending
			select x)
		{
			AddOperationLevelObjects(null, result, item.ToList(), forObject, showSchema);
		}
	}

	private void AddOperationLevelObjects(SchemaImportsAndChangesObjectModel parentNode, List<SchemaImportsAndChangesObjectModel> result, List<SchemaImportsAndChangesObject> data, bool forObject, bool showSchema)
	{
		if (data.Count <= 0)
		{
			return;
		}
		foreach (IGrouping<string, SchemaImportsAndChangesObject> item in from x in data
			group x by x.ObjectChange into x
			orderby x.Key
			select x)
		{
			SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel = new SchemaImportsAndChangesObjectModel(SchemaChangeLevelEnum.SchemaChangeLevel.Operation, item.First(), parentNode, showSchema);
			if (forObject)
			{
				List<SchemaImportsAndChangesObject> list = new List<SchemaImportsAndChangesObject>();
				List<SchemaImportsAndChangesObject> list2 = item.ToList();
				int? lowestLevel = list2.Min((SchemaImportsAndChangesObject x) => x.Level);
				if (list2.Any((SchemaImportsAndChangesObject x) => !x.Level.HasValue))
				{
					list = list2;
				}
				else
				{
					List<SchemaImportsAndChangesObject> specificLevelSubColumns = list2.Where((SchemaImportsAndChangesObject x) => x.Level == lowestLevel).ToList();
					CreateHierarchicalList(specificLevelSubColumns, list2, list);
				}
				AddElementsLevelObjects(schemaImportsAndChangesObjectModel, result, list, showSchema);
				schemaImportsAndChangesObjectModel.IsObjectImportDateLevel = false;
				result.Add(schemaImportsAndChangesObjectModel);
			}
			else
			{
				schemaImportsAndChangesObjectModel.IsObjectImportDateLevel = false;
				result.Add(schemaImportsAndChangesObjectModel);
				AddObjectsLevelObjects(schemaImportsAndChangesObjectModel, result, item.ToList(), showSchema);
			}
		}
		if (parentNode != null)
		{
			parentNode.Children = new SchemaImportsAndChangesBindingList(parentNode.Children.OrderBy((SchemaImportsAndChangesObjectModel x) => x.ChangeType).ToList());
		}
	}

	private void AddObjectsLevelObjects(SchemaImportsAndChangesObjectModel parentNode, List<SchemaImportsAndChangesObjectModel> result, List<SchemaImportsAndChangesObject> data, bool showSchema)
	{
		if (data.Count <= 0)
		{
			return;
		}
		foreach (var item in from x in data
			group x by new { x.ObjectIdCurrent, x.ObjectType, x.ObjectName } into x
			orderby x.Key.ObjectIdCurrent
			select x)
		{
			SchemaImportsAndChangesObjectModel parentNode2 = new SchemaImportsAndChangesObjectModel(SchemaChangeLevelEnum.SchemaChangeLevel.Object, item.First(), parentNode, showSchema);
			List<SchemaImportsAndChangesObject> list = new List<SchemaImportsAndChangesObject>();
			List<SchemaImportsAndChangesObject> list2 = item.ToList();
			int? lowestLevel = list2.Min((SchemaImportsAndChangesObject x) => x.Level);
			if (list2.Any((SchemaImportsAndChangesObject x) => !x.Level.HasValue))
			{
				list = list2;
			}
			else
			{
				List<SchemaImportsAndChangesObject> specificLevelSubColumns = list2.Where((SchemaImportsAndChangesObject x) => x.Level == lowestLevel).ToList();
				CreateHierarchicalList(specificLevelSubColumns, list2, list);
			}
			AddElementsLevelObjects(parentNode2, result, list, showSchema);
		}
	}

	private void CreateHierarchicalList(IEnumerable<SchemaImportsAndChangesObject> specificLevelSubColumns, List<SchemaImportsAndChangesObject> allColumns, List<SchemaImportsAndChangesObject> result)
	{
		if (specificLevelSubColumns == null)
		{
			return;
		}
		foreach (SchemaImportsAndChangesObject column in specificLevelSubColumns)
		{
			result.Add(column);
			CreateHierarchicalList((from x in allColumns
				where x.ParentId == column.ElementId
				orderby x.ElementId
				select x).ToList(), allColumns, result);
		}
	}

	private void AddElementsLevelObjects(SchemaImportsAndChangesObjectModel parentNode, List<SchemaImportsAndChangesObjectModel> result, List<SchemaImportsAndChangesObject> data, bool showSchema)
	{
		if (data.Count <= 0)
		{
			return;
		}
		foreach (IGrouping<int?, SchemaImportsAndChangesObject> item in from x in data
			where x.ElementId.HasValue && x.ElementId > 0
			group x by x.ElementId)
		{
			new SchemaImportsAndChangesObjectModel(SchemaChangeLevelEnum.SchemaChangeLevel.Element, item.First(), item.ToList(), parentNode, showSchema);
		}
	}

	private int InsertSchemaUpdateRow(SchemaUpdateRow row, Form owner = null)
	{
		try
		{
			return Convert.ToInt32(commands.Manipulation.SchemaImportsAndChanges.InsertSchemaUpdate(ConvertToSchemaUpdate(row)));
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while inserting the schema_update:", owner);
			return -1;
		}
	}

	private SchemaUpdate ConvertToSchemaUpdate(SchemaUpdateRow row)
	{
		return new SchemaUpdate
		{
			Type = SchemaUpdateTypeEnum.TypeToString(row.Type),
			DatabaseId = row.DatabaseId,
			ConnectionDatabaseType = row.ConnectionDatabaseType,
			ConnectionHost = row.ConnectionHost,
			ConnectionUser = row.ConnectionUser,
			ConnectionPort = row.ConnectionPort,
			ConnectionServiceName = row.ConnectionServiceName,
			ConnectionDatabaseName = row.ConnectionDatabaseName,
			ConnectionDBMSVersion = row.ConnectionDBMSVersion
		};
	}
}
