using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class RoleActionsController : ControllerBase
{
	private readonly IRoleActionsReadService service;

	public RoleActionsController(IRepositoryAccessManager repositoryAccessManager)
	{
		service = repositoryAccessManager.GetRepository().RoleActionsReadService;
	}

	[HttpGet("with-role-context")]
	public async Task<IActionResult> GetContextedRoleActions([Range(0, int.MaxValue)][FromQuery] int? roleId = null)
	{
		return Ok(await service.GetContextedRoleActions(roleId));
	}
}
