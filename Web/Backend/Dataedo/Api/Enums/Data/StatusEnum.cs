using System.Runtime.Serialization;
using Dataedo.Repository.Services.Services;

namespace Dataedo.Api.Enums.Data;

/// <summary>
/// The class providing enumeration of statuses of synchronization.
/// </summary>
[DataContract]
public class StatusEnum : BaseEnumConversions<StatusEnum.Status>
{
	/// <summary>
	/// Specifies applicable statuses of synchronization.
	/// </summary>
	public enum Status
	{
		/// <summary>
		/// A object exists in database.
		/// </summary>
		[EnumMember(Value = "Active")]
		Active = 0,
		/// <summary>
		/// A object is deleted from database.
		/// </summary>
		[EnumMember(Value = "Deleted")]
		Deleted = 1
	}
}
