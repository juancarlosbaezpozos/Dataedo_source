using System.Collections.Generic;
using System.Threading.Tasks;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.GlossaryEntriesRelationships.DTO;
using Dataedo.Repository.Services.Features.GlossaryEntriesRelationships.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

/// <summary>
/// The class providing actions for glossary entry relationship types.
/// </summary>
[Route("api/glossary-entry-relationship-types")]
[Authorize]
[ApiController]
public class GlossaryEntryRelationshipTypesController : ControllerBase
{
	private readonly IRepositoryAccessManager _repositoryAccessManager;

	/// <summary>
	/// Gets object providing access to repository for data of objects.
	/// </summary>
	protected IGlossaryEntryRelationshipTypesService Service => _repositoryAccessManager.GetRepository().GlossaryEntryRelationshipTypesService;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.GlossaryEntryRelationshipTypesController" /> class for actions for glossary entry relationship types objects.
	/// </summary>
	/// <param name="repositoryAccessManager">The object providing getting object for accessing to repository.</param>
	public GlossaryEntryRelationshipTypesController(IRepositoryAccessManager repositoryAccessManager)
	{
		_repositoryAccessManager = repositoryAccessManager;
	}

	/// <summary>
	/// Gets the basic information about glossary entry relationship types.
	/// </summary>
	/// <returns>The basic information about types.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="401">Unauthorized.</response>
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<GlossaryEntryRelationshipTypeDTO>), 200)]
	public virtual async Task<IActionResult> Get([FromQuery] bool? reversed)
	{
		return Ok(await Service.GetObjectsAsync(reversed));
	}
}
