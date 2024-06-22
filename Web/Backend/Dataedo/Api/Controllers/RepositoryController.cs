using System;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Threading.Tasks;
using Dataedo.Api.Configuration;
using Dataedo.Api.Enums;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Exceptions;
using Dataedo.Repository.Services.Features.Permissions.RoleActions.Enums;
using Dataedo.Repository.Services.Interfaces;
using Dataedo.Repository.Services.RepositoryAccess;
using Dataedo.Repository.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RepositoryController : ControllerBase
{
	/// <summary>
	/// Gets object providing access to repository for data of objects.
	/// </summary>
	private readonly IRepositoryService service;

	protected static string LastOperationName { get; private set; }

	protected static Exception LastOperationException { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.QuickSearchController" /> class for actions for searching objects.
	/// </summary>
	/// <param name="repositoryAccessManager">The object providing getting object for accessing to repository.</param>
	public RepositoryController(IRepositoryAccessManager repositoryAccessManager)
	{
		service = repositoryAccessManager.GetRepository().RepositoryService;
	}

	[Authorize]
	[HttpGet("/api/repository/has-permission")]
	public virtual async Task<IActionResult> HasPermission([FromQuery] RoleActionType.RoleAction roleAction)
	{
		return Ok(await service.HasPermission(roleAction));
	}

	[Authorize]
	[HttpGet("/api/repository/has-permission-with-status")]
	public virtual async Task<IActionResult> HasUsersViewPermission([FromQuery] RoleActionType.RoleAction roleAction)
	{
		return Ok(await service.HasPermissionWithStatus(roleAction));
	}

	[ProducesResponseType(typeof(string), 200)]
	[HttpGet("/api/repository/ping")]
	public virtual async Task<IActionResult> Ping()
	{
		RepositoryStatusEnum.Status status = RepositoryStatusEnum.Status.Active;
		return Ok(BaseEnumConversions<RepositoryStatusEnum.Status>.ToEnumString(status));
	}

	[ProducesResponseType(typeof(string), 200)]
	[HttpGet("/api/repository/identity")]
	public virtual async Task<IActionResult> ServerIdentity()
	{
		string user = WindowsIdentity.GetCurrent().Name;
		return Ok(user);
	}

	[HttpGet("/api/repository/name")]
	public async Task<IActionResult> RepositoryName()
	{
		return Ok(await service.GetRepositoryName());
	}

	[ProducesResponseType(typeof(void), 200)]
	[HttpGet("/api/repository")]
	public virtual async Task<IActionResult> Status()
	{
		RepositoryStatus repositoryStatus = new RepositoryStatus();
		RepositoryStatus repositoryStatus2 = repositoryStatus;
		repositoryStatus2.LocalServerName = await service.SearchLocalServerName();
		repositoryStatus.LastOperationName = LastOperationName;
		repositoryStatus.LastOperationErrorMessage = LastOperationException?.Message;
		RepositoryStatus status = repositoryStatus;
		LastOperationException = null;
		return Ok(status);
	}

	[ProducesResponseType(typeof(void), 202)]
	[HttpPost("/api/repository")]
	public virtual async Task<IActionResult> Create([FromForm] string serverName, [FromForm] int? port, [FromForm] bool windowsAuthentication, [FromForm] string login, [FromForm] string password, [FromForm] string database, [FromForm] bool dedicatedUser)
	{
		LastOperationName = "Create";
		RepositoryConnection connection = new RepositoryConnection
		{
			ServerName = serverName,
			Port = port,
			WindowsAuthentication = windowsAuthentication,
			Login = login,
			Password = password,
			Database = database
		};
		BackgroundProcessing.Instance.Start();
		Task.Run(async delegate
		{
			try
			{
				await service.CreateRepository(connection, dedicatedUser);
				LastOperationException = null;
			}
			catch (Exception ex)
			{
				Exception e = (LastOperationException = ex);
			}
			finally
			{
				BackgroundProcessing.Instance.Stop();
			}
		});
		return Accepted();
	}

	[ProducesResponseType(typeof(void), 202)]
	[HttpPost("/api/repository/upgrade")]
	public virtual async Task<IActionResult> Upgrade([FromForm] bool windowsAuthentication, [FromForm] string login, [FromForm] string password)
	{
		LastOperationName = "Upgrade";
		BackgroundProcessing.Instance.Start();
		Task.Run(async delegate
		{
			try
			{
				await service.UpgradeRepository(windowsAuthentication, login, password);
				LastOperationException = null;
			}
			catch (Exception ex)
			{
				Exception e = (LastOperationException = ex);
			}
			finally
			{
				BackgroundProcessing.Instance.Stop();
			}
		});
		return Accepted();
	}

	[ProducesResponseType(typeof(void), 200)]
	[ProducesResponseType(typeof(void), 409)]
	[HttpPost("/api/repository/set")]
	public virtual async Task<IActionResult> Set([FromForm] string serverName, [FromForm] int? port, [FromForm] bool windowsAuthentication, [FromForm] string login, [FromForm] string password, [FromForm] string database, [FromForm] bool dedicatedUser)
	{
		LastOperationName = "UseExisting";
		RepositoryConnection connection = new RepositoryConnection
		{
			ServerName = serverName,
			Port = port,
			WindowsAuthentication = windowsAuthentication,
			Login = login,
			Password = password,
			Database = database
		};
		try
		{
			BackgroundProcessing.Instance.Start();
			await service.SetRepository(connection, dedicatedUser);
		}
		catch (RepositoryNotSupportedException ex2)
		{
			BackgroundProcessing.Instance.Stop();
			return Conflict(ex2.Message);
		}
		catch (Exception ex)
		{
			BackgroundProcessing.Instance.Stop();
			throw ex;
		}
		BackgroundProcessing.Instance.Stop();
		return Ok();
	}

	[ProducesResponseType(typeof(bool), 200)]
	[ProducesResponseType(typeof(void), 409)]
	[HttpPost("/api/repository/first-account/check-login-exists")]
	public virtual async Task<IActionResult> AccountExists([FromForm] string login)
	{
		try
		{
			return Ok(await service.CheckLoginExists(login));
		}
		catch
		{
			return Conflict("Cannot check if login is available.");
		}
	}

	[ProducesResponseType(typeof(void), 200)]
	[ProducesResponseType(typeof(void), 409)]
	[HttpPost("/api/repository/first-account")]
	public virtual async Task<IActionResult> CreateFirstAccount([FromForm] bool windowsAuthentication, [FromForm] string login, [FromForm] string password)
	{
		bool flag = !windowsAuthentication;
		bool flag2 = flag;
		if (flag2)
		{
			flag2 = await service.IsIntegratedSecurityOnly();
		}
		if (flag2)
		{
			return Conflict("Signing in using password is disabled. To enable this feature open SSMS (SQL Server Management Studio), connect to repository, right click on server and click 'Properties'. Go to 'Security' tab, find the 'Server authentication' section and select 'SQL Server and Windows Authentication mode'.");
		}
		try
		{
			await service.CreateFirstAccount(windowsAuthentication, login, password);
			return Ok();
		}
		catch (WindowsAuthNameFormatException ex2)
		{
			return Conflict(ex2.Message);
		}
		catch (WindowsAuthNameNotFoundException ex3)
		{
			return Conflict(ex3.Message);
		}
		catch (SqlException ex)
		{
			return Conflict(ex.Message);
		}
		catch (RepositoryNotSupportedException)
		{
			return Conflict("Web Portal is not allowed to perform this operation. Try again or run Administration Console.");
		}
	}
}
