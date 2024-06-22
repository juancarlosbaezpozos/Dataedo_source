using System.Threading.Tasks;
using Dataedo.Api.Attributes.UsersView;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class PermissionsController : ControllerBase
{
	private readonly IPermissionsReadService service;

	public PermissionsController(IRepositoryAccessManager repositoryAccessManager)
	{
		service = repositoryAccessManager.GetRepository().PermissionService;
	}

	[HttpGet("user-repository-permissions")]
	[ServiceFilter(typeof(AnyUserViewPermissionAttribute))]
	public async Task<IActionResult> GetUserRepositoryPermissions([FromQuery] int userId)
	{
		return Ok(await service.GetUserRepositoryPermissions(userId));
	}

	[HttpGet("group-repository-permissions")]
	[ServiceFilter(typeof(UserViewPermissionAttribute))]
	public async Task<IActionResult> GetGroupRepositoryPermissions([FromQuery] int groupId)
	{
		return Ok(await service.GetGroupRepositoryPermissions(groupId));
	}

	[HttpGet("user-data-source-permissions")]
	[ServiceFilter(typeof(AnyUserViewPermissionAttribute))]
	public async Task<IActionResult> GetUserDataSourcePermissions([FromQuery] int userId)
	{
		return Ok(await service.GetUserDataSourcePermissions(userId));
	}

	[HttpGet("group-data-source-permissions")]
	[ServiceFilter(typeof(UserViewPermissionAttribute))]
	public async Task<IActionResult> GetGroupDataSourcePermissions([FromQuery] int groupId)
	{
		return Ok(await service.GetGroupDataSourcePermissions(groupId));
	}
}
