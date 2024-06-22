using System.Runtime.Serialization;
using Dataedo.Repository.Services.Services;

namespace Dataedo.Api.Enums;

public class UserStatusEnum : BaseEnumConversions<UserStatusEnum.Status>
{
	public enum Status
	{
		[EnumMember(Value = "NoAccess")]
		NoAccess = 0
	}
}
