using Dataedo.App.LoginFormTools.Tools.Licenses;

namespace Dataedo.App.LoginFormTools.Tools.Common;

internal class LicenseFileDataModel
{
    public string Path { get; private set; }

    public FileData FileData { get; private set; }

    public LicenseFileDataModel(string path, FileData fileData)
    {
        Path = path;
        FileData = fileData;
    }
}
