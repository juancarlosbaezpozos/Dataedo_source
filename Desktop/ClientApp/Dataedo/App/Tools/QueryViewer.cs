using Dataedo.App.Forms;

namespace Dataedo.App.Tools;

public static class QueryViewer
{
	public static bool ShowQueryWindow;

	public static void View(string title, string query)
	{
		if (ShowQueryWindow)
		{
			new QueryForm(title, query).ShowDialog();
		}
	}
}
