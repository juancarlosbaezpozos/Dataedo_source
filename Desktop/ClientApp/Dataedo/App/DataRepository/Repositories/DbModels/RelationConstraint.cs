using Dataedo.App.DataRepository.Models;
using Dataedo.Model.Data.Tables.Relations;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class RelationConstraint : IRelationConstraint
{
	private DbRepository repository;

	public string ForeignColumnPath { get; private set; }

	public string ForeignColumnName { get; private set; }

	public string PrimaryColumnPath { get; private set; }

	public string PrimaryColumnName { get; private set; }

	public RelationConstraint(DbRepository repository, RelationWithUniqueConstraintsObject row)
	{
		this.repository = repository;
		ForeignColumnPath = row.ColumnFkPath;
		ForeignColumnName = row.ColumnFkName;
		PrimaryColumnPath = row.ColumnPkPath;
		PrimaryColumnName = row.ColumnPkName;
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
