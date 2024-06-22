using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.CommunityView;
using Dataedo.Api.Attributes.DocumentationEditPermission;
using Dataedo.Api.Attributes.DocumentationViewPermission;
using Dataedo.Api.Attributes.DocumentationViewPermission.ScriptsViewPermission;
using Dataedo.Api.Attributes.LineageView;
using Dataedo.Api.Attributes.SchemaChangesView;
using Dataedo.Api.Controllers.Base;
using Dataedo.Api.Controllers.Interfaces;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Parameters.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Procedures.DTO;
using Dataedo.Repository.Services.Features.DatabaseObjects.Procedures.Interfaces;
using Dataedo.Repository.Services.Features.Dependencies;
using Dataedo.Repository.Services.Features.Feedback;
using Dataedo.Repository.Services.Features.Feedback.Enums;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.DTO;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.Interfaces;
using Dataedo.Repository.Services.Interfaces.Base;
using Dataedo.Repository.Services.JoinDefinitions;
using Dataedo.Repository.Services.JoinDefinitions.Base;
using Dataedo.Repository.Services.Parameters;
using Dataedo.Repository.Services.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

/// <summary>
/// The class providing actions for procedure objects.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ProceduresController : BaseObjectController, IDependencies
{
	private readonly IObjectSchemaUpdatesService schemaUpdatesService;

	/// <summary>
	/// Gets or sets the object providing access to repository for data of objects.
	/// </summary>
	protected new IProceduresService Service => base.Service as IProceduresService;

	protected new IWritableDatabaseObjectService WriteService => base.WriteService;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.ProceduresController" /> class for actions for procedures objects.
	/// </summary>
	/// <param name="repositoryAccessManager">The object providing getting object for accessing to repository.</param>
	public ProceduresController(IRepositoryAccessManager repositoryAccessManager)
		: base(repositoryAccessManager.GetRepository().ProceduresService, repositoryAccessManager.GetRepository().ProceduresWriteService)
	{
		columnDataLineageService = repositoryAccessManager.GetRepository().ProcedureColumnDataLineageService;
		schemaUpdatesService = repositoryAccessManager.GetRepository().ProcedureSchemaChangeTrackingService;
	}

	/// <summary>
	/// Gets the basic information about procedure.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>The basic information about object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(ProcedureDTO), 200)]
	[ServiceFilter(typeof(ProcedureFunctionViewPermissionAttribute))]
	[HttpGet("{id}")]
	public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [FromQuery] ProceduresJoinsEnum.ProceduresJoins[] join = null)
	{
		return Ok(await Service.GetObject(id, EnumTools.Convert<ProceduresJoinsEnum.ProceduresJoins, ProceduresFunctionsJoinsEnum.ProceduresFunctionsJoins>(join)));
	}

	/// <summary>
	/// Gets the list of basic information about parameters of procedure.
	/// </summary>
	/// <param name="id">The ID of procedure.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <param name="parameters">The parameters providing selecting required set of data by skipping and limiting result items.</param>
	/// <returns>The list of basic information about parameters of procedure.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(ParameterDTO), 200)]
	[HttpGet("{id}/parameters")]
	[ServiceFilter(typeof(ProcedureFunctionViewPermissionAttribute))]
	public virtual async Task<IActionResult> GetParameters([Range(0, int.MaxValue)] int id, [FromQuery] ParametersJoinsEnum.ParametersJoins[] join = null, [FromQuery] OffsetLimitParameters parameters = null)
	{
		return Ok(await Service.GetObjectParameters(id, join));
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
	[ServiceFilter(typeof(ProcedureFunctionScriptsViewPermissionAttribute))]
	public virtual async Task<IActionResult> GetScript([Range(0, int.MaxValue)] int id)
	{
		return Ok(await Service.GetScript(id));
	}

	[ServiceFilter(typeof(ProcedureFunctionEditPermissionAttribute))]
	public override Task<IActionResult> UpdateCustomField([FromForm] string fieldName, [FromForm] string value, int id)
	{
		return base.UpdateCustomField(fieldName, value, id);
	}

	[ServiceFilter(typeof(ProcedureFunctionEditPermissionAttribute))]
	public override Task<IActionResult> UpdateDescription([FromForm] string description, int id)
	{
		return base.UpdateDescription(description, id);
	}

	[ServiceFilter(typeof(ProcedureFunctionEditPermissionAttribute))]
	public override Task<IActionResult> UpdateTitle([FromForm][StringLength(80)] string title, int id)
	{
		return base.UpdateTitle(title, id);
	}

	[ServiceFilter(typeof(ProcedureFunctionCommunityViewPermissionAttribute))]
	public override Task<IActionResult> GetFeedbacks([Range(0, int.MaxValue)] int id, [FromQuery] CollectionFeedbackParameters<FeedbackFiltersEnum.FeedbackFilters> parameters = null)
	{
		return base.GetFeedbacks(id, parameters);
	}

	[ServiceFilter(typeof(ProcedureFunctionDependencyViewAttribute))]
	public override Task<IActionResult> GetUsedByDependencies([FromQuery] DependencyInformation dependencyInformation, [FromQuery][Range(0, int.MaxValue)][Required] int rootDatabaseId, [FromQuery] DependencyJoinsEnum.DependencyJoins[] join = null)
	{
		return base.GetUsedByDependencies(dependencyInformation, rootDatabaseId, join);
	}

	[ServiceFilter(typeof(ProcedureFunctionDependencyViewAttribute))]
	public override Task<IActionResult> GetUsesDependencies([FromQuery] DependencyInformation dependencyInformation, [FromQuery][Range(0, int.MaxValue)][Required] int rootDatabaseId, [FromQuery] DependencyJoinsEnum.DependencyJoins[] join = null)
	{
		return base.GetUsesDependencies(dependencyInformation, rootDatabaseId, join);
	}

	[ServiceFilter(typeof(ProcedureFunctionLineageViewPermissionAttribute))]
	public override Task<IActionResult> GetDataLineageDiagramData(int id)
	{
		return base.GetDataLineageDiagramData(id);
	}

	[HttpGet("{id}/has-permission")]
	public async Task<IActionResult> HasPermission([Range(0, int.MaxValue)] int id, [FromQuery] RoleActionType.RoleAction roleAction)
	{
		return Ok(await Service.HasPermission(id, roleAction));
	}

	[ServiceFilter(typeof(ProcedureFunctionLineageViewPermissionAttribute))]
	public override async Task<IActionResult> GetColumnDataLineage(int id)
	{
		return await base.GetColumnDataLineage(id);
	}

	[ProducesResponseType(typeof(SchemaUpdateDTO), 200)]
	[ServiceFilter(typeof(ProcedureFunctionSchemaChangesViewAttribute))]
	[HttpGet("{id}/schema-updates")]
	public virtual async Task<IActionResult> GetSchemaUpdates([Range(0, int.MaxValue)] int id, [FromQuery] OffsetLimitParameters parameters = null)
	{
		return Ok(await schemaUpdatesService.GetSchemaUpdates(id, parameters));
	}
}
