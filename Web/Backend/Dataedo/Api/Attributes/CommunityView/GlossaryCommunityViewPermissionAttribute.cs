using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.CommunityView;

public class GlossaryCommunityViewPermissionAttribute : BasePermissionAttribute
{
	public GlossaryCommunityViewPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.GlossariesPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.CommunityView);
	}
}
