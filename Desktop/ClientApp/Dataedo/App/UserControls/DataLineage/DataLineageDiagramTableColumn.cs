using Dataedo.App.Enums;

namespace Dataedo.App.UserControls.DataLineage;

public class DataLineageDiagramTableColumn : DataLineageDiagramColumn
{
	private FlowDirectionEnum.Direction? direction;

	public override FlowDirectionEnum.Direction? FlowDirection => direction;

	public void SetDirection(FlowDirectionEnum.Direction direction)
	{
		this.direction = direction;
	}
}
