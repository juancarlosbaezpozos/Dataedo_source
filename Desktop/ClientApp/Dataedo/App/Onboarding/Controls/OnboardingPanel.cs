using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Tools.UI;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.Onboarding.Controls;

public class OnboardingPanel : FlyoutPanel
{
	public class OnboardingPanelBeakForm : FlyoutPanelBeakForm
	{
		//protected internal override bool SyncLocationWithOwner
		//{
		//	protected get
		//	{
		//		return base.SyncLocationWithOwner;
		//	}
		//}

		public OnboardingPanelBeakForm(Control owner, FlyoutPanel flyoutPanel, FlyoutPanelOptions options)
			: base(owner, flyoutPanel, options)
		{
		}

		protected override void CheckToolWindowLocation()
		{
		}
	}

	private IOverlaySplashScreenHandle overlaySplashScreenHandle;

	private Func<Rectangle> getRectangle;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private SvgImageBox closeSvgImageBox;

	private LayoutControlItem closeSvgImageBoxLayoutControlItem;

	private LabelControl contentLabelControl;

	private LayoutControlItem contentLabelControlLayoutControlItem;

	private SimpleButton okSimpleButton;

	private LayoutControlItem okSimpleButtonLayoutControlItem;

	private EmptySpaceItem okSimpleButtonEmptySpaceItem;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem horizontalSpaceEmptySpaceItem;

	private LabelControl skipLabelControl;

	private LayoutControlItem skipLabelControlLayoutControlItem;

	public OnboardingSupport.OnboardingMessages OnboardingType { get; private set; }

	public Point OwnerLocation { get; set; }

	public event EventHandler SkipClick;

	public OnboardingPanel(string text, Size size, Func<Rectangle> getRectangle, IOverlaySplashScreenHandle overlaySplashScreenHandle = null)
	{
		this.overlaySplashScreenHandle = overlaySplashScreenHandle;
		this.getRectangle = getRectangle;
		InitializeComponent();
		contentLabelControl.Text = text;
		base.Width = size.Width;
		base.Height = size.Height;
		BackColor = SkinsManager.CurrentSkin.OnboardingPanelBackColor;
		contentLabelControl.ForeColor = SkinsManager.CurrentSkin.OnboardingPanelForeColor;
	}

	public OnboardingPanel(OnboardingSupport.OnboardingMessages onboardingType, OnboardingMessage message, Size size, Func<Rectangle> getRectangle, IOverlaySplashScreenHandle overlaySplashScreenHandle = null)
		: this(message.Message, size, getRectangle, overlaySplashScreenHandle)
	{
		OnboardingType = onboardingType;
	}

	public Point CalculateLocationForCreating(Rectangle rectangle)
	{
		Point empty = Point.Empty;
		return (base.OptionsBeakPanel.BeakLocation == BeakPanelBeakLocation.Top) ? new Point(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height) : ((base.OptionsBeakPanel.BeakLocation == BeakPanelBeakLocation.Left) ? new Point(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height / 2) : ((base.OptionsBeakPanel.BeakLocation == BeakPanelBeakLocation.Bottom) ? new Point(rectangle.X + rectangle.Width / 2, rectangle.Y) : ((base.OptionsBeakPanel.BeakLocation != BeakPanelBeakLocation.Right) ? new Point(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2) : new Point(rectangle.X, rectangle.Y + rectangle.Height / 2))));
	}

	public Point CalculateLocation(Rectangle rectangle)
	{
		Point empty = Point.Empty;
		return (base.OptionsBeakPanel.BeakLocation == BeakPanelBeakLocation.Top) ? new Point(rectangle.X, rectangle.Y + rectangle.Height) : ((base.OptionsBeakPanel.BeakLocation == BeakPanelBeakLocation.Left) ? new Point(rectangle.X + rectangle.Width, rectangle.Y) : ((base.OptionsBeakPanel.BeakLocation == BeakPanelBeakLocation.Bottom) ? new Point(rectangle.X, rectangle.Y) : ((base.OptionsBeakPanel.BeakLocation != BeakPanelBeakLocation.Right) ? new Point(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2) : new Point(rectangle.X, rectangle.Y))));
	}

	public void Close()
	{
		HideBeakForm();
		if (overlaySplashScreenHandle != null)
		{
			SplashScreenManager.CloseOverlayForm(overlaySplashScreenHandle);
		}
		Dispose();
	}

