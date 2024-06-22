using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Shared.Enums;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraRichEdit;

namespace Dataedo.App.UserControls;

public class CustomGridUserControl : GridViewWithRowHighlightUserControl
{
	protected class FilterColumn
	{
		public GridColumn Column { get; set; }

		public CheckedColumnFilterPopup Popup { get; set; }

		public FilterColumn(GridColumn column, CheckedColumnFilterPopup popup)
		{
			Column = column;
			Popup = popup;
		}
	}

	protected Dictionary<GridColumn, FilterColumn> ColumnToFilterColumnMapping { get; private set; }

	public CustomGridUserControl()
	{
		base.OptionsNavigation.AutoMoveRowFocus = false;
		base.CustomColumnSort += CustomGridUserControl_CustomColumnSort;
		base.EndSorting += CustomGridUserControl_EndSorting;
		ColumnToFilterColumnMapping = new Dictionary<GridColumn, FilterColumn>();
	}

	public void AddColumnToFilterMapping(GridColumn baseVisibleColumn, GridColumn columnForFiltering)
	{
		ColumnToFilterColumnMapping.Add(baseVisibleColumn, new FilterColumn(columnForFiltering, null));
		baseVisibleColumn.SortMode = ColumnSortMode.Custom;
		columnForFiltering.Visible = false;
		columnForFiltering.OptionsColumn.ShowInCustomizationForm = false;
		(baseVisibleColumn.View.GridControl.DefaultView as GridView).Columns.Add(columnForFiltering);
	}

	public void AddColumnToFilterMapping(GridColumn baseVisibleColumn, string propertyName)
	{
		GridColumn gridColumn = new GridColumn();
		gridColumn.FieldName = propertyName;
		gridColumn.OptionsColumn.AllowEdit = baseVisibleColumn.OptionsColumn.AllowEdit;
		gridColumn.OptionsColumn.ReadOnly = baseVisibleColumn.OptionsColumn.ReadOnly;
		gridColumn.OptionsFilter.AutoFilterCondition = baseVisibleColumn.OptionsFilter.AutoFilterCondition;
		gridColumn.OptionsFilter.FilterPopupMode = baseVisibleColumn.OptionsFilter.FilterPopupMode;
		gridColumn.OptionsFilter.ShowBlanksFilterItems = baseVisibleColumn.OptionsFilter.ShowBlanksFilterItems;
		AddColumnToFilterMapping(baseVisibleColumn, gridColumn);
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

	public override object GetAutoFilterValue(GridColumn column)
	{
		object autoFilterValue = base.GetAutoFilterValue(column);
		if (ColumnToFilterColumnMapping.TryGetValue(column, out var _) && autoFilterValue != null)
		{
			using (RichEditDocumentServer richEditDocumentServer = new RichEditDocumentServer())
			{
				richEditDocumentServer.HtmlText = autoFilterValue.ToString();
				return richEditDocumentServer.Text;
			}
		}
		return autoFilterValue;
	}

	public override void SetAutoFilterValue(GridColumn column, object value, AutoFilterCondition condition)
	{
		base.SetAutoFilterValue(column, value, condition);
	}

	protected override CriteriaOperator CreateAutoFilterCriterion(GridColumn column, AutoFilterCondition condition, object _value, string strVal)
	{
		if (ColumnToFilterColumnMapping.TryGetValue(column, out var value))
		{
			column = value.Column;
		}
		return base.CreateAutoFilterCriterion(column, condition, _value, strVal);
	}

	protected override void RaiseFilterPopupEvent(FilterPopup filterPopup)
	{
		base.RaiseFilterPopupEvent(filterPopup);
	}

	protected override void OnFilterPopupCloseUp(GridColumn column)
	{
		base.OnFilterPopupCloseUp(column);
	}

	protected override CheckedColumnFilterPopup CreateCheckedFilterPopup(GridColumn column, Control ownerControl, object creator)
	{
		return base.CreateCheckedFilterPopup(column, ownerControl, creator);
	}

	protected override object[] GetFilterPopupValues(GridColumn column, OperationCompleted completed)
	{
		return base.GetFilterPopupValues(column, completed);
	}

	protected override object[] GetFilterPopupValues(GridColumn column, OperationCompleted completed, CriteriaOperator parentCriteria)
	{
		return base.GetFilterPopupValues(column, completed, parentCriteria);
	}

	protected override object[] GetFilterPopupValues(GridColumn column, OperationCompleted completed, ColumnFilterArguments columnArgs)
	{
		return base.GetFilterPopupValues(column, completed, columnArgs);
	}

	private void CustomGridUserControl_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		CustomFieldRowExtended obj = e.Column.Tag as CustomFieldRowExtended;
		if (obj != null && obj.Type == CustomFieldTypeEnum.CustomFieldType.Integer)
		{
			int result = 0;
			bool flag = false;
			int result2 = 0;
			bool flag2 = false;
			flag = int.TryParse(e.Value1?.ToString(), out result);
			flag2 = int.TryParse(e.Value2?.ToString(), out result2);
			if (flag && flag2)
			{
				e.Result = result.CompareTo(result2);
			}
			else if (e.Value1 == null && e.Value2 != null)
			{
				e.Result = 1;
			}
			else if (e.Value1 != null && e.Value2 == null)
			{
				e.Result = -1;
			}
			else if ((!flag || e.Value1 == null) && flag2)
			{
				e.Result = 1;
			}
			else if (flag && (!flag2 || e.Value2 == null))
			{
				e.Result = -1;
			}
			else if (e.Value1 == null && e.Value2 == null)
			{
				e.Result = 0;
			}
			else
			{
				e.Result = Comparer.Default.Compare(e.Value1, e.Value2);
			}
			e.Handled = true;
		}
	}

	private void CustomGridUserControl_EndSorting(object sender, EventArgs e)
	{
		GridControl gridControl = (sender as GridView).GridControl;
		foreach (Control item in from Control x in gridControl.Controls
			where (x.Tag as RepositoryItem)?.Tag is CustomFieldRowExtended customFieldRowExtended && customFieldRowExtended.Type != CustomFieldTypeEnum.CustomFieldType.Text
			select x)
		{
			gridControl.Controls.Remove(item);
		}
	}
}
