namespace Dataedo.Repository.Services.DTO.Users;

public class AuthenticationResultDTO
{
	/// <summary>
	/// The token used for authenticating user in API.
	/// </summary>
	public string Token { get; set; }

	/// <summary>
	/// The token used for getting new token without providing credentials.
	/// </summary>
	public string RefreshToken { get; set; }
}
