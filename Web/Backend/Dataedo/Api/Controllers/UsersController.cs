using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Threading.Tasks;
using Dataedo.Api.AppSettings;
using Dataedo.Api.Attributes.Configuration;
using Dataedo.Api.Attributes.UsersView;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Api.Services;
using Dataedo.Repository.Services.DTO;
using Dataedo.Repository.Services.DTO.Users;
using Dataedo.Repository.Services.Exceptions;
using Dataedo.Repository.Services.Features.Configuration.Enums;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Dataedo.Repository.Services.Features.Permissions.Users.Interfaces;
using Dataedo.Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Dataedo.Api.Controllers;

/// <summary>
/// The class providing actions for managing API sessions.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class UsersController : ControllerBase
{
    private const string controllerName = "users";

    private const string isAdminPath = "get-if-is-admin";

    private const string loginPath = "login";

    public static readonly string IsAdminPath = "/api/users/get-if-is-admin";

    public static readonly string LoginPath = "/api/users/login";

    /// <summary>
    /// The object providing service for managing API sessions.
    /// </summary>
    private readonly IUsersSessionsService usersSessionsService;

    /// <summary>
    /// Gets object providing access to repository for data of objects.
    /// </summary>
    private readonly IUsersService service;

    private readonly Saml2Client idPConfig;

    private readonly ISessionsService sessionsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.UsersController" /> class with service.
    /// </summary>
    /// <param name="usersSessionsService">The object providing service for managing API sessions.</param>
    /// <param name="repositoryAccessManager">The object providing getting object for accessing to repository.</param>
    public UsersController(IUsersSessionsService usersSessionsService, IRepositoryAccessManager repositoryAccessManager, IOptions<Saml2Client> saml2ClientConfigAccessor)
    {
        this.usersSessionsService = usersSessionsService;
        service = repositoryAccessManager.GetRepository().UsersService;
        idPConfig = saml2ClientConfigAccessor.Value;
        sessionsService = repositoryAccessManager.GetRepository().SessionsService;
    }

    [AllowAnonymous]
    [HttpGet("idp")]
    public IActionResult IdP()
    {
        return Ok(new IdpConfigDTO(idPConfig?.DisplayName));
    }

    /// <summary>
    /// Authenticates user, checks authorization and returns authentication token.
    /// </summary>
    /// <param name="windowsAuthentication">The value indicating whether user should be authenticated as Windows user.</param>
    /// <param name="isLocalServer">The valie indicating whether server is local and integrated security should be used for connection.</param>
    /// <param name="login">The name of user.</param>
    /// <param name="password">The password of user.</param>
    /// <returns>The authentication tokens.</returns>
    /// <response code="200">Successful operation.</response>
    /// <response code="401">Unauthorized.</response>
    [ProducesResponseType(typeof(AuthenticationResultDTO), 200)]
    [AllowAnonymous]
    [HttpPost("login")]
    [ServiceFilter(typeof(StandardLoginMethodAttribute))]
    public async Task<IActionResult> LoginUser([Required][FromForm] LoginMethodEnum.Enum loginMethod, [Required][FromForm] string login, [Required][FromForm] string password, [Required][FromForm] string userAgent)
    {
        bool isCorrect = false;
        try
        {
            IActionResult result;
            try
            {
                AuthenticationResultDTO authenticationResult = await usersSessionsService.Login(loginMethod == LoginMethodEnum.Enum.ActiveDirectory || loginMethod == LoginMethodEnum.Enum.LocalWindows, loginMethod == LoginMethodEnum.Enum.LocalWindows, login, password);
                isCorrect = true;
                result = Ok(authenticationResult);
            }
            catch (UnauthorizedException ex3)
            {
                result = Conflict("These credentials do not match." + Environment.NewLine + ex3.Message);
            }
            catch (WindowsAuthNameFormatException ex2)
            {
                result = Conflict(ex2.Message);
            }
            catch (PrincipalServerDownException ex)
            {
                result = Conflict(ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
        finally
        {
            await sessionsService.Insert(login, LoginMethodEnum.TypeToString(loginMethod), isCorrect, base.Request.HttpContext.Connection.RemoteIpAddress.ToString(), userAgent, ProductVersion.Version);
        }
    }

    [AllowAnonymous]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(404)]
    [HttpGet("/api/users/identity")]
    public virtual async Task<IActionResult> UserIdentity()
    {
        try
        {
            string user = base.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(WindowsIdentity.GetCurrent().Name))
            {
                user = WindowsIdentity.GetCurrent().Name.Split('\\').GetValue(0)!.ToString() + "\\";
            }
            return Ok(user);
        }
        catch (PlatformNotSupportedException)
        {
            return NotFound();
        }
    }

    [Authorize]
    [HttpGet("/api/users/{id}/permissions")]
    [ServiceFilter(typeof(UserViewPermissionAttribute))]
    public virtual async Task<IActionResult> GetUserPermissions(int id)
    {
        return Ok(await service.GetUserPermissions(id));
    }

    [HttpGet("/api/users")]
    [ServiceFilter(typeof(AnyUserViewPermissionAttribute))]
    [Authorize]
    public async Task<IActionResult> GetUsers(int? groupId = null)
    {
        return Ok(await service.GetUsers(groupId));
    }

    [HttpGet("/api/users/deleted")]
    [ServiceFilter(typeof(UserManagePermissionAttribute))]
    [Authorize]
    public async Task<IActionResult> GetDeletedUsers()
    {
        return Ok(await service.GetDeletedUsers());
    }

    [HttpGet("/api/users/{id}")]
    [ServiceFilter(typeof(UserViewPermissionAttribute))]
    [Authorize]
    public async Task<IActionResult> GetUser(int id)
    {
        return Ok(await service.GetUser(id));
    }

    [HttpGet("/api/users/current-user")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        return Ok(await service.GetCurrentUser());
    }

    /// <summary>
    /// Checks refresh token and returns new tokens
    /// </summary>
    /// <param name="token">The authentication token.</param>
    /// <param name="refreshToken">The token used for refreshing authentication token.</param>
    /// <returns>The authentication tokens.</returns>
    /// <response code="200">Successful operation.</response>
    /// <response code="401">Unauthorized.</response>
    [ProducesResponseType(typeof(AuthenticationResultDTO), 200)]
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public IActionResult RefreshToken([Required][FromForm] string token, [Required][FromForm] string refreshToken)
    {
        AuthenticationResultDTO authenticationResult = usersSessionsService.RefreshToken(token, refreshToken);
        return Ok(authenticationResult);
    }

    /// <summary>
    /// Removes stored user session information of currently logged user.
    /// </summary>
    /// <returns>The empty result.</returns>
    /// <response code="200">Successful operation.</response>
    /// <response code="401">Unauthorized.</response>
    [ProducesResponseType(typeof(AuthenticationResultDTO), 200)]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        usersSessionsService.Logout();
        return Ok();
    }

    /// <summary>
    /// Gets whether user with provided login is an administrator.
    /// </summary>
    /// <param name="userName">&gt;The login.</param>
    /// <returns>The value indicating whether user with provided login is an administrator.</returns>
    [ProducesResponseType(typeof(bool), 200)]
    [Authorize]
    [HttpPost("get-if-is-admin")]
    public async Task<IActionResult> GetIfIsAdmin([Required][FromForm] string userName)
    {
        return Ok(await service.IsAdmin(userName));
    }

    [ProducesResponseType(typeof(bool), 202)]
    [Authorize]
    [HttpGet("/api/users/has-any-permission-to-action")]
    public virtual async Task<IActionResult> HasAnyPermissionToAction([FromQuery] RoleActionType.RoleAction roleAction)
    {
        return Ok(await service.HasAnyPermissionToAction(roleAction));
    }

    [ProducesResponseType(typeof(bool), 202)]
    [Authorize]
    [HttpGet("/api/users/has-repository-permission-to-action")]
    public virtual async Task<IActionResult> HasRepositoryPermissionToAction([FromQuery] RoleActionType.RoleAction roleAction)
    {
        return Ok(await service.HasRepositoryPermisionToAction(roleAction));
    }
}
