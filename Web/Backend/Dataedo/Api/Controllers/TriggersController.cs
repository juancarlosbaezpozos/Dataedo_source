using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.CommunityView;
using Dataedo.Api.Attributes.DocumentationEditPermission;
using Dataedo.Api.Attributes.DocumentationViewPermission;
using Dataedo.Api.Attributes.SchemaChangesView;
using Dataedo.Api.Controllers.Base;
using Dataedo.Api.Controllers.Interfaces;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Triggers.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Triggers.Interfaces;
using Dataedo.Repository.Services.Features.Dependencies;
using Dataedo.Repository.Services.Features.Dependencies.DTO;
using Dataedo.Repository.Services.Features.Feedback;
using Dataedo.Repository.Services.Features.Feedback.DTO;
using Dataedo.Repository.Services.Features.Feedback.Enums;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.DTO;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.Interfaces;
using Dataedo.Repository.Services.Interfaces.Base;
using Dataedo.Repository.Services.JoinDefinitions;
using Dataedo.Repository.Services.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

/// <summary>
/// The class providing actions for table objects.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class TriggersController : CustomFieldsController, IDependencies
{
	private readonly IObjectSchemaUpdatesService schemaUpdatesService;

	/// <summary>
	/// Gets object providing access to repository for data of objects.
	/// </summary>
	protected new ITriggersService Service => base.Service as ITriggersService;

	protected new IBaseWritableDatabaseObjectService WriteService => base.WriteService as IBaseWritableDatabaseObjectService;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.TriggersController" /> class for actions for trigger objects.
	/// </summary>
	/// <param name="repositoryAccessManager">The object providing getting object for accessing to repository.</param>
	public TriggersController(IRepositoryAccessManager repositoryAccessManager)
		: base(repositoryAccessManager.GetRepository().TriggersService, repositoryAccessManager.GetRepository().TriggersWriteService)
	{
		schemaUpdatesService = repositoryAccessManager.GetRepository().TriggerSchemaChangeTrackingService;
	}

	/// <summary>
	/// Gets the basic information about trigger.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>The basic information about object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(TriggerDTO), 200)]
	[HttpGet("{id}")]
	[ServiceFilter(typeof(TriggerPermissionViewAttribute))]
	public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [FromQuery] TriggersJoinsEnum.TriggersJoins[] join = null)
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
	[ServiceFilter(typeof(TriggerScriptViewPermissionAttribute))]
	public virtual async Task<IActionResult> GetScript([Range(0, int.MaxValue)] int id)
	{
		return Ok(await Service.GetScript(id));
	}

	/// <summary>
	/// Gets the list of top level of uses dependencies by specified server, database, schema, name and type.
	/// </summary>
	/// <param name="dependencyInformation">The information about referenced object.</param>
	/// <param name="rootDatabaseId">The ID of root database object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>A list of top level dependencies for the specified ID.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(DependencyObjectDTO), 200)]
	[HttpGet("dependencies/uses")]
	[ServiceFilter(typeof(TableViewStructureDependencyViewAttribute))]
	public virtual async Task<IActionResult> GetUsesDependencies([FromQuery] DependencyInformation dependencyInformation, [FromQuery][Required][Range(0, int.MaxValue)] int rootDatabaseId, [FromQuery] DependencyJoinsEnum.DependencyJoins[] join = null)
	{
		return Ok(await Service.GetUsesDependencies(dependencyInformation, rootDatabaseId));
	}

	/// <summary>
	/// Gets the list of top level of uses dependencies by specified server, database, schema, name and type.
	/// </summary>
	/// <param name="dependencyInformation">The information about referencing object.</param>
	/// <param name="rootDatabaseId">The ID of root database object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>A list of top level dependencies for the specified ID.</returns>
	/// <response code="200">Successful operation.</response>
	[ProducesResponseType(typeof(IEnumerable<DependencyObjectDTO>), 200)]
	[HttpGet("dependencies/used-by")]
	[ServiceFilter(typeof(TableViewStructureDependencyViewAttribute))]
	public virtual async Task<IActionResult> GetUsedByDependencies([FromQuery] DependencyInformation dependencyInformation, [FromQuery][Required][Range(0, int.MaxValue)] int rootDatabaseId, [FromQuery] DependencyJoinsEnum.DependencyJoins[] join = null)
	{
		return Ok(await Service.GetUsedByDependencies(dependencyInformation, rootDatabaseId));
	}

	/// <summary>
	/// Gets the metadata (creation, import and update information) information.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <returns>The metadata (creation, import and update information) information.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(IEnumerable<MetadataDTO>), 200)]
	[HttpGet("{id}/metadata")]
	[ServiceFilter(typeof(TriggerPermissionViewAttribute))]
	public virtual async Task<IActionResult> GetMetadata(int id)
	{
		await Task.FromException(new NotImplementedException());
		return Ok(null);
	}

	/// <summary>
	/// Gets the basic information about feedback.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <returns>The basic information about object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(FeedbackDTO), 200)]
	[HttpGet("feedback/{id}")]
	[ServiceFilter(typeof(TriggerCommunityViewPermissionAttribute))]
	public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [FromQuery] CollectionFeedbackParameters<FeedbackFiltersEnum.FeedbackFilters> parameters = null)
	{
		return Ok(await Service.GetFeedback(id, parameters));
	}

	[HttpGet("{id}/has-permission")]
	public async Task<IActionResult> HasPermission([Range(0, int.MaxValue)] int id, [FromQuery] RoleActionType.RoleAction roleAction)
	{
		return Ok(await Service.HasPermission(id, roleAction));
	}

	[ProducesResponseType(typeof(SchemaUpdateDTO), 200)]
	[ServiceFilter(typeof(TriggerSchemaChangesViewAttribute))]
	[HttpGet("{id}/schema-updates")]
	public virtual async Task<IActionResult> GetSchemaUpdates([Range(0, int.MaxValue)] int id, [FromQuery] OffsetLimitParameters parameters = null)
	{
		return Ok(await schemaUpdatesService.GetSchemaUpdates(id, parameters));
	}

	[HttpPost("{id}/put-description")]
	[ServiceFilter(typeof(TriggerEditPermissionAttribute))]
	public async Task<IActionResult> UpdateDescription([FromForm] string description, int id)
	{
		description = await WriteService.UpdateDescription(id, description);
		return Ok(description);
	}

	[ServiceFilter(typeof(TriggerEditPermissionAttribute))]
	public override Task<IActionResult> UpdateCustomField([FromForm] string fieldName, [FromForm] string value, int id)
	{
		return base.UpdateCustomField(fieldName, value, id);
	}
}
