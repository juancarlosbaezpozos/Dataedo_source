using System;

namespace Dataedo.App.API.Enums;

public static class AccountRoleEnum
{
	public enum AccountRole
	{
		Unknown = 0,
		Owner = 1,
		Administrator = 2,
		Billing = 3
	}

	public static AccountRole GetValue(string value)
	{
		return value switch
		{
			"OWNER" => AccountRole.Owner, 
			"ADMIN" => AccountRole.Administrator, 
			"BILLING" => AccountRole.Billing, 
			_ => AccountRole.Unknown, 
		};
	}

	public static string GetDisplayName(AccountRole accountRole)
	{
		return accountRole switch
		{
			AccountRole.Unknown => "Unknown", 
			AccountRole.Owner => "Owner", 
			AccountRole.Administrator => "Admin", 
			AccountRole.Billing => "Billing", 
			_ => throw new ArgumentException($"Provided type ({accountRole}) is not supported."), 
		};
	}
}
