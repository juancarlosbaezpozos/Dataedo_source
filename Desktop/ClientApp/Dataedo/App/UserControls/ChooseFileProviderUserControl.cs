using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Import.CloudStorage;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls;

public class ChooseFileProviderUserControl : BaseUserControl
{
	private IContainer components;

	private NonCustomizableLayoutControl objectTypesLayoutControl;

	private LayoutControlGroup layoutControlGroup3;

	private GridControl fileProviderGridControl;

	private GridView fileProviderGridView;

	private GridColumn iconGridColumn;

	private GridColumn nameGridColumn;

	private GridColumn urlGridColumn;

	private RepositoryItemButtonEdit UrlRepositoryItemButtonEdit;

	private GridColumn emptyGridColumn;

	private LayoutControlItem layoutControlItem1;

	public ChooseFileProviderUserControl()
	{
		InitializeComponent();
	}

	public void Init()
	{
		fileProviderGridControl.DataSource = FileImportTypeEnum.GetFileImportTypeObjects().ToList();
		fileProviderGridView.BestFitColumns();
	}

	public FileImportTypeObject GetSelectedObject()
	{
		return fileProviderGridView.GetFocusedRow() as FileImportTypeObject;
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
		this.objectTypesLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.fileProviderGridControl = new DevExpress.XtraGrid.GridControl();
		this.fileProviderGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.iconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.urlGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.UrlRepositoryItemButtonEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.emptyGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.objectTypesLayoutControl).BeginInit();
		this.objectTypesLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.fileProviderGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fileProviderGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.UrlRepositoryItemButtonEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		base.SuspendLayout();
		this.objectTypesLayoutControl.AllowCustomization = false;
		this.objectTypesLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.objectTypesLayoutControl.Controls.Add(this.fileProviderGridControl);
		this.objectTypesLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.objectTypesLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.objectTypesLayoutControl.Name = "objectTypesLayoutControl";
		this.objectTypesLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(748, 241, 808, 578);
		this.objectTypesLayoutControl.OptionsView.UseDefaultDragAndDropRendering = false;
		this.objectTypesLayoutControl.Padding = new System.Windows.Forms.Padding(1);
		this.objectTypesLayoutControl.Root = this.layoutControlGroup3;
		this.objectTypesLayoutControl.Size = new System.Drawing.Size(519, 385);
		this.objectTypesLayoutControl.TabIndex = 1;
		this.objectTypesLayoutControl.Text = "layoutControl2";
		this.fileProviderGridControl.Location = new System.Drawing.Point(2, 2);
		this.fileProviderGridControl.MainView = this.fileProviderGridView;
		this.fileProviderGridControl.Name = "fileProviderGridControl";
		this.fileProviderGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.UrlRepositoryItemButtonEdit });
		this.fileProviderGridControl.Size = new System.Drawing.Size(515, 381);
		this.fileProviderGridControl.TabIndex = 2;
		this.fileProviderGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.fileProviderGridView });
		this.fileProviderGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.iconGridColumn, this.nameGridColumn, this.urlGridColumn, this.emptyGridColumn });
		this.fileProviderGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.fileProviderGridView.GridControl = this.fileProviderGridControl;
		this.fileProviderGridView.Name = "fileProviderGridView";
		this.fileProviderGridView.OptionsBehavior.AutoPopulateColumns = false;
		this.fileProviderGridView.OptionsBehavior.Editable = false;
		this.fileProviderGridView.OptionsBehavior.ReadOnly = true;
		this.fileProviderGridView.OptionsCustomization.AllowFilter = false;
		this.fileProviderGridView.OptionsDetail.EnableMasterViewMode = false;
		this.fileProviderGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.fileProviderGridView.OptionsView.BestFitMode = DevExpress.XtraGrid.Views.Grid.GridBestFitMode.Fast;
		this.fileProviderGridView.OptionsView.ShowColumnHeaders = false;
		this.fileProviderGridView.OptionsView.ShowGroupPanel = false;
		this.fileProviderGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.fileProviderGridView.OptionsView.ShowIndicator = false;
		this.fileProviderGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.iconGridColumn.Caption = " ";
		this.iconGridColumn.FieldName = "Image";
		this.iconGridColumn.MaxWidth = 30;
		this.iconGridColumn.Name = "iconGridColumn";
		this.iconGridColumn.OptionsColumn.ShowCaption = false;
		this.iconGridColumn.Visible = true;
		this.iconGridColumn.VisibleIndex = 0;
		this.iconGridColumn.Width = 20;
		this.nameGridColumn.Caption = " ";
		this.nameGridColumn.FieldName = "DisplayName";
		this.nameGridColumn.MinWidth = 100;
		this.nameGridColumn.Name = "nameGridColumn";
		this.nameGridColumn.OptionsColumn.ShowCaption = false;
		this.nameGridColumn.Visible = true;
		this.nameGridColumn.VisibleIndex = 1;
		this.nameGridColumn.Width = 100;
		this.urlGridColumn.Caption = " ";
		this.urlGridColumn.ColumnEdit = this.UrlRepositoryItemButtonEdit;
		this.urlGridColumn.FieldName = "URL";
		this.urlGridColumn.MaxWidth = 120;
		this.urlGridColumn.MinWidth = 120;
		this.urlGridColumn.Name = "urlGridColumn";
		this.urlGridColumn.OptionsColumn.ShowCaption = false;
		this.urlGridColumn.Visible = true;
		this.urlGridColumn.VisibleIndex = 2;
		this.urlGridColumn.Width = 120;
		this.UrlRepositoryItemButtonEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.UrlRepositoryItemButtonEdit.AutoHeight = false;
		this.UrlRepositoryItemButtonEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.UrlRepositoryItemButtonEdit.Name = "UrlRepositoryItemButtonEdit";
		this.emptyGridColumn.Caption = "gridColumn1";
		this.emptyGridColumn.Name = "emptyGridColumn";
		this.emptyGridColumn.Visible = true;
		this.emptyGridColumn.VisibleIndex = 3;
		this.emptyGridColumn.Width = 429;
		this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup3.GroupBordersVisible = false;
		this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem1 });
		this.layoutControlGroup3.Name = "Root";
		this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup3.Size = new System.Drawing.Size(519, 385);
		this.layoutControlGroup3.TextVisible = false;
		this.layoutControlItem1.Control = this.fileProviderGridControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(519, 385);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Controls.Add(this.objectTypesLayoutControl);
		base.Name = "ChooseFileProviderUserControl";
		base.Size = new System.Drawing.Size(519, 385);
		((System.ComponentModel.ISupportInitialize)this.objectTypesLayoutControl).EndInit();
		this.objectTypesLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.fileProviderGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fileProviderGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.UrlRepositoryItemButtonEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		base.ResumeLayout(false);
	}
}
