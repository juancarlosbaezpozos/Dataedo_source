using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.CommunityView;
using Dataedo.Api.Attributes.DocumentationEditPermission;
using Dataedo.Api.Attributes.DocumentationViewPermission;
using Dataedo.Api.Attributes.LineageView;
using Dataedo.Api.Attributes.SchemaChangesView;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.DTO;
using Dataedo.Repository.Services.Features.CustomFields.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Procedures;
using Dataedo.Repository.Services.Features.DatabaseObjects.Procedures.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Tables;
using Dataedo.Repository.Services.Features.DatabaseObjects.Tables.DTO;
using Dataedo.Repository.Services.Features.DataLineage.ObjectLevel.DTO;
using Dataedo.Repository.Services.Features.Feedback;
using Dataedo.Repository.Services.Features.Feedback.Enums;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Dataedo.Repository.Services.Features.Relationships.DTO;
using Dataedo.Repository.Services.Features.Relationships.Enums;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.DTO;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.Interfaces;
using Dataedo.Repository.Services.Features.SubjectAreas;
using Dataedo.Repository.Services.Features.SubjectAreas.DTO;
using Dataedo.Repository.Services.Features.SubjectAreas.Interfaces;
using Dataedo.Repository.Services.JoinDefinitions;
using Dataedo.Repository.Services.JoinDefinitions.Base;
using Dataedo.Repository.Services.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

/// <summary>
/// The class providing actions for subject area objects.
/// </summary>
[Route("api/subject-areas")]
[Authorize]
[ApiController]
public class SubjectAreasController : ControllerBase
{
    /// <summary>
    /// Gets object providing access to repository for data of objects.
    /// </summary>
    private readonly ISubjectAreasService service;

    private readonly ISubjectAreaWriteService writeService;

    private readonly IObjectSchemaUpdatesService schemaUpdatesService;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.SubjectAreasController" /> class for actions for subject areas objects.
    /// </summary>
    /// <param name="repositoryAccessManager">The object providing getting object for accessing to repository.</param>
    public SubjectAreasController(IRepositoryAccessManager repositoryAccessManager)
    {
        service = repositoryAccessManager.GetRepository().SubjectAreasService;
        writeService = repositoryAccessManager.GetRepository().SubjectAreasWriteService;
        schemaUpdatesService = repositoryAccessManager.GetRepository().SubjectAreaSchemaChangeTrackingService;
    }

    /// <summary>
    /// Gets the list of basic information about subject areas.
    /// </summary>
    /// <param name="isRoot">The value indicating whether subject area should be found in root level; otherwise, in another levels.</param>
    /// <param name="join">The array of names of data to load with objects.</param>
    /// <param name="parameters">The parameters providing selecting required set of data by skipping and limiting result items.</param>
    /// <returns>The basic information about object.</returns>
    /// <response code="200">Successful operation.</response>
    /// <response code="400">Invalid ID supplied.</response>
    /// <response code="404">Object not found.</response>
    [ProducesResponseType(typeof(SubjectAreaDTO), 200)]
    [HttpGet]
    public virtual async Task<IActionResult> Get([Required] bool isRoot, [FromQuery] SubjectAreasJoinsEnum.SubjectAreasJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<SubjectAreaFiltersEnum.SubjectAreaFilters> parameters = null)
    {
        return Ok(await service.GetObjectsAsync(isRoot, join, parameters));
    }

