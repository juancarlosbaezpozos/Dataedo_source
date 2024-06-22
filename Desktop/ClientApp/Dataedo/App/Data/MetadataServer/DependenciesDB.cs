using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.Enums;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Data.Base.Commands.Results;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Dependencies;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer;

internal class DependenciesDB : CommonDBSupport
{
	public List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> GetUsedBy(string server, string database, string databaseTitle, bool? hasMultipleSchemas, bool? showSchema, bool? showSchemaOverride, bool contextShowSchema, string schema, string name, string title, string objectSource, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype objectSubtype, string source, DatabaseInfo currentDatabase, int objectId, bool notDeletedOnly, int? currentLevel = 1, int? maxLevel = null, bool notEmptyRootOnly = false)
	{
		return GetUsedBy(null, null, server, database, databaseTitle, hasMultipleSchemas, showSchemaOverride, contextShowSchema, contextShowSchema, schema, name, title, objectSource, objectType, objectSubtype, source, currentDatabase, objectId, notDeletedOnly, currentLevel, maxLevel, notEmptyRootOnly);
	}

	public List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> GetUsedBy(int? databaseId, SharedDatabaseTypeEnum.DatabaseType? databaseType, string server, string database, string databaseTitle, bool? hasMultipleSchemas, bool? showSchema, bool? showSchemaOverride, bool contextShowSchema, string schema, string name, string title, string objectSource, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype objectSubtype, string source, DatabaseInfo currentDatabase, int objectId, bool notDeletedOnly, int? currentLevel = 1, int? maxLevel = null, bool notEmptyRootOnly = false)
	{
		Dataedo.App.Data.MetadataServer.Model.DependencyRow.UniqueId = 0;
		Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow = new Dataedo.App.Data.MetadataServer.Model.DependencyRow(databaseId ?? currentDatabase.DocumentationId, databaseType ?? currentDatabase.DatabaseType, server, database, databaseTitle, hasMultipleSchemas, showSchema, showSchemaOverride, contextShowSchema, schema, name, title, objectSource, SharedObjectTypeEnum.TypeToString(objectType), objectId, null, objectType, objectSubtype, source, currentDatabase, isUsesObject: false);
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> list = new List<Dataedo.App.Data.MetadataServer.Model.DependencyRow>();
		if (CheckNesting(currentLevel, maxLevel))
		{
			FillUsedBy(dependencyRow, currentDatabase, notDeletedOnly, currentLevel + 1, maxLevel);
			FillAdditionalDependenciesForUsedBy(list, dependencyRow, server, database, schema, name, objectType, source, currentDatabase, objectId);
			dependencyRow.Children = new BindingList<Dataedo.App.Data.MetadataServer.Model.DependencyRow>(Sort(dependencyRow.Children));
		}
		if (notEmptyRootOnly)
		{
			BindingList<Dataedo.App.Data.MetadataServer.Model.DependencyRow> children = dependencyRow.Children;
			if (children == null || children.Count <= 0)
			{
				goto IL_0108;
			}
		}
		list.Insert(0, dependencyRow);
		goto IL_0108;
		IL_0108:
		return list;
	}

	private void FillAdditionalDependenciesForUsedBy(List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> nodes, Dataedo.App.Data.MetadataServer.Model.DependencyRow node, string server, string database, string schema, string name, SharedObjectTypeEnum.ObjectType objectType, string source, DatabaseInfo currentDatabase, int objectId)
	{
		if (objectType == SharedObjectTypeEnum.ObjectType.Table || objectType == SharedObjectTypeEnum.ObjectType.View || node.TypeEnum == SharedObjectTypeEnum.ObjectType.Structure)
		{
			IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> relationsForUsedBy = GetRelationsForUsedBy(node, server, database, schema, name, currentDatabase, objectId);
			AddRange(node.Children, relationsForUsedBy);
		}
	}

