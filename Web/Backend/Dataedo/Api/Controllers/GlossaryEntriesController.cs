using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.CommunityView;
using Dataedo.Api.Attributes.DocumentationEditPermission;
using Dataedo.Api.Attributes.DocumentationViewPermission;
using Dataedo.Api.Controllers.Base;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Feedback;
using Dataedo.Repository.Services.Features.Feedback.DTO;
using Dataedo.Repository.Services.Features.Feedback.Enums;
using Dataedo.Repository.Services.Features.GlossaryEntries.DTO;
using Dataedo.Repository.Services.Features.GlossaryEntries.Interfaces;
using Dataedo.Repository.Services.Features.GlossaryEntryType.Interfaces;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Dataedo.Repository.Services.Interfaces.Base;
using Dataedo.Repository.Services.JoinDefinitions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

/// <summary>
/// The class providing actions for glossary objects.
/// </summary>
[Route("api/glossary-entries")]
[Authorize]
[ApiController]
public class GlossaryEntriesController : CustomFieldsController
{
	private readonly IGlossaryEntryTypesService typesService;

	private IEntryWriteService writeService;

	/// <summary>
	/// Gets object providing access to repository for data of objects.
	/// </summary>
	protected new IGlossaryEntriesService Service => base.Service as IGlossaryEntriesService;

	protected new IWritableDatabaseObjectService WriteService => base.WriteService as IWritableDatabaseObjectService;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.GlossaryEntriesController" /> class for actions for glossary entries objects.
	/// </summary>
	/// <param name="repositoryAccessManager">The object providing getting object for accessing to repository.</param>
	public GlossaryEntriesController(IRepositoryAccessManager repositoryAccessManager)
		: base(repositoryAccessManager.GetRepository().GlossaryEntriesService, repositoryAccessManager.GetRepository().GlossaryEntriesWriteService)
	{
		writeService = repositoryAccessManager.GetRepository().EntryWriteService;
		typesService = repositoryAccessManager.GetRepository().GlossaryEntryTypeService;
	}

	/// <summary>
	/// Gets the basic information about glossary entry.
	/// </summary>
	/// <param name="id">The ID of glossary entry.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>The basic information about object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(GlossaryEntryDTO), 200)]
	[HttpGet("{id}")]
	[ServiceFilter(typeof(EntryViewPermissionAttribute))]
	public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [FromQuery] GlossaryEntriesJoinsEnum.GlossaryEntryJoins[] join = null)
	{
		return Ok(await Service.GetObjectAsync(id, join));
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
	[ServiceFilter(typeof(EntryViewPermissionAttribute))]
	[ServiceFilter(typeof(GlossaryEntryCommunityViewPermissionAttribute))]
	public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [FromQuery] CollectionFeedbackParameters<FeedbackFiltersEnum.FeedbackFilters> parameters = null)
	{
		return Ok(await Service.GetFeedback(id, parameters));
	}

	[HttpGet("{id}/has-permission")]
	public async Task<IActionResult> HasPermission([Range(0, int.MaxValue)] int id, [FromQuery] RoleActionType.RoleAction roleAction)
	{
		return Ok(await Service.HasPermission(id, roleAction));
	}

	[ServiceFilter(typeof(GlossaryAnyObjectDocumentationViewPermissionAttribute))]
	[HttpGet("types")]
	public async Task<IActionResult> GetTypes()
	{
		return Ok(await typesService.GetGlossaryEntryTypeDTOs());
	}

	[ServiceFilter(typeof(GlossaryEntryEditPermissionAttribute))]
	[HttpPost("{id}/put-description")]
	public async Task<IActionResult> UpdateDescription([FromForm] string description, int id)
	{
		description = await WriteService.UpdateDescription(id, description);
		return Ok(description);
	}

	[ServiceFilter(typeof(GlossaryEntryEditPermissionAttribute))]
	[HttpPost("{id}/put-title")]
	public async Task<IActionResult> UpdateTitle([FromForm][Required][StringLength(80)] string title, int id)
	{
		title = await WriteService.UpdateTitle(id, title);
		return Ok(title);
	}

	[ServiceFilter(typeof(GlossaryEntryEditPermissionAttribute))]
	public override Task<IActionResult> UpdateCustomField([FromForm] string fieldName, [FromForm] string value, int id)
	{
		return base.UpdateCustomField(fieldName, value, id);
	}

	[ServiceFilter(typeof(GlossaryEntryCreatePermissionAttribute))]
	[HttpPost("create")]
	public async Task<IActionResult> Create([FromBody] GlossaryEntryCreateDTO input)
	{
		return Ok(await writeService.Create(input));
	}

	[ServiceFilter(typeof(GlossaryEntryEditPermissionAttribute))]
	[HttpPost("{id}/delete")]
	public async Task<IActionResult> Delete(int id)
	{
		await writeService.Delete(id);
		return Ok();
	}

	[ServiceFilter(typeof(GlossaryEntryEditPermissionAttribute))]
	[HttpPost("{id}/update-type")]
	public async Task<IActionResult> UpdateType(int id, [FromForm] int typeId)
	{
		await writeService.UpdateType(id, typeId);
		return Ok();
	}

	[ServiceFilter(typeof(GlossaryEntryEditPermissionAttribute))]
	[ServiceFilter(typeof(TargetGlossaryEditPermissionAttribute))]
	[HttpPost("{id}/update-parent")]
	public async Task<IActionResult> UpdateParent(int id, [FromForm] int? parentId, [FromForm] int glossaryId)
	{
		await writeService.UpdateParent(id, parentId, glossaryId);
		return Ok();
	}
}
