using System;
using Dataedo.App.DataRepository.Models.Aggregates;

namespace Dataedo.App.DataRepository.Models;

public interface ITrigger : IModel, ICloneable, ICustomFieldsContainer
{
	string Name { get; }

	string Description { get; }

	bool Before { get; }

	bool After { get; }

	bool InsteadOf { get; }

	bool OnInsert { get; }

	bool OnUpdate { get; }

	bool OnDelete { get; }

	string Script { get; }
}