	protected override FlyoutPanelToolForm CreateBeakFormCore(Control owner, FlyoutPanel content, Rectangle rect, Point loc, Point offset)
	{
		BeakFlyoutPanelOptions beakFlyoutPanelOptions = new BeakFlyoutPanelOptions(CalculateLocationForCreating(rect));
		beakFlyoutPanelOptions.AnchorType = PopupToolWindowAnchor.Manual;
		beakFlyoutPanelOptions.AnimationType = PopupToolWindowAnimation.Fade;
		return new OnboardingPanelBeakForm(owner, content, beakFlyoutPanelOptions);
	}

	private void CloseSvgImageBox_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			Close();
		}
	}

	private void OkSimpleButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void SkipLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		this.SkipClick?.Invoke(sender, e);
	}

	private void CloseSvgImageBox_MouseHover(object sender, EventArgs e)
	{
		closeSvgImageBox.BackColor = SkinsManager.CurrentSkin.OnboardingPanelCloseButtonHoverBackColor;
	}

	private void CloseSvgImageBox_MouseLeave(object sender, EventArgs e)
	{
		closeSvgImageBox.BackColor = Color.Transparent;
	}

	public bool ShowOnboardingPanel()
	{
		try
		{
			ShowBeakForm(getRectangle());
			return true;
		}
		catch
		{
			return false;
		}
	}

	private void InitializeComponent()
	{
		DevExpress.XtraEditors.SvgImageItem svgImageItem = new DevExpress.XtraEditors.SvgImageItem("1/0");
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Onboarding.Controls.OnboardingPanel));
		this.nonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.skipLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.okSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.contentLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.closeSvgImageBox = new DevExpress.XtraEditors.SvgImageBox();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.contentLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.closeSvgImageBoxLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.okSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.horizontalSpaceEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.skipLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.okSimpleButtonEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.closeSvgImageBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.contentLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.closeSvgImageBoxLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.okSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.horizontalSpaceEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.skipLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.okSimpleButtonEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.Controls.Add(this.skipLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.okSimpleButton);
		this.nonCustomizableLayoutControl.Controls.Add(this.contentLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.closeSvgImageBox);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(710, 206, 1163, 789);
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(420, 115);
		this.nonCustomizableLayoutControl.TabIndex = 0;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.skipLabelControl.AllowHtmlString = true;
		this.skipLabelControl.Location = new System.Drawing.Point(20, 54);
		this.skipLabelControl.Name = "skipLabelControl";
		this.skipLabelControl.Size = new System.Drawing.Size(48, 13);
		this.skipLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.skipLabelControl.TabIndex = 8;
		this.skipLabelControl.Text = "<href>Skip guide</href>";
		this.skipLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(SkipLabelControl_HyperlinkClick);
		this.okSimpleButton.Location = new System.Drawing.Point(335, 50);
		this.okSimpleButton.Name = "okSimpleButton";
		this.okSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.okSimpleButton.StyleController = this.nonCustomizableLayoutControl;
		this.okSimpleButton.TabIndex = 7;
		this.okSimpleButton.Text = "OK";
		this.okSimpleButton.Click += new System.EventHandler(OkSimpleButton_Click);
		this.contentLabelControl.AllowHtmlString = true;
		this.contentLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.contentLabelControl.Location = new System.Drawing.Point(20, 20);
		this.contentLabelControl.Name = "contentLabelControl";
		this.contentLabelControl.Size = new System.Drawing.Size(356, 13);
		this.contentLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.contentLabelControl.TabIndex = 6;
		this.contentLabelControl.Text = "labelControl1";
		this.closeSvgImageBox.BackColor = System.Drawing.Color.Transparent;
		svgImageItem.Appearance.Normal.FillColor = System.Drawing.SystemColors.ControlDarkDark;
		this.closeSvgImageBox.CustomizedItems.Add(svgImageItem);
		this.closeSvgImageBox.Location = new System.Drawing.Point(398, 2);
		this.closeSvgImageBox.MaximumSize = new System.Drawing.Size(20, 20);
		this.closeSvgImageBox.MinimumSize = new System.Drawing.Size(20, 20);
		this.closeSvgImageBox.Name = "closeSvgImageBox";
		this.closeSvgImageBox.Padding = new System.Windows.Forms.Padding(4);
		this.closeSvgImageBox.Size = new System.Drawing.Size(20, 20);
		this.closeSvgImageBox.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Zoom;
		this.closeSvgImageBox.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("closeSvgImageBox.SvgImage");
		this.closeSvgImageBox.TabIndex = 4;
		this.closeSvgImageBox.MouseClick += new System.Windows.Forms.MouseEventHandler(CloseSvgImageBox_MouseClick);
		this.closeSvgImageBox.MouseLeave += new System.EventHandler(CloseSvgImageBox_MouseLeave);
		this.closeSvgImageBox.MouseHover += new System.EventHandler(CloseSvgImageBox_MouseHover);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[6] { this.contentLabelControlLayoutControlItem, this.closeSvgImageBoxLayoutControlItem, this.okSimpleButtonLayoutControlItem, this.horizontalSpaceEmptySpaceItem, this.skipLabelControlLayoutControlItem, this.okSimpleButtonEmptySpaceItem });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(420, 115);
		this.Root.TextVisible = false;
		this.contentLabelControlLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Top;
		this.contentLabelControlLayoutControlItem.Control = this.contentLabelControl;
		this.contentLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.contentLabelControlLayoutControlItem.Name = "contentLabelControlLayoutControlItem";
		this.contentLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(20, 20, 20, 5);
		this.contentLabelControlLayoutControlItem.Size = new System.Drawing.Size(396, 38);
		this.contentLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.contentLabelControlLayoutControlItem.TextVisible = false;
		this.closeSvgImageBoxLayoutControlItem.Control = this.closeSvgImageBox;
		this.closeSvgImageBoxLayoutControlItem.Location = new System.Drawing.Point(396, 0);
		this.closeSvgImageBoxLayoutControlItem.MaxSize = new System.Drawing.Size(24, 0);
		this.closeSvgImageBoxLayoutControlItem.MinSize = new System.Drawing.Size(24, 24);
		this.closeSvgImageBoxLayoutControlItem.Name = "closeSvgImageBoxLayoutControlItem";
		this.closeSvgImageBoxLayoutControlItem.Size = new System.Drawing.Size(24, 38);
		this.closeSvgImageBoxLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.closeSvgImageBoxLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.closeSvgImageBoxLayoutControlItem.TextVisible = false;
		this.okSimpleButtonLayoutControlItem.Control = this.okSimpleButton;
		this.okSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(333, 48);
		this.okSimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(87, 29);
		this.okSimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(87, 29);
		this.okSimpleButtonLayoutControlItem.Name = "okSimpleButtonLayoutControlItem";
		this.okSimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 5, 2, 5);
		this.okSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(87, 67);
		this.okSimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.okSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.okSimpleButtonLayoutControlItem.TextVisible = false;
		this.horizontalSpaceEmptySpaceItem.AllowHotTrack = false;
		this.horizontalSpaceEmptySpaceItem.Location = new System.Drawing.Point(0, 38);
		this.horizontalSpaceEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 10);
		this.horizontalSpaceEmptySpaceItem.MinSize = new System.Drawing.Size(104, 10);
		this.horizontalSpaceEmptySpaceItem.Name = "emptySpaceItem1";
		this.horizontalSpaceEmptySpaceItem.Size = new System.Drawing.Size(420, 10);
		this.horizontalSpaceEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.horizontalSpaceEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.skipLabelControlLayoutControlItem.Control = this.skipLabelControl;
		this.skipLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.skipLabelControlLayoutControlItem.Name = "skipLabelControlLayoutControlItem";
		this.skipLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(20, 2, 6, 2);
		this.skipLabelControlLayoutControlItem.Size = new System.Drawing.Size(70, 67);
		this.skipLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.skipLabelControlLayoutControlItem.TextVisible = false;
		this.okSimpleButtonEmptySpaceItem.AllowHotTrack = false;
		this.okSimpleButtonEmptySpaceItem.Location = new System.Drawing.Point(70, 48);
		this.okSimpleButtonEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 29);
		this.okSimpleButtonEmptySpaceItem.MinSize = new System.Drawing.Size(1, 29);
		this.okSimpleButtonEmptySpaceItem.Name = "okSimpleButtonEmptySpaceItem";
		this.okSimpleButtonEmptySpaceItem.Size = new System.Drawing.Size(263, 67);
		this.okSimpleButtonEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.okSimpleButtonEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 45);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 5);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(104, 5);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(420, 41);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.AutoSize = true;
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.Name = "OnboardingPanel";
		base.OptionsBeakPanel.CloseOnOuterClick = false;
		base.Size = new System.Drawing.Size(420, 115);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.closeSvgImageBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.contentLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.closeSvgImageBoxLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.okSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.horizontalSpaceEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.skipLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.okSimpleButtonEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this).EndInit();
		base.ResumeLayout(false);
	}
}
