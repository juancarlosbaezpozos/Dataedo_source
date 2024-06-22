using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.Profiling;

public class ProfilingAccesibilityAttribute : IAsyncActionFilter, IFilterMetadata
{
	private readonly IRepository repository;

	public ProfilingAccesibilityAttribute(IRepositoryAccessManager repositoryAccessManager)
	{
		repository = repositoryAccessManager.GetRepository();
	}

	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		if (!(await repository.ProfilingAccessibilityService.IsEnabled()))
		{
			context.Result = new NoContentResult();
		}
		else
		{
			await next();
		}
	}
}
