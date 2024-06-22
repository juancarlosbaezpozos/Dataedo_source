using System.Collections.Generic;
using System.Threading.Tasks;
using Dataedo.Repository.Services.Features.Dependencies;
using Dataedo.Repository.Services.Features.Dependencies.DTO;
using Dataedo.Repository.Services.JoinDefinitions;
using Microsoft.AspNetCore.Mvc;

namespace Dataedo.Api.Controllers.Interfaces;

/// <summary>
/// Represents object providing actions for dependencies.
/// </summary>
internal interface IDependencies
{
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
	Task<IActionResult> GetUsesDependencies(DependencyInformation dependencyInformation, int rootDatabaseId, DependencyJoinsEnum.DependencyJoins[] join = null);

	/// <summary>
	/// Gets the list of top level of uses dependencies by specified server, database, schema, name and type.
	/// </summary>
	/// <param name="dependencyInformation">The information about referencing object.</param>
	/// <param name="rootDatabaseId">The ID of root database object.</param>
	/// <param name="join">The array of names of data to load with objects.</param>
	/// <returns>A list of top level dependencies for the specified ID.</returns>
	/// <response code="200">Successful operation.</response>
	/// <response code="400">Invalid ID supplied.</response>
	/// <response code="404">Object not found.</response>
	[ProducesResponseType(typeof(IList<DependencyObjectDTO>), 200)]
	[HttpGet("dependencies/used-by")]
	Task<IActionResult> GetUsedByDependencies(DependencyInformation dependencyInformation, int rootDatabaseId, DependencyJoinsEnum.DependencyJoins[] join = null);
}
