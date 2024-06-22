using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoBuilderEventSpecificObjectTypeViewFlowsCount : ParametersWithUserDataedoBuilder
{
	private readonly string objectType;

	private readonly string view;

	private readonly string flowsCount;

	public ParametersWithUserDataedoBuilderEventSpecificObjectTypeViewFlowsCount(ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, string objectType, string view, string flowsCount)
		: base(trackingUserParameters, trackingDataedoParameters)
	{
		this.objectType = objectType;
		this.view = view;
		this.flowsCount = flowsCount;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetObjectType(nameValueCollection, objectType);
		SpecificParametersMapper.GetView(nameValueCollection, view);
		SpecificParametersMapper.GetFlowsCount(nameValueCollection, flowsCount);
		return this;
	}
}
