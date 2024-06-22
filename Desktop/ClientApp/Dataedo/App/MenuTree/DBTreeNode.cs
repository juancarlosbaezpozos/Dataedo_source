using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.Tools;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.WindowControls.MetadataEditorUserControlHelpers;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.DataLineage;
using Dataedo.Model.Data.Documentations;
using Dataedo.Model.Data.Interfaces;
using Dataedo.Model.Data.Progress;
using Dataedo.Shared.Enums;

namespace Dataedo.App.MenuTree;

[DebuggerDisplay("{TreeDisplayName}")]
public class DBTreeNode : IFlowDraggable
{
	public enum InsertElementEnum
	{
		OnValidating = 0,
		AlreadyInserted = 1
	}

	private SharedObjectTypeEnum.ObjectType? containedObjectsObjectType;

	private SharedDatabaseTypeEnum.DatabaseType? databaseType;

	private string imageName;

	private int? progressValue;

	public int Id { get; set; }

	public string BaseName { get; set; }

	public string OldBaseName { get; set; }

	public string Title { get; set; }

	public string Schema { get; set; }

	public string OldSchema { get; set; }

	public SharedObjectTypeEnum.ObjectType ContainedObjectsObjectType
	{
		get
		{
			return containedObjectsObjectType ?? ObjectType;
		}
		set
		{
			containedObjectsObjectType = value;
		}
	}

