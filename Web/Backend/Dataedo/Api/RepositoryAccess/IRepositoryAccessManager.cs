using Dataedo.Repository.Services.RepositoryAccess;

namespace Dataedo.Api.RepositoryAccess;

/// <summary>
/// Represents object providing getting access to repository services.
/// </summary>
public interface IRepositoryAccessManager
{
	/// <summary>
	/// Gets the object providing repository services.
	/// </summary>
	/// <returns>The object providing repository services.</returns>
	IRepository GetRepository();
}
