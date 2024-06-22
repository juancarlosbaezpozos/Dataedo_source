using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Dataedo.Api.AppSettings;
using Dataedo.Api.Authorization.JWT;
using Dataedo.Api.Authorization.Users;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository;
using Dataedo.Repository.EntityFrameworkModel.Models;
using Dataedo.Repository.Services.DTO.Users;
using Dataedo.Repository.Services.Exceptions;
using Dataedo.Repository.Services.Features.Permissions.DTO;
using Dataedo.Repository.Services.Features.Permissions.Tools;
using Dataedo.Repository.Services.Features.Permissions.UserGroups;
using Dataedo.Repository.Services.Features.Permissions.Users.Interfaces;
using Dataedo.Repository.Services.Parameters;
using Dataedo.Repository.Services.RepositoryAccess;
using Dataedo.Repository.Services.Services.CurrentUser;
using Dataedo.Repository.Services.SqlServer.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Dataedo.Api.Services;

/// <summary>
/// The class providing service for managing API sessions.
/// </summary>
/// <summary>
/// The class providing service for managing API sessions.
/// </summary>
public class UsersSessionsService : IUsersSessionsService
{
	private readonly IRepository repository;

	/// <summary>
	/// Gets object providing access to repository for data of objects.
	/// </summary>
	private readonly IUsersService service;

	/// <summary>
	/// The object providing access to HTTP context for getting currently logged user.
	/// </summary>
	private readonly IHttpContextAccessor httpContextAccessor;

	/// <summary>
	/// Gets the object providing application's configuration properties.
	/// </summary>
	private readonly IConfiguration configuration;

	/// <summary>
	/// The object providing configuration for JWT authentication management.
	/// </summary>
	private readonly JwtAuthentication jwtAuthentication;

	/// <summary>
	/// The connection string to server repository used when there is not logged user.
	/// </summary>
	private readonly DefaultConnection connectionStrings;

	/// <summary>
	/// Authenticates user: stores authenticated user data and creates session token.
	/// <para>Returns authentication token.</para>
	/// </summary>
	/// <param name="connectionData">The object used for building connection string.</param>
	/// <returns>The authentication token.</returns>
	public async Task<string> Login(RepositoryConnectionData connectionData)
	{
		if (!CanConnect(connectionData))
		{
			throw new UnauthorizedException("Authentication failed.");
		}
		if (!HasLicenseRecord(connectionData))
		{
			throw new UnauthorizedException("Authorization failed.");
		}
		Dataedo.Repository.EntityFrameworkModel.Models.Licenses license = GetLicense(connectionStrings.ConnectionString, connectionData.Login);
		string identifier = System.Guid.NewGuid().ToString();
		UserData userData = new UserData(identifier, connectionData.Login, RepositoryType.SqlServer, GetConnectionString(connectionData));
		UsersStorage.AddUser(identifier, userData);
		if (license == null)
		{
			throw new UnauthorizedException("User not found.");
		}
		return GenerateToken(userData, license.Login, license.LicenseId);
	}

	private bool CanConnect(RepositoryConnectionData connectionData)
	{
		return CanConnect(GetConnectionString(connectionData));
	}

	private bool HasLicenseRecord(RepositoryConnectionData connectionData)
	{
		Dataedo.Repository.EntityFrameworkModel.Models.Licenses license;
		return HasLicenseRecord(GetConnectionString(connectionData), connectionData.Login, out license);
	}

