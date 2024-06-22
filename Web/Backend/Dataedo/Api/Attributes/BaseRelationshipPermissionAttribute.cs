using System.Linq;
using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes;

public abstract class BaseRelationshipPermissionAttribute : IAsyncActionFilter, IFilterMetadata
{
	protected readonly IRepository repository;

	protected string paramName = "id";

	public BaseRelationshipPermissionAttribute(IRepositoryAccessManager accessManager)
	{
		repository = accessManager.GetRepository();
	}

	public virtual async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		ParameterDescriptor parameter = context.ActionDescriptor.Parameters.FirstOrDefault((ParameterDescriptor x) => x.Name.Equals(paramName));
		if (parameter == null)
		{
			await next();
		}
		var (pkDatabaseId, fkDatabaseId) = await GetIdParameters(context, parameter);
		if (!(await HasPermission(pkDatabaseId, fkDatabaseId)))
		{
			context.Result = new ForbidResult();
		}
		else
		{
			await next();
		}
	}

	protected abstract Task<(int, int)> GetIdParameters(ActionExecutingContext context, ParameterDescriptor parameter);

	protected abstract Task<bool> HasPermission(int pkDatabaseId, int fkDatabaseId);
}
