using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.CommunityView;
using Dataedo.Api.Attributes.DocumentationEditPermission;
using Dataedo.Api.Attributes.DocumentationViewPermission;
using Dataedo.Api.Attributes.UsersView;
using Dataedo.Api.Controllers.Base;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Enums;
using Dataedo.Repository.Services.Features.Feedback;
using Dataedo.Repository.Services.Features.Feedback.DTO;
using Dataedo.Repository.Services.Features.Feedback.Enums;
using Dataedo.Repository.Services.Features.Glossaries;
using Dataedo.Repository.Services.Features.Glossaries.DTO;
using Dataedo.Repository.Services.Features.Glossaries.Interfaces;
using Dataedo.Repository.Services.Features.GlossaryEntries;
using Dataedo.Repository.Services.Features.GlossaryEntries.DTO;
using Dataedo.Repository.Services.Features.Permissions.DTO;
using Dataedo.Repository.Services.Features.Permissions.Interfaces;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Dataedo.Repository.Services.Interfaces.Base;
using Dataedo.Repository.Services.JoinDefinitions;
using Dataedo.Repository.Services.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

/// <summary>
/// The class providing actions for glossary objects.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class GlossariesController : CustomFieldsController
{
	private readonly IPermissionsReadService permissionsService;

	private readonly IPermissionsWriteService permissionsWriteService;

	/// <summary>
	/// Gets object providing access to repository for data of objects.
	/// </summary>
	protected new IGlossariesService Service => base.Service as IGlossariesService;

	protected new IWritableDatabaseObjectService WriteService => base.WriteService as IWritableDatabaseObjectService;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.GlossariesController" /> class for actions for glossary objects.
	/// </summary>
	/// <param name="repositoryAccessManager">The object providing getting object for accessing to repository.</param>
	public GlossariesController(IRepositoryAccessManager repositoryAccessManager)
		: base(repositoryAccessManager.GetRepository().GlossariesService, repositoryAccessManager.GetRepository().GlossariesWriteService)
	{
		permissionsService = repositoryAccessManager.GetRepository().PermissionService;
		permissionsWriteService = repositoryAccessManager.GetRepository().PermissionsWriteService;
	}

	/// <summary>
	/// Gets the list of basic information about glossaries.
	/// </summary>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <param name="parameters">The parameters providing selecting required set of data by skipping and limiting result items.</param>
	/// <returns>The basic information about object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(GlossaryDTO), 200)]
	[HttpGet]
	public virtual async Task<IActionResult> Get([FromQuery] GlossariesJoinsEnum.GlossariesJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<GlossaryFiltersEnum.GlossaryFilters> parameters = null)
	{
		return Ok(await Service.GetObjectsAsync(join, parameters));
	}

	[ProducesResponseType(typeof(GlossaryDTO), 200)]
	[ServiceFilter(typeof(UserViewPermissionAttribute))]
	[HttpGet("all")]
	public virtual async Task<IActionResult> Get()
	{
		return Ok(await Service.GetAllObjectsAsync());
	}

	[ProducesResponseType(typeof(GlossaryDTO), 200)]
	[HttpGet("all-permitted")]
	public virtual async Task<IActionResult> GetAllPermitted([FromQuery] OrderByEnum orderByCreationDate = OrderByEnum.Default, [FromQuery] OffsetLimitParameters parameters = null)
	{
		return Ok(await Service.GetAllPermittedObjectsAsync(orderByCreationDate, parameters));
	}

	[HttpGet("{id}/has-permission")]
	public async Task<IActionResult> HasPermission([Range(0, int.MaxValue)] int id, [FromQuery] RoleActionType.RoleAction roleAction)
	{
		return Ok(await Service.HasPermission(id, roleAction));
	}

	/// <summary>
	/// Gets the basic information about glossaries.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>The basic information about object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(GlossaryDTO), 200)]
	[ServiceFilter(typeof(GlossaryViewPermissionAttribute))]
	[HttpGet("{id}")]
	public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [FromQuery] GlossariesJoinsEnum.GlossariesJoins[] join = null)
	{
		return Ok(await Service.GetObject(id, join));
	}

	/// <summary>
	/// Gets the list of basic information about entries of glossary.
	/// </summary>
	/// <param name="id">The ID of glossary.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <param name="parameters">The parameters providing selecting required set of data by skipping, limiting, filtering result items.</param>
	/// <returns>The list of basic information about entries of glossary.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(GlossaryEntryDTO), 200)]
	[ServiceFilter(typeof(GlossaryViewPermissionAttribute))]
	[HttpGet("{id}/entries")]
	public virtual async Task<IActionResult> GetEntries([Range(0, int.MaxValue)] int id, [FromQuery] GlossaryEntriesJoinsEnum.GlossaryEntryJoins[] join = null, [FromQuery] OffsetLimitFilterParameters<GlossaryEntryFiltersEnum.GlossaryEntryFilters> parameters = null)
	{
		return Ok(await Service.GetEntries(id, join, parameters));
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
	[ServiceFilter(typeof(GlossaryViewPermissionAttribute))]
	[ServiceFilter(typeof(GlossaryCommunityViewPermissionAttribute))]
	[HttpGet("feedback/{id}")]
	public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [FromQuery] CollectionFeedbackParameters<FeedbackFiltersEnum.FeedbackFilters> parameters = null)
	{
		return Ok(await Service.GetFeedback(id, parameters));
	}

	[HttpGet("{id}/users-with-permissions")]
	[ServiceFilter(typeof(GlossaryUserViewPermissionAttribute))]
	public async Task<IActionResult> GetUsersAndGroupsDatabasePermissions(int id)
	{
		return Ok(await permissionsService.GetUsersAndGroupsGlossaryPermissions(id));
	}

	[ServiceFilter(typeof(GlossaryEditPermissionAttribute))]
	[HttpPost("{id}/put-title")]
	public async Task<IActionResult> UpdateTitle([FromForm][Required][StringLength(80)] string title, int id)
	{
		title = await WriteService.UpdateTitle(id, title);
		return Ok(title);
	}

	[ServiceFilter(typeof(GlossaryEditPermissionAttribute))]
	[HttpPost("{id}/put-description")]
	public async Task<IActionResult> UpdateDescription([FromForm] string description, int id)
	{
		return Ok(await WriteService.UpdateDescription(id, description));
	}

	[ServiceFilter(typeof(GlossaryEditPermissionAttribute))]
	public override Task<IActionResult> UpdateCustomField([FromForm] string fieldName, [FromForm] string value, int id)
	{
		return base.UpdateCustomField(fieldName, value, id);
	}

	/// <summary>
	/// Allow add new permission for specific glossary
	/// </summary>
	/// <param name="id">Glossary id</param>
	/// <param name="permission">Permission object to add</param>
	/// <returns></returns>
	[HttpPost("{id}/create-user-permission")]
	[ServiceFilter(typeof(GlossaryUserManagePermissionAttribute))]
	public async Task<IActionResult> CreateUserPermission(int id, [FromForm] PermissionCreateDTO permission)
	{
		return Ok(await permissionsWriteService.CreateUserPermission(permission));
	}

	/// <summary>
	/// Allow remove permission for specific glossary
	/// </summary>
	/// <param name="id">Glossary id</param>
	/// <param name="permissionId">Permission id to remove</param>
	/// <returns></returns>
	[HttpPost("{id}/delete-user-permission/{permissionId}")]
	[ServiceFilter(typeof(GlossaryUserManagePermissionAttribute))]
	public async Task<IActionResult> Delete(int id, int permissionId)
	{
		await permissionsWriteService.Delete(permissionId);
		return Ok();
	}
}
