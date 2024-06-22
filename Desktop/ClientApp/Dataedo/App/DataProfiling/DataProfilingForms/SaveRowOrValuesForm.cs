using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;

namespace Dataedo.App.DataProfiling.DataProfilingForms;

public class SaveRowOrValuesForm : BaseXtraForm
{
	private IContainer components;

	private LayoutControl layoutControl1;

	private LayoutControlGroup Root;

	private SimpleButton quitButton;

	private SimpleButton saveButton;

	private CheckBox checkBox2;

	private CheckBox checkBox1;

	private LayoutControlItem layoutControlItem1;

	private EmptySpaceItem emptySpaceItem1;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem layoutControlItem3;

	private LayoutControlItem layoutControlItem4;

	private EmptySpaceItem emptySpaceItem2;

	private LabelControl labelControl1;

	private LayoutControlItem layoutControlItem5;

	public bool SaveRow { get; set; }

	public bool SaveValues { get; set; }

	public SaveRowOrValuesForm()
	{
		InitializeComponent();
		SetStartParameters();
	}

	private void SetStartParameters()
	{
		SaveRow = true;
		SaveValues = true;
	}

	private void QuitButton_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void SaveButton_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void RowDistributionCheckBox_CheckedChanged(object sender, EventArgs e)
	{
		if (SaveRow)
		{
			SaveRow = false;
		}
		else
		{
			SaveRow = true;
		}
	}

	private void ValuesCheckBox_CheckedChanged(object sender, EventArgs e)
	{
		if (SaveValues)
		{
			SaveValues = false;
		}
		else
		{
			SaveValues = true;
		}
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
		this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.quitButton = new DevExpress.XtraEditors.SimpleButton();
		this.saveButton = new DevExpress.XtraEditors.SimpleButton();
		this.checkBox2 = new System.Windows.Forms.CheckBox();
		this.checkBox1 = new System.Windows.Forms.CheckBox();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		base.SuspendLayout();
		this.layoutControl1.Controls.Add(this.labelControl1);
		this.layoutControl1.Controls.Add(this.quitButton);
		this.layoutControl1.Controls.Add(this.saveButton);
		this.layoutControl1.Controls.Add(this.checkBox2);
		this.layoutControl1.Controls.Add(this.checkBox1);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.Root = this.Root;
		this.layoutControl1.Size = new System.Drawing.Size(450, 111);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl1";
		this.labelControl1.Location = new System.Drawing.Point(12, 12);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(145, 13);
		this.labelControl1.StyleController = this.layoutControl1;
		this.labelControl1.TabIndex = 8;
		this.labelControl1.Text = "Select data you want to save:";
		this.quitButton.Location = new System.Drawing.Point(339, 77);
		this.quitButton.Name = "quitButton";
		this.quitButton.Size = new System.Drawing.Size(88, 22);
		this.quitButton.StyleController = this.layoutControl1;
		this.quitButton.TabIndex = 7;
		this.quitButton.Text = "Quit";
		this.quitButton.Click += new System.EventHandler(QuitButton_Click);
		this.saveButton.Location = new System.Drawing.Point(257, 77);
		this.saveButton.Name = "saveButton";
		this.saveButton.Size = new System.Drawing.Size(78, 22);
		this.saveButton.StyleController = this.layoutControl1;
		this.saveButton.TabIndex = 6;
		this.saveButton.Text = "Save";
		this.saveButton.Click += new System.EventHandler(SaveButton_Click);
		this.checkBox2.Checked = true;
		this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBox2.Location = new System.Drawing.Point(12, 53);
		this.checkBox2.Name = "checkBox2";
		this.checkBox2.Size = new System.Drawing.Size(426, 20);
		this.checkBox2.TabIndex = 5;
		this.checkBox2.Text = "Values profiling (Min, Max, Top, …) ";
		this.checkBox2.UseVisualStyleBackColor = true;
		this.checkBox2.CheckedChanged += new System.EventHandler(ValuesCheckBox_CheckedChanged);
		this.checkBox1.Checked = true;
		this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBox1.Location = new System.Drawing.Point(12, 29);
		this.checkBox1.Name = "checkBox1";
		this.checkBox1.Size = new System.Drawing.Size(426, 20);
		this.checkBox1.TabIndex = 4;
		this.checkBox1.Text = "Distribution (Distinct, NonDistinct, Empty, Null)";
		this.checkBox1.UseVisualStyleBackColor = true;
		this.checkBox1.CheckedChanged += new System.EventHandler(RowDistributionCheckBox_CheckedChanged);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[7] { this.layoutControlItem1, this.emptySpaceItem1, this.layoutControlItem2, this.layoutControlItem3, this.layoutControlItem4, this.emptySpaceItem2, this.layoutControlItem5 });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(450, 111);
		this.Root.TextVisible = false;
		this.layoutControlItem1.Control = this.checkBox1;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 17);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(430, 24);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 65);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(245, 26);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.Control = this.checkBox2;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 41);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(430, 24);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlItem3.Control = this.saveButton;
		this.layoutControlItem3.Location = new System.Drawing.Point(245, 65);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(82, 26);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.layoutControlItem4.Control = this.quitButton;
		this.layoutControlItem4.Location = new System.Drawing.Point(327, 65);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(92, 26);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(419, 65);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(11, 26);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.Control = this.labelControl1;
		this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(430, 17);
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(450, 111);
		base.Controls.Add(this.layoutControl1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_16;
		base.IconOptions.LargeImage = Dataedo.App.Properties.Resources.icon_32;
		base.MaximizeBox = false;
		base.Name = "SaveRowOrValuesForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Save";
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		base.ResumeLayout(false);
	}
}