	private IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> GetRelationsForUsedBy(Dataedo.App.Data.MetadataServer.Model.DependencyRow node, string server, string database, string schema, string name, DatabaseInfo currentDatabase, int? objectId)
	{
		return commands.Select.Dependencies.GetRelationsForUsedBy(node.RowDatabase.DocumentationId, objectId, server, database, schema, name, node.Type).Select(delegate(RelationDependencyObject x)
		{
			string source = x.Source;
			Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeType dependencyNodeType = ((source == "DBMS") ? Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeType.PKRelation : Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeType.PKUserRelation);
			SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(x.ObjectType);
			return new Dataedo.App.Data.MetadataServer.Model.DependencyRow(++Dataedo.App.Data.MetadataServer.Model.DependencyRow.UniqueId, x.DatabaseId, DatabaseTypeEnum.StringToType(x.DatabaseType), x.Server, x.Database, x.DatabaseTitle, x.MultipleSchemas.GetValueOrDefault(), x.ShowSchema, x.ShowSchemaOverride, node.ContextShowSchema, x.Schema, x.Name, x.Title, x.ObjectSource, x.DestinationObjectType, x.DestinationObjectId, x.RelationTitle, x.FkType, x.PkType, objectType, SharedObjectSubtypeEnum.StringToType(objectType, PrepareValue.ToString(x.Subtype)), source, currentDatabase, isUsesObject: false, node, dependencyNodeType, x.ObjectId, null, null, new DependencyDescriptions
			{
				DocumentationId = currentDatabase.DocumentationId,
				IsDescriptionForUsesDependency = false
			});
		});
	}

	public void FillUsedBy(Dataedo.App.Data.MetadataServer.Model.DependencyRow node, DatabaseInfo currentDatabase, bool notDeletedOnly, int? currentLevel = null, int? maxLevel = null)
	{
		if (node.Children.Count != 0)
		{
			return;
		}
		new List<Dataedo.App.Data.MetadataServer.Model.DependencyRow>();
		List<UsedByDependencyObject> usedBy = commands.Select.Dependencies.GetUsedBy(node.RowDatabase.DocumentationId, node.RowDatabase.Server, node.RowDatabase.DatabaseOrSchema, node.RowDatabase.Schema, node.Name, node.Type, GetNotStatusValue(notDeletedOnly));
		foreach (Dataedo.App.Data.MetadataServer.Model.DependencyRow filteredNode in GetFilteredNodes(node, currentDatabase, usedBy, isUsesObject: false))
		{
			if (filteredNode.TypeEnum != SharedObjectTypeEnum.ObjectType.Trigger)
			{
				_ = filteredNode.DependencyCommonType;
				_ = 2;
			}
			node.Children.Add(filteredNode);
			if (CheckNesting(currentLevel, maxLevel))
			{
				if (currentLevel == 1 && (node.TypeEnum == SharedObjectTypeEnum.ObjectType.Table || node.TypeEnum == SharedObjectTypeEnum.ObjectType.View || node.TypeEnum == SharedObjectTypeEnum.ObjectType.Structure))
				{
					IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> relationsForUsedBy = GetRelationsForUsedBy(filteredNode, filteredNode.RowDatabase.Server, filteredNode.RowDatabase.Database, filteredNode.RowDatabase.Schema, filteredNode.Name, currentDatabase, filteredNode.ObjectId);
					AddRange(filteredNode.Children, Sort(relationsForUsedBy));
				}
				if (!node.HasSameDependencyAncestor(filteredNode))
				{
					FillUsedBy(filteredNode, currentDatabase, notDeletedOnly, currentLevel + 1, maxLevel);
					filteredNode.Children = new BindingList<Dataedo.App.Data.MetadataServer.Model.DependencyRow>(Sort(filteredNode.Children));
				}
			}
		}
	}

