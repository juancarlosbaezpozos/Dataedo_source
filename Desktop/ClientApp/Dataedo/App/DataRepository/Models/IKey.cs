using System;
using System.Collections.Generic;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Models;

public interface IKey : IModel, ICloneable, ICustomFieldsContainer
{
	string Name { get; }

	string Description { get; }

	bool IsPk { get; }

	UserTypeEnum.UserType Source { get; }

	IEnumerable<PathName> Columns { get; }
}
