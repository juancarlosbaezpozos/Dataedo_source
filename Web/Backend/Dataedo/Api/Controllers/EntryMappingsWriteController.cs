using System.Threading.Tasks;
using Dataedo.Api.Attributes.DocumentationEditPermission;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.GlossaryEntryMappings.DTO;
using Dataedo.Repository.Services.Features.GlossaryEntryMappings.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/glossary-mappings")]
[Authorize]
[ApiController]
public class EntryMappingsWriteController : ControllerBase
{
	private readonly IEntryMappingWriteService writeService;

	public EntryMappingsWriteController(IRepositoryAccessManager repositoryAccessManager)
	{
		writeService = repositoryAccessManager.GetRepository().EntryMappingWriteService;
	}

	[HttpPost("match-glossary-entry")]
	[ServiceFilter(typeof(EntryMappingEditPermissionAttribute))]
	public async Task<IActionResult> Create([FromBody] EntryMappingDTO input)
	{
		return Ok(await writeService.Create(input));
	}

	[HttpPost("delete")]
	[ServiceFilter(typeof(EntryMappingEditPermissionAttribute))]
	public async Task<IActionResult> Delete([FromBody] EntryMappingDTO input)
	{
		await writeService.Delete(input);
		return Ok();
	}
}
