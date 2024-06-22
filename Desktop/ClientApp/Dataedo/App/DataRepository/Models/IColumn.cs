using System;
using System.Collections.Generic;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Models;

public interface IColumn : IModel, ICloneable, ICustomFieldsContainer, IHasLinkedTerms
{
	string Name { get; }

	string NameWithoutPath { get; }

	string Title { get; }

	string DisplayName { get; }

	string DisplayNameWithoutPath { get; }

	string Description { get; }

	int? OrdinalPosition { get; }

	string DataType { get; }

	string DataLength { get; }

	string DisplayDataType { get; }

	bool IsPrimaryKey { get; }

	bool? IsNullable { get; }

	bool? IsIdentity { get; }

	string ComputedFormula { get; }

	string DefaultValue { get; }

	string Path { get; }

	int Level { get; }

	string ItemType { get; }

	UserTypeEnum.UserType Source { get; }

	IEnumerable<IKey> Keys { get; }

	IEnumerable<IRelation> References { get; }

	IEnumerable<IColumn> Children { get; }
}
