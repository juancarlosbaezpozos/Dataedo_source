using Dataedo.App.Tools.Tracking.Interfaces;

namespace Dataedo.App.Tools.Tracking.Models;

public class TrackingUserParameters : ITrackingUserParameters
{
	public string Email => StaticData.Profile?.Email;

	public int? AccountId => StaticData.License?.AccountId;

	public string LicenseType => StaticData.License?.Type;
}
