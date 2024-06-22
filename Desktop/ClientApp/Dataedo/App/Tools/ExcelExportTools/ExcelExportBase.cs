using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Documentation;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.Export.XmlExportTools.Tools;
using Dataedo.App.Tools.XmlExportTools.Model;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.Modules;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;
using DevExpress.Spreadsheet;

namespace Dataedo.App.Tools.ExcelExportTools;

internal abstract class ExcelExportBase
{
	protected class DocumentationObjectRow
	{
		public ModuleExportData Documentation { get; set; }

		public ObjectRow ObjectRow { get; set; }

		public DocumentationObjectRow(ModuleExportData documentation, ObjectRow objectRow)
		{
			Documentation = documentation;
			ObjectRow = objectRow;
		}
	}

	protected List<SheetNameData> WorksheetNames;

	protected Dictionary<string, int> UsedSheetNames;

	protected IBackgroundProcessingWorkerReporting Worker;

	protected DocumentationModulesContainer DocumentationsModulesData;

	protected List<ObjectTypeHierarchy> ExcludedTypes;

	private Regex replaceRegex;

	private int worksheetIndex;

	private int FirstColumn;

	public ExcelExportBase()
	{
		UsedSheetNames = new Dictionary<string, int>();
		replaceRegex = new Regex("[\\\\/\\?\\:\\*\\[\\]]|^'|'$");
	}

