using Dataedo.App.API.Enums;
using Newtonsoft.Json;

namespace Dataedo.App.API.Models;

public class AccountRoleResult
{
	[JsonProperty("role_code")]
	public string RoleCode { get; set; }

	[JsonIgnore]
	public AccountRoleEnum.AccountRole RoleValue => AccountRoleEnum.GetValue(RoleCode);
}
