using System.Collections.Generic;
using System.Data.Common;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.DataProfiling.Enums;
using Dataedo.Data.Commands;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.DataProfiling;
using Dataedo.Model.Data.History;
using Dataedo.Shared.Enums;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.Data.MetadataServer;

internal class HistoryDB
{
	public const string TablesTableName = "tables";

	public const string ColumnsTableName = "columns";

	public const string TermsTableName = "glossary_terms";

	public const string ProceduresTableName = "procedures";

	public const string ParametersTableName = "parameters";

	public const string TriggersTableName = "triggers";

	public const string DatabaseTableName = "databases";

	public const string ModulesTableName = "modules";

	public const string TableRelationsTableName = "table_relations";

	public const string UniqueConstrainsTableName = "unique_constraints";

	public const string TitleField = "title";

	public const string DescriptionField = "description";

	public const string Product = "DESKTOP";

	public readonly CommandsSetBase commands;

	public ConfigurationKillerSwitchModel KillerSwitchMode { get; set; }

	public ConfigurationKillerSwitchEnum.EnabledNoSaveEnum? KillerSwitchModeEnum => ConfigurationKillerSwitchEnum.GetEnumValue(KillerSwitchMode?.Value);

	public bool SavingEnabled => KillerSwitchModeEnum == ConfigurationKillerSwitchEnum.EnabledNoSaveEnum.ENABLED;

	public HistoryDB()
	{
		commands = Dataedo.App.StaticData.Commands;
		SetSelectKillerSwitchMode();
	}

	public void SetSelectKillerSwitchMode()
	{
		if (commands.Select.History.CheckIsConfigurationTableExist())
		{
			KillerSwitchMode = commands.Select.History.GetConfigurationKillerSwitchModeHistory();
		}
	}

	public string SelectHtmlTextForHistoryRichEditControlByChangesHistoryId(int? changesHistoryId)
	{
		return DB.History.commands.Select.History.SelectHtmlTextForHistoryRichEditControlByChangesHistoryId(changesHistoryId);
	}

	public List<HistoryModel> SelectHistoryRows(int objectId, string columnName, string tableName)
	{
		if (HistoryGeneralHelper.IsWithHtmlDescription(columnName, tableName))
		{
			if (Dataedo.App.StaticData.IsProjectFile)
			{
				return DB.History.commands.Select.History.SelectHistoryValuesPlain(objectId, columnName, tableName);
			}
			return DB.History.commands.Select.History.SelectHistoryValuesPlainForServerRepo(objectId, columnName, tableName);
		}
		if (Dataedo.App.StaticData.IsProjectFile)
		{
			return DB.History.commands.Select.History.SelectHistoryValues(objectId, columnName, tableName);
		}
		return DB.History.commands.Select.History.SelectHistoryValuesForServerRepo(objectId, columnName, tableName);
	}

	internal ObjectWithTitleAndDescription SelectColumnTitleAndDescription(int objectId)
	{
		if (!SavingEnabled || objectId < 0)
		{
			return null;
		}
		return commands.Select.History.SelectColumnTitleAndDescription(objectId);
	}

	internal ObjectWithTitleAndHTMLDescriptionHistory SelectTableTitleAndHTMLDescription(int? objectId, SharedObjectTypeEnum.ObjectType? objectType)
	{
		if (!SavingEnabled || !objectId.HasValue || objectId < 0 || HistoryGeneralHelper.IsNotAcceptedType(objectType))
		{
			return null;
		}
		ObjectWithTitleAndHTMLDescriptionHistory objectWithTitleAndHTMLDescriptionHistory = commands.Select.History.SelectTableTitleAndDescription(objectId.Value);
		if (objectWithTitleAndHTMLDescriptionHistory == null)
		{
			return null;
		}
		objectWithTitleAndHTMLDescriptionHistory.Description = PrepareValue.GetHtmlText(objectWithTitleAndHTMLDescriptionHistory.DescriptionPlain, objectWithTitleAndHTMLDescriptionHistory.Description);
		return objectWithTitleAndHTMLDescriptionHistory;
	}

	public void InsertHistoryRow(int? databaseId, int? objectId, string title, string description, string descriptionPlain, string objectTable, bool saveTitle, bool saveDescription, SharedObjectTypeEnum.ObjectType? type)
	{
		if (!HistoryGeneralHelper.DontSaveHistory(databaseId, objectId, saveTitle || saveDescription, type))
		{
			int? userId = (Dataedo.App.StaticData.IsProjectFile ? null : new int?(Dataedo.App.StaticData.CurrentLicenseId));
			if (saveTitle)
			{
				HistoryModel historyModel = new HistoryModel(databaseId.Value, objectTable, "title", objectId.Value, title, null, userId, "DESKTOP", Dataedo.App.StaticData.ProductVersion);
				DB.History.commands.Manipulation.History.InsertHistoryRow(historyModel);
			}
			if (saveDescription)
			{
				HistoryModel historyModel2 = new HistoryModel(databaseId.Value, objectTable, "description", objectId.Value, description, descriptionPlain, userId, "DESKTOP", Dataedo.App.StaticData.ProductVersion);
				DB.History.commands.Manipulation.History.InsertHistoryRow(historyModel2);
			}
		}
	}

	public void InsertHistoryModel(HistoryModel historyModel)
	{
		DB.History.commands.Manipulation.History.InsertHistoryRow(historyModel);
	}

	public void InsertHistoryModels(HistoryModel[] historyModel, DbTransaction transaction = null)
	{
		DB.History.commands.Manipulation.History.InsertHistoryRows(historyModel, transaction);
	}

	public void InsertCommandToRepository(SqlCommand cmd)
	{
		commands.Manipulation.History.InsertCustomFieldsToHistory(cmd);
	}
}
