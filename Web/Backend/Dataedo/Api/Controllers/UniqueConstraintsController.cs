using System.Threading.Tasks;
using Dataedo.Api.Attributes.KeysRelationshipsManage;
using Dataedo.Api.Controllers.Base;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.Features.Keys.DTO;
using Dataedo.Repository.Services.Features.Keys.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/unique-constraints")]
[ApiController]
public class UniqueConstraintsController : CustomFieldsController
{
	protected new IConstraintWriteService WriteService { get; private set; }

	public UniqueConstraintsController(IRepositoryAccessManager repositoryAccessManager)
		: base(repositoryAccessManager.GetRepository().UniqueConstraintsService, repositoryAccessManager.GetRepository().UniqueConstraintsWriteService)
	{
		WriteService = repositoryAccessManager.GetRepository().UniqueConstraintsWriteService;
	}

	[ServiceFilter(typeof(BaseKeysManagePermissionAttribute))]
	[HttpPost("{id}/put-description")]
	public async Task<IActionResult> UpdateDescription([FromForm] string description, int id)
	{
		description = await WriteService.UpdateDescription(id, description);
		return Ok(description);
	}

	[ServiceFilter(typeof(BaseKeysManagePermissionAttribute))]
	public override async Task<IActionResult> UpdateCustomField([FromForm] string fieldName, [FromForm] string value, int id)
	{
		return Ok(await WriteService.UpdateCustomField(id, fieldName, value));
	}

	[ServiceFilter(typeof(BaseKeysManagePermissionAttribute))]
	[HttpPost("{id}/put-name")]
	public async Task<IActionResult> UpdateName([FromForm] string name, int id)
	{
		name = await WriteService.UpdateName(id, name);
		return Ok(name);
	}

	[HttpPost("create")]
	[ServiceFilter(typeof(KeyCreatePermissionAttribute))]
	public async Task<IActionResult> Create([FromForm] KeyCreateDTO model)
	{
		return Ok(await WriteService.Create(model));
	}

	[HttpPost("update")]
	[ServiceFilter(typeof(KeyUpdatePermissionAttribute))]
	public async Task<IActionResult> Update([FromForm] KeyUpdateDTO model)
	{
		return Ok(await WriteService.Update(model));
	}

	[HttpPost("{id}/delete")]
	[ServiceFilter(typeof(BaseKeysManagePermissionAttribute))]
	public async Task<IActionResult> Delete(int id)
	{
		await WriteService.Delete(id);
		return Ok();
	}
}
