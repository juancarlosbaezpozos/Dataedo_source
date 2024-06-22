using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls;

public class CaptionListBoxControl : BaseUserControl
{
	private IContainer components;

	private LabelControl captionLabelControl;

	private NonCustomizableLayoutControl layoutControl;

	private LayoutControlGroup layoutControlGroup1;

	private ImageListBoxControl imageListBoxControl;

	private LayoutControlItem captionLabelLayoutControlItem;

	private LayoutControlItem imageListBoxLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	private ToolTipControllerUserControl toolTipController;

	private LabelControl subCaptionLabelControl;

	private LayoutControlItem layoutControlItem1;

	[Browsable(true)]
	[DefaultValue(5)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public int CaptionSpace
	{
		get
		{
			return emptySpaceItem1.Height;
		}
		set
		{
			emptySpaceItem1.Height = value;
		}
	}

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public string Caption
	{
		get
		{
			return captionLabelControl.Text;
		}
		set
		{
			captionLabelControl.Text = value;
		}
	}

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public string SubCaption
	{
		get
		{
			return subCaptionLabelControl.Text;
		}
		set
		{
			if (!string.IsNullOrWhiteSpace(value))
			{
				layoutControlItem1.Visibility = LayoutVisibility.Always;
			}
			subCaptionLabelControl.Text = value;
		}
	}

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public Image CaptionImage
	{
		get
		{
			return captionLabelLayoutControlItem.Image;
		}
		set
		{
			captionLabelLayoutControlItem.Image = value;
		}
	}

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public string ToolTip
	{
		get
		{
			return captionLabelLayoutControlItem.OptionsToolTip.ToolTip;
		}
		set
		{
			captionLabelLayoutControlItem.OptionsToolTip.ToolTip = value;
		}
	}

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public new ContextMenuStrip ContextMenuStrip
	{
		get
		{
			return imageListBoxControl.ContextMenuStrip;
		}
		set
		{
			imageListBoxControl.ContextMenuStrip = value;
		}
	}

	public object DataSource
	{
		set
		{
			imageListBoxControl.DataSource = value;
		}
	}

	public string ImageIndexMember
	{
		set
		{
			imageListBoxControl.ImageIndexMember = value;
		}
	}

	public string DisplayMember
	{
		set
		{
			imageListBoxControl.DisplayMember = value;
		}
	}

	public string ValueMember
	{
		set
		{
			imageListBoxControl.ValueMember = value;
		}
	}

	public object ImageList
	{
		set
		{
			imageListBoxControl.ImageList = value;
		}
	}

	public int SelectedIndex
	{
		get
		{
			return imageListBoxControl.SelectedIndex;
		}
		set
		{
			imageListBoxControl.SelectedIndex = value;
		}
	}

	public object SelectedItem => imageListBoxControl.SelectedItem;

	[Browsable(true)]
	public new event MouseEventHandler MouseClick
	{
		add
		{
			imageListBoxControl.MouseClick += value;
		}
		remove
		{
			imageListBoxControl.MouseClick -= value;
		}
	}

	[Browsable(true)]
	public new event MouseEventHandler MouseDoubleClick
	{
		add
		{
			imageListBoxControl.MouseDoubleClick += value;
		}
		remove
		{
			imageListBoxControl.MouseDoubleClick -= value;
		}
	}

	[Browsable(true)]
	public new event KeyEventHandler KeyDown
	{
		add
		{
			imageListBoxControl.KeyDown += value;
		}
		remove
		{
			imageListBoxControl.KeyDown -= value;
		}
	}

	[Browsable(true)]
	public new event MouseEventHandler MouseDown
	{
		add
		{
			imageListBoxControl.MouseDown += value;
		}
		remove
		{
			imageListBoxControl.MouseDown -= value;
		}
	}

	public CaptionListBoxControl()
	{
		InitializeComponent();
		imageListBoxControl.BackColor = SkinColors.ControlColorFromSystemColors;
		imageListBoxControl.Appearance.Options.UseBackColor = true;
	}

	private void listBoxControl_DrawItem(object sender, ListBoxDrawItemEventArgs e)
	{
		if (e.State != 0)
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.ListSelectionBackColor;
		}
	}

