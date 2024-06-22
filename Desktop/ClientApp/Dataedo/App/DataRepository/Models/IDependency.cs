using System;
using System.Collections.Generic;
using Dataedo.App.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Models;

public interface IDependency : IModel, ICloneable
{
	string ReferenceType { get; }

	int? ReferenceId { get; }

	string ObjectName { get; }

	string ObjectFullDisplayName { get; }

	string ObjectFullDisplayNameShowSchema { get; }

	SharedObjectTypeEnum.ObjectType ObjectType { get; }

	SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype { get; }

	UserTypeEnum.UserType ObjectSource { get; }

	string Type { get; }

	UserTypeEnum.UserType Source { get; }

	CardinalityTypeEnum.CardinalityType? RelationFkCardinality { get; }

	CardinalityTypeEnum.CardinalityType? RelationPkCardinality { get; }

	IEnumerable<IDependency> Children { get; }
}
