using System;
using System.Collections.Generic;
using Dataedo.App.API.Enums;
using Dataedo.App.LoginFormTools.Tools.Licenses;
using Dataedo.LicenseFile;
using Dataedo.Shared.Licenses.Models;

namespace Dataedo.App.Helpers.Licenses;

public static class LicenseHelper
{
    public static FileData CreateLicenseForCmdImport()
    {
        var fileLicenseModel = new FileLicenseModel
        {
            Users = 100,
            Package = "DATA_CATALOG_CREATOR",
            PackageName = "Data Catalog Creator",
            Modules = new List<ModuleDataResult>
            {
                new()
                {
                    Module = "DESKTOP",
                    Name = "Dataedo Desktop",
                    Sort = 1,
                    Group = "UI",
                    GroupName = "Products",
                    GroupSort = 1
                },
                new()
                {
                    Module = "CONNECTOR_ALL",
                    Name = "All metadata connectors",
                    Sort = 2,
                    Group = "CONNECTOR",
                    GroupName = "Metadata Scanners",
                    GroupSort = 2
                },
                new()
                {
                    Module = "FUNCTIONALITIES_ALL",
                    Name = "All additional functionalities",
                    Sort = 3,
                    Group = "FUNCTIONALITIES",
                    GroupName = "Additional functionalities",
                    GroupSort = 3
                },
                new()
                {
                    Module = "CUSTOM_FIELDS",
                    Name = "Custom Fields",
                    Count = 100,
                    Sort = 4,
                    Group = "FUNCTIONALITIES",
                    GroupName = "Additional functionalities",
                    GroupSort = 3
                }
            }
        };
        var licenseFileModel = new LicenseFileModel
        {
            Type = LicenseTypeEnum.GetValue(LicenseTypeEnum.LicenseType.Subscription),
            AccountId = 0,
            AccountName = "Dataedo",
            Username = "CMD",
            CreatedOn = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds,
            Start = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds,
            End = (long)(DateTime.UtcNow.AddDays(1.0) - new DateTime(1970, 1, 1)).TotalSeconds,
            Licenses = new List<FileLicenseModel> { fileLicenseModel }
        };
        var fileData = new FileData();
        fileData.Map(licenseFileModel, fileLicenseModel);
        return fileData;
    }
}
