using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.LoginFormTools.Tools;
using Dataedo.App.LoginFormTools.Tools.Common;
using Dataedo.App.LoginFormTools.Tools.CustomEventArgs;
using Dataedo.App.LoginFormTools.Tools.Enums;
using Dataedo.App.LoginFormTools.Tools.Licenses;
using Dataedo.App.LoginFormTools.UserControls.Base;
using Dataedo.App.LoginFormTools.UserControls.Common;
using Dataedo.App.LoginFormTools.UserControls.Subcontrols;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.LoginFormTools.UserControls;

public class FileLicensesPageUserControl : BasePageUserControl
{
	private readonly DoubleClickSupport tileViewDoubleClickSupport;

	private readonly RecentListUserControl recentListUserControl;

	private readonly NoLicensesUserControl noLicensesUserControl;

	private FileData licenseFileModel;

	private string filePath;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private EmptySpaceItem logoPictureEditTopSeparatorEmptySpaceItem;

	private EmptySpaceItem logoPictureEditRightSeparatorEmptySpaceItem;

	private LabelControl headerLabelControl;

	private LayoutControlItem headerLabelControlLayoutControlItem;

	private LabelControl description1LabelControl;

	private LayoutControlItem description1LabelControlLayoutControlItem;

	private SimpleButton backSimpleButton;

	private LayoutControlItem backSimpleButtonLayoutControlItem;

	private EmptySpaceItem buttonsEmptySpaceItem;

	private SimpleButton nextSimpleButton;

	private LayoutControlItem nextSimpleButtonLayoutControlItem;

	private SmallLogoUserControl smallLogoUserControl;

	private LayoutControlItem smallLogoUserControlLayoutControlItem;

	private ToolTipController toolTipController;

	private PanelControl panelControl;

	private LayoutControlItem panelControlLayoutControlItem;

	private EmptySpaceItem separatorEmptySpaceItem;

	public FileLicensesPageUserControl()
	{
		InitializeComponent();
		recentListUserControl = new RecentListUserControl();
		recentListUserControl.Dock = DockStyle.Fill;
		noLicensesUserControl = new NoLicensesUserControl();
		noLicensesUserControl.Dock = DockStyle.Fill;
		recentListUserControl.TileViewKeyDown += TileView_KeyDown;
		recentListUserControl.FocusedRowChanged += TileView_FocusedRowChanged;
		tileViewDoubleClickSupport = new DoubleClickSupport(recentListUserControl.TileView);
		tileViewDoubleClickSupport.DoubleClick += TileViewDoubleClickSupport_DoubleClick;
		panelControl.Controls.Clear();
		panelControl.Controls.Add(recentListUserControl);
	}