	public ModuleExportData LoadDatabaseData(int documentationId, List<int> modules, List<ObjectTypeHierarchy> excludedTypes, CustomFieldsSupport customFieldsSupport)
	{
		Worker.ReportProgress("Loading data: database information...");
		ModuleExportData moduleExportData = new ModuleExportData
		{
			Module = new DatabaseRow(DB.Database.GetDataByIdWithCounter(documentationId, notDeletedOnly: true), customFieldsSupport),
			ExportData = new ExportData(),
			ModulesExportData = new List<ModuleExportData>()
		};
		DatabaseRow databaseRow = moduleExportData.Module as DatabaseRow;
		Worker.ReportProgress("Loading data...", 0);
		Worker.SetPercentTotalProgressStep(10f);
		Worker.SetPercentCurrentProgressStep(10f);
		moduleExportData.ExportData.TablesSource = new List<ObjectRow>();
		moduleExportData.ExportData.ViewsSource = new List<ObjectRow>();
		moduleExportData.ExportData.ProceduresSource = new List<ObjectRow>();
		moduleExportData.ExportData.FunctionsSource = new List<ObjectRow>();
		moduleExportData.ExportData.StructuresSource = new List<ObjectRow>();
		Worker.ReportProgress("Loading data: modules...");
		moduleExportData.ExportData.ModulesSource = LoadModuleRows(DB.Module.GetDataByDatabase(documentationId), customFieldsSupport);
		moduleExportData.ExportData.ModulesSource.Add(new ModuleRow
		{
			Id = -1
		});
		IEnumerable<ModuleRow> enumerable = moduleExportData.ExportData.ModulesSource.Where((ModuleRow x) => DocumentationsModulesData.IsModuleSelected(documentationId, x.IdValue));
		int num = enumerable.Count();
		Worker.RevertProgressStep();
		Worker.SetPercentTotalProgressStep(90f);
		Worker.SetPercentCurrentProgressStep(90f);
		Worker.ReportProgress("Loading data: modules...");
		Worker.DivideProgressStep(num);
		foreach (ModuleRow item in enumerable)
		{
			if (item.IdValue != -1)
			{
				Worker.ReportProgress("Loading data: module \"" + item.Title + "\"...");
			}
			else
			{
				Worker.ReportProgress("Loading data: other...");
			}
			IEnumerable<ObjectByModuleObject> enumerable2 = null;
			IEnumerable<ObjectByModuleObject> enumerable3 = null;
			List<ObjectByModuleObject> list = null;
			List<ObjectByModuleObject> list2 = null;
			IEnumerable<ObjectByModuleObject> enumerable4 = null;
			int num2 = 0;
			if (!excludedTypes.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Table)))
			{
				enumerable2 = ((item.IdValue == -1) ? DB.Table.GetTablesWithoutModule(databaseRow.IdValue, notDeletedOnly: true) : DB.Table.GetTablesByModule(item.IdValue, notDeletedOnly: true));
				num2 += enumerable2.Count();
			}
			if (!excludedTypes.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.View)))
			{
				enumerable3 = ((item.IdValue == -1) ? DB.Table.GetViewsWithoutModule(databaseRow.IdValue, notDeletedOnly: true) : DB.Table.GetViewsByModule(item.IdValue, notDeletedOnly: true));
				num2 += enumerable3.Count();
			}
			if (!excludedTypes.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Procedure)))
			{
				list = ((item.IdValue == -1) ? DB.Procedure.GetProceduresWithoutModule(databaseRow.IdValue, notDeletedOnly: true) : DB.Procedure.GetProceduresByModule(item.IdValue, notDeletedOnly: true));
				num2 += list.Count;
			}
			if (!excludedTypes.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Function)))
			{
				list2 = ((item.IdValue == -1) ? DB.Procedure.GetFunctionsWithoutModule(databaseRow.IdValue, notDeletedOnly: true) : DB.Procedure.GetFunctionsByModule(item.IdValue, notDeletedOnly: true));
				num2 += list2.Count;
			}
			if (!excludedTypes.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Structure)))
			{
				enumerable4 = ((item.IdValue == -1) ? DB.Table.GetStructuresWithoutModule(databaseRow.IdValue, notDeletedOnly: true) : DB.Table.GetStructuresByModule(item.IdValue, notDeletedOnly: true));
				num2 += enumerable4.Count();
			}
			ModuleExportData moduleExportData2 = new ModuleExportData
			{
				Module = item,
				ExportData = new ExportData(),
				IsForExport = DocumentationsModulesData.IsModuleSelected(documentationId, item.IdValue)
			};
			if (num2 > 0)
			{
				Worker.DivideProgressStep(num2);
				if (enumerable2 != null)
				{
					Worker.ReportProgress("Loading data: module \"" + item.Title + "\" tables...");
					moduleExportData2.ExportData.TablesSource = LoadRows(moduleExportData2, databaseRow.IdValue, enumerable2.ToList(), databaseRow.Type, customFieldsSupport);
					AddIfModuleIsSelected(documentationId, item.Id, moduleExportData.ExportData.TablesSource, moduleExportData2.ExportData.TablesSource);
				}
				if (enumerable3 != null)
				{
					Worker.ReportProgress("Loading data: module \"" + item.Title + "\" views...");
					moduleExportData2.ExportData.ViewsSource = LoadRows(moduleExportData2, databaseRow.IdValue, enumerable3.ToList(), databaseRow.Type, customFieldsSupport);
					AddIfModuleIsSelected(documentationId, item.Id, moduleExportData.ExportData.ViewsSource, moduleExportData2.ExportData.ViewsSource);
				}
				if (list != null)
				{
					Worker.ReportProgress("Loading data: module \"" + item.Title + "\" procedures...");
					moduleExportData2.ExportData.ProceduresSource = LoadRows(moduleExportData2, databaseRow.IdValue, list, databaseRow.Type, customFieldsSupport);
					AddIfModuleIsSelected(documentationId, item.Id, moduleExportData.ExportData.ProceduresSource, moduleExportData2.ExportData.ProceduresSource);
				}
				if (list2 != null)
				{
					Worker.ReportProgress("Loading data: module \"" + item.Title + "\" functions...");
					moduleExportData2.ExportData.FunctionsSource = LoadRows(moduleExportData2, databaseRow.IdValue, list2, databaseRow.Type, customFieldsSupport);
					AddIfModuleIsSelected(documentationId, item.Id, moduleExportData.ExportData.FunctionsSource, moduleExportData2.ExportData.FunctionsSource);
				}
				if (enumerable4 != null)
				{
					Worker.ReportProgress("Loading data: module \"" + item.Title + "\" structures...");
					moduleExportData2.ExportData.StructuresSource = LoadRows(moduleExportData2, databaseRow.IdValue, enumerable4.ToList(), databaseRow.Type, customFieldsSupport);
					AddIfModuleIsSelected(documentationId, item.Id, moduleExportData.ExportData.StructuresSource, moduleExportData2.ExportData.StructuresSource);
				}
				moduleExportData2.ExportData.ModulesData = null;
				bool flag = moduleExportData2.ExportData.SetObjectsDatabaseContext();
				moduleExportData.IsAnyFromAnotherDocumentation = moduleExportData.IsAnyFromAnotherDocumentation == true || flag;
				Worker.RevertProgressStep();
			}
			else
			{
				Worker.IncreaseProgress();
			}
			moduleExportData.ModulesExportData.Add(moduleExportData2);
		}
		Worker.RevertProgressStep();
		Worker.RevertProgressStep();
		return moduleExportData;
	}

	protected List<ObjectRow> LoadRows(ModuleExportData moduleExportData, int currentDatabaseId, List<ObjectByModuleObject> dataView, SharedDatabaseTypeEnum.DatabaseType? databaseType, CustomFieldsSupport customFieldsSupport)
	{
		List<ObjectRow> list = new List<ObjectRow>();
		foreach (ObjectByModuleObject item in dataView)
		{
			ObjectRow objectRow = (databaseType.HasValue ? (DatabaseSupportFactory.GetDatabaseSupport(databaseType).IsSchemaType ? new OracleObjectRow(item, customFieldsSupport, readDisplayedName: false, readDocumentationData: true) : new ObjectRow(item, customFieldsSupport, readDisplayedName: false, readDocumentationData: true)) : new OtherDatabaseObjectRow(item, customFieldsSupport, readDisplayedName: false, readDocumentationTitle: true));
			objectRow.IsFromAnotherDatabase = currentDatabaseId != objectRow.DatabaseId;
			moduleExportData.IsAnyFromAnotherDocumentation = moduleExportData.IsAnyFromAnotherDocumentation == true || objectRow.IsFromAnotherDatabase == true;
			Worker.SetCurrentObject(objectRow.ObjectName);
			list.Add(objectRow);
			Worker.IncreaseProgress();
		}
		return list;
	}

	protected List<ModuleRow> LoadModuleRows(List<ModuleObject> dataView, CustomFieldsSupport customFieldsSupport)
	{
		List<ModuleRow> list = new List<ModuleRow>();
		if (dataView.Count > 0)
		{
			Worker.DivideProgressStep(dataView.Count);
			foreach (ModuleObject item in dataView)
			{
				ModuleRow moduleRow = new ModuleRow(item, customFieldsSupport, forXml: true);
				Worker.SetCurrentObject(moduleRow.Name);
				list.Add(moduleRow);
				Worker.IncreaseProgress();
			}
			Worker.RevertProgressStep();
		}
		else
		{
			Worker.IncreaseProgress();
		}
		return list;
	}

	protected string GetJoinCondition(ObjectRow currentObject, RelationRow relationRow, ModuleExportData documentationExportData, bool isRelationTo)
	{
		return string.Join(values: (!isRelationTo) ? relationRow?.Columns?.Where((RelationColumnRow c) => !string.IsNullOrEmpty(c.ColumnFkName) && !string.IsNullOrEmpty(c.ColumnPkName))?.Select((RelationColumnRow c) => ColumnNames.GetFullName(c.ColumnFkPath, c.ColumnFkName) + " = " + relationRow.PKTableObjectName + "." + ColumnNames.GetFullName(c.ColumnPkPath, c.ColumnPkName)) : relationRow?.Columns?.Where((RelationColumnRow c) => !string.IsNullOrEmpty(c.ColumnFkName) && !string.IsNullOrEmpty(c.ColumnPkName))?.Select((RelationColumnRow c) => relationRow.FKTableObjectName + "." + ColumnNames.GetFullName(c.ColumnFkPath, c.ColumnFkName) + " = " + ColumnNames.GetFullName(c.ColumnPkPath, c.ColumnPkName)), separator: " and" + Environment.NewLine);
	}

	private void AddIfModuleIsSelected(int documentationId, int? moduleId, List<ObjectRow> destination, List<ObjectRow> source)
	{
		if (!moduleId.HasValue || !DocumentationsModulesData.IsModuleSelected(documentationId, moduleId.Value))
		{
			return;
		}
		destination.AddRange(source.Where((ObjectRow o) => !destination.Any((ObjectRow i) => i.ObjectId == o.ObjectId)));
	}

	private string CorrectSheetNameWhenAlreadyExists(string sheetName)
	{
		sheetName = replaceRegex.Replace(sheetName, "_");
		sheetName = new string(sheetName.Take(31).ToArray());
		string text = sheetName.ToLower();
		string text2 = sheetName;
		while (UsedSheetNames.ContainsKey(text2.ToLower()))
		{
			string text3 = " (" + (UsedSheetNames[text2.ToLower()] + 1) + ")";
			text2 = new string(text2.Take(31 - text3.Length).ToArray());
			if (text2.ToLower() == text)
			{
				UsedSheetNames[text2.ToLower()]++;
				text2 += text3;
				continue;
			}
			if (!UsedSheetNames.ContainsKey(text2.ToLower()))
			{
				UsedSheetNames.Add(text2.ToLower(), ++UsedSheetNames[text]);
			}
			text = text2.ToLower();
		}
		UsedSheetNames.Add(text2.ToLower(), 1);
		return text2;
	}

	protected void CellSetTextAndBold(Cell cell, string value)
	{
		cell.Value = value;
		cell.Font.Bold = true;
	}

	protected Worksheet AddWorksheet(IWorkbook workbook, int? documentationId, int? objectId, string sheetName, string title = null, SharedObjectTypeEnum.ObjectType? objectType = null, bool startFromBeginning = true)
	{
		sheetName = CorrectSheetNameWhenAlreadyExists(sheetName);
		WorksheetNames.Add(new SheetNameData(documentationId, objectId, sheetName, title, objectType));
		Worksheet worksheet = null;
		if (startFromBeginning)
		{
			worksheet = ((worksheetIndex == 0) ? workbook.Worksheets[0] : workbook.Worksheets.Insert(worksheetIndex, sheetName));
			worksheet.Name = sheetName;
			worksheetIndex++;
			return worksheet;
		}
		try
		{
			worksheet = workbook.Worksheets.Add();
			worksheet.Name = sheetName;
			return worksheet;
		}
		catch (Exception)
		{
			throw;
		}
	}

	protected Worksheet AddWorksheet(IWorkbook workbook, ObjectRow objectRow)
	{
		string text = CorrectSheetNameWhenAlreadyExists(objectRow.Name.ToXmlValidString());
		WorksheetNames.Add(new SheetNameData(objectRow.DatabaseId, objectRow.ObjectId, objectRow.Schema, objectRow.Name, objectRow.Type, text));
		Worksheet worksheet = workbook.Worksheets.Add();
		worksheet.Name = text;
		return worksheet;
	}

	protected void CreateTableHeader(Worksheet worksheet, string header, ref int currentRow)
	{
		worksheet.Cells[currentRow, FirstColumn].Font.Bold = true;
		worksheet.Cells[currentRow++, FirstColumn].Value = header;
	}

	protected CellRange InsertFromDataTable(Worksheet worksheet, DataTable sourceTable, ref int currentRow)
	{
		worksheet.Import(sourceTable, addHeader: true, currentRow, FirstColumn);
		CellRange cellRange = worksheet.Range.FromLTRB(FirstColumn, currentRow, sourceTable.Columns.Count - 1, sourceTable.Rows.Count + currentRow);
		CreateTableAndAddStyle(worksheet, cellRange);
		currentRow += sourceTable.Rows.Count + 2;
		return cellRange;
	}

	private string CutSheetName(string objectName, int numberOfSigns)
	{
		if (objectName.Length < numberOfSigns)
		{
			return objectName;
		}
		return objectName.Substring(0, numberOfSigns - 1);
	}

	private void CreateTableAndAddStyle(Worksheet worksheet, CellRange range)
	{
		worksheet.Tables.Add(range, hasHeaders: true).Style = worksheet.Workbook.TableStyles[BuiltInTableStyleId.TableStyleMedium2];
	}

	protected void CreateObjectHeader(Worksheet worksheet, ModuleExportData databaseExportData, bool showDocumentationTitle, ObjectRow objectRow, ref int currentRow)
	{
		int columnOffset = FirstColumn + 1;
		CellSetTextAndBold(worksheet.Cells[currentRow, FirstColumn], SharedObjectSubtypeEnum.TypeToStringForSingle(objectRow.Type, objectRow.Subtype) + ":");
		worksheet.Cells[currentRow, columnOffset].Value = objectRow.DisplayedName;
		currentRow += 2;
		if (showDocumentationTitle)
		{
			CellSetTextAndBold(worksheet.Cells[currentRow, FirstColumn], "Documentation");
			worksheet.Cells[currentRow++, columnOffset].Value = objectRow.DocumentationTitle;
		}
		CellSetTextAndBold(worksheet.Cells[currentRow, FirstColumn], "Schema");
		worksheet.Cells[currentRow++, columnOffset].Value = objectRow.Schema;
		CellSetTextAndBold(worksheet.Cells[currentRow, FirstColumn], "Name");
		worksheet.Cells[currentRow++, columnOffset].Value = objectRow.Name;
		CellSetTextAndBold(worksheet.Cells[currentRow, FirstColumn], "Title");
		worksheet.Cells[currentRow++, columnOffset].Value = objectRow.Title;
		AddCustomFieldsToObjectHeader(worksheet, objectRow, ref currentRow);
		currentRow++;
	}

	protected void AddCustomFieldsToObjectHeader(Worksheet worksheet, ISupportsCustomFields objectRow, ref int currentRow)
	{
		int columnOffset = FirstColumn + 1;
		foreach (CustomFieldDefinition item in objectRow.CustomFields?.GetValidFields(objectRow.ObjectTypeValue, mustHaveValue: false) ?? (from x in Enumerable.Empty<CustomFieldDefinition>()
			orderby 1
			select x))
		{
			CellSetTextAndBold(worksheet.Cells[currentRow, FirstColumn], item.CustomField.Title);
			worksheet.Cells[currentRow++, columnOffset].Value = item.FieldValue;
		}
	}

	protected void AddValuesWithCustomFields(string documentationTitle, bool showDocumentationTitle, DataTable sourceTable, ISupportsCustomFields objectRow, SharedObjectTypeEnum.ObjectType? objectType, params object[] currentValues)
	{
		List<object> list = new List<object>(currentValues);
		if (showDocumentationTitle)
		{
			list.Insert(0, documentationTitle);
		}
		IOrderedEnumerable<CustomFieldDefinition> source = objectRow.CustomFields?.GetValidFields(objectType, mustHaveValue: false) ?? (from x in Enumerable.Empty<CustomFieldDefinition>()
			orderby 1
			select x);
		list.AddRange(source.Select((CustomFieldDefinition x) => x.FieldValue).ToArray());
		sourceTable.Rows.Add(list.ToArray());
	}

	protected void AddValuesWithCustomFields(DataTable sourceTable, ISupportsCustomFields objectRow, SharedObjectTypeEnum.ObjectType? objectType, params object[] currentValues)
	{
		AddValuesWithCustomFields(null, showDocumentationTitle: false, sourceTable, objectRow, objectType, currentValues);
	}

	protected void AddValuesWithCustomFields(string documentationTitle, bool showDocumentationTitle, DataTable sourceTable, ObjectRow objectRow, SharedObjectTypeEnum.ObjectType? objectType, params object[] currentValues)
	{
		List<object> list = new List<object>(currentValues);
		if (showDocumentationTitle)
		{
			list.Insert(0, documentationTitle);
		}
		IOrderedEnumerable<CustomFieldDefinition> source = objectRow.CustomFields?.GetValidFields(objectType, mustHaveValue: false) ?? (from x in Enumerable.Empty<CustomFieldDefinition>()
			orderby 1
			select x);
		list.AddRange(source.Select((CustomFieldDefinition x) => x.FieldValue).ToArray());
		sourceTable.Rows.Add(list.ToArray());
	}

	protected void AddCustomFieldsColumns(DataTable sourceTable, CustomFieldContainer customFieldContainer, SharedObjectTypeEnum.ObjectType? objectType)
	{
		foreach (CustomFieldDefinition validField in customFieldContainer.GetValidFields(objectType, mustHaveValue: false))
		{
			sourceTable.Columns.Add(PreparaName(validField.CustomField.Title, (string name) => sourceTable.Columns.Contains(name)), typeof(string));
		}
	}

	protected string PreparaName(string name, Func<string, bool> containsFunction)
	{
		string text = name;
		int num = 1;
		while (containsFunction(text))
		{
			num++;
			text = $"{name} ({num})";
		}
		return text;
	}

	protected Worksheet InsertWoksheetIfNotEmpty(IWorkbook workbook, string name, string header, DataTable data, bool createHeader = true)
	{
		if (data.Rows.Count > 0)
		{
			Worksheet worksheet = AddWorksheet(workbook, null, null, name);
			int currentRow = 0;
			if (createHeader)
			{
				CreateTableHeader(worksheet, header, ref currentRow);
			}
			InsertFromDataTable(worksheet, data, ref currentRow);
			return worksheet;
		}
		return null;
	}

	protected Worksheet InsertWoksheetIfNotEmpty(IWorkbook workbook, string name, DataTable data, bool createHeader = true)
	{
		return InsertWoksheetIfNotEmpty(workbook, "(" + name + ")", name, data, createHeader);
	}
}
