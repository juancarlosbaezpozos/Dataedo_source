using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Following.DTO;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Dataedo.Shared.Enums;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.DocumentationViewPermission;

public class FollowingObjectDocumentationViewAttribute : BasePermissionAttribute
{
	private FollowingObjectTypeEnum.FollowingObjectType objectType;

	public FollowingObjectDocumentationViewAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		idParam = "model";
	}

	protected override async Task<bool> HasPermission(int id)
	{
		switch (objectType)
		{
		case FollowingObjectTypeEnum.FollowingObjectType.Database:
			return await repository.DatabasesPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationView);
		case FollowingObjectTypeEnum.FollowingObjectType.Table:
		case FollowingObjectTypeEnum.FollowingObjectType.View:
		case FollowingObjectTypeEnum.FollowingObjectType.Structure:
			return await repository.TableViewStructurePermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationView);
		case FollowingObjectTypeEnum.FollowingObjectType.Procedure:
		case FollowingObjectTypeEnum.FollowingObjectType.Function:
			return await repository.ProcedureFunctionPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationView);
		case FollowingObjectTypeEnum.FollowingObjectType.Column:
			return await repository.ColumnsPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationView);
		case FollowingObjectTypeEnum.FollowingObjectType.Glossary:
			return await repository.GlossariesPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationView);
		case FollowingObjectTypeEnum.FollowingObjectType.Glossary_Entry:
			return await repository.GlossaryEntriesPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationView);
		case FollowingObjectTypeEnum.FollowingObjectType.Subject_Area:
			return await repository.SubjectAreaPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationView);
		default:
			return false;
		}
	}

	protected override int GetIdParameter(ActionExecutingContext context, ParameterDescriptor parameter)
	{
		FollowingActionDTO param = (FollowingActionDTO)context.ActionArguments[parameter.Name];
		objectType = param.FollowingObjectType;
		return param.ObjectId;
	}
}
