using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Classes.Synchronize.Tools;
using Dataedo.App.Enums;
using Dataedo.App.Tools;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Model.Data.Dependencies;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer.Model;

public class DependencyRow
{
	public enum DependencyNodeType
	{
		Normal = 0,
		Relation = 1,
		UserRelation = 2,
		PKRelation = 3,
		PKUserRelation = 4,
		Trigger = 5
	}

	public enum DependencyNodeCommonType
	{
		Normal = 0,
		Relation = 1,
		Trigger = 2
	}

	public bool IsRoot
	{
		get
		{
			if (Parent == null)
			{
				return TypeEnum != SharedObjectTypeEnum.ObjectType.Trigger;
			}
			return false;
		}
	}

	public bool Changed { get; set; }

	public static int UniqueId { get; set; }

	public bool IsUsesObject { get; set; }

	public bool ContextShowSchema { get; private set; }

	public DependencyRow Parent { get; set; }

	public BindingList<DependencyRow> Children { get; set; }

	public int? DependencyId { get; set; }

	public DependencyDescriptions DependencyDescriptionsData { get; set; }

	public int Id { get; set; }

	public int ParentId
	{
		get
		{
			if (Parent == null || DependencyCommonType == DependencyNodeCommonType.Trigger)
			{
				return 0;
			}
			return Parent.Id;
		}
	}

	public string Type { get; set; }

	public string Subtype { get; set; }

	public DependencyNodeCommonType TypeToGoTo
	{
		get
		{
			if (SharedObjectTypeEnum.StringToType(Type) == SharedObjectTypeEnum.ObjectType.Trigger)
			{
				return DependencyNodeCommonType.Trigger;
			}
			return DependencyNodeCommonType.Normal;
		}
	}

	public SharedObjectTypeEnum.ObjectType TypeEnum => SharedObjectTypeEnum.StringToType(Type) ?? SharedObjectTypeEnum.ObjectType.UnresolvedEntity;

	public SharedObjectSubtypeEnum.ObjectSubtype SubtypeEnum => SharedObjectSubtypeEnum.StringToType(TypeEnum, Subtype);

	public string Name { get; set; }

	public string Title { get; set; }

	public string RelationTitle { get; set; }

	public Cardinality PKCardinality { get; set; } = new Cardinality();


	public Cardinality FKCardinality { get; set; } = new Cardinality();


	public string Source { get; set; }

	public string SourceString
	{
		get
		{
			if (!IsUserDefined)
			{
				return string.Empty;
			}
			return "user";
		}
	}

	public bool IsUserDefined => this?.Source == "USER";

	public string ObjectSource { get; set; }

	public string ObjectSourceString
	{
		get
		{
			if (!IsUserDefinedObject)
			{
				return string.Empty;
			}
			return "user";
		}
	}

	public bool IsUserDefinedObject => this?.ObjectSource == "USER";

	public string Status { get; set; }

	public bool IsActive => Status?.ToUpper() != "D";

	public bool CanDelete => IsUserDefined;

	public DatabaseInfo RowDatabase { get; set; }

	public DatabaseInfo CurrentDatabase { get; set; }

	public bool IsInCurrentDatabase
	{
		get
		{
			if (RowDatabase.ServerLower == CurrentDatabase.ServerLower)
			{
				return RowDatabase.DatabaseLower == CurrentDatabase.DatabaseLower;
			}
			return false;
		}
	}

	public DependencyNodeType DependencyType { get; set; }

	public DependencyNodeCommonType DependencyCommonType
	{
		get
		{
			switch (DependencyType)
			{
			case DependencyNodeType.Relation:
			case DependencyNodeType.UserRelation:
			case DependencyNodeType.PKRelation:
			case DependencyNodeType.PKUserRelation:
				return DependencyNodeCommonType.Relation;
			case DependencyNodeType.Normal:
				return DependencyNodeCommonType.Normal;
			case DependencyNodeType.Trigger:
				return DependencyNodeCommonType.Trigger;
			default:
				return DependencyNodeCommonType.Normal;
			}
		}
	}

	public string DependencyCommonTypeAsString => DependencyCommonType switch
	{
		DependencyNodeCommonType.Normal => "NORMAL", 
		DependencyNodeCommonType.Relation => "RELATION", 
		DependencyNodeCommonType.Trigger => "TRIGGER", 
		_ => "NORMAL", 
	};

