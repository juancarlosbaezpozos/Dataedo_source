using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.Licences;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.UserControls.WindowControls.MetadataEditorUserControlHelpers;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.Documentations;
using Dataedo.Model.Data.Modules;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.MenuTree;

public class DBTreeMenu
{
	public static TreeList metadataTreeList;

	public static DBTree dbTree = new DBTree();

	public static TreeListHelpers treeListHelpers;

	public void AddBusinessGlossary(int databaseId, Form owner = null)
	{
		metadataTreeList.BeginUpdate();
		try
		{
			DBTreeNode dBTreeNode = new DBTreeNode(null, DB.Database.GetDataByIdForMenu(databaseId), SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectSubtypeEnum.ObjectSubtype.None);
			dbTree.First().Nodes.Add(dBTreeNode);
			ReloadBusinessGlossaryData(dBTreeNode);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error when retrieving Business Glossary's data.", owner);
		}
		metadataTreeList.EndUpdate();
	}

	public static void ReloadDocumentationTerms(DBTreeNode folderNode)
	{
		ReloadBusinessGlossaryFolderData(folderNode);
	}

	public static void ReloadTermTerms(DBTreeNode termNode)
	{
		termNode.Nodes.Clear();
		PrepareBusinessGlossaryLevelData(termNode, termNode, DB.BusinessGlossary.GetAllTerms(termNode.DatabaseId)?.GroupBy((TermObject x) => x.ParentId));
	}

