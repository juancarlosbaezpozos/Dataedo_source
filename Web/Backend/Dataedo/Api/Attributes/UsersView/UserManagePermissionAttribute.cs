using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.Interfaces;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.UsersView;

public class UserManagePermissionAttribute : BaseRepositoryPermissionAttribute
{
	private IPermissionService repositoryPermissionService;

	public UserManagePermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		repositoryPermissionService = accessManager.GetRepository().RepositoryPermissionService;
	}

	protected override async Task<bool> HasPermission()
	{
		return await repositoryPermissionService.IsUserPermitted(RoleActionType.RoleAction.UsersManage);
	}
}
