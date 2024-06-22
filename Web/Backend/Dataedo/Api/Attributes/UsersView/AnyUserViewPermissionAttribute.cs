using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.Interfaces;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.UsersView;

public class AnyUserViewPermissionAttribute : BaseRepositoryPermissionAttribute
{
	private readonly IObjectPermissionService permissionService;

	public AnyUserViewPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		permissionService = accessManager.GetRepository().DatabasesPermissionService;
	}

	protected override async Task<bool> HasPermission()
	{
		return await permissionService.IsUserPermittedToAnySource(RoleActionType.RoleAction.UsersView);
	}
}