	public List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> GetUses(string server, string database, string databaseTitle, bool? hasMultipleSchemas, bool? showSchema, bool? showSchemaOverride, bool contextShowSchema, string schema, string name, string title, string objctSource, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype objectSubtype, string source, DatabaseInfo currentDatabase, int objectId, bool notDeletedOnly, bool addTriggers = true, int? currentLevel = 1, int? maxLevel = null, bool notEmptyRootOnly = false, bool notEmptyTriggersOnly = false)
	{
		return GetUses(null, null, server, database, databaseTitle, hasMultipleSchemas, showSchema, showSchemaOverride, contextShowSchema, schema, name, title, objctSource, objectType, objectSubtype, source, currentDatabase, objectId, notDeletedOnly, addTriggers, currentLevel, maxLevel, notEmptyRootOnly, notEmptyTriggersOnly);
	}

	public List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> GetUses(int? databaseId, SharedDatabaseTypeEnum.DatabaseType? databaseType, string server, string database, string databaseTitle, bool? hasMultipleSchemas, bool? showSchema, bool? showSchemaOverride, bool contextShowSchema, string schema, string name, string title, string objctSource, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype objectSubtype, string source, DatabaseInfo currentDatabase, int objectId, bool notDeletedOnly, bool addTriggers = true, int? currentLevel = 1, int? maxLevel = null, bool notEmptyRootOnly = false, bool notEmptyTriggersOnly = false)
	{
		Dataedo.App.Data.MetadataServer.Model.DependencyRow.UniqueId = 0;
		Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow = new Dataedo.App.Data.MetadataServer.Model.DependencyRow(databaseId ?? currentDatabase.DocumentationId, databaseType ?? currentDatabase.DatabaseType, server, database, databaseTitle, hasMultipleSchemas, showSchema, showSchemaOverride, contextShowSchema, schema, name, title, objctSource, SharedObjectTypeEnum.TypeToString(objectType), objectId, null, objectType, objectSubtype, source, currentDatabase, isUsesObject: true);
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> list = new List<Dataedo.App.Data.MetadataServer.Model.DependencyRow>();
		if (CheckNesting(currentLevel, maxLevel))
		{
			FillUses(dependencyRow, currentDatabase, notDeletedOnly, currentLevel + 1, maxLevel);
			if (dependencyRow.TypeEnum == SharedObjectTypeEnum.ObjectType.Table || dependencyRow.TypeEnum == SharedObjectTypeEnum.ObjectType.View || dependencyRow.TypeEnum == SharedObjectTypeEnum.ObjectType.Structure)
			{
				IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> relationsForUses = GetRelationsForUses(dependencyRow, server, database, schema, name, currentDatabase, objectId);
				AddRange(dependencyRow.Children, relationsForUses);
			}
			dependencyRow.Children = new BindingList<Dataedo.App.Data.MetadataServer.Model.DependencyRow>(Sort(dependencyRow.Children));
		}
		if (addTriggers && (objectType == SharedObjectTypeEnum.ObjectType.Table || objectType == SharedObjectTypeEnum.ObjectType.View || dependencyRow.TypeEnum == SharedObjectTypeEnum.ObjectType.Structure))
		{
			IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> triggersForUses = GetTriggersForUses(dependencyRow, server, database, schema, name, currentDatabase, objectId, notDeletedOnly, null, null, notEmptyTriggersOnly);
			if (dependencyRow.Parent != null)
			{
				AddRange(dependencyRow.Parent.Children, triggersForUses);
				dependencyRow.Parent.Children = new BindingList<Dataedo.App.Data.MetadataServer.Model.DependencyRow>(Sort(dependencyRow.Parent.Children));
			}
			else
			{
				list.AddRange(triggersForUses);
				list = Sort(list);
			}
		}
		if (notEmptyRootOnly)
		{
			BindingList<Dataedo.App.Data.MetadataServer.Model.DependencyRow> children = dependencyRow.Children;
			if (children == null || children.Count <= 0)
			{
				goto IL_01c4;
			}
		}
		list.Insert(0, dependencyRow);
		goto IL_01c4;
		IL_01c4:
		dependencyRow.Children = new BindingList<Dataedo.App.Data.MetadataServer.Model.DependencyRow>(Sort(dependencyRow.Children));
		return list;
	}

