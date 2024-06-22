using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls;

public class LoginCaptionListBoxItemControl : BaseXtraUserControl
{
	private bool isSelected;

	private IContainer components;

	private NonCustomizableLayoutControl mainNonCustomizableLayoutControl;

	private PictureEdit iconPictureEdit;

	private LayoutControlItem iconPictureEditLayoutControlItem;

	private EmptySpaceItem bottomMargin1EmptySpaceItem;

	private EmptySpaceItem bottomMargin2EmptySpaceItem;

	private LabelControl labelLabelControl;

	private LayoutControlItem labelLabelControlLayoutControlItem;

	private LayoutControlGroup Root;

	private EmptySpaceItem topMarginEmptySpaceItem;

	private NonCustomizableLayoutControl borderNonCustomizableLayoutControl;

	private LayoutControlGroup borderLayoutControlGroup;

	private LayoutControlItem layoutControlItem1;

	private HelpIconUserControl helpIconUserControl;

	private LayoutControlItem helpIconUserControlLayoutControlItem;

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public Image Image
	{
		get
		{
			return iconPictureEdit.Image;
		}
		set
		{
			iconPictureEdit.Image = value;
		}
	}

	public SvgImage SvgImage
	{
		get
		{
			return iconPictureEdit.SvgImage;
		}
		set
		{
			iconPictureEdit.SvgImage = value;
		}
	}

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
	public string Label
	{
		get
		{
			return labelLabelControl.Text;
		}
		set
		{
			labelLabelControl.Text = value;
		}
	}

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
	public VertAlignment LabelVerticalAlignment
	{
		get
		{
			return labelLabelControl.Appearance.TextOptions.VAlignment;
		}
		set
		{
			labelLabelControl.Appearance.TextOptions.VAlignment = value;
		}
	}

	public bool IsSelected
	{
		get
		{
			return isSelected;
		}
		set
		{
			if (isSelected != value)
			{
				isSelected = value;
				if (isSelected)
				{
					SetSelected();
				}
				else
				{
					SetNormal();
				}
			}
		}
	}

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public bool IsHelpIconVisible
	{
		get
		{
			return helpIconUserControlLayoutControlItem.Visibility == LayoutVisibility.Always;
		}
		set
		{
			helpIconUserControlLayoutControlItem.Visibility = ((!value) ? LayoutVisibility.Never : LayoutVisibility.Always);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Bindable(true)]
	public string ToolTipHeader
	{
		get
		{
			return helpIconUserControl.ToolTipHeader;
		}
		set
		{
			helpIconUserControl.ToolTipHeader = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Bindable(true)]
	[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
	public string ToolTipText
	{
		get
		{
			return helpIconUserControl.ToolTipText;
		}
		set
		{
			helpIconUserControl.ToolTipText = value;
		}
	}

	public new event EventHandler Click;

	public new event EventHandler DoubleClick;

	public new event MouseEventHandler MouseClick;

	public new event MouseEventHandler MouseDoubleClick;

	public LoginCaptionListBoxItemControl()
	{
		InitializeComponent();
		labelLabelControl.ForeColor = SkinColors.ControlForeColorFromSystemColors;
		SetNormal();
		SetStyle(ControlStyles.OptimizedDoubleBuffer, value: false);
		BackColor = SkinColors.ControlColorFromSystemColors;
		mainNonCustomizableLayoutControl.BackColor = SkinColors.ControlColorFromSystemColors;
		iconPictureEdit.BackColor = SkinColors.ControlColorFromSystemColors;
	}

	public void SetNormal()
	{
		borderNonCustomizableLayoutControl.BackColor = SkinColors.ControlBorderStored;
	}

	public void SetSelected()
	{
		borderNonCustomizableLayoutControl.BackColor = SkinsManager.CurrentSkin.ListSelectionBackColor;
	}

	private void mainNonCustomizableLayoutControl_Click(object sender, EventArgs e)
	{
		this.Click?.Invoke(this, e);
	}

	private void mainNonCustomizableLayoutControl_DoubleClick(object sender, EventArgs e)
	{
		this.DoubleClick?.Invoke(this, e);
	}

	private void mainNonCustomizableLayoutControl_MouseClick(object sender, MouseEventArgs e)
	{
		this.MouseClick?.Invoke(this, e);
	}

	private void mainNonCustomizableLayoutControl_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		this.MouseDoubleClick?.Invoke(this, e);
	}

	private void topMarginEmptySpaceItem_Click(object sender, EventArgs e)
	{
		this.Click?.Invoke(this, e);
	}

	private void topMarginEmptySpaceItem_DoubleClick(object sender, EventArgs e)
	{
		this.DoubleClick?.Invoke(this, e);
	}

	private void helpIconUserControlLayoutControlItem_Click(object sender, EventArgs e)
	{
		this.Click?.Invoke(this, e);
	}

	private void helpIconUserControlLayoutControlItem_DoubleClick(object sender, EventArgs e)
	{
		this.DoubleClick?.Invoke(this, e);
	}

	private void helpIconUserControl_Click(object sender, EventArgs e)
	{
		this.Click?.Invoke(this, e);
	}

	private void helpIconUserControl_DoubleClick(object sender, EventArgs e)
	{
		this.DoubleClick?.Invoke(this, e);
	}

	private void helpIconUserControl_MouseClick(object sender, MouseEventArgs e)
	{
		this.MouseClick?.Invoke(this, e);
	}

	private void helpIconUserControl_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		this.MouseDoubleClick?.Invoke(this, e);
	}

	private void iconPictureEdit_Click(object sender, EventArgs e)
	{
		this.Click?.Invoke(this, e);
	}

	private void iconPictureEdit_DoubleClick(object sender, EventArgs e)
	{
		this.DoubleClick?.Invoke(this, e);
	}

	private void iconPictureEdit_MouseClick(object sender, MouseEventArgs e)
	{
		this.MouseClick?.Invoke(this, e);
	}

	private void iconPictureEdit_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		this.MouseDoubleClick?.Invoke(this, e);
	}

