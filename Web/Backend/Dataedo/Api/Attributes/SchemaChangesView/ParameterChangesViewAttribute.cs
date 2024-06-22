using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.SchemaChangesView;

public class ParameterChangesViewAttribute : BasePermissionAttribute
{
	public ParameterChangesViewAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.ParameterChangesPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.SchemaChangesView);
	}
}