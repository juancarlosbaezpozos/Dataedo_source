using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.CommunityEditManage;

public class CommunityManagePermissionAttribute : BasePermissionAttribute
{
	public CommunityManagePermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		idParam = "feedbackId";
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.FeedbackWritePermissionService.IsUserPermitted(id, RoleActionType.RoleAction.CommunityManage);
	}
}
