using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.CommunityView;
using Dataedo.Api.Attributes.DocumentationEditPermission;
using Dataedo.Api.Attributes.DocumentationViewPermission;
using Dataedo.Api.Attributes.LineageView;
using Dataedo.Api.Attributes.SchemaChangesView;
using Dataedo.Api.Controllers.Interfaces;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Columns;
using Dataedo.Repository.Services.Features.DatabaseObjects.Columns.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Tables.Interfaces;
using Dataedo.Repository.Services.Features.Dependencies;
using Dataedo.Repository.Services.Features.Feedback;
using Dataedo.Repository.Services.Features.Feedback.Enums;
using Dataedo.Repository.Services.Features.GlossaryEntries;
using Dataedo.Repository.Services.Features.GlossaryEntries.DTO;
using Dataedo.Repository.Services.Features.Keys.DTO;
using Dataedo.Repository.Services.Features.Keys.Enums;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Dataedo.Repository.Services.Features.Relationships;
using Dataedo.Repository.Services.Features.Relationships.DTO;
using Dataedo.Repository.Services.Features.Relationships.Enums;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.DTO;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.Interfaces;
using Dataedo.Repository.Services.Interfaces.Base;
using Dataedo.Repository.Services.JoinDefinitions;
using Dataedo.Repository.Services.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers.Base;

public abstract class TablesViewsStructuresController : BaseObjectController, IDependencies
{
	private readonly IObjectSchemaUpdatesService schemaUpdatesService;

	/// <summary>
	/// Gets the object providing access to repository for data of objects.
	/// </summary>
	protected new ITablesViewsStructuresService Service => base.Service as ITablesViewsStructuresService;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.Base.TablesViewsController" /> class providing base actions for tables and views.
	/// </summary>
	/// <param name="service">The service object providing actions for data of objects.</param>
	public TablesViewsStructuresController(ITablesViewsStructuresService service, IWritableDatabaseObjectService writeService, IRepositoryAccessManager repositoryAccessManager)
		: base(service, writeService)
	{
		columnDataLineageService = repositoryAccessManager.GetRepository().TableColumnDataLineageService;
		schemaUpdatesService = repositoryAccessManager.GetRepository().TableSchemaChangeTrackingService;
	}

	/// <summary>
	/// Gets the basic information about columns of object.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <param name="parameters"></param>
	/// <returns>The list of basic information about columns of object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[HttpGet("{id}/columns")]
	[ServiceFilter(typeof(TableViewStructurePermissionViewAttribute))]
	public virtual async Task<IActionResult> GetColumns([Range(0, int.MaxValue)] int id, [FromQuery] ColumnsJoinsEnum.ColumnsJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<ColumnFiltersEnum.ColumnFilters> parameters = null)
	{
		return Ok(await Service.GetColumns(id, join, parameters));
	}

	[ProducesResponseType(typeof(ColumnDTO), 200)]
	[HttpGet("{id}/all-columns-in-hierarchical-order")]
	[ServiceFilter(typeof(TableViewStructurePermissionViewAttribute))]
	public virtual async Task<IActionResult> GetAllColumnsByObjectId([Range(0, int.MaxValue)] int id, [FromQuery] ColumnsJoinsEnum.ColumnsJoins[] join = null)
	{
		return Ok(await Service.GetAllColumnsInHierarchicalOrder(id));
	}

	/// <summary>
	/// Gets the basic information about keys of object.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <param name="parameters"></param>
	/// <returns>The list of basic information about keys of object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(KeyDTO), 200)]
	[HttpGet("{id}/keys")]
	[ServiceFilter(typeof(TableViewStructurePermissionViewAttribute))]
	public virtual async Task<IActionResult> GetKeys([Range(0, int.MaxValue)] int id, [FromQuery] KeysJoinsEnum.KeysJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<UniqueConstraintFiltersEnum.UniqueConstraintFilters> parameters = null)
	{
		return Ok(await Service.GetKeys(id, join, parameters));
	}

	/// <summary>
	/// Gets the basic information about relationships of object.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <param name="parameters"></param>
	/// <returns>The list of basic information about keys of object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(CollectionDTO<RelationshipDTO>), 200)]
	[HttpGet("{id}/relationships/referenced")]
	[ServiceFilter(typeof(TableViewStructurePermissionViewAttribute))]
	public virtual async Task<IActionResult> GetRelationshipsReferenced([Range(0, int.MaxValue)] int id, [FromQuery] RelationshipsJoinsEnum.RelationshipsJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<RelationshipFiltersEnum.RelationshipFilters> parameters = null)
	{
		return Ok(await Service.GetRelationships(RelationshipTypeEnum.Referenced, new int[1] { id }, join, parameters));
	}

