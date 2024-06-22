using System;
using System.IO;
using Dataedo.App.Documentation;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;

namespace Dataedo.App.Tools;

public class ProgressActions
{
	private XtraReport xtraReport;

	private ProgressBarControl progressBar;

	public ProgressActions(XtraReport xtraReport, ProgressBarControl progressBar)
	{
		this.xtraReport = xtraReport;
		this.progressBar = progressBar;
	}

	public void CreateDoc()
	{
		xtraReport.CreateDocument();
	}

	public Action CreateDocAction()
	{
		return delegate
		{
			xtraReport.PrintingSystem.ProgressReflector = new ReflectorBar(progressBar);
			CreateDoc();
			xtraReport.PrintingSystem.ResetProgressReflector();
		};
	}

	public void Export(string fileName)
	{
		if (ExportDocTypes.GetDocumentFormat(fileName) == DevExpress.XtraRichEdit.DocumentFormat.Undefined)
		{
			xtraReport.ExportToPdf(fileName);
		}
	}

	public Action ExportAction(string fileName)
	{
		ExportDocTypes.GetDocumentFormat(fileName);
		return delegate
		{
			Export(fileName);
			xtraReport.PrintingSystem.ResetProgressReflector();
		};
	}

	private void ExportToDOC(string extension, DevExpress.XtraRichEdit.DocumentFormat df, string fileName)
	{
		using (RichEditDocumentServer richEditDocumentServer = new RichEditDocumentServer())
		{
			xtraReport.ExportToHtml("test.html", new HtmlExportOptions
			{
				ExportMode = HtmlExportMode.SingleFile,
				EmbedImagesInHTML = true
			});
			richEditDocumentServer.LoadDocument("test.html", DevExpress.XtraRichEdit.DocumentFormat.Html);
			richEditDocumentServer.SaveDocument(fileName, df);
		}
		File.Delete("test.html");
	}
}
