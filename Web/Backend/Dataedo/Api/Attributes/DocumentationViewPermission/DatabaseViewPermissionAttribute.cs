using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.DocumentationViewPermission;

public class DatabaseViewPermissionAttribute : BasePermissionAttribute
{
	public DatabaseViewPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.DatabasesPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationView);
	}
}
