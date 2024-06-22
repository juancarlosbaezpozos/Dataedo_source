using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Forms.Tools;

public static class UniqueConstraintManager
{
	public static void AddIcons(UniqueConstraintRow row, IEnumerable<ColumnRow> tableColumns)
	{
		foreach (ColumnRow updatedColumn in GetUpdatedColumns(tableColumns, row.Columns))
		{
			updatedColumn.UniqueConstraintsDataContainer.AddUniqueConstraint(row.Id, row.IsPK, row.Status, row.Disabled, row.ConstraintType, null);
			updatedColumn.UniqueConstraintsDataContainer.SortData(updatedColumn.UniqueConstraintsDataContainer.Data);
			DB.Community.InsertFollowingToRepository(SharedObjectTypeEnum.ObjectType.Column, updatedColumn.Id);
		}
	}

	public static void RemoveUnnecessaryIcons(int id, IEnumerable<ColumnRow> tableColumns, IEnumerable<UniqueConstraintColumnRow> columns)
	{
		foreach (ColumnRow updatedColumn in GetUpdatedColumns(tableColumns, columns))
		{
			updatedColumn.UniqueConstraintsDataContainer.RemoveUniqueConstraint(id);
		}
	}

	private static IEnumerable<ColumnRow> GetUpdatedColumns(IEnumerable<ColumnRow> tableColumns, IEnumerable<UniqueConstraintColumnRow> columns)
	{
		return tableColumns.Where((ColumnRow x) => columns.Any((UniqueConstraintColumnRow y) => y.ColumnId == x.Id));
	}
}