	private IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> GetRelationsForUses(Dataedo.App.Data.MetadataServer.Model.DependencyRow node, string server, string database, string schema, string name, DatabaseInfo currentDatabase, int? objectId)
	{
		return commands.Select.Dependencies.GetRelationsForUses(node.RowDatabase.DocumentationId, objectId, server, database, schema, name, node.Type).Select(delegate(RelationDependencyObject x)
		{
			string source = x.Source;
			Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeType dependencyNodeType = ((source == "DBMS") ? Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeType.Relation : Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeType.UserRelation);
			SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(x.ObjectType);
			return new Dataedo.App.Data.MetadataServer.Model.DependencyRow(++Dataedo.App.Data.MetadataServer.Model.DependencyRow.UniqueId, x.DatabaseId, DatabaseTypeEnum.StringToType(x.DatabaseType), x.Server, x.Database, x.DatabaseTitle, x.MultipleSchemas.GetValueOrDefault(), x.ShowSchema, x.ShowSchemaOverride, node.ContextShowSchema, x.Schema, x.Name, x.Title, x.ObjectSource, x.DestinationObjectType, x.DestinationObjectId, x.RelationTitle, x.FkType, x.PkType, objectType, SharedObjectSubtypeEnum.StringToType(objectType, x.ObjectType), source, currentDatabase, isUsesObject: true, node, dependencyNodeType, x.ObjectId, null, null, new DependencyDescriptions
			{
				DocumentationId = currentDatabase.DocumentationId,
				IsDescriptionForUsesDependency = true
			});
		});
	}

	private IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> GetTriggersForUses(Dataedo.App.Data.MetadataServer.Model.DependencyRow node, string server, string database, string schema, string name, DatabaseInfo currentDatabase, int objectId, bool notDeletedOnly, int? currentLevel = null, int? maxLevel = null, bool notEmptyTriggersOnly = false)
	{
		IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> enumerable = commands.Select.Dependencies.GetTriggersForUses(node.RowDatabase.DocumentationId, objectId, node.RowDatabase.Server, node.RowDatabase.Database, node.RowDatabase.Schema, node.Name, node.Type).Select(delegate(TriggerUsesDependencyObject x)
		{
			Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow = new Dataedo.App.Data.MetadataServer.Model.DependencyRow(++Dataedo.App.Data.MetadataServer.Model.DependencyRow.UniqueId, currentDatabase.DocumentationId, currentDatabase.DatabaseType, x.Server, x.Database, x.DatabaseTitle, x.MultipleSchemas.GetValueOrDefault(), x.ShowSchema, x.ShowSchemaOverride, node.ContextShowSchema, x.Schema, x.Name, x.Title, "DBMS", x.DestinationObjectType, x.DestinationObjectId, null, null, null, SharedObjectTypeEnum.ObjectType.Trigger, SharedObjectSubtypeEnum.StringToType(SharedObjectTypeEnum.ObjectType.Trigger, x.Subtype), "DBMS", currentDatabase, isUsesObject: true, node, Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeType.Trigger, x.ObjectId, x.TableName, x.TableTitle, new DependencyDescriptions
			{
				DocumentationId = currentDatabase.DocumentationId,
				IsDescriptionForUsesDependency = true
			})
			{
				IsDisabled = x.IsDisabled
			};
			if (CheckNesting(currentLevel, maxLevel))
			{
				FillUses(dependencyRow, currentDatabase, notDeletedOnly, currentLevel + 1, maxLevel);
				dependencyRow.Children = new BindingList<Dataedo.App.Data.MetadataServer.Model.DependencyRow>(Sort(dependencyRow.Children));
			}
			return dependencyRow;
		});
		if (!notEmptyTriggersOnly)
		{
			return enumerable;
		}
		return enumerable.Where(delegate(Dataedo.App.Data.MetadataServer.Model.DependencyRow x)
		{
			BindingList<Dataedo.App.Data.MetadataServer.Model.DependencyRow> children = x.Children;
			return children != null && children.Count > 0;
		});
	}

