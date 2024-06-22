using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Dataedo.App.Forms.Helpers;
using Dataedo.App.Properties;
using Dataedo.App.Tools.UI;
using Dataedo.CustomControls;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;

namespace Dataedo.App.Forms;

public class ProgressWaitForm : WaitForm
{
	public enum SplashScreenCommand
	{
		SetSettings = 0,
		SetMax = 1,
		PerformStep = 2,
		SetLabel = 3,
		SwitchToMarquee = 4,
		SwitchToProgress = 5,
		SetAfterError = 6,
		SetBottomButtonAction = 7,
		SetCancellationToken = 8
	}

	private Action bottomButtonAction;

	private BackgroundWorker backgroundWorker;

	private CancellationTokenSource cancellationTokenSource;

	private IContainer components;

	private NonCustomizableLayoutControl layoutControl;

	private LayoutControlGroup Root;

	private PictureEdit pictureEdit;

	private LayoutControlItem pictureEditLayoutControlItem;

	private MarqueeProgressBarControl marqueeProgressBarControl;

	private LayoutControlItem marqueeProgressLayoutControlItem;

	private ProgressBarControl progressBarControl;

	private LayoutControlItem progressLayoutControlItem;

	private LabelControl labelControl;

	private LayoutControlItem labelControlLayoutControlItem;

	private SimpleButton bottomButton;

	private LayoutControlItem bottomButtonLayoutControlItem;

	private EmptySpaceItem bottomEmptySpaceItem;

	private SimpleSeparator bottomSeparator;

	private LabelControl titleLabelControl;

	private LayoutControlItem titleLayoutControlItem;

	private SimpleSeparator topSeparator;

	public ProgressWaitForm()
	{
		InitializeComponent();
		layoutControl.BackColor = SkinColors.ControlColorFromSystemColorsStored;
		progressBarControl.LookAndFeel.UseDefaultLookAndFeel = (marqueeProgressBarControl.LookAndFeel.UseDefaultLookAndFeel = false);
		progressBarControl.LookAndFeel.Style = (marqueeProgressBarControl.LookAndFeel.Style = LookAndFeelStyle.Flat);
		base.Height -= bottomButtonLayoutControlItem.Height + marqueeProgressBarControl.Height + titleLayoutControlItem.Height + pictureEditLayoutControlItem.Height;
		if (SkinsManager.IsCurrentSkinDark)
		{
			TargetLookAndFeel.SetSkinStyle(SkinSvgPalette.Bezier.VSDark);
		}
		else
		{
			TargetLookAndFeel.SetSkinStyle(SkinSvgPalette.Bezier.OfficeWhite);
		}
	}

	public override void ProcessCommand(Enum cmd, object arg)
	{
		base.ProcessCommand(cmd, arg);
		switch ((SplashScreenCommand)(object)cmd)
		{
		case SplashScreenCommand.SetSettings:
		{
			ProgressWaitFormSettings settings = arg as ProgressWaitFormSettings;
			SetSettings(settings);
			break;
		}
		case SplashScreenCommand.SetMax:
		{
			int maximum = (int)arg;
			progressBarControl.Properties.Maximum = maximum;
			break;
		}
		case SplashScreenCommand.PerformStep:
			progressBarControl.PerformStep();
			break;
		case SplashScreenCommand.SetLabel:
			labelControl.Text = (string)arg;
			break;
		case SplashScreenCommand.SwitchToMarquee:
			SwitchToMarquee();
			break;
		case SplashScreenCommand.SwitchToProgress:
			SwitchToProgress();
			break;
		case SplashScreenCommand.SetAfterError:
			SetAfterError();
			break;
		case SplashScreenCommand.SetBottomButtonAction:
			ShowBottomButton();
			bottomButtonAction = arg as Action;
			break;
		case SplashScreenCommand.SetCancellationToken:
			ShowBottomButton();
			cancellationTokenSource = arg as CancellationTokenSource;
			break;
		}
	}

