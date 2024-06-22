using System;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Dataedo.Api.AppSettings;
using Dataedo.Api.Attributes.Configuration;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Api.Services;
using Dataedo.Repository.Services.DTO.Users;
using Dataedo.Repository.Services.Exceptions;
using Dataedo.Repository.Services.Interfaces;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Dataedo.Api.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly Saml2Configuration configuration;

	private readonly IUsersSessionsService usersSessionsService;

	private readonly Saml2Client saml2Client;

	private readonly ISessionsService sessionsService;

	public AuthController(IOptions<Saml2Configuration> configAccessor, IUsersSessionsService usersSessionsService, IOptions<Saml2Client> saml2ClientConfigAccessor, IRepositoryAccessManager repositoryAccessManager)
	{
		configuration = configAccessor.Value;
		this.usersSessionsService = usersSessionsService;
		saml2Client = saml2ClientConfigAccessor.Value;
		sessionsService = repositoryAccessManager.GetRepository().SessionsService;
	}

	[HttpGet("login")]
	[ServiceFilter(typeof(SSOLoginMethodAttribute))]
	public IActionResult Login()
	{
		Saml2RedirectBinding binding = new Saml2RedirectBinding();
		binding.Bind(new Saml2AuthnRequest(configuration));
		return Ok(binding.RedirectLocation);
	}

	[HttpPost("assertion-consumer")]
	public async Task<IActionResult> AssertionConsumerService()
	{
		string name = string.Empty;
		try
		{
			Saml2PostBinding binding = new Saml2PostBinding();
			Saml2AuthnResponse saml2AuthnResponse = new Saml2AuthnResponse(configuration);
			binding.ReadSamlResponse(base.Request.ToGenericHttpRequest(), saml2AuthnResponse);
			name = saml2AuthnResponse.ClaimsIdentity.Name;
			if (saml2AuthnResponse.Status != 0)
			{
				await sessionsService.Insert(name, "SAML", isCorrect: false, base.Request.HttpContext.Connection.RemoteIpAddress.ToString(), string.Empty, ProductVersion.Version);
				throw new AuthenticationException($"SAML Resposne status: {saml2AuthnResponse.Status}");
			}
			binding.Unbind(base.Request.ToGenericHttpRequest(), saml2AuthnResponse);
			await saml2AuthnResponse.CreateSession(base.HttpContext, null, isPersistent: false, (ClaimsPrincipal claimsTransform) => ClaimsTransform.Transform(claimsTransform));
			AuthenticationResultDTO result = await usersSessionsService.SamlAuthentication(name);
			await sessionsService.Insert(name, "SAML", isCorrect: true, base.Request.HttpContext.Connection.RemoteIpAddress.ToString(), string.Empty, ProductVersion.Version);
			return Redirect(saml2Client.ClientUrl + "/saml?login=" + name + "&token=" + result.Token + "&refreshToken=" + result.RefreshToken);
		}
		catch (UnauthorizedException ex2)
		{
			await sessionsService.Insert(name, "SAML", isCorrect: false, base.Request.HttpContext.Connection.RemoteIpAddress.ToString(), string.Empty, ProductVersion.Version);
			return Redirect(saml2Client.ClientUrl + "?saml_error=" + ex2.Message);
		}
		catch (Exception ex)
		{
			await sessionsService.Insert(string.Empty, "SAML", isCorrect: false, base.Request.HttpContext.Connection.RemoteIpAddress.ToString(), string.Empty, ProductVersion.Version);
			throw ex;
		}
	}

	[HttpPost("logout")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Logout()
	{
		if (!base.User.Identity.IsAuthenticated)
		{
			return Ok();
		}
		await new Saml2LogoutRequest(configuration, base.User).DeleteSession(base.HttpContext);
		return Ok();
	}
}
