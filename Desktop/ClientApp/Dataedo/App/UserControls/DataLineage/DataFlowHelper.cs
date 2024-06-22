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
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.App.UserControls.ObjectBrowser;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.Model.Data.DataLineage;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.UserControls.DataLineage;

public static class DataFlowHelper
{
	public static void TrackFlowAdded(SharedObjectTypeEnum.ObjectType draggedObjectType, SharedObjectTypeEnum.ObjectType currentObjectType)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilderEventSpecificObjectTypeDraggedObjectType(new TrackingUserParameters(), new TrackingDataedoParameters(), SharedObjectTypeEnum.TypeToString(currentObjectType), SharedObjectTypeEnum.TypeToString(draggedObjectType)), TrackingEventEnum.DataLineageFlowAdded);
		});
	}

	public static void TrackColumnsCount(int modifiedColumnsCount, SharedObjectTypeEnum.ObjectType currentObjectType)
	{
		if (modifiedColumnsCount > 0)
		{
			TrackingRunner.Track(delegate
			{
				TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilderEventSpecificObjectTypeColumnsCount(new TrackingUserParameters(), new TrackingDataedoParameters(), SharedObjectTypeEnum.TypeToString(currentObjectType), modifiedColumnsCount), TrackingEventEnum.DataLineageColumnFlowAdded);
			});
		}
	}

	public static void SetDragDropEffect(DragEventArgs e, MetadataEditorUserControl metadataEditorUserControl, DBTreeNode currentObjectNode, FlowDirectionEnum.Direction flowDirection, bool isAllDataFlowsTab, ref string tooltipText)
	{
		IFlowDraggable flowDraggable = e.Data.GetData(typeof(ObjectBrowserItem)) as IFlowDraggable;
		if (flowDraggable == null && e.Data.GetData(typeof(TreeListNode)) is TreeListNode treeListNode)
		{
			flowDraggable = metadataEditorUserControl?.TreeListHelpers.GetNode(treeListNode);
		}
		bool flag = currentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.View && flowDraggable.ObjectType == SharedObjectTypeEnum.ObjectType.View && flowDirection == FlowDirectionEnum.Direction.OUT && currentObjectNode.Id != flowDraggable.Id;
		bool flag2 = CanBeDropped(flowDraggable.ObjectType, currentObjectNode.ObjectType, flowDirection, isAllDataFlowsTab, ref tooltipText);
		if (flowDraggable == null || !flowDraggable.IsNormalObject || flowDraggable.Deleted || !flag2 || flag)
		{
			e.Effect = DragDropEffects.None;
		}
		else
		{
			e.Effect = DragDropEffects.Link;
		}
	}

	public static bool CanBeDropped(SharedObjectTypeEnum.ObjectType draggedObjectType, SharedObjectTypeEnum.ObjectType currentObjectType, FlowDirectionEnum.Direction? flowDirection, bool? isAllDataFlowsTab, ref string tooltipText)
	{
		tooltipText = string.Empty;
		if (!SharedObjectTypeEnum.IsRegularObject(draggedObjectType))
		{
			return false;
		}
		if (!SharedObjectTypeEnum.IsRegularObject(currentObjectType))
		{
			return false;
		}
		switch (currentObjectType)
		{
		case SharedObjectTypeEnum.ObjectType.Table:
		case SharedObjectTypeEnum.ObjectType.Structure:
			switch (draggedObjectType)
			{
			case SharedObjectTypeEnum.ObjectType.Table:
			case SharedObjectTypeEnum.ObjectType.Structure:
				return false;
			case SharedObjectTypeEnum.ObjectType.View:
				if (flowDirection == FlowDirectionEnum.Direction.IN)
				{
					tooltipText = SetTooltipMessage(draggedObjectType);
					return false;
				}
				break;
			}
			break;
		case SharedObjectTypeEnum.ObjectType.View:
			if (flowDirection == FlowDirectionEnum.Direction.OUT && draggedObjectType != SharedObjectTypeEnum.ObjectType.View)
			{
				return false;
			}
			break;
		}
		return true;
	}

	public static string SetTooltipMessage(SharedObjectTypeEnum.ObjectType draggedObjectType)
	{
		if (draggedObjectType == SharedObjectTypeEnum.ObjectType.View)
		{
			return "View can't save data";
		}
		return string.Empty;
	}

	public static bool IsAlreadyOnList(IFlowDraggable dBTreeNode, BindingList<DataFlowRow> sourceList)
	{
		if (sourceList.Any((DataFlowRow x) => x.ObjectId == dBTreeNode.Id && x.ObjectType == dBTreeNode.ObjectType))
		{
			return true;
		}
		return false;
	}

	private static bool IsFlowAlreadyInProcess(IFlowDraggable nodeInNewFlow, DataProcessRow dataProcess, BindingList<DataFlowRow> sourceList)
	{
		if (sourceList.Any((DataFlowRow x) => x.ObjectId == nodeInNewFlow.Id && x.ObjectType == nodeInNewFlow.ObjectType && x.ProcessId == dataProcess.Id))
		{
			return true;
		}
		return false;
	}

	public static void CreateNewFlowAndAddToGrid(DataProcessRow dataProcess, IFlowDraggable nodeInNewFlow, DBTreeNode currentObjectNode, bool isInFlow, BindingList<DataFlowRow> sourceList, Form owner, AllDataFlowsContainer allDataFlowsContainer)
	{
		CreateNewFlow(dataProcess, nodeInNewFlow, nodeInNewFlow, currentObjectNode, isInFlow, allDataFlowsContainer).UpdateIcon();
	}

	public static DataFlowRow CreateNewFlow(DataProcessRow dataProcess, IFlowDraggable nodeInNewFlow, IFlowDraggable objectDraggedToFlow, DBTreeNode currentObjectNode, bool isInFlow, AllDataFlowsContainer allDataFlowsContainer)
	{
		if (objectDraggedToFlow == null)
		{
			objectDraggedToFlow = nodeInNewFlow;
		}
		string databaseName = ((objectDraggedToFlow.DatabaseId == currentObjectNode?.DatabaseId) ? null : objectDraggedToFlow.DatabaseTitle);
		DataFlowRow dataFlowRow = new DataFlowRow(dataProcess.Id, dataProcess.Name, dataProcess.ParentId == currentObjectNode.Id, isInFlow ? FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.IN) : FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.OUT), nodeInNewFlow.Id, nodeInNewFlow.ObjectType, nodeInNewFlow.Subtype, nodeInNewFlow.Source ?? UserTypeEnum.UserType.USER, UserTypeEnum.UserType.USER, databaseName, objectDraggedToFlow.DatabaseId, objectDraggedToFlow.BaseName, objectDraggedToFlow.Title, objectDraggedToFlow.Schema, objectDraggedToFlow.ShowSchema);
		dataFlowRow.UpdateIcon(objectDraggedToFlow.ObjectType, objectDraggedToFlow.Subtype, objectDraggedToFlow.Source ?? UserTypeEnum.UserType.USER);
		dataFlowRow.Process = dataProcess;
		dataFlowRow.RowState = ManagingRowsEnum.ManagingRows.ForAdding;
		if (currentObjectNode.Id == dataProcess.ParentId)
		{
			if (isInFlow)
			{
				dataProcess.InflowRows.Add(dataFlowRow);
			}
			else
			{
				dataProcess.OutflowRows.Add(dataFlowRow);
			}
		}
		else if (allDataFlowsContainer != null)
		{
			if (isInFlow)
			{
				allDataFlowsContainer.ReferencedOutflows.Add(dataFlowRow);
			}
			else
			{
				allDataFlowsContainer.ReferencedInflows.Add(dataFlowRow);
			}
		}
		return dataFlowRow;
	}

	public static bool ProcessDraggingToAllDataFlows(IFlowDraggable draggedObjectNode, DBTreeNode currentObjectNode, bool isInFlow, AllDataFlowsContainer allDataFlowsContainer, BindingList<DataFlowRow> inflowsList, BindingList<DataFlowRow> outflowsList, Form owner)
	{
		List<DataProcessRow> processes = allDataFlowsContainer.Processes;
		List<DataProcessRow> processesByProcessorId = DB.DataProcess.GetProcessesByProcessorId(draggedObjectNode.Id, draggedObjectNode.ObjectType);
		if (!processesByProcessorId.Any())
		{
			DataProcessRow dataProcessRow = DataProcessRow.AddDefaultProcess(draggedObjectNode, owner);
			if (dataProcessRow != null)
			{
				processesByProcessorId.Add(dataProcessRow);
			}
		}
		IFlowDraggable objectDraggedToFlow = null;
		DataProcessRow dataProcess;
		IFlowDraggable nodeInNewFlow;
		if (currentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table || currentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.Structure)
		{
			isInFlow = !isInFlow;
			if (processesByProcessorId.Count == 1)
			{
				dataProcess = processesByProcessorId.First();
				nodeInNewFlow = currentObjectNode;
			}
			else
			{
				GetProcessRow(currentObjectNode, draggedObjectNode, processesByProcessorId, currentObjectNode, processes, out dataProcess, out nodeInNewFlow, out objectDraggedToFlow, ref isInFlow, inflowsList, outflowsList, owner);
				if (dataProcess == null)
				{
					return false;
				}
			}
			objectDraggedToFlow = draggedObjectNode;
			CreateNewFlow(dataProcess, nodeInNewFlow, objectDraggedToFlow, currentObjectNode, isInFlow, allDataFlowsContainer);
			return true;
		}
		if (draggedObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table || draggedObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.Structure)
		{
			if (processes.Count == 1)
			{
				dataProcess = processes.First();
				nodeInNewFlow = draggedObjectNode;
			}
			else
			{
				GetProcessRow(currentObjectNode, currentObjectNode, processes, draggedObjectNode, processesByProcessorId, out dataProcess, out nodeInNewFlow, out objectDraggedToFlow, ref isInFlow, inflowsList, outflowsList, owner);
				if (dataProcess == null)
				{
					return false;
				}
			}
			CreateNewFlowAndAddToGrid(dataProcess, nodeInNewFlow, currentObjectNode, isInFlow, inflowsList, owner, allDataFlowsContainer);
			return true;
		}
		GetProcessRow(currentObjectNode, currentObjectNode, processes, draggedObjectNode, processesByProcessorId, out dataProcess, out nodeInNewFlow, out objectDraggedToFlow, ref isInFlow, inflowsList, outflowsList, owner);
		if (dataProcess == null)
		{
			return false;
		}
		CreateNewFlow(dataProcess, nodeInNewFlow, objectDraggedToFlow, currentObjectNode, isInFlow, allDataFlowsContainer);
		return true;
	}

	private static void GetProcessRow(DBTreeNode currentObjectNode, IFlowDraggable firstObject, List<DataProcessRow> firstObjectProcesses, IFlowDraggable secondObject, List<DataProcessRow> secondObjectProcesses, out DataProcessRow dataProcess, out IFlowDraggable nodeInNewFlow, out IFlowDraggable objectDraggedToFlow, ref bool isInFlow, BindingList<DataFlowRow> inflowsList, BindingList<DataFlowRow> outflowsList, Form owner)
	{
		dataProcess = null;
		nodeInNewFlow = null;
		objectDraggedToFlow = null;
		SelectProcessForm selectProcessForm = new SelectProcessForm();
		selectProcessForm.SetParameters(DataFlowRow.GetFlowCaption(firstObject, currentObjectNode?.DatabaseId).Replace(Environment.NewLine, " "), firstObjectProcesses, firstObject, DataFlowRow.GetFlowCaption(secondObject, currentObjectNode?.DatabaseId).Replace(Environment.NewLine, " "), secondObjectProcesses, secondObject, isInFlow);
		if (selectProcessForm.ShowDialog(owner) == DialogResult.OK)
		{
			dataProcess = selectProcessForm.SelectedProcessRow;
			isInFlow = selectProcessForm.IsInFlow;
			bool flag = isInFlow;
			if (dataProcess.ParentId != currentObjectNode.Id)
			{
				flag = !isInFlow;
			}
			if (dataProcess.ParentId == secondObject.Id && dataProcess.ParentObjectType == secondObject.ObjectType)
			{
				nodeInNewFlow = firstObject;
				objectDraggedToFlow = secondObject;
			}
			else if (dataProcess.ParentId == firstObject.Id && dataProcess.ParentObjectType == firstObject.ObjectType)
			{
				nodeInNewFlow = secondObject;
				objectDraggedToFlow = secondObject;
			}
			if (CheckIfFlowExists(dataProcess, isInFlow ? FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.IN) : FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.OUT), nodeInNewFlow.ObjectType, nodeInNewFlow.Id) || IsFlowAlreadyInProcess(nodeInNewFlow, dataProcess, flag ? inflowsList : outflowsList))
			{
				GeneralMessageBoxesHandling.Show("Flow already exists", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, owner);
				dataProcess = null;
				nodeInNewFlow = null;
			}
		}
	}

	private static bool CheckIfFlowExists(DataProcessRow dataProcessRow, string direction, SharedObjectTypeEnum.ObjectType objectType, int objectId)
	{
		bool flag = false;
		if (direction == FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.IN))
		{
			flag = dataProcessRow.InflowRows?.Any((DataFlowRow x) => x.ObjectType == objectType && x.ObjectId == objectId) ?? false;
		}
		else if (direction == FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.OUT))
		{
			flag = dataProcessRow.OutflowRows?.Any((DataFlowRow x) => x.ObjectType == objectType && x.ObjectId == objectId) ?? false;
		}
		if (!flag)
		{
			return DB.DataFlows.CheckIfFlowExists(dataProcessRow.Id, direction, objectType, objectId);
		}
		return true;
	}

	public static bool DragDrop(DragEventArgs e, DBTreeNode currentObjectNode, MetadataEditorUserControl metadataEditorUserControl, BindingList<DataFlowRow> inflowList, BindingList<DataFlowRow> outflowsList, DataProcessRow dataProcessRow, AllDataFlowsContainer allDataFlowsContainer, bool isInflowsGridControl, Form owner)
	{
		try
		{
			IFlowDraggable flowDraggable = e.Data.GetData(typeof(ObjectBrowserItem)) as IFlowDraggable;
			if (flowDraggable == null && e.Data.GetData(typeof(TreeListNode)) is TreeListNode treeListNode)
			{
				flowDraggable = metadataEditorUserControl?.TreeListHelpers.GetNode(treeListNode);
			}
			if (flowDraggable == null || !flowDraggable.IsNormalObject)
			{
				return false;
			}
			if (currentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.View && currentObjectNode.Id != flowDraggable.Id && !isInflowsGridControl)
			{
				return false;
			}
			if (dataProcessRow != null)
			{
				if (IsAlreadyOnList(flowDraggable, isInflowsGridControl ? inflowList : outflowsList))
				{
					GeneralMessageBoxesHandling.Show("Object already exists.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, owner);
					return false;
				}
				CreateNewFlowAndAddToGrid(dataProcessRow, flowDraggable, currentObjectNode, isInflowsGridControl, inflowList, owner, allDataFlowsContainer);
				return true;
			}
			if (allDataFlowsContainer != null)
			{
				return ProcessDraggingToAllDataFlows(flowDraggable, currentObjectNode, isInflowsGridControl, allDataFlowsContainer, inflowList, outflowsList, owner);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, owner);
		}
		return true;
	}

	public static void SetProcessIdInProcessFlows(DataProcessRow addedProcess)
	{
		if (addedProcess != null && addedProcess.InflowRows.Any())
		{
			foreach (DataFlowRow item in addedProcess.InflowRows.Where((DataFlowRow x) => x.RowState == ManagingRowsEnum.ManagingRows.ForAdding))
			{
				item.ProcessId = addedProcess.Id;
			}
		}
		if (addedProcess == null || !addedProcess.OutflowRows.Any())
		{
			return;
		}
		foreach (DataFlowRow item2 in addedProcess.OutflowRows.Where((DataFlowRow x) => x.RowState == ManagingRowsEnum.ManagingRows.ForAdding))
		{
			item2.ProcessId = addedProcess.Id;
		}
	}

	public static bool InsertDataFlow(DataFlowRow row, Form owner)
	{
		int? num = DB.DataFlows.Insert(row, owner);
		if (!num.HasValue)
		{
			return false;
		}
		row.Id = num.Value;
		row.RowState = ManagingRowsEnum.ManagingRows.Added;
		TrackFlowAdded(row.ObjectType, row.Process.ParentObjectType.Value);
		return true;
	}

	public static bool SynchronizeDataFlow(DataFlowRow row, Form owner)
	{
		if (row == null)
		{
			return false;
		}
		BindingList<DataFlowRow> flowsByProcessId = DB.DataFlows.GetFlowsByProcessId(row.ProcessId, null, owner);
		if (flowsByProcessId == null)
		{
			return false;
		}
		DataFlowRow dataFlowRow = flowsByProcessId.Where((DataFlowRow x) => x.Direction == row.Direction && x.ObjectType == row.ObjectType && x.ObjectId == row.ObjectId && x.Source == row.Source).FirstOrDefault();
		if (dataFlowRow != null)
		{
			row.Id = dataFlowRow.Id;
			row.RowState = ManagingRowsEnum.ManagingRows.Unchanged;
			return true;
		}
		int? num = DB.DataFlows.Insert(row, owner);
		if (!num.HasValue)
		{
			return false;
		}
		row.Id = num.Value;
		row.RowState = ManagingRowsEnum.ManagingRows.Added;
		TrackFlowAdded(row.ObjectType, row.Process.ParentObjectType.Value);
		return true;
	}
}
