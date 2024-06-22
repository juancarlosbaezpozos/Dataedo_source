using System.Windows.Forms;

namespace Dataedo.App.Tools.ERD.Canvas.CanvasEventHandlers;

public class CMouseEventArgs
{
	public readonly int X;

	public readonly int Y;

	public readonly MouseButtons Button;

	public readonly bool Ctrl;

	public CMouseEventArgs(int x, int y, MouseButtons button, bool ctrl = false)
	{
		X = x;
		Y = y;
		Button = button;
		Ctrl = ctrl;
	}
}
