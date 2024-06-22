using System.Threading.Tasks;
using Dataedo.Api.Attributes.ClassificationView;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Menus;
using Dataedo.Repository.Services.Features.Menus.DTO;
using Dataedo.Repository.Services.Features.Menus.Enums;
using Dataedo.Repository.Services.Features.Menus.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

/// <summary>
/// The class providing data for application menus.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class MenusController : ControllerBase
{
	/// <summary>
	/// The object providing access to repository for data of objects.
	/// </summary>
	private readonly IMenusService service;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.MenusController" /> class.
	/// </summary>
	public MenusController(IRepositoryAccessManager repositoryAccessManager)
	{
		service = repositoryAccessManager.GetRepository().MenusService;
	}

	/// <summary>
	/// Gets the collection of tree nodes of data dictionary tree.
	/// The collection is processed using provided parameters.
	/// </summary>
	/// <param name="treeParameters">
	/// The parameters providing selecting required set of data by skipping and limiting result items, filtering.
	/// The parameter also provides collection of identifiers of nodes that are expanded so their subnodes are also processed.
	/// </param>
	/// <returns>The collection of object representing nodes of tree.</returns>
	/// <response code="200">Successful operation.</response>
	[ProducesResponseType(typeof(DataDictionaryMenuNodeDTO), 200)]
	[HttpPost("data-dictionary")]
	public async Task<IActionResult> GetDataDictionaryMenu([FromForm] TreeParameters<DataDictionaryFiltersEnum.DataDictionaryFilters> treeParameters)
	{
		return Ok(await service.GetOptimizedTree(treeParameters, OptimizedTreeEnum.DataDictionary, appendChildlessRoot: true));
	}

	/// <summary>
	/// Gets the collection of tree nodes of glossary tree.
	/// The collection is processed using provided parameters.
	/// </summary>
	/// <returns>The collection of object representing nodes of tree.</returns>
	/// <response code="200">Successful operation.</response>
	[ProducesResponseType(typeof(DataDictionaryMenuNodeDTO), 200)]
	[HttpGet("glossaries")]
	public async Task<IActionResult> GetGlossariesMenu()
	{
		return Ok(await service.GetInMemoryTree(InMemoryMenuTreeType.Glossaries));
	}

	/// <summary>
	/// Gets the collection of tree nodes of subject areas tree.
	/// The collection is processed using provided parameters.
	/// </summary>
	/// <returns>The collection of object representing nodes of tree.</returns>
	/// <response code="200">Successful operation.</response>
	[ProducesResponseType(typeof(DataDictionaryMenuNodeDTO), 200)]
	[HttpPost("subject-areas")]
	public async Task<IActionResult> GetSubjectAreasMenu([FromForm] TreeParameters<DataDictionaryFiltersEnum.DataDictionaryFilters> treeParameters)
	{
		return Ok(await service.GetOptimizedTree(treeParameters, OptimizedTreeEnum.SubjectArea, appendChildlessRoot: false));
	}

	/// <summary>
	/// Gets the collection of tree nodes of subject areas tree.
	/// The collection is processed using provided parameters.
	/// </summary>
	/// <returns>The collection of object representing nodes of tree.</returns>
	/// <response code="200">Successful operation.</response>
	[ProducesResponseType(typeof(DataDictionaryMenuNodeDTO), 200)]
	[ServiceFilter(typeof(ClassificationViewPermissionAttribute))]
	[HttpGet("data-classification")]
	public async Task<IActionResult> GetDataClassificationMenu()
	{
		return Ok(await service.GetDataClassificationTree());
	}
}
