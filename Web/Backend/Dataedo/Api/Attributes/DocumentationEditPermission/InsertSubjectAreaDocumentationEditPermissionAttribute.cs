using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.DocumentationEditPermission;

public class InsertSubjectAreaDocumentationEditPermissionAttribute : BasePermissionAttribute
{
	public InsertSubjectAreaDocumentationEditPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		idParam = "databaseId";
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.SubjectAreaPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationEdit, rootIdProvided: true);
	}
}
