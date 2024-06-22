using System;
using System.Drawing;
using System.Linq;
using Dataedo.App.Tools;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.DataLineage;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize.DataLineage;

public class DataFlowRow : BasicRow
{
	public Guid Guid { get; } = Guid.NewGuid();


	public string Direction { get; set; }

	public int ProcessId { get; set; }

	public string ProcessName { get; set; }

	public string DatabaseName { get; set; }

	public int? DatabaseId { get; set; }

	public int ObjectId { get; set; }

	public string ObjectName { get; set; }

	public string ObjectTitle { get; set; }

	public string ObjectCaption { get; private set; }

	public string ObjectCaptionWithoutDatabase { get; private set; }

	public string ObjectCaptionInline { get; private set; }

	public string Schema { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype { get; set; }

	public UserTypeEnum.UserType ObjectSource { get; set; }

	public bool ShowSchema { get; set; }

	public bool IsProcessInSameProcessor { get; set; }

	public string CreationDateString { get; set; }

	public string CreatedBy { get; set; }

	public string LastModificationDateString { get; set; }

	public string ModifiedBy { get; set; }

	public Bitmap Icon { get; private set; }

	public DataProcessRow Process { get; internal set; }

	public string ColumnProcessName
	{
		get
		{
			if (IsProcessInSameProcessor)
			{
				return ProcessName;
			}
			return ObjectName + "." + ProcessName;
		}
	}

	public DataFlowRow()
	{
	}

	public DataFlowRow(DataFlowObject dataFlowObject)
	{
		base.Id = dataFlowObject.FlowId;
		ProcessId = dataFlowObject.ProcessId;
		ProcessName = dataFlowObject.ProcessName;
		IsProcessInSameProcessor = dataFlowObject.IsProcessInSameProcessor;
		Direction = dataFlowObject.Direction;
		ObjectId = dataFlowObject.ObjectId;
		base.ObjectType = SharedObjectTypeEnum.StringToType(dataFlowObject.ObjectType) ?? SharedObjectTypeEnum.ObjectType.UnresolvedEntity;
		LastModificationDateString = PrepareValue.SetDateTimeWithFormatting(dataFlowObject.LastModificationDate);
		ModifiedBy = dataFlowObject.ModifiedBy;
		CreationDateString = PrepareValue.SetDateTimeWithFormatting(dataFlowObject.CreationDate);
		CreatedBy = dataFlowObject.CreatedBy;
		base.Source = UserTypeEnum.ObjectToType(dataFlowObject.Source) ?? UserTypeEnum.UserType.USER;
		ObjectSubtype = SharedObjectSubtypeEnum.StringToType(base.ObjectType, dataFlowObject.ObjectSubtype);
		DatabaseName = dataFlowObject.DatabaseName;
		DatabaseId = dataFlowObject.DatabaseId;
		ObjectName = dataFlowObject.ObjectName;
		Schema = dataFlowObject.Schema;
		base.Name = dataFlowObject.ObjectName;
		ObjectTitle = dataFlowObject.ObjectTitle;
		ObjectSource = UserTypeEnum.ObjectToType(dataFlowObject.ObjectSource) ?? UserTypeEnum.UserType.USER;
		ShowSchema = GetShowSchema(dataFlowObject.ShowSchema, dataFlowObject.ShowSchemaOverride);
		UpdateIcon();
		SetCaption();
	}

	public static bool GetShowSchema(bool? databaseShowSchema, bool? databaseShowSchemaOverride)
	{
		if (databaseShowSchemaOverride != true)
		{
			if (!databaseShowSchemaOverride.HasValue)
			{
				return databaseShowSchema == true;
			}
			return false;
		}
		return true;
	}

	public DataFlowRow(int processId, string processName, bool isProcessInSameProcessor, string direction, int objectId, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype objectSubtype, UserTypeEnum.UserType objectSource, UserTypeEnum.UserType source, string databaseName = null, int? databaseId = null, string objectName = null, string objectTitle = null, string objectSchema = null, bool showSchema = false)
	{
		ProcessId = processId;
		ProcessName = processName;
		IsProcessInSameProcessor = isProcessInSameProcessor;
		Direction = direction;
		ObjectId = objectId;
		base.ObjectType = objectType;
		ObjectSubtype = objectSubtype;
		base.Source = source;
		DatabaseName = databaseName;
		DatabaseId = databaseId;
		base.Name = objectName;
		ObjectName = objectName;
		ObjectTitle = objectTitle;
		ObjectSource = objectSource;
		Schema = objectSchema;
		ShowSchema = showSchema;
		UpdateIcon();
		SetCaption();
	}

	private void SetCaption()
	{
		ObjectCaptionWithoutDatabase = GetFlowCaption(null, ObjectTitle, ObjectName, Schema, ShowSchema);
		ObjectCaption = GetFlowCaption(DatabaseName, ObjectTitle, ObjectName, Schema, ShowSchema);
		ObjectCaptionInline = GetFlowCaption(DatabaseName, ObjectTitle, ObjectName, Schema, ShowSchema, separateWithNewLine: false);
	}

	public static string GetFlowCaption(IFlowDraggable dbTreeNode, int? currentObjectDatabaseId)
	{
		return GetFlowCaption((dbTreeNode.DatabaseId == currentObjectDatabaseId) ? null : dbTreeNode.DatabaseTitle, dbTreeNode.Title, dbTreeNode.BaseName, dbTreeNode.Schema, dbTreeNode.ShowSchema);
	}

	private static string GetFlowCaption(string databaseName, string objectTitle, string objectName, string objectSchema, bool showSchema, bool separateWithNewLine = true)
	{
		databaseName = (string.IsNullOrEmpty(databaseName) ? null : ("[" + databaseName + "]"));
		objectName = (string.IsNullOrEmpty(objectTitle) ? objectName : (objectName + " (" + objectTitle + ")"));
		if (showSchema)
		{
			objectName = (string.IsNullOrEmpty(objectSchema) ? objectName : (objectSchema + "." + objectName));
		}
		string[] source = new string[2] { databaseName, objectName };
		return string.Join(separateWithNewLine ? Environment.NewLine : " ", source.Where((string s) => !string.IsNullOrEmpty(s)));
	}

	public void UpdateIcon()
	{
		Icon = IconsSupport.GetObjectIcon(base.ObjectType, ObjectSubtype, ObjectSource);
	}

	internal void UpdateIcon(SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype, UserTypeEnum.UserType userType)
	{
		Icon = IconsSupport.GetObjectIcon(objectType, subtype, userType);
	}
}
