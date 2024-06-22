using System.Collections.Generic;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.General;
using Dataedo.App.Data.MySQL;
using Dataedo.App.UserControls.ImportFilter;

namespace Dataedo.App.Data.MariaDB;

internal class SynchronizeMariaDB : SynchronizeMySQL
{
	public SynchronizeMariaDB(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		DatabaseVersionUpdate versionUpdate = synchronizeParameters.DatabaseRow.GetVersionUpdate();
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Table, "i.table_schema", "i.table_name");
		if (versionUpdate.Version >= 10 || (versionUpdate.Version == 10 && versionUpdate.Update >= 3))
		{
			yield return "select i.table_name as `name`,\r\n                            '' as `schema`,\r\n                            i.table_schema as `database_name`,\r\n                            case i.table_type\r\n                                when 'SYSTEM VERSIONED' then 'SYSTEM_VERSIONED_TABLE'\r\n                                else 'TABLE'\r\n                                end as `type`,\r\n                            case i.table_comment\r\n                                when '' then null   -- replace empty comment with null\r\n                                else i.table_comment\r\n                                end as `description`,\r\n                            null as `definition`,\r\n                            i.create_time as `create_date`,\r\n                            case when i.update_time is null\r\n                                then i.create_time\r\n                                else i.update_time\r\n                                end as `modify_date`,\r\n                            null as `function_type`\r\n                        from information_schema.tables i\r\n                        where i.table_type in ('BASE TABLE','SYSTEM VERSIONED')\r\n                            and i.table_schema = binary '" + synchronizeParameters.DatabaseName + "'\r\n                            " + filterString;
		}
		else
		{
			yield return "select i.table_name as `name`,\r\n                            '' as `schema`,\r\n                            i.table_schema as `database_name`,\r\n                            'TABLE' as `type`,\r\n                            case i.table_comment\r\n                                when '' then null   -- replace empty comment with null\r\n                                else i.table_comment\r\n                                end as `description`,\r\n                            null as `definition`,\r\n                            i.create_time as `create_date`,\r\n                            case when i.update_time is null\r\n                                then i.create_time\r\n                                else i.update_time\r\n                                end as `modify_date`,\r\n                            null as `function_type`\r\n                        from information_schema.tables i\r\n                        where i.table_type = 'BASE TABLE'\r\n                            and i.table_schema = binary '" + synchronizeParameters.DatabaseName + "'\r\n                            " + filterString;
		}
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "r.routine_schema", "r.specific_name");
		yield return "select\r\n                        'procedure' as `object_type`,\r\n                        count(1) as `count`\r\n                    from information_schema.routines r\r\n                    where r.routine_type='PROCEDURE'\r\n                        " + filterString + "\r\n                        and r.routine_schema = '" + synchronizeParameters.DatabaseName + "'";
		yield return "select\r\n                        'Package' as `object_type`,\r\n                        count(1) as `count`\r\n                    from information_schema.routines r\r\n                    where r.routine_type='PACKAGE'\r\n                        and r.routine_schema = '" + synchronizeParameters.DatabaseName + "'";
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		string filterString = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Procedure, "i.routine_schema", "i.specific_name");
		string packageFilter = synchronizeParameters.DatabaseRow.Filter.GetFilterString("and", null, FilterObjectTypeEnum.FilterObjectType.Procedure, null, "p1.specific_name");
		yield return "select \r\n                              i.specific_name as `name`,\r\n                              '' as `schema`,\r\n                              i.routine_schema as `database_name`,\r\n                              'PROCEDURE' as `type`,\r\n                              case i.routine_comment\r\n                                   when '' then null   -- brak komentarza (opisu) pokazuje jako '', nie jako null, w sql serverze null przy braku komentarza (opisu)\r\n                                   else i.routine_comment   -- jesli null albo slowny komentarz\r\n                              end as `description`,\r\n                              i.routine_definition as `definition`,      -- null dla tabel, dla pozostalych skrypt\r\n                              i.created as `create_date`,\r\n                              case when i.last_altered is null then\r\n                                    i.created\r\n                                else\r\n                                    i.last_altered\r\n                                end as `modify_date`,\r\n                              null as `function_type`\r\n                              from information_schema.routines i\r\n                              where i.routine_type = 'PROCEDURE'\r\n                                and i.routine_schema = binary '" + synchronizeParameters.DatabaseName + "'\r\n                                " + filterString;
		yield return "SELECT\r\n                            p1.specific_name as 'name',\r\n                            '' as `schema`,\r\n                            p1.db as 'database_name',\r\n                            'PACKAGE' as `type`,\r\n                             case p1.comment\r\n                                  when '' then null  \r\n                                  else p1.comment   \r\n                                  end as 'description',\r\n                            cast(concat('CREATE PACKAGE ',p1.specific_name,' ', p1.body, char(13),'CREATE PACKAGE BODY ',p1.specific_name, ' ', p2.body) AS CHAR(65535) CHARACTER SET utf8)  as 'definition',\r\n                            p1.created as 'create_date',\r\n                            case when p1.modified is null then\r\n                                p1.created\r\n                            else\r\n                                p1.modified\r\n                            end as 'modify_date',\r\n                            '......' as 'function_type'\r\n                            FROM mysql.proc p1\r\n                            left join  mysql.proc p2 on p1.db = p2.db and p1.specific_name = p2.specific_name\r\n                            and p2.type in ('PACKAGE BODY')\r\n                            where\r\n                            p1.type in ('PACKAGE')\r\n\t\t\t\t\t\t\tand p1.db = binary '" + synchronizeParameters.DatabaseName + "'\r\n                            " + packageFilter;
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		yield return string.Empty;
	}
}
