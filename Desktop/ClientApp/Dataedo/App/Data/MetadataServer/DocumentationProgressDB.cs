using System.Collections.Generic;
using Dataedo.App.Enums;
using Dataedo.App.Licences;
using Dataedo.App.Tools;
using Dataedo.Model.Data.Progress;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;

namespace Dataedo.App.Data.MetadataServer;

internal class DocumentationProgressDB : CommonDBSupport
{
	public DocumentationProgressDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public List<ProgressWithIdAndTypeObject> GetObjectDocumentationProgress(ProgressTypeModel progressType)
	{
		if (progressType.Type == ProgressTypeEnum.AllDocumentations)
		{
			return commands.Select.Progress.GetProgress();
		}
		if (progressType.Type == ProgressTypeEnum.TablesAndColumns)
		{
			return commands.Select.Progress.GetProgressForTablesViewsAndColumns();
		}
		return commands.Select.Progress.GetProgressForCustomField(progressType.FieldName, "'" + progressType.FieldName + "'");
	}

	public List<ProgressObject> GetObjectProgress(int id, SharedObjectTypeEnum.ObjectType type)
	{
		return type switch
		{
			SharedObjectTypeEnum.ObjectType.Function => commands.Select.Progress.GetProgressForProcedure(id, SharedObjectTypeEnum.ObjectType.Function), 
			SharedObjectTypeEnum.ObjectType.Procedure => commands.Select.Progress.GetProgressForProcedure(id, SharedObjectTypeEnum.ObjectType.Procedure), 
			SharedObjectTypeEnum.ObjectType.Table => commands.Select.Progress.GetProgressForTable(id, SharedObjectTypeEnum.ObjectType.Table), 
			SharedObjectTypeEnum.ObjectType.View => commands.Select.Progress.GetProgressForTable(id, SharedObjectTypeEnum.ObjectType.View), 
			SharedObjectTypeEnum.ObjectType.Structure => commands.Select.Progress.GetProgressForTable(id, SharedObjectTypeEnum.ObjectType.Structure), 
			SharedObjectTypeEnum.ObjectType.Module => commands.Select.Progress.GetProgressForModule(id), 
			SharedObjectTypeEnum.ObjectType.Term => commands.Select.Progress.GetProgressForTerm(id), 
			_ => null, 
		};
	}

	public List<ProgressObject> GetSingleTableViewAndColumnsProgress(int id, SharedObjectTypeEnum.ObjectType type)
	{
		return type switch
		{
			SharedObjectTypeEnum.ObjectType.Table => commands.Select.Progress.GetSingleTableViewAndColumnsProgress(id, SharedObjectTypeEnum.ObjectType.Table), 
			SharedObjectTypeEnum.ObjectType.View => commands.Select.Progress.GetSingleTableViewAndColumnsProgress(id, SharedObjectTypeEnum.ObjectType.View), 
			SharedObjectTypeEnum.ObjectType.Structure => commands.Select.Progress.GetSingleTableViewAndColumnsProgress(id, SharedObjectTypeEnum.ObjectType.Structure), 
			_ => null, 
		};
	}

	public List<ProgressObject> GetObjectCustomFieldProgress(int id, SharedObjectTypeEnum.ObjectType type, string fieldName)
	{
		string fieldNameToJoin = "'" + fieldName + "'";
		return type switch
		{
			SharedObjectTypeEnum.ObjectType.Function => commands.Select.Progress.GetProgressForProcedureCustomField(id, SharedObjectTypeEnum.ObjectType.Function, fieldName, fieldNameToJoin), 
			SharedObjectTypeEnum.ObjectType.Procedure => commands.Select.Progress.GetProgressForProcedureCustomField(id, SharedObjectTypeEnum.ObjectType.Procedure, fieldName, fieldNameToJoin), 
			SharedObjectTypeEnum.ObjectType.Table => commands.Select.Progress.GetProgressForTableCustomField(id, fieldName, fieldNameToJoin), 
			SharedObjectTypeEnum.ObjectType.View => commands.Select.Progress.GetProgressForViewCustomField(id, fieldName, fieldNameToJoin), 
			SharedObjectTypeEnum.ObjectType.Structure => commands.Select.Progress.GetProgressForObjectCustomField(id, fieldName, fieldNameToJoin), 
			SharedObjectTypeEnum.ObjectType.Module => commands.Select.Progress.GetProgressForModuleCustomField(id, fieldName, fieldNameToJoin), 
			SharedObjectTypeEnum.ObjectType.Term => commands.Select.Progress.GetProgressForTermCustomField(id, fieldName), 
			_ => null, 
		};
	}

	public List<ProgressObject> GetTableViewAndColumnsProgress(int id, SharedObjectTypeEnum.ObjectType type)
	{
		return type switch
		{
			SharedObjectTypeEnum.ObjectType.Table => commands.Select.Progress.GetProgressForTable(id, SharedObjectTypeEnum.ObjectType.Table), 
			SharedObjectTypeEnum.ObjectType.View => commands.Select.Progress.GetProgressForTable(id, SharedObjectTypeEnum.ObjectType.View), 
			SharedObjectTypeEnum.ObjectType.Structure => commands.Select.Progress.GetProgressForTable(id, SharedObjectTypeEnum.ObjectType.Structure), 
			_ => null, 
		};
	}

	public List<ProgressObject> GetCustomFieldProgress(int id, string fieldName, SharedObjectTypeEnum.ObjectType type)
	{
		string fieldNameToJoin = "'" + fieldName + "'";
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary))
		{
			return type switch
			{
				SharedObjectTypeEnum.ObjectType.Function => commands.Select.Progress.GetProgressForProcedureCustomField(id, SharedObjectTypeEnum.ObjectType.Function, fieldName, fieldNameToJoin), 
				SharedObjectTypeEnum.ObjectType.Procedure => commands.Select.Progress.GetProgressForProcedureCustomField(id, SharedObjectTypeEnum.ObjectType.Procedure, fieldName, fieldNameToJoin), 
				SharedObjectTypeEnum.ObjectType.Table => commands.Select.Progress.GetProgressForTableCustomField(id, fieldName, fieldNameToJoin), 
				SharedObjectTypeEnum.ObjectType.View => commands.Select.Progress.GetProgressForViewCustomField(id, fieldName, fieldNameToJoin), 
				SharedObjectTypeEnum.ObjectType.Structure => commands.Select.Progress.GetProgressForObjectCustomField(id, fieldName, fieldNameToJoin), 
				_ => null, 
			};
		}
		return type switch
		{
			SharedObjectTypeEnum.ObjectType.Function => commands.Select.Progress.GetProgressForProcedureCustomField(id, SharedObjectTypeEnum.ObjectType.Function, fieldName, fieldNameToJoin), 
			SharedObjectTypeEnum.ObjectType.Procedure => commands.Select.Progress.GetProgressForProcedureCustomField(id, SharedObjectTypeEnum.ObjectType.Procedure, fieldName, fieldNameToJoin), 
			SharedObjectTypeEnum.ObjectType.Table => commands.Select.Progress.GetProgressForTableCustomField(id, fieldName, fieldNameToJoin), 
			SharedObjectTypeEnum.ObjectType.View => commands.Select.Progress.GetProgressForViewCustomField(id, fieldName, fieldNameToJoin), 
			SharedObjectTypeEnum.ObjectType.Structure => commands.Select.Progress.GetProgressForObjectCustomField(id, fieldName, fieldNameToJoin), 
			SharedObjectTypeEnum.ObjectType.Term => commands.Select.Progress.GetProgressForTermCustomField(id, fieldName), 
			_ => null, 
		};
	}
}
