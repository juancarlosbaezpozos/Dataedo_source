using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.DocumentationViewPermission;

public class ERDDocumentationViewPermissionAttribute : BasePermissionAttribute
{
	public ERDDocumentationViewPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.ERDPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationView);
	}
}
