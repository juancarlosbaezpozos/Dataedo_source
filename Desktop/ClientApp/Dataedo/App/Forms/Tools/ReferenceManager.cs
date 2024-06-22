using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;

namespace Dataedo.App.Forms.Tools;

public static class ReferenceManager
{
	public static void AddReferences(IEnumerable<ColumnRow> tableColumns, RelationRow newRelation)
	{
		foreach (ColumnRow column in GetAddedReferenceColumns(tableColumns, newRelation.Columns))
		{
			RelationColumnRow relationColumnRow = newRelation.Columns.FirstOrDefault((RelationColumnRow x) => x.ColumnFkId == column.Id);
			column.ReferencesDataContainer.AddReference(newRelation, relationColumnRow.ColumnPkName, relationColumnRow.ColumnPkId);
		}
	}

	public static void RemoveReferences(RelationRow newRelation, IEnumerable<ColumnRow> tableColumns, IEnumerable<RelationColumnRow> columns)
	{
		foreach (ColumnRow column in GetAddedReferenceColumns(tableColumns, columns))
		{
			newRelation.Columns.FirstOrDefault((RelationColumnRow x) => x.ColumnFkId == column.Id);
			column.ReferencesDataContainer.RemoveReference(newRelation.Id);
		}
	}

	private static IEnumerable<ColumnRow> GetAddedReferenceColumns(IEnumerable<ColumnRow> tableColumns, IEnumerable<RelationColumnRow> columns)
	{
		return tableColumns.Where((ColumnRow x) => columns.Any((RelationColumnRow y) => y.ColumnFkId == x.Id));
	}
}
