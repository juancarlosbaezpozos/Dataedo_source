using System;
using System.Collections.Generic;
using Dataedo.App.API.Enums;
using Dataedo.LicenseFile;
using Dataedo.Shared.Licenses.Models;

namespace Dataedo.App.LoginFormTools.Tools.Licenses;

public class FileLicenseData
{
    public List<ModuleDataResult> Modules { get; set; }

    public int Users { get; set; }

    public string Package { get; set; }

    public string PackageName { get; set; }

    public string Organization { get; set; }

    public string ExpiresOn { get; set; }

    public bool IsValid { get; set; }

    public bool IsSupported { get; set; }

    public FileLicenseModel FileLicenseModel { get; private set; }

    public LicenseTypeEnum.LicenseType LicenseType { get; set; }

    public string University { get; set; }

    public FileLicenseData(FileLicenseModel license, DateTime? endUtcDateTime, string accountName, int accountId, LicenseTypeEnum.LicenseType licenseType, string university)
    {
        FileLicenseModel = license;
        PackageName = license.PackageName;
        Users = license.Users;
        Modules = license.Modules;
        Package = license.Package;
        IsSupported = LicenseDesktopValidator.Validate(Modules);
        IsValid = endUtcDateTime.HasValue && endUtcDateTime > DateTime.UtcNow && IsSupported;
        Organization = LicenseService.GetOrganization(accountName, accountId, university, licenseType);
        ExpiresOn = LicenseService.GetExpiresOn(IsSupported, endUtcDateTime);
        LicenseType = licenseType;
        University = university;
    }
}
