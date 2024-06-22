using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.DocumentationEditPermission;

public class ManageSubjectAreaDocumentationEditPermissionAttribute : BasePermissionAttribute
{
	public ManageSubjectAreaDocumentationEditPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		idParam = "id";
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.SubjectAreaPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationEdit);
	}
}
