using System.Collections.Specialized;
using Dataedo.App.Tools.Tracking.Interfaces;

namespace Dataedo.App.Tools.Tracking.Mappings;

public class DataedoParameterMappings
{
	private const string dataedoAppParameterName = "dataedo_app";

	private const string dataedoVersionParameterName = "dataedo_version";

	public NameValueCollection MapParameters(NameValueCollection nameValueCollection, ITrackingDataedoParameters trackingDataedoParameters)
	{
		nameValueCollection.Add("dataedo_app", trackingDataedoParameters.DataedoApp);
		nameValueCollection.Add("dataedo_version", trackingDataedoParameters.DataedoVersion);
		return nameValueCollection;
	}
}
