using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Dataedo.Repository.Services.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.CommunityEditManage;

public class CommunityAddWithoutLinkedObjectAttribute : IAsyncActionFilter, IFilterMetadata
{
	private readonly IRepository repository;

	public CommunityAddWithoutLinkedObjectAttribute(IRepositoryAccessManager accessManager)
	{
		repository = accessManager.GetRepository();
	}

	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		if (!(await HasPermission()))
		{
			context.Result = new ForbidResult();
		}
		else
		{
			await next();
		}
	}

	protected async Task<bool> HasPermission()
	{
		return await repository.RepositoryPermissionService.IsUserPermitted(RoleActionType.RoleAction.CommunityEdit);
	}
}
