using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Tools.CustomFields;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace Dataedo.App.Tools.Pannels;

public class CustomFieldsCellsTypesSupport
{
	private class CustomFieldRepositoryItem
	{
		private readonly string currentValue;

		public RepositoryItem RepositoryItem { get; private set; }

		public CustomFieldRepositoryItem(RepositoryItem repositoryItem)
		{
			RepositoryItem = repositoryItem;
		}

		public CustomFieldRepositoryItem(RepositoryItem repositoryItem, string additionalValues)
			: this(repositoryItem)
		{
			currentValue = additionalValues;
		}

		public bool HasDifferentCurrentValue(string valuesToCompare)
		{
			return currentValue != valuesToCompare;
		}
	}

	private Dictionary<int, Dictionary<int, CustomFieldRepositoryItem>> repositoryItemsCache = new Dictionary<int, Dictionary<int, CustomFieldRepositoryItem>>();

	private bool lockViewCellValueChangedEvent;

	private readonly bool isForSummaryTable;

	private readonly bool isForGrid;

	public CustomFieldsCellsTypesSupport(bool isForSummaryTable, bool isForGrid = true)
	{
		this.isForSummaryTable = isForSummaryTable;
		this.isForGrid = isForGrid;
	}

	public void SetCustomFieldColumn(GridView view, CustomFieldRowExtended field, GridColumn column)
	{
		SetViewEvents(view);
		UpdateCustomFieldColumn(field, column);
	}

	public void SetCustomFields(CustomFieldsSupport customFieldsSupport, Dictionary<SharedObjectTypeEnum.ObjectType, GridView> customFieldsSettings)
	{
		repositoryItemsCache = new Dictionary<int, Dictionary<int, CustomFieldRepositoryItem>>();
		foreach (KeyValuePair<SharedObjectTypeEnum.ObjectType, GridView> customFieldsSetting in customFieldsSettings)
		{
			SetCustomColumns(customFieldsSetting.Value, customFieldsSupport, customFieldsSetting.Key);
		}
	}

	public void SetCustomColumns(GridView view, CustomFieldsSupport customFieldsSupport, SharedObjectTypeEnum.ObjectType objectType, bool customFieldsAsArray = false)
	{
		SetCustomColumns(view, customFieldsSupport.GetVisibleFields(objectType), customFieldsAsArray);
	}

	public void SetCustomColumns(TreeList view, List<CustomFieldRowExtended> visibleCustomFields, bool customFieldsAsArray = false)
	{
		IEnumerable<TreeListColumn> existingCustomFieldsColumns = (customFieldsAsArray ? view.Columns.Where((TreeListColumn x) => x.FieldName.Contains("Field")) : view.Columns.Where((TreeListColumn x) => x.FieldName.Contains("field")));
		view.BeginUpdate();
		foreach (int item in from x in existingCustomFieldsColumns
			where !visibleCustomFields.Any((CustomFieldRowExtended y) => y.FieldName == x.FieldName)
			select x.AbsoluteIndex into x
			orderby x descending
			select x)
		{
			view.Columns.RemoveAt(item);
		}
		int num = 0;
		IEnumerable<TreeListColumn> source = view.Columns.Where((TreeListColumn x) => x.VisibleIndex >= 0 && !(x.Tag is CustomFieldRowExtended));
		if (source.Any())
		{
			num = source.Max((TreeListColumn x) => x.VisibleIndex);
		}
		foreach (CustomFieldRowExtended item2 in visibleCustomFields.Where((CustomFieldRowExtended x) => !existingCustomFieldsColumns.Any((TreeListColumn y) => y.FieldName == x.FieldName)))
		{
			string fieldName2 = (customFieldsAsArray ? item2.FieldPropertyName : item2.FieldName);
			TreeListColumn treeListColumn = new TreeListColumn();
			treeListColumn.FieldName = fieldName2;
			treeListColumn.Visible = true;
			treeListColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
			UpdateCustomFieldColumn(item2, treeListColumn);
			view.Columns.Add(treeListColumn);
		}
		foreach (CustomFieldRowExtended item3 in visibleCustomFields.Where((CustomFieldRowExtended x) => existingCustomFieldsColumns.Any((TreeListColumn y) => y.FieldName == x.FieldName)))
		{
			string fieldName = (customFieldsAsArray ? item3.FieldPropertyName : item3.FieldName);
			if (!string.IsNullOrWhiteSpace(fieldName))
			{
				TreeListColumn treeListColumn2 = view.Columns.FirstOrDefault((TreeListColumn x) => fieldName.Equals(x.FieldName));
				if (treeListColumn2 != null)
				{
					UpdateCustomFieldColumn(item3, treeListColumn2);
				}
			}
		}
		int num2 = num;
		List<TreeListColumn> list = (from x in view.Columns
			where x.VisibleIndex >= 0 && x.Tag is CustomFieldRowExtended
			orderby (x.Tag as CustomFieldRowExtended).OrdinalPosition
			select x).ToList();
		foreach (TreeListColumn item4 in list)
		{
			view.Columns.Remove(item4);
		}
		foreach (TreeListColumn item5 in list)
		{
			num2 = (item5.VisibleIndex = num2 + 1);
			view.Columns.Add(item5);
		}
		view.EndUpdate();
	}

