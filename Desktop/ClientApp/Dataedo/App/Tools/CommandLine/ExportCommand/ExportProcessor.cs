using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Template;
using Dataedo.App.Enums;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Licences;
using Dataedo.App.Tools.CommandLine.Tools;
using Dataedo.App.Tools.CommandLine.Xml;
using Dataedo.App.Tools.CommandLine.Xml.Commands.Base;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.CustomFields;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.ExcelExportTools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.Export.Universal;
using Dataedo.App.Tools.Export.Universal.Storage.Providers;
using Dataedo.App.Tools.Export.Universal.Templates;
using Dataedo.App.Tools.GeneralHandling;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;
using DevExpress.XtraSpreadsheet;

namespace Dataedo.App.Tools.CommandLine.ExportCommand;

internal abstract class ExportProcessor : ProcessorBase
{
	protected bool ProcessExcelExport(ExportBase command, Form owner = null)
	{
		string outputPath = null;
		if (!TryGetOutputFilePath(isFile: true, command.OutputPathParsed, command.Overwrite, ref outputPath, owner))
		{
			return false;
		}
		string text = null;
		IBaseTemplateModel baseTemplateModel = null;
		if (!CheckTemplatePath(command.TemplatePath))
		{
			return false;
		}
		try
		{
			text = GetTemplatePath(command.TemplatePath, DocFormatEnum.DocFormat.Excel, owner);
			baseTemplateModel = GetTemplate(command.TemplatePath, DocFormatEnum.DocFormat.Excel, owner);
		}
		catch (Exception exception)
		{
			if (!string.IsNullOrEmpty(command.TemplatePath))
			{
				GeneralExceptionHandling.Handle(exception, "Unable to load Excel template (\"" + command.TemplatePath + "\").", owner);
			}
			else
			{
				GeneralExceptionHandling.Handle(exception, "Unable to load Excel template.");
				base.Log.Write("Unable to load Excel template.");
			}
			return false;
		}
		if (baseTemplateModel == null)
		{
			if (!string.IsNullOrEmpty(text))
			{
				base.Log.Write("Unable to load Excel template (\"" + text + "\").");
			}
			else if (!string.IsNullOrEmpty(command.TemplatePath))
			{
				base.Log.Write("Unable to load Excel template (\"" + command.TemplatePath + "\").");
			}
			else
			{
				base.Log.Write("Unable to load Excel template.");
			}
			return false;
		}
		using (SpreadsheetControl spreadsheetControl = new SpreadsheetControl())
		{
			if (command.DataAsDocumentationModelBase.FirstOrDefault() != null)
			{
				CustomFieldsSupport customFieldsSupport = new CustomFieldsSupport();
				customFieldsSupport.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: false);
				SetCustomFields(customFieldsSupport, command);
				if (baseTemplateModel.Name == "Type per sheet")
				{
					new ExcelExportTypePerSheet().Export(spreadsheetControl, new BackgroundProcessingWorker(), GetDocumentationModulesContainer(command.DataAsDocumentationModelBase), GetExcludedObjects(command), customFieldsSupport, outputPath);
				}
				else
				{
					new ExcelExport().Export(spreadsheetControl, new BackgroundProcessingWorker(), GetDocumentationModulesContainer(command.DataAsDocumentationModelBase), GetExcludedObjects(command), customFieldsSupport, outputPath);
				}
			}
		}
		return true;
	}

	protected bool ProcessHtmlExport(ExportBase command, Form owner = null)
	{
		string outputPath = null;
		if (!TryGetOutputFilePath(isFile: false, command.OutputPathParsed, command.Overwrite, ref outputPath, owner))
		{
			return false;
		}
		string text = null;
		ITemplate template = null;
		if (!CheckTemplatePath(command.TemplatePath))
		{
			return false;
		}
		try
		{
			text = GetTemplatePath(command.TemplatePath, DocFormatEnum.DocFormat.HTML, owner);
			template = new LocalTemplate(text);
		}
		catch (Exception exception)
		{
			if (!string.IsNullOrEmpty(command.TemplatePath))
			{
				GeneralExceptionHandling.Handle(exception, "Unable to load HTML template (\"" + command.TemplatePath + "\").", owner);
			}
			else
			{
				GeneralExceptionHandling.Handle(exception, "Unable to load HTML template.", owner);
			}
			return false;
		}
		if (template == null)
		{
			if (!string.IsNullOrEmpty(text))
			{
				base.Log.Write("Unable to load HTML template (\"" + text + "\").");
			}
			else if (!string.IsNullOrEmpty(command.TemplatePath))
			{
				base.Log.Write("Unable to load HTML template (\"" + command.TemplatePath + "\").");
			}
			else
			{
				base.Log.Write("Unable to load HTML template.");
			}
			return false;
		}
		CustomFieldsSupport customFieldsSupport = new CustomFieldsSupport();
		customFieldsSupport.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: false);
		OtherFieldsSupport otherFieldsSupport = new OtherFieldsSupport();
		SetOtherFields(otherFieldsSupport, command);
		SetCustomFields(customFieldsSupport, command);
		LocalStorage destination = new LocalStorage(outputPath);
		ConsoleProgressTracker progress = new ConsoleProgressTracker();
		WizardOptions options = new WizardOptions
		{
			Title = (command as ExportVersion2)?.DocumentTitle,
			Scope = GetDocumentationModulesContainer(command.DataAsDocumentationModelBase),
			ExcludedObject = GetExcludedObjects(command),
			CustomFields = customFieldsSupport,
			OtherFields = otherFieldsSupport
		};
		try
		{
			TemplatesFactory.MakeHtmlTemplatesManager(owner).Export(new LocalTemplate(text), destination, progress, options);
			GC.Collect();
		}
		catch (OperationCanceledException)
		{
			return false;
		}
		return true;
	}

	protected bool ProcessPdfExport(ExportBase command, Form owner = null)
	{
		string outputPath = null;
		if (!TryGetOutputFilePath(isFile: true, command.OutputPathParsed, command.Overwrite, ref outputPath, owner))
		{
			return false;
		}
		if (!CheckTemplatePath(command.TemplatePath))
		{
			return false;
		}
		CustomFieldsSupport customFieldsSupport = new CustomFieldsSupport();
		customFieldsSupport.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: false);
		DocGeneratingOptions docGeneratingOptions = new DocGeneratingOptions(customFieldsSupport);
		SetGeneralData(command, docGeneratingOptions, owner);
		docGeneratingOptions.ExcludedObjects = GetExcludedObjects(command);
		docGeneratingOptions.SetLoadingErd();
		docGeneratingOptions.DocumentationTitle = (command as ExportVersion2)?.DocumentTitle;
		SetCustomFields(docGeneratingOptions.CustomFields, command);
		XtraReport xtraReport = null;
		try
		{
			xtraReport = DocGenerating.LoadTemplateWithData(docGeneratingOptions, out var dataLoaded, owner);
			if (!dataLoaded)
			{
				base.Log.Write("No data to export.");
				return false;
			}
		}
		catch (Exception exception)
		{
			if (!string.IsNullOrEmpty(command.TemplatePath))
			{
				GeneralExceptionHandling.Handle(exception, "Unable to load PDF template (\"" + command.TemplatePath + "\").", owner);
			}
			else
			{
				GeneralExceptionHandling.Handle(exception, "Unable to load PDF template.", owner);
			}
			return false;
		}
		if (xtraReport == null)
		{
			if (!string.IsNullOrEmpty(docGeneratingOptions.TemplateFilePath))
			{
				base.Log.Write("Unable to load PDF template (\"" + docGeneratingOptions.TemplateFilePath + "\").");
			}
			else if (!string.IsNullOrEmpty(command.TemplatePath))
			{
				base.Log.Write("Unable to load PDF template (\"" + command.TemplatePath + "\").");
			}
			else
			{
				base.Log.Write("Unable to load PDF template.");
			}
			return false;
		}
		List<string> list = new List<string>(xtraReport.ScriptReferences)
		{
			Assembly.GetAssembly(typeof(RichEditControl)).GetName().Name,
			"DevExpress.XtraRichEdit.v14.2.dll"
		};
		xtraReport.ScriptReferences = list.ToArray();
		xtraReport.ExportToPdf(outputPath);
		return true;
	}

	private void SetGeneralData(ExportBase command, DocGeneratingOptions docGeneratingOptions, Form owner = null)
	{
		docGeneratingOptions.TemplateFilePath = GetTemplatePath(command.TemplatePath, DocFormatEnum.DocFormat.PDF);
		docGeneratingOptions.Template = GetTemplate(command.TemplatePath, DocFormatEnum.DocFormat.PDF);
		docGeneratingOptions.DocumentationsModulesData = GetDocumentationModulesContainer(command.DataAsDocumentationModelBase);
		docGeneratingOptions.CoreTemplateFilePath = DocTemplateFile.GetCoreTemplateFilePath(docGeneratingOptions.Template, owner);
		docGeneratingOptions.CustomFields.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: false);
	}

	public ExportProcessor(Dataedo.App.Tools.CommandLine.Tools.Log log, string commandsFilePath)
		: base(log, commandsFilePath)
	{
	}

	protected abstract void InitializeExport(ExportBase exportCommand, ref int handlingResultsCount);

	protected bool ProcessExport(ExportBase exportCommand, Form owner = null)
	{
		SetModules(exportCommand);
		SetObjectTypes(exportCommand);
		if (exportCommand.Format == ExportBase.FormatEnum.PDF)
		{
			return ProcessPdfExport(exportCommand, owner);
		}
		if (exportCommand.Format == ExportBase.FormatEnum.HTML)
		{
			return ProcessHtmlExport(exportCommand, owner);
		}
		if (exportCommand.Format == ExportBase.FormatEnum.Excel)
		{
			return ProcessExcelExport(exportCommand, owner);
		}
		if (exportCommand.Format == ExportBase.FormatEnum.NotSet)
		{
			base.Log.Write("Output format is not set.");
			return false;
		}
		base.Log.Write("Output format is not supported.");
		return false;
	}

	protected override int ProcessInternal(CommandBase command, Form owner = null)
	{
		int handlingResultsCount = 0;
		if (command is ExportBase exportCommand)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			if (command.IsEnabled)
			{
				try
				{
					if (InitializeRepository(command as RepositoryCommandBase, owner))
					{
						InitializeExport(exportCommand, ref handlingResultsCount);
						if (handlingResultsCount == 0)
						{
							if (!ProcessExport(exportCommand, owner))
							{
								handlingResultsCount++;
							}
							handlingResultsCount += LogHandlingResults(GeneralHandlingSupport.StoredHandlingResults);
							GeneralHandlingSupport.ClearStoredHandlingResults();
						}
						else
						{
							handlingResultsCount++;
						}
					}
					else
					{
						handlingResultsCount++;
					}
				}
				catch (Exception exception)
				{
					GeneralHandlingSupport.StoreResult(GeneralExceptionHandling.Handle(exception, HandlingMethodEnumeration.HandlingMethod.NoAction, owner));
					handlingResultsCount++;
				}
				finally
				{
					handlingResultsCount += LogHandlingResults(GeneralHandlingSupport.StoredHandlingResults);
					GeneralHandlingSupport.ClearStoredHandlingResults();
					stopwatch.Stop();
					base.Log.WriteSimple("Elapsed time: " + stopwatch.Elapsed.ToString("hh\\:mm\\:ss") + ".");
					if (handlingResultsCount > 0)
					{
						base.Log.Write("Export failed.");
					}
					else
					{
						base.Log.Write("Export finished successfully.");
					}
				}
			}
			else
			{
				base.Log.Write("Command is not enabled.");
			}
		}
		else
		{
			base.Log.Write("Command is not valid.");
		}
		return handlingResultsCount;
	}

	protected abstract void SetModules(ExportBase exportCommand);

	protected void SetObjectTypes(ExportBase exportCommand)
	{
		if (exportCommand.ObjectTypes == null)
		{
			base.Log.Write("List of types of objects to export is not specified in command file.", "All types of objects will be exported.");
			exportCommand.ObjectTypes = ObjectTypesModel.GetExportAllModel();
		}
	}

	private bool TryGetOutputFilePath(bool isFile, string outputPathParsed, bool overwrite, ref string outputPath, Form owner = null)
	{
		if (string.IsNullOrEmpty(outputPathParsed))
		{
			base.Log.Write("Output path is not set.");
			return false;
		}
		try
		{
			outputPath = PathHelpers.GetRootedOrRelative(base.CommandsFilePath, outputPathParsed);
			base.Log.Write("Output path: \"" + outputPath + "\"");
			if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
			}
			new FileInfo(outputPath);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "The output path (\"" + outputPathParsed + "\") is invalid.", owner);
			return false;
		}
		if (isFile && !CheckFile(overwrite, outputPath))
		{
			return false;
		}
		if (!isFile && !CheckDirectory(overwrite, outputPath))
		{
			return false;
		}
		return true;
	}

	private bool CheckTemplatePath(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			base.Log.Write("Template path is not set.");
			return false;
		}
		return true;
	}

	private string GetTemplatePath(string path, DocFormatEnum.DocFormat format, Form owner = null)
	{
		return PathHelpers.GetRootedOrRelative(Path.Combine(DocTemplateFile.GetTemplatesPath(format, owner), path), path);
	}

	private IBaseTemplateModel GetTemplate(string path, DocFormatEnum.DocFormat format, Form owner = null)
	{
		PathHelpers.GetRootedOrRelative(Path.Combine(DocTemplateFile.GetTemplatesPath(format, owner), path), path);
		return new DocTemplateFile(DocFormatEnum.DocFormat.PDF, GetTemplatePath(path, format, owner)).Template;
	}

	private DocumentationModulesContainer GetDocumentationModulesContainer(IEnumerable<DocumentationModelBase> documentationModelBaseEnumerable)
	{
		DocumentationModulesContainer documentationModulesContainer = new DocumentationModulesContainer();
		foreach (DocumentationModelBase item in documentationModelBaseEnumerable.Where((DocumentationModelBase x) => x.Export))
		{
			documentationModulesContainer.Data.Add(new DocumentationModules
			{
				Documentation = item.RepositoryDatabaseRow,
				Modules = item.SelectedModulesIds.Select((int x) => new ModuleRow
				{
					Id = x
				}).ToList(),
				IsSelected = true
			});
		}
		return documentationModulesContainer;
	}

	private List<ObjectTypeHierarchy> GetExcludedObjects(ExportBase command)
	{
		List<ObjectTypeHierarchy> list = new List<ObjectTypeHierarchy>();
		if (command != null && command.ObjectTypes?.Documentation?.Export == true)
		{
			if (command == null || command.ObjectTypes?.Documentation?.DatabaseName?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(CustomExcludedTypeEnum.CustomExcludedType.DatabaseName));
			}
			if (command == null || command.ObjectTypes?.Documentation?.HostName?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(CustomExcludedTypeEnum.CustomExcludedType.HostName));
			}
		}
		if (command != null && command.ObjectTypes?.Tables?.Export == true)
		{
			if (command == null || command.ObjectTypes?.Tables?.Relations?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Relation));
			}
			if (command == null || command.ObjectTypes?.Tables?.Keys?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Key));
			}
			if (command == null || command.ObjectTypes?.Tables?.Triggers?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Trigger));
			}
			else if (command == null || command.ObjectTypes?.Tables?.Triggers?.Script?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Trigger, SharedObjectTypeEnum.ObjectType.Script, SharedObjectTypeEnum.ObjectType.Table));
			}
			if (command == null || command.ObjectTypes?.Tables?.Dependencies?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Dependency));
			}
		}
		else
		{
			list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Table));
		}
		if (command != null && command.ObjectTypes?.Views?.Export == true)
		{
			if (command == null || command.ObjectTypes?.Views?.Relations?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Relation));
			}
			if (command == null || command.ObjectTypes?.Views?.Keys?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Key));
			}
			if (command == null || command.ObjectTypes?.Views?.Triggers?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Trigger));
			}
			else if (command == null || command.ObjectTypes?.Views?.Triggers?.Script?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Trigger, SharedObjectTypeEnum.ObjectType.Script, SharedObjectTypeEnum.ObjectType.View));
			}
			if (command == null || command.ObjectTypes?.Views?.Dependencies?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Dependency));
			}
			if (command == null || command.ObjectTypes?.Views?.Script?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Script));
			}
		}
		else
		{
			list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.View));
		}
		if (command != null && command.ObjectTypes?.Procedures?.Export == true)
		{
			if (command == null || command.ObjectTypes?.Procedures?.Dependencies?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectTypeEnum.ObjectType.Dependency));
			}
			if (command == null || command.ObjectTypes?.Procedures?.Script?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectTypeEnum.ObjectType.Script));
			}
		}
		else
		{
			list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Procedure));
		}
		if (command != null && command.ObjectTypes?.Functions?.Export == true)
		{
			if (command == null || command.ObjectTypes?.Functions?.Dependencies?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Function, SharedObjectTypeEnum.ObjectType.Dependency));
			}
			if (command == null || command.ObjectTypes?.Functions?.Script?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Function, SharedObjectTypeEnum.ObjectType.Script));
			}
		}
		else
		{
			list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Function));
		}
		if (command != null && command.ObjectTypes?.Structures?.Export == true)
		{
			if (command == null || command.ObjectTypes?.Structures?.Relations?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Relation));
			}
			if (command == null || command.ObjectTypes?.Structures?.Keys?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Key));
			}
			if (command == null || command.ObjectTypes?.Structures?.Dependencies?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Dependency));
			}
			if (command == null || command.ObjectTypes?.Structures?.Schema?.Export != true)
			{
				list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Schema));
			}
		}
		else
		{
			list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Structure));
		}
		if (command != null && command.ObjectTypes?.ERDs?.Export == false)
		{
			list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.Erd));
		}
		bool flag = (command != null && command.ObjectTypes?.BusinessGloassaries?.Export == false) || !Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary);
		if (command == null || command.ObjectTypes?.BusinessGloassaries?.Terms?.Export != true || flag)
		{
			list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Term));
		}
		if (command == null || command.ObjectTypes?.BusinessGloassaries?.Categories?.Export != true || flag)
		{
			list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Category));
		}
		if (command == null || command.ObjectTypes?.BusinessGloassaries?.Rules?.Export != true || flag)
		{
			list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Rule));
		}
		if (command == null || command.ObjectTypes?.BusinessGloassaries?.Policies?.Export != true || flag)
		{
			list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Policy));
		}
		if (command == null || command.ObjectTypes?.BusinessGloassaries?.Other?.Export != true || flag)
		{
			list.Add(new ObjectTypeHierarchy(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Other));
		}
		return list;
	}

	public void SetCustomFields(CustomFieldsSupport customFieldsSupport, ExportBase command)
	{
		CustomFieldsModel customFieldsModel = (command as ExportVersion2)?.CustomFields;
		if (customFieldsModel == null)
		{
			base.Log.Write("List of custom fields to export is not specified in command file.", "Custom fields will not be exported.");
			return;
		}
		if (customFieldsModel.ExportNotSpecified)
		{
			customFieldsSupport.SelectAll();
		}
		else
		{
			customFieldsSupport.UnselectAll();
		}
		foreach (CustomFieldModelBase customField in customFieldsModel.CustomFields)
		{
			CustomFieldRowExtended fieldById = customFieldsSupport.GetFieldById(customField.CustomFieldId);
			if (fieldById != null)
			{
				fieldById.IsSelected = customField.Export;
			}
			else
			{
				base.Log.Write($"Custom field with ID: {customField.CustomFieldId} not found.");
			}
		}
	}

	public void SetOtherFields(OtherFieldsSupport otherFieldsSupport, ExportBase command)
	{
		FieldsModel fieldsModel = (command as ExportVersion2)?.Fields;
		if (fieldsModel == null)
		{
			base.Log.Write("List of fields to export is not specified in command file.", "All fields will be exported.");
			otherFieldsSupport.SelectAll();
			return;
		}
		if (fieldsModel.ExportNotSpecified)
		{
			otherFieldsSupport.SelectAll();
		}
		else
		{
			otherFieldsSupport.UnselectAll();
		}
		otherFieldsSupport.SetField(OtherFieldEnum.OtherField.Description, fieldsModel.Description);
		otherFieldsSupport.SetField(OtherFieldEnum.OtherField.Title, fieldsModel.Title);
		otherFieldsSupport.SetField(OtherFieldEnum.OtherField.DataType, fieldsModel.DataType);
		otherFieldsSupport.SetField(OtherFieldEnum.OtherField.Nullable, fieldsModel.Nullable);
		otherFieldsSupport.SetField(OtherFieldEnum.OtherField.Identity, fieldsModel.Identity);
		otherFieldsSupport.SetField(OtherFieldEnum.OtherField.DefaultComputed, fieldsModel.DefaultComputed);
	}

	private bool CheckFile(bool overwrite, string path)
	{
		if (!overwrite && File.Exists(path))
		{
			base.Log.Write("File exists and won't be overwritten. Change value of 'Overwrite' element (in Dataedo command file: \"" + base.CommandsFilePath + "\") to 'true' if you want the file to be overwritten if already exists.");
			return false;
		}
		return true;
	}

	private bool CheckDirectory(bool overwrite, string path)
	{
		if (!overwrite && Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Count() > 0)
		{
			base.Log.Write("Folder is not empty. Files won't be overwritten. Change value of 'Overwrite' element (in Dataedo command file: \"" + base.CommandsFilePath + "\") to 'true' if you want the files to be overwritten if already exists.");
			return false;
		}
		return true;
	}
}
