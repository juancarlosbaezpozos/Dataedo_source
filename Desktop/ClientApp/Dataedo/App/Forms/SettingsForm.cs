using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Onboarding;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.Tools.UI;
using Dataedo.CustomControls;
using Dataedo.Data.Tools;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Forms;

public class SettingsForm : BaseXtraForm
{
	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private SimpleButton cancelButton;

	private SimpleButton saveButton;

	private LayoutControlItem saveBtnLayoutControlItem;

	private LayoutControlItem cancelBtnLayoutControlItem;

	private SpinEdit timeoutSpinEdit;

	private LayoutControlItem timeoutLayoutControlItem;

	private ToggleSwitch themeToggleSwitch;

	private LayoutControlItem themeLayoutControlItem;

	private EmptySpaceItem buttonsEmptySpaceItem;

	private LayoutControlGroup configLayoutControlGroup;

	private LabelControl timeoutCommentLabelControl;

	private LabelControl onboardingCommetLabelControl;

	private LabelControl themeCommentLabelControl;

	private LayoutControlItem themeCommentLayoutControlItem;

	private LayoutControlItem onboardingCommentLayoutControlItem;

	private LayoutControlItem timeoutCommentLayoutControlItem;

	private EmptySpaceItem bottomEmptySpaceItem;

	private LabelControl onboardingLabelControl;

	private LabelControl timeoutLabelControl;

	private LabelControl themeLabelControl;

	private LayoutControlItem themeLabelLayoutControlItem;

	private LayoutControlItem timeoutLabelLayoutControlItem;

	private LayoutControlItem onboardingLabelLayoutControlItem;

	private CheckEdit onboardingCheckEdit;

	private LayoutControlItem onboardingLayoutControlItem;

	private SpinEdit repoTimeoutSpinEdit;

	private LabelControl repoTimeoutLabelControl;

	private LayoutControlItem repoTimeoutLayoutControlItem;

	private LayoutControlItem repoTimeoutLabelLayoutControlItem;

	private LabelControl repoTimeoutCommentLabelControl;

	private LayoutControlItem repoTimeoutCommentLayoutControlItem;

	private ToggleSwitch spellingToggleSwitch;

	private LayoutControlItem spellingLayoutControlItem;

	private LabelControl spellingLabelControl;

	private LayoutControlItem spellingLabelLayoutControlItem;

	private LabelControl themeCommentLabelControl1;

	private LayoutControlItem themeCommentLayoutControlItem1;

	private bool SwitchToLightThemeRequired
	{
		get
		{
			if (themeToggleSwitch.IsOn)
			{
				return SkinsManager.IsConfigSkinDark;
			}
			return false;
		}
	}

	private bool SwitchToDarkThemeRequired
	{
		get
		{
			if (!themeToggleSwitch.IsOn)
			{
				return !SkinsManager.IsConfigSkinDark;
			}
			return false;
		}
	}

	private int ConnectionTimeoutValue => (int)timeoutSpinEdit.Value;

	private int RepositoryTimeoutValue => (int)repoTimeoutSpinEdit.Value;

	private bool ResetOnboarding => onboardingCheckEdit.Checked;

	public SettingsForm()
	{
		InitializeComponent();
		Init();
		SkinsManager.SetToggleSwitchTheme(themeToggleSwitch);
		SkinsManager.SetToggleSwitchTheme(spellingToggleSwitch);
	}

	private void Init()
	{
		themeToggleSwitch.IsOn = !SkinsManager.IsConfigSkinDark;
		spellingToggleSwitch.IsOn = !LastConnectionInfo.LOGIN_INFO.TurnOffCheckSpelling;
		SetThemeLabel();
		timeoutSpinEdit.EditValue = CommandsWithTimeout.Timeout;
		if (StaticData.IsProjectFile)
		{
			repoTimeoutSpinEdit.EditValue = 0;
			repoTimeoutSpinEdit.Enabled = false;
			repoTimeoutLabelControl.Enabled = false;
			repoTimeoutCommentLabelControl.Text = "Not available for a file repository.";
		}
		else
		{
			repoTimeoutSpinEdit.EditValue = CommandsTimeout.Timeout;
		}
	}

