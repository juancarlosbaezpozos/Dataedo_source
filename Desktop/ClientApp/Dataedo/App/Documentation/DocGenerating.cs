using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dataedo.App.Classes.Documentation;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Documentation.Template.PdfTemplate;
using Dataedo.App.Documentation.Template.PdfTemplate.Model;
using Dataedo.App.Documentation.Tools;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Licences;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.App.Tools.UI;
using Dataedo.Shared.Enums;
using DevExpress.Compression;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraReports.UI;

namespace Dataedo.App.Documentation;

public class DocGenerating
{
	private int? currentDatabaseId;

	private DocumentationModulesContainer documentationsModulesData;

	public SaveFileDialog saveFileDialog;

	private BackgroundWorker docGenerateBackgroundWorker;

	private BackgroundWorkerManager backgroundWorkerManager;

	private DateTime exportStartDate;

	private Form owner;

	public ExportFile ExportFile { get; set; }

	public event EventHandler UpdateProgressEvent;

	public event EventHandler FinishedEvent;

	public DocGenerating(int? currentDatabaseId, DocumentationModulesContainer documentationsModulesData, int numberOfSteps, Form owner = null)
	{
		this.currentDatabaseId = currentDatabaseId;
		this.documentationsModulesData = documentationsModulesData;
		ExportFile = new ExportFile(this.currentDatabaseId);
		saveFileDialog = new SaveFileDialog();
		docGenerateBackgroundWorker = new BackgroundWorker();
		backgroundWorkerManager = new BackgroundWorkerManager(docGenerateBackgroundWorker);
		backgroundWorkerManager.SetMaxProgress(numberOfSteps);
		docGenerateBackgroundWorker.DoWork += docGenerateBackgroundWorker_DoWork;
		docGenerateBackgroundWorker.RunWorkerCompleted += docGenerateBackgroundWorker_RunWorkerCompleted;
		docGenerateBackgroundWorker.ProgressChanged += docGenerateBackgroundWorker_ProgressChanged;
		docGenerateBackgroundWorker.WorkerSupportsCancellation = true;
		docGenerateBackgroundWorker.WorkerReportsProgress = true;
		this.owner = owner;
	}

