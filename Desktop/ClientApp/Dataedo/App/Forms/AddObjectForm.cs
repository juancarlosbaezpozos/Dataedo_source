using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Forms;

public class AddObjectForm : BaseXtraForm
{
	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private SimpleButton option3SimpleButton;

	private SimpleButton option2SimpleButton;

	private SimpleButton option1SimpleButton;

	private LayoutControlItem option1SimpleButtonLayoutControlItem;

	private LayoutControlItem option2SimpleButtonLayoutControlItem;

	private LayoutControlItem option3SimpleButtonLayoutControlItem;

	public AddObjectForm()
	{
		InitializeComponent();
	}

	public void Initialize(string formTitle, string option1Text, Action option1Action, Image option1Image, string option2Text, Action option2Action, Image option2Image, string option3Text, Action option3Action, Image option3Image)
	{
		Text = formTitle;
		option1SimpleButton.Text = option1Text;
		option1SimpleButton.Click += delegate
		{
			option1Action?.Invoke();
		};
		option1SimpleButton.ImageOptions.Image = option1Image;
		option2SimpleButton.Text = option2Text;
		option2SimpleButton.Click += delegate
		{
			option2Action?.Invoke();
		};
		option2SimpleButton.ImageOptions.Image = option2Image;
		option3SimpleButton.Text = option3Text;
		option3SimpleButton.Click += delegate
		{
			option3Action?.Invoke();
		};
		option3SimpleButton.ImageOptions.Image = option3Image;
	}

