using System;
using System.Text;
using Dataedo.Api.AppSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Dataedo.Api.Authorization.JWT;

/// <summary>
/// The class providing configuration for tokens support.
/// </summary>
public static class Tokens
{
	/// <summary>
	/// The algorithm used for signing credentials.
	/// </summary>
	public const string SecurityAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";

	/// <summary>
	/// Gets the object providing configuration for JWT authentication management.
	/// </summary>
	/// <param name="configuration">The object providing application's configuration properties</param>
	/// <returns>The object providing configuration for JWT authentication management.</returns>
	public static JwtAuthentication GetJwtAuthentication(IConfiguration configuration)
	{
		return configuration.GetSection("JwtAuthentication").Get<JwtAuthentication>();
	}

	/// <summary>
	/// Gets the key for signing credentials.
	/// </summary>
	/// <param name="configuration">The object providing application's configuration properties.</param>
	/// <returns>The key for signing credentials as byte array.</returns>
	public static byte[] GetTokensKey(IConfiguration configuration)
	{
		return Encoding.ASCII.GetBytes(GetJwtAuthentication(configuration).TokensKey);
	}

	/// <summary>
	/// Gets the token validation parameters for validating tokens.
	/// </summary>
	/// <param name="configuration">The object providing application's configuration properties.</param>
	/// <param name="validateLifetime">The value inticating whether token expiration should be checked.</param>
	/// <returns>The token validation parameters for validating tokens.</returns>
	public static TokenValidationParameters GetTokenValidationParameters(IConfiguration configuration, bool validateLifetime = true)
	{
		JwtAuthentication jwtAuthentication = GetJwtAuthentication(configuration);
		return new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(GetTokensKey(configuration)),
			ValidIssuer = jwtAuthentication.ValidIssuer,
			ValidAudience = jwtAuthentication.ValidAudience,
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateLifetime = validateLifetime,
			ClockSkew = TimeSpan.FromSeconds(jwtAuthentication.ClockSkew)
		};
	}
}
