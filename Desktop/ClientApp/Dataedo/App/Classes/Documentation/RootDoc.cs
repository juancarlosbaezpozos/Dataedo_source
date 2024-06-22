using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Template.PdfTemplate.Model;
using Dataedo.App.Documentation.Template.PdfTemplate.Model.ChildModels;
using Dataedo.App.Licences;

namespace Dataedo.App.Classes.Documentation;

public class RootDoc
{
	public List<DatabaseDoc> Documentations { get; set; }

	public bool HaveDocumentationsNoModules { get; set; } = true;


	public DatabaseDoc.LayoutModel Header { get; set; }

	public DatabaseDoc.LayoutModel Footer { get; set; } = new DatabaseDoc.LayoutModel();


	public LocalizationModel Localization { get; set; } = new LocalizationModel();


	public bool PutDatabaseLogo { get; set; }

	public bool PutDataedoLogo { get; set; }

	public bool IsEducationEdition { get; set; }

	public List<LegendDocRow> Legend { get; set; }

	public TitlePage TitlePage { get; set; }

	public bool HasMultipleDocumentations
	{
		get
		{
			List<DatabaseDoc> documentations = Documentations;
			if (documentations == null)
			{
				return false;
			}
			return documentations.Count > 1;
		}
	}

	public RootDoc(DocGeneratingOptions docGeneratingOptions = null)
	{
		HaveDocumentationsNoModules = docGeneratingOptions?.DocumentationsModulesData?.GetSelectedModulesIds(null)?.All((int x) => x == -1) ?? true;
		if (docGeneratingOptions != null && docGeneratingOptions.Template is PdfTemplateModel)
		{
			PdfTemplateModel pdfTemplateModel = docGeneratingOptions.Template as PdfTemplateModel;
			Legend = LegendDoc.Load(pdfTemplateModel?.Customization?.Localization?.Legend);
			Localization = pdfTemplateModel?.Customization?.Localization ?? new LocalizationModel();
		}
		else
		{
			Legend = LegendDoc.Load(new LegendNamesModel());
		}
		if (Licence.IsEducation)
		{
			IsEducationEdition = true;
		}
		if (IsEducationEdition)
		{
			PutDataedoLogo = true;
		}
	}
}
