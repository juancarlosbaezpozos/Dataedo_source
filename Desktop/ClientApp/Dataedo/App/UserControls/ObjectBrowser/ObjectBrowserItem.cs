using System;
using System.Drawing;
using System.Text;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Enums;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.DataLineage;
using Dataedo.Shared.Enums;

namespace Dataedo.App.UserControls.ObjectBrowser;

public class ObjectBrowserItem : IFlowDraggable
{
	public int DatabaseId { get; set; }

	public string DatabaseType { get; set; }

	public string DatabaseClass { get; set; }

	public string DatabaseTitle { get; set; }

	public bool? DatabaseMultipleSchemas { get; set; }

	public bool? DatabaseShowSchema { get; set; }

	public bool? DatabaseShowSchemaOverride { get; set; }

	public DateTime? DbmsLastModificationDate { get; set; }

	public string Title { get; set; }

	public string BaseName { get; set; }

	public int Id { get; set; }

	public string Schema { get; set; }

	public string Status { get; set; }

	public DateTime? SynchronizationDate { get; set; }

	public bool Deleted => Status == "D";

	public bool IsSuggested { get; set; }

	public SharedObjectTypeEnum.ObjectType ObjectType { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype Subtype { get; set; }

	public UserTypeEnum.UserType? Source { get; set; }

	public SynchronizeStateEnum.SynchronizeState SynchronizeState => SynchronizeStateEnum.DBStringToState(Status);

	public bool IsInTheSameDatabase { get; set; }

	public bool ShowSchema
	{
		get
		{
			if (DatabaseRow.GetShowSchema(DatabaseShowSchema, DatabaseShowSchemaOverride))
			{
				return !string.IsNullOrWhiteSpace(Schema);
			}
			return false;
		}
	}

	public string DisplayName
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (ShowSchema)
			{
				stringBuilder.Append(Schema + ".");
			}
			stringBuilder.Append(BaseName ?? "");
			return stringBuilder.ToString();
		}
	}

	public Image NodeIcon
	{
		get
		{
			if (IsDummyObject)
			{
				return Resources.blank_16;
			}
			return IconsSupport.GetObjectIcon(ObjectType, Subtype, Source ?? UserTypeEnum.UserType.DBMS, SynchronizeState);
		}
	}

	public bool IsNormalObject => SharedObjectTypeEnum.IsRegularObject(ObjectType);

	public Image DatabaseIcon
	{
		get
		{
			if (IsDummyObject)
			{
				return Resources.blank_16;
			}
			return IconsSupport.GetDatabaseIconByName16(SharedDatabaseTypeEnum.StringToType(DatabaseType), ObjectType);
		}
	}

	public bool IsDummyObject { get; set; }

	public ObjectBrowserItem()
	{
	}

	public ObjectBrowserItem(ObjectForMenuObject objectForMenuObject)
	{
		DatabaseId = objectForMenuObject.DatabaseId;
		DatabaseType = objectForMenuObject.DatabaseType;
		DatabaseClass = objectForMenuObject.DatabaseClass;
		DatabaseTitle = objectForMenuObject.DatabaseTitle;
		DatabaseMultipleSchemas = objectForMenuObject.DatabaseMultipleSchemas;
		DatabaseShowSchema = objectForMenuObject.DatabaseShowSchema;
		DatabaseShowSchemaOverride = objectForMenuObject.DatabaseShowSchemaOverride;
		BaseName = objectForMenuObject.BaseName;
		Id = objectForMenuObject.Id;
		Schema = objectForMenuObject.Schema;
		Status = objectForMenuObject.Status;
		Title = objectForMenuObject.Title;
		DbmsLastModificationDate = objectForMenuObject.DbmsLastModificationDate;
		SynchronizationDate = objectForMenuObject.SynchronizationDate;
		ObjectType = SharedObjectTypeEnum.StringToType(objectForMenuObject.ObjectType) ?? SharedObjectTypeEnum.ObjectType.UnresolvedEntity;
		Subtype = SharedObjectSubtypeEnum.StringToType(ObjectType, objectForMenuObject.Subtype);
		Source = UserTypeEnum.ObjectToType(objectForMenuObject.Source);
		IsSuggested = false;
	}

	public void SetParentDatabase(int? parentDatabaseId)
	{
		IsInTheSameDatabase = parentDatabaseId.HasValue && parentDatabaseId == DatabaseId;
	}
}
