using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.Model.Data.Interfaces;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using DevExpress.XtraTreeList;

namespace Dataedo.App.UserControls.DataLineage;

public class DataProcessesCollection : BindingList<ProcessesContainer>, TreeList.IVirtualTreeListData
{
	private AllDataFlowsContainer allDataFlowsContainer;

	private List<DataProcessRow> deletedDataProcesses;

	private List<DataProcessRow> allDataProcesses;

	public List<DataProcessRow> AllDataProcesses => allDataProcesses;

	public void Init(SharedObjectTypeEnum.ObjectType objectType)
	{
		allDataFlowsContainer = new AllDataFlowsContainer(this);
		Add(allDataFlowsContainer);
		deletedDataProcesses = new List<DataProcessRow>();
		allDataProcesses = new List<DataProcessRow>();
	}

	public AllDataFlowsContainer GetAllDataFlowsContainer()
	{
		return allDataFlowsContainer;
	}

	public void LoadData(DBTreeNode currentObjectDbTreeNode, Form parentForm)
	{
		allDataProcesses = DB.DataProcess.GetProcessesByProcessorId(currentObjectDbTreeNode.Id, currentObjectDbTreeNode.ObjectType);
		if (!allDataProcesses.Any())
		{
			AddDefaultProcess(currentObjectDbTreeNode, parentForm);
			if (currentObjectDbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.View)
			{
				LoadProcessesFlows(currentObjectDbTreeNode.DatabaseId, parentForm);
			}
			return;
		}
		if (currentObjectDbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.View)
		{
			List<DataProcessRow> list = allDataProcesses;
			if (list != null && !list.Any())
			{
				AddDefaultProcess(currentObjectDbTreeNode, parentForm);
			}
		}
		LoadProcessesFlows(currentObjectDbTreeNode.DatabaseId, parentForm);
		LoadFlowColumns();
	}

