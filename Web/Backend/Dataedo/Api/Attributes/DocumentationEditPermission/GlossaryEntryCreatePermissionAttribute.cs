using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.GlossaryEntries.DTO;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.DocumentationEditPermission;

public class GlossaryEntryCreatePermissionAttribute : BasePermissionAttribute
{
	public GlossaryEntryCreatePermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		idParam = "input";
	}

	protected override int GetIdParameter(ActionExecutingContext context, ParameterDescriptor parameter)
	{
		GlossaryEntryCreateDTO param = (GlossaryEntryCreateDTO)context.ActionArguments[parameter.Name];
		return param.GlossaryId;
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.GlossariesPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationEdit);
	}
}