	public Rectangle GetItemRectangle(int index)
	{
		return imageListBoxControl.GetItemRectangle(index);
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
		this.components = new System.ComponentModel.Container();
		this.captionLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.subCaptionLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.imageListBoxControl = new DevExpress.XtraEditors.ImageListBoxControl();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.captionLabelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.imageListBoxLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.toolTipController = new Dataedo.App.UserControls.ToolTipControllerUserControl(this.components);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.imageListBoxControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.captionLabelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.imageListBoxLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		base.SuspendLayout();
		this.captionLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.captionLabelControl.Appearance.ForeColor = System.Drawing.Color.Gray;
		this.captionLabelControl.Appearance.Options.UseFont = true;
		this.captionLabelControl.Appearance.Options.UseForeColor = true;
		this.captionLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
		this.captionLabelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.captionLabelControl.Location = new System.Drawing.Point(0, 0);
		this.captionLabelControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.captionLabelControl.Name = "captionLabelControl";
		this.captionLabelControl.Size = new System.Drawing.Size(616, 13);
		this.captionLabelControl.TabIndex = 0;
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.Controls.Add(this.subCaptionLabelControl);
		this.layoutControl.Controls.Add(this.imageListBoxControl);
		this.layoutControl.Controls.Add(this.captionLabelControl);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1250, 195, 250, 350);
		this.layoutControl.OptionsView.ShareLookAndFeelWithChildren = false;
		this.layoutControl.Root = this.layoutControlGroup1;
		this.layoutControl.Size = new System.Drawing.Size(619, 380);
		this.layoutControl.TabIndex = 1;
		this.layoutControl.Text = "layoutControl1";
		this.layoutControl.ToolTipController = this.toolTipController;
		this.subCaptionLabelControl.Appearance.ForeColor = System.Drawing.Color.Gray;
		this.subCaptionLabelControl.Appearance.Options.UseForeColor = true;
		this.subCaptionLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
		this.subCaptionLabelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.subCaptionLabelControl.Location = new System.Drawing.Point(0, 13);
		this.subCaptionLabelControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.subCaptionLabelControl.Name = "subCaptionLabelControl";
		this.subCaptionLabelControl.Size = new System.Drawing.Size(619, 13);
		this.subCaptionLabelControl.TabIndex = 5;
		this.imageListBoxControl.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.imageListBoxControl.Appearance.Options.UseTextOptions = true;
		this.imageListBoxControl.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.imageListBoxControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.imageListBoxControl.HotTrackSelectMode = DevExpress.XtraEditors.HotTrackSelectMode.SelectItemOnClick;
		this.imageListBoxControl.ItemAutoHeight = true;
		this.imageListBoxControl.ItemHeight = 41;
		this.imageListBoxControl.Location = new System.Drawing.Point(0, 44);
		this.imageListBoxControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.imageListBoxControl.Name = "imageListBoxControl";
		this.imageListBoxControl.ShowFocusRect = false;
		this.imageListBoxControl.Size = new System.Drawing.Size(619, 336);
		this.imageListBoxControl.TabIndex = 4;
		this.imageListBoxControl.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(listBoxControl_DrawItem);
		this.layoutControlGroup1.ContentImageOptions.Image = Dataedo.App.Properties.Resources.question_16;
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.captionLabelLayoutControlItem, this.imageListBoxLayoutControlItem, this.emptySpaceItem1, this.layoutControlItem1 });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(619, 380);
		this.layoutControlGroup1.TextVisible = false;
		this.captionLabelLayoutControlItem.Control = this.captionLabelControl;
		this.captionLabelLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
		this.captionLabelLayoutControlItem.ImageOptions.Alignment = System.Drawing.ContentAlignment.MiddleRight;
		this.captionLabelLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.captionLabelLayoutControlItem.Name = "captionLabelLayoutControlItem";
		this.captionLabelLayoutControlItem.OptionsToolTip.AllowHtmlString = DevExpress.Utils.DefaultBoolean.True;
		this.captionLabelLayoutControlItem.OptionsToolTip.IconAllowHtmlString = DevExpress.Utils.DefaultBoolean.True;
		this.captionLabelLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.captionLabelLayoutControlItem.Size = new System.Drawing.Size(619, 13);
		this.captionLabelLayoutControlItem.Text = " ";
		this.captionLabelLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
		this.captionLabelLayoutControlItem.TextLocation = DevExpress.Utils.Locations.Right;
		this.captionLabelLayoutControlItem.TextSize = new System.Drawing.Size(3, 13);
		this.captionLabelLayoutControlItem.TextToControlDistance = 0;
		this.imageListBoxLayoutControlItem.Control = this.imageListBoxControl;
		this.imageListBoxLayoutControlItem.Location = new System.Drawing.Point(0, 44);
		this.imageListBoxLayoutControlItem.MinSize = new System.Drawing.Size(54, 4);
		this.imageListBoxLayoutControlItem.Name = "imageListBoxLayoutControlItem";
		this.imageListBoxLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.imageListBoxLayoutControlItem.Size = new System.Drawing.Size(619, 336);
		this.imageListBoxLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.imageListBoxLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.imageListBoxLayoutControlItem.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 26);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(104, 1);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(619, 18);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.Control = this.subCaptionLabelControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 13);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem1.Size = new System.Drawing.Size(619, 13);
		this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextToControlDistance = 0;
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.toolTipController.AllowHtmlText = true;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(83, 83, 83);
		base.Controls.Add(this.layoutControl);
		base.Name = "CaptionListBoxControl";
		base.Size = new System.Drawing.Size(619, 380);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.imageListBoxControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.captionLabelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.imageListBoxLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		base.ResumeLayout(false);
	}
}