	private string PrepareLicenseText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Please choose the license from the ones available below:");
		return stringBuilder.ToString();
	}

	internal override void SetParameter(object parameter, bool isCalledAsPrevious)
	{
		base.SetParameter(parameter, isCalledAsPrevious);
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserOsDataedoBuilder(new TrackingOSParameters(), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.LicenseLocal);
		});
		LicenseFileDataModel licenseFileDataModel = parameter as LicenseFileDataModel;
		licenseFileModel = licenseFileDataModel.FileData;
		filePath = licenseFileDataModel.Path;
		description1LabelControl.Text = PrepareLicenseText();
		tileViewDoubleClickSupport.SetParameters();
		recentListUserControl.GridControl.BeginUpdate();
		recentListUserControl.GridControl.DataSource = null;
		recentListUserControl.GridControl.EndUpdate();
		backSimpleButtonLayoutControlItem.Visibility = ((!base.BackButtonVisibility) ? LayoutVisibility.Never : LayoutVisibility.Always);
	}

	internal override async Task<bool> Navigated()
	{
		await base.Navigated();
		recentListUserControl.GridControl.BeginUpdate();
		recentListUserControl.GridControl.DataSource = licenseFileModel.Licenses;
		recentListUserControl.GridControl.EndUpdate();
		recentListUserControl.SetParameters();
		SetNextButtonAvailability();
		recentListUserControl.GridControl.ForceInitialize();
		if (licenseFileModel.Licenses.Count() > 0)
		{
			if (!panelControl.Controls.Contains(recentListUserControl))
			{
				panelControl.Controls.Clear();
				panelControl.Controls.Add(recentListUserControl);
			}
			FileLicenseData fileLicenseData = licenseFileModel.Licenses.FirstOrDefault((FileLicenseData x) => x.FileLicenseModel.Package.Equals(licenseFileModel?.LastSelectedLicense?.Package));
			if (fileLicenseData != null)
			{
				int focusedRowHandle = recentListUserControl.TileView.FindRow(fileLicenseData);
				recentListUserControl.TileView.FocusedRowHandle = focusedRowHandle;
			}
		}
		else if (!panelControl.Controls.Contains(noLicensesUserControl))
		{
			panelControl.Controls.Clear();
			panelControl.Controls.Add(noLicensesUserControl);
		}
		if (!base.SuppressNextAction && recentListUserControl.TileView.SelectedRowsCount > 0 && licenseFileModel.LastSelectedLicense != null)
		{
			await ProcessNextAction(GetSelectedItem());
		}
		return true;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		Control control = FindFocusedControl();
		if (keyData == Keys.Return)
		{
			ProcessNextAction(GetSelectedItem());
		}
		control?.Focus();
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void SetNextButtonAvailability()
	{
		nextSimpleButton.Enabled = GetSelectedItem()?.IsValid ?? false;
	}

	private async Task ProcessNextAction(FileLicenseData licenseDataResult)
	{
		if (licenseDataResult != null && licenseDataResult.IsValid)
		{
			ShowLoader();
			LicenseFileDataHelper.Save(licenseFileModel.LicenseFile, GetSelectedItem().FileLicenseModel);
			TrackingRunner.Track(delegate
			{
				TrackingService.MakeAsyncRequest(new ParametersWithUserOsDataedoBuilder(new TrackingOSParameters(), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.LicenseSelected);
			});
			OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.Next));
			HideLoader();
		}
	}

	private async void TileViewDoubleClickSupport_DoubleClick(object sender, EventArgs e)
	{
		await ProcessNextAction(GetSelectedItem());
	}

	private async void TileView_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			await ProcessNextAction(GetSelectedItem());
		}
	}

	private void TileView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		TileView tileView = sender as TileView;
		SetNextButtonAvailability();
		FileLicenseData selectedItem = GetSelectedItem();
		if (selectedItem == null || !selectedItem.IsValid)
		{
			tileView?.UnselectRow(e.FocusedRowHandle);
		}
	}

	private void BackSimpleButton_Click(object sender, EventArgs e)
	{
		recentListUserControl.SetLastFocusedRowHandle();
		OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.Back, new LicenseFileDataModel(filePath, licenseFileModel)));
	}

	private async void NextSimpleButton_Click(object sender, EventArgs e)
	{
		await ProcessNextAction(GetSelectedItem());
	}

	private FileLicenseData GetSelectedItem()
	{
		recentListUserControl.TileView.GetRow(recentListUserControl.TileView.FocusedRowHandle);
		return recentListUserControl.TileView.GetRow(recentListUserControl.TileView.FocusedRowHandle) as FileLicenseData;
	}

	private void Description1LabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		OpeningLinks.OpenLink(e);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.panelControl = new DevExpress.XtraEditors.PanelControl();
		this.smallLogoUserControl = new Dataedo.App.LoginFormTools.UserControls.Common.SmallLogoUserControl();
		this.nextSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.backSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.description1LabelControl = new DevExpress.XtraEditors.LabelControl();
		this.headerLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.logoPictureEditTopSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.logoPictureEditRightSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.headerLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.description1LabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.backSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.buttonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.nextSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.smallLogoUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.panelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.separatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.backSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nextSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.panelControl);
		this.mainLayoutControl.Controls.Add(this.smallLogoUserControl);
		this.mainLayoutControl.Controls.Add(this.nextSimpleButton);
		this.mainLayoutControl.Controls.Add(this.backSimpleButton);
		this.mainLayoutControl.Controls.Add(this.description1LabelControl);
		this.mainLayoutControl.Controls.Add(this.headerLabelControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(120, 526, 855, 685);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.mainLayoutControl.ToolTipController = this.toolTipController;
		this.panelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.panelControl.Location = new System.Drawing.Point(25, 126);
		this.panelControl.Margin = new System.Windows.Forms.Padding(0);
		this.panelControl.Name = "panelControl";
		this.panelControl.Size = new System.Drawing.Size(650, 270);
		this.panelControl.TabIndex = 30;
		this.smallLogoUserControl.Location = new System.Drawing.Point(27, 419);
		this.smallLogoUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.smallLogoUserControl.MaximumSize = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.MinimumSize = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.Name = "smallLogoUserControl";
		this.smallLogoUserControl.Size = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.TabIndex = 22;
		this.nextSimpleButton.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.nextSimpleButton.Appearance.Options.UseFont = true;
		this.nextSimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
		this.nextSimpleButton.Location = new System.Drawing.Point(590, 416);
		this.nextSimpleButton.MaximumSize = new System.Drawing.Size(85, 29);
		this.nextSimpleButton.MinimumSize = new System.Drawing.Size(85, 29);
		this.nextSimpleButton.Name = "nextSimpleButton";
		this.nextSimpleButton.Size = new System.Drawing.Size(85, 29);
		this.nextSimpleButton.StyleController = this.mainLayoutControl;
		this.nextSimpleButton.TabIndex = 20;
		this.nextSimpleButton.Text = "Next";
		this.nextSimpleButton.Click += new System.EventHandler(NextSimpleButton_Click);
		this.backSimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
		this.backSimpleButton.Location = new System.Drawing.Point(510, 416);
		this.backSimpleButton.MaximumSize = new System.Drawing.Size(70, 29);
		this.backSimpleButton.MinimumSize = new System.Drawing.Size(70, 29);
		this.backSimpleButton.Name = "backSimpleButton";
		this.backSimpleButton.Size = new System.Drawing.Size(70, 29);
		this.backSimpleButton.StyleController = this.mainLayoutControl;
		this.backSimpleButton.TabIndex = 18;
		this.backSimpleButton.Text = "Back";
		this.backSimpleButton.Click += new System.EventHandler(BackSimpleButton_Click);
		this.description1LabelControl.AllowHtmlString = true;
		this.description1LabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.description1LabelControl.Appearance.Options.UseFont = true;
		this.description1LabelControl.Appearance.Options.UseTextOptions = true;
		this.description1LabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
		this.description1LabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.description1LabelControl.Location = new System.Drawing.Point(27, 82);
		this.description1LabelControl.Name = "description1LabelControl";
		this.description1LabelControl.Size = new System.Drawing.Size(646, 16);
		this.description1LabelControl.StyleController = this.mainLayoutControl;
		this.description1LabelControl.TabIndex = 10;
		this.description1LabelControl.Text = "Account: ";
		this.description1LabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(Description1LabelControl_HyperlinkClick);
		this.headerLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 24f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.headerLabelControl.Appearance.Options.UseFont = true;
		this.headerLabelControl.AutoEllipsis = true;
		this.headerLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.headerLabelControl.Location = new System.Drawing.Point(27, 25);
		this.headerLabelControl.Name = "headerLabelControl";
		this.headerLabelControl.Size = new System.Drawing.Size(646, 39);
		this.headerLabelControl.StyleController = this.mainLayoutControl;
		this.headerLabelControl.TabIndex = 8;
		this.headerLabelControl.Text = "Available licenses";
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[10] { this.logoPictureEditTopSeparatorEmptySpaceItem, this.logoPictureEditRightSeparatorEmptySpaceItem, this.headerLabelControlLayoutControlItem, this.description1LabelControlLayoutControlItem, this.backSimpleButtonLayoutControlItem, this.buttonsEmptySpaceItem, this.nextSimpleButtonLayoutControlItem, this.smallLogoUserControlLayoutControlItem, this.panelControlLayoutControlItem, this.separatorEmptySpaceItem });
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 25, 25);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControlGroup.TextVisible = false;
		this.logoPictureEditTopSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.logoPictureEditTopSeparatorEmptySpaceItem.Location = new System.Drawing.Point(0, 371);
		this.logoPictureEditTopSeparatorEmptySpaceItem.MinSize = new System.Drawing.Size(1, 10);
		this.logoPictureEditTopSeparatorEmptySpaceItem.Name = "logoPictureEditTopSeparatorEmptySpaceItem";
		this.logoPictureEditTopSeparatorEmptySpaceItem.Size = new System.Drawing.Size(97, 21);
		this.logoPictureEditTopSeparatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.logoPictureEditTopSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.logoPictureEditRightSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.logoPictureEditRightSeparatorEmptySpaceItem.Location = new System.Drawing.Point(97, 371);
		this.logoPictureEditRightSeparatorEmptySpaceItem.Name = "logoPictureEditRightSeparatorEmptySpaceItem";
		this.logoPictureEditRightSeparatorEmptySpaceItem.Size = new System.Drawing.Size(368, 49);
		this.logoPictureEditRightSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.headerLabelControlLayoutControlItem.Control = this.headerLabelControl;
		this.headerLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.headerLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 55);
		this.headerLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 55);
		this.headerLabelControlLayoutControlItem.Name = "headerLabelControlLayoutControlItem";
		this.headerLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 16);
		this.headerLabelControlLayoutControlItem.Size = new System.Drawing.Size(650, 55);
		this.headerLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.headerLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.headerLabelControlLayoutControlItem.TextVisible = false;
		this.description1LabelControlLayoutControlItem.Control = this.description1LabelControl;
		this.description1LabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 55);
		this.description1LabelControlLayoutControlItem.Name = "description1LabelControlLayoutControlItem";
		this.description1LabelControlLayoutControlItem.Size = new System.Drawing.Size(650, 20);
		this.description1LabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.description1LabelControlLayoutControlItem.TextVisible = false;
		this.backSimpleButtonLayoutControlItem.Control = this.backSimpleButton;
		this.backSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(465, 391);
		this.backSimpleButtonLayoutControlItem.Name = "backSimpleButtonLayoutControlItem";
		this.backSimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(20, 0, 0, 0);
		this.backSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(90, 29);
		this.backSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.backSimpleButtonLayoutControlItem.TextVisible = false;
		this.buttonsEmptySpaceItem.AllowHotTrack = false;
		this.buttonsEmptySpaceItem.Location = new System.Drawing.Point(465, 371);
		this.buttonsEmptySpaceItem.Name = "buttonsEmptySpaceItem";
		this.buttonsEmptySpaceItem.Size = new System.Drawing.Size(185, 20);
		this.buttonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.nextSimpleButtonLayoutControlItem.Control = this.nextSimpleButton;
		this.nextSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(555, 391);
		this.nextSimpleButtonLayoutControlItem.Name = "nextSimpleButtonLayoutControlItem";
		this.nextSimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 0, 0, 0);
		this.nextSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(95, 29);
		this.nextSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.nextSimpleButtonLayoutControlItem.TextVisible = false;
		this.smallLogoUserControlLayoutControlItem.Control = this.smallLogoUserControl;
		this.smallLogoUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 392);
		this.smallLogoUserControlLayoutControlItem.MaxSize = new System.Drawing.Size(97, 28);
		this.smallLogoUserControlLayoutControlItem.MinSize = new System.Drawing.Size(97, 28);
		this.smallLogoUserControlLayoutControlItem.Name = "smallLogoUserControlLayoutControlItem";
		this.smallLogoUserControlLayoutControlItem.Size = new System.Drawing.Size(97, 28);
		this.smallLogoUserControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.smallLogoUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.smallLogoUserControlLayoutControlItem.TextVisible = false;
		this.panelControlLayoutControlItem.Control = this.panelControl;
		this.panelControlLayoutControlItem.Location = new System.Drawing.Point(0, 101);
		this.panelControlLayoutControlItem.Name = "panelControlLayoutControlItem";
		this.panelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.panelControlLayoutControlItem.Size = new System.Drawing.Size(650, 270);
		this.panelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.panelControlLayoutControlItem.TextVisible = false;
		this.separatorEmptySpaceItem.AllowHotTrack = false;
		this.separatorEmptySpaceItem.Location = new System.Drawing.Point(0, 75);
		this.separatorEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 26);
		this.separatorEmptySpaceItem.MinSize = new System.Drawing.Size(104, 26);
		this.separatorEmptySpaceItem.Name = "separatorEmptySpaceItem";
		this.separatorEmptySpaceItem.Size = new System.Drawing.Size(650, 26);
		this.separatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.separatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Name = "FileLicensesPageUserControl";
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.backSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nextSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).EndInit();
		base.ResumeLayout(false);
	}
}
