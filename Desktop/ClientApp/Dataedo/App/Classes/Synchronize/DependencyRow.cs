using Dataedo.App.Tools;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Data.Dependencies;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class DependencyRow : BaseRow
{
	public string ReferencingType { get; set; }

	public string ReferencingName { get; set; }

	public string ReferencingSchema { get; set; }

	public string ReferencingDatabase { get; set; }

	public string ReferencingServer { get; set; }

	public string ReferencedType { get; set; }

	public string ReferencedName { get; set; }

	public string ReferencedSchema { get; set; }

	public string ReferencedDatabase { get; set; }

	public string ReferencedServer { get; set; }

	public string IsCallerDependent { get; set; }

	public string IsAmbiguous { get; set; }

	public string DependencyType { get; set; }

	public SharedObjectTypeEnum.ObjectType? ReferencingObjectType { get; set; }

	public SharedObjectTypeEnum.ObjectType? ReferencedObjectType { get; set; }

	public int ReferencingId { get; set; }

	public int ReferencedId { get; set; }

	public int ReferencedDatabaseId { get; set; }

	public string JoinCondition { get; set; }

	public virtual string ReferencingObjectName => ReferencingSchema + "." + ReferencingName;

	public virtual string ReferencingObjectIdString => Paths.EncodeInvalidPathCharacters(ReferencingSchema + "_" + ReferencingName);

	public string ReferencingDisplayName => ReferencingObjectName;

	public virtual string ReferencedObjectName => ReferencedSchema + "." + ReferencedName;

	public virtual string ReferencedObjectIdString => Paths.EncodeInvalidPathCharacters(ReferencedSchema + "_" + ReferencedName);

	public string ReferencedDisplayName => ReferencedName;

	public DependencyRow(DependencySynchronizationObject dataReader)
	{
		ReferencingType = dataReader.ReferencingType;
		ReferencingName = dataReader.ReferencingEntityName;
		ReferencingSchema = dataReader.ReferencingSchemaName;
		ReferencingDatabase = dataReader.ReferencingDatabaseName;
		ReferencingServer = dataReader.ReferencingServer;
		ReferencedType = dataReader.ReferencedType;
		ReferencedName = dataReader.ReferencedEntityName;
		ReferencedSchema = dataReader.ReferencedSchemaName;
		ReferencedDatabase = dataReader.ReferencedDatabaseName;
		ReferencedServer = dataReader.ReferencedServer;
		IsCallerDependent = dataReader.IsCallerDependent;
		IsAmbiguous = dataReader.IsAmbiguous;
		DependencyType = dataReader.DependencyType;
	}
}
