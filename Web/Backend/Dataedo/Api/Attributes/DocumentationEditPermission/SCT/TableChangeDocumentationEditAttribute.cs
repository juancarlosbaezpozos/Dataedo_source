using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.DocumentationEditPermission.SCT;

public class TableChangeDocumentationEditAttribute : SCTDocumentationEditAttribute
{
	public TableChangeDocumentationEditAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.TableChangesPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationEdit);
	}
}
