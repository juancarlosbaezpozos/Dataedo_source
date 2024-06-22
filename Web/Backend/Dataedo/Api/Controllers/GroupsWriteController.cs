using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.UsersView;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.Groups.DTO;
using Dataedo.Repository.Services.Features.Permissions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class GroupsWriteController : ControllerBase
{
	private readonly IManagementObjectWriteService<GroupCreateDTO, GroupUpdateDTO> service;

	public GroupsWriteController(IRepositoryAccessManager repositoryAccessManager)
	{
		service = repositoryAccessManager.GetRepository().GroupsWriteService;
	}

	[ServiceFilter(typeof(UserManagePermissionAttribute))]
	[HttpPost("create")]
	public async Task<IActionResult> Create([FromForm] GroupCreateDTO group)
	{
		return Ok(await service.Create(group));
	}

	[ServiceFilter(typeof(UserManagePermissionAttribute))]
	[HttpPost("update")]
	public async Task<IActionResult> Update([FromForm] GroupUpdateDTO group)
	{
		await service.Update(group);
		return Ok();
	}

	[ServiceFilter(typeof(UserManagePermissionAttribute))]
	[HttpPost("{id}/delete")]
	public async Task<IActionResult> Update([Range(0, int.MaxValue)] int id)
	{
		await service.Delete(id);
		return Ok();
	}
}
