using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Model.Data.DataLineage;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer.DataLineage;

internal class DataProcessesDB : CommonDBSupport
{
	public DataProcessesDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public List<DataProcessRow> GetProcessesByProcessorId(int processorId, SharedObjectTypeEnum.ObjectType? processorType)
	{
		return (from x in commands.Select.DataProcesses.GetProcessesByProcessorId(processorId, SharedObjectTypeEnum.TypeToString(processorType))
			select new DataProcessRow(x) into x
			orderby x.Name
			select x).ToList();
	}

	public DataProcessRow GetProcessById(int processId)
	{
		return new DataProcessRow(commands.Select.DataProcesses.GetProcessById(processId));
	}

	public bool Delete(IEnumerable<int> dataProcesses, Form owner = null)
	{
		try
		{
			commands.Manipulation.DataProcesses.DeleteProcesses(dataProcesses.ToArray());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting the data processes:", owner);
			return false;
		}
		return true;
	}

	public int? Insert(DataProcessRow dataProcessRow, Form owner = null)
	{
		try
		{
			return commands.Manipulation.DataProcesses.InsertProcess(dataProcessRow.Name, dataProcessRow.ParentObjectType.Value, dataProcessRow.ParentId, dataProcessRow.Source, Dataedo.App.StaticData.IsProjectFile, dataProcessRow.Script);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while adding data processes:", owner);
			return null;
		}
	}

	public int? Synchronize(DataProcessRow dataProcessRow, Form owner = null)
	{
		try
		{
			DataProcessRow dataProcessRow2 = (from x in GetProcessesByProcessorId(dataProcessRow.ParentId, dataProcessRow.ParentObjectType.Value)
				where x.Name == dataProcessRow.Name
				select x).FirstOrDefault();
			if (dataProcessRow2 == null)
			{
				int? result = commands.Manipulation.DataProcesses.InsertProcess(dataProcessRow.Name, dataProcessRow.ParentObjectType.Value, dataProcessRow.ParentId, dataProcessRow.Source, Dataedo.App.StaticData.IsProjectFile, dataProcessRow.Script);
				dataProcessRow.Id = result.Value;
				dataProcessRow.RowState = ManagingRowsEnum.ManagingRows.Added;
				return result;
			}
			if (!string.Equals(dataProcessRow2.Script, dataProcessRow.Script, StringComparison.OrdinalIgnoreCase))
			{
				dataProcessRow.Id = dataProcessRow2.Id;
				commands.Manipulation.DataProcesses.UpdateProcess(dataProcessRow.Id, dataProcessRow.Name, dataProcessRow.Script);
				dataProcessRow.RowState = ManagingRowsEnum.ManagingRows.Updated;
				return dataProcessRow.Id;
			}
			dataProcessRow.Id = dataProcessRow2.Id;
			dataProcessRow.RowState = ManagingRowsEnum.ManagingRows.Unchanged;
			return dataProcessRow2.Id;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while synchronizing data processes:", owner);
			return null;
		}
	}

	public bool Update(DataProcessRow dataProcessRow, Form owner = null)
	{
		try
		{
			commands.Manipulation.DataProcesses.UpdateProcessName(dataProcessRow.Id, dataProcessRow.Name);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while adding data processes:", owner);
			return false;
		}
		return true;
	}

	public void CreateAutomaticViewsDataLineage(int databaseId, Form owner = null)
	{
		try
		{
			if (!Dataedo.App.StaticData.IsProjectFile)
			{
				commands.Manipulation.DataProcesses.CreateAutomaticViewsDataLineage(databaseId);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error creating automatic data lineage for views:", owner);
		}
	}

	public List<ColumnDataLineageResult> GetColumnDataLineageResultsForTable(int tableId, Form owner)
	{
		try
		{
			return commands.Select.DataProcesses.GetColumnDataLineageResultsForTable(tableId);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting column data lineage results:", owner);
			return null;
		}
	}
}
