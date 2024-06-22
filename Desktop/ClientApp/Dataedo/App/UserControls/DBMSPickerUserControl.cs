using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.Licences;
using Dataedo.App.Tools;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Interfaces;
using Dataedo.CustomControls;
using Dataedo.Shared.DatabasesSupport;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;

namespace Dataedo.App.UserControls;

public class DBMSPickerUserControl : UserControl, ISelectImportControl
{
	private int? databaseId;

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl1;

	private LayoutControlGroup Root;

	private GridControl dbmsGridControl;

	private GridView dbmsGridView;

	private GridColumn dbmsIconGridColumn;

	private GridColumn dbmsNameGridColumn;

	private LayoutControlItem layoutControlItem2;

	private GridColumn typeGridColumn;

	private GridColumn upgradeUrlColumn;

	private RepositoryItemHypertextLabel LabelURL;

	public SharedDatabaseTypeEnum.DatabaseType? ParentDatabaseType { get; set; }

	public DBMSGridModel DBMSGridModel { get; private set; }

	public SharedDatabaseTypeEnum.DatabaseType? SelectedDatabaseType { get; set; }

	public bool IsDBAdded { get; private set; }

	public event EventHandler DbmsGridViewDoubleClick;

	public event EventHandler FocusedDatabaseTypeChanged;

	public DBMSPickerUserControl()
	{
		InitializeComponent();
	}

	public void SetParameters(int? databaseId = null, SharedDatabaseTypeEnum.DatabaseType? databaseType = null, bool showOnlyDatabaseSubtypesItems = false)
	{
		this.databaseId = databaseId;
		ParentDatabaseType = null;
		if (!showOnlyDatabaseSubtypesItems)
		{
			DatabaseTypes.CreateDatabaseTypesDataSource();
			dbmsGridControl.DataSource = DatabaseTypes.databaseTypesDataSource;
		}
		else
		{
			DatabaseTypes.CreateDatabaseSubtypesDataSource(DatabaseSupportFactory.GetParentDatabaseType(databaseType));
			dbmsGridControl.DataSource = DatabaseTypes.databaseSubtypesDataSource;
			ParentDatabaseType = databaseType;
		}
		if (!databaseId.HasValue)
		{
			IsDBAdded = true;
			return;
		}
		IsDBAdded = false;
		SelectedDatabaseType = databaseType;
		if (DatabaseSupportFactoryShared.CheckIfTypeIsSupported(databaseType))
		{
			List<DBMSGridModel> list = dbmsGridView.DataSource as List<DBMSGridModel>;
			DBMSGridModel item = list.FirstOrDefault((DBMSGridModel x) => x.Type.Equals(DatabaseTypeEnum.TypeToString(databaseType)));
			dbmsGridView.FocusedRowHandle = list.IndexOf(item);
			dbmsGridView.Focus();
		}
	}

