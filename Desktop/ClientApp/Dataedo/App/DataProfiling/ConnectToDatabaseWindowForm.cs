using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Licences;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.UserControls.ConnectorsControls;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;

namespace Dataedo.App.DataProfiling;

public class ConnectToDatabaseWindowForm : BaseXtraForm
{
	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl1;

	private SimpleButton cancelButton;

	private SimpleButton connectButton;

	private DbConnectUserControlNew dbConnectUserControl;

	private LayoutControlGroup Root;

	private LayoutControlItem layoutControlItem1;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem2;

	public DatabaseRow CurrentDatabaseRow { get; set; }

	public ConnectToDatabaseWindowForm(int databaseId, SharedDatabaseTypeEnum.DatabaseType? databaseTypeOriginal)
	{
		InitializeComponent();
		bool flag = databaseTypeOriginal.HasValue && Connectors.HasDatabaseTypeConnector(databaseTypeOriginal.Value);
		IDatabaseSupport databaseSupport = DatabaseSupportFactory.GetDatabaseSupport(databaseTypeOriginal);
		DBMSGridModel dbmsGridModel = new DBMSGridModel(SharedDatabaseTypeEnum.TypeToString(databaseTypeOriginal), databaseSupport.GetFriendlyDisplayNameForImport(isLite: false), flag ? databaseSupport.TypeImage : ToolStripRenderer.CreateDisabledImage(databaseSupport.TypeImage), flag ? "" : ("<href=" + Links.ManageAccounts + ">(upgrade to connect)</href>"), DatabaseSupportFactory.GetDatabaseSupport(databaseTypeOriginal)?.ImportFolders, isDatabase: true);
		dbConnectUserControl.InitializeDatabaseRow(databaseId);
		dbConnectUserControl.SetParameters(databaseId, false, databaseTypeOriginal, false, dbmsGridModel);
		dbConnectUserControl.SetSwitchDatabaseAvailability(isAvailable: false);
		base.Shown += delegate
		{
			Activate();
		};
	}

	private void connectButton_Click(object sender, EventArgs e)
	{
		if (!dbConnectUserControl.TestConnection(testForGettingDatabasesList: false))
		{
			CurrentDatabaseRow = null;
			return;
		}
		CurrentDatabaseRow = dbConnectUserControl.DatabaseRow;
		dbConnectUserControl.Save();
		base.DialogResult = DialogResult.OK;
		Close();
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
		this.nonCustomizableLayoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
		this.connectButton = new DevExpress.XtraEditors.SimpleButton();
		this.dbConnectUserControl = new Dataedo.App.UserControls.ConnectorsControls.DbConnectUserControlNew();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).BeginInit();
		this.nonCustomizableLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl1.AllowCustomization = false;
		this.nonCustomizableLayoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl1.Controls.Add(this.cancelButton);
		this.nonCustomizableLayoutControl1.Controls.Add(this.connectButton);
		this.nonCustomizableLayoutControl1.Controls.Add(this.dbConnectUserControl);
		this.nonCustomizableLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl1.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl1.Name = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(943, 63, 650, 400);
		this.nonCustomizableLayoutControl1.Root = this.Root;
		this.nonCustomizableLayoutControl1.Size = new System.Drawing.Size(528, 402);
		this.nonCustomizableLayoutControl1.TabIndex = 0;
		this.nonCustomizableLayoutControl1.Text = "nonCustomizableLayoutControl1";
		this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelButton.Location = new System.Drawing.Point(435, 368);
		this.cancelButton.Name = "cancelButton";
		this.cancelButton.Size = new System.Drawing.Size(81, 22);
		this.cancelButton.StyleController = this.nonCustomizableLayoutControl1;
		this.cancelButton.TabIndex = 6;
		this.cancelButton.Text = "Cancel";
		this.connectButton.Location = new System.Drawing.Point(340, 368);
		this.connectButton.Name = "connectButton";
		this.connectButton.Size = new System.Drawing.Size(81, 22);
		this.connectButton.StyleController = this.nonCustomizableLayoutControl1;
		this.connectButton.TabIndex = 5;
		this.connectButton.Text = "Connect";
		this.connectButton.Click += new System.EventHandler(connectButton_Click);
		this.dbConnectUserControl.BackColor = System.Drawing.Color.Transparent;
		this.dbConnectUserControl.DatabaseRow = null;
		this.dbConnectUserControl.IsDBAdded = false;
		this.dbConnectUserControl.IsExporting = false;
		this.dbConnectUserControl.Location = new System.Drawing.Point(12, 12);
		this.dbConnectUserControl.Name = "dbConnectUserControl";
		this.dbConnectUserControl.SelectedDatabaseType = null;
		this.dbConnectUserControl.Size = new System.Drawing.Size(504, 352);
		this.dbConnectUserControl.TabIndex = 4;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.layoutControlItem1, this.layoutControlItem2, this.layoutControlItem3, this.emptySpaceItem1, this.emptySpaceItem2 });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(528, 402);
		this.Root.TextVisible = false;
		this.layoutControlItem1.Control = this.dbConnectUserControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(508, 356);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem2.Control = this.connectButton;
		this.layoutControlItem2.Location = new System.Drawing.Point(328, 356);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(85, 26);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(85, 26);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(85, 26);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlItem3.Control = this.cancelButton;
		this.layoutControlItem3.Location = new System.Drawing.Point(423, 356);
		this.layoutControlItem3.MaxSize = new System.Drawing.Size(85, 26);
		this.layoutControlItem3.MinSize = new System.Drawing.Size(85, 26);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(85, 26);
		this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(413, 356);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(10, 26);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(0, 356);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(328, 26);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(528, 402);
		base.Controls.Add(this.nonCustomizableLayoutControl1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.IconOptions.LargeImage = Dataedo.App.Properties.Resources.icon_32;
		base.MaximizeBox = false;
		this.MaximumSize = new System.Drawing.Size(742, 432);
		base.Name = "ConnectToDatabaseWindowForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Connect to database";
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).EndInit();
		this.nonCustomizableLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		base.ResumeLayout(false);
	}
}
