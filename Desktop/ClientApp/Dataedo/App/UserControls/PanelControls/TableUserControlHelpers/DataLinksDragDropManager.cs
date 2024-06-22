using System;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools.DragDrop;
using Dataedo.CustomMessageBox;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Shared.Enums;
using DevExpress.XtraGrid;

namespace Dataedo.App.UserControls.PanelControls.TableUserControlHelpers;

internal class DataLinksDragDropManager
{
	private DBTreeNode documentationDBTreeNode;

	private DBTreeNode objectDBTreeNode;

	private DataLinksManager dataLinksManager;

	public void SetObject(DBTreeNode documentationDBTreeNode, DBTreeNode objectDBTreeNode, DataLinksManager dataLinksManager)
	{
		this.documentationDBTreeNode = documentationDBTreeNode;
		this.objectDBTreeNode = objectDBTreeNode;
		this.dataLinksManager = dataLinksManager;
	}

	public void AddEvents(GridControl grid)
	{
		grid.AllowDrop = true;
		grid.DragDrop += Grid_DragDrop;
		grid.DragEnter += Grid_DragEnter;
		grid.DragOver += Grid_DragOver;
		grid.DragLeave += Grid_DragLeave;
	}

	private void Grid_DragDrop(object sender, DragEventArgs e)
	{
		DBTreeNode dragged = null;
		DataLinkObjectExtendedForTerms target = null;
		SourceAndTargetHelpers.RetrieveTargetFromGridAndDraggedFromTree<DataLinkObjectExtendedForTerms>(sender, e, out target, out dragged);
		if (!CheckIfDragDropIsValid(sender, e))
		{
			return;
		}
		_ = string.Empty;
		string text = target?.FullName ?? objectDBTreeNode.TreeDisplayName;
		if (CustomMessageBoxForm.Show("Do you want to add link to <b>" + dragged.DatabaseTitle + "." + dragged.Name + "</b> to <b>" + text + "</b>?", "Add link to Business Glossary", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			object obj = target?.ElementType ?? target?.ObjectType;
			if (obj == null)
			{
				obj = SharedObjectTypeEnum.TypeToString(objectDBTreeNode.ObjectType);
			}
			string objectType = (string)obj;
			DB.BusinessGlossary.InsertDataLink(new DataLinkObjectBase(dragged.Id, objectType, objectDBTreeNode.Id, target?.ElementId));
			dataLinksManager.RefreshDataLinks(forceRefresh: true);
			dataLinksManager.RefreshColumnsLinks();
		}
	}

	private void Grid_DragEnter(object sender, DragEventArgs e)
	{
		SetEffect(sender, e);
	}

	private void Grid_DragLeave(object sender, EventArgs e)
	{
	}

	private void Grid_DragOver(object sender, DragEventArgs e)
	{
		SetEffect(sender, e);
	}

	private void SetEffect(object sender, DragEventArgs e)
	{
		if (CheckIfDragDropIsValid(sender, e))
		{
			e.Effect = DragDropEffects.Link;
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private bool CheckIfDragDropIsValid(object sender, DragEventArgs e)
	{
		DBTreeNode dragged = null;
		DataLinkObjectExtendedForTerms target = null;
		SourceAndTargetHelpers.RetrieveTargetFromGridAndDraggedFromTree<DataLinkObjectExtendedForTerms>(sender, e, out target, out dragged);
		return CheckIfDragDropIsValid(dragged, target);
	}

	private bool CheckIfDragDropIsValid(DBTreeNode dragged, DataLinkObjectExtendedForTerms target)
	{
		if (dragged == null || !MatchingDragDropTypes.IsValidTermToObjectDragDrop(dragged.ObjectType, (target == null) ? new SharedObjectTypeEnum.ObjectType?(objectDBTreeNode.ObjectType) : ((target != null && target.ElementType != null) ? SharedObjectTypeEnum.StringToType(target.ElementType) : SharedObjectTypeEnum.StringToType(target.ObjectType))))
		{
			return false;
		}
		return true;
	}
}
