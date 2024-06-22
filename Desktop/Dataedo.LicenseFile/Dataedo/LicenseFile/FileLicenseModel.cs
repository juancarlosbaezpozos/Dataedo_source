using System.Collections.Generic;
using Dataedo.Shared.Licenses.Models;
using Newtonsoft.Json;

namespace Dataedo.LicenseFile;

public class FileLicenseModel
{
    [JsonProperty("modules")]
    public List<ModuleDataResult> Modules { get; set; }

    [JsonProperty("users")]
    [JsonRequired]
    public int Users { get; set; }

    [JsonProperty("package")]
    [JsonRequired]
    public string Package { get; set; }

    [JsonProperty("package_name")]
    [JsonRequired]
    public string PackageName { get; set; }
}
