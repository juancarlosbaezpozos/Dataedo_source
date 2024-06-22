using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.Interfaces;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.UsersView;

public class GlossaryUserViewPermissionAttribute : BasePermissionAttribute
{
	private readonly IObjectPermissionService glossaryPermissionsService;

	public GlossaryUserViewPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		glossaryPermissionsService = accessManager.GetRepository().GlossariesPermissionService;
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await glossaryPermissionsService.IsUserPermitted(id, RoleActionType.RoleAction.UsersView);
	}
}
