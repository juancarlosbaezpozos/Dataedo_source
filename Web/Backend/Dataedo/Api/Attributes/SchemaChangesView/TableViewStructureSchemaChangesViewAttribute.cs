using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.SchemaChangesView;

public class TableViewStructureSchemaChangesViewAttribute : BasePermissionAttribute
{
	public TableViewStructureSchemaChangesViewAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.TableViewStructurePermissionService.IsUserPermitted(id, RoleActionType.RoleAction.SchemaChangesView);
	}
}
