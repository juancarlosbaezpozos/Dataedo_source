using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;
using Dataedo.App.Tools.Tracking.Models;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoConnectionBuilderEventSpecificTimeImportedObjectsImportedTables : ParametersWithUserDataedoConnectionBuilder
{
	private readonly string time;

	private readonly string importedObjects;

	private readonly string importedTables;

	public ParametersWithUserDataedoConnectionBuilderEventSpecificTimeImportedObjectsImportedTables(TrackingConnectionParameters trackingConnectionParameters, ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, string time, string importedObjects, string importedTables)
		: base(trackingConnectionParameters, trackingUserParameters, trackingDataedoParameters)
	{
		this.time = time;
		this.importedObjects = importedObjects;
		this.importedTables = importedTables;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetTime(nameValueCollection, time);
		SpecificParametersMapper.GetImportedObjects(nameValueCollection, importedObjects);
		SpecificParametersMapper.GetImportedTables(nameValueCollection, importedTables);
		return this;
	}
}
