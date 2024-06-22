using System;
using System.Collections.Generic;
using Dataedo.App.DataRepository.Models.Aggregates;

namespace Dataedo.App.DataRepository.Models;

public interface IModule : IModel, ICloneable, ICustomFieldsContainer
{
	string Title { get; }

	string Description { get; }

	string Erd { get; }

	IDocumentation Documentation { get; }

	IList<ITable> Tables { get; }

	IList<IView> Views { get; }

	IList<IProcedure> Procedures { get; }

	IList<IFunction> Functions { get; }

	IList<IStructure> Structures { get; }
}
