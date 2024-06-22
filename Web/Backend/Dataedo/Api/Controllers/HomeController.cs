using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

/// <summary>
/// The class providing controller for start pages.
/// </summary>
[Authorize]
public class HomeController : Controller
{
	/// <summary>
	/// Calls controller base action. Redirects to Swagger documentation.
	/// </summary>
	/// <returns>A Swagger documentation page.</returns>
	[AllowAnonymous]
	public IActionResult Index()
	{
		return Redirect("swagger/index.html");
	}
}
