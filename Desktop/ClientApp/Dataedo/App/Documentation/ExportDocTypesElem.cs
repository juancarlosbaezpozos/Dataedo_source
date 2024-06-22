using Dataedo.App.Enums;
using DevExpress.XtraRichEdit;

namespace Dataedo.App.Documentation;

public class ExportDocTypesElem
{
	public string Name { get; set; }

	public string Extension { get; set; }

	public DevExpress.XtraRichEdit.DocumentFormat DocumentFormat { get; set; }

	public DocFormatEnum.DocFormat Format { get; set; }

	public ExportDocTypesElem(string name, string extension, DevExpress.XtraRichEdit.DocumentFormat documentFormat, DocFormatEnum.DocFormat format = DocFormatEnum.DocFormat.PDF)
	{
		Name = name;
		Extension = extension;
		DocumentFormat = documentFormat;
		Format = format;
	}
}
