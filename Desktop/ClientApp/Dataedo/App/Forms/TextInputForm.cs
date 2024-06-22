using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Forms;

public class TextInputForm : BaseXtraForm
{
	private readonly bool allowEmptyValue;

	private IContainer components;

	private NonCustomizableLayoutControl layoutControl;

	private LayoutControlGroup layoutControlGroup;

	private SimpleButton cancelSimpleButton;

	private SimpleButton okSimpleButton;

	private LayoutControlItem okBtnLayoutControlItem;

	private LayoutControlItem cancelBtnLayoutControlItem;

	private EmptySpaceItem emptySpaceItem;

	private CustomTextEdit customTextEdit;

	private LayoutControlItem textEditLayoutControlItem;

	public string SelectedValue => customTextEdit.Text;

	public TextInputForm(string title, string caption, string startValue = null, int? maxLength = null, string leftBtnText = "OK", string rightBtnText = "Cancel", Form owner = null, bool allowEmptyValue = true)
	{
		InitializeComponent();
		Text = title;
		textEditLayoutControlItem.Text = caption;
		if (!string.IsNullOrWhiteSpace(startValue))
		{
			customTextEdit.Text = startValue;
		}
		if (owner != null)
		{
			base.Owner = owner;
		}
		if (maxLength.HasValue)
		{
			customTextEdit.Properties.MaxLength = maxLength.Value;
		}
		okSimpleButton.Text = leftBtnText;
		cancelSimpleButton.Text = rightBtnText;
		this.allowEmptyValue = allowEmptyValue;
		if (!this.allowEmptyValue && string.IsNullOrWhiteSpace(SelectedValue))
		{
			okSimpleButton.Enabled = false;
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Escape:
			base.DialogResult = DialogResult.Cancel;
			Close();
			break;
		case Keys.Return:
			base.DialogResult = DialogResult.OK;
			Close();
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	public static string ShowForm(string title, string caption, string startValue = null, int? maxLength = null, string leftBtnText = "OK", string rightBtnText = "Cancel", Form owner = null, bool allowEmptyValue = true)
	{
		using TextInputForm textInputForm = new TextInputForm(title, caption, startValue, maxLength, leftBtnText, rightBtnText, owner, allowEmptyValue);
		if (textInputForm.ShowDialog() == DialogResult.OK)
		{
			return textInputForm.SelectedValue?.Trim();
		}
		return null;
	}

	private void CustomTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (!allowEmptyValue)
		{
			okSimpleButton.Enabled = !string.IsNullOrWhiteSpace(SelectedValue);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.TextInputForm));
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.customTextEdit = new Dataedo.App.UserControls.CustomTextEdit();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.okSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.okBtnLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cancelBtnLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.textEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.customTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.okBtnLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cancelBtnLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEditLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl.Controls.Add(this.customTextEdit);
		this.layoutControl.Controls.Add(this.cancelSimpleButton);
		this.layoutControl.Controls.Add(this.okSimpleButton);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(668, 178, 250, 350);
		this.layoutControl.Root = this.layoutControlGroup;
		this.layoutControl.Size = new System.Drawing.Size(284, 72);
		this.layoutControl.TabIndex = 0;
		this.layoutControl.Text = "layoutControl";
		this.customTextEdit.Location = new System.Drawing.Point(84, 8);
		this.customTextEdit.Margin = new System.Windows.Forms.Padding(2);
		this.customTextEdit.Name = "customTextEdit";
		this.customTextEdit.Size = new System.Drawing.Size(185, 20);
		this.customTextEdit.StyleController = this.layoutControl;
		this.customTextEdit.TabIndex = 7;
		this.customTextEdit.EditValueChanged += new System.EventHandler(CustomTextEdit_EditValueChanged);
		this.cancelSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelSimpleButton.Location = new System.Drawing.Point(195, 38);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.cancelSimpleButton.StyleController = this.layoutControl;
		this.cancelSimpleButton.TabIndex = 6;
		this.cancelSimpleButton.Text = "Cancel";
		this.okSimpleButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.okSimpleButton.Location = new System.Drawing.Point(105, 38);
		this.okSimpleButton.Name = "okSimpleButton";
		this.okSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.okSimpleButton.StyleController = this.layoutControl;
		this.okSimpleButton.TabIndex = 5;
		this.okSimpleButton.Text = "OK";
		this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup.GroupBordersVisible = false;
		this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.okBtnLayoutControlItem, this.cancelBtnLayoutControlItem, this.emptySpaceItem, this.textEditLayoutControlItem });
		this.layoutControlGroup.Name = "Root";
		this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup.Size = new System.Drawing.Size(284, 72);
		this.layoutControlGroup.TextVisible = false;
		this.okBtnLayoutControlItem.Control = this.okSimpleButton;
		this.okBtnLayoutControlItem.Location = new System.Drawing.Point(104, 36);
		this.okBtnLayoutControlItem.MaxSize = new System.Drawing.Size(90, 26);
		this.okBtnLayoutControlItem.MinSize = new System.Drawing.Size(90, 26);
		this.okBtnLayoutControlItem.Name = "okBtnLayoutControlItem";
		this.okBtnLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(1, 9, 2, 2);
		this.okBtnLayoutControlItem.Size = new System.Drawing.Size(90, 36);
		this.okBtnLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.okBtnLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.okBtnLayoutControlItem.TextVisible = false;
		this.cancelBtnLayoutControlItem.Control = this.cancelSimpleButton;
		this.cancelBtnLayoutControlItem.Location = new System.Drawing.Point(194, 36);
		this.cancelBtnLayoutControlItem.MaxSize = new System.Drawing.Size(90, 26);
		this.cancelBtnLayoutControlItem.MinSize = new System.Drawing.Size(90, 26);
		this.cancelBtnLayoutControlItem.Name = "cancelBtnLayoutControlItem";
		this.cancelBtnLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(1, 9, 2, 2);
		this.cancelBtnLayoutControlItem.Size = new System.Drawing.Size(90, 36);
		this.cancelBtnLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cancelBtnLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.cancelBtnLayoutControlItem.TextVisible = false;
		this.emptySpaceItem.AllowHotTrack = false;
		this.emptySpaceItem.Location = new System.Drawing.Point(0, 36);
		this.emptySpaceItem.Name = "emptySpaceItem";
		this.emptySpaceItem.Size = new System.Drawing.Size(104, 36);
		this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.textEditLayoutControlItem.Control = this.customTextEdit;
		this.textEditLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.textEditLayoutControlItem.Name = "textEditLayoutControlItem";
		this.textEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(15, 15, 8, 8);
		this.textEditLayoutControlItem.Size = new System.Drawing.Size(284, 36);
		this.textEditLayoutControlItem.Text = "Caption:";
		this.textEditLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
		this.textEditLayoutControlItem.TextSize = new System.Drawing.Size(39, 13);
		this.textEditLayoutControlItem.TextToControlDistance = 30;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(284, 72);
		base.Controls.Add(this.layoutControl);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("TextInputForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "TextInputForm";
		this.Text = "FormTitle";
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.customTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.okBtnLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cancelBtnLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEditLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
