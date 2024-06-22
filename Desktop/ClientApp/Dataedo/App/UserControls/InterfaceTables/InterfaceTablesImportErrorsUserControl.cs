using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Model.Data.InterfaceTables;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;

namespace Dataedo.App.UserControls.InterfaceTables;

public class InterfaceTablesImportErrorsUserControl : BaseUserControl
{
	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl1;

	private LayoutControlGroup Root;

	private GridControl errorsGridControl;

	private GridView errorsGridView;

	private GridColumn imageGridColumn;

	private GridColumn tableNameGridColumn;

	private GridColumn rowIdGridColumn;

	private GridColumn errorMessageGridColumn;

	private GridColumn ifIgnoredGridColumn;

	private LayoutControlItem layoutControlItem1;

	private EmptySpaceItem emptySpaceItem1;

	private LabelControl warningsCountLabelControl;

	private LabelControl errorCountLabelControl;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem2;

	public InterfaceTablesImportErrorsUserControl()
	{
		InitializeComponent();
	}

	public void SetParameters(List<InterfaceTableErrorObject> interfaceTableErrorObjects)
	{
		errorsGridControl.DataSource = (from x in interfaceTableErrorObjects
			orderby x.IsError descending, x.TableName
			select x).ToList();
		SetCountLabelsText();
	}

	private void SetCountLabelsText()
	{
		if (errorsGridControl.DataSource is List<InterfaceTableErrorObject> source)
		{
			errorCountLabelControl.Text = $"Errors: <b>{source.Count((InterfaceTableErrorObject x) => x.IsError == true)}</b>";
			warningsCountLabelControl.Text = $"Warnings: <b>{source.Count((InterfaceTableErrorObject x) => x.IsError == false)}</b>";
		}
	}

