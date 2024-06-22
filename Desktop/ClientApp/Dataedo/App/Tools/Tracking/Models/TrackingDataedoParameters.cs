using Dataedo.App.Tools.Tracking.Interfaces;

namespace Dataedo.App.Tools.Tracking.Models;

public class TrackingDataedoParameters : ITrackingDataedoParameters
{
	public string DataedoApp => StaticData.AppType;

	public string DataedoVersion => ProgramVersion.VersionWithBuild;
}
