using System;
using Dataedo.App.DataRepository.Models.Aggregates;

namespace Dataedo.App.DataRepository.Models;

public interface IParameter : IModel, ICloneable, ICustomFieldsContainer
{
	string Name { get; }

	string Description { get; }

	string Mode { get; }

	string DataType { get; }
}
