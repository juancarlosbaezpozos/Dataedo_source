using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.General;
using Dataedo.App.Data.QueryTools;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Shared.Enums;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.Data.SqlServer;

public class SynchronizeDB : SynchronizeDatabase
{
	protected virtual SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.SqlServer;

	protected virtual bool WithCustomFields => true;

	public override IEnumerable<string> CountTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "sch.[name]", "s.[name]");
		yield return "SELECT 'TABLE' AS 'object_type'\r\n                              ,COUNT(1) AS 'count'\r\n                         FROM\r\n                        sys.tables s\r\n                              INNER JOIN sys.schemas sch ON sch.schema_id = s.schema_id\r\n                        WHERE ISNULL(OBJECTPROPERTY (s.object_id, 'IsMSShipped'), 0) = 0\r\n                            AND\r\n                            (\r\n                                SELECT\r\n                                    major_id\r\n                                FROM\r\n                                    sys.extended_properties\r\n                                WHERE\r\n                                    major_id = s.[object_id] AND\r\n                                    minor_id = 0 AND\r\n                                    class = 1 AND\r\n                                    name = N'microsoft_database_tools_support'\r\n                            ) IS NULL\r\n                                   " + filterString;
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "sch.[name]", "s.[name]");
		yield return "SELECT 'VIEW' AS 'object_type'\r\n                          ,COUNT(1) AS 'count'\r\n                     FROM sys.views s\r\n                              INNER JOIN sys.schemas sch ON sch.schema_id = s.schema_id\r\n                     WHERE ISNULL(OBJECTPROPERTY (s.object_id, 'IsMSShipped'), 0) = 0\r\n                    AND\r\n                    (\r\n                        SELECT\r\n                            major_id\r\n                        FROM\r\n                            sys.extended_properties\r\n                        WHERE\r\n                            major_id = s.[object_id] AND\r\n                            minor_id = 0 AND\r\n                            class = 1 AND\r\n                            name = N'microsoft_database_tools_support'\r\n                    ) IS NULL\r\n                    " + filterString;
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "I.SPECIFIC_SCHEMA", "I.SPECIFIC_NAME");
		yield return "SELECT 'PROCEDURE' AS 'object_type'\r\n                              ,COUNT(1) AS 'count'\r\n                         FROM INFORMATION_SCHEMA.ROUTINES I\r\n                         INNER JOIN sys.procedures AS s ON I.SPECIFIC_NAME = s.name AND\r\n                            I.SPECIFIC_SCHEMA = (SELECT name FROM sys.schemas WHERE schema_id = s.schema_id)\r\n                         LEFT JOIN sys.sql_modules AS def ON def.object_id = s.object_id\r\n                         WHERE I.ROUTINE_TYPE = 'PROCEDURE'\r\n                              AND ISNULL(OBJECTPROPERTY (OBJECT_ID (I.ROUTINE_NAME), 'IsMSShipped'), 0) = 0\r\n                              AND (\r\n                                   SELECT major_id\r\n                                   FROM sys.extended_properties\r\n                                   WHERE major_id = OBJECT_ID(I.ROUTINE_NAME)\r\n                                        AND minor_id = 0\r\n                                        AND class = 1\r\n                                        AND name = N'microsoft_database_tools_support'\r\n                                   ) IS NULL\r\n                            " + filterString;
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "I.SPECIFIC_SCHEMA", "I.SPECIFIC_NAME");
		yield return "SELECT 'FUNCTION' AS 'object_type'\r\n                              ,COUNT(1) AS 'count'\r\n                         FROM INFORMATION_SCHEMA.ROUTINES I\r\n                         INNER JOIN sys.objects s\r\n                              ON I.SPECIFIC_NAME = s.name\r\n                                   AND I.SPECIFIC_SCHEMA = (SELECT name FROM sys.schemas WHERE schema_id = s.schema_id)\r\n                         LEFT JOIN sys.sql_modules AS def ON def.object_id = s.object_id\r\n                         WHERE I.ROUTINE_TYPE = 'FUNCTION'\r\n                                   AND s.type IN ('FN', 'IF', 'TF', 'AF', 'FS', 'FT')\r\n                              AND ISNULL(OBJECTPROPERTY (s.object_id, 'IsMSShipped'), 0) = 0\r\n                              AND (\r\n                                   SELECT major_id\r\n                                   FROM sys.extended_properties\r\n                                   WHERE major_id = s.object_id\r\n                                        AND minor_id = 0\r\n                                        AND class = 1\r\n                                        AND name = N'microsoft_database_tools_support'\r\n                                   ) IS NULL\r\n                            " + filterString;
	}

	protected virtual string GetTableTypesQuery()
	{
		DatabaseVersionUpdate versionUpdate = synchronizeParameters.DatabaseRow.GetVersionUpdate();
		if (versionUpdate.Version == 13)
		{
			return "CASE\r\n                                        WHEN s.IS_EXTERNAL = 1 THEN 'EXTERNAL_TABLE'\r\n                                        WHEN s.TEMPORAL_TYPE = 2 THEN 'SYSTEM_VERSIONED_TABLE'\r\n                                        WHEN s.TEMPORAL_TYPE = 1 THEN 'HISTORY_TABLE'\r\n                                        WHEN s.IS_FILETABLE = 1 THEN 'FILE_TABLE'\r\n                                        ELSE 'TABLE'\r\n                                    END AS [TYPE]";
		}
		if (versionUpdate.Version >= 14)
		{
			return "CASE\r\n                                        WHEN s.IS_EXTERNAL = 1 THEN 'EXTERNAL_TABLE'\r\n                                        WHEN s.IS_NODE = 1 THEN 'GRAPH_NODE_TABLE'\r\n                                        WHEN s.IS_EDGE = 1 THEN 'GRAPH_EDGE_TABLE'\r\n                                        WHEN s.TEMPORAL_TYPE = 2 THEN 'SYSTEM_VERSIONED_TABLE'\r\n                                        WHEN s.TEMPORAL_TYPE = 1 THEN 'HISTORY_TABLE'\r\n                                        WHEN s.IS_FILETABLE = 1 THEN 'FILE_TABLE'\r\n                                        ELSE 'TABLE'\r\n                                    END AS [TYPE]";
		}
		return "'TABLE' AS [TYPE]";
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "sch.[name]", "s.[name]");
		string tableTypesQuery = GetTableTypesQuery();
		DocumentationCustomFieldRow[] customFields = synchronizeParameters.CustomFields;
		string[] fields = customFields?.Select((DocumentationCustomFieldRow x) => x.FieldName).ToArray();
		string[] eps = customFields?.Select((DocumentationCustomFieldRow x) => x.ExtendedPropertyForQueries)?.ToArray();
		string epSelect = GetEpSelect(fields);
		yield return "SELECT\r\n                        s.[name] AS name,\r\n                        sch.[name] AS [schema],\r\n                        db_name() AS database_name,\r\n                        " + tableTypesQuery + ",\r\n                              td.value AS [description],\r\n                        '' AS definition,\r\n                        s.create_date,\r\n                         s.modify_date,\r\n                        NULL AS function_type\r\n                              " + epSelect + "\r\n                    FROM\r\n                        sys.tables s\r\n                              INNER JOIN sys.schemas sch ON sch.schema_id = s.schema_id\r\n                              LEFT JOIN sys.extended_properties td\r\n                                   ON\t\ttd.major_id = s.[object_id]\r\n                                   AND \ttd.minor_id = 0\r\n                                   AND\t\ttd.name = 'MS_Description'\r\n                              " + GetEpJoin("s.[object_id]", eps) + "\r\n                              WHERE ISNULL(OBJECTPROPERTY (s.[object_id], 'IsMSShipped'), 0) = 0\r\n                        AND\r\n                            (\r\n                                SELECT\r\n                                    major_id\r\n                                FROM\r\n                                    sys.extended_properties\r\n                                WHERE\r\n                                    major_id = s.[object_id] AND\r\n                                    minor_id = 0 AND\r\n                                    class = 1 AND\r\n                                    name = N'microsoft_database_tools_support'\r\n                            ) IS NULL\r\n                              " + filterString;
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "sch.[name]", "s.[name]");
		DocumentationCustomFieldRow[] customFields = synchronizeParameters.CustomFields;
		string[] fields = customFields?.Select((DocumentationCustomFieldRow x) => x.FieldName).ToArray();
		string[] eps = customFields?.Select((DocumentationCustomFieldRow x) => x.ExtendedPropertyForQueries)?.ToArray();
		string epSelect = GetEpSelect(fields);
		yield return "SELECT\r\n                        s.[name] AS name,\r\n                        sch.[name] AS [schema],\r\n                        db_name() AS database_name,\r\n                        'VIEW' AS [type],\r\n                              td.value AS [description],\r\n                              def.definition,\r\n                        s.create_date,\r\n                         s.modify_date,\r\n                        NULL AS function_type\r\n                        " + epSelect + "\r\n                    FROM\r\n                        sys.views s\r\n                              INNER JOIN sys.schemas sch ON sch.schema_id = s.schema_id\r\n                              LEFT JOIN sys.extended_properties td\r\n                              ON\t\ttd.major_id = s.[object_id]\r\n                                   AND \ttd.minor_id = 0\r\n                                   AND\t\ttd.name = 'MS_Description'\r\n                              INNER JOIN sys.sql_modules AS def\r\n                              ON def.object_id = s.object_id\r\n                        " + GetEpJoin("s.[object_id]", eps) + "\r\n                        WHERE ISNULL(OBJECTPROPERTY (s.[object_id], 'IsMSShipped'), 0) = 0\r\n                        AND\r\n                            (\r\n                                SELECT\r\n                                    major_id\r\n                                FROM\r\n                                    sys.extended_properties\r\n                                WHERE\r\n                                    major_id = s.[object_id] AND\r\n                                    minor_id = 0 AND\r\n                                    class = 1 AND\r\n                                    name = N'microsoft_database_tools_support'\r\n                            ) IS NULL\r\n                            " + filterString;
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "sch.[name]", "s.[name]");
		DocumentationCustomFieldRow[] customFields = synchronizeParameters.CustomFields;
		string[] fields = customFields?.Select((DocumentationCustomFieldRow x) => x.FieldName).ToArray();
		string[] eps = customFields?.Select((DocumentationCustomFieldRow x) => x.ExtendedPropertyForQueries)?.ToArray();
		string epSelect = GetEpSelect(fields);
		yield return "SELECT s.[name] AS [name],\r\n                sch.[name] AS [schema],\r\n                DB_NAME() AS [database_name],\r\n                CASE s.[type]\r\n                    WHEN 'P'\r\n                        THEN 'PROCEDURE'\r\n                    WHEN 'PC'\r\n                        THEN 'CLR_PROCEDURE'\r\n                    WHEN 'X'\r\n                        THEN 'EXTENDED_PROCEDURE'\r\n                    END AS [type],\r\n                sep.[value] AS [description],\r\n                def.[definition],\r\n                s.create_date,\r\n                s.modify_date AS modify_date,\r\n                NULL AS function_type\r\n            " + epSelect + "\r\n            FROM sys.procedures AS s\r\n            LEFT JOIN sys.sql_modules AS def ON def.object_id = s.object_id\r\n            INNER JOIN sys.schemas sch ON sch.schema_id = s.schema_id\r\n            LEFT JOIN sys.extended_properties sep ON sep.major_id = s.object_id\r\n                AND sep.minor_id = 0\r\n                AND sep.class = 1\r\n                AND sep.[name] = 'MS_Description'\r\n            " + GetEpJoin("s.object_id", eps) + "\r\n            WHERE (\r\n                    s.[type] = 'P'\r\n                    OR s.[type] = 'PC'\r\n                    OR s.[type] = 'X'\r\n                    )\r\n                AND ISNULL(OBJECTPROPERTY(s.object_id, 'IsMSShipped'), 0) = 0\r\n                AND (\r\n                    SELECT major_id\r\n                    FROM sys.extended_properties\r\n                    WHERE major_id = s.object_id\r\n                        AND minor_id = 0\r\n                        AND class = 1\r\n                        AND [name] = N'microsoft_database_tools_support'\r\n                    ) IS NULL\r\n            " + filterString + " ";
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "sch.[name]", "s.[name]");
		DocumentationCustomFieldRow[] customFields = synchronizeParameters.CustomFields;
		string[] fields = customFields?.Select((DocumentationCustomFieldRow x) => x.FieldName).ToArray();
		string[] eps = customFields?.Select((DocumentationCustomFieldRow x) => x.ExtendedPropertyForQueries)?.ToArray();
		string epSelect = GetEpSelect(fields);
		yield return "SELECT s.name AS name,\r\n                sch.name AS [schema],\r\n                '" + synchronizeParameters.DatabaseName + "' AS database_name, \r\n                CASE s.type\r\n                    WHEN 'FT'\r\n                        THEN 'CLR_FUNCTION'\r\n                    WHEN 'FS'\r\n                        THEN 'CLR_FUNCTION'\r\n                    WHEN 'AF'\r\n                        THEN 'CLR_FUNCTION'\r\n                    ELSE 'FUNCTION'\r\n                    END AS [type],\r\n                sep.value AS [description],\r\n                def.DEFINITION,\r\n                s.create_date,\r\n                s.modify_date AS modify_date,\r\n                CASE type\r\n                    WHEN 'FN'\r\n                        THEN 'Scalar-valued'\r\n                    WHEN 'IF'\r\n                        THEN 'Table-valued'\r\n                    WHEN 'TF'\r\n                        THEN 'Table-valued'\r\n                    END AS function_type\r\n    \t\t        " + epSelect + "\r\n                FROM sys.objects AS s\r\n                INNER JOIN sys.schemas sch ON sch.schema_id = s.schema_id\r\n                LEFT JOIN sys.sql_modules AS def ON def.object_id = s.object_id\r\n                LEFT JOIN sys.extended_properties sep ON sep.major_id = s.object_id\r\n                    AND sep.minor_id = 0\r\n                AND class = 1\r\n                AND sep.name = 'MS_Description'\r\n    \t        " + GetEpJoin("s.object_id", eps) + "\r\n                WHERE ISNULL(OBJECTPROPERTY(s.object_id, 'IsMSShipped'), 0) = 0\r\n                    AND s.type IN(\r\n                        'FN',\r\n                        'IF',\r\n                        'TF',\r\n                        'AF',\r\n                        'FS',\r\n                        'FT'\r\n                        )\r\n                    AND(\r\n                        SELECT major_id\r\n                        FROM sys.extended_properties\r\n                        WHERE major_id = s.object_id\r\n                            AND minor_id = 0\r\n                            AND class = 1\r\n                            AND name = N'microsoft_database_tools_support'\r\n                            ) IS NULL\r\n\t\t        " + filterString;
	}

	private string GetEpSelectForConstraints(string[] fields)
	{
		int num = fields.Length;
		StringBuilder stringBuilder = new StringBuilder();
		string arg = "epp";
		string arg2 = "ep";
		stringBuilder.AppendLine();
		for (int i = 0; i < num; i++)
		{
			string text = fields[i];
			string text2 = $"{arg}{i + 1}";
			string text3 = $"{arg2}{i + 1}";
			stringBuilder.AppendLine(",CASE WHEN i.is_primary_key = 1 THEN [" + text2 + "].[value] ELSE [" + text3 + "].[value] END as [" + text + "]");
		}
		return stringBuilder.ToString();
	}

	private string GetEpSelect(string[] fields)
	{
		int num = fields.Length;
		StringBuilder stringBuilder = new StringBuilder();
		string arg = "ep";
		stringBuilder.AppendLine();
		for (int i = 0; i < num; i++)
		{
			string text = fields[i];
			string text2 = $"{arg}{i + 1}";
			stringBuilder.AppendLine(",[" + text2 + "].[value] as [" + text + "]");
		}
		return stringBuilder.ToString();
	}

	private string GetEmptyEpSelect(string[] fields)
	{
		int num = fields.Length;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine();
		for (int i = 0; i < num; i++)
		{
			string text = fields[i];
			stringBuilder.AppendLine(",'' as [" + text + "]");
		}
		return stringBuilder.ToString();
	}

	private string GetEpJoin(string objectIdColumn, string[] eps, string baseAlias = "ep", string className = "1")
	{
		int num = eps.Length;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine();
		for (int i = 0; i < num; i++)
		{
			string text = eps[i];
			string text2 = $"[{baseAlias}{i + 1}]";
			stringBuilder.AppendLine("LEFT JOIN [sys].[extended_properties] " + text2);
			stringBuilder.AppendLine("ON " + text2 + ".[major_id] = " + objectIdColumn);
			stringBuilder.AppendLine("AND " + text2 + ".[name] = (N'" + text + "')");
			stringBuilder.AppendLine("AND " + text2 + ".[minor_id] = 0");
			stringBuilder.AppendLine("AND " + text2 + ".[class] = " + className);
		}
		return stringBuilder.ToString();
	}

	private string GetEpJoin(string baseObjectIdColumn, string objectIdColumn, string[] eps, string baseAlias = "ep", string className = "1")
	{
		int num = eps.Length;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine();
		for (int i = 0; i < num; i++)
		{
			string text = eps[i];
			string text2 = $"[{baseAlias}{i + 1}]";
			stringBuilder.AppendLine("LEFT JOIN [sys].[extended_properties] " + text2);
			stringBuilder.AppendLine("ON " + text2 + ".[major_id] = " + baseObjectIdColumn);
			stringBuilder.AppendLine("AND " + text2 + ".[name] = (N'" + text + "')");
			stringBuilder.AppendLine("AND " + text2 + ".[minor_id] = " + objectIdColumn);
			stringBuilder.AppendLine("AND " + text2 + ".[class] = " + className);
		}
		return stringBuilder.ToString();
	}

	public override IEnumerable<string> RelationsQuery()
	{
		DocumentationCustomFieldRow[] customFields = synchronizeParameters.CustomFields;
		string[] fields = customFields?.Select((DocumentationCustomFieldRow x) => x.FieldName).ToArray();
		string[] eps = customFields?.Select((DocumentationCustomFieldRow x) => x.ExtendedPropertyForQueries)?.ToArray();
		string epSelect = GetEpSelect(fields);
		string epJoin = GetEpJoin("[skc].[constraint_object_id]", eps);
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString(null, base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "[fks].[name]", "[fkt].[name]");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("OR", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "[pks].[name]", "[pkt].[name]");
		string text = FilterRulesCollection.CombineRelationsFilter(filterString, filterString2);
		yield return "SELECT\r\n                                               [sfk].[name] AS [NAME],\r\n                                               NULL AS [FK_TABLE_DATABASE_NAME],\r\n                                               [fkt].[name] AS [FK_TABLE_NAME],\r\n                                               [fks].[name] AS [FK_TABLE_SCHEMA],\r\n                                               [fkc].[name] AS [FK_COLUMN],\r\n                                               NULL AS [REF_TABLE_DATABASE_NAME],\r\n                                               [pkt].[name] AS [REF_TABLE_NAME],\r\n                                               [pks].[name] AS [REF_TABLE_SCHEMA],\r\n                                               [pkc].[name] AS [REF_COLUMN],\r\n                                               [skc].[constraint_column_id] AS [ORDINAL_POSITION],\r\n                                               [ep].[value] as [DESCRIPTION],\r\n                                               [sfk].[update_referential_action_desc] AS [UPDATE_RULE],\r\n                                               [sfk].[delete_referential_action_desc] AS [DELETE_RULE]\r\n                                               " + epSelect + "\r\n                                               FROM [sys].[foreign_keys] [sfk]\r\n                                               INNER JOIN [sys].[foreign_key_columns] [skc] ON [sfk].object_id = [skc].[constraint_object_id]\r\n                                               INNER JOIN [sys].[tables] [fkt] ON [fkt].[object_id] = [skc].[parent_object_id]\r\n                                               INNER JOIN [sys].[schemas] [fks] ON [fkt].[schema_id] = [fks].[schema_id]\r\n                                               INNER JOIN [sys].[columns] [fkc] ON [fkt].[object_id] = [fkc].[object_id]\r\n                                                    AND [skc].[parent_column_id] = [fkc].[column_id]\r\n                                               INNER JOIN [sys].[tables] [pkt] ON [pkt].[object_id] = [skc].[referenced_object_id]\r\n                                               INNER JOIN [sys].[schemas] [pks] ON [pkt].[schema_id] = [pks].[schema_id]\r\n                                               INNER JOIN [sys].[columns] [pkc] ON [pkt].[object_id] = [pkc].[object_id]\r\n                                                    AND [skc].[referenced_column_id] = [pkc].[column_id]\r\n                                               LEFT JOIN [sys].[extended_properties] [ep] ON [skc].[constraint_object_id] = [ep].[major_id]\r\n                                                    AND [ep].[minor_id] = 0\r\n                                                    AND [ep].[name] = 'MS_Description'\r\n                                                    AND [ep].[class] = 1\r\n                                                " + epJoin + "\r\n                                                WHERE 1 = 1\r\n                                                " + text;
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		DocumentationCustomFieldRow[] customFields = synchronizeParameters.CustomFields;
		string[] fields = customFields?.Select((DocumentationCustomFieldRow x) => x.FieldName).ToArray();
		string[] eps = customFields?.Select((DocumentationCustomFieldRow x) => x.ExtendedPropertyForQueries)?.ToArray();
		string epSelect = GetEpSelect(fields);
		string epJoin = GetEpJoin("sc.[object_id]", "sc.[column_id]", eps);
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "IC.TABLE_SCHEMA", "IC.TABLE_NAME");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.View, "IC.TABLE_SCHEMA", "IC.TABLE_NAME");
		yield return "SELECT\r\n                                            IC.TABLE_CATALOG AS database_name,\r\n                                            IC.TABLE_NAME AS [table_name],\r\n                                            IC.TABLE_SCHEMA AS [table_schema],\r\n                                            IC.COLUMN_NAME AS [name],\r\n                                            IC.ORDINAL_POSITION AS [position],\r\n                                            CASE \r\n                                                            WHEN IC.DOMAIN_NAME IS NULL OR IC.DOMAIN_NAME = '' THEN IC.DATA_TYPE\r\n                                                            ELSE IC.DOMAIN_NAME + ': ' + IC.DATA_TYPE END  AS [datatype],\r\n                                            ep.[value] AS [description],\r\n                                            CASE IPK.CONSTRAINT_TYPE WHEN 'PRIMARY KEY' THEN 'P' END AS [constraint_type],\r\n                                            CASE WHEN IC.IS_NULLABLE= 'YES' THEN 1 ELSE 0 END AS [nullable],\r\n                                            IC.COLUMN_DEFAULT as [default_value],\r\n                                            sc.is_identity,\r\n                                            sc.is_computed,\r\n                                            cmp.definition as [computed_formula],\r\n                                            " + QueryDataTypes.GetQueryForDataLength(synchronizeParameters.DatabaseRow.Type) + "\r\n                                            " + epSelect + "\r\n                                        FROM\r\n                                            INFORMATION_SCHEMA.COLUMNS IC\r\n                                            INNER JOIN\r\n                                                sys.columns sc\r\n                                            ON\r\n                                                OBJECT_ID(QUOTENAME(IC.TABLE_SCHEMA) + '.' + QUOTENAME(IC.TABLE_NAME)) = sc.[object_id]\r\n                                                AND IC.COLUMN_NAME = sc.name\r\n                                            LEFT JOIN\r\n                                                sys.extended_properties ep\r\n                                            ON\r\n                                                sc.[object_id] = ep.major_id\r\n                                                AND sc.[column_id] = ep.minor_id\r\n                                                AND ep.name = 'MS_Description'\r\n                                                AND ep.class = 1\r\n                                            LEFT JOIN\r\n                                                (SELECT\r\n                                                                 IKU.COLUMN_NAME,\r\n                                                                 IKU.TABLE_NAME,\r\n                                                                 IKU.TABLE_SCHEMA,\r\n                                                                 IKU.TABLE_CATALOG,\r\n                                                                 ITC.CONSTRAINT_TYPE\r\n                                                            FROM\r\n                                                                 INFORMATION_SCHEMA.KEY_COLUMN_USAGE IKU\r\n                                                                 INNER JOIN\r\n                                                                      INFORMATION_SCHEMA.TABLE_CONSTRAINTS ITC\r\n                                                                 ON\r\n                                                                      ITC.TABLE_NAME = IKU.TABLE_NAME\r\n                                                                      AND ITC.CONSTRAINT_NAME = IKU.CONSTRAINT_NAME\r\n                                                                      AND ITC.TABLE_SCHEMA = IKU.TABLE_SCHEMA) IPK\r\n                                                            ON\r\n                                                                 IPK.COLUMN_NAME = IC.COLUMN_NAME\r\n                                                                 AND IPK.TABLE_NAME = IC.TABLE_NAME\r\n                                                                 AND IPK.TABLE_SCHEMA = IC.TABLE_SCHEMA\r\n                                                                 AND IPK.TABLE_CATALOG = IC.TABLE_CATALOG\r\n                                                                 AND IPK.CONSTRAINT_TYPE = 'PRIMARY KEY'\r\n                                            LEFT JOIN\r\n                                                sys.computed_columns cmp\r\n                                            ON\r\n                                                sc.[object_id] = cmp.[object_id]\r\n                                                AND sc.[column_id] = cmp.[column_id]\r\n                                                " + epJoin + "\r\n                                        WHERE\r\n                                            (OBJECTPROPERTY(OBJECT_ID(QUOTENAME(IC.TABLE_SCHEMA) + '.' + QUOTENAME(IC.TABLE_NAME)), 'IsView')=1 " + filterString2 + ")\r\n                                            OR (OBJECTPROPERTY(OBJECT_ID(QUOTENAME(IC.TABLE_SCHEMA) + '.' + QUOTENAME(IC.TABLE_NAME)), 'IsView')=0 " + filterString + ")";
	}

	public override IEnumerable<string> TriggersQuery()
	{
		DocumentationCustomFieldRow[] customFields = synchronizeParameters.CustomFields;
		string[] fields = customFields?.Select((DocumentationCustomFieldRow x) => x.FieldName).ToArray();
		string[] eps = customFields?.Select((DocumentationCustomFieldRow x) => x.ExtendedPropertyForQueries)?.ToArray();
		string epSelect = GetEpSelect(fields);
		string epJoin = GetEpJoin("o.id", eps);
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "I.TABLE_SCHEMA", "OBJECT_NAME(o.parent_obj)");
		yield return "SELECT\r\n                                             o.name AS trigger_name\r\n                                            ,I.TABLE_SCHEMA AS table_schema \r\n                                            ,OBJECT_NAME(o.parent_obj) AS table_name\r\n                                            ,I.TABLE_CATALOG AS database_name\r\n                                            ,OBJECTPROPERTY(o.id, 'ExecIsUpdateTrigger') AS isupdate\r\n                                            ,OBJECTPROPERTY(o.id, 'ExecIsDeleteTrigger') AS isdelete\r\n                                            ,OBJECTPROPERTY(o.id, 'ExecIsInsertTrigger') AS isinsert\r\n                                            ,NULL AS isbefore\r\n                                            ,OBJECTPROPERTY(o.id, 'ExecIsAfterTrigger') AS isafter\r\n                                            ,OBJECTPROPERTY(o.id, 'ExecIsInsteadOfTrigger') AS isinsteadof\r\n                                            ,OBJECTPROPERTY(o.id, 'ExecIsTriggerDisabled') AS [disabled]\r\n                                            ,def.definition AS definition\r\n                                            ,ep.[value] AS [description]\r\n                                            ,o.[type]" + epSelect + "FROM sysobjects AS o\r\n                                        INNER JOIN sysobjects AS o2 ON o.parent_obj = o2.id\r\n                                        INNER JOIN sys.schemas s ON s.schema_id = o.uid\r\n                                        INNER JOIN INFORMATION_SCHEMA.TABLES I ON I.TABLE_NAME = OBJECT_NAME(o.parent_obj)\r\n                                             AND s.name = I.TABLE_SCHEMA\r\n                                        LEFT OUTER JOIN sys.sql_modules AS def ON o.id = def.object_id\r\n                                        LEFT OUTER JOIN\r\n                                                sys.extended_properties ep\r\n                                            ON\r\n                                                o.id = ep.major_id\r\n                                                AND ep.minor_id = 0\r\n                                                AND ep.name = 'MS_Description'\r\n                                                AND ep.class = 1" + epJoin + "WHERE o.type in ('TR', 'TA') " + filterString;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		DocumentationCustomFieldRow[] customFields = synchronizeParameters.CustomFields;
		string[] fields = customFields?.Select((DocumentationCustomFieldRow x) => x.FieldName).ToArray();
		string[] eps = customFields?.Select((DocumentationCustomFieldRow x) => x.ExtendedPropertyForQueries)?.ToArray();
		string epSelectForConstraints = GetEpSelectForConstraints(fields);
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "C.TABLE_SCHEMA", "C.TABLE_NAME");
		yield return "SELECT\r\n                        C.TABLE_CATALOG AS database_name,\r\n                        C.TABLE_NAME AS [table_name],\r\n                        C.TABLE_SCHEMA AS table_schema,\r\n                        i.name AS name,\r\n                        CASE is_primary_key\r\n                             WHEN 1 then 'P'\r\n                             ELSE 'U'\r\n                        END AS [type],\r\n                        C.COLUMN_NAME AS column_name,\r\n                        ic.key_ordinal AS column_ordinal,\r\n                        CASE WHEN i.is_primary_key = 1\r\n                             THEN epp.[value]\r\n                             ELSE ep.[value]\r\n                        END AS [description],\r\n                        i.is_disabled AS [disabled]" + epSelectForConstraints + "FROM\r\n                        sys.indexes i\r\n                        INNER JOIN sys.index_columns ic\r\n                             ON i.index_id = ic.index_id\r\n                             AND i.object_id = ic.object_id\r\n                        LEFT OUTER JOIN sys.extended_properties ep\r\n                             ON i.[object_id] = ep.major_id\r\n                             AND i.index_id = ep.minor_id\r\n                             AND ep.name = 'MS_Description'\r\n                             AND ep.class = 7 --INDEX\r\n                        LEFT OUTER JOIN sysobjects so\r\n                             ON so.parent_obj = i.[object_id] AND so.xtype = 'PK'\r\n                        INNER JOIN sysobjects so2\r\n                             ON i.[object_id] = so2.id\r\n                        INNER JOIN sys.schemas sch\r\n                             ON sch.schema_id = so2.uid\r\n                        INNER JOIN INFORMATION_SCHEMA.COLUMNS C\r\n                             ON OBJECT_NAME(i.object_id) = C.TABLE_NAME\r\n                             AND sch.name = C.TABLE_SCHEMA\r\n                             AND COL_NAME (i.object_id, ic.column_id) = C.COLUMN_NAME\r\n                        LEFT OUTER JOIN sys.extended_properties epp\r\n                             ON so.id = epp.major_id\r\n                             AND epp.minor_id = 0\r\n                             AND epp.name = 'MS_Description'\r\n                             AND epp.class = 1" + GetEpJoin("i.[object_id]", "i.index_id", eps, "ep", "7") + GetEpJoin("so.id", eps, "epp") + "WHERE (i.is_unique = 1 OR i.is_primary_key = 1)" + filterString;
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		DocumentationCustomFieldRow[] customFields = synchronizeParameters.CustomFields;
		string[] fields = customFields?.Select((DocumentationCustomFieldRow x) => x.FieldName).ToArray();
		string[] eps = customFields?.Select((DocumentationCustomFieldRow x) => x.ExtendedPropertyForQueries)?.ToArray();
		string epSelect = GetEpSelect(fields);
		string epEmptySelect = GetEmptyEpSelect(fields);
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Procedure, "IC.SPECIFIC_SCHEMA", "IC.SPECIFIC_NAME");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Function, "IC.SPECIFIC_SCHEMA", "IC.SPECIFIC_NAME");
		string secondFilterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("WHERE", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Function, "RC.TABLE_SCHEMA", "RC.TABLE_NAME");
		yield return "SELECT\r\n                            IC.SPECIFIC_CATALOG AS database_name,\r\n                        IC.SPECIFIC_NAME AS [procedure_name],\r\n                        IC.SPECIFIC_SCHEMA AS [procedure_schema],\r\n                            SUBSTRING(IC.PARAMETER_NAME, 2, LEN(IC.PARAMETER_NAME)) AS [name],\r\n                            IC.ORDINAL_POSITION AS [position],\r\n                            IC.PARAMETER_MODE AS [parameter_mode],\r\n                            IC.DATA_TYPE AS [datatype],\r\n                            " + QueryDataTypes.GetQueryForDataLength(synchronizeParameters.DatabaseRow.Type) + "\r\n                        , ep.[value] AS [description]\r\n                        " + epSelect + "\r\n                    FROM\r\n                            INFORMATION_SCHEMA.PARAMETERS IC\r\n                                    INNER JOIN\r\n                                        sys.all_parameters p\r\n                                    ON p.object_id = OBJECT_ID(QUOTENAME(IC.SPECIFIC_SCHEMA) + '.' + QUOTENAME(IC.SPECIFIC_NAME))\r\n                                        AND IC.PARAMETER_NAME = p.name\r\n                                    LEFT OUTER JOIN\r\n                            sys.extended_properties ep\r\n                        ON\r\n                            ep.major_id = p.object_id\r\n                            AND ep.minor_id = p.parameter_id\r\n                            AND ep.name = 'MS_Description'\r\n                            AND ep.class = 2 \r\n                            " + GetEpJoin("p.object_id", "p.parameter_id", eps, "ep", "2") + "\r\n                    WHERE\r\n                        (OBJECTPROPERTY(OBJECT_ID(QUOTENAME(IC.SPECIFIC_SCHEMA) + '.' + QUOTENAME(IC.SPECIFIC_NAME)),'IsProcedure')=1 " + filterString + ")\r\n                        OR (OBJECTPROPERTY(OBJECT_ID(QUOTENAME(IC.SPECIFIC_SCHEMA) + '.' + QUOTENAME(IC.SPECIFIC_NAME)),'IsProcedure')=0 " + filterString2 + ")";
		yield return "SELECT DISTINCT\r\n                        RC.TABLE_CATALOG AS database_name,\r\n                        RC.TABLE_NAME AS [procedure_name],\r\n                        RC.TABLE_SCHEMA AS [procedure_schema],\r\n                        '' AS name,\r\n                        0 AS position,\r\n                        'OUT' AS parameter_mode,\r\n                        'table type' AS datatype,\r\n                        NULL AS data_length,\r\n                        NULL AS [description]\r\n                        " + epEmptySelect + "\r\n                    FROM INFORMATION_SCHEMA.ROUTINE_COLUMNS RC \r\n                    " + secondFilterString;
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		synchronizeParameters.DatabaseRow.Name.ToUpper();
		string text = synchronizeParameters.DatabaseRow.Host.ToUpper();
		string filterStringForDependencies = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("a.referencing_schema_name", "a.referencing_entity_name", "a.referencing_type", "a.referenced_type");
		string filterStringForDependencies2 = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("a.referenced_schema_name", "a.referenced_entity_name", "a.referenced_type", "a.referencing_type");
		string text2 = FilterRulesCollection.CombineDependenciesFilter(filterStringForDependencies, filterStringForDependencies2);
		if (canReadExternalDependencies)
		{
			yield return "SELECT *\r\n\t\t\t\t\tFROM (\r\n\t\t\t\t\t\tSELECT DISTINCT rdo.*\r\n\t\t\t\t\t\t\t,CASE \r\n\t\t\t\t\t\t\t\tWHEN rgo.type_desc IN (\r\n\t\t\t\t\t\t\t\t\t\t'SQL_STORED_PROCEDURE'\r\n\t\t\t\t\t\t\t\t\t\t,'CLR_STORED_PROCEDURE'\r\n\t\t\t\t\t\t\t\t\t\t,'EXTENDED_STORED_PROCEDURE'\r\n\t\t\t\t\t\t\t\t\t\t,'REPLICATION_FILTER_PROCEDURE'\r\n\t\t\t\t\t\t\t\t\t\t)\r\n\t\t\t\t\t\t\t\t\tTHEN 'PROCEDURE'\r\n\t\t\t\t\t\t\t\tWHEN rgo.type_desc IN (\r\n\t\t\t\t\t\t\t\t\t\t'SQL_SCALAR_FUNCTION'\r\n\t\t\t\t\t\t\t\t\t\t,'SQL_TABLE_VALUED_FUNCTION'\r\n\t\t\t\t\t\t\t\t\t\t,'CLR_SCALAR_FUNCTION'\r\n\t\t\t\t\t\t\t\t\t\t,'CLR_TABLE_VALUED_FUNCTION'\r\n\t\t\t\t\t\t\t\t\t\t,'AGGREGATE_FUNCTION'\r\n\t\t\t\t\t\t\t\t\t\t,'SQL_INLINE_TABLE_VALUED_FUNCTION'\r\n\t\t\t\t\t\t\t\t\t\t)\r\n\t\t\t\t\t\t\t\t\tTHEN 'FUNCTION'\r\n\t\t\t\t\t\t\t\tWHEN rgo.type_desc IN (\r\n\t\t\t\t\t\t\t\t\t\t'CLR_TRIGGER'\r\n\t\t\t\t\t\t\t\t\t\t,'SQL_TRIGGER'\r\n\t\t\t\t\t\t\t\t\t\t)\r\n\t\t\t\t\t\t\t\t\tTHEN 'TRIGGER'\r\n\t\t\t\t\t\t\t\tWHEN rgo.type_desc IN (\r\n\t\t\t\t\t\t\t\t\t\t'INTERNAL_TABLE'\r\n\t\t\t\t\t\t\t\t\t\t,'SYSTEM_TABLE'\r\n\t\t\t\t\t\t\t\t\t\t,'USER_TABLE'\r\n\t\t\t\t\t\t\t\t\t\t)\r\n\t\t\t\t\t\t\t\t\tTHEN 'TABLE'\r\n\t\t\t\t\t\t\t\tWHEN rgo.type_desc IN ('VIEW')\r\n\t\t\t\t\t\t\t\t\tTHEN 'VIEW'\r\n\t\t\t\t\t\t\t\tELSE NULL\r\n\t\t\t\t\t\t\t\tEND AS referencing_type\r\n\t\t\t\t\t\t\t,'" + text + "' AS referencing_server\r\n\t\t\t\t\t\t\t,rgs.name AS referencing_schema_name\r\n\t\t\t\t\t\t\t,DB_NAME() AS referencing_database_name\r\n\t\t\t\t\t\t\t,rgo.name AS referencing_entity_name\r\n\t\t\t\t\t\t\t,ISNULL(rdsrvr.data_source, '" + text + "') AS referenced_server\r\n\t\t\t\t\t\t\t,ISNULL(sed.referenced_database_name, DB_NAME()) AS referenced_database_name\r\n\t\t\t\t\t\t\t,ISNULL(rds.name, ISNULL(sed.referenced_schema_name, rgs.name)) AS referenced_schema_name\r\n\t\t\t\t\t\t\t,CASE \r\n\t\t\t\t\t\t\t\tWHEN rdo.type_desc IN (\r\n\t\t\t\t\t\t\t\t\t\t'SQL_STORED_PROCEDURE'\r\n\t\t\t\t\t\t\t\t\t\t,'CLR_STORED_PROCEDURE'\r\n\t\t\t\t\t\t\t\t\t\t,'EXTENDED_STORED_PROCEDURE'\r\n\t\t\t\t\t\t\t\t\t\t,'REPLICATION_FILTER_PROCEDURE'\r\n\t\t\t\t\t\t\t\t\t\t)\r\n\t\t\t\t\t\t\t\t\tTHEN 'PROCEDURE'\r\n\t\t\t\t\t\t\t\tWHEN rdo.type_desc IN (\r\n\t\t\t\t\t\t\t\t\t\t'SQL_SCALAR_FUNCTION'\r\n\t\t\t\t\t\t\t\t\t\t,'SQL_TABLE_VALUED_FUNCTION'\r\n\t\t\t\t\t\t\t\t\t\t,'CLR_SCALAR_FUNCTION'\r\n\t\t\t\t\t\t\t\t\t\t,'CLR_TABLE_VALUED_FUNCTION'\r\n\t\t\t\t\t\t\t\t\t\t,'AGGREGATE_FUNCTION'\r\n\t\t\t\t\t\t\t\t\t\t,'SQL_INLINE_TABLE_VALUED_FUNCTION'\r\n\t\t\t\t\t\t\t\t\t\t)\r\n\t\t\t\t\t\t\t\t\tTHEN 'FUNCTION'\r\n\t\t\t\t\t\t\t\tWHEN rdo.type_desc IN (\r\n\t\t\t\t\t\t\t\t\t\t'CLR_TRIGGER'\r\n\t\t\t\t\t\t\t\t\t\t,'SQL_TRIGGER'\r\n\t\t\t\t\t\t\t\t\t\t)\r\n\t\t\t\t\t\t\t\t\tTHEN 'TRIGGER'\r\n\t\t\t\t\t\t\t\tWHEN rdo.type_desc IN (\r\n\t\t\t\t\t\t\t\t\t\t'INTERNAL_TABLE'\r\n\t\t\t\t\t\t\t\t\t\t,'SYSTEM_TABLE'\r\n\t\t\t\t\t\t\t\t\t\t,'USER_TABLE'\r\n\t\t\t\t\t\t\t\t\t\t)\r\n\t\t\t\t\t\t\t\t\tTHEN 'TABLE'\r\n\t\t\t\t\t\t\t\tWHEN rdo.type_desc IN ('VIEW')\r\n\t\t\t\t\t\t\t\t\tTHEN 'VIEW'\r\n\t\t\t\t\t\t\t\tELSE NULL\r\n\t\t\t\t\t\t\t\tEND AS referenced_type\r\n\t\t\t\t\t\t\t,sed.referenced_entity_name\r\n\t\t\t\t\t\t\t,CAST(sed.is_caller_dependent AS CHAR(1)) AS is_caller_dependent\r\n\t\t\t\t\t\t\t,CAST(sed.is_ambiguous AS CHAR(1)) AS is_ambiguous\r\n\t\t\t\t\t\t\t,NULL AS dependency_type\r\n\t\t\t\t\t\tFROM sys.sql_expression_dependencies AS sed\r\n\t\t\t\t\t\tINNER JOIN sys.objects AS rgo ON sed.referencing_id = rgo.object_id\r\n\t\t\t\t\t\tINNER JOIN sys.schemas rgs ON rgs.schema_id = rgo.schema_id\r\n\t\t\t\t\t\tLEFT JOIN sys.servers AS rdsrvr ON sed.referenced_server_name = rdsrvr.name collate database_default\r\n\t\t\t\t\t\tLEFT JOIN sys.objects rdo ON sed.referenced_id = rdo.object_id\r\n\t\t\t\t\t\tLEFT JOIN sys.schemas rds ON rds.schema_id = rdo.schema_id\r\n\t\t\t\t\t\t) a\r\n\t\t\t\t\tWHERE (\r\n\t\t\t\t\t\t\treferenced_database_name != DB_NAME()\r\n\t\t\t\t\t\t\tOR referenced_server != '" + text + "'\r\n\t\t\t\t\t\t\tOR referenced_type IS NOT NULL\r\n\t\t\t\t\t\t\t)\r\n\t\t\t\t\t\tAND referencing_type IS NOT NULL\r\n                            AND referencing_entity_name IS NOT NULL\r\n                            AND referenced_entity_name IS NOT NULL" + text2;
		}
		else
		{
			yield return "SELECT *\r\n                        FROM\r\n                        (SELECT DISTINCT\r\n                            CASE\r\n                                WHEN\r\n                                    o.type_desc IN ('SQL_STORED_PROCEDURE','CLR_STORED_PROCEDURE','EXTENDED_STORED_PROCEDURE','REPLICATION_FILTER_PROCEDURE')\r\n                                    THEN 'PROCEDURE'\r\n                                WHEN\r\n                                    o.type_desc IN ('SQL_SCALAR_FUNCTION','SQL_TABLE_VALUED_FUNCTION','CLR_SCALAR_FUNCTION','CLR_TABLE_VALUED_FUNCTION','AGGREGATE_FUNCTION','SQL_INLINE_TABLE_VALUED_FUNCTION')\r\n                                    THEN 'FUNCTION'\r\n                                WHEN\r\n                                    o.type_desc IN ('CLR_TRIGGER','SQL_TRIGGER')\r\n                                    THEN 'TRIGGER'\r\n                                WHEN\r\n                                    o.type_desc IN ('INTERNAL_TABLE','SYSTEM_TABLE','USER_TABLE')\r\n                                    THEN 'TABLE'\r\n                                WHEN\r\n                                    o.type_desc IN ('VIEW')\r\n                                    THEN 'VIEW'\r\n                                ELSE NULL\r\n                            END AS referencing_type,\r\n                                   '" + text + "' as referencing_server,\r\n                            ISNULL(referencing_schemas.name, SCHEMA_NAME()) AS referencing_schema_name,\r\n                            DB_NAME() as referencing_database_name,\r\n                            OBJECT_NAME(sed.object_id) AS referencing_entity_name,\r\n                            ISNULL(s.data_source, '" + text + "') as referenced_server,\r\n                            DB_NAME() as referenced_database_name,\r\n                            ISNULL(OBJECT_SCHEMA_NAME ( sed.referenced_major_id ), SCHEMA_NAME()) as referenced_schema_name,\r\n                            CASE\r\n                                WHEN\r\n                                    ao.type_desc IN ('SQL_STORED_PROCEDURE','CLR_STORED_PROCEDURE','EXTENDED_STORED_PROCEDURE','REPLICATION_FILTER_PROCEDURE')\r\n                                    THEN 'PROCEDURE'\r\n                                WHEN\r\n                                    ao.type_desc IN ('SQL_SCALAR_FUNCTION','SQL_TABLE_VALUED_FUNCTION','CLR_SCALAR_FUNCTION','CLR_TABLE_VALUED_FUNCTION','AGGREGATE_FUNCTION','SQL_INLINE_TABLE_VALUED_FUNCTION')\r\n                                    THEN 'FUNCTION'\r\n                                WHEN\r\n                                    ao.type_desc IN ('CLR_TRIGGER','SQL_TRIGGER')\r\n                                    THEN 'TRIGGER'\r\n                                WHEN\r\n                                    ao.type_desc IN ('INTERNAL_TABLE','SYSTEM_TABLE','USER_TABLE')\r\n                                    THEN 'TABLE'\r\n                                WHEN\r\n                                    ao.type_desc IN ('VIEW')\r\n                                    THEN 'VIEW'\r\n                                  ELSE\r\n                                       NULL\r\n                            END AS referenced_type,\r\n                            OBJECT_NAME(sed.referenced_major_id) AS referenced_entity_name,\r\n                            CAST (0 AS CHAR(1)) as is_caller_dependent,\r\n                            CAST (0 AS CHAR(1)) as is_ambiguous,\r\n                            NULL as dependency_type\r\n                        FROM \r\n                            sys.sql_dependencies AS sed\r\n                             LEFT JOIN sys.servers AS s ON '" + text + "' = s.name collate database_default\r\n                            LEFT JOIN sys.objects AS o ON sed.object_id = o.object_id\r\n                            LEFT JOIN sys.all_objects ao ON object_name(sed.referenced_major_id) = ao.name\r\n                            LEFT JOIN sys.schemas referencing_schemas ON referencing_schemas.schema_id = o.schema_id\r\n                        ) a \r\n                        WHERE referencing_type IS NOT NULL\r\n                            AND referencing_entity_name IS NOT NULL\r\n                            AND referenced_entity_name IS NOT NULL" + text2;
		}
	}

	public SynchronizeDB(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	private SharedDatabaseTypeEnum.DatabaseType GetForkType()
	{
		string dbmsVersion = synchronizeParameters.DatabaseRow.DbmsVersion;
		if (dbmsVersion.Contains("Azure SQL Data Warehouse"))
		{
			return SharedDatabaseTypeEnum.DatabaseType.AzureSQLDataWarehouse;
		}
		if (dbmsVersion.Contains("Microsoft SQL Server"))
		{
			return SharedDatabaseTypeEnum.DatabaseType.SqlServer;
		}
		if (dbmsVersion.Contains("Microsoft SQL Azure "))
		{
			return SharedDatabaseTypeEnum.DatabaseType.AzureSQLDatabase;
		}
		return DatabaseType;
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Counting objects to import");
		new ObservableCollection<ObjectRow>();
		if (!string.IsNullOrWhiteSpace(query.Query))
		{
			try
			{
				using SqlCommand sqlCommand = CommandsWithTimeout.SqlServer(query.Query, synchronizeParameters.DatabaseRow.Connection);
				using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
				while (sqlDataReader.Read())
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					ReadCount(sqlDataReader);
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
		if (base.ObjectsCounter != null)
		{
			try
			{
				backgroundWorkerManager.ReportProgress("Retrieving database's objects");
				if (!string.IsNullOrWhiteSpace(query.Query))
				{
					using SqlCommand sqlCommand = CommandsWithTimeout.SqlServer(query.Query, synchronizeParameters.DatabaseRow.Connection);
					using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
					while (sqlDataReader.Read())
					{
						if (synchronizeParameters.DbSynchLocker.IsCanceled)
						{
							return false;
						}
						AddDBObject(sqlDataReader, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
					}
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

	public override bool GetRelations(string query, Form owner = null)
	{
		if ((GetForkType() == SharedDatabaseTypeEnum.DatabaseType.AzureSQLDataWarehouse) ^ (DatabaseType == SharedDatabaseTypeEnum.DatabaseType.AzureSQLDataWarehouse))
		{
			return true;
		}
		try
		{
			using SqlCommand sqlCommand = CommandsWithTimeout.SqlServer(query, synchronizeParameters.DatabaseRow.Connection);
			using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
			while (sqlDataReader.Read())
			{
				AddRelation(sqlDataReader, DatabaseType, WithCustomFields);
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
			using SqlCommand sqlCommand = CommandsWithTimeout.SqlServer(query, synchronizeParameters.DatabaseRow.Connection);
			using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
			while (sqlDataReader.Read())
			{
				AddColumn(sqlDataReader, WithCustomFields);
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
				using SqlCommand sqlCommand = CommandsWithTimeout.SqlServer(query, synchronizeParameters.DatabaseRow.Connection);
				using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
				while (sqlDataReader.Read())
				{
					AddTrigger(sqlDataReader, WithCustomFields);
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
		base.UniqueConstraintRows = new ObservableCollection<UniqueConstraintRow>();
		try
		{
			if (!string.IsNullOrWhiteSpace(query))
			{
				using SqlCommand sqlCommand = CommandsWithTimeout.SqlServer(query, synchronizeParameters.DatabaseRow.Connection);
				using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
				while (sqlDataReader.Read())
				{
					AddUniqueConstraint(sqlDataReader, SharedDatabaseTypeEnum.DatabaseType.SqlServer, WithCustomFields);
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
				using SqlCommand sqlCommand = CommandsWithTimeout.SqlServer(query, synchronizeParameters.DatabaseRow.Connection);
				using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
				while (sqlDataReader.Read())
				{
					AddParameter(sqlDataReader, WithCustomFields);
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
		if (GetForkType() != DatabaseType)
		{
			return true;
		}
		try
		{
			if (!string.IsNullOrWhiteSpace(query))
			{
				using SqlCommand sqlCommand = CommandsWithTimeout.SqlServer(query, synchronizeParameters.DatabaseRow.Connection);
				using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
				while (sqlDataReader.Read())
				{
					AddDependency(sqlDataReader);
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
}
