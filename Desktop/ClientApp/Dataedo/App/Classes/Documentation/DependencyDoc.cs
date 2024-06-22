using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.Documentation.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Documentation;

public class DependencyDoc : IdStringBase
{
	public Bitmap Icon { get; set; }

	public string Name { get; set; }

	public string Title { get; set; }

	public string RelationTitle { get; set; }

	public string Description { get; set; }

	public Dataedo.App.Data.MetadataServer.Model.DependencyRow Parent { get; set; }

	public DatabaseInfo RowDatabase { get; set; }

	public DatabaseInfo CurrentDatabase { get; set; }

	public IEnumerable<DependencyDoc> Children { get; set; }

	public Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeType DependencyType { get; set; }

	public Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType DependencyCommonType
	{
		get
		{
			switch (DependencyType)
			{
			case Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeType.Relation:
			case Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeType.UserRelation:
			case Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeType.PKRelation:
			case Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeType.PKUserRelation:
				return Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Relation;
			case Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeType.Normal:
				return Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Normal;
			case Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeType.Trigger:
				return Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Trigger;
			default:
				return Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Normal;
			}
		}
	}

	public string DisplayName
	{
		get
		{
			string text = "";
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
			goto IL_0091;
			IL_0068:
			if (!string.IsNullOrEmpty(RowDatabase.Title))
			{
				text = text + RowDatabase.Title + ".";
			}
			goto IL_0091;
			IL_0091:
			if (num == 0 && ((Parent == null && !CurrentDatabase.IsCurrentDocumentation(RowDatabase.DatabaseOrSchemaLower)) || (Parent != null && !Parent.RowDatabase.IsCurrentDocumentation(RowDatabase.DatabaseOrSchemaLower))) && !string.IsNullOrEmpty(RowDatabase.DatabaseOrSchemaLower) && DependencyCommonType != Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Relation)
			{
				text = text + RowDatabase.DatabaseOrSchema + ".";
			}
			if ((DatabaseRow.GetShowSchema(CurrentDatabase.ShowSchema, CurrentDatabase.ShowSchemaOverride) || DatabaseRow.GetShowSchema(RowDatabase.ShowSchema, RowDatabase.ShowSchemaOverride)) && !string.IsNullOrEmpty(RowDatabase.Schema))
			{
				text = text + RowDatabase.Schema + ".";
			}
			return text + Name;
		}
	}

	public string DisplayNameWithObjectTitleAndRelationTitle
	{
		get
		{
			string[] value = new string[3]
			{
				DisplayName,
				(!string.IsNullOrEmpty(Title)) ? (" (" + Title + ")") : null,
				(!string.IsNullOrEmpty(RelationTitle)) ? (" (" + RelationTitle + ")") : null
			};
			return string.Join(" ", value);
		}
	}

	public DependencyDoc(Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow)
	{
		Icon = dependencyRow.Icon;
		Name = dependencyRow.Name;
		Title = dependencyRow.Title;
		RelationTitle = dependencyRow.RelationTitle;
		Description = dependencyRow?.DependencyDescriptionsData?.Description;
		Parent = dependencyRow.Parent;
		RowDatabase = dependencyRow.RowDatabase;
		DependencyType = dependencyRow.DependencyType;
		CurrentDatabase = dependencyRow.CurrentDatabase;
		Children = dependencyRow.Children.Select((Dataedo.App.Data.MetadataServer.Model.DependencyRow x) => new DependencyDoc(x));
		base.IdString = PdfLinksSupport.CreateIdString(RowDatabase.Server, RowDatabase.Database, RowDatabase.Schema, Name);
	}

	public static BindingList<DependencyDoc> GetUsesDependencies(int? databaseId, SharedDatabaseTypeEnum.DatabaseType? databaseType, string server, string database, string databaseTitle, bool? hasMultipleSchemas, bool? showSchema, bool? showSchemaOverride, bool contextShowSchema, string schema, string name, string title, string objectSource, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype, string source, DatabaseInfo currentDatabase, int objectId, Form owner = null)
	{
		try
		{
			return new BindingList<DependencyDoc>(new List<DependencyDoc>(from y in DB.Dependency.GetUses(databaseId, databaseType, server, database, databaseTitle, hasMultipleSchemas, showSchema, showSchemaOverride, contextShowSchema, schema, name, title, objectSource, objectType, subtype, source, currentDatabase, objectId, notDeletedOnly: true, addTriggers: true, 1, 2, notEmptyRootOnly: true, notEmptyTriggersOnly: true)
				select new DependencyDoc(y)));
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting dependencies.", owner);
			return null;
		}
	}

	public static BindingList<DependencyDoc> GetUsedByDependencies(int? databaseId, SharedDatabaseTypeEnum.DatabaseType? databaseType, string server, string database, string databaseTitle, bool? hasMultipleSchemas, bool? showSchema, bool? showSchemaOverride, bool contextShowSchema, string schema, string name, string title, string objectSource, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype, string source, DatabaseInfo currentDatabase, int objectId, Form owner = null)
	{
		try
		{
			return new BindingList<DependencyDoc>(new List<DependencyDoc>(from y in DB.Dependency.GetUsedBy(databaseId, databaseType, server, database, databaseTitle, hasMultipleSchemas, showSchema, showSchemaOverride, contextShowSchema, schema, name, title, objectSource, objectType, subtype, source, currentDatabase, objectId, notDeletedOnly: true, 1, 2, notEmptyRootOnly: true)
				select new DependencyDoc(y)));
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting dependencies.", owner);
			return null;
		}
	}
}
