using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.ImportDescriptions;

public class ChooseImportTypeForm : BaseXtraForm
{
	private MetadataEditorUserControl metadataEditorUserControl;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private SimpleButton option2SimpleButton;

	private SimpleButton option1SimpleButton;

	private LayoutControlItem option1SimpleButtonLayoutControlItem;

	private LayoutControlItem option2SimpleButtonLayoutControlItem;

	public ChooseImportTypeForm()
	{
		InitializeComponent();
		Text = "Import descriptions";
	}

	public void Initialize(MetadataEditorUserControl _metadataEditorUserControl)
	{
		metadataEditorUserControl = _metadataEditorUserControl;
		SetOptionButton(option1SimpleButton, "Columns", delegate
		{
			ProcessChooseImportFields(() => DefaultOptionAction(SharedObjectTypeEnum.ObjectType.Column));
		}, Resources.column_32);
		SetOptionButton(option2SimpleButton, "Tables/Views/Structures", delegate
		{
			ProcessChooseImportFields(() => DefaultOptionAction(SharedObjectTypeEnum.ObjectType.Table));
		}, Resources.table_32);
	}

	private void SetOptionButton(SimpleButton optionButton, string optionText, Action optionAction, Image optionImage)
	{
		optionButton.Text = optionText;
		optionButton.Click += delegate
		{
			optionAction?.Invoke();
		};
		optionButton.ImageOptions.Image = optionImage;
	}

	private void ProcessChooseImportFields(Func<bool> func)
	{
		try
		{
			if (func())
			{
				Close();
			}
			else
			{
				base.Visible = true;
			}
		}
		catch (Exception)
		{
			Close();
			throw;
		}
	}

	private bool DefaultOptionAction(SharedObjectTypeEnum.ObjectType type)
	{
		DialogResult dialogResult = new ChooseImportFieldsForm(this, metadataEditorUserControl, metadataEditorUserControl.GetFocusedNode(fromCustomFocus: true).DatabaseId, type).ShowDialog(this);
		if (dialogResult == DialogResult.OK || dialogResult == DialogResult.Cancel)
		{
			base.DialogResult = dialogResult;
			return true;
		}
		return false;
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Escape)
		{
			Close();
		}
		return base.ProcessCmdKey(ref msg, keyData);
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
		DevExpress.XtraLayout.ColumnDefinition columnDefinition = new DevExpress.XtraLayout.ColumnDefinition();
		DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
		DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
		DevExpress.XtraLayout.RowDefinition rowDefinition = new DevExpress.XtraLayout.RowDefinition();
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.option2SimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.option1SimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.option1SimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.option2SimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.option1SimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.option2SimpleButtonLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.option2SimpleButton);
		this.mainLayoutControl.Controls.Add(this.option1SimpleButton);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1303, 423, 1129, 741);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(398, 154);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.option2SimpleButton.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
		this.option2SimpleButton.ImageOptions.ImageToTextIndent = 20;
		this.option2SimpleButton.Location = new System.Drawing.Point(206, 17);
		this.option2SimpleButton.Name = "option2SimpleButton";
		this.option2SimpleButton.Size = new System.Drawing.Size(175, 120);
		this.option2SimpleButton.StyleController = this.mainLayoutControl;
		this.option2SimpleButton.TabIndex = 6;
		this.option2SimpleButton.Text = "Option 2";
		this.option1SimpleButton.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
		this.option1SimpleButton.ImageOptions.ImageToTextIndent = 20;
		this.option1SimpleButton.Location = new System.Drawing.Point(17, 17);
		this.option1SimpleButton.Name = "option1SimpleButton";
		this.option1SimpleButton.Size = new System.Drawing.Size(175, 120);
		this.option1SimpleButton.StyleController = this.mainLayoutControl;
		this.option1SimpleButton.TabIndex = 5;
		this.option1SimpleButton.Text = "Option 1";
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.option1SimpleButtonLayoutControlItem, this.option2SimpleButtonLayoutControlItem });
		this.mainLayoutControlGroup.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
		this.mainLayoutControlGroup.Name = "Root";
		columnDefinition.SizeType = System.Windows.Forms.SizeType.Percent;
		columnDefinition.Width = 50.0;
		columnDefinition2.SizeType = System.Windows.Forms.SizeType.Absolute;
		columnDefinition2.Width = 10.0;
		columnDefinition3.SizeType = System.Windows.Forms.SizeType.Percent;
		columnDefinition3.Width = 50.0;
		this.mainLayoutControlGroup.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[3] { columnDefinition, columnDefinition2, columnDefinition3 });
		rowDefinition.Height = 100.0;
		rowDefinition.SizeType = System.Windows.Forms.SizeType.Percent;
		this.mainLayoutControlGroup.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[1] { rowDefinition });
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(15, 15, 15, 15);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(398, 154);
		this.mainLayoutControlGroup.TextVisible = false;
		this.option1SimpleButtonLayoutControlItem.Control = this.option1SimpleButton;
		this.option1SimpleButtonLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.option1SimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(52, 26);
		this.option1SimpleButtonLayoutControlItem.Name = "option1SimpleButtonLayoutControlItem";
		this.option1SimpleButtonLayoutControlItem.Size = new System.Drawing.Size(179, 124);
		this.option1SimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.option1SimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.option1SimpleButtonLayoutControlItem.TextVisible = false;
		this.option2SimpleButtonLayoutControlItem.Control = this.option2SimpleButton;
		this.option2SimpleButtonLayoutControlItem.Location = new System.Drawing.Point(189, 0);
		this.option2SimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(52, 26);
		this.option2SimpleButtonLayoutControlItem.Name = "option2SimpleButtonLayoutControlItem";
		this.option2SimpleButtonLayoutControlItem.OptionsTableLayoutItem.ColumnIndex = 2;
		this.option2SimpleButtonLayoutControlItem.Size = new System.Drawing.Size(179, 124);
		this.option2SimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.option2SimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.option2SimpleButtonLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(398, 154);
		base.Controls.Add(this.mainLayoutControl);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ChooseImportTypeForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Add Object";
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.option1SimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.option2SimpleButtonLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
