using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.DocumentationEditPermission;

public class RemoveProcedureFromSubjectAreaPermissionAttribute : BasePermissionAttribute
{
	public RemoveProcedureFromSubjectAreaPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		idParam = "procedureId";
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.ProcedureFunctionPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationEdit);
	}
}
