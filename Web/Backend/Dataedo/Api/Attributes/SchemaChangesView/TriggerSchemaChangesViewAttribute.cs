using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.SchemaChangesView;

public class TriggerSchemaChangesViewAttribute : BasePermissionAttribute
{
	public TriggerSchemaChangesViewAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.TriggersPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.SchemaChangesView);
	}
}