	private void SaveButton_Click(object sender, EventArgs e)
	{
		if (SwitchToLightThemeRequired || SwitchToDarkThemeRequired)
		{
			SkinsManager.SwitchTheme(showMessage: false);
		}
		if (ResetOnboarding)
		{
			OnboardingSupport.ResetOnboarding();
		}
		if (ConnectionTimeoutValue != CommandsWithTimeout.Timeout)
		{
			CommandsWithTimeout.Timeout = ConnectionTimeoutValue;
			LastConnectionInfo.SetConnectionTimeout(ConnectionTimeoutValue);
		}
		if (!StaticData.IsProjectFile && RepositoryTimeoutValue != CommandsTimeout.Timeout)
		{
			CommandsTimeout.Timeout = RepositoryTimeoutValue;
			LastConnectionInfo.SetRepositoryTimeout(RepositoryTimeoutValue);
			StaticData.Commands.Manipulation.ConfigurationData.CommandsTimeout = RepositoryTimeoutValue;
			StaticData.Commands.Select.ConfigurationData.CommandsTimeout = RepositoryTimeoutValue;
			StaticData.Commands.Synchronization.ConfigurationData.CommandsTimeout = RepositoryTimeoutValue;
		}
		LastConnectionInfo.LOGIN_INFO.TurnOffCheckSpelling = !spellingToggleSwitch.IsOn;
		Close();
	}

	private void CancelButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void ThemeToggleSwitch_Toggled(object sender, EventArgs e)
	{
		SetThemeLabel();
	}

	private void SetThemeLabel()
	{
		if (themeToggleSwitch.IsOn)
		{
			themeLabelControl.Text = "light";
		}
		else
		{
			themeLabelControl.Text = "dark";
		}
	}

	private void SpellingToggleSwitch_Toggled(object sender, EventArgs e)
	{
		SetSpellingLabel();
	}

