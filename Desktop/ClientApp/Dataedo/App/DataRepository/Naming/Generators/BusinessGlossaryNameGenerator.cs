using Dataedo.App.DataRepository.Models;

namespace Dataedo.App.DataRepository.Naming.Generators;

internal class BusinessGlossaryNameGenerator : INameGenerator
{
	public string Generate(IModel model, NameOptions options)
	{
		return (model as IBusinessGlossary).Title;
	}
}