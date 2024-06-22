using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.DocumentationViewPermission;
using Dataedo.Api.Controllers.Interfaces;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Api.Services;
using Dataedo.Repository.Services.Features.DatabaseObjects.Tables.Interfaces;
using Dataedo.Repository.Services.Features.DatabaseObjects.Triggers;
using Dataedo.Repository.Services.Features.DatabaseObjects.Triggers.DTO;
using Dataedo.Repository.Services.Interfaces.Base;
using Dataedo.Repository.Services.JoinDefinitions;
using Dataedo.Repository.Services.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers.Base;

/// <summary>
/// The class providing actions for table and view objects.
/// </summary>
public abstract class TablesViewsController : TablesViewsStructuresController, IDependencies
{
	/// <summary>
	/// Gets the object providing access to repository for data of objects.
	/// </summary>
	protected new ITablesViewsService Service => base.Service as ITablesViewsService;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.Base.TablesViewsController" /> class providing base actions for tables and views.
	/// </summary>
	/// <param name="service">The service object providing actions for data of objects.</param>
	public TablesViewsController(ITablesViewsService service, IWritableDatabaseObjectService writeService, IRepositoryAccessManager repositoryAccessManager)
		: base(service, writeService, repositoryAccessManager)
	{
	}

	/// <summary>
	/// Gets the basic information about triggers of object.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <param name="parameters">The parameters providing selecting required set of data by skipping, limiting, filtering result items.</param>
	/// <returns>The list of basic information about triggers of object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(TriggerDTO), 200)]
	[HttpGet("{id}/triggers")]
	[ServiceFilter(typeof(TableViewStructurePermissionViewAttribute))]
	public virtual async Task<IActionResult> GetTriggers([Range(0, int.MaxValue)] int id, [FromQuery] TriggersJoinsEnum.TriggersJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<TriggerFiltersEnum.TriggerFilters> parameters = null)
	{
		return Ok(await Service.GetTriggers(id, join, parameters, JWTService.GetLogin(base.Request)));
	}
}
