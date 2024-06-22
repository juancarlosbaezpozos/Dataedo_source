using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.CommunityView;
using Dataedo.Api.Attributes.DocumentationEditPermission;
using Dataedo.Api.Attributes.DocumentationViewPermission;
using Dataedo.Api.Attributes.LineageView;
using Dataedo.Api.Attributes.SchemaChangesView;
using Dataedo.Api.Attributes.UsersView;
using Dataedo.Api.Controllers.Base;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Enums;
using Dataedo.Repository.Services.Features.DatabaseObjects.Procedures;
using Dataedo.Repository.Services.Features.DatabaseObjects.Procedures.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Tables;
using Dataedo.Repository.Services.Features.DatabaseObjects.Tables.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Triggers;
using Dataedo.Repository.Services.Features.DatabaseObjects.Triggers.DTO;
using Dataedo.Repository.Services.Features.Databases;
using Dataedo.Repository.Services.Features.Databases.DTO;
using Dataedo.Repository.Services.Features.Databases.Interfaces;
using Dataedo.Repository.Services.Features.DataLineage.ObjectLevel.DTO;
using Dataedo.Repository.Services.Features.Feedback;
using Dataedo.Repository.Services.Features.Feedback.DTO;
using Dataedo.Repository.Services.Features.Feedback.Enums;
using Dataedo.Repository.Services.Features.Permissions.DTO;
using Dataedo.Repository.Services.Features.Permissions.Interfaces;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Dataedo.Repository.Services.Features.Relationships;
using Dataedo.Repository.Services.Features.Relationships.DTO;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.DTO;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.Interfaces;
using Dataedo.Repository.Services.JoinDefinitions;
using Dataedo.Repository.Services.JoinDefinitions.Base;
using Dataedo.Repository.Services.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

