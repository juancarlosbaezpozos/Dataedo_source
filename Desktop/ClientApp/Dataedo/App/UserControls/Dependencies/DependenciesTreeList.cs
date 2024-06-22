using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.ViewInfo;

namespace Dataedo.App.UserControls.Dependencies;

public class DependenciesTreeList : TreeList
{
	public delegate bool DependencyAddingEventHandler(Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow);

	private MetadataEditorUserControl mainControl;

	private TreeListNode currentDragAndDropFocusedNode;

	private bool isUses;

	private IContainer components;

	private ToolTipController toolTipController;

	[Browsable(true)]
	public event DependencyAddingEventHandler DependencyAdding;

	[Browsable(true)]
	public event DependenciesUserControl.DependencyChangingHandler DependencyChanging;

	[Browsable(true)]
	public event EventHandler DependeciesChanged;

	public DependenciesTreeList()
	{
		InitializeComponent();
	}

	public void SetParameters(MetadataEditorUserControl control, bool isUses)
	{
		ForceInitialize();
		mainControl = control;
		this.isUses = isUses;
		foreach (TreeListNode node in base.Nodes)
		{
			if (node.Level == 0)
			{
				node.Expanded = true;
			}
		}
	}

	private void DependenciesTreeList_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		if (e.Node.Level != 0 || e.Node.HasChildren)
		{
			Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyFromNode = GetDependencyFromNode(e.Node);
			if (dependencyFromNode == null || dependencyFromNode.IsActive)
			{
				goto IL_0047;
			}
		}
		e.Appearance.ForeColor = Color.Gray;
		goto IL_0047;
		IL_0047:
		if (currentDragAndDropFocusedNode == e.Node)
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.TreeListCustomFocusBackColor;
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.TreeListCustomFocusForeColor;
		}
	}

	private Dataedo.App.Data.MetadataServer.Model.DependencyRow GetDependencyFromNode(TreeListNode node)
	{
		if (node == null)
		{
			return null;
		}
		return GetDataRecordByNode(node) as Dataedo.App.Data.MetadataServer.Model.DependencyRow;
	}

	private void DependenciesTreeList_KeyDown(object sender, KeyEventArgs e)
	{
		TreeListNode treeListNode = base.FocusedNode;
		switch (e.KeyCode)
		{
		case Keys.Left:
			if (treeListNode != null && treeListNode.HasChildren && treeListNode != null && treeListNode.Expanded)
			{
				treeListNode.Expanded = false;
				e.Handled = true;
			}
			break;
		case Keys.Right:
			treeListNode = base.FocusedNode;
			if (treeListNode != null && treeListNode.HasChildren && treeListNode != null && !treeListNode.Expanded)
			{
				treeListNode.Expanded = true;
				e.Handled = true;
			}
			break;
		}
	}

	private void toolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		TreeListHitInfo treeListHitInfo = CalcHitInfo(e.ControlMousePosition);
		Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyFromNode = GetDependencyFromNode(treeListHitInfo.Node);
		if (treeListHitInfo != null && treeListHitInfo.HitInfoType == HitInfoType.SelectImage)
		{
			string value = dependencyFromNode?.ToolTipText ?? "Unresolved Entity";
			if (!string.IsNullOrEmpty(value))
			{
				object @object = new TreeListCellToolTipInfo(treeListHitInfo.Node, treeListHitInfo.Column, null);
				e.Info = new ToolTipControlInfo(@object, value);
			}
		}
	}

	private void DependenciesTreeList_ShowingEditor(object sender, CancelEventArgs e)
	{
		TreeListNode treeListNode = base.FocusedNode;
		if (treeListNode != null && treeListNode.Level == 0)
		{
			e.Cancel = true;
		}
	}

	public void GoToObject(Form owner = null)
	{
		try
		{
			Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyFromNode = GetDependencyFromNode(base.FocusedNode);
			if (dependencyFromNode != null && dependencyFromNode.DestinationObjectId.HasValue)
			{
				mainControl.SelectDependencyObject(dependencyFromNode);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to go to selected object", owner);
		}
		finally
		{
			mainControl.SetWaitformVisibility(visible: false);
		}
	}

	public Dataedo.App.Data.MetadataServer.Model.DependencyRow GetDependencyRow(TreeListNode treeListNode)
	{
		return GetDataRecordByNode(treeListNode) as Dataedo.App.Data.MetadataServer.Model.DependencyRow;
	}

	public void ExpandParents(TreeListNode treeListNode)
	{
		if (treeListNode.ParentNode != null)
		{
			treeListNode.ParentNode.Expanded = true;
			ExpandParents(treeListNode.ParentNode);
		}
	}

	private void DependenciesTreeList_DragDrop(object sender, DragEventArgs e)
	{
		TreeList treeList = (TreeList)sender;
		DXDragEventArgs dXDragEventArgs = e.GetDXDragEventArgs(treeList);
		DBTreeNode dBTreeNode = mainControl.MetadataTreeList.GetDataRecordByNode(dXDragEventArgs.Node) as DBTreeNode;
		Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow2 = treeList.GetDataRecordByNode(dXDragEventArgs.TargetRootNode) as Dataedo.App.Data.MetadataServer.Model.DependencyRow;
		Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow3 = treeList.GetDataRecordByNode(dXDragEventArgs.TargetNode) as Dataedo.App.Data.MetadataServer.Model.DependencyRow;
		_ = dXDragEventArgs.DragInsertPosition;
		if (dBTreeNode != null && dependencyRow3 != null)
		{
			DatabaseRow databaseRow = new DatabaseRow(DB.Database.GetDataById(dBTreeNode.DatabaseId));
			Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow = new Dataedo.App.Data.MetadataServer.Model.DependencyRow(dBTreeNode.DatabaseId, dBTreeNode.DatabaseType, databaseRow.Host, databaseRow.Name, databaseRow.Title, databaseRow.HasMultipleSchemas, databaseRow.ShowSchema, databaseRow.ShowSchemaOverride, dependencyRow2.ContextShowSchema || DatabaseRow.GetShowSchema(databaseRow.ShowSchema, databaseRow.ShowSchemaOverride) || DatabaseRow.GetShowSchema(dependencyRow2.CurrentDatabase.ShowSchema, dependencyRow2.CurrentDatabase.ShowSchemaOverride), dBTreeNode.Schema, dBTreeNode.BaseName, dBTreeNode.Title, UserTypeEnum.TypeToString(dBTreeNode.Source.GetValueOrDefault()), SharedObjectTypeEnum.TypeToString(dBTreeNode.ObjectType), dBTreeNode.Id, null, dBTreeNode.ObjectType, dBTreeNode.Subtype, "DBMS", dependencyRow2.CurrentDatabase, isUses);
			dependencyRow.DestinationDatabaseId = dBTreeNode.DatabaseId;
			dependencyRow.Parent = dependencyRow3;
			dependencyRow.Source = "USER";
			dependencyRow.Status = "A";
			dependencyRow.IsSaved = false;
			if (this.DependencyAdding == null || this.DependencyAdding(dependencyRow))
			{
				dependencyRow3.Children.Add(dependencyRow);
				dXDragEventArgs.TargetNode.Expanded = true;
				TreeListNode treeListNode = dXDragEventArgs.TargetNode.Nodes.FirstOrDefault((TreeListNode x) => GetDataRecordByNode(x) is Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow4 && dependencyRow4.IsRepresentingSameDependency(dependencyRow));
				if (treeListNode != null)
				{
					base.FocusedNode = treeListNode;
				}
			}
		}
		SetAndRefreshDragAndDropFocusedNode(sender);
	}

	private void DependenciesTreeList_DragEnter(object sender, DragEventArgs e)
	{
		SetDragDropEffect(sender, e);
	}

	private void DependenciesTreeList_DragOver(object sender, DragEventArgs e)
	{
		SetDragDropEffect(sender, e);
	}

	private void DependenciesTreeList_DragLeave(object sender, EventArgs e)
	{
		SetAndRefreshDragAndDropFocusedNode(sender);
	}

	private void DependenciesTreeList_CalcNodeDragImageIndex(object sender, CalcNodeDragImageIndexEventArgs e)
	{
		TreeList obj = (TreeList)sender;
		DBTreeNode dBTreeNode = null;
		TreeListNode treeListNode = e.DragArgs.Data.GetData(typeof(TreeListNode)) as TreeListNode;
		Dataedo.App.Data.MetadataServer.Model.DependencyRow targetNodeData = obj.GetDataRecordByNode(e.Node) as Dataedo.App.Data.MetadataServer.Model.DependencyRow;
		if (treeListNode != null)
		{
			dBTreeNode = mainControl.MetadataTreeList.GetDataRecordByNode(treeListNode) as DBTreeNode;
		}
		if (dBTreeNode == null || !CanBeDraggedAndDropped(dBTreeNode, targetNodeData))
		{
			e.ImageIndex = -1;
		}
		else
		{
			e.ImageIndex = 1;
		}
	}

	private void SetDragDropEffect(object sender, DragEventArgs e)
	{
		TreeList treeList = (TreeList)sender;
		DXDragEventArgs dXDragEventArgs = e.GetDXDragEventArgs(treeList);
		_ = currentDragAndDropFocusedNode;
		if (dXDragEventArgs.TargetNode != null && dXDragEventArgs.Node != null && dXDragEventArgs.Node.TreeList != treeList)
		{
			DBTreeNode nodeData = mainControl.MetadataTreeList.GetDataRecordByNode(dXDragEventArgs.Node) as DBTreeNode;
			Dataedo.App.Data.MetadataServer.Model.DependencyRow targetNodeData = treeList.GetDataRecordByNode(dXDragEventArgs.TargetNode) as Dataedo.App.Data.MetadataServer.Model.DependencyRow;
			if (CanBeDraggedAndDropped(nodeData, targetNodeData))
			{
				e.Effect = DragDropEffects.Copy;
				SetAndRefreshDragAndDropFocusedNode(treeList, dXDragEventArgs.TargetNode);
			}
			else
			{
				e.Effect = DragDropEffects.None;
				SetAndRefreshDragAndDropFocusedNode(treeList);
			}
		}
		else
		{
			e.Effect = DragDropEffects.None;
			SetAndRefreshDragAndDropFocusedNode(treeList);
		}
	}

	private bool CanBeDraggedAndDropped(DBTreeNode nodeData, Dataedo.App.Data.MetadataServer.Model.DependencyRow targetNodeData)
	{
		if (nodeData != null && nodeData.IsNormalObject && ((targetNodeData != null && targetNodeData.DependencyCommonType == Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Normal) || (targetNodeData != null && targetNodeData.DependencyCommonType == Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Trigger)))
		{
			return true;
		}
		return false;
	}

	private void SetAndRefreshDragAndDropFocusedNode(TreeList list, TreeListNode currentForDropNode = null)
	{
		if (list != null)
		{
			TreeListNode node = currentDragAndDropFocusedNode;
			currentDragAndDropFocusedNode = currentForDropNode;
			list.RefreshNode(node);
		}
	}

	private void SetAndRefreshDragAndDropFocusedNode(object sender, TreeListNode currentForDropNode = null)
	{
		SetAndRefreshDragAndDropFocusedNode((TreeList)sender, currentForDropNode);
	}

	private void DependenciesTreeList_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopupWithoutSorting(e, base.CustomizationForm);
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
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		((System.ComponentModel.ISupportInitialize)this).BeginInit();
		base.SuspendLayout();
		this.toolTipController.AutoPopDelay = 15000;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.toolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(toolTipController_GetActiveObjectInfo);
		this.AllowDrop = true;
		this.Font = new System.Drawing.Font("Tahoma", 8.25f);
		base.OptionsBehavior.AllowExpandAnimation = DevExpress.Utils.DefaultBoolean.True;
		base.OptionsBehavior.AllowExpandOnDblClick = false;
		base.OptionsCustomization.AllowFilter = false;
		base.OptionsFilter.AllowFilterEditor = false;
		base.OptionsFind.AllowFindPanel = false;
		base.OptionsMenu.ShowExpandCollapseItems = false;
		base.OptionsView.ShowHorzLines = false;
		base.OptionsView.ShowIndicator = false;
		base.OptionsView.ShowVertLines = false;
		base.ToolTipController = this.toolTipController;
		base.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(DependenciesTreeList_NodeCellStyle);
		base.CalcNodeDragImageIndex += new DevExpress.XtraTreeList.CalcNodeDragImageIndexEventHandler(DependenciesTreeList_CalcNodeDragImageIndex);
		base.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(DependenciesTreeList_PopupMenuShowing);
		base.ShowingEditor += new System.ComponentModel.CancelEventHandler(DependenciesTreeList_ShowingEditor);
		base.DragDrop += new System.Windows.Forms.DragEventHandler(DependenciesTreeList_DragDrop);
		base.DragEnter += new System.Windows.Forms.DragEventHandler(DependenciesTreeList_DragEnter);
		base.DragOver += new System.Windows.Forms.DragEventHandler(DependenciesTreeList_DragOver);
		base.DragLeave += new System.EventHandler(DependenciesTreeList_DragLeave);
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(DependenciesTreeList_KeyDown);
		((System.ComponentModel.ISupportInitialize)this).EndInit();
		base.ResumeLayout(false);
	}
}
