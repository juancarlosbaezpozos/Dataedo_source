using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.DocumentationViewPermission.ScriptsViewPermission;

public class ViewScriptsViewPermissionAttribute : BasePermissionAttribute
{
	public ViewScriptsViewPermissionAttribute(IRepositoryAccessManager accessManager)
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
		return await repository.TableViewStructurePermissionService.IsUserPermittedToAllActionRoles(id, editRoles);
	}
}
