using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Configuration.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.Configuration;

public class SSOLoginMethodAttribute : LoginMethodAttribute
{
	public SSOLoginMethodAttribute(IRepositoryAccessManager repositoryAccessManager)
		: base(repositoryAccessManager)
	{
	}

	protected override async Task OnActionExecutionAsync(AuthenticationDTO authenticationMethods, ActionExecutingContext context, ActionExecutionDelegate next)
	{
		if (authenticationMethods.SSO.Value == true)
		{
			await next();
		}
		else
		{
			context.Result = new StatusCodeResult(405);
		}
	}
}
