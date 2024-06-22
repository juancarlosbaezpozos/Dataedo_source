using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Model.Data.Object;
using Dataedo.Model.Data.Tables.Triggers;
using Dataedo.Shared.Enums;
using Sap.Data.Hana;

namespace Dataedo.App.Data.SapHana;

internal class SynchronizeSapHana : SynchronizeDatabase
{
	public SynchronizeSapHana(SynchronizeParameters synchronizeParameters)
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
				using HanaCommand hanaCommand = CommandsWithTimeout.SapHana(query.Query, synchronizeParameters.DatabaseRow.Connection);
				using HanaDataReader hanaDataReader = hanaCommand.ExecuteReader();
				while (hanaDataReader.Read())
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					ReadCount(hanaDataReader);
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
				if (string.IsNullOrWhiteSpace(synchronizeParameters.DatabaseRow.Name))
				{
					using HanaCommand hanaCommand = CommandsWithTimeout.SapHana("select database_name from sys.m_database;", synchronizeParameters.DatabaseRow.Connection);
					using HanaDataReader hanaDataReader = hanaCommand.ExecuteReader();
					if (hanaDataReader.Read())
					{
						synchronizeParameters.DatabaseRow.Name = hanaDataReader["database_name"] as string;
					}
				}
				if (!string.IsNullOrWhiteSpace(query.Query))
				{
					using HanaCommand hanaCommand2 = CommandsWithTimeout.SapHana(query.Query, synchronizeParameters.DatabaseRow.Connection);
					using HanaDataReader hanaDataReader2 = hanaCommand2.ExecuteReader();
					while (hanaDataReader2.Read())
					{
						if (synchronizeParameters.DbSynchLocker.IsCanceled)
						{
							return false;
						}
						ObjectSynchronizationObject obj = new ObjectSynchronizationObject(hanaDataReader2, new List<int>(), SharedDatabaseTypeEnum.DatabaseType.SapHana);
						ObjectRow objectRow = synchronizeParameters.DatabaseRow.tableRows.Where((ObjectRow x) => x.Name == obj.Name && x.DatabaseName == obj.DatabaseName && x.Schema == obj.Schema && x.TypeAsString == obj.Type).FirstOrDefault();
						if (objectRow != null)
						{
							objectRow.Definition += obj.Definition;
						}
						else
						{
							AddDBObject(hanaDataReader2, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager);
						}
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
			using HanaCommand hanaCommand = CommandsWithTimeout.SapHana(query, synchronizeParameters.DatabaseRow.Connection);
			using HanaDataReader hanaDataReader = hanaCommand.ExecuteReader();
			while (hanaDataReader.Read())
			{
				AddRelation(hanaDataReader, SharedDatabaseTypeEnum.DatabaseType.SapHana);
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
			using HanaCommand hanaCommand = CommandsWithTimeout.SapHana(query, synchronizeParameters.DatabaseRow.Connection);
			using HanaDataReader hanaDataReader = hanaCommand.ExecuteReader();
			while (hanaDataReader.Read())
			{
				AddColumn(hanaDataReader);
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
			using HanaCommand hanaCommand = CommandsWithTimeout.SapHana(query, synchronizeParameters.DatabaseRow.Connection);
			using HanaDataReader hanaDataReader = hanaCommand.ExecuteReader();
			while (hanaDataReader.Read())
			{
				TriggerRow trig = new TriggerRow(new TriggerSynchronizationObject(hanaDataReader, new List<int>(), SharedDatabaseTypeEnum.DatabaseType.SapHana));
				TriggerRow triggerRow = base.TriggerRows.Where((TriggerRow x) => x.Name == trig.Name && x.TableSchema == trig.TableSchema && x.TableSchema == trig.TableSchema && x.DatabaseName == trig.DatabaseName).FirstOrDefault();
				if (triggerRow != null)
				{
					triggerRow.Definition += trig.Definition;
				}
				else
				{
					AddTrigger(hanaDataReader);
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
			using HanaCommand hanaCommand = CommandsWithTimeout.SapHana(query, synchronizeParameters.DatabaseRow.Connection);
			using HanaDataReader hanaDataReader = hanaCommand.ExecuteReader();
			while (hanaDataReader.Read())
			{
				AddUniqueConstraint(hanaDataReader, SharedDatabaseTypeEnum.DatabaseType.SapHana);
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
			using HanaCommand hanaCommand = CommandsWithTimeout.SapHana(query, synchronizeParameters.DatabaseRow.Connection);
			using HanaDataReader hanaDataReader = hanaCommand.ExecuteReader();
			while (hanaDataReader.Read())
			{
				AddParameter(hanaDataReader);
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
		return true;
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Table, "t.schema_name", "t.table_name");
		yield return "select \r\n                                'TABLE' AS object_type,\r\n                                COUNT(1) AS count\r\n                            from sys.tables t\r\n                            where table_type <> 'COLLECTION'  -- filter out collections to parse them differently\r\n                                and is_system_table = 'FALSE'\r\n                                and schema_name not like '%SYS%'\r\n                                and schema_name not in ('SAP_PA_APL', 'PAL_STEM_TFIDF')\r\n                                " + filterString;
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "v.schema_name", "v.view_name");
		yield return "select \r\n\t                        'VIEW' as object_type,\r\n\t                        count(1) as count\r\n                            from SYS.VIEWS v\r\n                            where schema_name not like '%SYS%'\r\n                            and schema_name not in ('SAP_PA_APL', 'PAL_STEM_TFIDF')\r\n                            " + filterString;
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "p.schema_name", "p.procedure_name");
		yield return "select \r\n\t                        'PROCEDURE' as object_type,\r\n\t                        count(1) as count\r\n                        from SYS.PROCEDURES p\r\n                        where schema_name not like '%SYS%'\r\n                        and schema_name not in ('SAP_PA_APL', 'PAL_STEM_TFIDF')\r\n                        " + filterString;
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "f.schema_name", "f.function_name");
		yield return "select \r\n\t                        'FUNCTION' as object_type,\r\n\t                        count(1) as count\r\n                        from SYS.FUNCTIONS f\r\n                        where schema_name not like '%SYS%'\r\n                        and schema_name not in ('SAP_PA_APL', 'PAL_STEM_TFIDF')\r\n                    " + filterString;
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "t.schema_name", "t.table_name");
		yield return "\r\n                            select distinct\r\n                                t.SCHEMA_NAME as schema,\r\n                                t.TABLE_NAME as name,\r\n                                gc.WORKSPACE_NAME as title,\r\n                                (select database_name from sys.m_database limit 1) as database_name,\r\n                                case gc.ENTITY_TYPE\r\n                                    when 'EDGE' then 'GRAPH_EDGE_TABLE'\r\n                                    when 'VERTEX' then 'GRAPH_NODE_TABLE'\r\n                                    else 'TABLE'\r\n                                end as type,\r\n                                case t.COMMENTS\r\n                                    when '' then null -- replace empty comment with null\r\n                                    else COMMENTS\r\n                                    end as description,\r\n                                t.CREATE_TIME as create_date,\r\n                                null as modify_date,\r\n                                '' as definition,\r\n                                null as function_type\r\n                            from SYS.TABLES t\r\n                            left join SYS.GRAPH_WORKSPACE_COLUMNS gc\r\n                                on t.SCHEMA_NAME = gc.ENTITY_SCHEMA_NAME\r\n                                and t.TABLE_NAME = gc.ENTITY_TABLE_NAME\r\n                            where TABLE_TYPE <> 'COLLECTION'  -- filter out collections to parse them differently\r\n                                and t.IS_SYSTEM_TABLE = 'FALSE'\r\n                                and t.SCHEMA_NAME not like '%SYS%'\r\n                                and t.SCHEMA_NAME not in  ('SAP_PA_APL', 'PAL_STEM_TFIDF')\r\n\r\n                                " + filterString;
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "v.schema_name", "v.view_name");
		yield return "\r\n                            select\r\n\t                            v.SCHEMA_NAME as schema,\r\n\t                            v.VIEW_NAME as name,\r\n                                '' as title,\r\n\t                            (select database_name from sys.m_database limit 1) as database_name,\r\n\t                            'VIEW' as type,\r\n\t                            case v.COMMENTS\r\n\t\t                            when '' then null -- replace empty comment with null\r\n\t\t                            else v.COMMENTS\r\n\t\t                            end as description,\r\n\t                            v.DEFINITION as definition,\r\n\t                            v.CREATE_TIME as create_date,\r\n                                null as modify_date,\r\n                                null as function_type\r\n                            from SYS.VIEWS v\r\n                            where SCHEMA_NAME not like '%SYS%'\r\n                                and SCHEMA_NAME not in  ('SAP_PA_APL', 'PAL_STEM_TFIDF')\r\n                                " + filterString;
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "p.schema_name", "p.procedure_name");
		yield return "\r\n                            select\r\n\t                            p.procedure_name as name,\r\n                                '' as title,\r\n                                p.schema_name as schema,\r\n                                (select database_name from sys.m_database limit 1) as database_name,\r\n                                'PROCEDURE' as type,\r\n                                '' as description,\r\n                                p.definition as definition,\r\n                                p.create_time as create_date,\r\n                                null as modify_date,\r\n                                null as function_type\r\n                            from SYS.PROCEDURES p\r\n                            where SCHEMA_NAME not like '%SYS%'\r\n                                and SCHEMA_NAME not in  ('SAP_PA_APL', 'PAL_STEM_TFIDF')\r\n                                " + filterString;
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "f.schema_name", "f.function_name");
		yield return "\r\n                            select\r\n\t                            f.function_name as name,\r\n                                '' as title,\r\n                                f.schema_name as schema,\r\n                                (select database_name from sys.m_database limit 1) as database_name,\r\n                                'FUNCTION' as type,\r\n                                f.definition as definition,\r\n                                f.create_time as create_date,\r\n                                null as modify_date,\r\n                                null as function_type,\r\n                                '' as description\r\n                            from SYS.FUNCTIONS f\r\n                            where SCHEMA_NAME not like '%SYS%'\r\n                                and SCHEMA_NAME not in  ('SAP_PA_APL', 'PAL_STEM_TFIDF')\r\n                                                            " + filterString;
	}

	public override IEnumerable<string> RelationsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString(null, base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "rc.schema_name", "rc.table_name");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("OR", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "rc.referenced_schema_name", "rc.referenced_table_name");
		string text = FilterRulesCollection.CombineRelationsFilter(filterString, filterString2);
		yield return "\r\n                    select\r\n                        rc.constraint_name as name,\r\n                        od.dependent_database_name as fk_table_database_name,\r\n                        rc.table_name AS fk_table_name,\r\n                        rc.schema_name as fk_table_schema,\r\n                        rc.column_name as fk_column,\r\n                        od.base_database_name as ref_table_database_name,\r\n                        rc.referenced_table_name AS ref_table_name,\r\n                        rc.referenced_schema_name as ref_table_schema,\r\n                        rc.referenced_column_name as ref_column,\r\n                        rc.position as ordinal_position,\r\n                        '' as description,\r\n                        rc.update_rule as update_rule,\r\n                        rc.delete_rule as delete_rule\r\n                    from sys.referential_constraints rc\r\n                    left join sys.graph_workspace_columns gc\r\n                        on rc.schema_name = gc.entity_schema_name\r\n                        and rc.table_name = gc.entity_table_name\r\n                        and rc.column_name = gc.entity_column_name\r\n                    left join sys.graph_workspace_columns gc2\r\n                        on rc.referenced_schema_name = gc2.entity_schema_name\r\n                        and rc.referenced_table_name = gc2.entity_table_name\r\n                        and rc.referenced_column_name = gc2.entity_column_name\r\n                    join sys.object_dependencies od\r\n                        on rc.table_name = od.dependent_object_name\r\n                        and rc.schema_name = od.dependent_schema_name\r\n                        and rc.referenced_table_name = od.base_object_name\r\n                        and rc.referenced_schema_name = od.base_schema_name\r\n                    where rc.schema_name not like '%SYS%'\r\n                    and rc.schema_name not in  ('SAP_PA_APL', 'PAL_STEM_TFIDF')\r\n                    and od.dependency_type = 5\r\n                    " + text;
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "r.table_schema", "r.table_name");
		yield return "select *\r\n                                from (\r\n                                    select\r\n    \t                                (select database_name from sys.m_database limit 1) as database_name,\r\n    \t                                t.TABLE_NAME as table_name,\r\n    \t                                t.SCHEMA_NAME as table_schema,\r\n    \t                                c.COLUMN_NAME as name,\r\n    \t                                c.POSITION as position,\r\n    \t                                c.DATA_TYPE_NAME as datatype,\r\n    \t                                case c.IS_NULLABLE\r\n    \t\t                                when 'TRUE' then 1\r\n    \t\t                                else 0\r\n    \t                                end as nullable,\r\n    \t                                c.DEFAULT_VALUE as default_value,\r\n    \t                                case c.GENERATION_TYPE\r\n    \t\t                                when 'NULL' then 0\r\n    \t\t                                else 1\r\n    \t                                end as is_computed,\r\n    \t                                case\r\n    \t\t                                when  cs.IS_PRIMARY_KEY = 'TRUE' then 'P'\r\n    \t\t                                when  cs.IS_UNIQUE_KEY = 'TRUE' and cs.IS_PRIMARY_KEY = 'FALSE' then 'U'\r\n    \t\t                                else NULL\r\n    \t                                end as constraint_type,\r\n    \t                                c.comments as description,\r\n                                        0 as is_identity,\r\n                                        null as computed_formula,\r\n                                        c.length as data_length\r\n                                    from SYS.TABLE_COLUMNS c\r\n                                    left join SYS.TABLES t \r\n    \t                                on c.TABLE_OID = t.TABLE_OID\r\n                                    left join SYS.CONSTRAINTS cs\r\n    \t                                on c.COLUMN_NAME = cs.COLUMN_NAME\r\n    \t                                and c.SCHEMA_NAME = cs.SCHEMA_NAME\r\n                                        and c.TABLE_NAME = cs.TABLE_NAME\r\n    \t                            left join SYS.GRAPH_WORKSPACE_COLUMNS gc\r\n                                        on c.SCHEMA_NAME = gc.ENTITY_SCHEMA_NAME\r\n                                        and c.TABLE_NAME = gc.ENTITY_TABLE_NAME\r\n                                    union\r\n                                    select\r\n                                        (select database_name from sys.m_database limit 1) as database_name,\r\n                                        v.VIEW_NAME as table_name,\r\n                                        v.schema_name as table_schema,\r\n                                        v.column_name as name,\r\n                                        v.position as position,\r\n                                        v.data_type_name as datatype,\r\n       \t                                case v.IS_NULLABLE\r\n    \t\t                                when 'TRUE' then 1\r\n    \t\t                                else 0\r\n    \t                                end as nullable,\r\n    \t                                v.DEFAULT_VALUE as default_value,\r\n    \t                                case v.GENERATION_TYPE\r\n    \t\t                                when 'NULL' then 0\r\n    \t\t                                else 1\r\n    \t                                end as is_computed,\r\n    \t                                NULL as constraint_type,\r\n    \t                                v.comments as description,\r\n                                        0 as is_identity,\r\n                                        null as computed_formula,\r\n                                        v.length as data_length\r\n                                    from sys.view_columns v\r\n                                ) as r\r\n                                where r.table_schema not like '%SYS%' and r.table_schema not in  ('SAP_PA_APL', 'PAL_STEM_TFIDF')\r\n                                " + filterString;
	}

	public override IEnumerable<string> TriggersQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "t.schema_name", "t.trigger_name");
		yield return "select\r\n\t                        'TR' as type,\r\n\t                        t.TRIGGER_NAME as trigger_name,\r\n\t                        (select database_name from sys.m_database limit 1) as database_name,\r\n                            t.SCHEMA_NAME as table_schema,\r\n\t                        t.SUBJECT_TABLE_NAME as table_name,\r\n\t                        case t.TRIGGER_EVENT \r\n\t\t                        when 'UPDATE' then 1\r\n                                else 0\r\n                            end as isupdate,\r\n                            case t.TRIGGER_EVENT \r\n                                 when 'DELETE' then 1\r\n                                 else 0\r\n                            end as isdelete,\r\n                            case t.TRIGGER_EVENT \r\n                                  when 'INSERT' then 1\r\n                                  else 0\r\n                            end as isinsert,\r\n                            case t.TRIGGER_EVENT \r\n                                  when 'BEFORE' then 1\r\n                                  else 0\r\n                            end as isbefore,\r\n                            case t.TRIGGER_EVENT \r\n                                   when 'AFTER' then 1\r\n                                   else 0\r\n                            end as isafter,\r\n                            case t.IS_ENABLED\r\n    \t                        when 'TRUE' then 0\r\n    \t                        else 1\r\n                            end as disabled,\r\n                            t.DEFINITION as definition,\r\n                            null as description,\r\n                            null as isinsteadof\r\n                        from SYS.TRIGGERS t\r\n                        where t.SCHEMA_NAME not like '%SYS%' and t.SCHEMA_NAME not in ('SAP_PA_APL', 'PAL_STEM_TFIDF')\r\n                    " + filterString;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "table_schema", "table_name");
		yield return "select * from (\r\n                        select\r\n                            (select database_name from sys.m_database limit 1) as database_name,\r\n                            c.table_name as table_name,\r\n                            c.schema_name as table_schema,\r\n                            c.constraint_name as name,\r\n                            c.column_name as column_name,\r\n                            c.position as column_ordinal,\r\n                            '' as description,\r\n                            0 as disabled,\r\n                            case\r\n                                when c.is_primary_key = 'TRUE' then 'P'\r\n                                when c.is_unique_key = 'TRUE' then 'U'\r\n                            end as type\r\n                        from SYS.CONSTRAINTS c\r\n                        left join SYS.GRAPH_WORKSPACE_COLUMNS gc\r\n                            on c.schema_name = gc.entity_schema_name\r\n                            and c.table_name = gc.entity_table_name\r\n                            and c.column_name = gc.entity_column_name\r\n                        where c.schema_name not like '%SYS%' and c.schema_name not in ('SAP_PA_APL', 'PAL_STEM_TFIDF')\r\n                        ) where 1=1 " + filterString;
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Procedure, "p.schema_name", "p.procedure_name");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Function, "f.schema_name", "f.function_name");
		yield return "select\r\n                            (select database_name from sys.m_database limit 1) as database_name,\r\n                            p.procedure_name as procedure_name,\r\n                            p.schema_name as procedure_schema,\r\n                            p.parameter_name as name,\r\n                            p.position as position,\r\n                            p.parameter_type as parameter_mode,\r\n                            p.data_type_name as datatype,\r\n                            p.length as data_length,\r\n                            '' as description\r\n                        from sys.procedure_parameters p\r\n                        where p.schema_name not like '%SYS%' and p.schema_name not in ('SAP_PA_APL', 'PAL_STEM_TFIDF')\r\n                        " + filterString + "\r\n                        union \r\n                        select\r\n                            (select database_name from sys.m_database limit 1) as database_name,\r\n                            f.function_name as procedure_name,\r\n                            f.schema_name as procedure_schema,\r\n                            f.parameter_name as name,\r\n                            f.position as position,\r\n                            f.parameter_type as parameter_mode,\r\n                            f.data_type_name as datatype,\r\n                            f.length as data_length,\r\n                            '' as description\r\n                        from sys.function_parameters f\r\n                        where f.schema_name not like '%SYS%' and f.schema_name not in ('SAP_PA_APL', 'PAL_STEM_TFIDF')\r\n                        " + filterString2;
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		yield return string.Empty;
	}
}
