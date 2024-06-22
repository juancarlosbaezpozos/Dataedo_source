using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Licences;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Search;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Dataedo.App.UserControls.PanelControls.Appearance;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Interfaces;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Data;
using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.UserControls.PanelControls.TableUserControlHelpers;

internal class DataLinksManager
{
	private BarButtonItem dataLinksAddLinkBarButtonItem;

	private BarButtonItem columnsAddLinkBarButtonItem;

	private readonly BasePanelControl basePanelControl;

	private readonly XtraTabControl tableXtraTabControl;

	private readonly XtraTabPage dataLinksXtraTabPage;

	private readonly GridPanelUserControl dataLinksGridPanelUserControl;

	private readonly GridPanelUserControl columnsGridPanelUserControl;

	private readonly GridControl dataLinksGrid;

	private readonly BulkCopyGridUserControl dataLinksGridView;

	private readonly PopupMenu linkedTermsPopupMenu;

	private readonly BarButtonItem goToObjectDataLinksToolStripMenuItem;

	private readonly BarButtonItem addLinkDataLinksToolStripMenuItem;

	private readonly BarButtonItem removeLinkDataLinksToolStripMenuItem;

	private GridControl tableColumnsGrid;

	private BulkCopyGridUserControl tableColumnsGridView;

	private GridColumn dataLinksGridColumn;

	private GridColumn termIconDataLinksGridColumn;

	private GridColumn objectIconDataLinksGridColumn;

	private bool showSchema;

	private int tableId;

	private string tableName;

	public BindingList<DataLinkObjectExtendedForTerms> objectsWithDataLink;

	private MetadataEditorUserControl MainControl => basePanelControl.MainControl;

	private bool TabPageChangedProgrammatically => basePanelControl.TabPageChangedProgrammatically;

	private UserControlHelpers UserControlHelpers => basePanelControl.UserControlHelpers;

	private ResultItem LastSearchResult => basePanelControl.LastSearchResult;

	public bool IsDataLinksDataLoaded { get; set; }

	public void PrepareDataLinksGridPanelUserControl()
	{
		dataLinksGridPanelUserControl.RemoveCustomFieldsButton();
		BarButtonItem barButtonItem = new BarButtonItem();
		barButtonItem.Glyph = Resources.data_link_add_16;
		barButtonItem.Caption = "Edit links to Business Glossary terms";
		barButtonItem.Name = "dataLinksAddLinkBarButtonItem";
		barButtonItem.ItemClick += AddLinkDataLinksBarButtonItem_ItemClick;
		dataLinksGridPanelUserControl.InsertAdditionalButtonBeforeRemoveButton(barButtonItem);
		dataLinksAddLinkBarButtonItem = barButtonItem;
	}

	public void PrepareColumnsGridPanelUserControl()
	{
		BarButtonItem barButtonItem = new BarButtonItem();
		barButtonItem.Glyph = Resources.data_link_add_16;
		barButtonItem.Caption = "Edit links to Business Glossary terms";
		barButtonItem.Name = "columnsAddLinkBarButtonItem";
		barButtonItem.ItemClick += ColumnsAddLinkBarButtonItem_ItemClick;
		columnsGridPanelUserControl.InsertAdditionalButtonBeforeRemoveButton(barButtonItem);
		columnsAddLinkBarButtonItem = barButtonItem;
	}

