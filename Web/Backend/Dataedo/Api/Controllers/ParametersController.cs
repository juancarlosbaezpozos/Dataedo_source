using System.Threading.Tasks;
using Dataedo.Api.Attributes.DocumentationEditPermission;
using Dataedo.Api.Controllers.Base;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Interfaces.Base;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParametersController : CustomFieldsController
{
	public new IBaseWritableDatabaseObjectService WriteService => base.WriteService as IBaseWritableDatabaseObjectService;

	public ParametersController(IRepositoryAccessManager repositoryAccessManager)
		: base(repositoryAccessManager.GetRepository().ParametersService, repositoryAccessManager.GetRepository().ParametersWriteService)
	{
	}

	[HttpPost("{id}/put-description")]
	[ServiceFilter(typeof(ParametersPermissionEditAttribute))]
	public async Task<IActionResult> UpdateDescription([FromForm] string description, int id)
	{
		return Ok(await WriteService.UpdateDescription(id, description));
	}

	[ServiceFilter(typeof(ParametersPermissionEditAttribute))]
	public override Task<IActionResult> UpdateCustomField([FromForm] string fieldName, [FromForm] string value, int id)
	{
		return base.UpdateCustomField(fieldName, value, id);
	}
}
