using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.DocumentationViewPermission;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Api.Services;
using Dataedo.Repository.Services.Features.Following.DTO;
using Dataedo.Repository.Services.Features.Following.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FollowingController : ControllerBase
{
	private readonly IFollowingWriteService followingWriteService;

	private readonly IFollowingService followingService;

	public FollowingController(IRepositoryAccessManager repositoryAccessManager)
	{
		followingWriteService = repositoryAccessManager.GetRepository().FollowingWriteService;
		followingService = repositoryAccessManager.GetRepository().FollowingService;
	}

	[ServiceFilter(typeof(FollowingObjectDocumentationViewAttribute))]
	[HttpPost("attach")]
	public async Task<IActionResult> Attach([FromBody] FollowingActionDTO model)
	{
		await followingWriteService.Attach(JWTService.GetLogin(base.Request), model.FollowingObjectType, model.ObjectId);
		return Ok();
	}

	[ServiceFilter(typeof(FollowingObjectDocumentationViewAttribute))]
	[HttpPost("detach")]
	public async Task<IActionResult> Detach([FromBody] FollowingActionDTO model)
	{
		await followingWriteService.Detach(JWTService.GetLogin(base.Request), model.FollowingObjectType, model.ObjectId);
		return Ok();
	}

	[ServiceFilter(typeof(FollowingObjectDocumentationViewAttribute))]
	[HttpGet("is-following")]
	public async Task<IActionResult> IsFollowing([Required][FromQuery] FollowingActionDTO model)
	{
		return Ok(await followingService.IsFollowing(JWTService.GetLogin(base.Request), model.FollowingObjectType, model.ObjectId));
	}

	[ServiceFilter(typeof(FollowingObjectDocumentationViewAttribute))]
	[HttpGet("followers")]
	public async Task<IActionResult> GetFollowers([Required][FromQuery] FollowingActionDTO model)
	{
		return Ok(await followingService.GetFollowers(model.ObjectId, model.FollowingObjectType));
	}
}
