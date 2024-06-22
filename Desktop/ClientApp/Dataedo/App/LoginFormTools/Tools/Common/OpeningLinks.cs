using System.Windows.Forms;
using Dataedo.App.Tools;
using DevExpress.Utils;

namespace Dataedo.App.LoginFormTools.Tools.Common;

internal class OpeningLinks
{
	public static void OpenDataedoLink(MouseEventArgs e)
	{
		if (e != null && e.Button == MouseButtons.Left)
		{
			Links.OpenLink(Links.Dataedo);
		}
	}

	public static void OpenLink(HyperlinkClickEventArgs e)
	{
		if (e != null && !string.IsNullOrEmpty(e.Link))
		{
			Links.OpenLink(e.Link);
		}
	}

	public static void OpenLinkWithOptionalEmail(HyperlinkClickEventArgs e, string email, char querySeparator)
	{
		if (string.IsNullOrEmpty(email))
		{
			OpenLink(e);
		}
		else
		{
			Links.OpenLink($"{e.Link}{querySeparator}email={email}");
		}
	}
}
