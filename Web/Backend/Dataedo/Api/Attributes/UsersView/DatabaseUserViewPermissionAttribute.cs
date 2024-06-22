using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.Interfaces;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.UsersView;

public class DatabaseUserViewPermissionAttribute : BasePermissionAttribute
{
	private readonly IObjectPermissionService databasesPermissionsService;

	public DatabaseUserViewPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		databasesPermissionsService = accessManager.GetRepository().DatabasesPermissionService;
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await databasesPermissionsService.IsUserPermitted(id, RoleActionType.RoleAction.UsersView);
	}
}
