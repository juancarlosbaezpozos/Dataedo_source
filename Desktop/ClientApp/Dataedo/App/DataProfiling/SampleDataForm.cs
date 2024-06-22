using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DataProfiling.Tools;
using Dataedo.App.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.CustomControls;
using Dataedo.DataSources.Base.DataProfiling.Model;
using Dataedo.DataSources.Base.Models;
using Dataedo.DataSources.Commands;
using Dataedo.DataSources.Enums;
using Dataedo.DataSources.Factories;
using Dataedo.Model.Data.DataProfiling;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.DataProfiling;

public class SampleDataForm : BaseXtraForm
{
	private int tableId;

	private string tableName;

	private string schema;

	private SharedObjectTypeEnum.ObjectType objectType;

	private int databaseId;

	private SharedDatabaseTypeEnum.DatabaseType? databaseType;

	private string databaseDBMSVersion;

	private DatabaseRow connectedDatabaseRow;

	private bool isInitializationInProgress;

	private readonly CancellationTokenSource cancellationTokenSource;

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private LabelControl tableTitleLabelControl;

	private LayoutControlItem layoutControlItem1;

	private LabelControl previevOnlyLabelControl;

	private LayoutControlItem layoutControlItem2;

	private LabelControl sampleDataTextLabelControl;

	private LayoutControlItem layoutControlItem3;

	private GridControl sampleDataGridControl;

	private GridView sampleDataGridControlGridView;

	private LayoutControlItem sampleDataGridControlLayoutControlItem;

	private SimpleButton closeBtn;

	private SimpleButton refreshBtn;

	private LayoutControlItem layoutControlItem4;

	private LayoutControlItem layoutControlItem5;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem2;

	private SplashScreenManager splashScreenManager1;

	public SampleDataForm()
	{
		InitializeComponent();
		cancellationTokenSource = new CancellationTokenSource();
		base.Shown += SampleDataForm_Shown;
	}

	public void SetParameters(int tableId, string tableName, string schema, SharedObjectTypeEnum.ObjectType objectType, int databaseId, SharedDatabaseTypeEnum.DatabaseType? databaseType, string databaseDBMSVersion)
	{
		this.tableId = tableId;
		this.tableName = tableName;
		this.schema = schema;
		this.objectType = objectType;
		this.databaseId = databaseId;
		this.databaseType = databaseType;
		this.databaseDBMSVersion = databaseDBMSVersion;
		tableTitleLabelControl.Text = tableName;
	}

	private async void SampleDataForm_Shown(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			isInitializationInProgress = true;
			await Task.Delay(100);
			await InitDataSourceAsync();
			if (connectedDatabaseRow != null)
			{
				DataProfilingTrackingHelper.TrackSampleData(connectedDatabaseRow);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, this);
		}
		finally
		{
			isInitializationInProgress = false;
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Escape && !isInitializationInProgress)
		{
			Close();
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private async Task InitDataSourceAsync()
	{
		try
		{
			StaticData.CrashedDatabaseType = databaseType;
			StaticData.CrashedDBMSVersion = databaseDBMSVersion;
			ForceRefresh();
			base.Enabled = false;
			await SetDataSourceAsync();
			base.Enabled = true;
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager1, show: false);
			GeneralExceptionHandling.Handle(exception, this);
			Close();
		}
		finally
		{
			StaticData.ClearDatabaseInfoForCrashes();
		}
	}

