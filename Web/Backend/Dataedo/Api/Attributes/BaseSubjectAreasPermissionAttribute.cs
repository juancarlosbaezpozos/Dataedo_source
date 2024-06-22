using System.Linq;
using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.SubjectAreas.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes;

public abstract class BaseSubjectAreasPermissionAttribute : BasePermissionAttribute
{
	public const string rootParam = "isRoot";

	private readonly ISubjectAreasService subjectAreaService;

	public BaseSubjectAreasPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		subjectAreaService = accessManager.GetRepository().SubjectAreasService;
	}

	public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		ParameterDescriptor objectIdParameter = context.ActionDescriptor.Parameters.FirstOrDefault((ParameterDescriptor x) => x.Name.Equals("subjectAreaId"));
		if (string.IsNullOrEmpty(objectIdParameter?.Name))
		{
			objectIdParameter = context.ActionDescriptor.Parameters.FirstOrDefault((ParameterDescriptor x) => x.Name.Equals(idParam));
		}
		ParameterDescriptor isRootParameter = context.ActionDescriptor.Parameters.FirstOrDefault((ParameterDescriptor x) => x.Name.Equals("isRoot"));
		if (objectIdParameter == null)
		{
			next();
		}
		int id = (int)context.ActionArguments[objectIdParameter.Name];
		bool isRoot = false;
		if (!string.IsNullOrEmpty(isRootParameter?.Name))
		{
			isRoot = (bool)context.ActionArguments[isRootParameter.Name];
		}
		if (!isRoot)
		{
			id = await subjectAreaService.GetSubjectAreaDatabaseId(id);
		}
		if (!(await HasPermission(id)))
		{
			context.Result = new ForbidResult();
		}
		else
		{
			await next();
		}
	}
}
