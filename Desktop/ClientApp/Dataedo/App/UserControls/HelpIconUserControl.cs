using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.UserControls.Base;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Dataedo.App.UserControls;

public class HelpIconUserControl : BaseUserControl
{
	private readonly SuperToolTip superToolTip;

	private MouseEventHandler mouseClick;

	private MouseEventHandler mouseDoubleClick;

	private EventHandler click;

	private EventHandler doubleClick;

	private IContainer components;

	private PictureEdit advancedHelpPictureEdit;

	private ToolTipController toolTipController;

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Bindable(true)]
	public string ToolTipHeader { get; set; }

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Bindable(true)]
	[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
	public string ToolTipText { get; set; }

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Bindable(true)]
	public int AutoPopDelay
	{
		get
		{
			return toolTipController.AutoPopDelay;
		}
		set
		{
			toolTipController.AutoPopDelay = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Bindable(true)]
	public bool KeepWhileHovered
	{
		get
		{
			return toolTipController.KeepWhileHovered;
		}
		set
		{
			toolTipController.KeepWhileHovered = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Bindable(true)]
	public int MaxToolTipWidth
	{
		get
		{
			return superToolTip.MaxWidth;
		}
		set
		{
			superToolTip.MaxWidth = value;
		}
	}

	[Browsable(true)]
	public new event EventHandler Click
	{
		add
		{
			click = (EventHandler)Delegate.Combine(click, value);
		}
		remove
		{
			click = (EventHandler)Delegate.Remove(click, value);
		}
	}

	[Browsable(true)]
	public new event EventHandler DoubleClick
	{
		add
		{
			doubleClick = (EventHandler)Delegate.Combine(doubleClick, value);
		}
		remove
		{
			doubleClick = (EventHandler)Delegate.Remove(doubleClick, value);
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

	public HelpIconUserControl()
	{
		InitializeComponent();
		superToolTip = new SuperToolTip();
		superToolTip.MaxWidth = 500;
		superToolTip.FixedTooltipWidth = false;
	}

	private void HelpIconUserControl_Load(object sender, EventArgs e)
	{
		BackColor = base.Parent.BackColor;
	}

	private void AdvancedHelpPictureEdit_MouseHover(object sender, EventArgs e)
	{
		superToolTip.Items.Clear();
		if (!string.IsNullOrWhiteSpace(ToolTipHeader))
		{
			ToolTipTitleItem toolTipTitleItem = new ToolTipTitleItem();
			toolTipTitleItem.Text = ToolTipHeader;
			superToolTip.Items.Add(toolTipTitleItem);
			superToolTip.Items.Add(new ToolTipSeparatorItem());
		}
		if (!string.IsNullOrWhiteSpace(ToolTipText))
		{
			ToolTipItem toolTipItem = new ToolTipItem();
			toolTipItem.Text = ToolTipText;
			superToolTip.Items.Add(toolTipItem);
		}
		ToolTipControllerShowEventArgs toolTipControllerShowEventArgs = new ToolTipControllerShowEventArgs();
		toolTipControllerShowEventArgs.ToolTipType = ToolTipType.SuperTip;
		toolTipControllerShowEventArgs.SuperTip = superToolTip;
		toolTipController.ShowHint(toolTipControllerShowEventArgs, Control.MousePosition);
	}

	private void AdvancedHelpPictureEdit_MouseLeave(object sender, EventArgs e)
	{
		toolTipController.HideHint();
	}

	private void ToolTipController_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (!string.IsNullOrEmpty(e.Link))
		{
			Links.OpenLink(e.Link, FindForm());
		}
	}

	private void AdvancedHelpPictureEdit_Click(object sender, EventArgs e)
	{
		click?.Invoke(sender, e);
	}

	private void AdvancedHelpPictureEdit_DoubleClick(object sender, EventArgs e)
	{
		doubleClick?.Invoke(sender, e);
	}

	private void AdvancedHelpPictureEdit_MouseClick(object sender, MouseEventArgs e)
	{
		mouseClick?.Invoke(sender, e);
	}

	private void AdvancedHelpPictureEdit_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		mouseDoubleClick?.Invoke(sender, e);
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
		this.advancedHelpPictureEdit = new DevExpress.XtraEditors.PictureEdit();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		((System.ComponentModel.ISupportInitialize)this.advancedHelpPictureEdit.Properties).BeginInit();
		base.SuspendLayout();
		this.advancedHelpPictureEdit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.advancedHelpPictureEdit.EditValue = Dataedo.App.Properties.Resources.question_16;
		this.advancedHelpPictureEdit.Location = new System.Drawing.Point(0, 0);
		this.advancedHelpPictureEdit.Margin = new System.Windows.Forms.Padding(0);
		this.advancedHelpPictureEdit.MaximumSize = new System.Drawing.Size(20, 20);
		this.advancedHelpPictureEdit.MinimumSize = new System.Drawing.Size(20, 20);
		this.advancedHelpPictureEdit.Name = "advancedHelpPictureEdit";
		this.advancedHelpPictureEdit.Properties.AllowFocused = false;
		this.advancedHelpPictureEdit.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.advancedHelpPictureEdit.Properties.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.advancedHelpPictureEdit.Properties.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.advancedHelpPictureEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.advancedHelpPictureEdit.Properties.Appearance.Options.UseBackColor = true;
		this.advancedHelpPictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.advancedHelpPictureEdit.Properties.ShowMenu = false;
		this.advancedHelpPictureEdit.Size = new System.Drawing.Size(20, 20);
		this.advancedHelpPictureEdit.TabIndex = 30;
		this.advancedHelpPictureEdit.Click += new System.EventHandler(AdvancedHelpPictureEdit_Click);
		this.advancedHelpPictureEdit.DoubleClick += new System.EventHandler(AdvancedHelpPictureEdit_DoubleClick);
		this.advancedHelpPictureEdit.MouseClick += new System.Windows.Forms.MouseEventHandler(AdvancedHelpPictureEdit_MouseClick);
		this.advancedHelpPictureEdit.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(AdvancedHelpPictureEdit_MouseDoubleClick);
		this.advancedHelpPictureEdit.MouseLeave += new System.EventHandler(AdvancedHelpPictureEdit_MouseLeave);
		this.advancedHelpPictureEdit.MouseHover += new System.EventHandler(AdvancedHelpPictureEdit_MouseHover);
		this.toolTipController.AllowHtmlText = true;
		this.toolTipController.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(ToolTipController_HyperlinkClick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Controls.Add(this.advancedHelpPictureEdit);
		this.MaximumSize = new System.Drawing.Size(20, 20);
		this.MinimumSize = new System.Drawing.Size(20, 20);
		base.Name = "HelpIconUserControl";
		base.Size = new System.Drawing.Size(20, 20);
		base.Load += new System.EventHandler(HelpIconUserControl_Load);
		((System.ComponentModel.ISupportInitialize)this.advancedHelpPictureEdit.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
