using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.GlossaryEntriesRelationships.DTO;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.DocumentationEditPermission;

public class EntryRelationshipEditPermission : BaseRelationshipPermissionAttribute
{
	public EntryRelationshipEditPermission(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		paramName = "input";
	}

	protected override async Task<(int, int)> GetIdParameters(ActionExecutingContext context, ParameterDescriptor parameter)
	{
		EntryRelationshipCreateDTO model = (EntryRelationshipCreateDTO)context.ActionArguments[parameter.Name];
		return await repository.GlossaryEntriesRelationshipAttributeService.GetDatabaseIds(model.SourceId, model.DestinationId);
	}

	protected override Task<bool> HasPermission(int sourceEntryDatabaseID, int destinationEntryDatabaseID)
	{
		return repository.GlossaryEntriesRelationshipsPermissionService.IsUserPermitted(sourceEntryDatabaseID, destinationEntryDatabaseID, RoleActionType.RoleAction.DocumentationEdit);
	}
}
