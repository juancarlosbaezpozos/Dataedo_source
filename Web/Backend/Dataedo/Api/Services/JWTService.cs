using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Dataedo.Api.Services;

public static class JWTService
{
	public const string authorizationHeader = "Authorization";

	private static readonly int prefixLength = "Bearer ".Length;

	private const string loginClaimName = "unique_name";

	public const string idClaim = "id";

	public static string GetLogin(HttpRequest request)
	{
		if (!request.Headers.ContainsKey("Authorization"))
		{
			return string.Empty;
		}
		string jwt = GetJwt(request);
		JwtSecurityToken token = GetToken(jwt);
		return token.Claims.FirstOrDefault((Claim x) => x.Type.Equals("unique_name")).Value;
	}

	public static int GetId(HttpRequest request)
	{
		string jwt = GetJwt(request);
		JwtSecurityToken token = GetToken(jwt);
		int.TryParse(token.Claims.FirstOrDefault((Claim x) => x.Type.Equals("id"))?.Value, out var result);
		return result;
	}

	private static string GetJwt(HttpRequest request)
	{
		if (!request.Headers.ContainsKey("Authorization"))
		{
			throw new UnauthorizedAccessException();
		}
		string jwt = request.Headers.FirstOrDefault((KeyValuePair<string, StringValues> x) => x.Key.Equals("Authorization")).Value.FirstOrDefault((string x) => x.StartsWith("Bearer"));
		if (string.IsNullOrEmpty(jwt))
		{
			throw new UnauthorizedAccessException();
		}
		return jwt.Substring(prefixLength);
	}

	private static JwtSecurityToken GetToken(string jwt)
	{
		JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
		return handler.ReadJwtToken(jwt);
	}
}
