using System;

namespace Dataedo.App.DataRepository.Models;

public interface ITermRelation : IModel, ICloneable
{
	ITerm RelatedTerm { get; }

	string Relationship { get; }

	string Description { get; }
}
