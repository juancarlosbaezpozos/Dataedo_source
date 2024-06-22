using System;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Common.Interfaces;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.Object;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class ObjectRow : ISupportsCustomFields
{
	private SynchronizeStateEnum.SynchronizeState stateBeforeSynchronization;

	public DateTime? DbmsLastModificationDate;

	public DateTime? DbmsCreationDate;

	public ObjectStatusEnum.ObjectStatus Status { get; set; }

	public bool FullImport { get; set; }

	public string Name { get; set; }

	public string DisplayedName { get; set; }

	public virtual string ObjectName
	{
		get
		{
			if (string.IsNullOrEmpty(Schema))
			{
				return Name;
			}
			return Schema + "." + Name;
		}
	}

	public virtual string ObjectIdString => Paths.EncodeInvalidPathCharacters($"{Schema}_{Name}_{ObjectId}");

	public string Title { get; set; }

	public UserTypeEnum.UserType Source { get; set; }

	public bool? IsFromAnotherDatabase { get; set; }

	public bool? IsInMultipleDatabaseModule { get; set; }

	public string DisplayName => GetDisplayName(IsFromAnotherDatabase, false);

	public string Description { get; set; }

	public string DescriptionSearch { get; set; }

	public string Schema { get; set; }

	public string Definition { get; set; }

	public string Language { get; set; }

	public string DatabaseName { get; set; }

	public SharedDatabaseTypeEnum.DatabaseType? DocumentationType { get; set; }

	public string DocumentationHost { get; set; }

	public string DocumentationName { get; set; }

	public string DocumentationTitle { get; set; }

	public bool? MultipleSchemasDatabase { get; set; }

	public int ObjectId { get; set; }

	public int? DatabaseId { get; set; }

	public SharedObjectTypeEnum.ObjectType? Type { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype? Subtype { get; set; }

	public string FunctionType { get; set; }

	public string TypeAsString => SharedObjectTypeEnum.TypeToString(Type);

	public string SubtypeAsString => SharedObjectSubtypeEnum.TypeToString(Type, Subtype);

	public string TypeDisplayString => SharedObjectTypeEnum.TypeToStringForSingle(Type);

	public string SubtypeDisplayString => SharedObjectSubtypeEnum.TypeToStringForSingle(Type, Subtype);

	public bool ToSynchronize { get; set; }

	public bool IsNew => SynchronizeState == SynchronizeStateEnum.SynchronizeState.New;

	public SynchronizeStateEnum.SynchronizeState SynchronizeState { get; set; }

	public bool IsNotSynchronized => SynchronizeState != SynchronizeStateEnum.SynchronizeState.Synchronized;

	public string StateToStringForDescription
	{
		get
		{
			if (!FullImport)
			{
				return SynchronizeStateEnum.StateToString(SynchronizeState);
			}
			return SynchronizeStateEnum.StateToStringForFullImport(SynchronizeState);
		}
	}

	public SynchronizeStateEnum.SynchronizeState StateBeforeSynchronization
	{
		get
		{
			return stateBeforeSynchronization;
		}
		set
		{
			if (StateBeforeSynchronization != value)
			{
				stateBeforeSynchronization = value;
				if (DatabaseId.HasValue && Type.HasValue)
				{
					DBTreeMenu.RefreshSynchState(Type.Value, Subtype.Value, Name, Schema, DatabaseId.Value, StateBeforeSynchronization);
				}
			}
		}
	}

	public DateTime? SynchronizationDate { get; set; }

	public string NameWithType => SharedObjectSubtypeEnum.TypeToStringForSingle(Type, Subtype) + " " + Name;

	public CustomFieldContainer CustomFields { get; set; }

	public SharedObjectTypeEnum.ObjectType? ObjectTypeValue => Type;

	public SharedObjectTypeEnum.ObjectType? ObjectTypeValueOrUnresolved => Type ?? SharedObjectTypeEnum.ObjectType.UnresolvedEntity;

	public ObjectRow()
	{
	}

	public ObjectRow(IExtendedBaseDataObjectWithCustomFields row, CustomFieldsSupport customFieldsSupport, bool readDisplayedName = true, bool readDocumentationData = false)
	{
		ReadDefaultData(row, customFieldsSupport, readDisplayedName, readDocumentationData);
	}

	public ObjectRow(ObjectDeletedFromDatabaseObject row, string databaseName)
	{
		ReadDefaultData(row);
		DatabaseName = databaseName;
		SynchronizeState = SynchronizeStateEnum.SynchronizeState.Deleted;
	}

	public ObjectRow(object name, object schema, object databaseName, object type, object dbmsCreationDate, object dbmsLastModificationDate, object description, object definition, object function_type, object language, int? databaseId, bool? multipleSchemasDatabase)
	{
		Name = PrepareValue.ToString(name);
		DatabaseName = PrepareValue.ToString(databaseName);
		Type = SharedObjectTypeEnum.StringToType(PrepareValue.ToString(type));
		Subtype = SharedObjectSubtypeEnum.StringToType(Type, PrepareValue.ToString(type));
		string text = PrepareValue.ToString(description);
		if (Type.HasValue && SharedObjectTypeEnum.IsFromDatabase(Type.Value))
		{
			Description = PrepareValue.FixDescription(text);
		}
		else
		{
			Description = text;
		}
		DescriptionSearch = text;
		Schema = PrepareValue.ToString(schema);
		ToSynchronize = false;
		DatabaseId = databaseId;
		MultipleSchemasDatabase = multipleSchemasDatabase;
		DbmsCreationDate = PrepareValue.ToDateTime(dbmsCreationDate);
		DbmsLastModificationDate = PrepareValue.ToDateTime(dbmsLastModificationDate);
		Definition = PrepareValue.ToString(definition);
		FunctionType = PrepareValue.ToString(function_type);
		Language = "SQL";
	}

	public ObjectRow(ObjectSynchronizationObject dataReader, int? databaseId, bool? multipleSchemasDatabase, SharedDatabaseTypeEnum.DatabaseType? databaseType, CustomFieldsSupport customFieldsSupport = null)
		: this(dataReader.Name, dataReader.Schema, dataReader.DatabaseName, dataReader.Type, dataReader.CreateDate, dataReader.ModifyDate, dataReader.Description, dataReader.Definition, dataReader.FunctionType, dataReader.Language, databaseId, multipleSchemasDatabase)
	{
		Language = ((databaseType == SharedDatabaseTypeEnum.DatabaseType.MongoDB || databaseType == SharedDatabaseTypeEnum.DatabaseType.CosmosDbSQL || databaseType == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) ? "JSON" : ((databaseType == SharedDatabaseTypeEnum.DatabaseType.Neo4j) ? "CYPHER" : "SQL"));
		if (databaseType == SharedDatabaseTypeEnum.DatabaseType.InterfaceTables)
		{
			ReadAdditionalDataForInterfaceTableObject(dataReader);
		}
		if (customFieldsSupport != null)
		{
			CustomFields = new CustomFieldContainer(customFieldsSupport);
			CustomFields.RetrieveCustomFields(dataReader);
		}
	}

	private void ReadAdditionalDataForInterfaceTableObject(ObjectSynchronizationObject dataReader)
	{
		if (dataReader is ObjectSynchronizationForInterfaceTableObject objectSynchronizationForInterfaceTableObject)
		{
			Subtype = SharedObjectSubtypeEnum.StringToType(Type, objectSynchronizationForInterfaceTableObject.Subtype);
		}
	}

	private void ReadDefaultData(IExtendedBaseDataObjectWithCustomFields row, CustomFieldsSupport customFieldsSupport, bool readDisplayedName = true, bool readDocumentationData = false)
	{
		ReadDefaultData(row, readDisplayedName, readDocumentationData);
		CustomFields = new CustomFieldContainer(customFieldsSupport);
		CustomFields.RetrieveCustomFields(row);
	}

	private void ReadDefaultData(IExtendedBaseDataObject row, bool readDisplayedName = true, bool readDocumentationData = false)
	{
		Name = row.Name;
		Title = row.Title;
		Schema = row.Schema;
		Description = row.Description;
		Type = SharedObjectTypeEnum.StringToType(row.ObjectType);
		Subtype = SharedObjectSubtypeEnum.StringToType(Type, row.Subtype);
		ObjectId = row.Id;
		Source = UserTypeEnum.ObjectToType(row.Source) ?? UserTypeEnum.UserType.DBMS;
		ToSynchronize = false;
		Status = ObjectStatusEnum.GetStatusFromString(row.Status);
		Definition = row.Definition;
		DatabaseId = row.DatabaseId;
		if (readDisplayedName)
		{
			DisplayedName = DBTreeMenu.SetNameOnlySchema(this, MultipleSchemasDatabase == true);
		}
		if (readDocumentationData)
		{
			IDocumentationData documentationData = row as IDocumentationData;
			DocumentationType = DatabaseTypeEnum.StringToType(documentationData.DatabaseType);
			DocumentationHost = documentationData.DatabaseHost;
			DocumentationName = documentationData.DatabaseName;
			DocumentationTitle = documentationData.DatabaseTitle;
		}
	}

	public void SetId()
	{
		if (Type == SharedObjectTypeEnum.ObjectType.Table || Type == SharedObjectTypeEnum.ObjectType.View)
		{
			ObjectId = DB.Table.GetIdByName(DatabaseId.Value, Name, Schema, SubtypeAsString);
		}
		else if (Type == SharedObjectTypeEnum.ObjectType.Procedure || Type == SharedObjectTypeEnum.ObjectType.Function)
		{
			ObjectId = DB.Procedure.GetIdByName(DatabaseId.Value, Name, Schema, SubtypeAsString);
		}
	}

	public void SetSynchStateBeforeSynchronization()
	{
		StateBeforeSynchronization = SynchronizeState;
	}

	public void AddNewObjectToTree()
	{
		DBTreeMenu.AddNewObjectToTree(this, MultipleSchemasDatabase == true);
	}

	private string GetDisplayName(bool? isFromAnotherDatabase, bool? isInMultipleDatabaseContext, bool withTitle = true)
	{
		return ((isFromAnotherDatabase != true && isInMultipleDatabaseContext != true) ? string.Empty : (DocumentationTitle + ".")) + ((string.IsNullOrEmpty(Title) || !withTitle) ? ObjectName : (ObjectName + " (" + Title + ")"));
	}
}
