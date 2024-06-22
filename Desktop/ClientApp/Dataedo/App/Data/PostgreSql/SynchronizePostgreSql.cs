using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.General;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Shared.Enums;
using Devart.Data.Universal;
using Npgsql;

namespace Dataedo.App.Data.PostgreSql;

public class SynchronizePostgreSql : SynchronizeDatabase
{
	protected virtual SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.PostgreSQL;

	public SynchronizePostgreSql(SynchronizeParameters synchronizeParameters)
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
				using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.PostgreSql(query.Query, synchronizeParameters.DatabaseRow.Connection);
				using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
				while (npgsqlDataReader.Read())
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					ReadCount(npgsqlDataReader);
				}
			}
			catch (Exception ex)
			{
				GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
				return false;
			}
		}
		return true;
	}

	public override bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (base.ObjectsCounter != null)
		{
			try
			{
				backgroundWorkerManager.ReportProgress("Retrieving database's objects");
				if (!string.IsNullOrWhiteSpace(query.Query))
				{
					using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.PostgreSql(query.Query, synchronizeParameters.DatabaseRow.Connection);
					using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
					while (npgsqlDataReader.Read())
					{
						if (synchronizeParameters.DbSynchLocker.IsCanceled)
						{
							return false;
						}
						AddDBObject(npgsqlDataReader, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
					}
				}
			}
			catch (UniException ex) when (ex.Message.StartsWith("cache lookup failed for type"))
			{
				GeneralMessageBoxesHandling.Show("A " + ex.Message + " occurred." + Environment.NewLine + "This may be caused by an unfinished drop or create command on one of the imported objects." + Environment.NewLine + "You can try using filters to identify and exclude these objects.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
				return false;
			}
			catch (Exception exception)
			{
				GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
				return false;
			}
		}
		return true;
	}

	public override bool GetRelations(string query, Form owner = null)
	{
		try
		{
			using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.PostgreSql(query, synchronizeParameters.DatabaseRow.Connection);
			using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
			while (npgsqlDataReader.Read())
			{
				AddRelation(npgsqlDataReader, DatabaseType);
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetColumns(string query, Form owner = null)
	{
		try
		{
			using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.PostgreSql(query, synchronizeParameters.DatabaseRow.Connection);
			using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
			while (npgsqlDataReader.Read())
			{
				AddColumn(npgsqlDataReader);
			}
		}
		catch (UniException ex) when ("unrecognized privilege: 39".Equals(ex.InnerException?.Message))
		{
			return GetColumnsForAmazon(owner);
		}
		catch (UniException ex2) when (ex2.Message.StartsWith("catalog is missing"))
		{
			GeneralMessageBoxesHandling.Show("A " + ex2.Message + " occurred." + Environment.NewLine + "This may be caused by an unfinished drop or create command on one of the imported objects." + Environment.NewLine + "You can try using filters to identify and exclude these objects.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			return false;
		}
		catch (Exception ex3)
		{
			GeneralExceptionHandling.Handle(ex3, ex3.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public bool GetColumnsForAmazon(Form owner = null)
	{
		string commandText = ColumnsQueryForAmazon();
		try
		{
			using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.PostgreSql(commandText, synchronizeParameters.DatabaseRow.Connection);
			using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
			while (npgsqlDataReader.Read())
			{
				AddColumn(npgsqlDataReader);
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetTriggers(string query, Form owner = null)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(query))
			{
				using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.PostgreSql(query, synchronizeParameters.DatabaseRow.Connection);
				using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
				while (npgsqlDataReader.Read())
				{
					AddTrigger(npgsqlDataReader);
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

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(query))
			{
				using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.PostgreSql(query, synchronizeParameters.DatabaseRow.Connection);
				using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
				while (npgsqlDataReader.Read())
				{
					AddUniqueConstraint(npgsqlDataReader, DatabaseType);
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

	public override bool GetParameters(string query, bool canReadParameters = true, Form owner = null)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(query))
			{
				using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.PostgreSql(query, synchronizeParameters.DatabaseRow.Connection);
				using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
				while (npgsqlDataReader.Read())
				{
					AddParameter(npgsqlDataReader);
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
		try
		{
			if (!string.IsNullOrWhiteSpace(query))
			{
				using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.PostgreSql(query, synchronizeParameters.DatabaseRow.Connection);
				using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
				while (npgsqlDataReader.Read())
				{
					AddDependency(npgsqlDataReader);
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

	public override IEnumerable<string> CountTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "nc.nspname", "c.relname");
		yield return "SELECT \r\n\t                        'table'::text AS \"object_type\",\r\n                            COUNT(1) AS \"count\"\r\n                        FROM\r\n                            pg_namespace nc\r\n                        JOIN pg_class c ON\r\n                            nc.oid = c.relnamespace\r\n                        WHERE\r\n                            c.relkind IN('r', 'p', 'f') --BASE TABLE\r\n                            AND nc.nspname NOT IN('pg_catalog', 'information_schema')\r\n                            AND c.relispartition = false\r\n                            " + filterString + ";";
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "nc.nspname", "c.relname");
		yield return "SELECT \r\n\t                        'view'::text AS \"object_type\",\r\n                            COUNT(1) AS \"count\"\r\n                        FROM\r\n                            pg_namespace nc\r\n                        JOIN pg_class c ON\r\n                            nc.oid = c.relnamespace\r\n                        WHERE\r\n                            c.relkind IN('v') --VIEW\r\n                            AND nc.nspname NOT IN('pg_catalog', 'information_schema')\r\n                            " + filterString + ";";
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		if (synchronizeParameters.DatabaseRow.GetVersionUpdate().Version < 11)
		{
			yield return string.Empty;
			yield break;
		}
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "nc.nspname", "p.proname");
		yield return "SELECT \r\n\t                            'procedure'::text AS \"object_type\",\r\n                                count(1) AS \"count\"\r\n                            FROM\r\n                                pg_namespace nc\r\n                            INNER JOIN pg_proc p ON \r\n                                nc.oid = p.pronamespace\r\n                            WHERE\r\n                                p.prokind = 'p' --PROCEDURE\r\n                                AND nc.nspname NOT IN('pg_catalog', 'information_schema')\r\n                                " + filterString + ";";
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "nc.nspname", "p.proname");
		DatabaseVersionUpdate versionUpdate = synchronizeParameters.DatabaseRow.GetVersionUpdate();
		string text = "WHERE nc.nspname NOT IN('pg_catalog', 'information_schema') AND p.prokind <> 'p' ";
		if (versionUpdate.Version < 11)
		{
			text = "WHERE nc.nspname NOT IN('pg_catalog', 'information_schema') ";
		}
		yield return "SELECT \r\n\t                            'function'::text AS \"object_type\",\r\n                                count(1) AS \"count\"\r\n                            FROM\r\n                                pg_namespace nc\r\n                            INNER JOIN pg_proc p ON \r\n                                nc.oid = p.pronamespace\r\n                            " + text + "\r\n                            " + filterString + ";";
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "pgn.nspname", "pgc.relname");
		yield return "SELECT \r\n                            pgc.relname AS \"name\",\r\n                            pgn.nspname AS \"schema\",\r\n                            current_catalog AS \"database_name\",\r\n                            CASE pgc.relkind \r\n                                WHEN 'r' THEN 'TABLE'::text \r\n                                WHEN 'p' THEN 'TABLE'::text\r\n                                WHEN 'f' THEN 'FOREIGN_TABLE'::text \r\n                            END AS \"type\",\r\n                            pgd.description AS \"description\",\r\n                            null AS \"definition\",     \r\n                            null AS \"create_date\",\r\n                            '01-01-2017'::date AS \"modify_date\",\r\n                            null AS \"function_type\"\r\n                            FROM\r\n                                pg_catalog.pg_class pgc\r\n                            INNER JOIN pg_catalog.pg_namespace pgn ON \r\n                                pgn.oid = pgc.relnamespace\r\n                            LEFT JOIN pg_description pgd ON \r\n                                pgd.objoid = pgc.oid \r\n                                AND \r\n                                (pgd.objsubid = 0 OR pgd.objsubid IS NULL)\r\n                            WHERE \r\n                                pgn.nspname NOT IN ('pg_catalog', 'information_schema')\r\n                                AND pgc.relkind IN ('r','f','p') /*r=table,f=foreign_table,v=view,m=matview*/ \r\n                                AND pgc.relispartition = false\r\n                                " + filterString + ";";
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		string viewFilterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "pgn.nspname", "pgc.relname");
		yield return "SELECT \r\n                            pgc.relname AS \"name\",\r\n                            pgn.nspname AS \"schema\",\r\n                            current_catalog AS \"database_name\",\r\n   \t                        'VIEW'::text AS \"type\",\r\n                            pgd.description AS \"description\",\r\n                            pgv.definition AS \"definition\",     \r\n                            null AS \"create_date\",\r\n                            '01-01-2017'::date AS \"modify_date\",\r\n                            null AS \"function_type\"\r\n                        FROM\r\n                            pg_catalog.pg_class pgc\r\n                        INNER JOIN pg_catalog.pg_namespace pgn ON \r\n                            pgn.oid = pgc.relnamespace\r\n                        LEFT JOIN pg_description pgd ON \r\n                            pgd.objoid = pgc.oid \r\n                            AND \r\n                            (pgd.objsubid = 0 OR pgd.objsubid IS NULL)\r\n                        LEFT JOIN pg_views pgv ON\r\n\t                        pgn.nspname = pgv.schemaname\r\n\t                        AND\r\n\t                        pgc.relname = pgv.viewname\r\n                        WHERE \r\n                            pgn.nspname NOT IN ('pg_catalog', 'information_schema')\r\n                            AND pgc.relkind = 'v'\r\n                            " + viewFilterString + ";";
		yield return "SELECT \r\n                            pgc.relname AS \"name\",\r\n                            pgn.nspname AS \"schema\",\r\n                            current_catalog AS \"database_name\",\r\n   \t                        'MATERIALIZED VIEW'::text AS \"type\",\r\n                            pgd.description AS \"description\",\r\n                            pgv.definition AS \"definition\",     \r\n                            null AS \"create_date\",\r\n                            '01-01-2017'::text AS \"modify_date\",\r\n                            null AS \"function_type\"\r\n                        FROM\r\n                            pg_catalog.pg_class pgc\r\n                        INNER JOIN pg_catalog.pg_namespace pgn ON \r\n                            pgn.oid = pgc.relnamespace\r\n                        LEFT JOIN pg_description pgd ON \r\n                            pgd.objoid = pgc.oid \r\n                            AND \r\n                            (pgd.objsubid = 0 OR pgd.objsubid IS NULL)\r\n                        LEFT JOIN pg_matviews pgv ON\r\n\t                        pgn.nspname = pgv.schemaname\r\n\t                        AND\r\n\t                        pgc.relname = pgv.matviewname\r\n                        WHERE \r\n                            pgn.nspname NOT IN ('pg_catalog', 'information_schema')\r\n                            AND pgc.relkind = 'mv'\r\n                            " + viewFilterString + ";";
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		if (synchronizeParameters.DatabaseRow.GetVersionUpdate().Version < 11)
		{
			yield return string.Empty;
			yield break;
		}
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "n.nspname", "p.proname");
		yield return "SELECT \r\n                                p.proname AS \"name\",\r\n                                n.nspname AS \"schema\",\r\n                                current_catalog AS \"database_name\",\r\n                                'PROCEDURE'::text AS \"type\",\r\n                                d.description AS \"description\",\r\n                                --l.lanname, --TEST\r\n                                CASE\r\n                                    WHEN l.lanname = 'internal' THEN p.prosrc\r\n                                    ELSE pg_get_functiondef(p.oid)\r\n                                END AS \"definition\",\r\n                                null AS \"create_date\",\r\n                                '01-01-2017'::date AS \"modify_date\", -- Any date to mark object as 'Forced reimport'\r\n                                null AS \"function_type\"\r\n                            FROM pg_proc p\r\n                            JOIN pg_language l ON \r\n                                l.oid = p.prolang\r\n                            JOIN pg_namespace n ON \r\n                                n.oid = p.pronamespace\r\n                            LEFT OUTER JOIN pg_description d ON \r\n                                d.objoid = p.oid\r\n                        WHERE \r\n                            n.nspname NOT IN('pg_catalog', 'information_schema') \r\n                            AND p.prokind = 'p'  \r\n                            " + filterString + ";";
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "n.nspname", "p.proname");
		string text = "WHERE n.nspname NOT IN('pg_catalog', 'information_schema') AND p.prokind <> 'p'";
		if (synchronizeParameters.DatabaseRow.GetVersionUpdate().Version < 11)
		{
			text = "WHERE n.nspname NOT IN('pg_catalog', 'information_schema')";
		}
		yield return "SELECT \r\n                                p.proname AS \"name\",\r\n                                n.nspname AS \"schema\",\r\n                                current_database() AS \"database_name\",\r\n                                'FUNCTION'::text AS \"type\",\r\n                                d.description AS \"description\",\r\n                                --l.lanname, --TEST\r\n                                CASE --pg_get_functiondef won't work for internal language\r\n                                    WHEN l.lanname = 'internal'::text THEN p.prosrc\r\n                                    ELSE pg_get_functiondef(p.oid)\r\n                                END AS \"definition\",\r\n                                null AS \"create_date\",\r\n                                '01-01-2017'::date AS \"modify_date\", -- Any date to mark object as 'Forced reimport'\r\n                                null AS \"function_type\"\r\n                            FROM \r\n                                pg_proc p\r\n                            JOIN pg_language l ON \r\n                                l.oid = p.prolang\r\n                            JOIN pg_namespace n ON \r\n                                n.oid = p.pronamespace\r\n                            LEFT OUTER JOIN pg_description d ON \r\n                                d.objoid = p.oid\r\n                                " + text + " \r\n                                " + filterString + ";";
	}

	public override IEnumerable<string> RelationsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString(null, base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "con.nspname", "con.relname");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("OR", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "ns.nspname ", "cl.relname");
		string text = FilterRulesCollection.CombineRelationsFilter(filterString, filterString2);
		yield return "SELECT con.conname AS \"name\"\r\n                    , con.nspname AS \"fk_table_schema\"\r\n                    , con.relname AS \"fk_table_name\"\r\n                    , '" + synchronizeParameters.DatabaseName + "'::text AS \"fk_table_database_name\"\r\n                    , att2.attname AS \"fk_column\"\r\n                    , ns.nspname AS \"ref_table_schema\"\r\n                    , cl.relname AS \"ref_table_name\"\r\n                    , '" + synchronizeParameters.DatabaseName + "'::text AS \"ref_table_database_name\"\r\n                    , att.attname AS \"ref_column\"\r\n                    , att2.attnum AS \"ordinal_position\"\r\n                    , NULL AS \"description\"\r\n                    , CASE\r\n                        WHEN con.confupdtype = 'a'\r\n                            THEN 'NO ACTION'::text\r\n                        WHEN con.confupdtype = 'r'\r\n                            THEN 'RESTRICT'::text\r\n                        WHEN con.confupdtype = 'c'\r\n                            THEN 'CASCADE'::text\r\n                        WHEN con.confupdtype = 'n'\r\n                            THEN 'SET NULL'::text\r\n                        WHEN con.confupdtype = 'd'\r\n                            THEN 'SET DEFAULT'::text\r\n                        ELSE con.confupdtype\r\n                        END AS \"update_rule\"\r\n                    , CASE\r\n                        WHEN con.confdeltype = 'a'\r\n                            THEN 'NO ACTION'::text\r\n                        WHEN con.confdeltype = 'r'\r\n                            THEN 'RESTRICT'::text\r\n                        WHEN con.confdeltype = 'c'\r\n                            THEN 'CASCADE'::text\r\n                        WHEN con.confdeltype = 'n'\r\n                            THEN 'SET NULL'::text\r\n                        WHEN con.confdeltype = 'd'\r\n                            THEN 'SET DEFAULT'::text\r\n                        ELSE con.confdeltype\r\n                        END AS \"delete_rule\"\r\n                FROM(\r\n                    SELECT unnest(con1.conkey) AS \"parent\"\r\n                        , unnest(con1.confkey) AS \"child\"\r\n                        , ns.nspname\r\n                        , cl.relname\r\n                        , con1.confupdtype\r\n                        , con1.confdeltype\r\n                        , con1.confrelid\r\n                        , con1.conrelid\r\n                        , con1.conname\r\n                    FROM pg_class cl\r\n                    INNER JOIN pg_namespace ns\r\n                        ON cl.relnamespace = ns.oid\r\n                    INNER JOIN pg_constraint con1\r\n                        ON con1.conrelid = cl.oid\r\n                    WHERE con1.contype = 'f'\r\n                    ) con\r\n                INNER JOIN pg_attribute att\r\n                    ON att.attrelid = con.confrelid\r\n                        AND att.attnum = con.child\r\n                INNER JOIN pg_class cl\r\n                    ON cl.oid = con.confrelid\r\n                INNER JOIN pg_attribute att2\r\n                    ON att2.attrelid = con.conrelid\r\n                        AND att2.attnum = con.parent\r\n                INNER JOIN pg_namespace ns\r\n                    ON cl.relnamespace = ns.oid \r\n                    " + text + ";";
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "pgn.nspname", "pgc.relname");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.View, "pgn.nspname", "pgc.relname");
		DatabaseVersionUpdate versionUpdate = synchronizeParameters.DatabaseRow.GetVersionUpdate();
		string text = "CASE \r\n\t\t                                WHEN pga.attgenerated = '' THEN pg_get_expr(pgad.adbin, pgad.adrelid) \r\n\t\t                                ELSE NULL\r\n\t                                END AS \"default_value\",\r\n                                    CASE \r\n                                        WHEN pga.attgenerated <> '' THEN pg_get_expr(pgad.adbin, pgad.adrelid) \r\n                                        ELSE NULL\r\n                                    END AS \"computed_formula\",\r\n                                    CASE\r\n                                        WHEN pga.attgenerated <> '' THEN 1\r\n                                        ELSE 0\r\n                                    END AS \"is_computed\",";
		if (versionUpdate.Version < 12)
		{
			text = "pg_get_expr(pgad.adbin, pgad.adrelid) AS \"default_value\",\r\n                                   NULL AS \"computed_formula\",\r\n                                   0 as \"is_computed\",";
		}
		string text2 = "CASE \r\n\t \t                            WHEN pga.attidentity <> '' THEN 1 \r\n\t \t                            ELSE 0 \r\n\t                              END AS \"is_identity\",";
		if (versionUpdate.Version < 10)
		{
			text2 = "0 AS \"is_identity\",";
		}
		yield return "SELECT\r\n\t                        CURRENT_CATALOG AS \"database_name\",\r\n\t                        pgc.relname AS \"table_name\",\r\n\t                        pgn.nspname AS \"table_schema\",\r\n\t                        pga.attname AS \"name\",\r\n\t                        pga.attnum AS \"position\",\r\n\t                        split_part( format_type( pga.atttypid, pga.atttypmod ), '(',1 ) AS \"datatype\",\r\n\t                        pgd.description AS \"description\",\r\n\t                        CASE\r\n\t\t                        WHEN pgi.indexrelid IS NOT NULL THEN 1\r\n\t\t                        ELSE 0\r\n\t                        END AS \"constraint_type\",\r\n\t                        CASE\r\n\t\t                        WHEN pga.attnotnull THEN 0\r\n\t\t                        ELSE 1\r\n\t                        END AS \"nullable\",\r\n\t                        " + text + "\r\n\t                        " + text2 + "\r\n\t                        REPLACE ( TRIM( TRAILING ')' FROM split_part( format_type( pga.atttypid, pga.atttypmod ),'(', 2 )), ',',', ') AS \"data_length\"\r\n                        FROM\r\n\t                        pg_catalog.pg_attribute pga\r\n                        INNER JOIN pg_catalog.pg_class pgc ON\r\n\t                        pga.attrelid = pgc.oid\r\n                        INNER JOIN pg_catalog.pg_namespace pgn ON\r\n\t                        pgn.oid = pgc.relnamespace\r\n                        LEFT JOIN pg_index pgi ON\r\n\t                        pgc.oid = pgi.indrelid\r\n\t                        AND pga.attnum = ANY( pgi.indkey )\r\n\t                        AND pgi.indisprimary\r\n                        LEFT JOIN pg_description pgd ON\r\n\t                        pgd.objoid = pgc.oid\r\n\t                        AND pgd.objsubid = pga.attnum\r\n                        LEFT JOIN pg_attrdef pgad ON \r\n\t                        pga.attrelid = pgad.adrelid \r\n\t                        AND pga.attnum = pgad.adnum\r\n                        WHERE\r\n\t                        pga.attnum > 0\r\n\t                        AND NOT pga.attisdropped\r\n\t                        AND pgn.nspname NOT IN ( 'pg_catalog','information_schema' )\r\n\t                        AND pgc.relkind IN ( 'r', 'f', 'p', 'v', 'm' ) /*r=table,v=view,m=matview,f=foreign table,p=partitioned table*/\r\n\t                        AND \r\n\t                        (\r\n\t\t                        ( pgc.relkind IN ( 'r', 'f', 'p' ) \r\n\t\t                        " + filterString + " )\r\n\t\t                        OR \r\n\t\t                        ( pgc.relkind IN ( 'v', 'm' ) \r\n\t\t                        " + filterString2 + " )\r\n\t                        )\r\n                        ORDER BY\r\n\t                        \"table_schema\",\r\n\t                        \"table_name\",\r\n\t                        \"position\";";
	}

	public string ColumnsQueryForAmazon()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "pgn.nspname", "pgc.relname");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.View, "pgn.nspname", "pgc.relname");
		return "SELECT\r\n                        current_catalog AS \"database_name\",\r\n                        pgc.relname AS \"table_name\",\r\n                        pgn.nspname AS \"table_schema\",\r\n                        pga.attname AS \"name\",\r\n                        pga.attnum AS \"position\",\r\n                        split_part(format_type(pga.atttypid, pga.atttypmod),'(', 1) AS \"datatype\",\r\n                        pgd.description as \"description\",\r\n                        CASE WHEN pgi.indexrelid IS NOT NULL THEN 1 ELSE 0 END AS \"constraint_type\",\r\n                        CASE WHEN pga.attnotnull THEN 0 ELSE 1 END AS \"nullable\",\r\n                        NULL AS \"default_value\",\r\n                        0 AS \"is_computed\",\r\n                        0 AS \"is_identity\",\r\n                        NULL as \"computed_formula\",\r\n                        replace(trim(trailing ')' from split_part(format_type(pga.atttypid, pga.atttypmod),'(', 2)),',',', ') AS \"data_length\"\r\n                        FROM\r\n                            pg_catalog.pg_attribute pga\r\n                             INNER JOIN pg_catalog.pg_class pgc ON pga.attrelid = pgc.oid\r\n                            INNER JOIN pg_catalog.pg_namespace pgn ON pgn.oid = pgc.relnamespace\r\n                             LEFT JOIN pg_index pgi ON pgc.oid = pgi.indrelid AND pga.attnum = ANY(pgi.indkey) AND pgi.indisprimary\r\n                             LEFT JOIN pg_description pgd ON pgd.objoid = pgc.oid AND pgd.objsubid = pga.attnum\r\n                        WHERE\r\n                            pga.attnum > 0\r\n                            AND NOT pga.attisdropped\r\n                                AND pgn.nspname NOT IN ('pg_catalog', 'information_schema')\r\n                                  AND pgc.relkind IN ('r','f','p','v','m') /*r=table,v=view,m=matview,f=foreign table*/\r\n                                  AND ((pgc.relkind IN ('r','f','p') " + filterString + ")\r\n                                            OR (pgc.relkind IN ('v','m') " + filterString2 + "))\r\n                        ORDER BY \"table_schema\", \"table_name\", \"position\";";
	}

	public override IEnumerable<string> TriggersQuery()
	{
		synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "t.event_object_schema", "t.event_object_table");
		yield return "SELECT \r\n\t                            pgt.tgname AS \"trigger_name\",\r\n\t                            'TR'::text AS \"TYPE\",\r\n\t                            pgn.nspname AS \"table_schema\",\r\n\t                            pgc.relname AS \"table_name\",\r\n\t                            CURRENT_CATALOG AS \"database_name\",\r\n                                /* TO GET TYPE OF TRIGGER CALCULATE BITWISE AND OF tgtype WITH BIT MASK */\r\n\t                            CASE WHEN pgt.tgtype & 16 <> 0 THEN 1 ELSE 0 END AS \"isupdate\",\r\n\t                            CASE WHEN pgt.tgtype & 8 <> 0 THEN 1 ELSE 0 END AS \"isdelete\",\r\n\t                            CASE WHEN pgt.tgtype & 4 <> 0  THEN 1 ELSE 0 END AS \"isinsert\",\r\n\t                            CASE WHEN pgt.tgtype & 66 = 2 THEN 1 ELSE 0 END AS \"isbefore\",\r\n\t                            CASE WHEN pgt.tgtype & 66 = 0 THEN 1 ELSE 0 END AS \"isafter\",\r\n\t                            CASE WHEN pgt.tgtype & 66 = 64 THEN 1 ELSE 0 END AS \"isinsteadof\",\r\n\t                            CASE WHEN pgt.tgenabled = 'D' THEN 1 ELSE 0 END AS \"disabled\",\r\n\t                            pg_get_triggerdef(pgt.oid) AS \"definition\",\r\n\t                            pgd.description AS \"description\"\r\n                            FROM \r\n\t                            pg_catalog.pg_trigger pgt\r\n                            INNER JOIN pg_catalog.pg_class pgc ON\r\n\t                            pgt.tgrelid = pgc.\"oid\"\r\n                            INNER JOIN pg_catalog.pg_namespace pgn ON\r\n\t                            pgc.relnamespace = pgn.\"oid\"\r\n                            LEFT JOIN pg_catalog.pg_description pgd ON\r\n\t                            pgt.\"oid\" = pgd.objoid\r\n                            WHERE\r\n\t                            NOT pgt.tgisinternal\r\n                            ORDER BY pgt.tgname;\r\n\r\n                            /* source: postgres/src/include/catalog/pg_trigger.h\r\n                             * \r\n                             * Bits within tgtype \r\n                             * \r\n                             * #define TRIGGER_TYPE_ROW\t\t\t\t    (1 << 0) -> 1\r\n                             * #define TRIGGER_TYPE_BEFORE\t\t\t\t(1 << 1) -> 2\r\n                             * #define TRIGGER_TYPE_INSERT\t\t\t\t(1 << 2) -> 4\r\n                             * #define TRIGGER_TYPE_DELETE\t\t\t\t(1 << 3) -> 8\r\n                             * #define TRIGGER_TYPE_UPDATE\t\t\t\t(1 << 4) -> 16\r\n                             * #define TRIGGER_TYPE_TRUNCATE\t\t\t(1 << 5) -> 32\r\n                             * #define TRIGGER_TYPE_INSTEAD\t\t\t    (1 << 6) -> 64\\\r\n                             * #define TRIGGER_TYPE_AFTER\t\t\t\t0\r\n\r\n                             * #define TRIGGER_TYPE_TIMING_MASK \\\r\n                             * \t(TRIGGER_TYPE_BEFORE | TRIGGER_TYPE_INSTEAD) -> 66 \r\n                       \r\n                              */";
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "table_schema", "table_name");
		yield return "SELECT\r\n\t                            \"database_name\",\r\n\t                            \"table_name\",\r\n\t                            \"table_schema\",\r\n\t                            \"name\",\r\n\t                            \"type\",\r\n\t                            pgatt.attname AS \"column_name\",\r\n                                --a.elem,\r\n\t                            a.nr AS \"column_ordinal\",\r\n\t                            \"description\",\r\n\t                            \"disabled\"\r\n                            FROM\r\n                            (\r\n\t                            SELECT\r\n\t\t                            CURRENT_CATALOG AS \"database_name\",\r\n\t\t                            pgr.relname AS \"table_name\",\r\n\t\t                            pgrn.nspname AS \"table_schema\",\r\n\t\t                            pgc.conname AS \"name\",\r\n\t\t                            UPPER(pgc.contype) AS \"type\",\r\n\t\t                            pgc.conkey  AS x,\r\n\t\t                            pgc.conkey,\r\n\t\t                            pgr.\"oid\" AS \"roid\",\r\n\t\t                            pgd.description AS \"description\",\r\n\t\t                            0 AS \"disabled\"\r\n\t                            FROM \r\n\t\t                            pg_catalog.pg_constraint pgc\r\n\t                            INNER JOIN pg_catalog.pg_class pgr ON -- constraint relation\r\n\t\t                            pgc.conrelid = pgr.\"oid\"\r\n\t                            INNER JOIN pg_catalog.pg_namespace pgrn ON -- constraint relation's namespace\r\n\t\t                            pgr.relnamespace = pgrn.\"oid\"\r\n\t                            INNER JOIN pg_catalog.pg_namespace pgcn ON -- constraint namespace\r\n\t\t                            pgc.connamespace = pgcn.\"oid\"\r\n\t                            LEFT JOIN pg_catalog.pg_description pgd ON -- constraint description\r\n\t\t                            pgc.\"oid\" = pgd.objoid\r\n\t                            WHERE\r\n\t\t                            pgc.contype IN ('p', 'u')\r\n\t\t                            AND pgr.relkind IN ('r', 'p')\r\n\t\t                            AND pgrn.nspname NOT IN ( 'pg_catalog','information_schema' )\r\n                            ) AS t,\r\n                            UNNEST(t.x) WITH ORDINALITY a(elem, nr),\r\n                            pg_catalog.pg_attribute pgatt \r\n                            WHERE\r\n\t                            t.\"roid\" = pgatt.attrelid\r\n\t                            AND a.elem = pgatt.attnum\r\n                                " + filterString + ";";
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Function, "pgn.nspname", "pgp.proname");
		yield return "SELECT\r\n\t                            \"database_name\",\r\n\t                            \"procedure_name\",\r\n\t                            \"procedure_schema\",\r\n\t                            CASE \r\n\t\t                            WHEN p.proargnames[args.idx] IS NULL THEN '$'::text || args.idx::TEXT\r\n\t\t                            ELSE p.proargnames[args.idx]\r\n\t                            END AS \"name\",\r\n\t                            args.idx AS \"position\",\r\n\t                            CASE \r\n\t\t                            WHEN p.proargmodes IS NULL THEN 'IN'::text\r\n                                    WHEN p.proargmodes[args.idx] = 'i' THEN 'IN'::text\r\n                                    WHEN p.proargmodes[args.idx] = 'o' THEN 'OUT'::text\r\n                                    WHEN p.proargmodes[args.idx] = 'b' THEN 'INOUT'::text\r\n                                    WHEN p.proargmodes[args.idx] = 'v' THEN 'IN'::text\r\n                                    WHEN p.proargmodes[args.idx] = 't' THEN 'OUT'::text\r\n\t                            END AS \"parameter_mode\",\r\n\t                            CASE \r\n\t\t                            WHEN pgt.typelem <> 0 AND pgt.typlen = -1 THEN 'ARRAY'::text\r\n\t\t                            WHEN pgtn.nspname = 'pg_catalog' THEN format_type(pgt.oid, null)\r\n\t\t                            ELSE pgt.typname\r\n                                 END AS \"datatype\",\r\n                                 NULL AS \"description\",\r\n                                 NULL AS \"data_length\"\r\n                            FROM\r\n                            (\r\n\t                            SELECT\r\n\t\t                            CURRENT_CATALOG AS \"database_name\",\r\n\t\t                            pgp.proname AS \"procedure_name\",\r\n\t\t                            pgn.nspname AS \"procedure_schema\",\r\n\t\t                            COALESCE(pgp.proallargtypes, pgp.proargtypes::oid[]) AS \"args\",\r\n\t\t                            pgp.proargmodes,\r\n\t\t                            pgp.proargnames\r\n\t                            FROM \r\n\t\t                            pg_catalog.pg_proc pgp\r\n\t                            INNER JOIN pg_catalog.pg_namespace pgn ON\r\n\t\t                            pgp.pronamespace = pgn.\"oid\"\r\n\t                            WHERE\r\n\t\t                            pgn.nspname NOT IN ('information_schema', 'pg_catalog')\r\n                                    " + filterString + "\r\n                            ) AS p,\r\n\t                            UNNEST (p.\"args\") WITH ORDINALITY args(elem, idx),\t\r\n\t                            pg_catalog.pg_type pgt,\r\n\t                            pg_catalog.pg_namespace pgtn\r\n                            WHERE\r\n\t                            args.elem = pgt.\"oid\"\r\n\t                            AND pgt.typnamespace = pgtn.\"oid\";";
		filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Function, "pgpn.nspname", "pgp.proname");
		yield return "SELECT\r\n\t                            CURRENT_CATALOG AS \"database_name\",\r\n\t                            pgp.proname AS \"procedure_name\",\r\n\t                            pgpn.nspname AS \"procedure_schema\",\r\n\t                            'Returns' AS \"name\",\r\n\t                            0 AS \"position\",\r\n\t                            'OUT' AS \"parameter_mode\",\r\n\t                            CASE \r\n\t\t                            WHEN pgt.typelem <> 0 AND pgt.typlen = -1 THEN 'ARRAY'::text\r\n\t\t                            WHEN pgtn.nspname = 'pg_catalog' THEN format_type(pgt.oid, null)\r\n\t\t                            ELSE pgt.typname\r\n                                 END AS \"datatype\",\r\n                                 NULL AS \"description\",\r\n                                 NULL AS \"data_length\"\r\n                            FROM\r\n\t                            pg_catalog.pg_proc pgp\r\n                            INNER JOIN pg_catalog.pg_namespace pgpn ON\r\n\t                            pgp.pronamespace = pgpn.\"oid\"\r\n                            INNER JOIN pg_catalog.pg_type pgt ON\r\n\t                            pgp.prorettype = pgt.\"oid\"\r\n                            INNER JOIN pg_catalog.pg_namespace pgtn ON\r\n\t                            pgt.typnamespace = pgtn.\"oid\"\r\n                            WHERE \r\n\t                            pgpn.nspname NOT IN ('information_schema', 'pg_catalog')\r\n                                " + filterString + ";";
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		string text = synchronizeParameters.DatabaseRow.Name.ToUpper();
		string text2 = synchronizeParameters.DatabaseRow.Host.ToUpper();
		string filterStringForDependencies = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("dependent_ns.nspname", "dependent_view.relname", "(CASE dependent_view.relkind WHEN 'v' then 'VIEW'::text WHEN 'm' THEN 'VIEW'::text END)", "CASE source_table.relkind\r\n                    WHEN 'r' THEN 'TABLE'::text\r\n                        WHEN 'f' THEN 'TABLE'::text\r\n                        WHEN 'p' THEN 'TABLE'::text\r\n                        WHEN 'i' THEN 'INDEX'::text\r\n                        WHEN 'S' THEN 'SEQUENCE'::text\r\n                        WHEN 'v' THEN 'VIEW'::text\r\n                        WHEN 'm' THEN 'VIEW'::text\r\n                    END");
		string filterStringForDependencies2 = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("source_ns.nspname ", "source_table.relname", "CASE source_table.relkind\r\n                    WHEN 'r' THEN 'TABLE'::text\r\n                        WHEN 'f' THEN 'TABLE'::text\r\n                        WHEN 'p' THEN 'TABLE'::text\r\n                        WHEN 'i' THEN 'INDEX'::text\r\n                        WHEN 'S' THEN 'SEQUENCE'::text\r\n                        WHEN 'v' THEN 'VIEW'::text\r\n                        WHEN 'm' THEN 'VIEW'::text\r\n                    END", "(CASE dependent_view.relkind WHEN 'v' then 'VIEW'::text WHEN 'm' THEN 'VIEW'::text END)");
		string text3 = FilterRulesCollection.CombineDependenciesFilter(filterStringForDependencies, filterStringForDependencies2);
		yield return "SELECT DISTINCT CASE dependent_view.relkind\r\n                            WHEN 'r' THEN 'TABLE'::text\r\n                            WHEN 'f' THEN 'TABLE'::text\r\n                            WHEN 'p' THEN 'TABLE'::text\r\n                            WHEN 'v' THEN 'VIEW'::text\r\n                            WHEN 'm' THEN 'VIEW'::text\r\n                       END as referencing_type,  \r\n                       dependent_ns.nspname as referencing_schema_name,\r\n                       '" + text2 + "'::text as referencing_server,\r\n                       '" + text + "'::text as referencing_database_name,\r\n                       dependent_view.relname as referencing_entity_name,\r\n                       '" + text2 + "'::text as referenced_server,\r\n                       '" + text + "'::text as referenced_database_name,\r\n                       source_ns.nspname as referenced_schema_name,\r\n                       CASE source_table.relkind\r\n                            WHEN 'r' THEN 'TABLE'::text\r\n                            WHEN 'f' THEN 'TABLE'::text\r\n                            WHEN 'p' THEN 'TABLE'::text\r\n                            WHEN 'v' THEN 'VIEW'::text\r\n                            WHEN 'm' THEN 'VIEW'::text\r\n                       END as referenced_type, \r\n                       source_table.relname as referenced_entity_name,\r\n                       null as is_caller_dependent,\r\n                       null as is_ambiguous,\r\n                       NULL as dependency_type\r\n                  FROM pg_depend \r\n                  INNER JOIN pg_rewrite \r\n                    ON pg_depend.objid = pg_rewrite.oid \r\n                  INNER JOIN pg_class as dependent_view \r\n                    ON pg_rewrite.ev_class = dependent_view.oid \r\n                  INNER JOIN pg_class as source_table \r\n                    ON pg_depend.refobjid = source_table.oid \r\n                  INNER JOIN pg_namespace dependent_ns \r\n                    ON dependent_ns.oid = dependent_view.relnamespace\r\n                  INNER JOIN pg_namespace source_ns \r\n                    ON source_ns.oid = source_table.relnamespace\r\n                 WHERE dependent_ns.nspname NOT IN('pg_catalog', 'information_schema')\r\n                         AND source_table.oid <> dependent_view.oid\r\n                            AND dependent_view.relname IS NOT NULL\r\n                            AND source_table.relname IS NOT NULL\r\n                   " + text3 + "\r\n                 ORDER BY 2, 3;";
	}
}
