using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.API.Enums;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Import.CloudStorage;
using Dataedo.App.Import.DataLake;
using Dataedo.App.LoginFormTools.Tools;
using Dataedo.LicenseFile;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using Dataedo.Shared.Licenses.Models;

namespace Dataedo.App.API.Models;

public class AppLicense
{
    public string Id { get; set; }

    public int? AccountId { get; set; }

    public string AccountName { get; set; }

    public string Type { get; set; }

    public List<ModuleDataResult> Modules { get; set; }

    public long? ValidTill { get; set; }

    public long? Start { get; set; }

    public long? End { get; set; }

    public DateTime? ValidTillUtcDateTime { get; set; }

    public DateTime? EndUtcDateTime => GetUtcDateTime(End);

    public string UserType { get; set; }

    public string PackageCode { get; set; }

    public string PackageName { get; set; }

    public List<AccountRoleResult> AccountRoles { get; set; }

    public string University { get; set; }

    public string Token { get; private set; }

    public bool IsValid
    {
        get
        {
            if (ValidTillUtcDateTime.HasValue)
            {
                return ValidTillUtcDateTime > DateTime.UtcNow;
            }
            return true;
        }
    }

    public LicenseTypeEnum.LicenseType LicenseTypeValue => LicenseTypeEnum.GetValue(Type);

    public bool IsFileLicense { get; set; }

    public LicenseUserTypeEnum.LicenseUserType LicenseUserTypeValue => LicenseUserTypeEnum.GetValue(UserType);

    public AppLicense(LicenseDataResult licenseDataResult)
    {
        Id = licenseDataResult.Id;
        AccountName = licenseDataResult.AccountName;
        Type = licenseDataResult.Type;
        Modules = licenseDataResult.Modules;
        ValidTill = licenseDataResult.ValidTill;
        ValidTillUtcDateTime = licenseDataResult.ValidTillUtcDateTime;
        Start = licenseDataResult.Start;
        End = licenseDataResult.End;
        AccountId = licenseDataResult.AccountId;
        AccountRoles = licenseDataResult.AccountRoles;
        PackageCode = licenseDataResult.Package;
        PackageName = licenseDataResult.PackageName;
        University = licenseDataResult.University;
        Token = licenseDataResult.Token;
        IsFileLicense = false;
    }

    public AppLicense(LicenseFileModel licenseFileModel, FileLicenseModel lastSelected)
    {
        AccountId = licenseFileModel.AccountId;
        AccountName = licenseFileModel.AccountName;
        Type = licenseFileModel.Type;
        Modules = lastSelected.Modules;
        Start = licenseFileModel.Start;
        End = licenseFileModel.End;
        PackageCode = lastSelected.Package;
        PackageName = lastSelected.PackageName;
        University = licenseFileModel.University;
        Id = null;
        ValidTill = null;
        ValidTillUtcDateTime = null;
        UserType = null;
        AccountRoles = null;
        Token = null;
        IsFileLicense = true;
    }

    public bool HasFunctionality(FunctionalityEnum.Functionality functionality)
    {
        List<ModuleDataResult> modules = Modules;
        if (modules == null || !modules.Any(x => x.FunctionalityValue == functionality))
        {
            return HasAllFunctionalities();
        }
        return true;
    }

    public bool HasAllFunctionalities()
    {
        return Modules?.Any((ModuleDataResult x) => x.FunctionalityValue == FunctionalityEnum.Functionality.FunctionalitiesAll) ?? false;
    }

    public int GetFunctionalityMaxCount(FunctionalityEnum.Functionality functionality)
    {
        return ((from x in Modules?.Where((ModuleDataResult x) => x.FunctionalityValue == functionality)
                 select x?.Count).Max()).GetValueOrDefault();
    }

    public string GetEndDateSuffix(DateTime utcNow)
    {
        if (!EndUtcDateTime.HasValue)
        {
            return null;
        }
        return EndDateSuffixService.GetEndDateSuffix(EndUtcDateTime.Value, utcNow);
    }

    private static DateTime? GetUtcDateTime(long? timestamp)
    {
        if (!timestamp.HasValue)
        {
            return null;
        }
        return DateTimeOffset.FromUnixTimeSeconds(timestamp.Value).UtcDateTime;
    }

    public bool HasAllConnectors()
    {
        return Modules?.Any((ModuleDataResult x) => x.IsConnectorItem && x.Module == "CONNECTOR_ALL") ?? false;
    }

    public bool HasNoConnectors()
    {
        return Modules?.Any((ModuleDataResult x) => x.IsConnectorItem && x.Module == "CONNECTOR_NONE") ?? false;
    }

    public bool HasDatabaseTypeConnector(SharedDatabaseTypeEnum.DatabaseType databaseType)
    {
        if (HasNoConnectors())
        {
            return false;
        }
        if (HasAllConnectors() || databaseType == SharedDatabaseTypeEnum.DatabaseType.Manual || DatabaseSupportFactory.IsTypeWithSubtypes(databaseType))
        {
            return true;
        }
        return Modules?.Any((ModuleDataResult x) => x.IsConnectorItem && x.Module == SharedDatabaseTypeEnum.GetConnectorLicenseString(databaseType)) ?? false;
    }

    public bool HasDataLakeTypeConnector(DataLakeTypeEnum.DataLakeType dataLakeType)
    {
        if (HasNoConnectors())
        {
            return false;
        }
        if (HasAllConnectors())
        {
            return true;
        }
        return Modules?.Any((ModuleDataResult x) => x.IsConnectorItem && x.Module == DataLakeTypeEnum.GetConnectorLicenseString(dataLakeType)) ?? false;
    }

    public bool HasCloudStorageTypeConnector(CloudStorageTypeEnum.CloudStorageType cloudStorageType)
    {
        if (HasNoConnectors())
        {
            return false;
        }
        if (HasAllConnectors())
        {
            return true;
        }
        return Modules?.Any((ModuleDataResult x) => x.IsConnectorItem && x.Module == CloudStorageTypeEnum.GetConnectorLicenseString(cloudStorageType)) ?? false;
    }
}
