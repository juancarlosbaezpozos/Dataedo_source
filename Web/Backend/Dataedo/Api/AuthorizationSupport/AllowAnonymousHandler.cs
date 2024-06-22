using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Dataedo.Api.AuthorizationSupport;

/// <summary>
/// The class providing allowing any action to be executed anonymously.
/// </summary>
public class AllowAnonymousHandler : IAuthorizationHandler
{
	/// <summary>
	/// Allows any authorization.
	/// </summary>
	/// <param name="context">The authorization information.</param>
	/// <returns>The task.</returns>
	public Task HandleAsync(AuthorizationHandlerContext context)
	{
		IAuthorizationRequirement[] pendingRequirements = context.PendingRequirements.ToArray();
		IAuthorizationRequirement[] array = pendingRequirements;
		foreach (IAuthorizationRequirement requirement in array)
		{
			context.Succeed(requirement);
		}
		return Task.CompletedTask;
	}
}