	private void ErrorsGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (e.Column == imageGridColumn && e.RowHandle >= 0)
		{
			bool? flag = (errorsGridView.GetRow(e.RowHandle) as InterfaceTableErrorObject)?.IsError;
			Image image = ((!flag.HasValue) ? null : ((!flag.GetValueOrDefault()) ? Resources.warning_16 : Resources.error_16));
			if (image != null)
			{
				e.Cache.DrawImage(image, new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, image.Width, image.Height));
				e.Handled = true;
			}
		}
	}

	public bool HasAnyErrorsOrWarnings()
	{
		if (errorsGridView.DataRowCount > 0)
		{
			return true;
		}
		return false;
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
		this.warningsCountLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.errorCountLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.errorsGridControl = new DevExpress.XtraGrid.GridControl();
		this.errorsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.imageGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.tableNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.rowIdGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.errorMessageGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.ifIgnoredGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).BeginInit();
		this.nonCustomizableLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.errorsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.errorsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl1.AllowCustomization = false;
		this.nonCustomizableLayoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl1.Controls.Add(this.warningsCountLabelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.errorCountLabelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.errorsGridControl);
		this.nonCustomizableLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl1.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl1.Name = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.BackColor = System.Drawing.Color.LightGray;
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.Font = new System.Drawing.Font("Tahoma", 10.25f);
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.Options.UseBackColor = true;
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.Options.UseFont = true;
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.Options.UseTextOptions = true;
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.nonCustomizableLayoutControl1.Root = this.Root;
		this.nonCustomizableLayoutControl1.Size = new System.Drawing.Size(569, 552);
		this.nonCustomizableLayoutControl1.TabIndex = 0;
		this.nonCustomizableLayoutControl1.Text = "nonCustomizableLayoutControl1";
		this.warningsCountLabelControl.AllowHtmlString = true;
		this.warningsCountLabelControl.Location = new System.Drawing.Point(68, 527);
		this.warningsCountLabelControl.Name = "warningsCountLabelControl";
		this.warningsCountLabelControl.Size = new System.Drawing.Size(48, 13);
		this.warningsCountLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		this.warningsCountLabelControl.TabIndex = 6;
		this.warningsCountLabelControl.Text = "Warnings:";
		this.errorCountLabelControl.AllowHtmlString = true;
		this.errorCountLabelControl.Location = new System.Drawing.Point(12, 527);
		this.errorCountLabelControl.Name = "errorCountLabelControl";
		this.errorCountLabelControl.Size = new System.Drawing.Size(30, 13);
		this.errorCountLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		this.errorCountLabelControl.TabIndex = 5;
		this.errorCountLabelControl.Text = "Errors:";
		this.errorsGridControl.Location = new System.Drawing.Point(12, 12);
		this.errorsGridControl.MainView = this.errorsGridView;
		this.errorsGridControl.Name = "errorsGridControl";
		this.errorsGridControl.Size = new System.Drawing.Size(545, 511);
		this.errorsGridControl.TabIndex = 4;
		this.errorsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.errorsGridView });
		this.errorsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.imageGridColumn, this.tableNameGridColumn, this.rowIdGridColumn, this.errorMessageGridColumn, this.ifIgnoredGridColumn });
		this.errorsGridView.GridControl = this.errorsGridControl;
		this.errorsGridView.Name = "errorsGridView";
		this.errorsGridView.OptionsBehavior.AutoPopulateColumns = false;
		this.errorsGridView.OptionsBehavior.Editable = false;
		this.errorsGridView.OptionsCustomization.AllowFilter = false;
		this.errorsGridView.OptionsCustomization.AllowGroup = false;
		this.errorsGridView.OptionsCustomization.AllowSort = false;
		this.errorsGridView.OptionsView.ShowGroupPanel = false;
		this.errorsGridView.OptionsView.ShowIndicator = false;
		this.errorsGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(ErrorsGridView_CustomDrawCell);
		this.imageGridColumn.Caption = "Image";
		this.imageGridColumn.MaxWidth = 25;
		this.imageGridColumn.Name = "imageGridColumn";
		this.imageGridColumn.OptionsColumn.ShowCaption = false;
		this.imageGridColumn.Visible = true;
		this.imageGridColumn.VisibleIndex = 0;
		this.imageGridColumn.Width = 25;
		this.tableNameGridColumn.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold);
		this.tableNameGridColumn.AppearanceHeader.Options.UseFont = true;
		this.tableNameGridColumn.Caption = "Table";
		this.tableNameGridColumn.FieldName = "TableWithSchema";
		this.tableNameGridColumn.Name = "tableNameGridColumn";
		this.tableNameGridColumn.Visible = true;
		this.tableNameGridColumn.VisibleIndex = 1;
		this.tableNameGridColumn.Width = 127;
		this.rowIdGridColumn.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold);
		this.rowIdGridColumn.AppearanceHeader.Options.UseFont = true;
		this.rowIdGridColumn.Caption = "Row ID";
		this.rowIdGridColumn.FieldName = "RowId";
		this.rowIdGridColumn.MaxWidth = 80;
		this.rowIdGridColumn.Name = "rowIdGridColumn";
		this.rowIdGridColumn.Visible = true;
		this.rowIdGridColumn.VisibleIndex = 2;
		this.rowIdGridColumn.Width = 55;
		this.errorMessageGridColumn.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold);
		this.errorMessageGridColumn.AppearanceHeader.Options.UseFont = true;
		this.errorMessageGridColumn.Caption = "Error";
		this.errorMessageGridColumn.FieldName = "ErrorMessage";
		this.errorMessageGridColumn.Name = "errorMessageGridColumn";
		this.errorMessageGridColumn.Visible = true;
		this.errorMessageGridColumn.VisibleIndex = 3;
		this.errorMessageGridColumn.Width = 172;
		this.ifIgnoredGridColumn.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold);
		this.ifIgnoredGridColumn.AppearanceHeader.Options.UseFont = true;
		this.ifIgnoredGridColumn.Caption = "If Ignored";
		this.ifIgnoredGridColumn.FieldName = "IfIgnoredMessage";
		this.ifIgnoredGridColumn.Name = "ifIgnoredGridColumn";
		this.ifIgnoredGridColumn.Visible = true;
		this.ifIgnoredGridColumn.VisibleIndex = 4;
		this.ifIgnoredGridColumn.Width = 164;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.layoutControlItem1, this.emptySpaceItem1, this.layoutControlItem2, this.layoutControlItem3, this.emptySpaceItem2 });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(569, 552);
		this.Root.TextVisible = false;
		this.layoutControlItem1.Control = this.errorsGridControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(549, 515);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(34, 515);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(22, 0);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(22, 10);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(22, 17);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.Control = this.errorCountLabelControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 515);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(34, 17);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlItem3.Control = this.warningsCountLabelControl;
		this.layoutControlItem3.Location = new System.Drawing.Point(56, 515);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(52, 17);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(108, 515);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(441, 17);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.nonCustomizableLayoutControl1);
		base.Name = "InterfaceTablesImportErrorsUserControl";
		base.Size = new System.Drawing.Size(569, 552);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).EndInit();
		this.nonCustomizableLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.errorsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.errorsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		base.ResumeLayout(false);
	}
}
