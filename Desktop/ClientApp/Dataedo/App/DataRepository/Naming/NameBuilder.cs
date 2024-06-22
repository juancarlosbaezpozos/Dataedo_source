using Dataedo.App.DataRepository.Models;

namespace Dataedo.App.DataRepository.Naming;

internal class NameBuilder
{
	private IModel Model;

	private NameOptions Options;

	private NameBuilder(IModel model)
	{
		Model = model;
		Options = new NameOptions();
	}

	public static NameBuilder For(IModel model)
	{
		return new NameBuilder(model);
	}

	public NameBuilder Full()
	{
		Options.Documentation = true;
		Options.Schema = true;
		Options.Title = true;
		return this;
	}

	public NameBuilder WithDocumentation()
	{
		Options.Documentation = true;
		return this;
	}

	public NameBuilder WithSchema()
	{
		Options.Schema = true;
		return this;
	}

	public NameBuilder WithTitle()
	{
		Options.Title = true;
		return this;
	}

	public string Build(IDocumentation current = null, bool showSchema = false)
	{
		Options.CurrentDocumentation = current;
		Options.SchowSchema = showSchema;
		return NameManager.Get(Model, Options);
	}
}
