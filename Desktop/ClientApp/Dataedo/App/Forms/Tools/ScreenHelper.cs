using System.Drawing;
using System.Windows.Forms;

namespace Dataedo.App.Forms.Tools;

public static class ScreenHelper
{
	private static Screen currentScreen { get; set; }

	public static void SetDefaultScreen(Point point, Size size)
	{
		currentScreen = Screen.FromPoint(new Point(point.X + size.Width / 2, point.Y + size.Height / 2));
	}

	public static void CenterInCurrentScreen(Form form, bool eventuallyCenterByCursor = true)
	{
		if (currentScreen != null)
		{
			CenterByScreen(currentScreen, form);
		}
		else if (eventuallyCenterByCursor)
		{
			CenterInCursorScreen(form);
		}
	}

	public static void CenterInCursorScreen(Form form)
	{
		CenterByScreen(Screen.FromPoint(new Point(Cursor.Position.X, Cursor.Position.Y)), form);
	}

	public static void Center(Form form)
	{
		CenterByScreen(Screen.FromPoint(new Point(form.Location.X, form.Location.Y)), form);
	}

	private static void CenterByScreen(Screen screen, Form form)
	{
		form.StartPosition = FormStartPosition.Manual;
		form.Location = new Point(screen.Bounds.Left + screen.Bounds.Width / 2 - form.Width / 2, screen.Bounds.Top + screen.Bounds.Height / 2 - form.Height / 2);
	}
}
