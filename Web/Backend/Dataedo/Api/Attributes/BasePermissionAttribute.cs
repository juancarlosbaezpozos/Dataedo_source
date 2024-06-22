using System.Linq;
using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes;

public abstract class BasePermissionAttribute : IAsyncActionFilter, IFilterMetadata
{
	protected readonly IRepositoryAccessManager repositoryAccessManager;

	protected readonly IRepository repository;

	protected string idParam = "id";

	public BasePermissionAttribute(IRepositoryAccessManager accessManager)
	{
		repositoryAccessManager = accessManager;
		repository = repositoryAccessManager.GetRepository();
	}

	public virtual async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		await OnActionExecutionAsync<int>(context, next);
	}

	protected virtual async Task OnActionExecutionAsync<T>(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		ParameterDescriptor parameter = context.ActionDescriptor.Parameters.FirstOrDefault((ParameterDescriptor x) => x.Name.Equals(idParam));
		if (parameter == null || !context.ActionArguments.ContainsKey(parameter?.Name))
		{
			context.Result = new BadRequestResult();
			return;
		}
		int id = GetIdParameter(context, parameter);
		if (!(await HasPermission(id)))
		{
			context.Result = new ForbidResult();
		}
		else
		{
			await next();
		}
	}

	protected virtual int GetIdParameter(ActionExecutingContext context, ParameterDescriptor parameter)
	{
		return (int)context.ActionArguments[parameter.Name];
	}

	protected abstract Task<bool> HasPermission(int id);
}
