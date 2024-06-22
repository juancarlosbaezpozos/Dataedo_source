using System.Collections.Generic;
using System.IO;
using Dataedo.App.Tools.Export.Universal.Templates;

namespace Dataedo.App.Tools.Export.Universal.Explorers;

internal class LocalTemplatesExplorer : ITemplatesExplorer
{
	private string dir;

	public LocalTemplatesExplorer(string dir)
	{
		this.dir = dir;
	}

	public IEnumerable<ITemplate> List()
	{
		List<ITemplate> list = new List<ITemplate>();
		string[] directories = Directory.GetDirectories(dir);
		foreach (string path in directories)
		{
			if (File.Exists(Path.Combine(path, "meta.xml")))
			{
				list.Add(new LocalTemplate(path));
			}
		}
		return list;
	}
}
