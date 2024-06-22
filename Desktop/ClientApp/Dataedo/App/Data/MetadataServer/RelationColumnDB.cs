using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Data.Base.Commands.Results;
using Dataedo.Model.Data.Tables.Relations;

namespace Dataedo.App.Data.MetadataServer;

internal class RelationColumnDB : CommonDBSupport
{
	public RelationColumnDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public List<RelationColumnRow> GetColumnsWithUniqueConstraintsByModuleId(int moduleId)
	{
		return GroupRelationColumns(from RelationWithUniqueConstraintsObject column in commands.Select.Relations.GetRelationWithUniqueConstraintsByModule(moduleId)
			select new RelationColumnRow(column, loadUniqueConstraintData: true)).ToList();
	}

	public static IEnumerable<RelationColumnRow> GroupRelationColumns(IEnumerable<RelationColumnRow> relationColumns)
	{
		return (from x in (from x in relationColumns
				group x by new { x.Id, x.ColumnFkId }).Select(x =>
			{
				RelationColumnRow relationColumnRow2 = x.First();
				relationColumnRow2.PkUniqueConstraintsDataContainer.Data.AddRange(x.SelectMany((RelationColumnRow y) => y.PkUniqueConstraintsDataContainer.Data).ToList());
				return relationColumnRow2;
			})
			group x by new { x.Id, x.ColumnPkId }).Select(x =>
		{
			RelationColumnRow relationColumnRow = x.First();
			relationColumnRow.FkUniqueConstraintsDataContainer.Data.AddRange(x.SelectMany((RelationColumnRow y) => y.FkUniqueConstraintsDataContainer.Data).ToList());
			return relationColumnRow;
		});
	}

	public bool Synchronize(IEnumerable<RelationRow> relations, DatabaseRow database, bool isDbAdded, int updateId, out int[] succeedTablesIds, Form owner = null)
	{
		try
		{
			RelationColumnForSynchronization[] items = relations.SelectMany((RelationRow r) => from c in r.Columns.Distinct()
				select new RelationColumnForSynchronization
				{
					DatabaseId = database.IdValue,
					SucceedTablesIds = string.Empty,
					RelationName = r.Name,
					IsPkFromCurrentDatabase = database.IsCurrentDocumentation(r.PKTableDatabaseName),
					PkTableDatabaseName = r.PKTableDatabaseName,
					PkTableSchema = r.PKTableSchema,
					PkTableName = r.PKTableName,
					PkColumnName = c.ColumnPkName,
					IsFkFromCurrentDatabase = database.IsCurrentDocumentation(r.FKTableDatabaseName),
					FkTableDatabaseName = r.FKTableDatabaseName,
					FkTableSchema = r.FKTableSchema,
					FkTableName = r.FKTableName,
					FkColumnName = c.ColumnFkName,
					OrdinalPosition = c.OrdinalPosition,
					UpdateId = updateId,
					PkColumnPath = c.ColumnPkPath,
					FkColumnPath = c.ColumnFkPath
				}).ToArray();
			IEnumerable<IGrouping<int?, IdSource>> source = from x in (from r in commands.Synchronization.RelationsColumns.SynchronizeRelationColumns(database.Host, items, isDbAdded)
					where r.Result == 0
					select r).SelectMany((ValueWithDataArrayResult<int, IdSource> r) => r.Data.Where((IdSource x) => x.IsFromCurrentDatabase))
				group x by x.RelationGroupId;
			List<IdSource> list = source.Where((IGrouping<int?, IdSource> x) => !x.Key.HasValue).SelectMany((IGrouping<int?, IdSource> x) => x).ToList();
			list.AddRange((from x in source
				where x.Key.HasValue
				select x.First()).ToArray());
			succeedTablesIds = list.Select((IdSource x) => x.Id.Value).ToArray();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while synchronizing the relationships' columns:", owner);
			succeedTablesIds = new int[0];
			return false;
		}
		return true;
	}
}
