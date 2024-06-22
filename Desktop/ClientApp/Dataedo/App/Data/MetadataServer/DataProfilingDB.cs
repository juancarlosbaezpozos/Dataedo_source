using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.DataProfiling;
using Dataedo.App.DataProfiling.Enums;
using Dataedo.App.DataProfiling.Models;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Tools;
using Dataedo.Data.Commands;
using Dataedo.DataSources.Base.DataProfiling.Model;
using Dataedo.Model.Data.DataProfiling;
using DevExpress.XtraSplashScreen;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.Data.MetadataServer;

internal class DataProfilingDB
{
	private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

	private CommandsSetBase commands;

	public DataProfilingForm DataProfiling { get; set; }

	public ConfigurationKillerSwitchModel KillerSwitchMode { get; set; }

	public ConfigurationKillerSwitchEnum.EnabledNoSaveEnum? KillerSwitchModeEnum => ConfigurationKillerSwitchEnum.GetEnumValue(KillerSwitchMode?.Value);

	public bool ProfilingEnabled => KillerSwitchModeEnum == ConfigurationKillerSwitchEnum.EnabledNoSaveEnum.ENABLED;

	public bool ProfilingNoSave => KillerSwitchModeEnum == ConfigurationKillerSwitchEnum.EnabledNoSaveEnum.NOSAVE;

	public DataProfilingDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public void SetSelectKillerSwitchMode()
	{
		commands = Dataedo.App.StaticData.Commands;
		if (commands.Select.DataProfiling.CheckIsConfigurationTableExist() == true)
		{
			KillerSwitchMode = commands.Select.DataProfiling.GetConfigurationKillerSwitchMode();
		}
		else
		{
			KillerSwitchMode = null;
		}
	}

	public bool IsDataProfilingDisabled()
	{
		if (KillerSwitchModeEnum != ConfigurationKillerSwitchEnum.EnabledNoSaveEnum.ENABLED)
		{
			return KillerSwitchModeEnum != ConfigurationKillerSwitchEnum.EnabledNoSaveEnum.NOSAVE;
		}
		return false;
	}

	public List<ColumnProfiledDataObject> SelectColumnsProfilingData(int tableId)
	{
		return commands.Select.DataProfiling.GetColumnsProfilingData(tableId);
	}

	public ColumnProfiledDataObject SelectOneColumnProfilingData(int tableId, int columnId)
	{
		return commands.Select.DataProfiling.GetOneColumnProfilingData(tableId, columnId);
	}

	public List<ColumnValuesDataObject> SelectColumnValuesDataObjectForColumn(int tableId, int columnId)
	{
		return commands.Select.DataProfiling.GetListOfValues(tableId, columnId);
	}

	public List<ColumnValuesDataObject> SelectColumnValuesDataObjectForTable(int tableId)
	{
		return commands.Select.DataProfiling.GetListOfValuesForTable(tableId);
	}

	public TableStatsRowCountDataObject SelectTableStats(int tableId)
	{
		return commands.Select.DataProfiling.GetTableStatsData(tableId).FirstOrDefault();
	}

	public List<TableStatsRowCountDataObject> GetTablesStatsForDatabase(int databaseId)
	{
		return commands.Select.DataProfiling.GetTableStatsDataForDatabase(databaseId);
	}

