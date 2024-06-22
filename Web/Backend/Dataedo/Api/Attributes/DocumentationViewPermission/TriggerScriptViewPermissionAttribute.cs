using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.DocumentationViewPermission;

public class TriggerScriptViewPermissionAttribute : BasePermissionAttribute
{
	public TriggerScriptViewPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.TriggersPermissionService.IsUserPermittedToAllActionRoles(id, RoleActionType.RoleAction.DocumentationView, RoleActionType.RoleAction.ScriptsView);
	}
}
