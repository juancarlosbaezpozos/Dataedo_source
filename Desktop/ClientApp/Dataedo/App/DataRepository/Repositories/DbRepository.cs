using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Repositories.DbModels;
using Dataedo.App.Documentation;
using Dataedo.App.Enums;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Export;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Documentations;
using Dataedo.Model.Data.Modules;
using Dataedo.Model.Data.Procedures.Parameters;
using Dataedo.Model.Data.Procedures.Procedures;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Model.Data.Tables.Constraints;
using Dataedo.Model.Data.Tables.Relations;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Model.Data.Tables.Triggers;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Repositories;

internal class DbRepository : IRepository
{
	public readonly bool NotDeletedOnly = true;

	public readonly DocumentationModulesContainer Scope;

	public readonly ExcludedObjects Excluded;

	public readonly CustomFieldsSupport CustomFields;

	public readonly OtherFieldsSupport OtherFields;

	public DbRepository(DocumentationModulesContainer scope = null, ExcludedObjects excluded = null, CustomFieldsSupport customFields = null, OtherFieldsSupport otherFields = null)
	{
		Scope = scope;
		Excluded = excluded;
		CustomFields = customFields;
		OtherFields = otherFields;
	}

	public virtual IList<IBusinessGlossary> GetBusinessGlossaries()
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.BusinessGlossary))
		{
			return null;
		}
		List<IBusinessGlossary> list = new List<IBusinessGlossary>();
		foreach (BusinessGlossaryObject businessGlossary2 in DB.BusinessGlossary.GetBusinessGlossaries(null))
		{
			int value = businessGlossary2.DocumentationId.Value;
			DocumentationModulesContainer scope = Scope;
			if (scope == null || scope.IsDatabaseSelected(value))
			{
				IBusinessGlossary businessGlossary = GetBusinessGlossary(value);
				if (businessGlossary != null)
				{
					list.Add(businessGlossary);
				}
			}
		}
		return list.OrderBy((IBusinessGlossary x) => x.Title).ToList();
	}

	public virtual IBusinessGlossary GetBusinessGlossary(int businessGlossaryId)
	{
		BusinessGlossaryObject row = DB.BusinessGlossary.GetBusinessGlossaries(businessGlossaryId).First();
		return new BusinessGlossary(this, row);
	}

	public IList<ITerm> GetRootTerms(int businessGlossaryId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Term))
		{
			return new List<ITerm>();
		}
		List<ITerm> list = new List<ITerm>();
		List<TermObject> rootTerms = DB.BusinessGlossary.GetRootTerms(businessGlossaryId);
		List<TermTypeObject> termTypes = DB.BusinessGlossary.GetTermTypes();
		foreach (TermObject row in rootTerms)
		{
			int value = row.TermId.Value;
			ITerm term = GetTerm(value);
			if (term != null)
			{
				SharedObjectTypeEnum.ObjectType? objectType = SharedTermTypeEnum.TermTypeToObjectType(SharedTermTypeEnum.StringToType(termTypes?.Where((TermTypeObject x) => x.Title == row.TypeTitle)?.FirstOrDefault()?.Code));
				if (!objectType.HasValue || !Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.BusinessGlossary, objectType.Value))
				{
					list.Add(term);
				}
			}
		}
		return list.OrderBy((ITerm x) => x.Title).ToList();
	}

	public virtual IList<ITerm> GetChildrenTerms(int termId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Term))
		{
			return new List<ITerm>();
		}
		List<ITerm> list = new List<ITerm>();
		foreach (TermObject term2 in DB.BusinessGlossary.GetTerms(termId))
		{
			int value = term2.TermId.Value;
			ITerm term = GetTerm(value);
			if (term != null)
			{
				list.Add(term);
			}
		}
		return list.OrderBy((ITerm x) => x.Title).ToList();
	}

	public virtual ITerm GetTerm(int termId)
	{
		TermObject term = DB.BusinessGlossary.GetTerm(termId);
		return new Term(this, term);
	}

	public virtual IList<ITermRelation> GetTermRelations(int termId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Term, SharedObjectTypeEnum.ObjectType.TermRelationship))
		{
			return null;
		}
		return (from x in DB.BusinessGlossary.GetTermRelationships(termId)
			select new TermRelation(this, x)).Cast<ITermRelation>().ToList();
	}

	public virtual IList<ITermDataLink> GetTermDataLinks(int termId)
	{
		return (from x in DB.BusinessGlossary.GetDataLinks<DataLinkObjectExtended>(termId)
			select new TermDataLink(this, x)).Cast<ITermDataLink>().ToList();
	}

	public virtual IList<ITerm> GetTableLinkedTerms(int tableId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.BusinessGlossary))
		{
			return null;
		}
		return (from x in DB.BusinessGlossary.GetDataLinks<DataLinkObjectExtended>(null, tableId)
			select new TermDataLink(this, x) into x
			where x.ObjectType == SharedObjectTypeEnum.ObjectType.Table && !x.InnerObjectType.HasValue
			select x.Term into x
			where IsTypeNotExcluded(x?.TypeIconId)
			select x).Cast<ITerm>().ToList();
	}

	public virtual IList<ITerm> GetViewLinkedTerms(int viewId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.BusinessGlossary))
		{
			return null;
		}
		return (from x in DB.BusinessGlossary.GetDataLinks<DataLinkObjectExtended>(null, viewId)
			select new TermDataLink(this, x) into x
			where x.ObjectType == SharedObjectTypeEnum.ObjectType.View && !x.InnerObjectType.HasValue
			select x.Term into x
			where IsTypeNotExcluded(x?.TypeIconId)
			select x).Cast<ITerm>().ToList();
	}

	public virtual IList<ITerm> GetStructureLinkedTerms(int structureId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.BusinessGlossary))
		{
			return null;
		}
		return (from x in DB.BusinessGlossary.GetDataLinks<DataLinkObjectExtended>(null, structureId)
			select new TermDataLink(this, x) into x
			where x.ObjectType == SharedObjectTypeEnum.ObjectType.Structure && !x.InnerObjectType.HasValue
			select x.Term into x
			where IsTypeNotExcluded(x?.TypeIconId)
			select x).Cast<ITerm>().ToList();
	}

	public virtual IList<ITerm> GetColumnLinkedTerms(int tableOrViewId, int columnId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.BusinessGlossary))
		{
			return null;
		}
		return (from x in DB.BusinessGlossary.GetDataLinks<DataLinkObjectExtended>(null, tableOrViewId)
			select new TermDataLink(this, x) into x
			where x.InnerObjectId == columnId && x.InnerObjectType == SharedObjectTypeEnum.ObjectType.Column
			select x.Term into x
			where IsTypeNotExcluded(x?.TypeIconId)
			select x).Cast<ITerm>().ToList();
	}

	public virtual IDocumentation GetDocumentation(int documentationId)
	{
		DocumentationObject dataById = DB.Database.GetDataById(documentationId);
		if ((DatabaseTypeEnum.StringToType(dataById.Type) == SharedDatabaseTypeEnum.DatabaseType.MongoDB || DatabaseTypeEnum.StringToType(dataById.Type) == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) && dataById.DifferentSchema == true)
		{
			dataById.Name = null;
		}
		return new Dataedo.App.DataRepository.Repositories.DbModels.Documentation(this, dataById);
	}

	public virtual IList<IDocumentation> GetDocumentations()
	{
		ExcludedObjects excluded = Excluded;
		if (excluded != null && excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Database))
		{
			return new List<IDocumentation>();
		}
		List<IDocumentation> list = new List<IDocumentation>();
		foreach (DocumentationModules selectedDocumentation in Scope.GetSelectedDocumentations())
		{
			int idValue = selectedDocumentation.Documentation.IdValue;
			DocumentationModulesContainer scope = Scope;
			if ((scope == null || scope.IsDocumentationSelected(idValue)) && GetDocumentation(idValue) is Dataedo.App.DataRepository.Repositories.DbModels.Documentation documentation)
			{
				Dataedo.App.DataRepository.Repositories.DbModels.Documentation documentation2 = (Dataedo.App.DataRepository.Repositories.DbModels.Documentation)documentation.Clone();
				documentation2.Name = (Excluded.IsExluded(CustomExcludedTypeEnum.CustomExcludedType.DatabaseName) ? "" : documentation2.Name);
				documentation2.Host = (Excluded.IsExluded(CustomExcludedTypeEnum.CustomExcludedType.HostName) ? "" : documentation2.Host);
				list.Add(documentation2);
			}
		}
		return list.OrderBy((IDocumentation x) => x.Title).ToList();
	}

	public virtual IEnumerable<CustomFieldRowExtended> GetCustomFields()
	{
		if (CustomFields == null)
		{
			return new List<CustomFieldRowExtended>();
		}
		return from x in CustomFields.Fields
			where x.IsSelected
			orderby x.OrdinalPosition
			select x;
	}

	private bool IsObjectModuleExcluded(int documentationId, Func<List<int>> modulesCallback)
	{
		if (Scope == null)
		{
			return false;
		}
		IEnumerable<IModule> enumerable = from x in modulesCallback()
			select GetModule(x) into x
			where x.Documentation.Id == documentationId
			select x;
		if ((Scope.SelectedDocumentations?.First((DocumentationModules x) => x.Documentation.Id == documentationId)?.Modules ?? new List<ModuleRow>()).Count() == 0)
		{
			return false;
		}
		if (enumerable.Count() == 0 && Scope.IsModuleSelected(documentationId, -1))
		{
			return false;
		}
		foreach (IModule item in enumerable)
		{
			if (Scope.IsModuleSelected(item.Id))
			{
				return false;
			}
		}
		return true;
	}

	public virtual IFunction GetFunction(int functionId)
	{
		ProcedureObject dataById = DB.Procedure.GetDataById(functionId);
		return new Function(this, dataById);
	}

	public virtual IList<IFunction> GetFunctions(int documentationId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Function))
		{
			return new List<IFunction>();
		}
		List<IFunction> list = new List<IFunction>();
		foreach (int row in DB.Procedure.GetFunctionsIdsByDatabase(documentationId, NotDeletedOnly))
		{
			if (!IsObjectModuleExcluded(documentationId, () => DB.Procedure.GetProcedureModules(row)))
			{
				IFunction function = GetFunction(row);
				if (function != null)
				{
					list.Add(function);
				}
			}
		}
		return list;
	}

	public virtual IModule GetModule(int moduleId)
	{
		ModuleObject dataById = DB.Module.GetDataById(moduleId);
		return new Module(this, dataById, !Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Erd));
	}

	public virtual IList<IFunction> GetModuleFunctions(int moduleId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Function))
		{
			return new List<IFunction>();
		}
		List<IFunction> list = new List<IFunction>();
		foreach (int item in DB.Procedure.GetFunctionsIdsByModule(moduleId, NotDeletedOnly))
		{
			if (GetFunction(item) is Function function)
			{
				Function function2 = (Function)function.Clone();
				function2.Module = GetModule(moduleId);
				list.Add(function2);
			}
		}
		return list;
	}

	public virtual IList<IProcedure> GetModuleProcedures(int moduleId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Procedure))
		{
			return new List<IProcedure>();
		}
		List<IProcedure> list = new List<IProcedure>();
		foreach (int item in DB.Procedure.GetProceduresIdsByModule(moduleId, NotDeletedOnly))
		{
			if (GetProcedure(item) is Procedure procedure)
			{
				Procedure procedure2 = (Procedure)procedure.Clone();
				procedure2.Module = GetModule(moduleId);
				list.Add(procedure2);
			}
		}
		return list;
	}

	public virtual IList<IModule> GetModules(int documentationId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Module))
		{
			return new List<IModule>();
		}
		List<IModule> list = new List<IModule>();
		foreach (int? item in DB.Module.GetDataIdsByDatabase(documentationId))
		{
			DocumentationModulesContainer scope = Scope;
			if (scope == null || scope.IsModuleSelected(item.Value))
			{
				IModule module = GetModule(item.Value);
				if (module != null)
				{
					list.Add(module);
				}
			}
		}
		return list;
	}

	public virtual IList<ITable> GetModuleTables(int moduleId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Table))
		{
			return new List<ITable>();
		}
		List<ITable> list = new List<ITable>();
		foreach (int item in DB.Table.GetTablesIdsByModule(moduleId, NotDeletedOnly))
		{
			if (GetTable(item) is Table table)
			{
				Table table2 = (Table)table.Clone();
				table2.Module = GetModule(moduleId);
				list.Add(table2);
			}
		}
		return list;
	}

	public virtual IList<IView> GetModuleViews(int moduleId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.View))
		{
			return new List<IView>();
		}
		List<IView> list = new List<IView>();
		foreach (int item in DB.Table.GetViewsIdsByModule(moduleId, NotDeletedOnly))
		{
			if (GetView(item) is View view)
			{
				View view2 = (View)view.Clone();
				view2.Module = GetModule(moduleId);
				list.Add(view2);
			}
		}
		return list;
	}

	public virtual IList<IStructure> GetModuleStructures(int moduleId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Structure))
		{
			return new List<IStructure>();
		}
		List<IStructure> list = new List<IStructure>();
		foreach (int item in DB.Table.GetStructuresIdsByModule(moduleId, NotDeletedOnly))
		{
			if (GetStructure(item) is Structure structure)
			{
				Structure structure2 = (Structure)structure.Clone();
				structure2.Module = GetModule(moduleId);
				list.Add(structure2);
			}
		}
		return list;
	}

	public virtual IProcedure GetProcedure(int procedureId)
	{
		ProcedureObject dataById = DB.Procedure.GetDataById(procedureId);
		return new Procedure(this, dataById);
	}

	public virtual IList<IProcedure> GetProcedures(int documentationId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Procedure))
		{
			return new List<IProcedure>();
		}
		List<IProcedure> list = new List<IProcedure>();
		foreach (int row in DB.Procedure.GetProceduresIdsByDatabase(documentationId, NotDeletedOnly))
		{
			if (!IsObjectModuleExcluded(documentationId, () => DB.Procedure.GetProcedureModules(row)))
			{
				IProcedure procedure = GetProcedure(row);
				if (procedure != null)
				{
					list.Add(procedure);
				}
			}
		}
		return list;
	}

	public virtual ITable GetTable(int tableId)
	{
		TableObject dataById = DB.Table.GetDataById(tableId);
		return new Table(this, dataById);
	}

	public virtual IList<ITable> GetTables(int documentationId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Table))
		{
			return new List<ITable>();
		}
		List<ITable> list = new List<ITable>();
		foreach (int row in DB.Table.GetTablesIdsByDatabase(documentationId, NotDeletedOnly))
		{
			if (!IsObjectModuleExcluded(documentationId, () => DB.Table.GetTableModules(row)))
			{
				ITable table = GetTable(row);
				if (table != null)
				{
					list.Add(table);
				}
			}
		}
		return list;
	}

	public virtual IView GetView(int viewId)
	{
		TableObject dataById = DB.Table.GetDataById(viewId);
		return new View(this, dataById);
	}

	public virtual IList<IView> GetViews(int documentationId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.View))
		{
			return new List<IView>();
		}
		List<IView> list = new List<IView>();
		foreach (int row in DB.Table.GetViewsIdsByDatabase(documentationId, NotDeletedOnly))
		{
			if (!IsObjectModuleExcluded(documentationId, () => DB.Table.GetTableModules(row)))
			{
				IView view = GetView(row);
				if (view != null)
				{
					list.Add(view);
				}
			}
		}
		return list;
	}

	public virtual IStructure GetStructure(int structureId)
	{
		TableObject dataById = DB.Table.GetDataById(structureId);
		return new Structure(this, dataById);
	}

	public virtual IList<IStructure> GetStructures(int documentationId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Structure))
		{
			return new List<IStructure>();
		}
		List<IStructure> list = new List<IStructure>();
		foreach (int row in DB.Table.GetStructuresIdsByDatabase(documentationId, NotDeletedOnly))
		{
			if (!IsObjectModuleExcluded(documentationId, () => DB.Table.GetTableModules(row)))
			{
				IStructure structure = GetStructure(row);
				if (structure != null)
				{
					list.Add(structure);
				}
			}
		}
		return list;
	}

	protected virtual IList<IColumn> GetColumns(int objectId)
	{
		return (from x in DB.Column.GetHierarchicalDataByTable(objectId, notDeletedOnly: true)
			select new Column(this, x, Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Key) ? null : GetKeys(objectId), Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Relation) ? null : GetRelations(objectId))).Cast<IColumn>().ToList();
	}

	public virtual IList<IColumn> GetTableColumns(int tableId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Column))
		{
			return null;
		}
		return GetColumns(tableId);
	}

	public virtual IList<IColumn> GetViewColumns(int viewId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Column))
		{
			return null;
		}
		return GetColumns(viewId);
	}

	public virtual IList<IColumn> GetStructureColumns(int structureId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Column))
		{
			return null;
		}
		return GetColumns(structureId);
	}

	protected virtual IList<Relation> GetRelations(int objectId)
	{
		return (from x in DB.Relation.GetDataByTableWithColumnsAndUniqueConstraints(objectId, NotDeletedOnly)
			group x by $"{x.RelationWithUniqueConstraint.TableRelationId}-{x.RelationWithUniqueConstraint.TableFkId}-{x.RelationWithUniqueConstraint.ColumnFkId}-{x.RelationWithUniqueConstraint.TablePkId}-{x.RelationWithUniqueConstraint.ColumnPkId}" into x
			select x.First() into x
			group x by x.RelationWithUniqueConstraint.TableRelationId into x
			select new Relation(this, x)).ToList();
	}

	public virtual IList<IRelation> GetTableRelations(int tableId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Relation))
		{
			return null;
		}
		IDocumentation documentation = GetTable(tableId).Documentation;
		IList<Relation> relations = GetRelations(tableId);
		foreach (Relation item in relations)
		{
			item.Documentation = documentation;
		}
		return relations.Cast<IRelation>().ToList();
	}

	public virtual IList<IRelation> GetViewRelations(int viewId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Relation))
		{
			return null;
		}
		IDocumentation documentation = GetView(viewId).Documentation;
		IList<Relation> relations = GetRelations(viewId);
		foreach (Relation item in relations)
		{
			item.Documentation = documentation;
		}
		return relations.Cast<IRelation>().ToList();
	}

	public virtual IList<IRelation> GetStructureRelations(int structureId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Relation))
		{
			return null;
		}
		IDocumentation documentation = GetStructure(structureId).Documentation;
		IList<Relation> relations = GetRelations(structureId);
		foreach (Relation item in relations)
		{
			item.Documentation = documentation;
		}
		return relations.Cast<IRelation>().ToList();
	}

	public virtual IList<IModule> GetModules(IEnumerable<int> modulesIds)
	{
		List<IModule> list = new List<IModule>();
		foreach (int modulesId in modulesIds)
		{
			IModule module = GetModule(modulesId);
			if (module != null)
			{
				list.Add(module);
			}
		}
		return list;
	}

	protected virtual IList<IKey> GetKeys(int objectId)
	{
		return (from x in (from x in DB.Constraint.GetDataWithColumnsByTableDoc(objectId, NotDeletedOnly)
				select new Key(this, x)).ToList()
			group x by x.Id).Select(delegate(IGrouping<int, Key> x)
		{
			Key key = x.First();
			key.SetColumns(x);
			return key;
		}).Cast<IKey>().ToList();
	}

	public virtual IList<IKey> GetTableKeys(int tableId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Key))
		{
			return null;
		}
		return GetKeys(tableId);
	}

	public virtual IList<IKey> GetViewKeys(int viewId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Key))
		{
			return null;
		}
		return GetKeys(viewId);
	}

	public virtual IList<IKey> GetStructureKeys(int structureId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Key))
		{
			return null;
		}
		return GetKeys(structureId);
	}

	protected virtual IList<ITrigger> GetTriggers(int objectId, SharedObjectTypeEnum.ObjectType objectType)
	{
		return (from x in DB.Trigger.GetDataByTable(objectId, NotDeletedOnly)
			select new Trigger(this, objectType, x)).Cast<ITrigger>().ToList();
	}

	public virtual IList<ITrigger> GetTableTriggers(int tableId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Trigger))
		{
			return null;
		}
		return GetTriggers(tableId, SharedObjectTypeEnum.ObjectType.Table);
	}

	public virtual IList<ITrigger> GetViewTriggers(int viewId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Trigger))
		{
			return null;
		}
		return GetTriggers(viewId, SharedObjectTypeEnum.ObjectType.View);
	}

	public virtual IDependencies GetTableDependencies(int tableId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Dependency))
		{
			return null;
		}
		ITable table = GetTable(tableId);
		IDocumentation documentation = table.Documentation;
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> uses = DB.Dependency.GetUses(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, DatabaseRow.GetShowSchema(documentation.ShowSchema, documentation.ShowSchemaOverride), table.Schema, table.Name, table.Title, UserTypeEnum.TypeToString(table.Source), SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.None, "DBMS", new DatabaseInfo(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, table.Schema), tableId, NotDeletedOnly, addTriggers: true, 1, null, notEmptyRootOnly: true);
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> usedBy = DB.Dependency.GetUsedBy(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, DatabaseRow.GetShowSchema(documentation.ShowSchema, documentation.ShowSchemaOverride), table.Schema, table.Name, table.Title, UserTypeEnum.TypeToString(table.Source), SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.None, "DBMS", new DatabaseInfo(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, table.Schema), tableId, NotDeletedOnly, 1, null, notEmptyRootOnly: true);
		return new Dependencies(this, uses, usedBy);
	}

	public virtual IDependencies GetViewDependencies(int viewId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Dependency))
		{
			return null;
		}
		IView view = GetView(viewId);
		IDocumentation documentation = view.Documentation;
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> uses = DB.Dependency.GetUses(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, DatabaseRow.GetShowSchema(documentation.ShowSchema, documentation.ShowSchemaOverride), view.Schema, view.Name, view.Title, UserTypeEnum.TypeToString(view.Source), SharedObjectTypeEnum.ObjectType.View, SharedObjectSubtypeEnum.ObjectSubtype.None, "DBMS", new DatabaseInfo(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, view.Schema), viewId, NotDeletedOnly, addTriggers: true, 1, null, notEmptyRootOnly: true);
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> usedBy = DB.Dependency.GetUsedBy(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, DatabaseRow.GetShowSchema(documentation.ShowSchema, documentation.ShowSchemaOverride), view.Schema, view.Name, view.Title, UserTypeEnum.TypeToString(view.Source), SharedObjectTypeEnum.ObjectType.View, SharedObjectSubtypeEnum.ObjectSubtype.None, "DBMS", new DatabaseInfo(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, view.Schema), viewId, NotDeletedOnly, 1, null, notEmptyRootOnly: true);
		return new Dependencies(this, uses, usedBy);
	}

	public virtual IDependencies GetStructureDependencies(int structureId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Dependency))
		{
			return null;
		}
		IStructure structure = GetStructure(structureId);
		IDocumentation documentation = structure.Documentation;
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> uses = DB.Dependency.GetUses(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, DatabaseRow.GetShowSchema(documentation.ShowSchema, documentation.ShowSchemaOverride), structure.Schema, structure.Name, structure.Title, UserTypeEnum.TypeToString(structure.Source), SharedObjectTypeEnum.ObjectType.Structure, SharedObjectSubtypeEnum.ObjectSubtype.None, "DBMS", new DatabaseInfo(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, structure.Schema), structureId, NotDeletedOnly, addTriggers: true, 1, null, notEmptyRootOnly: true);
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> usedBy = DB.Dependency.GetUsedBy(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, DatabaseRow.GetShowSchema(documentation.ShowSchema, documentation.ShowSchemaOverride), structure.Schema, structure.Name, structure.Title, UserTypeEnum.TypeToString(structure.Source), SharedObjectTypeEnum.ObjectType.Structure, SharedObjectSubtypeEnum.ObjectSubtype.None, "DBMS", new DatabaseInfo(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, structure.Schema), structureId, NotDeletedOnly, 1, null, notEmptyRootOnly: true);
		return new Dependencies(this, uses, usedBy);
	}

	public virtual IDependencies GetProcedureDependencies(int procedureId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectTypeEnum.ObjectType.Dependency))
		{
			return null;
		}
		IProcedure procedure = GetProcedure(procedureId);
		IDocumentation documentation = procedure.Documentation;
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> uses = DB.Dependency.GetUses(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, DatabaseRow.GetShowSchema(documentation.ShowSchema, documentation.ShowSchemaOverride), procedure.Schema, procedure.Name, procedure.Title, UserTypeEnum.TypeToString(procedure.Source), SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectSubtypeEnum.ObjectSubtype.None, "DBMS", new DatabaseInfo(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, procedure.Schema), procedureId, NotDeletedOnly, addTriggers: true, 1, null, notEmptyRootOnly: true);
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> usedBy = DB.Dependency.GetUsedBy(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, DatabaseRow.GetShowSchema(documentation.ShowSchema, documentation.ShowSchemaOverride), procedure.Schema, procedure.Name, procedure.Title, UserTypeEnum.TypeToString(procedure.Source), SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectSubtypeEnum.ObjectSubtype.None, "DBMS", new DatabaseInfo(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, procedure.Schema), procedureId, NotDeletedOnly, 1, null, notEmptyRootOnly: true);
		return new Dependencies(this, uses, usedBy);
	}

	public virtual IDependencies GetFunctionDependencies(int functionId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Function, SharedObjectTypeEnum.ObjectType.Dependency))
		{
			return null;
		}
		IFunction function = GetFunction(functionId);
		IDocumentation documentation = function.Documentation;
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> uses = DB.Dependency.GetUses(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, DatabaseRow.GetShowSchema(documentation.ShowSchema, documentation.ShowSchemaOverride), function.Schema, function.Name, function.Title, UserTypeEnum.TypeToString(function.Source), SharedObjectTypeEnum.ObjectType.Function, SharedObjectSubtypeEnum.ObjectSubtype.None, "DBMS", new DatabaseInfo(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, function.Schema), functionId, NotDeletedOnly, addTriggers: true, 1, null, notEmptyRootOnly: true);
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> usedBy = DB.Dependency.GetUsedBy(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, DatabaseRow.GetShowSchema(documentation.ShowSchema, documentation.ShowSchemaOverride), function.Schema, function.Name, function.Title, UserTypeEnum.TypeToString(function.Source), SharedObjectTypeEnum.ObjectType.Function, SharedObjectSubtypeEnum.ObjectSubtype.None, "DBMS", new DatabaseInfo(documentation.Id, documentation.Type, documentation.Host, documentation.Name, documentation.Title, documentation.HasMultipleSchemas, documentation.ShowSchema, documentation.ShowSchemaOverride, function.Schema), functionId, NotDeletedOnly, 1, null, notEmptyRootOnly: true);
		return new Dependencies(this, uses, usedBy);
	}

	protected virtual IList<IParameter> GetParameters(int objectId)
	{
		return (from x in DB.Parameter.GetDataByProcedureId(objectId, NotDeletedOnly)
			select new Parameter(this, x)).Cast<IParameter>().ToList();
	}

	public virtual IList<IParameter> GetProcedureParameters(int procedureId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectTypeEnum.ObjectType.Parameter))
		{
			return null;
		}
		return GetParameters(procedureId);
	}

	public virtual IList<IParameter> GetFunctionParameters(int functionId)
	{
		if (Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.Function, SharedObjectTypeEnum.ObjectType.Parameter))
		{
			return null;
		}
		return GetParameters(functionId);
	}

	public virtual IList<IModule> GetTableModules(int tableId)
	{
		List<IModule> list = new List<IModule>();
		foreach (int tableModule in DB.Table.GetTableModules(tableId))
		{
			IModule module = GetModule(tableModule);
			DocumentationModulesContainer scope = Scope;
			if ((scope == null || scope.IsModuleSelected(tableModule)) && module != null)
			{
				list.Add(module);
			}
		}
		return list;
	}

	public virtual IList<IModule> GetViewModules(int viewId)
	{
		List<IModule> list = new List<IModule>();
		foreach (int tableModule in DB.Table.GetTableModules(viewId))
		{
			IModule module = GetModule(tableModule);
			DocumentationModulesContainer scope = Scope;
			if ((scope == null || scope.IsModuleSelected(tableModule)) && module != null)
			{
				list.Add(module);
			}
		}
		return list;
	}

	public virtual IList<IModule> GetStructureModules(int structureId)
	{
		List<IModule> list = new List<IModule>();
		foreach (int tableModule in DB.Table.GetTableModules(structureId))
		{
			IModule module = GetModule(tableModule);
			DocumentationModulesContainer scope = Scope;
			if ((scope == null || scope.IsModuleSelected(tableModule)) && module != null)
			{
				list.Add(module);
			}
		}
		return list;
	}

	public virtual IList<IModule> GetProcedureModules(int procedureId)
	{
		List<IModule> list = new List<IModule>();
		foreach (int procedureModule in DB.Procedure.GetProcedureModules(procedureId))
		{
			IModule module = GetModule(procedureModule);
			DocumentationModulesContainer scope = Scope;
			if ((scope == null || scope.IsModuleSelected(procedureModule)) && module != null)
			{
				list.Add(module);
			}
		}
		return list;
	}

	public virtual IList<IModule> GetFunctionModules(int functionId)
	{
		List<IModule> list = new List<IModule>();
		foreach (int procedureModule in DB.Procedure.GetProcedureModules(functionId))
		{
			IModule module = GetModule(procedureModule);
			DocumentationModulesContainer scope = Scope;
			if ((scope == null || scope.IsModuleSelected(procedureModule)) && module != null)
			{
				list.Add(module);
			}
		}
		return list;
	}

	private bool IsTypeNotExcluded(int? typeIconId)
	{
		if (!typeIconId.HasValue)
		{
			return false;
		}
		if (typeIconId == 1 && Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Term))
		{
			return false;
		}
		if (typeIconId == 2 && Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Category))
		{
			return false;
		}
		if (typeIconId == 3 && Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Rule))
		{
			return false;
		}
		if (typeIconId == 4 && Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Policy))
		{
			return false;
		}
		if (typeIconId != 1 && typeIconId != 2 && typeIconId != 3 && typeIconId != 4 && Excluded.IsExluded(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Other))
		{
			return false;
		}
		return true;
	}
}
