using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Forms;

public class CheckedListForm : BaseXtraForm
{
	private Func<IEnumerable<string>, bool> validateValueFunction;

	private IContainer components;

	private NonCustomizableLayoutControl layoutControl;

	private LayoutControlGroup layoutControlGroup;

	private SimpleButton cancelSimpleButton;

	private SimpleButton okSimpleButton;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem2;

	private CheckedListBoxControl checkedListBoxControl;

	private LayoutControlItem layoutControlItem4;

	private HyperLinkEdit selectNoneHyperLinkEdit;

	private HyperLinkEdit selectAllHyperLinkEdit;

	private LayoutControlItem layoutControlItem1;

	private LayoutControlItem layoutControlItem5;

	private EmptySpaceItem emptySpaceItem3;

	public IEnumerable<string> CheckedValues => checkedListBoxControl.CheckedItems.Cast<string>();

	public CheckedListForm(IEnumerable<string> dataSource = null, IEnumerable<string> checkedItems = null, string title = null, Func<IEnumerable<string>, bool> validateValueFunction = null)
	{
		InitializeComponent();
		this.validateValueFunction = validateValueFunction;
		checkedListBoxControl.BeginUpdate();
		checkedListBoxControl.DataSource = dataSource;
		checkedListBoxControl.EndUpdate();
		Text = title;
		if (dataSource == null || checkedItems == null)
		{
			return;
		}
		for (int i = 0; i < dataSource.Count(); i++)
		{
			string text = dataSource.ElementAt(i);
			foreach (string checkedItem in checkedItems)
			{
				if (text != null && checkedItem != null && text.ToLower() == checkedItem.Trim().ToLower())
				{
					checkedListBoxControl.SetItemChecked(i, value: true);
				}
			}
		}
	}

	private void ManageSelection(bool select)
	{
		if (checkedListBoxControl.DataSource != null)
		{
			for (int i = 0; i < (checkedListBoxControl.DataSource as IEnumerable<string>).Count(); i++)
			{
				checkedListBoxControl.SetItemChecked(i, select);
			}
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Escape)
		{
			Close();
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void SelectAllHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		ManageSelection(select: true);
	}

	private void SelectNoneHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		ManageSelection(select: false);
	}

