using System;
using System.Threading;
using System.Windows.Forms;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Helpers;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App;

public static class ProgressWaitFormInvoker
{
	private static bool isSplashScreenShown;

	public static void ShowProgressWaitForm(int max, Form owner, ProgressWaitFormSettings progressWaitFormSettings)
	{
		if (!isSplashScreenShown)
		{
			owner.Invoke((Action)delegate
			{
				SplashScreenManager.ShowForm(owner, typeof(ProgressWaitForm), useFadeIn: true, useFadeOut: true, throwExceptionIfAlreadyOpened: false);
			});
			isSplashScreenShown = true;
			SplashScreenManager.Default.SendCommand(ProgressWaitForm.SplashScreenCommand.SetMax, max);
			SplashScreenManager.Default.SendCommand(ProgressWaitForm.SplashScreenCommand.SetSettings, progressWaitFormSettings);
		}
	}

	public static void ShowMarqueeProgressWaitForm(Form owner, ProgressWaitFormSettings progressWaitFormSettings)
	{
		if (!isSplashScreenShown)
		{
			owner.Invoke((Action)delegate
			{
				SplashScreenManager.ShowForm(owner, typeof(ProgressWaitForm), useFadeIn: true, useFadeOut: true, throwExceptionIfAlreadyOpened: false);
			});
			isSplashScreenShown = true;
			SplashScreenManager.Default.SendCommand(ProgressWaitForm.SplashScreenCommand.SwitchToMarquee, null);
			SplashScreenManager.Default.SendCommand(ProgressWaitForm.SplashScreenCommand.SetSettings, progressWaitFormSettings);
		}
	}

	public static void SwitchWaitFormToMarquee(string newLabel = null)
	{
		if (isSplashScreenShown)
		{
			SplashScreenManager.Default.SendCommand(ProgressWaitForm.SplashScreenCommand.SwitchToMarquee, null);
			if (!string.IsNullOrWhiteSpace(newLabel))
			{
				SplashScreenManager.Default.SendCommand(ProgressWaitForm.SplashScreenCommand.SetLabel, newLabel);
			}
		}
	}

	public static void ChangeWaitFormLabel(string newLabel = null)
	{
		if (isSplashScreenShown && !string.IsNullOrWhiteSpace(newLabel))
		{
			SplashScreenManager.Default.SendCommand(ProgressWaitForm.SplashScreenCommand.SetLabel, newLabel);
		}
	}

	public static void SwitchWaitFormToProgress(int max, string newLabel = null)
	{
		if (isSplashScreenShown)
		{
			SplashScreenManager.Default.SendCommand(ProgressWaitForm.SplashScreenCommand.SetMax, max);
			SplashScreenManager.Default.SendCommand(ProgressWaitForm.SplashScreenCommand.SwitchToProgress, null);
			if (!string.IsNullOrWhiteSpace(newLabel))
			{
				SplashScreenManager.Default.SendCommand(ProgressWaitForm.SplashScreenCommand.SetLabel, newLabel);
			}
		}
	}

	public static void StepProgressWaitForm(string newLabel = null)
	{
		if (isSplashScreenShown)
		{
			SplashScreenManager.Default.SendCommand(ProgressWaitForm.SplashScreenCommand.PerformStep, null);
			if (!string.IsNullOrWhiteSpace(newLabel))
			{
				SplashScreenManager.Default.SendCommand(ProgressWaitForm.SplashScreenCommand.SetLabel, newLabel);
			}
		}
	}

	public static void SetProgressWaitFormCancellationToken(CancellationTokenSource cancellationTokenSource)
	{
		if (isSplashScreenShown)
		{
			SplashScreenManager.Default.SendCommand(ProgressWaitForm.SplashScreenCommand.SetCancellationToken, cancellationTokenSource);
		}
	}

	public static void SetAfterError(string newLabel = null, Action buttonAction = null)
	{
		if (isSplashScreenShown)
		{
			SplashScreenManager.Default.SendCommand(ProgressWaitForm.SplashScreenCommand.SetAfterError, newLabel);
			if (!string.IsNullOrWhiteSpace(newLabel))
			{
				SplashScreenManager.Default.SendCommand(ProgressWaitForm.SplashScreenCommand.SetLabel, newLabel);
			}
			if (buttonAction != null)
			{
				SplashScreenManager.Default.SendCommand(ProgressWaitForm.SplashScreenCommand.SetBottomButtonAction, buttonAction);
			}
		}
	}

	public static void CloseProgressWaitForm()
	{
		if (isSplashScreenShown)
		{
			isSplashScreenShown = false;
			SplashScreenManager.CloseForm(throwExceptionIfAlreadyClosed: false);
		}
	}
}
