using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.CommunityView;

public class ProcedureFunctionCommunityViewPermissionAttribute : BasePermissionAttribute
{
	public ProcedureFunctionCommunityViewPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.ProcedureFunctionPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.CommunityView);
	}
}