	public void SetAddNewBusinessGlossaryTermMenu(BarItemLink addNewLinkedTermColumnsBarButtonItemLink, PopupMenu columnsPopupMenu, Form owner = null)
	{
		List<DBTreeNode> businessGlossaryNodes = MainControl.TreeListHelpers.GetBusinessGlossaryNodes();
		if (businessGlossaryNodes.Count <= 1)
		{
			string caption = ((tableColumnsGridView.SelectedRowsCount > 1) ? "Add new linked Business Glossary terms" : "Add new linked Business Glossary term");
			BarButtonItem barButtonItem = new BarButtonItem();
			barButtonItem.Manager = columnsPopupMenu.Manager;
			barButtonItem.Name = addNewLinkedTermColumnsBarButtonItemLink.Item.Name;
			barButtonItem.Caption = caption;
			barButtonItem.ImageOptions.Image = addNewLinkedTermColumnsBarButtonItemLink.Item.Glyph;
			barButtonItem.ItemClick += AddNewBusinessGlossaryTermToolStripMenuItem_Click;
			columnsPopupMenu.InsertItem(addNewLinkedTermColumnsBarButtonItemLink, barButtonItem);
		}
		else
		{
			string caption2 = ((tableColumnsGridView.SelectedRowsCount > 1) ? "Add new linked Business Glossary terms in" : "Add new linked Business Glossary term in");
			BarSubItem barSubItem = new BarSubItem();
			barSubItem.Manager = columnsPopupMenu.Manager;
			barSubItem.Name = addNewLinkedTermColumnsBarButtonItemLink.Item.Name;
			barSubItem.Caption = caption2;
			barSubItem.ImageOptions.Image = addNewLinkedTermColumnsBarButtonItemLink.Item.Glyph;
			IEnumerable<BarButtonItem> source = businessGlossaryNodes.Select(delegate(DBTreeNode x)
			{
				BarButtonItem barButtonItem2 = new BarButtonItem();
				barButtonItem2.Glyph = Resources.term_add_16;
				barButtonItem2.Caption = x.TreeDisplayNameUiEscaped;
				barButtonItem2.ItemClick += delegate
				{
					AddNewBusinessGlossaryTermFromColumns(x.DatabaseId, owner);
				};
				return barButtonItem2;
			});
			BarItem[] items = source.ToArray();
			barSubItem.AddItems(items);
			columnsPopupMenu.InsertItem(addNewLinkedTermColumnsBarButtonItemLink, barSubItem);
		}
		columnsPopupMenu.ItemLinks.Remove(addNewLinkedTermColumnsBarButtonItemLink);
		addNewLinkedTermColumnsBarButtonItemLink.Dispose();
	}

	public void AddGoToMenuItemsForColumns(PopupMenu tableColumnsPopupMenu, ColumnRow column)
	{
		BarItemLinkCollection itemLinks = tableColumnsPopupMenu.ItemLinks;
		BarItem[] items = column.DataLinksDataContainer.DistinctDataSorted.SelectMany(delegate(DataLinkData x)
		{
			List<BarButtonItem> list = new List<BarButtonItem>();
			BarButtonItem barButtonItem = new BarButtonItem
			{
				Caption = "Go to " + Escaping.EscapeTextForUI(x.TermTitleWithType),
				Glyph = x.ObjectImage,
				Tag = x
			};
			barButtonItem.ItemClick += delegate
			{
				DataLinkData dataLinkData = x;
				if (dataLinkData != null && dataLinkData.TermDocumentationId.HasValue && dataLinkData.TermId.HasValue && MainControl.ContinueAfterPossibleChanges())
				{
					MainControl.TreeListHelpers.OpenNode(dataLinkData.TermDocumentationId.Value, dataLinkData.TermId.Value, SharedObjectTypeEnum.ObjectType.Term);
				}
			};
			list.Add(barButtonItem);
			return list;
		}).ToArray();
		itemLinks.AddRange(items);
	}

