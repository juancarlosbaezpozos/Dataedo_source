using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dataedo.Data.Base.Tools;
using Dataedo.Model.Data.History;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.Data.MetadataServer.History;

internal static class HistoryBulkInserterHelper
{
	internal static void BulkInsertHistoryModels(List<HistoryModel> historyModelListToBulkInsert)
	{
		if (!DB.History.SavingEnabled || historyModelListToBulkInsert == null || historyModelListToBulkInsert.Count() < 1)
		{
			return;
		}
		int num = 0;
		int num2 = 69000;
		SqlCommand sqlCommand = new SqlCommand();
		foreach (HistoryModel item in historyModelListToBulkInsert)
		{
			sqlCommand.CommandText += $"INSERT INTO [changes_history] (\r\n                        [database_id],\r\n                        [table],\r\n                        [column],\r\n                        [row_id],\r\n                        [value],\r\n                        [value_plain],\r\n                        [user_id],\r\n                        [product],\r\n                        [client_version],\r\n                        [source_id],\r\n                        [created_by])\r\n                    VALUES (\r\n                        @val{num}val1,\r\n                        @val{num}val2,\r\n                        @val{num}val3,\r\n                        @val{num}val4,\r\n                        @val{num}val5,\r\n                        @val{num}val6,\r\n                        @val{num}val7,\r\n                        @val{num}val8,\r\n                        @val{num}val9,\r\n                        @val{num}val10,\r\n                        @val{num}val11);";
			sqlCommand.Parameters.Add($"@val{num}val1", SqlDbType.Int);
			sqlCommand.Parameters.Add($"@val{num}val2", SqlDbType.NVarChar);
			sqlCommand.Parameters.Add($"@val{num}val3", SqlDbType.NVarChar);
			sqlCommand.Parameters.Add($"@val{num}val4", SqlDbType.Int);
			sqlCommand.Parameters.Add($"@val{num}val5", SqlDbType.NVarChar);
			sqlCommand.Parameters.Add($"@val{num}val6", SqlDbType.NVarChar);
			sqlCommand.Parameters.Add($"@val{num}val7", SqlDbType.Int);
			sqlCommand.Parameters.Add($"@val{num}val8", SqlDbType.NVarChar);
			sqlCommand.Parameters.Add($"@val{num}val9", SqlDbType.NVarChar);
			sqlCommand.Parameters.Add($"@val{num}val10", SqlDbType.Int);
			sqlCommand.Parameters.Add($"@val{num}val11", SqlDbType.NVarChar);
			sqlCommand.Parameters[$"@val{num}val1"].Value = item.DatabaseId;
			sqlCommand.Parameters[$"@val{num}val2"].Value = item.Table;
			sqlCommand.Parameters[$"@val{num}val3"].Value = item.Column;
			sqlCommand.Parameters[$"@val{num}val4"].Value = item.RowId;
			if (string.IsNullOrEmpty(item?.Value))
			{
				sqlCommand.Parameters[$"@val{num}val5"].Value = DBNull.Value;
			}
			else
			{
				sqlCommand.Parameters[$"@val{num}val5"].Value = item?.Value;
			}
			if (string.IsNullOrEmpty(item?.ValuePlain))
			{
				sqlCommand.Parameters[$"@val{num}val6"].Value = DBNull.Value;
			}
			else
			{
				sqlCommand.Parameters[$"@val{num}val6"].Value = item?.ValuePlain;
			}
			if (!item.UserId.HasValue)
			{
				sqlCommand.Parameters[$"@val{num}val7"].Value = DBNull.Value;
			}
			else
			{
				sqlCommand.Parameters[$"@val{num}val7"].Value = item.UserId;
			}
			if (string.IsNullOrEmpty("DESKTOP"))
			{
				sqlCommand.Parameters[$"@val{num}val8"].Value = DBNull.Value;
			}
			else
			{
				sqlCommand.Parameters[$"@val{num}val8"].Value = "DESKTOP";
			}
			if (string.IsNullOrEmpty(Dataedo.App.StaticData.ProductVersion))
			{
				sqlCommand.Parameters[$"@val{num}val9"].Value = DBNull.Value;
			}
			else
			{
				sqlCommand.Parameters[$"@val{num}val9"].Value = Dataedo.App.StaticData.ProductVersion;
			}
			sqlCommand.Parameters[$"@val{num}val10"].Value = DBNull.Value;
			if (string.IsNullOrEmpty(LoginStrategy.GetUserName()))
			{
				sqlCommand.Parameters[$"@val{num}val11"].Value = DBNull.Value;
			}
			else
			{
				sqlCommand.Parameters[$"@val{num}val11"].Value = LoginStrategy.GetUserName();
			}
			if (sqlCommand.CommandText.Length > num2)
			{
				DB.History.InsertCommandToRepository(sqlCommand);
				sqlCommand = new SqlCommand
				{
					CommandText = string.Empty
				};
			}
			num++;
		}
		if (sqlCommand.CommandText.Length > 0)
		{
			DB.History.InsertCommandToRepository(sqlCommand);
		}
	}
}
