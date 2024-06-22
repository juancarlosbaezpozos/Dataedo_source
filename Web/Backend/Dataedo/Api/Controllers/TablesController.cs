using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.DocumentationViewPermission;
using Dataedo.Api.Controllers.Base;
using Dataedo.Api.Controllers.Interfaces;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.DatabaseObjects.Tables.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Tables.Interfaces;
using Dataedo.Repository.Services.JoinDefinitions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

/// <summary>
/// The class providing actions for table objects.# http
/// </summary>
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class TablesController : TablesViewsController, IDependencies
{
	protected new ITablesWriteService WriteService => base.WriteService as ITablesWriteService;

	/// <summary>
	/// Gets or sets the object providing access to repository for data of objects.
	/// </summary>
	protected new ITablesService Service => base.Service as ITablesService;

	[HttpPost("{id}/put-name")]
	public async Task<IActionResult> UpdateName([FromForm] string name, int id)
	{
		name = await WriteService.UpdateName(id, name);
		return Ok(name);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.TablesController" /> class for actions for tables objects.
	/// </summary>
	/// <param name="repositoryAccessManager">The object providing getting object for accessing to repository.</param>
	public TablesController(IRepositoryAccessManager repositoryAccessManager)
		: base(repositoryAccessManager.GetRepository().TablesService, repositoryAccessManager.GetRepository().TablesWriteService, repositoryAccessManager)
	{
	}

	/// <summary>
	/// Gets the basic information about table.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>The basic information about object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(TableDTO), 200)]
	[HttpGet("{id}")]
	[ServiceFilter(typeof(TableViewStructurePermissionViewAttribute))]
	public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [FromQuery] TablesJoinsEnum.TablesJoins[] join = null)
	{
		return Ok(await Service.GetObject(id, join));
	}
}
