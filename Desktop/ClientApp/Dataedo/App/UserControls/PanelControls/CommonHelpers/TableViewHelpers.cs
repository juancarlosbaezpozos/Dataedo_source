using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.Model.Data.Tables.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;

namespace Dataedo.App.UserControls.PanelControls.CommonHelpers;

internal static class TableViewHelpers
{
	internal static void LoadColumns(RepositoryItemLookUpEdit lookUpEdit, int tableId)
	{
		List<ColumnObject> dataByTable = DB.Column.GetDataByTable(tableId, notDeletedOnly: true);
		ColumnObject columnObject = new ColumnObject();
		columnObject.ColumnId = -1;
		columnObject.OrdinalPosition = -1;
		columnObject.TableId = -1;
		columnObject.Name = string.Empty;
		columnObject.Title = string.Empty;
		columnObject.Description = string.Empty;
		columnObject.Datatype = "nchar";
		columnObject.Status = "A";
		columnObject.CreationDate = DateTime.Now;
		columnObject.CreatedBy = string.Empty;
		columnObject.LastModificationDate = DateTime.Now;
		columnObject.ModifiedBy = string.Empty;
		columnObject.PrimaryKey = false;
		columnObject.DatatypeLen = "nchar(1)";
		columnObject.Nullable = false;
		dataByTable.Insert(0, columnObject);
		lookUpEdit.DropDownRows = ((dataByTable.Count < 25) ? dataByTable.Count : 25);
		lookUpEdit.DataSource = dataByTable;
	}

	internal static void LoadColumnsWithoutEmptyFirst(RepositoryItemLookUpEdit lookUpEdit, int? tableId)
	{
		if (tableId.HasValue)
		{
			List<ColumnObject> dataByTable = DB.Column.GetDataByTable(tableId.Value, notDeletedOnly: true);
			lookUpEdit.DropDownRows = ((dataByTable.Count < 25) ? dataByTable.Count : 25);
			lookUpEdit.DataSource = dataByTable;
		}
		else
		{
			lookUpEdit.DataSource = null;
		}
	}

	internal static void LoadColumnsWithoutEmptyFirst(RepositoryItemGridLookUpEdit lookUpEdit, ObjectRow objectRow, int? tableId)
	{
		if (tableId.HasValue)
		{
			BindingList<ColumnRow> dataObjectByTableId = DB.Column.GetDataObjectByTableId(objectRow, tableId.Value, notDeletedOnly: true);
			int num = (((dataObjectByTableId?.Count ?? 1) < 1) ? 1 : (dataObjectByTableId?.Count ?? 1));
			num = ((num > 25) ? 25 : num);
			lookUpEdit.DataSource = dataObjectByTableId;
		}
		else
		{
			lookUpEdit.DataSource = null;
		}
	}

	internal static void TableUserControl_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (e.Column.FieldName == "UniqueConstraintTypeInt")
		{
			UniqueConstraintType.UniqueConstraintTypeEnum uniqueConstraintTypeEnum = UniqueConstraintType.ToType((int)e.CellValue);
			if (!UniqueConstraintType.IsUserDefined(uniqueConstraintTypeEnum) && uniqueConstraintTypeEnum != 0)
			{
				Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, 16, 16);
				e.Cache.DrawImage(UniqueConstraintType.GetSmallIcon(uniqueConstraintTypeEnum), rect);
				e.Handled = true;
			}
		}
	}
}
