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
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.History;
using Dataedo.Shared.Enums;

namespace Dataedo.App.ImportDescriptions.Processing.Saving;

public class SaveTablesProcessor : SaveProcessorBase<TableImportDataModel>
{
	public SaveTablesProcessor(CustomFieldsSupport customFieldsSupport, IEnumerable<ImportDataModel> modelsGeneral)
		: base(customFieldsSupport)
	{
		base.Models = modelsGeneral.Cast<TableImportDataModel>().ToList();
	}

	public override bool ProcessSaving(int databaseId, Form owner)
	{
		IEnumerable<TableImportDataModel> enumerable = base.Models.Where((TableImportDataModel x) => x.IsAnySelected);
		if (!enumerable.Any())
		{
			return true;
		}
		OverlayWithProgress.Show("Saving data, please wait", owner);
		Dictionary<Table, SharedObjectTypeEnum.ObjectType> dictionary = new Dictionary<Table, SharedObjectTypeEnum.ObjectType>();
		OverlayWithProgress.SetSubtitle("Preparing data");
		foreach (TableImportDataModel item in enumerable)
		{
			string text = ((item.Title.IsSelected && item.Title.IsImported) ? item.Title.OverwriteValue : null);
			string text2 = ((item.Description.IsSelected && item.Description.IsImported) ? PrepareValue.FixDescription(item.Description.OverwriteValue) : null);
			ObjectWithTitleAndHTMLDescriptionHistory objectWithTitleAndHTMLDescriptionHistory = DB.History.SelectTableTitleAndHTMLDescription(item.TableId.Value, item.TableObjectType);
			HistoryCustomFieldsHelper.SaveCustomFieldsOnImportDescription(databaseId, item);
			bool flag = text != objectWithTitleAndHTMLDescriptionHistory?.Title;
			bool flag2 = text2 != objectWithTitleAndHTMLDescriptionHistory?.Description;
			flag = item.Title.IsSelected && item.Title.IsImported && flag;
			flag2 = item.Description.IsSelected && item.Description.IsImported && flag2;
			if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(objectWithTitleAndHTMLDescriptionHistory?.Title))
			{
				flag = false;
			}
			if (string.IsNullOrEmpty(text2) && string.IsNullOrEmpty(objectWithTitleAndHTMLDescriptionHistory?.Description))
			{
				flag2 = false;
			}
			Table table = new Table
			{
				Id = item.TableId.Value,
				Title = text,
				Description = text2,
				DescriptionPlain = ((item.Description.IsSelected && item.Description.IsImported) ? item.Description.OverwriteValue : null),
				IsTitleChangedHistory = flag,
				IsDescriptionChangedHistory = flag2
			};
			SetCustomFields(table, item);
			dictionary.Add(table, item.TableObjectType);
		}
		OverlayWithProgress.SetSubtitle("Updating data");
		OverlayWithProgress.SetProgressMax(dictionary.Count());
		if (DB.Table.UpdateTablesFromManualImport(dictionary.Keys.ToArray()))
		{
			OverlayWithProgress.SetSubtitle("Updating custom fields");
			OverlayWithProgress.SetProgressMax(dictionary.Count());
			List<HistoryModel> historyModelListToBulkInsert = new List<HistoryModel>();
			foreach (KeyValuePair<Table, SharedObjectTypeEnum.ObjectType> item2 in dictionary)
			{
				Table key = item2.Key;
				CustomFieldContainer customFieldContainer = new CustomFieldContainer(item2.Value, key.Id, base.CustomFieldsSupport);
				customFieldContainer.RetrieveCustomFields(key);
				customFieldContainer.UpdateCustomFieldDefinitionValues(DB.CustomField.UpdateCustomFieldValues);
				HistoryCreateHistoryModelHelper.ReturnCreatedHistoryModels(databaseId, historyModelListToBulkInsert, key?.Id, key?.Title, key?.Description, key?.DescriptionPlain, key.IsTitleChangedHistory, key.IsDescriptionChangedHistory, saveCustomfield: false, item2.Value, "tables", null);
				OverlayWithProgress.UpdateStatusProgress();
			}
			HistoryBulkInserterHelper.BulkInsertHistoryModels(historyModelListToBulkInsert);
			OverlayWithProgress.Close();
			base.CustomFieldsSupport.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: true);
			ShowSummaryMessage(owner);
			return true;
		}
		OverlayWithProgress.Close();
		return false;
	}
}
