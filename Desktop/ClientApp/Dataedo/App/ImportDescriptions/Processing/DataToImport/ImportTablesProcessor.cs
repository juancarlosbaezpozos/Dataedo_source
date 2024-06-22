using System.Collections.Generic;
using System.ComponentModel;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.ImportDescriptions.Tools;
using Dataedo.App.ImportDescriptions.Tools.Fields;
using Dataedo.App.UserControls;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Shared.Enums;

namespace Dataedo.App.ImportDescriptions.Processing.DataToImport;

public class ImportTablesProcessor : ImportProcessorBase<TableImportDataModel>
{
	public ImportTablesProcessor(int databaseId, List<FieldDefinition> fieldDefinitions, CustomGridUserControl dataGridView)
		: base(databaseId, fieldDefinitions, dataGridView, "table")
	{
	}

	public override string GetTooltipString(ImportDataModel row, string fieldName)
	{
		return GetTooltipStringBase(row, fieldName, "Table");
	}

	protected override bool ShouldCheckExisting(string fieldName)
	{
		if (!(fieldName == "Schema"))
		{
			return fieldName == "TableName";
		}
		return true;
	}

	protected override void SetExistingData(TableImportDataModel model)
	{
		SetData(model);
	}

	protected override void SetExistingData(BackgroundWorker backgroundWorker)
	{
		foreach (TableImportDataModel model in base.Models)
		{
			if (backgroundWorker.CancellationPending)
			{
				break;
			}
			SetData(model);
		}
	}

	private void SetData(TableImportDataModel model)
	{
		SharedObjectTypeEnum.ObjectType parentType;
		TableObject tableObject = TryFindTable(base.DatabaseId, model.Schema, model.TableName, out parentType);
		if (tableObject == null)
		{
			model.ClearData();
			return;
		}
		model.TableId = tableObject.Id;
		model.TableName = tableObject.Name;
		model.Title.CurrentValue = tableObject.Title;
		model.Description.CurrentValue = tableObject.DescriptionPlain;
		model.SetCustomFields(base.SelectedCustomFields, tableObject);
		model.TableObjectType = parentType;
		base.DataGridView.RefreshData();
	}

	public static TableObject TryFindTable(int databaseId, string schema, string tableName, out SharedObjectTypeEnum.ObjectType parentType)
	{
		parentType = SharedObjectTypeEnum.ObjectType.Table;
		TableObject dataByName = DB.Table.GetDataByName(databaseId, schema, tableName, SharedObjectTypeEnum.TypeToString(parentType));
		if (dataByName == null)
		{
			parentType = SharedObjectTypeEnum.ObjectType.View;
			dataByName = DB.Table.GetDataByName(databaseId, schema, tableName, SharedObjectTypeEnum.TypeToString(parentType));
		}
		if (dataByName == null)
		{
			parentType = SharedObjectTypeEnum.ObjectType.Structure;
			dataByName = DB.Table.GetDataByName(databaseId, schema, tableName, SharedObjectTypeEnum.TypeToString(parentType));
		}
		return dataByName;
	}

	public static int? TryFindTableId(int databaseId, string schema, string tableName, out SharedObjectTypeEnum.ObjectType parentType)
	{
		parentType = SharedObjectTypeEnum.ObjectType.Table;
		int? objectIdByName = DB.Table.GetObjectIdByName(databaseId, schema, tableName, SharedObjectTypeEnum.TypeToString(parentType));
		if (!objectIdByName.HasValue)
		{
			parentType = SharedObjectTypeEnum.ObjectType.View;
			objectIdByName = DB.Table.GetObjectIdByName(databaseId, schema, tableName, SharedObjectTypeEnum.TypeToString(parentType));
		}
		if (!objectIdByName.HasValue)
		{
			parentType = SharedObjectTypeEnum.ObjectType.Structure;
			objectIdByName = DB.Table.GetObjectIdByName(databaseId, schema, tableName, SharedObjectTypeEnum.TypeToString(parentType));
		}
		return objectIdByName;
	}
}
