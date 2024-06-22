namespace Dataedo.App.DataRepository.Naming;

public interface INameGenerator
{
	string Generate(IModel model, NameOptions options);
}
