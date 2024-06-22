using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.UserControls.Columns;

[Serializable]
public class UserViewInfo
{
	[XmlArray]
	[XmlArrayItem(Type = typeof(UserColumnInfo), ElementName = "UserColumn")]
	public List<UserColumnInfo> UserColumns;

	public string ViewName { get; set; }

	public void SaveColumns(GridView gridView)
	{
		UserColumns = gridView.Columns.Select((GridColumn x) => new UserColumnInfo(x)).ToList();
	}

	public void LoadColumns(GridView gridView)
	{
		gridView.BeginUpdate();
		foreach (GridColumn item in from GridColumn x in gridView.Columns
			where UserColumns.Where((UserColumnInfo y) => y.FieldName == x.FieldName && !y.Visible).Any()
			select x)
		{
			item.Visible = false;
		}
		foreach (UserColumnInfo item2 in from x in UserColumns
			where x.Visible
			orderby x.VisibleIndex
			select x)
		{
			GridColumn gridColumn = gridView.Columns[item2.FieldName];
			if (gridColumn != null)
			{
				gridColumn.VisibleIndex = item2.VisibleIndex;
				gridColumn.Width = item2.Width;
			}
		}
		gridView.EndUpdate();
	}
}