	public int? ObjectId { get; set; }

	public int? DestinationDatabaseId { get; set; }

	public string DestinationObjectType { get; set; }

	public int? DestinationObjectId { get; set; }

	public string TableName { get; set; }

	public string TableTitle { get; set; }

	public bool IsDisabled { get; set; }

	public string DirectionString
	{
		get
		{
			if (!IsUsesObject)
			{
				return "uses_";
			}
			return "used_by_";
		}
	}

	public string UserDefinedString
	{
		get
		{
			if (!IsUserDefined)
			{
				return string.Empty;
			}
			return "user_";
		}
	}

	public string UserDefinedObjectString
	{
		get
		{
			if (!IsUserDefinedObject)
			{
				return string.Empty;
			}
			return "_user";
		}
	}

	public string IsDisabledTypeString
	{
		get
		{
			if (IsDisabled)
			{
				return "_disabled";
			}
			if (!(Type != "TRIGGER"))
			{
				return "_active";
			}
			return string.Empty;
		}
	}

	public string IsActiveTypeString
	{
		get
		{
			if (!IsActive)
			{
				return "_deleted";
			}
			return string.Empty;
		}
	}

	public string FullTypeString => GetFullTypeString(TypeEnum, SubtypeEnum);

	public string FullTypeStringForMainType => GetFullTypeString(TypeEnum, null);

	public Bitmap Icon => Icons.SetIconForDependency(this);

	public bool ShowSchema
	{
		get
		{
			if (ContextShowSchema || DatabaseRow.GetShowSchema(RowDatabase.ShowSchema, RowDatabase.ShowSchemaOverride))
			{
				return !string.IsNullOrEmpty(RowDatabase.Schema);
			}
			return false;
		}
	}

	public string DisplayName => GetBaseDisplayName(ShowSchema, Name, (!string.IsNullOrEmpty(Title)) ? (" (" + Title + ")") : null, (!string.IsNullOrEmpty(RelationTitle)) ? (" (" + RelationTitle + ")") : null);

	public string DisplayNameWithoutTitle => GetBaseDisplayName(ShowSchema, Name, null, null);

	public string DisplayNameShowSchema => GetBaseDisplayName(true, Name, (!string.IsNullOrEmpty(Title)) ? (" (" + Title + ")") : null, (!string.IsNullOrEmpty(RelationTitle)) ? (" (" + RelationTitle + ")") : null);

	public string DisplayNameShowSchemaWithoutTitle => GetBaseDisplayName(true, Name, null, null);

	public string DisplayNameWithoutRelation => GetBaseDisplayName(ShowSchema, Name, (!string.IsNullOrEmpty(Title)) ? (" (" + Title + ")") : null);

	public string DisplayTableName => GetBaseDisplayName(ShowSchema, TableName, (!string.IsNullOrEmpty(TableTitle)) ? (" (" + TableTitle + ")") : null);

	public string ToolTipText
	{
		get
		{
			if (!IsRoot)
			{
				if (DependencyCommonType == DependencyNodeCommonType.Normal)
				{
					return (IsUsesObject ? "Uses " : "Used by ") + (IsUserDefined ? "(user-defined)" : string.Empty);
				}
				if (DependencyCommonType == DependencyNodeCommonType.Relation)
				{
					return ToolTips.GetRelationDescription(!IsUserDefined, CanDelete, FKCardinality, PKCardinality);
				}
				return SharedObjectSubtypeEnum.TypeToStringForSingle(TypeEnum, SubtypeEnum);
			}
			if (IsUserDefinedObject)
			{
				return SharedObjectSubtypeEnum.TypeToStringForSingle(TypeEnum, SubtypeEnum) + " (user-defined)";
			}
			return SharedObjectSubtypeEnum.TypeToStringForSingle(TypeEnum, SubtypeEnum);
		}
	}

	public string Description
	{
		get
		{
			return DependencyDescriptionsData.Description;
		}
		set
		{
			Changed = true;
			DependencyDescriptionsData.Description = value;
		}
	}

	public int? OrdinalPosition
	{
		get
		{
			return DependencyDescriptionsData.OrdinalPosition;
		}
		set
		{
			Changed = true;
			DependencyDescriptionsData.OrdinalPosition = value;
		}
	}

