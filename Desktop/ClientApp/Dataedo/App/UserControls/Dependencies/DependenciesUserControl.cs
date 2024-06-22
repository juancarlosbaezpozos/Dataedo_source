using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.UserControls.Dependencies;

public class DependenciesUserControl : BaseUserControl
{
	public delegate void DependencyChangingHandler();

	private float splitterPositionRatio = 0.5f;

	private int? objectId;

	private SharedObjectTypeEnum.ObjectType? objectType;

	private MetadataEditorUserControl mainControl;

	public BindingList<Dataedo.App.Data.MetadataServer.Model.DependencyRow> AddedDependencies;

	public BindingList<Dataedo.App.Data.MetadataServer.Model.DependencyRow> DeletedDependencies;

	public BindingList<int> DeletedRelations;

	private IContainer components;

	private NonCustomizableLayoutControl layoutControl;

	private LayoutControlGroup layoutControlGroup1;

	private SplitContainerControl splitContainerControl;

	private LayoutControlItem layoutControlItem1;

	private DependenciesTreeList childrenTreeList;

	private ImageCollection treeMenuImageCollection;

	private TreeListColumn childrenNameTreeListColumn;

	private DependenciesTreeList parentsTreeList;

	private LabelControl parentsLabelControl;

	private LabelControl childrenLabelControl;

	private PopupMenu dependencyPopupMenu;

	private BarButtonItem moveUpDependencyBarButtonItem;

	private BarButtonItem moveDownDependencyBarButtonItem;

	private BarManager barManager1;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private TreeListColumn parentsNameTreeListColumn;

	private RepositoryItemAutoHeightMemoEditTreeList childrenDescriptionAutoHeightMemoEdit;

	private RepositoryItemAutoHeightMemoEditTreeList parentsDescriptionAutoHeightMemoEdit;

	private BarButtonItem removeDependencyBarButtonItem;

	private BarButtonItem goToObjectBarButtonItem;

	public List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> ChangedDependencies => GetParentsDependency()?.Where((Dataedo.App.Data.MetadataServer.Model.DependencyRow x) => x.Changed)?.Union(from x in GetChildrenDependency()
		where x.Changed
		select x)?.ToList() ?? new List<Dataedo.App.Data.MetadataServer.Model.DependencyRow>();

	public GridControl RelationsGridControl { get; set; }

	public Action SetRelationsPageAsModified { get; set; }

	[Browsable(true)]
	public event EventHandler DependeciesChanged;

	[Browsable(true)]
	public event DependencyChangingHandler DependencyChanging;

	[Browsable(true)]
	public event DependencyChangingHandler BeforeChangingRelations;

	public DependenciesUserControl()
	{
		InitializeComponent();
		AddedDependencies = new BindingList<Dataedo.App.Data.MetadataServer.Model.DependencyRow>();
		DeletedDependencies = new BindingList<Dataedo.App.Data.MetadataServer.Model.DependencyRow>();
		DeletedRelations = new BindingList<int>();
		parentsTreeList.DependeciesChanged += ParentsTreeList_DependeciesChanged;
		parentsTreeList.FocusedNodeChanged += TreeList_FocusedNodeChanged;
		childrenTreeList.DependeciesChanged += ChildrenTreeList_DependeciesChanged;
		childrenTreeList.FocusedNodeChanged += TreeList_FocusedNodeChanged;
	}

	private void ParentsTreeList_DependeciesChanged(object sender, EventArgs e)
	{
		this?.DependeciesChanged(sender, e);
	}

	private void ChildrenTreeList_DependeciesChanged(object sender, EventArgs e)
	{
		this?.DependeciesChanged(sender, e);
	}

	public bool DependencyObjectEquals(int? objectId, SharedObjectTypeEnum.ObjectType? objectType)
	{
		if (this.objectId == objectId)
		{
			return this.objectType == objectType;
		}
		return false;
	}

	private List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> GetParentsDependency()
	{
		return (childrenTreeList.DataSource as List<Dataedo.App.Data.MetadataServer.Model.DependencyRow>) ?? new List<Dataedo.App.Data.MetadataServer.Model.DependencyRow>();
	}

	private List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> GetChildrenDependency()
	{
		return (parentsTreeList.DataSource as List<Dataedo.App.Data.MetadataServer.Model.DependencyRow>) ?? new List<Dataedo.App.Data.MetadataServer.Model.DependencyRow>();
	}

	private void DependenciesUserControl_Load(object sender, EventArgs e)
	{
		FitSize();
	}

	private void splitContainerControl_SplitterMoved(object sender, EventArgs e)
	{
		if (base.Size != Size.Empty)
		{
			splitterPositionRatio = (float)splitContainerControl.Panel1.Width / (float)(splitContainerControl.Panel1.Width + splitContainerControl.Panel2.Width);
		}
	}

	private void FitSize()
	{
		if (!base.DesignMode && base.ParentForm != null)
		{
			splitContainerControl.SplitterPosition = (int)((float)(splitContainerControl.Width - splitContainerControl.SplitterBounds.Width) * splitterPositionRatio);
		}
		FitColumns();
	}

	private void FitColumns()
	{
		parentsNameTreeListColumn.BestFit();
		childrenNameTreeListColumn.BestFit();
	}

	private void DependenciesUserControl_Resize(object sender, EventArgs e)
	{
		FitSize();
	}

	public void SetParameters(int documentationId, string databaseServer, string databaseName, string databaseTitle, bool? hasMultipleSchemas, bool? showSchema, bool? showSchemaOverride, bool contextShowSchema, string schema, string name, string title, string objectSource, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype, SharedDatabaseTypeEnum.DatabaseType? databaseType, int objectId, MetadataEditorUserControl control)
	{
		DeletedDependencies.Clear();
		ChangedDependencies.Clear();
		AddedDependencies.Clear();
		parentsTreeList.BeginUpdate();
		parentsTreeList.DataSource = null;
		parentsTreeList.EndUpdate();
		childrenTreeList.BeginUpdate();
		childrenTreeList.DataSource = null;
		childrenTreeList.EndUpdate();
		mainControl = control;
		parentsTreeList.BeginUpdate();
		parentsTreeList.DataSource = new DependencyBindingList(DependenciesDB.Sort(DB.Dependency.GetUses(databaseServer, databaseName, databaseTitle, hasMultipleSchemas, showSchema, showSchemaOverride, contextShowSchema, schema, name, title, objectSource, objectType, subtype, "DBMS", new DatabaseInfo(documentationId, databaseType, databaseServer, databaseName, databaseTitle, hasMultipleSchemas, showSchema, showSchemaOverride, schema), objectId, notDeletedOnly: false, addTriggers: true, maxLevel: 2, currentLevel: 1)).ToList());
		parentsTreeList.SetParameters(control, isUses: true);
		parentsTreeList.EndUpdate();
		childrenTreeList.BeginUpdate();
		childrenTreeList.DataSource = new DependencyBindingList(DependenciesDB.Sort(DB.Dependency.GetUsedBy(databaseServer, databaseName, databaseTitle, hasMultipleSchemas, showSchema, showSchemaOverride, contextShowSchema, schema, name, title, objectSource, objectType, subtype, "DBMS", new DatabaseInfo(documentationId, databaseType, databaseServer, databaseName, databaseTitle, hasMultipleSchemas, showSchema, showSchemaOverride, schema), objectId, notDeletedOnly: false, maxLevel: 2, currentLevel: 1)).ToList());
		childrenTreeList.SetParameters(control, isUses: false);
		childrenTreeList.EndUpdate();
		this.objectId = objectId;
		this.objectType = objectType;
		parentsNameTreeListColumn.ToolTip = "Hierarchy of objects used by this object imported from the source (not all sources provide this information). You can also drag & drop objects from repository navigator (left pane) to define dependencies manually. ";
		childrenNameTreeListColumn.ToolTip = "Hierarchy of objects that use this object imported from the source (not all sources provide this information). You can also drag & drop objects from repository navigator (left pane) to define dependencies manually.";
		FitColumns();
	}