	/// <summary>
	/// Gets the basic information about relationships of object.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <param name="parameters"></param>
	/// <returns>The list of basic information about keys of object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(CollectionDTO<RelationshipDTO>), 200)]
	[HttpGet("{id}/relationships/referencing")]
	[ServiceFilter(typeof(TableViewStructurePermissionViewAttribute))]
	public virtual async Task<IActionResult> GetRelationshipsReferencing([Range(0, int.MaxValue)] int id, [FromQuery] RelationshipsJoinsEnum.RelationshipsJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<RelationshipFiltersEnum.RelationshipFilters> parameters = null)
	{
		return Ok(await Service.GetRelationships(RelationshipTypeEnum.Referencing, new int[1] { id }, join, parameters));
	}

	/// <summary>
	/// Gets the basic information about glossary entries related to object.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <param name="parameters">The parameters providing selecting required set of data by skipping, limiting, filtering result items.</param>
	/// <returns>The list of basic information about glossary entries related to object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(GlossaryEntryDTO), 200)]
	[HttpGet("{id}/glossary-entries")]
	[ServiceFilter(typeof(TableViewStructurePermissionViewAttribute))]
	public virtual async Task<IActionResult> GetGlossaryEntries([Range(0, int.MaxValue)] int id, [FromQuery] GlossaryEntriesJoinsEnum.GlossaryEntryJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<GlossaryEntryFiltersEnum.GlossaryEntryFilters> parameters = null)
	{
		return Ok(await Service.GetGlossaryEntries(id, join, parameters));
	}

	[HttpGet("{id}/columns-any-entries")]
	[ServiceFilter(typeof(TableViewStructurePermissionViewAttribute))]
	public virtual async Task<IActionResult> ColumnsHasAnyEntries([Range(0, int.MaxValue)] int id, [FromQuery] GlossaryEntriesJoinsEnum.GlossaryEntryJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<GlossaryEntryFiltersEnum.GlossaryEntryFilters> parameters = null)
	{
		return Ok(await Service.ColumnsAnyEntries(id));
	}

	[ServiceFilter(typeof(TableViewStructureEditPermissionAttribute))]
	public override Task<IActionResult> UpdateCustomField([FromForm] string fieldName, [FromForm] string value, int id)
	{
		return base.UpdateCustomField(fieldName, value, id);
	}

	[ServiceFilter(typeof(TableViewStructureEditPermissionAttribute))]
	public override Task<IActionResult> UpdateDescription([FromForm] string description, int id)
	{
		return base.UpdateDescription(description, id);
	}

	[ServiceFilter(typeof(TableViewStructureEditPermissionAttribute))]
	public override Task<IActionResult> UpdateTitle([FromForm][StringLength(80)] string title, int id)
	{
		return base.UpdateTitle(title, id);
	}

	[ServiceFilter(typeof(TableViewStructureCommunityViewPermissionAttribute))]
	public override Task<IActionResult> GetFeedbacks([Range(0, int.MaxValue)] int id, [FromQuery] CollectionFeedbackParameters<FeedbackFiltersEnum.FeedbackFilters> parameters = null)
	{
		return base.GetFeedbacks(id, parameters);
	}

	[ServiceFilter(typeof(TableViewStructureDependencyViewAttribute))]
	public override Task<IActionResult> GetUsedByDependencies([FromQuery] DependencyInformation dependencyInformation, [FromQuery][Range(0, int.MaxValue)][Required] int rootDatabaseId, [FromQuery] DependencyJoinsEnum.DependencyJoins[] join = null)
	{
		return base.GetUsedByDependencies(dependencyInformation, rootDatabaseId, join);
	}

	[ServiceFilter(typeof(TableViewStructureDependencyViewAttribute))]
	public override Task<IActionResult> GetUsesDependencies([FromQuery] DependencyInformation dependencyInformation, [FromQuery][Range(0, int.MaxValue)][Required] int rootDatabaseId, [FromQuery] DependencyJoinsEnum.DependencyJoins[] join = null)
	{
		return base.GetUsesDependencies(dependencyInformation, rootDatabaseId, join);
	}

	[ServiceFilter(typeof(TableViewStructureLineageViewPermissionAttribute))]
	public override Task<IActionResult> GetDataLineageDiagramData(int id)
	{
		return base.GetDataLineageDiagramData(id);
	}

	[HttpGet("{id}/has-permission")]
	public async Task<IActionResult> HasPermission([Range(0, int.MaxValue)] int id, [FromQuery] RoleActionType.RoleAction roleAction)
	{
		return Ok(await Service.HasPermission(id, roleAction));
	}

	[ServiceFilter(typeof(TableViewStructureLineageViewPermissionAttribute))]
	public override async Task<IActionResult> GetColumnDataLineage(int id)
	{
		return await base.GetColumnDataLineage(id);
	}

	[ProducesResponseType(typeof(SchemaUpdateDTO), 200)]
	[ServiceFilter(typeof(TableViewStructureSchemaChangesViewAttribute))]
	[HttpGet("{id}/schema-updates")]
	public virtual async Task<IActionResult> GetSchemaUpdates([Range(0, int.MaxValue)] int id, [FromQuery] OffsetLimitParameters parameters = null)
	{
		return Ok(await schemaUpdatesService.GetSchemaUpdates(id, parameters));
	}
}
