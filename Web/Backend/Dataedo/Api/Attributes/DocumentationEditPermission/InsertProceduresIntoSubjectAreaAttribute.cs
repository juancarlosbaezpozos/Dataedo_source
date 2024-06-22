using System.Collections.Generic;
using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;

namespace Dataedo.Api.Attributes.DocumentationEditPermission;

public class InsertProceduresIntoSubjectAreaAttribute : BaseInsertIntoSubjectAreaDocumentationEditPermissionAttribute
{
	public InsertProceduresIntoSubjectAreaAttribute(IRepositoryAccessManager accessManager)
		: base(accessManager)
	{
	}

	protected override async Task<bool> HasPermission(IEnumerable<int> databaseObjectIds)
	{
		return await repository.ProceduresFunctionsAccessService.IsPermittedToAllResources(RoleActionType.RoleAction.DocumentationEdit, databaseObjectIds);
	}
}
