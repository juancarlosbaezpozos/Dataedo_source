using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.ObjectBrowser;
using Dataedo.App.UserControls.OverriddenControls;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.Model.Data.DataLineage;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.UserControls.DataLineage;

public class DataLineageFlowsUserControl : BaseUserControl
{
	public List<DataFlowRow> DeletedFlows;

	private DBTreeNode currentObjectNode;

	private MetadataEditorUserControl metadataEditorUserControl;

	private ObjectBrowserUserControl dataLineageObjectBrowserUserControl;

	private AllDataFlowsContainer allDataFlowsContainer;

	private string tooltipText = string.Empty;

	public EventHandler FocusedDataFlowChanged;

	private bool focusedRowChanging;

	private DataFlowRow previouslyFocusedFlow;

	private IContainer components;

	private LabelControl inflowsLabelControl;

	private LabelControl outflowsLabelControl;

	private PopupMenu popupMenu;

	private BarManager barManager1;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private BarButtonItem removeFlowBarButtonItem;

	private ToolTipController inGridToolTipController;

	private ToolTipController outGridToolTipController;

	private NonCustomizableLayoutControl inflowsLayoutControl;

	private TablePanel tablePanel1;

	private NonCustomizableLayoutControl outflowsLayoutControl;

	private LayoutControlGroup layoutControlGroup4;

	private LayoutControlItem layoutControlItem7;

	private EmptySpaceItem emptySpaceItem4;

	private LayoutControlGroup layoutControlGroup5;

	private LayoutControlItem layoutControlItem2;

	private EmptySpaceItem emptySpaceItem3;

	private NonCustomizableLayoutControl outflowsGridNonCustomizableLayoutControl;

	private GridControl outFlowsGridControl;

	private TileViewWithInfoText outFlowsTileView;

	private TileViewColumn outIconTileViewColumn;

	private TileViewColumn objectCaptionTileViewColumn;

	private TileViewColumn outProcessName;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem layoutControlItem3;

	private LayoutControlItem layoutControlItem6;

	private NonCustomizableLayoutControl inflowsGridNonCustomizableLayoutControl;

	private GridControl inFlowsGridControl;

	private TileViewWithInfoText inFlowsTileView;

	private TileViewColumn inIconTileViewColumn;

	private TileViewColumn inObjectCaptionTtileViewColumn;

	private TileViewColumn inProcessName;

	private LayoutControlGroup Root;

	private LayoutControlItem layoutControlItem1;

	private LayoutControlItem layoutControlItem4;

	private BarButtonItem goToBarButtonItem;

	private DataProcessRow DataProcessRow { get; set; }

	private BindingList<DataFlowRow> InFlowRows
	{
		get
		{
			if (allDataFlowsContainer == null)
			{
				return DataProcessRow?.InflowRows;
			}
			return allDataFlowsContainer.InflowRows;
		}
	}

	private BindingList<DataFlowRow> OutFlowRows
	{
		get
		{
			if (allDataFlowsContainer == null)
			{
				return DataProcessRow?.OutflowRows;
			}
			return allDataFlowsContainer.OutflowRows;
		}
	}

	[Browsable(true)]
	public event DataLineageUserControl.DataLineageEditedHandler DataLineageEdited;

	public DataLineageFlowsUserControl()
	{
		InitializeComponent();
		Init();
	}

	private void Init()
	{
		DeletedFlows = new List<DataFlowRow>();
		BackColor = SkinColors.ControlColorFromSystemColors;
	}

	private void SetCaptions()
	{
		string text = "Inflows";
		string text2 = "Outflows";
		string infoText = "Drop objects here to add inflow";
		string infoText2 = "Drop objects here to add outflow";
		if (currentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.View)
		{
			infoText2 = "View can only serve itself";
		}
		else if (currentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table || currentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.Structure)
		{
			text = "Saving to this object";
			text2 = "Reading from this object";
			infoText = "Drop here objects which" + Environment.NewLine + "saves data to this object";
			infoText2 = "Drop here objects which" + Environment.NewLine + "reads data from this object";
		}
		inFlowsTileView.SetInfoText(infoText);
		outFlowsTileView.SetInfoText(infoText2);
		inflowsLabelControl.Text = text;
		outflowsLabelControl.Text = text2;
		previouslyFocusedFlow = null;
	}

