using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Dataedo.Api.Services;

public static class ClaimsTransform
{
	public static ClaimsPrincipal Transform(ClaimsPrincipal incomingPrincipal)
	{
		if (!incomingPrincipal.Identity.IsAuthenticated)
		{
			return incomingPrincipal;
		}
		return CreateClaimsPrincipal(incomingPrincipal);
	}

	private static ClaimsPrincipal CreateClaimsPrincipal(ClaimsPrincipal incomingPrincipal)
	{
		List<Claim> claims = new List<Claim>();
		claims.AddRange(GetSaml2LogoutClaims(incomingPrincipal));
		claims.Add(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", GetClaimValue(incomingPrincipal, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")));
		return new ClaimsPrincipal(new ClaimsIdentity(claims, incomingPrincipal.Identity.AuthenticationType, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
	}

	private static IEnumerable<Claim> GetSaml2LogoutClaims(ClaimsPrincipal principal)
	{
		yield return GetClaim(principal, "http://schemas.itfoxtec.com/ws/2014/02/identity/claims/saml2nameid");
		yield return GetClaim(principal, "http://schemas.itfoxtec.com/ws/2014/02/identity/claims/saml2nameidformat");
		yield return GetClaim(principal, "http://schemas.itfoxtec.com/ws/2014/02/identity/claims/saml2sessionindex");
	}

	private static Claim GetClaim(ClaimsPrincipal principal, string claimType)
	{
		return ((ClaimsIdentity)principal.Identity).Claims.Where((Claim c) => c.Type == claimType).FirstOrDefault();
	}

	private static string GetClaimValue(ClaimsPrincipal principal, string claimType)
	{
		return GetClaim(principal, claimType)?.Value;
	}
}
