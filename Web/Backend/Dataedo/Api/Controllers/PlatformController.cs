using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Dataedo.Repository.Services.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformController : ControllerBase
{
    [ProducesResponseType(typeof(PlatformInfoDTO), 200)]
    [HttpGet("info")]
    public virtual async Task<IActionResult> Get()
    {
        PlatformInfoDTO result = new PlatformInfoDTO();
        result.WindowsAuthSupport = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        return Ok(result);
    }
}
