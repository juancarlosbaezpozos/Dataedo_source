using System.Drawing;
using DevExpress.XtraEditors;

namespace Dataedo.App.Forms.Tools;

public static class LookUpEditSizeHelper
{
	private static readonly int scrollBarHeight = 80;

	public static void SetDropDownSize(LookUpEdit edit, int objectsCount)
	{
		Size size2 = (edit.Properties.PopupFormMinSize = (edit.Properties.PopupFormSize = GetDropDownSize(edit.Width, objectsCount, edit.Height)));
		edit.Properties.DropDownRows = objectsCount;
	}

	public static Size GetDropDownSize(int dropDownWidth, int objectsCount, int rowHeight)
	{
		return new Size(dropDownWidth - scrollBarHeight, (rowHeight - 5) * objectsCount);
	}
}
