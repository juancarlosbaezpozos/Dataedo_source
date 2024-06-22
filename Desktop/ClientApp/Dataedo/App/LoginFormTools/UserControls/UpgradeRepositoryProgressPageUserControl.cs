using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.LoginFormTools.Tools.CustomEventArgs;
using Dataedo.App.LoginFormTools.Tools.Enums;
using Dataedo.App.LoginFormTools.Tools.Recent;
using Dataedo.App.LoginFormTools.Tools.Repository;
using Dataedo.App.LoginFormTools.Tools.ScriptsSupport;
using Dataedo.App.LoginFormTools.UserControls.Base;
using Dataedo.App.LoginFormTools.UserControls.Common;
using Dataedo.App.LoginFormTools.UserControls.Subcontrols;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.LoginFormTools.UserControls;

public class UpgradeRepositoryProgressPageUserControl : BasePageUserControl
{
	private readonly CreateSqlServerRepositoryUserControl createControl;

	private RepositoryOperation repositoryOperation;

	private StringBuilder exceptions;

	private int progressPercent;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private EmptySpaceItem logoPictureEditTopSeparatorEmptySpaceItem;

	private EmptySpaceItem logoPictureEditRightSeparatorEmptySpaceItem;

	private LabelControl headerLabelControl;

	private LayoutControlItem headerLabelControlLayoutControlItem;

	private LabelControl description1LabelControl;

	private LayoutControlItem description1LabelControlLayoutControlItem;

	private EmptySpaceItem buttonsEmptySpaceItem;

	private SimpleButton cancelSimpleButton;

	private LayoutControlItem connectToRepositorySimpleButtonLayoutControlItem;

	private PanelControl contentPanelControl;

	private LayoutControlItem contentPanelControlLayoutControlItem;

	private SmallLogoUserControl smallLogoUserControl;

	private LayoutControlItem smallLogoUserControlLayoutControlItem;

	private ToolTipController toolTipController;

	private ProgressBarControl progressBarControl;

	private BackgroundWorker backgroundWorker;

	private EmptySpaceItem separatorEmptySpaceItem;

	public RecentItemModel RecentItemModel => repositoryOperation.RecentItemModel;

	private List<Instruction> Instructions => repositoryOperation.Instructions;

	private string ConnectionString => repositoryOperation.ConnectionString;

	public UpgradeRepositoryProgressPageUserControl()
	{
		InitializeComponent();
		createControl = new CreateSqlServerRepositoryUserControl();
		createControl.Dock = DockStyle.Fill;
	}

	internal override void SetParameter(object parameter, bool isCalledAsPrevious)
	{
		base.SetParameter(parameter, isCalledAsPrevious);
		repositoryOperation = parameter as RepositoryOperation;
		exceptions = new StringBuilder();
		progressPercent = 0;
		progressBarControl.EditValue = 0;
		progressBarControl.Properties.Maximum = 100;
	}

	internal override async Task<bool> Navigated()
	{
		backgroundWorker.RunWorkerAsync();
		return await base.Navigated();
	}

