using DevExpress.XtraDiagram;
using DevExpress.XtraDiagram.ViewInfo;

namespace Dataedo.App.UserControls.OverriddenControls;

public class DiagramControlWithTransparentFocusedBorder : DiagramControl
{
	protected override DiagramControlViewInfo CreateViewInfo()
	{
		return new DiagramControlWithTransparentFocusedBorderViewInfo(this);
	}
}
