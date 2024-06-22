using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Forms.Helpers;
using Dataedo.App.ImportDescriptions.Tools;
using Dataedo.App.Licences;
using Dataedo.App.Tools.CustomFields;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Model.Data.History;
using Dataedo.Shared.Enums;

namespace Dataedo.App.ImportDescriptions.Processing.Saving;

public class SaveColumnsProcessor : SaveProcessorBase<ColumnImportDataModel>
{
	public SaveColumnsProcessor(CustomFieldsSupport customFieldsSupport, IEnumerable<ImportDataModel> modelsGeneral)
		: base(customFieldsSupport)
	{
		base.Models = modelsGeneral.Cast<ColumnImportDataModel>().ToList();
	}

	public override bool ProcessSaving(int databaseId, Form owner)
	{
		IEnumerable<ColumnImportDataModel> source = base.Models.Where((ColumnImportDataModel x) => x.IsAnySelected);
		if (!source.Any())
		{
			return true;
		}
		OverlayWithProgress.Show("Saving data, please wait", owner);
		OverlayWithProgress.SetSubtitle("Preparing data");
		Column[] array = source.Select(delegate(ColumnImportDataModel x)
		{
			ObjectWithTitleAndDescription objectWithTitleAndDescription = DB.History.SelectColumnTitleAndDescription(x.ColumnId.Value);
			string text = ((x.Title.IsSelected && x.Title.IsImported) ? x.Title.OverwriteValue : null);
			string text2 = ((x.Description.IsSelected && x.Description.IsImported) ? x.Description.OverwriteValue : null);
			bool flag = HistoryGeneralHelper.CheckAreValuesDiffrent(objectWithTitleAndDescription?.Title, text);
			bool flag2 = HistoryGeneralHelper.CheckAreValuesDiffrent(objectWithTitleAndDescription?.Description, text2);
			flag = x.Title.IsSelected && x.Title.IsImported && flag;
			flag2 = x.Description.IsSelected && x.Description.IsImported && flag2;
			Column column = new Column
			{
				ColumnId = x.ColumnId.Value,
				Title = text,
				Description = text2,
				IsTitleChangedHistory = flag,
				IsDescriptionChangedHistory = flag2
			};
			SetCustomFields(column, x);
			return column;
		}).ToArray();
		OverlayWithProgress.SetSubtitle("Updating data");
		OverlayWithProgress.SetProgressMax(array.Count());
		if (DB.Column.UpdateColumnsFromManualImport(array, delegate
		{
			OverlayWithProgress.UpdateStatusProgress();
		}))
		{
			OverlayWithProgress.SetSubtitle("Updating custom fields");
			OverlayWithProgress.SetProgressMax(array.Count());
			InsertImportDescriptionHistory(databaseId, array);
			OverlayWithProgress.UpdateStatusProgress();
			OverlayWithProgress.Close();
			base.CustomFieldsSupport.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: true);
			ShowSummaryMessage(owner);
			return true;
		}
		OverlayWithProgress.Close();
		return false;
	}

	internal void InsertImportDescriptionHistory(int databaseId, Column[] columns)
	{
		List<HistoryModel> historyModelListToBulkInsert = new List<HistoryModel>();
		foreach (Column column in columns)
		{
			if (column?.CustomFields == null)
			{
				continue;
			}
			foreach (KeyValuePair<string, BaseWithCustomFields.CustomFieldWithValue> item in column?.CustomFields)
			{
				HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, column?.ColumnId, null, item.Value.Value, null, saveTitle: false, saveDescription: false, saveCustomfield: true, SharedObjectTypeEnum.ObjectType.Column, "columns", item.Key);
			}
			DB.History.InsertHistoryRow(databaseId, column?.ColumnId, column?.Title, column?.Description, null, "columns", column.IsTitleChangedHistory, column.IsDescriptionChangedHistory, SharedObjectTypeEnum.ObjectType.Table);
		}
		HistoryBulkInserterHelper.BulkInsertHistoryModels(historyModelListToBulkInsert);
	}
}
