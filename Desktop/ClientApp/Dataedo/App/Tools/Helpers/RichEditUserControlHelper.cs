using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.UserControls;
using DevExpress.XtraRichEdit.API.Native;

namespace Dataedo.App.Tools.Helpers;

public static class RichEditUserControlHelper
{
	public static void SetLineNumbering(RichEditUserControl richEditUserControl)
	{
		if (richEditUserControl != null)
		{
			int left = 85;
			richEditUserControl.Views.SimpleView.AllowDisplayLineNumbers = true;
			richEditUserControl.Views.DraftView.AllowDisplayLineNumbers = true;
			richEditUserControl.Views.PrintLayoutView.AllowDisplayLineNumbers = true;
			richEditUserControl.Views.SimpleView.Padding = new Padding(left, 4, 0, 0);
			richEditUserControl.Views.DraftView.Padding = new Padding(left, 4, 0, 0);
			richEditUserControl.Document.Sections[0].LineNumbering.RestartType = LineNumberingRestart.Continuous;
			richEditUserControl.Document.Sections[0].LineNumbering.Start = 1;
			richEditUserControl.Document.Sections[0].LineNumbering.CountBy = 1;
			richEditUserControl.Document.Sections[0].LineNumbering.Distance = 0.1f;
			richEditUserControl.Document.CharacterStyles["Line Number"].FontName = "Tahoma";
			richEditUserControl.Document.CharacterStyles["Line Number"].FontSize = 10f;
			richEditUserControl.Document.CharacterStyles["Line Number"].ForeColor = Color.DarkGray;
			richEditUserControl.Document.CharacterStyles["Line Number"].Bold = true;
		}
	}
}
