using System;

namespace Dataedo.App.API.Enums;

public static class LicenseTypeEnum
{
    public enum LicenseType
    {
        Unknown = 0,
        Trial = 1,
        Educational = 2,
        Subscription = 3
    }

    public static LicenseType GetValue(string value)
    {
        switch (value)
        {
            case "TRIAL":
                return LicenseType.Trial;
            case "EDU":
            case "EDUCATIONAL":
                return LicenseType.Educational;
            case "SUB_USER":
            case "SUB_FREE":
            case "SUB":
                return LicenseType.Subscription;
            default:
                return LicenseType.Unknown;
        }
    }

    public static string GetValue(LicenseType value)
    {
        return value switch
        {
            LicenseType.Trial => "TRIAL",
            LicenseType.Educational => "EDU",
            LicenseType.Subscription => "SUB",
            _ => "UNKNOWN",
        };
    }

    public static string GetDisplayName(LicenseType licenseType)
    {
        return licenseType switch
        {
            LicenseType.Unknown => "Unknown",
            LicenseType.Trial => "Trial",
            LicenseType.Educational => "Educational (noncommercial use only)",
            LicenseType.Subscription => "Subscription",
            _ => throw new ArgumentException($"Provided type ({licenseType}) is not supported."),
        };
    }
}
