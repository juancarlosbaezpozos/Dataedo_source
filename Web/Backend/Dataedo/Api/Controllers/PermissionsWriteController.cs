using System.Threading.Tasks;
using Dataedo.Api.Attributes.UsersView;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.DTO;
using Dataedo.Repository.Services.Features.Permissions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class PermissionsWriteController : ControllerBase
{
	private readonly IPermissionsWriteService service;

	public PermissionsWriteController(IRepositoryAccessManager repositoryAccessManager)
	{
		service = repositoryAccessManager.GetRepository().PermissionsWriteService;
	}

	[HttpPost("create-user-permission")]
	[ServiceFilter(typeof(UserManagePermissionAttribute))]
	public async Task<IActionResult> CreateUserPermission([FromForm] PermissionCreateDTO permission)
	{
		return Ok(await service.CreateUserPermission(permission));
	}

	[HttpPost("create-group-permission")]
	[ServiceFilter(typeof(UserManagePermissionAttribute))]
	public async Task<IActionResult> CreateGroupPermission([FromForm] PermissionCreateDTO permission)
	{
		return Ok(await service.CreateGroupPermission(permission));
	}

	[HttpPost("{id}/delete")]
	[ServiceFilter(typeof(UserManagePermissionAttribute))]
	public async Task<IActionResult> Delete(int id)
	{
		await service.Delete(id);
		return Ok();
	}
}
