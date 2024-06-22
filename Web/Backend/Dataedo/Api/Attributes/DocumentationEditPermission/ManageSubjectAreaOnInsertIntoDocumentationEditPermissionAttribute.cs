using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Dataedo.Repository.Services.Features.SubjectAreas.DTO;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.DocumentationEditPermission;

public class ManageSubjectAreaOnInsertIntoDocumentationEditPermissionAttribute : BasePermissionAttribute
{
	public ManageSubjectAreaOnInsertIntoDocumentationEditPermissionAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		idParam = "model";
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.SubjectAreaPermissionService.IsUserPermitted(id, RoleActionType.RoleAction.DocumentationEdit);
	}

	protected override int GetIdParameter(ActionExecutingContext context, ParameterDescriptor parameter)
	{
		InsertObjectsIntoModuleDTO param = (InsertObjectsIntoModuleDTO)context.ActionArguments[parameter.Name];
		return param.SubjectAreaId;
	}
}
