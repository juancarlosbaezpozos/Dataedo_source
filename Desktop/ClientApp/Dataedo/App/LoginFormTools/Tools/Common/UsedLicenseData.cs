using Newtonsoft.Json;

namespace Dataedo.App.LoginFormTools.Tools.Common;

internal class UsedLicenseData
{
    [JsonProperty("licenseId")]
    public string Id { get; set; }
}
