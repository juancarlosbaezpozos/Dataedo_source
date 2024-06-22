using System.Collections.Generic;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraRichEdit;

namespace Dataedo.App.UserControls;

internal class CustomBandedGridView : BandedGridView
{
	protected class FilterColumn
	{
		public BandedGridColumn Column { get; set; }

		public CheckedColumnFilterPopup Popup { get; set; }

		public FilterColumn(BandedGridColumn column, CheckedColumnFilterPopup popup)
		{
			Column = column;
			Popup = popup;
		}
	}

	protected Dictionary<BandedGridColumn, FilterColumn> ColumnToFilterColumnMapping { get; private set; }

	public CustomBandedGridView()
	{
		ColumnToFilterColumnMapping = new Dictionary<BandedGridColumn, FilterColumn>();
	}

	public void AddColumnToFilterMapping(BandedGridColumn baseVisibleColumn, BandedGridColumn columnForFiltering)
	{
		ColumnToFilterColumnMapping.Add(baseVisibleColumn, new FilterColumn(columnForFiltering, null));
		columnForFiltering.Visible = false;
		columnForFiltering.OptionsColumn.ShowInCustomizationForm = false;
		(baseVisibleColumn.View.GridControl.DefaultView as GridView).Columns.Add(columnForFiltering);
	}

	public void AddColumnToFilterMapping(BandedGridColumn baseVisibleColumn, string propertyName)
	{
		BandedGridColumn bandedGridColumn = new BandedGridColumn();
		bandedGridColumn.FieldName = propertyName;
		bandedGridColumn.OptionsColumn.AllowEdit = baseVisibleColumn.OptionsColumn.AllowEdit;
		bandedGridColumn.OptionsColumn.ReadOnly = baseVisibleColumn.OptionsColumn.ReadOnly;
		bandedGridColumn.OptionsFilter.AutoFilterCondition = baseVisibleColumn.OptionsFilter.AutoFilterCondition;
		bandedGridColumn.OptionsFilter.FilterPopupMode = baseVisibleColumn.OptionsFilter.FilterPopupMode;
		bandedGridColumn.OptionsFilter.ShowBlanksFilterItems = baseVisibleColumn.OptionsFilter.ShowBlanksFilterItems;
		AddColumnToFilterMapping(baseVisibleColumn, bandedGridColumn);
		(baseVisibleColumn.View.GridControl.DefaultView as GridView).ShowFilterPopupCheckedListBox += delegate(object sender, FilterPopupCheckedListBoxEventArgs e)
		{
			if (e.Column != baseVisibleColumn)
			{
				return;
			}
			using RichEditDocumentServer richEditDocumentServer = new RichEditDocumentServer();
			foreach (CheckedListBoxItem item in e.CheckedComboBox.Items)
			{
				richEditDocumentServer.HtmlText = item.Value.ToString();
				item.Description = richEditDocumentServer.Text;
			}
		};
	}
}