	private void SetCustomColumns(GridView view, List<CustomFieldRowExtended> visibleCustomFields, bool customFieldsAsArray = false)
	{
		SetViewEvents(view);
		IEnumerable<GridColumn> existingCustomFieldsColumns = (customFieldsAsArray ? view.Columns.Where((GridColumn x) => x.FieldName.Contains("Field")) : view.Columns.Where((GridColumn x) => x.FieldName.Contains("field")));
		view.BeginUpdate();
		foreach (int item in from x in existingCustomFieldsColumns
			where !visibleCustomFields.Any((CustomFieldRowExtended y) => (customFieldsAsArray ? y.FieldPropertyName : y.FieldName) == x.FieldName)
			select x.AbsoluteIndex into x
			orderby x descending
			select x)
		{
			view.Columns.RemoveAt(item);
		}
		IEnumerable<GridColumn> source = view.Columns.Where((GridColumn x) => x.VisibleIndex >= 0 && !(x.Tag is CustomFieldRowExtended));
		if (source.Any())
		{
			source.Max((GridColumn x) => x.VisibleIndex);
		}
		foreach (CustomFieldRowExtended item2 in visibleCustomFields.Where((CustomFieldRowExtended x) => !existingCustomFieldsColumns.Any((GridColumn y) => y.FieldName == (customFieldsAsArray ? x.FieldPropertyName : x.FieldName))))
		{
			string fieldName2 = (customFieldsAsArray ? item2.FieldPropertyName : item2.FieldName);
			GridColumn gridColumn = ((view is BandedGridView) ? new BandedGridColumn() : new GridColumn());
			gridColumn.FieldName = fieldName2;
			gridColumn.Visible = true;
			gridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
			UpdateCustomFieldColumn(item2, gridColumn);
			if (view is BandedGridView)
			{
				(view as BandedGridView).Bands.FirstOrDefault().Columns.Add(gridColumn as BandedGridColumn);
			}
			else
			{
				view.Columns.Add(gridColumn);
			}
			gridColumn.BestFit();
		}
		foreach (CustomFieldRowExtended item3 in visibleCustomFields.Where((CustomFieldRowExtended x) => existingCustomFieldsColumns.Any((GridColumn y) => y.FieldName == (customFieldsAsArray ? x.FieldPropertyName : x.FieldName))))
		{
			string fieldName = (customFieldsAsArray ? item3.FieldPropertyName : item3.FieldName);
			if (!string.IsNullOrWhiteSpace(fieldName))
			{
				GridColumn gridColumn2 = view.Columns.FirstOrDefault((GridColumn x) => fieldName.Equals(x.FieldName));
				if (gridColumn2 != null)
				{
					UpdateCustomFieldColumn(item3, gridColumn2);
				}
			}
		}
		view.EndUpdate();
	}

	public static void SortCustomColumns(GridView view)
	{
		int num = 0;
		IEnumerable<GridColumn> source = view.Columns.Where((GridColumn x) => x.VisibleIndex >= 0 && !(x.Tag is CustomFieldRowExtended));
		if (source.Any())
		{
			num = source.Max((GridColumn x) => x.VisibleIndex);
		}
		int num2 = num;
		List<GridColumn> list = (from x in view.Columns
			where x.VisibleIndex >= 0 && x.Tag is CustomFieldRowExtended
			orderby (x.Tag as CustomFieldRowExtended).OrdinalPosition
			select x).ToList();
		view.BeginUpdate();
		foreach (GridColumn item in list)
		{
			view.Columns.Remove(item);
		}
		foreach (GridColumn item2 in list)
		{
			num2 = (item2.VisibleIndex = num2 + 1);
			view.Columns.Add(item2);
			if (view is BandedGridView)
			{
				(view as BandedGridView).Bands.FirstOrDefault().Columns.Add(item2 as BandedGridColumn);
			}
		}
		view.EndUpdate();
	}

	public void SetCustomColumnsForExistingColumns(GridView view)
	{
		SetViewEvents(view);
		view.BeginUpdate();
		foreach (GridColumn item in view.Columns.Where((GridColumn x) => x.Tag is CustomFieldRowExtended))
		{
			CustomFieldRowExtended field = (CustomFieldRowExtended)item.Tag;
			UpdateCustomFieldColumn(field, item, setTitle: false);
		}
		view.EndUpdate();
	}

