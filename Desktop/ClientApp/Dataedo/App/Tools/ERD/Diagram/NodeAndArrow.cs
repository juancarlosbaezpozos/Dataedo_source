using System.Collections.Generic;
using System.Linq;

namespace Dataedo.App.Tools.ERD.Diagram;

public class NodeAndArrow
{
	public Node NodeValue { get; set; }

	public LinkArrow Arrow { get; set; }

	public int LinkRelationId { get; set; }

	public NodeAndArrow(Node nodeValue, LinkArrow arrow, int linkRelationId)
	{
		NodeValue = nodeValue;
		Arrow = arrow;
		LinkRelationId = linkRelationId;
	}

	public static IEnumerable<NodeAndArrow> OrderByArrowsPositions(IEnumerable<NodeAndArrow> nodes, ArrowDirectionEnum direction)
	{
		return direction switch
		{
			ArrowDirectionEnum.Up => from x in nodes
				orderby x.NodeValue.Center.X, x.NodeValue.Center.Y descending, x.LinkRelationId
				select x, 
			ArrowDirectionEnum.Down => from x in nodes
				orderby x.NodeValue.Center.X, x.NodeValue.Center.Y, x.LinkRelationId
				select x, 
			ArrowDirectionEnum.Left => from x in nodes
				orderby x.NodeValue.Center.Y, x.NodeValue.Center.X descending, x.LinkRelationId
				select x, 
			ArrowDirectionEnum.Right => from x in nodes
				orderby x.NodeValue.Center.Y, x.NodeValue.Center.X, x.LinkRelationId
				select x, 
			_ => nodes, 
		};
	}
}