	private async void refreshBtn_Click(object sender, EventArgs e)
	{
		try
		{
			base.Enabled = false;
			await SetDataSourceAsync();
			base.Enabled = true;
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager1, show: false);
			base.Enabled = true;
			GeneralExceptionHandling.Handle(exception, this);
			Close();
		}
	}

	private async Task SetDataSourceAsync()
	{
		_ = 1;
		try
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager1, show: true);
			splashScreenManager1.SetWaitFormDescription("Connecting to database");
			connectedDatabaseRow = DataProfiler.ReturnConnectedDatabaseRow(connectedDatabaseRow, databaseId, databaseType, this, splashScreenManager1);
			if (connectedDatabaseRow == null)
			{
				Close();
				return;
			}
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager1, show: true);
			ProfilingDatabaseTypeEnum.ProfilingDatabaseType profilingDatabaseType = EnumToEnumChanger.GetProfilingDatabaseTypeEnum(databaseType);
			CommandsSet dbConnectionCommands = CommandsFactory.GetDbConnectionCommands(profilingDatabaseType, connectedDatabaseRow.ConnectionString, CommandsWithTimeout.Timeout);
			splashScreenManager1.SetWaitFormDescription("Getting columns");
			List<ColumnProfiledDataObject> columns = DB.DataProfiling.SelectColumnsProfilingData(tableId);
			List<ProfilingField> columnsDictionary = (from c in columns
				where !DataTypeChecker.TypeIsNotSupportedForProfiling(c.DataType, profilingDatabaseType)
				select c into x
				select new ProfilingField(x.Name, DataTypeChecker.IsStringType(x.DataType), isEncrypted: false)).ToList();
			if (!columnsDictionary.Any())
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager1, show: false);
				GeneralMessageBoxesHandling.Show("The <i>" + tableName + "</i> " + objectType.ToString().ToLower() + " does not contain any columns with supported data type", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
				Close();
				return;
			}
			splashScreenManager1.SetWaitFormDescription("Getting sample data");
			long num = await dbConnectionCommands.DataProfiling.GetObjectRowsCount(schema, tableName, cancellationTokenSource.Token);
			if (num == 0L)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager1, show: false);
				GeneralMessageBoxesHandling.Show("The <i>" + tableName + "</i> " + objectType.ToString().ToLower() + " does not contain any rows", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
				Close();
			}
			else
			{
				SampleData sampleDataFromTableProfiling = await dbConnectionCommands.DataProfiling.GetObjectSampleData(schema, tableName, columnsDictionary, 10, cancellationTokenSource.Token, num, null, objectType == SharedObjectTypeEnum.ObjectType.View);
				DataProfilingUtils.PopulateSampleDataGridFromDownloadedSampleData(columns, sampleDataGridControl, sampleDataGridControlGridView, sampleDataFromTableProfiling, profilingDatabaseType);
				sampleDataTextLabelControl.Text = $"Sample, randomly picked rows: {sampleDataGridControlGridView.DataRowCount}";
			}
		}
		catch
		{
			throw;
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager1, show: false);
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
		this.closeBtn = new DevExpress.XtraEditors.SimpleButton();
		this.refreshBtn = new DevExpress.XtraEditors.SimpleButton();
		this.sampleDataGridControl = new DevExpress.XtraGrid.GridControl();
		this.sampleDataGridControlGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.sampleDataTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.previevOnlyLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.tableTitleLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.sampleDataGridControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.sampleDataGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sampleDataGridControlGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sampleDataGridControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.closeBtn);
		this.nonCustomizableLayoutControl.Controls.Add(this.refreshBtn);
		this.nonCustomizableLayoutControl.Controls.Add(this.sampleDataGridControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.sampleDataTextLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.previevOnlyLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.tableTitleLabelControl);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(847, 106, 650, 400);
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(800, 387);
		this.nonCustomizableLayoutControl.TabIndex = 0;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.closeBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.closeBtn.Location = new System.Drawing.Point(710, 353);
		this.closeBtn.Name = "closeBtn";
		this.closeBtn.Size = new System.Drawing.Size(78, 22);
		this.closeBtn.StyleController = this.nonCustomizableLayoutControl;
		this.closeBtn.TabIndex = 71;
		this.closeBtn.Text = "Close";
		this.refreshBtn.ImageOptions.Image = Dataedo.App.Properties.Resources.refresh_16;
		this.refreshBtn.Location = new System.Drawing.Point(619, 353);
		this.refreshBtn.Name = "refreshBtn";
		this.refreshBtn.Size = new System.Drawing.Size(77, 22);
		this.refreshBtn.StyleController = this.nonCustomizableLayoutControl;
		this.refreshBtn.TabIndex = 70;
		this.refreshBtn.Text = "Refresh";
		this.refreshBtn.Click += new System.EventHandler(refreshBtn_Click);
		this.sampleDataGridControl.EmbeddedNavigator.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.sampleDataGridControl.EmbeddedNavigator.Appearance.Options.UseBackColor = true;
		this.sampleDataGridControl.Location = new System.Drawing.Point(12, 83);
		this.sampleDataGridControl.MainView = this.sampleDataGridControlGridView;
		this.sampleDataGridControl.MinimumSize = new System.Drawing.Size(400, 0);
		this.sampleDataGridControl.Name = "sampleDataGridControl";
		this.sampleDataGridControl.Size = new System.Drawing.Size(776, 266);
		this.sampleDataGridControl.TabIndex = 69;
		this.sampleDataGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.sampleDataGridControlGridView });
		this.sampleDataGridControlGridView.GridControl = this.sampleDataGridControl;
		this.sampleDataGridControlGridView.Name = "sampleDataGridControlGridView";
		this.sampleDataGridControlGridView.OptionsBehavior.AutoPopulateColumns = false;
		this.sampleDataGridControlGridView.OptionsBehavior.Editable = false;
		this.sampleDataGridControlGridView.OptionsView.ColumnAutoWidth = false;
		this.sampleDataGridControlGridView.OptionsView.ShowGroupPanel = false;
		this.sampleDataGridControlGridView.OptionsView.ShowIndicator = false;
		this.sampleDataTextLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.sampleDataTextLabelControl.Appearance.Options.UseFont = true;
		this.sampleDataTextLabelControl.Location = new System.Drawing.Point(12, 66);
		this.sampleDataTextLabelControl.Name = "sampleDataTextLabelControl";
		this.sampleDataTextLabelControl.Size = new System.Drawing.Size(185, 13);
		this.sampleDataTextLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.sampleDataTextLabelControl.TabIndex = 68;
		this.sampleDataTextLabelControl.Text = "Sample, randomly picked rows: 0";
		this.previevOnlyLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.previevOnlyLabelControl.Appearance.Options.UseFont = true;
		this.previevOnlyLabelControl.Enabled = false;
		this.previevOnlyLabelControl.Location = new System.Drawing.Point(12, 46);
		this.previevOnlyLabelControl.Name = "previevOnlyLabelControl";
		this.previevOnlyLabelControl.Size = new System.Drawing.Size(310, 16);
		this.previevOnlyLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.previevOnlyLabelControl.TabIndex = 67;
		this.previevOnlyLabelControl.Text = "This data is preview-only, it is not saved to repository!";
		this.tableTitleLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold);
		this.tableTitleLabelControl.Appearance.Options.UseFont = true;
		this.tableTitleLabelControl.AutoEllipsis = true;
		this.tableTitleLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.tableTitleLabelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tableTitleLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftBottom;
		this.tableTitleLabelControl.ImageOptions.Image = Dataedo.App.Properties.Resources.column_16;
		this.tableTitleLabelControl.Location = new System.Drawing.Point(12, 12);
		this.tableTitleLabelControl.Name = "tableTitleLabelControl";
		this.tableTitleLabelControl.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
		this.tableTitleLabelControl.Size = new System.Drawing.Size(776, 30);
		this.tableTitleLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.tableTitleLabelControl.TabIndex = 10;
		this.tableTitleLabelControl.Text = "Object name";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[8] { this.layoutControlItem1, this.layoutControlItem2, this.layoutControlItem3, this.sampleDataGridControlLayoutControlItem, this.layoutControlItem4, this.layoutControlItem5, this.emptySpaceItem1, this.emptySpaceItem2 });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(800, 387);
		this.Root.TextVisible = false;
		this.layoutControlItem1.Control = this.tableTitleLabelControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(780, 34);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem2.Control = this.previevOnlyLabelControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 34);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(780, 20);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlItem3.Control = this.sampleDataTextLabelControl;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 54);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(780, 17);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.sampleDataGridControlLayoutControlItem.Control = this.sampleDataGridControl;
		this.sampleDataGridControlLayoutControlItem.Location = new System.Drawing.Point(0, 71);
		this.sampleDataGridControlLayoutControlItem.Name = "sampleDataGridControlLayoutControlItem";
		this.sampleDataGridControlLayoutControlItem.Size = new System.Drawing.Size(780, 270);
		this.sampleDataGridControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.sampleDataGridControlLayoutControlItem.TextVisible = false;
		this.layoutControlItem4.Control = this.refreshBtn;
		this.layoutControlItem4.Location = new System.Drawing.Point(607, 341);
		this.layoutControlItem4.MaxSize = new System.Drawing.Size(81, 26);
		this.layoutControlItem4.MinSize = new System.Drawing.Size(81, 26);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(81, 26);
		this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.layoutControlItem5.Control = this.closeBtn;
		this.layoutControlItem5.Location = new System.Drawing.Point(698, 341);
		this.layoutControlItem5.MaxSize = new System.Drawing.Size(82, 26);
		this.layoutControlItem5.MinSize = new System.Drawing.Size(82, 26);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(82, 26);
		this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(688, 341);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(10, 0);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(10, 10);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(10, 26);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(0, 341);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(607, 26);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.splashScreenManager1.ClosingDelay = 500;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 387);
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_32;
		base.Name = "SampleDataForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Sample data";
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.sampleDataGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sampleDataGridControlGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sampleDataGridControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		base.ResumeLayout(false);
	}
}
