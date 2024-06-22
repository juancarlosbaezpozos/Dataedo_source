using System.Linq;
using Dataedo.App.UserControls.DataLineage;
using DevExpress.XtraTreeList;

namespace Dataedo.App.UserControls.OverriddenControls;

public class TreeListWithInfoText : TreeList
{
	private string infoText;

	public TreeListWithInfoText()
	{
		base.CustomDrawEmptyArea += delegate(object sedner, CustomDrawEmptyAreaEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(infoText))
			{
				ControlsUtils.AddInfoText(e.Cache, e.EmptyRectangles.First(), infoText, base.RowHeight);
				e.Handled = true;
			}
		};
	}

	public void SetInfoText(string text)
	{
		infoText = text;
	}
}