	private void labelLabelControl_Click(object sender, EventArgs e)
	{
		this.Click?.Invoke(this, e);
	}

	private void labelLabelControl_DoubleClick(object sender, EventArgs e)
	{
		this.DoubleClick?.Invoke(this, e);
	}

	private void labelLabelControl_MouseClick(object sender, MouseEventArgs e)
	{
		this.MouseClick?.Invoke(this, e);
	}

	private void labelLabelControl_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		this.MouseDoubleClick?.Invoke(this, e);
	}

	private void bottomMargin1EmptySpaceItem_Click(object sender, EventArgs e)
	{
		this.Click?.Invoke(this, e);
	}

	private void bottomMargin1EmptySpaceItem_DoubleClick(object sender, EventArgs e)
	{
		this.DoubleClick?.Invoke(this, e);
	}

	private void bottomMargin2EmptySpaceItem_Click(object sender, EventArgs e)
	{
		this.Click?.Invoke(this, e);
	}

	private void bottomMargin2EmptySpaceItem_DoubleClick(object sender, EventArgs e)
	{
		this.DoubleClick?.Invoke(this, e);
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
		this.borderNonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.mainNonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.helpIconUserControl = new Dataedo.App.UserControls.HelpIconUserControl();
		this.labelLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.iconPictureEdit = new DevExpress.XtraEditors.PictureEdit();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.iconPictureEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.bottomMargin1EmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.bottomMargin2EmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.labelLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.topMarginEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.helpIconUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.borderLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.borderNonCustomizableLayoutControl).BeginInit();
		this.borderNonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.mainNonCustomizableLayoutControl).BeginInit();
		this.mainNonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.iconPictureEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconPictureEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomMargin1EmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomMargin2EmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.labelLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.topMarginEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.helpIconUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.borderLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		base.SuspendLayout();
		this.borderNonCustomizableLayoutControl.AllowCustomization = false;
		this.borderNonCustomizableLayoutControl.BackColor = System.Drawing.Color.Red;
		this.borderNonCustomizableLayoutControl.Controls.Add(this.mainNonCustomizableLayoutControl);
		this.borderNonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.borderNonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.borderNonCustomizableLayoutControl.Margin = new System.Windows.Forms.Padding(0);
		this.borderNonCustomizableLayoutControl.Name = "borderNonCustomizableLayoutControl";
		this.borderNonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(-675, 76, 650, 580);
		this.borderNonCustomizableLayoutControl.Root = this.borderLayoutControlGroup;
		this.borderNonCustomizableLayoutControl.Size = new System.Drawing.Size(334, 214);
		this.borderNonCustomizableLayoutControl.TabIndex = 1;
		this.borderNonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.mainNonCustomizableLayoutControl.AllowCustomization = false;
		this.mainNonCustomizableLayoutControl.BackColor = System.Drawing.SystemColors.Control;
		this.mainNonCustomizableLayoutControl.Controls.Add(this.helpIconUserControl);
		this.mainNonCustomizableLayoutControl.Controls.Add(this.labelLabelControl);
		this.mainNonCustomizableLayoutControl.Controls.Add(this.iconPictureEdit);
		this.mainNonCustomizableLayoutControl.Location = new System.Drawing.Point(2, 2);
		this.mainNonCustomizableLayoutControl.Margin = new System.Windows.Forms.Padding(0);
		this.mainNonCustomizableLayoutControl.Name = "mainNonCustomizableLayoutControl";
		this.mainNonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(210, 90, 838, 761);
		this.mainNonCustomizableLayoutControl.Root = this.Root;
		this.mainNonCustomizableLayoutControl.Size = new System.Drawing.Size(330, 210);
		this.mainNonCustomizableLayoutControl.TabIndex = 0;
		this.mainNonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.mainNonCustomizableLayoutControl.Click += new System.EventHandler(mainNonCustomizableLayoutControl_Click);
		this.mainNonCustomizableLayoutControl.DoubleClick += new System.EventHandler(mainNonCustomizableLayoutControl_DoubleClick);
		this.mainNonCustomizableLayoutControl.MouseClick += new System.Windows.Forms.MouseEventHandler(mainNonCustomizableLayoutControl_MouseClick);
		this.mainNonCustomizableLayoutControl.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(mainNonCustomizableLayoutControl_MouseDoubleClick);
		this.helpIconUserControl.AutoPopDelay = 3600000;
		this.helpIconUserControl.BackColor = System.Drawing.Color.Transparent;
		this.helpIconUserControl.KeepWhileHovered = true;
		this.helpIconUserControl.Location = new System.Drawing.Point(303, 10);
		this.helpIconUserControl.MaximumSize = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.MaxToolTipWidth = 1000;
		this.helpIconUserControl.MinimumSize = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.Name = "helpIconUserControl";
		this.helpIconUserControl.Size = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.TabIndex = 21;
		this.helpIconUserControl.ToolTipHeader = "";
		this.helpIconUserControl.ToolTipText = "";
		this.helpIconUserControl.Click += new System.EventHandler(helpIconUserControl_Click);
		this.helpIconUserControl.DoubleClick += new System.EventHandler(helpIconUserControl_DoubleClick);
		this.helpIconUserControl.MouseClick += new System.Windows.Forms.MouseEventHandler(helpIconUserControl_MouseClick);
		this.helpIconUserControl.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(helpIconUserControl_MouseDoubleClick);
		this.labelLabelControl.AllowHtmlString = true;
		this.labelLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.labelLabelControl.Appearance.Options.UseFont = true;
		this.labelLabelControl.Appearance.Options.UseTextOptions = true;
		this.labelLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.labelLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
		this.labelLabelControl.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.labelLabelControl.Location = new System.Drawing.Point(7, 136);
		this.labelLabelControl.Name = "labelLabelControl";
		this.labelLabelControl.Size = new System.Drawing.Size(316, 37);
		this.labelLabelControl.StyleController = this.mainNonCustomizableLayoutControl;
		this.labelLabelControl.TabIndex = 20;
		this.labelLabelControl.Text = "Label - line 1";
		this.labelLabelControl.Click += new System.EventHandler(labelLabelControl_Click);
		this.labelLabelControl.DoubleClick += new System.EventHandler(labelLabelControl_DoubleClick);
		this.labelLabelControl.MouseClick += new System.Windows.Forms.MouseEventHandler(labelLabelControl_MouseClick);
		this.labelLabelControl.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(labelLabelControl_MouseDoubleClick);
		this.iconPictureEdit.EditValue = Dataedo.App.Properties.Resources.server_connect_64;
		this.iconPictureEdit.Location = new System.Drawing.Point(7, 37);
		this.iconPictureEdit.Name = "iconPictureEdit";
		this.iconPictureEdit.Properties.AllowFocused = false;
		this.iconPictureEdit.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.iconPictureEdit.Properties.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconPictureEdit.Properties.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconPictureEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.iconPictureEdit.Properties.Appearance.Options.UseBackColor = true;
		this.iconPictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.iconPictureEdit.Properties.ReadOnly = true;
		this.iconPictureEdit.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
		this.iconPictureEdit.Properties.ShowMenu = false;
		this.iconPictureEdit.Size = new System.Drawing.Size(316, 95);
		this.iconPictureEdit.StyleController = this.mainNonCustomizableLayoutControl;
		this.iconPictureEdit.TabIndex = 19;
		this.iconPictureEdit.Click += new System.EventHandler(iconPictureEdit_Click);
		this.iconPictureEdit.DoubleClick += new System.EventHandler(iconPictureEdit_DoubleClick);
		this.iconPictureEdit.MouseClick += new System.Windows.Forms.MouseEventHandler(iconPictureEdit_MouseClick);
		this.iconPictureEdit.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(iconPictureEdit_MouseDoubleClick);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[6] { this.iconPictureEditLayoutControlItem, this.bottomMargin1EmptySpaceItem, this.bottomMargin2EmptySpaceItem, this.labelLabelControlLayoutControlItem, this.topMarginEmptySpaceItem, this.helpIconUserControlLayoutControlItem });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
		this.Root.Size = new System.Drawing.Size(330, 210);
		this.Root.TextVisible = false;
		this.iconPictureEditLayoutControlItem.Control = this.iconPictureEdit;
		this.iconPictureEditLayoutControlItem.Location = new System.Drawing.Point(0, 30);
		this.iconPictureEditLayoutControlItem.MinSize = new System.Drawing.Size(24, 70);
		this.iconPictureEditLayoutControlItem.Name = "iconPictureEditLayoutControlItem";
		this.iconPictureEditLayoutControlItem.Size = new System.Drawing.Size(320, 99);
		this.iconPictureEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.iconPictureEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.iconPictureEditLayoutControlItem.TextVisible = false;
		this.bottomMargin1EmptySpaceItem.AllowHotTrack = false;
		this.bottomMargin1EmptySpaceItem.Location = new System.Drawing.Point(0, 170);
		this.bottomMargin1EmptySpaceItem.MaxSize = new System.Drawing.Size(0, 20);
		this.bottomMargin1EmptySpaceItem.MinSize = new System.Drawing.Size(104, 20);
		this.bottomMargin1EmptySpaceItem.Name = "bottomMargin1EmptySpaceItem";
		this.bottomMargin1EmptySpaceItem.Size = new System.Drawing.Size(320, 20);
		this.bottomMargin1EmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.bottomMargin1EmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.bottomMargin1EmptySpaceItem.Click += new System.EventHandler(bottomMargin1EmptySpaceItem_Click);
		this.bottomMargin1EmptySpaceItem.DoubleClick += new System.EventHandler(bottomMargin1EmptySpaceItem_DoubleClick);
		this.bottomMargin2EmptySpaceItem.AllowHotTrack = false;
		this.bottomMargin2EmptySpaceItem.Location = new System.Drawing.Point(0, 190);
		this.bottomMargin2EmptySpaceItem.MaxSize = new System.Drawing.Size(0, 10);
		this.bottomMargin2EmptySpaceItem.MinSize = new System.Drawing.Size(104, 10);
		this.bottomMargin2EmptySpaceItem.Name = "bottomMargin2EmptySpaceItem";
		this.bottomMargin2EmptySpaceItem.Size = new System.Drawing.Size(320, 10);
		this.bottomMargin2EmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.bottomMargin2EmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.bottomMargin2EmptySpaceItem.Click += new System.EventHandler(bottomMargin2EmptySpaceItem_Click);
		this.bottomMargin2EmptySpaceItem.DoubleClick += new System.EventHandler(bottomMargin2EmptySpaceItem_DoubleClick);
		this.labelLabelControlLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.labelLabelControlLayoutControlItem.Control = this.labelLabelControl;
		this.labelLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 129);
		this.labelLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(110, 1);
		this.labelLabelControlLayoutControlItem.Name = "labelLabelControlLayoutControlItem";
		this.labelLabelControlLayoutControlItem.Size = new System.Drawing.Size(320, 41);
		this.labelLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.labelLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.labelLabelControlLayoutControlItem.TextVisible = false;
		this.topMarginEmptySpaceItem.AllowHotTrack = false;
		this.topMarginEmptySpaceItem.Location = new System.Drawing.Point(0, 0);
		this.topMarginEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 30);
		this.topMarginEmptySpaceItem.MinSize = new System.Drawing.Size(104, 30);
		this.topMarginEmptySpaceItem.Name = "topMarginEmptySpaceItem";
		this.topMarginEmptySpaceItem.Size = new System.Drawing.Size(296, 30);
		this.topMarginEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.topMarginEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.topMarginEmptySpaceItem.Click += new System.EventHandler(topMarginEmptySpaceItem_Click);
		this.topMarginEmptySpaceItem.DoubleClick += new System.EventHandler(topMarginEmptySpaceItem_DoubleClick);
		this.helpIconUserControlLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.helpIconUserControlLayoutControlItem.Control = this.helpIconUserControl;
		this.helpIconUserControlLayoutControlItem.Location = new System.Drawing.Point(296, 0);
		this.helpIconUserControlLayoutControlItem.Name = "helpIconUserControlLayoutControlItem";
		this.helpIconUserControlLayoutControlItem.Size = new System.Drawing.Size(24, 30);
		this.helpIconUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.helpIconUserControlLayoutControlItem.TextVisible = false;
		this.helpIconUserControlLayoutControlItem.Click += new System.EventHandler(helpIconUserControlLayoutControlItem_Click);
		this.helpIconUserControlLayoutControlItem.DoubleClick += new System.EventHandler(helpIconUserControlLayoutControlItem_DoubleClick);
		this.borderLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.borderLayoutControlGroup.GroupBordersVisible = false;
		this.borderLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem1 });
		this.borderLayoutControlGroup.Name = "Root";
		this.borderLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(1, 1, 1, 1);
		this.borderLayoutControlGroup.Size = new System.Drawing.Size(334, 214);
		this.borderLayoutControlGroup.TextVisible = false;
		this.layoutControlItem1.Control = this.mainNonCustomizableLayoutControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(142, 1);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(1, 1, 1, 1);
		this.layoutControlItem1.Size = new System.Drawing.Size(332, 212);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		base.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.Controls.Add(this.borderNonCustomizableLayoutControl);
		base.Name = "LoginCaptionListBoxItemControl";
		base.Size = new System.Drawing.Size(334, 214);
		((System.ComponentModel.ISupportInitialize)this.borderNonCustomizableLayoutControl).EndInit();
		this.borderNonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.mainNonCustomizableLayoutControl).EndInit();
		this.mainNonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.iconPictureEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconPictureEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomMargin1EmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomMargin2EmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.labelLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.topMarginEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.helpIconUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.borderLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		base.ResumeLayout(false);
	}
}
