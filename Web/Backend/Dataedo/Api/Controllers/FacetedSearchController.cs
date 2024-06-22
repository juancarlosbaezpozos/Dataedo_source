using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Search;
using Dataedo.Repository.Services.Features.Search.DTO;
using Dataedo.Repository.Services.Features.Search.Interfaces;
using Dataedo.Repository.Services.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

/// <summary>
/// The class providing actions for searching objects.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class FacetedSearchController : ControllerBase
{
	/// <summary>
	/// Gets object providing access to repository for data of objects.
	/// </summary>
	private readonly IFacetedSearchService service;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.QuickSearchController" /> class for actions for searching objects.
	/// </summary>
	/// <param name="repositoryAccessManager">The object providing getting object for accessing to repository.</param>
	public FacetedSearchController(IRepositoryAccessManager repositoryAccessManager)
	{
		service = repositoryAccessManager.GetRepository().FacetedSearchService;
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/faceted-search/results")]
	public virtual async Task<IActionResult> GetResults([FromQuery] SearchFilters searchFilters, [FromQuery] OffsetLimitParameters parameters)
	{
		return Ok(await service.GetResults(searchFilters, parameters));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/faceted-search/asset-type-filters")]
	public virtual async Task<IActionResult> GetAssetTypesFilters([FromQuery] SearchFilters searchFilters)
	{
		return Ok(await service.GetAssetTypeFilters(searchFilters));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/faceted-search/database-filters")]
	public virtual async Task<IActionResult> GetDatabaseFilters([FromQuery] SearchFilters searchFilters)
	{
		return Ok(await service.GetDatabaseFilters(searchFilters));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/faceted-search/platform-filters")]
	public virtual async Task<IActionResult> GetPlatformFilters([FromQuery] SearchFilters searchFilters)
	{
		return Ok(await service.GetPlatformFilters(searchFilters));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/faceted-search/data-source-type-filters")]
	public virtual async Task<IActionResult> GetDataSourceTypeFilters([FromQuery] SearchFilters searchFilters)
	{
		return Ok(await service.GetDataSourceTypeFilters(searchFilters));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/faceted-search/host-filters")]
	public virtual async Task<IActionResult> GetHostFilters([FromQuery] SearchFilters searchFilters)
	{
		return Ok(await service.GetHostFilters(searchFilters));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/faceted-search/search-in-filters")]
	public virtual async Task<IActionResult> GetSearchInFilters()
	{
		return Ok(await service.GetSearchInFilters());
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/faceted-search/custom-field-filters")]
	public virtual async Task<IActionResult> GetCustomFieldFilters([FromQuery] SearchFilters searchFilters)
	{
		return Ok(await service.GetCustomFieldFilters(searchFilters));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/faceted-search/data-type-filters")]
	public virtual async Task<IActionResult> GetDataTypeFilters([FromQuery] SearchFilters searchFilters)
	{
		return Ok(await service.GetDataTypeFilters(searchFilters));
	}

	[ProducesResponseType(typeof(SearchDTO), 200)]
	[HttpGet("/api/faceted-search/object-subtype-filters")]
	public virtual async Task<IActionResult> GetObjectSubtypeFilters([FromQuery] SearchFilters searchFilters)
	{
		return Ok(await service.GetObjectSubtypeFilters(searchFilters));
	}
}
