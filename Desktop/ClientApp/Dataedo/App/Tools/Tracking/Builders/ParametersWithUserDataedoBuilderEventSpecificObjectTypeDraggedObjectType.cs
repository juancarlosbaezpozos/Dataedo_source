using Dataedo.App.Tools.Tracking.Interfaces;
using Dataedo.App.Tools.Tracking.Mappings;

namespace Dataedo.App.Tools.Tracking.Builders;

public class ParametersWithUserDataedoBuilderEventSpecificObjectTypeDraggedObjectType : ParametersWithUserDataedoBuilder
{
	private readonly string objectType;

	private readonly string draggedObjectType;

	public ParametersWithUserDataedoBuilderEventSpecificObjectTypeDraggedObjectType(ITrackingUserParameters trackingUserParameters, ITrackingDataedoParameters trackingDataedoParameters, string objectType, string draggedObjectType)
		: base(trackingUserParameters, trackingDataedoParameters)
	{
		this.objectType = objectType;
		this.draggedObjectType = draggedObjectType;
	}

	public override TrackingParametersBuilder BuildEventSpecificParameters()
	{
		SpecificParametersMapper.GetObjectType(nameValueCollection, objectType);
		SpecificParametersMapper.GetDraggedObjectType(nameValueCollection, draggedObjectType);
		return this;
	}
}
