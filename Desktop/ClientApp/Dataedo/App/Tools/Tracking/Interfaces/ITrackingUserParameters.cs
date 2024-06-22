namespace Dataedo.App.Tools.Tracking.Interfaces;

public interface ITrackingUserParameters
{
	string Email { get; }

	int? AccountId { get; }

	string LicenseType { get; }
}
