using System.Threading.Tasks;
using Dataedo.Api.Attributes.UsersView;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.Roles.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class RolesController : ControllerBase
{
	private readonly IRolesReadService service;

	public RolesController(IRepositoryAccessManager repositoryAccessManager)
	{
		service = repositoryAccessManager.GetRepository().RolesReadService;
	}

	[HttpGet]
	[ServiceFilter(typeof(AnyUserViewPermissionAttribute))]
	public async Task<IActionResult> GetRoles()
	{
		return Ok(await service.GetCollection());
	}

	[HttpGet("{id}/counters")]
	public async Task<IActionResult> GetCounters(int id)
	{
		return Ok(await service.GetUsersAndGroupsCount(id));
	}
}