	public void AddEvents(MetadataEditorUserControl control)
	{
	}

	public void DependencyButtonsVisibleChanged(object sender, EventArgs e)
	{
	}

	private void ShowDependencyButtons()
	{
		mainControl.SetRemoveDependencyButtonVisibility(new BoolEventArgs(CanFocusedNodeBeDeleted()));
	}

	public bool CanFocusedNodeBeDeleted()
	{
		return GetFocusedDependencyRow()?.CanDelete ?? false;
	}

	private DependenciesTreeList GetActiveTreeList()
	{
		if (childrenTreeList.Focused || childrenTreeList.ActiveEditor != null)
		{
			return childrenTreeList;
		}
		if (parentsTreeList.Focused || parentsTreeList.ActiveEditor != null)
		{
			return parentsTreeList;
		}
		return null;
	}

	public void UpdateDependenciesByRelation(RelationRow row)
	{
		UpdateDependencies(row, parentsTreeList);
		UpdateDependencies(row, childrenTreeList);
	}

	private void UpdateDependencies(RelationRow node, DependenciesTreeList treeList)
	{
		IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> enumerable = from x in treeList.DataSource as List<Dataedo.App.Data.MetadataServer.Model.DependencyRow>
			where x != null && x.DependencyCommonType == Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Relation && x.ObjectId == node.Id
			select (x);
		treeList.BeginUpdate();
		foreach (Dataedo.App.Data.MetadataServer.Model.DependencyRow item in enumerable)
		{
			item.Description = node.Description;
		}
		treeList.EndUpdate();
	}

	private void treeList_GetSelectImage(object sender, GetSelectImageEventArgs e)
	{
		Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow = (sender as DependenciesTreeList).GetDependencyRow(e.Node);
		IconsSupport.SetNodeImageIndex(treeMenuImageCollection, e, dependencyRow);
	}

	private TreeListNode GetFocusedNode()
	{
		return GetActiveTreeList()?.FocusedNode;
	}

	private Dataedo.App.Data.MetadataServer.Model.DependencyRow GetFocusedDependencyRow()
	{
		DependenciesTreeList activeTreeList = GetActiveTreeList();
		if (activeTreeList == null)
		{
			return null;
		}
		TreeListNode focusedNode = activeTreeList.FocusedNode;
		return activeTreeList.GetDependencyRow(focusedNode);
	}

	private void UpdateTreeListSameDependencies(Dataedo.App.Data.MetadataServer.Model.DependencyRow node, TreeList treeList)
	{
		IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> enumerable = from x in treeList.DataSource as List<Dataedo.App.Data.MetadataServer.Model.DependencyRow>
			where x != null && x != node && x.Parent != null && x.DependencyId == node.DependencyId && x.IsSameDependency(node)
			select (x);
		treeList.BeginUpdate();
		foreach (Dataedo.App.Data.MetadataServer.Model.DependencyRow item in enumerable)
		{
			item.Description = node.Description;
		}
		treeList.EndUpdate();
	}

	private void UpdateRelations(Dataedo.App.Data.MetadataServer.Model.DependencyRow node)
	{
		this.BeforeChangingRelations?.Invoke();
		if (RelationsGridControl == null)
		{
			return;
		}
		IEnumerable<RelationRow> enumerable = from x in RelationsGridControl.DataSource as BindingList<RelationRow>
			where x != null && node.DependencyCommonType == Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Relation && x.Id == node.ObjectId
			select (x);
		RelationsGridControl.BeginUpdate();
		foreach (RelationRow item in enumerable)
		{
			item.Description = node.Description;
			item.SetModified();
		}
		RelationsGridControl.EndUpdate();
	}

	private void DeleteRelation(Dataedo.App.Data.MetadataServer.Model.DependencyRow node)
	{
		this.BeforeChangingRelations?.Invoke();
		if (RelationsGridControl == null)
		{
			return;
		}
		BindingList<RelationRow> obj = RelationsGridControl.DataSource as BindingList<RelationRow>;
		IEnumerable<RelationRow> second = obj.Where((RelationRow x) => x != null && node.DependencyCommonType == Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Relation && x.Id == node.ObjectId);
		BindingList<RelationRow> bindingList = new BindingList<RelationRow>();
		foreach (RelationRow item in obj.Except(second))
		{
			bindingList.Add(item);
		}
		RelationsGridControl.BeginUpdate();
		RelationsGridControl.DataSource = bindingList;
		RelationsGridControl.EndUpdate();
	}

