using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.DocumentationViewPermission;

public class ColumnPermissionViewAttribute : BasePermissionAttribute
{
	public ColumnPermissionViewAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.ColumnsPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationView);
	}
}
