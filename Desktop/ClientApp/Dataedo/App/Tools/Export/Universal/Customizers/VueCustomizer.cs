using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using Dataedo.App.Common;
using Dataedo.App.Enums;
using Dataedo.App.Tools.Export.Universal.Exceptions;
using Dataedo.App.Tools.Export.Universal.Storage;
using Dataedo.App.Tools.Export.Universal.Storage.Providers;
using Dataedo.App.Tools.Export.Universal.Templates;
using Dataedo.App.Tools.MessageBoxes;

namespace Dataedo.App.Tools.Export.Universal.Customizers;

internal class VueCustomizer : ITemplateCustomizer
{
	public static readonly string[] CustomizableFiles = new string[6] { "user.js", "user.css", "favicon.ico", "config.json.js", "images/logo.png", "How to customize HTML template.url" };

	public bool IsCustomizable(ITemplate template, Type destination)
	{
		return typeof(LocalStorage).IsAssignableFrom(destination);
	}

	private bool IsUserTemplateDirectory(ITemplate template)
	{
		if (!(template is LocalTemplate localTemplate))
		{
			return false;
		}
		return localTemplate?.Storage.Dir.StartsWith(Paths.GetUserTemplatesPath(DocFormatEnum.DocFormat.HTML)) ?? false;
	}

	public string Customize(ITemplate template, IStorage destination, object customData = null, Form owner = null)
	{
		if (IsUserTemplateDirectory(template))
		{
			return CustomizeUserTemplate(template, destination, customData, owner);
		}
		return CustomizeAppTemplate(template, destination, customData, owner);
	}

	private string CustomizeAppTemplate(ITemplate template, IStorage destination, object customData = null, Form owner = null)
	{
		if (!(destination is LocalStorage))
		{
			throw new CustomizerDestinationStorageException("The customized template can be stored only to the local storage (directory).");
		}
		string[] customizableFiles = CustomizableFiles;
		foreach (string text in customizableFiles)
		{
			destination.SaveFile(text, template.ReadFile("dist/" + text));
		}
		VueCustomData vueCustomData = customData as VueCustomData;
		LocalTemplateMetadata localTemplateMetadata = new LocalTemplateMetadata
		{
			Name = (vueCustomData.Name ?? ("Custom Template (" + DateTime.Now.ToString("yyyyMMddHHmmss") + ")")),
			Description = template.Description,
			Transformer = "vue-custom",
			Customizer = template.CustomizerName,
			Version = template.Version
		};
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(LocalTemplateMetadata));
		using (StringWriter stringWriter = new Utf8StringWriter())
		{
			xmlSerializer.Serialize(stringWriter, localTemplateMetadata, new XmlSerializerNamespaces());
			destination.SaveFile("meta.xml", stringWriter.ToString());
		}
		destination.SaveFile("thumbnail.png", template.ReadFile("thumbnail.user.png"));
		GeneralMessageBoxesHandling.Show("Dataedo will open the folder with your custom template." + Environment.NewLine + "<href=" + Links.HowToCustomizeHTML + ">How to customize HTML template</href>", "Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, owner);
		destination.Open();
		return localTemplateMetadata.Name;
	}

	private string CustomizeUserTemplate(ITemplate template, IStorage destination, object customData = null, Form owner = null)
	{
		GeneralMessageBoxesHandling.Show("Dataedo will open the folder with your custom template." + Environment.NewLine + "<href=" + Links.HowToCustomizeHTML + ">How to customize HTML template</href>", "Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, owner);
		template.Open();
		return template.Name;
	}
}
