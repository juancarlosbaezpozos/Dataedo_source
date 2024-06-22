using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.DocumentationViewPermission;

public class TableViewStructurePermissionViewAttribute : BasePermissionAttribute
{
	public TableViewStructurePermissionViewAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.TableViewStructurePermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationView);
	}
}
