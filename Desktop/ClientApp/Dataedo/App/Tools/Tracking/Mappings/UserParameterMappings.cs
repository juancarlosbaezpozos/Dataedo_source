using System.Collections.Specialized;
using Dataedo.App.Tools.Tracking.Interfaces;

namespace Dataedo.App.Tools.Tracking.Mappings;

public class UserParameterMappings
{
	private const string emailParameterName = "email";

	private const string accountIdParameterName = "account_id";

	private const string licenseTypeParameterName = "license_type";

	public NameValueCollection MapParameters(NameValueCollection nameValueCollection, ITrackingUserParameters trackingUserParameters)
	{
		nameValueCollection.Add("email", trackingUserParameters?.Email);
		nameValueCollection.Add("account_id", trackingUserParameters?.AccountId?.ToString());
		nameValueCollection.Add("license_type", trackingUserParameters?.LicenseType);
		return nameValueCollection;
	}
}