	public string ObjectIdString => IdStrings.GetObjectIdString(RowDatabase.DatabaseType, RowDatabase.HasMultipleSchemas, RowDatabase.Schema, Name, DestinationObjectId);

	public string ParentObjectIdString => IdStrings.GetObjectIdString(RowDatabase.DatabaseType, RowDatabase.HasMultipleSchemas, RowDatabase.Schema, TableName, DestinationObjectId);

	public int DescriptionId { get; set; }

	public bool IsSaved { get; set; } = true;


	public string GetFullTypeString(SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype? objectSubtype)
	{
		string typeToGetIcon = GetTypeToGetIcon(objectType, objectSubtype);
		if (!IsRoot && DependencyCommonType == DependencyNodeCommonType.Normal && IsActive)
		{
			return DirectionString + UserDefinedString + typeToGetIcon.ToString().ToLower() + IsDisabledTypeString + IsActiveTypeString;
		}
		if (DependencyCommonType == DependencyNodeCommonType.Relation)
		{
			if (FKCardinality.Type.HasValue && PKCardinality.Type.HasValue)
			{
				if (IsUsesObject)
				{
					return "relationship_" + FKCardinality.Id + "_" + PKCardinality.Id + (string.IsNullOrEmpty(SourceString) ? string.Empty : ("_" + SourceString));
				}
				return "relationship_" + PKCardinality.Id + "_" + FKCardinality.Id + (string.IsNullOrEmpty(SourceString) ? string.Empty : ("_" + SourceString));
			}
			return string.Empty;
		}
		return typeToGetIcon.ToString().ToLower() + UserDefinedObjectString + IsDisabledTypeString + IsActiveTypeString;
	}

	public string GetTypeToGetIcon(SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype? objectSubtype)
	{
		if (DependencyType == DependencyNodeType.Relation)
		{
			return "RELATION";
		}
		if (DependencyType == DependencyNodeType.UserRelation)
		{
			return "USER_RELATION";
		}
		if (DependencyType == DependencyNodeType.PKRelation)
		{
			return "PK_RELATION";
		}
		if (DependencyType == DependencyNodeType.PKUserRelation)
		{
			return "USER_PK_RELATION";
		}
		if (Type == "UNRESOLVED_ENTITY" || Type == null)
		{
			return "UNRESOLVED";
		}
		return SharedObjectSubtypeEnum.TypeToString(objectType, objectSubtype);
	}

	private string GetBaseDisplayName(bool showSchema, params string[] additionalText)
	{
		StringBuilder stringBuilder = new StringBuilder();
		DatabaseInfo rowDatabase = RowDatabase;
		int num;
		if (rowDatabase != null)
		{
			_ = rowDatabase.DocumentationId;
			if (true && Parent == null && RowDatabase.DocumentationId != CurrentDatabase.DocumentationId)
			{
				num = 1;
				goto IL_0068;
			}
		}
		if (Parent != null)
		{
			num = ((RowDatabase.DocumentationId != CurrentDatabase.DocumentationId) ? 1 : 0);
			if (num != 0)
			{
				goto IL_0068;
			}
		}
		else
		{
			num = 0;
		}
		goto IL_0096;
		IL_0068:
		if (!string.IsNullOrEmpty(RowDatabase.Title))
		{
			stringBuilder.Append(RowDatabase.Title + ".");
		}
		goto IL_0096;
		IL_0096:
		if (num == 0 && ((Parent == null && !CurrentDatabase.IsCurrentDocumentation(RowDatabase.DatabaseOrSchemaLower)) || (Parent != null && !Parent.RowDatabase.IsCurrentDocumentation(RowDatabase.DatabaseOrSchemaLower))) && !string.IsNullOrEmpty(RowDatabase.DatabaseOrSchemaLower) && DependencyCommonType != DependencyNodeCommonType.Relation)
		{
			stringBuilder.Append(RowDatabase.DatabaseOrSchema + ".");
		}
		if (showSchema && !string.IsNullOrEmpty(RowDatabase.Schema))
		{
			stringBuilder.Append(RowDatabase.Schema + ".");
		}
		foreach (string value in additionalText)
		{
			stringBuilder.Append(value);
		}
		return stringBuilder.ToString();
	}

