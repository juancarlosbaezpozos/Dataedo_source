using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Feedback.FeedbackComments.DTO;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Dataedo.Repository.Services.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.CommunityEditManage;

public class CommunityAddCommentPermissionAttribute : IAsyncActionFilter, IFilterMetadata
{
	private const string paramName = "input";

	private readonly IRepository repository;

	public CommunityAddCommentPermissionAttribute(IRepositoryAccessManager accessManager)
	{
		repository = accessManager.GetRepository();
	}

	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		FeedbackCommentCreateDTO dto = (FeedbackCommentCreateDTO)context.ActionArguments["input"];
		if (!(await HasPermission(dto.FeedbackID)))
		{
			context.Result = new ForbidResult();
		}
		else
		{
			await next();
		}
	}

	protected async Task<bool> HasPermission(int id)
	{
		return await repository.FeedbackWritePermissionService.IsUserPermitted(id, RoleActionType.RoleAction.CommunityEdit);
	}
}
