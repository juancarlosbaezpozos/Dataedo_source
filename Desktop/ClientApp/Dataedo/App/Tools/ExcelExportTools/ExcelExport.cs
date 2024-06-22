using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.XmlExportTools.Model;
using Dataedo.Model.Data.Procedures.Parameters;
using Dataedo.Model.Data.Tables.Constraints;
using Dataedo.Model.Data.Tables.Relations;
using Dataedo.Model.Data.Tables.Triggers;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet;

namespace Dataedo.App.Tools.ExcelExportTools;

internal class ExcelExport : ExcelExportBase
{
	private class DocumentationModuleRow
	{
		public ModuleExportData Documentation { get; set; }

		public DatabaseRowBase ModuleRow { get; set; }

		public DocumentationModuleRow(ModuleExportData documentation, DatabaseRowBase moduleRow)
		{
			Documentation = documentation;
			ModuleRow = moduleRow;
		}
	}

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
		catch (Exception)
		{
			Worker.HasError = true;
			throw;
		}
	}

	private void ForEachDatabaseObjects(List<ModuleExportData> databasesExportData, Action<ModuleExportData> action)
	{
		Worker.DivideProgressStep(databasesExportData.Count);
		foreach (ModuleExportData databasesExportDatum in databasesExportData)
		{
			StaticData.CrashedDatabaseType = (databasesExportDatum?.Module as DatabaseRow)?.Type;
			StaticData.CrashedDBMSVersion = (databasesExportDatum?.Module as DatabaseRow)?.DbmsVersion;
			action(databasesExportDatum);
		}
		StaticData.ClearDatabaseInfoForCrashes();
		Worker.RevertProgressStep();
	}

	private void ExportDocumentations(IWorkbook workbook, List<ModuleExportData> databasesExportData, CustomFieldsSupport customFieldsSupport)
	{
		bool showDocumentationTitleInModules = databasesExportData.Count > 1;
		bool showDocumentationTitleInObjects = showDocumentationTitleInModules || databasesExportData.Any((ModuleExportData x) => x.IsAnyFromAnotherDocumentation == true);
		Worker.SetTotalProgressStepByPercent(95f);
		Worker.ReportProgress("Exporting objects...", 0);
		Worker.DivideProgressStep(5f);
		Worker.ReportProgress("Exporting objects: subject areas...");
		ForEachDatabaseObjects(databasesExportData, delegate(ModuleExportData databaseExportData)
		{
			ExportModules(workbook, databaseExportData, databaseExportData.ExportData.ModulesSource, showDocumentationTitleInModules);
		});
		Worker.ReportProgress("Exporting objects: tables...");
		ForEachDatabaseObjects(databasesExportData, delegate(ModuleExportData databaseExportData)
		{
			ExportTablesOrViews(workbook, databaseExportData, showDocumentationTitleInObjects, databaseExportData.ExportData.TablesSource, SharedObjectTypeEnum.ObjectType.Table, customFieldsSupport);
		});
		Worker.ReportProgress("Exporting objects: views...");
		ForEachDatabaseObjects(databasesExportData, delegate(ModuleExportData databaseExportData)
		{
			ExportTablesOrViews(workbook, databaseExportData, showDocumentationTitleInObjects, databaseExportData.ExportData.ViewsSource, SharedObjectTypeEnum.ObjectType.View, customFieldsSupport);
		});
		Worker.ReportProgress("Exporting objects: procedures...");
		ForEachDatabaseObjects(databasesExportData, delegate(ModuleExportData databaseExportData)
		{
			ExportProceduresOrFunctions(workbook, databaseExportData, showDocumentationTitleInObjects, databaseExportData.ExportData.ProceduresSource, SharedObjectTypeEnum.ObjectType.Procedure, customFieldsSupport);
		});
		Worker.ReportProgress("Exporting objects: functions...");
		ForEachDatabaseObjects(databasesExportData, delegate(ModuleExportData databaseExportData)
		{
			ExportProceduresOrFunctions(workbook, databaseExportData, showDocumentationTitleInObjects, databaseExportData.ExportData.FunctionsSource, SharedObjectTypeEnum.ObjectType.Function, customFieldsSupport);
		});
		Worker.ReportProgress("Exporting objects: structures...");
		ForEachDatabaseObjects(databasesExportData, delegate(ModuleExportData databaseExportData)
		{
			ExportTablesOrViews(workbook, databaseExportData, showDocumentationTitleInObjects, databaseExportData.ExportData.StructuresSource, SharedObjectTypeEnum.ObjectType.Structure, customFieldsSupport);
		});
		Worker.ReportProgress(100);
		Worker.RevertProgressStep();
		Worker.RevertTotalProgressStep();
		Worker.SetTotalProgressStepByPercent(5f);
		Worker.ReportProgress("Exporting documentation lists of objects...", 0);
		Worker.DivideProgressStep(4f);
		ExportModulesList(workbook, showDocumentationTitleInModules, databasesExportData);
		ExportObjectList(workbook, showDocumentationTitleInObjects, databasesExportData, SharedObjectTypeEnum.ObjectType.Table);
		ExportObjectList(workbook, showDocumentationTitleInObjects, databasesExportData, SharedObjectTypeEnum.ObjectType.View);
		ExportObjectList(workbook, showDocumentationTitleInObjects, databasesExportData, SharedObjectTypeEnum.ObjectType.Procedure);
		ExportObjectList(workbook, showDocumentationTitleInObjects, databasesExportData, SharedObjectTypeEnum.ObjectType.Function);
		ExportObjectList(workbook, showDocumentationTitleInObjects, databasesExportData, SharedObjectTypeEnum.ObjectType.Structure);
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
				DataTable sourceTable = new DataTable();
				if (showDocumentationTitle)
				{
					sourceTable.Columns.Add("Documentation", typeof(string));
				}
				sourceTable.Columns.Add("Title", typeof(string));
				AddCustomFieldsColumns(sourceTable, databasesExportData.First().Module.CustomFields, SharedObjectTypeEnum.ObjectType.Module);
				List<DocumentationModuleRow> addedRowsData = new List<DocumentationModuleRow>();
				databasesExportData.ForEach(delegate(ModuleExportData databaseExportData)
				{
					StaticData.CrashedDatabaseType = (databaseExportData?.Module as DatabaseRow)?.Type;
					StaticData.CrashedDBMSVersion = (databaseExportData?.Module as DatabaseRow)?.DbmsVersion;
					databaseExportData.ModulesExportDataForExport.Select((ModuleExportData x) => x.Module).ToList().ForEach(delegate(DatabaseRowBase moduleRow)
					{
						if (databaseExportData.ModulesExportDataForExport.Any((ModuleExportData x) => x.Module.Id == moduleRow.IdValue))
						{
							AddValuesWithCustomFields(databaseExportData.Module.Title, showDocumentationTitle, sourceTable, moduleRow, SharedObjectTypeEnum.ObjectType.Module, moduleRow.Title);
							addedRowsData.Add(new DocumentationModuleRow(databaseExportData, moduleRow));
						}
					});
				});
				StaticData.ClearDatabaseInfoForCrashes();
				CellRange cellRange = InsertFromDataTable(worksheet, sourceTable, ref currentRow);
				int columnOffset = (showDocumentationTitle ? 1 : 0);
				Worker.DivideProgressStep(addedRowsData.Count - 1);
				for (int i = 1; i < cellRange.RowCount; i++)
				{
					Cell cell = cellRange[i, columnOffset];
					DocumentationModuleRow currentAddedRowData = addedRowsData[i - 1];
					string displayText = cell.Value.ToString();
					SheetNameData sheetNameData = WorksheetNames.Where((SheetNameData x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Module && x.DocumentationId == currentAddedRowData.Documentation.Module.Id && x.ObjectId == currentAddedRowData.ModuleRow.Id).FirstOrDefault();
					if (sheetNameData != null)
					{
						worksheet.Hyperlinks.Add(cell, new StringBuilder().Append("'").Append(sheetNameData.SheetName).Append("'!A1")
							.ToString(), isExternal: false, displayText);
					}
					Worker.IncreaseProgress();
				}
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
				list = databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.TablesSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))).ToList();
			}
			else if (objectType == SharedObjectTypeEnum.ObjectType.View)
			{
				list = databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.ViewsSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))).ToList();
			}
			else if (objectType == SharedObjectTypeEnum.ObjectType.Procedure)
			{
				list = databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.ProceduresSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))).ToList();
			}
			else if (objectType == SharedObjectTypeEnum.ObjectType.Function)
			{
				list = databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.FunctionsSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))).ToList();
			}
			else if (objectType == SharedObjectTypeEnum.ObjectType.Structure)
			{
				list = databasesExportData.SelectMany((ModuleExportData x) => x.ExportData.StructuresSource.Select((ObjectRow y) => new DocumentationObjectRow(x, y))).ToList();
			}
			if (list != null && list.Count > 0)
			{
				Worksheet worksheet = AddWorksheet(workbook, null, null, new StringBuilder().Append("(").Append(SharedObjectTypeEnum.TypeToStringForMenu(objectType)).Append(")")
					.ToString());
				int currentRow = 0;
				DataTable dataTable = new DataTable();
				if (showDocumentationTitle)
				{
					dataTable.Columns.Add("Documentation", typeof(string));
				}
				dataTable.Columns.Add("Schema", typeof(string));
				dataTable.Columns.Add("Name", typeof(string));
				dataTable.Columns.Add("Title", typeof(string));
				AddCustomFieldsColumns(dataTable, databasesExportData.First().Module.CustomFields, objectType);
				for (int i = 0; i < list.Count; i++)
				{
					ObjectRow objectRow = list[i].ObjectRow;
					StaticData.CrashedDatabaseType = (list[i]?.Documentation?.Module as DatabaseRow)?.Type;
					StaticData.CrashedDBMSVersion = (list[i]?.Documentation?.Module as DatabaseRow)?.DbmsVersion;
					AddValuesWithCustomFields(objectRow.DocumentationTitle, showDocumentationTitle, dataTable, objectRow, objectType, objectRow.Schema, objectRow.Name, objectRow.Title);
				}
				StaticData.ClearDatabaseInfoForCrashes();
				CellRange cellRange = InsertFromDataTable(worksheet, dataTable, ref currentRow);
				int columnOffset = ((!showDocumentationTitle) ? 1 : 2);
				Worker.DivideProgressStep(cellRange.RowCount - 1);
				for (int j = 1; j < cellRange.RowCount; j++)
				{
					Cell cell = cellRange[j, columnOffset];
					DocumentationObjectRow currentAddedRowData = list[j - 1];
					string displayText = cell.Value.ToString();
					SheetNameData sheetNameData = WorksheetNames.Where((SheetNameData x) => x.ObjectType == objectType && x.DocumentationId == currentAddedRowData.ObjectRow.DatabaseId && x.ObjectId == currentAddedRowData.ObjectRow.ObjectId).FirstOrDefault();
					if (sheetNameData != null)
					{
						worksheet.Hyperlinks.Add(cell, new StringBuilder().Append("'").Append(sheetNameData.SheetName.Replace("'", "''")).Append("'!A1")
							.ToString(), isExternal: false, displayText);
					}
					Worker.IncreaseProgress();
				}
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

	private void ExportModules(IWorkbook workbook, ModuleExportData databaseExportData, List<ModuleRow> dataView, bool showDocumentationTitle)
	{
		if (databaseExportData?.ModulesExportDataForExport != null && databaseExportData.ModulesExportDataForExport.Count() > 0)
		{
			Worker.DivideProgressStep(databaseExportData.ModulesExportDataForExport.Count());
			foreach (ModuleRow item in databaseExportData.ModulesExportDataForExport.Select((ModuleExportData x) => x.Module as ModuleRow))
			{
				if (item != null)
				{
					Worker.SetCurrentObject(item.Title);
					Worksheet worksheet = AddWorksheet(workbook, databaseExportData.Module.Id, item.Id, "(Subject Area) " + item.Title, item.Title, SharedObjectTypeEnum.ObjectType.Module, startFromBeginning: false);
					int num = 0;
					int num2 = 0;
					int columnOffset = num2 + 1;
					CellSetTextAndBold(worksheet.Cells[num, num2], SharedObjectTypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Module) + ":");
					num += 2;
					if (showDocumentationTitle)
					{
						CellSetTextAndBold(worksheet.Cells[num, num2], "Documentation");
						worksheet.Cells[num, columnOffset].Value = databaseExportData.Module.Title;
						num++;
					}
					CellSetTextAndBold(worksheet.Cells[num, num2], "Title");
					worksheet.Cells[num, columnOffset].Value = item.Title;
					AddCustomFieldsToObjectHeader(worksheet, item, ref num);
					num++;
					if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Erd)))
					{
						Diagram diagram = new Diagram();
						new DiagramManager(databaseExportData.Module.Id.Value, item.Id.Value, item.ErdShowTypes, LinkStyleEnum.LinkStyle.Straight, item.DisplayDocumentationNameMode, diagram, null, databaseExportData, notDeletedOnly: true, setLinks: false, formatted: false, forHtml: false, null, item.ErdShowNullable);
						if (diagram.Elements.HasAnyNodes)
						{
							CellSetTextAndBold(worksheet.Cells[num++, num2], "Diagram:");
							worksheet.Pictures.AddPicture(diagram.ToImage(), worksheet.Cells[num, columnOffset]);
						}
					}
				}
				Worker.IncreaseProgress();
			}
			Worker.SetCurrentObject(null);
			Worker.RevertProgressStep();
		}
		else
		{
			Worker.IncreaseProgress();
		}
	}

	private void ExportTablesOrViews(IWorkbook workbook, ModuleExportData databaseExportData, bool showDocumentationTitle, List<ObjectRow> dataView, SharedObjectTypeEnum.ObjectType objectType, CustomFieldsSupport customFieldsSupport, ModuleExportData module = null)
	{
		if (dataView != null && dataView.Count > 0)
		{
			Worker.DivideProgressStep(dataView.Count);
			bool flag = !(databaseExportData.Module as DatabaseRow).Type.HasValue || !DatabaseSupportFactory.GetDatabaseSupport((databaseExportData.Module as DatabaseRow).Type).IsSchemaType;
			foreach (ObjectRow item in dataView)
			{
				Worker.SetCurrentObject(item.ObjectName);
				Worksheet worksheet = AddWorksheet(workbook, item);
				int currentRow = 0;
				CreateObjectHeader(worksheet, databaseExportData, showDocumentationTitle, item, ref currentRow);
				ExportColumns(worksheet, databaseExportData, item, ref currentRow, customFieldsSupport);
				IEnumerable<RelationRow> enumerable = null;
				if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.ObjectType == objectType && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Relation))
				{
					enumerable = ((!flag) ? (from o in DB.Relation.GetDataByTableWithColumnsAndUniqueConstraints(item.ObjectId, notDeletedOnly: true)
						select new RelationRow(o, isForXml: true, customFieldsSupport)
						{
							CurrentTableId = item.ObjectId
						}) : ((databaseExportData.Module as DatabaseRow).Type.HasValue ? ((IEnumerable<RelationRow>)(from o in DB.Relation.GetDataByTableWithColumnsAndUniqueConstraints(item.ObjectId, notDeletedOnly: true)
						select new OracleRelationRow(o, isForXml: true, customFieldsSupport)
						{
							CurrentTableId = item.ObjectId
						})) : ((IEnumerable<RelationRow>)(from o in DB.Relation.GetDataByTableWithColumnsAndUniqueConstraints(item.ObjectId, notDeletedOnly: true)
						select new OtherDatabaseRelationRow(o, isForXml: true, customFieldsSupport)
						{
							CurrentTableId = item.ObjectId
						}))));
					enumerable = RelationsDB.GroupRelations(enumerable);
					if (enumerable.Count() > 0)
					{
						CreateTableHeader(worksheet, "Relationships", ref currentRow);
						DataTable dataTable = new DataTable();
						dataTable.Columns.Add("#", typeof(int));
						dataTable.Columns.Add("Foreign database", typeof(string));
						dataTable.Columns.Add("Foreign table", typeof(string));
						dataTable.Columns.Add("Type", typeof(string));
						dataTable.Columns.Add("Primary database", typeof(string));
						dataTable.Columns.Add("Primary table", typeof(string));
						dataTable.Columns.Add("Join", typeof(string));
						dataTable.Columns.Add("Title", typeof(string));
						dataTable.Columns.Add("Relationship name", typeof(string));
						dataTable.Columns.Add("Description", typeof(string));
						AddCustomFieldsColumns(dataTable, databaseExportData.Module.CustomFields, SharedObjectTypeEnum.ObjectType.Relation);
						int num = 1;
						foreach (RelationRow item2 in enumerable.Where((RelationRow o) => o.PKTableId == item.ObjectId))
						{
							AddValuesWithCustomFields(dataTable, item2, SharedObjectTypeEnum.ObjectType.Relation, num++, item2.FKTableDatabaseName, item2.FKTableDisplayNameWithoutServer, item2.CardinalityDescription, item2.PKTableDatabaseName, item2.PKTableDisplayNameWithoutServer, GetJoinCondition(item, item2, databaseExportData, isRelationTo: true), item2.Title, item2.Name, item2.Description);
						}
						foreach (RelationRow item3 in enumerable.Where((RelationRow o) => o.FKTableId == item.ObjectId))
						{
							AddValuesWithCustomFields(dataTable, item3, SharedObjectTypeEnum.ObjectType.Relation, num++, item3.FKTableDatabaseName, item3.FKTableDisplayNameWithoutServer, item3.CardinalityDescription, item3.PKTableDatabaseName, item3.PKTableDisplayNameWithoutServer, GetJoinCondition(item, item3, databaseExportData, isRelationTo: false), item3.Title, item3.Name, item3.Description);
						}
						InsertFromDataTable(worksheet, dataTable, ref currentRow);
					}
				}
				if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.ObjectType == objectType && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Key))
				{
					IEnumerable<UniqueConstraintRow> uniqueConstraints = from o in DB.Constraint.GetDataByTableWithColumns(item.ObjectId, notDeletedOnly: true)
						select new UniqueConstraintRow(o, customFieldsSupport);
					uniqueConstraints = ConstraintDB.GroupUniqueConstraints(uniqueConstraints);
					if (uniqueConstraints.Count() > 0)
					{
						CreateTableHeader(worksheet, "Unique keys", ref currentRow);
						DataTable dataTable2 = new DataTable();
						dataTable2.Columns.Add("#", typeof(int));
						dataTable2.Columns.Add("Name", typeof(string));
						dataTable2.Columns.Add("Columns", typeof(string));
						dataTable2.Columns.Add("Description", typeof(string));
						AddCustomFieldsColumns(dataTable2, databaseExportData.Module.CustomFields, SharedObjectTypeEnum.ObjectType.Key);
						int num2 = 1;
						foreach (UniqueConstraintRow item4 in uniqueConstraints)
						{
							AddValuesWithCustomFields(dataTable2, item4, SharedObjectTypeEnum.ObjectType.Key, num2++, item4.Name, string.Join(", ", item4.Columns.Select((UniqueConstraintColumnRow c) => ColumnNames.GetFullName(c.ColumnPath, c.ColumnName))), item4.Description);
						}
						InsertFromDataTable(worksheet, dataTable2, ref currentRow);
					}
				}
				if (!ExcludedTypes.Any((ObjectTypeHierarchy x) => x.ObjectType == objectType && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Trigger))
				{
					List<TriggerObject> dataByTable = DB.Trigger.GetDataByTable(item.ObjectId, notDeletedOnly: true);
					if (dataByTable.Count > 0)
					{
						CreateTableHeader(worksheet, "Triggers", ref currentRow);
						DataTable dataTable3 = new DataTable();
						dataTable3.Columns.Add("#", typeof(int));
						dataTable3.Columns.Add("Name", typeof(string));
						dataTable3.Columns.Add("When", typeof(string));
						dataTable3.Columns.Add("Description", typeof(string));
						AddCustomFieldsColumns(dataTable3, databaseExportData.Module.CustomFields, SharedObjectTypeEnum.ObjectType.Trigger);
						int num3 = 1;
						foreach (TriggerObject item5 in dataByTable)
						{
							TriggerRow triggerRow = new TriggerRow(item5, customFieldsSupport);
							AddValuesWithCustomFields(dataTable3, triggerRow, SharedObjectTypeEnum.ObjectType.Trigger, num3++, triggerRow.Name, triggerRow.WhenRun, triggerRow.Description);
						}
						InsertFromDataTable(worksheet, dataTable3, ref currentRow);
					}
				}
				Worker.IncreaseProgress();
			}
			Worker.SetCurrentObject(null);
			Worker.RevertProgressStep();
		}
		else
		{
			Worker.IncreaseProgress();
		}
	}

	private void ExportColumns(Worksheet worksheet, ModuleExportData databaseExportData, ObjectRow table, ref int currentRow, CustomFieldsSupport customFieldsSupport)
	{
		BindingList<ColumnRow> dataObjectByTableId = DB.Column.GetDataObjectByTableId(null, table.ObjectId, notDeletedOnly: true, customFieldsSupport);
		if (DB.Column.GetDataByTable(table.ObjectId, notDeletedOnly: true).Count <= 0)
		{
			return;
		}
		CreateTableHeader(worksheet, "Columns", ref currentRow);
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Source", typeof(string));
		dataTable.Columns.Add("#", typeof(int));
		dataTable.Columns.Add("Path", typeof(string));
		dataTable.Columns.Add("Name", typeof(string));
		dataTable.Columns.Add("Title", typeof(string));
		dataTable.Columns.Add("Data type", typeof(string));
		dataTable.Columns.Add("Nullable", typeof(string));
		dataTable.Columns.Add("Default", typeof(string));
		dataTable.Columns.Add("Identity / Auto increment column", typeof(string));
		dataTable.Columns.Add("Computed", typeof(string));
		dataTable.Columns.Add("Computed formula", typeof(string));
		dataTable.Columns.Add("References", typeof(string));
		dataTable.Columns.Add("Description", typeof(string));
		AddCustomFieldsColumns(dataTable, databaseExportData.Module.CustomFields, SharedObjectTypeEnum.ObjectType.Column);
		int num = 1;
		foreach (ColumnRow item in dataObjectByTableId)
		{
			AddValuesWithCustomFields(dataTable, item, SharedObjectTypeEnum.ObjectType.Column, UserTypeEnum.TypeToString(item.Source), num++, item.Path, item.Name, item.Title, item.DataType, item.Nullable, item.DefaultValue, item.IsIdentity, item.IsComputed, item.ComputedFormula, item.ReferencesStringCommaDelimited, item.Description);
		}
		InsertFromDataTable(worksheet, dataTable, ref currentRow);
	}

	private void ExportProceduresOrFunctions(IWorkbook workbook, ModuleExportData databaseExportData, bool showDocumentationTitle, List<ObjectRow> dataView, SharedObjectTypeEnum.ObjectType objectType, CustomFieldsSupport customFieldsSupport, ModuleExportData moduleRow = null)
	{
		if (dataView != null && dataView.Count > 0)
		{
			Worker.DivideProgressStep(dataView.Count);
			foreach (ObjectRow item in dataView)
			{
				Worker.SetCurrentObject(item.ObjectName);
				Worksheet worksheet = AddWorksheet(workbook, item);
				int currentRow = 0;
				CreateObjectHeader(worksheet, databaseExportData, showDocumentationTitle, item, ref currentRow);
				List<ParameterObject> dataByProcedureId = DB.Parameter.GetDataByProcedureId(item.ObjectId, notDeletedOnly: true);
				if (dataByProcedureId.Count > 0)
				{
					CreateTableHeader(worksheet, "Input/Output", ref currentRow);
					DataTable dataTable = new DataTable();
					dataTable.Columns.Add("#", typeof(int));
					dataTable.Columns.Add("Mode", typeof(string));
					dataTable.Columns.Add("Name", typeof(string));
					dataTable.Columns.Add("Data type", typeof(string));
					dataTable.Columns.Add("Description", typeof(string));
					AddCustomFieldsColumns(dataTable, databaseExportData.Module.CustomFields, SharedObjectTypeEnum.ObjectType.Parameter);
					int num = 1;
					foreach (ParameterObject item2 in dataByProcedureId)
					{
						ParameterRow parameterRow = new ParameterRow(item2, customFieldsSupport);
						AddValuesWithCustomFields(dataTable, parameterRow, SharedObjectTypeEnum.ObjectType.Parameter, num++, parameterRow.ParameterMode, parameterRow.Name, parameterRow.DataType, parameterRow.Description);
					}
					InsertFromDataTable(worksheet, dataTable, ref currentRow);
				}
				Worker.IncreaseProgress();
			}
			Worker.SetCurrentObject(null);
			Worker.RevertProgressStep();
		}
		else
		{
			Worker.IncreaseProgress();
		}
	}
}
