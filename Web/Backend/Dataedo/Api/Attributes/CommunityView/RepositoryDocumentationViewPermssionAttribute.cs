using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.CommunityView;

public class RepositoryDocumentationViewPermssionAttribute : BaseRepositoryPermissionAttribute
{
	public RepositoryDocumentationViewPermssionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission()
	{
		return await repository.RepositoryPermissionService.IsUserPermitted(RoleActionType.RoleAction.DocumentationView);
	}
}
