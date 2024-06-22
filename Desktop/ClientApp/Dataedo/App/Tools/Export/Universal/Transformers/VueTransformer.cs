using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dataedo.App.Tools.Export.Universal.ProgressTrackers;
using Dataedo.App.Tools.Export.Universal.Storage;

namespace Dataedo.App.Tools.Export.Universal.Transformers;

internal class VueTransformer : ITemplateTransformer
{
	private readonly string LocalDataPartFileName = "data.local.json.js.part";

	private readonly string LocalDataFileName = "data.local.json.js";

	private readonly string WebDataPartFileName = "data.json.js.part";

	private readonly string WebDataFileName = "data.json.js";

	private readonly string ConfigFileName = "config.json.js";

	private JsonTransformer jsonTransformer;

	private bool threasholdReached;

	public VueTransformer(JsonTransformer jsonTransformer)
	{
		this.jsonTransformer = jsonTransformer;
		this.jsonTransformer.ThresholdReached += ThresholdReached;
	}

	private void ThresholdReached(object sender, EventArgs e)
	{
		threasholdReached = true;
	}

	public void Export(ITemplate template, IStorage destination, IProgressTracker progress = null, object options = null)
	{
		progress?.Started();
		threasholdReached = false;
		jsonTransformer.ForceReloadData = true;
		jsonTransformer.DistributedData = true;
		jsonTransformer.DataFileName = WebDataPartFileName;
		jsonTransformer.Export(template, destination, new PartProgressTracker(progress, 0, 75), options);
		jsonTransformer.ForceReloadData = false;
		jsonTransformer.DistributedData = false;
		jsonTransformer.DataFileName = LocalDataPartFileName;
		jsonTransformer.Export(template, destination, new PartProgressTracker(progress, 75, 95), options);
		progress.Log("Copy static template files...");
		CopyDirectory(template, destination, new SubProgressTracker(progress, 95, 98));
		progress.Log("Generating documentation fingerprint...");
		SetDataFingerprint(destination, new SubProgressTracker(progress, 98, 99));
		progress.Log("Searching for the optimal configuration...");
		AdjustConfigurationOptions(destination);
		progress.Log("Applying changes...");
		ReplaceData(destination, new SubProgressTracker(progress, 99, 100), options as WizardOptions);
		progress.Log("Finished!");
		progress?.Done();
	}

	private void CopyDirectory(ITemplate template, IStorage destination, IProgressTracker progress)
	{
		progress?.Started();
		IEnumerable<string> enumerable = template.ListFiles("dist");
		int num = 0;
		foreach (string item in enumerable)
		{
			progress?.Log(item);
			byte[] content = template.ReadFile("dist/" + item);
			destination.SaveFile(item, content);
			num++;
			progress?.OnProgress(num * 100 / enumerable.Count());
		}
		progress?.Done();
	}

	private void SetDataFingerprint(IStorage destination, IProgressTracker progress)
	{
		progress?.Started();
		string[] array = new string[2] { "index.html", "index.local.html" };
		foreach (string path in array)
		{
			byte[] bytes = destination.ReadFile(path);
			string @string = Encoding.UTF8.GetString(bytes);
			@string = @string.Replace("{{ data_hash }}", DateTime.Now.ToString("yyyyMMddHHmmss"));
			bytes = Encoding.UTF8.GetBytes(@string);
			destination.SaveFile(path, bytes);
		}
		progress?.Done();
	}

	private void AdjustConfigurationOptions(IStorage destination)
	{
		if (threasholdReached)
		{
			string text = destination.ReadTextFile(ConfigFileName);
			text = text.Replace("rememberMenuTreeState: true", "rememberMenuTreeState: false");
			destination.SaveFile(ConfigFileName, text);
		}
	}

	private void ReplaceData(IStorage destination, IProgressTracker progress, WizardOptions options)
	{
		progress?.Started();
		destination.DeleteFile(LocalDataFileName);
		destination.MoveFile(LocalDataPartFileName, LocalDataFileName);
		destination.DeleteFile(WebDataFileName);
		destination.MoveFile(WebDataPartFileName, WebDataFileName);
		progress?.Done();
	}
}
