using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Dataedo.App.LoginFormTools.UserControls.Common;

public class SmallLogoUserControl : XtraUserControl
{
	private IContainer components;

	private PictureEdit logoPictureEdit;

	public SmallLogoUserControl()
	{
		InitializeComponent();
	}

	private static void OpenDataedoLink(MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			Links.OpenLink(Links.Dataedo);
		}
	}

	private void SmallLogoUserControl_Load(object sender, EventArgs e)
	{
		if (!base.DesignMode && base.ParentForm != null)
		{
			logoPictureEdit.BackColor = base.ParentForm.BackColor;
		}
	}

	private void LogoPictureEdit_MouseClick(object sender, MouseEventArgs e)
	{
		OpenDataedoLink(e);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.logoPictureEdit = new DevExpress.XtraEditors.PictureEdit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEdit.Properties).BeginInit();
		base.SuspendLayout();
		this.logoPictureEdit.Cursor = System.Windows.Forms.Cursors.Hand;
		this.logoPictureEdit.EditValue = Dataedo.App.Properties.Resources.loginform_logo;
		this.logoPictureEdit.Location = new System.Drawing.Point(0, 0);
		this.logoPictureEdit.Margin = new System.Windows.Forms.Padding(0);
		this.logoPictureEdit.Name = "logoPictureEdit";
		this.logoPictureEdit.Properties.AllowFocused = false;
		this.logoPictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.logoPictureEdit.Properties.PictureAlignment = System.Drawing.ContentAlignment.MiddleLeft;
		this.logoPictureEdit.Properties.ReadOnly = true;
		this.logoPictureEdit.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
		this.logoPictureEdit.Properties.ShowMenu = false;
		this.logoPictureEdit.Size = new System.Drawing.Size(93, 24);
		this.logoPictureEdit.TabIndex = 8;
		this.logoPictureEdit.MouseClick += new System.Windows.Forms.MouseEventHandler(LogoPictureEdit_MouseClick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.logoPictureEdit);
		base.Margin = new System.Windows.Forms.Padding(0);
		this.MaximumSize = new System.Drawing.Size(93, 24);
		this.MinimumSize = new System.Drawing.Size(93, 24);
		base.Name = "SmallLogoUserControl";
		base.Size = new System.Drawing.Size(93, 24);
		base.Load += new System.EventHandler(SmallLogoUserControl_Load);
		((System.ComponentModel.ISupportInitialize)this.logoPictureEdit.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