	private void dbmsGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		DBMSGridModel = dbmsGridView.GetRow(e.FocusedRowHandle) as DBMSGridModel;
		SelectedDatabaseType = (string.IsNullOrWhiteSpace(DBMSGridModel.Type) ? null : DatabaseTypeEnum.StringToType(DBMSGridModel.Type));
		this.FocusedDatabaseTypeChanged?.Invoke(sender, e);
	}

	public DBMSGridModel GetFocusedDBMSGridModel()
	{
		DBMSGridModel dBMSGridModel = dbmsGridView.GetFocusedRow() as DBMSGridModel;
		SelectedDatabaseType = (string.IsNullOrWhiteSpace(dBMSGridModel?.Type) ? null : DatabaseTypeEnum.StringToType(dBMSGridModel.Type));
		return dBMSGridModel;
	}

	private void dbmsGridView_RowStyle(object sender, RowStyleEventArgs e)
	{
		DontShowManualDataSource();
		if (!Connectors.HasAllConnectors() && e.RowHandle >= 0 && sender is GridView gridView && gridView.GetRow(e.RowHandle) is DBMSGridModel dBMSGridModel)
		{
			SharedDatabaseTypeEnum.DatabaseType? databaseType = SharedDatabaseTypeEnum.StringToType(dBMSGridModel.Type);
			if (databaseType.HasValue && !Connectors.HasDatabaseTypeConnector(databaseType.Value))
			{
				e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridDisabledGridRowForeColor;
				e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
			}
		}
	}

	private void DontShowManualDataSource()
	{
		List<DBMSGridModel> list = dbmsGridView?.DataSource as List<DBMSGridModel>;
		if (list?.FirstOrDefault((DBMSGridModel x) => x.Type.Equals(DatabaseTypeEnum.TypeToString(SharedDatabaseTypeEnum.DatabaseType.Manual))) != null && list.Count > 0)
		{
			dbmsGridView.DeleteRow(list.Count - 1);
			dbmsGridView.FocusedRowHandle = 0;
		}
	}

	private void dbmsGridView_MouseDown(object sender, MouseEventArgs e)
	{
		if (!Connectors.HasAllConnectors() && sender is GridView gridView)
		{
			GridHitInfo gridHitInfo = gridView.CalcHitInfo(e.Location);
			if (gridHitInfo != null && gridHitInfo.RowHandle >= 0 && gridView.GetRow(gridHitInfo.RowHandle) is DBMSGridModel dBMSGridModel && gridHitInfo.Column.FieldName == "URL" && !Connectors.HasDatabaseTypeConnector(DatabaseTypeEnum.StringToType(dBMSGridModel.Type).Value))
			{
				Links.OpenLink(Links.ManageAccounts);
			}
		}
	}

	private void dbmsGridView_DoubleClick(object sender, EventArgs e)
	{
		if (e is DXMouseEventArgs dXMouseEventArgs)
		{
			GridHitInfo gridHitInfo = dbmsGridView.CalcHitInfo(dXMouseEventArgs.Location);
			if (gridHitInfo != null && gridHitInfo.InRow)
			{
				this.DbmsGridViewDoubleClick?.Invoke(sender, e);
			}
		}
	}

	private void DbmsGridView_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Tab)
		{
			if (e.Modifiers == Keys.Shift)
			{
				dbmsGridView.MovePrev();
			}
			else
			{
				dbmsGridView.MoveNext();
			}
			e.Handled = true;
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
		this.nonCustomizableLayoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.dbmsGridControl = new DevExpress.XtraGrid.GridControl();
		this.dbmsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.dbmsIconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.dbmsNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.typeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.upgradeUrlColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.LabelURL = new DevExpress.XtraEditors.Repository.RepositoryItemHypertextLabel();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).BeginInit();
		this.nonCustomizableLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dbmsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.LabelURL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl1.AllowCustomization = false;
		this.nonCustomizableLayoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl1.Controls.Add(this.dbmsGridControl);
		this.nonCustomizableLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl1.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl1.Name = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.Root = this.Root;
		this.nonCustomizableLayoutControl1.Size = new System.Drawing.Size(519, 413);
		this.nonCustomizableLayoutControl1.TabIndex = 0;
		this.nonCustomizableLayoutControl1.Text = "nonCustomizableLayoutControl1";
		this.dbmsGridControl.Location = new System.Drawing.Point(12, 12);
		this.dbmsGridControl.MainView = this.dbmsGridView;
		this.dbmsGridControl.Name = "dbmsGridControl";
		this.dbmsGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.LabelURL });
		this.dbmsGridControl.Size = new System.Drawing.Size(495, 389);
		this.dbmsGridControl.TabIndex = 27;
		this.dbmsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.dbmsGridView });
		this.dbmsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.dbmsIconGridColumn, this.dbmsNameGridColumn, this.typeGridColumn, this.upgradeUrlColumn });
		this.dbmsGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.dbmsGridView.GridControl = this.dbmsGridControl;
		this.dbmsGridView.Name = "dbmsGridView";
		this.dbmsGridView.OptionsBehavior.AutoPopulateColumns = false;
		this.dbmsGridView.OptionsBehavior.Editable = false;
		this.dbmsGridView.OptionsBehavior.ReadOnly = true;
		this.dbmsGridView.OptionsCustomization.AllowFilter = false;
		this.dbmsGridView.OptionsDetail.EnableMasterViewMode = false;
		this.dbmsGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.dbmsGridView.OptionsView.ShowColumnHeaders = false;
		this.dbmsGridView.OptionsView.ShowGroupPanel = false;
		this.dbmsGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.dbmsGridView.OptionsView.ShowIndicator = false;
		this.dbmsGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.dbmsGridView.RowHeight = 25;
		this.dbmsGridView.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(dbmsGridView_RowStyle);
		this.dbmsGridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(dbmsGridView_FocusedRowChanged);
		this.dbmsGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(DbmsGridView_KeyDown);
		this.dbmsGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(dbmsGridView_MouseDown);
		this.dbmsGridView.DoubleClick += new System.EventHandler(dbmsGridView_DoubleClick);
		this.dbmsIconGridColumn.FieldName = "Image";
		this.dbmsIconGridColumn.MaxWidth = 20;
		this.dbmsIconGridColumn.Name = "dbmsIconGridColumn";
		this.dbmsIconGridColumn.Visible = true;
		this.dbmsIconGridColumn.VisibleIndex = 0;
		this.dbmsIconGridColumn.Width = 20;
		this.dbmsNameGridColumn.FieldName = "Name";
		this.dbmsNameGridColumn.Name = "dbmsNameGridColumn";
		this.dbmsNameGridColumn.Visible = true;
		this.dbmsNameGridColumn.VisibleIndex = 1;
		this.dbmsNameGridColumn.Width = 348;
		this.typeGridColumn.FieldName = "Type";
		this.typeGridColumn.Name = "typeGridColumn";
		this.upgradeUrlColumn.Caption = "gridColumn1";
		this.upgradeUrlColumn.ColumnEdit = this.LabelURL;
		this.upgradeUrlColumn.FieldName = "URL";
		this.upgradeUrlColumn.MinWidth = 97;
		this.upgradeUrlColumn.Name = "upgradeUrlColumn";
		this.upgradeUrlColumn.Visible = true;
		this.upgradeUrlColumn.VisibleIndex = 2;
		this.upgradeUrlColumn.Width = 97;
		this.LabelURL.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.LabelURL.Name = "LabelURL";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem2 });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(519, 413);
		this.Root.TextVisible = false;
		this.layoutControlItem2.Control = this.dbmsGridControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(499, 393);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.nonCustomizableLayoutControl1);
		base.Name = "DBMSPickerUserControl";
		base.Size = new System.Drawing.Size(519, 413);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).EndInit();
		this.nonCustomizableLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dbmsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.LabelURL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		base.ResumeLayout(false);
	}
}
