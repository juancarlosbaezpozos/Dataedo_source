using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.SchemaChangesView;

public class SchemaUpdatesViewAttribute : BasePermissionAttribute
{
	public SchemaUpdatesViewAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.SchemaUpdatesPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.SchemaChangesView);
	}
}
