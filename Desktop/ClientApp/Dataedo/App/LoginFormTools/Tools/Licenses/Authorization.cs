using System;
using System.Threading.Tasks;
using Dataedo.App.API.Models;
using Dataedo.App.API.Services;

namespace Dataedo.App.LoginFormTools.Tools.Licenses;

internal static class Authorization
{
	private static DateTime nextAuthorizationCheck;

	private static readonly TimeSpan authorizationCheckInterval = new TimeSpan(0, 0, 30);

	private static readonly TimeSpan authorizationRetryCheckInterval = new TimeSpan(0, 0, 15);

	public static async Task<bool> CheckAuthorization()
	{
		if (nextAuthorizationCheck < DateTime.Now)
		{
			try
			{
				Result obj = await new LoginService().CheckIfAuthorized(StaticData.Token);
				nextAuthorizationCheck += authorizationCheckInterval;
				return obj.IsNotUnauthorized;
			}
			catch (Exception)
			{
				nextAuthorizationCheck += authorizationRetryCheckInterval;
				return true;
			}
		}
		return false;
	}
}
