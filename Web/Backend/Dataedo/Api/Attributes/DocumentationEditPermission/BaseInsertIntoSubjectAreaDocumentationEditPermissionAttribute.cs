using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.SubjectAreas.DTO;
using Dataedo.Repository.Services.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.DocumentationEditPermission;

public abstract class BaseInsertIntoSubjectAreaDocumentationEditPermissionAttribute : IAsyncActionFilter, IFilterMetadata
{
	protected readonly IRepository repository;

	protected string paramName = "model";

	public BaseInsertIntoSubjectAreaDocumentationEditPermissionAttribute(IRepositoryAccessManager accessManager)
	{
		repository = accessManager.GetRepository();
	}

	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		ParameterDescriptor parameter = context.ActionDescriptor.Parameters.FirstOrDefault((ParameterDescriptor x) => x.Name.Equals(paramName));
		if (parameter == null)
		{
			await next();
		}
		if (!(await HasPermission(await GetIdParameters(context, parameter))))
		{
			context.Result = new ForbidResult();
		}
		else
		{
			await next();
		}
	}

	protected async Task<IEnumerable<int>> GetIdParameters(ActionExecutingContext context, ParameterDescriptor parameter)
	{
		InsertObjectsIntoModuleDTO param = (InsertObjectsIntoModuleDTO)context.ActionArguments[parameter.Name];
		return param.ObjectIds?.ToList();
	}

	protected abstract Task<bool> HasPermission(IEnumerable<int> databaseObjectIds);
}
