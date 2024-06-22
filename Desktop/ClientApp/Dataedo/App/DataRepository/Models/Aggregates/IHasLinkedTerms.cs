using System.Collections.Generic;

namespace Dataedo.App.DataRepository.Models.Aggregates;

public interface IHasLinkedTerms
{
	IList<ITerm> LinkedTerms { get; }
}
