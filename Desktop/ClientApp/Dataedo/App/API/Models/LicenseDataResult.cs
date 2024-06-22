using System;
using System.Collections.Generic;
using Dataedo.App.API.Enums;
using Dataedo.App.LoginFormTools.Tools;
using Dataedo.App.LoginFormTools.Tools.Licenses;
using Dataedo.Shared.Licenses.Models;
using Newtonsoft.Json;

namespace Dataedo.App.API.Models;

public class LicenseDataResult
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("account_id")]
    public int? AccountId { get; set; }

    [JsonProperty("account_name")]
    public string AccountName { get; set; }

    [JsonProperty("university")]
    public string University { get; set; }

    [JsonProperty("start")]
    public long? Start { get; set; }

    [JsonProperty("end")]
    public long? End { get; set; }

    [JsonProperty("valid_till")]
    public long? ValidTill { get; set; }

    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("package")]
    public string Package { get; set; }

    [JsonProperty("package_name")]
    public string PackageName { get; set; }

    [JsonProperty("modules")]
    public List<ModuleDataResult> Modules { get; set; }

    [JsonProperty("account_roles")]
    public List<AccountRoleResult> AccountRoles { get; set; }

    [JsonIgnore]
    public LicenseTypeEnum.LicenseType LicenseTypeValue => LicenseTypeEnum.GetValue(Type);

    [JsonIgnore]
    public DateTime? StartUtcDateTime => GetUtcDateTime(Start);

    [JsonIgnore]
    public DateTime? EndUtcDateTime => GetUtcDateTime(End);

    [JsonIgnore]
    public DateTime? ValidTillUtcDateTime => GetUtcDateTime(ValidTill);

    [JsonIgnore]
    public bool IsEnded
    {
        get
        {
            if (EndUtcDateTime.HasValue)
            {
                return EndUtcDateTime < DateTime.UtcNow;
            }
            return true;
        }
    }

    [JsonIgnore]
    public bool IsSupported => LicenseDesktopValidator.Validate(Modules);

    [JsonIgnore]
    public bool IsValid
    {
        get
        {
            if (IsSupported)
            {
                if (ValidTillUtcDateTime.HasValue)
                {
                    return ValidTillUtcDateTime > DateTime.UtcNow;
                }
                return true;
            }
            return false;
        }
    }

    [JsonIgnore]
    public string ExpiresOn => LicenseService.GetExpiresOn(IsSupported, EndUtcDateTime);

    [JsonIgnore]
    public string Organization => LicenseService.GetOrganization(AccountName, AccountId.GetValueOrDefault(), University, LicenseTypeValue);

    private static DateTime? GetUtcDateTime(long? timestamp)
    {
        if (!timestamp.HasValue)
        {
            return null;
        }
        return DateTimeOffset.FromUnixTimeSeconds(timestamp.Value).UtcDateTime;
    }
}
