using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.CommunityEditManage;

public class CommunityEditPermissionAttribute : BasePermissionAttribute
{
	public CommunityEditPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		idParam = "feedbackId";
	}

	protected override async Task<bool> HasPermission(int id)
	{
		RoleActionType.RoleAction[] editRoles = new RoleActionType.RoleAction[2]
		{
			RoleActionType.RoleAction.CommunityEdit,
			RoleActionType.RoleAction.CommunityManage
		};
		return await repository.FeedbackEditCommentPermissionService.IsUserPermitted(id, editRoles);
	}
}
