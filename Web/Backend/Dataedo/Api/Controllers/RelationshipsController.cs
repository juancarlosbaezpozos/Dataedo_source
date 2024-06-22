using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.KeysRelationshipsManage;
using Dataedo.Api.Controllers.Base;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Relationships.DTO;
using Dataedo.Repository.Services.Features.Relationships.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class RelationshipsController : CustomFieldsController
{
	protected new IRelationshipWriteService WriteService => base.WriteService as IRelationshipWriteService;

	public RelationshipsController(IRepositoryAccessManager repositoryAccessManager)
		: base(repositoryAccessManager.GetRepository().BaseRelationshipsService, repositoryAccessManager.GetRepository().RelationshipWriteService)
	{
	}

	[HttpPost("{id}/put-description")]
	[ServiceFilter(typeof(RelationshipBasePermissionAttribute))]
	public async Task<IActionResult> UpdateDescription([FromForm] string description, int id)
	{
		return Ok(await WriteService.UpdateDescription(id, description));
	}

	[HttpPost("{id}/put-title")]
	[ServiceFilter(typeof(RelationshipBasePermissionAttribute))]
	public async Task<IActionResult> UpdateTitle([FromForm][StringLength(80)] string title, int id)
	{
		return Ok(await WriteService.UpdateTitle(id, title));
	}

	[HttpPost("{id}/put-name")]
	[ServiceFilter(typeof(RelationshipCreatePermissionAttribute))]
	public async Task<IActionResult> UpdateName([FromForm] string name, int id)
	{
		return Ok(await WriteService.UpdateName(id, name));
	}

	[HttpPost("create")]
	[ServiceFilter(typeof(RelationshipCreatePermissionAttribute))]
	public async Task<IActionResult> Create([FromForm] RelationshipCreateDTO body)
	{
		return Ok(await WriteService.Create(body));
	}

	[HttpPost("{id}/update")]
	[ServiceFilter(typeof(RelationshipUpdatePermissionAttribute))]
	public async Task<IActionResult> Update([FromForm] RelationshipUpdateDTO body, int id)
	{
		await WriteService.Update(body, id);
		return Ok();
	}

	[HttpPost("{id}/delete")]
	[ServiceFilter(typeof(RelationshipBasePermissionAttribute))]
	public async Task<IActionResult> Delete(int id)
	{
		await WriteService.Delete(id);
		return Ok();
	}

	[ServiceFilter(typeof(RelationshipBasePermissionAttribute))]
	public override Task<IActionResult> UpdateCustomField([FromForm] string fieldName, [FromForm] string value, int id)
	{
		return base.UpdateCustomField(fieldName, value, id);
	}
}
