using DevExpress.XtraDiagram;
using DevExpress.XtraDiagram.ViewInfo;

namespace Dataedo.App.UserControls.OverriddenControls;

public class DiagramControlWithTransparentFocusedBorderViewInfo : DiagramControlViewInfo
{
	public DiagramControlWithTransparentFocusedBorderViewInfo(DiagramControl owner)
		: base(owner)
	{
	}

	protected override DiagramDefaultAppearances CreateDefaultAppearances()
	{
		return new DiagramWithTransparentFocusedBorderDefaultAppearances(LookAndFeel);
	}
}
