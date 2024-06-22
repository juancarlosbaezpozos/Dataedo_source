using System.Runtime.Serialization;
using Dataedo.Repository.Services.Services;

namespace Dataedo.Api.Enums.Data;

/// <summary>
/// The class providing enumeration of connection types.
/// </summary>
[DataContract]
public class ConnectionTypeEnum : BaseEnumConversions<StatusEnum.Status>
{
	/// <summary>
	/// Specifies applicable statuses of connection types.
	/// <para>
	/// When using <see cref="T:Dataedo.Api.Enums.Data.ConnectionTypeEnum.ConnectionType" /> for JSON serialization use <code>[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]</code> attribute to make results contain <see cref="T:System.Runtime.Serialization.EnumMemberAttribute" /> EnumMember value instead of number value.
	/// </para>
	/// </summary>
	public enum ConnectionType
	{
		/// <summary>
		/// A direct connection to database.
		/// </summary>
		[EnumMember(Value = "Direct")]
		Direct = 0,
		/// <summary>
		/// A connection to database using Oracle client.
		/// </summary>
		[EnumMember(Value = "OracleClient")]
		OracleClient = 1
	}
}
