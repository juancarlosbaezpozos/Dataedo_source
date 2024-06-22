using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.LoginFormTools.Tools.CustomEventArgs;
using Dataedo.App.LoginFormTools.Tools.Enums;
using Dataedo.App.LoginFormTools.Tools.Recent;
using Dataedo.App.LoginFormTools.Tools.Repository;
using Dataedo.App.LoginFormTools.UserControls.Base;
using Dataedo.App.LoginFormTools.UserControls.Common;
using Dataedo.App.LoginFormTools.UserControls.Subcontrols;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.CustomControls;
using Dataedo.CustomMessageBox;
using Dataedo.Data.Commands.Enums;
using Dataedo.LicenseHelperLibrary.Repository;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.LoginFormTools.UserControls;

public class ConnectToServerRepositoryPageUserControl : BasePageUserControl
{
	private readonly ConnectToSqlServerRepositoryUserControl connectToControl;

	private RecentItemModel recentItemModel;

	private string oldConnectionString;

	private DatabaseType? oldRepositoryType;

	private CancellationTokenSource connectCancellationTokenSource;

	private static bool isFirstRun;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private EmptySpaceItem logoPictureEditRightSeparatorEmptySpaceItem;

	private LabelControl headerLabelControl;

	private LayoutControlItem headerLabelControlLayoutControlItem;

	private LabelControl description1LabelControl;

	private LayoutControlItem description1LabelControlLayoutControlItem;

	private SimpleButton backSimpleButton;

	private LayoutControlItem backSimpleButtonLayoutControlItem;

	private SimpleButton connectToRepositorySimpleButton;

	private LayoutControlItem connectToRepositorySimpleButtonLayoutControlItem;

	private PanelControl contentPanelControl;

	private LayoutControlItem contentPanelControlLayoutControlItem;

	private XtraScrollableControl xtraScrollableControl;

	private SmallLogoUserControl smallLogoUserControl;

	private LayoutControlItem smallLogoUserControlLayoutControlItem;

	private ToolTipController toolTipController;

	private EmptySpaceItem separatorEmptySpaceItem;

	public ConnectToServerRepositoryPageUserControl()
	{
		InitializeComponent();
		base.LoaderContext = this;
		connectToControl = new ConnectToSqlServerRepositoryUserControl();
		connectToControl.TimeConsumingOperationStarted += base.OnTimeConsumingOperationStarted;
		connectToControl.TimeConsumingOperationStopped += base.OnTimeConsumingOperationStopped;
		connectToControl.Dock = DockStyle.Fill;
		connectToControl.DetailsVisibilityChanged += ConnectToControl_DetailsVisibilityChanged;
		xtraScrollableControl.SuspendLayout();
		xtraScrollableControl.Controls.Clear();
		xtraScrollableControl.Controls.Add(connectToControl);
		xtraScrollableControl.ResumeLayout(performLayout: false);
		isFirstRun = true;
	}

	internal override void SetParameter(object parameter, bool isCalledAsPrevious)
	{
		base.SetParameter(parameter, isCalledAsPrevious);
		connectToControl.SetParameter();
		this.recentItemModel = null;
		if (!base.IsCalledAsPrevious || parameter != null)
		{
			if (parameter is RecentItemModel recentItemModel && recentItemModel.Data?.IsEmpty == false)
			{
				this.recentItemModel = recentItemModel;
				connectToControl.SetRepositoryLayoutItemVisibility(visible: true);
			}
			else
			{
				connectToControl.SetRepositoryLayoutItemVisibility(visible: false);
			}
		}
		backSimpleButton.Enabled = true;
		connectToRepositorySimpleButton.Enabled = true;
	}

	internal override async Task<bool> Navigated()
	{
		try
		{
			await base.Navigated();
			ShowLoader();
			bool flag = false;
			if (!base.IsCalledAsPrevious)
			{
				connectToControl.SetRecentData(recentItemModel, !base.IsCalledAsPrevious && !base.SuppressNextAction);
			}
			if (recentItemModel != null)
			{
				if (recentItemModel.Data.HasCompleteConnectionInfo)
				{
					if (!base.IsCalledAsPrevious && !base.SuppressNextAction)
					{
						flag = Connect(showMessage: true);
						if (flag)
						{
							connectToControl.ShowHideConnectionDetails();
						}
						connectToControl.ValidateRequiredFields();
					}
					connectToRepositorySimpleButton.Focus();
				}
				else
				{
					connectToControl.ShowHideConnectionDetails();
					if (!base.IsCalledAsPrevious && !base.SuppressNextAction)
					{
						connectToControl.ValidateRequiredFields();
					}
				}
			}
			if (!flag)
			{
				OnRequiresAction(this);
			}
			return flag;
		}
		finally
		{
			HideLoader();
		}
	}

