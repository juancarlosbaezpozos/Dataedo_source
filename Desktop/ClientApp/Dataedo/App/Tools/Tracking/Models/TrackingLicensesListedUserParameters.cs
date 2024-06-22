using Dataedo.App.Tools.Tracking.Interfaces;

namespace Dataedo.App.Tools.Tracking.Models;

public class TrackingLicensesListedUserParameters : ITrackingUserParameters
{
    private readonly string email;

    public string Email => email;

    public int? AccountId => null;

    public string LicenseType => null;

    public TrackingLicensesListedUserParameters(string email)
    {
        this.email = email;
    }
}
