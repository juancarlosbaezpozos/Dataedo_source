using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls.SummaryControls;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Forms;

public class ColumnsForm : BaseXtraForm
{
	private IContainer components;

	private ColumnsUserControl columnsUserControl;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem layoutControlItem1;

	private SimpleButton cancelSimpleButton;

	private SimpleButton saveSimpleButton;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem2;

	private EmptySpaceItem emptySpaceItem3;

	private EmptySpaceItem emptySpaceItem4;

	private NonCustomizableLayoutControl layoutControl1;

	public ColumnRow VisibleRow => columnsUserControl.VisibleRow;

	public ColumnsForm()
	{
		InitializeComponent();
	}

	public void SetColumns()
	{
		columnsUserControl.SetDataSourceByRow();
	}

	public void SetParameters(CustomFieldsSupport customFieldsSupport, Action editCustomFields, ColumnRow visibleRow, string columnName)
	{
		columnsUserControl.SetParameters(customFieldsSupport, editCustomFields, columnName);
		columnsUserControl.VisibleRow = visibleRow;
		Text = columnName + " columns";
	}

	private void saveSimpleButton_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void cancelSimpleButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.S | Keys.Control:
			base.DialogResult = DialogResult.OK;
			Close();
			break;
		case Keys.Escape:
			Close();
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void ColumnsForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (!columnsUserControl.IsChanged)
		{
			return;
		}
		if (base.DialogResult == DialogResult.OK)
		{
			columnsUserControl.Save();
			return;
		}
		DialogResult? dialogResult = GeneralMessageBoxesHandling.Show("Table has been changed, would you like to save these changes?", "Table has been changed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, this).DialogResult;
		if (dialogResult == DialogResult.Yes)
		{
			base.DialogResult = DialogResult.OK;
			columnsUserControl.Save();
		}
		else if (dialogResult != DialogResult.No)
		{
			base.DialogResult = DialogResult.Cancel;
			e.Cancel = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.ColumnsForm));
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.saveSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.columnsUserControl = new Dataedo.App.UserControls.SummaryControls.ColumnsUserControl();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		base.SuspendLayout();
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl1.Controls.Add(this.cancelSimpleButton);
		this.layoutControl1.Controls.Add(this.saveSimpleButton);
		this.layoutControl1.Controls.Add(this.columnsUserControl);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(1145, 561);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl1";
		this.cancelSimpleButton.Location = new System.Drawing.Point(1048, 527);
		this.cancelSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.StyleController = this.layoutControl1;
		this.cancelSimpleButton.TabIndex = 6;
		this.cancelSimpleButton.Text = "Cancel";
		this.cancelSimpleButton.Click += new System.EventHandler(cancelSimpleButton_Click);
		this.saveSimpleButton.Location = new System.Drawing.Point(949, 527);
		this.saveSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.saveSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.saveSimpleButton.Name = "saveSimpleButton";
		this.saveSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.saveSimpleButton.StyleController = this.layoutControl1;
		this.saveSimpleButton.TabIndex = 5;
		this.saveSimpleButton.Text = "Save";
		this.saveSimpleButton.Click += new System.EventHandler(saveSimpleButton_Click);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[7] { this.layoutControlItem1, this.layoutControlItem2, this.layoutControlItem3, this.emptySpaceItem1, this.emptySpaceItem2, this.emptySpaceItem3, this.emptySpaceItem4 });
		this.layoutControlGroup1.Name = "layoutControlGroup1";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(1145, 561);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem2.Control = this.saveSimpleButton;
		this.layoutControlItem2.Location = new System.Drawing.Point(947, 525);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(89, 26);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlItem3.Control = this.cancelSimpleButton;
		this.layoutControlItem3.Location = new System.Drawing.Point(1046, 525);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(89, 26);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 525);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(104, 24);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(947, 26);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(1036, 525);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(10, 26);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(0, 551);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(0, 10);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(104, 10);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(1145, 10);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(1135, 525);
		this.emptySpaceItem4.MaxSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem4.MinSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(10, 26);
		this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.columnsUserControl.BackColor = System.Drawing.Color.Transparent;
		this.columnsUserControl.Location = new System.Drawing.Point(2, 2);
		this.columnsUserControl.Name = "columnsUserControl";
		this.columnsUserControl.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
		this.columnsUserControl.Size = new System.Drawing.Size(1141, 521);
		this.columnsUserControl.TabIndex = 4;
		this.columnsUserControl.VisibleRow = null;
		this.layoutControlItem1.Control = this.columnsUserControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(1145, 525);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1145, 561);
		base.Controls.Add(this.layoutControl1);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("ColumnsForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Name = "ColumnsForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Columns";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ColumnsForm_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		base.ResumeLayout(false);
	}
}
