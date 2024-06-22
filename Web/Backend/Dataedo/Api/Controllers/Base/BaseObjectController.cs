using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Dataedo.Api.Controllers.Interfaces;
using Dataedo.Repository.Services.DTO;
using Dataedo.Repository.Services.Features.DataLineage.ColumnLevel.Interfaces;
using Dataedo.Repository.Services.Features.Dependencies;
using Dataedo.Repository.Services.Features.Dependencies.DTO;
using Dataedo.Repository.Services.Features.Feedback;
using Dataedo.Repository.Services.Features.Feedback.DTO;
using Dataedo.Repository.Services.Features.Feedback.Enums;
using Dataedo.Repository.Services.Features.SubjectAreas.DTO;
using Dataedo.Repository.Services.Interfaces.Base;
using Dataedo.Repository.Services.JoinDefinitions;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers.Base;

/// <summary>
/// The base class providing actions for objects.
/// </summary>
public abstract class BaseObjectController : CustomFieldsController, IDependencies
{
	protected IColumnDataLineageService columnDataLineageService;

	/// <summary>
	/// Gets the object providing actions for data of objects.
	/// </summary>
	protected new IBaseObjectService Service { get; private set; }

	/// <summary>
	/// Gets the object providing actions for updating objects.
	/// </summary>
	protected new IWritableDatabaseObjectService WriteService { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Dataedo.Api.Controllers.Base.BaseObjectController" /> class providing base actions.
	/// </summary>
	/// <param name="service">The service object providing actions for data of objects.</param>
	public BaseObjectController(IBaseObjectService service, IWritableDatabaseObjectService writeService = null)
		: base(service, writeService)
	{
		Service = service;
		WriteService = writeService;
	}

	/// <summary>
	/// Gets the subject areas that object belongs to.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>The subject areas that object belongs to.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(IEnumerable<SubjectAreaDTO>), 200)]
	[HttpGet("{id}/subject-areas")]
	public virtual async Task<IActionResult> GetSubjectAreas([Range(0, int.MaxValue)] int id, [FromQuery] SubjectAreasJoinsEnum.SubjectAreasJoins[] join = null)
	{
		return Ok(await Service.GetSubjectAreas(id, join));
	}

	/// <summary>
	/// Gets the HTML formatted description of object.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <returns>The HTML formatted description of object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(string), 200)]
	[HttpGet("{id}/description")]
	public virtual async Task<IActionResult> GetHtmlDescription(int id)
	{
		await Task.FromException(new NotImplementedException());
		return Ok(null);
	}

	/// <summary>
	/// Gets the plain text description of object.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <returns>The plain text description of object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(string), 200)]
	[HttpGet("{id}/plain-description")]
	public virtual async Task<IActionResult> GetPlainTextDescription(int id)
	{
		await Task.FromException(new NotImplementedException());
		return Ok(null);
	}

	/// <summary>
	/// Gets the list of top level of uses dependencies by specified server, database, schema, name and type.
	/// </summary>
	/// <param name="dependencyInformation">The information about referenced object.</param>
	/// <param name="rootDatabaseId">The ID of root database object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>A list of top level dependencies for the specified ID.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(DependencyObjectDTO), 200)]
	[HttpGet("dependencies/uses")]
	public virtual async Task<IActionResult> GetUsesDependencies([FromQuery] DependencyInformation dependencyInformation, [FromQuery][Required][Range(0, int.MaxValue)] int rootDatabaseId, [FromQuery] DependencyJoinsEnum.DependencyJoins[] join = null)
	{
		if (string.IsNullOrEmpty(dependencyInformation.Server))
		{
			return BadRequest(dependencyInformation);
		}
		return Ok(await Service.GetUsesDependencies(dependencyInformation, rootDatabaseId, join));
	}

	/// <summary>
	/// Gets the list of top level of uses dependencies by specified server, database, schema, name and type.
	/// </summary>
	/// <param name="dependencyInformation">The information about referencing object.</param>
	/// <param name="rootDatabaseId">The ID of root database object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>A list of top level dependencies for the specified ID.</returns>
	/// <response code="200">Successful operation.</response>
	[ProducesResponseType(typeof(IList<DependencyObjectDTO>), 200)]
	[HttpGet("dependencies/used-by")]
	public virtual async Task<IActionResult> GetUsedByDependencies([FromQuery] DependencyInformation dependencyInformation, [FromQuery][Required][Range(0, int.MaxValue)] int rootDatabaseId, [FromQuery] DependencyJoinsEnum.DependencyJoins[] join = null)
	{
		if (string.IsNullOrEmpty(dependencyInformation.Server))
		{
			return BadRequest(dependencyInformation);
		}
		return Ok(await Service.GetUsedByDependencies(dependencyInformation, rootDatabaseId, join));
	}

	/// <summary>
	/// Gets the metadata (creation, import and update information) information.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <returns>The metadata (creation, import and update information) information.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(IList<DatabaseObjectMetadataDTO>), 200)]
	[HttpGet("{id}/metadata")]
	public virtual async Task<IActionResult> GetMetadata(int id)
	{
		await Task.FromException(new NotImplementedException());
		return Ok(null);
	}

	/// <summary>
	/// Gets the basic information about feedback.
	/// </summary>
	/// <param name="id">The ID of object.</param>
	/// <returns>The basic information about object.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(FeedbackDTO), 200)]
	[HttpGet("feedback/{id}")]
	public virtual async Task<IActionResult> GetFeedbacks([Range(0, int.MaxValue)] int id, [FromQuery] CollectionFeedbackParameters<FeedbackFiltersEnum.FeedbackFilters> parameters = null)
	{
		return Ok(await Service.GetFeedback(id, parameters));
	}

	[ProducesResponseType(typeof(IList<DatabaseObjectMetadataDTO>), 200)]
	[HttpGet("{id}/data-lineage-diagram")]
	public virtual async Task<IActionResult> GetDataLineageDiagramData(int id)
	{
		return Ok(await Service.GetDataLineageDiagram(id));
	}

	[HttpGet("{id}/column-data-lineage")]
	public virtual async Task<IActionResult> GetColumnDataLineage(int id)
	{
		return Ok(await columnDataLineageService.GetData(id));
	}

	[HttpPost("{id}/put-title")]
	public virtual async Task<IActionResult> UpdateTitle([FromForm][StringLength(80)] string title, int id)
	{
		title = await WriteService.UpdateTitle(id, title);
		return Ok(title);
	}

	[HttpPost("{id}/put-description")]
	public virtual async Task<IActionResult> UpdateDescription([FromForm] string description, int id)
	{
		return Ok(await WriteService.UpdateDescription(id, description));
	}
}
