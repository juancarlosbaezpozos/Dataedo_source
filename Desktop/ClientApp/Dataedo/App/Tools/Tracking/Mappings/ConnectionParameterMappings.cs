using System.Collections.Specialized;
using Dataedo.App.Tools.Tracking.Models;

namespace Dataedo.App.Tools.Tracking.Mappings;

public class ConnectionParameterMappings
{
	private const string connectorParameterName = "connector";

	private const string dbVersionParameterName = "db_version";

	private const string connectionTypeParameterName = "connection_type";

	private const string sslModeParameterName = "ssl_mode";

	public NameValueCollection MapParameters(NameValueCollection nameValueCollection, TrackingConnectionParameters trackingConnectionParameters)
	{
		nameValueCollection.Add("connector", trackingConnectionParameters?.Connector);
		nameValueCollection.Add("db_version", trackingConnectionParameters?.DBVersion);
		nameValueCollection.Add("connection_type", trackingConnectionParameters?.ConnectionType);
		nameValueCollection.Add("ssl_mode", trackingConnectionParameters?.SSLMode);
		return nameValueCollection;
	}
}
