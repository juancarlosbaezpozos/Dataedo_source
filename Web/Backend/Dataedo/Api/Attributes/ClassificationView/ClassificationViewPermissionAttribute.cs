using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.Interfaces;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.ClassificationView;

public class ClassificationViewPermissionAttribute : BaseRepositoryPermissionAttribute
{
	private readonly IPermissionService permissionService;

	public ClassificationViewPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		permissionService = repository.DataClassificationPermissionService;
	}

	protected override async Task<bool> HasPermission()
	{
		return await permissionService.IsUserPermitted(RoleActionType.RoleAction.ClassificationView);
	}
}
