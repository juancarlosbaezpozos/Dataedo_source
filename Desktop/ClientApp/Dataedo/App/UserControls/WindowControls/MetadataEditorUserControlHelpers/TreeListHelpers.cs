using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DataProfiling.Tools;
using Dataedo.App.Enums;
using Dataedo.App.Helpers.Forms;
using Dataedo.App.Licences;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.DragDrop;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.FormsSupport;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Dataedo.App.UserControls.PanelControls;
using Dataedo.App.UserControls.PanelControls.TableUserControlHelpers.Interfaces;
using Dataedo.App.UserControls.SummaryControls;
using Dataedo.CustomMessageBox;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.FeedbackWidgetData;
using Dataedo.Model.Data.Interfaces;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraTreeList.ViewInfo;
using RecentProjectsLibrary;

namespace Dataedo.App.UserControls.WindowControls.MetadataEditorUserControlHelpers;

public class TreeListHelpers
{
	private class CollapseAllOperation : TreeListOperation
	{
		public override bool NeedsFullIteration => true;

		public override void Execute(TreeListNode node)
		{
			if (node.HasChildren)
			{
				node.Expanded = false;
			}
		}

		public override bool NeedsVisitChildren(TreeListNode node)
		{
			return true;
		}
	}

	public const string ModulesTreeFolderName = "Subject Areas";

	public const string TablesTreeFolderName = "Tables";

	public const string ViewsTreeFolderName = "Views";

	public const string StructuresTreeFolderName = "Structures";

	public const string ProceduresTreeFolderName = "Procedures";

	public const string FunctionsTreeFolderName = "Functions";

	public const string TermsTreeFolderName = "Terms";

	public static readonly CultureInfo NodesCountCulture = CultureInfo.GetCultureInfo("en-US");

	private readonly MetadataEditorUserControl metadataEditorUserControl;

	private readonly TreeList metadataTreeList;

	private TableUserControl tableUserControl;

	private TableSummaryUserControl tableSummaryUserControl;

	private TableSummaryUserControl viewSummaryUserControl;

	private TableSummaryUserControl objectSummaryUserControl;

	private ProcedureSummaryUserControl procedureSummaryUserControl;

	private ProcedureSummaryUserControl functionSummaryUserControl;

	private ModuleSummaryUserControl moduleSummaryUserControl;

	private TermSummaryUserControl termSummaryUserControl;

	public bool LockFocus { get; set; }

	public bool StartedToFocus { get; set; }

	public bool DisableFocusNodeChangedEventActions { get; set; }

	public bool DisableFocusEvents { get; set; }

	public Point? MouseDownPoint { get; set; }

	public TreeListNode CustomFocusedTreeListNode { get; set; }

	public TreeListNode FocusNodeToRestoreAfterRename { get; set; }

	public bool IsCustomFocus => CustomFocusedTreeListNode != metadataTreeList.FocusedNode;

	public TreeListNode ParentForAddModule { get; set; }

	public bool ShowProgress => metadataEditorUserControl.ShowProgress;

	public ProgressTypeModel ProgressType => metadataEditorUserControl.ProgressType;