    /// <summary>
    /// Gets the basic information about subject area.
    /// </summary>
    /// <param name="id">The ID of object.</param>
    /// <param name="isRoot">The value indicating whether subject area should be found in root level; otherwise, in another levels.</param>
    /// <param name="join">The array of names of data to load with objects.</param>
    /// <returns>The basic information about object.</returns>
    /// <response code="200">Successful operation.</response>
    /// <response code="400">Invalid ID supplied.</response>
    /// <response code="404">Object not found.</response>
    [ProducesResponseType(typeof(SubjectAreaDTO), 200)]
    [HttpGet("{id}")]
    [ServiceFilter(typeof(SubjectAreaDocumentationViewPermissionAttribute))]
    public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [Required] bool isRoot, [FromQuery] SubjectAreasJoinsEnum.SubjectAreasJoins[] join = null)
    {
        return Ok(await service.GetObject(id, isRoot, join));
    }

    [HttpGet("{id}/has-permission")]
    public async Task<IActionResult> HasPermission([Range(0, int.MaxValue)] int id, [Required] bool isRoot, [FromQuery] RoleActionType.RoleAction roleAction)
    {
        return Ok(await service.HasPermission(id, isRoot, roleAction));
    }

    [ProducesResponseType(typeof(SubjectAreaDiagramDTO), 200)]
    [ServiceFilter(typeof(RepositoryDocumentationViewPermssionAttribute))]
    [HttpGet("repository-diagram")]
    public virtual async Task<IActionResult> GetDiagram()
    {
        return Ok(await service.GetDiagram());
    }

    [ProducesResponseType(typeof(SubjectAreaDiagramDTO), 200)]
    [ServiceFilter(typeof(SubjectAreaDocumentationViewPermissionAttribute))]
    [HttpGet("diagram")]
    public virtual async Task<IActionResult> GetDiagram([FromQuery][Range(0, int.MaxValue)] int id, [Required] bool isRoot)
    {
        return Ok(await service.GetDiagram(id));
    }

    [HttpGet("grouped-links")]
    public async Task<IActionResult> GetObjectsAsyncGroupedByDatabase([FromQuery] string filter = null)
    {
        return Ok(await service.GetObjectsAsyncGroupedByDatabase(filter));
    }

    /// <summary>
    /// Gets the ERD of subject area.
    /// </summary>
    /// <param name="id">The ID of subject area.</param>
    /// <param name="isRoot">The value indicating whether subject area should be found in root level; otherwise, in another levels.</param>
    /// <param name="join">The array of names of data to load with objects.</param>
    /// <returns>The list of basic information about functions in subject area.</returns>
    /// <response code="200">Successful operation.</response>
    /// <response code="400">Invalid ID supplied.</response>
    /// <response code="404">Object not found.</response>
    [HttpGet("{id}/erd")]
    [ServiceFilter(typeof(ERDDocumentationViewPermissionAttribute))]
    public virtual async Task<IActionResult> GetErd([Range(0, int.MaxValue)] int id, [Required] bool isRoot, [FromQuery] ErdsJoinsEnum.ErdsJoins[] join = null)
    {
        return Ok(await service.GetErd(id, isRoot, join));
    }

    /// <summary>
    /// Gets the list of basic information about subject areas that their parent is subject area with provided ID.
    /// </summary>
    /// <param name="id">The ID of subject area.</param>
    /// <param name="isRoot">The value indicating whether subject area should be found in root level; otherwise, in another levels.</param>
    /// <param name="join">The array of names of data to load with objects.</param>
    /// <param name="parameters">The parameters providing selecting required set of data by skipping and limiting result items.</param>
    /// <returns>The list of basic information about subject areas that their parent is subject area with provided ID.</returns>
    /// <response code="200">Successful operation.</response>
    /// <response code="400">Invalid ID supplied.</response>
    /// <response code="404">Object not found.</response>
    [ProducesResponseType(typeof(FunctionDTO), 200)]
    [HttpGet("{id}/subject-areas")]
    [ServiceFilter(typeof(SubjectAreaDocumentationViewPermissionAttribute))]
    public virtual async Task<IActionResult> GetSubjectAreas([Range(0, int.MaxValue)] int id, [Required] bool isRoot, [FromQuery] SubjectAreasJoinsEnum.SubjectAreasJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<SubjectAreaFiltersEnum.SubjectAreaFilters> parameters = null)
    {
        return Ok(await service.GetSubjectAreas(id, isRoot, join, parameters));
    }

    /// <summary>
    /// Gets the list of basic information about tables in subject area.
    /// </summary>
    /// <param name="id">The ID of subject area.</param>
    /// <param name="isRoot">The value indicating whether subject area should be found in root level; otherwise, in another levels.</param>
    /// <param name="join">The array of names of data to load with objects.</param>
    /// <param name="parameters">The parameters providing selecting required set of data by skipping and limiting result items.</param>
    /// <returns>The list of basic information about tables in subject area.</returns>
    /// <response code="200">Successful operation.</response>
    /// <response code="400">Invalid ID supplied.</response>
    /// <response code="404">Object not found.</response>
    [ProducesResponseType(typeof(TableDTO), 200)]
    [HttpGet("{id}/tables")]
    [ServiceFilter(typeof(SubjectAreaDocumentationViewPermissionAttribute))]
    public virtual async Task<IActionResult> GetTables([Range(0, int.MaxValue)] int id, [Required] bool isRoot, [FromQuery] TablesJoinsEnum.TablesJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<TableFiltersEnum.TableFilters> parameters = null)
    {
        return Ok(await service.GetTables(id, isRoot, join, parameters));
    }

    [ProducesResponseType(typeof(TableDTO), 200)]
    [HttpGet("{id}/structures")]
    [ServiceFilter(typeof(SubjectAreaDocumentationViewPermissionAttribute))]
    public virtual async Task<IActionResult> GetStructures([Range(0, int.MaxValue)] int id, [Required] bool isRoot, [FromQuery] StructuresJoinsEnum.StructuresJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<TableFiltersEnum.TableFilters> parameters = null)
    {
        return Ok(await service.GetStructures(id, isRoot, join, parameters));
    }

    /// <summary>
    /// Gets the list of basic information about views in subject area.
    /// </summary>
    /// <param name="id">The ID of subject area.</param>
    /// <param name="isRoot">The value indicating whether subject area should be found in root level; otherwise, in another levels.</param>
    /// <param name="join">The array of names of data to load with objects.</param>
    /// <param name="parameters">The parameters providing selecting required set of data by skipping and limiting result items.</param>
    /// <returns>The list of basic information about views in subject area.</returns>
    /// <response code="200">Successful operation.</response>
    /// <response code="400">Invalid ID supplied.</response>
    /// <response code="404">Object not found.</response>
    [ProducesResponseType(typeof(ViewDTO), 200)]
    [HttpGet("{id}/views")]
    [ServiceFilter(typeof(SubjectAreaDocumentationViewPermissionAttribute))]
    public virtual async Task<IActionResult> GetViews([Range(0, int.MaxValue)] int id, [Required] bool isRoot, [FromQuery] ViewsJoinsEnum.ViewsJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<TableFiltersEnum.TableFilters> parameters = null)
    {
        return Ok(await service.GetViews(id, isRoot, join, parameters));
    }

    [ProducesResponseType(typeof(CollectionDTO<RelationshipDTO>), 200)]
    [HttpGet("{id}/relationships/referencing")]
    public virtual async Task<IActionResult> GetRelationshipsReferencing([Range(0, int.MaxValue)] int id, [FromQuery] RelationshipsJoinsEnum.RelationshipsJoins[] join = null)
    {
        CollectionDTO<RelationshipDTO> result = await service.GetRelationships(RelationshipTypeEnum.Referencing, id, join);
        if (result == null)
        {
            return NoContent();
        }
        return Ok(result);
    }

    [ProducesResponseType(typeof(CollectionDTO<RelationshipDTO>), 200)]
    [HttpGet("{id}/relationships/referenced")]
    public virtual async Task<IActionResult> GetRelationshipsReferenced([Range(0, int.MaxValue)] int id, [FromQuery] RelationshipsJoinsEnum.RelationshipsJoins[] join = null)
    {
        CollectionDTO<RelationshipDTO> result = await service.GetRelationships(RelationshipTypeEnum.Referenced, id, join);
        if (result == null)
        {
            return NoContent();
        }
        return Ok(result);
    }

    /// <summary>
    /// Gets the list of basic information about procedures in subject area.
    /// </summary>
    /// <param name="id">The ID of subject area.</param>
    /// <param name="isRoot">The value indicating whether subject area should be found in root level; otherwise, in another levels.</param>
    /// <param name="join">The array of names of data to load with objects.</param>
    /// <param name="parameters">The parameters providing selecting required set of data by skipping and limiting result items.</param>
    /// <returns>The list of basic information about procedures in subject area.</returns>
    /// <response code="200">Successful operation.</response>
    /// <response code="400">Invalid ID supplied.</response>
    /// <response code="404">Object not found.</response>
    [ProducesResponseType(typeof(ProcedureDTO), 200)]
    [HttpGet("{id}/procedures")]
    [ServiceFilter(typeof(SubjectAreaDocumentationViewPermissionAttribute))]
    public virtual async Task<IActionResult> GetProcedures([Range(0, int.MaxValue)] int id, [Required] bool isRoot, [FromQuery] ProceduresFunctionsJoinsEnum.ProceduresFunctionsJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<ProcedureFiltersEnum.ProcedureFilters> parameters = null)
    {
        CollectionDTO<ProcedureFunctionDTO> result = await service.GetProcedures(id, isRoot, join, parameters);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>
    /// Gets the list of basic information about functions in subject area.
    /// </summary>
    /// <param name="id">The ID of subject area.</param>
    /// <param name="isRoot">The value indicating whether subject area should be found in root level; otherwise, in another levels.</param>
    /// <param name="join">The array of names of data to load with objects.</param>
    /// <param name="parameters">The parameters providing selecting required set of data by skipping and limiting result items.</param>
    /// <returns>The list of basic information about functions in subject area.</returns>
    /// <response code="200">Successful operation.</response>
    /// <response code="400">Invalid ID supplied.</response>
    /// <response code="404">Object not found.</response>
    [ProducesResponseType(typeof(FunctionDTO), 200)]
    [HttpGet("{id}/functions")]
    [ServiceFilter(typeof(SubjectAreaDocumentationViewPermissionAttribute))]
    public virtual async Task<IActionResult> GetFunctions([Range(0, int.MaxValue)] int id, [Required] bool isRoot, [FromQuery] ProceduresFunctionsJoinsEnum.ProceduresFunctionsJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<ProcedureFiltersEnum.ProcedureFilters> parameters = null)
    {
        return Ok(await service.GetFunctions(id, isRoot, join, parameters));
    }

    /// <summary>
    /// Gets the values of custom fields of object.
    /// </summary>
    /// <param name="id">The ID of object.</param>
    /// <param name="isRoot">The value indicating whether subject area should be found in root level; otherwise, in another levels.</param>
    /// <param name="join">The array of names of data to load with objects.</param>
    /// <returns>The custom fields of object.</returns>
    /// <response code="200">Successful operation.</response>
    /// <response code="400">Invalid ID supplied.</response>
    /// <response code="404">Object not found.</response>
    [ProducesResponseType(typeof(IEnumerable<CustomFieldDTO>), 200)]
    [HttpGet("{id}/custom-fields")]
    [ServiceFilter(typeof(SubjectAreaDocumentationViewPermissionAttribute))]
    public virtual async Task<IActionResult> GetCustomFields(int id, [Required] bool isRoot, [FromQuery] CustomFieldsJoinsEnum.CustomFieldsJoins[] join = null)
    {
        return Ok(await service.GetCustomFields(id, isRoot, join));
    }

    /// <summary>
    /// Gets the definitions of custom fields applicable for this type of object (that are configured to be used for this type of object).
    /// </summary>
    /// <param name="isRoot">The value indicating whether subject area should be found in root level; otherwise, in another levels.</param>
    /// <param name="join">The array of names of data to load with objects.</param>
    /// <returns>The definitions of custom fields of object.</returns>
    /// <response code="200">Successful operation.</response>
    /// <response code="400">Invalid ID supplied.</response>
    /// <response code="404">Object not found.</response>
    [ProducesResponseType(typeof(IEnumerable<CustomFieldDefinitionDTO>), 200)]
    [HttpGet("custom-fields-definitions")]
    public virtual async Task<IActionResult> GetCustomFieldsDefinitionsForClass([Required] bool isRoot, [FromQuery] CustomFieldDefinitionJoinsEnum.CustomFieldDefinitionJoins[] join = null)
    {
        return Ok(await service.GetCustomFieldsDefinitions(isRoot, join));
    }

    [HttpGet("feedback/{id}")]
    [ServiceFilter(typeof(SubjectAreaDocumentationViewPermissionAttribute))]
    [ServiceFilter(typeof(SubjectAreaCommunityViewPermissionAttribute))]
    public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [FromQuery] CollectionFeedbackParameters<FeedbackFiltersEnum.FeedbackFilters> parameters = null)
    {
        return Ok(await service.GetFeedback(id, parameters));
    }

    [ProducesResponseType(typeof(DataLineageDiagramDTO), 200)]
    [ServiceFilter(typeof(SubjectAreaLineageViewPermissionAttribute))]
    [HttpGet("{id}/data-lineage-diagram")]
    public virtual async Task<IActionResult> GetDataLineage([Range(0, int.MaxValue)] int id)
    {
        return Ok(await service.GetDataLineage(id));
    }

    [HttpGet("{subjectAreaId}/tables/{id}")]
    [ServiceFilter(typeof(TableViewStructurePermissionViewAttribute))]
    [ServiceFilter(typeof(SubjectAreaDocumentationViewPermissionAttribute))]
    public virtual async Task<IActionResult> GetTable([Range(0, int.MaxValue)] int subjectAreaId, [Range(0, int.MaxValue)] int id, [FromQuery] TablesJoinsEnum.TablesJoins[] join = null)
    {
        return Ok(await service.GetTable(id, join));
    }

    [HttpGet("{subjectAreaId}/views/{id}")]
    [ServiceFilter(typeof(TableViewStructurePermissionViewAttribute))]
    [ServiceFilter(typeof(SubjectAreaDocumentationViewPermissionAttribute))]
    public virtual async Task<IActionResult> GetView([Range(0, int.MaxValue)] int subjectAreaId, [Range(0, int.MaxValue)] int id, [FromQuery] ViewsJoinsEnum.ViewsJoins[] join = null)
    {
        return Ok(await service.GetView(id, join));
    }

    [HttpGet("{subjectAreaId}/structures/{id}")]
    [ServiceFilter(typeof(TableViewStructurePermissionViewAttribute))]
    [ServiceFilter(typeof(SubjectAreaDocumentationViewPermissionAttribute))]
    public virtual async Task<IActionResult> GetStructure([Range(0, int.MaxValue)] int subjectAreaId, [Range(0, int.MaxValue)] int id, [FromQuery] StructuresJoinsEnum.StructuresJoins[] join = null)
    {
        return Ok(await service.GetStructure(id, join));
    }

    [HttpGet("{subjectAreaId}/procedures/{id}")]
    [ServiceFilter(typeof(ProcedureFunctionViewPermissionAttribute))]
    [ServiceFilter(typeof(SubjectAreaDocumentationViewPermissionAttribute))]
    public virtual async Task<IActionResult> GetProcedure([Range(0, int.MaxValue)] int subjectAreaId, [Range(0, int.MaxValue)] int id, [FromQuery] ProceduresJoinsEnum.ProceduresJoins[] join = null)
    {
        return Ok(await service.GetProcedure(id, join));
    }

    [HttpGet("{subjectAreaId}/functions/{id}")]
    [ServiceFilter(typeof(ProcedureFunctionViewPermissionAttribute))]
    [ServiceFilter(typeof(SubjectAreaDocumentationViewPermissionAttribute))]
    public virtual async Task<IActionResult> GetFunction([Range(0, int.MaxValue)] int subjectAreaId, [Range(0, int.MaxValue)] int id, [FromQuery] FunctionsJoinsEnum.FunctionsJoins[] join = null)
    {
        return Ok(await service.GetFunction(id, join));
    }

    [ProducesResponseType(typeof(SchemaUpdateDTO), 200)]
    [ServiceFilter(typeof(SubjectAreaSchemaChangesViewAttribute))]
    [HttpGet("{subjectAreaId}/schema-updates")]
    public virtual async Task<IActionResult> GetSchemaUpdates([Range(0, int.MaxValue)] int subjectAreaId, [FromQuery] OffsetLimitParameters parameters = null)
    {
        return Ok(await schemaUpdatesService.GetSchemaUpdates(subjectAreaId, parameters));
    }

    [ServiceFilter(typeof(ManageSubjectAreaDocumentationEditPermissionAttribute))]
    [HttpPost("{id}/put-title")]
    public async Task<IActionResult> UpdateTitle([FromForm][Required][StringLength(80)] string title, int id)
    {
        title = await writeService.UpdateTitle(id, title);
        return Ok(title);
    }

    [ServiceFilter(typeof(ManageSubjectAreaDocumentationEditPermissionAttribute))]
    [HttpPost("{id}/put-description")]
    public async Task<IActionResult> UpdateDescription([FromForm] string description, int id)
    {
        description = await writeService.UpdateDescription(id, description);
        return Ok(description);
    }

    [ServiceFilter(typeof(ManageSubjectAreaDocumentationEditPermissionAttribute))]
    [HttpPost("{id}/put-custom-field")]
    public async Task<IActionResult> UpdateCustomField([FromForm] string fieldName, [FromForm] string value, int id)
    {
        return Ok(await writeService.UpdateCustomField(id, fieldName, value));
    }

    [ServiceFilter(typeof(InsertTablesIntoSubjectAreaAttribute))]
    [ServiceFilter(typeof(ManageSubjectAreaOnInsertIntoDocumentationEditPermissionAttribute))]
    [HttpPost("post-tables")]
    public async Task<IActionResult> InsertTable([FromForm] InsertObjectsIntoModuleDTO model)
    {
        await writeService.InsertTables(model.SubjectAreaId, model.ObjectIds);
        return Ok();
    }

    [ServiceFilter(typeof(InsertProceduresIntoSubjectAreaAttribute))]
    [ServiceFilter(typeof(ManageSubjectAreaOnInsertIntoDocumentationEditPermissionAttribute))]
    [HttpPost("post-procedures")]
    public async Task<IActionResult> InsertProcedure([FromForm] InsertObjectsIntoModuleDTO model)
    {
        await writeService.InsertProcedures(model.SubjectAreaId, model.ObjectIds);
        return Ok();
    }

    [ServiceFilter(typeof(ManageSubjectAreaDocumentationEditPermissionAttribute))]
    [ServiceFilter(typeof(RemoveTableFromSubjectAreaPermissionAttribute))]
    [HttpPost("{id}/delete-table")]
    public async Task<IActionResult> RemoveTable(int id, [FromQuery] int tableId)
    {
        await writeService.RemoveTable(id, tableId);
        return Ok();
    }

    [ServiceFilter(typeof(ManageSubjectAreaDocumentationEditPermissionAttribute))]
    [ServiceFilter(typeof(RemoveProcedureFromSubjectAreaPermissionAttribute))]
    [HttpPost("{id}/delete-procedure")]
    public async Task<IActionResult> RemoveProcedure(int id, [FromQuery] int procedureId)
    {
        await writeService.RemoveProcedure(id, procedureId);
        return Ok();
    }

    [ServiceFilter(typeof(InsertSubjectAreaDocumentationEditPermissionAttribute))]
    [HttpPost("post-subject-area")]
    public async Task<IActionResult> PostSubjectArea([FromForm][Required][Range(0, int.MaxValue)] int databaseId, [FromForm][Required(AllowEmptyStrings = false)][StringLength(80)] string title)
    {
        return Ok(await writeService.InsertSubjectArea(databaseId, title));
    }
}