	public void Initialize(string formTitle, string option1Text, Action option1Action, Image option1Image, string option2Text, Action option2Action, Image option2Image)
	{
		Text = formTitle;
		option1SimpleButton.Text = option1Text;
		option1SimpleButton.Click += delegate
		{
			option1Action?.Invoke();
		};
		option1SimpleButton.ImageOptions.Image = option1Image;
		option2SimpleButton.Hide();
		option3SimpleButton.Text = option2Text;
		option3SimpleButton.Click += delegate
		{
			option2Action?.Invoke();
		};
		option3SimpleButton.ImageOptions.Image = option2Image;
		base.Width = option3SimpleButton.Location.X;
		if (mainLayoutControlGroup.OptionsTableLayoutGroup.ColumnDefinitions.Count == 5)
		{
			mainLayoutControlGroup.OptionsTableLayoutGroup.ColumnDefinitions.RemoveAt(0);
			mainLayoutControlGroup.OptionsTableLayoutGroup.ColumnDefinitions.RemoveAt(0);
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
		DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
		DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
		DevExpress.XtraLayout.RowDefinition rowDefinition = new DevExpress.XtraLayout.RowDefinition();
		new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.AddObjectForm));
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.option3SimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.option2SimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.option1SimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.option1SimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.option2SimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.option3SimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.option1SimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.option2SimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.option3SimpleButtonLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.option3SimpleButton);
		this.mainLayoutControl.Controls.Add(this.option2SimpleButton);
		this.mainLayoutControl.Controls.Add(this.option1SimpleButton);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2787, 626, 832, 651);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(594, 154);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.option3SimpleButton.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
		this.option3SimpleButton.ImageOptions.ImageToTextIndent = 20;
		this.option3SimpleButton.Location = new System.Drawing.Point(399, 17);
		this.option3SimpleButton.Name = "option3SimpleButton";
		this.option3SimpleButton.Size = new System.Drawing.Size(178, 120);
		this.option3SimpleButton.StyleController = this.mainLayoutControl;
		this.option3SimpleButton.TabIndex = 7;
		this.option3SimpleButton.Text = "Option 3";
		this.option2SimpleButton.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
		this.option2SimpleButton.ImageOptions.ImageToTextIndent = 20;
		this.option2SimpleButton.Location = new System.Drawing.Point(208, 17);
		this.option2SimpleButton.Name = "option2SimpleButton";
		this.option2SimpleButton.Size = new System.Drawing.Size(177, 120);
		this.option2SimpleButton.StyleController = this.mainLayoutControl;
		this.option2SimpleButton.TabIndex = 6;
		this.option2SimpleButton.Text = "Option 2";
		this.option1SimpleButton.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
		this.option1SimpleButton.ImageOptions.ImageToTextIndent = 20;
		this.option1SimpleButton.Location = new System.Drawing.Point(17, 17);
		this.option1SimpleButton.Name = "option1SimpleButton";
		this.option1SimpleButton.Size = new System.Drawing.Size(177, 120);
		this.option1SimpleButton.StyleController = this.mainLayoutControl;
		this.option1SimpleButton.TabIndex = 5;
		this.option1SimpleButton.Text = "Option 1";
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.option1SimpleButtonLayoutControlItem, this.option2SimpleButtonLayoutControlItem, this.option3SimpleButtonLayoutControlItem });
		this.mainLayoutControlGroup.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
		this.mainLayoutControlGroup.Name = "Root";
		columnDefinition.SizeType = System.Windows.Forms.SizeType.Percent;
		columnDefinition.Width = 33.333333333333336;
		columnDefinition2.SizeType = System.Windows.Forms.SizeType.Absolute;
		columnDefinition2.Width = 10.0;
		columnDefinition3.SizeType = System.Windows.Forms.SizeType.Percent;
		columnDefinition3.Width = 33.333333333333336;
		columnDefinition4.SizeType = System.Windows.Forms.SizeType.Absolute;
		columnDefinition4.Width = 10.0;
		columnDefinition5.SizeType = System.Windows.Forms.SizeType.Percent;
		columnDefinition5.Width = 33.333333333333336;
		this.mainLayoutControlGroup.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[5] { columnDefinition, columnDefinition2, columnDefinition3, columnDefinition4, columnDefinition5 });
		rowDefinition.Height = 100.0;
		rowDefinition.SizeType = System.Windows.Forms.SizeType.Percent;
		this.mainLayoutControlGroup.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[1] { rowDefinition });
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(15, 15, 15, 15);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(594, 154);
		this.mainLayoutControlGroup.TextVisible = false;
		this.option1SimpleButtonLayoutControlItem.Control = this.option1SimpleButton;
		this.option1SimpleButtonLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.option1SimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(52, 26);
		this.option1SimpleButtonLayoutControlItem.Name = "option1SimpleButtonLayoutControlItem";
		this.option1SimpleButtonLayoutControlItem.Size = new System.Drawing.Size(181, 124);
		this.option1SimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.option1SimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.option1SimpleButtonLayoutControlItem.TextVisible = false;
		this.option2SimpleButtonLayoutControlItem.Control = this.option2SimpleButton;
		this.option2SimpleButtonLayoutControlItem.Location = new System.Drawing.Point(191, 0);
		this.option2SimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(52, 26);
		this.option2SimpleButtonLayoutControlItem.Name = "option2SimpleButtonLayoutControlItem";
		this.option2SimpleButtonLayoutControlItem.OptionsTableLayoutItem.ColumnIndex = 2;
		this.option2SimpleButtonLayoutControlItem.Size = new System.Drawing.Size(181, 124);
		this.option2SimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.option2SimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.option2SimpleButtonLayoutControlItem.TextVisible = false;
		this.option3SimpleButtonLayoutControlItem.Control = this.option3SimpleButton;
		this.option3SimpleButtonLayoutControlItem.Location = new System.Drawing.Point(382, 0);
		this.option3SimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(52, 26);
		this.option3SimpleButtonLayoutControlItem.Name = "option3SimpleButtonLayoutControlItem";
		this.option3SimpleButtonLayoutControlItem.OptionsTableLayoutItem.ColumnIndex = 4;
		this.option3SimpleButtonLayoutControlItem.Size = new System.Drawing.Size(182, 124);
		this.option3SimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.option3SimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.option3SimpleButtonLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(594, 154);
		base.Controls.Add(this.mainLayoutControl);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_16;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AddObjectForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Add Object";
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.option1SimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.option2SimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.option3SimpleButtonLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
