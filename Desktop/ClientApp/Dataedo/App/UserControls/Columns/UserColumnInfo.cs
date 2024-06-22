using System;
using DevExpress.XtraGrid.Columns;

namespace Dataedo.App.UserControls.Columns;

[Serializable]
public class UserColumnInfo
{
	public string FieldName { get; set; }

	public bool Visible { get; set; }

	public int VisibleIndex { get; set; }

	public int Width { get; set; }

	public UserColumnInfo()
	{
	}

	public UserColumnInfo(GridColumn column)
	{
		FieldName = column.FieldName;
		Visible = column.Visible;
		VisibleIndex = column.VisibleIndex;
		Width = column.Width;
	}
}
