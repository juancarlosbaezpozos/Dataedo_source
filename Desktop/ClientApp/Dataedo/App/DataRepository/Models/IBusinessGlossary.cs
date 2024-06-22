using System;
using System.Collections.Generic;
using Dataedo.App.DataRepository.Models.Aggregates;

namespace Dataedo.App.DataRepository.Models;

public interface IBusinessGlossary : IModel, ICloneable, ICustomFieldsContainer
{
	string Title { get; }

	string Description { get; }

	IList<ITerm> Terms { get; }
}