	private void SetSpellingLabel()
	{
		if (spellingToggleSwitch.IsOn)
		{
			spellingLabelControl.Text = "on";
		}
		else
		{
			spellingLabelControl.Text = "off";
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
		this.nonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.onboardingCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.onboardingLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.timeoutLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.themeLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.timeoutCommentLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.onboardingCommetLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.themeCommentLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.themeToggleSwitch = new DevExpress.XtraEditors.ToggleSwitch();
		this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
		this.saveButton = new DevExpress.XtraEditors.SimpleButton();
		this.timeoutSpinEdit = new DevExpress.XtraEditors.SpinEdit();
		this.repoTimeoutSpinEdit = new DevExpress.XtraEditors.SpinEdit();
		this.repoTimeoutLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.repoTimeoutCommentLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.spellingToggleSwitch = new DevExpress.XtraEditors.ToggleSwitch();
		this.spellingLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.themeCommentLabelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.saveBtnLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cancelBtnLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.buttonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.configLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.themeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.timeoutLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.themeCommentLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.timeoutCommentLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.bottomEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.themeLabelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.timeoutLabelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.onboardingLabelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.onboardingCommentLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.onboardingLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.repoTimeoutLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.repoTimeoutLabelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.repoTimeoutCommentLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.spellingLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.spellingLabelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.themeCommentLayoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.onboardingCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.themeToggleSwitch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.timeoutSpinEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repoTimeoutSpinEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spellingToggleSwitch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveBtnLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cancelBtnLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.configLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.themeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.timeoutLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.themeCommentLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.timeoutCommentLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.themeLabelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.timeoutLabelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.onboardingLabelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.onboardingCommentLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.onboardingLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repoTimeoutLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repoTimeoutLabelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repoTimeoutCommentLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spellingLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spellingLabelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.themeCommentLayoutControlItem1).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.onboardingCheckEdit);
		this.nonCustomizableLayoutControl.Controls.Add(this.onboardingLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.timeoutLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.themeLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.timeoutCommentLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.onboardingCommetLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.themeCommentLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.themeToggleSwitch);
		this.nonCustomizableLayoutControl.Controls.Add(this.cancelButton);
		this.nonCustomizableLayoutControl.Controls.Add(this.saveButton);
		this.nonCustomizableLayoutControl.Controls.Add(this.timeoutSpinEdit);
		this.nonCustomizableLayoutControl.Controls.Add(this.repoTimeoutSpinEdit);
		this.nonCustomizableLayoutControl.Controls.Add(this.repoTimeoutLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.repoTimeoutCommentLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.spellingToggleSwitch);
		this.nonCustomizableLayoutControl.Controls.Add(this.spellingLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.themeCommentLabelControl1);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(873, 0, 650, 400);
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(537, 367);
		this.nonCustomizableLayoutControl.TabIndex = 0;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.onboardingCheckEdit.Location = new System.Drawing.Point(96, 136);
		this.onboardingCheckEdit.Name = "onboardingCheckEdit";
		this.onboardingCheckEdit.Properties.Caption = "";
		this.onboardingCheckEdit.Properties.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Radio;
		this.onboardingCheckEdit.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.onboardingCheckEdit.Size = new System.Drawing.Size(124, 20);
		this.onboardingCheckEdit.StyleController = this.nonCustomizableLayoutControl;
		this.onboardingCheckEdit.TabIndex = 39;
		this.onboardingLabelControl.Location = new System.Drawing.Point(232, 140);
		this.onboardingLabelControl.Name = "onboardingLabelControl";
		this.onboardingLabelControl.Size = new System.Drawing.Size(29, 13);
		this.onboardingLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.onboardingLabelControl.TabIndex = 38;
		this.onboardingLabelControl.Text = "restart";
		this.timeoutLabelControl.Location = new System.Drawing.Point(232, 197);
		this.timeoutLabelControl.Name = "timeoutLabelControl";
		this.timeoutLabelControl.Size = new System.Drawing.Size(40, 13);
		this.timeoutLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.timeoutLabelControl.TabIndex = 37;
		this.timeoutLabelControl.Text = "seconds";
		this.themeLabelControl.Location = new System.Drawing.Point(232, 27);
		this.themeLabelControl.Name = "themeLabelControl";
		this.themeLabelControl.Size = new System.Drawing.Size(19, 13);
		this.themeLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.themeLabelControl.TabIndex = 36;
		this.themeLabelControl.Text = "light";
		this.timeoutCommentLabelControl.Appearance.ForeColor = System.Drawing.Color.Gray;
		this.timeoutCommentLabelControl.Appearance.Options.UseForeColor = true;
		this.timeoutCommentLabelControl.Location = new System.Drawing.Point(24, 218);
		this.timeoutCommentLabelControl.Name = "timeoutCommentLabelControl";
		this.timeoutCommentLabelControl.Size = new System.Drawing.Size(167, 13);
		this.timeoutCommentLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.timeoutCommentLabelControl.TabIndex = 35;
		this.timeoutCommentLabelControl.Text = "Configure the data sources timeout.";
		this.onboardingCommetLabelControl.Appearance.ForeColor = System.Drawing.Color.Gray;
		this.onboardingCommetLabelControl.Appearance.Options.UseForeColor = true;
		this.onboardingCommetLabelControl.Location = new System.Drawing.Point(24, 161);
		this.onboardingCommetLabelControl.Name = "onboardingCommetLabelControl";
		this.onboardingCommetLabelControl.Size = new System.Drawing.Size(262, 13);
		this.onboardingCommetLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.onboardingCommetLabelControl.TabIndex = 34;
		this.onboardingCommetLabelControl.Text = "Start all onboarding tooltips and tutorials from beginning.";
		this.themeCommentLabelControl.Appearance.ForeColor = System.Drawing.Color.Gray;
		this.themeCommentLabelControl.Appearance.Options.UseForeColor = true;
		this.themeCommentLabelControl.Location = new System.Drawing.Point(24, 47);
		this.themeCommentLabelControl.Name = "themeCommentLabelControl";
		this.themeCommentLabelControl.Size = new System.Drawing.Size(260, 13);
		this.themeCommentLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.themeCommentLabelControl.TabIndex = 33;
		this.themeCommentLabelControl.Text = "Change between light and dark mode. Requires restart.";
		this.themeToggleSwitch.EditValue = true;
		this.themeToggleSwitch.Location = new System.Drawing.Point(96, 24);
		this.themeToggleSwitch.Name = "themeToggleSwitch";
		this.themeToggleSwitch.Properties.AllowFocused = false;
		this.themeToggleSwitch.Properties.Appearance.Options.UseTextOptions = true;
		this.themeToggleSwitch.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.themeToggleSwitch.Properties.OffText = "";
		this.themeToggleSwitch.Properties.OnText = "";
		this.themeToggleSwitch.Properties.ShowText = false;
		this.themeToggleSwitch.Size = new System.Drawing.Size(124, 19);
		this.themeToggleSwitch.StyleController = this.nonCustomizableLayoutControl;
		this.themeToggleSwitch.TabIndex = 28;
		this.themeToggleSwitch.Toggled += new System.EventHandler(ThemeToggleSwitch_Toggled);
		this.cancelButton.Location = new System.Drawing.Point(426, 333);
		this.cancelButton.Name = "cancelButton";
		this.cancelButton.Size = new System.Drawing.Size(99, 22);
		this.cancelButton.StyleController = this.nonCustomizableLayoutControl;
		this.cancelButton.TabIndex = 5;
		this.cancelButton.Text = "Cancel";
		this.cancelButton.Click += new System.EventHandler(CancelButton_Click);
		this.saveButton.Location = new System.Drawing.Point(328, 333);
		this.saveButton.Name = "saveButton";
		this.saveButton.Size = new System.Drawing.Size(94, 22);
		this.saveButton.StyleController = this.nonCustomizableLayoutControl;
		this.saveButton.TabIndex = 4;
		this.saveButton.Text = "Save";
		this.saveButton.Click += new System.EventHandler(SaveButton_Click);
		this.timeoutSpinEdit.EditValue = new decimal(new int[4] { 1, 0, 0, 0 });
		this.timeoutSpinEdit.Location = new System.Drawing.Point(169, 194);
		this.timeoutSpinEdit.MaximumSize = new System.Drawing.Size(54, 0);
		this.timeoutSpinEdit.Name = "timeoutSpinEdit";
		this.timeoutSpinEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.timeoutSpinEdit.Properties.Increment = new decimal(new int[4] { 10, 0, 0, 0 });
		this.timeoutSpinEdit.Properties.IsFloatValue = false;
		this.timeoutSpinEdit.Properties.Mask.EditMask = "N00";
		this.timeoutSpinEdit.Properties.MaxValue = new decimal(new int[4] { 600, 0, 0, 0 });
		this.timeoutSpinEdit.Properties.MinValue = new decimal(new int[4] { 1, 0, 0, 0 });
		this.timeoutSpinEdit.Size = new System.Drawing.Size(51, 20);
		this.timeoutSpinEdit.StyleController = this.nonCustomizableLayoutControl;
		this.timeoutSpinEdit.TabIndex = 27;
		this.repoTimeoutSpinEdit.EditValue = new decimal(new int[4] { 1, 0, 0, 0 });
		this.repoTimeoutSpinEdit.Location = new System.Drawing.Point(169, 251);
		this.repoTimeoutSpinEdit.MaximumSize = new System.Drawing.Size(54, 0);
		this.repoTimeoutSpinEdit.Name = "repoTimeoutSpinEdit";
		this.repoTimeoutSpinEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repoTimeoutSpinEdit.Properties.Increment = new decimal(new int[4] { 10, 0, 0, 0 });
		this.repoTimeoutSpinEdit.Properties.IsFloatValue = false;
		this.repoTimeoutSpinEdit.Properties.Mask.EditMask = "N00";
		this.repoTimeoutSpinEdit.Properties.MaxValue = new decimal(new int[4] { 600, 0, 0, 0 });
		this.repoTimeoutSpinEdit.Properties.MinValue = new decimal(new int[4] { 1, 0, 0, 0 });
		this.repoTimeoutSpinEdit.Size = new System.Drawing.Size(51, 20);
		this.repoTimeoutSpinEdit.StyleController = this.nonCustomizableLayoutControl;
		this.repoTimeoutSpinEdit.TabIndex = 27;
		this.repoTimeoutLabelControl.Location = new System.Drawing.Point(232, 254);
		this.repoTimeoutLabelControl.Name = "repoTimeoutLabelControl";
		this.repoTimeoutLabelControl.Size = new System.Drawing.Size(40, 13);
		this.repoTimeoutLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.repoTimeoutLabelControl.TabIndex = 37;
		this.repoTimeoutLabelControl.Text = "seconds";
		this.repoTimeoutCommentLabelControl.Appearance.ForeColor = System.Drawing.Color.Gray;
		this.repoTimeoutCommentLabelControl.Appearance.Options.UseForeColor = true;
		this.repoTimeoutCommentLabelControl.Location = new System.Drawing.Point(24, 275);
		this.repoTimeoutCommentLabelControl.Name = "repoTimeoutCommentLabelControl";
		this.repoTimeoutCommentLabelControl.Size = new System.Drawing.Size(332, 13);
		this.repoTimeoutCommentLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.repoTimeoutCommentLabelControl.TabIndex = 35;
		this.repoTimeoutCommentLabelControl.Text = "Configure the repository timeout (metadata search, SCT, classification).";
		this.spellingToggleSwitch.EditValue = true;
		this.spellingToggleSwitch.Location = new System.Drawing.Point(96, 80);
		this.spellingToggleSwitch.Name = "spellingToggleSwitch";
		this.spellingToggleSwitch.Properties.AllowFocused = false;
		this.spellingToggleSwitch.Properties.Appearance.Options.UseTextOptions = true;
		this.spellingToggleSwitch.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.spellingToggleSwitch.Properties.OffText = "";
		this.spellingToggleSwitch.Properties.OnText = "";
		this.spellingToggleSwitch.Properties.ShowText = false;
		this.spellingToggleSwitch.Size = new System.Drawing.Size(124, 19);
		this.spellingToggleSwitch.StyleController = this.nonCustomizableLayoutControl;
		this.spellingToggleSwitch.TabIndex = 28;
		this.spellingToggleSwitch.Toggled += new System.EventHandler(SpellingToggleSwitch_Toggled);
		this.spellingLabelControl.Location = new System.Drawing.Point(232, 83);
		this.spellingLabelControl.Name = "spellingLabelControl";
		this.spellingLabelControl.Size = new System.Drawing.Size(12, 13);
		this.spellingLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.spellingLabelControl.TabIndex = 36;
		this.spellingLabelControl.Text = "on";
		this.themeCommentLabelControl1.Appearance.ForeColor = System.Drawing.Color.Gray;
		this.themeCommentLabelControl1.Appearance.Options.UseForeColor = true;
		this.themeCommentLabelControl1.Location = new System.Drawing.Point(24, 103);
		this.themeCommentLabelControl1.Name = "themeCommentLabelControl1";
		this.themeCommentLabelControl1.Size = new System.Drawing.Size(301, 13);
		this.themeCommentLabelControl1.StyleController = this.nonCustomizableLayoutControl;
		this.themeCommentLabelControl1.TabIndex = 33;
		this.themeCommentLabelControl1.Text = "Check spelling and grammar. Underline errors and suggest fixes.";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.saveBtnLayoutControlItem, this.cancelBtnLayoutControlItem, this.buttonsEmptySpaceItem, this.configLayoutControlGroup });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(537, 367);
		this.Root.TextVisible = false;
		this.saveBtnLayoutControlItem.Control = this.saveButton;
		this.saveBtnLayoutControlItem.Location = new System.Drawing.Point(316, 321);
		this.saveBtnLayoutControlItem.MaxSize = new System.Drawing.Size(98, 26);
		this.saveBtnLayoutControlItem.MinSize = new System.Drawing.Size(98, 26);
		this.saveBtnLayoutControlItem.Name = "saveBtnLayoutControlItem";
		this.saveBtnLayoutControlItem.Size = new System.Drawing.Size(98, 26);
		this.saveBtnLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.saveBtnLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveBtnLayoutControlItem.TextVisible = false;
		this.cancelBtnLayoutControlItem.Control = this.cancelButton;
		this.cancelBtnLayoutControlItem.Location = new System.Drawing.Point(414, 321);
		this.cancelBtnLayoutControlItem.MaxSize = new System.Drawing.Size(103, 26);
		this.cancelBtnLayoutControlItem.MinSize = new System.Drawing.Size(103, 26);
		this.cancelBtnLayoutControlItem.Name = "cancelBtnLayoutControlItem";
		this.cancelBtnLayoutControlItem.Size = new System.Drawing.Size(103, 26);
		this.cancelBtnLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cancelBtnLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.cancelBtnLayoutControlItem.TextVisible = false;
		this.buttonsEmptySpaceItem.AllowHotTrack = false;
		this.buttonsEmptySpaceItem.Location = new System.Drawing.Point(0, 321);
		this.buttonsEmptySpaceItem.Name = "buttonsEmptySpaceItem";
		this.buttonsEmptySpaceItem.Size = new System.Drawing.Size(316, 26);
		this.buttonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.configLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[16]
		{
			this.themeLayoutControlItem, this.timeoutLayoutControlItem, this.themeCommentLayoutControlItem, this.timeoutCommentLayoutControlItem, this.bottomEmptySpaceItem, this.themeLabelLayoutControlItem, this.timeoutLabelLayoutControlItem, this.onboardingLabelLayoutControlItem, this.onboardingCommentLayoutControlItem, this.onboardingLayoutControlItem,
			this.repoTimeoutLayoutControlItem, this.repoTimeoutLabelLayoutControlItem, this.repoTimeoutCommentLayoutControlItem, this.spellingLayoutControlItem, this.spellingLabelLayoutControlItem, this.themeCommentLayoutControlItem1
		});
		this.configLayoutControlGroup.Location = new System.Drawing.Point(0, 0);
		this.configLayoutControlGroup.Name = "configLayoutControlGroup";
		this.configLayoutControlGroup.Size = new System.Drawing.Size(517, 321);
		this.configLayoutControlGroup.TextVisible = false;
		this.themeLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.themeLayoutControlItem.Control = this.themeToggleSwitch;
		this.themeLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.themeLayoutControlItem.MaxSize = new System.Drawing.Size(200, 23);
		this.themeLayoutControlItem.MinSize = new System.Drawing.Size(200, 23);
		this.themeLayoutControlItem.Name = "themeLayoutControlItem";
		this.themeLayoutControlItem.Size = new System.Drawing.Size(200, 23);
		this.themeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.themeLayoutControlItem.Text = "Theme";
		this.themeLayoutControlItem.TextSize = new System.Drawing.Size(69, 13);
		this.timeoutLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.timeoutLayoutControlItem.Control = this.timeoutSpinEdit;
		this.timeoutLayoutControlItem.CustomizationFormText = "Connection timeout";
		this.timeoutLayoutControlItem.Location = new System.Drawing.Point(0, 170);
		this.timeoutLayoutControlItem.MaxSize = new System.Drawing.Size(200, 24);
		this.timeoutLayoutControlItem.MinSize = new System.Drawing.Size(200, 24);
		this.timeoutLayoutControlItem.Name = "timeoutLayoutControlItem";
		this.timeoutLayoutControlItem.Size = new System.Drawing.Size(200, 24);
		this.timeoutLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.timeoutLayoutControlItem.Text = "Connection timeout";
		this.timeoutLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.timeoutLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.timeoutLayoutControlItem.TextToControlDistance = 45;
		this.themeCommentLayoutControlItem.Control = this.themeCommentLabelControl;
		this.themeCommentLayoutControlItem.Location = new System.Drawing.Point(0, 23);
		this.themeCommentLayoutControlItem.Name = "themeCommentLayoutControlItem";
		this.themeCommentLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 18);
		this.themeCommentLayoutControlItem.Size = new System.Drawing.Size(493, 33);
		this.themeCommentLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.themeCommentLayoutControlItem.TextVisible = false;
		this.timeoutCommentLayoutControlItem.Control = this.timeoutCommentLabelControl;
		this.timeoutCommentLayoutControlItem.Location = new System.Drawing.Point(0, 194);
		this.timeoutCommentLayoutControlItem.Name = "timeoutCommentLayoutControlItem";
		this.timeoutCommentLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 18);
		this.timeoutCommentLayoutControlItem.Size = new System.Drawing.Size(493, 33);
		this.timeoutCommentLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.timeoutCommentLayoutControlItem.TextVisible = false;
		this.bottomEmptySpaceItem.AllowHotTrack = false;
		this.bottomEmptySpaceItem.Location = new System.Drawing.Point(0, 284);
		this.bottomEmptySpaceItem.Name = "bottomEmptySpaceItem";
		this.bottomEmptySpaceItem.Size = new System.Drawing.Size(493, 13);
		this.bottomEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.themeLabelLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.themeLabelLayoutControlItem.Control = this.themeLabelControl;
		this.themeLabelLayoutControlItem.Location = new System.Drawing.Point(200, 0);
		this.themeLabelLayoutControlItem.Name = "themeLabelLayoutControlItem";
		this.themeLabelLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 2, 2, 2);
		this.themeLabelLayoutControlItem.Size = new System.Drawing.Size(293, 23);
		this.themeLabelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.themeLabelLayoutControlItem.TextVisible = false;
		this.timeoutLabelLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.timeoutLabelLayoutControlItem.Control = this.timeoutLabelControl;
		this.timeoutLabelLayoutControlItem.Location = new System.Drawing.Point(200, 170);
		this.timeoutLabelLayoutControlItem.Name = "timeoutLabelLayoutControlItem";
		this.timeoutLabelLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 2, 2, 2);
		this.timeoutLabelLayoutControlItem.Size = new System.Drawing.Size(293, 24);
		this.timeoutLabelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.timeoutLabelLayoutControlItem.TextVisible = false;
		this.onboardingLabelLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.onboardingLabelLayoutControlItem.Control = this.onboardingLabelControl;
		this.onboardingLabelLayoutControlItem.Location = new System.Drawing.Point(200, 112);
		this.onboardingLabelLayoutControlItem.Name = "onboardingLabelLayoutControlItem";
		this.onboardingLabelLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 2, 2, 2);
		this.onboardingLabelLayoutControlItem.Size = new System.Drawing.Size(293, 25);
		this.onboardingLabelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.onboardingLabelLayoutControlItem.TextVisible = false;
		this.onboardingCommentLayoutControlItem.Control = this.onboardingCommetLabelControl;
		this.onboardingCommentLayoutControlItem.Location = new System.Drawing.Point(0, 137);
		this.onboardingCommentLayoutControlItem.Name = "onboardingCommentLayoutControlItem";
		this.onboardingCommentLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 18);
		this.onboardingCommentLayoutControlItem.Size = new System.Drawing.Size(493, 33);
		this.onboardingCommentLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.onboardingCommentLayoutControlItem.TextVisible = false;
		this.onboardingLayoutControlItem.Control = this.onboardingCheckEdit;
		this.onboardingLayoutControlItem.Location = new System.Drawing.Point(0, 112);
		this.onboardingLayoutControlItem.MaxSize = new System.Drawing.Size(200, 0);
		this.onboardingLayoutControlItem.MinSize = new System.Drawing.Size(200, 25);
		this.onboardingLayoutControlItem.Name = "onboardingLayoutControlItem";
		this.onboardingLayoutControlItem.Size = new System.Drawing.Size(200, 25);
		this.onboardingLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.onboardingLayoutControlItem.Text = "Onboarding";
		this.onboardingLayoutControlItem.TextSize = new System.Drawing.Size(69, 13);
		this.repoTimeoutLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.repoTimeoutLayoutControlItem.Control = this.repoTimeoutSpinEdit;
		this.repoTimeoutLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.repoTimeoutLayoutControlItem.CustomizationFormText = "Connection timeout";
		this.repoTimeoutLayoutControlItem.Location = new System.Drawing.Point(0, 227);
		this.repoTimeoutLayoutControlItem.MaxSize = new System.Drawing.Size(200, 24);
		this.repoTimeoutLayoutControlItem.MinSize = new System.Drawing.Size(200, 24);
		this.repoTimeoutLayoutControlItem.Name = "repoTimeoutLayoutControlItem";
		this.repoTimeoutLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.repoTimeoutLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.repoTimeoutLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.repoTimeoutLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.repoTimeoutLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.repoTimeoutLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.repoTimeoutLayoutControlItem.Size = new System.Drawing.Size(200, 24);
		this.repoTimeoutLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.repoTimeoutLayoutControlItem.Text = "Repository timeout";
		this.repoTimeoutLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.repoTimeoutLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.repoTimeoutLayoutControlItem.TextToControlDistance = 45;
		this.repoTimeoutLabelLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.repoTimeoutLabelLayoutControlItem.Control = this.repoTimeoutLabelControl;
		this.repoTimeoutLabelLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.repoTimeoutLabelLayoutControlItem.CustomizationFormText = "timeoutLabelLayoutControlItem";
		this.repoTimeoutLabelLayoutControlItem.Location = new System.Drawing.Point(200, 227);
		this.repoTimeoutLabelLayoutControlItem.Name = "repoTimeoutLabelLayoutControlItem";
		this.repoTimeoutLabelLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.repoTimeoutLabelLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.repoTimeoutLabelLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.repoTimeoutLabelLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.repoTimeoutLabelLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.repoTimeoutLabelLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.repoTimeoutLabelLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 2, 2, 2);
		this.repoTimeoutLabelLayoutControlItem.Size = new System.Drawing.Size(293, 24);
		this.repoTimeoutLabelLayoutControlItem.Text = "timeoutLabelLayoutControlItem";
		this.repoTimeoutLabelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.repoTimeoutLabelLayoutControlItem.TextVisible = false;
		this.repoTimeoutCommentLayoutControlItem.Control = this.repoTimeoutCommentLabelControl;
		this.repoTimeoutCommentLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.repoTimeoutCommentLayoutControlItem.CustomizationFormText = "timeoutCommentLayoutControlItem";
		this.repoTimeoutCommentLayoutControlItem.Location = new System.Drawing.Point(0, 251);
		this.repoTimeoutCommentLayoutControlItem.Name = "repoTimeoutCommentLayoutControlItem";
		this.repoTimeoutCommentLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.repoTimeoutCommentLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.repoTimeoutCommentLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.repoTimeoutCommentLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.repoTimeoutCommentLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.repoTimeoutCommentLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.repoTimeoutCommentLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 18);
		this.repoTimeoutCommentLayoutControlItem.Size = new System.Drawing.Size(493, 33);
		this.repoTimeoutCommentLayoutControlItem.Text = "timeoutCommentLayoutControlItem";
		this.repoTimeoutCommentLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.repoTimeoutCommentLayoutControlItem.TextVisible = false;
		this.spellingLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.spellingLayoutControlItem.Control = this.spellingToggleSwitch;
		this.spellingLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.spellingLayoutControlItem.CustomizationFormText = "Check spelling";
		this.spellingLayoutControlItem.Location = new System.Drawing.Point(0, 56);
		this.spellingLayoutControlItem.MaxSize = new System.Drawing.Size(200, 23);
		this.spellingLayoutControlItem.MinSize = new System.Drawing.Size(200, 23);
		this.spellingLayoutControlItem.Name = "spellingLayoutControlItem";
		this.spellingLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.spellingLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.spellingLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.spellingLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.spellingLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.spellingLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.spellingLayoutControlItem.Size = new System.Drawing.Size(200, 23);
		this.spellingLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.spellingLayoutControlItem.Text = "Check spelling";
		this.spellingLayoutControlItem.TextSize = new System.Drawing.Size(69, 13);
		this.spellingLabelLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.spellingLabelLayoutControlItem.Control = this.spellingLabelControl;
		this.spellingLabelLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.spellingLabelLayoutControlItem.CustomizationFormText = "themeLabelLayoutControlItem";
		this.spellingLabelLayoutControlItem.Location = new System.Drawing.Point(200, 56);
		this.spellingLabelLayoutControlItem.Name = "spellingLabelLayoutControlItem";
		this.spellingLabelLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.spellingLabelLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.spellingLabelLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.spellingLabelLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.spellingLabelLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.spellingLabelLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.spellingLabelLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 2, 2, 2);
		this.spellingLabelLayoutControlItem.Size = new System.Drawing.Size(293, 23);
		this.spellingLabelLayoutControlItem.Text = "themeLabelLayoutControlItem";
		this.spellingLabelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.spellingLabelLayoutControlItem.TextVisible = false;
		this.themeCommentLayoutControlItem1.Control = this.themeCommentLabelControl1;
		this.themeCommentLayoutControlItem1.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.themeCommentLayoutControlItem1.CustomizationFormText = "themeCommentLayoutControlItem";
		this.themeCommentLayoutControlItem1.Location = new System.Drawing.Point(0, 79);
		this.themeCommentLayoutControlItem1.Name = "themeCommentLayoutControlItem1";
		this.themeCommentLayoutControlItem1.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.themeCommentLayoutControlItem1.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.themeCommentLayoutControlItem1.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.themeCommentLayoutControlItem1.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.themeCommentLayoutControlItem1.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.themeCommentLayoutControlItem1.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.themeCommentLayoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 18);
		this.themeCommentLayoutControlItem1.Size = new System.Drawing.Size(493, 33);
		this.themeCommentLayoutControlItem1.Text = "themeCommentLayoutControlItem";
		this.themeCommentLayoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.themeCommentLayoutControlItem1.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(537, 367);
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.MaximizeBox = false;
		base.Name = "SettingsForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Configuration";
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.onboardingCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.themeToggleSwitch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.timeoutSpinEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repoTimeoutSpinEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spellingToggleSwitch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveBtnLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cancelBtnLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.configLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.themeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.timeoutLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.themeCommentLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.timeoutCommentLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.themeLabelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.timeoutLabelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.onboardingLabelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.onboardingCommentLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.onboardingLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repoTimeoutLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repoTimeoutLabelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repoTimeoutCommentLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spellingLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spellingLabelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.themeCommentLayoutControlItem1).EndInit();
		base.ResumeLayout(false);
	}
}
