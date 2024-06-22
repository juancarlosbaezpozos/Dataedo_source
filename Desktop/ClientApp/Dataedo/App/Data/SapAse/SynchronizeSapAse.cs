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
using Sybase.Data.AseClient;

namespace Dataedo.App.Data.SapAse;

internal class SynchronizeSapAse : SynchronizeDatabase
{
	public SynchronizeSapAse(SynchronizeParameters synchronizeParameters)
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
				using AseCommand aseCommand = CommandsWithTimeout.SapAse(query.Query, synchronizeParameters.DatabaseRow.Connection);
				using AseDataReader aseDataReader = aseCommand.ExecuteReader();
				while (aseDataReader.Read())
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					ReadCount(aseDataReader);
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
					using AseCommand aseCommand = CommandsWithTimeout.SapAse(query.Query, synchronizeParameters.DatabaseRow.Connection);
					using AseDataReader aseDataReader = aseCommand.ExecuteReader();
					while (aseDataReader.Read())
					{
						if (synchronizeParameters.DbSynchLocker.IsCanceled)
						{
							return false;
						}
						ObjectSynchronizationObject obj = new ObjectSynchronizationObject(aseDataReader, new List<int>(), SharedDatabaseTypeEnum.DatabaseType.SapAse);
						ObjectRow objectRow = synchronizeParameters.DatabaseRow.tableRows.Where((ObjectRow x) => x.Name == obj.Name && x.DatabaseName == obj.DatabaseName && x.Schema == obj.Schema && x.TypeAsString == obj.Type).FirstOrDefault();
						if (objectRow != null)
						{
							objectRow.Definition += obj.Definition;
						}
						else
						{
							AddDBObject(aseDataReader, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager);
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
			using AseCommand aseCommand = CommandsWithTimeout.SapAse(query, synchronizeParameters.DatabaseRow.Connection);
			using AseDataReader aseDataReader = aseCommand.ExecuteReader();
			while (aseDataReader.Read())
			{
				AddRelation(aseDataReader, SharedDatabaseTypeEnum.DatabaseType.SapAse);
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
			using AseCommand aseCommand = CommandsWithTimeout.SapAse(query, synchronizeParameters.DatabaseRow.Connection);
			using AseDataReader aseDataReader = aseCommand.ExecuteReader();
			while (aseDataReader.Read())
			{
				AddColumn(aseDataReader);
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
			using AseCommand aseCommand = CommandsWithTimeout.SapAse(query, synchronizeParameters.DatabaseRow.Connection);
			using AseDataReader aseDataReader = aseCommand.ExecuteReader();
			while (aseDataReader.Read())
			{
				TriggerRow trig = new TriggerRow(new TriggerSynchronizationObject(aseDataReader, new List<int>(), SharedDatabaseTypeEnum.DatabaseType.SapAse));
				TriggerRow triggerRow = base.TriggerRows.Where((TriggerRow x) => x.Name == trig.Name && x.TableSchema == trig.TableSchema && x.TableSchema == trig.TableSchema && x.DatabaseName == trig.DatabaseName).FirstOrDefault();
				if (triggerRow != null)
				{
					triggerRow.Definition += trig.Definition;
				}
				else
				{
					AddTrigger(aseDataReader);
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
			using AseCommand aseCommand = CommandsWithTimeout.SapAse(query, synchronizeParameters.DatabaseRow.Connection);
			using AseDataReader aseDataReader = aseCommand.ExecuteReader();
			while (aseDataReader.Read())
			{
				AddUniqueConstraint(aseDataReader, SharedDatabaseTypeEnum.DatabaseType.SapAse);
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
			using AseCommand aseCommand = CommandsWithTimeout.SapAse(query, synchronizeParameters.DatabaseRow.Connection);
			using AseDataReader aseDataReader = aseCommand.ExecuteReader();
			while (aseDataReader.Read())
			{
				AddParameter(aseDataReader);
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
			using AseCommand aseCommand = CommandsWithTimeout.SapAse(query, synchronizeParameters.DatabaseRow.Connection);
			using AseDataReader aseDataReader = aseCommand.ExecuteReader();
			while (aseDataReader.Read())
			{
				AddDependency(aseDataReader);
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
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "su.name", "so.name");
		yield return "select \r\n                                'TABLE' AS 'object_type',\r\n                                COUNT(1) AS 'count'\r\n                            from sysobjects so\r\n                            inner join sysusers su\r\n                                on so.uid = su.uid\r\n                            where so.type = 'U'\r\n                                " + filterString;
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "su.name", "so.name");
		yield return "select \r\n                                'VIEW' AS 'object_type',\r\n                                COUNT(1) AS 'count'\r\n                            from sysobjects so\r\n                            inner join sysusers su\r\n                                on so.uid = su.uid\r\n                            where so.type = 'V'\r\n                                and so.schemaid = 0\r\n                                " + filterString;
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "su.name", "so.name");
		yield return "select \r\n                                'PROCEDURE' AS 'object_type',\r\n                                COUNT(1) AS 'count'\r\n                            from sysobjects so\r\n                            inner join sysusers su\r\n                                on so.uid = su.uid\r\n                            where so.type in ('P','XP') \r\n                                and so.schemaid = 0\r\n                                " + filterString;
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Function, "su.name", "so.name");
		yield return "select \r\n                    'FUNCTION' AS 'object_type',\r\n                    COUNT(1) AS 'count'\r\n                from sysobjects so\r\n                inner join sysusers su\r\n                    on so.uid = su.uid\r\n                where so.type in ('F', 'SF')\r\n                    and so.schemaid = 0\r\n                    " + filterString;
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Table, "su.name", "so.name");
		yield return "select \r\n                                so.name as 'name',\r\n                                su.name as 'schema',\r\n                                db_name() as 'database_name',\r\n                                'TABLE' as 'type',\r\n                                null as 'description',\r\n                                '' as 'definition',\r\n                                so.crdate as 'create_date',\r\n                                null as modify_date,\r\n                                null as 'function_type'\r\n                            from sysobjects so\r\n                            inner join sysusers su\r\n                                on so.uid = su.uid\r\n                            where so.type = 'U'\r\n                                and so.schemaid = 0\r\n                                " + filterString;
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.View, "su.name", "so.name");
		yield return "select \r\n                                so.name as 'name',\r\n                                su.name as 'schema',\r\n                                db_name() as 'database_name',\r\n                                'VIEW' as 'type',\r\n                                '' as 'description',\r\n                                scom.text as 'definition',\r\n                                so.crdate as 'create_date',\r\n                                null as modify_date,\r\n                                null as 'function_type'\r\n                            from sysobjects so\r\n                            inner join sysusers su\r\n                                on so.uid = su.uid\r\n                            left join syscomments scom\r\n                                ON so.id = scom.id\r\n                            where so.type = 'V'\r\n                                and so.schemaid = 0\r\n                                " + filterString + "\r\n                            order by so.id, scom.colid";
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "su.name", "so.name");
		yield return "select \r\n                                so.name as 'name',\r\n                                su.name as 'schema',\r\n                                db_name() as 'database_name',\r\n                                'PROCEDURE' as 'type',\r\n                                '' as 'description',\r\n                                scom.text as 'definition',\r\n                                so.crdate as 'create_date',\r\n                                null as modify_date,\r\n                                null as 'function_type'\r\n                            from sysobjects so\r\n                            inner join sysusers su\r\n                                on so.uid = su.uid\r\n                            left join syscomments scom\r\n                                ON so.id = scom.id\r\n                            where (so.type = 'P'\r\n                                or so.type = 'XP')\r\n                                and so.schemaid = 0\r\n                                " + filterString + "\r\n                            order by so.id, scom.colid";
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("AND", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "su.name", "so.name");
		yield return "select \r\n                                so.name as 'name',\r\n                                su.name as 'schema',\r\n                                db_name() as 'database_name',\r\n                                'FUNCTION' as 'type',\r\n                                '' as 'description',\r\n                                scom.text as 'definition',\r\n                                so.crdate as 'create_date',\r\n                                null as modify_date,\r\n                                null as 'function_type'\r\n                            from sysobjects so\r\n                            inner join sysusers su\r\n                                on so.uid = su.uid\r\n                            left join syscomments scom\r\n                                ON so.id = scom.id\r\n                            where so.type in ('F', 'SF')\r\n                                and so.schemaid = 0\r\n                                " + filterString + "\r\n                            order by so.id, scom.colid";
	}

	public override IEnumerable<string> RelationsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString(null, base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "su.name", "so.name");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("OR", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "su_ref.name", "so_ref.name");
		string combinedFilter = FilterRulesCollection.CombineRelationsFilter(filterString, filterString2);
		for (int i = 1; i < 17; i++)
		{
			yield return $" SELECT so_sr.name AS 'NAME'\r\n\t                            ,isnull(sr.frgndbname, db_name()) AS 'FK_TABLE_DATABASE_NAME'\r\n\t                            ,so.name AS 'FK_TABLE_NAME'\r\n\t                            ,su.name AS 'FK_TABLE_SCHEMA'\r\n\t                            ,sc.name AS 'FK_COLUMN'\r\n\t                            ,isnull(sr.pmrydbname, db_name()) AS 'REF_TABLE_DATABASE_NAME'\r\n\t                            ,so_ref.name AS 'REF_TABLE_NAME'\r\n\t                            ,su_ref.name AS 'REF_TABLE_SCHEMA'\r\n\t                            ,sc_ref.name AS 'REF_COLUMN'\r\n\t                            ,{i} AS 'ORDINAL_POSITION'\r\n\t                            ,NULL AS 'DESCRIPTION'\r\n\t                            ,'' AS 'UPDATE_RULE'\r\n\t                            ,'' AS 'DELETE_RULE'\r\n                            FROM sysreferences sr\r\n                            INNER JOIN sysobjects so_sr ON so_sr.id = sr.constrid\r\n                            INNER JOIN sysobjects so ON sr.tableid = so.id\r\n                            INNER JOIN sysusers su ON so.uid = su.uid\r\n                            INNER JOIN sysobjects so_ref ON sr.reftabid = so_ref.id\r\n                            INNER JOIN sysusers su_ref ON so.uid = su_ref.uid\r\n                            INNER JOIN syscolumns sc ON sr.tableid = sc.id\r\n\t                            AND sr.fokey{i} = sc.colid\r\n\t                            AND sc.number = 0\r\n                            INNER JOIN syscolumns sc_ref ON sr.reftabid = sc_ref.id\r\n\t                            AND sr.refkey{i} = sc_ref.colid\r\n\t                            AND sc_ref.number = 0\r\n                            WHERE sr.fokey{i} != 0 \r\n                            {combinedFilter} ";
		}
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("or", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "su.name", "so.name");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("or", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.View, "su.name", "so.name");
		yield return "select \r\n                db_name() as 'database_name',\r\n                so.name as 'table_name',\r\n                su.name as 'table_schema',\r\n                sc.name as 'name',\r\n                sc.colid as 'position',\r\n                st.name as 'datatype',\r\n                st.length as 'data_length',\r\n                null as 'description',\r\n                (select 'P' as PK_FLAG from syskeys K where so.id=K.id\r\n                        and K.type = 1 and (sc.colid =K.key1 or sc.colid =K.key2 OR sc.colid =K.key3  OR sc.colid =K.key4 OR sc.colid =K.key5 OR sc.colid =K.key6 OR sc.colid =K.key7 Or sc.colid =K.key8 )) as 'constraint_type',\r\n                case \r\n                    when sc.status & 8 = 8 then 1\r\n                    else 0\r\n                end as 'nullable', \r\n                case when scom.text like 'DEFAULT%' then scom.text else null end as 'default_value',\r\n                case \r\n                    when sc.status & 128 = 128 then 1\r\n                    else 0\r\n                end as 'is_identity',\r\n                case \r\n                    when scom.text is null then 0\r\n                    else 1\r\n                 end as 'is_computed',\r\n                 case when scom.text not like 'DEFAULT%' then scom.text else null end as 'computed_formula'\r\n            from syscolumns sc\r\n            inner join sysobjects so\r\n                on so.id = sc.id\r\n                and so.schemaid = 0\r\n                and sc.number = 0\r\n                and so.type in ('U', 'V')\r\n            inner join sysusers su\r\n                on so.uid = su.uid\r\n            inner join systypes st\r\n                on sc.usertype = st.usertype\r\n            left join syscomments scom\r\n                on scom.id = sc.cdefault\r\n            where 1 = 1\r\n            " + filterString + "\r\n            " + filterString2;
	}

