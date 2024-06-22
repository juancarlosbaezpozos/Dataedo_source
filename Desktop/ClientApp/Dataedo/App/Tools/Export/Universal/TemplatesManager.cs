using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Tools.Export.Universal.Exceptions;
using Dataedo.App.Tools.Export.Universal.Storage;

namespace Dataedo.App.Tools.Export.Universal;

public class TemplatesManager
{
	private IDictionary<string, ITemplateTransformer> transformers;

	private IDictionary<string, ITemplateCustomizer> customizers;

	private ICollection<ITemplatesExplorer> explorers;

	private IComparer<ITemplate> templatesComparer;

	public TemplatesManager(IComparer<ITemplate> templatesComparer)
	{
		transformers = new Dictionary<string, ITemplateTransformer>();
		customizers = new Dictionary<string, ITemplateCustomizer>();
		explorers = new List<ITemplatesExplorer>();
		this.templatesComparer = templatesComparer;
	}

	public void RegisterTemplateTransformer(string name, ITemplateTransformer transformer)
	{
		transformers.Add(name, transformer);
	}

	public void RegisterTemplateCustomizer(string name, ITemplateCustomizer customizer)
	{
		customizers.Add(name, customizer);
	}

	public void RegisterTemplateExplorer(ITemplatesExplorer explorer)
	{
		explorers.Add(explorer);
	}

	public IEnumerable<ITemplate> ListTemplates()
	{
		List<ITemplate> list = new List<ITemplate>();
		foreach (ITemplatesExplorer explorer in explorers)
		{
			foreach (ITemplate item in explorer.List())
			{
				list.Add(item);
			}
		}
		return list.ToList();
	}

	public void Export(ITemplate template, IStorage destination, IProgressTracker progress = null, object options = null)
	{
		if (template.TransformerName == null)
		{
			throw new UnrecognizedTemplateTransformerException("The template '" + template.Name + "' is no longer supported.");
		}
		if (!transformers.ContainsKey(template.TransformerName))
		{
			throw new UnrecognizedTemplateTransformerException("The template '" + template.Name + "' requires the transformer '" + template.TransformerName + "' which is not supported.");
		}
		transformers[template.TransformerName].Export(template, destination, progress, options);
	}

	public bool IsCustomizable(ITemplate template, Type destination)
	{
		if (template.CustomizerName == null)
		{
			return false;
		}
		if (!transformers.ContainsKey(template.CustomizerName))
		{
			throw new UnrecognizedTemplateTransformerException("The template '" + template.Name + "' requires the customizer '" + template.CustomizerName + "' which is not supported.");
		}
		return customizers[template.CustomizerName].IsCustomizable(template, destination);
	}

	public bool IsCustomizable(ITemplate template, IStorage destination)
	{
		return IsCustomizable(template, destination.GetType());
	}

	public string Customize(ITemplate template, IStorage destination, object customData = null, Form owner = null)
	{
		if (template.CustomizerName == null)
		{
			throw new UnrecognizedTemplateTransformerException("The template '" + template.Name + "' is no longer supported.");
		}
		if (!transformers.ContainsKey(template.CustomizerName))
		{
			throw new UnrecognizedTemplateTransformerException("The template '" + template.Name + "' requires the transformer '" + template.CustomizerName + "' which is not supported.");
		}
		return customizers[template.CustomizerName].Customize(template, destination, customData, owner);
	}
}
