using System;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Forms;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Dataedo.App.UserControls.PanelControls;
using Dataedo.App.UserControls.PanelControls.Appearance;
using Dataedo.App.UserControls.PanelControls.TableUserControlHelpers.Interfaces;
using Dataedo.App.UserControls.SummaryControls;
using Dataedo.App.UserControls.WindowControls.MetadataEditorUserControlHelpers;
using Dataedo.Shared.Enums;
using DevExpress.XtraBars;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.UserControls.WindowControls.MetadataEditorUserControlFeatures;

public class DataLinksSupport
{
	private readonly MetadataEditorUserControl metadataEditorUserControl;

	private readonly ModuleSummaryUserControl moduleSummaryUserControl;

	private readonly DBTreeMenu dbTreeMenu;

	private readonly TreeList metadataTreeList;

	private TermUserControl VisibleTermUserControl => metadataEditorUserControl.GetVisibleUserControl() as TermUserControl;

	private TreeListHelpers TreeListHelpers => metadataEditorUserControl.TreeListHelpers;

	private SearchTreeNodeOperation SearchTreeNodeOperation => metadataEditorUserControl.SearchTreeNodeOperation;

	private void RefreshSearchDatabasesFromMetadataTreeList(int? id = null, string title = null)
	{
		metadataEditorUserControl.RefreshSearchDatabasesFromMetadataTreeList(id, title);
	}

	private bool ContinueAfterPossibleChanges(Action ifChangedNoCancelAction = null, Action ifChangedNoAction = null)
	{
		return metadataEditorUserControl.ContinueAfterPossibleChanges(ifChangedNoCancelAction, ifChangedNoAction);
	}

	private void SetControlsDuringAddingModuleEnabled(bool enabled)
	{
		metadataEditorUserControl.SetControlsDuringAddingModuleEnabled(enabled);
	}

	public DataLinksSupport(MetadataEditorUserControl metadataEditorUserControl, ModuleSummaryUserControl moduleSummaryUserControl, DBTreeMenu dbTreeMenu, TreeList metadataTreeList)
	{
		this.metadataEditorUserControl = metadataEditorUserControl;
		this.moduleSummaryUserControl = moduleSummaryUserControl;
		this.dbTreeMenu = dbTreeMenu;
		this.metadataTreeList = metadataTreeList;
	}

