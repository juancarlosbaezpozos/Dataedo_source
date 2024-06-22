using System.Collections.Generic;
using System.Linq;
using ColorCode;
using Dataedo.App.Documentation.Template;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Tools.CustomFields;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;

namespace Dataedo.App.Documentation;

public class DocGeneratingOptions
{
	private CodeColorizer codeColorizer;

	public string TemplateFilePath { get; set; }

	public string CoreTemplateFilePath { get; set; }

	public string DocumentationTitle { get; set; }

	public DocumentationModulesContainer DocumentationsModulesData { get; set; }

	public List<ObjectTypeHierarchy> ExcludedObjects { get; set; }

	public CustomFieldsSupport CustomFields { get; set; }

	public ProgressBarControl ProgressBar { get; set; }

	public bool LoadGrayIcons => TemplateFilePath.StartsWith("Economic");

	public bool LoadErdDiagrams { get; set; }

	public bool LoadDependencies { get; set; }

	public bool PutDataedoLogo { get; set; }

	public IBaseTemplateModel Template { get; set; }

	public bool AppendObjectsWithoutModule(int? documentationId)
	{
		return DocumentationsModulesData.GetSelectedModulesIds(documentationId).Any((int x) => x == -1);
	}

	public List<int> DBModulesId(int? documentationId)
	{
		return (from x in DocumentationsModulesData.GetSelectedModulesIds(documentationId)
			where x != -1
			select x).ToList();
	}

	public List<int> ModulesId(int? documentationId)
	{
		return DocumentationsModulesData.GetSelectedModulesIds(documentationId).ToList();
	}

	public DocGeneratingOptions(CustomFieldsSupport customFieldsSupport)
	{
		codeColorizer = new CodeColorizer();
		DocumentationsModulesData = new DocumentationModulesContainer();
		ExcludedObjects = new List<ObjectTypeHierarchy>();
		LoadErdDiagrams = (LoadDependencies = true);
		PutDataedoLogo = false;
		CustomFields = customFieldsSupport;
	}

	public void SetLoadingErd()
	{
		LoadErdDiagrams = !ExcludedObjects.Where((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Erd)).Any();
	}

	public string ColorizeSyntax(string sourceCode, string language, int fontSize = 8)
	{
		sourceCode = sourceCode?.Trim();
		if (string.IsNullOrEmpty(sourceCode))
		{
			return null;
		}
		string text = null;
		if (language != "SQL")
		{
			using RichEditDocumentServer richEditDocumentServer = new RichEditDocumentServer();
			richEditDocumentServer.Text = sourceCode;
			text = richEditDocumentServer.HtmlText;
		}
		else
		{
			text = codeColorizer.Colorize(sourceCode, Languages.Sql);
		}
		return $"<div style=\"font-size: {fontSize}pt;\">" + text + "</div>";
	}
}
