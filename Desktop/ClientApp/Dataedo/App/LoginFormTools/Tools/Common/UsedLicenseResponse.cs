using Newtonsoft.Json;

namespace Dataedo.App.LoginFormTools.Tools.Common;

internal class UsedLicenseResponse
{
    [JsonProperty("data")]
    public UsedLicenseData Data { get; set; }
}
