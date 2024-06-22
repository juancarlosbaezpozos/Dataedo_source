namespace Dataedo.App.Tools.ERD.Diagram;

public class NodeRelationContainer
{
	public int? TableId { get; set; }

	public string RelationStatus { get; set; }

	public NodeRelationContainer(int? tableId, string relationStatus)
	{
		TableId = tableId;
		RelationStatus = relationStatus;
	}
}
