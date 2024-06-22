using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.MenuTree;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.UserControls.DataLineage;

public class AllDataFlowsContainer : ProcessesContainer
{
	public BindingList<DataFlowRow> InflowRows = new BindingList<DataFlowRow>();

	public BindingList<DataFlowRow> OutflowRows = new BindingList<DataFlowRow>();

	public BindingList<DataFlowRow> ReferencedInflows = new BindingList<DataFlowRow>();

	public BindingList<DataFlowRow> ReferencedOutflows = new BindingList<DataFlowRow>();

	public List<DataLineageColumnsFlowRow> Columns => base.Processes.SelectMany((DataProcessRow x) => x.Columns).ToList();

	public AllDataFlowsContainer(DataProcessesCollection dataProcessesCollection)
		: base(dataProcessesCollection)
	{
		base.Name = "All Processes";
	}

	public void RefreshInflowsAndOutflows(DBTreeNode currentObjectNode, Form owner)
	{
		GetObjectReferencedFlows(currentObjectNode, owner);
		InflowRows = ((base.Processes != null) ? new BindingList<DataFlowRow>((from x in ReferencedInflows.Concat(base.Processes.Where((DataProcessRow x) => x.InflowRows != null).SelectMany((DataProcessRow x) => x.InflowRows))
			where x.RowState != ManagingRowsEnum.ManagingRows.Deleted
			select x).ToList()) : new BindingList<DataFlowRow>());
		OutflowRows = ((base.Processes != null) ? new BindingList<DataFlowRow>((from x in ReferencedOutflows.Concat(base.Processes.Where((DataProcessRow x) => x.OutflowRows != null && x.RowState != ManagingRowsEnum.ManagingRows.Deleted).SelectMany((DataProcessRow x) => x.OutflowRows))
			where x.RowState != ManagingRowsEnum.ManagingRows.Deleted
			select x).ToList()) : new BindingList<DataFlowRow>());
	}

	private void GetObjectReferencedFlows(DBTreeNode currentObjectNode, Form owner)
	{
		if (ReferencedOutflows == null || !ReferencedOutflows.Any())
		{
			ReferencedOutflows = DB.DataFlows.GetFlowsRelatedToObject(currentObjectNode.Id, SharedObjectTypeEnum.TypeToString(currentObjectNode.ObjectType), FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.IN), currentObjectNode.DatabaseId, owner);
		}
		if (ReferencedInflows != null && ReferencedInflows.Any())
		{
			return;
		}
		ReferencedInflows = DB.DataFlows.GetFlowsRelatedToObject(currentObjectNode.Id, SharedObjectTypeEnum.TypeToString(currentObjectNode.ObjectType), FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.OUT), currentObjectNode.DatabaseId, owner);
		if (currentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.View)
		{
			DataFlowRow item = ReferencedInflows?.Where((DataFlowRow x) => x.ObjectId == currentObjectNode.Id)?.FirstOrDefault();
			ReferencedInflows?.Remove(item);
		}
	}
}
