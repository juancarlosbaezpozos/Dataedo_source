using Dataedo.App.Documentation;

namespace Dataedo.App.Classes.Documentation.Common;

public class DefinitionDoc
{
	private DocGeneratingOptions docGeneratingOptions;

	private int fontSize;

	public string PlainTextDefinition { get; set; }

	public string Language { get; set; }

	public string ColorizedHtmlDefinition
	{
		get
		{
			if (docGeneratingOptions == null)
			{
				return PlainTextDefinition;
			}
			return docGeneratingOptions.ColorizeSyntax(PlainTextDefinition, Language, fontSize);
		}
	}

	public DefinitionDoc(string plainTextDefinition, string language, DocGeneratingOptions docGeneratingOptions, int fontSize = 7)
	{
		PlainTextDefinition = plainTextDefinition;
		Language = language;
		this.docGeneratingOptions = docGeneratingOptions;
		this.fontSize = fontSize;
	}
}
