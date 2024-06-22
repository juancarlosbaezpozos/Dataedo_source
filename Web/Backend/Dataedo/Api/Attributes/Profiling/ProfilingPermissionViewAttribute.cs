using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.Profiling;

public class ProfilingPermissionViewAttribute : BasePermissionAttribute
{
	public ProfilingPermissionViewAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.ColumnsPermissionService.IsUserPermittedToAnyActionRole(id, RoleActionType.RoleAction.ProfilingViewData, RoleActionType.RoleAction.ProfilingViewDistribution);
	}
}
