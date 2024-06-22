using System.Threading.Tasks;
using Dataedo.Api.Attributes.UsersView;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Permissions.Groups.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class GroupsController : ControllerBase
{
	private readonly IGroupsReadService service;

	public GroupsController(IRepositoryAccessManager repositoryAccessManager)
	{
		service = repositoryAccessManager.GetRepository().GroupsReadService;
	}

	[HttpGet]
	[ServiceFilter(typeof(UserViewPermissionAttribute))]
	public async Task<IActionResult> GetGroups([FromQuery] int? userId)
	{
		return Ok(await service.GetCollection(userId));
	}

	[HttpGet("{id}")]
	[ServiceFilter(typeof(UserViewPermissionAttribute))]
	public async Task<IActionResult> GetGroup(int id)
	{
		return Ok(await service.GetGroup(id));
	}
}
