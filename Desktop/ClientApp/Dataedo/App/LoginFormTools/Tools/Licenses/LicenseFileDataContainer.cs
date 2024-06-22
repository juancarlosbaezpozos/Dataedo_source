using System.Xml.Serialization;
using Dataedo.LicenseFile;

namespace Dataedo.App.LoginFormTools.Tools.Licenses;

public class LicenseFileDataContainer
{
    [XmlElement]
    public LicenseFileModel LicenseFile { get; set; }

    [XmlElement]
    public FileLicenseModel LastSelectedLicense { get; set; }
}
