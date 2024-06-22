using Dataedo.Shared.Enums;
using DevExpress.XtraGrid;

namespace Dataedo.App.Tools;

public class ToolTipData
{
	public GridControl GridControl { get; set; }

	public SharedObjectTypeEnum.ObjectType ObjectType { get; set; }

	public int ColumnVisibleIndex { get; set; }

	public ToolTipData(GridControl gridControl, SharedObjectTypeEnum.ObjectType objectType, int columnVisibleIndex)
	{
		GridControl = gridControl;
		ObjectType = objectType;
		ColumnVisibleIndex = columnVisibleIndex;
	}
}
