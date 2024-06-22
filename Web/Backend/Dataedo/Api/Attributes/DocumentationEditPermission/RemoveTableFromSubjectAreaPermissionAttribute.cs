using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.DocumentationEditPermission;

public class RemoveTableFromSubjectAreaPermissionAttribute : BasePermissionAttribute
{
	public RemoveTableFromSubjectAreaPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		idParam = "tableId";
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.TableViewStructurePermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationEdit);
	}
}
