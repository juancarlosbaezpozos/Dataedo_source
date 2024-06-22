namespace Dataedo.Api.AppSettings;

/// <summary>
/// The class providing configuration for session management.
/// </summary>
public class JwtAuthentication
{
	/// <summary>
	/// Gets or sets the key value for generating key for securing session.
	/// </summary>
	public string TokensKey { get; set; }

	/// <summary>
	/// Gets or sets the valid issuer.
	/// </summary>
	public string ValidIssuer { get; set; }

	/// <summary>
	/// Gets or sets the valid audience.
	/// </summary>
	public string ValidAudience { get; set; }

	/// <summary>
	/// Gets or sets the number of seconds that access token is valid.
	/// </summary>
	public int TokenExpiresIn { get; set; }

	/// <summary>
	/// Gets or sets the number of seconds that refresh token is valid.
	/// </summary>
	public int RefreshTokenExpiresIn { get; set; }

	/// <summary>
	/// Gets or sets the number of seconds of clock skew.
	/// </summary>
	public int ClockSkew { get; set; }
}
