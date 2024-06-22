using System;
using System.Collections.Generic;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Models;

public interface IProcedure : IModel, ICloneable, ICustomFieldsContainer
{
	string Schema { get; }

	string Name { get; }

	string Title { get; }

	string Description { get; }

	UserTypeEnum.UserType Source { get; }

	SharedObjectSubtypeEnum.ObjectSubtype Subtype { get; }

	string Script { get; }

	DateTime? ImportedAt { get; }

	IDocumentation Documentation { get; }

	IList<IParameter> Parameters { get; }

	IDependencies Dependencies { get; }

	IModule Module { get; }

	IList<IModule> Modules { get; }
}
