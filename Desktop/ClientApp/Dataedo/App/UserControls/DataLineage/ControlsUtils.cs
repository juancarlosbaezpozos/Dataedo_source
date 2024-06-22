using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.UI;
using Dataedo.Model.Data.DataLineage;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;

namespace Dataedo.App.UserControls.DataLineage;

public static class ControlsUtils
{
	public static void AddInfoText(GraphicsCache cache, Rectangle emptySpaceRectangle, string text, int rectangleHeight)
	{
		StringFormat strFormat = new StringFormat
		{
			Alignment = StringAlignment.Center,
			LineAlignment = StringAlignment.Center
		};
		Rectangle bounds = emptySpaceRectangle.TopBoundRect(rectangleHeight);
		cache.DrawString(text, cache.GetFont(new Font("Tahoma", 9f), FontStyle.Regular), cache.GetSolidBrush(SkinsManager.CurrentSkin.InfoTextForeColor), bounds, strFormat);
	}

	public static void LoadColumns(RepositoryItemGridLookUpEdit lookUpEdit, DataFlowRow dataFlowRow, Form owner)
	{
		if (dataFlowRow == null)
		{
			lookUpEdit.DataSource = null;
			return;
		}
		IEnumerable<ColumnForDataLineageColumnsDropdown> collection = from column in DB.DataFlows.GetColumnsByObject(dataFlowRow.ObjectId, dataFlowRow.ObjectType, owner)
			select new ColumnForDataLineageColumnsDropdown(column, dataFlowRow);
		List<ColumnForDataLineageColumnsDropdown> list = lookUpEdit.DataSource as List<ColumnForDataLineageColumnsDropdown>;
		if (list == null)
		{
			list = new List<ColumnForDataLineageColumnsDropdown>();
		}
		list.AddRange(collection);
		lookUpEdit.DataSource = list;
	}

	public static void SetGridLookUpEditHeight(GridLookUpEdit gridLookUpEdit, int maxElementsOnList = 15)
	{
		if (gridLookUpEdit != null && gridLookUpEdit.Properties.View != null && gridLookUpEdit.Properties.DataSource != null)
		{
			int dataRowCount = gridLookUpEdit.Properties.View.DataRowCount;
			int num = ((dataRowCount > maxElementsOnList) ? maxElementsOnList : dataRowCount);
			int width = ((gridLookUpEdit.Properties.PopupFormMinSize.Width > 0) ? gridLookUpEdit.Properties.PopupFormMinSize.Width : gridLookUpEdit.Width);
			gridLookUpEdit.Properties.PopupFormMinSize = new Size(width, (gridLookUpEdit.Height + 2) * num);
			int width2 = ((gridLookUpEdit.Properties.PopupFormSize.Width > 0) ? gridLookUpEdit.Properties.PopupFormSize.Width : gridLookUpEdit.Width);
			gridLookUpEdit.Properties.PopupFormSize = new Size(width2, (gridLookUpEdit.Height + 2) * num);
		}
	}

	public static int GetGridLookUpEditFilteredRowCount<T>(GridLookUpEdit gridLookUpEdit, Form owner = null)
	{
		try
		{
			if (gridLookUpEdit == null)
			{
				return 0;
			}
			IQueryable<T> queryable = (gridLookUpEdit.Properties.View.DataSource as IEnumerable<T>)?.AsQueryable();
			if (queryable == null)
			{
				return 0;
			}
			queryable = queryable.AppendWhere(new CriteriaToExpressionConverter(), gridLookUpEdit.Properties.View.ActiveFilterCriteria) as IQueryable<T>;
			return queryable.Count();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, owner);
		}
		return 0;
	}
}
