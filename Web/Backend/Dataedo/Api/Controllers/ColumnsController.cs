using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.CommunityView;
using Dataedo.Api.Attributes.DocumentationEditPermission;
using Dataedo.Api.Attributes.DocumentationViewPermission;
using Dataedo.Api.Attributes.Profiling;
using Dataedo.Api.Attributes.SchemaChangesView;
using Dataedo.Api.Controllers.Base;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Columns.Interfaces;
using Dataedo.Repository.Services.Features.Feedback;
using Dataedo.Repository.Services.Features.Feedback.DTO;
using Dataedo.Repository.Services.Features.Feedback.Enums;
using Dataedo.Repository.Services.Features.GlossaryEntries;
using Dataedo.Repository.Services.Features.GlossaryEntries.DTO;
using Dataedo.Repository.Services.Features.Keys.DTO;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Dataedo.Repository.Services.Features.Relationships.DTO;
using Dataedo.Repository.Services.Features.Relationships.Enums;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.DTO;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.Interfaces;
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
public class ColumnsController : CustomFieldsController
{
	private readonly IObjectSchemaUpdatesService schemaUpdatesService;

	/// <summary>
	/// Gets object providing access to repository for data of objects.
	/// </summary>
	protected new IColumnsService Service => base.Service as IColumnsService;

	protected new IColumnsWriteService WriteService => base.WriteService as IColumnsWriteService;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.ColumnsController" /> class for actions for column objects.
	/// </summary>
	/// <param name="repositoryAccessManager">The object providing getting object for accessing to repository.</param>
	public ColumnsController(IRepositoryAccessManager repositoryAccessManager)
		: base(repositoryAccessManager.GetRepository().ColumnsService, repositoryAccessManager.GetRepository().ColumnsWriteService)
	{
		schemaUpdatesService = repositoryAccessManager.GetRepository().ColumnSchemaChangeTrackingService;
	}

