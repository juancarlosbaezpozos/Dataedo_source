using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.DocumentationViewPermission;
using Dataedo.Api.Controllers.Base;
using Dataedo.Api.Controllers.Interfaces;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Tables.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Tables.Interfaces;
using Dataedo.Repository.Services.JoinDefinitions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class StructuresController : TablesViewsStructuresController, IDependencies
{
	protected new IStructuresService Service => base.Service as IStructuresService;

	public StructuresController(IRepositoryAccessManager repositoryAccessManager)
		: base(repositoryAccessManager.GetRepository().StructuresService, repositoryAccessManager.GetRepository().StructuresWriteService, repositoryAccessManager)
	{
	}

	[ProducesResponseType(typeof(StructureDTO), 200)]
	[HttpGet("{id}")]
	[ServiceFilter(typeof(TableViewStructurePermissionViewAttribute))]
	public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [FromQuery] StructuresJoinsEnum.StructuresJoins[] join = null)
	{
		return Ok(await Service.GetObject(id, join));
	}

	[ProducesResponseType(typeof(ScriptDTO), 200)]
	[HttpGet("{id}/script")]
	[ServiceFilter(typeof(TableViewStructurePermissionViewAttribute))]
	public virtual async Task<IActionResult> GetScript([Range(0, int.MaxValue)] int id)
	{
		return Ok(await Service.GetScript(id));
	}
}
