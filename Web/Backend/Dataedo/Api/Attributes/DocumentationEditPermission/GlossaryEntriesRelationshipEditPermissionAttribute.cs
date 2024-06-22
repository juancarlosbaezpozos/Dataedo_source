using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.DocumentationEditPermission;

public class GlossaryEntriesRelationshipEditPermissionAttribute : BaseRelationshipPermissionAttribute
{
	public GlossaryEntriesRelationshipEditPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<(int, int)> GetIdParameters(ActionExecutingContext context, ParameterDescriptor parameter)
	{
		int relationshipId = (int)context.ActionArguments[parameter.Name];
		return await repository.GlossaryEntriesRelationshipAttributeService.GetDatabaseIdsByRelationship(relationshipId);
	}

	protected override Task<bool> HasPermission(int sourceDatabaseId, int destinationDatabaseId)
	{
		return repository.GlossaryEntriesRelationshipsPermissionService.IsUserPermitted(sourceDatabaseId, destinationDatabaseId, RoleActionType.RoleAction.DocumentationEdit);
	}
}
