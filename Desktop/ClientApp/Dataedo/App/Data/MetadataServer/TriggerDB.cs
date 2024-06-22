using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.FeedbackWidgetData;
using Dataedo.Model.Data.Tables.Triggers;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer;

internal class TriggerDB : CommonDBSupport
{
	public TriggerDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public List<TriggerObject> GetDataByTable(int tableId, bool notDeletedOnly = false)
	{
		return commands.Select.Triggers.GetTriggers(tableId, GetNotStatusValue(notDeletedOnly));
	}

	public FeedbackWidgetDataObject GetTriggerFeedbackWidgetData(int triggerId)
	{
		return commands.Select.Triggers.GetTriggerFeedbackWidgetData(triggerId);
	}

	public BindingList<TriggerRow> GetDataObjectByTableId(int tableId, CustomFieldsSupport customFieldsSupport = null)
	{
		return new BindingList<TriggerRow>((from TriggerObject trigger in GetDataByTable(tableId)
			select new TriggerRow(trigger, customFieldsSupport)).ToList());
	}

	public int Synchronize(IEnumerable<TriggerRow> triggers, string tableName, string schema, int tableId, int databaseId, bool isDbAdded, int updateId, CustomFieldsSupport customFieldsSupport, Form owner = null)
	{
		try
		{
			HistoryImportChangesHelper.CheckTriggerChangesInTriggerDB(triggers, databaseId, tableId, isDbAdded, customFieldsSupport);
			int result = commands.Synchronization.Triggers.SynchronizeTriggers(triggers.Select((TriggerRow r) => ConvertRowToSynchronizationItem(r, tableName, schema, databaseId, updateId)).ToArray(), tableName, schema, databaseId, isDbAdded, updateId);
			HistoryImportChangesHelper.InsertHistoryRowsInTriggerDB(triggers, databaseId, tableId, isDbAdded);
			return result;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while synchronizing the triggers:", owner);
			return -1;
		}
	}

	private TriggerForSynchronization ConvertRowToSynchronizationItem(TriggerRow row, string tableName, string schema, int databaseId, int updateId)
	{
		TriggerForSynchronization triggerForSynchronization = new TriggerForSynchronization
		{
			BaseName = tableName,
			BaseSchema = schema,
			BaseId = databaseId,
			Name = row.Name,
			Before = row.Before,
			After = row.After,
			InsteadOf = row.InsteadOf,
			OnInsert = row.OnInsert,
			OnUpdate = row.OnUpdate,
			OnDelete = row.OnDelete,
			Disabled = row.Disabled,
			Definition = row.Definition,
			Description = row.Description,
			UpdateId = updateId,
			Subtype = SharedObjectSubtypeEnum.TypeToString(row.ObjectType, row.Subtype)
		};
		SetCustomFields(triggerForSynchronization, row);
		return triggerForSynchronization;
	}

	public bool Update(List<TriggerRow> rows, Form owner = null)
	{
		if (rows != null)
		{
			try
			{
				commands.Manipulation.Triggers.UpdateTriggers(rows.Select((TriggerRow x) => ConvertRowToItem(x)).ToArray());
				CustomFieldContainer.SaveValuesForDefinition(rows.Select((TriggerRow x) => x.CustomFields), DB.CustomField.UpdateCustomFieldValues);
			}
			catch (Exception exception)
			{
				GeneralExceptionHandling.Handle(exception, "Error while updating columns:", owner);
				return false;
			}
		}
		return true;
	}

	private Trigger ConvertRowToItem(TriggerRow row)
	{
		Trigger trigger = new Trigger
		{
			TriggerId = row.Id,
			Title = row.Title,
			Description = row.Description
		};
		SetCustomFields(trigger, row);
		return trigger;
	}

	public bool Delete(BindingList<int> triggers, Form owner = null)
	{
		try
		{
			commands.Manipulation.Triggers.DeleteTriggers(triggers.ToArray(), Dataedo.App.StaticData.IsProjectFile);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating triggers:", owner);
			return false;
		}
		return true;
	}
}
