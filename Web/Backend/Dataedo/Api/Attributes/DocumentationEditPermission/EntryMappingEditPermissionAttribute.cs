using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.GlossaryEntryMappings.DTO;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.DocumentationEditPermission;

public class EntryMappingEditPermissionAttribute : BaseRelationshipPermissionAttribute
{
	public EntryMappingEditPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		paramName = "input";
	}

	protected override async Task<(int, int)> GetIdParameters(ActionExecutingContext context, ParameterDescriptor parameter)
	{
		EntryMappingDTO model = (EntryMappingDTO)context.ActionArguments[parameter.Name];
		return await repository.EntryMappingAttibuteService.GetDatabaseIds(model.EntryId, model.ObjectId, model.ObjectType, model.ElementId);
	}

	protected override Task<bool> HasPermission(int sourceDatabaseId, int destinationDatabaseId)
	{
		return repository.GlossaryEntriesRelationshipsPermissionService.IsUserPermitted(sourceDatabaseId, destinationDatabaseId, RoleActionType.RoleAction.DocumentationEdit);
	}
}
