using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.QueryTools;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Shared.Enums;
using Npgsql;

namespace Dataedo.App.Data.Redshift;

internal class SynchronizeRedshift : SynchronizeDatabase
{
	public SynchronizeRedshift(SynchronizeParameters synchronizeParameters)
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
				using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.Redshift(query.Query, synchronizeParameters.DatabaseRow.Connection);
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
					using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.Redshift(query.Query, synchronizeParameters.DatabaseRow.Connection);
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
			using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.Redshift(query, synchronizeParameters.DatabaseRow.Connection);
			using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
			while (npgsqlDataReader.Read())
			{
				AddRelation(npgsqlDataReader, SharedDatabaseTypeEnum.DatabaseType.Redshift);
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
			using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.Redshift(query, synchronizeParameters.DatabaseRow.Connection);
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
		return true;
	}

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		try
		{
			using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.Redshift(query, synchronizeParameters.DatabaseRow.Connection);
			using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
			while (npgsqlDataReader.Read())
			{
				AddUniqueConstraint(npgsqlDataReader, SharedDatabaseTypeEnum.DatabaseType.Redshift);
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
		if (!canReadParameters)
		{
			return true;
		}
		try
		{
			using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.Redshift(query, synchronizeParameters.DatabaseRow.Connection);
			using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
			while (npgsqlDataReader.Read())
			{
				AddParameter(npgsqlDataReader);
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
			using NpgsqlCommand npgsqlCommand = CommandsWithTimeout.Redshift(query, synchronizeParameters.DatabaseRow.Connection);
			using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
			while (npgsqlDataReader.Read())
			{
				AddDependency(npgsqlDataReader);
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
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "t.table_schema", "t.table_name");
		yield return "select 'table' as object_type,\r\n                        count(1) as count\r\n                    from svv_tables t\r\n                    where t.table_type in ('BASE TABLE', 'EXTERNAL TABLE')\r\n                       and t.table_schema not in('pg_catalog', 'pg_internal', 'information_schema')\r\n                       and t.table_schema not like 'pg_temp%'\r\n                       " + filterString;
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "v.table_schema", "v.table_name");
		yield return "select 'view' as object_type,\r\n\t                    count(1) as count\r\n                    from svv_tables v\r\n                    where v.table_type = 'VIEW'\r\n                        and v.table_schema not in('pg_catalog', 'pg_internal', 'information_schema')\r\n                        and v.table_schema not like 'pg_temp%'\r\n                        " + filterString;
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "r.routine_schema", "r.routine_name");
		yield return "select 'function' as object_type,\r\n                        count(1) as count\r\n                    from information_schema.routines r\r\n                    where r.routine_type = 'FUNCTION'\r\n                        and r.routine_schema not in('pg_catalog', 'pg_internal', 'information_schema')\r\n                        and r.routine_schema not like 'pg_temp%'\r\n                        " + filterString;
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "t.table_schema", "t.table_name");
		yield return "select t.table_name as name,\r\n                        t.table_schema as schema,\r\n                        t.table_catalog as database_name,\r\n                        case t.table_type when 'BASE TABLE' then 'TABLE'\r\n                        \twhen 'EXTERNAL TABLE' then 'EXTERNAL_TABLE'\r\n                        \telse 'TABLE'\r\n                        end as type,\r\n                        t.remarks as description,\r\n                        null as definition,\r\n                        null as create_date,\r\n                        '01-01-2017' as modify_date, -- any date to mark object as 'forced reimport'\r\n                        null as function_type,\r\n                        null as language\r\n                    from svv_tables t\r\n                    where t.table_type in ('BASE TABLE', 'EXTERNAL TABLE')\r\n                        and t.table_schema not in('pg_catalog', 'pg_internal', 'information_schema')\r\n                        and t.table_schema not like 'pg_temp%'\r\n                        " + filterString;
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "t.table_schema", "t.table_name");
		yield return "select t.table_name as name,\r\n                        t.table_schema as schema,\r\n                        t.table_catalog as database_name,\r\n                        'VIEW' as type,\r\n                        t.remarks as description,\r\n                        v.view_definition as definition,\r\n                        null as create_date,\r\n                        '01-01-2017' as modify_date, -- any date to mark object as 'forced reimport'\r\n                        null as function_type,\r\n                        null as language\r\n                    from svv_tables t\r\n                    \tleft outer join information_schema.views v -- is schema view has database table and pg_views doesn't\r\n                    \ton t.table_name = v.table_name\r\n                    \tand t.table_schema = v.table_schema\r\n                    \tand t.table_catalog = v.table_catalog\r\n                    \tand t.table_type = 'VIEW'\r\n                    where t.table_type = 'VIEW'\r\n                        and t.table_schema not in('pg_catalog', 'pg_internal', 'information_schema')\r\n                        and t.table_schema not like 'pg_temp%'\r\n                        " + filterString;
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "n.nspname", "p.proname");
		yield return "select p.proname as name,\r\n                        n.nspname as schema,\r\n                        current_database() as database_name,\r\n                        'FUNCTION' AS type,\r\n                        d.description as description,\r\n                        p.prosrc as definition,\r\n                        null as create_date,\r\n                        '01-01-2017' as modify_date, -- any date to mark object as 'forced reimport'\r\n                        null as function_type,\r\n                        l.lanname as language\r\n                    from pg_proc p\r\n                    \tinner join pg_namespace n\r\n                    \t\ton n.oid = p.pronamespace\r\n                    \tinner join pg_language l\r\n                    \t\ton l.oid = p.prolang\r\n                    \tleft outer join pg_description d\r\n                    \t\ton d.objoid = p.oid\r\n                    where n.nspname not in('pg_catalog', 'pg_internal', 'information_schema') \r\n                        and n.nspname not like 'pg_temp%'\r\n                        " + filterString;
	}

	public override IEnumerable<string> RelationsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString(null, base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "rc.constraint_schema", "fk.table_name");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("OR", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "rc.unique_constraint_schema ", "pk.table_name");
		string text = FilterRulesCollection.CombineRelationsFilter(filterString, filterString2, "and");
		yield return "select\r\n\t                rc.constraint_name as name,\r\n\t                rc.constraint_catalog AS fk_table_database_name,\r\n\t                fk.table_name as fk_table_name,\r\n\t                rc.constraint_schema as fk_table_schema,\r\n\t                fk.column_name as fk_column,\r\n\t                rc.unique_constraint_catalog AS ref_table_database_name,\r\n\t                pk.table_name as ref_table_name,\r\n\t                rc.unique_constraint_schema as ref_table_schema,\r\n\t                pk.column_name as ref_column,\r\n\t                fk.ordinal_position as ordinal_position,\r\n\t                null as description,\r\n\t                rc.update_rule as update_rule,\r\n\t                rc.delete_rule as delete_rule\r\n                from information_schema.referential_constraints rc\r\n                    inner join information_schema.key_column_usage fk \r\n\t\t\t\t\t\ton rc.constraint_name = fk.constraint_name\r\n\t\t\t\t\t\tand rc.constraint_schema = fk.constraint_schema\r\n                    inner join information_schema.key_column_usage pk \r\n\t\t\t\t\t\ton rc.unique_constraint_name = pk.constraint_name\r\n\t\t\t\t\t\tand rc.unique_constraint_schema = pk.constraint_schema\r\n\t\t\t\t\t\tand fk.ordinal_position = pk.ordinal_position\r\n                        " + text;
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "c.table_schema", "c.table_name");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.View, "c.table_schema", "c.table_name");
		yield return "\r\n\t\t\t\tselect c.table_catalog as database_name,\r\n                       c.table_name as table_name,\r\n                       c.table_schema as table_schema,\r\n                       c.column_name as name,\r\n                       c.ordinal_position as position,\r\n                       c.data_type as datatype,\r\n                       c.remarks as description,\r\n                       null as constraint_type,\t-- niepotrzebne?\r\n                       case c.is_nullable\r\n                            when 'YES' then 1\r\n                            else 0\r\n                       end as nullable,\r\n                       c.column_default as default_value,\r\n                       0 as is_computed,\r\n                       case when column_default like '%identity%' then 1\r\n                            else 0\r\n                       end as is_identity,\r\n                       null as computed_formula,\r\n                       " + QueryDataTypes.GetQueryForDataLength(synchronizeParameters.DatabaseRow.Type) + "\r\n                  from svv_columns c\r\n\t\t\t\t\t   inner join svv_tables t on c.table_name = t.table_name and c.table_schema = t.table_schema and c.table_catalog = t.table_catalog\r\n                 where c.table_schema not in ('pg_catalog', 'pg_internal', 'information_schema')\r\n                   and c.table_schema not like 'pg_temp%'\r\n                   and c.table_catalog = '" + synchronizeParameters.DatabaseName + "'\r\n                   and ((t.table_type = 'VIEW' " + filterString2 + ") or (t.table_type <> 'VIEW' " + filterString + "))\r\n                 order by table_schema, table_name, position";
	}

	public override IEnumerable<string> TriggersQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "tc.constraint_schema", "tc.table_name");
		yield return "select\r\n                       tc.constraint_schema as database_name,\r\n\t                   tc.table_name as table_name,\r\n\t                   tc.constraint_schema as table_schema,\r\n\t                   tc.constraint_name as name,\r\n\t                   case tc.constraint_type\r\n                            when 'PRIMARY KEY' then 'P'\r\n                            when 'UNIQUE' then 'U'\r\n                       end as type,\r\n\t                   kcu.column_name as column_name,\r\n\t                   kcu.ordinal_position as column_ordinal,\r\n\t                   '' as description, \r\n\t                   0 as disabled\r\n                   from information_schema.table_constraints tc\r\n                        inner\r\n                   join information_schema.key_column_usage kcu\r\n                  on kcu.constraint_name = tc.constraint_name\r\n                    and kcu.constraint_schema = tc.constraint_schema\r\n                        and kcu.table_name = tc.table_name\r\n                   where tc.constraint_type in ('PRIMARY KEY', 'UNIQUE')\r\n                        " + filterString + " ";
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Function, "p.specific_schema", "r.routine_name");
		string secondFilterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Function, "r.specific_schema", "r.routine_name");
		yield return "\r\n\t\t\t    select r.specific_catalog as database_name,\r\n                       r.routine_name as procedure_name,\r\n                       p.specific_schema as procedure_schema,\r\n                       case when p.parameter_name is not null then p.parameter_name\r\n                       else '$' || cast(p.ordinal_position as text)\r\n                       end as name,\r\n                       p.ordinal_position as position,\r\n                       p.parameter_mode,\r\n                       p.data_type as datatype,\r\n                       p.character_maximum_length as data_length,  \r\n                       null as description\r\n                  from information_schema.parameters p\r\n                  inner join information_schema.routines r\r\n              \t    on r.specific_name = p.specific_name\r\n\t\t\t    where p.specific_schema not in ('information_schema', 'pg_catalog', 'pg_internal')\r\n\t\t\t    and p.specific_schema not like 'pg_temp_%'             \r\n                    " + filterString;
		yield return "\r\n\t\t\t    select r.specific_catalog as database_name,\r\n                       r.routine_name as procedure_name,\r\n                       r.specific_schema as procedure_schema,\r\n                       'Returns' as name,\r\n                       0 as position,\r\n                       'OUT' as parameter_mode,\r\n                       r.data_type as datatype,\r\n                       r.character_maximum_length as data_length, \r\n                       null as description\r\n                  from information_schema.routines r\r\n\t\t\t    where r.specific_schema not in ('information_schema', 'pg_catalog', 'pg_internal')\r\n\t\t\t      and r.specific_schema not like 'pg_temp_%'             \r\n                    " + secondFilterString;
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		synchronizeParameters.DatabaseRow.Name.ToUpper();
		string text = synchronizeParameters.DatabaseRow.Host.ToUpper();
		string filterStringForDependencies = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("d.view_schema", "d.view_name", "'VIEW'", "(case t.table_type when 'BASE TABLE' then 'TABLE'\r\n                    when 'VIEW' then 'VIEW'\r\n\t\t\t\t\telse t.table_type end)");
		string filterStringForDependencies2 = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("d.table_schema", "d.table_name", "(case t.table_type when 'BASE TABLE' then 'TABLE'\r\n                    when 'VIEW' then 'VIEW'\r\n\t\t\t\t\telse t.table_type end)", "'VIEW'");
		string text2 = FilterRulesCollection.CombineDependenciesFilter(filterStringForDependencies, filterStringForDependencies2);
		yield return "\r\n\t\t\t\t\tselect\r\n\t\t\t\t\t\t'VIEW' as referencing_type,  \r\n                        d.view_schema as referencing_schema_name,\r\n\t\t\t\t\t\t'" + text + "' as referencing_server,\r\n\t\t\t\t\t\td.view_catalog as referencing_database_name,\r\n\t\t\t\t\t\td.view_name as referencing_entity_name,\r\n\t\t\t\t\t\t'" + text + "' as referenced_server,\r\n\t\t\t\t\t\td.table_catalog as referenced_database_name,\r\n\t\t\t\t\t\td.table_schema as referenced_schema_name,\r\n\t\t\t\t\t\tcase t.table_type when 'BASE TABLE' then 'TABLE'\r\n\t\t\t\t\t\t\twhen 'VIEW' then 'VIEW'\r\n\t\t\t\t\t\t\telse t.table_type end\r\n\t\t\t\t\t\tas referenced_type, \r\n\t\t\t\t\t\td.table_name as referenced_entity_name,\r\n\t\t\t\t\t\tnull as is_caller_dependent,\r\n\t\t\t\t\t\tnull as is_ambiguous,\r\n\t\t\t\t\t\tnull as dependency_type\r\n\t\t\t\t\tfrom information_schema.view_table_usage d\r\n\t\t\t\t\t\tleft outer join information_schema.tables t\r\n\t\t\t\t\t\ton d.table_catalog = t.table_catalog\r\n\t\t\t\t\t\tand d.table_schema = t.table_schema\r\n\t\t\t\t\t\tand d.table_name = t.table_name\r\n                    where 1=1\r\n                            AND referencing_entity_name IS NOT NULL\r\n                            AND referenced_entity_name IS NOT NULL\r\n                       " + text2;
	}
}
