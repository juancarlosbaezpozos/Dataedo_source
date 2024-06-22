using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Tools.UI;
using Dataedo.CustomControls;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using RecentProjectsLibrary;

namespace Dataedo.App.UserControls;

public class LoginCaptionListBoxControl : UserControl
{
	private MouseEventHandler mouseClick;

	private MouseEventHandler mouseDoubleClick;

	private EventHandler selectedIndexChanged;

	private int lastSelectedIndex = -1;

	private int? itemsHeight;

	private int currentToolTipItemIndex = -1;

	private Timer toolTipTimer = new Timer();

	private IContainer components;

	private LabelControl captionLabelControl;

	private NonCustomizableLayoutControl layoutControl;

	private LayoutControlGroup layoutControlGroup1;

	private ImageListBoxControl imageListBoxControl;

	private LayoutControlItem captionLabelLayoutControlItem;

	private LayoutControlItem imageListBoxLayoutControlItem;

	private EmptySpaceItem emptySpaceItem;

	private ToolTipController toolTipController;

	private LabelControl subCaptionLabelControl;

	private LayoutControlItem subCaptionLabelLayoutControlItem;

	private EmptySpaceItem bottomEmptySpaceItem;

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
				subCaptionLabelLayoutControlItem.Visibility = LayoutVisibility.Always;
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

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public int ItemHeight
	{
		get
		{
			return imageListBoxControl.ItemHeight;
		}
		set
		{
			imageListBoxControl.ItemHeight = value;
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

	public bool IsAnySelected => SelectedIndex != -1;

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public int? ItemsHeight
	{
		get
		{
			return itemsHeight;
		}
		set
		{
			if (itemsHeight != value)
			{
				itemsHeight = value;
				if (itemsHeight.HasValue)
				{
					bottomEmptySpaceItem.SizeConstraintsType = SizeConstraintsType.Default;
					Size size3 = (bottomEmptySpaceItem.MinSize = (bottomEmptySpaceItem.MaxSize = new Size(0, 1)));
					imageListBoxLayoutControlItem.SizeConstraintsType = SizeConstraintsType.Custom;
					imageListBoxControl.Height = itemsHeight.Value;
					size3 = (imageListBoxLayoutControlItem.MinSize = (imageListBoxLayoutControlItem.MaxSize = new Size(0, itemsHeight.Value + 1)));
				}
				else
				{
					imageListBoxLayoutControlItem.SizeConstraintsType = SizeConstraintsType.Default;
					bottomEmptySpaceItem.SizeConstraintsType = SizeConstraintsType.Custom;
					Size size3 = (bottomEmptySpaceItem.MinSize = (bottomEmptySpaceItem.MaxSize = new Size(0, 1)));
				}
			}
		}
	}

	[Browsable(true)]
	public new event MouseEventHandler MouseClick
	{
		add
		{
			mouseClick = (MouseEventHandler)Delegate.Combine(mouseClick, value);
		}
		remove
		{
			mouseClick = (MouseEventHandler)Delegate.Remove(mouseClick, value);
		}
	}

	[Browsable(true)]
	public new event MouseEventHandler MouseDoubleClick
	{
		add
		{
			mouseDoubleClick = (MouseEventHandler)Delegate.Combine(mouseDoubleClick, value);
		}
		remove
		{
			mouseDoubleClick = (MouseEventHandler)Delegate.Remove(mouseDoubleClick, value);
		}
	}

	[Browsable(true)]
	public event EventHandler SelectedIndexChanged
	{
		add
		{
			selectedIndexChanged = (EventHandler)Delegate.Combine(selectedIndexChanged, value);
		}
		remove
		{
			selectedIndexChanged = (EventHandler)Delegate.Remove(selectedIndexChanged, value);
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

	public LoginCaptionListBoxControl()
	{
		InitializeComponent();
		imageListBoxControl.BackColor = SkinColors.ControlColorFromSystemColors;
		imageListBoxControl.Appearance.Options.UseBackColor = true;
		captionLabelControl.ForeColor = SkinsManager.CurrentSkin.LoginFormTitleForeColor;
		imageListBoxControl.MouseClick += ImageListBoxControl_MouseClick;
		imageListBoxControl.MouseDoubleClick += ImageListBoxControl_MouseDoubleClick;
		toolTipTimer = new Timer();
		toolTipTimer.Interval = 500;
		toolTipTimer.Tick += ToolTipTimer_Tick;
	}

	public void SelectFirstIfAnyNotSelected()
	{
		if (!IsAnySelected && (imageListBoxControl.DataSource as List<RecentProject>).Count > 0)
		{
			SelectedIndex = 0;
		}
	}

	public void SelectLastSelectedIfAnyNotSelected()
	{
		if (!IsAnySelected && (imageListBoxControl.DataSource as List<RecentProject>).Count > 0)
		{
			SelectedIndex = ((lastSelectedIndex != -1 && lastSelectedIndex < (imageListBoxControl.DataSource as List<RecentProject>).Count) ? lastSelectedIndex : 0);
		}
	}

	private void ImageListBoxControl_MouseClick(object sender, MouseEventArgs e)
	{
		base.ParentForm.BeginInvoke((Action)delegate
		{
			mouseClick(sender, e);
		});
		int num = imageListBoxControl.IndexFromPoint(e.Location);
		if (num > -1 && e.Button == MouseButtons.Right)
		{
			imageListBoxControl.SelectedIndex = num;
		}
	}

	private void ImageListBoxControl_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		base.ParentForm.BeginInvoke((Action)delegate
		{
			mouseDoubleClick(sender, e);
		});
	}

	private void listBoxControl_DrawItem(object sender, ListBoxDrawItemEventArgs e)
	{
		if (e.State != 0)
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.ListSelectionBackColor;
		}
	}

	private void imageListBoxControl_SelectedIndexChanged(object sender, EventArgs e)
	{
		selectedIndexChanged?.Invoke(sender, e);
		if (imageListBoxControl.SelectedIndex != -1)
		{
			lastSelectedIndex = imageListBoxControl.SelectedIndex;
		}
	}

	private void imageListBoxControl_MouseLeave(object sender, EventArgs e)
	{
		toolTipController.HideHint();
		currentToolTipItemIndex = -1;
		toolTipTimer.Stop();
	}

	private void imageListBoxControl_MouseMove(object sender, MouseEventArgs e)
	{
		int num = imageListBoxControl.IndexFromPoint(PointToClient(Control.MousePosition));
		if (num <= 0)
		{
			currentToolTipItemIndex = -1;
			toolTipTimer.Stop();
			toolTipController.HideHint();
		}
		else if (num != currentToolTipItemIndex)
		{
			toolTipController.HideHint();
			currentToolTipItemIndex = imageListBoxControl.IndexFromPoint(PointToClient(Control.MousePosition));
			if (!string.IsNullOrEmpty((imageListBoxControl.DataSource as List<RecentProject>)?[currentToolTipItemIndex - 1]?.ToolTipText))
			{
				toolTipTimer.Start();
			}
		}
	}

	private void ToolTipTimer_Tick(object sender, EventArgs e)
	{
		string value = (imageListBoxControl.DataSource as List<RecentProject>)?[currentToolTipItemIndex - 1]?.ToolTipText;
		if (!string.IsNullOrEmpty(value))
		{
			SuperToolTip superToolTip = new SuperToolTip();
			superToolTip.MaxWidth = base.ParentForm.Width;
			superToolTip.FixedTooltipWidth = false;
			ToolTipItem toolTipItem = new ToolTipItem();
			toolTipItem.Text = value;
			superToolTip.Items.Add(toolTipItem);
			ToolTipControllerShowEventArgs toolTipControllerShowEventArgs = new ToolTipControllerShowEventArgs();
			toolTipControllerShowEventArgs.ToolTipType = ToolTipType.SuperTip;
			toolTipControllerShowEventArgs.SuperTip = superToolTip;
			toolTipController.ShowHint(toolTipControllerShowEventArgs, Control.MousePosition);
			toolTipTimer.Stop();
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Up:
			if (SelectedIndex - 1 >= 0)
			{
				SelectedIndex--;
			}
			return true;
		case Keys.Down:
			if (SelectedIndex + 1 < (imageListBoxControl.DataSource as List<RecentProject>).Count)
			{
				SelectedIndex++;
			}
			return true;
		default:
			return base.ProcessCmdKey(ref msg, keyData);
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
		this.components = new System.ComponentModel.Container();
		this.captionLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.subCaptionLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.imageListBoxControl = new DevExpress.XtraEditors.ImageListBoxControl();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.captionLabelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.imageListBoxLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.subCaptionLabelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.bottomEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.imageListBoxControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.captionLabelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.imageListBoxLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.subCaptionLabelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.captionLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75f);
		this.captionLabelControl.Appearance.ForeColor = System.Drawing.Color.LightGray;
		this.captionLabelControl.Appearance.Options.UseFont = true;
		this.captionLabelControl.Appearance.Options.UseForeColor = true;
		this.captionLabelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.captionLabelControl.Location = new System.Drawing.Point(0, 0);
		this.captionLabelControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.captionLabelControl.LookAndFeel.UseDefaultLookAndFeel = false;
		this.captionLabelControl.Name = "captionLabelControl";
		this.captionLabelControl.Size = new System.Drawing.Size(619, 30);
		this.captionLabelControl.TabIndex = 0;
		this.captionLabelControl.Text = "Caption";
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.Controls.Add(this.subCaptionLabelControl);
		this.layoutControl.Controls.Add(this.imageListBoxControl);
		this.layoutControl.Controls.Add(this.captionLabelControl);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1417, 374, 1038, 630);
		this.layoutControl.OptionsView.ShareLookAndFeelWithChildren = false;
		this.layoutControl.Root = this.layoutControlGroup1;
		this.layoutControl.Size = new System.Drawing.Size(619, 380);
		this.layoutControl.TabIndex = 1;
		this.layoutControl.Text = "layoutControl1";
		this.layoutControl.ToolTipController = this.toolTipController;
		this.subCaptionLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 10f);
		this.subCaptionLabelControl.Appearance.ForeColor = System.Drawing.Color.Gray;
		this.subCaptionLabelControl.Appearance.Options.UseFont = true;
		this.subCaptionLabelControl.Appearance.Options.UseForeColor = true;
		this.subCaptionLabelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.subCaptionLabelControl.Location = new System.Drawing.Point(0, 30);
		this.subCaptionLabelControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.subCaptionLabelControl.Name = "subCaptionLabelControl";
		this.subCaptionLabelControl.Size = new System.Drawing.Size(619, 16);
		this.subCaptionLabelControl.TabIndex = 5;
		this.subCaptionLabelControl.Text = "Subcaption";
		this.imageListBoxControl.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.imageListBoxControl.Appearance.Font = new System.Drawing.Font("Tahoma", 12f);
		this.imageListBoxControl.Appearance.Options.UseFont = true;
		this.imageListBoxControl.Appearance.Options.UseTextOptions = true;
		this.imageListBoxControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.imageListBoxControl.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.imageListBoxControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.imageListBoxControl.HotTrackSelectMode = DevExpress.XtraEditors.HotTrackSelectMode.SelectItemOnClick;
		this.imageListBoxControl.ItemHeight = 64;
		this.imageListBoxControl.ItemPadding = new System.Windows.Forms.Padding(10, 10, -10, 10);
		this.imageListBoxControl.Location = new System.Drawing.Point(0, 66);
		this.imageListBoxControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.imageListBoxControl.Name = "imageListBoxControl";
		this.imageListBoxControl.ShowFocusRect = false;
		this.imageListBoxControl.Size = new System.Drawing.Size(619, 309);
		this.imageListBoxControl.TabIndex = 4;
		this.imageListBoxControl.ToolTipController = this.toolTipController;
		this.imageListBoxControl.SelectedIndexChanged += new System.EventHandler(imageListBoxControl_SelectedIndexChanged);
		this.imageListBoxControl.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(listBoxControl_DrawItem);
		this.imageListBoxControl.MouseLeave += new System.EventHandler(imageListBoxControl_MouseLeave);
		this.imageListBoxControl.MouseMove += new System.Windows.Forms.MouseEventHandler(imageListBoxControl_MouseMove);
		this.toolTipController.AllowHtmlText = true;
		this.toolTipController.Appearance.Options.UseTextOptions = true;
		this.toolTipController.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.toolTipController.AutoPopDelay = 32000;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.captionLabelLayoutControlItem, this.imageListBoxLayoutControlItem, this.emptySpaceItem, this.subCaptionLabelLayoutControlItem, this.bottomEmptySpaceItem });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(619, 380);
		this.layoutControlGroup1.TextVisible = false;
		this.captionLabelLayoutControlItem.Control = this.captionLabelControl;
		this.captionLabelLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
		this.captionLabelLayoutControlItem.CustomizationFormText = "captionLabelLayoutControlItem";
		this.captionLabelLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.captionLabelLayoutControlItem.MaxSize = new System.Drawing.Size(0, 30);
		this.captionLabelLayoutControlItem.MinSize = new System.Drawing.Size(1, 30);
		this.captionLabelLayoutControlItem.Name = "captionLabelLayoutControlItem";
		this.captionLabelLayoutControlItem.OptionsToolTip.AllowHtmlString = DevExpress.Utils.DefaultBoolean.True;
		this.captionLabelLayoutControlItem.OptionsToolTip.IconAllowHtmlString = DevExpress.Utils.DefaultBoolean.True;
		this.captionLabelLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.captionLabelLayoutControlItem.Size = new System.Drawing.Size(619, 30);
		this.captionLabelLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.captionLabelLayoutControlItem.Text = " ";
		this.captionLabelLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
		this.captionLabelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.captionLabelLayoutControlItem.TextToControlDistance = 0;
		this.captionLabelLayoutControlItem.TextVisible = false;
		this.imageListBoxLayoutControlItem.Control = this.imageListBoxControl;
		this.imageListBoxLayoutControlItem.Location = new System.Drawing.Point(0, 66);
		this.imageListBoxLayoutControlItem.Name = "imageListBoxLayoutControlItem";
		this.imageListBoxLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.imageListBoxLayoutControlItem.Size = new System.Drawing.Size(619, 309);
		this.imageListBoxLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.imageListBoxLayoutControlItem.TextVisible = false;
		this.emptySpaceItem.AllowHotTrack = false;
		this.emptySpaceItem.Location = new System.Drawing.Point(0, 46);
		this.emptySpaceItem.MaxSize = new System.Drawing.Size(0, 20);
		this.emptySpaceItem.MinSize = new System.Drawing.Size(104, 20);
		this.emptySpaceItem.Name = "emptySpaceItem";
		this.emptySpaceItem.Size = new System.Drawing.Size(619, 20);
		this.emptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.subCaptionLabelLayoutControlItem.Control = this.subCaptionLabelControl;
		this.subCaptionLabelLayoutControlItem.Location = new System.Drawing.Point(0, 30);
		this.subCaptionLabelLayoutControlItem.MaxSize = new System.Drawing.Size(0, 16);
		this.subCaptionLabelLayoutControlItem.MinSize = new System.Drawing.Size(1, 16);
		this.subCaptionLabelLayoutControlItem.Name = "subCaptionLabelLayoutControlItem";
		this.subCaptionLabelLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.subCaptionLabelLayoutControlItem.Size = new System.Drawing.Size(619, 16);
		this.subCaptionLabelLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.subCaptionLabelLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
		this.subCaptionLabelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.subCaptionLabelLayoutControlItem.TextToControlDistance = 0;
		this.subCaptionLabelLayoutControlItem.TextVisible = false;
		this.subCaptionLabelLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.bottomEmptySpaceItem.AllowHotTrack = false;
		this.bottomEmptySpaceItem.Location = new System.Drawing.Point(0, 375);
		this.bottomEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 5);
		this.bottomEmptySpaceItem.MinSize = new System.Drawing.Size(104, 5);
		this.bottomEmptySpaceItem.Name = "bottomEmptySpaceItem";
		this.bottomEmptySpaceItem.Size = new System.Drawing.Size(619, 5);
		this.bottomEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.bottomEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.layoutControl);
		base.Name = "LoginCaptionListBoxControl";
		base.Size = new System.Drawing.Size(619, 380);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.imageListBoxControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.captionLabelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.imageListBoxLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.subCaptionLabelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).EndInit();
		base.ResumeLayout(false);
	}
}
