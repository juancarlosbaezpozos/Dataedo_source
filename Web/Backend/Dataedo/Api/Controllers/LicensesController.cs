using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Dataedo.Api.Authorization.Users;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Exceptions;
using Dataedo.Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

/// <summary>
/// The class providing actions for table objects.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class LicensesController : ControllerBase
{
    /// <summary>
    /// The object providing access to HTTP context for getting currently logged user.
    /// </summary>
    private readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// Gets object providing access to repository for licenses.
    /// </summary>
    private readonly ILicensesService service;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.LicensesController" /> class for actions for licenses.
    /// </summary>
    /// <param name="repositoryAccessManager">The object providing getting object for accessing to repository.</param>
    /// <param name="httpContextAccessor">The object providing access to HTTP context for getting currently logged user.</param>
    public LicensesController(IRepositoryAccessManager repositoryAccessManager, IHttpContextAccessor httpContextAccessor)
    {
        service = repositoryAccessManager.GetRepository().LicensesService;
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Updates the key of license of logged in user.
    /// </summary>
    /// <param name="key">The license key to set in license record.</param>
    /// <response code="200">Successful operation.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">License record not found.</response>
    /// <response code="422">
    /// Key is invalid.
    ///
    /// <para>Reason is returned in validation-result header.</para>
    ///
    /// <para>Available values:</para>
    ///
    /// <list type="bullet">
    /// <item><para>"None" - reason not set;</para></item>
    /// <item><para>"Valid"- key is valid (should be never returned);</para></item>
    /// <item><para>"NoKey" - key is empty (not provided);</para></item>
    /// <item><para>"IncorrectLicenseKey" - key is not parsable;</para></item>
    /// <item><para>"Lite" - unsupported Lite edition;</para></item>
    /// <item><para>"VersionNotSupported" - license for earlier version;</para></item>
    /// <item><para>"TrialEnded" - expired trial license;</para></item>
    /// <item><para>"SubscriptionExpired" - expired subscription;</para></item>
    /// <item><para>"InsufficientLicenseLevel" - license is not valid for Web;</para></item>
    /// <item><para>"OtherError" - processing error occurred;</para></item>
    /// </list>
    /// </response>
    [ProducesResponseType(typeof(void), 200)]
    [HttpPatch("user")]
    public async Task<IActionResult> UpdateLicenseKey([Required][FromForm] string key)
    {
        UserData user = GetLoggedUser();
        await service.UpdateLicenseKey(user.Username, key);
        return Ok();
    }

    private UserData GetLoggedUser()
    {
        Claim identifier = httpContextAccessor?.HttpContext?.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        if (identifier == null)
        {
            throw new UnauthorizedException("User is not logged in.");
        }
        UserData user = UsersStorage.GetUser(identifier.Value);
        if (user == null)
        {
            throw new UnauthorizedException("User data not found.");
        }
        return user;
    }
}
