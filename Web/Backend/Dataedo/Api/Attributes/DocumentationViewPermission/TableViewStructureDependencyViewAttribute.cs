using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Dependencies;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dataedo.Api.Attributes.DocumentationViewPermission;

public class TableViewStructureDependencyViewAttribute : BasePermissionAttribute
{
	public TableViewStructureDependencyViewAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
		idParam = "dependencyInformation";
	}

	protected override async Task<bool> HasPermission(int id)
	{
		return await repository.TableViewStructurePermissionService.IsUserPermittedToAllActionRolesByDatabaseId(id, RoleActionType.RoleAction.DocumentationView, RoleActionType.RoleAction.DependenciesView, RoleActionType.RoleAction.SourceConnectionView);
	}

	protected override int GetIdParameter(ActionExecutingContext context, ParameterDescriptor parameter)
	{
		DependencyInformation dependencyInformation = (DependencyInformation)context.ActionArguments[parameter.Name];
		return dependencyInformation.DatabaseId;
	}
}