	private void LoadProcessesFlows(int databaseId, Form parentForm)
	{
		foreach (DataProcessRow allDataProcess in AllDataProcesses)
		{
			BindingList<DataFlowRow> flowsByProcessId = DB.DataFlows.GetFlowsByProcessId(allDataProcess.Id, databaseId, parentForm);
			if (flowsByProcessId == null)
			{
				continue;
			}
			foreach (DataFlowRow item in flowsByProcessId)
			{
				item.Process = allDataProcess;
			}
			allDataProcess.InflowRows = new BindingList<DataFlowRow>(flowsByProcessId.Where((DataFlowRow x) => x.Direction == FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.IN)).ToList());
			allDataProcess.OutflowRows = new BindingList<DataFlowRow>(flowsByProcessId.Where((DataFlowRow x) => x.Direction == FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.OUT)).ToList());
		}
	}

	private void AddDefaultProcess(DBTreeNode currentObjectDbTreeNode, Form parentForm)
	{
		DataProcessRow dataProcessRow = DataProcessRow.AddDefaultProcess(currentObjectDbTreeNode, parentForm);
		if (dataProcessRow != null)
		{
			allDataProcesses.Add(dataProcessRow);
		}
	}

	private void LoadFlowColumns()
	{
		List<DataLineageColumnsFlowRow> dataColumnsFlows = DB.DataFlows.GetDataColumnsFlows(AllDataProcesses);
		foreach (DataProcessRow process in AllDataProcesses)
		{
			process.Columns = dataColumnsFlows.Where((DataLineageColumnsFlowRow x) => x.ProcessRow == process).ToList();
		}
	}

	public bool SaveData(List<DataFlowRow> deletedDataFlowRows, List<DataLineageColumnsFlowRow> deletedColumnRows, Form owner)
	{
		return SaveProcesses(owner) & SaveDataFlows(deletedDataFlowRows, owner) & SaveFlowsColumns(deletedColumnRows, owner);
	}

	private bool SaveProcesses(Form owner)
	{
		try
		{
			foreach (DataProcessRow addedProcess in GetAddedProcesses())
			{
				int? num = DB.DataProcess.Insert(addedProcess, owner);
				if (num.HasValue && num.HasValue)
				{
					addedProcess.Id = num.Value;
					addedProcess.RowState = ManagingRowsEnum.ManagingRows.Added;
					DataFlowHelper.SetProcessIdInProcessFlows(addedProcess);
					TrackingRunner.Track(delegate
					{
						TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilderEventSpecificObjectType(new TrackingUserParameters(), new TrackingDataedoParameters(), SharedObjectTypeEnum.TypeToString(addedProcess.ParentObjectType)), TrackingEventEnum.DataLineageProcessAdded);
					});
				}
			}
			foreach (DataProcessRow changedProcess in GetChangedProcesses())
			{
				DB.DataProcess.Update(changedProcess, owner);
			}
			IEnumerable<DataProcessRow> deletedProcesses = GetDeletedProcesses();
			if (deletedProcesses.Any())
			{
				DB.DataProcess.Delete(deletedProcesses.Select((DataProcessRow x) => x.Id), owner);
			}
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while saving data processes.", owner);
			return false;
		}
	}

	private bool SaveDataFlows(List<DataFlowRow> deletedDataFlowRows, Form owner)
	{
		try
		{
			bool flag = true;
			foreach (DataProcessRow allDataProcess in AllDataProcesses)
			{
				foreach (DataFlowRow item in allDataProcess.InflowRows.Where((DataFlowRow x) => x.RowState == ManagingRowsEnum.ManagingRows.ForAdding))
				{
					flag &= DataFlowHelper.InsertDataFlow(item, owner);
				}
				foreach (DataFlowRow item2 in allDataProcess.OutflowRows.Where((DataFlowRow x) => x.RowState == ManagingRowsEnum.ManagingRows.ForAdding))
				{
					flag &= DataFlowHelper.InsertDataFlow(item2, owner);
				}
			}
			AllDataFlowsContainer obj = allDataFlowsContainer;
			if (obj != null && obj.ReferencedInflows?.Count > 0)
			{
				foreach (DataFlowRow item3 in allDataFlowsContainer.ReferencedInflows.Where((DataFlowRow x) => x.RowState == ManagingRowsEnum.ManagingRows.ForAdding))
				{
					flag &= DataFlowHelper.InsertDataFlow(item3, owner);
				}
			}
			AllDataFlowsContainer obj2 = allDataFlowsContainer;
			if (obj2 != null && obj2.ReferencedOutflows?.Count > 0)
			{
				foreach (DataFlowRow item4 in allDataFlowsContainer.ReferencedOutflows.Where((DataFlowRow x) => x.RowState == ManagingRowsEnum.ManagingRows.ForAdding))
				{
					flag &= DataFlowHelper.InsertDataFlow(item4, owner);
				}
			}
			IEnumerable<int> enumerable = deletedDataFlowRows?.Select((DataFlowRow x) => x.Id);
			if (enumerable.Any())
			{
				flag &= DB.DataFlows.Delete(enumerable, owner);
			}
			return flag;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while saving data flows.", owner);
			return false;
		}
	}

	private bool SaveFlowsColumns(List<DataLineageColumnsFlowRow> deletedColumnRows, Form owner)
	{
		try
		{
			foreach (DataProcessRow allDataProcess in AllDataProcesses)
			{
				if (allDataProcess.Columns == null || allDataProcess.Columns.Count == 0)
				{
					continue;
				}
				foreach (DataLineageColumnsFlowRow item in allDataProcess.Columns.Where((DataLineageColumnsFlowRow c) => c.IsRowComplete && c.RowState == ManagingRowsEnum.ManagingRows.ForAdding))
				{
					int? num = DB.DataFlows.InsertColumnRow(item, owner);
					if (num.HasValue)
					{
						item.RowState = ManagingRowsEnum.ManagingRows.Added;
						item.DataColumnsFlowId = num.Value;
					}
				}
				foreach (DataLineageColumnsFlowRow item2 in allDataProcess.Columns.Where((DataLineageColumnsFlowRow c) => !c.IsRowEmpty && c.RowState == ManagingRowsEnum.ManagingRows.Updated))
				{
					DB.DataFlows.UpdateDataColumnFlow(item2, owner);
					item2.RowState = ManagingRowsEnum.ManagingRows.Unchanged;
				}
			}
			DB.DataFlows.DeleteEmptyDataColumnsFlowsRecords();
			if (deletedColumnRows != null)
			{
				DB.DataFlows.DeleteDataColumnsFlows(deletedColumnRows.Select((DataLineageColumnsFlowRow x) => x.DataColumnsFlowId).ToList());
			}
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while saving data flows columns.", owner);
			return false;
		}
	}

	public void Add(DataProcessRow dataProcessRow)
	{
		allDataProcesses.Add(dataProcessRow);
		SortProcesses();
	}

	public void SortProcesses()
	{
		allDataProcesses = allDataProcesses.OrderBy((DataProcessRow x) => x.Name).ToList();
	}

	public void Remove(DataProcessRow dataProcessRow)
	{
		if (dataProcessRow.RowState != ManagingRowsEnum.ManagingRows.ForAdding)
		{
			deletedDataProcesses.Add(dataProcessRow);
		}
		allDataProcesses.Remove(dataProcessRow);
		dataProcessRow.RowState = ManagingRowsEnum.ManagingRows.Deleted;
	}

	public bool ProcessAlreadyExistsCaseSensitive(string processName)
	{
		return allDataProcesses.Where((DataProcessRow x) => x.Name.Equals(processName, StringComparison.Ordinal) && x.RowState != ManagingRowsEnum.ManagingRows.Deleted).Any();
	}

	public bool ProcessAlreadyExistsIgnoreCase(string processName)
	{
		return allDataProcesses.Where((DataProcessRow x) => x.Name.Equals(processName, StringComparison.OrdinalIgnoreCase) && x.RowState != ManagingRowsEnum.ManagingRows.Deleted).Any();
	}

	public bool IsProcessPendingDeletion(string processName)
	{
		return deletedDataProcesses.Where((DataProcessRow x) => x.Name.Equals(processName, StringComparison.OrdinalIgnoreCase)).Any();
	}

	public IEnumerable<DataProcessRow> GetDeletedProcesses()
	{
		return deletedDataProcesses;
	}

	public IEnumerable<DataProcessRow> GetAddedProcesses()
	{
		return AllDataProcesses.Where((DataProcessRow p) => p.RowState == ManagingRowsEnum.ManagingRows.ForAdding);
	}

	public IEnumerable<DataProcessRow> GetChangedProcesses()
	{
		return AllDataProcesses.Where((DataProcessRow p) => p.RowState == ManagingRowsEnum.ManagingRows.Updated);
	}

	public void ClearData()
	{
		deletedDataProcesses.Clear();
	}

	public void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
	{
		object node = info.Node;
		if (node != null)
		{
			if (node is DataProcessRow)
			{
				info.Children = null;
			}
			else if (node is AllDataFlowsContainer)
			{
				info.Children = allDataProcesses;
			}
		}
	}

	public void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
	{
		object node = info.Node;
		if (node != null && info.Column.FieldName == "Name")
		{
			info.CellData = (node as IName)?.Name;
		}
	}

	public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
	{
	}

	public int GetAllModifiedColumnsCount()
	{
		return AllDataProcesses.SelectMany((DataProcessRow p) => p.Columns.Where((DataLineageColumnsFlowRow c) => (c.IsRowComplete && c.RowState == ManagingRowsEnum.ManagingRows.ForAdding) || c.RowState == ManagingRowsEnum.ManagingRows.Updated)).Count();
	}
}
