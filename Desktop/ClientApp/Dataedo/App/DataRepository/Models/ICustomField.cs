using System;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Models;

public interface ICustomField : IModel, ICloneable
{
	string Name { get; }

	string Value { get; }

	CustomFieldTypeEnum.CustomFieldType? Type { get; }
}
