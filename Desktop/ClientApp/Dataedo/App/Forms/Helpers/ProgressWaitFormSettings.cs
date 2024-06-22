using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;

namespace Dataedo.App.Forms.Helpers;

public class ProgressWaitFormSettings
{
	public string FormTitle { get; set; }

	public string ProgressLabel { get; set; }

	public Bitmap Picture { get; set; }

	public BackgroundWorker BackgroundWorker { get; set; }

	public CancellationTokenSource CancellationTokenSource { get; set; }

	public Action ButtonAction { get; set; }

	public string ButtonText { get; set; }

	public string ButtonWarning { get; set; }
}
