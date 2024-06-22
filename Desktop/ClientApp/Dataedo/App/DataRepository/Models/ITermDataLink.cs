using System;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Models;

public interface ITermDataLink : IModel, ICloneable
{
	int ObjectId { get; }

	ITerm Term { get; }

	SharedObjectTypeEnum.ObjectType? ObjectType { get; }

	SharedObjectSubtypeEnum.ObjectSubtype? ObjectSubtype { get; }

	string ObjectName { get; }

	string ObjectNameWithSchemaAndTitle { get; }

	UserTypeEnum.UserType ObjectSource { get; }

	IDocumentation ObjectDocumentation { get; }

	int? InnerObjectId { get; }

	SharedObjectTypeEnum.ObjectType? InnerObjectType { get; }

	SharedObjectSubtypeEnum.ObjectSubtype? InnerObjectSubtype { get; }

	string InnerPath { get; }

	string InnerName { get; }

	string InnerObjectName { get; }

	UserTypeEnum.UserType? InnerObjectSource { get; }
}
