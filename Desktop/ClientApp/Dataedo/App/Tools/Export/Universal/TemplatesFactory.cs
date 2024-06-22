using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Documentation;
using Dataedo.App.Enums;
using Dataedo.App.Tools.Export.Universal.Customizers;
using Dataedo.App.Tools.Export.Universal.Explorers;
using Dataedo.App.Tools.Export.Universal.Helpers;
using Dataedo.App.Tools.Export.Universal.Transformers;

namespace Dataedo.App.Tools.Export.Universal;

public class TemplatesFactory
{
	public static TemplatesManager MakeHtmlTemplatesManager(Form owner = null)
	{
		TemplatesManager templatesManager = new TemplatesManager(new HTMLTemplatesByIsValidThenIsUserTemplateThenByName());
		LocalTemplatesExplorer localTemplatesExplorer = new LocalTemplatesExplorer(DocTemplateFile.GetTemplatesPath(DocFormatEnum.DocFormat.HTML, owner));
		templatesManager.RegisterTemplateExplorer(localTemplatesExplorer);
		LocalTemplatesExplorer explorer = new LocalTemplatesExplorer(Paths.GetUserTemplatesPath(DocFormatEnum.DocFormat.HTML));
		templatesManager.RegisterTemplateExplorer(explorer);
		VueTransformer vueTransformer = new VueTransformer(new JsonTransformer());
		templatesManager.RegisterTemplateTransformer("vue", vueTransformer);
		ITemplate vueBaseTemplate = localTemplatesExplorer.List().FirstOrDefault((ITemplate x) => x.TransformerName == "vue");
		templatesManager.RegisterTemplateTransformer("vue-custom", new VueCustomTransformer(vueTransformer, vueBaseTemplate));
		templatesManager.RegisterTemplateCustomizer("vue", new VueCustomizer());
		return templatesManager;
	}
}