	private void SetAfterError()
	{
		pictureEdit.EditValue = Resources.no_error;
		Color color3 = (marqueeProgressBarControl.Properties.Appearance.BackColor = (progressBarControl.Properties.Appearance.BackColor = Color.FromArgb(255, 163, 163)));
		RepositoryItemMarqueeProgressBar properties = marqueeProgressBarControl.Properties;
		RepositoryItemMarqueeProgressBar properties2 = marqueeProgressBarControl.Properties;
		RepositoryItemProgressBar properties3 = progressBarControl.Properties;
		Color color5 = (progressBarControl.Properties.EndColor = Color.FromArgb(244, 0, 0));
		Color color7 = (properties3.StartColor = color5);
		color3 = (properties.StartColor = (properties2.EndColor = color7));
		SetBottomButtonAfterError();
	}

	private void SetSettings(ProgressWaitFormSettings settings)
	{
		if (settings != null)
		{
			if (!string.IsNullOrEmpty(settings.FormTitle))
			{
				topSeparator.Visibility = LayoutVisibility.Always;
				titleLayoutControlItem.Visibility = LayoutVisibility.Always;
				titleLabelControl.Text = settings.FormTitle;
			}
			if (!string.IsNullOrEmpty(settings.ProgressLabel))
			{
				labelControl.Text = settings.ProgressLabel;
			}
			if (settings.Picture != null)
			{
				base.Height += pictureEditLayoutControlItem.Height;
				CenterWaitForm();
				pictureEditLayoutControlItem.Visibility = LayoutVisibility.Always;
				pictureEdit.EditValue = settings.Picture;
			}
			if (settings.BackgroundWorker != null)
			{
				ShowBottomButton();
				backgroundWorker = settings.BackgroundWorker;
			}
			if (settings.CancellationTokenSource != null)
			{
				ShowBottomButton();
				cancellationTokenSource = settings.CancellationTokenSource;
			}
			if (settings.ButtonAction != null)
			{
				ShowBottomButton();
				bottomButtonAction = settings.ButtonAction;
			}
			if (!string.IsNullOrEmpty(settings.ButtonWarning))
			{
				ShowBottomButton();
				bottomButton.SuperTip = new SuperToolTip();
				bottomButton.ImageOptions.Image = Resources.warning_16;
				bottomButton.SuperTip.Items.Add(settings.ButtonWarning);
			}
			if (!string.IsNullOrEmpty(settings.ButtonText))
			{
				ShowBottomButton();
				bottomButton.Text = settings.ButtonText;
			}
		}
	}

	private void SetBottomButtonAfterError()
	{
		bottomButton.Text = "Close";
		bottomButton.Tag = "CLOSE";
		bottomButton.SuperTip?.Items?.Clear();
		bottomButton.SuperTip = null;
		bottomButton.ImageOptions.Image = null;
		backgroundWorker = null;
		cancellationTokenSource = null;
		bottomButtonAction = null;
	}

	private void CenterWaitForm()
	{
		try
		{
			Form parentForm = SplashScreenManager.Default.Properties.ParentForm;
			base.Location = new Point(parentForm.Location.X + parentForm.Width / 2 - base.ClientSize.Width / 2, parentForm.Location.Y + parentForm.Height / 2 - base.ClientSize.Height / 2);
		}
		catch
		{
			CenterToScreen();
		}
	}

	private void ShowBottomButton()
	{
		if (bottomButtonLayoutControlItem.Visibility == LayoutVisibility.Never)
		{
			base.Height += bottomButtonLayoutControlItem.Height;
			CenterWaitForm();
			bottomButtonLayoutControlItem.Visibility = LayoutVisibility.Always;
			bottomEmptySpaceItem.Visibility = LayoutVisibility.Always;
			bottomSeparator.Visibility = LayoutVisibility.Always;
		}
	}

	private void SwitchToMarquee()
	{
		marqueeProgressLayoutControlItem.Visibility = LayoutVisibility.Always;
		progressLayoutControlItem.Visibility = LayoutVisibility.Never;
	}

	private void SwitchToProgress()
	{
		marqueeProgressLayoutControlItem.Visibility = LayoutVisibility.Never;
		progressLayoutControlItem.Visibility = LayoutVisibility.Always;
		progressBarControl.EditValue = 0;
	}

	private void BottomButton_Click(object sender, EventArgs e)
	{
		bottomButtonAction?.Invoke();
		backgroundWorker?.CancelAsync();
		cancellationTokenSource?.Cancel();
	}