	private void OkSimpleButton_Click(object sender, EventArgs e)
	{
		if (validateValueFunction == null || validateValueFunction(CheckedValues))
		{
			base.DialogResult = DialogResult.OK;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.CheckedListForm));
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.selectNoneHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.selectAllHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.checkedListBoxControl = new DevExpress.XtraEditors.CheckedListBoxControl();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.okSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.selectNoneHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.selectAllHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkedListBoxControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		base.SuspendLayout();
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.Controls.Add(this.selectNoneHyperLinkEdit);
		this.layoutControl.Controls.Add(this.selectAllHyperLinkEdit);
		this.layoutControl.Controls.Add(this.checkedListBoxControl);
		this.layoutControl.Controls.Add(this.cancelSimpleButton);
		this.layoutControl.Controls.Add(this.okSimpleButton);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(668, 178, 250, 350);
		this.layoutControl.Root = this.layoutControlGroup;
		this.layoutControl.Size = new System.Drawing.Size(284, 262);
		this.layoutControl.TabIndex = 0;
		this.layoutControl.Text = "layoutControl";
		this.selectNoneHyperLinkEdit.EditValue = "Select none";
		this.selectNoneHyperLinkEdit.Location = new System.Drawing.Point(66, 2);
		this.selectNoneHyperLinkEdit.MaximumSize = new System.Drawing.Size(70, 0);
		this.selectNoneHyperLinkEdit.MinimumSize = new System.Drawing.Size(70, 0);
		this.selectNoneHyperLinkEdit.Name = "selectNoneHyperLinkEdit";
		this.selectNoneHyperLinkEdit.Properties.AllowFocused = false;
		this.selectNoneHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.selectNoneHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.selectNoneHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.selectNoneHyperLinkEdit.Size = new System.Drawing.Size(70, 18);
		this.selectNoneHyperLinkEdit.StyleController = this.layoutControl;
		this.selectNoneHyperLinkEdit.TabIndex = 9;
		this.selectNoneHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(SelectNoneHyperLinkEdit_OpenLink);
		this.selectAllHyperLinkEdit.EditValue = "Select all";
		this.selectAllHyperLinkEdit.Location = new System.Drawing.Point(2, 2);
		this.selectAllHyperLinkEdit.MaximumSize = new System.Drawing.Size(60, 0);
		this.selectAllHyperLinkEdit.MinimumSize = new System.Drawing.Size(60, 0);
		this.selectAllHyperLinkEdit.Name = "selectAllHyperLinkEdit";
		this.selectAllHyperLinkEdit.Properties.AllowFocused = false;
		this.selectAllHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.selectAllHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.selectAllHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.selectAllHyperLinkEdit.Size = new System.Drawing.Size(60, 18);
		this.selectAllHyperLinkEdit.StyleController = this.layoutControl;
		this.selectAllHyperLinkEdit.TabIndex = 8;
		this.selectAllHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(SelectAllHyperLinkEdit_OpenLink);
		this.checkedListBoxControl.CheckOnClick = true;
		this.checkedListBoxControl.Location = new System.Drawing.Point(2, 26);
		this.checkedListBoxControl.Name = "checkedListBoxControl";
		this.checkedListBoxControl.Size = new System.Drawing.Size(280, 208);
		this.checkedListBoxControl.StyleController = this.layoutControl;
		this.checkedListBoxControl.TabIndex = 7;
		this.cancelSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelSimpleButton.Location = new System.Drawing.Point(202, 238);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.cancelSimpleButton.StyleController = this.layoutControl;
		this.cancelSimpleButton.TabIndex = 6;
		this.cancelSimpleButton.Text = "Cancel";
		this.okSimpleButton.Location = new System.Drawing.Point(106, 238);
		this.okSimpleButton.Name = "okSimpleButton";
		this.okSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.okSimpleButton.StyleController = this.layoutControl;
		this.okSimpleButton.TabIndex = 5;
		this.okSimpleButton.Text = "OK";
		this.okSimpleButton.Click += new System.EventHandler(OkSimpleButton_Click);
		this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup.GroupBordersVisible = false;
		this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[8] { this.layoutControlItem2, this.layoutControlItem3, this.emptySpaceItem1, this.emptySpaceItem2, this.layoutControlItem4, this.layoutControlItem1, this.layoutControlItem5, this.emptySpaceItem3 });
		this.layoutControlGroup.Name = "Root";
		this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup.Size = new System.Drawing.Size(284, 262);
		this.layoutControlGroup.TextVisible = false;
		this.layoutControlItem2.Control = this.okSimpleButton;
		this.layoutControlItem2.Location = new System.Drawing.Point(104, 236);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(84, 26);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlItem3.Control = this.cancelSimpleButton;
		this.layoutControlItem3.Location = new System.Drawing.Point(200, 236);
		this.layoutControlItem3.MaxSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.MinSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 236);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(104, 26);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(188, 236);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(12, 26);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(12, 26);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.emptySpaceItem2.Size = new System.Drawing.Size(12, 26);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.Control = this.checkedListBoxControl;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 24);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(284, 212);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.layoutControlItem1.Control = this.selectAllHyperLinkEdit;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(64, 24);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem5.Control = this.selectNoneHyperLinkEdit;
		this.layoutControlItem5.Location = new System.Drawing.Point(64, 0);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(74, 24);
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(138, 0);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(146, 24);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(146, 24);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(146, 24);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(284, 262);
		base.Controls.Add(this.layoutControl);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("CheckedListForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Name = "CheckedListForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "CheckedListForm";
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.selectNoneHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.selectAllHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkedListBoxControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		base.ResumeLayout(false);
	}
}
