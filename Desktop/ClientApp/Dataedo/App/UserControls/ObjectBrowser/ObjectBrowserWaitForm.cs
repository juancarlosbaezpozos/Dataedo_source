using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Tools.UI;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraWaitForm;

namespace Dataedo.App.UserControls.ObjectBrowser;

public class ObjectBrowserWaitForm : WaitForm
{
	public enum WaitFormCommand
	{

	}

	public const int FORM_WIDTH = 170;

	public const int FORM_HEIGHT = 20;

	private IContainer components;

	private ProgressPanel progressPanel;

	private TableLayoutPanel tableLayoutPanel1;

	public ObjectBrowserWaitForm()
	{
		InitializeComponent();
		progressPanel.AutoHeight = true;
		base.Width = 170;
		base.Height = 20;
		BackColor = SkinsManager.CurrentSkin.ControlBackColor;
		progressPanel.AppearanceDescription.ForeColor = SkinsManager.CurrentSkin.TextFieldForeColor;
	}

	public override void SetCaption(string caption)
	{
		base.SetCaption(caption);
		progressPanel.Caption = caption;
	}

	public override void SetDescription(string description)
	{
		base.SetDescription(description);
		progressPanel.Description = description;
	}

	public override void ProcessCommand(Enum cmd, object arg)
	{
		base.ProcessCommand(cmd, arg);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
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
		this.progressPanel = new DevExpress.XtraWaitForm.ProgressPanel();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.tableLayoutPanel1.SuspendLayout();
		base.SuspendLayout();
		this.progressPanel.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.progressPanel.Appearance.Options.UseBackColor = true;
		this.progressPanel.AppearanceCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f);
		this.progressPanel.AppearanceCaption.Options.UseFont = true;
		this.progressPanel.AppearanceDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.progressPanel.AppearanceDescription.Options.UseFont = true;
		this.progressPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.progressPanel.Description = "Looking for suggestions...";
		this.progressPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.progressPanel.ImageHorzOffset = 5;
		this.progressPanel.Location = new System.Drawing.Point(0, 1);
		this.progressPanel.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
		this.progressPanel.Name = "progressPanel";
		this.progressPanel.ShowCaption = false;
		this.progressPanel.Size = new System.Drawing.Size(170, 18);
		this.progressPanel.TabIndex = 0;
		this.progressPanel.Text = "progressPanel1";
		this.tableLayoutPanel1.AutoSize = true;
		this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
		this.tableLayoutPanel1.ColumnCount = 1;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel1.Controls.Add(this.progressPanel, 0, 0);
		this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 1;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(170, 20);
		this.tableLayoutPanel1.TabIndex = 1;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		base.ClientSize = new System.Drawing.Size(170, 20);
		base.Controls.Add(this.tableLayoutPanel1);
		this.DoubleBuffered = true;
		base.Name = "ObjectBrowserWaitForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
		this.Text = "Wait";
		this.tableLayoutPanel1.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
