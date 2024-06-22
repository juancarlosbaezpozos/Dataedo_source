using Dataedo.DataProcessing.Classes;
using Dataedo.DBTConnector.Models;
using Dataedo.Model.Data.DdlScript;
using Dataedo.Model.Data.DynamoDB;
using Dataedo.Model.Data.MongoDB;
using Dataedo.Model.Data.Neo4j;
using Dataedo.Model.Data.Salesforce;
using Dataedo.Model.Data.Snowlfake;
using Dataedo.Model.Data.SSAS;
using Dataedo.Model.Data.Tables.Constraints;
using Dataedo.Model.Data.UniqueConstraints;
using Dataedo.Model.Enums;
using Dataedo.Shared.Tools.ERD;
using Npgsql;

namespace Dataedo.App.Classes.Synchronize;

public class UniqueConstraintColumnRow : RelationConstraintColumnRow
{
	public int ConstraintId { get; set; }

	public int ColumnId { get; set; }

	public int? ColumnParentId { get; set; }

	public string ColumnName { get; set; }

	public string ColumnTitle { get; set; }

	public string ColumnPath { get; set; }

	public string ColumnFullName => ColumnNames.GetFullName(ColumnPath, ColumnName);

	public string ColumnFullNameFormatted => ColumnNames.GetFullNameFormatted(ColumnPath, ColumnName);

	public string ColumnFullNameFormattedWithTitle => ColumnNames.GetFullNameFormatted(ColumnPath, ColumnName, ColumnTitle);

	public bool IsDescend { get; set; }

	public bool IsChecked { get; set; }

	public bool IsDisabled { get; set; }

	public UniqueConstraintColumnRow()
	{
	}

	public UniqueConstraintColumnRow(NpgsqlDataReader dataReader)
	{
		ColumnName = PrepareValue.ToString(dataReader["column_name"]);
		base.OrdinalPosition = PrepareValue.ToInt(dataReader["column_ordinal"]);
	}

	public UniqueConstraintColumnRow(MongoDBConstraint dataReader)
	{
		ColumnName = dataReader.ColumnName;
		base.OrdinalPosition = dataReader.OrdinalPosition;
	}

	public UniqueConstraintColumnRow(Neo4jConstraint dataReader)
	{
		ColumnName = dataReader.ColumnName;
		base.OrdinalPosition = dataReader.OrdinalPosition;
	}

	public UniqueConstraintColumnRow(SsasConstraint dataReader)
	{
		ColumnName = dataReader.ColumnName;
		base.OrdinalPosition = dataReader.OrdinalPosition;
	}

	public UniqueConstraintColumnRow(UniqueConstraintBaseObject dataReader)
	{
		ColumnName = dataReader.ColumnName;
		base.OrdinalPosition = dataReader.OrdinalPosition;
	}

	public UniqueConstraintColumnRow(SalesforceConstraint dataReader)
	{
		ColumnName = dataReader.ColumnName;
		base.OrdinalPosition = dataReader.OrdinalPosition;
	}

	public UniqueConstraintColumnRow(DdlScriptConstraint dataReader)
	{
		ColumnName = dataReader.ColumnName;
		base.OrdinalPosition = dataReader.OrdinalPosition;
	}

	public UniqueConstraintColumnRow(UniqueConstraintDataverseWrapper dataReader)
	{
		if (dataReader.isAltKey)
		{
			ColumnName = dataReader.ColumnName;
			base.OrdinalPosition = dataReader.ColumnPosition;
		}
		else
		{
			ColumnName = dataReader.Column.LogicalName;
			base.OrdinalPosition = dataReader.Column.ColumnNumber;
		}
	}

	public UniqueConstraintColumnRow(DynamoDBConstraint dynamoDBDataReader)
	{
		ColumnName = dynamoDBDataReader.ColumnName;
		base.OrdinalPosition = dynamoDBDataReader.OrdinalPosition;
	}

	public UniqueConstraintColumnRow(UniqueColumnDbt uniqueColumnDbt)
	{
		ColumnName = uniqueColumnDbt.ColumnName;
		base.OrdinalPosition = 1;
	}

	public UniqueConstraintColumnRow(UniqueConstraintColumnRow other)
	{
		base.Id = other.Id;
		ConstraintId = other.ConstraintId;
		ColumnId = other.ColumnId;
		base.OrdinalPosition = other.OrdinalPosition;
		ColumnName = other.ColumnName;
		ColumnPath = other.ColumnPath;
		ColumnParentId = other.ColumnParentId;
		base.RowState = ManagingRowsEnum.ManagingRows.Unchanged;
	}

	public void InitializeData(UniqueConstraintColumnSynchronizationObject data)
	{
		base.OrdinalPosition = data.OrdinalPosition;
		ColumnName = data.ColumnName;
	}

	public void InitializeOracleData(UniqueConstraintColumnSynchronizationObjectForOracle data)
	{
		InitializeData(data);
		IsDescend = data.IsDescend;
		IsDisabled = data.IsDisabled;
	}

	public void InitializeInterfaceTablesData(UniqueConstraintColumnSynchronizationObjectForInterfaceTables data)
	{
		InitializeData(data);
		ColumnPath = data.ColumnPath;
	}

	public UniqueConstraintColumnRow(UniqueConstraintWithColumnObject row)
	{
		base.Id = row.UniqueConstraintColumnId ?? (-1);
		ConstraintId = row.UniqueConstraintId;
		ColumnId = row.ColumnId ?? (-1);
		base.OrdinalPosition = row.OrdinalPosition;
		ColumnName = row.ColumnName;
		ColumnTitle = row.ColumnTitle;
		ColumnPath = row.ColumnPath;
		ColumnParentId = row.ColumnParentId;
		base.RowState = ManagingRowsEnum.ManagingRows.Unchanged;
	}

	public UniqueConstraintColumnRow(SnowflakeConstraint dataReader)
	{
		ColumnName = dataReader.ColumnName;
		base.OrdinalPosition = dataReader.OrdinalPosition;
	}

	public UniqueConstraintColumnRow(int? maxOrdinalPosition)
	{
		ColumnId = -1;
		base.OrdinalPosition = ((!maxOrdinalPosition.HasValue) ? new int?(1) : (maxOrdinalPosition + 1));
		base.RowState = ManagingRowsEnum.ManagingRows.ForAdding;
	}

	public void Clear()
	{
		base.Id = -1;
	}
}
