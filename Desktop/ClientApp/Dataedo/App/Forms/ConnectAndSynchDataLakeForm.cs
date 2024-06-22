using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Base.EventArgs;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.DataSourceControls;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls;
using Dataedo.CustomControls;
using DevExpress.XtraWizard;

namespace Dataedo.App.Forms;

public class ConnectAndSynchDataLakeForm : BaseXtraForm
{
	private IContainer components;

	private WizardControl wizardControl;

	private WizardPage connectionTypeWizardPage;

	private CompletionWizardPage completionWizardPage1;

	private ConnectionTypeUserControl connectionTypeUserControl;

	private WizardPage connectionWizardPage;

	private FileSystemSourceUserControl fileSystemSourceUserControl;

	private WizardPage choosingDataWizardPage;

	private HiererchyAndListsUserControl hiererchyAndListsUserControl;

	public ConnectAndSynchDataLakeForm()
	{
		InitializeComponent();
		Text = "Add Data Lake";
	}

	private void wizardControl_SelectedPageChanged(object sender, WizardPageChangedEventArgs e)
	{
	}

	private void wizardControl_NextClick(object sender, WizardCommandButtonClickEventArgs e)
	{
	}

	private void wizardControl_PrevClick(object sender, WizardCommandButtonClickEventArgs e)
	{
	}

	private void wizardControl_CancelClick(object sender, CancelEventArgs e)
	{
	}

	private void wizardControl_CustomizeCommandButtons(object sender, CustomizeCommandButtonsEventArgs e)
	{
	}

	private void connectionTypeUserControl_AllowContinueChanged(object sender, NavigationEventArgs e)
	{
		connectionTypeWizardPage.AllowNext = e?.IsAllowed ?? false;
	}

	private void connectionTypeUserControl_Continue(object sender, EventArgs e)
	{
		wizardControl.SelectedPageIndex++;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.ConnectAndSynchDataLakeForm));
		this.wizardControl = new DevExpress.XtraWizard.WizardControl();
		this.connectionTypeWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.connectionTypeUserControl = new Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.ConnectionTypeUserControl();
		this.completionWizardPage1 = new DevExpress.XtraWizard.CompletionWizardPage();
		this.connectionWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.fileSystemSourceUserControl = new Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.DataSourceControls.FileSystemSourceUserControl();
		this.choosingDataWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.hiererchyAndListsUserControl = new Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.HiererchyAndListsUserControl();
		((System.ComponentModel.ISupportInitialize)this.wizardControl).BeginInit();
		this.wizardControl.SuspendLayout();
		this.connectionTypeWizardPage.SuspendLayout();
		this.connectionWizardPage.SuspendLayout();
		this.choosingDataWizardPage.SuspendLayout();
		base.SuspendLayout();
		this.wizardControl.Controls.Add(this.connectionTypeWizardPage);
		this.wizardControl.Controls.Add(this.completionWizardPage1);
		this.wizardControl.Controls.Add(this.connectionWizardPage);
		this.wizardControl.Controls.Add(this.choosingDataWizardPage);
		this.wizardControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.wizardControl.Image = Dataedo.App.Properties.Resources.empty_image;
		this.wizardControl.ImageLayout = System.Windows.Forms.ImageLayout.None;
		this.wizardControl.ImageWidth = 1;
		this.wizardControl.Name = "wizardControl";
		this.wizardControl.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[4] { this.connectionTypeWizardPage, this.connectionWizardPage, this.choosingDataWizardPage, this.completionWizardPage1 });
		this.wizardControl.Size = new System.Drawing.Size(840, 560);
		this.wizardControl.UseCancelButton = false;
		this.wizardControl.SelectedPageChanged += new DevExpress.XtraWizard.WizardPageChangedEventHandler(wizardControl_SelectedPageChanged);
		this.wizardControl.CancelClick += new System.ComponentModel.CancelEventHandler(wizardControl_CancelClick);
		this.wizardControl.NextClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(wizardControl_NextClick);
		this.wizardControl.PrevClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(wizardControl_PrevClick);
		this.wizardControl.CustomizeCommandButtons += new DevExpress.XtraWizard.WizardCustomizeCommandButtonsEventHandler(wizardControl_CustomizeCommandButtons);
		this.connectionTypeWizardPage.Controls.Add(this.connectionTypeUserControl);
		this.connectionTypeWizardPage.DescriptionText = "Please choose connection type for data source you want to document.";
		this.connectionTypeWizardPage.Name = "connectionTypeWizardPage";
		this.connectionTypeWizardPage.Size = new System.Drawing.Size(808, 417);
		this.connectionTypeWizardPage.Text = "Connection";
		this.connectionTypeUserControl.BackColor = System.Drawing.Color.Transparent;
		this.connectionTypeUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.connectionTypeUserControl.Location = new System.Drawing.Point(0, 0);
		this.connectionTypeUserControl.Name = "connectionTypeUserControl";
		this.connectionTypeUserControl.Size = new System.Drawing.Size(808, 417);
		this.connectionTypeUserControl.TabIndex = 0;
		this.connectionTypeUserControl.AllowContinueChanged += new System.EventHandler<Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Base.EventArgs.NavigationEventArgs>(connectionTypeUserControl_AllowContinueChanged);
		this.connectionTypeUserControl.Continue += new System.EventHandler(connectionTypeUserControl_Continue);
		this.completionWizardPage1.Name = "completionWizardPage1";
		this.completionWizardPage1.Size = new System.Drawing.Size(807, 428);
		this.connectionWizardPage.Controls.Add(this.fileSystemSourceUserControl);
		this.connectionWizardPage.DescriptionText = "Please provide connection details to data source you would like to document";
		this.connectionWizardPage.Name = "connectionWizardPage";
		this.connectionWizardPage.Size = new System.Drawing.Size(808, 417);
		this.connectionWizardPage.Text = "Connection details";
		this.fileSystemSourceUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.fileSystemSourceUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.fileSystemSourceUserControl.Location = new System.Drawing.Point(0, 0);
		this.fileSystemSourceUserControl.Name = "fileSystemSourceUserControl";
		this.fileSystemSourceUserControl.Size = new System.Drawing.Size(808, 417);
		this.fileSystemSourceUserControl.TabIndex = 0;
		this.choosingDataWizardPage.Controls.Add(this.hiererchyAndListsUserControl);
		this.choosingDataWizardPage.DescriptionText = "Select objects you want to import";
		this.choosingDataWizardPage.Name = "choosingDataWizardPage";
		this.choosingDataWizardPage.Size = new System.Drawing.Size(808, 417);
		this.choosingDataWizardPage.Text = "Objects to import";
		this.hiererchyAndListsUserControl.BackColor = System.Drawing.Color.Transparent;
		this.hiererchyAndListsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.hiererchyAndListsUserControl.Location = new System.Drawing.Point(0, 0);
		this.hiererchyAndListsUserControl.Name = "hiererchyAndListsUserControl";
		this.hiererchyAndListsUserControl.Size = new System.Drawing.Size(808, 417);
		this.hiererchyAndListsUserControl.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(840, 560);
		base.Controls.Add(this.wizardControl);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("ConnectAndSynchDataLakeForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		this.MinimumSize = new System.Drawing.Size(840, 590);
		base.Name = "ConnectAndSynchDataLakeForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Add Data Lake";
		((System.ComponentModel.ISupportInitialize)this.wizardControl).EndInit();
		this.wizardControl.ResumeLayout(false);
		this.connectionTypeWizardPage.ResumeLayout(false);
		this.connectionWizardPage.ResumeLayout(false);
		this.choosingDataWizardPage.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
