using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.DocumentationEditPermission;

public class ColumnEditPermissionAttribute : BasePermissionAttribute
{
	public ColumnEditPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.ColumnsPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationEdit);
	}
}