	protected override void Dispose(bool disposing)
	{
		createControl?.Dispose();
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void CancelSimpleButton_Click(object sender, EventArgs e)
	{
		if (GeneralMessageBoxesHandling.Show("Please confirm.\nYou are about to cancel the ongoing update.\nThis can leave the repository in an unstable state so that it will be unusable.\nAre you sure?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation).DialogResult == DialogResult.Yes)
		{
			backgroundWorker.CancelAsync();
		}
	}

	private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
	{
		base.AllowClosing = false;
		using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
		sqlConnection.Open();
		int count = Instructions.Count;
		for (int i = 0; i < count; i++)
		{
			Instruction instruction = Instructions[i];
			if (backgroundWorker.CancellationPending)
			{
				e.Cancel = true;
				break;
			}
			try
			{
				using SqlCommand sqlCommand = CommandsWithTimeout.SqlServerForRepository(instruction.Script, sqlConnection);
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				exceptions.AppendLine(ex.Message);
				break;
			}
			int num = (int)Math.Round(((float)i + 1f) / (float)Instructions.Count * 100f);
			if (progressPercent != num)
			{
				progressPercent = num;
				backgroundWorker.ReportProgress(num);
			}
		}
	}

	private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		if (!backgroundWorker.CancellationPending)
		{
			progressBarControl.Increment(e.ProgressPercentage - (int)progressBarControl.EditValue);
			progressBarControl.Update();
		}
	}

	private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		base.AllowClosing = true;
		string title;
		string description;
		if (exceptions.Length == 0 && !e.Cancelled)
		{
			title = "Repository upgraded";
			description = "Repository has been successfully upgraded.";
			OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.OperationCompletedSuccessfully, new RepositoryOperationCompleted
			{
				Title = title,
				Description = description,
				Exceptions = exceptions,
				RecentItemModel = RecentItemModel
			}));
			return;
		}
		if (exceptions.Length > 0)
		{
			title = "Repository not upgraded";
			description = "Repository upgrade failed. Repository may be broken, it is recommended to restore it from backup.";
		}
		else if (e.Cancelled)
		{
			title = "Repository not upgraded";
			description = "Operation was canceled by user.";
		}
		else
		{
			title = "Repository not created";
			description = "Operation failed.";
		}
		OnAction(this, new ActionEventArgs(ActionResultEnum.ActionResult.OperationCompletedNotSuccessfully, new RepositoryOperationCompleted
		{
			Title = title,
			Description = description,
			Exceptions = exceptions,
			RecentItemModel = RecentItemModel
		}));
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.smallLogoUserControl = new Dataedo.App.LoginFormTools.UserControls.Common.SmallLogoUserControl();
		this.contentPanelControl = new DevExpress.XtraEditors.PanelControl();
		this.progressBarControl = new DevExpress.XtraEditors.ProgressBarControl();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.description1LabelControl = new DevExpress.XtraEditors.LabelControl();
		this.headerLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.logoPictureEditTopSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.logoPictureEditRightSeparatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.headerLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.description1LabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.buttonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.connectToRepositorySimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.contentPanelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.smallLogoUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
		this.separatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.contentPanelControl).BeginInit();
		this.contentPanelControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.progressBarControl.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectToRepositorySimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.contentPanelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.smallLogoUserControl);
		this.mainLayoutControl.Controls.Add(this.contentPanelControl);
		this.mainLayoutControl.Controls.Add(this.cancelSimpleButton);
		this.mainLayoutControl.Controls.Add(this.description1LabelControl);
		this.mainLayoutControl.Controls.Add(this.headerLabelControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3338, 376, 855, 685);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.mainLayoutControl.ToolTipController = this.toolTipController;
		this.smallLogoUserControl.Location = new System.Drawing.Point(27, 419);
		this.smallLogoUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.smallLogoUserControl.MaximumSize = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.MinimumSize = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.Name = "smallLogoUserControl";
		this.smallLogoUserControl.Size = new System.Drawing.Size(93, 24);
		this.smallLogoUserControl.TabIndex = 22;
		this.contentPanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.contentPanelControl.Controls.Add(this.progressBarControl);
		this.contentPanelControl.Location = new System.Drawing.Point(27, 128);
		this.contentPanelControl.Name = "contentPanelControl";
		this.contentPanelControl.Size = new System.Drawing.Size(646, 259);
		this.contentPanelControl.TabIndex = 21;
		this.progressBarControl.Location = new System.Drawing.Point(20, 105);
		this.progressBarControl.Name = "progressBarControl";
		this.progressBarControl.Properties.ShowTitle = true;
		this.progressBarControl.Properties.Step = 1;
		this.progressBarControl.Size = new System.Drawing.Size(606, 41);
		this.progressBarControl.TabIndex = 18;
		this.cancelSimpleButton.AllowFocus = false;
		this.cancelSimpleButton.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.cancelSimpleButton.Appearance.Options.UseFont = true;
		this.cancelSimpleButton.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
		this.cancelSimpleButton.Location = new System.Drawing.Point(590, 416);
		this.cancelSimpleButton.MaximumSize = new System.Drawing.Size(85, 29);
		this.cancelSimpleButton.MinimumSize = new System.Drawing.Size(85, 29);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(85, 29);
		this.cancelSimpleButton.StyleController = this.mainLayoutControl;
		this.cancelSimpleButton.TabIndex = 20;
		this.cancelSimpleButton.Text = "Cancel";
		this.cancelSimpleButton.Click += new System.EventHandler(CancelSimpleButton_Click);
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
		this.description1LabelControl.Text = "Please wait while the repository is being upgraded.";
		this.headerLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 24f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.headerLabelControl.Appearance.Options.UseFont = true;
		this.headerLabelControl.AutoEllipsis = true;
		this.headerLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.headerLabelControl.Location = new System.Drawing.Point(27, 25);
		this.headerLabelControl.Name = "headerLabelControl";
		this.headerLabelControl.Size = new System.Drawing.Size(646, 39);
		this.headerLabelControl.StyleController = this.mainLayoutControl;
		this.headerLabelControl.TabIndex = 8;
		this.headerLabelControl.Text = "Upgrading repository";
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[9] { this.logoPictureEditTopSeparatorEmptySpaceItem, this.logoPictureEditRightSeparatorEmptySpaceItem, this.headerLabelControlLayoutControlItem, this.description1LabelControlLayoutControlItem, this.buttonsEmptySpaceItem, this.connectToRepositorySimpleButtonLayoutControlItem, this.contentPanelControlLayoutControlItem, this.smallLogoUserControlLayoutControlItem, this.separatorEmptySpaceItem });
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 25, 25);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControlGroup.TextVisible = false;
		this.logoPictureEditTopSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.logoPictureEditTopSeparatorEmptySpaceItem.Location = new System.Drawing.Point(0, 364);
		this.logoPictureEditTopSeparatorEmptySpaceItem.MinSize = new System.Drawing.Size(1, 10);
		this.logoPictureEditTopSeparatorEmptySpaceItem.Name = "logoPictureEditTopSeparatorEmptySpaceItem";
		this.logoPictureEditTopSeparatorEmptySpaceItem.Size = new System.Drawing.Size(97, 28);
		this.logoPictureEditTopSeparatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.logoPictureEditTopSeparatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.logoPictureEditRightSeparatorEmptySpaceItem.AllowHotTrack = false;
		this.logoPictureEditRightSeparatorEmptySpaceItem.Location = new System.Drawing.Point(97, 364);
		this.logoPictureEditRightSeparatorEmptySpaceItem.Name = "logoPictureEditRightSeparatorEmptySpaceItem";
		this.logoPictureEditRightSeparatorEmptySpaceItem.Size = new System.Drawing.Size(458, 56);
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
		this.buttonsEmptySpaceItem.AllowHotTrack = false;
		this.buttonsEmptySpaceItem.Location = new System.Drawing.Point(555, 364);
		this.buttonsEmptySpaceItem.Name = "buttonsEmptySpaceItem";
		this.buttonsEmptySpaceItem.Size = new System.Drawing.Size(95, 27);
		this.buttonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.connectToRepositorySimpleButtonLayoutControlItem.Control = this.cancelSimpleButton;
		this.connectToRepositorySimpleButtonLayoutControlItem.Location = new System.Drawing.Point(555, 391);
		this.connectToRepositorySimpleButtonLayoutControlItem.Name = "connectToRepositorySimpleButtonLayoutControlItem";
		this.connectToRepositorySimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 0, 0, 0);
		this.connectToRepositorySimpleButtonLayoutControlItem.Size = new System.Drawing.Size(95, 29);
		this.connectToRepositorySimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.connectToRepositorySimpleButtonLayoutControlItem.TextVisible = false;
		this.contentPanelControlLayoutControlItem.Control = this.contentPanelControl;
		this.contentPanelControlLayoutControlItem.Location = new System.Drawing.Point(0, 101);
		this.contentPanelControlLayoutControlItem.Name = "contentPanelControlLayoutControlItem";
		this.contentPanelControlLayoutControlItem.Size = new System.Drawing.Size(650, 263);
		this.contentPanelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.contentPanelControlLayoutControlItem.TextVisible = false;
		this.smallLogoUserControlLayoutControlItem.Control = this.smallLogoUserControl;
		this.smallLogoUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 392);
		this.smallLogoUserControlLayoutControlItem.MaxSize = new System.Drawing.Size(97, 28);
		this.smallLogoUserControlLayoutControlItem.MinSize = new System.Drawing.Size(97, 28);
		this.smallLogoUserControlLayoutControlItem.Name = "smallLogoUserControlLayoutControlItem";
		this.smallLogoUserControlLayoutControlItem.Size = new System.Drawing.Size(97, 28);
		this.smallLogoUserControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.smallLogoUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.smallLogoUserControlLayoutControlItem.TextVisible = false;
		this.backgroundWorker.WorkerReportsProgress = true;
		this.backgroundWorker.WorkerSupportsCancellation = true;
		this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(BackgroundWorker_DoWork);
		this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(BackgroundWorker_ProgressChanged);
		this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
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
		base.Name = "UpgradeRepositoryProgressPageUserControl";
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.contentPanelControl).EndInit();
		this.contentPanelControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.progressBarControl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditTopSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEditRightSeparatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.description1LabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectToRepositorySimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.contentPanelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.smallLogoUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).EndInit();
		base.ResumeLayout(false);
	}
}
