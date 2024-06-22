using Dataedo.App.Classes;
using Dataedo.App.MenuTree;

namespace Dataedo.App.Tools;

public class SummaryEdit : BasicEdit, IEdit
{
	private bool isEdited;

	private DBTreeNode node;

	public override bool IsEdited
	{
		get
		{
			return isEdited;
		}
		set
		{
			isEdited = value;
			CommonFunctionsPanels.SetNodesTitle(isEdited, node.GetRootNode(), node);
		}
	}

	public SummaryEdit(DBTreeNode node)
	{
		this.node = node;
	}
}