	public void FillUses(Dataedo.App.Data.MetadataServer.Model.DependencyRow node, DatabaseInfo currentDatabase, bool notDeletedOnly, int? currentLevel = null, int? maxLevel = null)
	{
		if (node.Children.Count != 0)
		{
			return;
		}
		new List<Dataedo.App.Data.MetadataServer.Model.DependencyRow>();
		List<UsesDependencyObject> uses = commands.Select.Dependencies.GetUses(node.RowDatabase.DocumentationId, node.RowDatabase.Server, node.RowDatabase.DatabaseOrSchema, node.RowDatabase.Schema, node.Name, node.Type);
		foreach (Dataedo.App.Data.MetadataServer.Model.DependencyRow filteredNode in GetFilteredNodes(node, currentDatabase, uses, isUsesObject: true))
		{
			node.Children.Add(filteredNode);
			if (CheckNesting(currentLevel, maxLevel))
			{
				if (currentLevel == 1 && (node.TypeEnum == SharedObjectTypeEnum.ObjectType.Table || node.TypeEnum == SharedObjectTypeEnum.ObjectType.View || node.TypeEnum == SharedObjectTypeEnum.ObjectType.Structure))
				{
					IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> relationsForUses = GetRelationsForUses(filteredNode, filteredNode.RowDatabase.Server, filteredNode.RowDatabase.Database, filteredNode.RowDatabase.Schema, filteredNode.Name, currentDatabase, filteredNode.ObjectId);
					AddRange(filteredNode.Children, Sort(relationsForUses));
				}
				if (!node.HasSameDependencyAncestor(filteredNode))
				{
					FillUses(filteredNode, currentDatabase, notDeletedOnly, currentLevel + 1, maxLevel);
					filteredNode.Children = new BindingList<Dataedo.App.Data.MetadataServer.Model.DependencyRow>(Sort(filteredNode.Children));
				}
			}
		}
	}

	public DependenciesDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public bool HasPermissionForObject(string objectName)
	{
		return HasPermissionForObject(objectName);
	}

	public void UpdateUserDependencies(string newSchema, string newName, string server, string database, string schema, string name, string type)
	{
		commands.Manipulation.Dependencies.UpdateUsedByDependenciesNameAndSchema(newSchema, newName, server, database, schema, name, type);
		commands.Manipulation.Dependencies.UpdateUsesDependenciesNameAndSchema(newSchema, newName, server, database, schema, name, type);
	}

	public void DeleteUserDependencies(IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> dependencies)
	{
		commands.Manipulation.Dependencies.DeleteUserDependencies(from x in dependencies
			where x.DependencyId.HasValue
			select x.DependencyId.Value);
	}

	public void InsertDependencies(IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> dependencies)
	{
		foreach (Dataedo.App.Data.MetadataServer.Model.DependencyRow dependency in dependencies)
		{
			if (dependency.IsUsesObject)
			{
				string dependencyType = ((dependency.Type != "UNRESOLVED_ENTITY") ? dependency.Type : null);
				dependency.DependencyId = InsertDependency(dependency.Parent.RowDatabase.Server, dependency.Parent.RowDatabase.DatabaseOrSchema, dependency.Parent.RowDatabase.Schema, dependency.Parent.Name, dependency.Parent.Type, dependency.RowDatabase.Server, dependency.RowDatabase.DatabaseOrSchema, dependency.RowDatabase.Schema, dependency.Name, dependency.Type, dependencyType);
				dependency.IsSaved = true;
			}
			else
			{
				string dependencyType2 = ((dependency.Parent.Type != "UNRESOLVED_ENTITY") ? dependency.Parent.Type : null);
				dependency.DependencyId = InsertDependency(dependency.RowDatabase.Server, dependency.RowDatabase.DatabaseOrSchema, dependency.RowDatabase.Schema, dependency.Name, dependency.Type, dependency.Parent.RowDatabase.Server, dependency.Parent.RowDatabase.DatabaseOrSchema, dependency.Parent.RowDatabase.Schema, dependency.Parent.Name, dependency.Parent.Type, dependencyType2);
				dependency.IsSaved = true;
			}
		}
	}

