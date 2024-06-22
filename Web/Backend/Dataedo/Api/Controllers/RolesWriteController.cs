using System.Threading.Tasks;
using Dataedo.Api.Attributes.UsersView;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.Interfaces;
using Dataedo.Repository.Services.Features.Permissions.Roles.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class RolesWriteController : ControllerBase
{
	private readonly IManagementObjectWriteService<RoleCreateDTO, RoleUpdateDTO> rolesWriteService;

	public RolesWriteController(IRepositoryAccessManager repositoryAccessManager)
	{
		rolesWriteService = repositoryAccessManager.GetRepository().RolesWriteService;
	}

	[ServiceFilter(typeof(UserManagePermissionAttribute))]
	[HttpPost("create")]
	public async Task<IActionResult> CreateRole([FromForm] RoleCreateDTO roleCreateDTO)
	{
		return Ok(await rolesWriteService.Create(roleCreateDTO));
	}

	[ServiceFilter(typeof(UserManagePermissionAttribute))]
	[HttpPost("update")]
	public async Task<IActionResult> UpdateRole([FromForm] RoleUpdateDTO roleCreateDTO)
	{
		await rolesWriteService.Update(roleCreateDTO);
		return Ok();
	}

	[ServiceFilter(typeof(UserManagePermissionAttribute))]
	[HttpPost("{id}/delete")]
	public async Task<IActionResult> Delete(int id)
	{
		await rolesWriteService.Delete(id);
		return Ok();
	}
}
