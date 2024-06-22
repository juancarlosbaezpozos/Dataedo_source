using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Keys.DTO;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.KeysRelationshipsManage;

public class KeyCreatePermissionAttribute : BasePermissionAttribute
{
	public KeyCreatePermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		idParam = "model";
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.TableViewStructurePermissionService.IsUserPermitted(id, RoleActionType.RoleAction.KeysRelationsManage);
	}

	public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		await base.OnActionExecutionAsync<KeyCreateDTO>(context, next);
	}

	protected override int GetIdParameter(ActionExecutingContext context, ParameterDescriptor parameter)
	{
		KeyCreateDTO param = (KeyCreateDTO)context.ActionArguments[parameter.Name];
		return param.ParentDatabaseObjectId;
	}
}
