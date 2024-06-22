using Dataedo.App.Tools.Export.Universal.Storage;

namespace Dataedo.App.Tools.Export.Universal;

public interface ITemplateTransformer
{
	void Export(ITemplate template, IStorage destination, IProgressTracker progress = null, object options = null);
}
