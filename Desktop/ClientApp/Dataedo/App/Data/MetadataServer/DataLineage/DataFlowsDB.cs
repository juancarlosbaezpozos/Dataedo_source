using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Model.Data.DataLineage;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer.DataLineage;

public class DataFlowsDB : CommonDBSupport
{
	public DataFlowsDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public BindingList<DataFlowRow> GetFlowsByProcessId(int processId, int? currentObjectDatabaseId, Form owner = null)
	{
		try
		{
			return new BindingList<DataFlowRow>((from x in commands.Select.DataFlows.GetFlowsByProcessId(processId, currentObjectDatabaseId)
				select new DataFlowRow(x)).ToList());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting data flows:", owner);
			return null;
		}
	}

	public BindingList<DataFlowRow> GetFlowsRelatedToObject(int objectId, string objectType, string direction, int? currentObjectDatabaseId, Form owner = null)
	{
		try
		{
			return new BindingList<DataFlowRow>((from x in commands.Select.DataFlows.GetFlowsRelatedToObject(objectId, objectType, direction, currentObjectDatabaseId)
				select new DataFlowRow(x)).ToList());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting data flows related to object:", owner);
			return null;
		}
	}

	public BindingList<DataFlowRow> GetFlowsByProcessor(int processorId, string processorObjectType, int? currentObjectDatabaseId, Form owner = null)
	{
		try
		{
			return new BindingList<DataFlowRow>((from x in commands.Select.DataFlows.GetFlowsByProcessor(processorId, processorObjectType, currentObjectDatabaseId)
				select new DataFlowRow(x)).ToList());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting data flows for processor:", owner);
			return null;
		}
	}

	public bool Delete(IEnumerable<int> dataFlowsIDs, Form owner = null)
	{
		try
		{
			commands.Manipulation.DataFlows.DeleteFlows(dataFlowsIDs.ToArray());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting the data flows:", owner);
			return false;
		}
		return true;
	}

	public bool Delete(int dataFlowID, Form owner = null)
	{
		try
		{
			commands.Manipulation.DataFlows.DeleteFlow(dataFlowID);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting the data flows:", owner);
			return false;
		}
		return true;
	}

	public int? Insert(DataFlowRow dataFlowRow, Form owner = null)
	{
		try
		{
			return commands.Manipulation.DataFlows.InsertFlow(dataFlowRow.ProcessId, dataFlowRow.Direction, dataFlowRow.ObjectType, dataFlowRow.ObjectId, dataFlowRow.Source, Dataedo.App.StaticData.IsProjectFile);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while adding data flow:", owner);
			return null;
		}
	}

	public bool UpdateFlowDirection(DataFlowRow dataFlowRow, Form owner = null)
	{
		try
		{
			commands.Manipulation.DataFlows.UpdateFlowDirection(dataFlowRow.Id, dataFlowRow.Direction);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating data flow direction:", owner);
			return false;
		}
		return true;
	}

	public int? GetProcessFlowsNumber(int processId, string direction, Form owner = null)
	{
		try
		{
			return commands.Select.DataFlows.GetProcessFlowsNumber(processId, direction);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting number of process' flows:", owner);
			return null;
		}
	}

	public int? CountFlowsForObject(int processorId, string processorObjectType)
	{
		return commands.Select.DataFlows.CountFlowsForObject(processorId, processorObjectType);
	}

	public bool CheckIfFlowExists(int processId, string direction, SharedObjectTypeEnum.ObjectType objectType, int objectId)
	{
		return commands.Select.DataFlows.CheckIfFlowExists(processId, direction, objectType, objectId);
	}

	public int? InsertColumnRow(DataLineageColumnsFlowRow columnRow, Form owner = null)
	{
		try
		{
			return commands.Manipulation.DataFlows.InsertDataColumnFlow(columnRow.InflowId, columnRow.InflowColumnId, columnRow.OutflowId, columnRow.OutflowColumnId, Dataedo.App.StaticData.IsProjectFile, UserTypeEnum.TypeToString(UserTypeEnum.UserType.USER));
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while adding data lineage column row:", owner);
			return null;
		}
	}

	public bool UpdateDataColumnFlow(DataLineageColumnsFlowRow columnRow, Form owner = null)
	{
		try
		{
			commands.Manipulation.DataFlows.UpdateDataColumnFlow(columnRow.DataColumnsFlowId, columnRow.InflowId, columnRow.InflowColumnId, columnRow.OutflowId, columnRow.OutflowColumnId);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating data lineage column row:", owner);
			return false;
		}
		return true;
	}

	public List<DataLineageDropdownColumnObject> GetColumnsByObject(int objectId, SharedObjectTypeEnum.ObjectType objectType, Form owner = null)
	{
		try
		{
			switch (objectType)
			{
			case SharedObjectTypeEnum.ObjectType.Table:
			case SharedObjectTypeEnum.ObjectType.View:
			case SharedObjectTypeEnum.ObjectType.Structure:
				return commands.Select.DataFlows.GetColumnsFromColumnsTableByObjectId(objectId);
			case SharedObjectTypeEnum.ObjectType.Function:
			case SharedObjectTypeEnum.ObjectType.Procedure:
				return commands.Select.DataFlows.GetColumnsFromParametersTableByObjectId(objectId);
			default:
				return null;
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting flow columns:", owner);
			return null;
		}
	}

	public void DeleteEmptyDataColumnsFlowsRecords()
	{
		if (Dataedo.App.StaticData.IsProjectFile)
		{
			commands.Manipulation.DataFlows.DeleteEmptyDataColumnsFlowsRecords();
		}
	}

	public void DeleteDataColumnsFlows(List<int> columnIDs)
	{
		if (columnIDs != null && columnIDs.Count != 0)
		{
			commands.Manipulation.DataFlows.DeleteDataColumnsFlows(columnIDs);
		}
	}

	public List<DataLineageColumnsFlowRow> GetDataColumnsFlows(List<DataProcessRow> processes, Form owner = null)
	{
		try
		{
			List<int> list = processes.Select((DataProcessRow x) => x.Id).ToList();
			if (list == null || list.Count == 0)
			{
				return null;
			}
			return (from x in commands.Select.DataFlows.GetDataColumnsFlows(list)
				select new DataLineageColumnsFlowRow(x, processes.FirstOrDefault((DataProcessRow p) => p.Id == x.ProcessId))).ToList();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting data flow columns:", owner);
			return null;
		}
	}

	public List<DataLineageColumnsFlowRow> GetDataColumnsFlows(int processID, Form owner = null)
	{
		try
		{
			return (from x in commands.Select.DataFlows.GetDataColumnsFlows(new List<int> { processID })
				select new DataLineageColumnsFlowRow(x, null)).ToList();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting data flow columns:", owner);
			return null;
		}
	}

	public bool CheckIfFlowsAreAssignedInColumns(List<int> dataFlowsIDs, Form owner = null)
	{
		try
		{
			if (dataFlowsIDs == null || !dataFlowsIDs.Any())
			{
				return false;
			}
			return commands.Select.DataFlows.CheckIfFlowsAreAssignedInColumns(dataFlowsIDs);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while checking if data flows are in columns:", owner);
			return false;
		}
	}
}
