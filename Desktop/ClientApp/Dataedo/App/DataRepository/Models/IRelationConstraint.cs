namespace Dataedo.App.DataRepository.Models;

public interface IRelationConstraint
{
	string ForeignColumnPath { get; }

	string ForeignColumnName { get; }

	string PrimaryColumnPath { get; }

	string PrimaryColumnName { get; }
}