	public bool IsRepresentingSameObject(DependencyRow other)
	{
		if (RowDatabase.ServerLower == other.RowDatabase.ServerLower && RowDatabase.DatabaseLower == other.RowDatabase.DatabaseLower && DependencyType == other.DependencyType && Type == other.Type && RowDatabase.Schema == other.RowDatabase.Schema && Name == other.Name)
		{
			return TableName == other.TableName;
		}
		return false;
	}

	public bool IsRepresentingSameDependency(DependencyRow other)
	{
		if (IsRepresentingSameObject(other) && Parent != null && other.Parent != null)
		{
			return Parent.IsRepresentingSameObject(other.Parent);
		}
		return false;
	}

	public DependencyRow(int id, int? databaseId, SharedDatabaseTypeEnum.DatabaseType? databaseType, string server, string database, string databaseTitle, bool? hasMultipleSchemas, bool? showSchema, bool? showSchemaOverride, bool contextShowSchema, string schema, string name, string title, string objectSource, string destinationObjectType, int? destinationObjectId, string relationTitle, string fkCardinalityType, string pkCardinalityType, SharedObjectTypeEnum.ObjectType? objectType, SharedObjectSubtypeEnum.ObjectSubtype? objectSubtype, string source, DatabaseInfo currentDatabase, bool isUsesObject, DependencyRow parent, DependencyNodeType dependencyNodeType, int objectId, string tableName, string tableTitle, DependencyDescriptions dependencyDescriptions = null)
	{
		IsUsesObject = isUsesObject;
		Name = name;
		Title = title;
		ObjectSource = objectSource;
		DestinationObjectType = destinationObjectType;
		DestinationObjectId = destinationObjectId;
		DestinationDatabaseId = databaseId;
		RelationTitle = relationTitle;
		FKCardinality.Type = CardinalityTypeEnum.StringToType(fkCardinalityType);
		PKCardinality.Type = CardinalityTypeEnum.StringToType(pkCardinalityType);
		Type = SharedObjectTypeEnum.TypeToString(objectType);
		Subtype = SharedObjectSubtypeEnum.TypeToString(objectType, objectSubtype);
		RowDatabase = new DatabaseInfo(databaseId.GetValueOrDefault(), databaseType, server, database, databaseTitle, hasMultipleSchemas, showSchema, showSchemaOverride, schema);
		Source = source;
		ContextShowSchema = contextShowSchema;
		CurrentDatabase = currentDatabase;
		Children = new BindingList<DependencyRow>();
		Id = id;
		Parent = parent;
		DependencyType = dependencyNodeType;
		ObjectId = objectId;
		TableName = tableName;
		TableTitle = tableTitle;
		DependencyDescriptionsData = dependencyDescriptions;
	}

	public DependencyRow(int? databaseId, SharedDatabaseTypeEnum.DatabaseType? databaseType, string server, string database, string databaseTitle, bool? hasMultipleSchemas, bool? showSchema, bool? showSchemaOverride, bool contextShowSchema, string schema, string name, string title, string objectSource, string destinationObjectType, int? destinationObjectId, string relationTitle, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype objectSubtype, string source, DatabaseInfo currentDatabase, bool isUsesObject)
	{
		IsUsesObject = isUsesObject;
		Name = name;
		Title = title;
		ObjectSource = objectSource;
		DestinationObjectType = destinationObjectType;
		DestinationObjectId = destinationObjectId;
		RelationTitle = relationTitle;
		Type = SharedObjectTypeEnum.TypeToString(objectType);
		Subtype = SharedObjectSubtypeEnum.TypeToString(objectType, objectSubtype);
		RowDatabase = new DatabaseInfo(databaseId.GetValueOrDefault(), databaseType, server, database, databaseTitle, hasMultipleSchemas, showSchema, showSchemaOverride, schema);
		Source = source;
		ContextShowSchema = contextShowSchema;
		CurrentDatabase = currentDatabase;
		Id = ++UniqueId;
		Children = new BindingList<DependencyRow>();
		DependencyDescriptionsData = new DependencyDescriptions();
	}

