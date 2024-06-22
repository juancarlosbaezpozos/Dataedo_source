using System;
using System.Collections.Generic;
using Dataedo.App.DataRepository.Models.Aggregates;

namespace Dataedo.App.DataRepository.Models;

public interface ITerm : IModel, ICloneable, ICustomFieldsContainer
{
	string Title { get; }

	string Description { get; }

	int? TypeIconId { get; }

	IBusinessGlossary BusinessGlossary { get; }

	IList<ITerm> Terms { get; }

	IList<ITermRelation> RelatedTerms { get; }

	IList<ITermDataLink> DataLinks { get; }
}
