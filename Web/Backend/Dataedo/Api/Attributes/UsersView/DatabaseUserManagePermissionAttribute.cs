using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.Interfaces;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.UsersView;

public class DatabaseUserManagePermissionAttribute : BasePermissionAttribute
{
	private readonly IObjectPermissionService databasesPermissionsService;

	private readonly IPermissionService repositoryPermissionService;

	public DatabaseUserManagePermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		databasesPermissionsService = accessManager.GetRepository().DatabasesPermissionService;
		repositoryPermissionService = accessManager.GetRepository().RepositoryPermissionService;
	}

	protected override async Task<bool> HasPermission(int id)
	{
		if (await repositoryPermissionService.IsUserPermitted(RoleActionType.RoleAction.UsersManage))
		{
			return true;
		}
		return await databasesPermissionsService.IsUserPermitted(id, RoleActionType.RoleAction.UsersManage);
	}
}
