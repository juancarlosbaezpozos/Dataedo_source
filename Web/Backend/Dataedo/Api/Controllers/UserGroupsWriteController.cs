using System.Collections.Generic;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.UsersView;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.UserGroups.DTO;
using Dataedo.Repository.Services.Features.Permissions.UserGroups.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class UserGroupsWriteController : ControllerBase
{
	private readonly IUsersGroupWriteService service;

	public UserGroupsWriteController(IRepositoryAccessManager repositoryAccessManager)
	{
		service = repositoryAccessManager.GetRepository().UserGroupWriteService;
	}

	[ServiceFilter(typeof(UserManagePermissionAttribute))]
	[HttpPost("create")]
	public async Task<IActionResult> Create([FromForm] UserGroupInputDTO userGroupCreateDTO)
	{
		return Ok(await service.Create(userGroupCreateDTO));
	}

	[ServiceFilter(typeof(UserManagePermissionAttribute))]
	[HttpPost("update")]
	public async Task<IActionResult> Update([FromForm] int userId, [FromForm] IEnumerable<int> ids = null)
	{
		await service.Update(userId, ids);
		return Ok();
	}

	[ServiceFilter(typeof(UserManagePermissionAttribute))]
	[HttpPost("delete")]
	public async Task<IActionResult> Delete([FromForm] UserGroupInputDTO input)
	{
		await service.Delete(input);
		return Ok();
	}
}
