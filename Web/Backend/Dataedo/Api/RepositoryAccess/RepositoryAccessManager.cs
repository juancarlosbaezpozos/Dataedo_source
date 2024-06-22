using System.Security.Claims;
using Dataedo.Api.AppSettings;
using Dataedo.Api.Authorization.Users;
using Dataedo.Api.Controllers;
using Dataedo.Api.Services;
using Dataedo.Repository;
using Dataedo.Repository.Services.Features.Notifications.Interfaces;
using Dataedo.Repository.Services.RepositoryAccess;
using Dataedo.Repository.Services.Services;
using Dataedo.Repository.Services.Services.CurrentUser;
using Dataedo.Repository.Services.SqlServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dataedo.Api.RepositoryAccess;

/// <summary>
/// The class providing managing access to repository for currently logged user.
/// </summary>
public class RepositoryAccessManager : IRepositoryAccessManager
{
	/// <summary>
	/// The object providing access to HTTP context for getting currently logged user.
	/// </summary>
	private readonly IHttpContextAccessor httpContextAccessor;

	/// <summary>
	/// The object providing configuring logging system.
	/// </summary>
	private readonly ILoggerFactory loggerFactory;

	private readonly INotificationService notificationService;

	private readonly ConfigurationService configurationService;

	private readonly ClassService classService;

	private readonly TypeService typeService;

	private readonly DbmsService dbmsService;

	/// <summary>
	/// The connection string to server repository used when there is not logged user.
	/// </summary>
	private readonly DefaultConnection connectionStrings;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.RepositoryAccess.RepositoryAccessManager" /> class.
	/// </summary>
	/// <param name="httpContextAccessor">The object providing access to HTTP context for getting currently logged user.</param>
	/// <param name="loggerFactory">The object providing configuring logging system.</param>
	/// <param name="defaultConnection">The configuration of default repository.</param>
	public RepositoryAccessManager(IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory, IOptionsSnapshot<DefaultConnection> defaultConnection, INotificationService notificationService, ConfigurationService configurationService, ClassService classService, TypeService typeService, DbmsService dbmsService)
	{
		this.httpContextAccessor = httpContextAccessor;
		this.loggerFactory = loggerFactory;
		this.notificationService = notificationService;
		this.configurationService = configurationService;
		this.classService = classService;
		this.typeService = typeService;
		this.dbmsService = dbmsService;
		connectionStrings = defaultConnection.Value;
	}

	/// <summary>
	/// Gets the object providing repository services.
	/// <para>If user is not confirmed as logged then default connection is used.</para>
	/// </summary>
	/// <returns>The object providing repository services.</returns>
	public IRepository GetRepository()
	{
		CurrentUserService currentUserService = null;
		Claim identifier = httpContextAccessor?.HttpContext?.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
		if (identifier != null)
		{
			UserData user = UsersStorage.GetUser(identifier?.Value);
			IHttpContextAccessor obj = httpContextAccessor;
			if (obj != null && obj.HttpContext?.Request?.Headers.ContainsKey("Authorization") == true)
			{
				currentUserService = new CurrentUserService(JWTService.GetLogin(httpContextAccessor?.HttpContext?.Request), JWTService.GetId(httpContextAccessor?.HttpContext?.Request));
			}
			if (user != null)
			{
				IHttpContextAccessor obj2 = httpContextAccessor;
				if (obj2 != null && obj2.HttpContext?.Request?.Path.HasValue == true)
				{
					IHttpContextAccessor obj3 = httpContextAccessor;
					if (obj3 != null && obj3.HttpContext?.Request?.Path.Value.Equals(UsersController.LoginPath) == true)
					{
						return Dataedo.Repository.RepositoryAccess.Create(user.RepositoryType, user.ConnectionString, loggerFactory, currentUserService, notificationService, ProductVersion.Version, configurationService, classService, typeService, dbmsService);
					}
				}
			}
		}
		return Dataedo.Repository.RepositoryAccess.Create(RepositoryType.SqlServer, connectionStrings.ConnectionString, loggerFactory, currentUserService, notificationService, ProductVersion.Version, configurationService, classService, typeService, dbmsService);
	}
}
