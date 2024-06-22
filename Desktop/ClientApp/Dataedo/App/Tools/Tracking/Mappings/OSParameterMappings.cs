using System.Collections.Specialized;
using Dataedo.App.Tools.Tracking.Models;

namespace Dataedo.App.Tools.Tracking.Mappings;

public class OSParameterMappings
{
	private const string osParameterName = "os";

	private const string osVersionParameterName = "os_version";

	private const string displaySizeParameterName = "display_size";

	private const string displayScalingParameterName = "display_scaling";

	private const string deviceNameParameterName = "device_name";

	private const string deviceIdParameterName = "device_id";

	public NameValueCollection MapParameters(NameValueCollection nameValueCollection, TrackingOSParameters trackingOSParameters)
	{
		nameValueCollection.Add("os", trackingOSParameters?.OS);
		nameValueCollection.Add("os_version", trackingOSParameters?.OSVersion);
		nameValueCollection.Add("display_size", trackingOSParameters?.DisplaySize);
		nameValueCollection.Add("display_scaling", trackingOSParameters?.DisplayScaling);
		nameValueCollection.Add("device_name", trackingOSParameters?.DeviceName);
		nameValueCollection.Add("device_id", trackingOSParameters?.DeviceId);
		return nameValueCollection;
	}
}
