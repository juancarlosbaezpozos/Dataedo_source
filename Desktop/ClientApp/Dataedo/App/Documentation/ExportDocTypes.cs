using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dataedo.App.Enums;
using DevExpress.XtraRichEdit;

namespace Dataedo.App.Documentation;

public class ExportDocTypes
{
	public static List<ExportDocTypesElem> exportTypes = new List<ExportDocTypesElem>();

	public static List<ExportDocTypesElem> GetExportTypes()
	{
		if (exportTypes.Count == 0)
		{
			exportTypes.Add(new ExportDocTypesElem("PDF", ".pdf", DevExpress.XtraRichEdit.DocumentFormat.Undefined));
			exportTypes.Add(new ExportDocTypesElem("SQL", ".sql", DevExpress.XtraRichEdit.DocumentFormat.PlainText, DocFormatEnum.DocFormat.DDL));
		}
		return exportTypes;
	}

	public static DevExpress.XtraRichEdit.DocumentFormat GetDocumentFormat(string fileName)
	{
		string extension = Path.GetExtension(fileName);
		GetExportTypes();
		DevExpress.XtraRichEdit.DocumentFormat? documentFormat = exportTypes.SingleOrDefault((ExportDocTypesElem x) => x.Extension.Equals(extension))?.DocumentFormat;
		if (!documentFormat.HasValue)
		{
			documentFormat = DevExpress.XtraRichEdit.DocumentFormat.Undefined;
		}
		return documentFormat.Value;
	}

	public static string GetFilters()
	{
		StringBuilder stringBuilder = new StringBuilder();
		GetExportTypes();
		foreach (ExportDocTypesElem exportType in exportTypes)
		{
			stringBuilder.Append(exportType.Name).Append(" (*").Append(exportType.Extension)
				.Append(")|*")
				.Append(exportType.Extension)
				.Append("|");
		}
		string text = stringBuilder.ToString();
		if (exportTypes.Count <= 0)
		{
			return text;
		}
		return text.Substring(0, text.Length - 1);
	}
}
