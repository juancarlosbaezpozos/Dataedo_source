using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.LoginFormTools.Tools.CustomEventArgs;
using Dataedo.App.LoginFormTools.Tools.Enums;

namespace Dataedo.App.LoginFormTools.UserControls.Base;

public class BasePageUserControl : PageWithLoaderUserControl
{
	public delegate void ActionEventHandler(object sender, ActionEventArgs e);

	private IContainer components;

	public bool HasEventsApplied { get; set; }

	public LoginFormPageEnum.LoginFormPage? CalledFrom { get; set; }

	public bool IsCalledAsPrevious { get; set; }

	public bool SuppressNextAction { get; set; }

	public bool AllowClosing { get; set; }

	public bool BackButtonVisibility { get; set; }

	public bool ForceClean { get; internal set; }

	public event ActionEventHandler Action;

	public event EventHandler RequiresAction;

	public BasePageUserControl()
	{
		InitializeComponent();
	}

	internal virtual void SetParameter(object parameter, bool isCalledAsPrevious)
	{
		IsCalledAsPrevious = isCalledAsPrevious;
		SuppressNextAction = false;
		AllowClosing = true;
	}

	internal virtual async Task<bool> Navigated()
	{
		(FindForm() as LoginFormNew)?.SetUserInfo();
		return true;
	}

	internal Control FindFocusedControl()
	{
		IContainerControl containerControl = this;
		Control control = null;
		while (containerControl != null)
		{
			control = containerControl.ActiveControl;
			containerControl = control as IContainerControl;
		}
		return control;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	protected void OnAction(object sender)
	{
		this.Action?.Invoke(sender, new ActionEventArgs(ActionResultEnum.ActionResult.None));
	}

	protected void OnAction(object sender, ActionEventArgs e)
	{
		this.Action?.Invoke(sender, e);
	}

	protected void OnRequiresAction(object sender)
	{
		this.RequiresAction?.Invoke(sender, EventArgs.Empty);
	}

	protected void OnRequiresAction(object sender, EventArgs e)
	{
		this.RequiresAction?.Invoke(sender, e);
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
