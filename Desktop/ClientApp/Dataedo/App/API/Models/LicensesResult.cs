using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dataedo.App.API.Models;

internal class LicensesResult
{
    [JsonProperty("licenses")]
    public List<LicenseDataResult> Licenses { get; set; }
}
