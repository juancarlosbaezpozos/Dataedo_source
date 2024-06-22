using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.SchemaChangesView;

public class ProcedureFunctionSchemaChangesViewAttribute : BasePermissionAttribute
{
	public ProcedureFunctionSchemaChangesViewAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.ProcedureFunctionPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.SchemaChangesView);
	}
}