	public void DeleteDbElement(MetadataEditorUserControl metadataEditorUserControl, SplashScreenManager splashScreenManager, DBTreeMenu dbTreeMenu, Func<Action, Action, bool> continueAfterPossibleChanges, Func<BasePanelControl> getVisibleUserControl, Action clearDataPanel, bool onlyFromModule = false, bool fromCustomFocus = false)
	{
		try
		{
			if (!Licenses.CheckRepositoryVersionAfterLogin())
			{
				return;
			}
			metadataTreeList.CloseEditor();
			DBTreeNode node = GetNode(metadataTreeList.FocusedNode);
			TreeListNode deletedNode = (fromCustomFocus ? CustomFocusedTreeListNode : metadataTreeList.FocusedNode);
			DBTreeNode deletedDbTreeNode = GetNode(deletedNode);
			if (deletedDbTreeNode == null || deletedDbTreeNode.IsFolder || (onlyFromModule && GetFocusedNode() == deletedDbTreeNode && !continueAfterPossibleChanges(null, null)))
			{
				return;
			}
			ModuleUserControl moduleUserControl = getVisibleUserControl() as ModuleUserControl;
			bool flag = deletedDbTreeNode.ContainsNode(node);
			if ((flag || (!onlyFromModule && moduleUserControl != null)) && !continueAfterPossibleChanges(delegate
			{
				metadataEditorUserControl.OpenPageControl();
			}, null))
			{
				return;
			}
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			bool saveChanges = deletedDbTreeNode.Id == deletedDbTreeNode.DatabaseId && deletedDbTreeNode.DatabaseId != node.DatabaseId;
			bool flag2 = deletedDbTreeNode.Nodes.Count > 0;
			bool num = ((deletedDbTreeNode.IsInModule && onlyFromModule) ? CommonFunctionsDatabase.DeleteObjectFromModule(splashScreenManager, deletedDbTreeNode, metadataEditorUserControl.FindForm()) : DeleteSelectedObject(splashScreenManager, deletedDbTreeNode, saveChanges, continueAfterPossibleChanges, metadataEditorUserControl.FindForm()));
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			if (!num)
			{
				return;
			}
			if (!onlyFromModule && (GetFocusedNode() == deletedDbTreeNode || node.IsRepresentingSameObject(GetNode(deletedNode))))
			{
				getVisibleUserControl()?.Edit.SetUnchanged();
			}
			if (deletedDbTreeNode.IsInModule && !onlyFromModule)
			{
				TreeListNode treeListNode = metadataTreeList.Nodes.SingleOrDefault((TreeListNode x) => GetNode(x).Id == deletedDbTreeNode.DatabaseId).Nodes.SingleOrDefault((TreeListNode x) => GetNode(x).Name.Equals(deletedDbTreeNode.ParentNode.Name));
				treeListNode.Expanded = true;
				TreeListNode node2 = treeListNode.Nodes.SingleOrDefault((TreeListNode x) => GetNode(x).Id == deletedDbTreeNode.Id);
				treeListNode.Expanded = false;
				metadataTreeList.DeleteNode(node2);
			}
			TreeListNode treeListNode2 = null;
			if (flag && deletedDbTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Term)
			{
				treeListNode2 = deletedNode.ParentNode;
				if (treeListNode2 == null)
				{
					treeListNode2 = metadataTreeList.Nodes.FirstOrDefault((TreeListNode x) => x != deletedNode);
				}
				if (treeListNode2 != null)
				{
					metadataTreeList.FocusedNode = treeListNode2;
				}
				else
				{
					clearDataPanel();
				}
			}
			else if (deletedDbTreeNode == node && deletedDbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Term)
			{
				treeListNode2 = deletedNode.ParentNode;
				if (treeListNode2 != null)
				{
					metadataTreeList.FocusedNode = treeListNode2;
				}
				else
				{
					clearDataPanel();
				}
			}
			TreeListNode parentNode = deletedNode.ParentNode;
			metadataTreeList.DeleteNode(deletedNode);
			if (parentNode != null)
			{
				metadataTreeList.RefreshNode(parentNode);
			}
			if (metadataTreeList.Nodes.Count == 0)
			{
				clearDataPanel();
			}
			else if (!onlyFromModule && getVisibleUserControl() is ModuleUserControl)
			{
				metadataEditorUserControl.OpenPageControl();
			}
			if (node.IsNormalObject && deletedDbTreeNode.Id == deletedDbTreeNode.DatabaseId)
			{
				metadataEditorUserControl.RefreshTree(showWaitForm: true);
			}
			else if ((!deletedDbTreeNode.IsInModule || !onlyFromModule) && deletedDbTreeNode.Id != deletedDbTreeNode.DatabaseId)
			{
				metadataEditorUserControl.RemoveFromModulesInTree(deletedDbTreeNode);
				DB.Database.UpdateDocumentationShowSchemaFlag(deletedDbTreeNode.DatabaseId);
				if (deletedDbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Term && flag2)
				{
					DrawingHelpers.SuspendDrawing(metadataTreeList);
					metadataTreeList.BeginUpdate();
					try
					{
						DisableFocusEvents = true;
						bool flag3 = node.ContainsNode(deletedDbTreeNode);
						dbTreeMenu.RefeshDBData(deletedDbTreeNode.ParentNode, metadataEditorUserControl.ShowProgress, metadataEditorUserControl.ProgressType);
						TreeListNode treeListNode3 = this.metadataEditorUserControl.SearchTreeNodeOperation.FindNode(parentNode.Nodes, node.Id, node.ObjectType);
						if (treeListNode3 != null)
						{
							metadataTreeList.FocusedNode = treeListNode3;
							this.metadataEditorUserControl.OpenPageControl();
						}
						else if (treeListNode2 == null && flag3)
						{
							clearDataPanel();
						}
					}
					finally
					{
						DisableFocusEvents = false;
						metadataTreeList.EndUpdate();
						DrawingHelpers.ResumeDrawing(metadataTreeList);
					}
					if (this.metadataEditorUserControl.ShowProgress)
					{
						this.metadataEditorUserControl.RefreshProgress(showWaitForm: true, deletedDbTreeNode.ParentNode);
					}
				}
			}
			else if (!deletedDbTreeNode.IsNormalObject)
			{
				metadataEditorUserControl.RefreshTree(showWaitForm: true);
			}
			if (metadataEditorUserControl.ShowProgress)
			{
				DBTreeNode.AggregateProgressUp(deletedDbTreeNode.ParentNode, metadataEditorUserControl.ProgressType);
			}
			if (deletedDbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Database || deletedDbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
			{
				metadataEditorUserControl.RefreshSearchDatabasesFromMetadataTreeList();
				this.metadataEditorUserControl.RebuildHomePage(forceReload: false);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, metadataEditorUserControl.FindForm());
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	private bool DeleteSelectedObject(SplashScreenManager splashScreenManager, DBTreeNode nodeToDelete, bool saveChanges, Func<Action, Action, bool> continueAfterPossibleChanges, Form owner = null)
	{
		if (saveChanges)
		{
			continueAfterPossibleChanges(null, null);
		}
		return CommonFunctionsDatabase.DeleteObject(splashScreenManager, nodeToDelete, owner);
	}

	public void DragDropTerms(TreeListNode targetTreeNode, DBTreeNode targetDBNode, DBTreeNode draggedDBNodeFromTree, List<object> draggedRowsFromGrid, int? draggedDatabaseId, string draggedDatabaseTitle, int? draggedObjectId, string draggedBaseName, SharedObjectTypeEnum.ObjectType? draggedObjectType, Form owner = null)
	{
		if (CheckWhetherIsValidTermDragDrop(draggedObjectId, draggedObjectType, targetDBNode))
		{
			int? targetId = ((targetDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.Term) ? new int?(targetDBNode.Id) : null);
			bool num = draggedDatabaseId == targetDBNode.DatabaseId;
			if (!num)
			{
				_ = draggedDatabaseTitle + "." + draggedBaseName;
			}
			if (!num)
			{
				_ = targetDBNode.DatabaseTitle + "." + targetDBNode.BaseName;
			}
			else
			{
				_ = targetDBNode.BaseName;
			}
			if (draggedRowsFromGrid != null)
			{
				foreach (TermObject item in draggedRowsFromGrid?.Cast<TermObject>()?.ToList())
				{
					DBTreeNode mainNode = null;
					if (ShowProgress)
					{
						mainNode = DBTreeMenu.FindDBNodeById(SharedObjectTypeEnum.ObjectType.Term, item.TermId.Value)?.ParentNode;
					}
					ProcessChangingTermParent(item.TermId.Value, targetId, targetDBNode, targetTreeNode, owner);
					if ((ShowProgress && metadataEditorUserControl.ProgressType.Type == ProgressTypeEnum.AllDocumentations) || (ShowProgress && metadataEditorUserControl.CustomFieldsSupport.Fields.Where((CustomFieldRowExtended x) => x.TermVisibility).Any((CustomFieldRowExtended x) => x.FieldName.Equals(metadataEditorUserControl.ProgressType.FieldName))))
					{
						DBTreeNode mainNode2 = DBTreeMenu.FindDBNodeById(SharedObjectTypeEnum.ObjectType.Term, item.TermId.Value);
						DBTreeNode.AggregateProgressDown(mainNode, ProgressType);
						DBTreeNode.AggregateProgressUp(mainNode, ProgressType);
						DBTreeNode.AggregateProgressUp(mainNode2, ProgressType);
					}
				}
				return;
			}
			DBTreeNode mainNode3 = null;
			if (ShowProgress)
			{
				mainNode3 = DBTreeMenu.FindDBNodeById(SharedObjectTypeEnum.ObjectType.Term, draggedObjectId.Value).ParentNode;
			}
			ProcessChangingTermParent(draggedObjectId.Value, targetId, targetDBNode, targetTreeNode, owner);
			if ((ShowProgress && metadataEditorUserControl.ProgressType.Type == ProgressTypeEnum.AllDocumentations) || (ShowProgress && metadataEditorUserControl.CustomFieldsSupport.Fields.Where((CustomFieldRowExtended x) => x.TermVisibility).Any((CustomFieldRowExtended x) => x.FieldName.Equals(metadataEditorUserControl.ProgressType.FieldName))))
			{
				DBTreeNode mainNode4 = DBTreeMenu.FindDBNodeById(SharedObjectTypeEnum.ObjectType.Term, draggedDBNodeFromTree.Id);
				DBTreeNode.AggregateProgressDown(mainNode3, ProgressType);
				DBTreeNode.AggregateProgressUp(mainNode3, ProgressType);
				DBTreeNode.AggregateProgressUp(mainNode4, ProgressType);
			}
		}
		else if (CheckWhetherIsTermToObjectDragDrop(draggedObjectType, targetDBNode))
		{
			int? targetId2 = ((targetDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table || targetDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.View || targetDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.Structure) ? new int?(targetDBNode.Id) : null);
			string empty = string.Empty;
			empty = ((draggedRowsFromGrid != null && draggedRowsFromGrid.Count != 1) ? ($"Do you want to add <b>{draggedRowsFromGrid.Count} " + SharedObjectTypeEnum.TypeToStringForSingleLower(draggedObjectType) + "s' data links</b> to <b>" + targetDBNode.DatabaseTitle + "." + targetDBNode.DisplayNameWithShema + "</b>?") : ("Do you want to add link to <b>" + draggedDatabaseTitle + "." + draggedBaseName + "</b> to <b>" + targetDBNode.DatabaseTitle + "." + targetDBNode.DisplayNameWithShema + "</b>?"));
			if (CustomMessageBoxForm.Show(empty, "Add link", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			bool flag = false;
			if (draggedRowsFromGrid == null)
			{
				if (targetId2.HasValue)
				{
					flag = ProcessAddingDataLink(draggedObjectId.Value, targetDBNode.ObjectType, targetId2.Value, null) || flag;
					draggedRowsFromGrid = new List<object> { draggedDBNodeFromTree };
				}
			}
			else
			{
				foreach (TermObject item2 in draggedRowsFromGrid?.Cast<TermObject>()?.ToList())
				{
					if (targetId2.HasValue)
					{
						flag = ProcessAddingDataLink(item2.TermId.Value, targetDBNode.ObjectType, targetId2.Value, null) || flag;
					}
				}
			}
			RefreshDataLinks(draggedObjectId, targetId2);
		}
		else
		{
			if (!IsValidObjectToTermDragDrop(draggedObjectType, targetDBNode))
			{
				return;
			}
			int id = targetDBNode.Id;
			string empty2 = string.Empty;
			empty2 = ((draggedRowsFromGrid != null && draggedRowsFromGrid.Count != 1) ? ($"Do you want to add <b>{draggedRowsFromGrid.Count} " + SharedObjectTypeEnum.TypeToStringForSingleLower(draggedObjectType) + "s</b> to <b>" + targetDBNode.DatabaseTitle + "." + targetDBNode.DisplayNameWithShema + "</b> links?") : ("Do you want to add link to <b>" + draggedDatabaseTitle + "." + draggedBaseName + "</b> to <b>" + targetDBNode.DisplayNameWithShema + "</b>?"));
			if (CustomMessageBoxForm.Show(empty2, "Add link", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			bool flag2 = false;
			if (draggedRowsFromGrid == null)
			{
				if (id != -1)
				{
					flag2 = ProcessAddingDataLink(id, draggedObjectType.Value, draggedObjectId.Value, null) || flag2;
					draggedRowsFromGrid = new List<object> { draggedDBNodeFromTree };
				}
			}
			else if (id != -1)
			{
				if (draggedRowsFromGrid.FirstOrDefault() is ObjectWithModulesObject)
				{
					foreach (ObjectWithModulesObject item3 in draggedRowsFromGrid?.Cast<ObjectWithModulesObject>()?.ToList())
					{
						SharedObjectTypeEnum.ObjectType? dragged = SharedObjectTypeEnum.StringToType(PrepareValue.ToString(item3.ObjectType));
						if (MatchingDragDropTypes.IsValidObjectToTermDragDrop(dragged))
						{
							int id2 = item3.Id;
							flag2 = ProcessAddingDataLink(id, dragged.Value, id2, null) || flag2;
						}
					}
				}
				else if (draggedRowsFromGrid.FirstOrDefault() is ColumnRow)
				{
					foreach (ColumnRow item4 in draggedRowsFromGrid?.Cast<ColumnRow>()?.ToList())
					{
						if (MatchingDragDropTypes.IsValidObjectToTermDragDrop(SharedObjectTypeEnum.ObjectType.Column))
						{
							flag2 = ProcessAddingDataLink(id, SharedObjectTypeEnum.ObjectType.Column, item4.TableId.Value, item4.Id) || flag2;
						}
					}
				}
			}
			if (flag2)
			{
				RefreshDataLinks(draggedObjectId, id);
			}
		}
	}

	private void RefreshDataLinks(int? draggedObjectId, int? targetId)
	{
		BasePanelControl visibleUserControl = GetVisibleUserControl();
		if (visibleUserControl is TermUserControl && (visibleUserControl.ObjectId == draggedObjectId || visibleUserControl.ObjectId == targetId))
		{
			(visibleUserControl as TermUserControl).SetLinksDataIfNotModified();
		}
		else if (visibleUserControl is IBusinessGlossaryObject && (visibleUserControl.ObjectId == draggedObjectId || visibleUserControl.ObjectId == targetId))
		{
			(visibleUserControl as IBusinessGlossaryObject).RefreshDataLinks();
		}
	}

	public void KeyDown(object sender, KeyEventArgs e, MetadataEditorUserControl metadataEditorUserControl, SplashScreenManager splashScreenManager, DBTreeMenu dbTreeMenu, Func<Action, Action, bool> continueAfterPossibleChanges, Func<BasePanelControl> getVisibleUserControl, Action clearDataPanel)
	{
		TreeListNode focusedNode = metadataTreeList.FocusedNode;
		DBTreeNode node = GetNode(focusedNode);
		if (node == null || !node.Available)
		{
			e.Handled = true;
			return;
		}
		switch (e.KeyCode)
		{
		case Keys.Delete:
			DeleteDbElement(metadataEditorUserControl, splashScreenManager, dbTreeMenu, continueAfterPossibleChanges, getVisibleUserControl, clearDataPanel, onlyFromModule: true);
			break;
		case Keys.F2:
			if (node.ObjectType == SharedObjectTypeEnum.ObjectType.Table)
			{
				ChangeNodeNameAndSchema(node);
			}
			else
			{
				StartEditTitle();
			}
			break;
		case Keys.Left:
			if (focusedNode.HasChildren && focusedNode.Expanded)
			{
				focusedNode.Expanded = false;
				e.Handled = true;
			}
			else if (focusedNode.ParentNode != null)
			{
				focusedNode.ParentNode.Selected = true;
			}
			break;
		case Keys.Right:
			if (focusedNode.HasChildren && !focusedNode.Expanded)
			{
				focusedNode.Expanded = true;
				e.Handled = true;
			}
			break;
		}
	}

	public void DragDrop(object sender, DragEventArgs e, Form owner = null)
	{
		TreeListNode focusedTreeListNode = GetFocusedTreeListNode();
		DBTreeNode node = GetNode(focusedTreeListNode);
		TreeList obj = sender as TreeList;
		Point pt = obj.PointToClient(new Point(e.X, e.Y));
		TreeListNode node2 = obj.CalcHitInfo(pt).Node;
		DBTreeNode dBTreeNode = (DBTreeNode)metadataTreeList.GetDataRecordByNode(node2);
		TreeListNode node3 = e.Data.GetData(typeof(TreeListNode)) as TreeListNode;
		DBTreeNode node4 = DBTreeMenu.FindMainObjectsFolder((DBTreeNode)metadataTreeList.GetDataRecordByNode(node3));
		if (dBTreeNode == null)
		{
			LockFocus = false;
			StartedToFocus = false;
			return;
		}
		DBTreeNode dBTreeNode2 = null;
		List<object> list = null;
		object typedDataObject = GetTypedDataObject<ObjectWithModulesObject>(e.Data);
		if (typedDataObject == null)
		{
			if (GetTypedDataObject<TermObject>(e.Data) != null)
			{
				typedDataObject = GetTypedDataObject<TermObject>(e.Data);
			}
			else if (GetTypedDataObject<ColumnRow>(e.Data) != null)
			{
				typedDataObject = GetTypedDataObject<ColumnRow>(e.Data);
			}
		}
		SharedObjectTypeEnum.ObjectType? objectType = null;
		SharedObjectSubtypeEnum.ObjectSubtype? draggedRowSubtype = null;
		int? num = null;
		string text = null;
		int? num2 = null;
		string text2 = null;
		SharedObjectTypeEnum.ObjectType? objectType2 = null;
		SharedObjectTypeEnum.ObjectType? objectType3 = null;
		if (typedDataObject == null)
		{
			TreeListNode node5 = e.Data.GetData(typeof(TreeListNode)) as TreeListNode;
			DBTreeNode dBTreeNode3 = (DBTreeNode)metadataTreeList.GetDataRecordByNode(node5);
			if (dBTreeNode3 != null)
			{
				if (dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database && dBTreeNode3.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
				{
					if (dBTreeNode3.DatabaseId == dBTreeNode.DatabaseId)
					{
						e.Effect = DragDropEffects.None;
						return;
					}
					if (DB.Module.UpdateModuleDatabase(dBTreeNode3.Id, dBTreeNode.DatabaseId, owner))
					{
						MoveDbTreeNodeToDatabase(dBTreeNode3, dBTreeNode, owner);
					}
					e.Effect = DragDropEffects.None;
					LockFocus = false;
					DisableFocusNodeChangedEventActions = false;
					node2.Expand();
					SetFocusedNodeAfterDragDrop(node, node2);
					return;
				}
				if (((dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database && dBTreeNode3.ObjectType == SharedObjectTypeEnum.ObjectType.Table && SharedObjectTypeEnum.StringToTypeForMenu(dBTreeNode.Name) == SharedObjectTypeEnum.ObjectType.Table) || (dBTreeNode3.ObjectType == SharedObjectTypeEnum.ObjectType.View && SharedObjectTypeEnum.StringToTypeForMenu(dBTreeNode.Name) == SharedObjectTypeEnum.ObjectType.View) || (dBTreeNode3.ObjectType == SharedObjectTypeEnum.ObjectType.Structure && SharedObjectTypeEnum.StringToTypeForMenu(dBTreeNode.Name) == SharedObjectTypeEnum.ObjectType.Structure) || (dBTreeNode3.ObjectType == SharedObjectTypeEnum.ObjectType.Procedure && SharedObjectTypeEnum.StringToTypeForMenu(dBTreeNode.Name) == SharedObjectTypeEnum.ObjectType.Procedure) || (dBTreeNode3.ObjectType == SharedObjectTypeEnum.ObjectType.Function && SharedObjectTypeEnum.StringToTypeForMenu(dBTreeNode.Name) == SharedObjectTypeEnum.ObjectType.Function)) && e.Effect == DragDropEffects.Move && dBTreeNode3.DatabaseId != dBTreeNode.DatabaseId)
				{
					if ((dBTreeNode3.ObjectType != SharedObjectTypeEnum.ObjectType.Procedure && dBTreeNode3.ObjectType != SharedObjectTypeEnum.ObjectType.Function) ? DB.Table.UpdateTableDatabase(dBTreeNode3.Id, dBTreeNode.DatabaseId, owner) : DB.Procedure.UpdateProcedureDatabase(dBTreeNode3.Id, dBTreeNode.DatabaseId, owner))
					{
						MoveDbTreeNodeToDatabase(dBTreeNode3, dBTreeNode, owner);
					}
					e.Effect = DragDropEffects.None;
					LockFocus = false;
					DisableFocusNodeChangedEventActions = false;
					node2.Expand();
					SetFocusedNodeAfterDragDrop(node, node2);
					return;
				}
				dBTreeNode2 = new DBTreeNode(dBTreeNode, dBTreeNode3);
				if (dBTreeNode2.ObjectType == SharedObjectTypeEnum.ObjectType.Module && dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
				{
					metadataTreeList.SetNodeIndex(node5, metadataTreeList.GetNodeIndex(node2));
					dBTreeNode2 = null;
					e.Effect = DragDropEffects.None;
					ActualizeModulesOrder(node2, metadataEditorUserControl.ParentForm);
					if (GetVisibleSummaryUserControl() == moduleSummaryUserControl)
					{
						moduleSummaryUserControl.SetParameters();
					}
					LockFocus = false;
					StartedToFocus = false;
					return;
				}
				if (dBTreeNode2 == null || (dBTreeNode2.ObjectType != SharedObjectTypeEnum.ObjectType.Table && dBTreeNode2.ObjectType != SharedObjectTypeEnum.ObjectType.View && dBTreeNode2.ObjectType != SharedObjectTypeEnum.ObjectType.Procedure && dBTreeNode2.ObjectType != SharedObjectTypeEnum.ObjectType.Function && dBTreeNode2.ObjectType != SharedObjectTypeEnum.ObjectType.Structure && dBTreeNode2.ObjectType != SharedObjectTypeEnum.ObjectType.Term))
				{
					dBTreeNode2 = null;
				}
			}
		}
		else
		{
			if (typedDataObject is ObjectWithModulesObject)
			{
				ObjectWithModulesObject objectWithModulesObject = typedDataObject as ObjectWithModulesObject;
				objectType = SharedObjectTypeEnum.StringToType(PrepareValue.ToString(objectWithModulesObject.ObjectType));
				draggedRowSubtype = SharedObjectSubtypeEnum.StringToType(objectType, PrepareValue.ToString(objectWithModulesObject.Subtype));
			}
			else if (typedDataObject is TermObject)
			{
				objectType = SharedObjectTypeEnum.ObjectType.Term;
			}
			else if (typedDataObject is ColumnRow)
			{
				objectType = SharedObjectTypeEnum.ObjectType.Column;
			}
			SharedObjectTypeEnum.ObjectType? objectType4 = metadataEditorUserControl.TreeListHelpers.GetFocusedNode()?.ObjectType;
			switch (objectType)
			{
			case SharedObjectTypeEnum.ObjectType.Table:
				list = tableSummaryUserControl.DragRows.DraggedRows;
				break;
			case SharedObjectTypeEnum.ObjectType.View:
				list = viewSummaryUserControl.DragRows.DraggedRows;
				break;
			case SharedObjectTypeEnum.ObjectType.Structure:
				list = objectSummaryUserControl.DragRows.DraggedRows;
				break;
			case SharedObjectTypeEnum.ObjectType.Procedure:
				list = procedureSummaryUserControl.DragRows.DraggedRows;
				break;
			case SharedObjectTypeEnum.ObjectType.Function:
				list = functionSummaryUserControl.DragRows.DraggedRows;
				break;
			case SharedObjectTypeEnum.ObjectType.Term:
				list = termSummaryUserControl.DragRows.DraggedRows;
				break;
			case SharedObjectTypeEnum.ObjectType.Column:
				if (SharedObjectTypeEnum.IsSupportingBG(objectType4))
				{
					list = tableUserControl.ColumnsDragRows.DraggedRows;
				}
				break;
			}
			for (int i = 0; i < list.Count; i++)
			{
				object obj2 = list[i];
				if (obj2 is ObjectWithModulesObject)
				{
					ObjectWithModulesObject objectWithModulesObject2 = obj2 as ObjectWithModulesObject;
					SharedObjectTypeEnum.ObjectType? objectType5 = SharedObjectTypeEnum.StringToType(objectWithModulesObject2.ObjectType);
					if (objectType5.HasValue && !CheckIfParentNotContainChild(dBTreeNode, objectType5.Value, objectWithModulesObject2.Id))
					{
						list.RemoveAt(i);
						i--;
					}
				}
				else if (obj2 is TermObject)
				{
					if (dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary || dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Term)
					{
						if (!CheckWhetherIsValidTermDragDrop((obj2 as IBasicData).Id.Value, SharedObjectTypeEnum.ObjectType.Term, dBTreeNode))
						{
							list.RemoveAt(i);
							i--;
						}
					}
					else if ((dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table || dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.View) && !CheckWhetherIsTermToObjectDragDrop(SharedObjectTypeEnum.ObjectType.Term, dBTreeNode))
					{
						list.RemoveAt(i);
						i--;
					}
				}
				else if (obj2 is ColumnRow && dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Term && !IsValidObjectToTermDragDrop(SharedObjectTypeEnum.ObjectType.Column, dBTreeNode))
				{
					list.RemoveAt(i);
					i--;
				}
			}
			if (list.Count > 0)
			{
				DBTreeNode dBTreeNode5;
				if (list[0] is ObjectWithModulesObject)
				{
					dBTreeNode2 = new DBTreeNode(dBTreeNode, list[0] as ObjectWithModulesObject, objectType.Value, draggedRowSubtype.Value);
				}
				else if (list[0] is TermObject)
				{
					TermObject obj3 = list[0] as TermObject;
					DBTreeNode dBTreeNode4 = new DBTreeNode(dBTreeNode, list[0] as TermObject, objectType.Value);
					num = obj3.DatabaseId;
					text = obj3.DatabaseTitle;
					num2 = obj3.TermId;
					text2 = dBTreeNode4.BaseName;
					objectType2 = SharedObjectTypeEnum.ObjectType.Term;
				}
				else if (list[0] is ColumnRow)
				{
					ColumnRow columnRow = list[0] as ColumnRow;
					dBTreeNode5 = DBTreeMenu.FindDBNodeById(columnRow.ParentObjectType.Value, columnRow.TableId.Value);
					num = columnRow.DocumentationId;
					text = columnRow.DocumentationTitle;
					num2 = columnRow.TableId.Value;
					text2 = dBTreeNode5.BaseName + "." + columnRow.Name;
					objectType2 = columnRow.ParentObjectType;
					_ = columnRow.Id;
					objectType3 = SharedObjectTypeEnum.ObjectType.Column;
				}
				dBTreeNode5 = DBTreeMenu.FindDBNodeById(objectType2 ?? objectType3 ?? dBTreeNode2.ObjectType, num2 ?? dBTreeNode2.Id);
				if (dBTreeNode2 != null)
				{
					dBTreeNode2.Points = dBTreeNode5.Points;
					dBTreeNode2.TotalPoints = dBTreeNode5.TotalPoints;
					dBTreeNode2.PointsSum = dBTreeNode5.PointsSum;
					dBTreeNode2.TotalPointsSum = dBTreeNode5.TotalPointsSum;
					dBTreeNode2.ProgressValue = dBTreeNode5.ProgressValue;
				}
			}
		}
		if (dBTreeNode2 != null || list != null)
		{
			if ((dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module && SharedObjectTypeEnum.StringToTypeForMenu(dBTreeNode.Name) == dBTreeNode2?.ObjectType) || dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
			{
				DragDropToModule(dBTreeNode, dBTreeNode2, list?.Cast<ObjectWithModulesObject>()?.ToList(), objectType, draggedRowSubtype, owner);
				metadataTreeList.RefreshNode(node2);
			}
			else
			{
				DragDropTerms(node2, dBTreeNode, dBTreeNode2, list, num ?? dBTreeNode2?.DatabaseId, text ?? dBTreeNode2?.DatabaseTitle, num2 ?? dBTreeNode2.Id, text2 ?? dBTreeNode2.BaseName, objectType3 ?? objectType2 ?? dBTreeNode2?.ObjectType, owner);
			}
		}
		e.Effect = DragDropEffects.None;
		LockFocus = false;
		DisableFocusNodeChangedEventActions = false;
		GetVisibleSummaryUserControl()?.SetParameters();
		DBTreeMenu.RefreshFolderInDataBase(node4);
		DBTreeMenu.RefreshFolderInDataBase(DBTreeMenu.FindMainObjectsFolder(dBTreeNode));
		SetFocusedNodeAfterDragDrop(node, node2);
	}

	private void SetFocusedNodeAfterDragDrop(DBTreeNode focusedNode, TreeListNode targetTreeNode)
	{
		if (focusedNode == null || targetTreeNode == null || GetNode(GetFocusedTreeListNode()) != null)
		{
			return;
		}
		try
		{
			DisableFocusEvents = true;
			TreeListNode focusedNode2 = metadataEditorUserControl.SearchTreeNodeOperation.FindNode(targetTreeNode.Nodes, focusedNode.Id, focusedNode.ObjectType);
			metadataTreeList.FocusedNode = focusedNode2;
		}
		finally
		{
			DisableFocusEvents = false;
		}
	}

	private void MoveDbTreeNodeToDatabase(DBTreeNode draggedDBNodeOriginal, DBTreeNode targetDBNode, Form owner = null)
	{
		if (draggedDBNodeOriginal == null || targetDBNode == null || draggedDBNodeOriginal.DatabaseId == targetDBNode.DatabaseId || (draggedDBNodeOriginal.ObjectType != SharedObjectTypeEnum.ObjectType.Module && draggedDBNodeOriginal.Source != UserTypeEnum.UserType.USER))
		{
			return;
		}
		List<DBTreeNode> list = draggedDBNodeOriginal.Nodes.Flatten((DBTreeNode x) => x.Nodes).ToList();
		metadataTreeList.BeginUpdate();
		draggedDBNodeOriginal.ParentNode.Nodes.Remove(draggedDBNodeOriginal);
		metadataTreeList.EndDelete();
		foreach (DBTreeNode subnode2 in list)
		{
			foreach (DBTreeNode subnodeNode in list.Where((DBTreeNode x) => x.ParentNode?.Id == subnode2.Id && x.ParentNode?.Name == subnode2.Name))
			{
				if (!subnode2.Nodes.Any((DBTreeNode n) => n == subnodeNode))
				{
					subnode2.Nodes.Add(subnodeNode);
				}
			}
		}
		foreach (DBTreeNode subnode in list.Where((DBTreeNode x) => x.ParentNode?.Id == draggedDBNodeOriginal.Id && x.ParentNode?.Name == draggedDBNodeOriginal.Name))
		{
			if (!draggedDBNodeOriginal.Nodes.Any((DBTreeNode n) => n == subnode))
			{
				draggedDBNodeOriginal.Nodes.Add(subnode);
			}
		}
		DBTreeMenu.RefreshFolderInDataBase(draggedDBNodeOriginal.ParentNode);
		draggedDBNodeOriginal.ParentNode = targetDBNode;
		targetDBNode.Nodes.Add(draggedDBNodeOriginal);
		DBTreeMenu.RefreshFolderInDataBase(targetDBNode);
		draggedDBNodeOriginal.DatabaseId = targetDBNode.DatabaseId;
		RefreshNormalObjectsNames(targetDBNode);
		metadataTreeList.EndUpdate();
		metadataEditorUserControl.ReloadUserControlAfterMovingNode(draggedDBNodeOriginal);
	}

	private void ProcessChangingTermParent(int termId, int? targetId, DBTreeNode targetDBNode, TreeListNode targetTreeNode, Form owner = null)
	{
		DBTreeNode nodeInTree = DBTreeMenu.FindDBNodeById(SharedObjectTypeEnum.ObjectType.Term, termId);
		if (!DB.BusinessGlossary.ChangeTermParent(termId, targetDBNode.DatabaseId, targetId, owner))
		{
			return;
		}
		List<DBTreeNode> list = nodeInTree.Nodes.Flatten((DBTreeNode x) => x.Nodes).ToList();
		metadataTreeList.BeginUpdate();
		nodeInTree.ParentNode.Nodes.Remove(nodeInTree);
		metadataTreeList.EndDelete();
		foreach (DBTreeNode subnode in list)
		{
			foreach (DBTreeNode item in list.Where((DBTreeNode x) => x.ParentNode?.Id == subnode.Id))
			{
				if (!subnode.Nodes.Contains(item))
				{
					subnode.Nodes.Add(item);
				}
			}
		}
		foreach (DBTreeNode item2 in list.Where((DBTreeNode x) => x.ParentNode?.Id == nodeInTree.Id))
		{
			if (!nodeInTree.Nodes.Contains(item2))
			{
				nodeInTree.Nodes.Add(item2);
			}
		}
		nodeInTree.ParentNode = targetDBNode;
		targetDBNode.Nodes.Add(nodeInTree);
		metadataTreeList.EndUpdate();
		targetTreeNode.Expanded = true;
		if (nodeInTree.DatabaseId != targetDBNode.DatabaseId)
		{
			ChangeSubtermsDatabase(termId, targetDBNode.DatabaseId, nodeInTree, owner);
		}
		if (nodeInTree.DatabaseId != targetDBNode.DatabaseId && !StaticData.IsProjectFile)
		{
			List<int> result = new List<int>();
			int[] termIDs = GetIDs(result, termId)?.ToArray();
			BaseFeedbackWidgetDataObject movedTermsFeedbackData = DB.BusinessGlossary.GetMovedTermsFeedbackData(termIDs);
			DB.BusinessGlossary.IncreaseBGFeedbackData(targetDBNode.DatabaseId, movedTermsFeedbackData);
			DB.BusinessGlossary.DecreaseBGFeedbackData(nodeInTree.DatabaseId, movedTermsFeedbackData);
		}
		nodeInTree.DatabaseId = targetDBNode.DatabaseId;
	}

	private IList<int> GetIDs(IList<int> result, int termId)
	{
		result.Add(termId);
		foreach (DBTreeNode item in DBTreeMenu.FindDBNodeById(SharedObjectTypeEnum.ObjectType.Term, termId).Nodes.Flatten((DBTreeNode x) => x.Nodes).ToList())
		{
			GetIDs(result, item.Id);
		}
		return result.Distinct().ToList();
	}

	private void ChangeSubtermsDatabase(int parentId, int databaseId, DBTreeNode nodeToChangeParent, Form owner = null)
	{
		if (!DB.BusinessGlossary.ChangeTermsParent(parentId, databaseId, parentId))
		{
			return;
		}
		foreach (DBTreeNode node in nodeToChangeParent.Nodes)
		{
			node.DatabaseId = databaseId;
			ChangeSubtermsDatabase(node.Id, databaseId, node, owner);
		}
	}

	private bool ProcessAddingDataLink(int termId, SharedObjectTypeEnum.ObjectType objectType, int objectId, int? elementId)
	{
		return DB.BusinessGlossary.InsertDataLink(new DataLinkObjectBase(termId, SharedObjectTypeEnum.TypeToString(objectType), objectId, elementId));
	}

	public void DragDropFromTreeList(object sender, DragEventArgs e)
	{
	}

	private void DragDropToModule(DBTreeNode targetDBNode, DBTreeNode draggedDBNodeFromTree, List<ObjectWithModulesObject> draggedRowsFromGrid, SharedObjectTypeEnum.ObjectType? draggedRowType, SharedObjectSubtypeEnum.ObjectSubtype? draggedRowSubtype, Form owner = null)
	{
		DBTreeNode dBTreeNode = null;
		if (targetDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			dBTreeNode = targetDBNode;
			targetDBNode = targetDBNode.GetDirectoryNode(draggedDBNodeFromTree.ObjectType);
			if (targetDBNode == null)
			{
				targetDBNode = DBTreeMenu.PrepareObjectFolder(dBTreeNode, draggedDBNodeFromTree.ObjectType, addFolderToParentNode: true);
			}
			if (draggedDBNodeFromTree == null || targetDBNode.Nodes.Any((DBTreeNode x) => x.Id == draggedDBNodeFromTree.Id))
			{
				LockFocus = false;
				StartedToFocus = false;
				return;
			}
			draggedDBNodeFromTree.ParentNode = targetDBNode;
		}
		else
		{
			dBTreeNode = targetDBNode.ParentNode;
		}
		metadataTreeList.BeginUpdate();
		List<DBTreeNode> newNodes = new List<DBTreeNode>();
		if (draggedRowsFromGrid == null || draggedRowsFromGrid.Count == 1)
		{
			newNodes.Add(draggedDBNodeFromTree);
		}
		else
		{
			draggedRowsFromGrid.ForEach(delegate(ObjectWithModulesObject row)
			{
				DBTreeNode dBTreeNode2 = new DBTreeNode(targetDBNode, row, draggedRowType.Value, draggedRowSubtype.Value);
				dBTreeNode2.AdjustName(row);
				DBTreeNode dBTreeNode3 = DBTreeMenu.FindDBNodeById(dBTreeNode2.ObjectType, dBTreeNode2.Id);
				dBTreeNode2.Points = dBTreeNode3.Points;
				dBTreeNode2.TotalPoints = dBTreeNode3.TotalPoints;
				dBTreeNode2.PointsSum = dBTreeNode3.PointsSum;
				dBTreeNode2.TotalPointsSum = dBTreeNode3.PointsSum;
				dBTreeNode2.ProgressValue = dBTreeNode3.ProgressValue;
				newNodes.Add(dBTreeNode2);
			});
		}
		if (DB.Module.AddLinks(dBTreeNode.Id, newNodes, out var relationsSucceedTablesIdsArray, owner))
		{
			newNodes.ForEach(delegate(DBTreeNode node)
			{
				if (relationsSucceedTablesIdsArray.Where((int x) => PrepareValue.ToInt(x) == node.Id).Count() > 0)
				{
					targetDBNode.Nodes.Add(node);
					DBTreeMenu.RefreshModuleFolderInDataBase(targetDBNode);
				}
			});
			if (ShowProgress)
			{
				DBTreeNode.AggregateProgressUp(targetDBNode, ProgressType);
			}
		}
		metadataTreeList.EndUpdate();
	}

	public void DragEnter(object sender, DragEventArgs e)
	{
		DBTreeNode dBTreeNode = GetDBTreeNode(e.Data);
		DBTreeNode dBTreeNode2 = GetDBTreeNode(e);
		if (dBTreeNode != null)
		{
			SetDragDropEffect(dBTreeNode, dBTreeNode2, e);
		}
		else if (GetTypedDataObject<ObjectWithModulesObject>(e.Data) != null)
		{
			if (CheckIfDraggingAllowed(e.Data, dBTreeNode2))
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}
		else if (GetTypedDataObject<ColumnRow>(e.Data) != null)
		{
			ColumnRow typedDataObject = GetTypedDataObject<ColumnRow>(e.Data);
			if (typedDataObject != null)
			{
				SetDragDropEffect(typedDataObject, dBTreeNode2, e);
			}
		}
		else if (GetTypedDataObject<TermObject>(e.Data) != null)
		{
			TermObject typedDataObject2 = GetTypedDataObject<TermObject>(e.Data);
			if (typedDataObject2 != null)
			{
				SetDragDropEffect(typedDataObject2, dBTreeNode2, e);
			}
		}
		DisableFocusNodeChangedEventActions = true;
		LockFocus = true;
		StartedToFocus = false;
	}

	public void DragOver(object sender, DragEventArgs e)
	{
		DBTreeNode dBTreeNode = GetDBTreeNode(e.Data);
		DBTreeNode dBTreeNode2 = GetDBTreeNode(e);
		if (dBTreeNode != null)
		{
			if (!CheckIfDraggingAllowed(e.Data, dBTreeNode2))
			{
				e.Effect = DragDropEffects.None;
			}
			else
			{
				SetDragDropEffect(dBTreeNode, dBTreeNode2, e);
			}
		}
		else if (GetTypedDataObject<ObjectWithModulesObject>(e.Data) != null)
		{
			if (CheckIfDraggingAllowed(e.Data, dBTreeNode2))
			{
				if (dBTreeNode2.ObjectType == SharedObjectTypeEnum.ObjectType.Term)
				{
					e.Effect = DragDropEffects.Link;
				}
				else
				{
					e.Effect = DragDropEffects.Copy;
				}
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}
		else if (GetTypedDataObject<ColumnRow>(e.Data) != null)
		{
			ColumnRow typedDataObject = GetTypedDataObject<ColumnRow>(e.Data);
			if (typedDataObject != null)
			{
				if (!CheckIfDraggingAllowed(typedDataObject, dBTreeNode2))
				{
					e.Effect = DragDropEffects.None;
				}
				else
				{
					SetDragDropEffect(typedDataObject, dBTreeNode2, e);
				}
			}
		}
		else
		{
			if (GetTypedDataObject<TermObject>(e.Data) == null)
			{
				return;
			}
			TermObject typedDataObject2 = GetTypedDataObject<TermObject>(e.Data);
			if (typedDataObject2 != null)
			{
				if (!CheckIfDraggingAllowed(e.Data, dBTreeNode2))
				{
					e.Effect = DragDropEffects.None;
				}
				else
				{
					SetDragDropEffect(typedDataObject2, dBTreeNode2, e);
				}
			}
		}
	}

	public void DragLeave(object sender, EventArgs e)
	{
		DisableFocusNodeChangedEventActions = false;
		LockFocus = false;
		StartedToFocus = false;
	}

	public void NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		DBTreeNode node = GetNode(e.Node);
		if (node != null && node.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted)
		{
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.DeletedObjectForeColor;
		}
		if (e.Node == CustomFocusedTreeListNode && metadataTreeList.FocusedNode != e.Node)
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.TreeListCustomFocusBackColor;
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.TreeListCustomFocusForeColor;
		}
	}

	public void BeforeFocusNode(object sender, BeforeFocusNodeEventArgs e, Func<bool> continueAfterPossibleChanges, Form owner = null)
	{
		bool flag = true;
		try
		{
			if (e.Node == null)
			{
				e.CanFocus = false;
				StartedToFocus = false;
				return;
			}
			if (e.Node != null && !LockFocus)
			{
				flag = continueAfterPossibleChanges();
				e.CanFocus = flag && !LockFocus;
			}
			else if (LockFocus)
			{
				e.CanFocus = false;
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error when loading data for selected object.", owner);
		}
		if (flag)
		{
			StartedToFocus = true;
		}
		else
		{
			StartedToFocus = false;
		}
	}

	public void BeforeCollapse(object sender, BeforeCollapseEventArgs e, Func<bool> continueAfterPossibleChanges)
	{
		metadataEditorUserControl.HasVisibleControlChanges();
		TreeListNode treeListNode = metadataTreeList.FocusedNode;
		while (treeListNode?.ParentNode != null)
		{
			if (treeListNode.ParentNode == e.Node)
			{
				e.CanCollapse = continueAfterPossibleChanges();
				if (e.CanCollapse)
				{
					bool lockFocus = LockFocus;
					try
					{
						LockFocus = false;
						metadataTreeList.FocusedNode = treeListNode.ParentNode;
						break;
					}
					finally
					{
						LockFocus = lockFocus;
					}
				}
				break;
			}
			treeListNode = treeListNode.ParentNode;
		}
	}

	public void FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e, Action<TreeListNode> processFocusNode)
	{
		if (!LockFocus)
		{
			processFocusNode(e.Node);
		}
	}

	public void MouseDown(object sender, MouseEventArgs e, MetadataEditorUserControl metadataEditorUserControl, Point mousePosition, PopupMenu treePopupMenu, BarButtonItem refreshButtonItem, BarSubItem addObjectBarButtonItem, BarButtonItem addImportStructureBarButtonItem, BarButtonItem addTableBarButtonItem, BarSubItem bulkAddObjectsBarButtonItem, BarButtonItem bulkAddTableBarButtonItem, BarButtonItem bulkAddStructureBarButtonItem, BarButtonItem addModuleBarButtonItem, BarButtonItem moveUpBarButtonItem, BarButtonItem moveDownBarButtonItem, BarButtonItem renameBarButtonItem, BarButtonItem synchronizeSchemaBarButtonItem, BarButtonItem importDescriptionsBarButtonItem, BarButtonItem generateDocumentationBarButtonItem, BarButtonItem designTableBarButtonItem, BarButtonItem profileTableBarButtonItem, BarButtonItem dataProfilingBarButtonItem, BarButtonItem previewSampleDataBarButtonItem, BarButtonItem addTermBarButtonItem, BarButtonItem deleteBarButtonItem, BarButtonItem addTermRelatedTermBarButtonItem, BarButtonItem addDataLinkBarButtonItem, BarButtonItem addNewTermBarButtonItem, BarButtonItem expandAllBarButtonItem, BarButtonItem collapseAllBarButtonItem, BarButtonItem copyDescriptionsBarButtonItem, BarSubItem addBarButtonItem, BarButtonItem addBusinessGlossaryBarButtonItem, BarSubItem displayOptionsSchemaBarSubItem, BarButtonItem alwaysDisplaySchemaBarButtonItem, BarButtonItem neverDisplaySchemaBarButtonItem, BarButtonItem defaultDisplaySchemaBarButtonItem, BarSubItem connectToExistingBarButtonItem, BarButtonItem connectToBarButtonItem, BarButtonItem createNewRepositoryBarButtonItem, BarButtonItem copyConnectionBarButtonItem, BarButtonItem addViewButtonItem, BarButtonItem duplicateModuleButtonItem, BarButtonItem bulkAddViewBarButtonItem, BarButtonItem addProcedureButtonItem, BarButtonItem designProcedureBarButtonItem, BarButtonItem addFunctionBarButtonItem, BarButtonItem designFunctionBarButtonItem, BarButtonItem clearTableProfilingBarButtonItem, BarButtonItem sortModulesAlphabeticallyBarButtonItem, BarButtonItem moveToTopBarButtonItem, BarButtonItem moveToBottomBarButtonItem)
	{
		LockFocus = true;
		MouseDownPoint = e.Location;
		if (e.Button != MouseButtons.Right)
		{
			return;
		}
		bool flag = false;
		TreeListHitInfo treeListHitInfo = (sender as TreeList).CalcHitInfo(new Point(e.X, e.Y));
		if (treeListHitInfo.HitInfoType == HitInfoType.Empty)
		{
			flag = true;
		}
		else if (treeListHitInfo.HitInfoType != HitInfoType.Cell)
		{
			return;
		}
		metadataTreeList.CloseEditor();
		TreeListNode node = treeListHitInfo.Node;
		DBTreeNode node2 = GetNode(node);
		if (!flag && (node2 == null || !node2.Available))
		{
			return;
		}
		treePopupMenu.ClearLinks();
		displayOptionsSchemaBarSubItem.ClearLinks();
		addObjectBarButtonItem.ClearLinks();
		if (node2 != null && node2.ObjectType == SharedObjectTypeEnum.ObjectType.Repository)
		{
			treePopupMenu.AddItem(connectToBarButtonItem);
			SetRecentDropDownMenu(treePopupMenu, connectToExistingBarButtonItem);
			if (connectToExistingBarButtonItem.ItemLinks.Count > 0)
			{
				treePopupMenu.AddItem(connectToExistingBarButtonItem);
			}
			treePopupMenu.AddItem(createNewRepositoryBarButtonItem);
		}
		if ((node2 != null && node2.ObjectType == SharedObjectTypeEnum.ObjectType.Database) || (node2 != null && node2.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary) || flag || (node2 != null && node2.ObjectType == SharedObjectTypeEnum.ObjectType.Repository))
		{
			BarItemLink barItemLink = treePopupMenu.AddItem(addBarButtonItem);
			if (node2 != null && node2.ObjectType == SharedObjectTypeEnum.ObjectType.Repository)
			{
				barItemLink.BeginGroup = true;
			}
		}
		addBusinessGlossaryBarButtonItem.Caption = "Business Glossary";
		addBusinessGlossaryBarButtonItem.Enabled = true;
		if (flag)
		{
			treePopupMenu.ShowPopup(mousePosition);
			return;
		}
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = false;
		bool flag8 = SharedObjectTypeEnum.IsFromDatabase(node2.ObjectType);
		if (node2.ObjectType == SharedObjectTypeEnum.ObjectType.Database)
		{
			ParentForAddModule = new SearchTreeNodeOperation().FindModuleDirectory(metadataTreeList.Nodes.FirstNode.Nodes, node2.DatabaseId);
		}
		else if (node2.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			ParentForAddModule = node.ParentNode;
			flag2 = true;
		}
		else if (node2.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Module))
		{
			ParentForAddModule = node;
			flag3 = true;
		}
		else if (!(node2.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Table)))
		{
			if (node2.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.View))
			{
				flag4 = true;
			}
			else if (node2.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Procedure))
			{
				flag5 = true;
			}
			else if (node2.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Function))
			{
				flag6 = true;
			}
		}
		if (node2.IsFolder || node2.Nodes.Count > 0)
		{
			flag7 = true;
		}
		if (!(flag2 || flag3 || flag7 || flag8 || flag4 || flag5 || flag6))
		{
			return;
		}
		metadataTreeList.FocusedNode = node;
		if (node2.Id != 0 && (node2.ObjectType == SharedObjectTypeEnum.ObjectType.Table || node2.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Table) || node2.ObjectType == SharedObjectTypeEnum.ObjectType.Structure || node2.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Structure) || node2.ObjectType == SharedObjectTypeEnum.ObjectType.Database))
		{
			if (node2.ObjectType != SharedObjectTypeEnum.ObjectType.Table && !(node2.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Table)) && (node2.DatabaseClass != SharedDatabaseClassEnum.DatabaseClass.Database || node2.ContainedObjectsObjectType == SharedObjectTypeEnum.ObjectType.Structure) && node2.ObjectType != SharedObjectTypeEnum.ObjectType.Structure && !(node2.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Structure)) && (node2.DatabaseClass != SharedDatabaseClassEnum.DatabaseClass.DataLake || node2.ContainedObjectsObjectType == SharedObjectTypeEnum.ObjectType.Table))
			{
				return;
			}
			if (node2.ObjectType == SharedObjectTypeEnum.ObjectType.Database)
			{
				bulkAddObjectsBarButtonItem.ClearLinks();
				if (node2.DatabaseType != SharedDatabaseTypeEnum.DatabaseType.Manual)
				{
					treePopupMenu.AddItem(copyConnectionBarButtonItem);
					if (!DB.DataProfiling.IsDataProfilingDisabled())
					{
						treePopupMenu.AddItem(dataProfilingBarButtonItem);
					}
				}
				if (SharedDatabaseClassEnum.MayContainTableField(node2.DatabaseClass))
				{
					addObjectBarButtonItem.AddItem(addTableBarButtonItem);
					bulkAddObjectsBarButtonItem.AddItem(bulkAddTableBarButtonItem);
				}
				if (SharedDatabaseClassEnum.MayContainStructure(node2.DatabaseClass))
				{
					addObjectBarButtonItem.AddItem(addImportStructureBarButtonItem);
					bulkAddObjectsBarButtonItem.AddItem(bulkAddStructureBarButtonItem);
				}
				if (SharedDatabaseClassEnum.MayContainView(node2.DatabaseClass))
				{
					addObjectBarButtonItem.AddItem(addViewButtonItem);
					bulkAddObjectsBarButtonItem.AddItem(bulkAddViewBarButtonItem);
				}
				if (SharedDatabaseClassEnum.MayContainProcedure(node2.DatabaseClass))
				{
					addObjectBarButtonItem.AddItem(addProcedureButtonItem);
				}
				if (SharedDatabaseClassEnum.MayContainFunction(node2.DatabaseClass))
				{
					addObjectBarButtonItem.AddItem(addFunctionBarButtonItem);
				}
				if (bulkAddObjectsBarButtonItem.ItemLinks.Count > 0)
				{
					addObjectBarButtonItem.AddItem(bulkAddObjectsBarButtonItem);
				}
			}
			else if (node2.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database || node2.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module)
			{
				if (node2.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Table))
				{
					treePopupMenu.AddItem(addTableBarButtonItem);
					treePopupMenu.AddItem(bulkAddTableBarButtonItem);
					if (!DB.DataProfiling.IsDataProfilingDisabled())
					{
						treePopupMenu.AddItem(dataProfilingBarButtonItem);
					}
				}
				else if (node2.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Structure))
				{
					treePopupMenu.AddItem(addImportStructureBarButtonItem);
					treePopupMenu.AddItem(bulkAddStructureBarButtonItem);
				}
			}
			else if (node2.ObjectType == SharedObjectTypeEnum.ObjectType.Table)
			{
				treePopupMenu.AddItem(addTableBarButtonItem);
				treePopupMenu.AddItem(bulkAddTableBarButtonItem);
			}
			else if (node2.ObjectType == SharedObjectTypeEnum.ObjectType.Structure)
			{
				treePopupMenu.AddItem(addImportStructureBarButtonItem);
				treePopupMenu.AddItem(bulkAddStructureBarButtonItem);
			}
		}
		if (flag4 || node2.ObjectType == SharedObjectTypeEnum.ObjectType.View)
		{
			treePopupMenu.AddItem(addViewButtonItem);
			treePopupMenu.AddItem(bulkAddViewBarButtonItem);
		}
		if (flag4 && !DB.DataProfiling.IsDataProfilingDisabled())
		{
			treePopupMenu.AddItem(dataProfilingBarButtonItem);
		}
		if (flag2)
		{
			treePopupMenu.AddItem(addModuleBarButtonItem);
			treePopupMenu.AddItem(duplicateModuleButtonItem);
			treePopupMenu.AddItem(moveUpBarButtonItem);
			treePopupMenu.AddItem(moveDownBarButtonItem);
			treePopupMenu.AddItem(moveToTopBarButtonItem);
			treePopupMenu.AddItem(moveToBottomBarButtonItem);
			treePopupMenu.AddItem(sortModulesAlphabeticallyBarButtonItem);
			treePopupMenu.AddItem(renameBarButtonItem);
		}
		else if (flag3)
		{
			treePopupMenu.AddItem(addModuleBarButtonItem);
			treePopupMenu.AddItem(sortModulesAlphabeticallyBarButtonItem);
		}
		if (node2.ObjectType == SharedObjectTypeEnum.ObjectType.Repository)
		{
			treePopupMenu.AddItem(generateDocumentationBarButtonItem);
		}
		else if (node2.ObjectType == SharedObjectTypeEnum.ObjectType.Database)
		{
			treePopupMenu.AddItem(addModuleBarButtonItem);
			if (addObjectBarButtonItem.ItemLinks.Count > 0)
			{
				treePopupMenu.AddItem(addObjectBarButtonItem);
			}
			if (!node2.IsWelcomeDocumentation)
			{
				synchronizeSchemaBarButtonItem.Caption = SharedDatabaseTypeEnum.SynchronizeText(node2.DatabaseType);
				if (!string.IsNullOrEmpty(synchronizeSchemaBarButtonItem.Caption))
				{
					treePopupMenu.AddItem(synchronizeSchemaBarButtonItem);
				}
				treePopupMenu.AddItem(importDescriptionsBarButtonItem);
				treePopupMenu.AddItem(copyDescriptionsBarButtonItem);
				treePopupMenu.AddItem(generateDocumentationBarButtonItem);
			}
			treePopupMenu.AddItem(displayOptionsSchemaBarSubItem);
			displayOptionsSchemaBarSubItem.AddItem(alwaysDisplaySchemaBarButtonItem);
			displayOptionsSchemaBarSubItem.AddItem(neverDisplaySchemaBarButtonItem);
			displayOptionsSchemaBarSubItem.AddItem(defaultDisplaySchemaBarButtonItem);
			if (treeListHitInfo.Node == metadataTreeList.FocusedNode)
			{
				treePopupMenu.AddItem(renameBarButtonItem);
			}
			copyDescriptionsBarButtonItem.Caption = "Copy descriptions (beta)";
			copyDescriptionsBarButtonItem.Enabled = true;
			copyDescriptionsBarButtonItem.Visibility = BarItemVisibility.Always;
			if (StaticData.IsProjectFile)
			{
				copyDescriptionsBarButtonItem.Visibility = BarItemVisibility.Never;
			}
		}
		else if (node2.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
		{
			if (treeListHitInfo.Node == metadataTreeList.FocusedNode)
			{
				treePopupMenu.AddItem(renameBarButtonItem);
			}
		}
		else if (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary) && node2.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database && node2 != null && node2.ParentNode?.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
		{
			AddAddTermPopupMenu(treePopupMenu, addTermBarButtonItem);
		}
		else if (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary) && node2.ObjectType == SharedObjectTypeEnum.ObjectType.Term)
		{
			AddAddTermPopupMenu(treePopupMenu, addTermBarButtonItem);
			treePopupMenu.AddItem(addTermRelatedTermBarButtonItem);
		}
		if (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary) && (node2.ObjectType == SharedObjectTypeEnum.ObjectType.Table || node2.ObjectType == SharedObjectTypeEnum.ObjectType.View))
		{
			AddAddNewTermPopupMenu(addObjectBarButtonItem, addNewTermBarButtonItem);
		}
		if (addObjectBarButtonItem.ItemLinks.Count > 0 && node2.ObjectType != SharedObjectTypeEnum.ObjectType.Database)
		{
			treePopupMenu.AddItem(addObjectBarButtonItem);
		}
		if (node2.ObjectType == SharedObjectTypeEnum.ObjectType.Term)
		{
			addDataLinkBarButtonItem.Caption = "Edit links to Data Dictionary elements";
		}
		else
		{
			addDataLinkBarButtonItem.Caption = "Edit links to Business Glossary terms";
		}
		if (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary) && (node2.ObjectType == SharedObjectTypeEnum.ObjectType.Term || node2.ObjectType == SharedObjectTypeEnum.ObjectType.Table || node2.ObjectType == SharedObjectTypeEnum.ObjectType.View || node2.ObjectType == SharedObjectTypeEnum.ObjectType.Structure))
		{
			treePopupMenu.AddItem(addDataLinkBarButtonItem);
			if (node2.ObjectType == SharedObjectTypeEnum.ObjectType.Term && treeListHitInfo.Node == metadataTreeList.FocusedNode)
			{
				treePopupMenu.AddItem(renameBarButtonItem);
			}
		}
		if ((node2.Id != 0 && flag8 && (node2.ObjectType == SharedObjectTypeEnum.ObjectType.Table || node2.ObjectType == SharedObjectTypeEnum.ObjectType.Structure)) || node2.ObjectType == SharedObjectTypeEnum.ObjectType.View)
		{
			string text = SharedObjectTypeEnum.TypeToStringForSingle(node2.ObjectType);
			designTableBarButtonItem.Enabled = true;
			designTableBarButtonItem.Caption = "Design " + text;
			designTableBarButtonItem.Hint = string.Empty;
			designTableBarButtonItem.Glyph = IconsForButtonsFinder.ReturnImageForDesignButtonItem16(node2.ObjectType);
			treePopupMenu.AddItem(designTableBarButtonItem);
			if (DataProfilingUtils.NodeCanBeProfilled(node2) && !DB.DataProfiling.IsDataProfilingDisabled())
			{
				profileTableBarButtonItem.Caption = DataProfilingUtils.GetButtonNameByObjectType(node2.ObjectType);
				treePopupMenu.AddItem(profileTableBarButtonItem);
				treePopupMenu.AddItem(previewSampleDataBarButtonItem);
				if (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
				{
					treePopupMenu.AddItem(clearTableProfilingBarButtonItem);
				}
			}
		}
		if ((node2.ObjectType == SharedObjectTypeEnum.ObjectType.Table || node2.ObjectType == SharedObjectTypeEnum.ObjectType.Structure) && node2.Source == UserTypeEnum.UserType.USER && treeListHitInfo.Node == metadataTreeList.FocusedNode)
		{
			treePopupMenu.AddItem(renameBarButtonItem);
		}
		if (flag5 || node2.ObjectType == SharedObjectTypeEnum.ObjectType.Procedure)
		{
			treePopupMenu.AddItem(addProcedureButtonItem);
			if (node2.Id != 0 && flag8)
			{
				designProcedureBarButtonItem.Glyph = IconsForButtonsFinder.ReturnImageForDesignButtonItem16(node2.ObjectType);
				treePopupMenu.AddItem(designProcedureBarButtonItem);
			}
		}
		if (flag6 || node2.ObjectType == SharedObjectTypeEnum.ObjectType.Function)
		{
			treePopupMenu.AddItem(addFunctionBarButtonItem);
			if (node2.Id != 0 && flag8)
			{
				designFunctionBarButtonItem.Glyph = IconsForButtonsFinder.ReturnImageForDesignButtonItem16(node2.ObjectType);
				treePopupMenu.AddItem(designFunctionBarButtonItem);
			}
		}
		if (flag8)
		{
			deleteBarButtonItem.Caption = (node2.IsInModule ? "Remove from Subject Area" : "Remove from repository");
			treePopupMenu.AddItem(deleteBarButtonItem);
		}
		if (flag7)
		{
			treePopupMenu.AddItem(expandAllBarButtonItem).BeginGroup = true;
			treePopupMenu.AddItem(collapseAllBarButtonItem);
			treePopupMenu.AddItem(refreshButtonItem);
		}
		CustomFocusedTreeListNode = treeListHitInfo.Node;
		metadataTreeList.Refresh();
		treePopupMenu.ShowPopup(mousePosition);
	}

	private void SetRecentDropDownMenu(PopupMenu treePopupMenu, BarSubItem connectToExistingBarButtonItem)
	{
		List<RecentProject> list = RecentProjectsHelper.GetList(ProgramVersion.Major, SkinsManager.CurrentSkin.ListSelectionForeColor);
		connectToExistingBarButtonItem.ClearLinks();
		BarItem[] items = list.Take(5).Select(delegate(RecentProject x)
		{
			BarButtonItem barButtonItem = new BarButtonItem(treePopupMenu.Manager, x.DisplayNameShort, 0);
			barButtonItem.Tag = x;
			barButtonItem.Glyph = ((x.Type == RepositoryType.File) ? Resources.file_repository_16 : Resources.server_repository_16);
			barButtonItem.ItemClick += delegate
			{
				RunRecentProject(x);
			};
			return barButtonItem;
		}).ToArray();
		connectToExistingBarButtonItem.AddItems(items);
	}

	private void RunRecentProject(RecentProject recentProject)
	{
		metadataEditorUserControl.OpenRecentEvent?.Invoke(this, recentProject);
	}

	private void AddAddTermPopupMenu(PopupMenu treePopupMenu, BarButtonItem addTermBarButtonItem)
	{
		if (metadataEditorUserControl.BusinessGlossarySupport.TermTypes.Count > 1)
		{
			BarSubItem barSubItem = new BarSubItem(treePopupMenu.Manager, "New", 20);
			BarButtonItem[] termTypesBarButtonItems = metadataEditorUserControl.BusinessGlossarySupport.GetTermTypesBarButtonItems(treePopupMenu.Manager, metadataEditorUserControl.BusinessGlossarySupport.AddTermBarButtonItem_ItemClick);
			BarItem[] items = termTypesBarButtonItems;
			barSubItem.AddItems(items);
			treePopupMenu.AddItem(barSubItem);
		}
		else if (metadataEditorUserControl.BusinessGlossarySupport.TermTypes.Count == 1)
		{
			TermTypeObject termTypeObject2 = (TermTypeObject)(addTermBarButtonItem.Tag = metadataEditorUserControl.BusinessGlossarySupport.TermTypes[0]);
			addTermBarButtonItem.Caption = "Add " + termTypeObject2.TitleAsSuffixWord;
			addTermBarButtonItem.Glyph = BusinessGlossarySupport.GetTermIcon(termTypeObject2.IconId);
			treePopupMenu.AddItem(addTermBarButtonItem);
		}
		else
		{
			addTermBarButtonItem.Tag = null;
			addTermBarButtonItem.Caption = "Add term";
			addTermBarButtonItem.Glyph = Resources.term_add_16;
			treePopupMenu.AddItem(addTermBarButtonItem);
		}
	}

	private void AddAddNewTermPopupMenu(BarSubItem subItem, BarButtonItem addNewTermBarButtonItem)
	{
		List<DBTreeNode> businessGlossaryNodes = GetBusinessGlossaryNodes();
		addNewTermBarButtonItem.ItemClick -= AddNewTermBarButtonItem_ItemClick;
		if (businessGlossaryNodes.Count <= 1)
		{
			addNewTermBarButtonItem.ItemClick += AddNewTermBarButtonItem_ItemClick;
			addNewTermBarButtonItem.Tag = null;
			addNewTermBarButtonItem.Caption = "Linked Business Glossary Term";
			addNewTermBarButtonItem.Glyph = Resources.term_add_16;
			subItem.AddItem(addNewTermBarButtonItem);
			return;
		}
		BarSubItem barSubItem = new BarSubItem(subItem.Manager, "Linked Business Glossary Term in", 11);
		IEnumerable<BarButtonItem> source = businessGlossaryNodes.Select(delegate(DBTreeNode x)
		{
			BarButtonItem barButtonItem = new BarButtonItem();
			barButtonItem.Tag = x.DatabaseId;
			barButtonItem.Caption = x.TreeDisplayNameUiEscaped;
			barButtonItem.Glyph = Resources.term_add_16;
			barButtonItem.ItemClick += delegate
			{
				metadataEditorUserControl.DataLinksSupport.AddNewTermFromContextMenu(x.DatabaseId);
			};
			return barButtonItem;
		});
		BarItem[] items = source.ToArray();
		barSubItem.AddItems(items);
		subItem.AddItem(barSubItem);
	}

	private void AddNewTermBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		metadataEditorUserControl.DataLinksSupport.AddNewTermFromContextMenu(null);
	}

	public void MouseUp(object sender, MouseEventArgs e, MetadataEditorUserControl metadataEditorUserControl, TreeListColumn progressTreeListColumn, Action<TreeListNode> processFocusNode)
	{
		LockFocus = false;
		if (e.Button != MouseButtons.Left)
		{
			return;
		}
		TreeListHitInfo treeListHitInfo = (sender as TreeList).CalcHitInfo(new Point(e.X, e.Y));
		if (treeListHitInfo.Node == null && MouseDownPoint.HasValue)
		{
			treeListHitInfo = (sender as TreeList).CalcHitInfo(MouseDownPoint.Value);
		}
		if (treeListHitInfo == null || treeListHitInfo.HitInfoType != HitInfoType.Cell || treeListHitInfo?.Column != progressTreeListColumn || StartedToFocus)
		{
			if (treeListHitInfo.Node == null && MouseDownPoint.HasValue)
			{
				treeListHitInfo = (sender as TreeList).CalcHitInfo(MouseDownPoint.Value);
			}
			if (StartedToFocus)
			{
				metadataTreeList.FocusedNode = treeListHitInfo.Node;
				if (StartedToFocus)
				{
					processFocusNode(treeListHitInfo.Node);
				}
				return;
			}
		}
		MouseDownPoint = null;
	}

	public void GetSelectImage(object sender, GetSelectImageEventArgs e, ImageCollection treeMenuImageCollection)
	{
		if (e.Node == null)
		{
			return;
		}
		DBTreeNode node = GetNode(e.Node);
		if (node != null)
		{
			if (node.ObjectType != SharedObjectTypeEnum.ObjectType.Database)
			{
				IconsSupport.SetNodeImageIndex(treeMenuImageCollection, e, node);
			}
			else
			{
				IconsSupport.SetTreeImage(node.ObjectType, node.DatabaseType, e, treeMenuImageCollection);
			}
		}
	}

	private BasePanelControl GetVisibleUserControl()
	{
		return metadataEditorUserControl.GetVisibleUserControl();
	}

	private BaseSummaryUserControl GetVisibleSummaryUserControl()
	{
		return metadataEditorUserControl.GetVisibleSummaryUserControl();
	}

	public TreeListHelpers(MetadataEditorUserControl metadataEditorUserControl, TreeList metadataTreeList)
	{
		this.metadataEditorUserControl = metadataEditorUserControl;
		this.metadataTreeList = metadataTreeList;
	}

	public void InitializeControls(TableUserControl tableUserControl, TableSummaryUserControl tableSummaryUserControl, TableSummaryUserControl viewSummaryUserControl, TableSummaryUserControl objectSummaryUserControl, ProcedureSummaryUserControl procedureSummaryUserControl, ProcedureSummaryUserControl functionSummaryUserControl, ModuleSummaryUserControl moduleSummaryUserControl, TermSummaryUserControl termSummaryUserControl)
	{
		this.tableUserControl = tableUserControl;
		this.tableSummaryUserControl = tableSummaryUserControl;
		this.viewSummaryUserControl = viewSummaryUserControl;
		this.objectSummaryUserControl = objectSummaryUserControl;
		this.procedureSummaryUserControl = procedureSummaryUserControl;
		this.functionSummaryUserControl = functionSummaryUserControl;
		this.moduleSummaryUserControl = moduleSummaryUserControl;
		this.termSummaryUserControl = termSummaryUserControl;
	}

	public static bool IsObjectOrFolder(DBTreeNode dBTreeNode, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (dBTreeNode != null)
		{
			if (dBTreeNode.ObjectType != objectType)
			{
				if (dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database || dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module)
				{
					return dBTreeNode.Name == SharedObjectTypeEnum.TypeToStringForMenu(objectType);
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public TreeListNode GetFocusedDocumentationNode(bool fromCustomFocus = false)
	{
		TreeListNode treeListNode = ((!fromCustomFocus) ? metadataTreeList.FocusedNode : CustomFocusedTreeListNode);
		while (treeListNode?.ParentNode != null)
		{
			treeListNode = treeListNode.ParentNode;
		}
		return treeListNode;
	}

	public List<DBTreeNode> GetBusinessGlossaryNodes()
	{
		return (metadataTreeList.DataSource as DBTree).Where((DBTreeNode x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary).ToList();
	}

	public TreeListNode GetFocusedTreeListNode(bool fromCustomFocus = false)
	{
		if (!fromCustomFocus)
		{
			return metadataTreeList.FocusedNode;
		}
		return CustomFocusedTreeListNode;
	}

	public DBTreeNode GetFocusedDocumentationDBTreeNode(bool fromCustomFocus = false)
	{
		return GetNode(GetFocusedDocumentationNode(fromCustomFocus));
	}

	public DBTreeNode GetFocusedNode(bool fromCustomFocus = false)
	{
		return GetNode(GetFocusedTreeListNode(fromCustomFocus));
	}

	public DBTreeNode GetNode(TreeListNode treeListNode)
	{
		return (DBTreeNode)metadataTreeList.GetDataRecordByNode(treeListNode);
	}

	public T GetTypedDataObject<T>(IDataObject data) where T : class
	{
		return data.GetData(typeof(T)) as T;
	}

	public TreeListNode GetTreeListNodeObject(IDataObject data)
	{
		return data.GetData(typeof(TreeListNode)) as TreeListNode;
	}

	public DBTreeNode GetDBTreeNode(TreeListNode treeListNode)
	{
		return (DBTreeNode)metadataTreeList.GetDataRecordByNode(treeListNode);
	}

	public DBTreeNode GetDBTreeNode(IDataObject data)
	{
		TreeListNode treeListNodeObject = GetTreeListNodeObject(data);
		return (DBTreeNode)metadataTreeList.GetDataRecordByNode(treeListNodeObject);
	}

	public DBTreeNode GetDBTreeNode(DragEventArgs e)
	{
		Point pt = metadataTreeList.PointToClient(new Point(e.X, e.Y));
		TreeListNode node = metadataTreeList.CalcHitInfo(pt).Node;
		return (DBTreeNode)metadataTreeList.GetDataRecordByNode(node);
	}

	public void OpenNode(IGoTo goToObject)
	{
		if (goToObject != null)
		{
			OpenNode(goToObject.DocumentationId, goToObject.ObjectId, SharedObjectTypeEnum.StringToType(goToObject.ObjectType));
		}
	}

	public void OpenNode(int? documentationId, int objectId, SharedObjectTypeEnum.ObjectType? objectType)
	{
		TreeListNode goToNode = metadataEditorUserControl.SearchTreeNodeOperation.FindNode(metadataEditorUserControl.MetadataTreeList.Nodes, documentationId, objectId, objectType);
		OpenNode(goToNode);
	}

	public TreeListNode GetTreeListNodeByObject(int? documentationId, int objectId, SharedObjectTypeEnum.ObjectType? objectType)
	{
		return metadataEditorUserControl.SearchTreeNodeOperation.FindNode(metadataEditorUserControl.MetadataTreeList.Nodes, documentationId, objectId, objectType);
	}

	public DBTreeNode GetDBTreeNodeByObject(int? documentationId, int objectId, SharedObjectTypeEnum.ObjectType? objectType)
	{
		TreeListNode treeListNode = metadataEditorUserControl.SearchTreeNodeOperation.FindNode(metadataEditorUserControl.MetadataTreeList.Nodes, documentationId, objectId, objectType);
		return GetNode(treeListNode);
	}

	public void OpenNode(TreeListNode goToNode)
	{
		DBTreeNode dBTreeNode = metadataEditorUserControl.TreeListHelpers.GetDBTreeNode(goToNode);
		if (dBTreeNode != null)
		{
			metadataEditorUserControl.OpenPageControl(showControl: true, dBTreeNode);
			metadataEditorUserControl.MetadataTreeList.FocusedNode = goToNode;
		}
	}

	public bool CheckWhetherIsTermDragDrop(DBTreeNode dragged, DBTreeNode target)
	{
		return CheckWhetherIsTermDragDrop(dragged?.ObjectType, target);
	}

	public bool CheckWhetherIsTermDragDrop(SharedObjectTypeEnum.ObjectType? draggedObjectType, DBTreeNode target)
	{
		return MatchingDragDropTypes.IsValidTermDragDrop(draggedObjectType, target?.ObjectType, target?.ParentNode?.ObjectType);
	}

	public bool CheckWhetherIsTermToObjectDragDrop(DBTreeNode dragged, DBTreeNode target)
	{
		return CheckWhetherIsTermToObjectDragDrop(dragged?.ObjectType, target);
	}

	public bool CheckWhetherIsObjectToTermDragDrop(DBTreeNode dragged, DBTreeNode target)
	{
		return CheckWhetherIsObjectToTermDragDrop(dragged?.ObjectType, target);
	}

	public bool CheckWhetherIsTermToObjectDragDrop(SharedObjectTypeEnum.ObjectType? draggedObjectType, DBTreeNode target)
	{
		if (target != null)
		{
			return MatchingDragDropTypes.IsValidTermToObjectDragDrop(draggedObjectType, target.ObjectType);
		}
		return false;
	}

	public bool CheckWhetherIsObjectToTermDragDrop(SharedObjectTypeEnum.ObjectType? draggedObjectType, DBTreeNode target)
	{
		if (target != null)
		{
			return MatchingDragDropTypes.IsValidObjectToTermDragDrop(draggedObjectType, target.ObjectType);
		}
		return false;
	}

	public bool IsValidObjectToTermDragDrop(DBTreeNode dragged, DBTreeNode target)
	{
		return IsValidObjectToTermDragDrop(dragged?.ObjectType, target);
	}

	public bool IsValidObjectToTermDragDrop(SharedObjectTypeEnum.ObjectType? draggedObjectType, DBTreeNode target)
	{
		if (target != null)
		{
			return MatchingDragDropTypes.IsValidObjectToTermDragDrop(draggedObjectType, target.ObjectType);
		}
		return false;
	}

	public bool CheckWhetherIsValidTermDragDrop(DBTreeNode dragged, DBTreeNode target)
	{
		return CheckWhetherIsValidTermDragDrop(dragged?.Id, dragged?.ObjectType, target);
	}

	public bool CheckWhetherIsValidTermDragDrop(int? draggedObjectId, SharedObjectTypeEnum.ObjectType? draggedObjectType, DBTreeNode target)
	{
		if (!draggedObjectId.HasValue || !draggedObjectType.HasValue || target == null)
		{
			return false;
		}
		if (!MatchingDragDropTypes.IsValidTermDragDrop(draggedObjectType, target.ObjectType, target.ParentNode?.ObjectType) || target.ContainsNode(draggedObjectId.Value, draggedObjectType, checkSubnodes: false))
		{
			return false;
		}
		for (DBTreeNode dBTreeNode = target; dBTreeNode != null; dBTreeNode = dBTreeNode.ParentNode)
		{
			if (dBTreeNode.ObjectType == draggedObjectType && dBTreeNode.Id == draggedObjectId)
			{
				return false;
			}
		}
		return MatchingDragDropTypes.IsValidTermDragDrop(draggedObjectType, target.ObjectType, target.ParentNode?.ObjectType);
	}

	public void SetDragDropEffect(DBTreeNode dragged, DBTreeNode target, DragEventArgs e)
	{
		if (dragged == null || target == null)
		{
			return;
		}
		if (CheckWhetherIsTermDragDrop(dragged, target))
		{
			e.Effect = DragDropEffects.Move;
		}
		else if (CheckWhetherIsTermToObjectDragDrop(dragged, target) || CheckWhetherIsObjectToTermDragDrop(dragged, target))
		{
			e.Effect = DragDropEffects.Link;
		}
		else if ((dragged.ObjectType == SharedObjectTypeEnum.ObjectType.Table && target.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database && SharedObjectTypeEnum.StringToTypeForMenu(target.Name) == SharedObjectTypeEnum.ObjectType.Table) || (dragged.ObjectType == SharedObjectTypeEnum.ObjectType.View && target.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database && SharedObjectTypeEnum.StringToTypeForMenu(target.Name) == SharedObjectTypeEnum.ObjectType.View) || (dragged.ObjectType == SharedObjectTypeEnum.ObjectType.Structure && target.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database && SharedObjectTypeEnum.StringToTypeForMenu(target.Name) == SharedObjectTypeEnum.ObjectType.Structure) || (dragged.ObjectType == SharedObjectTypeEnum.ObjectType.Procedure && target.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database && SharedObjectTypeEnum.StringToTypeForMenu(target.Name) == SharedObjectTypeEnum.ObjectType.Procedure) || (dragged.ObjectType == SharedObjectTypeEnum.ObjectType.Function && target.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database && SharedObjectTypeEnum.StringToTypeForMenu(target.Name) == SharedObjectTypeEnum.ObjectType.Function))
		{
			if (dragged.Source != UserTypeEnum.UserType.USER || dragged.IsInModule)
			{
				e.Effect = DragDropEffects.None;
			}
			else
			{
				e.Effect = DragDropEffects.Move;
			}
		}
		else if (dragged.ObjectType == SharedObjectTypeEnum.ObjectType.Module && target.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			e.Effect = DragDropEffects.Move;
		}
		else
		{
			e.Effect = DragDropEffects.Copy;
		}
	}

	public void SetDragDropEffect(ColumnRow dragged, DBTreeNode target, DragEventArgs e)
	{
		if (CheckWhetherIsTermDragDrop(dragged.ObjectType, target))
		{
			e.Effect = DragDropEffects.Move;
		}
		else if (CheckWhetherIsTermToObjectDragDrop(dragged.ObjectType, target) || IsValidObjectToTermDragDrop(dragged.ObjectType, target))
		{
			e.Effect = DragDropEffects.Link;
		}
		else
		{
			e.Effect = DragDropEffects.Copy;
		}
	}

	public void SetDragDropEffect(TermObject dragged, DBTreeNode target, DragEventArgs e)
	{
		if (CheckWhetherIsTermDragDrop(SharedObjectTypeEnum.ObjectType.Term, target))
		{
			e.Effect = DragDropEffects.Move;
		}
		else if (CheckWhetherIsTermToObjectDragDrop(SharedObjectTypeEnum.ObjectType.Term, target) || IsValidObjectToTermDragDrop(SharedObjectTypeEnum.ObjectType.Term, target))
		{
			e.Effect = DragDropEffects.Link;
		}
		else if (target != null)
		{
			e.Effect = DragDropEffects.Copy;
		}
	}

	public bool CheckIfParentNotContainChild(DBTreeNode target, SharedObjectTypeEnum.ObjectType childObjectType, int childObjectId)
	{
		if (target != null && target.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			DBTreeNode directoryNode = target.GetDirectoryNode(childObjectType);
			if (directoryNode == null)
			{
				return true;
			}
			return !directoryNode.Nodes.Any((DBTreeNode x) => x.Id == childObjectId);
		}
		if (target != null && target.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module)
		{
			return !target.Nodes.Any((DBTreeNode x) => x.Id == childObjectId);
		}
		return true;
	}

	public bool CheckIfParentNotContainChild(DBTreeNode target, DBTreeNode child)
	{
		if (target != null && target.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			DBTreeNode directoryNode = target.GetDirectoryNode(child.ObjectType);
			if (directoryNode == null)
			{
				return true;
			}
			return !directoryNode.Nodes.Any((DBTreeNode x) => x.Id == child.Id);
		}
		if (target != null && target.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module)
		{
			return !target.Nodes.Any((DBTreeNode x) => x.Id == child.Id);
		}
		return true;
	}

	public bool CheckIfDraggingAllowed(IDataObject data, DBTreeNode targetDBNode)
	{
		ObjectWithModulesObject typedDataObject = GetTypedDataObject<ObjectWithModulesObject>(data);
		bool doesNotContain = true;
		int? num = null;
		SharedObjectTypeEnum.ObjectType? draggedRowType = null;
		List<object> source = null;
		bool flag = false;
		if (GetTypedDataObject<ObjectWithModulesObject>(data) != null)
		{
			draggedRowType = SharedObjectTypeEnum.StringToType(PrepareValue.ToString(typedDataObject.ObjectType));
			SharedObjectSubtypeEnum.StringToType(draggedRowType, PrepareValue.ToString(typedDataObject.Subtype));
			if (draggedRowType.HasValue)
			{
				switch (draggedRowType.GetValueOrDefault())
				{
				case SharedObjectTypeEnum.ObjectType.Table:
					source = tableSummaryUserControl.DragRows.DraggedRows;
					goto IL_0112;
				case SharedObjectTypeEnum.ObjectType.View:
					source = viewSummaryUserControl.DragRows.DraggedRows;
					goto IL_0112;
				case SharedObjectTypeEnum.ObjectType.Structure:
					source = objectSummaryUserControl.DragRows.DraggedRows;
					goto IL_0112;
				case SharedObjectTypeEnum.ObjectType.Procedure:
					source = procedureSummaryUserControl.DragRows.DraggedRows;
					goto IL_0112;
				case SharedObjectTypeEnum.ObjectType.Function:
					{
						source = functionSummaryUserControl.DragRows.DraggedRows;
						goto IL_0112;
					}
					IL_0112:
					return source.Cast<ObjectWithModulesObject>().Any(delegate(ObjectWithModulesObject x)
					{
						doesNotContain = CheckIfParentNotContainChild(targetDBNode, draggedRowType.Value, x.Id);
						return CheckIfDraggingAllowed(x.Id, draggedRowType, targetDBNode, doesNotContain);
					});
				}
			}
			return false;
		}
		if (GetTypedDataObject<TermObject>(data) != null)
		{
			source = termSummaryUserControl.DragRows.DraggedRows;
			draggedRowType = SharedObjectTypeEnum.ObjectType.Term;
			return source.Cast<TermObject>().Any((TermObject x) => CheckIfDraggingAllowed(x.TermId, draggedRowType, targetDBNode, doesNotContain));
		}
		if (GetTypedDataObject<ColumnRow>(data) != null)
		{
			DBTreeNode dBTreeNode = targetDBNode;
			if (dBTreeNode != null)
			{
				_ = dBTreeNode.Id;
				if (0 == 0)
				{
					ColumnRow typedDataObject2 = GetTypedDataObject<ColumnRow>(data);
					if (typedDataObject2.ParentObjectType == SharedObjectTypeEnum.ObjectType.Table || typedDataObject2.ParentObjectType == SharedObjectTypeEnum.ObjectType.View || typedDataObject2.ParentObjectType == SharedObjectTypeEnum.ObjectType.Structure)
					{
						source = tableUserControl.ColumnsDragRows.DraggedRows;
					}
					draggedRowType = SharedObjectTypeEnum.ObjectType.Column;
					return source.Cast<ColumnRow>().Any((ColumnRow x) => CheckIfDraggingAllowed(targetDBNode.Id, draggedRowType, targetDBNode, doesNotContain));
				}
			}
			return false;
		}
		DBTreeNode dBTreeNode2 = (DBTreeNode)metadataTreeList.GetDataRecordByNode(data.GetData(typeof(TreeListNode)) as TreeListNode);
		if (dBTreeNode2 == null)
		{
			return false;
		}
		num = dBTreeNode2.Id;
		draggedRowType = dBTreeNode2.ObjectType;
		doesNotContain = CheckIfParentNotContainChild(targetDBNode, dBTreeNode2);
		flag = CheckIfCanDragUserObject(draggedRowType, targetDBNode, dBTreeNode2);
		return CheckIfDraggingAllowed(num, draggedRowType, targetDBNode, doesNotContain) || flag;
	}

	public bool CheckIfDraggingAllowed(ColumnRow data, DBTreeNode targetDBNode)
	{
		return CheckIfDraggingAllowed(targetDBNode?.Id, data.ObjectType, targetDBNode, doesNotContain: true);
	}

	private bool CheckIfDraggingAllowed(int? draggedObjectId, SharedObjectTypeEnum.ObjectType? draggedRowType, DBTreeNode targetDBNode, bool doesNotContain)
	{
		bool flag = draggedRowType.HasValue && (draggedRowType == SharedObjectTypeEnum.ObjectType.Table || draggedRowType == SharedObjectTypeEnum.ObjectType.View || draggedRowType == SharedObjectTypeEnum.ObjectType.Procedure || draggedRowType == SharedObjectTypeEnum.ObjectType.Function || draggedRowType == SharedObjectTypeEnum.ObjectType.Structure || draggedRowType == SharedObjectTypeEnum.ObjectType.Module || draggedRowType == SharedObjectTypeEnum.ObjectType.Term || draggedRowType == SharedObjectTypeEnum.ObjectType.Column);
		bool flag2 = draggedRowType != SharedObjectTypeEnum.ObjectType.Module && targetDBNode != null && ((targetDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module && SharedObjectTypeEnum.StringToTypeForMenu(targetDBNode.Name) == draggedRowType) || (targetDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module && (draggedRowType == SharedObjectTypeEnum.ObjectType.Table || draggedRowType == SharedObjectTypeEnum.ObjectType.View || draggedRowType == SharedObjectTypeEnum.ObjectType.Procedure || draggedRowType == SharedObjectTypeEnum.ObjectType.Function || draggedRowType == SharedObjectTypeEnum.ObjectType.Structure)) || MatchingDragDropTypes.IsValidTermToObjectDragDrop(draggedRowType, targetDBNode.ObjectType) || MatchingDragDropTypes.IsValidObjectToTermDragDrop(draggedRowType, targetDBNode.ObjectType));
		if (doesNotContain)
		{
			if (!(flag2 && flag))
			{
				return CheckWhetherIsValidTermDragDrop(draggedObjectId, draggedRowType, targetDBNode);
			}
			return true;
		}
		return false;
	}

	private bool CheckIfCanDragUserObject(SharedObjectTypeEnum.ObjectType? draggedRowType, DBTreeNode targetDBNode, DBTreeNode draggedDBNode)
	{
		if (!draggedRowType.HasValue || targetDBNode == null || draggedDBNode == null)
		{
			return false;
		}
		if (targetDBNode.DatabaseClass == SharedDatabaseClassEnum.DatabaseClass.Database && ((draggedRowType == SharedObjectTypeEnum.ObjectType.Module && targetDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database) || (!draggedDBNode.IsInModule && ((draggedDBNode.Source == UserTypeEnum.UserType.USER && draggedRowType == SharedObjectTypeEnum.ObjectType.Table && targetDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database && SharedObjectTypeEnum.StringToTypeForMenu(targetDBNode.Name) == SharedObjectTypeEnum.ObjectType.Table) || (draggedRowType == SharedObjectTypeEnum.ObjectType.View && targetDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database && SharedObjectTypeEnum.StringToTypeForMenu(targetDBNode.Name) == SharedObjectTypeEnum.ObjectType.View) || (draggedRowType == SharedObjectTypeEnum.ObjectType.Structure && targetDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database && SharedObjectTypeEnum.StringToTypeForMenu(targetDBNode.Name) == SharedObjectTypeEnum.ObjectType.Structure) || (draggedRowType == SharedObjectTypeEnum.ObjectType.Procedure && targetDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database && SharedObjectTypeEnum.StringToTypeForMenu(targetDBNode.Name) == SharedObjectTypeEnum.ObjectType.Procedure) || (draggedRowType == SharedObjectTypeEnum.ObjectType.Function && targetDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database && SharedObjectTypeEnum.StringToTypeForMenu(targetDBNode.Name) == SharedObjectTypeEnum.ObjectType.Function)))))
		{
			return true;
		}
		if (draggedDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module && targetDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module && draggedDBNode.DatabaseId == targetDBNode.DatabaseId)
		{
			return true;
		}
		return false;
	}

	public bool ChangeNodeNameAndSchema(DBTreeNode dbTreeNode)
	{
		if (Licenses.CheckRepositoryVersionAfterLogin() && metadataTreeList.FocusedNode != null && (dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table || dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Structure) && dbTreeNode.Source == UserTypeEnum.UserType.USER)
		{
			metadataTreeList.OptionsBehavior.Editable = true;
			dbTreeNode.Name = DBTreeMenu.SetName(dbTreeNode.Schema, dbTreeNode.BaseName, dbTreeNode.Title, dbTreeNode.ObjectType, dbTreeNode.DatabaseType, withSchema: true);
			return true;
		}
		return false;
	}

	public bool StartEditTitle(bool fromCustomFocus = false)
	{
		if (Licenses.CheckRepositoryVersionAfterLogin())
		{
			DBTreeNode focusedNode = GetFocusedNode(fromCustomFocus);
			if (metadataTreeList.FocusedNode != null && (CustomFocusedTreeListNode == null || CustomFocusedTreeListNode == metadataTreeList.FocusedNode) && (focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Database || focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary || focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module || focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Term))
			{
				metadataTreeList.OptionsBehavior.Editable = true;
				return true;
			}
		}
		return false;
	}

	public void ExpandAll(bool fromCustomFocus = false)
	{
		GetFocusedTreeListNode(fromCustomFocus)?.ExpandAll();
	}

	public void CollaspeAll(bool fromCustomFocus = false)
	{
		TreeListNode focusedTreeListNode = GetFocusedTreeListNode(fromCustomFocus);
		if (focusedTreeListNode != null)
		{
			CollaspeAll(focusedTreeListNode);
		}
	}

	public void CollaspeAll(TreeListNode node)
	{
		try
		{
			if (node != null)
			{
				metadataTreeList.SuspendLayout();
				node.Expanded = false;
				metadataEditorUserControl.MetadataTreeList.NodesIterator.DoLocalOperation(new CollapseAllOperation(), node.Nodes);
			}
		}
		finally
		{
			metadataTreeList.ResumeLayout();
		}
	}

	public void ProgressUpIfProgressVisible(DBTreeNode termDbTreeNode)
	{
		if (ShowProgress)
		{
			DBTreeNode.ProgressUp(termDbTreeNode, ProgressType);
		}
	}

	public void AggregateProgressUpIfProgressVisible(DBTreeNode termDbTreeNode)
	{
		if (ShowProgress)
		{
			DBTreeNode.AggregateProgressUp(termDbTreeNode, ProgressType);
		}
	}

	public void AggregateProgressDownIfProgressVisible(DBTreeNode termDbTreeNode)
	{
		if (ShowProgress)
		{
			DBTreeNode.AggregateProgressDown(termDbTreeNode, ProgressType);
		}
	}

	public Rectangle? GetNodeBounds(TreeListNode treeListNode)
	{
		RowInfo rowInfo = metadataTreeList.ViewInfo.RowsInfo[treeListNode];
		if (rowInfo != null)
		{
			CellInfo cellInfo = rowInfo.Cells.Last();
			if (cellInfo != null)
			{
				return new Rectangle(metadataTreeList.PointToScreen(cellInfo.Bounds.Location), cellInfo.Bounds.Size);
			}
		}
		return null;
	}

	public List<int> GetSelectedNodeTreeIndexPath(Form owner = null)
	{
		List<int> list = new List<int>();
		try
		{
			TreeListNode treeListNode = metadataTreeList.FocusedNode;
			if (treeListNode != null)
			{
				if (treeListNode.ParentNode != null)
				{
					list.Add(treeListNode.ParentNode.Nodes.IndexOf(treeListNode));
					int num = 0;
					while (treeListNode != null)
					{
						treeListNode = treeListNode.ParentNode;
						if (treeListNode?.ParentNode != null)
						{
							list.Add(treeListNode.ParentNode.Nodes.IndexOf(treeListNode));
							num++;
							if (num > 100)
							{
								break;
							}
							continue;
						}
						list.Add(metadataTreeList.Nodes.IndexOf(treeListNode));
						break;
					}
				}
				else
				{
					list.Add(metadataTreeList.Nodes.IndexOf(treeListNode));
				}
			}
			list.Reverse();
			return list;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, owner);
			return list;
		}
	}

	public void SelectNodeByIndexPath(List<int> nodesIndexes, Form owner = null)
	{
		try
		{
			if (nodesIndexes == null || !nodesIndexes.Any() || !(metadataTreeList.Nodes?.Count > nodesIndexes[0]))
			{
				return;
			}
			TreeListNode treeListNode = metadataTreeList.Nodes?[nodesIndexes[0]];
			if (nodesIndexes.Count == 1)
			{
				if (treeListNode != null)
				{
					OpenNode(treeListNode);
				}
				return;
			}
			for (int i = 1; i < nodesIndexes.Count; i++)
			{
				if (treeListNode == null)
				{
					continue;
				}
				treeListNode.Expand();
				if (treeListNode.Nodes?.Count > nodesIndexes[i])
				{
					treeListNode = treeListNode.Nodes[nodesIndexes[i]];
					treeListNode?.Expand();
					if (i == nodesIndexes.Count - 1)
					{
						OpenNode(treeListNode);
					}
				}
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, owner);
		}
	}

	public void RefreshNormalObjectsNames(DBTreeNode selectedNode)
	{
		if (selectedNode.IsNormalObject)
		{
			selectedNode.SetName();
		}
		foreach (DBTreeNode node in selectedNode.Nodes)
		{
			RefreshNormalObjectsNames(node);
		}
	}

	public void ActualizeModulesOrder(TreeListNode treeListNode, Form owner)
	{
		if (treeListNode == null)
		{
			return;
		}
		DBTreeNode dBTreeNode = GetDBTreeNode(treeListNode);
		if ((dBTreeNode == null || dBTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database) && (dBTreeNode == null || dBTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Module))
		{
			return;
		}
		TreeListNode treeListNode2 = treeListNode;
		if (dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			treeListNode2 = treeListNode.ParentNode;
		}
		treeListNode2.Expand();
		foreach (TreeListNode node in treeListNode2.Nodes)
		{
			dBTreeNode = GetNode(node);
			if (!DB.Module.UpdateOrdinalPosition(dBTreeNode.Id, metadataTreeList.GetNodeIndex(node) + 1, owner))
			{
				break;
			}
		}
	}
}
