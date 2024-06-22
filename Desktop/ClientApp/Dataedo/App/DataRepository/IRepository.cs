using System.Collections.Generic;
using Dataedo.App.DataRepository.Models;

namespace Dataedo.App.DataRepository;

internal interface IRepository
{
	IList<IDocumentation> GetDocumentations();

	IDocumentation GetDocumentation(int documentationId);

	IList<IModule> GetModules(int documentationId);

	IList<IModule> GetModules(IEnumerable<int> modulesIds);

	IModule GetModule(int moduleId);

	IList<ITable> GetTables(int documentationId);

	IList<ITable> GetModuleTables(int moduleId);

	ITable GetTable(int tableId);

	IList<IView> GetViews(int documentationId);

	IList<IView> GetModuleViews(int moduleId);

	IView GetView(int viewId);

	IList<IStructure> GetStructures(int documentationId);

	IList<IStructure> GetModuleStructures(int moduleId);

	IStructure GetStructure(int structureId);

	IList<IProcedure> GetProcedures(int documentationId);

	IList<IProcedure> GetModuleProcedures(int moduleId);

	IProcedure GetProcedure(int procedureId);

	IList<IFunction> GetFunctions(int documentationId);

	IList<IFunction> GetModuleFunctions(int moduleId);

	IFunction GetFunction(int functionId);

	IList<IColumn> GetTableColumns(int tableId);

	IList<IColumn> GetViewColumns(int viewId);

	IList<IColumn> GetStructureColumns(int structureId);

	IList<IRelation> GetTableRelations(int tableId);

	IList<IRelation> GetViewRelations(int viewId);

	IList<IRelation> GetStructureRelations(int structureId);

	IList<IKey> GetTableKeys(int tableId);

	IList<IKey> GetViewKeys(int viewId);

	IList<IKey> GetStructureKeys(int structureId);

	IList<ITrigger> GetTableTriggers(int tableId);

	IList<ITrigger> GetViewTriggers(int viewId);

	IDependencies GetTableDependencies(int tableId);

	IDependencies GetViewDependencies(int viewId);

	IDependencies GetStructureDependencies(int structureId);

	IDependencies GetProcedureDependencies(int procedureId);

	IDependencies GetFunctionDependencies(int functionId);

	IList<IParameter> GetProcedureParameters(int procedureId);

	IList<IParameter> GetFunctionParameters(int functionId);

	IList<IModule> GetTableModules(int tableId);

	IList<IModule> GetViewModules(int viewId);

	IList<IModule> GetStructureModules(int structureId);

	IList<IModule> GetProcedureModules(int procedureId);

	IList<IModule> GetFunctionModules(int functionId);

	IList<IBusinessGlossary> GetBusinessGlossaries();

	IBusinessGlossary GetBusinessGlossary(int businessGlossaryId);

	IList<ITerm> GetRootTerms(int businessGlossaryId);

	IList<ITerm> GetChildrenTerms(int termId);

	ITerm GetTerm(int termId);

	IList<ITermRelation> GetTermRelations(int termId);

	IList<ITermDataLink> GetTermDataLinks(int termId);

	IList<ITerm> GetTableLinkedTerms(int tableId);

	IList<ITerm> GetViewLinkedTerms(int viewId);

	IList<ITerm> GetStructureLinkedTerms(int structureId);

	IList<ITerm> GetColumnLinkedTerms(int tableOrViewId, int columnId);
}
