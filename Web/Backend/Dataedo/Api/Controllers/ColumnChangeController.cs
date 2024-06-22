using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.DocumentationEditPermission.SCT;
using Dataedo.Api.Attributes.SystemManagementSettings;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.EntityFrameworkModel.Models;
using Dataedo.Repository.Services.DTO;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.DTO;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.Interfaces;
using Dataedo.Repository.Services.Features.SchemaChangeTracking.Interfaces.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
public class ColumnChangeController : ControllerBase
{
	private readonly IObjectChangeDetailsService objectDetailsService;

	private readonly ISCTCommandsService editService;

	public ColumnChangeController(IRepositoryAccessManager repositoryAccessManager)
	{
		objectDetailsService = repositoryAccessManager.GetRepository().ObjectChangeDetailsService;
		editService = repositoryAccessManager.GetRepository().SCTObjectChangesService;
	}

	[ProducesResponseType(typeof(IEnumerable<ChangeDetailsRowDTO>), 200)]
	[ServiceFilter(typeof(SystemManagementSettingsAttribute))]
	[HttpGet("{id}/details")]
	public virtual async Task<IActionResult> GetObjectChangeDetails([Range(0, int.MaxValue)] int id)
	{
		return Ok(await objectDetailsService.GetColumnChangeDetails(id));
	}

	[ServiceFilter(typeof(ColumnChangeDocumentationEditAttribute))]
	[HttpPost("update-description")]
	public virtual async Task<IActionResult> UpdateDescription([FromForm] DescriptionUpdateDTO descriptionUpdateDTO)
	{
		return Ok(await editService.UpdateDescription<ColumnsChanges>(descriptionUpdateDTO));
	}
}