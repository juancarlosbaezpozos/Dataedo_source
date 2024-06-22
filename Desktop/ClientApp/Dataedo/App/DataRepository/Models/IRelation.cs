using System;
using System.Collections.Generic;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.App.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Models;

public interface IRelation : IModel, ICloneable, ICustomFieldsContainer
{
	string Name { get; }

	string Title { get; }

	string Description { get; }

	UserTypeEnum.UserType Source { get; }

	IDocumentation Documentation { get; }

	IModel ForeignModel { get; }

	IModel PrimaryModel { get; }

	SharedObjectTypeEnum.ObjectType? ForeignModelType { get; }

	SharedObjectTypeEnum.ObjectType? PrimaryModelType { get; }

	CardinalityTypeEnum.CardinalityType? ForeignCardinality { get; }

	CardinalityTypeEnum.CardinalityType? PrimaryCardinality { get; }

	IEnumerable<IRelationConstraint> Constraints { get; }
}
