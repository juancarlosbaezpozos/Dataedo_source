using System.Threading.Tasks;
using Dataedo.Repository.Services.DTO.Users;
using Dataedo.Repository.Services.Parameters;

namespace Dataedo.Api.Services;

/// <summary>
/// Represents object providing service for managing API sessions.
/// </summary>
public interface IUsersSessionsService
{
	/// <summary>
	/// Authenticates user: stores authenticated user data and creates session token.
	/// <para>Returns authentication token.</para>
	/// </summary>
	/// <param name="connectionData">The object used for building connection string.</param>
	/// <returns>The authentication token.</returns>
	Task<string> Login(RepositoryConnectionData connectionData);

	/// <summary>
	/// Authenticates user: stores authenticated user data and creates session token.
	/// <para>Returns authentication token.</para>
	/// </summary>
	/// <param name="windowsAuthentication">The value indicating whether user should be authenticated as Windows user.</param>
	/// <param name="isLocalServer">The valie indicating whether server is local and integrated security should be used for connection.</param>
	/// <param name="username">The name of user.</param>
	/// <param name="password">The password of user.</param>
	/// <returns>The authentication token.</returns>
	Task<AuthenticationResultDTO> Login(bool windowsAuthentication, bool isLocalServer, string username, string password);

	AuthenticationResultDTO RefreshToken(string token, string refreshToken);

	/// <summary>
	/// Removes stored user session information of currently logged user.
	/// </summary>
	void Logout();

	Task<AuthenticationResultDTO> SamlAuthentication(string username);
}
