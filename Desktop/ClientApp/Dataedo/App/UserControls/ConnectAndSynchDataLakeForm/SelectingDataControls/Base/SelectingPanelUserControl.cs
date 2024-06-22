using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.UserControls.Base;
using DevExpress.Utils.Layout;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base;

public class SelectingPanelUserControl : BaseUserControl
{
	private IContainer components;

	private SimpleButton leftAllSimpleButton;

	private SimpleButton leftSingleSimpleButton;

	private SimpleButton rightSingleSimpleButton;

	private TablePanel buttonsTablePanel;

	private SimpleButton rightAllSimpleButton;

	public event EventHandler MoveAllRightClick;

	public event EventHandler MoveSingleRightClick;

	public event EventHandler MoveAllLeftClick;

	public event EventHandler MoveSingleLeftClick;

	public SelectingPanelUserControl()
	{
		InitializeComponent();
	}

	private void rightAllSimpleButton_Click(object sender, EventArgs e)
	{
		this.MoveAllRightClick?.Invoke(this, e);
	}

	private void rightSingleSimpleButton_Click(object sender, EventArgs e)
	{
		this.MoveSingleRightClick?.Invoke(this, e);
	}

	private void leftSingleSimpleButton_Click(object sender, EventArgs e)
	{
		this.MoveAllLeftClick?.Invoke(this, e);
	}

	private void leftAllSimpleButton_Click(object sender, EventArgs e)
	{
		this.MoveSingleLeftClick?.Invoke(this, e);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base.SelectingPanelUserControl));
		this.leftAllSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.leftSingleSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.rightSingleSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.buttonsTablePanel = new DevExpress.Utils.Layout.TablePanel();
		this.rightAllSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.buttonsTablePanel).BeginInit();
		this.buttonsTablePanel.SuspendLayout();
		base.SuspendLayout();
		this.leftAllSimpleButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.buttonsTablePanel.SetColumn(this.leftAllSimpleButton, 0);
		this.leftAllSimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.leftAllSimpleButton.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("leftAllSimpleButton.ImageOptions.SvgImage");
		this.leftAllSimpleButton.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
		this.leftAllSimpleButton.Location = new System.Drawing.Point(3, 309);
		this.leftAllSimpleButton.Name = "leftAllSimpleButton";
		this.buttonsTablePanel.SetRow(this.leftAllSimpleButton, 4);
		this.leftAllSimpleButton.Size = new System.Drawing.Size(90, 34);
		this.leftAllSimpleButton.TabIndex = 4;
		this.leftAllSimpleButton.Text = "simpleButton1";
		this.leftAllSimpleButton.Click += new System.EventHandler(leftAllSimpleButton_Click);
		this.leftSingleSimpleButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.buttonsTablePanel.SetColumn(this.leftSingleSimpleButton, 0);
		this.leftSingleSimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.leftSingleSimpleButton.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("leftSingleSimpleButton.ImageOptions.SvgImage");
		this.leftSingleSimpleButton.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
		this.leftSingleSimpleButton.Location = new System.Drawing.Point(3, 269);
		this.leftSingleSimpleButton.Name = "leftSingleSimpleButton";
		this.buttonsTablePanel.SetRow(this.leftSingleSimpleButton, 3);
		this.leftSingleSimpleButton.Size = new System.Drawing.Size(90, 34);
		this.leftSingleSimpleButton.TabIndex = 4;
		this.leftSingleSimpleButton.Text = "simpleButton1";
		this.leftSingleSimpleButton.Click += new System.EventHandler(leftSingleSimpleButton_Click);
		this.rightSingleSimpleButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.buttonsTablePanel.SetColumn(this.rightSingleSimpleButton, 0);
		this.rightSingleSimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.rightSingleSimpleButton.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("rightSingleSimpleButton.ImageOptions.SvgImage");
		this.rightSingleSimpleButton.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
		this.rightSingleSimpleButton.Location = new System.Drawing.Point(3, 229);
		this.rightSingleSimpleButton.Name = "rightSingleSimpleButton";
		this.buttonsTablePanel.SetRow(this.rightSingleSimpleButton, 2);
		this.rightSingleSimpleButton.Size = new System.Drawing.Size(90, 34);
		this.rightSingleSimpleButton.TabIndex = 4;
		this.rightSingleSimpleButton.Text = "simpleButton1";
		this.rightSingleSimpleButton.Click += new System.EventHandler(rightSingleSimpleButton_Click);
		this.buttonsTablePanel.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 5f));
		this.buttonsTablePanel.Controls.Add(this.rightAllSimpleButton);
		this.buttonsTablePanel.Controls.Add(this.rightSingleSimpleButton);
		this.buttonsTablePanel.Controls.Add(this.leftSingleSimpleButton);
		this.buttonsTablePanel.Controls.Add(this.leftAllSimpleButton);
		this.buttonsTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.buttonsTablePanel.Location = new System.Drawing.Point(0, 0);
		this.buttonsTablePanel.Name = "buttonsTablePanel";
		this.buttonsTablePanel.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 26f), new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 26f), new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 26f), new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 26f), new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 26f), new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 26f));
		this.buttonsTablePanel.Size = new System.Drawing.Size(96, 531);
		this.buttonsTablePanel.TabIndex = 18;
		this.rightAllSimpleButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.buttonsTablePanel.SetColumn(this.rightAllSimpleButton, 0);
		this.rightAllSimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.rightAllSimpleButton.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("rightAllSimpleButton.ImageOptions.SvgImage");
		this.rightAllSimpleButton.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
		this.rightAllSimpleButton.Location = new System.Drawing.Point(3, 189);
		this.rightAllSimpleButton.Name = "rightAllSimpleButton";
		this.buttonsTablePanel.SetRow(this.rightAllSimpleButton, 1);
		this.rightAllSimpleButton.Size = new System.Drawing.Size(90, 34);
		this.rightAllSimpleButton.TabIndex = 5;
		this.rightAllSimpleButton.Text = "simpleButton1";
		this.rightAllSimpleButton.Click += new System.EventHandler(rightAllSimpleButton_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.buttonsTablePanel);
		base.Name = "SelectingPanelUserControl";
		base.Size = new System.Drawing.Size(96, 531);
		((System.ComponentModel.ISupportInitialize)this.buttonsTablePanel).EndInit();
		this.buttonsTablePanel.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