	private void parentsTreeList_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		HandleTreeListCellValueChanged(e.Node, sender as DependenciesTreeList);
	}

	private void childrenTreeList_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		HandleTreeListCellValueChanged(e.Node, sender as DependenciesTreeList);
	}

	private void parentsTreeList_DependencyChanging()
	{
		this.DependencyChanging?.Invoke();
	}

	private void childrenTreeList_DependencyChanging()
	{
		this.DependencyChanging?.Invoke();
	}

	private bool parentsTreeList_DependencyAdding(Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow)
	{
		return AddDependency(dependencyRow);
	}

	private bool childrenTreeList_DependencyAdding(Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow)
	{
		return AddDependency(dependencyRow);
	}

	private void HandleTreeListCellValueChanged(TreeListNode treeListNode, DependenciesTreeList treeList)
	{
		Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow = treeList.GetDependencyRow(treeListNode);
		UpdateTreeListSameDependencies(dependencyRow, parentsTreeList);
		UpdateTreeListSameDependencies(dependencyRow, childrenTreeList);
		if (dependencyRow.ObjectId.HasValue && dependencyRow.DependencyCommonType == Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Relation)
		{
			UpdateRelations(dependencyRow);
		}
	}

	private void treeList_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		this.DependencyChanging?.Invoke();
	}

	private void TreeListFocusedChanged(object sender, EventArgs e)
	{
		bool value = (childrenTreeList.Focused && childrenTreeList.FocusedNode != null) || (parentsTreeList.Focused && parentsTreeList.FocusedNode != null);
		mainControl.SetMoveDependencyButtonsVisibility(new BoolEventArgs(value));
		mainControl.SetRemoveDependencyButtonVisibility(new BoolEventArgs(CanFocusedNodeBeDeleted()));
	}

	private void TreeList_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		mainControl.SetRemoveDependencyButtonVisibility(new BoolEventArgs(GetFocusedDependencyRow()?.CanDelete ?? false));
	}

	private void TreeList_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Right)
		{
			return;
		}
		DependenciesTreeList dependenciesTreeList = sender as DependenciesTreeList;
		dependenciesTreeList.Focus();
		TreeListHitInfo treeListHitInfo = dependenciesTreeList.CalcHitInfo(new Point(e.X, e.Y));
		if (treeListHitInfo != null && treeListHitInfo.HitInfoType == HitInfoType.Cell)
		{
			dependenciesTreeList.FocusedNode = treeListHitInfo.Node;
			dependenciesTreeList.CloseEditor();
			Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow = dependenciesTreeList.GetDependencyRow(treeListHitInfo.Node);
			Image image = null;
			switch (dependencyRow.TypeEnum)
			{
			case SharedObjectTypeEnum.ObjectType.Table:
			case SharedObjectTypeEnum.ObjectType.Trigger:
				image = Resources.table_16;
				break;
			case SharedObjectTypeEnum.ObjectType.View:
				image = Resources.view_16;
				break;
			case SharedObjectTypeEnum.ObjectType.Structure:
				image = Resources.object_16;
				break;
			case SharedObjectTypeEnum.ObjectType.Procedure:
				image = Resources.procedure_16;
				break;
			case SharedObjectTypeEnum.ObjectType.Function:
				image = Resources.function_16;
				break;
			default:
				image = Resources.unresolved_16;
				break;
			}
			goToObjectBarButtonItem.Glyph = image;
			string value = ((dependencyRow.TypeEnum == SharedObjectTypeEnum.ObjectType.Trigger) ? dependencyRow.DisplayTableName : dependencyRow.DisplayNameWithoutRelation);
			if (!dependencyRow.DestinationObjectId.HasValue)
			{
				goToObjectBarButtonItem.Caption = Escaping.EscapeTextForUI(value) + " (unavailable)";
				goToObjectBarButtonItem.Enabled = false;
			}
			else if (dependencyRow.DestinationObjectId != objectId)
			{
				goToObjectBarButtonItem.Caption = "Go to " + Escaping.EscapeTextForUI(value);
				goToObjectBarButtonItem.Enabled = true;
			}
			goToObjectBarButtonItem.Visibility = ((dependencyRow.DestinationObjectId.HasValue && dependencyRow.DestinationObjectId == objectId) ? BarItemVisibility.Never : BarItemVisibility.Always);
			goToObjectBarButtonItem.Enabled = dependencyRow.DestinationObjectId.HasValue;
			removeDependencyBarButtonItem.Visibility = ((dependencyRow == null || !dependencyRow.CanDelete) ? BarItemVisibility.Never : BarItemVisibility.Always);
			dependencyPopupMenu.ShowPopup(Control.MousePosition);
		}
	}

	internal void CloseAllEditors()
	{
		parentsTreeList.CloseEditor();
		childrenTreeList.CloseEditor();
	}

	private void removeDependencyBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		DeleteDependency();
	}

	private void treeList_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Delete)
		{
			DeleteDependency();
		}
	}

	public bool AddDependency(Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow)
	{
		try
		{
			mainControl.SetWaitformVisibility(visible: true);
			if (dependencyRow.IsUsesObject)
			{
				DB.Dependency.FillUses(dependencyRow, dependencyRow.CurrentDatabase, notDeletedOnly: false);
			}
			else
			{
				DB.Dependency.FillUsedBy(dependencyRow, dependencyRow.CurrentDatabase, notDeletedOnly: false);
			}
			DependenciesTreeList sourceDependenciesTreeList = (dependencyRow.IsUsesObject ? parentsTreeList : childrenTreeList);
			if ((sourceDependenciesTreeList.DataSource as DependencyBindingList).GetFirstSameDependency(dependencyRow) == null)
			{
				AddedDependencies.Add(dependencyRow);
				this.DependencyChanging?.Invoke();
				return true;
			}
			TreeListNode sameDependencyTreeListNode = null;
			sourceDependenciesTreeList.NodesIterator.DoOperation(delegate(TreeListNode x)
			{
				if ((sourceDependenciesTreeList.GetDataRecordByNode(x) as Dataedo.App.Data.MetadataServer.Model.DependencyRow).IsRepresentingSameDependency(dependencyRow))
				{
					sameDependencyTreeListNode = x;
				}
			});
			if (sameDependencyTreeListNode != null)
			{
				sourceDependenciesTreeList.FocusedNode = sameDependencyTreeListNode;
				sourceDependenciesTreeList.ExpandParents(sourceDependenciesTreeList.FocusedNode);
			}
			return false;
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			mainControl.SetWaitformVisibility(visible: false);
		}
	}

	public void DeleteDependency()
	{
		try
		{
			DependenciesTreeList activeTreeList = GetActiveTreeList();
			if (activeTreeList == null)
			{
				return;
			}
			TreeListNode focusedNode = GetFocusedNode();
			if (focusedNode == null)
			{
				return;
			}
			Dataedo.App.Data.MetadataServer.Model.DependencyRow dependency = activeTreeList.GetDependencyRow(focusedNode);
			Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow = dependency;
			if (dependencyRow == null || !dependencyRow.CanDelete || !CommonFunctionsDatabase.AskIfDeleting(dependency, FindForm()))
			{
				return;
			}
			if (dependency.IsSaved)
			{
				if (dependency.DependencyCommonType == Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Normal)
				{
					DeletedDependencies.Add(dependency);
				}
				else if (dependency.DependencyType == Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeType.UserRelation || dependency.DependencyType == Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeType.PKUserRelation)
				{
					DeleteRelation(dependency);
					DeletedRelations.Add(dependency.ObjectId.Value);
				}
			}
			else
			{
				Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow2 = AddedDependencies.FirstOrDefault((Dataedo.App.Data.MetadataServer.Model.DependencyRow x) => x.Id == dependency.Id && x.IsUsesObject == dependency.IsUsesObject);
				if (dependencyRow2 != null)
				{
					AddedDependencies.Remove(dependencyRow2);
				}
			}
			activeTreeList.DeleteNode(focusedNode);
			this.DependencyChanging?.Invoke();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting dependency.", FindForm());
		}
	}

	private void goToObjectBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		GetActiveTreeList()?.GoToObject(FindForm());
	}

	private void parentsTreeList_BeforeExpand(object sender, BeforeExpandEventArgs e)
	{
		try
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(StaticData.splashScreenManager, show: true);
			if (!(sender is TreeList treeList) || !(treeList.GetDataRecordByNode(e.Node) is Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow))
			{
				return;
			}
			foreach (Dataedo.App.Data.MetadataServer.Model.DependencyRow child in dependencyRow.Children)
			{
				DB.Dependency.FillUses(child, dependencyRow.CurrentDatabase, notDeletedOnly: false, 1, 1);
			}
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(StaticData.splashScreenManager, show: false);
		}
	}

	private void childrenTreeList_BeforeExpand(object sender, BeforeExpandEventArgs e)
	{
		try
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(StaticData.splashScreenManager, show: true);
			if (!(sender is TreeList treeList) || !(treeList.GetDataRecordByNode(e.Node) is Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow))
			{
				return;
			}
			foreach (Dataedo.App.Data.MetadataServer.Model.DependencyRow child in dependencyRow.Children)
			{
				DB.Dependency.FillUsedBy(child, dependencyRow.CurrentDatabase, notDeletedOnly: false, 1, 1);
			}
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(StaticData.splashScreenManager, show: false);
		}
	}

	private void DependenciesUserControl_Leave(object sender, EventArgs e)
	{
		parentsTreeList.DestroyCustomization();
		childrenTreeList.DestroyCustomization();
	}

	private void ParentsTreeList_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInTreelistHeaderPopup(e);
	}

	private void ChildrenTreeList_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInTreelistHeaderPopup(e);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.Dependencies.DependenciesUserControl));
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
		this.parentsTreeList = new Dataedo.App.UserControls.Dependencies.DependenciesTreeList();
		this.parentsNameTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.parentsDescriptionAutoHeightMemoEdit = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEditTreeList();
		this.treeMenuImageCollection = new DevExpress.Utils.ImageCollection(this.components);
		this.parentsLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.childrenTreeList = new Dataedo.App.UserControls.Dependencies.DependenciesTreeList();
		this.childrenNameTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.childrenDescriptionAutoHeightMemoEdit = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEditTreeList();
		this.childrenLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.dependencyPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.moveUpDependencyBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveDownDependencyBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.goToObjectBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeDependencyBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl).BeginInit();
		this.splitContainerControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.parentsTreeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.parentsDescriptionAutoHeightMemoEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.treeMenuImageCollection).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.childrenTreeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.childrenDescriptionAutoHeightMemoEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dependencyPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).BeginInit();
		base.SuspendLayout();
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl.Controls.Add(this.splitContainerControl);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.Root = this.layoutControlGroup1;
		this.layoutControl.Size = new System.Drawing.Size(571, 397);
		this.layoutControl.TabIndex = 0;
		this.layoutControl.Text = "layoutControl1";
		this.splitContainerControl.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl.Name = "splitContainerControl";
		this.splitContainerControl.Panel1.Controls.Add(this.parentsTreeList);
		this.splitContainerControl.Panel1.Controls.Add(this.parentsLabelControl);
		this.splitContainerControl.Panel1.Text = "Panel1";
		this.splitContainerControl.Panel2.Controls.Add(this.childrenTreeList);
		this.splitContainerControl.Panel2.Controls.Add(this.childrenLabelControl);
		this.splitContainerControl.Panel2.Text = "Panel2";
		this.splitContainerControl.Size = new System.Drawing.Size(571, 397);
		this.splitContainerControl.SplitterPosition = 283;
		this.splitContainerControl.TabIndex = 4;
		this.splitContainerControl.Text = "splitContainerControl1";
		this.splitContainerControl.SplitterMoved += new System.EventHandler(splitContainerControl_SplitterMoved);
		this.parentsTreeList.AllowDrop = true;
		this.parentsTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[1] { this.parentsNameTreeListColumn });
		this.parentsTreeList.Dock = System.Windows.Forms.DockStyle.Fill;
		this.parentsTreeList.Font = new System.Drawing.Font("Tahoma", 8.25f);
		this.parentsTreeList.ImageIndexFieldName = "Type";
		this.parentsTreeList.KeyFieldName = "Id";
		this.parentsTreeList.Location = new System.Drawing.Point(0, 18);
		this.parentsTreeList.Name = "parentsTreeList";
		this.parentsTreeList.OptionsBehavior.Editable = false;
		this.parentsTreeList.OptionsBehavior.ReadOnly = true;
		this.parentsTreeList.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.parentsTreeList.ParentFieldName = "ParentId";
		this.parentsTreeList.PreviewFieldName = "DisplayName";
		this.parentsTreeList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.parentsDescriptionAutoHeightMemoEdit });
		this.parentsTreeList.SelectImageList = this.treeMenuImageCollection;
		this.parentsTreeList.Size = new System.Drawing.Size(283, 379);
		this.parentsTreeList.TabIndex = 0;
		this.parentsTreeList.DependencyAdding += new Dataedo.App.UserControls.Dependencies.DependenciesTreeList.DependencyAddingEventHandler(parentsTreeList_DependencyAdding);
		this.parentsTreeList.DependencyChanging += new Dataedo.App.UserControls.Dependencies.DependenciesUserControl.DependencyChangingHandler(parentsTreeList_DependencyChanging);
		this.parentsTreeList.GetSelectImage += new DevExpress.XtraTreeList.GetSelectImageEventHandler(treeList_GetSelectImage);
		this.parentsTreeList.BeforeExpand += new DevExpress.XtraTreeList.BeforeExpandEventHandler(parentsTreeList_BeforeExpand);
		this.parentsTreeList.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(ParentsTreeList_PopupMenuShowing);
		this.parentsTreeList.CellValueChanging += new DevExpress.XtraTreeList.CellValueChangedEventHandler(treeList_CellValueChanging);
		this.parentsTreeList.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(parentsTreeList_CellValueChanged);
		this.parentsTreeList.Enter += new System.EventHandler(TreeListFocusedChanged);
		this.parentsTreeList.KeyDown += new System.Windows.Forms.KeyEventHandler(treeList_KeyDown);
		this.parentsTreeList.Leave += new System.EventHandler(TreeListFocusedChanged);
		this.parentsTreeList.MouseDown += new System.Windows.Forms.MouseEventHandler(TreeList_MouseDown);
		this.parentsNameTreeListColumn.Caption = "Name";
		this.parentsNameTreeListColumn.FieldName = "DisplayName";
		this.parentsNameTreeListColumn.MinWidth = 33;
		this.parentsNameTreeListColumn.Name = "parentsNameTreeListColumn";
		this.parentsNameTreeListColumn.OptionsColumn.AllowEdit = false;
		this.parentsNameTreeListColumn.OptionsColumn.AllowSort = false;
		this.parentsNameTreeListColumn.Visible = true;
		this.parentsNameTreeListColumn.VisibleIndex = 0;
		this.parentsDescriptionAutoHeightMemoEdit.Name = "parentsDescriptionAutoHeightMemoEdit";
		this.parentsDescriptionAutoHeightMemoEdit.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.treeMenuImageCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("treeMenuImageCollection.ImageStream");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.function_16, "function_16", typeof(Dataedo.App.Properties.Resources), 0);
		this.treeMenuImageCollection.Images.SetKeyName(0, "function_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.function_deleted_16, "function_deleted_16", typeof(Dataedo.App.Properties.Resources), 1);
		this.treeMenuImageCollection.Images.SetKeyName(1, "function_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.procedure_16, "procedure_16", typeof(Dataedo.App.Properties.Resources), 2);
		this.treeMenuImageCollection.Images.SetKeyName(2, "procedure_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.procedure_deleted_16, "procedure_deleted_16", typeof(Dataedo.App.Properties.Resources), 3);
		this.treeMenuImageCollection.Images.SetKeyName(3, "procedure_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_1x_1x_24, "relation_1x_1x_24", typeof(Dataedo.App.Properties.Resources), 4);
		this.treeMenuImageCollection.Images.SetKeyName(4, "relation_1x_1x_24");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_1x_1x_user_24, "relation_1x_1x_user_24", typeof(Dataedo.App.Properties.Resources), 5);
		this.treeMenuImageCollection.Images.SetKeyName(5, "relation_1x_1x_user_24");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_1x_mx_24, "relation_1x_mx_24", typeof(Dataedo.App.Properties.Resources), 6);
		this.treeMenuImageCollection.Images.SetKeyName(6, "relation_1x_mx_24");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_1x_mx_user_24, "relation_1x_mx_user_24", typeof(Dataedo.App.Properties.Resources), 7);
		this.treeMenuImageCollection.Images.SetKeyName(7, "relation_1x_mx_user_24");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_mx_1x_24, "relation_mx_1x_24", typeof(Dataedo.App.Properties.Resources), 8);
		this.treeMenuImageCollection.Images.SetKeyName(8, "relation_mx_1x_24");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_mx_1x_user_24, "relation_mx_1x_user_24", typeof(Dataedo.App.Properties.Resources), 9);
		this.treeMenuImageCollection.Images.SetKeyName(9, "relation_mx_1x_user_24");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.table_16, "table_16", typeof(Dataedo.App.Properties.Resources), 10);
		this.treeMenuImageCollection.Images.SetKeyName(10, "table_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.table_user_16, "table_user_16", typeof(Dataedo.App.Properties.Resources), 11);
		this.treeMenuImageCollection.Images.SetKeyName(11, "table_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.table_deleted_16, "table_deleted_16", typeof(Dataedo.App.Properties.Resources), 12);
		this.treeMenuImageCollection.Images.SetKeyName(12, "table_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.trigger_active_16, "trigger_active_16", typeof(Dataedo.App.Properties.Resources), 13);
		this.treeMenuImageCollection.Images.SetKeyName(13, "trigger_active_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.trigger_deleted_16, "trigger_deleted_16", typeof(Dataedo.App.Properties.Resources), 14);
		this.treeMenuImageCollection.Images.SetKeyName(14, "trigger_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.trigger_disabled_16, "trigger_disabled_16", typeof(Dataedo.App.Properties.Resources), 15);
		this.treeMenuImageCollection.Images.SetKeyName(15, "trigger_disabled_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.unresolved_16, "unresolved_16", typeof(Dataedo.App.Properties.Resources), 16);
		this.treeMenuImageCollection.Images.SetKeyName(16, "unresolved_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_function_16, "used_by_function_16", typeof(Dataedo.App.Properties.Resources), 17);
		this.treeMenuImageCollection.Images.SetKeyName(17, "used_by_function_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_procedure_16, "used_by_procedure_16", typeof(Dataedo.App.Properties.Resources), 18);
		this.treeMenuImageCollection.Images.SetKeyName(18, "used_by_procedure_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_table_16, "used_by_table_16", typeof(Dataedo.App.Properties.Resources), 19);
		this.treeMenuImageCollection.Images.SetKeyName(19, "used_by_table_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_trigger_active_16, "used_by_trigger_active_16", typeof(Dataedo.App.Properties.Resources), 20);
		this.treeMenuImageCollection.Images.SetKeyName(20, "used_by_trigger_active_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_trigger_disabled_16, "used_by_trigger_disabled_16", typeof(Dataedo.App.Properties.Resources), 21);
		this.treeMenuImageCollection.Images.SetKeyName(21, "used_by_trigger_disabled_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_unresolved_16, "used_by_unresolved_16", typeof(Dataedo.App.Properties.Resources), 22);
		this.treeMenuImageCollection.Images.SetKeyName(22, "used_by_unresolved_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_user_function_16, "used_by_user_function_16", typeof(Dataedo.App.Properties.Resources), 23);
		this.treeMenuImageCollection.Images.SetKeyName(23, "used_by_user_function_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_user_procedure_16, "used_by_user_procedure_16", typeof(Dataedo.App.Properties.Resources), 24);
		this.treeMenuImageCollection.Images.SetKeyName(24, "used_by_user_procedure_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_user_table_16, "used_by_user_table_16", typeof(Dataedo.App.Properties.Resources), 25);
		this.treeMenuImageCollection.Images.SetKeyName(25, "used_by_user_table_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_user_table_user_16, "used_by_user_table_user_16", typeof(Dataedo.App.Properties.Resources), 26);
		this.treeMenuImageCollection.Images.SetKeyName(26, "used_by_user_table_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_user_trigger_active_16, "used_by_user_trigger_active_16", typeof(Dataedo.App.Properties.Resources), 27);
		this.treeMenuImageCollection.Images.SetKeyName(27, "used_by_user_trigger_active_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_user_trigger_disabled_16, "used_by_user_trigger_disabled_16", typeof(Dataedo.App.Properties.Resources), 28);
		this.treeMenuImageCollection.Images.SetKeyName(28, "used_by_user_trigger_disabled_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_user_unresolved_16, "used_by_user_unresolved_16", typeof(Dataedo.App.Properties.Resources), 29);
		this.treeMenuImageCollection.Images.SetKeyName(29, "used_by_user_unresolved_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_user_view_16, "used_by_user_view_16", typeof(Dataedo.App.Properties.Resources), 30);
		this.treeMenuImageCollection.Images.SetKeyName(30, "used_by_user_view_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_view_16, "used_by_view_16", typeof(Dataedo.App.Properties.Resources), 31);
		this.treeMenuImageCollection.Images.SetKeyName(31, "used_by_view_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_function_16, "uses_function_16", typeof(Dataedo.App.Properties.Resources), 32);
		this.treeMenuImageCollection.Images.SetKeyName(32, "uses_function_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_procedure_16, "uses_procedure_16", typeof(Dataedo.App.Properties.Resources), 33);
		this.treeMenuImageCollection.Images.SetKeyName(33, "uses_procedure_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_table_16, "uses_table_16", typeof(Dataedo.App.Properties.Resources), 34);
		this.treeMenuImageCollection.Images.SetKeyName(34, "uses_table_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_trigger_active_16, "uses_trigger_active_16", typeof(Dataedo.App.Properties.Resources), 35);
		this.treeMenuImageCollection.Images.SetKeyName(35, "uses_trigger_active_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_trigger_disabled_16, "uses_trigger_disabled_16", typeof(Dataedo.App.Properties.Resources), 36);
		this.treeMenuImageCollection.Images.SetKeyName(36, "uses_trigger_disabled_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_unresolved_16, "uses_unresolved_16", typeof(Dataedo.App.Properties.Resources), 37);
		this.treeMenuImageCollection.Images.SetKeyName(37, "uses_unresolved_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_user_function_16, "uses_user_function_16", typeof(Dataedo.App.Properties.Resources), 38);
		this.treeMenuImageCollection.Images.SetKeyName(38, "uses_user_function_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_user_procedure_16, "uses_user_procedure_16", typeof(Dataedo.App.Properties.Resources), 39);
		this.treeMenuImageCollection.Images.SetKeyName(39, "uses_user_procedure_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_user_table_16, "uses_user_table_16", typeof(Dataedo.App.Properties.Resources), 40);
		this.treeMenuImageCollection.Images.SetKeyName(40, "uses_user_table_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_user_table_user_16, "uses_user_table_user_16", typeof(Dataedo.App.Properties.Resources), 41);
		this.treeMenuImageCollection.Images.SetKeyName(41, "uses_user_table_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_user_trigger_active_16, "uses_user_trigger_active_16", typeof(Dataedo.App.Properties.Resources), 42);
		this.treeMenuImageCollection.Images.SetKeyName(42, "uses_user_trigger_active_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_user_trigger_disabled_16, "uses_user_trigger_disabled_16", typeof(Dataedo.App.Properties.Resources), 43);
		this.treeMenuImageCollection.Images.SetKeyName(43, "uses_user_trigger_disabled_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_user_unresolved_16, "uses_user_unresolved_16", typeof(Dataedo.App.Properties.Resources), 44);
		this.treeMenuImageCollection.Images.SetKeyName(44, "uses_user_unresolved_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_user_view_16, "uses_user_view_16", typeof(Dataedo.App.Properties.Resources), 45);
		this.treeMenuImageCollection.Images.SetKeyName(45, "uses_user_view_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_view_16, "uses_view_16", typeof(Dataedo.App.Properties.Resources), 46);
		this.treeMenuImageCollection.Images.SetKeyName(46, "uses_view_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.view_16, "view_16", typeof(Dataedo.App.Properties.Resources), 47);
		this.treeMenuImageCollection.Images.SetKeyName(47, "view_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.view_deleted_16, "view_deleted_16", typeof(Dataedo.App.Properties.Resources), 48);
		this.treeMenuImageCollection.Images.SetKeyName(48, "view_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.collection_16, "collection_16", typeof(Dataedo.App.Properties.Resources), 49);
		this.treeMenuImageCollection.Images.SetKeyName(49, "collection_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.cube_16, "cube_16", typeof(Dataedo.App.Properties.Resources), 50);
		this.treeMenuImageCollection.Images.SetKeyName(50, "cube_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.custom_object_16, "custom_object_16", typeof(Dataedo.App.Properties.Resources), 51);
		this.treeMenuImageCollection.Images.SetKeyName(51, "custom_object_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.dimension_16, "dimension_16", typeof(Dataedo.App.Properties.Resources), 52);
		this.treeMenuImageCollection.Images.SetKeyName(52, "dimension_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.editioning_view_16, "editioning_view_16", typeof(Dataedo.App.Properties.Resources), 53);
		this.treeMenuImageCollection.Images.SetKeyName(53, "editioning_view_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.entity_16, "entity_16", typeof(Dataedo.App.Properties.Resources), 54);
		this.treeMenuImageCollection.Images.SetKeyName(54, "entity_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_object_16, "external_object_16", typeof(Dataedo.App.Properties.Resources), 55);
		this.treeMenuImageCollection.Images.SetKeyName(55, "external_object_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_table_16, "external_table_16", typeof(Dataedo.App.Properties.Resources), 56);
		this.treeMenuImageCollection.Images.SetKeyName(56, "external_table_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.file_table_16, "file_table_16", typeof(Dataedo.App.Properties.Resources), 57);
		this.treeMenuImageCollection.Images.SetKeyName(57, "file_table_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.flat_file_16, "flat_file_16", typeof(Dataedo.App.Properties.Resources), 58);
		this.treeMenuImageCollection.Images.SetKeyName(58, "flat_file_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.foreign_table_16, "foreign_table_16", typeof(Dataedo.App.Properties.Resources), 59);
		this.treeMenuImageCollection.Images.SetKeyName(59, "foreign_table_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_edge_table_16, "graph_edge_table_16", typeof(Dataedo.App.Properties.Resources), 60);
		this.treeMenuImageCollection.Images.SetKeyName(60, "graph_edge_table_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_node_table_16, "graph_node_table_16", typeof(Dataedo.App.Properties.Resources), 61);
		this.treeMenuImageCollection.Images.SetKeyName(61, "graph_node_table_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_table_16, "graph_table_16", typeof(Dataedo.App.Properties.Resources), 62);
		this.treeMenuImageCollection.Images.SetKeyName(62, "graph_table_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.history_table_16, "history_table_16", typeof(Dataedo.App.Properties.Resources), 63);
		this.treeMenuImageCollection.Images.SetKeyName(63, "history_table_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.search_index_16, "search_index_16", typeof(Dataedo.App.Properties.Resources), 64);
		this.treeMenuImageCollection.Images.SetKeyName(64, "search_index_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.indexed_view_16, "indexed_view_16", typeof(Dataedo.App.Properties.Resources), 65);
		this.treeMenuImageCollection.Images.SetKeyName(65, "indexed_view_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.materialized_view_16, "materialized_view_16", typeof(Dataedo.App.Properties.Resources), 66);
		this.treeMenuImageCollection.Images.SetKeyName(66, "materialized_view_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.named_query_16, "named_query_16", typeof(Dataedo.App.Properties.Resources), 67);
		this.treeMenuImageCollection.Images.SetKeyName(67, "named_query_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.object_16, "object_16", typeof(Dataedo.App.Properties.Resources), 68);
		this.treeMenuImageCollection.Images.SetKeyName(68, "object_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.package_16, "package_16", typeof(Dataedo.App.Properties.Resources), 69);
		this.treeMenuImageCollection.Images.SetKeyName(69, "package_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_active_16, "rule_active_16", typeof(Dataedo.App.Properties.Resources), 70);
		this.treeMenuImageCollection.Images.SetKeyName(70, "rule_active_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_disabled_16, "rule_disabled_16", typeof(Dataedo.App.Properties.Resources), 71);
		this.treeMenuImageCollection.Images.SetKeyName(71, "rule_disabled_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.standard_object_16, "standard_object_16", typeof(Dataedo.App.Properties.Resources), 72);
		this.treeMenuImageCollection.Images.SetKeyName(72, "standard_object_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.system_versioned_table_16, "system_versioned_table_16", typeof(Dataedo.App.Properties.Resources), 73);
		this.treeMenuImageCollection.Images.SetKeyName(73, "system_versioned_table_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_mx_mx_24, "relation_mx_mx_24", typeof(Dataedo.App.Properties.Resources), 74);
		this.treeMenuImageCollection.Images.SetKeyName(74, "relation_mx_mx_24");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_mx_mx_user_24, "relation_mx_mx_user_24", typeof(Dataedo.App.Properties.Resources), 75);
		this.treeMenuImageCollection.Images.SetKeyName(75, "relation_mx_mx_user_24");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_object_16, "used_by_object_16", typeof(Dataedo.App.Properties.Resources), 76);
		this.treeMenuImageCollection.Images.SetKeyName(76, "used_by_object_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_user_object_16, "used_by_user_object_16", typeof(Dataedo.App.Properties.Resources), 77);
		this.treeMenuImageCollection.Images.SetKeyName(77, "used_by_user_object_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_user_object_user_16, "used_by_user_object_user_16", typeof(Dataedo.App.Properties.Resources), 78);
		this.treeMenuImageCollection.Images.SetKeyName(78, "used_by_user_object_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_object_16, "uses_object_16", typeof(Dataedo.App.Properties.Resources), 79);
		this.treeMenuImageCollection.Images.SetKeyName(79, "uses_object_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_user_object_16, "uses_user_object_16", typeof(Dataedo.App.Properties.Resources), 80);
		this.treeMenuImageCollection.Images.SetKeyName(80, "uses_user_object_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_user_object_user_16, "uses_user_object_user_16", typeof(Dataedo.App.Properties.Resources), 81);
		this.treeMenuImageCollection.Images.SetKeyName(81, "uses_user_object_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.object_user_16, "object_user_16", typeof(Dataedo.App.Properties.Resources), 82);
		this.treeMenuImageCollection.Images.SetKeyName(82, "object_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_structure_16, "used_by_structure_16", typeof(Dataedo.App.Properties.Resources), 83);
		this.treeMenuImageCollection.Images.SetKeyName(83, "used_by_structure_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_user_structure_16, "used_by_user_structure_16", typeof(Dataedo.App.Properties.Resources), 84);
		this.treeMenuImageCollection.Images.SetKeyName(84, "used_by_user_structure_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.used_by_user_structure_user_16, "used_by_user_structure_user_16", typeof(Dataedo.App.Properties.Resources), 85);
		this.treeMenuImageCollection.Images.SetKeyName(85, "used_by_user_structure_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_structure_16, "uses_structure_16", typeof(Dataedo.App.Properties.Resources), 86);
		this.treeMenuImageCollection.Images.SetKeyName(86, "uses_structure_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_user_structure_16, "uses_user_structure_16", typeof(Dataedo.App.Properties.Resources), 87);
		this.treeMenuImageCollection.Images.SetKeyName(87, "uses_user_structure_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.uses_user_structure_user_16, "uses_user_structure_user_16", typeof(Dataedo.App.Properties.Resources), 88);
		this.treeMenuImageCollection.Images.SetKeyName(88, "uses_user_structure_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.avro_record_16, "avro_record_16", typeof(Dataedo.App.Properties.Resources), 89);
		this.treeMenuImageCollection.Images.SetKeyName(89, "avro_record_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.avro_record_user_16, "avro_record_user_16", typeof(Dataedo.App.Properties.Resources), 90);
		this.treeMenuImageCollection.Images.SetKeyName(90, "avro_record_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.cdm_16, "cdm_16", typeof(Dataedo.App.Properties.Resources), 91);
		this.treeMenuImageCollection.Images.SetKeyName(91, "cdm_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.cdm_user_16, "cdm_user_16", typeof(Dataedo.App.Properties.Resources), 92);
		this.treeMenuImageCollection.Images.SetKeyName(92, "cdm_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.csv_16, "csv_16", typeof(Dataedo.App.Properties.Resources), 93);
		this.treeMenuImageCollection.Images.SetKeyName(93, "csv_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.csv_user_16, "csv_user_16", typeof(Dataedo.App.Properties.Resources), 94);
		this.treeMenuImageCollection.Images.SetKeyName(94, "csv_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.delimited_text_16, "delimited_text_16", typeof(Dataedo.App.Properties.Resources), 95);
		this.treeMenuImageCollection.Images.SetKeyName(95, "delimited_text_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.delimited_text_user_16, "delimited_text_user_16", typeof(Dataedo.App.Properties.Resources), 96);
		this.treeMenuImageCollection.Images.SetKeyName(96, "delimited_text_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.directory_16, "directory_16", typeof(Dataedo.App.Properties.Resources), 97);
		this.treeMenuImageCollection.Images.SetKeyName(97, "directory_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.directory_user_16, "directory_user_16", typeof(Dataedo.App.Properties.Resources), 98);
		this.treeMenuImageCollection.Images.SetKeyName(98, "directory_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.json_16, "json_16", typeof(Dataedo.App.Properties.Resources), 99);
		this.treeMenuImageCollection.Images.SetKeyName(99, "json_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.json_user_16, "json_user_16", typeof(Dataedo.App.Properties.Resources), 100);
		this.treeMenuImageCollection.Images.SetKeyName(100, "json_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.orc_16, "orc_16", typeof(Dataedo.App.Properties.Resources), 101);
		this.treeMenuImageCollection.Images.SetKeyName(101, "orc_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.orc_user_16, "orc_user_16", typeof(Dataedo.App.Properties.Resources), 102);
		this.treeMenuImageCollection.Images.SetKeyName(102, "orc_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.parquet_16, "parquet_16", typeof(Dataedo.App.Properties.Resources), 103);
		this.treeMenuImageCollection.Images.SetKeyName(103, "parquet_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.parquet_user_16, "parquet_user_16", typeof(Dataedo.App.Properties.Resources), 104);
		this.treeMenuImageCollection.Images.SetKeyName(104, "parquet_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.xml_16, "xml_16", typeof(Dataedo.App.Properties.Resources), 105);
		this.treeMenuImageCollection.Images.SetKeyName(105, "xml_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.xml_user_16, "xml_user_16", typeof(Dataedo.App.Properties.Resources), 106);
		this.treeMenuImageCollection.Images.SetKeyName(106, "xml_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.structure_16, "structure_16", typeof(Dataedo.App.Properties.Resources), 107);
		this.treeMenuImageCollection.Images.SetKeyName(107, "structure_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.structure_user_16, "structure_user_16", typeof(Dataedo.App.Properties.Resources), 108);
		this.treeMenuImageCollection.Images.SetKeyName(108, "structure_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.delta_lake_16, "delta_lake_16", typeof(Dataedo.App.Properties.Resources), 109);
		this.treeMenuImageCollection.Images.SetKeyName(109, "delta_lake_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.delta_lake_user_16, "delta_lake_user_16", typeof(Dataedo.App.Properties.Resources), 110);
		this.treeMenuImageCollection.Images.SetKeyName(110, "delta_lake_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.excel_table_16, "excel_table_16", typeof(Dataedo.App.Properties.Resources), 111);
		this.treeMenuImageCollection.Images.SetKeyName(111, "excel_table_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.excel_table_user_16, "excel_table_user_16", typeof(Dataedo.App.Properties.Resources), 112);
		this.treeMenuImageCollection.Images.SetKeyName(112, "excel_table_user_16");
		this.parentsLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.parentsLabelControl.Appearance.Options.UseFont = true;
		this.parentsLabelControl.Appearance.Options.UseTextOptions = true;
		this.parentsLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.parentsLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.parentsLabelControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.parentsLabelControl.Location = new System.Drawing.Point(0, 0);
		this.parentsLabelControl.Name = "parentsLabelControl";
		this.parentsLabelControl.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
		this.parentsLabelControl.Size = new System.Drawing.Size(283, 18);
		this.parentsLabelControl.TabIndex = 1;
		this.parentsLabelControl.Text = "Uses";
		this.childrenTreeList.AllowDrop = true;
		this.childrenTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[1] { this.childrenNameTreeListColumn });
		this.childrenTreeList.Dock = System.Windows.Forms.DockStyle.Fill;
		this.childrenTreeList.Font = new System.Drawing.Font("Tahoma", 8.25f);
		this.childrenTreeList.ImageIndexFieldName = "Type";
		this.childrenTreeList.KeyFieldName = "Id";
		this.childrenTreeList.Location = new System.Drawing.Point(0, 18);
		this.childrenTreeList.Name = "childrenTreeList";
		this.childrenTreeList.OptionsBehavior.Editable = false;
		this.childrenTreeList.OptionsBehavior.ReadOnly = true;
		this.childrenTreeList.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.childrenTreeList.ParentFieldName = "ParentId";
		this.childrenTreeList.PreviewFieldName = "DisplayName";
		this.childrenTreeList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.childrenDescriptionAutoHeightMemoEdit });
		this.childrenTreeList.SelectImageList = this.treeMenuImageCollection;
		this.childrenTreeList.Size = new System.Drawing.Size(278, 379);
		this.childrenTreeList.TabIndex = 0;
		this.childrenTreeList.DependencyAdding += new Dataedo.App.UserControls.Dependencies.DependenciesTreeList.DependencyAddingEventHandler(childrenTreeList_DependencyAdding);
		this.childrenTreeList.DependencyChanging += new Dataedo.App.UserControls.Dependencies.DependenciesUserControl.DependencyChangingHandler(childrenTreeList_DependencyChanging);
		this.childrenTreeList.GetSelectImage += new DevExpress.XtraTreeList.GetSelectImageEventHandler(treeList_GetSelectImage);
		this.childrenTreeList.BeforeExpand += new DevExpress.XtraTreeList.BeforeExpandEventHandler(childrenTreeList_BeforeExpand);
		this.childrenTreeList.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(ChildrenTreeList_PopupMenuShowing);
		this.childrenTreeList.CellValueChanging += new DevExpress.XtraTreeList.CellValueChangedEventHandler(treeList_CellValueChanging);
		this.childrenTreeList.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(childrenTreeList_CellValueChanged);
		this.childrenTreeList.Enter += new System.EventHandler(TreeListFocusedChanged);
		this.childrenTreeList.KeyDown += new System.Windows.Forms.KeyEventHandler(treeList_KeyDown);
		this.childrenTreeList.Leave += new System.EventHandler(TreeListFocusedChanged);
		this.childrenTreeList.MouseDown += new System.Windows.Forms.MouseEventHandler(TreeList_MouseDown);
		this.childrenNameTreeListColumn.Caption = "Name";
		this.childrenNameTreeListColumn.FieldName = "DisplayName";
		this.childrenNameTreeListColumn.MinWidth = 33;
		this.childrenNameTreeListColumn.Name = "childrenNameTreeListColumn";
		this.childrenNameTreeListColumn.OptionsColumn.AllowEdit = false;
		this.childrenNameTreeListColumn.OptionsColumn.AllowSort = false;
		this.childrenNameTreeListColumn.Visible = true;
		this.childrenNameTreeListColumn.VisibleIndex = 0;
		this.childrenNameTreeListColumn.Width = 92;
		this.childrenDescriptionAutoHeightMemoEdit.Name = "childrenDescriptionAutoHeightMemoEdit";
		this.childrenDescriptionAutoHeightMemoEdit.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.childrenLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.childrenLabelControl.Appearance.Options.UseFont = true;
		this.childrenLabelControl.Appearance.Options.UseTextOptions = true;
		this.childrenLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.childrenLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.childrenLabelControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.childrenLabelControl.Location = new System.Drawing.Point(0, 0);
		this.childrenLabelControl.Name = "childrenLabelControl";
		this.childrenLabelControl.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
		this.childrenLabelControl.Size = new System.Drawing.Size(278, 18);
		this.childrenLabelControl.TabIndex = 2;
		this.childrenLabelControl.Text = "Used by";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem1 });
		this.layoutControlGroup1.Name = "layoutControlGroup1";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(571, 397);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem1.Control = this.splitContainerControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem1.Size = new System.Drawing.Size(571, 397);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.dependencyPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.moveUpDependencyBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.moveDownDependencyBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.goToObjectBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeDependencyBarButtonItem)
		});
		this.dependencyPopupMenu.Manager = this.barManager1;
		this.dependencyPopupMenu.Name = "dependencyPopupMenu";
		this.moveUpDependencyBarButtonItem.Caption = "Move up";
		this.moveUpDependencyBarButtonItem.Id = 0;
		this.moveUpDependencyBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_up_16;
		this.moveUpDependencyBarButtonItem.Name = "moveUpDependencyBarButtonItem";
		this.moveUpDependencyBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
		this.moveDownDependencyBarButtonItem.Caption = "Move down";
		this.moveDownDependencyBarButtonItem.Id = 1;
		this.moveDownDependencyBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_down_16;
		this.moveDownDependencyBarButtonItem.Name = "moveDownDependencyBarButtonItem";
		this.moveDownDependencyBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
		this.goToObjectBarButtonItem.Caption = "Go to object";
		this.goToObjectBarButtonItem.Id = 3;
		this.goToObjectBarButtonItem.Name = "goToObjectBarButtonItem";
		this.goToObjectBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(goToObjectBarButtonItem_ItemClick);
		this.removeDependencyBarButtonItem.Caption = "Remove dependency from repository";
		this.removeDependencyBarButtonItem.Id = 2;
		this.removeDependencyBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeDependencyBarButtonItem.Name = "removeDependencyBarButtonItem";
		this.removeDependencyBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
		this.removeDependencyBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(removeDependencyBarButtonItem_ItemClick);
		this.barManager1.DockControls.Add(this.barDockControlTop);
		this.barManager1.DockControls.Add(this.barDockControlBottom);
		this.barManager1.DockControls.Add(this.barDockControlLeft);
		this.barManager1.DockControls.Add(this.barDockControlRight);
		this.barManager1.Form = this;
		this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[4] { this.moveUpDependencyBarButtonItem, this.moveDownDependencyBarButtonItem, this.removeDependencyBarButtonItem, this.goToObjectBarButtonItem });
		this.barManager1.MaxItemId = 4;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager1;
		this.barDockControlTop.Size = new System.Drawing.Size(571, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 397);
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlBottom.Size = new System.Drawing.Size(571, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 397);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(571, 0);
		this.barDockControlRight.Manager = this.barManager1;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 397);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.layoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "DependenciesUserControl";
		base.Size = new System.Drawing.Size(571, 397);
		base.Load += new System.EventHandler(DependenciesUserControl_Load);
		base.Leave += new System.EventHandler(DependenciesUserControl_Leave);
		base.Resize += new System.EventHandler(DependenciesUserControl_Resize);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl).EndInit();
		this.splitContainerControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.parentsTreeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.parentsDescriptionAutoHeightMemoEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.treeMenuImageCollection).EndInit();
		((System.ComponentModel.ISupportInitialize)this.childrenTreeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.childrenDescriptionAutoHeightMemoEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dependencyPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
