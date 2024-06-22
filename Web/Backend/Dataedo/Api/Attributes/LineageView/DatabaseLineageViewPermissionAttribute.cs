using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.LineageView;

public class DatabaseLineageViewPermissionAttribute : BasePermissionAttribute
{
	public DatabaseLineageViewPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.DatabasesPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.LineageView);
	}
}
