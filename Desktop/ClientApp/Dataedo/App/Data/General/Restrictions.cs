using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.General;

public class Restrictions
{
	public SharedDatabaseTypeEnum.DatabaseType? DatabaseType { get; set; }

	public RestrictionType Type { get; set; }

	public string Restriction { get; set; }

	public bool IsEmpty => string.IsNullOrWhiteSpace(Restriction);

	public string WhereClauseWord => Type switch
	{
		RestrictionType.In => "IN", 
		RestrictionType.NotIn => "NOT IN", 
		_ => "IN", 
	};

	public string JoinWord => Type switch
	{
		RestrictionType.In => "OR", 
		RestrictionType.NotIn => "AND", 
		_ => "OR", 
	};
}
