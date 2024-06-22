using System;
using Dataedo.App.API.Enums;

namespace Dataedo.App.LoginFormTools.Tools.Licenses;

public static class LicenseService
{
    public static string GetOrganization(string accountName, int accountId, string university, LicenseTypeEnum.LicenseType licenseType)
    {
        return licenseType switch
        {
            LicenseTypeEnum.LicenseType.Subscription => $"{accountName} [{accountId}]",
            LicenseTypeEnum.LicenseType.Educational => university,
            _ => string.Empty,
        };
    }

    public static string GetExpiresOn(bool isSupported, DateTime? endDate)
    {
        if (!isSupported)
        {
            return "Not supported";
        }
        return EndDateSuffixService.GetEndDateSuffix(endDate.Value);
    }
}