	public void SetParameters(MetadataEditorUserControl metadataEditorUserControl, ObjectBrowserUserControl dataLineageObjectBrowserUserControl, DataProcessRow dataProcessRow, DBTreeNode currentObjectNode, AllDataFlowsContainer allDataFlowsContainer = null)
	{
		try
		{
			if (this.metadataEditorUserControl != null)
			{
				this.metadataEditorUserControl.MetadataTreeList.DragEnter -= MetadataTreeList_DragEnter;
				this.metadataEditorUserControl.MetadataTreeList.QueryContinueDrag -= MetadataTreeList_QueryContinueDrag;
			}
			if (this.dataLineageObjectBrowserUserControl != null)
			{
				this.dataLineageObjectBrowserUserControl.GridControl.DragEnter -= MetadataTreeList_DragEnter;
				this.dataLineageObjectBrowserUserControl.GridControl.QueryContinueDrag -= MetadataTreeList_QueryContinueDrag;
			}
			this.metadataEditorUserControl = metadataEditorUserControl;
			this.metadataEditorUserControl.MetadataTreeList.DragEnter += MetadataTreeList_DragEnter;
			this.metadataEditorUserControl.MetadataTreeList.QueryContinueDrag += MetadataTreeList_QueryContinueDrag;
			this.dataLineageObjectBrowserUserControl = dataLineageObjectBrowserUserControl;
			this.dataLineageObjectBrowserUserControl.GridControl.DragEnter += MetadataTreeList_DragEnter;
			this.dataLineageObjectBrowserUserControl.GridControl.QueryContinueDrag += MetadataTreeList_QueryContinueDrag;
			this.metadataEditorUserControl?.SetWaitformVisibility(visible: true);
			if (dataProcessRow == null && allDataFlowsContainer == null)
			{
				inflowsLayoutControl.Visible = false;
				outflowsLayoutControl.Visible = false;
				this.metadataEditorUserControl?.SetWaitformVisibility(visible: false);
				return;
			}
			inflowsLayoutControl.Visible = true;
			outflowsLayoutControl.Visible = true;
			this.currentObjectNode = currentObjectNode;
			DataProcessRow = dataProcessRow;
			this.allDataFlowsContainer = allDataFlowsContainer;
			SetCaptions();
			RefreshDataSource();
			inFlowsTileView.FocusedRowHandle = int.MinValue;
			outFlowsTileView.FocusedRowHandle = int.MinValue;
			this.metadataEditorUserControl?.SetWaitformVisibility(visible: false);
		}
		catch (Exception exception)
		{
			this.metadataEditorUserControl?.SetWaitformVisibility(visible: false);
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	private void RefreshDataSource()
	{
		if (currentObjectNode != null)
		{
			if (allDataFlowsContainer != null)
			{
				LoadAllFlowsForProcessor();
			}
			else if (DataProcessRow != null)
			{
				LoadProcessFlows();
			}
		}
	}

	private void LoadAllFlowsForProcessor()
	{
		try
		{
			if (currentObjectNode != null)
			{
				allDataFlowsContainer.RefreshInflowsAndOutflows(currentObjectNode, base.ParentForm);
				inFlowsGridControl.DataSource = allDataFlowsContainer.InflowRows;
				outFlowsGridControl.DataSource = allDataFlowsContainer.OutflowRows;
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	private void LoadProcessFlows()
	{
		try
		{
			if (DataProcessRow != null && currentObjectNode != null)
			{
				if (DataProcessRow.InflowRows == null)
				{
					DataProcessRow.InflowRows = new BindingList<DataFlowRow>();
				}
				inFlowsGridControl.DataSource = DataProcessRow.InflowRows;
				if (DataProcessRow.OutflowRows == null)
				{
					DataProcessRow.OutflowRows = new BindingList<DataFlowRow>();
				}
				outFlowsGridControl.DataSource = DataProcessRow.OutflowRows;
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	private void GridControl_MouseClick(object sender, MouseEventArgs e)
	{
		popupMenu.ItemLinks.Clear();
		removeFlowBarButtonItem.Tag = null;
		if (!(sender is GridControl gridControl) || !(gridControl.FocusedView is TileView tileView))
		{
			return;
		}
		if (e.Button == MouseButtons.Right)
		{
			TileViewHitInfo tileViewHitInfo = tileView.CalcHitInfo(e.Location);
			if (tileView.GetRow(tileViewHitInfo.RowHandle) is DataFlowRow dataFlowRow)
			{
				removeFlowBarButtonItem.Tag = tileView;
				popupMenu.ItemLinks.Add(removeFlowBarButtonItem);
				goToBarButtonItem.ImageOptions.Image = dataFlowRow.Icon;
				goToBarButtonItem.Caption = "Go to " + dataFlowRow.ObjectCaption;
				goToBarButtonItem.Tag = dataFlowRow;
				popupMenu.ItemLinks.Add(goToBarButtonItem);
			}
			popupMenu.ShowPopup(barManager1, tileView.GridControl.PointToScreen(e.Location));
		}
		else
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			TileViewHitInfo tileViewHitInfo2 = tileView.CalcHitInfo(e.Location);
			if (tileView.GetRow(tileViewHitInfo2.RowHandle) is DataFlowRow dataFlowRow2)
			{
				if (dataFlowRow2 == previouslyFocusedFlow)
				{
					tileView.FocusedRowHandle = int.MinValue;
					previouslyFocusedFlow = null;
				}
				else
				{
					previouslyFocusedFlow = dataFlowRow2;
				}
			}
			else
			{
				tileView.FocusedRowHandle = int.MinValue;
				previouslyFocusedFlow = null;
			}
		}
	}

	private void RemoveFlowBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (e.Item.Tag is TileView tileView)
		{
			ProcessDataFlowDeletion(tileView);
		}
	}

	private void ProcessDataFlowDeletion(TileView tileView)
	{
		try
		{
			if (tileView == null)
			{
				return;
			}
			int[] selectedRows = tileView.GetSelectedRows();
			IEnumerable<DataFlowRow> enumerable = selectedRows.Select((int r) => tileView.GetRow(r) as DataFlowRow);
			if (!enumerable.Any())
			{
				return;
			}
			bool isTileViewInflows = tileView == inFlowsTileView;
			if (!AskForDeleteConfirmationIfNeeded(enumerable, isTileViewInflows))
			{
				return;
			}
			int[] array = selectedRows;
			foreach (int rowHandle in array)
			{
				DataFlowRow dataFlowRow = tileView.GetRow(rowHandle) as DataFlowRow;
				dataFlowRow.RowState = ManagingRowsEnum.ManagingRows.Deleted;
				DeletedFlows.Add(dataFlowRow);
				tileView.GridControl.BeginUpdate();
				if (tileView == inFlowsTileView)
				{
					InFlowRows.Remove(dataFlowRow);
					dataFlowRow.Process?.InflowRows?.Remove(dataFlowRow);
					this.DataLineageEdited?.Invoke();
				}
				else if (tileView == outFlowsTileView)
				{
					OutFlowRows.Remove(dataFlowRow);
					dataFlowRow.Process?.OutflowRows?.Remove(dataFlowRow);
					this.DataLineageEdited?.Invoke();
				}
				RemoveRowFocus(tileView.GridControl);
				tileView.GridControl.EndUpdate();
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	private bool AskForDeleteConfirmationIfNeeded(IEnumerable<DataFlowRow> selectedDataFlowRows, bool isTileViewInflows)
	{
		IEnumerable<int> source = from x in selectedDataFlowRows
			where !x.IsProcessInSameProcessor
			select x.Id;
		bool flag = DB.DataFlows.CheckIfFlowsAreAssignedInColumns(source.ToList());
		if (selectedDataFlowRows.Where((DataFlowRow x) => x.IsProcessInSameProcessor).Any((DataFlowRow x) => x.Process.Columns.Any((DataLineageColumnsFlowRow c) => c.IsRowComplete && (c.InflowRow.Guid == x.Guid || c.OutflowRow.Guid == x.Guid))) || flag)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (selectedDataFlowRows.Count() == 1)
			{
				stringBuilder.Append("You are about to delete");
				stringBuilder.Append(isTileViewInflows ? " inflow" : " outflow");
				stringBuilder.AppendLine(" <b>" + selectedDataFlowRows.FirstOrDefault().ObjectCaptionInline + "</b>");
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("There are column-level flows assigned, which also get deleted.");
			}
			else
			{
				stringBuilder.Append("You are about to delete");
				stringBuilder.AppendLine(isTileViewInflows ? " inflows:" : " outflows:");
				stringBuilder.AppendLine();
				foreach (DataFlowRow selectedDataFlowRow in selectedDataFlowRows)
				{
					stringBuilder.AppendLine("- <b>" + selectedDataFlowRow.ObjectCaptionInline + "</b>");
				}
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("There are column-level flows assigned, which also get deleted.");
			}
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(stringBuilder.ToString(), "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, null, 2, base.ParentForm);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.OK)
			{
				return false;
			}
		}
		return true;
	}

	private void FlowsGridControl_DragEnter(object sender, DragEventArgs e)
	{
		FlowDirectionEnum.Direction flowDirection = ((sender as GridControl == inFlowsGridControl) ? FlowDirectionEnum.Direction.IN : FlowDirectionEnum.Direction.OUT);
		DataFlowHelper.SetDragDropEffect(e, metadataEditorUserControl, currentObjectNode, flowDirection, allDataFlowsContainer != null, ref tooltipText);
	}

	private void InFlowsGridControl_DragOver(object sender, DragEventArgs e)
	{
		if (!string.IsNullOrEmpty(tooltipText))
		{
			inFlowsGridControl.ToolTipController.ShowHint(tooltipText, Control.MousePosition);
		}
	}

	private void FlowsGridControl_DragLeave(object sender, EventArgs e)
	{
		tooltipText = string.Empty;
		inFlowsGridControl.ToolTipController.HideHint();
	}

	private void FlowsGridControl_DragDrop(object sender, DragEventArgs e)
	{
		inGridToolTipController.HideHint();
		GridControl gridControl = sender as GridControl;
		if (DataFlowHelper.DragDrop(e, currentObjectNode, metadataEditorUserControl, InFlowRows, OutFlowRows, DataProcessRow, allDataFlowsContainer, gridControl == inFlowsGridControl, FindForm()))
		{
			if (allDataFlowsContainer != null)
			{
				RefreshDataSource();
			}
			RemoveRowFocus(gridControl);
			this.DataLineageEdited?.Invoke();
		}
	}

	private void MetadataTreeList_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
	{
		if (e.Action != 0)
		{
			SetDefaultGridLayoutBackColor();
		}
	}

	private void MetadataTreeList_DragEnter(object sender, DragEventArgs e)
	{
		SetDefaultGridLayoutBackColor();
		IFlowDraggable flowDraggable = e.Data.GetData(typeof(ObjectBrowserItem)) as IFlowDraggable;
		if (flowDraggable == null && e.Data.GetData(typeof(TreeListNode)) is TreeListNode treeListNode)
		{
			flowDraggable = metadataEditorUserControl?.TreeListHelpers.GetNode(treeListNode);
		}
		if (flowDraggable != null && flowDraggable.IsNormalObject && !flowDraggable.Deleted)
		{
			if (DataFlowHelper.CanBeDropped(flowDraggable.ObjectType, currentObjectNode.ObjectType, FlowDirectionEnum.Direction.IN, allDataFlowsContainer != null, ref tooltipText))
			{
				inflowsGridNonCustomizableLayoutControl.BackColor = SkinsManager.CurrentSkin.DataedoColor;
			}
			bool flag = DataFlowHelper.CanBeDropped(flowDraggable.ObjectType, currentObjectNode.ObjectType, FlowDirectionEnum.Direction.OUT, allDataFlowsContainer != null, ref tooltipText);
			if (currentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.View && currentObjectNode.Id != flowDraggable.Id)
			{
				flag = false;
			}
			if (flag)
			{
				outflowsGridNonCustomizableLayoutControl.BackColor = SkinsManager.CurrentSkin.DataedoColor;
			}
		}
	}

	private void SetDefaultGridLayoutBackColor()
	{
		Color color2 = (inflowsGridNonCustomizableLayoutControl.BackColor = (outflowsGridNonCustomizableLayoutControl.BackColor = SkinColors.ControlColorFromSystemColors));
	}

	private void inFlowsTileView_CustomItemTemplate(object sender, TileViewCustomItemTemplateEventArgs e)
	{
		SetTileItemTemplate(inFlowsTileView, e);
	}

	private void outFlowsTileView_CustomItemTemplate(object sender, TileViewCustomItemTemplateEventArgs e)
	{
		SetTileItemTemplate(outFlowsTileView, e);
	}

	private void SetTileItemTemplate(TileViewWithInfoText tileViewWithInfoText, TileViewCustomItemTemplateEventArgs e)
	{
		if (allDataFlowsContainer == null)
		{
			e.Template = e.Templates["Default"];
		}
		else
		{
			e.Template = e.Templates["ExternalProcessTemplate"];
		}
	}

	private void InGridToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		GenetrateProcessNameTooltip(inFlowsTileView, e);
	}

	private void OutGridToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		GenetrateProcessNameTooltip(outFlowsTileView, e);
	}

	private void GenetrateProcessNameTooltip(TileView tileView, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
	}

	private void inFlowsTileView_ItemCustomize(object sender, TileViewItemCustomizeEventArgs e)
	{
		SetTileViewItemBackgroundColor(inFlowsTileView, e);
	}

	private void outFlowsTileView_ItemCustomize(object sender, TileViewItemCustomizeEventArgs e)
	{
		SetTileViewItemBackgroundColor(outFlowsTileView, e);
	}

	private static void SetTileViewItemBackgroundColor(TileView tileView, TileViewItemCustomizeEventArgs e)
	{
		DataFlowRow dataFlowRow = tileView.GetRow(e.RowHandle) as DataFlowRow;
		if (tileView.FocusedRowHandle == e.RowHandle)
		{
			e.Item.AppearanceItem.Normal.BackColor = Color.Empty;
		}
		else if (!dataFlowRow.IsProcessInSameProcessor)
		{
			e.Item.AppearanceItem.Normal.BackColor = SkinsManager.CurrentSkin.GridNonEditableColumnsBackColor;
		}
	}

	private void InFlowsTileView_ItemDoubleClick(object sender, TileViewItemClickEventArgs e)
	{
		SelectObjectAndProcess(inFlowsTileView, e);
	}

	private void OutFlowsTileView_ItemDoubleClick(object sender, TileViewItemClickEventArgs e)
	{
		SelectObjectAndProcess(outFlowsTileView, e);
	}

	private void SelectObjectAndProcess(TileView tileView, TileViewItemClickEventArgs e)
	{
		if (tileView.GetRow(e.Item.RowHandle) is DataFlowRow dataFlowRow)
		{
			GoToDataFlowRow(dataFlowRow);
		}
	}

	private void GoToDataFlowRow(DataFlowRow dataFlowRow)
	{
		if (!dataFlowRow.DatabaseId.HasValue)
		{
			return;
		}
		int objectId = dataFlowRow.ObjectId;
		SharedObjectTypeEnum.ObjectType objectType = dataFlowRow.ObjectType;
		if (!dataFlowRow.IsProcessInSameProcessor)
		{
			DataProcessRow dataProcessRow = dataFlowRow.Process ?? DB.DataProcess.GetProcessById(dataFlowRow.ProcessId);
			if (!dataProcessRow.ParentObjectType.HasValue)
			{
				return;
			}
			objectId = dataProcessRow.ParentId;
			objectType = dataProcessRow.ParentObjectType.Value;
		}
		metadataEditorUserControl.SelectObjectAndShowDataLineageTab(objectType, new ObjectEventArgs(dataFlowRow.DatabaseId.Value, objectId, null), null, selectDiagramTab: false, DataLineageUserControl.ColumnsExpanded);
	}

	private void InFlowsTileView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (!focusedRowChanging)
			{
				focusedRowChanging = true;
				outFlowsTileView.FocusedRowHandle = int.MinValue;
				DataFlowRow value = inFlowsTileView.GetRow(e.FocusedRowHandle) as DataFlowRow;
				FocusedDataFlowChanged?.Invoke(null, new GenericEventArgs<DataFlowRow>(value));
			}
		}
		finally
		{
			focusedRowChanging = false;
		}
	}

	private void OutFlowsTileView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (!focusedRowChanging)
			{
				focusedRowChanging = true;
				inFlowsTileView.FocusedRowHandle = int.MinValue;
				DataFlowRow value = outFlowsTileView.GetRow(e.FocusedRowHandle) as DataFlowRow;
				FocusedDataFlowChanged?.Invoke(null, new GenericEventArgs<DataFlowRow>(value));
			}
		}
		finally
		{
			focusedRowChanging = false;
		}
	}

	public List<DataFlowRow> GetAllInflowRows()
	{
		return InFlowRows?.OrderBy((DataFlowRow x) => x.Id).ToList();
	}

	public List<DataFlowRow> GetAllOutflowRows()
	{
		return OutFlowRows?.OrderBy((DataFlowRow x) => x.Id).ToList();
	}

	private void RemoveRowFocus(GridControl gridControl)
	{
		if (gridControl == inFlowsGridControl)
		{
			inFlowsTileView.FocusedRowHandle = int.MinValue;
		}
		else
		{
			outFlowsTileView.FocusedRowHandle = int.MinValue;
		}
	}

	private void GoToBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (e.Item.Tag is DataFlowRow dataFlowRow)
		{
			GoToDataFlowRow(dataFlowRow);
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			if (keyData == Keys.Delete && (inFlowsTileView.IsFocusedView || outFlowsTileView.IsFocusedView))
			{
				ProcessDataFlowDeletion(inFlowsTileView.IsFocusedView ? inFlowsTileView : outFlowsTileView);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
		return base.ProcessCmdKey(ref msg, keyData);
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
		DevExpress.XtraGrid.Views.Tile.ItemTemplate itemTemplate = new DevExpress.XtraGrid.Views.Tile.ItemTemplate();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition2 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement2 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement3 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition3 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition4 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement4 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement5 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.ItemTemplate itemTemplate2 = new DevExpress.XtraGrid.Views.Tile.ItemTemplate();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition5 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition6 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement6 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement7 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement8 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition3 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition4 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition7 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition8 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement9 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement10 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		this.inIconTileViewColumn = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.inObjectCaptionTtileViewColumn = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.inProcessName = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.outIconTileViewColumn = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.objectCaptionTileViewColumn = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.outProcessName = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.inGridToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.inflowsLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.inflowsLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.inflowsGridNonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.inFlowsGridControl = new DevExpress.XtraGrid.GridControl();
		this.inFlowsTileView = new Dataedo.App.UserControls.OverriddenControls.TileViewWithInfoText();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.outGridToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.outflowsLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.outflowsLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.outflowsGridNonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.outFlowsGridControl = new DevExpress.XtraGrid.GridControl();
		this.outFlowsTileView = new Dataedo.App.UserControls.OverriddenControls.TileViewWithInfoText();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
		this.popupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.removeFlowBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.goToBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.tablePanel1 = new DevExpress.Utils.Layout.TablePanel();
		((System.ComponentModel.ISupportInitialize)this.inflowsLayoutControl).BeginInit();
		this.inflowsLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.inflowsGridNonCustomizableLayoutControl).BeginInit();
		this.inflowsGridNonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.inFlowsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.inFlowsTileView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.outflowsLayoutControl).BeginInit();
		this.outflowsLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.outflowsGridNonCustomizableLayoutControl).BeginInit();
		this.outflowsGridNonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.outFlowsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.outFlowsTileView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.popupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tablePanel1).BeginInit();
		this.tablePanel1.SuspendLayout();
		base.SuspendLayout();
		this.inIconTileViewColumn.Caption = "Icon";
		this.inIconTileViewColumn.FieldName = "Icon";
		this.inIconTileViewColumn.Name = "inIconTileViewColumn";
		this.inIconTileViewColumn.OptionsColumn.FixedWidth = true;
		this.inIconTileViewColumn.Visible = true;
		this.inIconTileViewColumn.VisibleIndex = 0;
		this.inObjectCaptionTtileViewColumn.Caption = "In Object Caption";
		this.inObjectCaptionTtileViewColumn.FieldName = "ObjectCaption";
		this.inObjectCaptionTtileViewColumn.Name = "inObjectCaptionTtileViewColumn";
		this.inObjectCaptionTtileViewColumn.Visible = true;
		this.inObjectCaptionTtileViewColumn.VisibleIndex = 1;
		this.inProcessName.Caption = "Process Name";
		this.inProcessName.FieldName = "ColumnProcessName";
		this.inProcessName.Name = "inProcessName";
		this.inProcessName.Visible = true;
		this.inProcessName.VisibleIndex = 2;
		this.outIconTileViewColumn.Caption = "Icon";
		this.outIconTileViewColumn.FieldName = "Icon";
		this.outIconTileViewColumn.Name = "outIconTileViewColumn";
		this.outIconTileViewColumn.OptionsColumn.FixedWidth = true;
		this.outIconTileViewColumn.Visible = true;
		this.outIconTileViewColumn.VisibleIndex = 0;
		this.objectCaptionTileViewColumn.Caption = "Object Caption";
		this.objectCaptionTileViewColumn.FieldName = "ObjectCaption";
		this.objectCaptionTileViewColumn.Name = "objectCaptionTileViewColumn";
		this.objectCaptionTileViewColumn.Visible = true;
		this.objectCaptionTileViewColumn.VisibleIndex = 1;
		this.outProcessName.Caption = "Out Process Name";
		this.outProcessName.FieldName = "ColumnProcessName";
		this.outProcessName.Name = "outProcessName";
		this.outProcessName.Visible = true;
		this.outProcessName.VisibleIndex = 2;
		this.inGridToolTipController.AutoPopDelay = 10000;
		this.inGridToolTipController.CloseOnClick = DevExpress.Utils.DefaultBoolean.False;
		this.inGridToolTipController.InitialDelay = 1;
		this.inGridToolTipController.KeepWhileHovered = true;
		this.inGridToolTipController.ReshowDelay = 1;
		this.inGridToolTipController.Rounded = true;
		this.inGridToolTipController.RoundRadius = 10;
		this.inGridToolTipController.ShowBeak = true;
		this.inGridToolTipController.ToolTipAnchor = DevExpress.Utils.ToolTipAnchor.Cursor;
		this.inGridToolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(InGridToolTipController_GetActiveObjectInfo);
		this.inflowsLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.inflowsLabelControl.Appearance.Options.UseFont = true;
		this.inflowsLabelControl.Location = new System.Drawing.Point(2, 2);
		this.inflowsLabelControl.Name = "inflowsLabelControl";
		this.inflowsLabelControl.Padding = new System.Windows.Forms.Padding(5, 1, 0, 5);
		this.inflowsLabelControl.Size = new System.Drawing.Size(46, 19);
		this.inflowsLabelControl.StyleController = this.inflowsLayoutControl;
		this.inflowsLabelControl.TabIndex = 4;
		this.inflowsLabelControl.Text = "Inflows";
		this.inflowsLayoutControl.AllowCustomization = false;
		this.inflowsLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.tablePanel1.SetColumn(this.inflowsLayoutControl, 0);
		this.inflowsLayoutControl.Controls.Add(this.inflowsGridNonCustomizableLayoutControl);
		this.inflowsLayoutControl.Controls.Add(this.inflowsLabelControl);
		this.inflowsLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.inflowsLayoutControl.Location = new System.Drawing.Point(0, 3);
		this.inflowsLayoutControl.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
		this.inflowsLayoutControl.Name = "inflowsLayoutControl";
		this.inflowsLayoutControl.Root = this.layoutControlGroup5;
		this.tablePanel1.SetRow(this.inflowsLayoutControl, 0);
		this.inflowsLayoutControl.Size = new System.Drawing.Size(368, 584);
		this.inflowsLayoutControl.TabIndex = 1;
		this.inflowsLayoutControl.Text = "nonCustomizableLayoutControl2";
		this.inflowsGridNonCustomizableLayoutControl.AllowCustomization = false;
		this.inflowsGridNonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.inflowsGridNonCustomizableLayoutControl.Controls.Add(this.inFlowsGridControl);
		this.inflowsGridNonCustomizableLayoutControl.Location = new System.Drawing.Point(2, 25);
		this.inflowsGridNonCustomizableLayoutControl.Name = "inflowsGridNonCustomizableLayoutControl";
		this.inflowsGridNonCustomizableLayoutControl.Root = this.Root;
		this.inflowsGridNonCustomizableLayoutControl.Size = new System.Drawing.Size(364, 557);
		this.inflowsGridNonCustomizableLayoutControl.TabIndex = 5;
		this.inflowsGridNonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.inFlowsGridControl.AllowDrop = true;
		this.inFlowsGridControl.Location = new System.Drawing.Point(2, 2);
		this.inFlowsGridControl.MainView = this.inFlowsTileView;
		this.inFlowsGridControl.Name = "inFlowsGridControl";
		this.inFlowsGridControl.Size = new System.Drawing.Size(360, 553);
		this.inFlowsGridControl.TabIndex = 4;
		this.inFlowsGridControl.ToolTipController = this.inGridToolTipController;
		this.inFlowsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.inFlowsTileView });
		this.inFlowsGridControl.DragDrop += new System.Windows.Forms.DragEventHandler(FlowsGridControl_DragDrop);
		this.inFlowsGridControl.DragEnter += new System.Windows.Forms.DragEventHandler(FlowsGridControl_DragEnter);
		this.inFlowsGridControl.DragOver += new System.Windows.Forms.DragEventHandler(InFlowsGridControl_DragOver);
		this.inFlowsGridControl.DragLeave += new System.EventHandler(FlowsGridControl_DragLeave);
		this.inFlowsGridControl.MouseClick += new System.Windows.Forms.MouseEventHandler(GridControl_MouseClick);
		this.inFlowsTileView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
		this.inFlowsTileView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.inIconTileViewColumn, this.inObjectCaptionTtileViewColumn, this.inProcessName });
		this.inFlowsTileView.GridControl = this.inFlowsGridControl;
		this.inFlowsTileView.Name = "inFlowsTileView";
		this.inFlowsTileView.OptionsTiles.ColumnCount = 1;
		this.inFlowsTileView.OptionsTiles.ItemPadding = new System.Windows.Forms.Padding(2);
		this.inFlowsTileView.OptionsTiles.ItemSize = new System.Drawing.Size(318, 71);
		this.inFlowsTileView.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.inFlowsTileView.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		tableColumnDefinition.Length.Value = 67.0;
		tableColumnDefinition2.Length.Value = 365.0;
		itemTemplate.Columns.Add(tableColumnDefinition);
		itemTemplate.Columns.Add(tableColumnDefinition2);
		tileViewItemElement.Column = this.inIconTileViewColumn;
		tileViewItemElement.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.NoScale;
		tileViewItemElement.ImageOptions.ImageSize = new System.Drawing.Size(22, 22);
		tileViewItemElement.Text = "inIconTileViewColumn";
		tileViewItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.Column = this.inObjectCaptionTtileViewColumn;
		tileViewItemElement2.ColumnIndex = 1;
		tileViewItemElement2.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement2.Text = "inObjectCaptionTtileViewColumn";
		tileViewItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement2.TextLocation = new System.Drawing.Point(5, 0);
		tileViewItemElement3.Appearance.Normal.ForeColor = System.Drawing.Color.FromArgb(102, 102, 102);
		tileViewItemElement3.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement3.Column = this.inProcessName;
		tileViewItemElement3.ColumnIndex = 1;
		tileViewItemElement3.ImageOptions.Image = Dataedo.App.Properties.Resources.process_16;
		tileViewItemElement3.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement3.ImageOptions.ImageLocation = new System.Drawing.Point(3, 0);
		tileViewItemElement3.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
		tileViewItemElement3.RowIndex = 1;
		tileViewItemElement3.Text = "inProcessName";
		tileViewItemElement3.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement3.TextLocation = new System.Drawing.Point(5, 0);
		itemTemplate.Elements.Add(tileViewItemElement);
		itemTemplate.Elements.Add(tileViewItemElement2);
		itemTemplate.Elements.Add(tileViewItemElement3);
		itemTemplate.Name = "ExternalProcessTemplate";
		tableRowDefinition.Length.Value = 44.0;
		tableRowDefinition2.Length.Value = 25.0;
		itemTemplate.Rows.Add(tableRowDefinition);
		itemTemplate.Rows.Add(tableRowDefinition2);
		this.inFlowsTileView.Templates.Add(itemTemplate);
		tableColumnDefinition3.Length.Value = 67.0;
		tableColumnDefinition4.Length.Value = 365.0;
		this.inFlowsTileView.TileColumns.Add(tableColumnDefinition3);
		this.inFlowsTileView.TileColumns.Add(tableColumnDefinition4);
		this.inFlowsTileView.TileRows.Add(obj);
		tileViewItemElement4.Column = this.inIconTileViewColumn;
		tileViewItemElement4.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement4.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.NoScale;
		tileViewItemElement4.ImageOptions.ImageSize = new System.Drawing.Size(22, 22);
		tileViewItemElement4.Text = "inIconTileViewColumn";
		tileViewItemElement4.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement5.Column = this.inObjectCaptionTtileViewColumn;
		tileViewItemElement5.ColumnIndex = 1;
		tileViewItemElement5.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement5.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement5.Text = "inObjectCaptionTtileViewColumn";
		tileViewItemElement5.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement5.TextLocation = new System.Drawing.Point(5, 0);
		this.inFlowsTileView.TileTemplate.Add(tileViewItemElement4);
		this.inFlowsTileView.TileTemplate.Add(tileViewItemElement5);
		this.inFlowsTileView.ItemDoubleClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(InFlowsTileView_ItemDoubleClick);
		this.inFlowsTileView.ItemCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewItemCustomizeEventHandler(inFlowsTileView_ItemCustomize);
		this.inFlowsTileView.CustomItemTemplate += new DevExpress.XtraGrid.Views.Tile.TileViewCustomItemTemplateEventHandler(inFlowsTileView_CustomItemTemplate);
		this.inFlowsTileView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(InFlowsTileView_FocusedRowChanged);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem1 });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(364, 557);
		this.Root.TextVisible = false;
		this.layoutControlItem1.Control = this.inFlowsGridControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(364, 557);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlGroup5.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup5.GroupBordersVisible = false;
		this.layoutControlGroup5.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.layoutControlItem2, this.emptySpaceItem3, this.layoutControlItem4 });
		this.layoutControlGroup5.Name = "layoutControlGroup5";
		this.layoutControlGroup5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup5.Size = new System.Drawing.Size(368, 584);
		this.layoutControlGroup5.TextVisible = false;
		this.layoutControlItem2.Control = this.inflowsLabelControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(50, 23);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(50, 0);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(318, 23);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.Control = this.inflowsGridNonCustomizableLayoutControl;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 23);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(368, 561);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.outGridToolTipController.AutoPopDelay = 10000;
		this.outGridToolTipController.CloseOnClick = DevExpress.Utils.DefaultBoolean.False;
		this.outGridToolTipController.InitialDelay = 1;
		this.outGridToolTipController.KeepWhileHovered = true;
		this.outGridToolTipController.ReshowDelay = 1;
		this.outGridToolTipController.Rounded = true;
		this.outGridToolTipController.RoundRadius = 10;
		this.outGridToolTipController.ShowBeak = true;
		this.outGridToolTipController.ToolTipAnchor = DevExpress.Utils.ToolTipAnchor.Cursor;
		this.outGridToolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(OutGridToolTipController_GetActiveObjectInfo);
		this.outflowsLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.outflowsLabelControl.Appearance.Options.UseFont = true;
		this.outflowsLabelControl.Location = new System.Drawing.Point(2, 2);
		this.outflowsLabelControl.Name = "outflowsLabelControl";
		this.outflowsLabelControl.Padding = new System.Windows.Forms.Padding(6, 1, 0, 5);
		this.outflowsLabelControl.Size = new System.Drawing.Size(55, 19);
		this.outflowsLabelControl.StyleController = this.outflowsLayoutControl;
		this.outflowsLabelControl.TabIndex = 5;
		this.outflowsLabelControl.Text = "Outflows";
		this.outflowsLayoutControl.AllowCustomization = false;
		this.outflowsLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.tablePanel1.SetColumn(this.outflowsLayoutControl, 1);
		this.outflowsLayoutControl.Controls.Add(this.outflowsGridNonCustomizableLayoutControl);
		this.outflowsLayoutControl.Controls.Add(this.outflowsLabelControl);
		this.outflowsLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.outflowsLayoutControl.Location = new System.Drawing.Point(370, 3);
		this.outflowsLayoutControl.Margin = new System.Windows.Forms.Padding(1, 3, 0, 3);
		this.outflowsLayoutControl.Name = "outflowsLayoutControl";
		this.outflowsLayoutControl.Root = this.layoutControlGroup4;
		this.tablePanel1.SetRow(this.outflowsLayoutControl, 0);
		this.outflowsLayoutControl.Size = new System.Drawing.Size(368, 584);
		this.outflowsLayoutControl.TabIndex = 0;
		this.outflowsLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.outflowsGridNonCustomizableLayoutControl.AllowCustomization = false;
		this.outflowsGridNonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.outflowsGridNonCustomizableLayoutControl.Controls.Add(this.outFlowsGridControl);
		this.outflowsGridNonCustomizableLayoutControl.Location = new System.Drawing.Point(2, 25);
		this.outflowsGridNonCustomizableLayoutControl.Name = "outflowsGridNonCustomizableLayoutControl";
		this.outflowsGridNonCustomizableLayoutControl.Root = this.layoutControlGroup1;
		this.outflowsGridNonCustomizableLayoutControl.Size = new System.Drawing.Size(364, 557);
		this.outflowsGridNonCustomizableLayoutControl.TabIndex = 6;
		this.outflowsGridNonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl2";
		this.outFlowsGridControl.AllowDrop = true;
		this.outFlowsGridControl.Location = new System.Drawing.Point(2, 2);
		this.outFlowsGridControl.MainView = this.outFlowsTileView;
		this.outFlowsGridControl.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
		this.outFlowsGridControl.Name = "outFlowsGridControl";
		this.outFlowsGridControl.Size = new System.Drawing.Size(360, 553);
		this.outFlowsGridControl.TabIndex = 4;
		this.outFlowsGridControl.ToolTipController = this.outGridToolTipController;
		this.outFlowsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.outFlowsTileView });
		this.outFlowsGridControl.DragDrop += new System.Windows.Forms.DragEventHandler(FlowsGridControl_DragDrop);
		this.outFlowsGridControl.DragEnter += new System.Windows.Forms.DragEventHandler(FlowsGridControl_DragEnter);
		this.outFlowsGridControl.MouseClick += new System.Windows.Forms.MouseEventHandler(GridControl_MouseClick);
		this.outFlowsTileView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
		this.outFlowsTileView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.outIconTileViewColumn, this.objectCaptionTileViewColumn, this.outProcessName });
		this.outFlowsTileView.GridControl = this.outFlowsGridControl;
		this.outFlowsTileView.Name = "outFlowsTileView";
		this.outFlowsTileView.OptionsTiles.ItemPadding = new System.Windows.Forms.Padding(0);
		this.outFlowsTileView.OptionsTiles.ItemSize = new System.Drawing.Size(364, 71);
		this.outFlowsTileView.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.outFlowsTileView.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		tableColumnDefinition5.Length.Value = 50.0;
		tableColumnDefinition6.Length.Value = 290.0;
		itemTemplate2.Columns.Add(tableColumnDefinition5);
		itemTemplate2.Columns.Add(tableColumnDefinition6);
		tileViewItemElement6.Column = this.outIconTileViewColumn;
		tileViewItemElement6.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement6.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.NoScale;
		tileViewItemElement6.ImageOptions.ImageSize = new System.Drawing.Size(22, 22);
		tileViewItemElement6.Text = "outIconTileViewColumn";
		tileViewItemElement6.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement7.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement7.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		tileViewItemElement7.Column = this.objectCaptionTileViewColumn;
		tileViewItemElement7.ColumnIndex = 1;
		tileViewItemElement7.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement7.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement7.Text = "objectCaptionTileViewColumn";
		tileViewItemElement7.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement7.TextLocation = new System.Drawing.Point(5, 0);
		tileViewItemElement8.Appearance.Normal.ForeColor = System.Drawing.Color.FromArgb(102, 102, 102);
		tileViewItemElement8.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement8.Column = this.outProcessName;
		tileViewItemElement8.ColumnIndex = 1;
		tileViewItemElement8.ImageOptions.Image = Dataedo.App.Properties.Resources.process_16;
		tileViewItemElement8.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement8.ImageOptions.ImageLocation = new System.Drawing.Point(3, 0);
		tileViewItemElement8.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
		tileViewItemElement8.RowIndex = 1;
		tileViewItemElement8.Text = "outProcessName";
		tileViewItemElement8.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement8.TextLocation = new System.Drawing.Point(5, 0);
		itemTemplate2.Elements.Add(tileViewItemElement6);
		itemTemplate2.Elements.Add(tileViewItemElement7);
		itemTemplate2.Elements.Add(tileViewItemElement8);
		itemTemplate2.Name = "ExternalProcessTemplate";
		tableRowDefinition3.Length.Value = 44.0;
		tableRowDefinition4.Length.Value = 25.0;
		itemTemplate2.Rows.Add(tableRowDefinition3);
		itemTemplate2.Rows.Add(tableRowDefinition4);
		this.outFlowsTileView.Templates.Add(itemTemplate2);
		tableColumnDefinition7.Length.Value = 49.0;
		tableColumnDefinition8.Length.Value = 293.0;
		this.outFlowsTileView.TileColumns.Add(tableColumnDefinition7);
		this.outFlowsTileView.TileColumns.Add(tableColumnDefinition8);
		this.outFlowsTileView.TileRows.Add(obj2);
		tileViewItemElement9.Column = this.outIconTileViewColumn;
		tileViewItemElement9.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement9.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.NoScale;
		tileViewItemElement9.ImageOptions.ImageSize = new System.Drawing.Size(22, 22);
		tileViewItemElement9.Text = "outIconTileViewColumn";
		tileViewItemElement9.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement10.Appearance.Normal.Options.UseTextOptions = true;
		tileViewItemElement10.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		tileViewItemElement10.Column = this.objectCaptionTileViewColumn;
		tileViewItemElement10.ColumnIndex = 1;
		tileViewItemElement10.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement10.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement10.Text = "objectCaptionTileViewColumn";
		tileViewItemElement10.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement10.TextLocation = new System.Drawing.Point(5, 0);
		this.outFlowsTileView.TileTemplate.Add(tileViewItemElement9);
		this.outFlowsTileView.TileTemplate.Add(tileViewItemElement10);
		this.outFlowsTileView.ItemDoubleClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(OutFlowsTileView_ItemDoubleClick);
		this.outFlowsTileView.ItemCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewItemCustomizeEventHandler(outFlowsTileView_ItemCustomize);
		this.outFlowsTileView.CustomItemTemplate += new DevExpress.XtraGrid.Views.Tile.TileViewCustomItemTemplateEventHandler(outFlowsTileView_CustomItemTemplate);
		this.outFlowsTileView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(OutFlowsTileView_FocusedRowChanged);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem3 });
		this.layoutControlGroup1.Name = "layoutControlGroup1";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(364, 557);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem3.Control = this.outFlowsGridControl;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(364, 557);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup4.GroupBordersVisible = false;
		this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.layoutControlItem7, this.emptySpaceItem4, this.layoutControlItem6 });
		this.layoutControlGroup4.Name = "layoutControlGroup4";
		this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup4.Size = new System.Drawing.Size(368, 584);
		this.layoutControlGroup4.TextVisible = false;
		this.layoutControlItem7.Control = this.outflowsLabelControl;
		this.layoutControlItem7.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.Size = new System.Drawing.Size(59, 23);
		this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.TextVisible = false;
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(59, 0);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(309, 23);
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.Control = this.outflowsGridNonCustomizableLayoutControl;
		this.layoutControlItem6.Location = new System.Drawing.Point(0, 23);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.Size = new System.Drawing.Size(368, 561);
		this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.TextVisible = false;
		this.popupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.removeFlowBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.goToBarButtonItem)
		});
		this.popupMenu.Manager = this.barManager1;
		this.popupMenu.Name = "popupMenu";
		this.removeFlowBarButtonItem.Caption = "Remove flow";
		this.removeFlowBarButtonItem.Id = 0;
		this.removeFlowBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeFlowBarButtonItem.Name = "removeFlowBarButtonItem";
		this.removeFlowBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(RemoveFlowBarButtonItem_ItemClick);
		this.goToBarButtonItem.Caption = "Go to";
		this.goToBarButtonItem.Id = 1;
		this.goToBarButtonItem.Name = "goToBarButtonItem";
		this.goToBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(GoToBarButtonItem_ItemClick);
		this.barManager1.DockControls.Add(this.barDockControlTop);
		this.barManager1.DockControls.Add(this.barDockControlBottom);
		this.barManager1.DockControls.Add(this.barDockControlLeft);
		this.barManager1.DockControls.Add(this.barDockControlRight);
		this.barManager1.Form = this;
		this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[2] { this.removeFlowBarButtonItem, this.goToBarButtonItem });
		this.barManager1.MaxItemId = 2;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager1;
		this.barDockControlTop.Size = new System.Drawing.Size(737, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 590);
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlBottom.Size = new System.Drawing.Size(737, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 590);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(737, 0);
		this.barDockControlRight.Manager = this.barManager1;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 590);
		this.tablePanel1.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 50f), new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 50f));
		this.tablePanel1.Controls.Add(this.inflowsLayoutControl);
		this.tablePanel1.Controls.Add(this.outflowsLayoutControl);
		this.tablePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tablePanel1.Location = new System.Drawing.Point(0, 0);
		this.tablePanel1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
		this.tablePanel1.Name = "tablePanel1";
		this.tablePanel1.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 26f));
		this.tablePanel1.Size = new System.Drawing.Size(737, 590);
		this.tablePanel1.TabIndex = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tablePanel1);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "DataLineageFlowsUserControl";
		base.Size = new System.Drawing.Size(737, 590);
		((System.ComponentModel.ISupportInitialize)this.inflowsLayoutControl).EndInit();
		this.inflowsLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.inflowsGridNonCustomizableLayoutControl).EndInit();
		this.inflowsGridNonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.inFlowsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.inFlowsTileView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.outflowsLayoutControl).EndInit();
		this.outflowsLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.outflowsGridNonCustomizableLayoutControl).EndInit();
		this.outflowsGridNonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.outFlowsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.outFlowsTileView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.popupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tablePanel1).EndInit();
		this.tablePanel1.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
