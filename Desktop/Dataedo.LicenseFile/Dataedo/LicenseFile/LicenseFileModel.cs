using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dataedo.LicenseFile;

public class LicenseFileModel
{
    [JsonProperty("type")]
    [JsonRequired]
    public string Type { get; set; }

    [JsonProperty("account_id")]
    [JsonRequired]
    public int AccountId { get; set; }

    [JsonProperty("account_name")]
    [JsonRequired]
    public string AccountName { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("university")]
    public string University { get; set; }

    [JsonProperty("document_date")]
    public long? DocumentDate { get; set; }

    [JsonProperty("document_id")]
    public int DocumentId { get; set; }

    [JsonProperty("created_on")]
    [JsonRequired]
    public long CreatedOn { get; set; }

    [JsonProperty("start")]
    [JsonRequired]
    public long? Start { get; set; }

    [JsonProperty("end")]
    [JsonRequired]
    public long? End { get; set; }

    [JsonProperty("licenses")]
    [JsonRequired]
    public List<FileLicenseModel> Licenses { get; set; }
}
