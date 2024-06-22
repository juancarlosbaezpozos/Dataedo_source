using System.Collections.Generic;
using System.Threading.Tasks;
using Dataedo.Repository.Services.Features.CustomFields.DTO;
using Dataedo.Repository.Services.Features.CustomFields.Interfaces;
using Dataedo.Repository.Services.Interfaces.Base;
using Dataedo.Repository.Services.JoinDefinitions;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers.Base;

/// <summary>
/// The base class providing actions for objects.
/// </summary>
public abstract class CustomFieldsController : ControllerBase
{
	/// <summary>
	/// Gets or sets the object providing actions for data of objects.
	/// </summary>
	protected IObjectCustomFields Service { get; set; }

	/// <summary>
	/// Gets the object providing actions for updating objects.
	/// </summary>
	protected ICustomFieldsWriteMarkerService WriteService { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.Base.CustomFieldsController" /> class providing base actions.
	/// </summary>
	/// <param name="service">The service object providing actions for data of objects.</param>
	/// <param name="writeService">The service object providing actions for updating data of objects.</param>
	public CustomFieldsController(IObjectCustomFields service, ICustomFieldsWriteMarkerService writeService)
	{
		Service = service;
		WriteService = writeService;
	}

	/// <summary>
	/// Gets the values of custom fields of object.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>The custom fields of object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(IEnumerable<CustomFieldDTO>), 200)]
	[HttpGet("{id}/custom-fields")]
	public virtual async Task<IActionResult> GetCustomFields(int id, [FromQuery] CustomFieldsJoinsEnum.CustomFieldsJoins[] join = null)
	{
		return Ok(await Service.GetCustomFields(id, join));
	}

	/// <summary>
	/// Gets the definitions of custom fields applicable for this type of object (that are configured to be used for this type of object).
	/// </summary>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>The definitions of custom fields of object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(IEnumerable<CustomFieldDefinitionDTO>), 200)]
	[HttpGet("custom-fields-definitions")]
	public virtual async Task<IActionResult> GetCustomFieldsDefinitionsForClass([FromQuery] CustomFieldDefinitionJoinsEnum.CustomFieldDefinitionJoins[] join = null, [FromQuery] int? objectId = null)
	{
		return Ok(await Service.GetCustomFieldsDefinitions(join, objectId));
	}

	[HttpPost("{id}/put-custom-field")]
	public virtual async Task<IActionResult> UpdateCustomField([FromForm] string fieldName, [FromForm] string value, int id)
	{
		_ = new string[0];
		string[] resultValue = ((!(WriteService is IBaseWritableDatabaseObjectService)) ? (await (WriteService as IWritableDatabaseObjectService).UpdateCustomField(id, fieldName, value)) : (await (WriteService as IBaseWritableDatabaseObjectService).UpdateCustomField(id, fieldName, value)));
		return Ok(resultValue);
	}
}
