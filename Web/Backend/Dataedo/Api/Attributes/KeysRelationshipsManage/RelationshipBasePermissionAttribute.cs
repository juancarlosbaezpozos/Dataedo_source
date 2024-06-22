using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.KeysRelationshipsManage;

public class RelationshipBasePermissionAttribute : BaseRelationshipPermissionAttribute
{
	public RelationshipBasePermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<(int, int)> GetIdParameters(ActionExecutingContext context, ParameterDescriptor parameter)
	{
		int relationshipId = (int)context.ActionArguments[parameter.Name];
		return await repository.AttributeRelationshipsService.GetDatabaseIdsByRelationship(relationshipId);
	}

	protected override Task<bool> HasPermission(int pkDatabaseId, int fkDatabaseId)
	{
		return repository.RelationshipsPermissionService.IsUserPermitted(pkDatabaseId, fkDatabaseId, RoleActionType.RoleAction.KeysRelationsManage);
	}
}
