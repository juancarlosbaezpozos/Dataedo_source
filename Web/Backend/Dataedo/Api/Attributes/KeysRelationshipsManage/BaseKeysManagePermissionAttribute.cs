using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.KeysRelationshipsManage;

public class BaseKeysManagePermissionAttribute : BasePermissionAttribute
{
	public BaseKeysManagePermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.KeysPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.KeysRelationsManage);
	}
}
