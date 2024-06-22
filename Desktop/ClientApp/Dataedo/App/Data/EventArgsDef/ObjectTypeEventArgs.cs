using System;
using Dataedo.App.MenuTree;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.EventArgsDef;

internal class ObjectTypeEventArgs : EventArgs
{
	public DBTreeNode ParentNode { get; set; }

	public SharedObjectTypeEnum.ObjectType? ObjectType { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype? Subtype { get; set; }

	public DBTreeNode Node { get; set; }

	public bool IsButtonEnabled { get; set; }

	public bool Available { get; set; }

	public int DatabaseId { get; set; }

	public bool IsFromModule
	{
		get
		{
			DBTreeNode parentNode = ParentNode;
			if (parentNode == null)
			{
				return false;
			}
			return parentNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module;
		}
	}

	public string Name { get; set; }

	public SharedDatabaseTypeEnum.DatabaseType? DatabaseType { get; set; }

	public bool IsBusinessGlossary
	{
		get
		{
			if (ObjectType != SharedObjectTypeEnum.ObjectType.BusinessGlossary)
			{
				DBTreeNode rootNode = Node.GetRootNode();
				if (rootNode == null)
				{
					return false;
				}
				return rootNode.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary;
			}
			return true;
		}
	}

	public ObjectTypeEventArgs(DBTreeNode node, bool isSynchButtonEnabled = false)
	{
		ObjectType = node.ObjectType;
		Subtype = node.Subtype;
		IsButtonEnabled = isSynchButtonEnabled;
		if (node.ParentNode != null && node.ParentNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module)
		{
			ParentNode = node.ParentNode.ParentNode;
		}
		else
		{
			ParentNode = node.ParentNode;
		}
		Available = node.Available;
		DatabaseId = node.DatabaseId;
		DatabaseType = node.DatabaseType;
		Node = node;
	}

	public ObjectTypeEventArgs(bool isSynchButtonEnabled)
	{
		IsButtonEnabled = isSynchButtonEnabled;
	}
}