	public int? InsertDependency(string referencingServer, string referencingDatabase, string referencingSchema, string referencingName, string referencingType, string referencedServer, string referencedDatabase, string referencedSchema, string referencedName, string referencedType, string dependencyType)
	{
		return commands.Manipulation.Dependencies.InsertDependency(referencingServer, referencingDatabase, referencingSchema, referencingName, referencingType, referencedServer, referencedDatabase, referencedSchema, referencedName, referencedType, isCallerDependent: false, isAmbiguous: false, dependencyType, "A", "USER");
	}

	private static int GetSortValueByType(SharedObjectTypeEnum.ObjectType? objectType, Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType? dependencyType)
	{
		int num = 0;
		num = dependencyType switch
		{
			Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Normal => num, 
			Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Relation => num + 10, 
			Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Trigger => num + 20, 
			_ => num + 90, 
		};
		return objectType switch
		{
			SharedObjectTypeEnum.ObjectType.Table => num + 1, 
			SharedObjectTypeEnum.ObjectType.View => num + 2, 
			SharedObjectTypeEnum.ObjectType.Procedure => num + 3, 
			SharedObjectTypeEnum.ObjectType.Function => num + 4, 
			SharedObjectTypeEnum.ObjectType.Trigger => num + 5, 
			SharedObjectTypeEnum.ObjectType.UnresolvedEntity => num + 6, 
			_ => num + 9, 
		};
	}

	private static int? GetSortValueByOrdinalPosition(int? ordinalPosition)
	{
		return ordinalPosition ?? int.MaxValue;
	}

	public static List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> Sort(IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> list)
	{
		return (from x in list
			orderby (!x.IsRoot) ? 1 : 0, GetSortValueByOrdinalPosition(x.OrdinalPosition), GetSortValueByType(x.TypeEnum, x.DependencyCommonType), x.DisplayName
			select x).ToList();
	}

	private static void AddRange(BindingList<Dataedo.App.Data.MetadataServer.Model.DependencyRow> target, IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> rows)
	{
		foreach (Dataedo.App.Data.MetadataServer.Model.DependencyRow row in rows)
		{
			target.Add(row);
		}
	}

	private bool CheckNesting(int? currentLevel, int? maxLevel)
	{
		if (maxLevel.HasValue && currentLevel.HasValue)
		{
			return currentLevel < maxLevel;
		}
		return true;
	}

	public bool Synchronize(List<Dataedo.App.Classes.Synchronize.DependencyRow> dependencies, string host, IEnumerable<string> databaseNames, bool canReadExternalDependencies, bool isDbAdded, out string[] succeedTablesIds, Form owner = null)
	{
		try
		{
			List<DependencyForSynchronization> list = new List<DependencyForSynchronization>();
			for (int i = 0; i < dependencies.Count; i++)
			{
				list.Add(new DependencyForSynchronization
				{
					ReferencingObject = new DependencyObjectForSynchronization
					{
						Server = dependencies[i].ReferencingServer,
						Database = dependencies[i].ReferencingDatabase,
						Schema = dependencies[i].ReferencingSchema,
						Name = dependencies[i].ReferencingName,
						Type = dependencies[i].ReferencingType
					},
					ReferencedObject = new DependencyObjectForSynchronization
					{
						Server = dependencies[i].ReferencedServer,
						Database = dependencies[i].ReferencedDatabase,
						Schema = dependencies[i].ReferencedSchema,
						Name = dependencies[i].ReferencedName,
						Type = dependencies[i].ReferencedType
					},
					IsCallerDependent = dependencies[i].IsCallerDependent,
					IsAmbiguous = dependencies[i].IsAmbiguous,
					DependencyType = dependencies[i].DependencyType
				});
			}
			List<ValueWithDataArrayResult<int, string>> list2 = new List<ValueWithDataArrayResult<int, string>>();
			foreach (string databaseName in databaseNames)
			{
				list2.AddRange(commands.Synchronization.Dependencies.SynchronizeDependencies(host, databaseName, canReadExternalDependencies, list.ToArray(), isDbAdded));
			}
			succeedTablesIds = list2.Where((ValueWithDataArrayResult<int, string> x) => x.Result == 0).SelectMany((ValueWithDataArrayResult<int, string> x) => x.Data).ToArray();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while synchronizing the dependencies:", owner);
			succeedTablesIds = new string[0];
			return false;
		}
		return true;
	}