	private void AddLinkDataLinksBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		DataLinkObjectExtendedForTerms dataLinkObjectExtendedForTerms = dataLinksGridView.GetFocusedRow() as DataLinkObjectExtendedForTerms;
		int? elementId = ((dataLinksGridView.SelectedRowsCount != 1) ? null : dataLinkObjectExtendedForTerms?.ElementId);
		if (MainControl.DataLinksSupport.StartAddingLinkToBusinessGlossary(dataLinkObjectExtendedForTerms?.ObjectType ?? SharedObjectTypeEnum.TypeToString(basePanelControl.ObjectType), tableId, elementId, fromCustomFocus: false, (dataLinkObjectExtendedForTerms != null && dataLinkObjectExtendedForTerms.ObjectDocumentationShowSchemaEffective) || basePanelControl.ContextShowSchema || basePanelControl.ShowSchema, MainControl?.FindForm()) == DialogResult.OK)
		{
			RefreshDataLinks(forceRefresh: true);
			RefreshColumnsLinks();
		}
	}

	public void AddNewBusinessGlossaryTermToolStripMenuItem_Click(object sender, EventArgs e)
	{
		AddNewBusinessGlossaryTermFromColumns(null, MainControl?.FindForm());
	}

	public void AddNewBusinessGlossaryTermFromColumns(int? businessGlossaryId, Form owner = null)
	{
		int[] selectedRows = tableColumnsGridView.GetSelectedRows();
		if (MainControl.ContinueAfterPossibleChanges())
		{
			List<BusinessGlossarySupport.ObjectDefinition> list = new List<BusinessGlossarySupport.ObjectDefinition>();
			int[] array = selectedRows;
			foreach (int rowHandle in array)
			{
				ColumnRow columnRow = tableColumnsGridView.GetRow(rowHandle) as ColumnRow;
				int id = columnRow.Id;
				string name = (string.IsNullOrEmpty(columnRow.Title) ? columnRow.Name : columnRow.Title);
				list.Add(new BusinessGlossarySupport.ObjectDefinition(name, columnRow.ObjectType, tableId, id));
			}
			MainControl.BusinessGlossarySupport.AddNewBusinessGlossaryTerm(businessGlossaryId, owner, list.ToArray());
			RefreshDataLinks(forceRefresh: true);
			RefreshColumnsLinks();
		}
	}

	private void ColumnsAddLinkBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (tableColumnsGridView.GetFocusedRow() is ColumnRow columnRow)
		{
			int? elementId = ((tableColumnsGridView.SelectedRowsCount == 1) ? new int?(columnRow.Id) : null);
			TreeListNode focusedTreeListNode = MainControl.TreeListHelpers.GetFocusedTreeListNode();
			if (MainControl.DataLinksSupport.StartAddingDataLink(focusedTreeListNode, fromCustomFocus: false, (columnRow != null) ? SharedObjectTypeEnum.TypeToString(columnRow.ObjectType) : SharedObjectTypeEnum.TypeToString(MainControl.TreeListHelpers.GetNode(focusedTreeListNode).ObjectType), elementId, MainControl?.FindForm()) == DialogResult.OK)
			{
				RefreshDataLinks(forceRefresh: true);
				RefreshColumnsLinks();
			}
		}
	}

	private void GoToObjectDataLinksToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (MainControl.ContinueAfterPossibleChanges())
		{
			MainControl.TreeListHelpers.OpenNode(dataLinksGridView.GetFocusedRow() as IGoTo);
			DataLinkObject obj = dataLinksGridView.GetFocusedRow() as DataLinkObject;
			if (obj != null && obj.ElementId.HasValue)
			{
				(MainControl.GetVisibleUserControl() as ITabChangable)?.ChangeTab(SharedObjectTypeEnum.ObjectType.Column);
			}
		}
	}

	private void DataLinksGridView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		Point pt = dataLinksGridView.GridControl.PointToClient(Control.MousePosition);
		GridHitInfo gridHitInfo = dataLinksGridView.CalcHitInfo(pt);
		bool flag = gridHitInfo.InRowCell || gridHitInfo.InRow;
		removeLinkDataLinksToolStripMenuItem.Visibility = ((!flag) ? BarItemVisibility.Never : BarItemVisibility.Always);
		goToObjectDataLinksToolStripMenuItem.Visibility = ((!flag) ? BarItemVisibility.Never : BarItemVisibility.Always);
		addLinkDataLinksToolStripMenuItem.Tag = flag;
		addLinkDataLinksToolStripMenuItem.Visibility = ((flag && dataLinksGridView.SelectedRowsCount != 0 && dataLinksGridView.SelectedRowsCount != 1) ? BarItemVisibility.Never : BarItemVisibility.Always);
		if (flag)
		{
			if (dataLinksGridView.GetFocusedRow() is IGoTo goTo)
			{
				goToObjectDataLinksToolStripMenuItem.Caption = "Go to " + Escaping.EscapeTextForUI(goTo.FullName);
				DataLinkObjectExtendedForTerms dataLinkObjectExtendedForTerms = goTo as DataLinkObjectExtendedForTerms;
				goToObjectDataLinksToolStripMenuItem.ImageOptions.Image = IconsSupport.GetObjectIcon(dataLinkObjectExtendedForTerms.TypeForIcon, dataLinkObjectExtendedForTerms.SubtypeForIcon, dataLinkObjectExtendedForTerms.SourceForIcon);
			}
			else
			{
				goToObjectDataLinksToolStripMenuItem.Visibility = BarItemVisibility.Never;
			}
		}
		linkedTermsPopupMenu.ShowPopup(Control.MousePosition);
	}

	private void RemoveLinkDataLinksToolStripMenuItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ProcessDeletingLink();
	}

	private void AddLinkDataLinksToolStripMenuItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		BarButtonItem barButtonItem = addLinkDataLinksToolStripMenuItem;
		DataLinkObjectExtendedForTerms dataLinkObjectExtendedForTerms = dataLinksGridView.GetFocusedRow() as DataLinkObjectExtendedForTerms;
		int? elementId = ((dataLinksGridView.SelectedRowsCount != 1) ? null : ((barButtonItem.Tag as bool? != true) ? null : dataLinkObjectExtendedForTerms?.ElementId));
		if (MainControl.DataLinksSupport.StartAddingDataLink(MainControl.TreeListHelpers.GetFocusedTreeListNode(), fromCustomFocus: false, dataLinkObjectExtendedForTerms?.ObjectType ?? SharedObjectTypeEnum.TypeToString(basePanelControl.ObjectType), elementId, MainControl?.FindForm()) == DialogResult.OK)
		{
			RefreshDataLinks(forceRefresh: true);
			RefreshColumnsLinks();
		}
	}

	private void GoToObjectDataLinksToolStripMenuItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (MainControl.ContinueAfterPossibleChanges())
		{
			MainControl.TreeListHelpers.OpenNode(dataLinksGridView.GetFocusedRow() as IGoTo);
			DataLinkObject obj = dataLinksGridView.GetFocusedRow() as DataLinkObject;
			if (obj != null && obj.ElementId.HasValue)
			{
				(MainControl.GetVisibleUserControl() as ITabChangable)?.ChangeTab(SharedObjectTypeEnum.ObjectType.Column);
			}
		}
	}

	private void AddDataLinkToolStripMenuItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ColumnRow columnRow = tableColumnsGridView.GetFocusedRow() as ColumnRow;
		int? elementId = ((tableColumnsGridView.SelectedRowsCount == 1) ? new int?(columnRow.Id) : null);
		if (MainControl.DataLinksSupport.StartAddingDataLink(MainControl.TreeListHelpers.GetFocusedTreeListNode(), fromCustomFocus: false, SharedObjectTypeEnum.TypeToString(columnRow.ObjectType), elementId, MainControl?.FindForm()) == DialogResult.OK)
		{
			RefreshDataLinks(forceRefresh: true);
			RefreshColumnsLinks();
		}
	}

	public bool UpdateDataLinks(Form owner = null)
	{
		try
		{
			if (objectsWithDataLink != null)
			{
				DB.BusinessGlossary.ProcessSavingDataLinks(objectsWithDataLink);
				objectsWithDataLink.Where((DataLinkObjectExtendedForTerms x) => !x.IsDeleted).ToList().ForEach(delegate(DataLinkObjectExtendedForTerms x)
				{
					x.SetAsCurrentlyAdded();
				});
			}
			return true;
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(basePanelControl.GetSplashScreenManager(), show: false);
			GeneralExceptionHandling.Handle(exception, "Error while updating data links", owner);
			return false;
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(basePanelControl.GetSplashScreenManager(), show: true);
		}
	}

	private void ProcessDeletingLink()
	{
		if (CommonFunctionsDatabase.AskIfDeleting(dataLinksGridView.GetSelectedRows(), dataLinksGridView, SharedObjectTypeEnum.ObjectType.DataLink))
		{
			int[] selectedRows = dataLinksGridView.GetSelectedRows();
			foreach (int rowHandle in selectedRows)
			{
				DataLinkObjectExtended obj = dataLinksGridView.GetRow(rowHandle) as DataLinkObjectExtended;
				obj.IsModified = true;
				obj.IsSelected = false;
			}
			dataLinksGridView.RefreshData();
			basePanelControl.SetTabPageTitle(isEdited: true, dataLinksXtraTabPage, basePanelControl.Edit);
		}
	}

	public void RefreshColumnsLinks()
	{
		CommonActions.RefreshColumnsLinks(basePanelControl.DatabaseId, basePanelControl.ObjectId, basePanelControl.CustomFieldsSupport, tableColumnsGrid, dataLinksGridColumn);
	}

	private void DataLinksGridView_CustomRowFilter(object sender, RowFilterEventArgs e)
	{
		DataLinkObjectExtendedForTerms dataLinkObjectExtendedForTerms = objectsWithDataLink[e.ListSourceRow];
		if (dataLinkObjectExtendedForTerms != null && dataLinkObjectExtendedForTerms.IsDeleted)
		{
			e.Visible = false;
			e.Handled = true;
		}
	}

	private void DataLinksGridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		if (e.Column.Equals(termIconDataLinksGridColumn))
		{
			DataLinkObjectExtendedForTerms dataLinkObjectExtendedForTerms = e.Row as DataLinkObjectExtendedForTerms;
			e.Value = BusinessGlossarySupport.GetTermIcon(dataLinkObjectExtendedForTerms.TermIconId);
		}
		else if (e.Column.Equals(objectIconDataLinksGridColumn))
		{
			DataLinkObjectExtended dataLinkObjectExtended = e.Row as DataLinkObjectExtended;
			e.Value = IconsSupport.GetObjectIcon(dataLinkObjectExtended.TypeForIcon, dataLinkObjectExtended.SubtypeForIcon, dataLinkObjectExtended.SourceForIcon);
		}
	}

	private void DataLinksGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		dataLinksGridPanelUserControl.SetButtonVisibility(dataLinksAddLinkBarButtonItem, Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary) && (dataLinksGridView.SelectedRowsCount == 0 || dataLinksGridView.SelectedRowsCount == 1));
		dataLinksGridPanelUserControl.SetRemoveButtonVisibility(dataLinksGridView.SelectedRowsCount > 0);
	}

	private void DataLinksGridView_DataSourceChanged(object sender, EventArgs e)
	{
		dataLinksGridPanelUserControl.SetButtonVisibility(dataLinksAddLinkBarButtonItem, Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary));
		dataLinksGridPanelUserControl.SetRemoveButtonVisibility(value: false);
	}

	private void TableColumnsGridView_RowCellClick(object sender, RowCellClickEventArgs e)
	{
		SetColumnsAddLinkBarButtonItemVisibility();
	}

	private void TableColumnsGridView_DataSourceChanged(object sender, EventArgs e)
	{
		SetColumnsAddLinkBarButtonItemVisibility();
	}

	private void SetColumnsAddLinkBarButtonItemVisibility()
	{
		columnsGridPanelUserControl.SetButtonVisibility(columnsAddLinkBarButtonItem, Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary) && tableColumnsGridView.SelectedRowsCount == 1 && tableColumnsGridView.FocusedRowHandle >= 0);
	}

	public DataLinksManager(BasePanelControl basePanelControl, XtraTabControl tableXtraTabControl, XtraTabPage dataLinksXtraTabPage, GridPanelUserControl dataLinksGridPanelUserControl, GridPanelUserControl columnsGridPanelUserControl, GridControl dataLinksGrid, BulkCopyGridUserControl dataLinksGridView, GridControl tableColumnsGrid, BulkCopyGridUserControl tableColumnsGridView, GridColumn dataLinksGridColumn, GridColumn termIconDataLinksGridColumn, GridColumn objectIconDataLinksGridColumn, PopupMenu linkedTermsPopupMenu, BarButtonItem goToObjectDataLinksToolStripMenuItem, BarButtonItem addLinkDataLinksToolStripMenuItem, BarButtonItem removeLinkDataLinksToolStripMenuItem)
	{
		this.basePanelControl = basePanelControl;
		this.tableXtraTabControl = tableXtraTabControl;
		this.dataLinksXtraTabPage = dataLinksXtraTabPage;
		this.dataLinksGridPanelUserControl = dataLinksGridPanelUserControl;
		this.columnsGridPanelUserControl = columnsGridPanelUserControl;
		this.dataLinksGrid = dataLinksGrid;
		this.dataLinksGridView = dataLinksGridView;
		this.tableColumnsGrid = tableColumnsGrid;
		this.tableColumnsGridView = tableColumnsGridView;
		this.dataLinksGridColumn = dataLinksGridColumn;
		this.termIconDataLinksGridColumn = termIconDataLinksGridColumn;
		this.objectIconDataLinksGridColumn = objectIconDataLinksGridColumn;
		this.linkedTermsPopupMenu = linkedTermsPopupMenu;
		this.goToObjectDataLinksToolStripMenuItem = goToObjectDataLinksToolStripMenuItem;
		this.addLinkDataLinksToolStripMenuItem = addLinkDataLinksToolStripMenuItem;
		this.removeLinkDataLinksToolStripMenuItem = removeLinkDataLinksToolStripMenuItem;
	}

	public void SetParameters(bool showSchema, int tableId, string tableName)
	{
		this.tableId = tableId;
		this.tableName = tableName;
		this.showSchema = showSchema;
		dataLinksGridPanelUserControl.SetRemoveButton("Remove data link", Resources.delete_16);
	}

	public void SetEvents(BulkCopyGridUserControl dataLinksGridView, GridPanelUserControl dataLinksGridPanelUserControl, GridPanelUserControl columnsGridPanelUserControl, BarButtonItem removeLinkDataLinksToolStripMenuItem, BarButtonItem addLinkDataLinksToolStripMenuItem, BarButtonItem goToObjectDataLinksToolStripMenuItem, PopupMenu linkedTermsPopupMenu, BarButtonItem editDataLinksToolStripMenuItem)
	{
		this.dataLinksGridView.CustomRowFilter += DataLinksGridView_CustomRowFilter;
		this.dataLinksGridPanelUserControl.Delete += ProcessDeletingLink;
		this.dataLinksGridView.SelectionChanged += DataLinksGridView_SelectionChanged;
		this.dataLinksGridView.DataSourceChanged += DataLinksGridView_DataSourceChanged;
		this.dataLinksGridView.CustomUnboundColumnData += DataLinksGridView_CustomUnboundColumnData;
		tableColumnsGridView.RowCellClick += TableColumnsGridView_RowCellClick;
		tableColumnsGridView.DataSourceChanged += TableColumnsGridView_DataSourceChanged;
		removeLinkDataLinksToolStripMenuItem.ItemClick += RemoveLinkDataLinksToolStripMenuItem_ItemClick;
		addLinkDataLinksToolStripMenuItem.ItemClick += AddLinkDataLinksToolStripMenuItem_ItemClick;
		goToObjectDataLinksToolStripMenuItem.ItemClick += GoToObjectDataLinksToolStripMenuItem_ItemClick;
		editDataLinksToolStripMenuItem.ItemClick += AddDataLinkToolStripMenuItem_ItemClick;
		dataLinksGridView.GridControl.ProcessGridKey += delegate(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && !dataLinksGridView.IsEditing)
			{
				ProcessDeletingLink();
			}
		};
		this.dataLinksGridView.PopupMenuShowing += DataLinksGridView_PopupMenuShowing;
	}

	public void RefreshDataLinks(bool forceRefresh = false, bool refreshImmediatelyIfLoaded = false, bool refreshImmediatelyIfNotLoaded = false)
	{
		if (IsRefreshRequired(dataLinksXtraTabPage, !IsDataLinksDataLoaded, forceRefresh) || (refreshImmediatelyIfNotLoaded && !IsDataLinksDataLoaded) || (refreshImmediatelyIfLoaded && IsDataLinksDataLoaded))
		{
			MainControl.SetWaitformVisibility(visible: true);
			objectsWithDataLink = new BindingList<DataLinkObjectExtendedForTerms>(DB.BusinessGlossary.GetDataLinks<DataLinkObjectExtendedForTerms>(null, tableId));
			foreach (DataLinkObjectExtendedForTerms item in objectsWithDataLink)
			{
				item.ContextShowSchema = showSchema;
			}
			dataLinksGrid.BeginUpdate();
			dataLinksGrid.DataSource = objectsWithDataLink;
			dataLinksGrid.EndUpdate();
			CommonFunctionsPanels.SetBestFitForColumns(dataLinksGridView);
			FillHighlights(SharedObjectTypeEnum.ObjectType.DataLink);
			IsDataLinksDataLoaded = true;
			MainControl.SetWaitformVisibility(visible: false);
			dataLinksGridPanelUserControl.SetRemoveButtonVisibility(dataLinksGridView.SelectedRowsCount > 0);
		}
		else if (forceRefresh)
		{
			IsDataLinksDataLoaded = false;
		}
	}

	public void AddEventForAutoFilterRow()
	{
		CommonFunctionsPanels.AddEventForAutoFilterRow(dataLinksGridView);
	}

	public void AddToolTipDataForColumnsGrid(List<ToolTipData> toolTipDataList, GridControl tableColumnsGrid, GridColumn nameTableColumnsGridColumn, GridColumn dataLinksGridColumn)
	{
		toolTipDataList.Add(new ToolTipData(tableColumnsGrid, SharedObjectTypeEnum.ObjectType.DataLink, nameTableColumnsGridColumn.VisibleIndex));
		toolTipDataList.Add(new ToolTipData(tableColumnsGrid, SharedObjectTypeEnum.ObjectType.DataLink, dataLinksGridColumn.VisibleIndex));
	}

	public void AddToolTipDataForDataLinkGrid(List<ToolTipData> toolTipDataList, GridControl dataLinksGrid, GridColumn termIconDataLinksGridColumn, GridColumn objectDataLinksGridColumn, GridColumn typeDataLinksGridColumn, GridColumn objectIconDataLinksGridColumn)
	{
		toolTipDataList.Add(new ToolTipData(dataLinksGrid, SharedObjectTypeEnum.ObjectType.DataLink, termIconDataLinksGridColumn.VisibleIndex));
		toolTipDataList.Add(new ToolTipData(dataLinksGrid, SharedObjectTypeEnum.ObjectType.DataLink, objectDataLinksGridColumn.VisibleIndex));
		toolTipDataList.Add(new ToolTipData(dataLinksGrid, SharedObjectTypeEnum.ObjectType.DataLink, typeDataLinksGridColumn.VisibleIndex));
		toolTipDataList.Add(new ToolTipData(dataLinksGrid, SharedObjectTypeEnum.ObjectType.Table, objectIconDataLinksGridColumn.VisibleIndex));
	}

	public void ProcessUpdating(ref bool isError)
	{
		if (!isError && UpdateDataLinks(dataLinksGrid?.FindForm()))
		{
			basePanelControl.SetTabPageTitle(isEdited: false, dataLinksXtraTabPage);
		}
		else if (!isError)
		{
			isError = true;
		}
	}

	private bool IsRefreshRequired(XtraTabPage tabPage, bool additionalCondition = true, bool forceRefresh = false)
	{
		if (!forceRefresh || tableXtraTabControl.SelectedTabPage != tabPage)
		{
			if (tableXtraTabControl.SelectedTabPage == tabPage && additionalCondition)
			{
				return !TabPageChangedProgrammatically;
			}
			return false;
		}
		return true;
	}

	private void FillHighlights(SharedObjectTypeEnum.ObjectType? elementType)
	{
		if (UserControlHelpers.IsSearchActive && UserControlHelpers != null && LastSearchResult != null)
		{
			MainControl.FillHighlights(LastSearchResult, allowChangingTab: false, elementType);
		}
	}
}
