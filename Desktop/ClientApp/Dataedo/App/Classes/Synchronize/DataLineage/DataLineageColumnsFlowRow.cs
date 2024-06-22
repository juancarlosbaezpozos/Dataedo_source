using System;
using System.Drawing;
using System.Linq;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Data.DataLineage;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize.DataLineage;

public class DataLineageColumnsFlowRow : StatedObject
{
	public Guid Guid { get; } = Guid.NewGuid();


	public int DataColumnsFlowId { get; set; } = -1;


	public int InflowId => InflowRow?.Id ?? (-1);

	public int InflowColumnId { get; set; } = -1;


	public string InflowColumnName { get; set; }

	public string InflowObjectName => InflowRow?.ObjectCaptionInline;

	public Image InflowColumnImage { get; set; } = Resources.blank_16;


	public Image InflowObjectImage => InflowRow?.Icon ?? Resources.blank_16;

	public DataFlowRow InflowRow { get; set; }

	public int OutflowId => OutflowRow?.Id ?? (-1);

	public int OutflowColumnId { get; set; } = -1;


	public string OutflowColumnName { get; set; }

	public string OutflowObjectName => OutflowRow?.ObjectCaptionInline;

	public Image OutflowColumnImage { get; set; } = Resources.blank_16;


	public Image OutflowObjectImage => OutflowRow?.Icon ?? Resources.blank_16;

	public DataFlowRow OutflowRow { get; set; }

	public DataProcessRow ProcessRow { get; set; }

	public Image ConnectionIcon
	{
		get
		{
			if (!IsRowComplete)
			{
				return Resources.blank_16;
			}
			return Resources.flow_direction_16;
		}
	}

	public bool IsRowComplete
	{
		get
		{
			if (InflowId >= 0 && InflowColumnId >= 0 && OutflowId >= 0)
			{
				return OutflowColumnId >= 0;
			}
			return false;
		}
	}

	public bool IsRowEmpty
	{
		get
		{
			if (InflowId < 0 && InflowColumnId < 0 && OutflowId < 0)
			{
				return OutflowColumnId < 0;
			}
			return false;
		}
	}

	public DataLineageColumnsFlowRow()
	{
	}

	public DataLineageColumnsFlowRow(DataLineageColumnsFlowRow dataLineageColumnsFlowRow, bool copyInflows)
	{
		if (copyInflows)
		{
			InflowRow = dataLineageColumnsFlowRow.InflowRow;
			InflowColumnId = dataLineageColumnsFlowRow.InflowColumnId;
			InflowColumnName = dataLineageColumnsFlowRow.InflowColumnName;
			InflowColumnImage = dataLineageColumnsFlowRow.InflowColumnImage;
		}
		else
		{
			OutflowRow = dataLineageColumnsFlowRow.OutflowRow;
			OutflowColumnId = dataLineageColumnsFlowRow.OutflowColumnId;
			OutflowColumnName = dataLineageColumnsFlowRow.OutflowColumnName;
			OutflowColumnImage = dataLineageColumnsFlowRow.OutflowColumnImage;
		}
		ProcessRow = dataLineageColumnsFlowRow.ProcessRow;
		base.RowState = ManagingRowsEnum.ManagingRows.ForAdding;
	}

	public DataLineageColumnsFlowRow(DataColumnsFlowObject dataColumnsFlowObject, DataProcessRow dataProcessRow)
	{
		if (dataColumnsFlowObject != null)
		{
			ProcessRow = dataProcessRow;
			InflowRow = dataProcessRow?.InflowRows?.FirstOrDefault((DataFlowRow x) => x.Id == dataColumnsFlowObject.InflowId);
			OutflowRow = dataProcessRow?.OutflowRows?.FirstOrDefault((DataFlowRow x) => x.Id == dataColumnsFlowObject.OutflowId);
			DataColumnsFlowId = dataColumnsFlowObject.DataColumnsFlowId;
			InflowColumnId = dataColumnsFlowObject.InflowColumnId;
			InflowColumnName = dataColumnsFlowObject.InflowColumnName;
			if (!string.IsNullOrEmpty(dataColumnsFlowObject.InflowParameterMode))
			{
				ParameterRow.ModeEnum? mode = ParameterRow.GetMode(dataColumnsFlowObject.InflowParameterMode);
				InflowColumnImage = Icons.GetParameterIcon(mode, null);
			}
			else
			{
				InflowColumnImage = IconsSupport.GetObjectIcon(SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Column), dataColumnsFlowObject.InflowColumnItemType, null);
			}
			OutflowColumnId = dataColumnsFlowObject.OutflowColumnId;
			OutflowColumnName = dataColumnsFlowObject.OutflowColumnName;
			if (!string.IsNullOrEmpty(dataColumnsFlowObject.OutflowParameterMode))
			{
				ParameterRow.ModeEnum? mode2 = ParameterRow.GetMode(dataColumnsFlowObject.OutflowParameterMode);
				OutflowColumnImage = Icons.GetParameterIcon(mode2, null);
			}
			else
			{
				OutflowColumnImage = IconsSupport.GetObjectIcon(SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Column), dataColumnsFlowObject.OutflowColumnItemType, null);
			}
		}
	}
}
