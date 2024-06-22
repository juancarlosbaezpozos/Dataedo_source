using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Search.DTO;
using Dataedo.Repository.Services.Features.Search.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

/// <summary>
/// The class providing actions for searching objects.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class QuickSearchController : ControllerBase
{
	/// <summary>
	/// Gets object providing access to repository for data of objects.
	/// </summary>
	private readonly IQuickSearchService service;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.QuickSearchController" /> class for actions for searching objects.
	/// </summary>
	/// <param name="repositoryAccessManager">The object providing getting object for accessing to repository.</param>
	public QuickSearchController(IRepositoryAccessManager repositoryAccessManager)
	{
		service = repositoryAccessManager.GetRepository().QuickSearchService;
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/quick-search/terms")]
	public virtual async Task<IActionResult> GetTerms([FromQuery] string input)
	{
		return Ok(await service.SearchTermsAsync(input));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/quick-search/tables")]
	public virtual async Task<IActionResult> GetTables([FromQuery] string input)
	{
		return Ok(await service.SearchTablesAsync(input));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/quick-search/columns")]
	public virtual async Task<IActionResult> GetColumns([FromQuery] string input)
	{
		return Ok(await service.SearchColumnsAsync(input));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/quick-search/views")]
	public virtual async Task<IActionResult> GetViews([FromQuery] string input)
	{
		return Ok(await service.SearchViewsAsync(input));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/quick-search/other-entries")]
	public virtual async Task<IActionResult> GetOtherEntries([FromQuery] string input)
	{
		return Ok(await service.SearchOtherEntriesAsync(input));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/quick-search/databases")]
	public virtual async Task<IActionResult> GetDatabases([FromQuery] string input)
	{
		return Ok(await service.SearchDatabasesAsync(input));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/quick-search/glossaries")]
	public virtual async Task<IActionResult> GetGlossaries([FromQuery] string input)
	{
		return Ok(await service.SearchGlossariesAsync(input));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/quick-search/procedures")]
	public virtual async Task<IActionResult> GetProcedures([FromQuery] string input)
	{
		return Ok(await service.SearchProceduresAsync(input));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/quick-search/functions")]
	public virtual async Task<IActionResult> GetFunctions([FromQuery] string input)
	{
		return Ok(await service.SearchFunctionsAsync(input));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/quick-search/triggers")]
	public virtual async Task<IActionResult> GetTriggers([FromQuery] string input)
	{
		return Ok(await service.SearchTriggersAsync(input));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/quick-search/parameters")]
	public virtual async Task<IActionResult> GetParameters([FromQuery] string input)
	{
		return Ok(await service.SearchParametersAsync(input));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/quick-search/subject-areas")]
	public virtual async Task<IActionResult> GetSubjectAreas([FromQuery] string input)
	{
		return Ok(await service.SearchSubjectAreasAsync(input));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/quick-search/structures")]
	public virtual async Task<IActionResult> GetStructures([FromQuery] string input)
	{
		return Ok(await service.SearchStructuresAsync(input));
	}
}