	private string GetConnectionString(RepositoryConnectionData connectionData)
	{
		SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
		if (!connectionData.Port.HasValue || connectionData.Port == 1433)
		{
			builder.DataSource = connectionData.ServerName;
		}
		else
		{
			builder.DataSource = $"{connectionData.ServerName},{connectionData.Port}";
		}
		builder.InitialCatalog = connectionData.Database;
		builder.UserID = connectionData.Login;
		builder.Password = connectionData.Password;
		return builder.ConnectionString;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Services.UsersSessionsService" /> class with session configuration.
	/// </summary>
	/// <param name="repositoryAccessManager">The object providing getting object for accessing to repository.</param>
	/// <param name="httpContextAccessor">The object providing access to HTTP context for getting currently logged user.</param>
	/// <param name="configuration">The object providing application's configuration properties.</param>
	/// <param name="jwtAuthentication">The object providing configuration for JWT authentication management.</param>
	/// <param name="defaultConnection">The configuration of default repository.</param>
	public UsersSessionsService(IRepositoryAccessManager repositoryAccessManager, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IOptions<JwtAuthentication> jwtAuthentication, IOptions<DefaultConnection> defaultConnection)
	{
		repository = repositoryAccessManager.GetRepository();
		service = repositoryAccessManager.GetRepository().UsersService;
		this.httpContextAccessor = httpContextAccessor;
		this.configuration = configuration;
		this.jwtAuthentication = jwtAuthentication.Value;
		connectionStrings = defaultConnection.Value;
	}

	/// <summary>
	/// Authenticates user: stores authenticated user data and creates session token.
	/// <para>Returns authentication token.</para>
	/// </summary>
	/// <param name="windowsAuthentication">The value indicating whether user should be authenticated as Windows user.</param>
	/// <param name="isLocalServer">The valie indicating whether server is local and integrated security should be used for connection.</param>
	/// <param name="username">The name of user.</param>
	/// <param name="password">The password of user.</param>
	/// <returns>The authentication tokens.</returns>
	public async Task<AuthenticationResultDTO> Login(bool windowsAuthentication, bool isLocalServer, string username, string password)
	{
		SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
		{
			ConnectionString = connectionStrings.ConnectionString
		};
		if (windowsAuthentication)
		{
			if (!CheckIfValidWindowsUser(username, password, isLocalServer))
			{
				throw new UnauthorizedException(InvalidCredentials(username));
			}
			if (isLocalServer && !ChcekIfPermittedSqlServerUser(builder.ConnectionString, username))
			{
				throw new UnauthorizedException("'" + username + "' is missing permissions in the repository. Assign the users role in the database to continue.");
			}
		}
		else
		{
			builder.UserID = username;
			builder.Password = password;
			builder.IntegratedSecurity = false;
			if (!CanConnect(builder.ConnectionString))
			{
				throw new UnauthorizedException("These credentials do not match.");
			}
		}
		Dataedo.Repository.EntityFrameworkModel.Models.Licenses license;
		bool hasLicenseRecord = HasLicenseRecord(builder.ConnectionString, username, out license);
		if ((license?.Deleted).GetValueOrDefault())
		{
			throw new UnauthorizedException("User " + username + " is deleted.");
		}
		if (!hasLicenseRecord)
		{
			await CreateLicenseRecord(builder.ConnectionString, username, builder.UserID);
			HasLicenseRecord(builder.ConnectionString, username, out license);
		}
		string identifier = GetIdentifier();
		UserData userData = new UserData(identifier, username, RepositoryType.SqlServer, builder.ConnectionString);
		AuthenticationResultDTO authenticationResult = PrepareAuthenticationResult(userData, license.Login, license.LicenseId);
		SetRefreshToken(userData, authenticationResult.RefreshToken);
		UsersStorage.AddUser(identifier, userData);
		UsersStorage.RemoveRecordsWithExpiredTokens();
		return authenticationResult;
	}

	private bool ValidateMachineContext(string username, string password)
	{
		try
		{
			using PrincipalContext context = new PrincipalContext(ContextType.Machine);
			return context.ValidateCredentials(username, password);
		}
		catch (Exception ex)
		{
			throw new UnauthorizedException(ex.Message);
		}
	}

	public async Task<AuthenticationResultDTO> SamlAuthentication(string username)
	{
		Dataedo.Repository.EntityFrameworkModel.Models.Licenses license;
		bool hasLicenseRecord = HasLicenseRecord(connectionStrings.ConnectionString, username, out license);
		if ((license?.Deleted).GetValueOrDefault())
		{
			throw new UnauthorizedException("User " + username + " is deleted.");
		}
		if (!hasLicenseRecord)
		{
			SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
			builder.ConnectionString = connectionStrings.ConnectionString;
			license = await CreateLicenseRecord(connectionStrings.ConnectionString, username, builder.UserID);
		}
		string identifier = GetIdentifier();
		UserData userData = new UserData(identifier, username, RepositoryType.SqlServer, connectionStrings.ConnectionString);
		AuthenticationResultDTO authenticationResult = PrepareAuthenticationResult(userData, username, license.LicenseId);
		SetRefreshToken(userData, authenticationResult.RefreshToken);
		UsersStorage.AddUser(identifier, userData);
		UsersStorage.RemoveRecordsWithExpiredTokens();
		return authenticationResult;
	}

	public AuthenticationResultDTO RefreshToken(string token, string refreshToken)
	{
		if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
		{
			throw new SecurityTokenException("Invalid token.");
		}
		ClaimsPrincipal principal = GetExpiredTokenPrincipal(token);
		string username = principal.Identity.Name;
		string identity = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
		UserData userData = UsersStorage.GetUser(identity);
		if (userData == null || userData.Username != username || userData.RefreshToken != refreshToken)
		{
			throw new SecurityTokenException("Invalid token.");
		}
		if (userData.IsRefreshTokenExpired)
		{
			UsersStorage.RemoveUser(userData.Identifier);
			throw new UnauthorizedException("Token expired.");
		}
		UsersStorage.RemoveUser(userData.Identifier);
		Dataedo.Repository.EntityFrameworkModel.Models.Licenses license = GetLicense(connectionStrings.ConnectionString, username);
		if (license == null)
		{
			throw new UnauthorizedException("User not found.");
		}
		string identifier = (userData.Identifier = GetIdentifier());
		AuthenticationResultDTO authenticationResult = PrepareAuthenticationResult(userData, license.Login, license.LicenseId);
		SetRefreshToken(userData, authenticationResult.RefreshToken);
		UsersStorage.AddUser(userData.Identifier, userData);
		UsersStorage.RemoveRecordsWithExpiredTokens();
		return authenticationResult;
	}

	/// <summary>
	/// Removes stored user session information of currently logged user.
	/// </summary>
	public void Logout()
	{
		Claim identifier = httpContextAccessor?.HttpContext?.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
		UsersStorage.RemoveUser(identifier.Value);
		UsersStorage.RemoveRecordsWithExpiredTokens();
	}

	private AuthenticationResultDTO PrepareAuthenticationResult(UserData userData, string login, int licenseId)
	{
		AuthenticationResultDTO authenticationResult = new AuthenticationResultDTO();
		authenticationResult.Token = GenerateToken(userData, login, licenseId);
		authenticationResult.RefreshToken = GenerateRefreshToken();
		return authenticationResult;
	}

	private string GetIdentifier()
	{
		string identifier;
		do
		{
			identifier = System.Guid.NewGuid().ToString();
		}
		while (UsersStorage.CheckIfUserExists(identifier));
		return identifier;
	}

	private void SetRefreshToken(UserData userData, string refreshToken)
	{
		userData.SetRefreshToken(refreshToken, DateTime.UtcNow.AddSeconds(jwtAuthentication.RefreshTokenExpiresIn));
	}

	private ClaimsPrincipal GetExpiredTokenPrincipal(string token)
	{
		JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
		SecurityToken securityToken;
		ClaimsPrincipal principal = tokenHandler.ValidateToken(token, Tokens.GetTokenValidationParameters(configuration, validateLifetime: false), out securityToken);
		if (!(securityToken is JwtSecurityToken jwtSecurityToken) || jwtSecurityToken.Header.Alg.Equals("http://www.w3.org/2001/04/xmldsig-more#hmac-sha256", StringComparison.InvariantCultureIgnoreCase))
		{
			throw new SecurityTokenException("Invalid token.");
		}
		return principal;
	}

	private string GenerateToken(UserData userData, string login, int id)
	{
		JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
		byte[] key = Tokens.GetTokensKey(configuration);
		List<Claim> claims = new List<Claim>
		{
			new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", userData.Identifier),
			new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", login),
			new Claim("id", id.ToString())
		};
		SecurityTokenDescriptor tokenDescriptior = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims.ToArray()),
			Expires = DateTime.UtcNow.AddSeconds(jwtAuthentication.TokenExpiresIn),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256")
		};
		SecurityToken token = tokenHandler.CreateToken(tokenDescriptior);
		return tokenHandler.WriteToken(token);
	}

	private string GenerateRefreshToken()
	{
		byte[] randomBytes = new byte[64];
		using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
		randomNumberGenerator.GetBytes(randomBytes);
		return Convert.ToBase64String(randomBytes);
	}

	private bool CanConnect(string connectionString)
	{
		try
		{
			new CustomRepositoryContext(connectionString).Licenses.FirstOrDefault((Dataedo.Repository.EntityFrameworkModel.Models.Licenses x) => x.Deleted != (bool?)true);
			return true;
		}
		catch (Exception ex)
		{
			throw new UnauthorizedException("Authentication failed." + Environment.NewLine + ex.Message);
		}
	}

	private bool HasLicenseRecord(string connectionString, string username, out Dataedo.Repository.EntityFrameworkModel.Models.Licenses license)
	{
		try
		{
			license = GetLicense(connectionString, username);
			return license != null;
		}
		catch
		{
			throw new UnauthorizedException("Authorization failed.");
		}
	}

	private async Task<Dataedo.Repository.EntityFrameworkModel.Models.Licenses> CreateLicenseRecord(string connectionString, string username, string connectionStringUserId)
	{
		CustomRepositoryContext context = new CustomRepositoryContext(connectionString, new CurrentUserService(connectionStringUserId, -1), ProductVersion.Version);
		Dataedo.Repository.EntityFrameworkModel.Models.Licenses license = new Dataedo.Repository.EntityFrameworkModel.Models.Licenses
		{
			Login = username?.Trim(),
			Key = string.Empty,
			CreationDate = DateTime.Now,
			LastModificationDate = DateTime.Now,
			IsOffline = true
		};
		List<int> defaultGroups = await repository.GroupsQueryService.GetDefaultGroupIds().ToListAsync();
		if (defaultGroups != null && defaultGroups.Count > 0)
		{
			foreach (int groupId in defaultGroups)
			{
				license.UsersUserGroups.Add(UserGroupMapper.GetUserGroup(groupId));
			}
		}
		context.Licenses.Add(license);
		await context.SaveChangesAsync();
		await CreateFirstUserInitialPermissions(context, license);
		return license;
	}

	private async Task CreateFirstUserInitialPermissions(CustomRepositoryContext context, Dataedo.Repository.EntityFrameworkModel.Models.Licenses license)
	{
		if (await context.Sessions.Where((Sessions x) => x.Product.Equals("WEB") && x.Status.Equals("OK")).CountAsync() != 0)
		{
			return;
		}
		List<int> roleIds = await repository.RolesQueryService.GetRolesIds().ToListAsync();
		List<Permissions> firstUserPermissions = new List<Permissions>();
		foreach (int role in roleIds)
		{
			PermissionCreateDTO input = new PermissionCreateDTO();
			input.EntityId = license.LicenseId;
			input.ObjectType = "REPOSITORY";
			input.RoleId = role;
			firstUserPermissions.Add(PermissionMapper.MapUserPermission(input));
		}
		if (firstUserPermissions != null && firstUserPermissions.Count() > 0)
		{
			context.Permissions.AddRange(firstUserPermissions);
			await context.SaveChangesAsync();
		}
	}

	private string InvalidCredentials(string userName)
	{
		return "Authentication failed for '" + userName + "'.";
	}

	private Dataedo.Repository.EntityFrameworkModel.Models.Licenses GetLicense(string connectionString, string username)
	{
		return new CustomRepositoryContext(connectionString).Licenses.FirstOrDefault((Dataedo.Repository.EntityFrameworkModel.Models.Licenses x) => x.Login == username);
	}

	private string GetDomain(string usernameDomain)
	{
		int separatorPositon = usernameDomain.IndexOf("\\");
		if (separatorPositon <= -1)
		{
			return string.Empty;
		}
		return usernameDomain.Substring(0, separatorPositon);
	}

	private string GetUsername(string usernameDomain)
	{
		int separatorPositon = usernameDomain.IndexOf("\\");
		if (separatorPositon <= -1)
		{
			return string.Empty;
		}
		return usernameDomain.Substring(separatorPositon + 1, usernameDomain.Length - separatorPositon - 1);
	}

	private bool ChcekIfPermittedSqlServerUser(string connectionString, string username)
	{
		using SqlConnection connection = new SqlConnection(connectionString);
		connection.Open();
		using SqlCommand checkUserCommand = new SqlCommand("SELECT SUSER_ID (@username)", connection);
		checkUserCommand.Parameters.Add("username", SqlDbType.NVarChar).Value = username;
		object result = checkUserCommand.ExecuteScalar();
		return result != null && result.GetType() == typeof(int) && (int)result > 0;
	}

	private bool CheckIfValidWindowsUser(string username, string password, bool isLocalServer)
	{
		string domainFromUsername = GetDomain(username);
		string usernameFromUsername = GetUsername(username);
		if (string.IsNullOrEmpty(domainFromUsername) || string.IsNullOrEmpty(usernameFromUsername))
		{
			throw new WindowsAuthNameFormatException(username, "The '" + username + "' is not a valid Windows Authentication name." + Environment.NewLine + "Give the complete name: <domain\\username>.");
		}
		try
		{
			if (isLocalServer)
			{
				return ValidateMachineContext(username, password);
			}
			using PrincipalContext context = new PrincipalContext(ContextType.Domain, domainFromUsername);
			if (!context.ValidateCredentials(username, password))
			{
				return ValidateMachineContext(username, password);
			}
		}
		catch (System.DirectoryServices.AccountManagement.PrincipalServerDownException)
		{
			throw new Dataedo.Repository.Services.Exceptions.PrincipalServerDownException(domainFromUsername, "The domain '" + domainFromUsername + "' could not be contacted. Check domain name again.");
		}
		catch
		{
			throw;
		}
		return false;
	}
}
