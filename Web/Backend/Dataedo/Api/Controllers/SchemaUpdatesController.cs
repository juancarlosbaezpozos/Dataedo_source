using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.DocumentationEditPermission.SCT;
using Dataedo.Api.Attributes.SchemaChangesView;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.DTO;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.DTO;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.Interfaces;
using Dataedo.Repository.Services.Interfaces.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
public class SchemaUpdatesController : ControllerBase
{
	private readonly IObjectChangesService subjectAreaChangesService;

	private readonly IDatabaseObjectChangesService databaseChangesService;

	private readonly IObjectChangesService tableChangesService;

	private readonly IObjectChangesService triggerChangesService;

	private readonly IObjectChangesService columnChangesService;

	private readonly IObjectChangesService procedureChangesService;

	private readonly ISchemaUpdateDetailsService schemaUpdateDetailsService;

	private readonly IDescriptionUpdateService editService;

	public SchemaUpdatesController(IRepositoryAccessManager repositoryAccessManager)
	{
		subjectAreaChangesService = repositoryAccessManager.GetRepository().SubjectAreaChangesService;
		databaseChangesService = repositoryAccessManager.GetRepository().DatabaseChangesService;
		tableChangesService = repositoryAccessManager.GetRepository().TableChangesService;
		triggerChangesService = repositoryAccessManager.GetRepository().TriggerChangesService;
		columnChangesService = repositoryAccessManager.GetRepository().ColumnChangesService;
		procedureChangesService = repositoryAccessManager.GetRepository().ProcedureChangesService;
		schemaUpdateDetailsService = repositoryAccessManager.GetRepository().SchemaUpdateDetailsService;
		editService = repositoryAccessManager.GetRepository().SchemaUpdateEditService;
	}

	[ProducesResponseType(typeof(ObjectChangeDTO), 200)]
	[ServiceFilter(typeof(SchemaUpdatesViewAttribute))]
	[HttpGet("{id}/database-changes")]
	public virtual async Task<IActionResult> GetDatabaseChanges([Range(0, int.MaxValue)] int id)
	{
		return Ok(await databaseChangesService.GetChanges(id));
	}

	[ProducesResponseType(typeof(ObjectChangeDTO), 200)]
	[ServiceFilter(typeof(SchemaUpdatesViewAttribute))]
	[HttpGet("{id}/table-changes")]
	public virtual async Task<IActionResult> GetTableChanges([Range(0, int.MaxValue)] int id, [FromQuery][Required][Range(0, int.MaxValue)] int tableId)
	{
		return Ok(await tableChangesService.GetChanges(id, tableId));
	}

	[ProducesResponseType(typeof(ObjectChangeDTO), 200)]
	[ServiceFilter(typeof(SchemaUpdatesViewAttribute))]
	[HttpGet("{id}/trigger-changes")]
	public virtual async Task<IActionResult> GetTriggerChanges([Range(0, int.MaxValue)] int id, [FromQuery][Required][Range(0, int.MaxValue)] int triggerId)
	{
		return Ok(await triggerChangesService.GetChanges(id, triggerId));
	}

	[ProducesResponseType(typeof(ObjectChangeDTO), 200)]
	[ServiceFilter(typeof(SchemaUpdatesViewAttribute))]
	[HttpGet("{id}/column-changes")]
	public virtual async Task<IActionResult> GetColumnChanges([Range(0, int.MaxValue)] int id, [FromQuery][Required][Range(0, int.MaxValue)] int columnId)
	{
		return Ok(await columnChangesService.GetChanges(id, columnId));
	}

	[ProducesResponseType(typeof(ObjectChangeDTO), 200)]
	[ServiceFilter(typeof(SchemaUpdatesViewAttribute))]
	[HttpGet("{id}/procedure-changes")]
	public virtual async Task<IActionResult> GetProcedureChanges([Range(0, int.MaxValue)] int id, [FromQuery][Required][Range(0, int.MaxValue)] int procedureId)
	{
		return Ok(await procedureChangesService.GetChanges(id, procedureId));
	}

	[ProducesResponseType(typeof(ObjectChangeDTO), 200)]
	[ServiceFilter(typeof(SchemaUpdatesViewAttribute))]
	[HttpGet("{id}/subject-area-changes")]
	public virtual async Task<IActionResult> GetSubjectAreaChanges([Range(0, int.MaxValue)] int id, [FromQuery][Required][Range(0, int.MaxValue)] int subjectAreaId)
	{
		return Ok(await subjectAreaChangesService.GetChanges(id, subjectAreaId));
	}

	[ProducesResponseType(typeof(SchemaUpdateDetailsDTO), 200)]
	[ServiceFilter(typeof(SchemaUpdatesViewAttribute))]
	[HttpGet("{id}/details")]
	public virtual async Task<IActionResult> GetSchemaUpdateDetails([Range(0, int.MaxValue)] int id)
	{
		return Ok(await schemaUpdateDetailsService.GetDetails(id));
	}

	[ServiceFilter(typeof(SchemaUpdateDocumentationEditAttribute))]
	[HttpPost("update-description")]
	public virtual async Task<IActionResult> UpdateDescription([FromForm] DescriptionUpdateDTO descriptionUpdateDTO)
	{
		return Ok(await editService.UpdateDescription(descriptionUpdateDTO.Id, descriptionUpdateDTO.Description));
	}
}
