using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Forms.Helpers;
using Dataedo.App.ImportDescriptions.Tools;
using Dataedo.App.ImportDescriptions.Tools.Fields;
using Dataedo.App.UserControls;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;
using DevExpress.DataProcessing;

namespace Dataedo.App.ImportDescriptions.Processing.DataToImport;

public class ImportColumnsProcessor : ImportProcessorBase<ColumnImportDataModel>
{
	public ImportColumnsProcessor(int databaseId, List<FieldDefinition> fieldDefinitions, CustomGridUserControl dataGridView)
		: base(databaseId, fieldDefinitions, dataGridView, "column")
	{
		RegularGridColumns.Add("Column name", "ColumnName");
	}

	protected override bool PrepareRegularColumnField(ColumnImportDataModel rowModel, string fieldDataString, string columnFieldName)
	{
		bool flag = base.PrepareRegularColumnField(rowModel, fieldDataString, columnFieldName);
		if (!flag && columnFieldName == "ColumnName")
		{
			rowModel.ColumnName = ImportDataModel.ShortenedString(fieldDataString, 80);
			return true;
		}
		return flag;
	}

	public override string GetTooltipString(ImportDataModel row, string fieldName)
	{
		return GetTooltipStringBase(row, fieldName, "Column");
	}

	protected override bool ShouldCheckExisting(string fieldName)
	{
		if (!(fieldName == "Schema") && !(fieldName == "TableName"))
		{
			return fieldName == "ColumnName";
		}
		return true;
	}

	protected override void SetExistingData(ColumnImportDataModel model)
	{
		string columnName = model.ColumnName;
		if (string.IsNullOrWhiteSpace(columnName))
		{
			model.ClearData();
			return;
		}
		SharedObjectTypeEnum.ObjectType parentType;
		ColumnViewObject columnViewObject = TryFindColumn(model, columnName, null, out parentType);
		if (columnViewObject == null && columnName.Contains("."))
		{
			int num = columnName.LastIndexOf(".");
			string path = columnName.Substring(0, num);
			columnName = columnName.Substring(num + 1);
			columnViewObject = TryFindColumn(model, columnName, path, out parentType);
		}
		if (columnViewObject != null && ColumnNames.GetFullName(columnViewObject.Path, columnViewObject.Name) != model.ColumnName)
		{
			model.ClearData();
			return;
		}
		if (columnViewObject == null)
		{
			model.ClearData();
			return;
		}
		model.SetData(columnViewObject, base.SelectedCustomFields, parentType);
		base.DataGridView.RefreshData();
	}

	protected override void SetExistingData(BackgroundWorker backgroundWorker)
	{
		var enumerable = from x in base.Models
			group x by new { x.Schema, x.TableName };
		int num = 1;
		foreach (var item in enumerable)
		{
			try
			{
				if (backgroundWorker.CancellationPending)
				{
					break;
				}
				var key = item.Key;
				OverlayWithProgress.SetSubtitle("Getting tables data" + ((enumerable.Count() > 1) ? $" ({num++}/{enumerable.Count()})" : string.Empty));
				SharedObjectTypeEnum.ObjectType parentType;
				int? num2 = ImportTablesProcessor.TryFindTableId(base.DatabaseId, key.Schema, key.TableName, out parentType);
				if (!num2.HasValue)
				{
					item.ForEach(delegate(ColumnImportDataModel x)
					{
						x.ClearData();
					});
					continue;
				}
				Dictionary<string, ColumnObject> dictionary = DB.Column.GetDataByTable(num2.Value, notDeletedOnly: true).ToDictionary((ColumnObject x) => ColumnNames.GetFullName(x.Path, x.Name));
				if (!dictionary.Any())
				{
					continue;
				}
				foreach (ColumnImportDataModel item2 in item)
				{
					if (!string.IsNullOrWhiteSpace(item2.ColumnName))
					{
						if (backgroundWorker.CancellationPending)
						{
							break;
						}
						if (!dictionary.ContainsKey(item2.ColumnName))
						{
							item2.ClearData();
						}
						else
						{
							item2.SetData(dictionary[item2.ColumnName], base.SelectedCustomFields, parentType);
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}

	private ColumnViewObject TryFindColumn(ColumnImportDataModel model, string columnName, string path, out SharedObjectTypeEnum.ObjectType parentType)
	{
		parentType = SharedObjectTypeEnum.ObjectType.Table;
		ColumnViewObject dataByName = DB.Column.GetDataByName(base.DatabaseId, model.Schema, model.TableName, SharedObjectTypeEnum.TypeToString(parentType), columnName, path);
		if (dataByName == null)
		{
			parentType = SharedObjectTypeEnum.ObjectType.View;
			dataByName = DB.Column.GetDataByName(base.DatabaseId, model.Schema, model.TableName, SharedObjectTypeEnum.TypeToString(parentType), columnName, path);
		}
		if (dataByName == null)
		{
			parentType = SharedObjectTypeEnum.ObjectType.Structure;
			dataByName = DB.Column.GetDataByName(base.DatabaseId, model.Schema, model.TableName, SharedObjectTypeEnum.TypeToString(parentType), columnName, path);
		}
		return dataByName;
	}
}
