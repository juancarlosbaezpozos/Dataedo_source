using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.LoginFormTools.Tools.Common;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.LoginFormTools.UserControls.Base;

public class PageWithLoaderUserControl : XtraUserControl
{
	private Control loaderContext;

	private OverlayWindowOptions overlayWindowOptions;

	private IOverlaySplashScreenHandle overlaySplashScreenHandle;

	private bool showLoaderWaiting;

	private IContainer components;

	protected Control LoaderContext
	{
		get
		{
			return loaderContext;
		}
		set
		{
			if (value != loaderContext)
			{
				loaderContext = value;
				if (loaderContext == null)
				{
					overlayWindowOptions = LoaderTools.GetOverlayWindowOptions(null);
				}
				else
				{
					overlayWindowOptions = LoaderTools.GetOverlayWindowOptions(loaderContext);
				}
			}
		}
	}

	public event EventHandler TimeConsumingOperationStarted;

	public event EventHandler TimeConsumingOperationStopped;

	public PageWithLoaderUserControl()
	{
		InitializeComponent();
		overlayWindowOptions = LoaderTools.GetOverlayWindowOptions();
	}

	public void ShowLoader()
	{
		HideLoader();
		try
		{
			Form parentForm = base.ParentForm;
			if (parentForm != null && parentForm.Visible)
			{
				Control control = LoaderContext ?? this;
				if (control.Visible && control.IsHandleCreated)
				{
					overlaySplashScreenHandle = SplashScreenManager.ShowOverlayForm(LoaderContext ?? this, overlayWindowOptions);
					showLoaderWaiting = false;
				}
			}
			else
			{
				showLoaderWaiting = true;
			}
		}
		catch
		{
		}
	}

	public void CheckLoaderVisibility()
	{
		if (showLoaderWaiting)
		{
			ShowLoader();
		}
	}

	public void HideLoader()
	{
		showLoaderWaiting = false;
		if (overlaySplashScreenHandle != null)
		{
			SplashScreenManager.CloseOverlayForm(overlaySplashScreenHandle);
		}
	}

	protected override void Dispose(bool disposing)
	{
		overlaySplashScreenHandle?.Dispose();
		if (disposing && components != null)
		{
			components.Dispose();
		}
	}

	protected void OnTimeConsumingOperationStarted(object sender)
	{
		this.TimeConsumingOperationStarted?.Invoke(sender, EventArgs.Empty);
	}

	protected void OnTimeConsumingOperationStarted(object sender, EventArgs e)
	{
		this.TimeConsumingOperationStarted?.Invoke(sender, e);
	}

	protected void OnTimeConsumingOperationStopped(object sender)
	{
		this.TimeConsumingOperationStopped?.Invoke(sender, EventArgs.Empty);
	}

	protected void OnTimeConsumingOperationStopped(object sender, EventArgs e)
	{
		this.TimeConsumingOperationStopped?.Invoke(sender, e);
	}

	private void InitializeComponent()
	{
		base.SuspendLayout();
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.MaximumSize = new System.Drawing.Size(700, 470);
		this.MinimumSize = new System.Drawing.Size(700, 470);
		base.Name = "BasePageUserControl";
		base.Size = new System.Drawing.Size(700, 470);
		base.ResumeLayout(false);
	}
}
