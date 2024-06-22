using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Shared.Enums;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.Data.InterfaceTables;

public class SynchronizeInterfaceTables : SynchronizeDatabase
{
	protected virtual SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.InterfaceTables;

	protected virtual bool WithCustomFields => true;

	public string DatabaseNameToImport => synchronizeParameters.DatabaseRow.Param1;

	public SynchronizeInterfaceTables(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	public override bool PreCmdImportOperations(Form owner)
	{
		try
		{
			return DB.InterfaceTables.ValidateAllImportTables(DatabaseNameToImport);
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Counting objects to import");
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
			catch (Exception ex)
			{
				GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
				return false;
			}
		}
		return true;
	}

	public override bool GetRelations(string query, Form owner = null)
	{
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
					AddUniqueConstraint(sqlDataReader, SharedDatabaseTypeEnum.DatabaseType.InterfaceTables, WithCustomFields);
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

	public override IEnumerable<string> CountTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "t.[table_schema]", "t.[table_name]");
		yield return "SELECT 'TABLE' AS 'object_type', COUNT(1) AS 'count' FROM [import_tables] AS t \r\n                            WHERE (t.[error_failed] IS NULL OR t.[error_failed] = 0) \r\n                                AND t.[database_name] = '" + DatabaseNameToImport + "' AND t.[object_type] = 'TABLE'\r\n                                " + filterString;
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "v.[table_schema]", "v.[table_name]");
		yield return "SELECT 'VIEW' AS 'object_type', COUNT(1) AS 'count' FROM [import_tables] AS v \r\n                            WHERE (v.[error_failed] IS NULL OR v.[error_failed] = 0) \r\n                                AND v.[database_name] = '" + DatabaseNameToImport + "' AND v.[object_type] = 'VIEW'\r\n                                " + filterString;
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "p.[procedure_schema]", "p.[procedure_name]");
		yield return "SELECT 'PROCEDURE' AS 'object_type', COUNT(1) AS 'count' FROM [import_procedures] AS p \r\n                            WHERE (p.[error_failed] IS NULL OR p.[error_failed] = 0) \r\n                                AND p.[database_name] = '" + DatabaseNameToImport + "' AND p.[object_type] = 'PROCEDURE'\r\n                                " + filterString;
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "f.[procedure_schema]", "f.[procedure_name]");
		yield return "SELECT 'FUNCTION' AS 'object_type', COUNT(1) AS 'count' FROM [import_procedures] AS f \r\n                            WHERE (f.[error_failed] IS NULL OR f.[error_failed] = 0) \r\n                                AND f.[database_name] = '" + DatabaseNameToImport + "' AND f.[object_type] = 'FUNCTION'\r\n                                " + filterString;
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		string tableFilterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "t.[table_schema]", "t.[table_name]");
		List<DocumentationCustomFieldRow> fields = synchronizeParameters.CustomFields.Where((DocumentationCustomFieldRow x) => !string.IsNullOrWhiteSpace(x.ExtendedPropertyForQueries)).ToList();
		string cfSelect = GetCustomFieldsSelect("t", fields);
		yield return "SELECT \r\n                            t.[table_name] AS [name],\r\n                            t.[table_schema] AS [schema],\r\n                            t.[database_name] AS [database_name],\r\n                            'TABLE' AS [type],\r\n                            t.[object_subtype] AS [subtype],\r\n                            t.[language] AS [language],\r\n                            t.[description] AS [description],\r\n                            t.[definition] AS [definition],\r\n                            t.[dbms_created] AS [create_date],\r\n                            t.[dbms_last_modified] AS [modify_date],\r\n                            NULL AS [function_type]\r\n                            " + cfSelect + "\r\n                            FROM [import_tables] AS t\r\n                            WHERE (t.[error_failed] IS NULL OR t.[error_failed] = 0) \r\n                                AND t.[database_name] = '" + DatabaseNameToImport + "' \r\n                                AND (UPPER(t.[object_type]) = 'TABLE' \r\n                                    OR UPPER(t.[object_type]) NOT IN ('TABLE', 'VIEW', 'STRUCTURE'))\r\n                                " + tableFilterString;
		yield return "SELECT \r\n                            t.[table_name] AS [name],\r\n                            t.[table_schema] AS [schema],\r\n                            t.[database_name] AS [database_name],\r\n                            'STRUCTURE' AS [type],\r\n                            t.[object_subtype] AS [subtype],\r\n                            t.[language] AS [language],\r\n                            t.[description] AS [description],\r\n                            t.[definition] AS [definition],\r\n                            t.[dbms_created] AS [create_date],\r\n                            t.[dbms_last_modified] AS [modify_date],\r\n                            NULL AS [function_type]\r\n                            " + cfSelect + "\r\n                            FROM [import_tables] AS t\r\n                            WHERE (t.[error_failed] IS NULL OR t.[error_failed] = 0) \r\n                                AND t.[database_name] = '" + DatabaseNameToImport + "' \r\n                                AND (UPPER(t.[object_type]) = 'STRUCTURE')\r\n                                " + tableFilterString;
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "v.[table_schema]", "v.[table_name]");
		List<DocumentationCustomFieldRow> fields = synchronizeParameters.CustomFields.Where((DocumentationCustomFieldRow x) => !string.IsNullOrWhiteSpace(x.ExtendedPropertyForQueries)).ToList();
		string customFieldsSelect = GetCustomFieldsSelect("v", fields);
		yield return "SELECT \r\n                            v.[table_name] AS [name],\r\n                            v.[table_schema] AS [schema],\r\n                            v.[database_name] AS [database_name],\r\n                            UPPER(v.[object_type]) AS [type],\r\n                            v.[object_subtype] AS [subtype],\r\n                            v.[language] AS [language],\r\n                            v.[description] AS [description],\r\n                            v.[definition] AS [definition],\r\n                            v.[dbms_created] AS [create_date],\r\n                            v.[dbms_last_modified] AS [modify_date],\r\n                            NULL AS function_type\r\n                            " + customFieldsSelect + "\r\n                            FROM [import_tables] AS v\r\n                            WHERE (v.[error_failed] IS NULL OR v.[error_failed] = 0) \r\n                                AND v.[database_name] = '" + DatabaseNameToImport + "' \r\n                                AND UPPER(v.[object_type]) = 'VIEW'\r\n                                " + filterString;
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "p.[procedure_schema]", "p.[procedure_name]");
		List<DocumentationCustomFieldRow> fields = synchronizeParameters.CustomFields.Where((DocumentationCustomFieldRow x) => !string.IsNullOrWhiteSpace(x.ExtendedPropertyForQueries)).ToList();
		string customFieldsSelect = GetCustomFieldsSelect("p", fields);
		yield return "SELECT \r\n                            p.[procedure_name] AS [name],\r\n                            p.[procedure_schema] AS [schema],\r\n                            p.[database_name] AS [database_name],\r\n                            'PROCEDURE' AS [type],\r\n                            p.[object_subtype] AS [subtype],\r\n                            p.[language] AS [language],\r\n                            p.[description] AS [description],\r\n                            p.[definition] AS [definition],\r\n                            p.[dbms_created] AS [create_date],\r\n                            p.[dbms_last_modified] AS [modify_date],\r\n                            p.[function_type] AS [function_type]\r\n                            " + customFieldsSelect + "\r\n                            FROM [import_procedures] AS p\r\n                            WHERE (p.[error_failed] IS NULL OR p.[error_failed] = 0) \r\n                                AND p.[database_name] = '" + DatabaseNameToImport + "' \r\n                                AND (UPPER(p.[object_type]) = 'PROCEDURE' \r\n                                    OR UPPER(p.[object_type]) NOT IN ('PROCEDURE', 'FUNCTION'))\r\n                                " + filterString;
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "f.[procedure_schema]", "f.[procedure_name]");
		List<DocumentationCustomFieldRow> fields = synchronizeParameters.CustomFields.Where((DocumentationCustomFieldRow x) => !string.IsNullOrWhiteSpace(x.ExtendedPropertyForQueries)).ToList();
		string customFieldsSelect = GetCustomFieldsSelect("f", fields);
		yield return "SELECT \r\n                            f.[procedure_name] AS [name],\r\n                            f.[procedure_schema] AS [schema],\r\n                            f.[database_name] AS [database_name],\r\n                            UPPER(f.[object_type]) AS [type],\r\n                            f.[object_subtype] AS [subtype],\r\n                            f.[language] AS [language],\r\n                            f.[description] AS [description],\r\n                            f.[definition] AS [definition],\r\n                            f.[dbms_created] AS [create_date],\r\n                            f.[dbms_last_modified] AS [modify_date],\r\n                            f.[function_type] AS [function_type]\r\n                            " + customFieldsSelect + "\r\n                            FROM [import_procedures] AS f\r\n                            WHERE (f.[error_failed] IS NULL OR f.[error_failed] = 0) \r\n                                AND f.[database_name] = '" + DatabaseNameToImport + "' \r\n                                AND UPPER(f.[object_type]) = 'FUNCTION'\r\n                                " + filterString;
	}

	private static string GetCustomFieldsSelect(string tableAlias, List<DocumentationCustomFieldRow> fields)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine();
		foreach (DocumentationCustomFieldRow field in fields)
		{
			if (!string.IsNullOrWhiteSpace(field.ExtendedPropertyForQueries))
			{
				stringBuilder.AppendLine(",[" + tableAlias + "].[" + field.ExtendedPropertyForQueries + "] AS [" + field.FieldName + "]");
			}
		}
		return stringBuilder.ToString();
	}

	public override IEnumerable<string> RelationsQuery()
	{
		List<DocumentationCustomFieldRow> fields = synchronizeParameters.CustomFields.Where((DocumentationCustomFieldRow x) => !string.IsNullOrWhiteSpace(x.ExtendedPropertyForQueries)).ToList();
		string customFieldsSelect = GetCustomFieldsSelect("r", fields);
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString(null, base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "r.[foreign_table_schema]", "r.[foreign_table_name]");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("OR", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "r.[primary_table_schema]", "r.[primary_table_name]");
		string text = FilterRulesCollection.CombineRelationsFilter(filterString, filterString2);
		yield return "SELECT\r\n                            r.[key_name] AS [name],\r\n                            NULL AS [fk_table_database_name],\r\n                            r.[foreign_table_schema] AS [fk_table_schema],\r\n                            r.[foreign_table_name] AS [fk_table_name],\r\n                            r.[foreign_column_name] AS [fk_column],\r\n                            r.[foreign_column_path] AS [fk_column_path],\r\n                            NULL AS [ref_table_database_name],\r\n                            r.[primary_table_schema] AS [ref_table_schema],\r\n                            r.[primary_table_name] AS [ref_table_name],\r\n                            r.[primary_column_name] AS [ref_column],\r\n                            r.[primary_column_path] AS [ref_column_path],\r\n                            r.[column_pair_order] AS [ordinal_position],\r\n                            r.[description] AS [description],\r\n                            NULL AS [update_rule],\r\n                            NULL AS [delete_rule]\r\n                            " + customFieldsSelect + "\r\n                            FROM [import_tables_foreign_keys_columns] AS r\r\n                            WHERE (r.[error_failed] IS NULL OR r.[error_failed] = 0) \r\n                                AND r.[database_name] = '" + DatabaseNameToImport + "'\r\n                            " + text;
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		List<DocumentationCustomFieldRow> fields = synchronizeParameters.CustomFields.Where((DocumentationCustomFieldRow x) => !string.IsNullOrWhiteSpace(x.ExtendedPropertyForQueries)).ToList();
		string customFieldsSelect = GetCustomFieldsSelect("c", fields);
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "c.table_schema", "c.table_name");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.View, "c.table_schema", "c.table_name");
		yield return "SELECT \r\n                            c.[database_name] AS [database_name],\r\n                            c.[table_name] AS [table_name],\r\n                            c.[table_schema] AS [table_schema],\r\n                            c.[column_name] AS [name],\r\n                            c.[ordinal_position] AS [position],\r\n                            c.[datatype] AS [datatype],\r\n                            c.[description] AS [description],\r\n                            CASE c.[is_identity] WHEN '1' THEN 'P' END AS [constraint_type],\r\n                            c.[nullable] AS [nullable],\r\n                            c.[default_value] AS [default_value],\r\n                            c.[is_identity] AS [is_identity],\r\n                            c.[is_computed] AS [is_computed],\r\n                            c.[computed_formula] AS [computed_formula],\r\n                            c.[data_length] AS [data_length],\r\n                            c.[column_path] AS [path],\r\n                            c.[column_level] AS [level],\r\n                            UPPER(c.[item_type]) AS [type]\r\n                            " + customFieldsSelect + "\r\n                            FROM [import_columns] AS c\r\n                            WHERE (c.[error_failed] IS NULL OR c.[error_failed] = 0) \r\n                                AND c.[database_name] = '" + DatabaseNameToImport + "'\r\n                                AND ((UPPER(c.[table_object_type]) = 'VIEW' " + filterString2 + ") \r\n                                OR (UPPER(c.[table_object_type]) != 'VIEW' " + filterString + "))\r\n                            ORDER BY c.[column_level]";
	}

	public override IEnumerable<string> TriggersQuery()
	{
		List<DocumentationCustomFieldRow> fields = synchronizeParameters.CustomFields.Where((DocumentationCustomFieldRow x) => !string.IsNullOrWhiteSpace(x.ExtendedPropertyForQueries)).ToList();
		string customFieldsSelect = GetCustomFieldsSelect("t", fields);
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "t.table_schema", "t.table_name");
		yield return "SELECT\r\n                            t.[trigger_name] AS [trigger_name],\r\n                            t.[table_schema] AS [table_schema],\r\n                            t.[table_name] AS [table_name],\r\n                            t.[database_name] AS [database_name],\r\n                            t.[on_update] AS [isupdate],\r\n                            t.[on_delete] AS [isdelete],\r\n                            t.[on_insert] AS [isinsert],\r\n                            t.[before] AS [isbefore],\r\n                            t.[after] AS [isafter],\r\n                            t.[instead_of] AS [isinsteadof],\r\n                            t.[disabled] AS [disabled],\r\n                            t.[definition] AS [definition],\r\n                            t.[description] AS [description],\r\n                            UPPER(t.[trigger_type]) AS [type]\r\n                            " + customFieldsSelect + "\r\n                            FROM [import_triggers] AS t\r\n                            WHERE (t.[error_failed] IS NULL OR t.[error_failed] = 0) \r\n                                AND t.[database_name] = '" + DatabaseNameToImport + "'\r\n                            " + filterString;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		List<DocumentationCustomFieldRow> fields = synchronizeParameters.CustomFields.Where((DocumentationCustomFieldRow x) => !string.IsNullOrWhiteSpace(x.ExtendedPropertyForQueries)).ToList();
		string customFieldsSelect = GetCustomFieldsSelect("c", fields);
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "c.table_schema", "c.table_name");
		yield return "SELECT\r\n                            c.[database_name] AS [database_name],\r\n                            c.[table_schema] AS [table_schema],\r\n                            c.[table_name] AS [table_name],\r\n                            c.[key_name] AS [name],\r\n                            CASE UPPER(c.[key_type])\r\n                                WHEN 'PK' then 'P'\r\n                                ELSE 'U'\r\n                            END AS [type],\r\n                            c.[column_name] AS [column_name],\r\n                            c.[column_order] AS [column_ordinal],\r\n                            c.[column_path] AS [column_path],\r\n                            c.[description] AS [description],\r\n                            c.[disabled] AS [disabled]\r\n                            " + customFieldsSelect + "\r\n                            FROM [import_tables_keys_columns] AS c\r\n                            WHERE (c.[error_failed] IS NULL OR c.[error_failed] = 0) \r\n                                AND c.[database_name] = '" + DatabaseNameToImport + "'\r\n                            " + filterString;
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		List<DocumentationCustomFieldRow> fields = synchronizeParameters.CustomFields.Where((DocumentationCustomFieldRow x) => !string.IsNullOrWhiteSpace(x.ExtendedPropertyForQueries)).ToList();
		string customFieldsSelect = GetCustomFieldsSelect("p", fields);
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Procedure, "p.procedure_schema", "p.procedure_name");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Function, "p.procedure_schema", "p.procedure_name");
		yield return "SELECT \r\n                            p.[database_name] AS [database_name],\r\n                            p.[procedure_name] AS [procedure_name],\r\n                            p.[procedure_schema] AS [procedure_schema],\r\n                            p.[parameter_name] AS [name],\r\n                            p.[ordinal_position] AS [position],\r\n                            CASE \r\n                                WHEN UPPER(p.[parameter_mode]) IN ('IN', 'OUT', 'INOUT') \r\n                                    THEN UPPER(p.[parameter_mode])\r\n                                ELSE 'IN'\r\n                                END AS [parameter_mode],\r\n                            p.[datatype] AS [datatype],\r\n                            p.[data_length] AS [data_length],\r\n                            p.[description] AS [description]\r\n                            " + customFieldsSelect + "\r\n                            FROM [import_procedures_parameters] AS p\r\n                            WHERE (p.[error_failed] IS NULL OR p.[error_failed] = 0) \r\n                                AND p.[database_name] = '" + DatabaseNameToImport + "'\r\n                                AND ((UPPER(p.[procedure_object_type]) = 'PROCEDURE' " + filterString + ") \r\n                                OR (UPPER(p.[procedure_object_type]) != 'PROCEDURE' " + filterString2 + "))";
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		yield return string.Empty;
	}
}