	public void AddDataLinkBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddDataLinkFromContextMenu(metadataEditorUserControl.FindForm());
	}

	public void AddNewTermBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddNewTermFromContextMenu(null);
	}

	private void AddDataLinkFromContextMenu(Form owner = null)
	{
		DBTreeNode node = TreeListHelpers.GetNode(TreeListHelpers.GetFocusedTreeListNode(fromCustomFocus: true));
		if (node.ObjectType == SharedObjectTypeEnum.ObjectType.Term)
		{
			moduleSummaryUserControl?.SetEmptyModulesListPanelVisibility(visible: false);
			StartAddingDataLink(TreeListHelpers.GetFocusedTreeListNode(fromCustomFocus: true), fromCustomFocus: true, owner);
		}
		else if (SharedObjectTypeEnum.IsSupportingBG(node.ObjectType))
		{
			StartAddingLinkToBusinessGlossary(TreeListHelpers.GetFocusedTreeListNode(fromCustomFocus: true), fromCustomFocus: true, metadataEditorUserControl?.FindForm());
		}
	}

	public void AddNewTermFromContextMenu(int? businessGlossaryId)
	{
		if (ContinueAfterPossibleChanges())
		{
			DBTreeNode node = TreeListHelpers.GetNode(TreeListHelpers.GetFocusedTreeListNode(fromCustomFocus: true));
			if (SharedObjectTypeEnum.IsSupportingBG(node.ObjectType))
			{
				metadataEditorUserControl.BusinessGlossarySupport.AddNewBusinessGlossaryTerm(businessGlossaryId, metadataEditorUserControl?.FindForm(), new BusinessGlossarySupport.ObjectDefinition(node));
			}
		}
	}

	public DialogResult StartAddingDataLink(int rootTermId, bool fromCustomFocus, Form owner = null)
	{
		try
		{
			if (!Licenses.CheckRepositoryVersionAfterLogin() || !ContinueAfterPossibleChanges())
			{
				return DialogResult.Abort;
			}
			AddDataLinksForm addDataLinksForm = new AddDataLinksForm();
			addDataLinksForm.SetParameters(rootTermId, metadataEditorUserControl, metadataEditorUserControl.CustomFieldsSupport);
			if (!addDataLinksForm.IsDisposed)
			{
				addDataLinksForm.ShowDialog();
			}
			if (addDataLinksForm.DialogResult == DialogResult.OK)
			{
				VisibleTermUserControl?.RefreshDataLinks(forceRefresh: true);
			}
			return addDataLinksForm.DialogResult;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, owner);
			return DialogResult.Abort;
		}
	}

	public DialogResult StartAddingDataLink(TreeListNode node, bool fromCustomFocus, Form owner = null)
	{
		DBTreeNode node2 = TreeListHelpers.GetNode(node);
		if (node2.ObjectType == SharedObjectTypeEnum.ObjectType.Term)
		{
			return StartAddingDataLink(node2.Id, fromCustomFocus, owner);
		}
		if (SharedObjectTypeEnum.IsSupportingBG(node2.ObjectType))
		{
			return StartAddingLinkToBusinessGlossary(SharedObjectTypeEnum.TypeToString(node2.ObjectType), node2.Id, null, fromCustomFocus, node2.ShowSchema, owner);
		}
		return DialogResult.Cancel;
	}

	public DialogResult StartAddingDataLink(TreeListNode node, bool fromCustomFocus, string objectType, int? elementId, Form owner = null)
	{
		DBTreeNode node2 = TreeListHelpers.GetNode(node);
		if (SharedObjectTypeEnum.IsSupportingBG(node2.ObjectType))
		{
			return StartAddingLinkToBusinessGlossary(objectType, node2.Id, elementId, fromCustomFocus, node2.ShowSchema, owner);
		}
		return DialogResult.Cancel;
	}

	public DialogResult StartAddingLinkToBusinessGlossary(string objectType, int objectId, int? elementId, bool fromCustomFocus, bool contextShowSchema, Form owner = null)
	{
		try
		{
			if (!Licenses.CheckRepositoryVersionAfterLogin() || !ContinueAfterPossibleChanges())
			{
				return DialogResult.Abort;
			}
			AddLinkToBusinessGlossaryForm addLinkToBusinessGlossaryForm = new AddLinkToBusinessGlossaryForm();
			addLinkToBusinessGlossaryForm.SetParameters(objectType, objectId, elementId, metadataEditorUserControl, metadataEditorUserControl.CustomFieldsSupport, contextShowSchema);
			addLinkToBusinessGlossaryForm.ShowDialog();
			if (addLinkToBusinessGlossaryForm.DialogResult == DialogResult.OK)
			{
				VisibleTermUserControl?.RefreshDataLinks(forceRefresh: true);
				(metadataEditorUserControl.GetVisibleUserControl() as IBusinessGlossaryObject)?.RefreshDataLinks();
			}
			return addLinkToBusinessGlossaryForm.DialogResult;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, owner);
			return DialogResult.Abort;
		}
	}

	public DialogResult StartAddingLinkToBusinessGlossary(TreeListNode node, bool fromCustomFocus, Form owner = null)
	{
		DBTreeNode node2 = TreeListHelpers.GetNode(node);
		if (SharedObjectTypeEnum.IsSupportingBG(node2.ObjectType))
		{
			DialogResult num = StartAddingLinkToBusinessGlossary(SharedObjectTypeEnum.TypeToString(node2.ObjectType), node2.Id, null, fromCustomFocus, node2.ShowSchema, owner);
			if (num == DialogResult.OK)
			{
				metadataEditorUserControl.TreeListHelpers.LockFocus = false;
				metadataEditorUserControl.OpenPageControl(showControl: true, TreeListHelpers.GetNode(node));
				metadataTreeList.FocusedNode = node;
				ITabChangable obj = metadataEditorUserControl.GetVisibleUserControl() as ITabChangable;
				if (obj == null)
				{
					return num;
				}
				obj.ChangeTab(SharedObjectTypeEnum.ObjectType.DataLink);
			}
			return num;
		}
		return DialogResult.Cancel;
	}
}
