using System;
using System.Collections.Generic;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Models;

public interface IDocumentation : IModel, ICloneable, ICustomFieldsContainer
{
	string Name { get; }

	string Title { get; }

	string Description { get; }

	string Host { get; }

	string Dbms { get; }

	SharedDatabaseTypeEnum.DatabaseType? Type { get; }

	bool HasMultipleSchemas { get; }

	bool? ShowSchema { get; }

	bool? ShowSchemaOverride { get; }

	bool ShowSchemaEffective { get; }

	IList<IModule> Modules { get; }

	IList<ITable> Tables { get; }

	IList<IView> Views { get; }

	IList<IProcedure> Procedures { get; }

	IList<IFunction> Functions { get; }

	IList<IStructure> Structures { get; }
}
