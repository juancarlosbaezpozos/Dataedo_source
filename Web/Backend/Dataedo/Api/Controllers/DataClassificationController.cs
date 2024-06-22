using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Attributes.ClassificationView;
using Dataedo.Api.RepositoryAccess;
using Dataedo.Repository.Services.DTO;
using Dataedo.Repository.Services.Features.Classification.Enums;
using Dataedo.Repository.Services.Features.Classification.Interfaces;
using Dataedo.Repository.Services.Features.DatabaseObjects.Columns.DTO;
using Dataedo.Repository.Services.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DataClassificationController : ControllerBase
{
	private IDataClassificationService service;

	public DataClassificationController(IRepositoryAccessManager repositoryAccessManager)
	{
		service = repositoryAccessManager.GetRepository().DataClassificationService;
	}

	[ProducesResponseType(typeof(CollectionDTO<ColumnDTO>), 200)]
	[HttpGet("{id}")]
	[ServiceFilter(typeof(ClassificationViewPermissionAttribute))]
	public virtual async Task<IActionResult> Get([Range(0, int.MaxValue)] int id, [FromQuery] OffsetLimitFilterParameters<ClassifiedDataFiltersEnum.ClassifiedDataFilters> offsetLimitParameters)
	{
		return Ok(await service.GetClassifiedData(id, offsetLimitParameters));
	}

	[ProducesResponseType(typeof(CollectionDTO<ColumnDTO>), 200)]
	[HttpGet("{id}/field/{fieldId}")]
	[ServiceFilter(typeof(ClassificationViewPermissionAttribute))]
	public virtual async Task<IActionResult> GetByField([Range(0, int.MaxValue)] int id, int fieldId, [FromQuery] OffsetLimitFilterParameters<ClassifiedDataFiltersEnum.ClassifiedDataFilters> offsetLimitParameters)
	{
		return Ok(await service.GetClassifiedDataByField(id, fieldId, offsetLimitParameters));
	}

	[ProducesResponseType(typeof(CollectionDTO<ColumnDTO>), 200)]
	[HttpGet("{id}/value/{value}")]
	[ServiceFilter(typeof(ClassificationViewPermissionAttribute))]
	public virtual async Task<IActionResult> GetByValue([Range(0, int.MaxValue)] int id, string value, [FromQuery] OffsetLimitFilterParameters<ClassifiedDataFiltersEnum.ClassifiedDataFilters> offsetLimitParameters)
	{
		return Ok(await service.GetClassifiedDataByValue(id, value, offsetLimitParameters));
	}
}
