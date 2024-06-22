using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;

namespace Dataedo.App.UserControls.RadioGroupWithImages;

public class RadioGroupWithImagesViewInfo : RadioGroupViewInfo
{
	public RadioGroupWithImagesViewInfo(RepositoryItem item)
		: base(item)
	{
	}

	protected override CheckObjectPainter CreateRadioPainter()
	{
		if (!IsPrinting)
		{
			CheckPainterHelper.GetPainter(LookAndFeel);
		}
		else
		{
			CheckPainterHelper.GetPainter(ActiveLookAndFeelStyle.Flat);
		}
		return new RadioImageButtonPainter(LookAndFeel);
	}
}
