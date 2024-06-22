using System.Linq;
using Dataedo.App.Tools.Export.Universal.Customizers;
using Dataedo.App.Tools.Export.Universal.ProgressTrackers;
using Dataedo.App.Tools.Export.Universal.Storage;

namespace Dataedo.App.Tools.Export.Universal.Transformers;

internal class VueCustomTransformer : ITemplateTransformer
{
	private VueTransformer vueTransformer;

	private ITemplate vueBaseTemplate;

	public VueCustomTransformer(VueTransformer vueTransformer, ITemplate vueBaseTemplate)
	{
		this.vueTransformer = vueTransformer;
		this.vueBaseTemplate = vueBaseTemplate;
	}

	public void Export(ITemplate template, IStorage destination, IProgressTracker progress = null, object options = null)
	{
		vueTransformer.Export(vueBaseTemplate, destination, new PartProgressTracker(progress, 0, 99), options);
		progress.Log("Copying customizable files...");
		CopyCustomizableFiles(template, destination, new SubProgressTracker(progress, 99, 100));
		progress.Log("Finished!");
		progress?.Done();
	}

	private void CopyCustomizableFiles(ITemplate template, IStorage destination, IProgressTracker progress = null)
	{
		progress?.Started();
		int num = 0;
		string[] customizableFiles = VueCustomizer.CustomizableFiles;
		string[] array = customizableFiles;
		foreach (string path in array)
		{
			if (template.HasFile(path))
			{
				destination.SaveFile(path, template.ReadFile(path));
			}
			num++;
			progress?.OnProgress(num * 100 / customizableFiles.Count());
		}
		progress?.Done();
	}
}
