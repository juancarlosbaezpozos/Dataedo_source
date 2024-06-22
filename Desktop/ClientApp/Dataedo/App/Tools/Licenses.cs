using Dataedo.Data.Commands;
using Dataedo.LicenseHelperLibrary.Repository;

namespace Dataedo.App.Tools;

public class Licenses
{
    public static void Initialize(CommandsSetBase commands)
    {
    }

    public static bool CheckRepositoryVersionAfterLogin()
    {
        RepositoryVersionEnum.RepositoryVersion versionMatch;
        return StaticData.LicenseHelper.CheckRepositoryVersion(StaticData.DataedoConnectionString, ProgramVersion.Major, ProgramVersion.Minor, ProgramVersion.Build, StaticData.IsProjectFile, out versionMatch);
    }
}
