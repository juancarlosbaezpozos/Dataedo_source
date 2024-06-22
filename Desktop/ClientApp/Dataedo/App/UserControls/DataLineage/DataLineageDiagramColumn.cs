using System;
using System.Collections.Generic;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.App.Enums;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram;

namespace Dataedo.App.UserControls.DataLineage;

public class DataLineageDiagramColumn : DiagramShape, IDiagramItem
{
	public Guid Guid { get; } = Guid.NewGuid();


	public DataFlowRow DataFlow { get; set; }

	public List<DataLineageColumnsFlowRow> DataLineageColumnsFlowRows { get; set; } = new List<DataLineageColumnsFlowRow>();


	public virtual FlowDirectionEnum.Direction? FlowDirection => FlowDirectionEnum.StringToType(DataFlow?.Direction);
}
