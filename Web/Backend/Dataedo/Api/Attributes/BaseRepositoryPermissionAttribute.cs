using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes;

public abstract class BaseRepositoryPermissionAttribute : IAsyncActionFilter, IFilterMetadata
{
	protected readonly IRepositoryAccessManager repositoryAccessManager;

	protected readonly IRepository repository;

	public BaseRepositoryPermissionAttribute(IRepositoryAccessManager accessManager)
	{
		repositoryAccessManager = accessManager;
		repository = repositoryAccessManager.GetRepository();
	}

	public virtual async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
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

	protected abstract Task<bool> HasPermission();
}
