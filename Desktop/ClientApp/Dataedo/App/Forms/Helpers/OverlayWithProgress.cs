using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.Forms.Helpers;

public class OverlayWithProgress
{
	private static IOverlaySplashScreenHandle overlayHandle;

	private static OverlayTextPainterEx title;

	private static OverlayTextPainterEx subtitleLabel;

	private static OverlayTextPainterEx progressLabel;

	private static int current;

	private static int count;

	public static void Show(string _title, Control control, int _count = 0)
	{
		if (control != null && (!(control is GridControl) || (control as GridControl).IsLoaded))
		{
			current = 0;
			count = _count;
			title = new OverlayTextPainterEx(TextPosition.Title, _title);
			subtitleLabel = new OverlayTextPainterEx(TextPosition.Subtitle);
			progressLabel = new OverlayTextPainterEx(TextPosition.Percentage);
			IOverlayWindowPainter customPainter = new OverlayWindowCompositePainter(title, subtitleLabel, progressLabel);
			overlayHandle = SplashScreenManager.ShowOverlayForm(control, null, null, null, null, null, null, customPainter);
		}
	}

	public static void Show(string _title, Control control, OverlayImagePainter button, int _count = 0)
	{
		if (control != null && (!(control is GridControl) || (control as GridControl).IsLoaded))
		{
			current = 0;
			count = _count;
			title = new OverlayTextPainterEx(TextPosition.Title, _title);
			subtitleLabel = new OverlayTextPainterEx(TextPosition.Subtitle);
			progressLabel = new OverlayTextPainterEx(TextPosition.Percentage);
			bool? fadeIn = true;
			bool? fadeOut = true;
			int? opacity = 75;
			IOverlayWindowPainter customPainter = new OverlayWindowCompositePainter(title, subtitleLabel, progressLabel, button);
			overlayHandle = SplashScreenManager.ShowOverlayForm(control, fadeIn, fadeOut, null, null, opacity, null, customPainter);
		}
	}

	public static void SetProgressMax(int _count)
	{
		count = _count;
		current = 0;
		progressLabel.Text = string.Empty;
	}

	public static void ClearProgress()
	{
		progressLabel.Text = string.Empty;
		current = 0;
	}

	public static void UpdateStatusProgress()
	{
		current++;
		progressLabel.Text = $"{(float)current / (float)count * 100f:F0}%";
	}

	public static void SetSubtitle(string subtitle)
	{
		subtitleLabel.Text = subtitle;
	}

	public static void SetProgressLabel(string stageInfo)
	{
		progressLabel.Text = stageInfo;
	}

	public static void Close()
	{
		if (overlayHandle != null)
		{
			SplashScreenManager.CloseOverlayForm(overlayHandle);
		}
		overlayHandle = null;
		title = null;
		subtitleLabel = null;
		progressLabel = null;
	}
}
