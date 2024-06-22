using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DataProfiling.Models;
using Dataedo.App.DataProfiling.Tools;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.DataSources.Base.DataProfiling.Model;
using Dataedo.Model.Data.DataProfiling;
using Dataedo.Shared.Enums;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Data;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.DataProfiling.DataProfilingUserControls;

public class NavigationUserControl : BaseUserControl
{
	private BindingList<INavigationObject> navigationModelData;

	private INavigationObject lastNavObjectPPM;

	private IContainer components;

	private NonCustomizableLayoutControl leftSectionNavigationLayoutControl;

	private HyperLinkEdit selectNoneHyperLinkEdit;

	private HyperLinkEdit selectAllHyperLinkEdit;

	private TreeList navigationTreeList;

	private TreeListColumn iconTreeListColumn;

	private RepositoryItemPictureEdit repositoryItemPictureEdit1;

	private TreeListColumn profileCheckboxTreeListColumn;

	private RepositoryItemCheckEdit profileCheckboxRepositoryItemCheckEdit;

	private TreeListColumn nameTreeListColumn;

	private TreeListColumn titleTreeListColumn;

	private TreeListColumn datatypeTreeListColumn;

	private TreeListColumn completionTreeListColumn;

	private TreeListColumn sparklineRowDistributionColumn;

	private TreeListColumn sparklineTopValuesColumn;

	private RepositoryItemProgressBar completionEndedTreeListProgressBarControl;

	private RepositoryItemProgressBar completionNotEndedProgressBarControl;

	private HyperLinkEdit allEmptyHyperLinkEdit1;

	private LayoutControlGroup columnsNavigationLayoutControlGroup;

	private LayoutControlItem treeNavigationListLayoutControlItem;

	private LayoutControlItem allCheckHyperLinkEditLayoutControlItem;

	private LayoutControlItem noneCheckLayoutControlItem;

	private EmptySpaceItem emptySpaceItem6;

	private LayoutControlItem allEmptyHyperLinkEditLayoutControlItem1;

	private EmptySpaceItem emptySpaceItem7;

	private PopupMenu navigationValuesPopupMenu;

	private BarManager barManager1;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private BarButtonItem previewSampleDataBarButtonItem;

	private BarButtonItem profileColumnBarButtonItem;

	private BarButtonItem clearAllDataButtonItem;

	private BarButtonItem profileTableBarButtonItem;

	private RepositoryItemProgressBar completionRepositoryItemProgressBar;

	private LabelControl dbTitleLabelControl;

	private LayoutControlItem dbTitleLayoutControlItem;

	private EmptySpaceItem navTopEmptySpaceItem;

	private HelpIconUserControl helpIconUserControl;

	private LayoutControlItem helpIconLayoutControlItem;

	private RepositoryItemProgressBar completionWithErrorsProgressBarControl;

	private RepositoryItemTextEdit noDataRepositoryItemTextEdit;

	private RepositoryItemTextEdit completionErrorItemTextEdit;

	private DataProfilingForm DataProfilingForm => FindForm() as DataProfilingForm;

	private INavigationObject FocusedNavigationObject => navigationTreeList.GetDataRecordByNode(navigationTreeList.FocusedNode) as INavigationObject;

	private TableNavigationObject FocusedTableNavigationObject => FocusedNavigationObject as TableNavigationObject;

	public IEnumerable<TableNavigationObject> AllNavTables => navigationModelData.OfType<TableNavigationObject>();

	public IEnumerable<ColumnNavigationObject> AllNavColumns => navigationModelData.OfType<ColumnNavigationObject>();

	public IEnumerable<INavigationObject> AllNavObjects => navigationModelData;

	public bool IsThereAnyProfilingData => navigationModelData.OfType<ColumnNavigationObject>().Any((ColumnNavigationObject x) => x != null && (x.Column?.ProfilingDate).HasValue);

	public bool NoObjectsSelectedForProfiling => navigationModelData.Where((INavigationObject c) => c.ProfileCheckbox).Count() == 0;

	public int WidthWithoutHorizontalScroll
	{
		get
		{
			int num = navigationTreeList.Width - navigationTreeList.ViewInfo.ViewRects.Client.Width;
			int columnTotalWidth = navigationTreeList.ViewInfo.ViewRects.ColumnTotalWidth;
			int num2 = treeNavigationListLayoutControlItem.Padding.Width;
			return columnTotalWidth + num + num2;
		}
	}

	public int CalculatedWidth
	{
		get
		{
			int num = navigationTreeList.Width - navigationTreeList.ViewInfo.ViewRects.Client.Width;
			int num2 = navigationTreeList.VisibleColumns.Sum((TreeListColumn x) => x.Width);
			int num3 = treeNavigationListLayoutControlItem.Padding.Width;
			return num2 + num + num3;
		}
	}

	public event EventHandler<INavigationObject> FocusedNavigationObjectChanged;

	public NavigationUserControl()
	{
		InitializeComponent();
		navigationModelData = new BindingList<INavigationObject>();
	}

	public void Init()
	{
		navigationTreeList.MouseWheel += delegate(object s, MouseEventArgs e)
		{
			NavigationTreeList_MouseDown(e);
		};
		profileCheckboxRepositoryItemCheckEdit.CheckedChanged += delegate(object s, EventArgs e)
		{
			ProfileCheckboxChanged((s as CheckEdit)?.Checked);
		};
		ChangeSelectedCellsColor();
		SetNavigationTreeListFieldNames();
	}

	public void SetDatabaseTitleAndImage(string dbTitle, SharedDatabaseTypeEnum.DatabaseType? databaseType)
	{
		int num = TextRenderer.MeasureText(dbTitle, dbTitleLabelControl.Font).Width + dbTitleLayoutControlItem.Padding.Width + dbTitleLabelControl.ImageOptions.Image.Width;
		int num2 = dbTitleLayoutControlItem.MaxSize.Width;
		if (num > num2)
		{
			num = num2;
		}
		int num3 = dbTitleLayoutControlItem.Height;
		dbTitleLayoutControlItem.MinSize = new Size(num, num3);
		dbTitleLayoutControlItem.MaxSize = new Size(num, num3);
		dbTitleLayoutControlItem.Size = new Size(num, num3);
		dbTitleLabelControl.Text = dbTitle;
		dbTitleLabelControl.ImageOptions.Image = IconsSupport.GetDatabaseIconByName16(databaseType, SharedObjectTypeEnum.ObjectType.Database);
	}

	private void ChangeSelectedCellsColor()
	{
		navigationTreeList.Appearance.SelectedRow.BackColor = SkinsManager.CurrentSkin.GridSelectionBackColor;
		navigationTreeList.Appearance.SelectedRow.Options.UseBackColor = true;
		navigationTreeList.Appearance.FocusedRow.BackColor = SkinsManager.CurrentSkin.GridHighlightRowBackColor;
		navigationTreeList.Appearance.FocusedRow.Options.UseBackColor = true;
		navigationTreeList.Appearance.FocusedCell.BackColor = SkinsManager.CurrentSkin.GridSelectionBackColor;
		navigationTreeList.Appearance.FocusedCell.Options.UseBackColor = true;
	}

