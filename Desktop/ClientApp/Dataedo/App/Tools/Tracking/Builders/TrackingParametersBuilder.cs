using System.Collections.Specialized;
using Dataedo.App.Tools.Tracking.Enums;

namespace Dataedo.App.Tools.Tracking.Builders;

public abstract class TrackingParametersBuilder
{
	private const string eventNameParameterName = "eventName";

	protected NameValueCollection nameValueCollection;

	public NameValueCollection Build(TrackingEventEnum trackingEventEnum)
	{
		nameValueCollection = new NameValueCollection();
		nameValueCollection.Add("eventName", trackingEventEnum.ToString());
		BuildOSParameters().BuildDataedoParameters().BuildRepoParameters().BuildConnectionParameters()
			.BuildUserParameters()
			.BuildEventSpecificParameters();
		return nameValueCollection;
	}

	public abstract TrackingParametersBuilder BuildOSParameters();

	public abstract TrackingParametersBuilder BuildDataedoParameters();

	public abstract TrackingParametersBuilder BuildRepoParameters();

	public abstract TrackingParametersBuilder BuildConnectionParameters();

	public abstract TrackingParametersBuilder BuildUserParameters();

	public abstract TrackingParametersBuilder BuildEventSpecificParameters();
}
