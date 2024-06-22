using System.Threading;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.SystemManagementSettings;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Configuration.DTO;
using Dataedo.Repository.Services.Features.Configuration.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/system-settings")]
[ApiController]
public class SystemSettingsController : ControllerBase
{
    private readonly ISecuritySettingsService securitySettingsService;

    public SystemSettingsController(IRepositoryAccessManager repositoryAccessManager)
    {
        securitySettingsService = repositoryAccessManager.GetRepository().SecuritySettingsService;
    }

    [HttpGet("security")]
    public async Task<IActionResult> GetSecuritySettings(CancellationToken cancellationToken)
    {
        return Ok(await securitySettingsService.GetAuthenticationSettings(cancellationToken));
    }

    [HttpPost("security")]
    [Authorize]
    [ServiceFilter(typeof(SystemManagementSettingsAttribute))]
    public async Task<IActionResult> PostSecuritySettings(PostAuthenticationDTO authenticationDTO, CancellationToken cancellationToken)
    {
        await securitySettingsService.SaveSecuritySettings(authenticationDTO, cancellationToken);
        return NoContent();
    }
}