	private void SetNavigationTreeListFieldNames()
	{
		navigationTreeList.KeyFieldName = "IdForTreeList";
		navigationTreeList.ParentFieldName = "ParentTreeNodeId";
	}

	private void NavigationTreeList_MouseDown(MouseEventArgs e)
	{
		PostEditor();
		if (e.Delta > 0)
		{
			navigationTreeList.HorzScrollStep--;
		}
		else
		{
			navigationTreeList.HorzScrollStep++;
		}
	}

	private void ProfileCheckboxChanged(bool? isChecked)
	{
		if (!isChecked.HasValue)
		{
			return;
		}
		PostEditor();
		TableNavigationObject focusedTableNavigationObject = FocusedTableNavigationObject;
		if (focusedTableNavigationObject == null)
		{
			return;
		}
		foreach (ColumnNavigationObject column in focusedTableNavigationObject.Columns)
		{
			column.ProfileCheckbox = isChecked.Value;
		}
		RefreshNavigationTreeList();
	}

	public void LoadNavigationData(IEnumerable<TableSimpleData> tables)
	{
		navigationModelData = new BindingList<INavigationObject>();
		foreach (TableSimpleData table in tables)
		{
			TableNavigationObject navTable = AddTableToNavigation(table);
			AddColumnsToNavigation(navTable);
		}
		navigationTreeList.BeginUpdate();
		navigationTreeList.DataSource = null;
		navigationTreeList.DataSource = navigationModelData;
		if (tables.Count() == 1)
		{
			navigationTreeList.ExpandAll();
		}
		navigationTreeList.EndUpdate();
	}

	private TableNavigationObject AddTableToNavigation(TableSimpleData tableSimpleData)
	{
		TableNavigationObject tableNavigationObject = new TableNavigationObject(tableSimpleData);
		navigationModelData.Add(tableNavigationObject);
		return tableNavigationObject;
	}

	private void AddColumnsToNavigation(TableNavigationObject navTable)
	{
		int tableId = navTable.TableId;
		foreach (ColumnProfiledDataObject tableAllColumnsProfileDataObject in DataProfilingForm.GetTableAllColumnsProfileDataObjects(tableId))
		{
			ColumnNavigationObject item = new ColumnNavigationObject(navTable, tableAllColumnsProfileDataObject, tableAllColumnsProfileDataObject.ColumnId, navTable.TableId, null);
			navigationModelData.Add(item);
			navTable.Columns.Add(item);
		}
	}

	public TableNavigationObject GetTableNavigationObject(int tableId)
	{
		return (from x in navigationModelData.OfType<TableNavigationObject>()
			where x.TableId == tableId
			select x).FirstOrDefault();
	}

	public ColumnNavigationObject GetColumnNavigationObject(int columnId)
	{
		return (from x in navigationModelData.OfType<ColumnNavigationObject>()
			where x.ColumnId == columnId
			select x).FirstOrDefault();
	}

	public bool IsTableInCurrentProfilingSession(int tableId)
	{
		return navigationModelData.Where((INavigationObject x) => x.TableId == tableId).Any();
	}

	public void SetFocusOnSpecificColumn(int columnId)
	{
		ColumnNavigationObject columnNavigationObject = GetColumnNavigationObject(columnId);
		if (columnNavigationObject != null)
		{
			int index = navigationModelData.IndexOf(columnNavigationObject);
			TreeListNode nodeByVisibleIndex = navigationTreeList.GetNodeByVisibleIndex(index);
			if (nodeByVisibleIndex != null)
			{
				navigationTreeList.SetFocusedNode(nodeByVisibleIndex);
			}
		}
	}

	public void SelectOnlyOneColumnForProfiling(int columnId)
	{
		navigationModelData.ForEach(delegate(INavigationObject x)
		{
			x.ProfileCheckbox = false;
		});
		GetColumnNavigationObject(columnId).ProfileCheckbox = true;
	}

	public void PostEditor()
	{
		navigationTreeList.PostEditor();
	}

	private void NavigationTreeList_AfterExpand(object sender, NodeEventArgs e)
	{
		if (!(navigationModelData[e.Node.Id] is TableNavigationObject tableNavigationObject) || !e.Node.Expanded)
		{
			return;
		}
		try
		{
			tableNavigationObject.ListOfValues = DB.DataProfiling.SelectColumnValuesDataObjectForTable(tableNavigationObject.TableId);
		}
		catch (Exception ex)
		{
			DataProfiler.ShowErrorMessageBox(ex, FindForm());
		}
	}

	private void NavigationTreeList_RowClick(object sender, RowClickEventArgs e)
	{
		if (e.Button == MouseButtons.Right)
		{
			int id = e.Node.Id;
			INavigationObject navigationObject = (lastNavObjectPPM = navigationTreeList.GetRow(id) as INavigationObject);
			if (navigationObject is ColumnNavigationObject)
			{
				previewSampleDataBarButtonItem.Visibility = BarItemVisibility.Never;
				profileTableBarButtonItem.Visibility = BarItemVisibility.Never;
				profileColumnBarButtonItem.Visibility = BarItemVisibility.Always;
				navigationValuesPopupMenu.ShowPopup(Control.MousePosition);
			}
			else if (navigationObject is TableNavigationObject tableNavigationObject)
			{
				profileTableBarButtonItem.Caption = $"Profile {tableNavigationObject.ObjectType} (all columns)";
				previewSampleDataBarButtonItem.Visibility = BarItemVisibility.Always;
				profileTableBarButtonItem.Visibility = BarItemVisibility.Always;
				profileColumnBarButtonItem.Visibility = BarItemVisibility.Never;
				navigationValuesPopupMenu.ShowPopup(Control.MousePosition);
			}
		}
	}

	private void NavigationTreeList_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
	{
		e.Allow = false;
	}

