using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Configuration.DTO;
using Dataedo.Repository.Services.Features.Configuration.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.Configuration;

public abstract class LoginMethodAttribute : IAsyncActionFilter, IFilterMetadata
{
	protected const int statusCode = 405;

	protected readonly ISecuritySettingsService service;

	public LoginMethodAttribute(IRepositoryAccessManager repositoryAccessManager)
	{
		service = repositoryAccessManager.GetRepository().SecuritySettingsService;
	}

	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		await OnActionExecutionAsync(await service.GetAuthenticationSettings(context.HttpContext.RequestAborted), context, next);
	}

	protected abstract Task OnActionExecutionAsync(AuthenticationDTO authenticationDTO, ActionExecutingContext context, ActionExecutionDelegate next);
}