/// <summary>
/// The class providing actions for database objects.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class DatabasesController : CustomFieldsController
{
	protected readonly IDatabaseSchemaUpdatesService schemaUpdatesService;

	private readonly IPermissionsReadService permissionsService;

	private readonly IPermissionsWriteService permissionsWriteService;

	/// <summary>
	/// Gets object providing access to repository for data of objects.
	/// </summary>
	protected new IDatabasesService Service => base.Service as IDatabasesService;

	protected new IDatabasesWriteService WriteService => base.WriteService as IDatabasesWriteService;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.DatabasesController" /> class for actions for databases objects.
	/// </summary>
	/// <param name="repositoryAccessManager">The object providing getting object for accessing to repository.</param>
	public DatabasesController(IRepositoryAccessManager repositoryAccessManager)
		: base(repositoryAccessManager.GetRepository().DatabasesService, repositoryAccessManager.GetRepository().DatabasesWriteService)
	{
		schemaUpdatesService = repositoryAccessManager.GetRepository().DatabaseSchemaChangeTrackingService;
		permissionsService = repositoryAccessManager.GetRepository().PermissionService;
		permissionsWriteService = repositoryAccessManager.GetRepository().PermissionsWriteService;
	}

	/// <summary>
	/// Gets the list of basic information about databases.
	/// </summary>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <param name="parameters">The parameters providing selecting required set of data by skipping and limiting result items.</param>
	/// <returns>The basic information about object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(DatabaseDTO), 200)]
	[HttpGet]
	public virtual async Task<IActionResult> Get([FromQuery] DatabasesJoinsEnum.DatabasesJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<DatabaseFiltersEnum.DatabaseFilters> parameters = null)
	{
		return Ok(await Service.GetObjectsAsync(join, parameters));
	}

	[ProducesResponseType(typeof(DatabaseDTO), 200)]
	[ServiceFilter(typeof(UserViewPermissionAttribute))]
	[HttpGet("all")]
	public virtual async Task<IActionResult> Get()
	{
		return Ok(await Service.GetAllObjectsAsync());
	}

	[ProducesResponseType(typeof(DatabaseDTO), 200)]
	[HttpGet("all-permitted")]
	public virtual async Task<IActionResult> GetAllPermitted([FromQuery] OrderByEnum orderByCreationDate = OrderByEnum.Default, [FromQuery] OffsetLimitParameters parameters = null)
	{
		return Ok(await Service.GetAllPermittedObjectsAsync(orderByCreationDate, parameters));
	}

	/// <summary>
	/// Gets the basic information about database.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>The basic information about object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ServiceFilter(typeof(DatabaseViewPermissionAttribute))]
	[ProducesResponseType(typeof(DatabaseDTO), 200)]
	[HttpGet("{id}")]
	public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [FromQuery] DatabasesJoinsEnum.DatabasesJoins[] join = null)
	{
		return Ok(await Service.GetObject(id, join));
	}

	/// <summary>
	/// Gets the list of basic information about tables in database.
	/// </summary>
	/// <param name="id">The ID of database.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <param name="parameters">The parameters providing selecting required set of data by skipping and limiting result items.</param>
	/// <returns>The list of basic information about tables in database.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(TableDTO), 200)]
	[ServiceFilter(typeof(DatabaseViewPermissionAttribute))]
	[HttpGet("{id}/tables")]
	public virtual async Task<IActionResult> GetTables([Range(0, int.MaxValue)] int id, [FromQuery] TablesJoinsEnum.TablesJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<TableFiltersEnum.TableFilters> parameters = null)
	{
		return Ok(await Service.GetTables(id, join, parameters));
	}

	[ProducesResponseType(typeof(SchemaUpdateDTO), 200)]
	[ServiceFilter(typeof(DatabaseSchemaChangesViewAttribute))]
	[HttpGet("{id}/schema-updates")]
	public virtual async Task<IActionResult> GetSchemaUpdates([Range(0, int.MaxValue)] int id, [FromQuery] bool withChangesOnly, [FromQuery] OffsetLimitParameters parameters = null)
	{
		return Ok(await schemaUpdatesService.GetSchemaUpdates(id, withChangesOnly, parameters));
	}

	[HttpGet("{id}/has-permission")]
	public async Task<IActionResult> HasPermission([Range(0, int.MaxValue)] int id, [FromQuery] RoleActionType.RoleAction roleAction)
	{
		return Ok(await Service.HasPermission(id, roleAction));
	}

	/// <summary>
	/// Gets the list of basic information about views in database.
	/// </summary>
	/// <param name="id">The ID of database.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <param name="parameters">The parameters providing selecting required set of data by skipping and limiting result items.</param>
	/// <returns>The list of basic information about views in database.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(ViewDTO), 200)]
	[ServiceFilter(typeof(DatabaseViewPermissionAttribute))]
	[HttpGet("{id}/views")]
	public virtual async Task<IActionResult> GetViews([Range(0, int.MaxValue)] int id, [FromQuery] ViewsJoinsEnum.ViewsJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<TableFiltersEnum.TableFilters> parameters = null)
	{
		return Ok(await Service.GetViews(id, join, parameters));
	}

	/// <summary>
	/// Gets the list of basic information about procedures in database.
	/// </summary>
	/// <param name="id">The ID of database.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <param name="parameters">The parameters providing selecting required set of data by skipping and limiting result items.</param>
	/// <returns>The list of basic information about procedures in database.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(ProcedureDTO), 200)]
	[ServiceFilter(typeof(DatabaseViewPermissionAttribute))]
	[HttpGet("{id}/procedures")]
	public virtual async Task<IActionResult> GetProcedures([Range(0, int.MaxValue)] int id, [FromQuery] ProceduresFunctionsJoinsEnum.ProceduresFunctionsJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<ProcedureFiltersEnum.ProcedureFilters> parameters = null)
	{
		return Ok(await Service.GetProcedures(id, join, parameters));
	}

	/// <summary>
	/// Gets the list of basic information about functions in database.
	///
	/// </summary>
	/// <param name="id">The ID of database.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <param name="parameters">The parameters providing selecting required set of data by skipping and limiting result items.</param>
	/// <returns>The list of basic information about functions in database.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(FunctionDTO), 200)]
	[ServiceFilter(typeof(DatabaseViewPermissionAttribute))]
	[HttpGet("{id}/functions")]
	public virtual async Task<IActionResult> GetFunctions([Range(0, int.MaxValue)] int id, [FromQuery] ProceduresFunctionsJoinsEnum.ProceduresFunctionsJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<ProcedureFiltersEnum.ProcedureFilters> parameters = null)
	{
		return Ok(await Service.GetFunctions(id, join, parameters));
	}

	[ProducesResponseType(typeof(RelationshipDTO), 200)]
	[ServiceFilter(typeof(DatabaseViewPermissionAttribute))]
	[HttpGet("{id}/relationships")]
	public virtual async Task<IActionResult> GetRelationships([Range(0, int.MaxValue)] int id, [FromQuery] RelationshipsJoinsEnum.RelationshipsJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<RelationshipFiltersEnum.RelationshipFilters> parameters = null)
	{
		return Ok(await Service.GetRelationships(id, join, parameters));
	}

	[ProducesResponseType(typeof(TriggerDTO), 200)]
	[ServiceFilter(typeof(DatabaseViewPermissionAttribute))]
	[HttpGet("{id}/triggers")]
	public virtual async Task<IActionResult> GetTriggers([Range(0, int.MaxValue)] int id, [FromQuery] TriggersJoinsEnum.TriggersJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<TriggerFiltersEnum.TriggerFilters> parameters = null)
	{
		return Ok(await Service.GetTriggers(id, join, parameters));
	}

	/// <summary>
	/// Gets the basic information about feedback.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <returns>The basic information about object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(FeedbackCollection), 200)]
	[ServiceFilter(typeof(DatabaseViewPermissionAttribute))]
	[ServiceFilter(typeof(DatabaseCommunityViewPermissionAttribute))]
	[HttpGet("feedback/{id}")]
	public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [FromQuery] CollectionFeedbackParameters<FeedbackFiltersEnum.FeedbackFilters> parameters = null)
	{
		return Ok(await Service.GetFeedback(id, parameters));
	}

	[ProducesResponseType(typeof(ProcedureDTO), 200)]
	[ServiceFilter(typeof(DatabaseViewPermissionAttribute))]
	[HttpGet("{id}/structures")]
	public virtual async Task<IActionResult> GetStructures([Range(0, int.MaxValue)] int id, [FromQuery] StructuresJoinsEnum.StructuresJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<TableFiltersEnum.TableFilters> parameters = null)
	{
		return Ok(await Service.GetStructures(id, join, parameters));
	}

	[ProducesResponseType(typeof(DataLineageDiagramDTO), 200)]
	[ServiceFilter(typeof(DatabaseLineageViewPermissionAttribute))]
	[HttpGet("{id}/data-lineage-diagram")]
	public virtual async Task<IActionResult> GetDataLineage([Range(0, int.MaxValue)] int id)
	{
		return Ok(await Service.GetDataLineage(id));
	}

	[HttpGet("{id}/users-with-permissions")]
	[ServiceFilter(typeof(DatabaseUserViewPermissionAttribute))]
	public async Task<IActionResult> GetUsersAndGroupsDatabasePermissions(int id)
	{
		return Ok(await permissionsService.GetUsersAndGroupsDatabasePermissions(id));
	}

	[ServiceFilter(typeof(DatabaseEditPermissionAttribute))]
	[HttpPost("{id}/put-title")]
	public async Task<IActionResult> UpdateTitle([FromForm][Required][StringLength(80)] string title, int id)
	{
		title = await WriteService.UpdateTitle(id, title);
		return Ok(title);
	}

	[ServiceFilter(typeof(DatabaseEditPermissionAttribute))]
	[HttpPost("{id}/put-description")]
	public async Task<IActionResult> UpdateDescription([FromForm] string description, int id)
	{
		description = await WriteService.UpdateDescription(id, description);
		return Ok(description);
	}

	[ServiceFilter(typeof(DatabaseEditPermissionAttribute))]
	[HttpPost("{id}/put-name")]
	public async Task<IActionResult> UpdateName([FromForm] string name, int id)
	{
		name = await WriteService.UpdateName(id, name);
		return Ok(name);
	}

	[ServiceFilter(typeof(DatabaseEditPermissionAttribute))]
	public override Task<IActionResult> UpdateCustomField([FromForm] string fieldName, [FromForm] string value, int id)
	{
		return base.UpdateCustomField(fieldName, value, id);
	}

	/// <summary>
	/// Allow add new permission for specific database
	/// </summary>
	/// <param name="id">Database id</param>
	/// <param name="permission">Permission object to add</param>
	/// <returns></returns>
	[HttpPost("{id}/create-user-permission")]
	[ServiceFilter(typeof(DatabaseUserManagePermissionAttribute))]
	public async Task<IActionResult> CreateUserPermission(int id, [FromForm] PermissionCreateDTO permission)
	{
		return Ok(await permissionsWriteService.CreateUserPermission(permission));
	}

	/// <summary>
	/// Allow remove permission for specific database
	/// </summary>
	/// <param name="id">Database id</param>
	/// <param name="permissionId">Permission id to remove</param>
	/// <returns></returns>
	[HttpPost("{id}/delete-user-permission/{permissionId}")]
	[ServiceFilter(typeof(DatabaseUserManagePermissionAttribute))]
	public async Task<IActionResult> Delete(int id, int permissionId)
	{
		await permissionsWriteService.Delete(permissionId);
		return Ok();
	}
}
