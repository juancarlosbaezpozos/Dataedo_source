using System;
using System.Collections.Generic;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Models;

public interface IStructure : IModel, ICloneable, ICustomFieldsContainer, IHasLinkedTerms
{
	string Schema { get; }

	string Name { get; }

	string Title { get; }

	string Location { get; }

	string Description { get; }

	string Script { get; }

	UserTypeEnum.UserType Source { get; }

	SharedObjectSubtypeEnum.ObjectSubtype Subtype { get; }

	DateTime? ImportedAt { get; }

	IDocumentation Documentation { get; }

	IList<IColumn> Columns { get; }

	IList<IRelation> Relations { get; }

	IList<IKey> Keys { get; }

	IDependencies Dependencies { get; }

	IModule Module { get; }

	IList<IModule> Modules { get; }
}
