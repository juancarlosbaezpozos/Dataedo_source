using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Documentation;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.ERD;
using Dataedo.App.Tools.ERD.Diagram;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.XmlExportTools.Model;
using Dataedo.Model.Data.Procedures.Parameters;
using Dataedo.Model.Data.Tables.Constraints;
using Dataedo.Model.Data.Tables.Relations;
using Dataedo.Model.Data.Tables.Triggers;
using Dataedo.Shared.Enums;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet;

namespace Dataedo.App.Tools.ExcelExportTools;

internal class ExcelExportTypePerSheet : ExcelExportBase
{
	public void Export(SpreadsheetControl spreadsheetControl, IBackgroundProcessingWorkerReporting worker, DocumentationModulesContainer documentationsModulesData, List<ObjectTypeHierarchy> excludedTypes, CustomFieldsSupport customFieldsSupport, string path)
	{
		IWorkbook document = spreadsheetControl.Document;
		WorksheetNames = new List<SheetNameData>();
		Worker = worker;
		Worker.ReportProgress("Preparing...", 0);
		ExcludedTypes = excludedTypes ?? new List<ObjectTypeHierarchy>();
		DocumentationsModulesData = documentationsModulesData ?? new DocumentationModulesContainer();
		List<ModuleExportData> list = new List<ModuleExportData>();
		Worker.SetTotalProgressStep(100f);
		Worker.SetProgressStep(100f);
		try
		{
			document.BeginUpdate();
			Worker.SetPercentTotalProgressStep(5f);
			Worker.ReportProgress("Loading data...", 0);
			Worker.DivideProgressStep(DocumentationsModulesData.SelectedDocumentations.Count());
			foreach (DocumentationModules selectedDocumentation in DocumentationsModulesData.SelectedDocumentations)
			{
				StaticData.CrashedDatabaseType = selectedDocumentation?.Documentation?.Type;
				StaticData.CrashedDBMSVersion = selectedDocumentation?.Documentation?.DbmsVersion;
				list.Add(LoadDatabaseData(selectedDocumentation.Documentation.IdValue, selectedDocumentation.Modules.Select((ModuleRow x) => x.IdValue).ToList(), excludedTypes, customFieldsSupport));
			}
			StaticData.ClearDatabaseInfoForCrashes();
			Worker.RevertProgressStep();
			Worker.RevertTotalProgressStep();
			Worker.SetPercentTotalProgressStep(85f);
			ExportDocumentations(document, list, customFieldsSupport);
			Worker.RevertTotalProgressStep();
			Worker.SetPercentTotalProgressStep(10f);
			Worker.DivideProgressStep(document.Worksheets.Count);
			Worker.ReportProgress("Preparing worksheets...", 0);
			foreach (Worksheet item in document.Worksheets.ToList())
			{
				Worker.SetCurrentObject(item.Name);
				item.Columns.AutoFit(0, item.GetUsedRange().ColumnCount);
				Worker.IncreaseProgress();
			}
			Worker.SetCurrentObject(null);
			document.Worksheets.ActiveWorksheet = document.Worksheets.FirstOrDefault();
			Worker.RevertProgressStep();
			Worker.RevertTotalProgressStep();
			if (Path.GetExtension(path).ToLower() == ".xls")
			{
				spreadsheetControl.SaveDocument(path, DevExpress.Spreadsheet.DocumentFormat.Xls);
			}
			else
			{
				spreadsheetControl.SaveDocument(path, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
			}
			worker.HasResult = true;
			Worker.ReportProgress("Finished!");
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception exception)
		{
			Worker.HasError = true;
			GeneralFileExceptionHandling.Handle(exception, "An error occurred while exporting documentation.");
		}
	}

	private void ExportDocumentations(IWorkbook workbook, List<ModuleExportData> databasesExportData, CustomFieldsSupport customFieldsSupport)
	{
		bool flag = databasesExportData.Count > 1;
		bool showDocumentationTitle = flag || databasesExportData.Any((ModuleExportData x) => x.IsAnyFromAnotherDocumentation == true);
		Worker.SetTotalProgressStepByPercent(10f);
		Worker.ReportProgress("Exporting lists of objects...", 0);
		Worker.DivideProgressStep(5f);
		ExportModulesList(workbook, flag, databasesExportData);
		ExportObjectList(workbook, showDocumentationTitle, databasesExportData, SharedObjectTypeEnum.ObjectType.Table);
		ExportObjectList(workbook, showDocumentationTitle, databasesExportData, SharedObjectTypeEnum.ObjectType.View);
		ExportObjectList(workbook, showDocumentationTitle, databasesExportData, SharedObjectTypeEnum.ObjectType.Procedure);
		ExportObjectList(workbook, showDocumentationTitle, databasesExportData, SharedObjectTypeEnum.ObjectType.Function);
		ExportObjectList(workbook, showDocumentationTitle, databasesExportData, SharedObjectTypeEnum.ObjectType.Structure);
		Worker.ReportProgress(100);
		Worker.RevertProgressStep();
		Worker.RevertTotalProgressStep();
		Worker.SetTotalProgressStepByPercent(90f);
		Worker.ReportProgress("Exporting objects' data...", 0);
		Worker.DivideProgressStep(7f);
		Worker.ReportProgress("Exporting columns...");
		ExportColumns(workbook, showDocumentationTitle, databasesExportData, customFieldsSupport);
		Worker.ReportProgress("Exporting relationships...");
		ExportRelations(workbook, showDocumentationTitle, databasesExportData, customFieldsSupport);
		Worker.ReportProgress("Exporting relationships - columns...");
		ExportRelationsColumns(workbook, showDocumentationTitle, databasesExportData, customFieldsSupport);
		Worker.ReportProgress("Exporting unique keys...");
		ExportUniqueKeys(workbook, showDocumentationTitle, databasesExportData, customFieldsSupport);
		Worker.ReportProgress("Exporting unique keys - columns...");
		ExportUniqueKeysColumns(workbook, showDocumentationTitle, databasesExportData, customFieldsSupport);
		Worker.ReportProgress("Exporting triggers...");
		ExportTriggers(workbook, showDocumentationTitle, databasesExportData, customFieldsSupport);
		Worker.ReportProgress("Exporting input-output...");
		ExportInputOutput(workbook, showDocumentationTitle, databasesExportData, customFieldsSupport);
		Worker.ReportProgress(100);
		Worker.RevertProgressStep();
		Worker.RevertTotalProgressStep();
	}

	private void ExportModulesList(IWorkbook workbook, bool showDocumentationTitle, List<ModuleExportData> databasesExportData)
	{
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Module)))
		{
			if (databasesExportData.Any((ModuleExportData x) => x.ModulesExportDataForExport.Count() > 0))
			{
				Worksheet worksheet = AddWorksheet(workbook, null, null, new StringBuilder().Append("(").Append(SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Module)).Append(")")
					.ToString());
				int currentRow = 0;
				Worker.DivideProgressStep(databasesExportData.Count);
				databasesExportData.ForEach(delegate(ModuleExportData databaseExportData)
				{
					StaticData.CrashedDatabaseType = (databaseExportData?.Module as DatabaseRow)?.Type;
					StaticData.CrashedDBMSVersion = (databaseExportData?.Module as DatabaseRow)?.DbmsVersion;
					databaseExportData.ModulesExportDataForExport.Select((ModuleExportData x) => x.Module).ToList().ForEach(delegate(DatabaseRowBase moduleRow)
					{
						int num;
						if (showDocumentationTitle)
						{
							ExcelExportTypePerSheet excelExportTypePerSheet = this;
							CellCollection cells = worksheet.Cells;
							num = currentRow;
							currentRow = num + 1;
							excelExportTypePerSheet.CellSetTextAndBold(cells[num, 0], databaseExportData.Module.Title);
						}
						ExcelExportTypePerSheet excelExportTypePerSheet2 = this;
						CellCollection cells2 = worksheet.Cells;
						num = currentRow;
						currentRow = num + 1;
						excelExportTypePerSheet2.CellSetTextAndBold(cells2[num, 0], moduleRow.Title);
						AddCustomFieldsToObjectHeader(worksheet, moduleRow, ref currentRow);
						ModuleRow moduleRow2 = moduleRow as ModuleRow;
						if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Erd)))
						{
							Diagram diagram = new Diagram();
							new DiagramManager(databaseExportData.Module.Id.Value, moduleRow.Id.Value, moduleRow2?.ErdShowTypes ?? false, LinkStyleEnum.LinkStyle.Straight, moduleRow2.DisplayDocumentationNameMode, diagram, null, null, notDeletedOnly: true, setLinks: false, formatted: false, forHtml: false, null, moduleRow2?.ErdShowNullable ?? false);
							Picture picture = worksheet.Pictures.AddPicture(diagram.ToImage(), worksheet.Cells[currentRow, 1]);
							currentRow = picture.BottomRightCell.BottomRowIndex + 2;
						}
					});
					Worker.IncreaseProgress();
				});
				StaticData.ClearDatabaseInfoForCrashes();
				Worker.RevertProgressStep();
			}
			else
			{
				Worker.IncreaseProgress();
			}
		}
		else
		{
			Worker.IncreaseProgress();
		}
	}

	private void ExportObjectList(IWorkbook workbook, bool showDocumentationTitle, List<ModuleExportData> databasesExportData, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.IsType(objectType)))
		{
			List<DocumentationObjectRow> list = null;
			if (objectType == SharedObjectTypeEnum.ObjectType.Table)
			{
				list = databasesExportData.SelectMany((ModuleExportData x) => from r in x.ExportData.TablesSource
					orderby r.DisplayName
					select r into y
					select new DocumentationObjectRow(x, y)).ToList();
			}
			else if (objectType == SharedObjectTypeEnum.ObjectType.View)
			{
				list = databasesExportData.SelectMany((ModuleExportData x) => from r in x.ExportData.ViewsSource
					orderby r.DisplayName
					select r into y
					select new DocumentationObjectRow(x, y)).ToList();
			}
			else if (objectType == SharedObjectTypeEnum.ObjectType.Procedure)
			{
				list = databasesExportData.SelectMany((ModuleExportData x) => from r in x.ExportData.ProceduresSource
					orderby r.DisplayName
					select r into y
					select new DocumentationObjectRow(x, y)).ToList();
			}
			else if (objectType == SharedObjectTypeEnum.ObjectType.Function)
			{
				list = databasesExportData.SelectMany((ModuleExportData x) => from r in x.ExportData.FunctionsSource
					orderby r.DisplayName
					select r into y
					select new DocumentationObjectRow(x, y)).ToList();
			}
			else if (objectType == SharedObjectTypeEnum.ObjectType.Structure)
			{
				list = databasesExportData.SelectMany((ModuleExportData x) => from r in x.ExportData.StructuresSource
					orderby r.DisplayName
					select r into y
					select new DocumentationObjectRow(x, y)).ToList();
			}
			if (list != null && list.Count > 0)
			{
				Worksheet worksheet = AddWorksheet(workbook, null, null, new StringBuilder().Append("(").Append(SharedObjectTypeEnum.TypeToStringForMenu(objectType)).Append(")")
					.ToString());
				int currentRow = 0;
				DataTable sourceTable = new DataTable();
				if (showDocumentationTitle)
				{
					sourceTable.Columns.Add("Documentation", typeof(string));
				}
				sourceTable.Columns.Add("Schema", typeof(string));
				sourceTable.Columns.Add("Name", typeof(string));
				sourceTable.Columns.Add("Title", typeof(string));
				AddCustomFieldsColumns(sourceTable, databasesExportData.First().Module.CustomFields, objectType);
				list.ToList().ForEach(delegate(DocumentationObjectRow objectRow)
				{
					StaticData.CrashedDatabaseType = (objectRow?.Documentation?.Module as DatabaseRow)?.Type;
					StaticData.CrashedDBMSVersion = (objectRow?.Documentation?.Module as DatabaseRow)?.DbmsVersion;
					AddValuesWithCustomFields(objectRow.ObjectRow.DocumentationTitle, showDocumentationTitle, sourceTable, objectRow.ObjectRow, objectType, objectRow.ObjectRow.Schema, objectRow.ObjectRow.Name, objectRow.ObjectRow.Title);
				});
				StaticData.ClearDatabaseInfoForCrashes();
				InsertFromDataTable(worksheet, sourceTable, ref currentRow);
			}
			else
			{
				Worker.IncreaseProgress();
			}
		}
		else
		{
			Worker.IncreaseProgress();
		}
	}

	private void ExportColumns(IWorkbook workbook, bool showDocumentationTitle, List<ModuleExportData> databasesExportData, CustomFieldsSupport customFieldsSupport)
	{
		List<DocumentationObjectRow> list = new List<DocumentationObjectRow>();
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Table)))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.TablesSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.View)))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.ViewsSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Structure)))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.StructuresSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (list.Count > 0)
		{
			DataTable dataTable = new DataTable();
			if (showDocumentationTitle)
			{
				dataTable.Columns.Add("Documentation", typeof(string));
			}
			dataTable.Columns.Add("Schema", typeof(string));
			dataTable.Columns.Add("Table/View", typeof(string));
			dataTable.Columns.Add("Type", typeof(string));
			dataTable.Columns.Add("#", typeof(int));
			dataTable.Columns.Add("Path", typeof(string));
			dataTable.Columns.Add("Column", typeof(string));
			dataTable.Columns.Add("Source", typeof(string));
			dataTable.Columns.Add("Title", typeof(string));
			dataTable.Columns.Add("Data type", typeof(string));
			dataTable.Columns.Add("Nullable", typeof(string));
			dataTable.Columns.Add("Default", typeof(string));
			dataTable.Columns.Add("Identity / Auto increment column", typeof(string));
			dataTable.Columns.Add("Computed", typeof(string));
			dataTable.Columns.Add("Computed formula", typeof(string));
			dataTable.Columns.Add("References", typeof(string));
			dataTable.Columns.Add("Description", typeof(string));
			AddCustomFieldsColumns(dataTable, databasesExportData.First().Module.CustomFields, SharedObjectTypeEnum.ObjectType.Column);
			Worker.DivideProgressStep(list.Count);
			foreach (DocumentationObjectRow item in list)
			{
				StaticData.CrashedDatabaseType = (item?.Documentation?.Module as DatabaseRow)?.Type;
				StaticData.CrashedDBMSVersion = (item?.Documentation?.Module as DatabaseRow)?.DbmsVersion;
				Worker.SetCurrentObject(item.ObjectRow.ObjectName);
				foreach (ColumnRow item2 in DB.Column.GetDataObjectByTableId(null, item.ObjectRow.ObjectId, notDeletedOnly: true, customFieldsSupport))
				{
					AddValuesWithCustomFields(item.ObjectRow.DocumentationTitle, showDocumentationTitle, dataTable, item2, SharedObjectTypeEnum.ObjectType.Column, item.ObjectRow.Schema, item.ObjectRow.Name, SharedObjectSubtypeEnum.TypeToStringForSingle(item.ObjectRow.Type, item.ObjectRow.Subtype), item2.Position, item2.Path, item2.Name, UserTypeEnum.TypeToString(item2.Source), item2.Title, item2.DataType, item2.Nullable, item2.DefaultValue, item2.IsIdentity, item2.IsComputed, item2.ComputedFormula, item2.ReferencesStringCommaDelimited, item2.Description);
				}
				Worker.IncreaseProgress();
			}
			StaticData.ClearDatabaseInfoForCrashes();
			InsertWoksheetIfNotEmpty(workbook, "Columns", dataTable, createHeader: false);
			Worker.SetCurrentObject(null);
			Worker.RevertProgressStep();
		}
		else
		{
			Worker.IncreaseProgress();
		}
	}

	private void ExportRelations(IWorkbook workbook, bool showDocumentationTitle, List<ModuleExportData> databasesExportData, CustomFieldsSupport customFieldsSupport)
	{
		List<DocumentationObjectRow> list = new List<DocumentationObjectRow>();
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Table && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Relation))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.TablesSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => (x.ObjectType == SharedObjectTypeEnum.ObjectType.View) & (x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Relation)))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.ViewsSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Structure && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Relation))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.StructuresSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (list.Count > 0)
		{
			DataTable dataTable = new DataTable();
			if (showDocumentationTitle)
			{
				dataTable.Columns.Add("Documentation", typeof(string));
			}
			dataTable.Columns.Add("Relationship name", typeof(string));
			dataTable.Columns.Add("Foreign database", typeof(string));
			dataTable.Columns.Add("Foreign table", typeof(string));
			dataTable.Columns.Add("Type", typeof(string));
			dataTable.Columns.Add("Primary database", typeof(string));
			dataTable.Columns.Add("Primary table", typeof(string));
			dataTable.Columns.Add("Title", typeof(string));
			dataTable.Columns.Add("Description", typeof(string));
			AddCustomFieldsColumns(dataTable, databasesExportData.First().Module.CustomFields, SharedObjectTypeEnum.ObjectType.Relation);
			HashSet<int> hashSet = new HashSet<int>();
			Worker.DivideProgressStep(list.Count);
			foreach (DocumentationObjectRow item in list)
			{
				StaticData.CrashedDatabaseType = (item?.Documentation?.Module as DatabaseRow)?.Type;
				StaticData.CrashedDBMSVersion = (item?.Documentation?.Module as DatabaseRow)?.DbmsVersion;
				Worker.SetCurrentObject(item.ObjectRow.ObjectName);
				bool num = !(item.Documentation.Module as DatabaseRow).Type.HasValue || !DatabaseSupportFactory.GetDatabaseSupport((item.Documentation.Module as DatabaseRow).Type).IsSchemaType;
				IEnumerable<RelationRow> enumerable = null;
				enumerable = ((!num) ? (from o in DB.Relation.GetDataByTableWithColumnsAndUniqueConstraints(item.ObjectRow.ObjectId, notDeletedOnly: true)
					select new RelationRow(o, isForXml: true, customFieldsSupport)
					{
						CurrentTableId = item.ObjectRow.ObjectId
					}) : ((item.Documentation.Module as DatabaseRow).Type.HasValue ? ((IEnumerable<RelationRow>)(from o in DB.Relation.GetDataByTableWithColumnsAndUniqueConstraints(item.ObjectRow.ObjectId, notDeletedOnly: true)
					select new OracleRelationRow(o, isForXml: true, customFieldsSupport)
					{
						CurrentTableId = item.ObjectRow.ObjectId
					})) : ((IEnumerable<RelationRow>)(from o in DB.Relation.GetDataByTableWithColumnsAndUniqueConstraints(item.ObjectRow.ObjectId, notDeletedOnly: true)
					select new OtherDatabaseRelationRow(o, isForXml: true, customFieldsSupport)
					{
						CurrentTableId = item.ObjectRow.ObjectId
					}))));
				enumerable = RelationsDB.GroupRelations(enumerable);
				foreach (RelationRow item2 in enumerable)
				{
					if (!hashSet.Contains(item2.Id))
					{
						AddValuesWithCustomFields(item.ObjectRow.DocumentationTitle, showDocumentationTitle, dataTable, item2, SharedObjectTypeEnum.ObjectType.Relation, item2.Name, item2.FKTableDatabaseName, item2.FKTableObjectNameWithoutServer, item2.CardinalityDescription, item2.PKTableDatabaseName, item2.PKTableObjectNameWithoutServer, item2.Title, item2.Description);
						hashSet.Add(item2.Id);
					}
				}
				Worker.IncreaseProgress();
			}
			StaticData.ClearDatabaseInfoForCrashes();
			InsertWoksheetIfNotEmpty(workbook, "Relationships", dataTable, createHeader: false);
			Worker.SetCurrentObject(null);
			Worker.RevertProgressStep();
		}
		else
		{
			Worker.IncreaseProgress();
		}
	}

	private void ExportRelationsColumns(IWorkbook workbook, bool showDocumentationTitle, List<ModuleExportData> databasesExportData, CustomFieldsSupport customFieldsSupport)
	{
		List<DocumentationObjectRow> list = new List<DocumentationObjectRow>();
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Table && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Relation))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.TablesSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => (x.ObjectType == SharedObjectTypeEnum.ObjectType.View) & (x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Relation)))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.ViewsSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Structure && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Relation))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.StructuresSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (list.Count > 0)
		{
			DataTable dataTable = new DataTable();
			if (showDocumentationTitle)
			{
				dataTable.Columns.Add("Documentation", typeof(string));
			}
			dataTable.Columns.Add("Relationship name", typeof(string));
			dataTable.Columns.Add("Foreign database", typeof(string));
			dataTable.Columns.Add("Foreign table", typeof(string));
			dataTable.Columns.Add("Type", typeof(string));
			dataTable.Columns.Add("Primary database", typeof(string));
			dataTable.Columns.Add("Primary table", typeof(string));
			dataTable.Columns.Add("Foreign table column path", typeof(string));
			dataTable.Columns.Add("Foreign table column", typeof(string));
			dataTable.Columns.Add("Primary table column path", typeof(string));
			dataTable.Columns.Add("Primary table column", typeof(string));
			dataTable.Columns.Add("Title", typeof(string));
			dataTable.Columns.Add("Description", typeof(string));
			HashSet<int> hashSet = new HashSet<int>();
			Worker.DivideProgressStep(list.Count);
			foreach (DocumentationObjectRow item in list)
			{
				StaticData.CrashedDatabaseType = (item?.Documentation?.Module as DatabaseRow)?.Type;
				StaticData.CrashedDBMSVersion = (item?.Documentation?.Module as DatabaseRow)?.DbmsVersion;
				Worker.SetCurrentObject(item.ObjectRow.ObjectName);
				bool num = !(item.Documentation.Module as DatabaseRow).Type.HasValue || !DatabaseSupportFactory.GetDatabaseSupport((item.Documentation.Module as DatabaseRow).Type).IsSchemaType;
				IEnumerable<RelationRow> enumerable = null;
				enumerable = ((!num) ? (from o in DB.Relation.GetDataByTableWithColumnsAndUniqueConstraints(item.ObjectRow.ObjectId, notDeletedOnly: true)
					select new RelationRow(o, isForXml: true, customFieldsSupport)
					{
						CurrentTableId = item.ObjectRow.ObjectId
					}) : ((item.Documentation.Module as DatabaseRow).Type.HasValue ? ((IEnumerable<RelationRow>)(from o in DB.Relation.GetDataByTableWithColumnsAndUniqueConstraints(item.ObjectRow.ObjectId, notDeletedOnly: true)
					select new OracleRelationRow(o, isForXml: true, customFieldsSupport)
					{
						CurrentTableId = item.ObjectRow.ObjectId
					})) : ((IEnumerable<RelationRow>)(from o in DB.Relation.GetDataByTableWithColumnsAndUniqueConstraints(item.ObjectRow.ObjectId, notDeletedOnly: true)
					select new OtherDatabaseRelationRow(o, isForXml: true, customFieldsSupport)
					{
						CurrentTableId = item.ObjectRow.ObjectId
					}))));
				enumerable = RelationsDB.GroupRelations(enumerable);
				foreach (RelationRow item2 in enumerable)
				{
					if (hashSet.Contains(item2.Id))
					{
						continue;
					}
					foreach (RelationColumnRow column in item2.Columns)
					{
						if (showDocumentationTitle)
						{
							dataTable.Rows.Add(item.ObjectRow.DocumentationTitle, item2.Name, item2.FKTableDatabaseName, item2.FKTableObjectNameWithoutServer, item2.CardinalityDescription, item2.PKTableDatabaseName, item2.PKTableObjectNameWithoutServer, column.ColumnFkPath, column.ColumnFkName, column.ColumnPkPath, column.ColumnPkName, item2.Title, item2.Description);
						}
						else
						{
							dataTable.Rows.Add(item2.Name, item2.FKTableDatabaseName, item2.FKTableObjectNameWithoutServer, item2.CardinalityDescription, item2.PKTableDatabaseName, item2.PKTableObjectNameWithoutServer, column.ColumnFkPath, column.ColumnFkName, column.ColumnPkPath, column.ColumnPkName, item2.Title, item2.Description);
						}
					}
					hashSet.Add(item2.Id);
				}
				Worker.IncreaseProgress();
			}
			StaticData.ClearDatabaseInfoForCrashes();
			InsertWoksheetIfNotEmpty(workbook, "Relationships - Columns", dataTable, createHeader: false);
			Worker.SetCurrentObject(null);
			Worker.RevertProgressStep();
		}
		else
		{
			Worker.IncreaseProgress();
		}
	}

	private void ExportUniqueKeys(IWorkbook workbook, bool showDocumentationTitle, List<ModuleExportData> databasesExportData, CustomFieldsSupport customFieldsSupport)
	{
		List<DocumentationObjectRow> list = new List<DocumentationObjectRow>();
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Table && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Key))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.TablesSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => (x.ObjectType == SharedObjectTypeEnum.ObjectType.View) & (x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Key)))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.ViewsSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Table && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Key))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.StructuresSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (list.Count > 0)
		{
			DataTable dataTable = new DataTable();
			if (showDocumentationTitle)
			{
				dataTable.Columns.Add("Documentation", typeof(string));
			}
			dataTable.Columns.Add("Schema", typeof(string));
			dataTable.Columns.Add("Table", typeof(string));
			dataTable.Columns.Add("Key name", typeof(string));
			dataTable.Columns.Add("Description", typeof(string));
			AddCustomFieldsColumns(dataTable, databasesExportData.First().Module.CustomFields, SharedObjectTypeEnum.ObjectType.Key);
			Worker.DivideProgressStep(list.Count);
			foreach (DocumentationObjectRow item in list)
			{
				StaticData.CrashedDatabaseType = (item?.Documentation?.Module as DatabaseRow)?.Type;
				StaticData.CrashedDBMSVersion = (item?.Documentation?.Module as DatabaseRow)?.DbmsVersion;
				Worker.SetCurrentObject(item.ObjectRow.ObjectName);
				foreach (UniqueConstraintRow item2 in from o in DB.Constraint.GetDataByTable(item.ObjectRow.ObjectId, notDeletedOnly: true)
					select new UniqueConstraintRow(o, customFieldsSupport))
				{
					AddValuesWithCustomFields(item.ObjectRow.DocumentationTitle, showDocumentationTitle, dataTable, item2, SharedObjectTypeEnum.ObjectType.Key, item.ObjectRow.Schema, item.ObjectRow.Name, item2.Name, item2.Description);
				}
				Worker.IncreaseProgress();
			}
			StaticData.ClearDatabaseInfoForCrashes();
			InsertWoksheetIfNotEmpty(workbook, "Unique keys", dataTable, createHeader: false);
			Worker.SetCurrentObject(null);
			Worker.RevertProgressStep();
		}
		else
		{
			Worker.IncreaseProgress();
		}
	}

	private void ExportUniqueKeysColumns(IWorkbook workbook, bool showDocumentationTitle, List<ModuleExportData> databasesExportData, CustomFieldsSupport customFieldsSupport)
	{
		List<DocumentationObjectRow> list = new List<DocumentationObjectRow>();
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Table && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Key))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.TablesSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => (x.ObjectType == SharedObjectTypeEnum.ObjectType.View) & (x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Key)))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.ViewsSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Table && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Key))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.StructuresSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (list.Count > 0)
		{
			DataTable dataTable = new DataTable();
			if (showDocumentationTitle)
			{
				dataTable.Columns.Add("Documentation", typeof(string));
			}
			dataTable.Columns.Add("Schema", typeof(string));
			dataTable.Columns.Add("Table", typeof(string));
			dataTable.Columns.Add("Key name", typeof(string));
			dataTable.Columns.Add("Path", typeof(string));
			dataTable.Columns.Add("Column", typeof(string));
			dataTable.Columns.Add("Description", typeof(string));
			Worker.DivideProgressStep(list.Count);
			foreach (DocumentationObjectRow item in list)
			{
				StaticData.CrashedDatabaseType = (item?.Documentation?.Module as DatabaseRow)?.Type;
				StaticData.CrashedDBMSVersion = (item?.Documentation?.Module as DatabaseRow)?.DbmsVersion;
				Worker.SetCurrentObject(item.ObjectRow.ObjectName);
				foreach (UniqueConstraintRow item2 in from o in DB.Constraint.GetDataByTableWithColumns(item.ObjectRow.ObjectId, notDeletedOnly: true)
					select new UniqueConstraintRow(o, customFieldsSupport))
				{
					foreach (UniqueConstraintColumnRow column in item2.Columns)
					{
						if (showDocumentationTitle)
						{
							dataTable.Rows.Add(item.ObjectRow.DocumentationTitle, item.ObjectRow.Schema, item.ObjectRow.Name, item2.Name, column.ColumnPath, column.ColumnName, item2.Description);
						}
						else
						{
							dataTable.Rows.Add(item.ObjectRow.Schema, item.ObjectRow.Name, item2.Name, column.ColumnPath, column.ColumnName, item2.Description);
						}
					}
				}
				Worker.IncreaseProgress();
			}
			StaticData.ClearDatabaseInfoForCrashes();
			InsertWoksheetIfNotEmpty(workbook, "Unique keys - Columns", dataTable, createHeader: false);
			Worker.SetCurrentObject(null);
			Worker.RevertProgressStep();
		}
		else
		{
			Worker.IncreaseProgress();
		}
	}

	private void ExportTriggers(IWorkbook workbook, bool showDocumentationTitle, List<ModuleExportData> databasesExportData, CustomFieldsSupport customFieldsSupport)
	{
		List<DocumentationObjectRow> list = new List<DocumentationObjectRow>();
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Table && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Trigger))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.TablesSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.View && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Trigger))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.ViewsSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Table && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Trigger))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.StructuresSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (list.Count > 0)
		{
			DataTable dataTable = new DataTable();
			if (showDocumentationTitle)
			{
				dataTable.Columns.Add("Documentation", typeof(string));
			}
			dataTable.Columns.Add("Schema", typeof(string));
			dataTable.Columns.Add("Table", typeof(string));
			dataTable.Columns.Add("Status", typeof(string));
			dataTable.Columns.Add("Trigger", typeof(string));
			dataTable.Columns.Add("When", typeof(string));
			dataTable.Columns.Add("Insert", typeof(string));
			dataTable.Columns.Add("Update", typeof(string));
			dataTable.Columns.Add("Delete", typeof(string));
			dataTable.Columns.Add("Description", typeof(string));
			AddCustomFieldsColumns(dataTable, databasesExportData.First().Module.CustomFields, SharedObjectTypeEnum.ObjectType.Trigger);
			Worker.DivideProgressStep(list.Count);
			foreach (DocumentationObjectRow item in list)
			{
				StaticData.CrashedDatabaseType = (item?.Documentation?.Module as DatabaseRow)?.Type;
				StaticData.CrashedDBMSVersion = (item?.Documentation?.Module as DatabaseRow)?.DbmsVersion;
				Worker.SetCurrentObject(item.ObjectRow.ObjectName);
				foreach (TriggerRow item2 in from o in DB.Trigger.GetDataByTable(item.ObjectRow.ObjectId, notDeletedOnly: true)
					select new TriggerRow(o, customFieldsSupport))
				{
					AddValuesWithCustomFields(item.ObjectRow.DocumentationTitle, showDocumentationTitle, dataTable, item2, SharedObjectTypeEnum.ObjectType.Trigger, item.ObjectRow.Schema, item.ObjectRow.Name, ObjectStatusEnum.StatusToLongString(item.ObjectRow.Status), item2.Name, item2.WhenRun, item2.OnInsert ? "Y" : string.Empty, item2.OnUpdate ? "Y" : string.Empty, item2.OnDelete ? "Y" : string.Empty, item2.Description);
				}
				Worker.IncreaseProgress();
			}
			StaticData.ClearDatabaseInfoForCrashes();
			InsertWoksheetIfNotEmpty(workbook, "Triggers", dataTable, createHeader: false);
			Worker.SetCurrentObject(null);
			Worker.RevertProgressStep();
		}
		else
		{
			Worker.IncreaseProgress();
		}
	}

	private void ExportInputOutput(IWorkbook workbook, bool showDocumentationTitle, List<ModuleExportData> databasesExportData, CustomFieldsSupport customFieldsSupport)
	{
		List<DocumentationObjectRow> list = new List<DocumentationObjectRow>();
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Procedure)))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.ProceduresSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Function)))
		{
			list.AddRange(databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.FunctionsSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))));
		}
		if (list.Count > 0)
		{
			DataTable dataTable = new DataTable();
			if (showDocumentationTitle)
			{
				dataTable.Columns.Add("Documentation", typeof(string));
			}
			dataTable.Columns.Add("Schema", typeof(string));
			dataTable.Columns.Add("Procedure/Function", typeof(string));
			dataTable.Columns.Add("Type", typeof(string));
			dataTable.Columns.Add("#", typeof(int));
			dataTable.Columns.Add("Parameter", typeof(string));
			dataTable.Columns.Add("Mode", typeof(string));
			dataTable.Columns.Add("Data type", typeof(string));
			dataTable.Columns.Add("Description", typeof(string));
			AddCustomFieldsColumns(dataTable, databasesExportData.First().Module.CustomFields, SharedObjectTypeEnum.ObjectType.Parameter);
			Worker.DivideProgressStep(list.Count);
			foreach (DocumentationObjectRow item in list)
			{
				StaticData.CrashedDatabaseType = (item?.Documentation?.Module as DatabaseRow)?.Type;
				StaticData.CrashedDBMSVersion = (item?.Documentation?.Module as DatabaseRow)?.DbmsVersion;
				Worker.SetCurrentObject(item.ObjectRow.ObjectName);
				foreach (ParameterRow item2 in from o in DB.Parameter.GetDataByProcedureId(item.ObjectRow.ObjectId, notDeletedOnly: true)
					select new ParameterRow(o, customFieldsSupport))
				{
					AddValuesWithCustomFields(item.ObjectRow.DocumentationTitle, showDocumentationTitle, dataTable, item2, SharedObjectTypeEnum.ObjectType.Parameter, item.ObjectRow.Schema, item.ObjectRow.Name, SharedObjectSubtypeEnum.TypeToStringForSingle(item.ObjectRow.Type, item.ObjectRow.Subtype), item2.Position, item2.Name, item2.ParameterMode, item2.DataType, item2.Description);
				}
				Worker.IncreaseProgress();
			}
			StaticData.ClearDatabaseInfoForCrashes();
			InsertWoksheetIfNotEmpty(workbook, "Input-Output", dataTable, createHeader: false);
			Worker.SetCurrentObject(null);
			Worker.RevertProgressStep();
		}
		else
		{
			Worker.IncreaseProgress();
		}
	}
}
