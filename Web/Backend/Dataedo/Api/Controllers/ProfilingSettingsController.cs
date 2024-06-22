using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Profiling.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
public class ProfilingSettingsController : ControllerBase
{
	private readonly IProfilingAccessibilityService service;

	public ProfilingSettingsController(IRepositoryAccessManager repositoryAccessManager)
	{
		service = repositoryAccessManager.GetRepository().ProfilingAccessibilityService;
	}

	[HttpGet("is-enabled")]
	public async Task<IActionResult> IsProfilingEnabled()
	{
		return Ok(await service.IsEnabled());
	}
}