	private void docGenerateBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		this.UpdateProgressEvent?.Invoke(null, new BackgroundWorkerProgressEventArgs(e.ProgressPercentage, e.UserState as List<string>));
	}

	private void LoadData(XtraReport xtraReport, DocGeneratingOptions docGeneratingOptions, DoWorkEventArgs e, Form owner = null)
	{
		if (!docGenerateBackgroundWorker.CancellationPending)
		{
			backgroundWorkerManager.ReportProgress("Loading data");
			LoadData(xtraReport, docGeneratingOptions, owner);
		}
		else
		{
			e.Cancel = true;
		}
	}

	private static bool LoadData(XtraReport xtraReport, DocGeneratingOptions docGeneratingOptions, Form owner = null)
	{
		int num = 1;
		RootDoc rootDoc = new RootDoc(docGeneratingOptions)
		{
			Documentations = new List<DatabaseDoc>()
		};
		foreach (DocumentationModules item2 in docGeneratingOptions.DocumentationsModulesData.SelectedDocumentations.Where((DocumentationModules x) => !docGeneratingOptions.ExcludedObjects.Any((ObjectTypeHierarchy y) => y.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary && y.ObjectSubtype == SharedObjectTypeEnum.ObjectType.BusinessGlossary) || x.Documentation.ObjectTypeValue != SharedObjectTypeEnum.ObjectType.BusinessGlossary))
		{
			StaticData.CrashedDatabaseType = item2?.Documentation?.Type;
			StaticData.CrashedDBMSVersion = item2?.Documentation?.DbmsVersion;
			DatabaseDoc item = new DatabaseDoc(rootDoc, item2.Documentation.IdValue, docGeneratingOptions, num++, docGeneratingOptions.DocumentationsModulesData.SelectedDocumentationsCount, owner);
			rootDoc.Documentations.Add(item);
		}
		StaticData.ClearDatabaseInfoForCrashes();
		if (!rootDoc.Documentations.Any())
		{
			return false;
		}
		rootDoc.TitlePage = rootDoc.Documentations.FirstOrDefault()?.TitlePage;
		rootDoc.TitlePage.DatabaseTitle = docGeneratingOptions.DocumentationTitle;
		Helper.ApplyData(rootDoc, docGeneratingOptions?.Template as PdfTemplateModel);
		xtraReport.DataSource = new List<RootDoc>(new RootDoc[1] { rootDoc });
		return true;
	}

	public XtraReport LoadTemplate(DocGeneratingOptions docGeneratingOptions, DoWorkEventArgs e, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Loading template");
		try
		{
			if (!docGenerateBackgroundWorker.CancellationPending)
			{
				XtraReport xtraReport = LoadTemplatePrivate(docGeneratingOptions, (Stream stream) => LoadRepx(stream, docGeneratingOptions, e, owner));
				if (xtraReport == null)
				{
					GeneralMessageBoxesHandling.Show("Unable to load template", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, this.owner);
				}
				return xtraReport;
			}
			e.Cancel = true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while loading template file.", owner);
		}
		return null;
	}

	public static XtraReport LoadTemplateWithData(DocGeneratingOptions docGeneratingOptions, out bool dataLoaded, Form owner = null)
	{
		bool dataLoadedLocal = false;
		XtraReport result = LoadTemplatePrivate(docGeneratingOptions, delegate(Stream stream)
		{
			using MemoryStream ms = new MemoryStream();
			XtraReport xtraReport = LoadTemplateAndSetParameters(stream, ms, docGeneratingOptions);
			dataLoadedLocal = LoadData(xtraReport, docGeneratingOptions, owner);
			SetParametersAfterLoadingData(xtraReport);
			return xtraReport;
		});
		dataLoaded = dataLoadedLocal;
		return result;
	}

	private static XtraReport LoadTemplatePrivate(DocGeneratingOptions docGeneratingOptions, Func<Stream, XtraReport> processLoading)
	{
		XtraReport xtraReport = null;
		string coreTemplateFilePath = docGeneratingOptions.CoreTemplateFilePath;
		if (Path.HasExtension(coreTemplateFilePath))
		{
			try
			{
				using ZipArchive archive = ZipArchive.Read(coreTemplateFilePath);
				xtraReport = ProcessLoadingFromZipItem(archive, processLoading);
			}
			catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException)
			{
				using ZipArchive archive2 = ZipArchive.Read(coreTemplateFilePath, Encoding.GetEncoding(1250));
				xtraReport = ProcessLoadingFromZipItem(archive2, processLoading);
			}
			if (xtraReport == null)
			{
				using (ZipArchive archive3 = ZipArchive.Read(coreTemplateFilePath, Encoding.GetEncoding(1250)))
				{
					return ProcessLoadingFromZipItem(archive3, processLoading);
				}
			}
		}
		else
		{
			FileInfo[] files = new DirectoryInfo(coreTemplateFilePath).GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				if (fileInfo.Extension == ".repx")
				{
					using Stream arg = fileInfo.Open(FileMode.Open);
					xtraReport = processLoading(arg);
				}
			}
		}
		return xtraReport;
	}

	private static XtraReport ProcessLoadingFromZipItem(ZipArchive archive, Func<Stream, XtraReport> processLoading)
	{
		ZipItem zipItem = archive.Where((ZipItem x) => x.Name.EndsWith(".repx", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
		if (zipItem != null)
		{
			using (Stream arg = zipItem.Open())
			{
				return processLoading(arg);
			}
		}
		return null;
	}

	private XtraReport LoadRepx(Stream stream, DocGeneratingOptions docGeneratingOptions, DoWorkEventArgs e, Form owner = null)
	{
		using MemoryStream ms = new MemoryStream();
		XtraReport xtraReport = LoadTemplateAndSetParameters(stream, ms, docGeneratingOptions);
		LoadData(xtraReport, docGeneratingOptions, e, owner);
		SetParametersAfterLoadingData(xtraReport);
		return xtraReport;
	}

	private static XtraReport LoadTemplateAndSetParameters(Stream stream, MemoryStream ms, DocGeneratingOptions docGeneratingOptions)
	{
		int num = 0;
		do
		{
			byte[] buffer = new byte[1024];
			num = stream.Read(buffer, 0, 1024);
			ms.Write(buffer, 0, num);
		}
		while (stream.CanRead && num > 0);
		ms.Position = 0L;
		XtraReport xtraReport = XtraReport.FromStream(ms);
		PdfLinksSupport @object = new PdfLinksSupport();
		xtraReport.AfterPrint += @object.AfterPrint;
		if (docGeneratingOptions != null)
		{
			Helper.ApplyStyles(xtraReport, docGeneratingOptions?.Template as PdfTemplateModel, generalOnly: false);
		}
		if (Licence.HasWatermark)
		{
			xtraReport.DrawWatermark = true;
			xtraReport.Watermark.Font = new Font(xtraReport.Watermark.Font.FontFamily, 120f);
			xtraReport.Watermark.ForeColor = Color.FromArgb(128, 128, 128);
			xtraReport.Watermark.Text = "TRIAL";
			xtraReport.Watermark.TextDirection = DirectionMode.ForwardDiagonal;
			xtraReport.Watermark.TextTransparency = 150;
			xtraReport.Watermark.ShowBehind = false;
		}
		return xtraReport;
	}

	private static void SetParametersAfterLoadingData(XtraReport report)
	{
		RootDoc rootDoc = ((List<RootDoc>)report.DataSource)?.FirstOrDefault();
		if (rootDoc != null)
		{
			TitlePage titlePage = rootDoc.TitlePage;
			report.ExportOptions.Pdf.DocumentOptions.Title = titlePage.DatabaseTitle + " – Data Dictionary";
			report.ExportOptions.Pdf.DocumentOptions.Author = titlePage.UserName;
			report.ExportOptions.Pdf.DocumentOptions.Producer = "Dataedo";
			report.ExportOptions.Pdf.ConvertImagesToJpeg = false;
			report.ExportOptions.Pdf.ImageQuality = PdfJpegImageQuality.Highest;
		}
	}

	public ExportFile ChooseSavePathData(string fileName, SaveFileDialog specialSaveFileDialog = null)
	{
		if (specialSaveFileDialog == null)
		{
			saveFileDialog.AddExtension = true;
			saveFileDialog.Filter = ExportDocTypes.GetFilters();
			saveFileDialog.FileName = fileName;
			ExportFile = ((saveFileDialog.ShowDialog() == DialogResult.OK) ? new ExportFile(saveFileDialog) : null);
			return ExportFile;
		}
		specialSaveFileDialog.AddExtension = true;
		specialSaveFileDialog.FileName = fileName;
		ExportFile = ((specialSaveFileDialog.ShowDialog() == DialogResult.OK) ? new ExportFile(specialSaveFileDialog) : null);
		return ExportFile;
	}

	public bool SetExportData(ExportFile exportFile)
	{
		string text = Path.Combine(exportFile.Directory, exportFile.FileName);
		text += exportFile.Extension;
		bool flag = File.Exists(text);
		if (!exportFile.Correct && flag)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("The file " + text + " already exists. Overwrite it?", "File exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question, null, 1, owner);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
			{
				return false;
			}
		}
		if (!Paths.IsValidName(exportFile.FileName, text, showMessage: true, owner))
		{
			return false;
		}
		if (flag)
		{
			try
			{
				using (new FileStream(text, FileMode.Open))
				{
				}
			}
			catch (IOException ex)
			{
				GeneralMessageBoxesHandling.Show(string.Format("File is in use. Close it and try again.", text), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, owner);
				return false;
			}
		}
		try
		{
			File.WriteAllText(text, string.Empty);
			File.Delete(text);
		}
		catch (Exception exception)
		{
			GeneralFileExceptionHandling.Handle(exception, "An error occurred while exporting documentation.", owner);
			return false;
		}
		saveFileDialog.FileName = text;
		return true;
	}

	public void GenerateDocument(DocGeneratingOptions docGeneratingOptions)
	{
		docGenerateBackgroundWorker.RunWorkerAsync(docGeneratingOptions);
	}

	private void docGenerateBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
	{
		exportStartDate = DateTime.UtcNow;
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingRepoParameters(), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.ExportRun);
		});
		try
		{
			SkinsManager.SetResources(SkinsManager.DefaultSkin.ResourceManager);
			e.Result = false;
			XtraReport xtraReport = LoadTemplate(e.Argument as DocGeneratingOptions, e, owner);
			e.Result = ExportDocument(xtraReport, e);
		}
		finally
		{
			SkinsManager.SetResources();
		}
	}

	public void Cancel()
	{
		if (docGenerateBackgroundWorker.IsBusy)
		{
			backgroundWorkerManager.ReportProgress("Canceling");
			docGenerateBackgroundWorker.CancelAsync();
		}
	}

	private void docGenerateBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		this.FinishedEvent?.Invoke(null, null);
		if (e.Error != null)
		{
			GeneralFileExceptionHandling.Handle(e.Error, "An error occurred while exporting documentation.", owner);
		}
		double duration = (DateTime.UtcNow - exportStartDate).TotalSeconds;
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilderWithEventSpecificTime(new TrackingRepoParameters(), new TrackingDataedoParameters(), new TrackingUserParameters(), duration.ToString()), (e.Error == null && !e.Cancelled) ? TrackingEventEnum.ExportFinished : TrackingEventEnum.ExportFailed);
		});
		if (!e.Cancelled && e.Error == null && Convert.ToBoolean(e.Result))
		{
			Paths.OpenSavedFile(saveFileDialog.FileName, owner);
		}
	}

	public bool ExportDocument(XtraReport xtraReport, DoWorkEventArgs e)
	{
		if (xtraReport != null)
		{
			try
			{
				if (docGenerateBackgroundWorker.CancellationPending)
				{
					e.Cancel = true;
					return true;
				}
				backgroundWorkerManager.ReportProgress(new List<string> { "Creating document", "Creating document" });
				ProgressBarControl progressBar = (e.Argument as DocGeneratingOptions).ProgressBar;
				ProgressActions progressActions = new ProgressActions(xtraReport, progressBar);
				progressActions.CreateDoc();
				if (docGenerateBackgroundWorker.CancellationPending)
				{
					e.Cancel = true;
					return true;
				}
				backgroundWorkerManager.ReportProgress(new List<string> { "Exporting document", "Exporting document" });
				progressActions.Export(saveFileDialog.FileName);
				backgroundWorkerManager.IncrementProgress();
			}
			catch (Exception exception)
			{
				GeneralFileExceptionHandling.Handle(exception, "An error occurred while exporting documentation.", owner);
				return false;
			}
			finally
			{
				xtraReport?.Dispose();
			}
			return true;
		}
		return false;
	}
}
