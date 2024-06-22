using Dataedo.App.API.Models;
using Dataedo.App.Tools;
using Dataedo.Data.Commands.Enums;
using Dataedo.LicenseFile;

namespace Dataedo.App.LoginFormTools.Tools.Licenses;

public static class LicenseFileDataHelper
{
    public static void Save(LicenseFileModel licenseFileModel, FileLicenseModel lastSelected)
    {
        LastConnectionInfo.LOGIN_INFO.LicenseFileData = XmlFileSimpleAesService.EncryptData(new LicenseFileDataContainer
        {
            LicenseFile = licenseFileModel,
            LastSelectedLicense = lastSelected
        });
        LastConnectionInfo.LOGIN_INFO.SessionData = null;
        StaticData.License = new AppLicense(licenseFileModel, lastSelected);
        StaticData.LicenseEnum = LicenseEnum.LocalFile;
        StaticData.Profile = null;
        LastConnectionInfo.Save();
    }

    public static LicenseFileDataContainer GetLicenseFileData()
    {
        return XmlFileSimpleAesService.DecryptData<LicenseFileDataContainer>(LastConnectionInfo.LOGIN_INFO.LicenseFileData);
    }
}
