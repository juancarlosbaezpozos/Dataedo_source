using System;
using System.Collections.Generic;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Naming.Generators;

namespace Dataedo.App.DataRepository.Naming;

public static class NameManager
{
	private static IDictionary<Type, INameGenerator> generators = new Dictionary<Type, INameGenerator>
	{
		{
			typeof(IDocumentation),
			new DocumentationNameGenerator()
		},
		{
			typeof(ITable),
			new TableNameGenerator()
		},
		{
			typeof(IView),
			new ViewNameGenerator()
		},
		{
			typeof(IProcedure),
			new ProcedureNameGenerator()
		},
		{
			typeof(IFunction),
			new FunctionNameGenerator()
		},
		{
			typeof(IStructure),
			new StructureNameGenerator()
		},
		{
			typeof(IBusinessGlossary),
			new BusinessGlossaryNameGenerator()
		},
		{
			typeof(ITerm),
			new TermNameGenerator()
		}
	};

	public static string Get(IModel model, NameOptions options)
	{
		Type type = model.GetType();
		foreach (Type key in generators.Keys)
		{
			if (key.IsAssignableFrom(type))
			{
				return generators[key].Generate(model, options);
			}
		}
		throw new Exception($"Cannot get naming generator for model of type '{model.GetType()}'.");
	}
}
