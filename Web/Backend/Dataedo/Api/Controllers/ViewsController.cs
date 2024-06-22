using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.DocumentationViewPermission;
using Dataedo.Api.Attributes.DocumentationViewPermission.ScriptsViewPermission;
using Dataedo.Api.Controllers.Base;
using Dataedo.Api.Controllers.Interfaces;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Tables.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Tables.Interfaces;
using Dataedo.Repository.Services.Interfaces.Base;
using Dataedo.Repository.Services.JoinDefinitions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

/// <summary>
/// The class providing actions for view objects.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ViewsController : TablesViewsController, IDependencies
{
	/// <summary>
	/// Gets or sets the object providing access to repository for data of objects.
	/// </summary>
	protected new IViewsService Service => base.Service as IViewsService;

	protected new IWritableDatabaseObjectService WriteService => base.WriteService;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.ViewsController" /> class for actions for view objects.
	/// </summary>
	/// <param name="repositoryAccessManager">The object providing getting object for accessing to repository.</param>
	public ViewsController(IRepositoryAccessManager repositoryAccessManager)
		: base(repositoryAccessManager.GetRepository().ViewsService, repositoryAccessManager.GetRepository().ViewsWriteService, repositoryAccessManager)
	{
	}

	/// <summary>
	/// Gets the basic information about view.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>The basic information about object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(ViewDTO), 200)]
	[HttpGet("{id}")]
	[ServiceFilter(typeof(TableViewStructurePermissionViewAttribute))]
	public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [FromQuery] ViewsJoinsEnum.ViewsJoins[] join = null)
	{
		return Ok(await Service.GetObject(id, join));
	}

	/// <summary>
	/// Gets the script of procedure.
	/// </summary>
	/// <param name="id">The ID of procedure.</param>
	/// <returns>The script of procedure.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(ScriptDTO), 200)]
	[HttpGet("{id}/script")]
	[ServiceFilter(typeof(ViewScriptsViewPermissionAttribute))]
	public virtual async Task<IActionResult> GetScript([Range(0, int.MaxValue)] int id)
	{
		return Ok(await Service.GetScript(id));
	}
}
