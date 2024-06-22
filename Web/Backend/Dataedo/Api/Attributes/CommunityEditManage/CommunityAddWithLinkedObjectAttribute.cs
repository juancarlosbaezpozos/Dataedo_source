using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Feedback.DTO;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Dataedo.Repository.Services.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.CommunityEditManage;

public class CommunityAddWithLinkedObjectAttribute : IAsyncActionFilter, IFilterMetadata
{
	private readonly IRepository repository;

	private const string paramName = "feedbackDTO";

	public CommunityAddWithLinkedObjectAttribute(IRepositoryAccessManager accessManager)
	{
		repository = accessManager.GetRepository();
	}

	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		FeedbackWithLinkedObjectCreateDTO feedbackDTO = (FeedbackWithLinkedObjectCreateDTO)context.ActionArguments["feedbackDTO"];
		if (!(await repository.FeedbackWritePermissionService.IsUserPermitted(feedbackDTO.ObjectId, feedbackDTO.ObjectClass, RoleActionType.RoleAction.CommunityEdit)))
		{
			context.Result = new ForbidResult();
		}
		else
		{
			await next();
		}
	}
}
