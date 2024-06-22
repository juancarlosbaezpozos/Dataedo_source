using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools.UI;
using Dataedo.App.Tools.UI.Skins;
using Dataedo.App.Tools.UI.Skins.Base;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Forms;

public class StartupSettingsForm : BaseXtraForm
{
	private bool isLightSelected = true;

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl2;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl1;

	private PictureEdit darkThemePictureEdit;

	private PictureEdit lightThemePictureEdit;

	private LayoutControlGroup Root;

	private LayoutControlItem lightThemeImageLayoutControlItem;

	private SimpleButton okSimpleButton;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem1;

	private LayoutControlItem darkThemeImageLayoutControlItem;

	private EmptySpaceItem lightThemeEmptySpaceItem;

	private EmptySpaceItem darkThemeEmptySpaceItem;

	private EmptySpaceItem emptySpaceItem2;

	public StartupSettingsForm()
	{
		InitializeComponent();
	}

	public static void SetTheme(bool isLightTheme)
	{
		if (isLightTheme)
		{
			SkinsManager.SaveSkinSettings(OfficeWhite.SkinNameValue, OfficeWhite.PaletteValue, showMessage: false);
		}
		else
		{
			SkinsManager.SaveSkinSettings(VsDark.SkinNameValue, VsDark.PaletteValue, showMessage: false);
		}
		SkinsManager.SetSkin();
	}

	private void StartupSettingsForm_Load(object sender, EventArgs e)
	{
		BaseSkin currentSkin = SkinsManager.CurrentSkin;
		isLightSelected = !(currentSkin.SkinName == VsDark.SkinNameValue) || !(currentSkin.Palette == VsDark.PaletteValue);
		BackColor = SkinsManager.CurrentSkin.ControlBackColor;
		lightThemeEmptySpaceItem.AppearanceItemCaption.ForeColor = SkinsManager.CurrentSkin.ControlForeColor;
		darkThemeEmptySpaceItem.AppearanceItemCaption.ForeColor = SkinsManager.CurrentSkin.ControlForeColor;
	}

	private void okSimpleButton_Click(object sender, EventArgs e)
	{
		SaveSkinSettings();
	}

