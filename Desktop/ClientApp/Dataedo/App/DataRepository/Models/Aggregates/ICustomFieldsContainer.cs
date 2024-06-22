using System.Collections.Generic;

namespace Dataedo.App.DataRepository.Models.Aggregates;

public interface ICustomFieldsContainer
{
	IList<ICustomField> CustomFields { get; }
}
