using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Tools.UI;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Onboarding.Home.Controls;

public class AddDatabaseUserControl : UserControl
{
	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private LabelControl headerLabelControl;

	private LayoutControlItem headerLabelControlLayoutControlItem;

	private LabelControl textLabelControl;

	private LayoutControlItem textLabelControlLayoutControlItem;

	private LabelControl addDatabaseButtonLabelControl;

	private LayoutControlItem addDatabaseButtonLabelControlLayoutControlItem;

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public string Title
	{
		get
		{
			return headerLabelControl.Text;
		}
		set
		{
			headerLabelControl.Text = value;
			headerLabelControlLayoutControlItem.Visibility = ((value == null) ? LayoutVisibility.Never : LayoutVisibility.Always);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public new string Text
	{
		get
		{
			return textLabelControl.Text;
		}
		set
		{
			textLabelControl.Text = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public event EventHandler AddDatabaseButtonClick
	{
		add
		{
			addDatabaseButtonLabelControl.Click += value;
		}
		remove
		{
			addDatabaseButtonLabelControl.Click -= value;
		}
	}

	public AddDatabaseUserControl()
	{
		InitializeComponent();
		headerLabelControl.Appearance.Options.UseFont = true;
		addDatabaseButtonLabelControl.Appearance.Options.UseFont = true;
		addDatabaseButtonLabelControl.Appearance.Options.UseBackColor = true;
		addDatabaseButtonLabelControl.Appearance.Options.UseForeColor = true;
		addDatabaseButtonLabelControl.Appearance.BackColor = SkinsManager.CurrentSkin.OnboardingAddDatabaseButtonBackColor;
		addDatabaseButtonLabelControl.Appearance.ForeColor = SkinsManager.CurrentSkin.OnboardingAddDatabaseButtonForeColor;
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
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.addDatabaseButtonLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.headerLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.textLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.headerLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.textLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.addDatabaseButtonLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.addDatabaseButtonLabelControlLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.addDatabaseButtonLabelControl);
		this.mainLayoutControl.Controls.Add(this.headerLabelControl);
		this.mainLayoutControl.Controls.Add(this.textLabelControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Margin = new System.Windows.Forms.Padding(0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2890, 480, 805, 588);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(361, 145);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.addDatabaseButtonLabelControl.Appearance.BackColor = System.Drawing.Color.FromArgb(58, 91, 167);
		this.addDatabaseButtonLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 13f, System.Drawing.FontStyle.Bold);
		this.addDatabaseButtonLabelControl.Appearance.ForeColor = System.Drawing.SystemColors.Control;
		this.addDatabaseButtonLabelControl.Appearance.Options.UseBackColor = true;
		this.addDatabaseButtonLabelControl.Appearance.Options.UseFont = true;
		this.addDatabaseButtonLabelControl.Appearance.Options.UseForeColor = true;
		this.addDatabaseButtonLabelControl.Appearance.Options.UseTextOptions = true;
		this.addDatabaseButtonLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.addDatabaseButtonLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.addDatabaseButtonLabelControl.Location = new System.Drawing.Point(2, 66);
		this.addDatabaseButtonLabelControl.MaximumSize = new System.Drawing.Size(170, 35);
		this.addDatabaseButtonLabelControl.MinimumSize = new System.Drawing.Size(170, 35);
		this.addDatabaseButtonLabelControl.Name = "addDatabaseButtonLabelControl";
		this.addDatabaseButtonLabelControl.Size = new System.Drawing.Size(170, 35);
		this.addDatabaseButtonLabelControl.StyleController = this.mainLayoutControl;
		this.addDatabaseButtonLabelControl.TabIndex = 7;
		this.addDatabaseButtonLabelControl.Text = "Add Database";
		this.headerLabelControl.AllowHtmlString = true;
		this.headerLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 14f, System.Drawing.FontStyle.Bold);
		this.headerLabelControl.Appearance.Options.UseFont = true;
		this.headerLabelControl.Location = new System.Drawing.Point(2, 2);
		this.headerLabelControl.Name = "headerLabelControl";
		this.headerLabelControl.Size = new System.Drawing.Size(357, 24);
		this.headerLabelControl.StyleController = this.mainLayoutControl;
		this.headerLabelControl.TabIndex = 4;
		this.headerLabelControl.Text = "Your Catalog";
		this.textLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.textLabelControl.Appearance.Options.UseFont = true;
		this.textLabelControl.Location = new System.Drawing.Point(2, 38);
		this.textLabelControl.Name = "textLabelControl";
		this.textLabelControl.Size = new System.Drawing.Size(216, 16);
		this.textLabelControl.StyleController = this.mainLayoutControl;
		this.textLabelControl.TabIndex = 4;
		this.textLabelControl.Text = "You did not import any databases yet.";
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.headerLabelControlLayoutControlItem, this.textLabelControlLayoutControlItem, this.addDatabaseButtonLabelControlLayoutControlItem });
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(361, 145);
		this.mainLayoutControlGroup.TextVisible = false;
		this.headerLabelControlLayoutControlItem.Control = this.headerLabelControl;
		this.headerLabelControlLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.headerLabelControlLayoutControlItem.CustomizationFormText = "headerLabelControlLayoutControlItem";
		this.headerLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.headerLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 36);
		this.headerLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(1, 36);
		this.headerLabelControlLayoutControlItem.Name = "headerLabelControlLayoutControlItem";
		this.headerLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 10);
		this.headerLabelControlLayoutControlItem.Size = new System.Drawing.Size(361, 36);
		this.headerLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.headerLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.headerLabelControlLayoutControlItem.TextVisible = false;
		this.textLabelControlLayoutControlItem.Control = this.textLabelControl;
		this.textLabelControlLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.textLabelControlLayoutControlItem.CustomizationFormText = "headerLabelControlLayoutControlItem";
		this.textLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 36);
		this.textLabelControlLayoutControlItem.Name = "textLabelControlLayoutControlItem";
		this.textLabelControlLayoutControlItem.Size = new System.Drawing.Size(361, 20);
		this.textLabelControlLayoutControlItem.Text = "headerLabelControlLayoutControlItem";
		this.textLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.textLabelControlLayoutControlItem.TextVisible = false;
		this.addDatabaseButtonLabelControlLayoutControlItem.Control = this.addDatabaseButtonLabelControl;
		this.addDatabaseButtonLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 56);
		this.addDatabaseButtonLabelControlLayoutControlItem.Name = "addDatabaseButtonLabelControlLayoutControlItem";
		this.addDatabaseButtonLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 10, 2);
		this.addDatabaseButtonLabelControlLayoutControlItem.Size = new System.Drawing.Size(361, 89);
		this.addDatabaseButtonLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.addDatabaseButtonLabelControlLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Name = "AddDatabaseUserControl";
		base.Size = new System.Drawing.Size(361, 145);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.addDatabaseButtonLabelControlLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
