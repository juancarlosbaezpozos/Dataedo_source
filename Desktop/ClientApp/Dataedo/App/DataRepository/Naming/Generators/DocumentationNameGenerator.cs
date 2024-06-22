using Dataedo.App.DataRepository.Models;

namespace Dataedo.App.DataRepository.Naming.Generators;

public class DocumentationNameGenerator : INameGenerator
{
	public string Generate(IModel model, NameOptions options)
	{
		IDocumentation documentation = model as IDocumentation;
		if (options.Title && !string.IsNullOrEmpty(documentation.Title))
		{
			return documentation.Title;
		}
		return documentation.Name;
	}
}
