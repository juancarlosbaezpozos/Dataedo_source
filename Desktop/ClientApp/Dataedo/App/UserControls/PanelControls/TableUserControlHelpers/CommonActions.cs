using System.ComponentModel;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Tools.CustomFields;
using Dataedo.Model.Data.Tables.Tables;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;

namespace Dataedo.App.UserControls.PanelControls.TableUserControlHelpers;

internal class CommonActions
{
	public static void RefreshColumnsLinks(int? databaseId, int tableId, CustomFieldsSupport customFieldsSupport, GridControl tableColumnsGrid, GridColumn dataLinksGridColumn)
	{
		if (tableColumnsGrid == null)
		{
			return;
		}
		TableObject dataById = DB.Table.GetDataById(tableId);
		BindingList<ColumnRow> dataObjectByTableId = DB.Column.GetDataObjectByTableId(new ObjectRow(dataById, customFieldsSupport), tableId, notDeletedOnly: false, customFieldsSupport);
		if (!(tableColumnsGrid.DataSource is BindingList<ColumnRow> bindingList))
		{
			return;
		}
		tableColumnsGrid.BeginUpdate();
		foreach (ColumnRow columnRow in bindingList)
		{
			ColumnRow columnRow2 = dataObjectByTableId.FirstOrDefault((ColumnRow x) => x.Id == columnRow.Id);
			if (columnRow2 != null)
			{
				columnRow.DataLinksDataContainer = columnRow2.DataLinksDataContainer;
			}
		}
		tableColumnsGrid.EndUpdate();
		dataLinksGridColumn.Visible = dataObjectByTableId.Any((ColumnRow x) => x.DataLinksDataContainer.HasData);
		if (dataLinksGridColumn.Visible && tableColumnsGrid.MainView.RowCount <= 1000)
		{
			dataLinksGridColumn.BestFit();
		}
	}

	public static void FocusColumn(BulkCopyGridUserControl gridView, BindingList<ColumnRow> columns, int id)
	{
		int dataSourceIndex = columns.IndexOf(columns.FirstOrDefault((ColumnRow x) => x.Id == id));
		int rowHandle = gridView.GetRowHandle(dataSourceIndex);
		gridView.OptionsSelection.MultiSelect = false;
		gridView.ClearSelection();
		gridView.FocusedRowHandle = rowHandle;
		gridView.OptionsSelection.MultiSelect = true;
		gridView.SelectRow(rowHandle);
	}
}
