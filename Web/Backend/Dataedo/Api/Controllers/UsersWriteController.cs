using System.Threading.Tasks;
using Dataedo.Api.Attributes.UsersView;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.Interfaces;
using Dataedo.Repository.Services.Features.Permissions.Users.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class UsersWriteController : ControllerBase
{
	private readonly IUsersWriteService service;

	public UsersWriteController(IRepositoryAccessManager repositoryAccessManager)
	{
		service = repositoryAccessManager.GetRepository().UsersWriteService;
	}

	[ServiceFilter(typeof(UserManagePermissionAttribute))]
	[HttpPost("create")]
	[Authorize]
	public async Task<IActionResult> CreateUser([FromForm] UserCreateDTO userCreateDTO)
	{
		return Ok(await service.Create(userCreateDTO));
	}

	[ServiceFilter(typeof(UserManagePermissionAttribute))]
	[HttpPost("update")]
	[Authorize]
	public async Task<IActionResult> UpdateUser([FromForm] UserUpdateDTO userUpdateDTO)
	{
		await service.Update(userUpdateDTO);
		return Ok();
	}

	[ServiceFilter(typeof(UserManagePermissionAttribute))]
	[HttpPost("{id}/delete")]
	[Authorize]
	public async Task<IActionResult> Delete(int id)
	{
		await service.Delete(id);
		return Ok();
	}

	[ServiceFilter(typeof(UserManagePermissionAttribute))]
	[HttpPost("{id}/restore")]
	[Authorize]
	public async Task<IActionResult> Restore(int id)
	{
		await service.Restore(id);
		return NoContent();
	}
}