	private void NavigationTreeList_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
	{
		if (e.Column.Name != "completionTreeListColumn")
		{
			return;
		}
		INavigationObject navigationObject = navigationModelData[e.Node.Id];
		if (navigationObject.RowsCount == 0)
		{
			e.RepositoryItem = noDataRepositoryItemTextEdit;
		}
		else if (navigationObject is TableNavigationObject tableNavigationObject)
		{
			if (tableNavigationObject.Columns.All((ColumnNavigationObject x) => x.ProfilingFailed))
			{
				e.RepositoryItem = completionErrorItemTextEdit;
			}
			else if (tableNavigationObject.AllColumnProfiled && tableNavigationObject.Columns.Any((ColumnNavigationObject x) => x.ErrorOccurred))
			{
				e.RepositoryItem = completionWithErrorsProgressBarControl;
			}
			else if (tableNavigationObject.AllColumnProfiled && tableNavigationObject.Completion == 100.0)
			{
				e.RepositoryItem = completionEndedTreeListProgressBarControl;
			}
			else if (navigationObject.Completion < 100.0)
			{
				e.RepositoryItem = completionNotEndedProgressBarControl;
			}
		}
		else if (navigationObject is ColumnNavigationObject columnNavigationObject)
		{
			if (columnNavigationObject.ProfilingFailed)
			{
				e.RepositoryItem = completionErrorItemTextEdit;
			}
			else if (columnNavigationObject.ProfilingCompletedWithErrors)
			{
				e.RepositoryItem = completionWithErrorsProgressBarControl;
			}
			else if (columnNavigationObject.Completion == 100.0)
			{
				e.RepositoryItem = completionEndedTreeListProgressBarControl;
			}
			else if (columnNavigationObject.Completion < 100.0)
			{
				e.RepositoryItem = completionNotEndedProgressBarControl;
			}
		}
	}

	private void NavigationTreeList_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
	{
		if (e.Column == completionTreeListColumn)
		{
			if (e.CellText == "NO DATA")
			{
				e.Cache.FillRectangle(new SolidBrush(Color.Gray), e.Bounds.ApplyPadding(new System.Windows.Forms.Padding(2, 1, 2, 1)));
				e.DefaultDraw();
				e.Handled = true;
			}
			else if (e.CellText == "FAILED")
			{
				e.Cache.FillRectangle(new SolidBrush(Color.Red), e.Bounds.ApplyPadding(new System.Windows.Forms.Padding(2, 1, 2, 1)));
				e.DefaultDraw();
				e.Handled = true;
			}
			return;
		}
		string text = "sparklineRowDistributionColumn";
		string text2 = "sparklineTopValuesColumn";
		if ((e?.Column?.Name != text && e?.Column?.Name != text2) || !(navigationTreeList.GetRow(e.Node.Id) as INavigationObject is ColumnNavigationObject columnNavigationObject))
		{
			return;
		}
		ColumnProfiledDataObject profiledColumn = columnNavigationObject.Column;
		if (profiledColumn == null)
		{
			return;
		}
		if (columnNavigationObject.TextForSparkline == "100% NULL")
		{
			e.Appearance.ForeColor = Color.Black;
		}
		else
		{
			e.Appearance.ForeColor = Color.White;
		}
		if (!profiledColumn.RowCount.HasValue || profiledColumn.RowCount == 0)
		{
			return;
		}
		if (e.Column.Name == text)
		{
			DataProfilingUtils.ShowRowDistributionSparklines(e.Cache, e.Bounds, profiledColumn);
		}
		else if (e.Column.Name == text2 && (profiledColumn.ValuesListMode == "T" || profiledColumn.ValuesListMode == "R"))
		{
			int count = 20;
			List<long?> list = null;
			if (list == null || list.Count == 0)
			{
				list = columnNavigationObject.FieldMostCommonlyUsedValues?.Where(delegate(StringValueWithCount x)
				{
					if (x == null)
					{
						return false;
					}
					_ = x.Count;
					return true;
				})?.Take(count)?.Select((Func<StringValueWithCount, long?>)((StringValueWithCount x) => x?.Count))?.ToList();
			}
			if (list == null || list.Count == 0)
			{
				list = columnNavigationObject.Table.ListOfValues?.Where((ColumnValuesDataObject c) => c.ColumnId == profiledColumn.ColumnId)?.Where((ColumnValuesDataObject x) => x?.RowCount.HasValue ?? false)?.Take(count)?.Select((ColumnValuesDataObject x) => x?.RowCount)?.ToList();
			}
			if (list == null || list.Count == 0)
			{
				list = columnNavigationObject.ListOfValues?.Where((ColumnValuesDataObject x) => x?.RowCount.HasValue ?? false)?.Take(count)?.Select((ColumnValuesDataObject x) => x?.RowCount)?.ToList();
			}
			if (list == null || !list.Any())
			{
				return;
			}
			DataProfilingUtils.DrawValuesSparklines(list, e.Cache, e.Bounds, 2);
		}
		e.Appearance.BackColor = Color.Transparent;
		e.Appearance.BackColor2 = Color.Transparent;
		e.DefaultDraw();
		e.Handled = true;
	}

