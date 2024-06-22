using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.General;
using Dataedo.App.Data.QueryTools;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Synchronization_Tools;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Shared.Enums;
using MySqlConnector;

namespace Dataedo.App.Data.MySQL;

internal class SynchronizeMySQL : SynchronizeDatabase
{
	private readonly LowerCaseTableNamesHelper lowerCaseTableNamesHelper;

	protected virtual SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.MySQL;

	public SynchronizeMySQL(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
		lowerCaseTableNamesHelper = new LowerCaseTableNamesHelper(synchronizeParameters.DatabaseRow.Connection);
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Counting objects to import");
		if (!string.IsNullOrWhiteSpace(query.Query))
		{
			try
			{
				using MySqlCommand mySqlCommand = CommandsWithTimeout.MySQL(query.Query, synchronizeParameters.DatabaseRow.Connection);
				using MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
				while (mySqlDataReader.Read())
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					ReadCount(mySqlDataReader);
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
			backgroundWorkerManager.ReportProgress("Retrieving database's objects");
			try
			{
				if (!string.IsNullOrWhiteSpace(query.Query))
				{
					using MySqlCommand mySqlCommand = CommandsWithTimeout.MySQL(query.Query, synchronizeParameters.DatabaseRow.Connection);
					using MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
					while (mySqlDataReader.Read())
					{
						if (synchronizeParameters.DbSynchLocker.IsCanceled)
						{
							return false;
						}
						AddDBObject(mySqlDataReader, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
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
			using MySqlCommand mySqlCommand = CommandsWithTimeout.MySQL(query, synchronizeParameters.DatabaseRow.Connection);
			using MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
			while (mySqlDataReader.Read())
			{
				AddRelation(mySqlDataReader, DatabaseType);
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
			using MySqlCommand mySqlCommand = CommandsWithTimeout.MySQL(query, synchronizeParameters.DatabaseRow.Connection);
			using MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
			while (mySqlDataReader.Read())
			{
				AddColumn(mySqlDataReader);
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
				using MySqlCommand mySqlCommand = CommandsWithTimeout.MySQL(query, synchronizeParameters.DatabaseRow.Connection);
				using MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
				while (mySqlDataReader.Read())
				{
					AddTrigger(mySqlDataReader);
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
				using MySqlCommand mySqlCommand = CommandsWithTimeout.MySQL(query, synchronizeParameters.DatabaseRow.Connection);
				using MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
				while (mySqlDataReader.Read())
				{
					AddUniqueConstraint(mySqlDataReader, SharedDatabaseTypeEnum.DatabaseType.MySQL);
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
		if (!canReadParameters)
		{
			return true;
		}
		try
		{
			if (!string.IsNullOrWhiteSpace(query))
			{
				using MySqlCommand mySqlCommand = CommandsWithTimeout.MySQL(query, synchronizeParameters.DatabaseRow.Connection);
				using MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
				while (mySqlDataReader.Read())
				{
					AddParameter(mySqlDataReader);
				}
			}
		}
		catch (Exception ex)
		{
			if (ex.InnerException is MySqlException && (ex.InnerException as MySqlException).ErrorCode == MySqlErrorCode.ParseError)
			{
				GeneralMessageBoxesHandling.Show("Due to a <href=" + Links.MariaDBPackage + ">known issue solved in MariaDB 10.3.15</href> import will fail as long as any packages exist in database." + Environment.NewLine + "Update your MariaDB to a newer version, or drop the packages to continue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
				return false;
			}
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetDependencies(string query, Form owner = null)
	{
		if (!string.IsNullOrWhiteSpace(query))
		{
			try
			{
				using MySqlCommand mySqlCommand = CommandsWithTimeout.MySQL(query, synchronizeParameters.DatabaseRow.Connection);
				using MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
				while (mySqlDataReader.Read())
				{
					AddDependency(mySqlDataReader);
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

	public override IEnumerable<string> CountTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Table, "tab.table_schema", "tab.table_name");
		yield return "select \r\n                        'table' as `object_type`,\r\n                        count(1) as `count`\r\n                    from information_schema.tables tab\r\n                    where tab.table_type='BASE TABLE'\r\n                        " + filterString + "\r\n                        and tab.table_schema = '" + synchronizeParameters.DatabaseName + "'";
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.View, "tab.table_schema", "tab.table_name");
		yield return "select 'view' as `object_type`,\r\n                        count(1) as `count`\r\n                    from information_schema.tables tab\r\n                    where tab.table_type='VIEW'\r\n                        " + filterString + "\r\n                        and tab.table_schema = '" + synchronizeParameters.DatabaseName + "'";
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "r.routine_schema", "r.specific_name");
		yield return "select\r\n                        'procedure' as `object_type`,\r\n                        count(1) as `count`\r\n                    from information_schema.routines r\r\n                    where r.routine_type='PROCEDURE'\r\n                        " + filterString + "\r\n                        and r.routine_schema = '" + synchronizeParameters.DatabaseName + "'";
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Function, "r.routine_schema", "r.specific_name");
		yield return "select\r\n                        'function' as `object_type`,\r\n                        count(1) as `count`\r\n                    from information_schema.routines r\r\n                    where r.routine_type='FUNCTION'\r\n                        " + filterString + "\r\n                        and r.routine_schema = '" + synchronizeParameters.DatabaseName + "'";
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Table, "i.table_schema", "i.table_name");
		yield return "select i.table_name as `name`,\r\n                            '' as `schema`,\r\n                            i.table_schema as `database_name`,\r\n                            'TABLE' as `type`,\r\n                            case i.table_comment\r\n                                when '' then null   -- replace empty comment with null\r\n                                else i.table_comment\r\n                                end as `description`,\r\n                            null as `definition`,\r\n                            i.create_time as `create_date`,\r\n                            case when i.update_time is null\r\n                                then i.create_time\r\n                                else i.update_time\r\n                                end as `modify_date`,\r\n                            null as `function_type`\r\n                        from information_schema.tables i\r\n                        where i.table_type = 'BASE TABLE'\r\n                            and i.table_schema = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " '" + synchronizeParameters.DatabaseName + "'\r\n                            " + filterString;
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.View, "i.table_schema", "i.table_name");
		yield return "select i.table_name as `name`,\r\n                              '' as `schema`,\r\n                              i.table_schema as `database_name`,\r\n                              'VIEW' as `type`,\r\n                              case i.table_comment\r\n                                    when '' then null   -- replace empty comment with null\r\n                                    else i.table_comment\r\n                                    end as `description`,\r\n                              v.view_definition as `definition`,\r\n                              i.create_time as `create_date`,\r\n                              case when i.update_time is null\r\n                                   then i.create_time\r\n                                   else i.update_time\r\n                                   end as `modify_date`,\r\n                              null as `function_type`\r\n                         from information_schema.tables i\r\n                              left outer join information_schema.views v\r\n                                   on " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " v.table_schema = i.table_schema\r\n                                   and " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " v.table_name = i.table_name\r\n                        where i.table_type = 'VIEW'\r\n                            and i.table_schema = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " '" + synchronizeParameters.DatabaseName + "'\r\n                            and v.table_schema = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " '" + synchronizeParameters.DatabaseName + "'\r\n                            " + filterString;
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "i.routine_schema", "i.specific_name");
		yield return "select \r\n                              i.specific_name as `name`,\r\n                              '' as `schema`,\r\n                              i.routine_schema as `database_name`,\r\n                              'PROCEDURE' as `type`,\r\n                              case i.routine_comment\r\n                                   when '' then null   -- brak komentarza (opisu) pokazuje jako '', nie jako null, w sql serverze null przy braku komentarza (opisu)\r\n                                   else i.routine_comment   -- jesli null albo slowny komentarz\r\n                              end as `description`,\r\n                              i.routine_definition as `definition`,      -- null dla tabel, dla pozostalych skrypt\r\n                              i.created as `create_date`,\r\n                              case when i.last_altered is null then\r\n                                    i.created\r\n                                else\r\n                                    i.last_altered\r\n                                end as `modify_date`,\r\n                              null as `function_type`\r\n                              from information_schema.routines i\r\n                              where i.routine_type = 'PROCEDURE'\r\n                                and i.routine_schema = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " '" + synchronizeParameters.DatabaseName + "'\r\n                                " + filterString;
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Function, "i.routine_schema", "i.specific_name");
		yield return "select \r\n                              i.specific_name as `name`,\r\n                              '' as `schema`,\r\n                              i.routine_schema as `database_name`,\r\n                              'FUNCTION' as `type`,\r\n                              case i.routine_comment\r\n                                   when '' then null   -- brak komentarza (opisu) pokazuje jako '', nie jako null, w sql serverze null przy braku komentarza (opisu)\r\n                                   else i.routine_comment   -- jesli null albo slowny komentarz\r\n                              end as `description`,\r\n                              i.routine_definition as `definition`,      -- null dla tabel, dla pozostalych skrypt\r\n                              i.created as `create_date`,\r\n                              case when i.last_altered is null then\r\n                                    i.created\r\n                                else\r\n                                    i.last_altered\r\n                                end as `modify_date`,\r\n                              '......' as `function_type`\r\n                              from information_schema.routines i\r\n                              where i.routine_type = 'FUNCTION'\r\n                                and i.routine_schema = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " '" + synchronizeParameters.DatabaseName + "'\r\n                                " + filterString;
	}

	public override IEnumerable<string> RelationsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString(null, base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "rc.constraint_schema", "rc.table_name");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("or", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "rc.constraint_schema", "rc.referenced_table_name");
		string text = FilterRulesCollection.CombineRelationsFilter(filterString, filterString2, "and");
		yield return "select\r\n                rc.constraint_name as `name`,\r\n                rc.constraint_schema AS `fk_table_database_name`,\r\n                rc.table_name as `fk_table_name`,\r\n                '' as `fk_table_schema`,\r\n                kcu.column_name as `fk_column`,\r\n                kcu.referenced_table_schema AS `ref_table_database_name`,\r\n                rc.referenced_table_name as `ref_table_name`,\r\n                '' as `ref_table_schema`,\r\n                kcu.referenced_column_name as `ref_column`,\r\n                kcu.ordinal_position as `ordinal_position`,\r\n                null as `description`,\r\n                rc.update_rule as `update_rule`,\r\n                rc.delete_rule as `delete_rule`\r\n                from information_schema.referential_constraints rc\r\n                    inner join information_schema.key_column_usage kcu \r\n                            on kcu.constraint_name=rc.constraint_name\r\n                            and " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " kcu.constraint_schema=rc.constraint_schema\r\n                            and " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " kcu.table_name=rc.table_name\r\n                where kcu.referenced_table_schema = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " '" + synchronizeParameters.DatabaseName + "'\r\n                    and kcu.table_schema = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " '" + synchronizeParameters.DatabaseName + "'\r\n                    and rc.constraint_schema = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " '" + synchronizeParameters.DatabaseName + "'\r\n                    " + text;
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "c.table_schema", "c.table_name");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.View, "c.table_schema", "c.table_name");
		string text = (getComputedFormula ? "c.generation_expression as `computed_formula`," : "null as `computed_formula`,");
		yield return "select \r\n                   c.table_schema as `database_name`,\r\n                   c.table_name as `table_name`,\r\n                   '' as `table_schema`,\r\n                   c.column_name as `name`,\r\n                   c.ordinal_position as `position`,\r\n                   case when c.column_type like '%unsigned%'\r\n                        then concat('unsigned', ' ', c.data_type)\r\n                        else c.data_type\r\n                   end as `datatype`,\r\n                   case c.column_comment\r\n                        when '' then null   -- brak komentarza (opisu) pokazuje jako '', nie jako null, w sql serverze null przy braku komentarza (opisu)\r\n                        else c.column_comment   -- jesli null albo slowny komentarz\r\n                   end as `description`, \r\n                   CASE iku.constraint_name\r\n                       WHEN 'PRIMARY' THEN 'P'\r\n                   END AS `constraint_type`,\r\n                   case c.is_nullable\r\n                        when 'YES' then 1\r\n                        else 0\r\n                   end as `nullable`, \r\n                   c.column_default as `default_value`,\r\n                   case c.extra\r\n                        when 'auto_increment' then 1\r\n                        else 0 \r\n                   end as `is_identity`,\r\n                   case\r\n                        when c.extra like '%GENERATED%' then 1\r\n                        else 0\r\n                   end as `is_computed`,\r\n                   " + text + "\r\n                   " + QueryDataTypes.GetQueryForDataLength(synchronizeParameters.DatabaseRow.Type) + " \r\n                   FROM information_schema.columns c\r\n                   LEFT JOIN information_schema.views v on c.table_schema = v.table_schema AND c.table_name = v.table_name\r\n                   LEFT OUTER JOIN (SELECT\r\n                        ik.constraint_name,\r\n                        ik.table_name,\r\n                        ik.column_name\r\n                        FROM information_schema.key_column_usage ik WHERE ik.table_schema = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " '" + synchronizeParameters.DatabaseName + "'\r\n                            AND ik.constraint_name= 'PRIMARY'\r\n                            AND ik.constraint_schema = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " '" + synchronizeParameters.DatabaseName + "') iku\r\n                                ON  iku.table_name = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " c.table_name\r\n                                    AND iku.column_name = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " c.column_name   \r\n                   WHERE c.table_schema = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " '" + synchronizeParameters.DatabaseName + "'\r\n                         AND ((v.table_schema is not null " + filterString2 + ")\r\n                            OR(v.table_schema is null " + filterString + "))";
	}

	public override IEnumerable<string> TriggersQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "it.event_object_schema", "it.event_object_table");
		yield return "select \r\n                    'TR' as 'type',\r\n                   it.trigger_name as `trigger_name`,\r\n                   '' as `table_schema`,\r\n                   it.event_object_table as `table_name`,\r\n                   it.event_object_schema as `database_name`,\r\n                   case it.event_manipulation \r\n                        when 'UPDATE' then 1\r\n                        else 0\r\n                   end as `isupdate`,\r\n                   case it.event_manipulation \r\n                        when 'DELETE' then 1\r\n                        else 0\r\n                   end as `isdelete`,\r\n                   case it.event_manipulation \r\n                        when 'INSERT' then 1\r\n                        else 0\r\n                   end as `isinsert`,\r\n                   case it.action_timing \r\n                        when 'BEFORE' then 1\r\n                        else 0\r\n                   end as `isbefore`,\r\n                   case it.action_timing \r\n                        when 'AFTER' then 1\r\n                        else 0\r\n                   end as `isafter`,\r\n                   0 as `isinsteadof`,\r\n                   0 as  `disabled`,\r\n                   it.action_statement as `definition`,\r\n                   null as `description` \r\n                   from information_schema.triggers it \r\n                   where it.event_object_schema = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " '" + synchronizeParameters.DatabaseName + "'" + filterString;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "c.table_schema", "c.table_name");
		yield return "select \r\n                   tc.constraint_schema as `database_name`,\r\n                   tc.table_name as `table_name`,\r\n                   '' as `table_schema`,\r\n                   tc.constraint_name as `name`,\r\n                   case tc.constraint_type\r\n                        when 'PRIMARY KEY' then 'P'\r\n                        when 'UNIQUE' then 'U'\r\n                   end as `type`,\r\n                   kcu.column_name as `column_name`,\r\n                   kcu.ordinal_position as `column_ordinal`,\r\n                   case c.column_comment\r\n                        when '' then null  \r\n                        else c.column_comment\r\n                        end as `description`, \r\n                   0 as `disabled` \r\n                   from information_schema.table_constraints tc \r\n                        inner join information_schema.key_column_usage kcu \r\n                             on kcu.constraint_name=tc.constraint_name\r\n                             and kcu.constraint_schema=tc.constraint_schema\r\n                             and " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " kcu.table_name=tc.table_name\r\n                        inner join information_schema.columns c\r\n                             on kcu.column_name=c.column_name\r\n                             and kcu.constraint_schema=c.table_schema\r\n                             and " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " kcu.table_name=c.table_name\r\n                   where tc.constraint_type in ('PRIMARY KEY','UNIQUE')\r\n                        and tc.table_schema = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " '" + synchronizeParameters.DatabaseName + "'\r\n                        and kcu.table_schema = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " '" + synchronizeParameters.DatabaseName + "'\r\n                        and c.table_schema = " + lowerCaseTableNamesHelper.GetBinaryKeyword() + " '" + synchronizeParameters.DatabaseName + "'" + filterString;
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Procedure, "ic.specific_schema", "ic.specific_name");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Function, "ic.specific_schema", "ic.specific_name");
		yield return "SELECT\r\n                ic.specific_schema as `database_name`,\r\n                ic.specific_name as `procedure_name`,\r\n                '' as `procedure_schema`,\r\n                case when ic.routine_type = 'FUNCTION'\r\n                    and ic.parameter_mode is null\r\n                    then ''\r\n                else\r\n                    ic.parameter_name\r\n                    end\r\n                    as `name`,\r\n                ic.ordinal_position as `position`,\r\n                case \r\n                    when ic.parameter_mode is null then 'OUT'          -- czy zawsze - do spr, dla testowych jest dobrze\r\n                    else ic.parameter_mode\r\n                end as `parameter_mode`,\r\n                ic.data_type as `datatype`, \r\n                " + QueryDataTypes.GetQueryForDataLength(synchronizeParameters.DatabaseRow.Type) + ", \r\n                null as `description`\r\n                from information_schema.parameters ic\r\n                where ic.specific_schema = '" + synchronizeParameters.DatabaseName + "'\r\n                and ((ic.routine_type = 'FUNCTION' " + filterString2 + ") or (ic.routine_type <> 'FUNCTION' " + filterString + "))";
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		DatabaseVersionUpdate version = DatabaseSupportFactory.GetDatabaseSupport(SharedDatabaseTypeEnum.DatabaseType.MySQL).GetVersion(synchronizeParameters.DatabaseRow.Connection);
		if (version.Version > 8 || (version.Version == 8 && version.Update > 0) || (version.Version == 8 && version.Update == 0 && version.Build >= 13))
		{
			string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Table, "tu.table_schema", "tu.table_name");
			string functionFilter = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Function, "ru.table_schema", "ru.specific_name");
			yield return "select\r\n                            '" + synchronizeParameters.DatabaseRow.Host + "' as REFERENCING_SERVER,\r\n                            'VIEW' as REFERENCING_TYPE,\r\n                            '' as REFERENCING_SCHEMA_NAME,\r\n                            tu.view_schema as REFERENCING_DATABASE_NAME,\r\n                            tu.view_name as REFERENCING_ENTITY_NAME,\r\n                            '" + synchronizeParameters.DatabaseRow.Host + "' as REFERENCED_SERVER,\r\n                            tu.view_schema as REFERENCED_DATABASE_NAME,\r\n                            tu.table_schema as REFERENCED_SCHEMA_NAME,\r\n                            'TABLE' REFERENCED_TYPE,\r\n                            tu.table_name as REFERENCED_ENTITY_NAME,\r\n                                                NULL AS IS_CALLER_DEPENDENT,\r\n                                                NULL AS IS_AMBIGUOUS,\r\n                                                NULL AS DEPENDENCY_TYPE    \r\n                        from information_schema.view_table_usage tu\r\n                        where tu.view_schema = '" + synchronizeParameters.DatabaseName + "' \r\n                            AND tu.view_name IS NOT NULL\r\n                            AND tu.table_name IS NOT NULL\r\n                        " + filterString;
			yield return "select\r\n                            '" + synchronizeParameters.DatabaseRow.Host + "' as REFERENCING_SERVER,\r\n                            'VIEW' as REFERENCING_TYPE,\r\n                            '' as REFERENCING_SCHEMA_NAME,\r\n                            ru.table_schema as REFERENCING_DATABASE_NAME,\r\n                            ru.table_name as REFERENCING_ENTITY_NAME,\r\n                            '" + synchronizeParameters.DatabaseRow.Host + "' as REFERENCED_SERVER,\r\n                            ru.table_schema as REFERENCED_DATABASE_NAME,\r\n                            ru.specific_schema as REFERENCED_SCHEMA_NAME,\r\n                            'FUNCTION' REFERENCED_TYPE,\r\n                            ru.specific_name as REFERENCED_ENTITY_NAME,\r\n                                                NULL AS IS_CALLER_DEPENDENT,\r\n                                                NULL AS IS_AMBIGUOUS,\r\n                                                NULL AS DEPENDENCY_TYPE    \r\n                        from information_schema.view_routine_usage ru\r\n                        where ru.table_schema = '" + synchronizeParameters.DatabaseName + "'\r\n                            AND ru.table_name IS NOT NULL\r\n                            AND ru.specific_name IS NOT NULL\r\n                        " + functionFilter;
		}
		else
		{
			yield return string.Empty;
		}
	}

	protected override string JoinObjectsNames(IEnumerable<ObjectRow> rows)
	{
		return RestrictionsForQueries.JoinObjectsNamesForMySQL(rows);
	}
}