	private void TitleLayoutControlItem_CustomDraw(object sender, ItemCustomDrawEventArgs e)
	{
		e.DefaultDraw();
		e.Cache.FillRectangle(SkinsManager.CurrentSkin.ControlBackColor, e.Bounds);
		e.Handled = true;
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
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.titleLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.pictureEdit = new DevExpress.XtraEditors.PictureEdit();
		this.marqueeProgressBarControl = new DevExpress.XtraEditors.MarqueeProgressBarControl();
		this.progressBarControl = new DevExpress.XtraEditors.ProgressBarControl();
		this.labelControl = new DevExpress.XtraEditors.LabelControl();
		this.bottomButton = new DevExpress.XtraEditors.SimpleButton();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.pictureEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.marqueeProgressLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.progressLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.labelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.bottomButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.bottomEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.bottomSeparator = new DevExpress.XtraLayout.SimpleSeparator();
		this.titleLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.topSeparator = new DevExpress.XtraLayout.SimpleSeparator();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.marqueeProgressBarControl.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.progressBarControl.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pictureEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.marqueeProgressLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.labelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomSeparator).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.topSeparator).BeginInit();
		base.SuspendLayout();
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl.Controls.Add(this.titleLabelControl);
		this.layoutControl.Controls.Add(this.pictureEdit);
		this.layoutControl.Controls.Add(this.marqueeProgressBarControl);
		this.layoutControl.Controls.Add(this.progressBarControl);
		this.layoutControl.Controls.Add(this.labelControl);
		this.layoutControl.Controls.Add(this.bottomButton);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Margin = new System.Windows.Forms.Padding(2);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(574, 0, 812, 500);
		this.layoutControl.Root = this.Root;
		this.layoutControl.Size = new System.Drawing.Size(386, 445);
		this.layoutControl.TabIndex = 2;
		this.layoutControl.Text = "layoutControl1";
		this.titleLabelControl.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
		this.titleLabelControl.Appearance.Options.UseFont = true;
		this.titleLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
		this.titleLabelControl.ImageOptions.Alignment = System.Drawing.ContentAlignment.MiddleLeft;
		this.titleLabelControl.ImageOptions.Image = Dataedo.App.Properties.Resources.icon_16;
		this.titleLabelControl.IndentBetweenImageAndText = 8;
		this.titleLabelControl.Location = new System.Drawing.Point(3, 3);
		this.titleLabelControl.Margin = new System.Windows.Forms.Padding(0);
		this.titleLabelControl.Name = "titleLabelControl";
		this.titleLabelControl.Padding = new System.Windows.Forms.Padding(10, 5, 0, 5);
		this.titleLabelControl.Size = new System.Drawing.Size(110, 30);
		this.titleLabelControl.StyleController = this.layoutControl;
		this.titleLabelControl.TabIndex = 10;
		this.titleLabelControl.Text = "labelControl1";
		this.pictureEdit.Location = new System.Drawing.Point(11, 46);
		this.pictureEdit.Name = "pictureEdit";
		this.pictureEdit.Properties.AllowFocused = false;
		this.pictureEdit.Properties.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.pictureEdit.Properties.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.pictureEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.pictureEdit.Properties.Appearance.Options.UseBackColor = true;
		this.pictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.pictureEdit.Properties.ShowMenu = false;
		this.pictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
		this.pictureEdit.Size = new System.Drawing.Size(364, 265);
		this.pictureEdit.StyleController = this.layoutControl;
		this.pictureEdit.TabIndex = 4;
		this.marqueeProgressBarControl.EditValue = 0;
		this.marqueeProgressBarControl.Location = new System.Drawing.Point(11, 323);
		this.marqueeProgressBarControl.Name = "marqueeProgressBarControl";
		this.marqueeProgressBarControl.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.marqueeProgressBarControl.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.marqueeProgressBarControl.Properties.EndColor = System.Drawing.Color.FromArgb(77, 130, 184);
		this.marqueeProgressBarControl.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.marqueeProgressBarControl.Properties.StartColor = System.Drawing.Color.FromArgb(77, 130, 184);
		this.marqueeProgressBarControl.Size = new System.Drawing.Size(364, 21);
		this.marqueeProgressBarControl.StyleController = this.layoutControl;
		this.marqueeProgressBarControl.TabIndex = 5;
		this.progressBarControl.Location = new System.Drawing.Point(11, 348);
		this.progressBarControl.Name = "progressBarControl";
		this.progressBarControl.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.progressBarControl.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.progressBarControl.Properties.EndColor = System.Drawing.Color.FromArgb(77, 130, 184);
		this.progressBarControl.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.progressBarControl.Properties.ShowTitle = true;
		this.progressBarControl.Properties.StartColor = System.Drawing.Color.FromArgb(77, 130, 184);
		this.progressBarControl.Properties.Step = 1;
		this.progressBarControl.ShowProgressInTaskBar = true;
		this.progressBarControl.Size = new System.Drawing.Size(364, 21);
		this.progressBarControl.StyleController = this.layoutControl;
		this.progressBarControl.TabIndex = 9;
		this.labelControl.Location = new System.Drawing.Point(161, 376);
		this.labelControl.Name = "labelControl";
		this.labelControl.Size = new System.Drawing.Size(63, 13);
		this.labelControl.StyleController = this.layoutControl;
		this.labelControl.TabIndex = 6;
		this.labelControl.Text = "Please wait...";
		this.bottomButton.AllowFocus = false;
		this.bottomButton.Location = new System.Drawing.Point(299, 416);
		this.bottomButton.Margin = new System.Windows.Forms.Padding(10);
		this.bottomButton.Name = "bottomButton";
		this.bottomButton.Size = new System.Drawing.Size(72, 22);
		this.bottomButton.StyleController = this.layoutControl;
		this.bottomButton.TabIndex = 8;
		this.bottomButton.Tag = "CANCEL";
		this.bottomButton.Text = "Cancel";
		this.bottomButton.Click += new System.EventHandler(BottomButton_Click);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[9] { this.pictureEditLayoutControlItem, this.marqueeProgressLayoutControlItem, this.progressLayoutControlItem, this.labelControlLayoutControlItem, this.bottomButtonLayoutControlItem, this.bottomEmptySpaceItem, this.bottomSeparator, this.titleLayoutControlItem, this.topSeparator });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(386, 445);
		this.Root.TextVisible = false;
		this.pictureEditLayoutControlItem.Control = this.pictureEdit;
		this.pictureEditLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.pictureEditLayoutControlItem.CustomizationFormText = "pictureEditLayoutControlItem";
		this.pictureEditLayoutControlItem.Location = new System.Drawing.Point(0, 35);
		this.pictureEditLayoutControlItem.MinSize = new System.Drawing.Size(24, 24);
		this.pictureEditLayoutControlItem.Name = "pictureEditLayoutControlItem";
		this.pictureEditLayoutControlItem.OptionsPrint.AppearanceItem.BackColor = System.Drawing.Color.Transparent;
		this.pictureEditLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.pictureEditLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseBackColor = true;
		this.pictureEditLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.pictureEditLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.pictureEditLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.pictureEditLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.pictureEditLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.pictureEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10);
		this.pictureEditLayoutControlItem.Size = new System.Drawing.Size(384, 285);
		this.pictureEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.pictureEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.pictureEditLayoutControlItem.TextVisible = false;
		this.pictureEditLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.marqueeProgressLayoutControlItem.Control = this.marqueeProgressBarControl;
		this.marqueeProgressLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.marqueeProgressLayoutControlItem.CustomizationFormText = "marqueeProgressLayoutControlItem";
		this.marqueeProgressLayoutControlItem.Location = new System.Drawing.Point(0, 320);
		this.marqueeProgressLayoutControlItem.MaxSize = new System.Drawing.Size(0, 25);
		this.marqueeProgressLayoutControlItem.MinSize = new System.Drawing.Size(70, 25);
		this.marqueeProgressLayoutControlItem.Name = "marqueeProgressLayoutControlItem";
		this.marqueeProgressLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.marqueeProgressLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.marqueeProgressLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.marqueeProgressLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.marqueeProgressLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.marqueeProgressLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.marqueeProgressLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 2, 2);
		this.marqueeProgressLayoutControlItem.Size = new System.Drawing.Size(384, 25);
		this.marqueeProgressLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.marqueeProgressLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.marqueeProgressLayoutControlItem.TextVisible = false;
		this.marqueeProgressLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.progressLayoutControlItem.Control = this.progressBarControl;
		this.progressLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.progressLayoutControlItem.CustomizationFormText = "progressLayoutControlItem";
		this.progressLayoutControlItem.Location = new System.Drawing.Point(0, 345);
		this.progressLayoutControlItem.MaxSize = new System.Drawing.Size(0, 25);
		this.progressLayoutControlItem.MinSize = new System.Drawing.Size(70, 25);
		this.progressLayoutControlItem.Name = "progressLayoutControlItem";
		this.progressLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.progressLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.progressLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.progressLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.progressLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.progressLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.progressLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 2, 2);
		this.progressLayoutControlItem.Size = new System.Drawing.Size(384, 25);
		this.progressLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.progressLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.progressLayoutControlItem.TextVisible = false;
		this.labelControlLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.labelControlLayoutControlItem.Control = this.labelControl;
		this.labelControlLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.labelControlLayoutControlItem.CustomizationFormText = "layoutControlItem3";
		this.labelControlLayoutControlItem.Location = new System.Drawing.Point(0, 370);
		this.labelControlLayoutControlItem.Name = "labelControlLayoutControlItem";
		this.labelControlLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.labelControlLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.labelControlLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.labelControlLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.labelControlLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.labelControlLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.labelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 5, 20);
		this.labelControlLayoutControlItem.Size = new System.Drawing.Size(384, 38);
		this.labelControlLayoutControlItem.Text = "layoutControlItem3";
		this.labelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.labelControlLayoutControlItem.TextVisible = false;
		this.bottomButtonLayoutControlItem.Control = this.bottomButton;
		this.bottomButtonLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.bottomButtonLayoutControlItem.CustomizationFormText = "runClassificationButtonLayoutControlItem";
		this.bottomButtonLayoutControlItem.Location = new System.Drawing.Point(284, 409);
		this.bottomButtonLayoutControlItem.MaxSize = new System.Drawing.Size(100, 34);
		this.bottomButtonLayoutControlItem.MinSize = new System.Drawing.Size(100, 34);
		this.bottomButtonLayoutControlItem.Name = "bottomButtonLayoutControlItem";
		this.bottomButtonLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.bottomButtonLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.bottomButtonLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.bottomButtonLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.bottomButtonLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.bottomButtonLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.bottomButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(14, 14, 6, 6);
		this.bottomButtonLayoutControlItem.Size = new System.Drawing.Size(100, 34);
		this.bottomButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.bottomButtonLayoutControlItem.Text = "runClassificationButtonLayoutControlItem";
		this.bottomButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.bottomButtonLayoutControlItem.TextVisible = false;
		this.bottomButtonLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.bottomEmptySpaceItem.AllowHotTrack = false;
		this.bottomEmptySpaceItem.Location = new System.Drawing.Point(0, 409);
		this.bottomEmptySpaceItem.Name = "bottomEmptySpaceItem";
		this.bottomEmptySpaceItem.Size = new System.Drawing.Size(284, 34);
		this.bottomEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.bottomEmptySpaceItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.bottomSeparator.AllowHotTrack = false;
		this.bottomSeparator.Location = new System.Drawing.Point(0, 408);
		this.bottomSeparator.Name = "bottomSeparator";
		this.bottomSeparator.Size = new System.Drawing.Size(384, 1);
		this.bottomSeparator.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.titleLayoutControlItem.Control = this.titleLabelControl;
		this.titleLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.titleLayoutControlItem.Name = "titleLayoutControlItem";
		this.titleLayoutControlItem.Size = new System.Drawing.Size(384, 34);
		this.titleLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.titleLayoutControlItem.TextVisible = false;
		this.titleLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.titleLayoutControlItem.CustomDraw += new System.EventHandler<DevExpress.XtraLayout.ItemCustomDrawEventArgs>(TitleLayoutControlItem_CustomDraw);
		this.topSeparator.AllowHotTrack = false;
		this.topSeparator.Location = new System.Drawing.Point(0, 34);
		this.topSeparator.Name = "topSeparator";
		this.topSeparator.Size = new System.Drawing.Size(384, 1);
		this.topSeparator.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(386, 445);
		base.Controls.Add(this.layoutControl);
		base.Margin = new System.Windows.Forms.Padding(2);
		base.Name = "ProgressWaitForm";
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pictureEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.marqueeProgressBarControl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.progressBarControl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pictureEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.marqueeProgressLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.labelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomSeparator).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.topSeparator).EndInit();
		base.ResumeLayout(false);
	}
}
