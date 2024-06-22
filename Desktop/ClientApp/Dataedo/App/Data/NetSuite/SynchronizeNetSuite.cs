using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Shared.Enums;
using NetSuite.SuiteAnalyticsConnect;

namespace Dataedo.App.Data.NetSuite;

internal class SynchronizeNetSuite : SynchronizeDatabase
{
	public SynchronizeNetSuite(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Counting objects to import");
		if (!string.IsNullOrWhiteSpace(query.Query))
		{
			try
			{
				using OpenAccessCommand openAccessCommand = CommandsWithTimeout.NetSuiteCommand(query.Query, synchronizeParameters.DatabaseRow.Connection);
				using OpenAccessDataReader openAccessDataReader = openAccessCommand.ExecuteReader();
				while (openAccessDataReader.Read())
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					ReadCount(openAccessDataReader);
				}
			}
			catch (Exception exception)
			{
				GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
				return false;
			}
		}
		return true;
	}

	public override bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Retrieving database's objects");
		if (base.ObjectsCounter != null && !string.IsNullOrWhiteSpace(query.Query))
		{
			try
			{
				using OpenAccessCommand openAccessCommand = CommandsWithTimeout.NetSuiteCommand(query.Query, synchronizeParameters.DatabaseRow.Connection);
				using OpenAccessDataReader openAccessDataReader = openAccessCommand.ExecuteReader();
				while (openAccessDataReader.Read())
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					AddDBObject(openAccessDataReader, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
				}
			}
			catch (Exception exception)
			{
				GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
				return false;
			}
		}
		return true;
	}

	public override bool GetColumns(string query, Form owner = null)
	{
		try
		{
			using OpenAccessCommand openAccessCommand = CommandsWithTimeout.NetSuiteCommand(query, synchronizeParameters.DatabaseRow.Connection);
			using OpenAccessDataReader openAccessDataReader = openAccessCommand.ExecuteReader();
			while (openAccessDataReader.Read())
			{
				AddColumn(openAccessDataReader);
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetRelations(string query, Form owner = null)
	{
		try
		{
			using OpenAccessCommand openAccessCommand = CommandsWithTimeout.NetSuiteCommand(query, synchronizeParameters.DatabaseRow.Connection);
			using OpenAccessDataReader openAccessDataReader = openAccessCommand.ExecuteReader();
			while (openAccessDataReader.Read())
			{
				AddRelation(openAccessDataReader, SharedDatabaseTypeEnum.DatabaseType.NetSuite);
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetParameters(string query, bool canReadParameters = true, Form owner = null)
	{
		throw new NotImplementedException();
	}

	public override bool GetTriggers(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(query))
			{
				using OpenAccessCommand openAccessCommand = CommandsWithTimeout.NetSuiteCommand(query, synchronizeParameters.DatabaseRow.Connection);
				using OpenAccessDataReader openAccessDataReader = openAccessCommand.ExecuteReader();
				while (openAccessDataReader.Read())
				{
					AddUniqueConstraint(openAccessDataReader, SharedDatabaseTypeEnum.DatabaseType.NetSuite);
				}
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetDependencies(string query, Form owner = null)
	{
		throw new NotImplementedException();
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, null, "[table_name]");
		yield return "SELECT \r\n                                'TABLE' AS [object_type],\r\n                                COUNT(1) AS [count] \r\n                            FROM \r\n                                OA_TABLES \r\n                            WHERE\r\n                                [table_owner] <> 'SYSTEM'\r\n                                " + filterString;
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, null, "table_name");
		yield return "SELECT\r\n                                [table_name] AS [name],\r\n                                [table_type] AS [type],\r\n                                '' AS [definition],\r\n                                NULL AS [database_name],\r\n                                NULL AS [create_date],\r\n                                NULL AS [modify_date],\r\n                                NULL AS [function_type],\r\n                                [remarks] AS [description]\r\n                            FROM\r\n                                OA_TABLES\r\n                            WHERE\r\n                                [table_owner] <> 'SYSTEM'\r\n                                " + filterString;
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> RelationsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString(null, base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, null, "[fktable_name]");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("OR", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, null, "[pktable_name]");
		string text = FilterRulesCollection.CombineRelationsFilter(filterString, filterString2);
		yield return "SELECT\r\n                                [fk_name] AS [NAME],\r\n                                NULL AS [FK_TABLE_DATABASE_NAME],\r\n                                [fktable_name] AS [FK_TABLE_NAME],\r\n                                '' AS [FK_TABLE_SCHEMA],\r\n                                [fkcolumn_name] AS [FK_COLUMN],\r\n                                NULL AS [REF_TABLE_DATABASE_NAME],\r\n                                [pktable_name] AS [REF_TABLE_NAME],\r\n                                '' AS [REF_TABLE_SCHEMA],\r\n                                [pkcolumn_name] AS [REF_COLUMN],\r\n                                [key_seq] AS [ORDINAL_POSITION],\r\n                                NULL as [DESCRIPTION],\r\n                                NULL AS [UPDATE_RULE],\r\n                                NULL AS [DELETE_RULE]\r\n                            FROM \r\n                                OA_FKEYS\r\n                            WHERE\r\n                                [pktable_owner] <> 'SYSTEM'\r\n                                AND [pkcolumn_name] is not null\r\n                                AND FKCOLUMN_NAME is not null\r\n                                " + text;
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, null, "table_name");
		yield return "SELECT\r\n                                [table_name] AS [table_name],\r\n                                [column_name] AS [name],\r\n                                [type_name] AS [data_type],\r\n                                [oa_length] AS [data_length],\r\n                                [remarks] AS [description],\r\n                                [oa_nullable] AS [nullable]\r\n                            FROM\r\n                                OA_COLUMNS\r\n                            WHERE\r\n                                [table_owner] <> 'SYSTEM'\r\n                                " + filterString;
	}

	public override IEnumerable<string> TriggersQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, null, "pktable_name");
		yield return "SELECT\r\n                                [pktable_name] AS [table_name],\r\n                                '' AS [table_schema],\r\n                                [pk_name] AS [name],\r\n                                'P' AS [type],\r\n                                [pkcolumn_name] AS [column_name],\r\n                                [key_seq] AS [column_ordinal]\r\n                            FROM\r\n                                OA_FKEYS\r\n                            WHERE\r\n                                [pktable_owner] <> 'SYSTEM'\r\n                                AND [pkcolumn_name] is not null\r\n                                AND FKCOLUMN_NAME is null\r\n                                " + filterString;
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		yield return string.Empty;
	}
}