	/// <summary>
	/// Gets the basic information about column.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>The basic information about object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[HttpGet("{id}")]
	[ServiceFilter(typeof(ColumnPermissionViewAttribute))]
	public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [FromQuery] ColumnsJoinsEnum.ColumnsJoins[] join = null)
	{
		return Ok(await Service.GetObject(id, join));
	}

	/// <summary>
	/// Gets the collection of columns that their parent is current column.
	/// </summary>
	/// <param name="id">The ID of column.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>The collection of columns that their parent is current column.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[HttpGet("subcolumns")]
	public virtual async Task<IActionResult> GetColumns([FromQuery] IEnumerable<int> columnIds, [FromQuery] ColumnsJoinsEnum.ColumnsJoins[] join = null)
	{
		return Ok(await Service.GetColumns(columnIds, join));
	}

	/// <summary>
	/// Gets the metadata (creation, import and update information) information.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <returns>The metadata (creation, import and update information) information.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(MetadataDTO), 200)]
	[HttpGet("{id}/metadata")]
	[ServiceFilter(typeof(ColumnPermissionViewAttribute))]
	public virtual async Task<IActionResult> GetMetadata(int id)
	{
		return Ok(await Service.GetMetadata(id));
	}

	/// <summary>
	/// Gets the basic information about glossary entries related to column.
	/// </summary>
	/// <param name="id">The ID of column.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <param name="parameters">The parameters providing selecting required set of data by skipping, limiting, filtering result items.</param>
	/// <returns>The list of basic information about glossary entries related to column.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(GlossaryEntryDTO), 200)]
	[HttpGet("{id}/glossary-entries")]
	[ServiceFilter(typeof(ColumnPermissionViewAttribute))]
	public virtual async Task<IActionResult> GetGlossaryEntries([Range(0, int.MaxValue)] int id, [FromQuery] GlossaryEntriesJoinsEnum.GlossaryEntryJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<GlossaryEntryFiltersEnum.GlossaryEntryFilters> parameters = null)
	{
		return Ok(await Service.GetGlossaryEntries(id, join, parameters));
	}

	/// <summary>
	/// Gets the object containing collection of relationships that consists of column with provided ID as referenced column.
	/// </summary>
	/// <param name="id">The ID of column.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>The object containing collection of relationships that consists of column with provided ID as referenced column.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(CollectionDTO<RelationshipDTO>), 200)]
	[HttpGet("{id}/relationships/referenced")]
	[ServiceFilter(typeof(ColumnPermissionViewAttribute))]
	public virtual async Task<IActionResult> GetRelationshipsReferenced([Range(0, int.MaxValue)] int id, [FromQuery] RelationshipsJoinsEnum.RelationshipsJoins[] join = null)
	{
		return Ok(await Service.GetRelationships(RelationshipTypeEnum.Referenced, id, join));
	}

	/// <summary>
	/// Gets the object containing collection of relationships that consists of column with provided ID as referencing column.
	/// </summary>
	/// <param name="id">The ID of column.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>The object containing collection of relationships that consists of column with provided ID as referencing column.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(CollectionDTO<RelationshipDTO>), 200)]
	[HttpGet("{id}/relationships/referencing")]
	[ServiceFilter(typeof(ColumnPermissionViewAttribute))]
	public virtual async Task<IActionResult> GetRelationshipsReferencing([Range(0, int.MaxValue)] int id, [FromQuery] RelationshipsJoinsEnum.RelationshipsJoins[] join = null)
	{
		return Ok(await Service.GetRelationships(RelationshipTypeEnum.Referencing, id, join));
	}

	/// <summary>
	/// Gets the object containing collection of keys that consists of column with provided ID.
	/// </summary>
	/// <param name="id">The ID of column.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>The object containing collection of keys that consists of column with provided ID.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(CollectionDTO<KeyDTO>), 200)]
	[HttpGet("{id}/keys")]
	[ServiceFilter(typeof(ColumnPermissionViewAttribute))]
	public virtual async Task<IActionResult> GetKeys([Range(0, int.MaxValue)] int id, [FromQuery] KeysJoinsEnum.KeysJoins[] join = null)
	{
		return Ok(await Service.GetKeys(id, join));
	}

	[ProducesResponseType(typeof(CollectionDTO<KeyDTO>), 200)]
	[HttpGet("{id}/linked-columns")]
	[ServiceFilter(typeof(ColumnPermissionViewAttribute))]
	public virtual async Task<IActionResult> GetLinkedColumns([Range(0, int.MaxValue)] int id, [FromQuery] ColumnsJoinsEnum.ColumnsJoins[] join = null)
	{
		return Ok(await Service.GetLinkedColumns(id, join));
	}

	[ProducesResponseType(typeof(CollectionDTO<KeyDTO>), 200)]
	[HttpGet("{id}/columns-similar-by-name-or-title")]
	[ServiceFilter(typeof(ColumnPermissionViewAttribute))]
	public virtual async Task<IActionResult> GetSimilarColumnsByNameOrTitle([Range(0, int.MaxValue)] int id, [FromQuery] ColumnsJoinsEnum.ColumnsJoins[] join = null)
	{
		return Ok(await Service.GetSimilarColumnsByNameOrTitle(id, join));
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
	[ServiceFilter(typeof(ColumnCommunityViewPermissionAttribute))]
	public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [FromQuery] CollectionFeedbackParameters<FeedbackFiltersEnum.FeedbackFilters> parameters = null)
	{
		return Ok(await Service.GetFeedback(id, "COLUMN", parameters));
	}

	[HttpGet("{id}/profiling")]
	[ServiceFilter(typeof(ProfilingAccesibilityAttribute))]
	[ServiceFilter(typeof(ProfilingPermissionViewAttribute))]
	public virtual async Task<IActionResult> GetProfiling([Range(0, int.MaxValue)] int id)
	{
		return Ok(await Service.GetProfiling(id));
	}

	[HttpGet("{id}/has-permission")]
	public async Task<IActionResult> HasPermission([Range(0, int.MaxValue)] int id, [FromQuery] RoleActionType.RoleAction roleAction)
	{
		return Ok(await Service.HasPermission(id, roleAction));
	}

	[ProducesResponseType(typeof(SchemaUpdateDTO), 200)]
	[ServiceFilter(typeof(ColumnSchemaChangesViewAttribute))]
	[HttpGet("{id}/schema-updates")]
	public virtual async Task<IActionResult> GetSchemaUpdates([Range(0, int.MaxValue)] int id, [FromQuery] OffsetLimitParameters parameters = null)
	{
		return Ok(await schemaUpdatesService.GetSchemaUpdates(id, parameters));
	}

	[HttpPost("{id}/put-title")]
	[ServiceFilter(typeof(ColumnEditPermissionAttribute))]
	public async Task<IActionResult> UpdateTitle([FromForm][StringLength(80)] string title, int id)
	{
		title = await WriteService.UpdateTitle(id, title);
		return Ok(title);
	}

	[HttpPost("{id}/put-description")]
	[ServiceFilter(typeof(ColumnEditPermissionAttribute))]
	public async Task<IActionResult> UpdateDescription([FromForm] string description, int id)
	{
		description = await WriteService.UpdateDescription(id, description);
		return Ok(description);
	}

	[HttpPost("{id}/put-name")]
	[ServiceFilter(typeof(ColumnEditPermissionAttribute))]
	public async Task<IActionResult> UpdateName([FromForm] string name, int id)
	{
		name = await WriteService.UpdateName(id, name);
		return Ok(name);
	}

	[ServiceFilter(typeof(ColumnEditPermissionAttribute))]
	public override Task<IActionResult> UpdateCustomField([FromForm] string fieldName, [FromForm] string value, int id)
	{
		return base.UpdateCustomField(fieldName, value, id);
	}
}
