namespace Dataedo.LicenseHelperLibrary.Enums;

public static class ValidationResultEnum
{
    public enum ValidationResult
    {
        None = 0,
        Valid = 1,
        NoKey = 2,
        IncorrectLicenseKey = 3,
        Lite = 4,
        VersionNotSupported = 5,
        TrialEnded = 6,
        SubscriptionExpired = 7,
        InsufficientLicenseLevel = 8
    }
}
