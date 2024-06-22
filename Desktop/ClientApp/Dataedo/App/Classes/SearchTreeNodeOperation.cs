using System.Collections.Generic;
using System.Linq;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools.FormsSupport;
using Dataedo.Shared.Enums;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.Classes;

public class SearchTreeNodeOperation
{
	private enum SearchTypeEnum
	{
		Id = 0,
		ObjectType = 1,
		Name = 2,
		DataRecord = 3
	}

	public TreeListNode FindNode(TreeListNodes nodes, int? documentationId, int objectId, SharedObjectTypeEnum.ObjectType? objectType)
	{
		TreeList treeList = nodes.FirstNode?.TreeList;
		if (treeList == null)
		{
			return null;
		}
		DrawingHelpers.SuspendDrawing(treeList);
		try
		{
			return FindNodePrivate(nodes, documentationId, objectId, objectType, lookInSubnodes: true, objectType != SharedObjectTypeEnum.ObjectType.Module);
		}
		finally
		{
			DrawingHelpers.ResumeDrawing(treeList);
		}
	}

	public TreeListNode FindNode(TreeListNodes nodes, int objectId, SharedObjectTypeEnum.ObjectType? objectType)
	{
		TreeList treeList = nodes.FirstNode?.TreeList;
		if (treeList == null)
		{
			return null;
		}
		DrawingHelpers.SuspendDrawing(treeList);
		try
		{
			return FindNodePrivate(nodes, null, objectId, objectType, lookInSubnodes: true, objectType != SharedObjectTypeEnum.ObjectType.Module);
		}
		finally
		{
			DrawingHelpers.ResumeDrawing(treeList);
		}
	}

	public TreeListNode FindNode(TreeListNodes nodes, int objectId, params SharedObjectTypeEnum.ObjectType?[] objectTypes)
	{
		TreeList treeList = nodes.FirstNode?.TreeList;
		if (treeList == null)
		{
			return null;
		}
		DrawingHelpers.SuspendDrawing(treeList);
		try
		{
			return FindNodePrivate(nodes, null, objectId, objectTypes, lookInSubnodes: true, !objectTypes.Any((SharedObjectTypeEnum.ObjectType? x) => x == SharedObjectTypeEnum.ObjectType.Module));
		}
		finally
		{
			DrawingHelpers.ResumeDrawing(treeList);
		}
	}

	public TreeListNode FindNodePrivate(TreeListNodes nodes, int? documentationId, int objectId, SharedObjectTypeEnum.ObjectType? objectType, bool lookInSubnodes = true, bool skipModules = true)
	{
		return FindNodePrivate(nodes, documentationId, objectId, new SharedObjectTypeEnum.ObjectType?[1] { objectType }, lookInSubnodes, skipModules);
	}

	public TreeListNode FindNodePrivate(TreeListNodes nodes, int? documentationId, int objectId, SharedObjectTypeEnum.ObjectType?[] objectTypes, bool lookInSubnodes = true, bool skipModules = true)
	{
		if (nodes.Count == 1 && ((DBTreeNode)nodes[0].TreeList.GetDataRecordByNode(nodes[0])).ObjectType == SharedObjectTypeEnum.ObjectType.Repository)
		{
			nodes = nodes[0].Nodes;
		}
		foreach (TreeListNode node in nodes)
		{
			DBTreeNode dBTreeNode = node.TreeList.GetDataRecordByNode(node) as DBTreeNode;
			if ((skipModules && dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module) || (documentationId.HasValue && dBTreeNode.DatabaseId != documentationId))
			{
				continue;
			}
			if (dBTreeNode.Id == objectId && objectTypes.Contains(dBTreeNode.ObjectType))
			{
				return node;
			}
			if (lookInSubnodes)
			{
				bool expanded = node.Expanded;
				node.Expanded = true;
				TreeListNode treeListNode2 = FindNodePrivate(node.Nodes, documentationId, objectId, objectTypes, lookInSubnodes, skipModules);
				node.Expanded = expanded;
				if (treeListNode2 != null)
				{
					return treeListNode2;
				}
			}
		}
		return null;
	}

	private TreeListNode FindNode(TreeListNodes nodes, SearchTypeEnum searchType, int objectId = -1, SharedObjectTypeEnum.ObjectType? objectType = null, string name = null, DBTreeNode dataRecord = null)
	{
		if (nodes == null)
		{
			return null;
		}
		if (nodes.Count == 1 && ((DBTreeNode)nodes[0].TreeList.GetDataRecordByNode(nodes[0])).ObjectType == SharedObjectTypeEnum.ObjectType.Repository)
		{
			nodes = nodes[0].Nodes;
		}
		foreach (TreeListNode node in nodes)
		{
			DBTreeNode dBTreeNode = (DBTreeNode)node.TreeList.GetDataRecordByNode(node);
			switch (searchType)
			{
			case SearchTypeEnum.Id:
				if (dBTreeNode.Id == objectId)
				{
					return node;
				}
				break;
			case SearchTypeEnum.ObjectType:
				if (SharedObjectTypeEnum.StringToTypeForMenu(dBTreeNode.Name) == objectType)
				{
					return node;
				}
				break;
			case SearchTypeEnum.Name:
				if (dBTreeNode.BaseName.Contains(name))
				{
					return node;
				}
				break;
			case SearchTypeEnum.DataRecord:
				if (dBTreeNode == dataRecord)
				{
					return node;
				}
				break;
			}
		}
		return null;
	}

