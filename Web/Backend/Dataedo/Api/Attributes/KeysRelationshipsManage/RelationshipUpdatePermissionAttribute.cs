using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Relationships.DTO;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.KeysRelationshipsManage;

public class RelationshipUpdatePermissionAttribute : RelationshipBasePermissionAttribute
{
	public RelationshipUpdatePermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		paramName = "body";
	}

	protected override async Task<(int, int)> GetIdParameters(ActionExecutingContext context, ParameterDescriptor parameter)
	{
		RelationshipUpdateDTO relationshipUpdateDTO = (RelationshipUpdateDTO)context.ActionArguments[parameter.Name];
		return await repository.AttributeRelationshipsService.GetDatabaseIdsByTables(relationshipUpdateDTO);
	}
}