	private void SetViewEvents(GridView view)
	{
		view.CustomRowCellEdit -= View_CustomRowCellEdit;
		view.CustomRowCellEdit += View_CustomRowCellEdit;
		view.CellValueChanged -= View_CellValueChanged;
		view.CellValueChanged += View_CellValueChanged;
		view.MouseDown -= View_MouseDown;
		view.MouseDown += View_MouseDown;
		view.MouseUp -= View_MouseUp;
		view.MouseUp += View_MouseUp;
		view.FocusedColumnChanged -= View_FocusedColumnChanged;
		view.FocusedColumnChanged += View_FocusedColumnChanged;
		view.FocusedRowChanged -= View_FocusedRowChanged;
		view.FocusedRowChanged += View_FocusedRowChanged;
		view.KeyDown -= View_KeyDown;
		view.KeyDown += View_KeyDown;
	}

	private void View_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		HandleSettingReporitoryItems(sender, e);
	}

	private void HandleSettingReporitoryItems(object sender, CustomRowCellEditEventArgs e)
	{
		if (e.Column.Tag is CustomFieldRowExtended customFieldRowExtended && CustomFieldTypeEnum.IsClosedDefinitionType(customFieldRowExtended.Type))
		{
			int dataSourceRowIndex = e.Column.View.GetDataSourceRowIndex(e.RowHandle);
			int absoluteIndex = e.Column.AbsoluteIndex;
			SetRepositoryItemInCache(dataSourceRowIndex, absoluteIndex, customFieldRowExtended, e.CellValue);
			e.RepositoryItem = repositoryItemsCache[absoluteIndex][dataSourceRowIndex].RepositoryItem;
		}
	}

	private void SetRepositoryItemInCache(int datasourceRowIndex, int columnIndex, CustomFieldRowExtended field, object cellValue, bool forceChange = false)
	{
		if (!repositoryItemsCache.ContainsKey(columnIndex))
		{
			repositoryItemsCache.Add(columnIndex, new Dictionary<int, CustomFieldRepositoryItem>());
		}
		bool flag = repositoryItemsCache[columnIndex].ContainsKey(datasourceRowIndex);
		if (!flag || forceChange)
		{
			RepositoryItem repositoryItem = CreateRepositoryItem(field, cellValue);
			if (!flag)
			{
				repositoryItemsCache[columnIndex].Add(datasourceRowIndex, new CustomFieldRepositoryItem(repositoryItem, cellValue as string));
			}
			else
			{
				repositoryItemsCache[columnIndex][datasourceRowIndex] = new CustomFieldRepositoryItem(CreateRepositoryItem(field, cellValue), cellValue as string);
			}
		}
		else if (flag && repositoryItemsCache[columnIndex][datasourceRowIndex].HasDifferentCurrentValue(cellValue as string))
		{
			repositoryItemsCache[columnIndex][datasourceRowIndex] = new CustomFieldRepositoryItem(CreateRepositoryItem(field, cellValue), cellValue as string);
		}
	}

	private RepositoryItem CreateRepositoryItem(CustomFieldRowExtended field, object fieldValue)
	{
		return CustomFieldsRepositoryItems.GetProperRepositoryItem(field, isForGrid, isForSummaryTable, fieldValue as string);
	}

	private void View_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode != Keys.Return || !(sender is GridView gridView))
		{
			return;
		}
		GridCellInfo columnInfo = GetColumnInfo(gridView, gridView.FocusedRowHandle, gridView.FocusedColumn);
		if (columnInfo?.Editor is RepositoryItemTokenEdit)
		{
			if (gridView.IsEditing)
			{
				TokenEdit obj = gridView.ActiveEditor as TokenEdit;
				if (obj == null || obj.IsTextEditorActive)
				{
					return;
				}
			}
			gridView.ShowEditorByKey(e);
			if (gridView.ActiveEditor is TokenEdit tokenEdit)
			{
				gridView.ShowEditorByKey(e);
				tokenEdit.Select();
				tokenEdit.ActivateTextEditor();
				e.SuppressKeyPress = true;
			}
		}
		else if (columnInfo?.Editor is RepositoryItemLookUpEdit)
		{
			if (!gridView.IsEditing)
			{
				gridView.ShowEditorByKey(e);
			}
			if (gridView.ActiveEditor is LookUpEdit lookUpEdit)
			{
				lookUpEdit.Select();
				lookUpEdit.ShowPopup();
				e.SuppressKeyPress = true;
				e.Handled = true;
			}
		}
		else if (columnInfo?.Editor is RepositoryItemCheckedComboBoxEdit)
		{
			if (!gridView.IsEditing)
			{
				gridView.ShowEditorByKey(e);
			}
			if (gridView.ActiveEditor is CheckedComboBoxEdit checkedComboBoxEdit)
			{
				checkedComboBoxEdit.Select();
				checkedComboBoxEdit.ShowPopup();
				e.SuppressKeyPress = true;
				e.Handled = true;
			}
		}
		else if (columnInfo?.Editor is RepositoryItemMRUEdit)
		{
			if (!gridView.IsEditing)
			{
				gridView.ShowEditorByKey(e);
			}
			if (gridView.ActiveEditor is MRUEdit mRUEdit)
			{
				mRUEdit.Select();
				mRUEdit.ShowPopup();
				e.SuppressKeyPress = true;
				e.Handled = true;
			}
		}
	}

	private void View_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
	{
		if (sender is GridView gridView)
		{
			CellChanged(gridView, gridView.FocusedRowHandle, e.FocusedColumn);
		}
	}

	private void View_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		if (sender is GridView gridView)
		{
			gridView.CloseEditor();
			CellChanged(gridView, e.FocusedRowHandle, gridView.FocusedColumn);
			gridView.RefreshData();
		}
	}

	private void CellChanged(GridView view, int rowHandle, GridColumn gridColumn)
	{
		GridCellInfo columnInfo = GetColumnInfo(view, rowHandle, gridColumn);
		if (columnInfo?.Editor is RepositoryItemTokenEdit)
		{
			(view.ActiveEditor as TokenEdit)?.Select();
		}
		else if (columnInfo?.Editor is RepositoryItemLookUpEdit)
		{
			view.ShowEditor();
			(view.ActiveEditor as LookUpEdit)?.Select();
		}
		else if (columnInfo?.Editor is RepositoryItemCheckedComboBoxEdit)
		{
			view.ShowEditor();
			(view.ActiveEditor as CheckedComboBoxEdit)?.Select();
		}
	}

	private GridCellInfo GetColumnInfo(GridView view, int rowHandle, GridColumn gridColumn)
	{
		return (view.GetViewInfo() as GridViewInfo)?.GetGridCellInfo(rowHandle, gridColumn);
	}

	private void View_MouseDown(object sender, MouseEventArgs e)
	{
	}

	private void View_MouseUp(object sender, MouseEventArgs e)
	{
	}

	private void View_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		if (!lockViewCellValueChangedEvent)
		{
			GridView gridView = sender as GridView;
			CustomFieldRowExtended obj = gridView.FocusedColumn.Tag as CustomFieldRowExtended;
			if (obj != null && obj.Type == CustomFieldTypeEnum.CustomFieldType.Date)
			{
				lockViewCellValueChangedEvent = true;
				gridView.SetRowCellValue(e.RowHandle, e.Column, CustomFieldTypeEnum.GetDefaultFormattedDate(e.Value));
				lockViewCellValueChangedEvent = false;
			}
		}
	}

	private void UpdateCustomFieldColumn(CustomFieldRowExtended field, GridColumn column, bool setTitle = true)
	{
		column.ColumnEdit = CustomFieldsRepositoryItems.GetProperRepositoryItem(field, isForGrid, isForSummaryTable, null);
		if (setTitle)
		{
			column.Caption = field.Title;
		}
		column.Tag = field;
		column.OptionsColumn.AllowEdit = true;
		column.OptionsColumn.ReadOnly = false;
		column.OptionsFilter.AutoFilterCondition = ((field.Type == CustomFieldTypeEnum.CustomFieldType.Text) ? DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains : DevExpress.XtraGrid.Columns.AutoFilterCondition.Equals);
		if (field.Type == CustomFieldTypeEnum.CustomFieldType.Integer)
		{
			column.SortMode = ColumnSortMode.Custom;
		}
	}

	private void UpdateCustomFieldColumn(CustomFieldRowExtended field, TreeListColumn column)
	{
		column.ColumnEdit = CustomFieldsRepositoryItems.GetProperRepositoryItem(field, isForGrid, isForSummaryTable, null);
		column.Caption = field.Title;
		column.Tag = field;
		column.OptionsColumn.AllowEdit = true;
		column.OptionsColumn.ReadOnly = false;
		column.OptionsFilter.AutoFilterCondition = ((field.Type == CustomFieldTypeEnum.CustomFieldType.Text) ? DevExpress.XtraTreeList.Columns.AutoFilterCondition.Contains : DevExpress.XtraTreeList.Columns.AutoFilterCondition.Equals);
		if (field.Type == CustomFieldTypeEnum.CustomFieldType.Integer)
		{
			column.SortMode = ColumnSortMode.Custom;
		}
	}
}