	public DependencyRow(UsesDependencyObject dataRowView, int id, DependencyRow parent, bool isUsesObject)
	{
		IsUsesObject = isUsesObject;
		DependencyId = dataRowView.DependencyId;
		Name = dataRowView.Name;
		Title = dataRowView.Title;
		ObjectSource = dataRowView.ObjectSource;
		DestinationObjectType = dataRowView.DestinationObjectType;
		DestinationObjectId = dataRowView.DestinationObjectId;
		DestinationDatabaseId = dataRowView.DestinationDatabaseId;
		Type = dataRowView.Type ?? SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.UnresolvedEntity);
		Subtype = dataRowView.Subtype;
		Source = dataRowView.Source;
		Status = dataRowView.Status;
		RowDatabase = new DatabaseInfo(DestinationDatabaseId ?? parent.RowDatabase.DocumentationId, DatabaseTypeEnum.StringToType(dataRowView.DatabaseType), dataRowView.Server, dataRowView.Database, dataRowView.DatabaseTitle, dataRowView.MultipleSchemas, dataRowView.ShowSchema, dataRowView.ShowSchemaOverride, dataRowView.Schema);
		ContextShowSchema = parent.ContextShowSchema;
		CurrentDatabase = parent.CurrentDatabase;
		Id = id;
		Parent = parent;
		Children = new BindingList<DependencyRow>();
		DependencyDescriptionsData = new DependencyDescriptions();
		DependencyDescriptionsObjectInformation referencingObject = new DependencyDescriptionsObjectInformation
		{
			Server = dataRowView.Server,
			Database = dataRowView.Database,
			Schema = dataRowView.Schema,
			Name = dataRowView.Name,
			Type = (dataRowView.Type ?? SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.UnresolvedEntity))
		};
		DependencyDescriptionsObjectInformation referencedObject = new DependencyDescriptionsObjectInformation
		{
			Server = parent.RowDatabase.Server,
			Database = parent.RowDatabase.Database,
			Schema = parent.RowDatabase.Schema,
			Name = parent?.Name,
			Type = parent?.Type
		};
		DependencyDescriptionsData.IsDescriptionForUsesDependency = isUsesObject;
		if (isUsesObject)
		{
			DependencyDescriptionsData.ReferencingObject = referencingObject;
			DependencyDescriptionsData.ReferencedObject = referencedObject;
		}
		else
		{
			DependencyDescriptionsData.ReferencedObject = referencedObject;
			DependencyDescriptionsData.ReferencingObject = referencingObject;
		}
	}

	public DependencyRow(UsedByDependencyObject dataRowView, int id, DependencyRow parent, bool isUsesObject)
		: this((UsesDependencyObject)dataRowView, id, parent, isUsesObject)
	{
		TableName = dataRowView.TableName;
		TableTitle = dataRowView.TableTitle;
	}

	public bool HasSameDependencyAncestor(DependencyRow other)
	{
		if (other != null)
		{
			if (!IsSameDependency(other))
			{
				if (Parent != null)
				{
					return Parent.HasSameDependencyAncestor(other);
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public bool IsSameDependency(string serverLower, string databaseLower, string schema, string name)
	{
		if (RowDatabase.ServerLower == serverLower && RowDatabase.DatabaseLower == databaseLower && RowDatabase.Schema == schema)
		{
			return Name == name;
		}
		return false;
	}

	public bool IsSameDependency(string schema, string name)
	{
		if (RowDatabase.Schema == schema)
		{
			return Name == name;
		}
		return false;
	}

	public bool IsSameDependency(DependencyRow other)
	{
		if (other != null && other.DestinationObjectId == DestinationObjectId)
		{
			return other.Type == Type;
		}
		return false;
	}

	public List<DependencyRow> GetNodesList()
	{
		return GetNodesList(this);
	}

	public DependencyRow GetFirstSameDependency(DependencyRow dependencyRow)
	{
		DependencyRow dependencyRow2 = Children.FirstOrDefault((DependencyRow x) => x.IsRepresentingSameDependency(dependencyRow));
		if (dependencyRow2 == null)
		{
			foreach (DependencyRow child in Children)
			{
				dependencyRow2 = child.GetFirstSameDependency(dependencyRow);
				if (dependencyRow2 != null)
				{
					return dependencyRow2;
				}
			}
			return dependencyRow2;
		}
		return dependencyRow2;
	}

	public static List<DependencyRow> GetNodesList(DependencyRow dependencyRow)
	{
		List<DependencyRow> list = new List<DependencyRow>();
		list.Add(dependencyRow);
		foreach (DependencyRow child in dependencyRow.Children)
		{
			list.AddRange(GetNodesList(child));
		}
		return list;
	}

	public override string ToString()
	{
		return DisplayName;
	}
}