	public bool ActualizeDependencies(IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> dependencies, int documentationId, Form owner = null)
	{
		if (dependencies == null || dependencies.Count() == 0)
		{
			return true;
		}
		try
		{
			commands.Manipulation.Dependencies.InsertOrUpdateDependenciesDescriptions(ConvertDependencies(dependencies, documentationId));
			commands.Manipulation.Relations.UpdateRelationsDescriptions((from r in dependencies
				where r.Parent != null && r.DependencyCommonType == Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Relation
				select new ObjectIdDescription
				{
					BaseId = r.ObjectId.Value,
					Description = r.Description
				}).ToArray());
			foreach (Dataedo.App.Data.MetadataServer.Model.DependencyRow dependency in dependencies)
			{
				dependency.Changed = false;
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the dependency:", owner);
			return false;
		}
		return true;
	}

	private DependencyDescriptions[] ConvertDependencies(IEnumerable<Dataedo.App.Data.MetadataServer.Model.DependencyRow> dependencies, int documentationId)
	{
		return dependencies.Where((Dataedo.App.Data.MetadataServer.Model.DependencyRow r) => r.Parent != null).Select(delegate(Dataedo.App.Data.MetadataServer.Model.DependencyRow r)
		{
			DependencyDescriptions dependencyDescriptions = new DependencyDescriptions
			{
				Id = r?.DependencyDescriptionsData?.Id,
				DocumentationId = documentationId,
				Description = ((r.DependencyCommonType != Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Relation) ? r.Description : null),
				OrdinalPosition = r.OrdinalPosition,
				IsDescriptionForUsesDependency = r.IsUsesObject
			};
			DependencyDescriptionsObjectInformation dependencyDescriptionsObjectInformation = new DependencyDescriptionsObjectInformation
			{
				Server = r.RowDatabase.Server,
				Database = r.RowDatabase.Database,
				Schema = r.RowDatabase.Schema,
				Name = r.Name,
				Type = r.Type
			};
			DependencyDescriptionsObjectInformation dependencyDescriptionsObjectInformation2 = new DependencyDescriptionsObjectInformation
			{
				Server = r.Parent?.RowDatabase.Server,
				Database = r.Parent?.RowDatabase.Database,
				Schema = r.Parent?.RowDatabase.Schema,
				Name = r.Parent?.Name,
				Type = r.Parent?.Type
			};
			if (r.IsUsesObject)
			{
				dependencyDescriptions.ReferencingObject = dependencyDescriptionsObjectInformation2;
				dependencyDescriptions.ReferencedObject = dependencyDescriptionsObjectInformation;
			}
			else
			{
				dependencyDescriptions.ReferencingObject = dependencyDescriptionsObjectInformation;
				dependencyDescriptions.ReferencedObject = dependencyDescriptionsObjectInformation2;
			}
			return dependencyDescriptions;
		})?.ToArray();
	}

	private List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> GetFilteredNodes(Dataedo.App.Data.MetadataServer.Model.DependencyRow node, DatabaseInfo currentDatabase, List<UsesDependencyObject> nodesView, bool isUsesObject)
	{
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> list = new List<Dataedo.App.Data.MetadataServer.Model.DependencyRow>();
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> list2 = new List<Dataedo.App.Data.MetadataServer.Model.DependencyRow>();
		for (int i = 0; i < nodesView.Count; i++)
		{
			Dataedo.App.Data.MetadataServer.Model.DependencyRow item = new Dataedo.App.Data.MetadataServer.Model.DependencyRow(nodesView[i], ++Dataedo.App.Data.MetadataServer.Model.DependencyRow.UniqueId, node, isUsesObject);
			list.Add(item);
		}
		foreach (var item2 in from x in list
			group x by new
			{
				x.RowDatabase.ServerLower,
				x.RowDatabase.DatabaseLower,
				x.DependencyType,
				x.Type,
				x.RowDatabase.Schema,
				x.Name
			})
		{
			if (item2 != null && item2.Count() == 1)
			{
				Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow = item2.FirstOrDefault();
				if (dependencyRow != null)
				{
					list2.Add(dependencyRow);
				}
				continue;
			}
			Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow2 = item2.FirstOrDefault((Dataedo.App.Data.MetadataServer.Model.DependencyRow x) => x.RowDatabase.DocumentationId == node.RowDatabase.DocumentationId);
			if (dependencyRow2 != null && dependencyRow2.DestinationObjectId.HasValue)
			{
				list2.Add(dependencyRow2);
			}
			else if (item2.Any((Dataedo.App.Data.MetadataServer.Model.DependencyRow x) => x.DestinationObjectId.HasValue))
			{
				list2.AddRange(item2.Where((Dataedo.App.Data.MetadataServer.Model.DependencyRow x) => x.DestinationObjectId.HasValue));
			}
			else if (dependencyRow2 == null)
			{
				list2.AddRange(item2);
			}
			else
			{
				list2.Add(dependencyRow2);
			}
		}
		return list2;
	}

	private List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> GetFilteredNodes(Dataedo.App.Data.MetadataServer.Model.DependencyRow node, DatabaseInfo currentDatabase, List<UsedByDependencyObject> nodesView, bool isUsesObject)
	{
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> list = new List<Dataedo.App.Data.MetadataServer.Model.DependencyRow>();
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> list2 = new List<Dataedo.App.Data.MetadataServer.Model.DependencyRow>();
		for (int i = 0; i < nodesView.Count; i++)
		{
			Dataedo.App.Data.MetadataServer.Model.DependencyRow item = new Dataedo.App.Data.MetadataServer.Model.DependencyRow(nodesView[i], ++Dataedo.App.Data.MetadataServer.Model.DependencyRow.UniqueId, node, isUsesObject);
			list.Add(item);
		}
		foreach (var item2 in from x in list
			group x by new
			{
				x.RowDatabase.ServerLower,
				x.RowDatabase.DatabaseLower,
				x.DependencyType,
				x.Type,
				x.RowDatabase.Schema,
				x.Name
			})
		{
			if (item2 != null && item2.Count() == 1)
			{
				Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow = item2.FirstOrDefault();
				if (dependencyRow != null)
				{
					list2.Add(dependencyRow);
				}
				continue;
			}
			Dataedo.App.Data.MetadataServer.Model.DependencyRow dependencyRow2 = item2.FirstOrDefault((Dataedo.App.Data.MetadataServer.Model.DependencyRow x) => x.RowDatabase.DocumentationId == node.RowDatabase.DocumentationId);
			if (dependencyRow2 != null && dependencyRow2.DestinationObjectId.HasValue)
			{
				list2.Add(dependencyRow2);
			}
			else if (item2.Any((Dataedo.App.Data.MetadataServer.Model.DependencyRow x) => x.DestinationObjectId.HasValue))
			{
				list2.AddRange(item2.Where((Dataedo.App.Data.MetadataServer.Model.DependencyRow x) => x.DestinationObjectId.HasValue));
			}
			else if (dependencyRow2 == null)
			{
				list2.AddRange(item2);
			}
			else
			{
				list2.Add(dependencyRow2);
			}
		}
		return list2;
	}

	internal static string ConvertToString(object toConvert)
	{
		if (!Convert.IsDBNull(toConvert))
		{
			return Convert.ToString(toConvert);
		}
		return null;
	}
}
