using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;
using Dataedo.App.Tools.Tracking.Models;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoConnectionBuilderEventSpecificTimeImportedObjects : ParametersWithUserDataedoConnectionBuilder
{
	private readonly string time;

	private readonly string importedObjects;

	public ParametersWithUserDataedoConnectionBuilderEventSpecificTimeImportedObjects(TrackingConnectionParameters trackingConnectionParameters, ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, string time, string importedObjects)
		: base(trackingConnectionParameters, trackingUserParameters, trackingDataedoParameters)
	{
		this.time = time;
		this.importedObjects = importedObjects;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetTime(nameValueCollection, time);
		SpecificParametersMapper.GetImportedObjects(nameValueCollection, importedObjects);
		return this;
	}
}
