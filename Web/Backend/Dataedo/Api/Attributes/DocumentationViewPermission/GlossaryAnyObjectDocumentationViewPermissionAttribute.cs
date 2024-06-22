using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.DocumentationViewPermission;

public class GlossaryAnyObjectDocumentationViewPermissionAttribute : BaseRepositoryPermissionAttribute
{
	public GlossaryAnyObjectDocumentationViewPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override Task<bool> HasPermission()
	{
		return repository.GlossaryEntriesPermissionService.IsUserPermittedToAnySource(RoleActionType.RoleAction.DocumentationView);
	}
}
