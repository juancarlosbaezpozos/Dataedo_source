using System.Threading.Tasks;
using Dataedo.Api.Attributes.DocumentationEditPermission;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.GlossaryEntriesRelationships.DTO;
using Dataedo.Repository.Services.Features.GlossaryEntriesRelationships.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/glossary-entries-relationships")]
[Authorize]
[ApiController]
public class EntryRelationshipsWriteController : ControllerBase
{
	private readonly IEntryRelationshipWriteService writeService;

	public EntryRelationshipsWriteController(IRepositoryAccessManager repositoryAccessManager)
	{
		writeService = repositoryAccessManager.GetRepository().GlossaryEntryRelationshipsWriteService;
	}

	[HttpPost("create")]
	[ServiceFilter(typeof(EntryRelationshipEditPermission))]
	public async Task<IActionResult> Create([FromBody] EntryRelationshipCreateDTO input)
	{
		return Ok(await writeService.Create(input));
	}

	[ServiceFilter(typeof(GlossaryEntriesRelationshipEditPermissionAttribute))]
	[HttpPost("{id}/delete")]
	public async Task<IActionResult> Delete(int id)
	{
		await writeService.Delete(id);
		return Ok();
	}

	[HttpPost("{id}/put-description")]
	[ServiceFilter(typeof(GlossaryEntriesRelationshipEditPermissionAttribute))]
	public async Task<IActionResult> UpdateDescription([FromForm] string description, int id)
	{
		description = await writeService.UpdateDescription(id, description);
		return Ok(description);
	}
}