	public SharedObjectTypeEnum.ObjectType ObjectType { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype Subtype { get; set; }

	public string CustomSubtype { get; set; }

	public object CustomInfo { get; set; }

	public UserTypeEnum.UserType? Source { get; set; }

	public bool IsNormalObject => SharedObjectTypeEnum.IsRegularObject(ObjectType);

	public DBTree Owner { get; set; }

	public int DatabaseId { get; set; }

	public string DatabaseTitle { get; set; }

	public SharedDatabaseTypeEnum.DatabaseType? DatabaseType
	{
		get
		{
			return databaseType;
		}
		set
		{
			databaseType = value;
		}
	}

	public SharedDatabaseClassEnum.DatabaseClass? DatabaseClass { get; private set; }

	public SynchronizeStateEnum.SynchronizeState SynchronizeState { get; set; }

	public DBTreeNode ParentNode { get; set; }

	public InsertElementEnum InsertElement { get; set; }

	public bool Available { get; set; }

	public bool IsWelcomeDocumentation => DatabaseId == 0;

	private string name { get; set; }

	public string OldName { get; set; }

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			if (!(Name == value))
			{
				name = value;
				OnChanged();
			}
		}
	}

	public bool Deleted => SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted;

	public string TreeDisplayName
	{
		get
		{
			if (ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database)
			{
				if (ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Database && ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Module)
				{
					return Name;
				}
				return GetNodeCounterSuffix(Name) ?? "";
			}
			return "Subject Areas";
		}
	}

	public string TreeDisplayNameUiEscaped => Escaping.EscapeTextForUI(TreeDisplayName);

	public string ImageName
	{
		get
		{
			return imageName;
		}
		set
		{
			if (!(ImageName == value))
			{
				imageName = value;
				OnChanged();
			}
		}
	}

	public bool IsFolder
	{
		get
		{
			if (ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Database && ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Module)
			{
				return ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database;
			}
			return true;
		}
	}

	public bool IsFolderForCounter
	{
		get
		{
			if (ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Database)
			{
				return ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module;
			}
			return true;
		}
	}

	public bool IsInModule
	{
		get
		{
			DBTreeNode parentNode = ParentNode;
			if (parentNode == null)
			{
				return false;
			}
			return parentNode.ParentNode?.ObjectType == SharedObjectTypeEnum.ObjectType.Module;
		}
	}

	public DBTreeNode ModuleNode
	{
		get
		{
			if (!IsInModule)
			{
				return null;
			}
			return ParentNode?.ParentNode;
		}
	}

	public string NameWithSchema => Schema + "." + BaseName;

	public string DisplayNameWithShema
	{
		get
		{
			if (!string.IsNullOrEmpty(Schema))
			{
				return NameWithSchema;
			}
			return BaseName;
		}
	}

	public bool? IsMultipleSchemasDatabase { get; private set; }

	public bool? DatabaseShowSchema { get; private set; }

	public bool? DatabaseShowSchemaOverride { get; set; }

	public bool ShowSchema => DatabaseRow.GetShowSchema(DatabaseShowSchema, DatabaseShowSchemaOverride);

	public DBTree Nodes { get; set; }

	public bool HasMultipleDatabases => CheckIfAnyFromAnotherDatabase(this);

	public int? ProgressValue
	{
		get
		{
			return progressValue;
		}
		set
		{
			if (SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted)
			{
				progressValue = null;
			}
			else
			{
				progressValue = value;
			}
			OnChanged();
		}
	}

	public int Points { get; set; }

	public int TotalPoints { get; set; }

	public int ColumnsPoints { get; internal set; }

	public int TotalColumnsPoints { get; internal set; }

	public int FKRelationsPoints { get; internal set; }

	public int PKRelationsPoints { get; internal set; }

	public int KeysPoints { get; internal set; }

	public int TriggersPoints { get; internal set; }

	public int ParametersPoints { get; internal set; }

	public int TotalParametersPoints { get; internal set; }

	public int TotalTriggersPoints { get; internal set; }

	public int TotalKeysPoints { get; internal set; }

	public int TotalPKRelationsPoints { get; internal set; }

	public int TotalFKRelationsPoints { get; internal set; }

	public int TotalObjectPoints { get; internal set; }

	public int ObjectPoints { get; internal set; }

	public int PointsSum { get; set; }

	public int TotalPointsSum { get; set; }

	public string DBMSVersion { get; private set; }

	private static int CountAllNodes(DBTree nodes)
	{
		int num = nodes.Count();
		foreach (DBTreeNode node in nodes)
		{
			if (node != null && node.Nodes?.Any() == true)
			{
				num += CountAllNodes(node.Nodes);
			}
		}
		return num;
	}

	private string GetNodeCounterSuffix(string name)
	{
		if (IsFolderForCounter)
		{
			DBTree nodes = Nodes;
			if (nodes != null && nodes.Count() > 0)
			{
				return name = name + " [" + ((BaseName == "Terms") ? CountAllNodes(Nodes) : Nodes.Count()).ToString("N0", TreeListHelpers.NodesCountCulture) + "]";
			}
		}
		return name;
	}

	private void OnChanged()
	{
		if (Owner != null)
		{
			int position = Owner.IndexOf(this);
			Owner.ResetItem(position);
		}
	}

	public DBTreeNode()
	{
		Available = true;
		SetEmptyNodeProgress();
		Nodes = new DBTree();
	}

	private bool CheckIfAnyFromAnotherDatabase(DBTreeNode node)
	{
		if (node.Nodes != null)
		{
			if (node.Nodes.Any(delegate(DBTreeNode x)
			{
				DBTreeNode parentNode = ParentNode;
				if (parentNode != null)
				{
					_ = parentNode.DatabaseId;
					if (true)
					{
						return ParentNode.DatabaseId != x.DatabaseId;
					}
				}
				return false;
			}))
			{
				return true;
			}
			foreach (DBTreeNode node2 in node.Nodes)
			{
				if (CheckIfAnyFromAnotherDatabase(node2))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void ReadDefaultData(DBTreeNode parentNode, ObjectWithModulesObject rowDB, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype)
	{
		SharedObjectTypeEnum.GetKeyFieldName(objectType);
		ParentNode = parentNode;
		Id = rowDB.Id;
		BaseName = rowDB.Name;
		Title = rowDB.Title;
		Schema = rowDB.Schema;
		ObjectType = objectType;
		Subtype = SharedObjectSubtypeEnum.StringToType(objectType, rowDB.Subtype);
		ImageName = SharedObjectSubtypeEnum.TypeToString(ObjectType, Subtype);
		Source = UserTypeEnum.ObjectToType(rowDB.Source);
		DatabaseId = PrepareValue.ToInt(rowDB.DatabaseId).Value;
		DatabaseType = DatabaseTypeEnum.StringToType(rowDB.DatabaseType);
		DatabaseTitle = PrepareValue.ToString(rowDB.DatabaseTitle);
		IsMultipleSchemasDatabase = rowDB.DatabaseMultipleSchemas.GetValueOrDefault();
		DatabaseType = DatabaseTypeEnum.StringToType(rowDB.DatabaseType);
		if (SharedObjectTypeEnum.StringToType(PrepareValue.ToString(rowDB.DatabaseType)) == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
		{
			ObjectType = SharedObjectTypeEnum.ObjectType.BusinessGlossary;
		}
		SynchronizeState = SynchronizeStateEnum.DBStringToState(rowDB.Status);
		Available = true;
		SetName();
		Nodes = new DBTree();
	}

	private void ReadDefaultData(DBTreeNode parentNode, DocumentationForMenuObject rowDB, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype)
	{
		ParentNode = parentNode;
		Id = rowDB.DatabaseId;
		BaseName = rowDB.Name;
		Title = rowDB.Title;
		ObjectType = objectType;
		Subtype = subtype;
		ImageName = SharedObjectSubtypeEnum.TypeToString(ObjectType, Subtype);
		DatabaseId = rowDB.DatabaseId;
		DatabaseType = DatabaseTypeEnum.StringToType(rowDB.Type);
		DatabaseTitle = rowDB.Title;
		IsMultipleSchemasDatabase = PrepareValue.ToBool(rowDB.MultipleSchemas);
		DatabaseShowSchema = rowDB.ShowSchema;
		DatabaseShowSchemaOverride = rowDB.ShowSchemaOverride;
		DatabaseType = DatabaseTypeEnum.StringToType(rowDB.Type);
		if (SharedObjectTypeEnum.StringToType(rowDB.Type) == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
		{
			ObjectType = SharedObjectTypeEnum.ObjectType.BusinessGlossary;
		}
		DBMSVersion = rowDB.DbmsVersion;
		Available = true;
		SetName();
		Nodes = new DBTree();
	}

	private void ReadDefaultData(DBTreeNode parentNode, IMenuObject rowDB, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype)
	{
		ParentNode = parentNode;
		Id = rowDB.Id;
		BaseName = rowDB.BaseName;
		Title = rowDB.Title;
		Schema = rowDB.Schema;
		ObjectType = objectType;
		Subtype = subtype;
		ImageName = SharedObjectSubtypeEnum.TypeToString(ObjectType, Subtype);
		DatabaseId = rowDB.DatabaseId;
		DatabaseType = DatabaseTypeEnum.StringToType(rowDB.DatabaseType);
		DatabaseClass = SharedDatabaseClassEnum.StringToType(rowDB.DatabaseClass);
		DatabaseTitle = rowDB.DatabaseTitle;
		IsMultipleSchemasDatabase = rowDB.DatabaseMultipleSchemas;
		DatabaseShowSchema = rowDB.DatabaseShowSchema;
		DatabaseShowSchemaOverride = parentNode.ShowSchema || rowDB.DatabaseShowSchemaOverride == true || (!rowDB.DatabaseShowSchemaOverride.HasValue && rowDB.DatabaseShowSchema == true);
		SynchronizeState = SynchronizeStateEnum.DBStringToState(rowDB.Status);
		Available = true;
		SetName();
		Nodes = new DBTree();
	}

	public DBTreeNode GetClosestNode(SharedObjectTypeEnum.ObjectType objectType)
	{
		if (ObjectType != objectType)
		{
			return GetParentNode(this, objectType);
		}
		return this;
	}

	private static DBTreeNode GetParentNode(DBTreeNode node, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (node.ParentNode != null)
		{
			if (node.ParentNode.ObjectType == objectType)
			{
				return node.ParentNode;
			}
			return GetParentNode(node.ParentNode, objectType);
		}
		return null;
	}

	public DBTreeNode(DBTreeNode parentNode, ObjectWithModulesObject rowDB, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype)
		: this()
	{
		DatabaseType = parentNode?.DatabaseType;
		DBMSVersion = parentNode?.DBMSVersion;
		ReadDefaultData(parentNode, rowDB, objectType, subtype);
		OldName = Name;
		OldSchema = Schema;
		OldBaseName = BaseName;
		SetName();
	}

	public DBTreeNode(DBTreeNode parentNode, DocumentationForMenuObject rowDB, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype)
		: this()
	{
		DatabaseType = parentNode?.DatabaseType ?? DatabaseTypeEnum.StringToType(rowDB.Type);
		DatabaseClass = parentNode?.DatabaseClass ?? SharedDatabaseClassEnum.StringToType(rowDB.Class);
		DBMSVersion = parentNode?.DBMSVersion;
		ReadDefaultData(parentNode, rowDB, objectType, subtype);
		OldName = Name;
		OldSchema = Schema;
		OldBaseName = BaseName;
		SetName();
	}

	public DBTreeNode(DBTreeNode parentNode, IMenuObject rowDB, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype)
		: this()
	{
		DatabaseType = parentNode?.DatabaseType;
		DBMSVersion = parentNode?.DBMSVersion;
		ReadDefaultData(parentNode, rowDB, objectType, subtype);
		OldName = Name;
		OldSchema = Schema;
		OldBaseName = BaseName;
		if (rowDB is IMenuObjectWithSource)
		{
			Source = UserTypeEnum.ObjectToType((rowDB as ObjectForMenuObject).Source);
		}
		SetName();
	}

	public DBTreeNode(DBTreeNode parentNode, DBTreeNode dbTreeNode)
		: this()
	{
		ParentNode = parentNode;
		Id = dbTreeNode.Id;
		Name = dbTreeNode.name;
		BaseName = dbTreeNode.BaseName;
		Title = dbTreeNode.Title;
		Schema = dbTreeNode.Schema;
		ObjectType = dbTreeNode.ObjectType;
		Subtype = dbTreeNode.Subtype;
		Source = dbTreeNode.Source;
		ImageName = dbTreeNode.imageName;
		DatabaseId = dbTreeNode.DatabaseId;
		DBMSVersion = parentNode?.DBMSVersion;
		DatabaseTitle = dbTreeNode.DatabaseTitle;
		DatabaseType = dbTreeNode.DatabaseType;
		IsMultipleSchemasDatabase = dbTreeNode.IsMultipleSchemasDatabase;
		DatabaseShowSchema = dbTreeNode.DatabaseShowSchema;
		DatabaseShowSchemaOverride = parentNode.ShowSchema || dbTreeNode.ShowSchema;
		SynchronizeState = dbTreeNode.SynchronizeState;
		Available = dbTreeNode.Available;
		Points = dbTreeNode.Points;
		TotalPoints = dbTreeNode.TotalPoints;
		PointsSum = dbTreeNode.PointsSum;
		TotalPointsSum = dbTreeNode.TotalPointsSum;
		ProgressValue = dbTreeNode.ProgressValue;
		Nodes = new DBTree();
		OldName = Name;
		OldSchema = Schema;
		OldBaseName = BaseName;
		SetName();
	}

	public DBTreeNode(DBTreeNode parentNode, IBasicData basicDataObject, SharedObjectTypeEnum.ObjectType objectType)
		: this(parentNode, basicDataObject.Id.Value, basicDataObject.Title, objectType, "term", parentNode.DatabaseId)
	{
	}

	public DBTreeNode(DBTreeNode parentNode, int id, string name, SharedObjectTypeEnum.ObjectType objectType, string imageName, int databaseId, string basename = null, string schema = null, SynchronizeStateEnum.SynchronizeState synchronizeState = SynchronizeStateEnum.SynchronizeState.Synchronized, SharedObjectSubtypeEnum.ObjectSubtype subtype = SharedObjectSubtypeEnum.ObjectSubtype.None)
		: this()
	{
		ParentNode = parentNode;
		Id = id;
		Name = name;
		BaseName = ((basename == null) ? Name : basename);
		ObjectType = objectType;
		Subtype = subtype;
		ImageName = imageName;
		DatabaseId = databaseId;
		DatabaseTitle = parentNode?.DatabaseTitle;
		DatabaseType = parentNode?.DatabaseType;
		DatabaseClass = parentNode?.DatabaseClass;
		DatabaseShowSchema = parentNode?.DatabaseShowSchema;
		DatabaseShowSchemaOverride = parentNode?.DatabaseShowSchemaOverride;
		DBMSVersion = parentNode?.DBMSVersion;
		SynchronizeState = synchronizeState;
		Schema = schema;
		Available = true;
		Nodes = new DBTree();
		OldName = Name;
		OldSchema = Schema;
		OldBaseName = BaseName;
	}

	public void SetName()
	{
		if (ObjectType != SharedObjectTypeEnum.ObjectType.Database && ObjectType != SharedObjectTypeEnum.ObjectType.BusinessGlossary)
		{
			Name = DBTreeMenu.SetNameOnlySchema(Schema, BaseName, ShowSchema);
		}
		else
		{
			Name = Title;
		}
		if (ObjectType != SharedObjectTypeEnum.ObjectType.Database && ObjectType != SharedObjectTypeEnum.ObjectType.BusinessGlossary && DatabaseId != GetParentNode(this, SharedObjectTypeEnum.ObjectType.Database)?.Id)
		{
			Name = DatabaseTitle + "." + Name;
		}
		if (ObjectType != SharedObjectTypeEnum.ObjectType.Database && ObjectType != SharedObjectTypeEnum.ObjectType.BusinessGlossary && ObjectType != SharedObjectTypeEnum.ObjectType.Module && !string.IsNullOrEmpty(Title))
		{
			Name = Name + " (" + Title + ")";
		}
	}

	public void SetEmptyNodeProgress()
	{
		Points = 0;
		TotalPoints = 0;
		ProgressValue = null;
		TotalColumnsPoints = 0;
		ColumnsPoints = 0;
		TotalFKRelationsPoints = 0;
		FKRelationsPoints = 0;
		TotalKeysPoints = 0;
		KeysPoints = 0;
		TotalObjectPoints = 0;
		ObjectPoints = 0;
		TotalParametersPoints = 0;
		ParametersPoints = 0;
		TotalPKRelationsPoints = 0;
		PKRelationsPoints = 0;
		TotalTriggersPoints = 0;
		TriggersPoints = 0;
		PointsSum = 0;
		TotalPointsSum = 0;
	}

	public void AdjustName(ObjectWithModulesObject rowDB)
	{
		Name = DBTreeMenu.SetName(Schema, BaseName, PrepareValue.ToString(rowDB.Title), ObjectType, DatabaseTypeEnum.StringToType(DB.Database.GetDatabaseTypeById(DatabaseId)), ShowSchema);
	}

	public DBTreeNode GetDirectoryNode(SharedObjectTypeEnum.ObjectType objectType)
	{
		foreach (DBTreeNode node in Nodes)
		{
			if (SharedObjectTypeEnum.StringToTypeForMenu(node.Name) == objectType)
			{
				return node;
			}
		}
		return null;
	}

	public DBTreeNode GetRootNode()
	{
		DBTreeNode dBTreeNode = this;
		while (dBTreeNode.ParentNode != null)
		{
			dBTreeNode = dBTreeNode.ParentNode;
		}
		return dBTreeNode;
	}

	public void UpdateIdForDirectories()
	{
		foreach (DBTreeNode node in Nodes)
		{
			node.Id = Id;
		}
	}

	public void SetAsUnavailable()
	{
		Available = false;
		Name += " [unavailable]";
		ImageName = "documentation_disabled";
	}

	public void ChangeManualTableSchemaAndName(bool useSchema)
	{
		string text = "(?(.+(?=\\s\\(.*\\)))(?<name>.+)\\s\\((?<title>.*)\\)|(?<name>.*))";
		string text2 = "(?(.*?\\s\\(.*?\\)(?=.*?)$)" + text + "|(?<name>.*))";
		string text3 = "(?(.*?(?=\\..+?))(?<schema>.*?)\\.";
		string pattern = text3 + text2 + "|" + text2 + ")";
		Match match = Regex.Match(Name, pattern);
		Schema = (useSchema ? match.Groups["schema"].Value : null);
		BaseName = ((useSchema || string.IsNullOrEmpty(match.Groups["schema"].Value)) ? match.Groups["name"].Value : (match.Groups["schema"].Value + "." + match.Groups["name"].Value));
		Title = match.Groups["title"].Value;
	}

	public static int? CalculateProgress(int points, int totalPoints)
	{
		if (points == 0 && totalPoints == 0)
		{
			return null;
		}
		if (points == totalPoints)
		{
			return 100;
		}
		double num = (double)points * 100.0 / (double)totalPoints;
		if (num > 0.0 && num < 1.0)
		{
			return 1;
		}
		if (num >= 99.0 && num < 100.0)
		{
			return 99;
		}
		return (int)num;
	}

	public static ProgressClass AggregateProgressDown(DBTreeNode mainNode, ProgressTypeModel progressType)
	{
		if (mainNode == null)
		{
			return new ProgressClass();
		}
		return DescriptionProgressDown(mainNode, progressType);
	}

	private static ProgressClass ProgressDownForFoldersAndModules(DBTreeNode mainNode, ProgressTypeModel progressType)
	{
		int num = 0;
		int num2 = 0;
		foreach (DBTreeNode node in mainNode.Nodes)
		{
			ProgressClass progressClass = AggregateProgressDown(node, progressType);
			if (node.SynchronizeState != SynchronizeStateEnum.SynchronizeState.Deleted)
			{
				num += progressClass.Points;
				num2 += progressClass.TotalPoints;
			}
		}
		if (mainNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database)
		{
			num += mainNode.ObjectPoints;
			num2 += mainNode.TotalObjectPoints;
			mainNode.ProgressValue = CalculateProgress(num, num2);
			return new ProgressClass(mainNode.ObjectPoints, mainNode.TotalObjectPoints);
		}
		if (mainNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			num += mainNode.ObjectPoints;
			num2 += mainNode.TotalObjectPoints;
			mainNode.Points = num;
			mainNode.TotalPoints = num2;
			mainNode.PointsSum = num;
			mainNode.TotalPointsSum = num2;
			mainNode.ProgressValue = CalculateProgress(num, num2);
		}
		else if (mainNode.Nodes.Count == 0 || num2 == 0 || mainNode.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted)
		{
			mainNode.SetEmptyNodeProgress();
		}
		else
		{
			mainNode.Points = num;
			mainNode.TotalPoints = num2;
			mainNode.PointsSum = num;
			mainNode.TotalPointsSum = num2;
			mainNode.ProgressValue = CalculateProgress(num, num2);
		}
		return new ProgressClass(num, num2);
	}

	private static ProgressClass ProgressDownForTerms(DBTreeNode mainNode, ProgressTypeModel progressType)
	{
		int num = 0;
		int num2 = 0;
		foreach (DBTreeNode node in mainNode.Nodes)
		{
			ProgressClass progressClass = AggregateProgressDown(node, progressType);
			num += progressClass.Points;
			num2 += progressClass.TotalPoints;
		}
		mainNode.PointsSum = mainNode.Points + num;
		mainNode.TotalPointsSum = mainNode.TotalPoints + num2;
		mainNode.ProgressValue = CalculateProgress(mainNode.PointsSum, mainNode.TotalPointsSum);
		return new ProgressClass(mainNode.PointsSum, mainNode.TotalPointsSum);
	}

	public static ProgressClass AggregateProgressUp(DBTreeNode mainNode, ProgressTypeModel progressType)
	{
		if (mainNode == null)
		{
			return new ProgressClass();
		}
		return progressType.Type switch
		{
			ProgressTypeEnum.AllDocumentations => DescriptionAndCustomFieldProgressUp(mainNode, progressType), 
			ProgressTypeEnum.TablesAndColumns => TablesViewsAndColumnProgressUp(mainNode, progressType), 
			ProgressTypeEnum.SelectedCustomField => DescriptionAndCustomFieldProgressUp(mainNode, progressType), 
			_ => new ProgressClass(), 
		};
	}

	private static ProgressClass TablesViewsAndColumnProgressUp(DBTreeNode mainNode, ProgressTypeModel progressType)
	{
		switch (mainNode.ObjectType)
		{
		case SharedObjectTypeEnum.ObjectType.Database:
		case SharedObjectTypeEnum.ObjectType.BusinessGlossary:
			return ProgressUpForDatabase(mainNode);
		case SharedObjectTypeEnum.ObjectType.Module:
		case SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database:
		case SharedObjectTypeEnum.ObjectType.Folder_Database:
		case SharedObjectTypeEnum.ObjectType.Folder_Module:
			return ProgressUpForFoldersAndModules(mainNode, progressType);
		case SharedObjectTypeEnum.ObjectType.Table:
		case SharedObjectTypeEnum.ObjectType.View:
		case SharedObjectTypeEnum.ObjectType.Structure:
			return ProgressUp(mainNode, progressType);
		default:
			return new ProgressClass();
		}
	}

	private static ProgressClass DescriptionAndCustomFieldProgressUp(DBTreeNode mainNode, ProgressTypeModel progressType)
	{
		switch (mainNode.ObjectType)
		{
		case SharedObjectTypeEnum.ObjectType.Database:
		case SharedObjectTypeEnum.ObjectType.BusinessGlossary:
			return ProgressUpForDatabase(mainNode);
		case SharedObjectTypeEnum.ObjectType.Module:
		case SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database:
		case SharedObjectTypeEnum.ObjectType.Folder_Database:
		case SharedObjectTypeEnum.ObjectType.Folder_Module:
			return ProgressUpForFoldersAndModules(mainNode, progressType);
		case SharedObjectTypeEnum.ObjectType.Function:
		case SharedObjectTypeEnum.ObjectType.Procedure:
		case SharedObjectTypeEnum.ObjectType.Table:
		case SharedObjectTypeEnum.ObjectType.View:
		case SharedObjectTypeEnum.ObjectType.Structure:
			return ProgressUp(mainNode, progressType);
		case SharedObjectTypeEnum.ObjectType.Term:
			if (mainNode.Nodes.Count == 0)
			{
				return ProgressUp(mainNode, progressType);
			}
			return ProgressUpForTerms(mainNode, progressType);
		default:
			return new ProgressClass();
		}
	}

	private static ProgressClass DescriptionProgressDown(DBTreeNode mainNode, ProgressTypeModel progressType)
	{
		switch (mainNode.ObjectType)
		{
		case SharedObjectTypeEnum.ObjectType.Module:
		{
			ProgressClass progressClass = ProgressDownForFoldersAndModules(mainNode, progressType);
			mainNode.ProgressValue = CalculateProgress(progressClass.Points, progressClass.TotalPoints);
			return progressClass;
		}
		case SharedObjectTypeEnum.ObjectType.Database:
		case SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database:
		case SharedObjectTypeEnum.ObjectType.Folder_Database:
		case SharedObjectTypeEnum.ObjectType.Folder_Module:
		case SharedObjectTypeEnum.ObjectType.BusinessGlossary:
			return ProgressDownForFoldersAndModules(mainNode, progressType);
		case SharedObjectTypeEnum.ObjectType.Function:
		case SharedObjectTypeEnum.ObjectType.Procedure:
		case SharedObjectTypeEnum.ObjectType.Table:
		case SharedObjectTypeEnum.ObjectType.View:
		case SharedObjectTypeEnum.ObjectType.Structure:
			mainNode.ProgressValue = CalculateProgress(mainNode.Points, mainNode.TotalPoints);
			return new ProgressClass(mainNode.Points, mainNode.TotalPoints);
		case SharedObjectTypeEnum.ObjectType.Term:
			if (mainNode.Nodes.Count == 0)
			{
				mainNode.PointsSum = mainNode.Points;
				mainNode.TotalPointsSum = mainNode.TotalPoints;
				mainNode.ProgressValue = CalculateProgress(mainNode.Points, mainNode.TotalPoints);
				return new ProgressClass(mainNode.Points, mainNode.TotalPoints);
			}
			return ProgressDownForTerms(mainNode, progressType);
		default:
			return new ProgressClass();
		}
	}

	internal static ProgressClass ProgressUp(DBTreeNode mainNode, ProgressTypeModel progressType)
	{
		ProgressObject progressObject = ((progressType.Type == ProgressTypeEnum.AllDocumentations) ? (from x in DB.DocumentationProgress.GetObjectProgress(mainNode.Id, mainNode.ObjectType)
			select (x)).SingleOrDefault() : ((progressType.Type != ProgressTypeEnum.TablesAndColumns) ? (from x in DB.DocumentationProgress.GetCustomFieldProgress(mainNode.Id, progressType.FieldName, mainNode.ObjectType)
			select (x)).SingleOrDefault() : (from x in DB.DocumentationProgress.GetSingleTableViewAndColumnsProgress(mainNode.Id, mainNode.ObjectType)
			select (x)).SingleOrDefault()));
		if (progressObject == null || mainNode.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted)
		{
			mainNode.SetEmptyNodeProgress();
		}
		else
		{
			mainNode.Points = progressObject.Points.GetValueOrDefault();
			mainNode.TotalPoints = progressObject.Total.GetValueOrDefault();
			mainNode.PointsSum = mainNode.Points;
			mainNode.TotalPointsSum = mainNode.TotalPoints;
			mainNode.ProgressValue = CalculateProgress(mainNode.Points, mainNode.TotalPoints);
		}
		AggregateProgressUp(mainNode.ParentNode, progressType);
		if (mainNode.IsInModule)
		{
			DBTreeNode dBTreeNode = DBTreeMenu.FindDBNodeById(mainNode.ObjectType, mainNode.Id);
			dBTreeNode.Points = mainNode.Points;
			dBTreeNode.TotalPoints = mainNode.TotalPoints;
			dBTreeNode.PointsSum = mainNode.PointsSum;
			dBTreeNode.TotalPointsSum = mainNode.TotalPointsSum;
			dBTreeNode.ProgressValue = mainNode.progressValue;
			AggregateProgressUp(dBTreeNode.ParentNode, progressType);
		}
		else
		{
			foreach (DBTreeNode item in DBTreeMenu.FindNodesInModules(mainNode))
			{
				item.Points = mainNode.Points;
				item.TotalPoints = mainNode.TotalPoints;
				item.PointsSum = mainNode.PointsSum;
				item.TotalPointsSum = mainNode.TotalPointsSum;
				item.ProgressValue = mainNode.progressValue;
				AggregateProgressUp(item.ParentNode, progressType);
			}
		}
		return new ProgressClass(mainNode.Points, mainNode.TotalPoints);
	}

	private static ProgressClass ProgressUpForFoldersAndModules(DBTreeNode mainNode, ProgressTypeModel progressType)
	{
		int num = 0;
		int num2 = 0;
		foreach (DBTreeNode node in mainNode.Nodes)
		{
			if (node.SynchronizeState != SynchronizeStateEnum.SynchronizeState.Deleted)
			{
				if (mainNode != null && mainNode.ParentNode?.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
				{
					num += node.PointsSum;
					num2 += node.TotalPointsSum;
				}
				else
				{
					num += node.Points;
					num2 += node.TotalPoints;
				}
			}
		}
		if (mainNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database)
		{
			num += mainNode.ObjectPoints;
			num2 += mainNode.TotalObjectPoints;
			mainNode.ProgressValue = CalculateProgress(num, num2);
		}
		else if (mainNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			num += mainNode.ObjectPoints;
			num2 += mainNode.TotalObjectPoints;
			mainNode.Points = num;
			mainNode.TotalPoints = num2;
			mainNode.PointsSum = num;
			mainNode.TotalPointsSum = num2;
			mainNode.ProgressValue = CalculateProgress(num, num2);
		}
		else if (mainNode.Nodes.Count == 0 || num2 == 0 || mainNode.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted)
		{
			mainNode.SetEmptyNodeProgress();
		}
		else
		{
			mainNode.Points = num;
			mainNode.TotalPoints = num2;
			mainNode.PointsSum = num;
			mainNode.TotalPointsSum = num2;
			mainNode.ProgressValue = CalculateProgress(num, num2);
		}
		AggregateProgressUp(mainNode.ParentNode, progressType);
		return new ProgressClass(num, num2);
	}

	private static ProgressClass ProgressUpForTerms(DBTreeNode mainNode, ProgressTypeModel progressType)
	{
		int num = 0;
		int num2 = 0;
		foreach (DBTreeNode node in mainNode.Nodes)
		{
			num += node.PointsSum;
			num2 += node.TotalPointsSum;
		}
		mainNode.PointsSum = mainNode.Points + num;
		mainNode.TotalPointsSum = mainNode.TotalPoints + num2;
		mainNode.ProgressValue = CalculateProgress(mainNode.PointsSum, mainNode.TotalPointsSum);
		AggregateProgressUp(mainNode.ParentNode, progressType);
		return new ProgressClass(mainNode.PointsSum, mainNode.TotalPointsSum);
	}

	private static ProgressClass ProgressUpForDatabase(DBTreeNode mainNode)
	{
		int num = 0;
		int num2 = 0;
		foreach (DBTreeNode node in mainNode.Nodes)
		{
			if (node.SynchronizeState != SynchronizeStateEnum.SynchronizeState.Deleted)
			{
				if (node.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database)
				{
					num += node.ObjectPoints;
					num2 += node.TotalObjectPoints;
				}
				else if (mainNode != null && mainNode.ParentNode?.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
				{
					num += node.PointsSum;
					num2 += node.TotalPointsSum;
				}
				else
				{
					num += node.Points;
					num2 += node.TotalPoints;
				}
			}
		}
		mainNode.Points = num;
		mainNode.TotalPoints = num2;
		mainNode.PointsSum = num;
		mainNode.TotalPointsSum = num2;
		mainNode.ProgressValue = CalculateProgress(num, num2);
		return new ProgressClass();
	}

	public bool IsRepresentingSameObject(DBTreeNode other)
	{
		if (DatabaseId == other.DatabaseId && ObjectType == other.ObjectType)
		{
			return Id == other.Id;
		}
		return false;
	}

	public bool ContainsNode(DBTreeNode node, bool checkSubnodes = true)
	{
		return ContainsNode(this, node, checkSubnodes);
	}

	public bool ContainsNode(int objectId, SharedObjectTypeEnum.ObjectType? objectType, bool checkSubnodes = true)
	{
		return ContainsNode(this, objectId, objectType, checkSubnodes);
	}

	private static bool ContainsNode(DBTreeNode rootNode, DBTreeNode node, bool checkSubnodes = true)
	{
		if (checkSubnodes)
		{
			foreach (DBTreeNode node2 in rootNode.Nodes)
			{
				if (node2 == node)
				{
					return true;
				}
				if (ContainsNode(node2, node, checkSubnodes))
				{
					return true;
				}
			}
			return false;
		}
		return rootNode.Nodes.Contains(node);
	}

	private static bool ContainsNode(DBTreeNode rootNode, int objectId, SharedObjectTypeEnum.ObjectType? objectType, bool checkSubnodes = true)
	{
		if (checkSubnodes)
		{
			foreach (DBTreeNode node in rootNode.Nodes)
			{
				if (node.Id == objectId && node.ObjectType == objectType)
				{
					return true;
				}
				if (ContainsNode(node, objectId, objectType, checkSubnodes))
				{
					return true;
				}
			}
			return false;
		}
		return rootNode.Nodes.Any((DBTreeNode x) => x.Id == objectId && x.ObjectType == objectType);
	}
}