	public void RemoveAllProfilingForSingleColumn(int tableId, int columnId, SplashScreenManager splashScreenManager, Form owner = null)
	{
		CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
		try
		{
			bool flag = false;
			DataProfilingForm dataProfiling = DataProfiling;
			int num;
			if (dataProfiling == null)
			{
				num = 0;
			}
			else
			{
				num = (dataProfiling.IsTableInCurrentProfilingSession(tableId) ? 1 : 0);
				if (num != 0)
				{
					flag = DataProfiling.ClearAllColumnProfilingAsync(columnId).GetAwaiter().GetResult();
				}
			}
			if (num == 0 || !flag)
			{
				ColumnProfiledDataObject columnProfiledDataObject = DB.DataProfiling.SelectOneColumnProfilingData(tableId, columnId);
				if (columnProfiledDataObject != null)
				{
					RemoveAllProfilingForSingleColumn(columnProfiledDataObject);
				}
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while clearing column profiling data", owner);
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	public void RemoveAllProfilingForSingleTable(int tableId, SplashScreenManager splashScreenManager, Form owner = null)
	{
		CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
		try
		{
			DB.DataProfiling.RemoveTableRowCountFromRepository(tableId);
			bool flag = false;
			DataProfilingForm dataProfiling = DataProfiling;
			int num;
			if (dataProfiling == null)
			{
				num = 0;
			}
			else
			{
				num = (dataProfiling.IsTableInCurrentProfilingSession(tableId) ? 1 : 0);
				if (num != 0)
				{
					flag = DataProfiling.ClearAllTableProfilingAsync(tableId).GetAwaiter().GetResult();
				}
			}
			if (num != 0 && flag)
			{
				return;
			}
			List<ColumnProfiledDataObject> list = DB.DataProfiling.SelectColumnsProfilingData(tableId);
			if (list == null || list.Count == 0)
			{
				return;
			}
			foreach (ColumnProfiledDataObject item in list)
			{
				RemoveAllProfilingForSingleColumn(item);
			}
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			GeneralExceptionHandling.Handle(exception, "Error while clearing table profiling data", owner);
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	public void RemoveAllProfilingForSingleColumn(ColumnProfiledDataObject column)
	{
		column.NullAllProfilingData();
		DeleteColumnListValues(column.ColumnId);
		commands.Manipulation.DataProfiling.UpdateProfilingData(column);
	}

	public void RemoveDistributionProfilingForSingleColumn(ColumnProfiledDataObject column)
	{
		column.NullDistributionSectionOfProfilingData();
		if (column.ThereIsNoProfiling())
		{
			column.ProfilingDate = null;
			column.ProfilingDataType = null;
			column.RowCount = null;
			commands.Manipulation.DataProfiling.UpdateOnlyRowDataInProfilingData(column);
		}
		else
		{
			commands.Manipulation.DataProfiling.ClearOnlyRowDataInProfilingData(column);
		}
	}

	public void RemoveValueSectionProfilingForSingleColumn(ColumnProfiledDataObject column)
	{
		column.NullValueSectionOfProfilingData();
		DeleteColumnListValues(column.ColumnId);
		if (column.ThereIsNoProfiling())
		{
			column.ProfilingDate = null;
			column.ProfilingDataType = null;
			column.RowCount = null;
			commands.Manipulation.DataProfiling.UpdateOnlyValueSectionDataInProfilingData(column);
		}
		else
		{
			commands.Manipulation.DataProfiling.ClearOnlyValueSectionInProfilingData(column);
		}
	}

	public void DeleteColumnListValues(int columnId)
	{
		commands.Manipulation.DataProfiling.DeleteValuesList(columnId);
	}

	public void RemoveTableRowCountFromRepository(int tableId)
	{
		commands.Manipulation.DataProfiling.DeleteRowCountsTableStats(tableId);
		commands.Manipulation.DataProfiling.ClearTableRowsStats(tableId);
	}

	public void SaveColumnProfilingRowDistribution(ColumnProfiledDataObject column)
	{
		commands.Manipulation.DataProfiling.UpdateOnlyRowDataInProfilingData(column);
	}

	public void SaveColumnProfilingValueSection(ColumnProfiledDataObject column)
	{
		commands.Manipulation.DataProfiling.UpdateOnlyValueSectionDataInProfilingData(column);
	}

	public void SaveColumnAllProfiling(ColumnProfiledDataObject column)
	{
		int maxLength = 250;
		column.ValueMinString = Truncate(column.ValueMinString, maxLength);
		column.ValueMaxString = Truncate(column.ValueMaxString, maxLength);
		commands.Manipulation.DataProfiling.UpdateProfilingData(column);
	}

	public void SaveColumnValues(ColumnNavigationObject column)
	{
		DeleteColumnListValues(column.ColumnId);
		InsertProperListOfValuesToRepository(column, Dataedo.App.StaticData.IsProjectFile);
		commands.Manipulation.DataProfiling.UpdateOnlyValuesListDataInProfilingData(column.Column);
	}

	public void SaveTableRowCountToRepository(int tableId, long? rowCount)
	{
		commands.Manipulation.DataProfiling.DeleteRowCountsTableStats(tableId);
		TableStatsRowCountDataObject tableStatsRowCountDataObject = new TableStatsRowCountDataObject(tableId, "Profiling", rowCount);
		commands.Manipulation.DataProfiling.InsertTablesStatsRow(tableStatsRowCountDataObject);
		commands.Manipulation.DataProfiling.UpdateTableRowsStats(tableStatsRowCountDataObject);
	}

	private void InsertProperListOfValuesToRepository(ColumnNavigationObject columnNavigation, bool isProjectFile)
	{
		if (columnNavigation.ValuesListMode == "S")
		{
			if (isProjectFile)
			{
				InsertRandomValuesToFileRepo(columnNavigation);
			}
			else
			{
				InsertRandomValues(columnNavigation);
			}
		}
		else if (isProjectFile)
		{
			InsertMostPopularValuesToFileRepo(columnNavigation);
		}
		else
		{
			InsertMostPopularValues(columnNavigation);
		}
	}

	private void InsertMostPopularValuesToFileRepo(ColumnNavigationObject columnNavigation)
	{
		if (columnNavigation?.FieldMostCommonlyUsedValues == null)
		{
			return;
		}
		foreach (StringValueWithCount fieldMostCommonlyUsedValue in columnNavigation.FieldMostCommonlyUsedValues)
		{
			string text = "NULL";
			if (fieldMostCommonlyUsedValue.Value != null)
			{
				text = fieldMostCommonlyUsedValue.Value.ToString();
				if (text.Length > 250)
				{
					text = Truncate(text, 250);
				}
			}
			int? rowCount = null;
			if (fieldMostCommonlyUsedValue != null)
			{
				rowCount = fieldMostCommonlyUsedValue.Count;
			}
			ColumnValuesDataObject columnValue = new ColumnValuesDataObject(columnNavigation.ColumnId, text, rowCount, columnNavigation.ValuesListMode, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			commands.Manipulation.DataProfiling.InsertColumnValuesCmdToFileRepo(columnValue);
		}
	}

	private void InsertRandomValuesToFileRepo(ColumnNavigationObject columnNavigation)
	{
		if (columnNavigation?.ObjectSampleData == null)
		{
			return;
		}
		object[][] values = columnNavigation.ObjectSampleData.Values;
		foreach (object[] array in values)
		{
			string text;
			if (array[0] != null)
			{
				text = array[0].ToString();
				if (text.Length > 250)
				{
					text = Truncate(text, 250);
				}
			}
			else
			{
				text = "NULL";
			}
			ColumnValuesDataObject columnValue = new ColumnValuesDataObject(columnNavigation.ColumnId, text, null, columnNavigation.ValuesListMode, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			commands.Manipulation.DataProfiling.InsertColumnValuesCmdToFileRepo(columnValue);
		}
	}

	private void InsertMostPopularValues(ColumnNavigationObject columnNavigation)
	{
		if (columnNavigation?.FieldMostCommonlyUsedValues == null)
		{
			return;
		}
		int num = 0;
		int num2 = 69000;
		SqlCommand sqlCommand = new SqlCommand();
		foreach (StringValueWithCount fieldMostCommonlyUsedValue in columnNavigation.FieldMostCommonlyUsedValues)
		{
			sqlCommand.CommandText += $"INSERT INTO [column_values] (\r\n                        [column_id],\r\n                        [value],\r\n                        [type],\r\n                        [row_count],\r\n                        [created_by],\r\n                        [modified_by],\r\n                        [creation_date],\r\n                        [last_modification_date])\r\n                    VALUES (\r\n                        @val{num}val1,\r\n                        @val{num}val2,\r\n                        @val{num}val3,\r\n                        @val{num}val4,\r\n                        @val{num}val5,\r\n                        @val{num}val6,\r\n                        @val{num}val7,\r\n                        @val{num}val8);";
			sqlCommand.Parameters.Add($"@val{num}val1", SqlDbType.Int);
			sqlCommand.Parameters.Add($"@val{num}val2", SqlDbType.NVarChar);
			sqlCommand.Parameters.Add($"@val{num}val3", SqlDbType.Char);
			sqlCommand.Parameters.Add($"@val{num}val4", SqlDbType.Int);
			sqlCommand.Parameters.Add($"@val{num}val5", SqlDbType.NVarChar);
			sqlCommand.Parameters.Add($"@val{num}val6", SqlDbType.NVarChar);
			sqlCommand.Parameters.Add($"@val{num}val7", SqlDbType.DateTime);
			sqlCommand.Parameters.Add($"@val{num}val8", SqlDbType.DateTime);
			sqlCommand.Parameters[$"@val{num}val1"].Value = columnNavigation.ColumnId;
			string text;
			if (fieldMostCommonlyUsedValue.Value != null)
			{
				text = fieldMostCommonlyUsedValue.Value.ToString();
				if (text.Length > 250)
				{
					text = Truncate(text, 250);
				}
			}
			else
			{
				text = "NULL";
			}
			sqlCommand.Parameters[$"@val{num}val2"].Value = text;
			sqlCommand.Parameters[$"@val{num}val3"].Value = columnNavigation.ValuesListMode;
			object value = ((fieldMostCommonlyUsedValue == null) ? DBNull.Value : ((object)fieldMostCommonlyUsedValue.Count));
			sqlCommand.Parameters[$"@val{num}val4"].Value = value;
			sqlCommand.Parameters[$"@val{num}val5"].Value = LoginStrategy.GetUserName();
			sqlCommand.Parameters[$"@val{num}val6"].Value = LoginStrategy.GetUserName();
			sqlCommand.Parameters[$"@val{num}val7"].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			sqlCommand.Parameters[$"@val{num}val8"].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			if (sqlCommand.CommandText.Length > num2)
			{
				commands.Manipulation.DataProfiling.InsertColumnValuesCmd(sqlCommand);
				sqlCommand = new SqlCommand
				{
					CommandText = string.Empty
				};
			}
			num++;
		}
		if (sqlCommand.CommandText.Length > 1)
		{
			commands.Manipulation.DataProfiling.InsertColumnValuesCmd(sqlCommand);
		}
	}

	private void InsertRandomValues(ColumnNavigationObject columnNavigation)
	{
		if (columnNavigation?.ObjectSampleData == null)
		{
			return;
		}
		int num = 0;
		int num2 = 69000;
		SqlCommand sqlCommand = new SqlCommand();
		object[][] values = columnNavigation.ObjectSampleData.Values;
		foreach (object[] array in values)
		{
			sqlCommand.CommandText += $"INSERT INTO [column_values] (\r\n                        [column_id],\r\n                        [value],\r\n                        [type],\r\n                        [row_count],\r\n                        [created_by],\r\n                        [modified_by],\r\n                        [creation_date],\r\n                        [last_modification_date])\r\n                    VALUES (\r\n                        @val{num}val1,\r\n                        @val{num}val2,\r\n                        @val{num}val3,\r\n                        @val{num}val4,\r\n                        @val{num}val5,\r\n                        @val{num}val6,\r\n                        @val{num}val7,\r\n                        @val{num}val8);";
			sqlCommand.Parameters.Add($"@val{num}val1", SqlDbType.Int);
			sqlCommand.Parameters.Add($"@val{num}val2", SqlDbType.NVarChar);
			sqlCommand.Parameters.Add($"@val{num}val3", SqlDbType.Char);
			sqlCommand.Parameters.Add($"@val{num}val4", SqlDbType.Int);
			sqlCommand.Parameters.Add($"@val{num}val5", SqlDbType.NVarChar);
			sqlCommand.Parameters.Add($"@val{num}val6", SqlDbType.NVarChar);
			sqlCommand.Parameters.Add($"@val{num}val7", SqlDbType.DateTime);
			sqlCommand.Parameters.Add($"@val{num}val8", SqlDbType.DateTime);
			sqlCommand.Parameters[$"@val{num}val1"].Value = columnNavigation.ColumnId;
			string text;
			if (array[0] != null)
			{
				text = array[0].ToString();
				if (text.Length > 250)
				{
					text = Truncate(text, 250);
				}
			}
			else
			{
				text = "NULL";
			}
			sqlCommand.Parameters[$"@val{num}val2"].Value = text;
			sqlCommand.Parameters[$"@val{num}val3"].Value = columnNavigation.ValuesListMode;
			sqlCommand.Parameters[$"@val{num}val4"].Value = DBNull.Value;
			sqlCommand.Parameters[$"@val{num}val5"].Value = LoginStrategy.GetUserName();
			sqlCommand.Parameters[$"@val{num}val6"].Value = LoginStrategy.GetUserName();
			sqlCommand.Parameters[$"@val{num}val7"].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			sqlCommand.Parameters[$"@val{num}val8"].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			if (sqlCommand.CommandText.Length > num2)
			{
				commands.Manipulation.DataProfiling.InsertColumnValuesCmd(sqlCommand);
				sqlCommand = new SqlCommand
				{
					CommandText = string.Empty
				};
			}
			num++;
		}
		if (sqlCommand.CommandText.Length > 0)
		{
			commands.Manipulation.DataProfiling.InsertColumnValuesCmd(sqlCommand);
		}
	}

	private string Truncate(string value, int maxLength)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}
		if (value.Length > maxLength)
		{
			return value.Substring(0, maxLength);
		}
		return value;
	}
}
