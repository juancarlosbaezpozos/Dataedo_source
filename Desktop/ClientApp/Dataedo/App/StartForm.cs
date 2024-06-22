using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.LoginFormTools;
using Dataedo.App.Tools.UI;
using Dataedo.CustomControls;

namespace Dataedo.App;

public class StartForm : BaseXtraForm
{
	private readonly LoginFormNew loginForm;

	private IContainer components;

	public MainForm MainForm { get; private set; }

	public StartForm(string fileName)
	{
		SkinsManager.SetSkin();
		InitializeComponent();
		loginForm = ((fileName == null) ? new LoginFormNew(this) : new LoginFormNew(this, fileName));
		loginForm.FormClosed += StartForm_FormClosed;
	}

	public void CreateMainForm()
	{
		MainForm = new MainForm(loginForm, loadData: true);
		MainForm.FormClosed += MainForm_FormClosed;
	}

	public void ShowMainForm()
	{
		MainForm.ShowDialog(this, setCustomMessageDefaultOwner: true, showWithOwner: false);
	}

	private void StartForm_FormClosed(object sender, FormClosedEventArgs e)
	{
		MainForm mainForm = MainForm;
		if (mainForm == null || !mainForm.Visible)
		{
			Close();
		}
	}

	private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
	{
		if (StaticData.SignInAfterAfterSignOutInProgress)
		{
			MainForm.Instance = null;
			MainForm.FormClosed -= MainForm_FormClosed;
			MainForm.Dispose();
			MainForm = null;
			loginForm.StartPosition = FormStartPosition.CenterScreen;
			loginForm.TopMost = false;
			loginForm.SignInAfterSignOut();
			loginForm.Show();
		}
		else if (StaticData.ClosingMainFormAfterErrorInProgress)
		{
			MainForm.Instance = null;
			MainForm.FormClosed -= MainForm_FormClosed;
			MainForm.Dispose();
			MainForm = null;
			StaticData.ClosingMainFormAfterErrorInProgress = false;
			loginForm.StartPosition = FormStartPosition.CenterScreen;
			loginForm.TopMost = false;
			loginForm.OpenRecentPage();
			loginForm.Show();
		}
		else
		{
			Close();
		}
	}

	protected override void Dispose(bool disposing)
	{
		loginForm?.Dispose();
		MainForm?.Dispose();
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void StartForm_Shown(object sender, EventArgs e)
	{
		Hide();
		loginForm.Show();
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.StartForm));
		base.SuspendLayout();
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(740, 486);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.IconOptions.Image = (System.Drawing.Image)resources.GetObject("StartForm.IconOptions.Image");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "StartForm";
		base.Opacity = 0.0;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Dataedo";
		base.Shown += new System.EventHandler(StartForm_Shown);
		base.ResumeLayout(false);
	}
}