	public override IEnumerable<string> TriggersQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "su.name", "so.name");
		yield return "select\r\n                    'TR' AS 'type',\r\n                    strig.name as 'trigger_name',\r\n                    su.name as 'table_schema',\r\n                    so.name as 'table_name',\r\n                    db_name() as 'database_name',\r\n                    case \r\n                        when so.updtrig != 0 then 1\r\n                        when sc.status & 512 = 512 then 1\r\n                        else 0\r\n                    end as 'isupdate',\r\n                    case \r\n                        when so.deltrig != 0 then 1\r\n                        when sc.status & 128 = 128 then 1\r\n                        else 0\r\n                    end as 'isdelete',\r\n                    case \r\n                        when so.instrig != 0 then 1\r\n                        when sc.status & 256 = 256 then 1\r\n                        else 0\r\n                    end as 'isinsert',\r\n                    null as 'isbefore',\r\n                    null as 'isafter',\r\n                    null as 'isinsteadof',\r\n                    case\r\n                        when isnull(sc.status,0) & 4096  = 4096 then 1\r\n                        when isnull(sc.status,0) != 0 then 0\r\n                        when (strig.id = so.deltrig and so.sysstat2 & 2097152 = 2097152) then 1\r\n                        when (strig.id = so.instrig and so.sysstat2 & 1048576 = 1048576) then 1\r\n                        when (strig.id = so.updtrig and so.sysstat2 & 4194304 = 4194304) then 1\r\n                        else 0\r\n                    end as 'disabled',\r\n                    scom.text as 'definition',\r\n                    '' as 'description'\r\n                from sysobjects strig\r\n                inner join sysusers su\r\n                    on strig.uid = su.uid\r\n                left join sysobjects so\r\n                    on strig.deltrig = so.id\r\n                left join sysconstraints sc\r\n                    on strig.id = sc.constrid\r\n                left join syscomments scom\r\n                    on strig.id = scom.id\r\n                where strig.type = 'TR'\r\n                    " + filterString;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForTables, FilterObjectTypeEnum.FilterObjectType.Table, "so.name", "su.name");
		yield return "SELECT\r\n                                db_name() as 'database_name',\r\n                                so.name as 'table_name',\r\n                                su.name as 'table_schema',\r\n                                si.name as 'name',\r\n                                case when (si.status & 2048 = 2048) then 'P'\r\n                                    else 'U'\r\n                                end as 'type',\r\n                                sc.name as 'column_name',\r\n                                sc.colid as 'column_ordinal',\r\n                                '' as 'description', \r\n                                0 as 'disabled' \r\n                            from   sysobjects so \r\n                            inner join sysindexes si \r\n                                on si.id = so.id\r\n                            inner join syscolumns sc \r\n                                on sc.id = so.id\r\n                            inner join sysusers su\r\n                                on so.uid = su.uid\r\n                            where\r\n                                so.type = 'U'\r\n                                and si.indid > 0\r\n                                and si.status & 2 = 2\r\n                                and index_col(so.name, si.indid, sc.colid) <> null\r\n                                " + filterString;
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Function, "so.name", "su.name");
		string filterString2 = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", base.RestrictionsForProcedures, FilterObjectTypeEnum.FilterObjectType.Procedure, "so.name", "su.name");
		yield return "select     \r\n    db_name() as 'database_name',\r\n    so.name as 'procedure_name',\r\n    su.name as 'procedure_schema',\r\n    sc.name as 'name',\r\n    case when sc.status2 = 2 then 0\r\n        else sc.colid\r\n    end as 'position',\r\n    case when sc.status2 = 2 then 'OUT'\r\n        else 'IN'\r\n    end as 'parameter_mode',\r\n    st.name as 'datatype',\r\n    st.length as 'data_length',\r\n    null as 'description'\r\nfrom syscolumns sc\r\ninner join sysobjects so\r\n    on so.id = sc.id\r\n    and so.schemaid = 0\r\n    and so.type in ('P', 'XP', 'SF','F')\r\ninner join sysusers su\r\n    on so.uid = su.uid\r\ninner join systypes st\r\n    on sc.usertype = st.usertype\r\n    " + filterString + "\r\n    " + filterString2;
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		synchronizeParameters.DatabaseRow.Name.ToUpper();
		string text = synchronizeParameters.DatabaseRow.Host.ToUpper();
		string text2 = "(CASE WHEN so.type IN ('V') THEN 'VIEW'\r\n                      WHEN so.type IN ('P','XP') THEN 'PROCEDURE'\r\n                      WHEN so.type IN ('TR') THEN 'TRIGGER'\r\n                      WHEN so.type in ('F','SF') THEN 'FUNCTION'\r\n                    ELSE NULL\r\n                END)";
		string text3 = "(CASE WHEN so_dep.type IN ('U') THEN 'TABLE'\r\n                      WHEN so.type IN ('P','XP') THEN 'PROCEDURE'\r\n                      WHEN so_dep.type IN ('V') THEN 'VIEW'\r\n                    ELSE NULL\r\n                END)";
		string filterStringForDependencies = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("su.name", "so.name", text2, text3);
		string filterStringForDependencies2 = synchronizeParameters.DatabaseRow.Filter.GetFilterStringForDependencies("su_dep.name", "so_dep.name", text3, text2);
		string text4 = FilterRulesCollection.CombineDependenciesFilter(filterStringForDependencies, filterStringForDependencies2);
		yield return "select\r\n                    case so.type\r\n                        when 'V' then 'VIEW'\r\n                        when 'P' then 'PROCEDURE'\r\n                        when 'XP' then 'PROCEDURE'\r\n                        when 'TR' then 'TRIGGER'\r\n                        when 'F' then 'FUNCTION'\r\n                        when 'SF' then 'FUNCTION'\r\n                    end as 'referencing_type',\r\n                    su.name as 'referencing_schema_name',\r\n                    '" + text + "' as 'referencing_server',\r\n                    db_name() as 'referencing_database_name',\r\n                    so.name as 'referencing_entity_name',\r\n                    '" + text + "' as 'referenced_server',\r\n                    db_name() as 'referenced_database_name',\r\n                    su_dep.name as 'referenced_schema_name',\r\n                    case so_dep.type\r\n                        when 'U' then 'TABLE'\r\n                        when 'V' then 'VIEW'\r\n                        when 'F' then 'FUNCTION'\r\n                        when 'SF' then 'FUNCTION'\r\n                        when 'P' then 'PROCEDURE'\r\n                        when 'XP' then 'PROCEDURE'\r\n                        when 'TR' then 'TRIGGER'\r\n                    end as 'referenced_type',\r\n                    so_dep.name as 'referenced_entity_name',\r\n                    null as is_caller_dependent,\r\n                    null as is_ambiguous,\r\n                    null as dependency_type\r\n                from sysdepends sd\r\n                inner join sysobjects so\r\n                    on sd.id = so.id\r\n                    and so.schemaid = 0\r\n                inner join sysusers su\r\n                    on so.uid = su.uid\r\n                inner join sysobjects so_dep\r\n                    on sd.depid = so_dep.id\r\n                inner join sysusers su_dep\r\n                    on so.uid = su_dep.uid\r\n                    " + text4;
	}
}
