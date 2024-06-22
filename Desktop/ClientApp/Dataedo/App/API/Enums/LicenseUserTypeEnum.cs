using System;

namespace Dataedo.App.API.Enums;

public static class LicenseUserTypeEnum
{
    public enum LicenseUserType
    {
        Unknown = 0,
        Editor = 1,
        Web = 2
    }

    public static LicenseUserType GetValue(string value)
    {
        if (!(value == "EDITOR"))
        {
            if (value == "WEB")
            {
                return LicenseUserType.Web;
            }
            return LicenseUserType.Unknown;
        }
        return LicenseUserType.Editor;
    }

    public static string GetDisplayName(LicenseUserType licenseUserType)
    {
        return licenseUserType switch
        {
            LicenseUserType.Unknown => "Unknown",
            LicenseUserType.Editor => "Editor",
            LicenseUserType.Web => "Viewer",
            _ => throw new ArgumentException($"Provided type ({licenseUserType}) is not supported."),
        };
    }
}