	private void SaveSkinSettings()
	{
		if (isLightSelected)
		{
			SkinsManager.SaveSkinSettings(OfficeWhite.SkinNameValue, OfficeWhite.PaletteValue, showMessage: false);
		}
		else
		{
			SkinsManager.SaveSkinSettings(VsDark.SkinNameValue, VsDark.PaletteValue, showMessage: false);
		}
		SkinsManager.SetSkin();
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void darkThemeImageLayoutControlItem_CustomDraw(object sender, ItemCustomDrawEventArgs e)
	{
		if (!isLightSelected)
		{
			DrawSelection(e);
		}
	}

	private void lightThemeImageLayoutControlItem_CustomDraw(object sender, ItemCustomDrawEventArgs e)
	{
		if (isLightSelected)
		{
			DrawSelection(e);
		}
	}

	private void DrawSelection(ItemCustomDrawEventArgs e)
	{
		e.Cache.Graphics.DrawRectangle(new Pen(SkinsManager.CurrentSkin.DataedoColor, 5f), new Rectangle(e.Bounds.Location.X + 10, e.Bounds.Location.Y + 10, e.Bounds.Width - 20, e.Bounds.Height - 20));
	}

	private void darkThemePictureEdit_Click(object sender, EventArgs e)
	{
		isLightSelected = false;
		lightThemeImageLayoutControlItem.Invalidate();
		darkThemeImageLayoutControlItem.Invalidate();
	}

	private void lightThemePictureEdit_Click(object sender, EventArgs e)
	{
		isLightSelected = true;
		lightThemeImageLayoutControlItem.Invalidate();
		darkThemeImageLayoutControlItem.Invalidate();
	}

	private void lightThemePictureEdit_DoubleClick(object sender, EventArgs e)
	{
		SaveSkinSettings();
	}

	private void darkThemePictureEdit_DoubleClick(object sender, EventArgs e)
	{
		SaveSkinSettings();
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
		DevExpress.XtraLayout.RowDefinition rowDefinition = new DevExpress.XtraLayout.RowDefinition();
		DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
		DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
		DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.StartupSettingsForm));
		this.nonCustomizableLayoutControl2 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.nonCustomizableLayoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.darkThemePictureEdit = new DevExpress.XtraEditors.PictureEdit();
		this.lightThemePictureEdit = new DevExpress.XtraEditors.PictureEdit();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.lightThemeEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.darkThemeEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.lightThemeImageLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.darkThemeImageLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.okSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl2).BeginInit();
		this.nonCustomizableLayoutControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).BeginInit();
		this.nonCustomizableLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.darkThemePictureEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lightThemePictureEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lightThemeEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.darkThemeEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lightThemeImageLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.darkThemeImageLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl2.AllowCustomization = false;
		this.nonCustomizableLayoutControl2.Controls.Add(this.nonCustomizableLayoutControl1);
		this.nonCustomizableLayoutControl2.Controls.Add(this.okSimpleButton);
		this.nonCustomizableLayoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl2.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl2.Name = "nonCustomizableLayoutControl2";
		this.nonCustomizableLayoutControl2.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(-1204, 227, 650, 649);
		this.nonCustomizableLayoutControl2.Root = this.layoutControlGroup1;
		this.nonCustomizableLayoutControl2.Size = new System.Drawing.Size(578, 296);
		this.nonCustomizableLayoutControl2.TabIndex = 18;
		this.nonCustomizableLayoutControl2.Text = "nonCustomizableLayoutControl2";
		this.nonCustomizableLayoutControl1.AllowCustomization = false;
		this.nonCustomizableLayoutControl1.Controls.Add(this.darkThemePictureEdit);
		this.nonCustomizableLayoutControl1.Controls.Add(this.lightThemePictureEdit);
		this.nonCustomizableLayoutControl1.Location = new System.Drawing.Point(12, 12);
		this.nonCustomizableLayoutControl1.Margin = new System.Windows.Forms.Padding(0);
		this.nonCustomizableLayoutControl1.Name = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2368, 232, 948, 680);
		this.nonCustomizableLayoutControl1.Root = this.Root;
		this.nonCustomizableLayoutControl1.Size = new System.Drawing.Size(554, 246);
		this.nonCustomizableLayoutControl1.TabIndex = 18;
		this.nonCustomizableLayoutControl1.Text = "nonCustomizableLayoutControl1";
		this.darkThemePictureEdit.EditValue = Dataedo.App.Properties.Resources.dark_theme_thumbnail;
		this.darkThemePictureEdit.Location = new System.Drawing.Point(292, 52);
		this.darkThemePictureEdit.Name = "darkThemePictureEdit";
		this.darkThemePictureEdit.Properties.AllowFocused = false;
		this.darkThemePictureEdit.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.darkThemePictureEdit.Properties.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.darkThemePictureEdit.Properties.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.darkThemePictureEdit.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(37, 37, 38);
		this.darkThemePictureEdit.Properties.Appearance.Options.UseBackColor = true;
		this.darkThemePictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.darkThemePictureEdit.Properties.PictureAlignment = System.Drawing.ContentAlignment.TopCenter;
		this.darkThemePictureEdit.Properties.ReadOnly = true;
		this.darkThemePictureEdit.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
		this.darkThemePictureEdit.Properties.ShowMenu = false;
		this.darkThemePictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
		this.darkThemePictureEdit.Size = new System.Drawing.Size(237, 168);
		this.darkThemePictureEdit.StyleController = this.nonCustomizableLayoutControl1;
		this.darkThemePictureEdit.TabIndex = 21;
		this.darkThemePictureEdit.Click += new System.EventHandler(darkThemePictureEdit_Click);
		this.darkThemePictureEdit.DoubleClick += new System.EventHandler(darkThemePictureEdit_DoubleClick);
		this.lightThemePictureEdit.EditValue = Dataedo.App.Properties.Resources.light_theme_thumbnail;
		this.lightThemePictureEdit.Location = new System.Drawing.Point(25, 52);
		this.lightThemePictureEdit.Name = "lightThemePictureEdit";
		this.lightThemePictureEdit.Properties.AllowFocused = false;
		this.lightThemePictureEdit.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.lightThemePictureEdit.Properties.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.lightThemePictureEdit.Properties.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.lightThemePictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lightThemePictureEdit.Properties.PictureAlignment = System.Drawing.ContentAlignment.TopCenter;
		this.lightThemePictureEdit.Properties.ReadOnly = true;
		this.lightThemePictureEdit.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
		this.lightThemePictureEdit.Properties.ShowMenu = false;
		this.lightThemePictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
		this.lightThemePictureEdit.Size = new System.Drawing.Size(237, 168);
		this.lightThemePictureEdit.StyleController = this.nonCustomizableLayoutControl1;
		this.lightThemePictureEdit.TabIndex = 18;
		this.lightThemePictureEdit.Click += new System.EventHandler(lightThemePictureEdit_Click);
		this.lightThemePictureEdit.DoubleClick += new System.EventHandler(lightThemePictureEdit_DoubleClick);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.lightThemeEmptySpaceItem, this.darkThemeEmptySpaceItem, this.lightThemeImageLayoutControlItem, this.darkThemeImageLayoutControlItem });
		this.Root.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
		this.Root.Name = "Root";
		columnDefinition.SizeType = System.Windows.Forms.SizeType.Percent;
		columnDefinition.Width = 50.0;
		columnDefinition2.SizeType = System.Windows.Forms.SizeType.Percent;
		columnDefinition2.Width = 50.0;
		this.Root.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[2] { columnDefinition, columnDefinition2 });
		rowDefinition.Height = 50.0;
		rowDefinition.SizeType = System.Windows.Forms.SizeType.Percent;
		rowDefinition2.Height = 26.0;
		rowDefinition2.SizeType = System.Windows.Forms.SizeType.Absolute;
		rowDefinition3.Height = 198.0;
		rowDefinition3.SizeType = System.Windows.Forms.SizeType.Absolute;
		rowDefinition4.Height = 50.0;
		rowDefinition4.SizeType = System.Windows.Forms.SizeType.Percent;
		this.Root.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[4] { rowDefinition, rowDefinition2, rowDefinition3, rowDefinition4 });
		this.Root.Size = new System.Drawing.Size(554, 246);
		this.Root.TextVisible = false;
		this.lightThemeEmptySpaceItem.AllowHotTrack = false;
		this.lightThemeEmptySpaceItem.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Bold);
		this.lightThemeEmptySpaceItem.AppearanceItemCaption.Options.UseFont = true;
		this.lightThemeEmptySpaceItem.AppearanceItemCaption.Options.UseTextOptions = true;
		this.lightThemeEmptySpaceItem.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.lightThemeEmptySpaceItem.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.lightThemeEmptySpaceItem.Location = new System.Drawing.Point(0, 1);
		this.lightThemeEmptySpaceItem.MaxSize = new System.Drawing.Size(267, 26);
		this.lightThemeEmptySpaceItem.MinSize = new System.Drawing.Size(267, 26);
		this.lightThemeEmptySpaceItem.Name = "lightThemeEmptySpaceItem";
		this.lightThemeEmptySpaceItem.OptionsTableLayoutItem.RowIndex = 1;
		this.lightThemeEmptySpaceItem.Size = new System.Drawing.Size(267, 26);
		this.lightThemeEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.lightThemeEmptySpaceItem.Text = "Light theme";
		this.lightThemeEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.lightThemeEmptySpaceItem.TextVisible = true;
		this.darkThemeEmptySpaceItem.AllowHotTrack = false;
		this.darkThemeEmptySpaceItem.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Bold);
		this.darkThemeEmptySpaceItem.AppearanceItemCaption.Options.UseFont = true;
		this.darkThemeEmptySpaceItem.AppearanceItemCaption.Options.UseTextOptions = true;
		this.darkThemeEmptySpaceItem.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.darkThemeEmptySpaceItem.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.darkThemeEmptySpaceItem.Location = new System.Drawing.Point(267, 1);
		this.darkThemeEmptySpaceItem.MaxSize = new System.Drawing.Size(267, 26);
		this.darkThemeEmptySpaceItem.MinSize = new System.Drawing.Size(267, 26);
		this.darkThemeEmptySpaceItem.Name = "darkThemeEmptySpaceItem";
		this.darkThemeEmptySpaceItem.OptionsTableLayoutItem.ColumnIndex = 1;
		this.darkThemeEmptySpaceItem.OptionsTableLayoutItem.RowIndex = 1;
		this.darkThemeEmptySpaceItem.Size = new System.Drawing.Size(267, 26);
		this.darkThemeEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.darkThemeEmptySpaceItem.Text = "Dark theme";
		this.darkThemeEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.darkThemeEmptySpaceItem.TextVisible = true;
		this.lightThemeImageLayoutControlItem.Control = this.lightThemePictureEdit;
		this.lightThemeImageLayoutControlItem.CustomizationFormText = "lightThemeLayoutControlItem";
		this.lightThemeImageLayoutControlItem.Location = new System.Drawing.Point(0, 27);
		this.lightThemeImageLayoutControlItem.MaxSize = new System.Drawing.Size(267, 198);
		this.lightThemeImageLayoutControlItem.MinSize = new System.Drawing.Size(267, 198);
		this.lightThemeImageLayoutControlItem.Name = "lightThemeImageLayoutControlItem";
		this.lightThemeImageLayoutControlItem.OptionsTableLayoutItem.RowIndex = 2;
		this.lightThemeImageLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(15, 15, 15, 15);
		this.lightThemeImageLayoutControlItem.Size = new System.Drawing.Size(267, 198);
		this.lightThemeImageLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.lightThemeImageLayoutControlItem.Text = "Light theme";
		this.lightThemeImageLayoutControlItem.TextLocation = DevExpress.Utils.Locations.Top;
		this.lightThemeImageLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.lightThemeImageLayoutControlItem.TextVisible = false;
		this.lightThemeImageLayoutControlItem.CustomDraw += new System.EventHandler<DevExpress.XtraLayout.ItemCustomDrawEventArgs>(lightThemeImageLayoutControlItem_CustomDraw);
		this.darkThemeImageLayoutControlItem.Control = this.darkThemePictureEdit;
		this.darkThemeImageLayoutControlItem.Location = new System.Drawing.Point(267, 27);
		this.darkThemeImageLayoutControlItem.MaxSize = new System.Drawing.Size(267, 198);
		this.darkThemeImageLayoutControlItem.MinSize = new System.Drawing.Size(267, 198);
		this.darkThemeImageLayoutControlItem.Name = "darkThemeImageLayoutControlItem";
		this.darkThemeImageLayoutControlItem.OptionsTableLayoutItem.ColumnIndex = 1;
		this.darkThemeImageLayoutControlItem.OptionsTableLayoutItem.RowIndex = 2;
		this.darkThemeImageLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(15, 15, 15, 15);
		this.darkThemeImageLayoutControlItem.Size = new System.Drawing.Size(267, 198);
		this.darkThemeImageLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.darkThemeImageLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.darkThemeImageLayoutControlItem.TextVisible = false;
		this.darkThemeImageLayoutControlItem.CustomDraw += new System.EventHandler<DevExpress.XtraLayout.ItemCustomDrawEventArgs>(darkThemeImageLayoutControlItem_CustomDraw);
		this.okSimpleButton.Location = new System.Drawing.Point(437, 262);
		this.okSimpleButton.Name = "okSimpleButton";
		this.okSimpleButton.Size = new System.Drawing.Size(108, 22);
		this.okSimpleButton.StyleController = this.nonCustomizableLayoutControl2;
		this.okSimpleButton.TabIndex = 4;
		this.okSimpleButton.Text = "OK";
		this.okSimpleButton.Click += new System.EventHandler(okSimpleButton_Click);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.layoutControlItem2, this.layoutControlItem3, this.emptySpaceItem1, this.emptySpaceItem2 });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Size = new System.Drawing.Size(578, 296);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem2.Control = this.okSimpleButton;
		this.layoutControlItem2.Location = new System.Drawing.Point(425, 250);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(0, 26);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(25, 26);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(112, 26);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlItem3.Control = this.nonCustomizableLayoutControl1;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(558, 250);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 250);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(425, 26);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(537, 250);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(21, 26);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(10, 21);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.emptySpaceItem2.Size = new System.Drawing.Size(21, 26);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(578, 296);
		base.Controls.Add(this.nonCustomizableLayoutControl2);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("StartupSettingsForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.MaximizeBox = false;
		base.Name = "StartupSettingsForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Dataedo";
		base.Load += new System.EventHandler(StartupSettingsForm_Load);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl2).EndInit();
		this.nonCustomizableLayoutControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).EndInit();
		this.nonCustomizableLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.darkThemePictureEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lightThemePictureEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lightThemeEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.darkThemeEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lightThemeImageLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.darkThemeImageLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		base.ResumeLayout(false);
	}
}
