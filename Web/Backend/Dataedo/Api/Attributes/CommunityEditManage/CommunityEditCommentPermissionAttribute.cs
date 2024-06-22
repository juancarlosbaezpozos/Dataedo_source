using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.CommunityEditManage;

public class CommunityEditCommentPermissionAttribute : BasePermissionAttribute
{
	public CommunityEditCommentPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		idParam = "commentId";
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.FeedbackCommentWritePermissionService.IsUserPermitted(id, RoleActionType.RoleAction.CommunityEdit, RoleActionType.RoleAction.CommunityManage);
	}
}
