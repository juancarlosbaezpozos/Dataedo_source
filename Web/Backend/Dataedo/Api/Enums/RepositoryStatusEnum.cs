using System.Runtime.Serialization;
using Dataedo.Repository.Services.Services;

namespace Dataedo.Api.Enums;

public class RepositoryStatusEnum : BaseEnumConversions<RepositoryStatusEnum.Status>
{
	public enum Status
	{
		/// <summary>
		/// Default repository status if everything is cofigured correctly.
		/// </summary>
		[EnumMember(Value = "Active")]
		Active = 0,
		/// <summary>
		/// Connection is fully or partially configured, but due to the network issues
		/// application cannot connect to database and continue validation.
		/// </summary>
		[EnumMember(Value = "LostConnection")]
		LostConnection = 1,
		/// <summary>
		/// Cannot establish connection or check repository status
		/// because other background process is performing (repository
		/// upgrade or creator).
		/// </summary>
		[EnumMember(Value = "BackgroundProcessing")]
		BackgroundProcessing = 2,
		/// <summary>
		/// Connected to database, but repository wasn't prepared.
		/// User has to choose existing repository or create a new one.
		///
		/// This status is common on first run (after installation).
		/// </summary>
		[EnumMember(Value = "RepositoryNotConfigured")]
		RepositoryNotConfigured = 3,
		/// <summary>
		/// Sucessfully connected to repository, but detected that
		/// repository is in invalid version and should be upgraded.
		/// </summary>
		[EnumMember(Value = "UpgradeRequired")]
		UpgradeRequired = 4,
		/// <summary>
		/// Sucessfully connected to repository, but detected that
		/// repository is in version which is not supported
		/// (newer than application or too old).
		/// </summary>
		[EnumMember(Value = "VersionNotSupported")]
		VersionNotSupported = 5,
		/// <summary>
		/// Successfully connected to repository, but detected error
		/// that blocks from proper repository usage and requires
		/// server administrator intervention (e.g. corrupted database).
		/// </summary>
		[EnumMember(Value = "Invalid")]
		Invalid = 6,
		/// <summary>
		/// Successfully connected to repository, but no users
		/// found in the repository, first admin account must be created.
		/// </summary>
		[EnumMember(Value = "AccountNotConfigured")]
		AccountNotConfigured = 7
	}
}