	private void ReloadBusinessGlossaryData(DBTreeNode businessGlossaryNode)
	{
		businessGlossaryNode.Nodes.Clear();
		if (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary))
		{
			ReloadBusinessGlossaryFolderData(PrepareObjectFolder(businessGlossaryNode, SharedObjectTypeEnum.ObjectType.Term, addFolderToParentNode: true));
		}
	}

	private static void ReloadBusinessGlossaryFolderData(DBTreeNode termsFolder)
	{
		termsFolder.Nodes.Clear();
		PrepareBusinessGlossaryLevelData(termsFolder, null, DB.BusinessGlossary.GetAllTerms(termsFolder.DatabaseId)?.GroupBy((TermObject x) => x.ParentId));
	}

	private static void PrepareBusinessGlossaryLevelData(DBTreeNode termsFolder, DBTreeNode parentNode, IEnumerable<IGrouping<int?, TermObject>> groupedTerms)
	{
		if (groupedTerms == null)
		{
			return;
		}
		foreach (IGrouping<int?, TermObject> item in groupedTerms.Where((IGrouping<int?, TermObject> x) => x.Key == parentNode?.Id))
		{
			foreach (TermObject item2 in item.OrderBy((TermObject x) => x.Title))
			{
				DBTreeNode dBTreeNode = new DBTreeNode(termsFolder, item2.TermId.Value, item2.Title, SharedObjectTypeEnum.ObjectType.Term, "term", item2.DatabaseId.Value);
				dBTreeNode.CustomSubtype = item2.TypeTitle;
				dBTreeNode.CustomInfo = item2.TypeIconId;
				termsFolder.Nodes.Add(dBTreeNode);
				PrepareBusinessGlossaryLevelData(dBTreeNode, dBTreeNode, groupedTerms);
			}
		}
	}

	public DBTreeMenu(TreeList metadataTreeListParam = null, TreeListHelpers treeListHelpersParam = null)
	{
		metadataTreeList = metadataTreeListParam;
		treeListHelpers = treeListHelpersParam;
	}

	public static void RefreshSynchState(SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype, string name, string schema, int databaseId, SynchronizeStateEnum.SynchronizeState synchronizeState)
	{
		metadataTreeList?.Invoke((Action)delegate
		{
			FindDBNodesByName(objectType, name, schema, databaseId).ToList().ForEach(delegate(DBTreeNode node)
			{
				node.SynchronizeState = synchronizeState;
				node.ImageName = ChooseStateImage(objectType, subtype, synchronizeState);
				node.Source = UserTypeEnum.UserType.DBMS;
				node.Subtype = subtype;
			});
		});
	}

	public static void RefeshNodeTitle(int databaseId, object title, SharedObjectTypeEnum.ObjectType objectType, string name, string schema, SharedDatabaseTypeEnum.DatabaseType? databasetType, bool withSchema)
	{
		metadataTreeList.Invoke((Action)delegate
		{
			metadataTreeList.BeginUpdate();
			IEnumerable<DBTreeNode> enumerable = FindDBNodesByName(objectType, name, schema, databaseId);
			foreach (DBTreeNode item in enumerable)
			{
				if (item.ParentNode != null && item.DatabaseId != item.ParentNode.DatabaseId)
				{
					item.Name = SetOutsideDocumentationName(schema, name, title, item.DatabaseTitle, objectType, databasetType, withSchema);
				}
				else
				{
					item.Name = SetName(schema, name, title, objectType, databasetType, withSchema);
				}
			}
			enumerable.ToList().ForEach(delegate(DBTreeNode node)
			{
				node.Title = title?.ToString();
			});
			metadataTreeList.EndUpdate();
		});
	}

	public static void RefreshNodeName(int databaseId, SharedObjectTypeEnum.ObjectType objectType, string name, string oldName, string schema, string oldSchema, string title, SharedDatabaseTypeEnum.DatabaseType? databasetType, bool withSchema, SharedObjectSubtypeEnum.ObjectSubtype subtype)
	{
		metadataTreeList.BeginUpdate();
		IEnumerable<DBTreeNode> enumerable = FindDBNodesByName(objectType, oldName, oldSchema, databaseId);
		foreach (DBTreeNode item in enumerable)
		{
			if (item.ParentNode != null && item.DatabaseId != item.ParentNode.DatabaseId)
			{
				item.Name = SetOutsideDocumentationName(schema, name, title, item.DatabaseTitle, objectType, databasetType, withSchema);
			}
			else
			{
				item.Name = SetName(schema, name, title, objectType, databasetType, withSchema);
			}
		}
		foreach (DBTreeNode item2 in enumerable)
		{
			item2.BaseName = name;
			item2.Schema = schema;
			item2.Title = title;
			item2.Subtype = subtype;
		}
		metadataTreeList.EndUpdate();
	}

	public static void SetSource(int databaseId, SharedObjectTypeEnum.ObjectType objectType, string oldName, string oldSchema, UserTypeEnum.UserType source)
	{
		foreach (DBTreeNode item in FindDBNodesByName(objectType, oldName, oldSchema, databaseId))
		{
			item.Source = source;
		}
	}

	public static void RefeshNodeTitle(int objectId, object title, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (objectId <= 0)
		{
			return;
		}
		if (metadataTreeList.InvokeRequired)
		{
			metadataTreeList.Invoke((Action)delegate
			{
				DBTreeNode dBTreeNode2 = FindDBNodeById(objectType, objectId);
				if (dBTreeNode2 != null)
				{
					dBTreeNode2.Name = title as string;
				}
			});
		}
		else
		{
			DBTreeNode dBTreeNode = FindDBNodeById(objectType, objectId);
			if (dBTreeNode != null)
			{
				dBTreeNode.Name = title as string;
			}
		}
	}

	public static void RefeshNodeTitle(int objectId, object title, SharedObjectTypeEnum.ObjectType objectType, object customInfo)
	{
		if (objectId > 0)
		{
			DBTreeNode dBTreeNode = FindDBNodeById(objectType, objectId);
			dBTreeNode.Name = title as string;
			dBTreeNode.CustomInfo = customInfo;
			metadataTreeList.RefreshNode(metadataTreeList.FocusedNode);
		}
	}

	public static void RefreshNodeName(int objectId, string baseName, string name, SharedObjectTypeEnum.ObjectType objectType)
	{
		DBTreeNode dBTreeNode = FindDBNodeById(objectType, objectId);
		dBTreeNode.BaseName = baseName;
		dBTreeNode.Name = name;
	}

	public static void RefreshNodeNameInModule(int objectId, int moduleId, string baseName, string name, SharedObjectTypeEnum.ObjectType objectType)
	{
		DBTreeNode dBTreeNode = FindDBNodeByIdInModule(objectType, objectId, moduleId);
		dBTreeNode.BaseName = baseName;
		dBTreeNode.Name = name;
	}

	public static string SetNameOnlySchema(ObjectRow row, bool withSchema)
	{
		return SetNameOnlySchema(row.Schema, row.Name, withSchema);
	}

	public static string SetNameOnlySchema(string schema, string name, bool withSchema)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (withSchema && !string.IsNullOrEmpty(schema))
		{
			stringBuilder.Append(schema).Append(".");
		}
		stringBuilder.Append(name);
		return stringBuilder.ToString();
	}

	public static string SetName(string schema, string name, object title, SharedObjectTypeEnum.ObjectType objectType, SharedDatabaseTypeEnum.DatabaseType? databasetType, bool withSchema)
	{
		if (objectType == SharedObjectTypeEnum.ObjectType.Module || objectType == SharedObjectTypeEnum.ObjectType.Term)
		{
			return title.ToString();
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(SetNameOnlySchema(schema, name, withSchema));
		if (ValidateFields.IsFieldNotEmpty(title))
		{
			stringBuilder.Append(" (").Append(title).Append(")");
		}
		return stringBuilder.ToString();
	}

	public static string SetOutsideDocumentationName(string schema, string name, object title, string documentationTitle, SharedObjectTypeEnum.ObjectType objectType, SharedDatabaseTypeEnum.DatabaseType? databasetType, bool withSchema)
	{
		if (objectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			return title.ToString();
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(documentationTitle + ".");
		stringBuilder.Append(SetNameOnlySchema(schema, name, withSchema));
		if (ValidateFields.IsFieldNotEmpty(title))
		{
			stringBuilder.Append(" (").Append(title).Append(")");
		}
		return stringBuilder.ToString();
	}

	public static void AddNewObjectToTree(ObjectRow objectRow, bool withSchema)
	{
		DBTreeNode dBTreeNode = FindDBFolder(objectRow.Type.Value, objectRow.DatabaseId.Value);
		if (dBTreeNode != null)
		{
			DBTreeNode dBTreeNode2 = new DBTreeNode(dBTreeNode, objectRow.ObjectId, SetName(objectRow.Schema, objectRow.Name, null, dBTreeNode.ObjectType, DatabaseTypeEnum.StringToType(DB.Database.GetDatabaseTypeById(objectRow.DatabaseId.Value)), withSchema), objectRow.Type.Value, ChooseStateImage(objectRow.Type.Value, objectRow.Subtype.Value, SynchronizeStateEnum.SynchronizeState.New), objectRow.DatabaseId.Value, objectRow.Name, objectRow.Schema, SynchronizeStateEnum.SynchronizeState.New, objectRow.Subtype.Value);
			if (dBTreeNode2.ObjectType == SharedObjectTypeEnum.ObjectType.Table || dBTreeNode2.ObjectType == SharedObjectTypeEnum.ObjectType.View || dBTreeNode2.ObjectType == SharedObjectTypeEnum.ObjectType.Function || dBTreeNode2.ObjectType == SharedObjectTypeEnum.ObjectType.Procedure || dBTreeNode2.ObjectType == SharedObjectTypeEnum.ObjectType.Structure)
			{
				dBTreeNode2.Source = UserTypeEnum.UserType.DBMS;
			}
			dBTreeNode.Nodes.Add(dBTreeNode2);
		}
	}

	public static void AddManualObjectToTree(int databaseId, int tableId, string schema, string name, string title, SharedObjectTypeEnum.ObjectType type, SharedObjectSubtypeEnum.ObjectSubtype subtype, UserTypeEnum.UserType source, DBTreeNode moduleNode = null, SynchronizeStateEnum.SynchronizeState synchronizeState = SynchronizeStateEnum.SynchronizeState.Synchronized)
	{
		DBTreeNode dBTreeNode = ((moduleNode == null) ? FindDBFolder(type, databaseId) : moduleNode);
		if (dBTreeNode == null)
		{
			dBTreeNode = PrepareObjectFolder(FindDBNodeById(SharedObjectTypeEnum.ObjectType.Database, databaseId), type, addFolderToParentNode: true);
		}
		if (dBTreeNode != null)
		{
			DBTreeNode dBTreeNode2 = new DBTreeNode(dBTreeNode, tableId, SetName(schema, name, title, dBTreeNode.ObjectType, DatabaseTypeEnum.StringToType(DB.Database.GetDatabaseTypeById(databaseId)), dBTreeNode.ShowSchema), type, (source == UserTypeEnum.UserType.USER) ? "table_user" : "table", databaseId, name, schema, synchronizeState, subtype);
			dBTreeNode2.Title = title;
			dBTreeNode2.Source = source;
			dBTreeNode2.Subtype = subtype;
			dBTreeNode.Nodes.Add(dBTreeNode2);
			if (moduleNode == null && treeListHelpers != null)
			{
				RefreshFolderInDataBase(dBTreeNode);
			}
		}
	}

	public static DBTreeNode FindMainObjectsFolder(DBTreeNode targetDBNode)
	{
		if ((targetDBNode == null || targetDBNode.ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database) && ((targetDBNode != null && targetDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database) || (targetDBNode != null && targetDBNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module)))
		{
			return targetDBNode;
		}
		if (targetDBNode?.ParentNode != null)
		{
			return FindMainObjectsFolder(targetDBNode.ParentNode);
		}
		return null;
	}

	public static void RefreshFolderInDataBase(DBTreeNode node)
	{
		TreeListNode treeListNode = metadataTreeList.Nodes.FirstOrDefault();
		if (treeListNode != null && node != null)
		{
			treeListNode = treeListNode.Nodes.Where((TreeListNode y) => treeListHelpers.GetNode(y).DatabaseId == node.DatabaseId).FirstOrDefault();
			TreeListNode treeListNode2 = treeListNode.Nodes.Where((TreeListNode y) => treeListHelpers.GetNode(y).ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database && treeListHelpers.GetNode(y).BaseName.Equals(node.BaseName)).FirstOrDefault();
			if (treeListNode2 != null)
			{
				metadataTreeList.RefreshNode(treeListNode2);
			}
		}
	}

	public static void RefreshModuleFolderInDataBase(DBTreeNode node)
	{
		TreeListNode treeListNode = metadataTreeList.Nodes.FirstOrDefault();
		if (treeListNode != null && node != null)
		{
			TreeListNode treeListNode2 = ((treeListNode.Nodes.Where((TreeListNode y) => treeListHelpers.GetNode(y).DatabaseId == node.DatabaseId).FirstOrDefault()?.Nodes?.Where((TreeListNode y) => treeListHelpers.GetNode(y).ContainedObjectsObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database).FirstOrDefault())?.Nodes?.Where((TreeListNode y) => treeListHelpers.GetNode(y).BaseName == node.ParentNode.BaseName).FirstOrDefault())?.Nodes?.Where((TreeListNode y) => treeListHelpers.GetNode(y).BaseName == node.BaseName && treeListHelpers.GetNode(y).ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module).FirstOrDefault();
			if (treeListNode2 != null)
			{
				metadataTreeList.RefreshNode(treeListNode2);
			}
		}
	}

	public static void AddObjectToModule(int databaseId, int tableId, int moduleId, string schema, string name, string title, UserTypeEnum.UserType source, SharedObjectTypeEnum.ObjectType type, SharedObjectSubtypeEnum.ObjectSubtype subtype, int? selectedObjectDatabaseId = null)
	{
		DBTreeNode dBTreeNode = FindModuleFolder(databaseId, type, moduleId);
		if (dBTreeNode == null)
		{
			dBTreeNode = PrepareObjectFolder(FindDBNodeById(SharedObjectTypeEnum.ObjectType.Module, moduleId), type, addFolderToParentNode: true);
		}
		int databaseId2 = (selectedObjectDatabaseId.HasValue ? selectedObjectDatabaseId.Value : databaseId);
		DBTreeNode dBTreeNode2 = FindDBNodeByIdInModule(type, tableId, moduleId);
		if (dBTreeNode != null && dBTreeNode2 == null)
		{
			DBTreeNode dBTreeNode3 = new DBTreeNode(dBTreeNode, tableId, SetName(schema, name, title, dBTreeNode.ObjectType, DatabaseTypeEnum.StringToType(DB.Database.GetDatabaseTypeById(databaseId)), withSchema: true), type, string.Empty, databaseId2, name, schema);
			dBTreeNode3.Title = title;
			dBTreeNode3.Source = source;
			dBTreeNode3.ObjectType = type;
			dBTreeNode3.Subtype = subtype;
			dBTreeNode.Nodes.Add(dBTreeNode3);
			RefreshModuleFolderInDataBase(dBTreeNode);
		}
	}

	private static string ChooseStateImage(SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype, SynchronizeStateEnum.SynchronizeState synchronizeState)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(SharedObjectSubtypeEnum.TypeToStringForSettings(objectType, subtype));
		if (synchronizeState != SynchronizeStateEnum.SynchronizeState.Synchronized)
		{
			stringBuilder.Append("_").Append(SynchronizeStateEnum.StateToStringForImage(synchronizeState));
		}
		return stringBuilder.ToString();
	}

	public void AddDatabase(int databaseId, Form owner = null)
	{
		metadataTreeList.BeginUpdate();
		try
		{
			DBTreeNode dBTreeNode = new DBTreeNode(null, DB.Database.GetDataByIdForMenu(databaseId), SharedObjectTypeEnum.ObjectType.Database, SharedObjectSubtypeEnum.ObjectSubtype.None);
			dbTree.First().Nodes.Add(dBTreeNode);
			ReloadDatabaseData(dBTreeNode);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error when retrieving database's data.", owner);
		}
		metadataTreeList.EndUpdate();
	}

	public DBTree LoadDBData(Form owner = null)
	{
		metadataTreeList.BeginUpdate();
		dbTree.Clear();
		try
		{
			DBTreeNode dBTreeNode = new DBTreeNode();
			dBTreeNode.Name = (StaticData.IsProjectFile ? Path.GetFileName(LastConnectionInfo.LOGIN_INFO.ProjectFilePath) : (LastConnectionInfo.LOGIN_INFO.DataedoDatabase + "@" + LastConnectionInfo.LOGIN_INFO.DataedoHost));
			dBTreeNode.ObjectType = SharedObjectTypeEnum.ObjectType.Repository;
			dbTree.Add(dBTreeNode);
			foreach (DBTreeNode item in (from database in DB.Database.GetDataForMenu()
				select new DBTreeNode(null, database, SharedObjectTypeEnum.ObjectType.Database, SharedObjectSubtypeEnum.ObjectSubtype.None)).ToList())
			{
				if (item.ObjectType == SharedObjectTypeEnum.ObjectType.Database)
				{
					dBTreeNode.Nodes.Add(item);
					ReloadDatabaseData(item);
				}
				else if (item.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
				{
					dBTreeNode.Nodes.Add(item);
					if (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary))
					{
						ReloadBusinessGlossaryData(item);
					}
				}
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error when retrieving databases' data", owner);
		}
		metadataTreeList.EndUpdate();
		return dbTree;
	}

	public bool OnlyDefaultRepository()
	{
		if (dbTree.Count == 2 && dbTree[0].DatabaseId == 0)
		{
			return dbTree[1].DatabaseId == 1;
		}
		return false;
	}

	public static DBTreeNode FindModuleFolder(int databaseId, SharedObjectTypeEnum.ObjectType objectType, int moduleId)
	{
		return (from x in dbTree.Flatten((DBTreeNode i) => i.Nodes)
			where x.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module && SharedObjectTypeEnum.StringToTypeForMenu(x.Name) == objectType && x != null && x.ParentNode.Id == moduleId && x != null && x.ParentNode?.ObjectType == SharedObjectTypeEnum.ObjectType.Module
			select x).FirstOrDefault();
	}

	public static DBTreeNode[] FindDatabaseModulesFolders(int databaseId, SharedObjectTypeEnum.ObjectType objectType)
	{
		return (from x in dbTree.Flatten((DBTreeNode i) => i.Nodes)
			where x.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module && SharedObjectTypeEnum.StringToTypeForMenu(x.Name) == objectType && x != null && x.ParentNode?.ObjectType == SharedObjectTypeEnum.ObjectType.Module && x != null && x.ParentNode?.ParentNode.DatabaseId == databaseId
			select x).ToArray();
	}

	public static DBTreeNode FindDBNodeById(SharedObjectTypeEnum.ObjectType objectType, int objectId)
	{
		return (from x in dbTree.Flatten((DBTreeNode i) => i.Nodes)
			where x.ObjectType == objectType && x.Id == objectId
			select x).FirstOrDefault();
	}

	public static IEnumerable<DBTreeNode> FindNodesById(SharedObjectTypeEnum.ObjectType objectType, int objectId)
	{
		List<DBTreeNode> list = new List<DBTreeNode>();
		DBTreeNode dBTreeNode = FindDBNodeById(objectType, objectId);
		IEnumerable<DBTreeNode> collection = FindNodesInModules(dBTreeNode);
		list.Add(dBTreeNode);
		list.AddRange(collection);
		return list;
	}

	public static DBTreeNode FindDBNodeByIdInModule(SharedObjectTypeEnum.ObjectType objectType, int objectId, int? moduleId)
	{
		return (from x in dbTree.Flatten((DBTreeNode i) => i.Nodes)
			where x.ObjectType == objectType && x.Id == objectId && x.ParentNode.Id == moduleId
			select x).FirstOrDefault();
	}

	public static IEnumerable<DBTreeNode> FindDBNodesByName(SharedObjectTypeEnum.ObjectType objectType, string name, string schema, int databaseId)
	{
		return from x in dbTree.Flatten((DBTreeNode i) => i.Nodes)
			where x.ObjectType == objectType && x.BaseName == name && x.Schema == schema && x.DatabaseId == databaseId
			select x;
	}

	private static DBTreeNode FindDBFolder(SharedObjectTypeEnum.ObjectType objectType, int databaseId)
	{
		string folderName = SharedObjectTypeEnum.TypeToStringForMenu(objectType);
		return (from x in dbTree.Flatten((DBTreeNode i) => i.Nodes)
			where x.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database && x.BaseName == folderName && x.DatabaseId == databaseId
			select x).FirstOrDefault();
	}

	public static IEnumerable<DBTreeNode> FindNodesInModules(DBTreeNode node)
	{
		string folderName = SharedObjectTypeEnum.TypeToStringForMenu(node.ObjectType);
		return from y in (from y in dbTree.Flatten((DBTreeNode x) => x.Nodes)
				where y.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module && y.DatabaseId == node.DatabaseId && y.BaseName == folderName
				select y).Flatten((DBTreeNode x) => x.Nodes)
			where y.Id == node.Id
			select y;
	}

	public static IEnumerable<DBTreeNode> FindLowestLevelDBNodes(DBTreeNode mainNode = null)
	{
		DBTree sequence = ((mainNode != null) ? mainNode.Nodes : dbTree);
		return from x in sequence.Flatten((DBTreeNode i) => i.Nodes)
			where x.ObjectType == SharedObjectTypeEnum.ObjectType.Table || x.ObjectType == SharedObjectTypeEnum.ObjectType.View || x.ObjectType == SharedObjectTypeEnum.ObjectType.Structure || x.ObjectType == SharedObjectTypeEnum.ObjectType.Procedure || x.ObjectType == SharedObjectTypeEnum.ObjectType.Function || x.ObjectType == SharedObjectTypeEnum.ObjectType.Module || x.ObjectType == SharedObjectTypeEnum.ObjectType.Term
			select x;
	}

	public static void DeleteNode(DBTreeNode node)
	{
		node.Owner.Remove(node);
	}

	public void RefeshDBData(DBTreeNode node, bool showProgress, ProgressTypeModel progressType)
	{
		switch (node.ObjectType)
		{
		case SharedObjectTypeEnum.ObjectType.Database:
			ReloadDatabaseData(node);
			break;
		case SharedObjectTypeEnum.ObjectType.BusinessGlossary:
			ReloadBusinessGlossaryData(node);
			break;
		case SharedObjectTypeEnum.ObjectType.Module:
			ReloadModuleData(node);
			break;
		case SharedObjectTypeEnum.ObjectType.Term:
			ReloadTermTerms(node);
			break;
		}
		if (node.IsFolder)
		{
			ReloadFolderData(SharedObjectTypeEnum.StringToTypeForMenu(node.Name).Value, node);
		}
	}

	private void ReloadDatabaseData(DBTreeNode databaseNode)
	{
		databaseNode.Nodes.Clear();
		if (databaseNode.Available && !databaseNode.IsWelcomeDocumentation)
		{
			AddModules(databaseNode);
			AddObjects(databaseNode, SharedObjectTypeEnum.ObjectType.Table);
			AddObjects(databaseNode, SharedObjectTypeEnum.ObjectType.View);
			AddObjects(databaseNode, SharedObjectTypeEnum.ObjectType.Procedure);
			AddObjects(databaseNode, SharedObjectTypeEnum.ObjectType.Function);
			AddObjects(databaseNode, SharedObjectTypeEnum.ObjectType.Structure);
		}
	}

	private static DBTreeNode AddFolder(DBTreeNode parentNode, string name, SharedObjectTypeEnum.ObjectType folderOwnerType)
	{
		string empty = string.Empty;
		empty = ((folderOwnerType != SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database) ? "folder" : "folder_module");
		return new DBTreeNode(parentNode, parentNode.Id, name, folderOwnerType, empty, parentNode.DatabaseId);
	}

	private void AddModules(DBTreeNode parentNode)
	{
		DBTreeNode dBTreeNode = AddFolder(parentNode, SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Module), SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database);
		parentNode.Nodes.Add(dBTreeNode);
		ReloadModules(dBTreeNode);
	}

	public static DBTreeNode PrepareObjectFolder(DBTreeNode parentNode, SharedObjectTypeEnum.ObjectType objectType, bool addFolderToParentNode)
	{
		string name = SharedObjectTypeEnum.TypeToStringForMenu(objectType);
		DBTreeNode dBTreeNode = ((parentNode.ObjectType == SharedObjectTypeEnum.ObjectType.Database || parentNode.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary) ? AddFolder(parentNode, name, SharedObjectTypeEnum.ObjectType.Folder_Database) : AddFolder(parentNode, name, SharedObjectTypeEnum.ObjectType.Folder_Module));
		dBTreeNode.ContainedObjectsObjectType = objectType;
		if (addFolderToParentNode)
		{
			parentNode.Nodes.Add(dBTreeNode);
		}
		return dBTreeNode;
	}

	public static void AddObjects(DBTreeNode parentNode, SharedObjectTypeEnum.ObjectType objectType, IEnumerable<ObjectForMenuObjectWithModuleId> objectsToAdd = null)
	{
		DBTreeNode dBTreeNode = PrepareObjectFolder(parentNode, objectType, addFolderToParentNode: false);
		if (objectsToAdd == null)
		{
			ReloadFolderData(objectType, dBTreeNode);
		}
		else
		{
			ReloadDataForModuleFolder(dBTreeNode, objectsToAdd, objectType);
		}
		if (parentNode.DatabaseClass == SharedDatabaseClassEnum.DatabaseClass.DataLake)
		{
			if ((objectType != SharedObjectTypeEnum.ObjectType.Structure || (parentNode != null && parentNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module)) && !dBTreeNode.Nodes.Any())
			{
				return;
			}
		}
		else if (parentNode != null && parentNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module && !dBTreeNode.Nodes.Any())
		{
			return;
		}
		parentNode.Nodes.Add(dBTreeNode);
	}

	private static void ReloadFolderData(SharedObjectTypeEnum.ObjectType objectType, DBTreeNode folderNode)
	{
		folderNode.Nodes.Clear();
		if (folderNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database || folderNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database)
		{
			switch (objectType)
			{
			case SharedObjectTypeEnum.ObjectType.Module:
				ReloadModules(folderNode);
				break;
			case SharedObjectTypeEnum.ObjectType.Table:
				ReloadDBTables(folderNode);
				break;
			case SharedObjectTypeEnum.ObjectType.View:
				ReloadDBViews(folderNode);
				break;
			case SharedObjectTypeEnum.ObjectType.Procedure:
				ReloadDBProcedures(folderNode);
				break;
			case SharedObjectTypeEnum.ObjectType.Function:
				ReloadDBFunctions(folderNode);
				break;
			case SharedObjectTypeEnum.ObjectType.Term:
				ReloadDocumentationTerms(folderNode);
				break;
			case SharedObjectTypeEnum.ObjectType.Structure:
				ReloadDBTables(folderNode, objectType);
				break;
			}
		}
		else if (folderNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module)
		{
			switch (objectType)
			{
			case SharedObjectTypeEnum.ObjectType.Table:
				ReloadModuleTables(folderNode);
				break;
			case SharedObjectTypeEnum.ObjectType.View:
				ReloadModuleViews(folderNode);
				break;
			case SharedObjectTypeEnum.ObjectType.Procedure:
				ReloadModuleProcedures(folderNode);
				break;
			case SharedObjectTypeEnum.ObjectType.Function:
				ReloadModuleFunctions(folderNode);
				break;
			case SharedObjectTypeEnum.ObjectType.Structure:
				ReloadModuleTables(folderNode, objectType);
				break;
			}
			RefreshModuleFolderInDataBase(folderNode);
		}
	}

	public DBTreeNode AddEmptyModule(DBTreeNode parentNode, int id, string name, bool showProgress, int? nodePositionIndex = null)
	{
		metadataTreeList.BeginUpdate();
		DBTreeNode dBTreeNode = new DBTreeNode(parentNode, id, name, SharedObjectTypeEnum.ObjectType.Module, SharedObjectTypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Module), parentNode.DatabaseId);
		if (nodePositionIndex.HasValue && nodePositionIndex < parentNode.Nodes.Count)
		{
			parentNode.Nodes.Insert(nodePositionIndex.Value, dBTreeNode);
		}
		else
		{
			parentNode.Nodes.Add(dBTreeNode);
		}
		if (showProgress)
		{
			dBTreeNode.SetEmptyNodeProgress();
		}
		metadataTreeList.EndUpdate();
		return dBTreeNode;
	}

	public static void ReloadModules(DBTreeNode folderNode)
	{
		if (folderNode.Nodes.Count() > 0)
		{
			folderNode.Nodes.Clear();
		}
		IEnumerable<DBTreeNode> source = from ModuleForMenuObject module in DB.Module.GetDataByDatabaseForMenu(folderNode.Id)
			select new DBTreeNode(folderNode, module, SharedObjectTypeEnum.ObjectType.Module, SharedObjectSubtypeEnum.ObjectSubtype.None);
		Dictionary<int, List<ObjectForMenuObjectWithModuleId>> modulesTables = (from x in DB.Table.GetObjectsForMenuByDatabaseForModules(folderNode.DatabaseId, SharedObjectTypeEnum.ObjectType.Table)
			group x by x.ModuleId).ToDictionary((IGrouping<int, ObjectForMenuObjectWithModuleId> x) => x.Key, (IGrouping<int, ObjectForMenuObjectWithModuleId> x) => x.ToList());
		Dictionary<int, List<ObjectForMenuObjectWithModuleId>> modulesViews = (from x in DB.Table.GetObjectsForMenuByDatabaseForModules(folderNode.DatabaseId, SharedObjectTypeEnum.ObjectType.View)
			group x by x.ModuleId).ToDictionary((IGrouping<int, ObjectForMenuObjectWithModuleId> x) => x.Key, (IGrouping<int, ObjectForMenuObjectWithModuleId> x) => x.ToList());
		Dictionary<int, List<ObjectForMenuObjectWithModuleId>> modulesStructures = (from x in DB.Table.GetObjectsForMenuByDatabaseForModules(folderNode.DatabaseId, SharedObjectTypeEnum.ObjectType.Structure)
			group x by x.ModuleId).ToDictionary((IGrouping<int, ObjectForMenuObjectWithModuleId> x) => x.Key, (IGrouping<int, ObjectForMenuObjectWithModuleId> x) => x.ToList());
		Dictionary<int, List<ObjectForMenuObjectWithModuleId>> modulesProcedures = (from x in DB.Procedure.GetObjectsForMenuByDatabaseForModules(folderNode.DatabaseId, SharedObjectTypeEnum.ObjectType.Procedure)
			group x by x.ModuleId).ToDictionary((IGrouping<int, ObjectForMenuObjectWithModuleId> x) => x.Key, (IGrouping<int, ObjectForMenuObjectWithModuleId> x) => x.ToList());
		Dictionary<int, List<ObjectForMenuObjectWithModuleId>> modulesFunctions = (from x in DB.Procedure.GetObjectsForMenuByDatabaseForModules(folderNode.DatabaseId, SharedObjectTypeEnum.ObjectType.Function)
			group x by x.ModuleId).ToDictionary((IGrouping<int, ObjectForMenuObjectWithModuleId> x) => x.Key, (IGrouping<int, ObjectForMenuObjectWithModuleId> x) => x.ToList());
		source.ToList().ForEach(delegate(DBTreeNode x)
		{
			folderNode.Nodes.Add(x);
			AddModuleData(x, modulesTables.ContainsKey(x.Id) ? modulesTables[x.Id] : null, modulesViews.ContainsKey(x.Id) ? modulesViews[x.Id] : null, modulesStructures.ContainsKey(x.Id) ? modulesStructures[x.Id] : null, modulesProcedures.ContainsKey(x.Id) ? modulesProcedures[x.Id] : null, modulesFunctions.ContainsKey(x.Id) ? modulesFunctions[x.Id] : null);
		});
	}

	private static void ReloadModuleData(DBTreeNode moduleNode)
	{
		moduleNode.Nodes.Clear();
		AddObjects(moduleNode, SharedObjectTypeEnum.ObjectType.Table);
		AddObjects(moduleNode, SharedObjectTypeEnum.ObjectType.View);
		AddObjects(moduleNode, SharedObjectTypeEnum.ObjectType.Procedure);
		AddObjects(moduleNode, SharedObjectTypeEnum.ObjectType.Function);
		AddObjects(moduleNode, SharedObjectTypeEnum.ObjectType.Structure);
	}

	private static void AddModuleData(DBTreeNode moduleNode, IEnumerable<ObjectForMenuObjectWithModuleId> tablesToAdd, IEnumerable<ObjectForMenuObjectWithModuleId> viewsToAdd, IEnumerable<ObjectForMenuObjectWithModuleId> structuresToAdd, IEnumerable<ObjectForMenuObjectWithModuleId> proceduresToAdd, IEnumerable<ObjectForMenuObjectWithModuleId> functionsToAdd)
	{
		moduleNode.Nodes.Clear();
		if (tablesToAdd != null)
		{
			AddObjects(moduleNode, SharedObjectTypeEnum.ObjectType.Table, tablesToAdd);
		}
		if (viewsToAdd != null)
		{
			AddObjects(moduleNode, SharedObjectTypeEnum.ObjectType.View, viewsToAdd);
		}
		if (structuresToAdd != null)
		{
			AddObjects(moduleNode, SharedObjectTypeEnum.ObjectType.Structure, structuresToAdd);
		}
		if (proceduresToAdd != null)
		{
			AddObjects(moduleNode, SharedObjectTypeEnum.ObjectType.Procedure, proceduresToAdd);
		}
		if (functionsToAdd != null)
		{
			AddObjects(moduleNode, SharedObjectTypeEnum.ObjectType.Function, functionsToAdd);
		}
	}

	public static void ReloadDBTables(DBTreeNode folderNode, SharedObjectTypeEnum.ObjectType objectType = SharedObjectTypeEnum.ObjectType.Table)
	{
		ReloadData(folderNode, DB.Table.GetByDatabaseForMenu(folderNode.Id, objectType), objectType);
	}

	public static void ReloadModuleTables(DBTreeNode folderNode, SharedObjectTypeEnum.ObjectType objectType = SharedObjectTypeEnum.ObjectType.Table)
	{
		ReloadDataForModuleFolder(folderNode, DB.Table.GetByModuleForMenu(folderNode.Id, objectType), objectType);
	}

	public static void ReloadDBViews(DBTreeNode folderNode)
	{
		ReloadData(folderNode, DB.Table.GetByDatabaseForMenu(folderNode.Id, SharedObjectTypeEnum.ObjectType.View), SharedObjectTypeEnum.ObjectType.View);
	}

	public static void ReloadModuleViews(DBTreeNode folderNode)
	{
		ReloadDataForModuleFolder(folderNode, DB.Table.GetByModuleForMenu(folderNode.Id, SharedObjectTypeEnum.ObjectType.View), SharedObjectTypeEnum.ObjectType.View);
	}

	public static void ReloadDBProcedures(DBTreeNode folderNode)
	{
		ReloadData(folderNode, DB.Procedure.GetProceduresByDatabaseForMenu(folderNode.Id), SharedObjectTypeEnum.ObjectType.Procedure);
	}

	public static void ReloadModuleProcedures(DBTreeNode folderNode)
	{
		ReloadDataForModuleFolder(folderNode, DB.Procedure.GetProceduresByModuleForMenu(folderNode.Id), SharedObjectTypeEnum.ObjectType.Procedure);
	}

	public static void ReloadDBFunctions(DBTreeNode folderNode)
	{
		ReloadData(folderNode, DB.Procedure.GetFunctionsByDatabaseForMenu(folderNode.Id), SharedObjectTypeEnum.ObjectType.Function);
	}

	public static void ReloadModuleFunctions(DBTreeNode folderNode)
	{
		ReloadDataForModuleFolder(folderNode, DB.Procedure.GetFunctionsByModuleForMenu(folderNode.Id), SharedObjectTypeEnum.ObjectType.Function);
	}

	private static void ReloadData(DBTreeNode folderNode, IEnumerable<ObjectForMenuObject> dataView, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (dataView == null)
		{
			return;
		}
		foreach (DBTreeNode item in from ObjectForMenuObject dbObject in dataView
			select new DBTreeNode(folderNode, dbObject, objectType, SharedObjectSubtypeEnum.StringToType(objectType, dbObject.Subtype)))
		{
			folderNode.Nodes.Add(item);
		}
	}

	private static void ReloadDataForModuleFolder(DBTreeNode folderNode, IEnumerable<ObjectForMenuObject> dataView, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (dataView == null)
		{
			return;
		}
		foreach (DBTreeNode newObject in from ObjectForMenuObject dbObject in dataView
			select new DBTreeNode(folderNode, dbObject, objectType, SharedObjectSubtypeEnum.StringToType(objectType, PrepareValue.ToString(dbObject.Subtype))))
		{
			DBTreeNode dBTreeNode = folderNode.Nodes.SingleOrDefault((DBTreeNode x) => x.Id == newObject.Id);
			if (dBTreeNode != null)
			{
				dBTreeNode = new DBTreeNode(folderNode, newObject);
				RefreshNodeNameInModule(dBTreeNode.Id, dBTreeNode.ModuleNode.Id, dBTreeNode.BaseName, dBTreeNode.Name, dBTreeNode.ObjectType);
			}
			else
			{
				folderNode.Nodes.Add(newObject);
			}
		}
	}

	public static void RefreshAllModules(int databaseId, SharedObjectTypeEnum.ObjectType objectType, int? currentModuleId = null)
	{
		DBTreeNode[] array = FindDatabaseModulesFolders(databaseId, objectType);
		foreach (DBTreeNode dBTreeNode in array)
		{
			if ((dBTreeNode != null) ? (dBTreeNode.ParentNode.Id == currentModuleId) : (!currentModuleId.HasValue))
			{
				metadataTreeList.SetFocusedNode(metadataTreeList.FocusedNode.ParentNode);
			}
			ReloadFolderData(objectType, dBTreeNode);
		}
	}

	public void RefreshAllModulesAndSelect(int databaseId, SharedObjectTypeEnum.ObjectType objectType, int? moduleId = null, int? objectId = null)
	{
		RefreshAllModules(databaseId, objectType, moduleId);
		if (!moduleId.HasValue || !objectId.HasValue)
		{
			return;
		}
		bool isNotFocused = true;
		TreeListNode nodeOutsideModules = null;
		metadataTreeList.NodesIterator.DoOperation(delegate(TreeListNode x)
		{
			if (isNotFocused)
			{
				DBTreeNode dBTreeNode = metadataTreeList.GetDataRecordByNode(x) as DBTreeNode;
				if (dBTreeNode != null && dBTreeNode.ParentNode?.ParentNode?.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
				{
					DBTreeNode parentNode = dBTreeNode.ParentNode;
					if (((parentNode != null) ? (parentNode.ParentNode?.Id == moduleId) : (!moduleId.HasValue)) && dBTreeNode.ObjectType == objectType && dBTreeNode.Id == objectId)
					{
						metadataTreeList.RefreshNode(x.ParentNode);
						metadataTreeList.FocusedNode = x;
						isNotFocused = false;
					}
				}
				if (dBTreeNode != null && dBTreeNode.ParentNode?.ParentNode?.ObjectType == SharedObjectTypeEnum.ObjectType.Database && dBTreeNode.DatabaseId == databaseId && dBTreeNode.ObjectType == objectType && dBTreeNode.Id == objectId)
				{
					nodeOutsideModules = x;
				}
			}
		});
		if (isNotFocused && nodeOutsideModules != null)
		{
			metadataTreeList.RefreshNode(metadataTreeList.FocusedNode);
			metadataTreeList.FocusedNode = nodeOutsideModules;
		}
	}
}
