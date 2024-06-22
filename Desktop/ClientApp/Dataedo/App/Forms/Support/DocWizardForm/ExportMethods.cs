using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Documentation;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.ExcelExportTools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Export;
using DevExpress.XtraSpreadsheet;

namespace Dataedo.App.Forms.Support.DocWizardForm;

internal class ExportMethods
{
	internal static void ExportToExcel(SpreadsheetControl spreadsheetControl, BackgroundProcessingWorker worker, string filename, DocumentationModulesContainer documentationsModulesData, List<ObjectTypeHierarchy> excludedTypes, CustomFieldsSupport customFieldsSupport, string templateName)
	{
		try
		{
			int value = (from x in documentationsModulesData.GetSelectedDocumentations()
				select x.Documentation.Id).FirstOrDefault() ?? (-1);
			documentationsModulesData.GetSelectedModulesIds(value).ToList();
			if (templateName == "Type per sheet")
			{
				new ExcelExportTypePerSheet().Export(spreadsheetControl, worker, documentationsModulesData, excludedTypes, customFieldsSupport, filename);
			}
			else
			{
				new ExcelExport().Export(spreadsheetControl, worker, documentationsModulesData, excludedTypes, customFieldsSupport, filename);
			}
		}
		catch (Exception exception)
		{
			GeneralFileExceptionHandling.Handle(exception, "An error occurred while exporting documentation.");
		}
	}
}
