using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.XtraDiagram;

namespace Dataedo.App.UserControls.OverriddenControls;

public class DiagramWithTransparentFocusedBorderDefaultAppearances : DiagramDefaultAppearances
{
	public DiagramWithTransparentFocusedBorderDefaultAppearances(UserLookAndFeel lookAndFeel)
		: base(lookAndFeel)
	{
	}

	public override Color GetSelectionBorderColor()
	{
		return Color.Transparent;
	}

	public override Color GetSelectionPartBackColor()
	{
		return Color.Transparent;
	}
}
