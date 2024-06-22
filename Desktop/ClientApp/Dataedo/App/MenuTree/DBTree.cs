using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraTreeList;

namespace Dataedo.App.MenuTree;

public class DBTree : BindingList<DBTreeNode>, TreeList.IVirtualTreeListData
{
	public DBTree()
	{
	}

	public DBTree(IList<DBTreeNode> list)
		: base(list)
	{
	}

	void TreeList.IVirtualTreeListData.VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
	{
		DBTreeNode dBTreeNode = info.Node as DBTreeNode;
		info.Children = dBTreeNode.Nodes;
	}

	protected override void InsertItem(int index, DBTreeNode item)
	{
		item.Owner = this;
		base.InsertItem(index, item);
	}

	void TreeList.IVirtualTreeListData.VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
	{
		DBTreeNode dBTreeNode = info.Node as DBTreeNode;
		string fieldName = info.Column.FieldName;
		if (!(fieldName == "name"))
		{
			if (fieldName == "progress")
			{
				info.CellData = dBTreeNode.ProgressValue;
			}
		}
		else
		{
			info.CellData = dBTreeNode.TreeDisplayName;
		}
	}

	void TreeList.IVirtualTreeListData.VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
	{
		DBTreeNode dBTreeNode = info.Node as DBTreeNode;
		if (info.Column.FieldName == "name")
		{
			dBTreeNode.Name = (string)info.NewCellData;
		}
	}
}
