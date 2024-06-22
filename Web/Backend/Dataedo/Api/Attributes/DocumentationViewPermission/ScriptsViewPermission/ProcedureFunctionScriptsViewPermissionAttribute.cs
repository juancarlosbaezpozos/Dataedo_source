using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.DocumentationViewPermission.ScriptsViewPermission;

public class ProcedureFunctionScriptsViewPermissionAttribute : BasePermissionAttribute
{
	public ProcedureFunctionScriptsViewPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		RoleActionType.RoleAction[] editRoles = new RoleActionType.RoleAction[2]
		{
			RoleActionType.RoleAction.DocumentationView,
			RoleActionType.RoleAction.ScriptsView
		};
		return await repository.ProcedureFunctionPermissionService.IsUserPermittedToAllActionRoles(id, editRoles);
	}
}
