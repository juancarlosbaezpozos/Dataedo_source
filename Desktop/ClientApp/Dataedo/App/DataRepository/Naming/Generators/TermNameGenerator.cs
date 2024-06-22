using Dataedo.App.DataRepository.Models;

namespace Dataedo.App.DataRepository.Naming.Generators;

internal class TermNameGenerator : INameGenerator
{
	public string Generate(IModel model, NameOptions options)
	{
		return (model as ITerm).Title;
	}
}