	private void NavigationTreeList_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		this.FocusedNavigationObjectChanged?.Invoke(this, FocusedNavigationObject);
	}

	private void navigationTreeList_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		if (e.Column == completionTreeListColumn)
		{
			string displayText = e.Node.GetDisplayText(e.Column);
			if (displayText == "FAILED" || displayText == "NO DATA")
			{
				e.Appearance.ForeColor = Color.White;
			}
		}
	}

	private void navigationTreeList_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		if (e.Column != nameTreeListColumn || string.IsNullOrWhiteSpace(e.DisplayText))
		{
			return;
		}
		INavigationObject navigationObject = navigationTreeList.GetDataRecordByNode(e.Node) as INavigationObject;
		if (navigationObject is ColumnNavigationObject columnNavigationObject)
		{
			if (DataProfilingForm.ColumnRequiresSaving(columnNavigationObject.TableId, columnNavigationObject.ColumnId) && !e.DisplayText.EndsWith("*", StringComparison.InvariantCulture))
			{
				e.DisplayText += "*";
			}
		}
		else if (navigationObject is TableNavigationObject tableNavigationObject && tableNavigationObject.Columns.Any((ColumnNavigationObject x) => DataProfilingForm.ColumnRequiresSaving(x.TableId, x.ColumnId)) && !e.DisplayText.EndsWith("*", StringComparison.InvariantCulture))
		{
			e.DisplayText += "*";
		}
	}

	private void CheckAllHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		foreach (INavigationObject navigationModelDatum in navigationModelData)
		{
			navigationModelDatum.ProfileCheckbox = true;
		}
		PostEditor();
		RefreshNavigationTreeList();
	}

	private void AllEmptyHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		foreach (TableNavigationObject allNavTable in AllNavTables)
		{
			foreach (ColumnNavigationObject column in allNavTable.Columns)
			{
				if (column == null || !(column.Column?.ProfilingDate).HasValue)
				{
					column.ProfileCheckbox = true;
				}
				else
				{
					column.ProfileCheckbox = false;
				}
			}
			if (allNavTable.Columns.All((ColumnNavigationObject x) => x.ProfileCheckbox))
			{
				allNavTable.ProfileCheckbox = true;
			}
			else
			{
				allNavTable.ProfileCheckbox = false;
			}
		}
		PostEditor();
		RefreshNavigationTreeList();
	}

	private void UnCheckAllhyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		foreach (INavigationObject navigationModelDatum in navigationModelData)
		{
			navigationModelDatum.ProfileCheckbox = false;
		}
		PostEditor();
		RefreshNavigationTreeList();
	}

	private async void ClearAllDataButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (lastNavObjectPPM is TableNavigationObject tableNavigationObject)
		{
			if (GeneralMessageBoxesHandling.Show("Do you want to delete all profiling data from the <b>" + lastNavObjectPPM.Name + "</b> " + tableNavigationObject.ObjectType.ToString().ToLower() + "?", "Clear all Profiling Data", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, null, 1, FindForm()).DialogResult == DialogResult.OK)
			{
				await DataProfilingForm.ClearAllTableProfilingAsync(tableNavigationObject);
			}
		}
		else if (lastNavObjectPPM is ColumnNavigationObject navColumn)
		{
			await DataProfilingForm.ClearAllColumnProfilingAsync(navColumn);
		}
	}

	private async void ProfileTablePPMBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (lastNavObjectPPM is TableNavigationObject tableNavigationObject)
		{
			await DataProfilingForm.FullProfileSingleNavTableAsync(tableNavigationObject.TableId);
		}
	}

	private async void PreviewSampleDataPPMBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (lastNavObjectPPM is TableNavigationObject navTable)
		{
			await DataProfilingForm.PreviewSampleDataForSingleNavTableAsync(navTable);
		}
	}

	private async void ProfileColumnPPMBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (lastNavObjectPPM is ColumnNavigationObject columnNavigationObject)
		{
			await DataProfilingForm.FullProfileSingleNavColumnAsync(columnNavigationObject.ColumnId);
		}
	}

	public void EnablePopupMenuButtons()
	{
		profileColumnBarButtonItem.Enabled = true;
		profileTableBarButtonItem.Enabled = true;
		clearAllDataButtonItem.Enabled = true;
		previewSampleDataBarButtonItem.Enabled = true;
	}

	public void DisablePopupMenuButtons()
	{
		profileColumnBarButtonItem.Enabled = false;
		profileTableBarButtonItem.Enabled = false;
		clearAllDataButtonItem.Enabled = false;
		previewSampleDataBarButtonItem.Enabled = false;
	}

	public void RefreshNavigationTreeList()
	{
		navigationTreeList.Refresh();
		navigationTreeList.RefreshNode(navigationTreeList.FocusedNode);
	}

	public void RefreshNode(INavigationObject navObj)
	{
		if (navObj == null || navigationTreeList?.AccessibilityObject == null)
		{
			return;
		}
		TreeListColumn treeListColumn = completionTreeListColumn;
		if (treeListColumn != null && treeListColumn.Visible)
		{
			int index = navigationModelData.IndexOf(navObj);
			TreeListNode nodeByVisibleIndex = navigationTreeList.GetNodeByVisibleIndex(index);
			if (nodeByVisibleIndex != null)
			{
				navigationTreeList.RefreshCell(nodeByVisibleIndex, completionTreeListColumn);
			}
			RefreshNavigationTreeList();
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.completionRepositoryItemProgressBar = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
		this.leftSectionNavigationLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.helpIconUserControl = new Dataedo.App.UserControls.HelpIconUserControl();
		this.dbTitleLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.selectNoneHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.selectAllHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.navigationTreeList = new DevExpress.XtraTreeList.TreeList();
		this.iconTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.repositoryItemPictureEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.profileCheckboxTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.profileCheckboxRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.nameTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.titleTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.datatypeTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.completionTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.sparklineRowDistributionColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.sparklineTopValuesColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.completionEndedTreeListProgressBarControl = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
		this.completionNotEndedProgressBarControl = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
		this.completionWithErrorsProgressBarControl = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
		this.noDataRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.completionErrorItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.allEmptyHyperLinkEdit1 = new DevExpress.XtraEditors.HyperLinkEdit();
		this.columnsNavigationLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.treeNavigationListLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.allCheckHyperLinkEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.allEmptyHyperLinkEditLayoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem7 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.noneCheckLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.dbTitleLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.navTopEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.helpIconLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.clearAllDataButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.previewSampleDataBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.profileColumnBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.profileTableBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.navigationValuesPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		((System.ComponentModel.ISupportInitialize)this.completionRepositoryItemProgressBar).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.leftSectionNavigationLayoutControl).BeginInit();
		this.leftSectionNavigationLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.selectNoneHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.selectAllHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.navigationTreeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemPictureEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.profileCheckboxRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.completionEndedTreeListProgressBarControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.completionNotEndedProgressBarControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.completionWithErrorsProgressBarControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.noDataRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.completionErrorItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allEmptyHyperLinkEdit1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnsNavigationLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.treeNavigationListLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allCheckHyperLinkEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allEmptyHyperLinkEditLayoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.noneCheckLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dbTitleLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.navTopEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.helpIconLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.navigationValuesPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).BeginInit();
		base.SuspendLayout();
		this.completionRepositoryItemProgressBar.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.completionRepositoryItemProgressBar.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.completionRepositoryItemProgressBar.EndColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.completionRepositoryItemProgressBar.Name = "completionRepositoryItemProgressBar";
		this.completionRepositoryItemProgressBar.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.completionRepositoryItemProgressBar.StartColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.leftSectionNavigationLayoutControl.AllowCustomization = false;
		this.leftSectionNavigationLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.leftSectionNavigationLayoutControl.Controls.Add(this.helpIconUserControl);
		this.leftSectionNavigationLayoutControl.Controls.Add(this.dbTitleLabelControl);
		this.leftSectionNavigationLayoutControl.Controls.Add(this.selectNoneHyperLinkEdit);
		this.leftSectionNavigationLayoutControl.Controls.Add(this.selectAllHyperLinkEdit);
		this.leftSectionNavigationLayoutControl.Controls.Add(this.navigationTreeList);
		this.leftSectionNavigationLayoutControl.Controls.Add(this.allEmptyHyperLinkEdit1);
		this.leftSectionNavigationLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.leftSectionNavigationLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.leftSectionNavigationLayoutControl.Name = "leftSectionNavigationLayoutControl";
		this.leftSectionNavigationLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(999, 663, 650, 400);
		this.leftSectionNavigationLayoutControl.Root = this.columnsNavigationLayoutControlGroup;
		this.leftSectionNavigationLayoutControl.Size = new System.Drawing.Size(300, 719);
		this.leftSectionNavigationLayoutControl.TabIndex = 5;
		this.leftSectionNavigationLayoutControl.Text = "layoutControl2";
		this.helpIconUserControl.AutoPopDelay = 5000;
		this.helpIconUserControl.BackColor = System.Drawing.Color.Transparent;
		this.helpIconUserControl.KeepWhileHovered = false;
		this.helpIconUserControl.Location = new System.Drawing.Point(268, 12);
		this.helpIconUserControl.MaximumSize = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.MaxToolTipWidth = 500;
		this.helpIconUserControl.MinimumSize = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.Name = "helpIconUserControl";
		this.helpIconUserControl.Size = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.TabIndex = 28;
		this.helpIconUserControl.ToolTipHeader = null;
		this.helpIconUserControl.ToolTipText = "Select tables, views and columns and use the menu to start profiling.";
		this.dbTitleLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold);
		this.dbTitleLabelControl.Appearance.Options.UseFont = true;
		this.dbTitleLabelControl.AutoEllipsis = true;
		this.dbTitleLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.dbTitleLabelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.dbTitleLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftBottom;
		this.dbTitleLabelControl.ImageOptions.Image = Dataedo.App.Properties.Resources.documentation_16;
		this.dbTitleLabelControl.Location = new System.Drawing.Point(2, 2);
		this.dbTitleLabelControl.Name = "dbTitleLabelControl";
		this.dbTitleLabelControl.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
		this.dbTitleLabelControl.Size = new System.Drawing.Size(262, 40);
		this.dbTitleLabelControl.StyleController = this.leftSectionNavigationLayoutControl;
		this.dbTitleLabelControl.TabIndex = 27;
		this.dbTitleLabelControl.Text = "dbTitle";
		this.selectNoneHyperLinkEdit.EditValue = "Select none";
		this.selectNoneHyperLinkEdit.Location = new System.Drawing.Point(162, 46);
		this.selectNoneHyperLinkEdit.Name = "selectNoneHyperLinkEdit";
		this.selectNoneHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.selectNoneHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.selectNoneHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.selectNoneHyperLinkEdit.Size = new System.Drawing.Size(66, 18);
		this.selectNoneHyperLinkEdit.StyleController = this.leftSectionNavigationLayoutControl;
		this.selectNoneHyperLinkEdit.TabIndex = 25;
		this.selectNoneHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(UnCheckAllhyperLinkEdit_OpenLink);
		this.selectAllHyperLinkEdit.EditValue = "Select all";
		this.selectAllHyperLinkEdit.Location = new System.Drawing.Point(2, 46);
		this.selectAllHyperLinkEdit.Name = "selectAllHyperLinkEdit";
		this.selectAllHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.selectAllHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.selectAllHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.selectAllHyperLinkEdit.Size = new System.Drawing.Size(54, 18);
		this.selectAllHyperLinkEdit.StyleController = this.leftSectionNavigationLayoutControl;
		this.selectAllHyperLinkEdit.TabIndex = 24;
		this.selectAllHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(CheckAllHyperLinkEdit_OpenLink);
		this.navigationTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[8] { this.iconTreeListColumn, this.profileCheckboxTreeListColumn, this.nameTreeListColumn, this.titleTreeListColumn, this.datatypeTreeListColumn, this.completionTreeListColumn, this.sparklineRowDistributionColumn, this.sparklineTopValuesColumn });
		this.navigationTreeList.Location = new System.Drawing.Point(2, 71);
		this.navigationTreeList.Name = "navigationTreeList";
		this.navigationTreeList.OptionsBehavior.AllowExpandAnimation = DevExpress.Utils.DefaultBoolean.True;
		this.navigationTreeList.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.navigationTreeList.OptionsCustomization.AllowColumnMoving = false;
		this.navigationTreeList.OptionsCustomization.AllowQuickHideColumns = false;
		this.navigationTreeList.OptionsMenu.ShowExpandCollapseItems = false;
		this.navigationTreeList.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.navigationTreeList.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFocus;
		this.navigationTreeList.OptionsView.ShowBandsMode = DevExpress.Utils.DefaultBoolean.False;
		this.navigationTreeList.OptionsView.ShowHorzLines = false;
		this.navigationTreeList.OptionsView.ShowIndicator = false;
		this.navigationTreeList.OptionsView.ShowVertLines = false;
		this.navigationTreeList.OptionsView.TreeLineStyle = DevExpress.XtraTreeList.LineStyle.None;
		this.navigationTreeList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.completionEndedTreeListProgressBarControl, this.repositoryItemPictureEdit1, this.completionNotEndedProgressBarControl, this.profileCheckboxRepositoryItemCheckEdit, this.completionWithErrorsProgressBarControl, this.noDataRepositoryItemTextEdit, this.completionErrorItemTextEdit });
		this.navigationTreeList.RowHeight = 20;
		this.navigationTreeList.Size = new System.Drawing.Size(296, 644);
		this.navigationTreeList.TabIndex = 4;
		this.navigationTreeList.TreeLevelWidth = 27;
		this.navigationTreeList.RowClick += new DevExpress.XtraTreeList.RowClickEventHandler(NavigationTreeList_RowClick);
		this.navigationTreeList.CustomNodeCellEdit += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(NavigationTreeList_CustomNodeCellEdit);
		this.navigationTreeList.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(navigationTreeList_NodeCellStyle);
		this.navigationTreeList.AfterExpand += new DevExpress.XtraTreeList.NodeEventHandler(NavigationTreeList_AfterExpand);
		this.navigationTreeList.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(NavigationTreeList_FocusedNodeChanged);
		this.navigationTreeList.CustomColumnDisplayText += new DevExpress.XtraTreeList.CustomColumnDisplayTextEventHandler(navigationTreeList_CustomColumnDisplayText);
		this.navigationTreeList.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(NavigationTreeList_CustomDrawNodeCell);
		this.navigationTreeList.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(NavigationTreeList_PopupMenuShowing);
		this.iconTreeListColumn.Caption = " ";
		this.iconTreeListColumn.ColumnEdit = this.repositoryItemPictureEdit1;
		this.iconTreeListColumn.FieldName = "IconTreeList";
		this.iconTreeListColumn.MaxWidth = 21;
		this.iconTreeListColumn.MinWidth = 21;
		this.iconTreeListColumn.Name = "iconTreeListColumn";
		this.iconTreeListColumn.OptionsColumn.AllowEdit = false;
		this.iconTreeListColumn.OptionsColumn.AllowSort = false;
		this.iconTreeListColumn.OptionsColumn.ReadOnly = true;
		this.iconTreeListColumn.OptionsFilter.AllowFilter = false;
		this.iconTreeListColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.iconTreeListColumn.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
		this.iconTreeListColumn.Visible = true;
		this.iconTreeListColumn.VisibleIndex = 1;
		this.iconTreeListColumn.Width = 21;
		this.repositoryItemPictureEdit1.Name = "repositoryItemPictureEdit1";
		this.profileCheckboxTreeListColumn.Caption = " ";
		this.profileCheckboxTreeListColumn.ColumnEdit = this.profileCheckboxRepositoryItemCheckEdit;
		this.profileCheckboxTreeListColumn.FieldName = "ProfileCheckbox";
		this.profileCheckboxTreeListColumn.MaxWidth = 30;
		this.profileCheckboxTreeListColumn.MinWidth = 30;
		this.profileCheckboxTreeListColumn.Name = "profileCheckboxTreeListColumn";
		this.profileCheckboxTreeListColumn.OptionsColumn.AllowSize = false;
		this.profileCheckboxTreeListColumn.OptionsColumn.AllowSort = false;
		this.profileCheckboxTreeListColumn.OptionsColumn.FixedWidth = true;
		this.profileCheckboxTreeListColumn.OptionsFilter.AllowFilter = false;
		this.profileCheckboxTreeListColumn.Visible = true;
		this.profileCheckboxTreeListColumn.VisibleIndex = 0;
		this.profileCheckboxTreeListColumn.Width = 30;
		this.profileCheckboxRepositoryItemCheckEdit.AutoHeight = false;
		this.profileCheckboxRepositoryItemCheckEdit.Name = "profileCheckboxRepositoryItemCheckEdit";
		this.nameTreeListColumn.Caption = "Name";
		this.nameTreeListColumn.FieldName = "DisplayName";
		this.nameTreeListColumn.MaxWidth = 2100;
		this.nameTreeListColumn.MinWidth = 110;
		this.nameTreeListColumn.Name = "nameTreeListColumn";
		this.nameTreeListColumn.OptionsColumn.AllowEdit = false;
		this.nameTreeListColumn.OptionsColumn.AllowSort = false;
		this.nameTreeListColumn.OptionsColumn.ReadOnly = true;
		this.nameTreeListColumn.OptionsFilter.AllowFilter = false;
		this.nameTreeListColumn.Visible = true;
		this.nameTreeListColumn.VisibleIndex = 2;
		this.nameTreeListColumn.Width = 110;
		this.titleTreeListColumn.Caption = "Title";
		this.titleTreeListColumn.FieldName = "Title";
		this.titleTreeListColumn.Name = "titleTreeListColumn";
		this.titleTreeListColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraTreeList.Columns.AutoFilterCondition.Contains;
		this.titleTreeListColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraTreeList.FilterPopupMode.CheckedList;
		this.titleTreeListColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.titleTreeListColumn.Width = 70;
		this.datatypeTreeListColumn.Caption = "Datatype";
		this.datatypeTreeListColumn.FieldName = "DataType";
		this.datatypeTreeListColumn.MaxWidth = 100;
		this.datatypeTreeListColumn.MinWidth = 100;
		this.datatypeTreeListColumn.Name = "datatypeTreeListColumn";
		this.datatypeTreeListColumn.OptionsColumn.AllowEdit = false;
		this.datatypeTreeListColumn.OptionsColumn.AllowSort = false;
		this.datatypeTreeListColumn.OptionsColumn.ReadOnly = true;
		this.datatypeTreeListColumn.OptionsFilter.AllowFilter = false;
		this.datatypeTreeListColumn.Visible = true;
		this.datatypeTreeListColumn.VisibleIndex = 3;
		this.datatypeTreeListColumn.Width = 100;
		this.completionTreeListColumn.Caption = "Profiling Progress";
		this.completionTreeListColumn.ColumnEdit = this.completionRepositoryItemProgressBar;
		this.completionTreeListColumn.FieldName = "Completion";
		this.completionTreeListColumn.MaxWidth = 100;
		this.completionTreeListColumn.MinWidth = 100;
		this.completionTreeListColumn.Name = "completionTreeListColumn";
		this.completionTreeListColumn.OptionsColumn.AllowEdit = false;
		this.completionTreeListColumn.OptionsColumn.AllowSort = false;
		this.completionTreeListColumn.OptionsColumn.ReadOnly = true;
		this.completionTreeListColumn.OptionsFilter.AllowFilter = false;
		this.completionTreeListColumn.Visible = true;
		this.completionTreeListColumn.VisibleIndex = 4;
		this.completionTreeListColumn.Width = 100;
		this.sparklineRowDistributionColumn.AppearanceCell.BackColor = System.Drawing.Color.Transparent;
		this.sparklineRowDistributionColumn.AppearanceCell.ForeColor = System.Drawing.Color.White;
		this.sparklineRowDistributionColumn.AppearanceCell.Options.UseBackColor = true;
		this.sparklineRowDistributionColumn.AppearanceCell.Options.UseForeColor = true;
		this.sparklineRowDistributionColumn.AppearanceCell.Options.UseTextOptions = true;
		this.sparklineRowDistributionColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.sparklineRowDistributionColumn.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.sparklineRowDistributionColumn.Caption = "Row Distribution";
		this.sparklineRowDistributionColumn.FieldName = "TextForSparkline";
		this.sparklineRowDistributionColumn.MaxWidth = 110;
		this.sparklineRowDistributionColumn.MinWidth = 110;
		this.sparklineRowDistributionColumn.Name = "sparklineRowDistributionColumn";
		this.sparklineRowDistributionColumn.OptionsColumn.AllowEdit = false;
		this.sparklineRowDistributionColumn.OptionsColumn.AllowSort = false;
		this.sparklineRowDistributionColumn.OptionsColumn.ReadOnly = true;
		this.sparklineRowDistributionColumn.OptionsFilter.AllowFilter = false;
		this.sparklineRowDistributionColumn.Visible = true;
		this.sparklineRowDistributionColumn.VisibleIndex = 5;
		this.sparklineRowDistributionColumn.Width = 110;
		this.sparklineTopValuesColumn.AppearanceCell.BackColor = System.Drawing.Color.Transparent;
		this.sparklineTopValuesColumn.AppearanceCell.BackColor2 = System.Drawing.Color.Transparent;
		this.sparklineTopValuesColumn.AppearanceCell.Options.UseBackColor = true;
		this.sparklineTopValuesColumn.Caption = "Top values";
		this.sparklineTopValuesColumn.FieldName = "Top values";
		this.sparklineTopValuesColumn.MaxWidth = 105;
		this.sparklineTopValuesColumn.MinWidth = 105;
		this.sparklineTopValuesColumn.Name = "sparklineTopValuesColumn";
		this.sparklineTopValuesColumn.OptionsColumn.AllowEdit = false;
		this.sparklineTopValuesColumn.OptionsColumn.AllowFocus = false;
		this.sparklineTopValuesColumn.OptionsColumn.AllowSort = false;
		this.sparklineTopValuesColumn.OptionsColumn.ReadOnly = true;
		this.sparklineTopValuesColumn.OptionsFilter.AllowFilter = false;
		this.sparklineTopValuesColumn.Visible = true;
		this.sparklineTopValuesColumn.VisibleIndex = 6;
		this.sparklineTopValuesColumn.Width = 105;
		this.completionEndedTreeListProgressBarControl.DisplayFormat.FormatString = "{0}%";
		this.completionEndedTreeListProgressBarControl.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.completionEndedTreeListProgressBarControl.EndColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.completionEndedTreeListProgressBarControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
		this.completionEndedTreeListProgressBarControl.LookAndFeel.UseDefaultLookAndFeel = false;
		this.completionEndedTreeListProgressBarControl.Name = "completionEndedTreeListProgressBarControl";
		this.completionEndedTreeListProgressBarControl.PercentView = false;
		this.completionEndedTreeListProgressBarControl.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.completionEndedTreeListProgressBarControl.ShowTitle = true;
		this.completionEndedTreeListProgressBarControl.StartColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.completionNotEndedProgressBarControl.DisplayFormat.FormatString = "{0}%";
		this.completionNotEndedProgressBarControl.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.completionNotEndedProgressBarControl.EndColor = System.Drawing.Color.FromArgb(214, 132, 41);
		this.completionNotEndedProgressBarControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
		this.completionNotEndedProgressBarControl.LookAndFeel.UseDefaultLookAndFeel = false;
		this.completionNotEndedProgressBarControl.Name = "completionNotEndedProgressBarControl";
		this.completionNotEndedProgressBarControl.PercentView = false;
		this.completionNotEndedProgressBarControl.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.completionNotEndedProgressBarControl.ShowTitle = true;
		this.completionNotEndedProgressBarControl.StartColor = System.Drawing.Color.FromArgb(214, 132, 41);
		this.completionWithErrorsProgressBarControl.DisplayFormat.FormatString = "{0}% SUCCESS";
		this.completionWithErrorsProgressBarControl.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.completionWithErrorsProgressBarControl.EndColor = System.Drawing.Color.Gray;
		this.completionWithErrorsProgressBarControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
		this.completionWithErrorsProgressBarControl.LookAndFeel.UseDefaultLookAndFeel = false;
		this.completionWithErrorsProgressBarControl.Name = "completionWithErrorsProgressBarControl";
		this.completionWithErrorsProgressBarControl.PercentView = false;
		this.completionWithErrorsProgressBarControl.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.completionWithErrorsProgressBarControl.ShowTitle = true;
		this.completionWithErrorsProgressBarControl.StartColor = System.Drawing.Color.Gray;
		this.noDataRepositoryItemTextEdit.AutoHeight = false;
		this.noDataRepositoryItemTextEdit.DisplayFormat.FormatString = "NO DATA";
		this.noDataRepositoryItemTextEdit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.noDataRepositoryItemTextEdit.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
		this.noDataRepositoryItemTextEdit.LookAndFeel.UseDefaultLookAndFeel = false;
		this.noDataRepositoryItemTextEdit.Name = "noDataRepositoryItemTextEdit";
		this.noDataRepositoryItemTextEdit.ReadOnly = true;
		this.completionErrorItemTextEdit.AutoHeight = false;
		this.completionErrorItemTextEdit.DisplayFormat.FormatString = "FAILED";
		this.completionErrorItemTextEdit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.completionErrorItemTextEdit.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
		this.completionErrorItemTextEdit.LookAndFeel.UseDefaultLookAndFeel = false;
		this.completionErrorItemTextEdit.Name = "completionErrorItemTextEdit";
		this.completionErrorItemTextEdit.ReadOnly = true;
		this.allEmptyHyperLinkEdit1.EditValue = "Select not profiled";
		this.allEmptyHyperLinkEdit1.Location = new System.Drawing.Point(60, 46);
		this.allEmptyHyperLinkEdit1.Name = "allEmptyHyperLinkEdit1";
		this.allEmptyHyperLinkEdit1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.allEmptyHyperLinkEdit1.Properties.Appearance.Options.UseBackColor = true;
		this.allEmptyHyperLinkEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.allEmptyHyperLinkEdit1.Size = new System.Drawing.Size(98, 18);
		this.allEmptyHyperLinkEdit1.StyleController = this.leftSectionNavigationLayoutControl;
		this.allEmptyHyperLinkEdit1.TabIndex = 26;
		this.allEmptyHyperLinkEdit1.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(AllEmptyHyperLinkEdit_OpenLink);
		this.columnsNavigationLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.columnsNavigationLayoutControlGroup.GroupBordersVisible = false;
		this.columnsNavigationLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[9] { this.treeNavigationListLayoutControlItem, this.allCheckHyperLinkEditLayoutControlItem, this.allEmptyHyperLinkEditLayoutControlItem1, this.emptySpaceItem7, this.noneCheckLayoutControlItem, this.dbTitleLayoutControlItem, this.emptySpaceItem6, this.navTopEmptySpaceItem, this.helpIconLayoutControlItem });
		this.columnsNavigationLayoutControlGroup.Name = "Root";
		this.columnsNavigationLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.columnsNavigationLayoutControlGroup.Size = new System.Drawing.Size(300, 719);
		this.columnsNavigationLayoutControlGroup.TextVisible = false;
		this.treeNavigationListLayoutControlItem.Control = this.navigationTreeList;
		this.treeNavigationListLayoutControlItem.Location = new System.Drawing.Point(0, 69);
		this.treeNavigationListLayoutControlItem.Name = "treeNavigationListLayoutControlItem";
		this.treeNavigationListLayoutControlItem.Size = new System.Drawing.Size(300, 648);
		this.treeNavigationListLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.treeNavigationListLayoutControlItem.TextVisible = false;
		this.allCheckHyperLinkEditLayoutControlItem.Control = this.selectAllHyperLinkEdit;
		this.allCheckHyperLinkEditLayoutControlItem.Location = new System.Drawing.Point(0, 44);
		this.allCheckHyperLinkEditLayoutControlItem.MaxSize = new System.Drawing.Size(58, 25);
		this.allCheckHyperLinkEditLayoutControlItem.MinSize = new System.Drawing.Size(58, 25);
		this.allCheckHyperLinkEditLayoutControlItem.Name = "allCheckHyperLinkEditLayoutControlItem";
		this.allCheckHyperLinkEditLayoutControlItem.Size = new System.Drawing.Size(58, 25);
		this.allCheckHyperLinkEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.allCheckHyperLinkEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.allCheckHyperLinkEditLayoutControlItem.TextVisible = false;
		this.allEmptyHyperLinkEditLayoutControlItem1.Control = this.allEmptyHyperLinkEdit1;
		this.allEmptyHyperLinkEditLayoutControlItem1.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.allEmptyHyperLinkEditLayoutControlItem1.CustomizationFormText = "allEmptyHyperLinkEditLayoutControlItem";
		this.allEmptyHyperLinkEditLayoutControlItem1.Location = new System.Drawing.Point(58, 44);
		this.allEmptyHyperLinkEditLayoutControlItem1.MaxSize = new System.Drawing.Size(102, 25);
		this.allEmptyHyperLinkEditLayoutControlItem1.MinSize = new System.Drawing.Size(102, 25);
		this.allEmptyHyperLinkEditLayoutControlItem1.Name = "allEmptyHyperLinkEditLayoutControlItem1";
		this.allEmptyHyperLinkEditLayoutControlItem1.Size = new System.Drawing.Size(102, 25);
		this.allEmptyHyperLinkEditLayoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.allEmptyHyperLinkEditLayoutControlItem1.Text = "allEmptyHyperLinkEditLayoutControlItem";
		this.allEmptyHyperLinkEditLayoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.allEmptyHyperLinkEditLayoutControlItem1.TextVisible = false;
		this.emptySpaceItem7.AllowHotTrack = false;
		this.emptySpaceItem7.Location = new System.Drawing.Point(0, 717);
		this.emptySpaceItem7.MaxSize = new System.Drawing.Size(2, 2);
		this.emptySpaceItem7.MinSize = new System.Drawing.Size(2, 2);
		this.emptySpaceItem7.Name = "emptySpaceItem7";
		this.emptySpaceItem7.Size = new System.Drawing.Size(300, 2);
		this.emptySpaceItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem7.TextSize = new System.Drawing.Size(0, 0);
		this.noneCheckLayoutControlItem.Control = this.selectNoneHyperLinkEdit;
		this.noneCheckLayoutControlItem.Location = new System.Drawing.Point(160, 44);
		this.noneCheckLayoutControlItem.MaxSize = new System.Drawing.Size(70, 25);
		this.noneCheckLayoutControlItem.MinSize = new System.Drawing.Size(70, 25);
		this.noneCheckLayoutControlItem.Name = "noneCheckLayoutControlItem";
		this.noneCheckLayoutControlItem.Size = new System.Drawing.Size(70, 25);
		this.noneCheckLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.noneCheckLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.noneCheckLayoutControlItem.TextVisible = false;
		this.dbTitleLayoutControlItem.Control = this.dbTitleLabelControl;
		this.dbTitleLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.dbTitleLayoutControlItem.MaxSize = new System.Drawing.Size(266, 44);
		this.dbTitleLayoutControlItem.MinSize = new System.Drawing.Size(266, 44);
		this.dbTitleLayoutControlItem.Name = "dbTitleLayoutControlItem";
		this.dbTitleLayoutControlItem.Size = new System.Drawing.Size(266, 44);
		this.dbTitleLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.dbTitleLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.dbTitleLayoutControlItem.TextVisible = false;
		this.emptySpaceItem6.AllowHotTrack = false;
		this.emptySpaceItem6.Location = new System.Drawing.Point(230, 44);
		this.emptySpaceItem6.Name = "emptySpaceItem6";
		this.emptySpaceItem6.Size = new System.Drawing.Size(70, 25);
		this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
		this.navTopEmptySpaceItem.AllowHotTrack = false;
		this.navTopEmptySpaceItem.Location = new System.Drawing.Point(290, 0);
		this.navTopEmptySpaceItem.Name = "navTopEmptySpaceItem";
		this.navTopEmptySpaceItem.Size = new System.Drawing.Size(10, 44);
		this.navTopEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.helpIconLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.helpIconLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.helpIconLayoutControlItem.Control = this.helpIconUserControl;
		this.helpIconLayoutControlItem.Location = new System.Drawing.Point(266, 0);
		this.helpIconLayoutControlItem.MaxSize = new System.Drawing.Size(24, 44);
		this.helpIconLayoutControlItem.MinSize = new System.Drawing.Size(24, 44);
		this.helpIconLayoutControlItem.Name = "helpIconLayoutControlItem";
		this.helpIconLayoutControlItem.Size = new System.Drawing.Size(24, 44);
		this.helpIconLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.helpIconLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.helpIconLayoutControlItem.TextVisible = false;
		this.clearAllDataButtonItem.Caption = "Clear all Profiling Data";
		this.clearAllDataButtonItem.Id = 26;
		this.clearAllDataButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.all_data_deleted_16;
		this.clearAllDataButtonItem.Name = "clearAllDataButtonItem";
		this.clearAllDataButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ClearAllDataButtonItem_ItemClick);
		this.previewSampleDataBarButtonItem.Caption = "Preview Sample Rows";
		this.previewSampleDataBarButtonItem.Id = 27;
		this.previewSampleDataBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.sample_data_16;
		this.previewSampleDataBarButtonItem.Name = "previewSampleDataBarButtonItem";
		this.previewSampleDataBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(PreviewSampleDataPPMBarButtonItem_ItemClick);
		this.profileColumnBarButtonItem.Caption = "Profile Column";
		this.profileColumnBarButtonItem.Id = 15;
		this.profileColumnBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.profile_column_16;
		this.profileColumnBarButtonItem.Name = "profileColumnBarButtonItem";
		this.profileColumnBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ProfileColumnPPMBarButtonItem_ItemClick);
		this.profileTableBarButtonItem.Caption = "Profile Table (all columns)";
		this.profileTableBarButtonItem.Id = 25;
		this.profileTableBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.profile_table_16;
		this.profileTableBarButtonItem.Name = "profileTableBarButtonItem";
		this.profileTableBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ProfileTablePPMBarButtonItem_ItemClick);
		this.navigationValuesPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.previewSampleDataBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.profileColumnBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.profileTableBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.clearAllDataButtonItem)
		});
		this.navigationValuesPopupMenu.Manager = this.barManager1;
		this.navigationValuesPopupMenu.Name = "navigationValuesPopupMenu";
		this.barManager1.DockControls.Add(this.barDockControlTop);
		this.barManager1.DockControls.Add(this.barDockControlBottom);
		this.barManager1.DockControls.Add(this.barDockControlLeft);
		this.barManager1.DockControls.Add(this.barDockControlRight);
		this.barManager1.Form = this;
		this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[4] { this.previewSampleDataBarButtonItem, this.profileColumnBarButtonItem, this.profileTableBarButtonItem, this.clearAllDataButtonItem });
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager1;
		this.barDockControlTop.Size = new System.Drawing.Size(300, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 719);
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlBottom.Size = new System.Drawing.Size(300, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 719);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(300, 0);
		this.barDockControlRight.Manager = this.barManager1;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 719);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.leftSectionNavigationLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		this.MinimumSize = new System.Drawing.Size(300, 0);
		base.Name = "NavigationUserControl";
		base.Size = new System.Drawing.Size(300, 719);
		((System.ComponentModel.ISupportInitialize)this.completionRepositoryItemProgressBar).EndInit();
		((System.ComponentModel.ISupportInitialize)this.leftSectionNavigationLayoutControl).EndInit();
		this.leftSectionNavigationLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.selectNoneHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.selectAllHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.navigationTreeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemPictureEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.profileCheckboxRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.completionEndedTreeListProgressBarControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.completionNotEndedProgressBarControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.completionWithErrorsProgressBarControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.noDataRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.completionErrorItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allEmptyHyperLinkEdit1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnsNavigationLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.treeNavigationListLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allCheckHyperLinkEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allEmptyHyperLinkEditLayoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.noneCheckLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dbTitleLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.navTopEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.helpIconLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.navigationValuesPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
