using System.Collections.Generic;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Repositories.DbModels;
using Dataedo.App.Documentation;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Export;

namespace Dataedo.App.DataRepository.Repositories;

internal class DbCachedRepository : DbRepository
{
	private IDictionary<int, IDocumentation> Documentations;

	private IDictionary<int, IModule> Modules;

	private IDictionary<int, ITable> Tables;

	private IDictionary<int, IList<Relation>> Relations;

	private IDictionary<int, IList<IKey>> Keys;

	private IDictionary<int, IBusinessGlossary> BusinessGlossaries;

	private IDictionary<int, ITerm> Terms;

	private IDictionary<int, IList<IModule>> TableModules;

	private IDictionary<int, IList<IModule>> ViewModules;

	private IDictionary<int, IList<IModule>> ProcedureModules;

	private IDictionary<int, IList<IModule>> FunctionModules;

	public DbCachedRepository(DocumentationModulesContainer scope = null, ExcludedObjects excluded = null, CustomFieldsSupport customFields = null, OtherFieldsSupport otherFields = null)
		: base(scope, excluded, customFields, otherFields)
	{
		Documentations = new Dictionary<int, IDocumentation>();
		Modules = new Dictionary<int, IModule>();
		Tables = new Dictionary<int, ITable>();
		Relations = new Dictionary<int, IList<Relation>>();
		Keys = new Dictionary<int, IList<IKey>>();
		BusinessGlossaries = new Dictionary<int, IBusinessGlossary>();
		Terms = new Dictionary<int, ITerm>();
		TableModules = new Dictionary<int, IList<IModule>>();
		ViewModules = new Dictionary<int, IList<IModule>>();
		ProcedureModules = new Dictionary<int, IList<IModule>>();
		FunctionModules = new Dictionary<int, IList<IModule>>();
	}

	public override IDocumentation GetDocumentation(int documentationId)
	{
		lock (Documentations)
		{
			if (Documentations.ContainsKey(documentationId))
			{
				return Documentations[documentationId];
			}
			IDocumentation documentation = base.GetDocumentation(documentationId);
			Documentations.Add(documentationId, documentation);
			return documentation;
		}
	}

	public override IModule GetModule(int moduleId)
	{
		lock (Modules)
		{
			if (Modules.ContainsKey(moduleId))
			{
				return Modules[moduleId];
			}
			IModule module = base.GetModule(moduleId);
			Modules.Add(moduleId, module);
			return module;
		}
	}

	public override ITable GetTable(int tableId)
	{
		lock (Tables)
		{
			if (Tables.ContainsKey(tableId))
			{
				return Tables[tableId];
			}
			ITable table = base.GetTable(tableId);
			Tables.Add(tableId, table);
			return table;
		}
	}

	protected override IList<Relation> GetRelations(int objectId)
	{
		lock (Relations)
		{
			if (Relations.ContainsKey(objectId))
			{
				return Relations[objectId];
			}
			IList<Relation> relations = base.GetRelations(objectId);
			Relations.Add(objectId, relations);
			return relations;
		}
	}

	protected override IList<IKey> GetKeys(int objectId)
	{
		lock (Keys)
		{
			if (Keys.ContainsKey(objectId))
			{
				return Keys[objectId];
			}
			IList<IKey> keys = base.GetKeys(objectId);
			Keys.Add(objectId, keys);
			return keys;
		}
	}

	public override IList<IModule> GetTableModules(int tableId)
	{
		lock (TableModules)
		{
			if (TableModules.ContainsKey(tableId))
			{
				return TableModules[tableId];
			}
			IList<IModule> tableModules = base.GetTableModules(tableId);
			TableModules.Add(tableId, tableModules);
			return tableModules;
		}
	}

	public override IList<IModule> GetViewModules(int viewId)
	{
		lock (ViewModules)
		{
			if (ViewModules.ContainsKey(viewId))
			{
				return ViewModules[viewId];
			}
			IList<IModule> viewModules = base.GetViewModules(viewId);
			ViewModules.Add(viewId, viewModules);
			return viewModules;
		}
	}

	public override IList<IModule> GetProcedureModules(int procedureId)
	{
		lock (ProcedureModules)
		{
			if (ProcedureModules.ContainsKey(procedureId))
			{
				return ProcedureModules[procedureId];
			}
			IList<IModule> procedureModules = base.GetProcedureModules(procedureId);
			ProcedureModules.Add(procedureId, procedureModules);
			return procedureModules;
		}
	}

	public override IList<IModule> GetFunctionModules(int functionId)
	{
		lock (FunctionModules)
		{
			if (FunctionModules.ContainsKey(functionId))
			{
				return FunctionModules[functionId];
			}
			IList<IModule> functionModules = base.GetFunctionModules(functionId);
			FunctionModules.Add(functionId, functionModules);
			return functionModules;
		}
	}

	public override IBusinessGlossary GetBusinessGlossary(int businessGlossaryId)
	{
		lock (BusinessGlossaries)
		{
			if (BusinessGlossaries.ContainsKey(businessGlossaryId))
			{
				return BusinessGlossaries[businessGlossaryId];
			}
			IBusinessGlossary businessGlossary = base.GetBusinessGlossary(businessGlossaryId);
			BusinessGlossaries.Add(businessGlossaryId, businessGlossary);
			return businessGlossary;
		}
	}

	public override ITerm GetTerm(int termId)
	{
		lock (Terms)
		{
			if (Terms.ContainsKey(termId))
			{
				return Terms[termId];
			}
			ITerm term = base.GetTerm(termId);
			Terms.Add(termId, term);
			return term;
		}
	}
}
