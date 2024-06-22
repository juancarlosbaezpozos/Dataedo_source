using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Dataedo.App.Tools;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;

namespace Dataedo.App.LoginFormTools.UserControls.Common;

[ToolboxItem(true)]
public class OptionButtonUserControl : SimpleButton
{
	private class ButtonPainter : BaseButtonPainter
	{
		private Rectangle bounds;

		public Image ToolTipImage { get; set; }

		public bool HasImage => ToolTipImage != null;

		public Rectangle ToolTipImageBounds => new Rectangle(bounds.Width - (ToolTipImage?.Width ?? 0) - 10, 10, ToolTipImage?.Width ?? 0, ToolTipImage?.Height ?? 0);

		protected override void DrawContent(ControlGraphicsInfoArgs info)
		{
			base.DrawContent(info);
			bounds = info.Bounds;
			if (ToolTipImage != null)
			{
				SimpleButtonViewInfo simpleButtonViewInfo = info.ViewInfo as SimpleButtonViewInfo;
				info.Cache.DrawImage(ToolTipImage, new Point(simpleButtonViewInfo.Bounds.Width - ToolTipImage.Width - 10, 10));
			}
		}
	}

	private readonly ToolTipController hintImageToolTipController;

	private readonly SuperToolTip hintImageSuperToolTip;

	private ButtonPainter painter;

	private Image hintImage;

	private bool isHintImageHovered;

	private IContainer components;

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public Image HintImage
	{
		get
		{
			return hintImage;
		}
		set
		{
			if (hintImage != value)
			{
				hintImage = value;
				painter.ToolTipImage = hintImage;
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Bindable(true)]
	public string HintImageToolTipHeader { get; set; }

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Bindable(true)]
	[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
	public string HintImageToolTipText { get; set; }

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Bindable(true)]
	public int HintImageMaxToolTipWidth
	{
		get
		{
			return hintImageSuperToolTip.MaxWidth;
		}
		set
		{
			hintImageSuperToolTip.MaxWidth = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Bindable(true)]
	public int HintImageAutoPopDelay
	{
		get
		{
			return hintImageToolTipController.AutoPopDelay;
		}
		set
		{
			hintImageToolTipController.AutoPopDelay = value;
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
			return hintImageToolTipController.KeepWhileHovered;
		}
		set
		{
			hintImageToolTipController.KeepWhileHovered = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public event EventHandler HintImageToolTipMouseHover;

	public OptionButtonUserControl()
	{
		InitializeComponent();
		hintImageToolTipController = new ToolTipController();
		hintImageToolTipController.AllowHtmlText = true;
		hintImageToolTipController.HyperlinkClick += HintImageToolTipController_HyperlinkClick;
		hintImageSuperToolTip = new SuperToolTip();
		hintImageSuperToolTip.MaxWidth = 500;
		hintImageSuperToolTip.FixedTooltipWidth = false;
	}

	protected override BaseControlPainter CreatePainter()
	{
		painter = new ButtonPainter();
		return painter;
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
	}

	private void OnHintImageToolTipMouseHover()
	{
		if (!isHintImageHovered)
		{
			isHintImageHovered = true;
			this.HintImageToolTipMouseHover?.Invoke(this, EventArgs.Empty);
			ShowHintImageToolTip();
		}
	}

	private void OnHintImageToolTipMouseLeave()
	{
		if (isHintImageHovered)
		{
			isHintImageHovered = false;
			HideHintImageToolTip();
		}
	}

	private void ShowHintImageToolTip()
	{
		hintImageSuperToolTip.Items.Clear();
		if (!string.IsNullOrWhiteSpace(HintImageToolTipHeader))
		{
			ToolTipTitleItem toolTipTitleItem = new ToolTipTitleItem();
			toolTipTitleItem.Text = HintImageToolTipHeader;
			hintImageSuperToolTip.Items.Add(toolTipTitleItem);
			hintImageSuperToolTip.Items.Add(new ToolTipSeparatorItem());
		}
		if (!string.IsNullOrWhiteSpace(HintImageToolTipText))
		{
			ToolTipItem toolTipItem = new ToolTipItem();
			toolTipItem.Text = HintImageToolTipText;
			hintImageSuperToolTip.Items.Add(toolTipItem);
		}
		ToolTipControllerShowEventArgs toolTipControllerShowEventArgs = new ToolTipControllerShowEventArgs();
		toolTipControllerShowEventArgs.ToolTipType = ToolTipType.SuperTip;
		toolTipControllerShowEventArgs.SuperTip = hintImageSuperToolTip;
		hintImageToolTipController.ShowHint(toolTipControllerShowEventArgs, Control.MousePosition);
	}

	private void HideHintImageToolTip()
	{
		hintImageToolTipController.HideHint();
	}

	private void OptionButtonUserControl_MouseMove(object sender, MouseEventArgs e)
	{
		if (painter.HasImage)
		{
			if (painter.ToolTipImageBounds.Contains(e.Location))
			{
				OnHintImageToolTipMouseHover();
			}
			else
			{
				OnHintImageToolTipMouseLeave();
			}
		}
	}

	private void HintImageToolTipController_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (!string.IsNullOrEmpty(e.Link))
		{
			Links.OpenLink(e.Link);
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
		base.SuspendLayout();
		base.MouseMove += new System.Windows.Forms.MouseEventHandler(OptionButtonUserControl_MouseMove);
		base.ResumeLayout(false);
	}
}