	protected override void Dispose(bool disposing)
	{
		connectToControl?.Dispose();
		connectCancellationTokenSource?.Dispose();
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Return && connectToControl.AllowEnterNextAction)
		{
			Connect(showMessage: true);
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void ConnectToControl_DetailsVisibilityChanged(object sender, EventArgs e)
	{
		headerLabelControl.Text = ((connectToControl.AreDetailsShown || !connectToControl.IsRepositoryLocationValid) ? "Connect to server repository" : ("Connect to " + connectToControl.RepositoryLocation + " repository"));
	}

	private void BackSimpleButton_Click(object sender, EventArgs e)
	{
		connectToControl.CancelAsyncOperations();
		OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.Back));
	}

	private void ConnectToRepositorySimpleButton_Click(object sender, EventArgs e)
	{
		connectToControl.CancelAsyncOperations();
		Connect(showMessage: true);
	}

	private bool Connect(bool showMessage)
	{
		if (!connectToControl.ValidateRequiredFields())
		{
			return false;
		}
		try
		{
			backSimpleButton.Enabled = false;
			connectToRepositorySimpleButton.Enabled = false;
			ShowLoader();
			RepositoryExistsStatusEnum? connectionResult;
			Exception exception;
			string connectionString = connectToControl.GetConnectionString(out connectionResult, out exception);
			string database = StaticData.Database;
			StaticData.Database = connectToControl.Database;
			StaticData.SetRepositoryData(RepositoriesDBHelper.GetRepositoryData(StaticData.Database, isProjectFile: false, connectionString));
			TrackingRunner.Track(delegate
			{
				TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingConnectToRepositoryRepoParameters(isFile: false), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.ServerRepoConnecting);
			});
			if (ConnectProcess(showMessage))
			{
				OnTimeConsumingOperationStopped(this);
				OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.Connected));
				return true;
			}
			if (database != null)
			{
				StaticData.SetRepositoryData(RepositoriesDBHelper.GetRepositoryData(database, StaticData.LastValidRepositoryType == DatabaseType.SqlServerCe, StaticData.DataedoLastValidConnectionString));
				StaticData.Database = database;
			}
			TrackingRunner.Track(delegate
			{
				TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingConnectToRepositoryRepoParameters(isFile: false), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.ServerRepoConnectionFailed);
			});
			return false;
		}
		catch (Exception)
		{
			TrackingRunner.Track(delegate
			{
				TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoRepoBuilder(new TrackingConnectToRepositoryRepoParameters(isFile: false), new TrackingDataedoParameters(), new TrackingUserParameters()), TrackingEventEnum.ServerRepoConnectionFailed);
			});
			throw;
		}
		finally
		{
			backSimpleButton.Enabled = true;
			connectToRepositorySimpleButton.Enabled = true;
			HideLoader();
		}
	}

	private bool ConnectProcess(bool showMessage)
	{
		connectToControl.PrepareConnectionString(out var connectionResult, out var exception);
		if (!connectionResult.HasValue)
		{
			connectionResult = connectToControl.CheckIfRepositoryExists(ref exception);
		}
		if (connectionResult == RepositoryExistsStatusEnum.NotExists)
		{
			if (showMessage)
			{
				string message = "Unable to connect to repository." + Environment.NewLine + "Specified database doesn't exist or you don't have access to it." + Environment.NewLine + "To grant access to existing repository please use <href=" + Paths.GetAdminConsolePath() + ">Administration Console</href>.";
				string text = "Unable to connect to repository." + Environment.NewLine + "Specified database doesn't exist or you don't have access to it." + Environment.NewLine + "To grant access to existing repository please use Administration Console: " + Paths.GetAdminConsolePath() + ".";
				HideLoader();
				string messageSimple = text;
				GeneralMessageBoxesHandling.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, base.ParentForm, messageSimple);
			}
			return false;
		}
		if (connectionResult == RepositoryExistsStatusEnum.Error)
		{
			List<string> values = new List<string>
			{
				exception?.Message,
				exception?.InnerException?.Message
			};
			string text2 = Environment.NewLine + Environment.NewLine + ((exception != null && exception.InnerException?.Message != null) ? exception.InnerException.Message : exception.Message);
			HideLoader();
			GeneralMessageBoxesHandling.Show("Unable to connect to repository." + text2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, string.Join(Environment.NewLine + Environment.NewLine, values) ?? "", 1, base.ParentForm);
			return false;
		}
		if (!connectToControl.CheckIfRepositoryIsValid())
		{
			string message2 = "The database you're connecting to is not a Dataedo repository." + Environment.NewLine + "To import metadata from another database, create a repository first. <href=" + Links.HowToDocumentSQLServerDatabase + ">Learn more</href>" + Environment.NewLine + "If you were connecting to previously created Dataedo repository, <href=" + Links.SupportContact + ">contact us</href>";
			HideLoader();
			CustomMessageBoxForm.Show(message2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, base.ParentForm);
			return false;
		}
		oldConnectionString = StaticData.DataedoLastValidConnectionString;
		oldRepositoryType = StaticData.LastValidRepositoryType;
		StaticData.Initialize(connectToControl.RepositoryType, connectToControl.PreparedConnectionString);
		DB.ReloadClasses();
		StaticData.DataedoConnectionString = connectToControl.PreparedConnectionString;
		StaticData.Host = connectToControl.ServerName;
		StaticData.RepositoryType = connectToControl.RepositoryType;
		Licenses.Initialize(StaticData.Commands);
		if (!CheckRepositoryVersion(out var _, out var _, out var _))
		{
			SetOldConnectionString();
			return false;
		}
		StaticData.DataedoLastValidConnectionString = connectToControl.PreparedConnectionString;
		StaticData.LastValidRepositoryType = connectToControl.RepositoryType;
		connectToControl.AddRecentRecord();
		return true;
	}

	private bool CheckRepositoryVersion(out bool upgradeCancelled, out bool isLicenseValid, out RepositoryVersionEnum.RepositoryVersion versionMatch)
	{
		upgradeCancelled = false;
		isLicenseValid = true;
		versionMatch = RepositoryVersionEnum.RepositoryVersion.NOT_SET;
		if (!LoginHelper.GetRepositoryVersion(out var repositoryVersion))
		{
			return false;
		}
		repositoryVersion.GetMatchingRepositoryVersion(ProgramVersion.Major, ProgramVersion.Minor, ProgramVersion.Build, ProgramVersion.ReleaseType);
		versionMatch = StaticData.LicenseHelper.CompareRepositoryVersion(StaticData.DataedoConnectionString, ProgramVersion.Major, ProgramVersion.Minor, ProgramVersion.Build);
		if (!repositoryVersion.Stable)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("Repository is unstable and may cause errors. It is recommended to restore it from backup before starting work with Dataedo. Do you want to continue anyway?", "Error", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
			{
				return false;
			}
		}
		if (versionMatch == RepositoryVersionEnum.RepositoryVersion.OLDER || (ConfigHelper.GetForceRepoUpgradeConfigValue() && isFirstRun))
		{
			isFirstRun = false;
			if (isLicenseValid)
			{
				using UpgradeForm upgradeForm = new UpgradeForm(repositoryVersion, isFile: false, connectToControl.Database);
				bool detachWebCatalog = false;
				upgradeForm.DetachWebCatalogClicked += delegate
				{
					detachWebCatalog = true;
				};
				upgradeForm.UpgradeRepositoryClicked += delegate
				{
					OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.UpgradeServerRepository, new UpgradeDataModel(RecentSupport.GetRecentItemModel(connectToControl), repositoryVersion, connectToControl.PreparedConnectionString, detachWebCatalog)));
				};
				upgradeCancelled = upgradeForm.ShowDialog(base.ParentForm) != DialogResult.OK;
			}
			return false;
		}
		if (!StaticData.LicenseHelper.CheckRepositoryVersion(StaticData.DataedoConnectionString, ProgramVersion.Major, ProgramVersion.Minor, ProgramVersion.Build, isFile: false, out versionMatch))
		{
			return false;
		}
		return true;
	}

	private void SetOldConnectionString()
	{
		if (oldRepositoryType.HasValue && !string.IsNullOrEmpty(oldConnectionString))
		{
			StaticData.Initialize(oldRepositoryType.Value, oldConnectionString);
			DB.ReloadClasses();
			StaticData.DataedoConnectionString = oldConnectionString;
			StaticData.RepositoryType = oldRepositoryType.Value;
			Licenses.Initialize(StaticData.Commands);
		}
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.smallLogoUserControl = new Dataedo.App.LoginFormTools.UserControls.Common.SmallLogoUserControl();
		this.contentPanelControl = new DevExpress.XtraEditors.PanelControl();
		this.xtraScrollableControl = new DevExpress.XtraEditors.XtraScrollableControl();
		this.connectToRepositorySimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.backSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.description1LabelControl = new DevExpress.XtraEditors.LabelControl();
		this.headerLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.logoPictureEditRightSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.headerLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.description1LabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.backSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.connectToRepositorySimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.contentPanelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.smallLogoUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.separatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.contentPanelControl).BeginInit();
		this.contentPanelControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.backSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectToRepositorySimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.contentPanelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.smallLogoUserControl);
		this.mainLayoutControl.Controls.Add(this.contentPanelControl);
		this.mainLayoutControl.Controls.Add(this.connectToRepositorySimpleButton);
		this.mainLayoutControl.Controls.Add(this.backSimpleButton);
		this.mainLayoutControl.Controls.Add(this.description1LabelControl);
		this.mainLayoutControl.Controls.Add(this.headerLabelControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3613, 454, 855, 685);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.mainLayoutControl.ToolTipController = this.toolTipController;
		this.smallLogoUserControl.Location = new System.Drawing.Point(27, 418);
		this.smallLogoUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.smallLogoUserControl.MaximumSize = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.MinimumSize = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.Name = "smallLogoUserControl";
		this.smallLogoUserControl.Size = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.TabIndex = 22;
		this.contentPanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.contentPanelControl.Controls.Add(this.xtraScrollableControl);
		this.contentPanelControl.Location = new System.Drawing.Point(27, 140);
		this.contentPanelControl.Name = "contentPanelControl";
		this.contentPanelControl.Size = new System.Drawing.Size(646, 274);
		this.contentPanelControl.TabIndex = 21;
		this.xtraScrollableControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraScrollableControl.Location = new System.Drawing.Point(0, 0);
		this.xtraScrollableControl.Name = "xtraScrollableControl";
		this.xtraScrollableControl.Size = new System.Drawing.Size(646, 274);
		this.xtraScrollableControl.TabIndex = 0;
		this.connectToRepositorySimpleButton.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.connectToRepositorySimpleButton.Appearance.Options.UseFont = true;
		this.connectToRepositorySimpleButton.ImageOptions.Image = Dataedo.App.Properties.Resources.connect_16;
		this.connectToRepositorySimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
		this.connectToRepositorySimpleButton.Location = new System.Drawing.Point(590, 416);
		this.connectToRepositorySimpleButton.MaximumSize = new System.Drawing.Size(85, 29);
		this.connectToRepositorySimpleButton.MinimumSize = new System.Drawing.Size(85, 29);
		this.connectToRepositorySimpleButton.Name = "connectToRepositorySimpleButton";
		this.connectToRepositorySimpleButton.Size = new System.Drawing.Size(85, 29);
		this.connectToRepositorySimpleButton.StyleController = this.mainLayoutControl;
		this.connectToRepositorySimpleButton.TabIndex = 20;
		this.connectToRepositorySimpleButton.Text = "Connect";
		this.connectToRepositorySimpleButton.Click += new System.EventHandler(ConnectToRepositorySimpleButton_Click);
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
		this.description1LabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.description1LabelControl.Appearance.Options.UseFont = true;
		this.description1LabelControl.Appearance.Options.UseTextOptions = true;
		this.description1LabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
		this.description1LabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.description1LabelControl.Location = new System.Drawing.Point(27, 82);
		this.description1LabelControl.Name = "description1LabelControl";
		this.description1LabelControl.Size = new System.Drawing.Size(646, 32);
		this.description1LabelControl.StyleController = this.mainLayoutControl;
		this.description1LabelControl.TabIndex = 10;
		this.description1LabelControl.Text = "Connect to the repository database on an SQL Server instance.\r\nThis is not an option to connect to the database you want to document!";
		this.headerLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 24f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.headerLabelControl.Appearance.Options.UseFont = true;
		this.headerLabelControl.AutoEllipsis = true;
		this.headerLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.headerLabelControl.Location = new System.Drawing.Point(27, 25);
		this.headerLabelControl.Name = "headerLabelControl";
		this.headerLabelControl.Size = new System.Drawing.Size(646, 39);
		this.headerLabelControl.StyleController = this.mainLayoutControl;
		this.headerLabelControl.TabIndex = 8;
		this.headerLabelControl.Text = "Connect to server repository";
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[8] { this.logoPictureEditRightSeparatorEmptySpaceItem, this.headerLabelControlLayoutControlItem, this.description1LabelControlLayoutControlItem, this.backSimpleButtonLayoutControlItem, this.connectToRepositorySimpleButtonLayoutControlItem, this.contentPanelControlLayoutControlItem, this.smallLogoUserControlLayoutControlItem, this.separatorEmptySpaceItem });
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 25, 25);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControlGroup.TextVisible = false;
		this.logoPictureEditRightSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.logoPictureEditRightSeparatorEmptySpaceItem.Location = new System.Drawing.Point(97, 391);
		this.logoPictureEditRightSeparatorEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 28);
		this.logoPictureEditRightSeparatorEmptySpaceItem.MinSize = new System.Drawing.Size(10, 28);
		this.logoPictureEditRightSeparatorEmptySpaceItem.Name = "logoPictureEditRightSeparatorEmptySpaceItem";
		this.logoPictureEditRightSeparatorEmptySpaceItem.Size = new System.Drawing.Size(388, 29);
		this.logoPictureEditRightSeparatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
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
		this.description1LabelControlLayoutControlItem.Size = new System.Drawing.Size(650, 36);
		this.description1LabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.description1LabelControlLayoutControlItem.TextVisible = false;
		this.backSimpleButtonLayoutControlItem.Control = this.backSimpleButton;
		this.backSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(485, 391);
		this.backSimpleButtonLayoutControlItem.Name = "backSimpleButtonLayoutControlItem";
		this.backSimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.backSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(70, 29);
		this.backSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.backSimpleButtonLayoutControlItem.TextVisible = false;
		this.connectToRepositorySimpleButtonLayoutControlItem.Control = this.connectToRepositorySimpleButton;
		this.connectToRepositorySimpleButtonLayoutControlItem.Location = new System.Drawing.Point(555, 391);
		this.connectToRepositorySimpleButtonLayoutControlItem.Name = "connectToRepositorySimpleButtonLayoutControlItem";
		this.connectToRepositorySimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 0, 0, 0);
		this.connectToRepositorySimpleButtonLayoutControlItem.Size = new System.Drawing.Size(95, 29);
		this.connectToRepositorySimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.connectToRepositorySimpleButtonLayoutControlItem.TextVisible = false;
		this.contentPanelControlLayoutControlItem.Control = this.contentPanelControl;
		this.contentPanelControlLayoutControlItem.Location = new System.Drawing.Point(0, 113);
		this.contentPanelControlLayoutControlItem.Name = "contentPanelControlLayoutControlItem";
		this.contentPanelControlLayoutControlItem.Size = new System.Drawing.Size(650, 278);
		this.contentPanelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.contentPanelControlLayoutControlItem.TextVisible = false;
		this.smallLogoUserControlLayoutControlItem.Control = this.smallLogoUserControl;
		this.smallLogoUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 391);
		this.smallLogoUserControlLayoutControlItem.MaxSize = new System.Drawing.Size(97, 28);
		this.smallLogoUserControlLayoutControlItem.MinSize = new System.Drawing.Size(97, 28);
		this.smallLogoUserControlLayoutControlItem.Name = "smallLogoUserControlLayoutControlItem";
		this.smallLogoUserControlLayoutControlItem.Size = new System.Drawing.Size(97, 29);
		this.smallLogoUserControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.smallLogoUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.smallLogoUserControlLayoutControlItem.TextVisible = false;
		this.separatorEmptySpaceItem.AllowHotTrack = false;
		this.separatorEmptySpaceItem.Location = new System.Drawing.Point(0, 91);
		this.separatorEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 22);
		this.separatorEmptySpaceItem.MinSize = new System.Drawing.Size(104, 22);
		this.separatorEmptySpaceItem.Name = "separatorEmptySpaceItem";
		this.separatorEmptySpaceItem.Size = new System.Drawing.Size(650, 22);
		this.separatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.separatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Name = "ConnectToServerRepositoryPageUserControl";
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.contentPanelControl).EndInit();
		this.contentPanelControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.backSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectToRepositorySimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.contentPanelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).EndInit();
		base.ResumeLayout(false);
	}
}
