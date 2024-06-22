using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Relationships.DTO;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.KeysRelationshipsManage;

public class RelationshipCreatePermissionAttribute : RelationshipBasePermissionAttribute
{
	public RelationshipCreatePermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		paramName = "body";
	}

	protected override async Task<(int, int)> GetIdParameters(ActionExecutingContext context, ParameterDescriptor parameter)
	{
		RelationshipCreateDTO relationshipCreateDTO = (RelationshipCreateDTO)context.ActionArguments[parameter.Name];
		return await repository.AttributeRelationshipsService.GetDatabaseIdsByTables(relationshipCreateDTO);
	}
}