	public List<DBTreeNode> FindNodesByType(TreeListNodes nodes, params SharedObjectTypeEnum.ObjectType[] types)
	{
		List<DBTreeNode> list = new List<DBTreeNode>();
		foreach (TreeListNode node in nodes)
		{
			DBTreeNode dbTreeNode = (DBTreeNode)node.TreeList.GetDataRecordByNode(node);
			if (types.Any((SharedObjectTypeEnum.ObjectType x) => x == dbTreeNode.ObjectType))
			{
				list.Add(dbTreeNode);
			}
		}
		return list;
	}

	public List<TreeListNode> FindRawNodesByType(TreeListNodes nodes, SharedObjectTypeEnum.ObjectType type)
	{
		List<TreeListNode> list = new List<TreeListNode>();
		foreach (TreeListNode node in nodes)
		{
			if (((DBTreeNode)node.TreeList.GetDataRecordByNode(node)).ObjectType == type)
			{
				list.Add(node);
			}
		}
		return list;
	}

	public List<DBTreeNode> FindLowestLevelNodesByTypes(TreeListNodes nodes, List<SharedObjectTypeEnum.ObjectType> types)
	{
		List<DBTreeNode> list = new List<DBTreeNode>();
		foreach (TreeListNode node in nodes)
		{
			foreach (TreeListNode node2 in node.Nodes)
			{
				foreach (TreeListNode node3 in node2.Nodes)
				{
					DBTreeNode dBTreeNode = (DBTreeNode)node3.TreeList.GetDataRecordByNode(node3);
					SharedObjectTypeEnum.ObjectType objectType = dBTreeNode.ObjectType;
					if (objectType == SharedObjectTypeEnum.ObjectType.Folder_Module || objectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database || objectType == SharedObjectTypeEnum.ObjectType.Module)
					{
						foreach (TreeListNode node4 in node3.Nodes)
						{
							foreach (TreeListNode node5 in node4.Nodes)
							{
								DBTreeNode dBTreeNode2 = (DBTreeNode)node5.TreeList.GetDataRecordByNode(node5);
								if (types.Contains(dBTreeNode2.ObjectType))
								{
									list.Add(dBTreeNode2);
								}
							}
						}
					}
					else if (types.Contains(objectType))
					{
						list.Add(dBTreeNode);
					}
				}
			}
		}
		return list;
	}

	public TreeListNode FindNode(TreeListNodes nodes, int objectId)
	{
		return FindNode(nodes, SearchTypeEnum.Id, objectId);
	}

	public TreeListNode FindNode(TreeListNodes nodes, SharedObjectTypeEnum.ObjectType objectType)
	{
		return FindNode(nodes, SearchTypeEnum.ObjectType, -1, objectType);
	}

	public TreeListNode FindNode(TreeListNodes nodes, string name)
	{
		return FindNode(nodes, SearchTypeEnum.Name, -1, null, name);
	}

	public TreeListNode FindNode(TreeListNodes nodes, DBTreeNode node)
	{
		return FindNode(nodes, SearchTypeEnum.DataRecord, -1, null, null, node);
	}

	public TreeListNode FindDependencyNode(TreeListNodes nodes, string baseName, string schema)
	{
		foreach (TreeListNode node in nodes)
		{
			DBTreeNode dBTreeNode = (DBTreeNode)node.TreeList.GetDataRecordByNode(node);
			if (dBTreeNode.BaseName == baseName && dBTreeNode.Schema == schema)
			{
				return node;
			}
		}
		return null;
	}

	public TreeListNode FindNode(TreeListNode focusedNode, TreeListNodes nodes, SharedObjectTypeEnum.ObjectType objectType, int databaseId, int objectId, int? moduleId = null)
	{
		TreeListNode treeListNode = focusedNode;
		if (treeListNode == null)
		{
			TreeListNode treeListNode2 = FindNode(nodes, databaseId);
			if (moduleId.HasValue)
			{
				TreeListNode treeListNode3 = FindNode(treeListNode2.Nodes, SharedObjectTypeEnum.ObjectType.Module);
				treeListNode2 = FindNode(treeListNode3.Nodes, moduleId.Value);
			}
			if (treeListNode2 != null)
			{
				treeListNode2.Expanded = true;
				treeListNode = FindNode(treeListNode2.Nodes, objectType);
			}
		}
		if (treeListNode != null)
		{
			treeListNode.Expanded = true;
			return FindNode(treeListNode.Nodes, objectId);
		}
		return null;
	}

	public TreeListNode FindModuleNode(TreeListNode focusedNode, TreeListNodes nodes, int databaseId, string name)
	{
		TreeListNode treeListNode = focusedNode;
		if (treeListNode != null)
		{
			treeListNode = FindModuleDirectory(nodes, databaseId);
		}
		if (treeListNode != null)
		{
			treeListNode.Expanded = true;
			return FindNode(treeListNode.Nodes, name);
		}
		return null;
	}

	public TreeListNode FindModuleNode(TreeListNode focusedNode, TreeListNodes nodes, int databaseId, DBTreeNode node)
	{
		TreeListNode treeListNode = focusedNode;
		if (treeListNode != null)
		{
			treeListNode = FindModuleDirectory(nodes, databaseId);
		}
		if (treeListNode != null)
		{
			treeListNode.Expanded = true;
			return FindNode(treeListNode.Nodes, node);
		}
		return null;
	}

	public TreeListNode FindModuleDirectory(TreeListNodes nodes, int databaseId)
	{
		TreeListNode treeListNode = FindNode(nodes, databaseId);
		return FindNode(treeListNode.Nodes, SharedObjectTypeEnum.ObjectType.Module);
	}

	public TreeListNode FindModuleDirectory(TreeListNodes nodes)
	{
		return FindNode(nodes, SharedObjectTypeEnum.ObjectType.Module);
	}

	public TreeListNode FindTermsDirectory(TreeListNodes nodes)
	{
		return FindNode(nodes, SharedObjectTypeEnum.ObjectType.Term);
	}
}
