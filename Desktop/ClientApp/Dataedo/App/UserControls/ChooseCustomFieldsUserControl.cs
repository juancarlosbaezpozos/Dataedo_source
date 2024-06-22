using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.ExtendedPropertiesExport;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Model.Data.OtherObjects;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls;

public class ChooseCustomFieldsUserControl : BaseUserControl
{
	private const string msDescription = "MS_Description";

	private const string description = "Description";

	private List<IExtendedProperty> extendedPropertiesExportDataSource;

	private IContainer components;

	private NonCustomizableLayoutControl customFieldsLayoutControl;

	private HyperLinkEdit noneCustomFieldsHyperLinkEdit;

	private HyperLinkEdit allCustomFieldsHyperLinkEdit;

	private LayoutControlGroup layoutControlGroup6;

	private LayoutControlItem layoutControlItem13;

	private LayoutControlItem layoutControlItem14;

	private EmptySpaceItem emptySpaceItem7;

	private GridControl customFieldsGridControl;

	private CustomGridUserControl customFieldsGridView;

	private GridColumn selectedCustomFieldsGridColumn;

	private RepositoryItemCheckEdit selectedCustomFieldsRepositoryItemCheckEdit;

	private GridColumn titleCustomFieldsGridColumn;

	private GridColumn customFieldsExtendedPropertyGridColumn;

	private RepositoryItemTextEdit extendedPropertyRepositoryItemTextEdit;

	private LayoutControlItem customFieldsGridLayoutControlItem;

	public List<IExtendedProperty> ExtendedPropertiesExportDataSource => extendedPropertiesExportDataSource;

	public string MSDescription => "MS_Description";

	public string Description => "Description";

	public CustomFieldsSupport CustomFieldsSupport { get; protected set; }

	public OtherFieldsSupport OtherFieldsSupport { get; set; }

	public bool IsExtendedPropertyExport { get; set; }

	public ChooseCustomFieldsUserControl()
	{
		InitializeComponent();
		IsExtendedPropertyExport = false;
		LengthValidation.SetExtendedPropertiesLimit(extendedPropertyRepositoryItemTextEdit);
	}

	public void Initialize(CustomFieldsSupport customFieldsSupport)
	{
		CustomFieldsSupport = customFieldsSupport;
	}

	public void LoadCustomFields(DocFormatEnum.DocFormat format)
	{
		List<SelectWithTitleObject> list = new List<SelectWithTitleObject>();
		if (format == DocFormatEnum.DocFormat.HTML)
		{
			OtherFieldsSupport = new OtherFieldsSupport();
			list.AddRange(OtherFieldsSupport.Fields);
		}
		list.AddRange(CustomFieldsSupport.Fields);
		customFieldsGridControl.DataSource = list;
		extendedPropertiesExportDataSource = new List<IExtendedProperty>();
	}

	public void LoadDocumentationCustomFields(int? databaseId)
	{
		CustomFieldsSupport.LoadDocumentationCustomFields(databaseId);
	}

	public void SetView(DocFormatEnum.DocFormat docFormat)
	{
		customFieldsGridControl.BeginUpdate();
		if (IsExtendedPropertyExport)
		{
			LoadExtendedPropertiesFields();
			customFieldsGridControl.DataSource = extendedPropertiesExportDataSource;
		}
		else
		{
			CustomFieldsSupport.SelectAll();
			LoadCustomFields(docFormat);
		}
		GridColumn gridColumn = customFieldsExtendedPropertyGridColumn;
		bool visible = (customFieldsGridView.OptionsView.ShowColumnHeaders = IsExtendedPropertyExport);
		gridColumn.Visible = visible;
		SetLinesVisibility();
		customFieldsGridControl.EndUpdate();
	}

	public void UpdateCustomFieldsExtendedProperties(IEnumerable<DocumentationCustomFieldRow> customFields, int databaseId)
	{
		DB.CustomField.InsertOrUpdateDocumentationCustomFields(customFields, databaseId);
	}

	public bool ValidateExtendedPropertyColumnValue()
	{
		int dataRowCount = customFieldsGridView.DataRowCount;
		int num = extendedPropertiesExportDataSource.Where((IExtendedProperty x) => x is DocumentationCustomFieldRow).Count();
		for (int i = dataRowCount - num; i < dataRowCount; i++)
		{
			int rowHandle = customFieldsGridView.GetRowHandle(i);
			DocumentationCustomFieldRow documentationCustomFieldRow = customFieldsGridView.GetRow(rowHandle) as DocumentationCustomFieldRow;
			if (documentationCustomFieldRow.IsSelected && string.IsNullOrWhiteSpace(documentationCustomFieldRow.ExtendedProperty))
			{
				return false;
			}
		}
		return true;
	}

	public bool ValidateExtendedPropertySelectedItems()
	{
		return extendedPropertiesExportDataSource.Any((IExtendedProperty x) => x.IsSelected);
	}

	public void ValidateColumns()
	{
		customFieldsGridView.ClearColumnErrors();
		foreach (DocumentationCustomFieldRow documentationCustomField in CustomFieldsSupport.DocumentationCustomFields)
		{
			documentationCustomField.HasEmptyExtendedPropertyField = documentationCustomField.IsSelected && string.IsNullOrEmpty(documentationCustomField.ExtendedProperty);
		}
		customFieldsGridView.RefreshData();
	}

	private void LoadExtendedPropertiesFields()
	{
		extendedPropertiesExportDataSource.Add(new ExtendedPropertyModel
		{
			IsSelected = true,
			Title = "Description",
			ExtendedProperty = "MS_Description"
		});
		extendedPropertiesExportDataSource.AddRange(CustomFieldsSupport.DocumentationCustomFields.Cast<IExtendedProperty>().ToList());
	}

	private void SetLinesVisibility()
	{
		DefaultBoolean defaultBoolean = ((!IsExtendedPropertyExport) ? DefaultBoolean.False : DefaultBoolean.True);
		DefaultBoolean defaultBoolean4 = (customFieldsGridView.OptionsView.ShowHorizontalLines = (customFieldsGridView.OptionsView.ShowVerticalLines = defaultBoolean));
	}

	private void allCustomFieldsHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		customFieldsGridControl.BeginUpdate();
		if (IsExtendedPropertyExport)
		{
			CustomFieldsSupport.SelectAllDocumentationCustomFields();
		}
		else
		{
			OtherFieldsSupport?.SelectAll();
			CustomFieldsSupport.SelectAll();
		}
		customFieldsGridControl.EndUpdate();
	}

	private void noneCustomFieldsHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		customFieldsGridControl.BeginUpdate();
		if (IsExtendedPropertyExport)
		{
			CustomFieldsSupport.UnselectAllDocumentationCustomFields();
		}
		else
		{
			OtherFieldsSupport?.UnselectAll();
			CustomFieldsSupport.UnselectAll();
		}
		customFieldsGridControl.EndUpdate();
	}

	private void selectedCustomFieldsRepositoryItemCheckEdit_EditValueChanged(object sender, EventArgs e)
	{
		customFieldsGridView.CloseEditor();
	}

	private void customFieldsGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (customFieldsGridView.GetRow(e.RowHandle) is IExtendedProperty extendedProperty)
		{
			if (e.RowHandle == 0 && e.CellValue.Equals(MSDescription) && extendedProperty.IsSelected)
			{
				e.Appearance.BackColor = Color.FromArgb(234, 234, 234);
				e.Appearance.ForeColor = Color.Gray;
			}
			else if (extendedProperty.IsSelected)
			{
				e.Appearance.BackColor = Color.Transparent;
				e.Appearance.ForeColor = SkinColors.ControlForeColorFromSystemColors;
			}
			else
			{
				e.Appearance.BackColor = Color.FromArgb(234, 234, 234);
				e.Appearance.ForeColor = Color.Gray;
			}
		}
	}

	private void customFieldsGridView_ShowingEditor(object sender, CancelEventArgs e)
	{
		IExtendedProperty extendedProperty = customFieldsGridView.GetRow(customFieldsGridView.FocusedRowHandle) as IExtendedProperty;
		GridColumn focusedColumn = customFieldsGridView.FocusedColumn;
		if (customFieldsExtendedPropertyGridColumn.Equals(focusedColumn) && !extendedProperty.IsSelected)
		{
			e.Cancel = true;
		}
		else if (customFieldsExtendedPropertyGridColumn.Equals(focusedColumn) && extendedProperty is ExtendedPropertyModel && extendedProperty.IsSelected)
		{
			e.Cancel = true;
		}
	}

	private void customFieldsGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		(customFieldsGridView.GetRow(e.RowHandle) as DocumentationCustomFieldRow).HasEmptyExtendedPropertyField = false;
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
		this.customFieldsLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.noneCustomFieldsHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.allCustomFieldsHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem7 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.customFieldsGridControl = new DevExpress.XtraGrid.GridControl();
		this.customFieldsGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.selectedCustomFieldsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.selectedCustomFieldsRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.titleCustomFieldsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.customFieldsExtendedPropertyGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.extendedPropertyRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.customFieldsGridLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.customFieldsLayoutControl).BeginInit();
		this.customFieldsLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.noneCustomFieldsHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allCustomFieldsHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem13).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem14).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.selectedCustomFieldsRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.extendedPropertyRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsGridLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.customFieldsLayoutControl.AllowCustomization = false;
		this.customFieldsLayoutControl.Controls.Add(this.customFieldsGridControl);
		this.customFieldsLayoutControl.Controls.Add(this.noneCustomFieldsHyperLinkEdit);
		this.customFieldsLayoutControl.Controls.Add(this.allCustomFieldsHyperLinkEdit);
		this.customFieldsLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.customFieldsLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.customFieldsLayoutControl.Name = "customFieldsLayoutControl";
		this.customFieldsLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(902, 291, 808, 578);
		this.customFieldsLayoutControl.OptionsView.UseDefaultDragAndDropRendering = false;
		this.customFieldsLayoutControl.Root = this.layoutControlGroup6;
		this.customFieldsLayoutControl.Size = new System.Drawing.Size(610, 557);
		this.customFieldsLayoutControl.TabIndex = 2;
		this.customFieldsLayoutControl.Text = "layoutControl2";
		this.noneCustomFieldsHyperLinkEdit.EditValue = "None";
		this.noneCustomFieldsHyperLinkEdit.Location = new System.Drawing.Point(27, 2);
		this.noneCustomFieldsHyperLinkEdit.MaximumSize = new System.Drawing.Size(40, 0);
		this.noneCustomFieldsHyperLinkEdit.MinimumSize = new System.Drawing.Size(40, 0);
		this.noneCustomFieldsHyperLinkEdit.Name = "noneCustomFieldsHyperLinkEdit";
		this.noneCustomFieldsHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.noneCustomFieldsHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.noneCustomFieldsHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.noneCustomFieldsHyperLinkEdit.Size = new System.Drawing.Size(40, 18);
		this.noneCustomFieldsHyperLinkEdit.StyleController = this.customFieldsLayoutControl;
		this.noneCustomFieldsHyperLinkEdit.TabIndex = 8;
		this.noneCustomFieldsHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(noneCustomFieldsHyperLinkEdit_OpenLink);
		this.allCustomFieldsHyperLinkEdit.EditValue = "All";
		this.allCustomFieldsHyperLinkEdit.Location = new System.Drawing.Point(2, 2);
		this.allCustomFieldsHyperLinkEdit.MaximumSize = new System.Drawing.Size(25, 0);
		this.allCustomFieldsHyperLinkEdit.MinimumSize = new System.Drawing.Size(25, 0);
		this.allCustomFieldsHyperLinkEdit.Name = "allCustomFieldsHyperLinkEdit";
		this.allCustomFieldsHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.allCustomFieldsHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.allCustomFieldsHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.allCustomFieldsHyperLinkEdit.Size = new System.Drawing.Size(25, 18);
		this.allCustomFieldsHyperLinkEdit.StyleController = this.customFieldsLayoutControl;
		this.allCustomFieldsHyperLinkEdit.TabIndex = 7;
		this.allCustomFieldsHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(allCustomFieldsHyperLinkEdit_OpenLink);
		this.layoutControlGroup6.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup6.GroupBordersVisible = false;
		this.layoutControlGroup6.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.layoutControlItem13, this.layoutControlItem14, this.emptySpaceItem7, this.customFieldsGridLayoutControlItem });
		this.layoutControlGroup6.Name = "Root";
		this.layoutControlGroup6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup6.Size = new System.Drawing.Size(610, 557);
		this.layoutControlGroup6.TextVisible = false;
		this.layoutControlItem13.Control = this.allCustomFieldsHyperLinkEdit;
		this.layoutControlItem13.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem13.MaxSize = new System.Drawing.Size(25, 24);
		this.layoutControlItem13.MinSize = new System.Drawing.Size(25, 24);
		this.layoutControlItem13.Name = "layoutControlItem3";
		this.layoutControlItem13.Size = new System.Drawing.Size(25, 24);
		this.layoutControlItem13.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem13.TextVisible = false;
		this.layoutControlItem14.Control = this.noneCustomFieldsHyperLinkEdit;
		this.layoutControlItem14.Location = new System.Drawing.Point(25, 0);
		this.layoutControlItem14.MaxSize = new System.Drawing.Size(45, 24);
		this.layoutControlItem14.MinSize = new System.Drawing.Size(45, 24);
		this.layoutControlItem14.Name = "layoutControlItem6";
		this.layoutControlItem14.Size = new System.Drawing.Size(45, 24);
		this.layoutControlItem14.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem14.TextVisible = false;
		this.emptySpaceItem7.AllowHotTrack = false;
		this.emptySpaceItem7.Location = new System.Drawing.Point(70, 0);
		this.emptySpaceItem7.Name = "emptySpaceItem4";
		this.emptySpaceItem7.Size = new System.Drawing.Size(540, 24);
		this.emptySpaceItem7.TextSize = new System.Drawing.Size(0, 0);
		this.customFieldsGridControl.Location = new System.Drawing.Point(0, 24);
		this.customFieldsGridControl.MainView = this.customFieldsGridView;
		this.customFieldsGridControl.Margin = new System.Windows.Forms.Padding(0);
		this.customFieldsGridControl.Name = "customFieldsGridControl";
		this.customFieldsGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.selectedCustomFieldsRepositoryItemCheckEdit, this.extendedPropertyRepositoryItemTextEdit });
		this.customFieldsGridControl.Size = new System.Drawing.Size(610, 533);
		this.customFieldsGridControl.TabIndex = 10;
		this.customFieldsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.customFieldsGridView });
		this.customFieldsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.selectedCustomFieldsGridColumn, this.titleCustomFieldsGridColumn, this.customFieldsExtendedPropertyGridColumn });
		this.customFieldsGridView.GridControl = this.customFieldsGridControl;
		this.customFieldsGridView.Name = "customFieldsGridView";
		this.customFieldsGridView.OptionsMenu.EnableColumnMenu = false;
		this.customFieldsGridView.OptionsSelection.MultiSelect = true;
		this.customFieldsGridView.OptionsView.ShowGroupPanel = false;
		this.customFieldsGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
		this.customFieldsGridView.OptionsView.ShowIndicator = false;
		this.customFieldsGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.True;
		this.selectedCustomFieldsGridColumn.Caption = " ";
		this.selectedCustomFieldsGridColumn.ColumnEdit = this.selectedCustomFieldsRepositoryItemCheckEdit;
		this.selectedCustomFieldsGridColumn.FieldName = "IsSelected";
		this.selectedCustomFieldsGridColumn.MaxWidth = 20;
		this.selectedCustomFieldsGridColumn.Name = "selectedCustomFieldsGridColumn";
		this.selectedCustomFieldsGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.selectedCustomFieldsGridColumn.OptionsFilter.AllowFilter = false;
		this.selectedCustomFieldsGridColumn.Visible = true;
		this.selectedCustomFieldsGridColumn.VisibleIndex = 0;
		this.selectedCustomFieldsGridColumn.Width = 20;
		this.selectedCustomFieldsRepositoryItemCheckEdit.AutoHeight = false;
		this.selectedCustomFieldsRepositoryItemCheckEdit.Name = "selectedCustomFieldsRepositoryItemCheckEdit";
		this.titleCustomFieldsGridColumn.Caption = "Field";
		this.titleCustomFieldsGridColumn.FieldName = "Title";
		this.titleCustomFieldsGridColumn.Name = "titleCustomFieldsGridColumn";
		this.titleCustomFieldsGridColumn.OptionsColumn.AllowEdit = false;
		this.titleCustomFieldsGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.titleCustomFieldsGridColumn.OptionsColumn.ReadOnly = true;
		this.titleCustomFieldsGridColumn.OptionsFilter.AllowFilter = false;
		this.titleCustomFieldsGridColumn.Visible = true;
		this.titleCustomFieldsGridColumn.VisibleIndex = 1;
		this.customFieldsExtendedPropertyGridColumn.Caption = "Extended Property";
		this.customFieldsExtendedPropertyGridColumn.ColumnEdit = this.extendedPropertyRepositoryItemTextEdit;
		this.customFieldsExtendedPropertyGridColumn.FieldName = "ExtendedProperty";
		this.customFieldsExtendedPropertyGridColumn.Name = "customFieldsExtendedPropertyGridColumn";
		this.customFieldsExtendedPropertyGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.customFieldsExtendedPropertyGridColumn.OptionsFilter.AllowFilter = false;
		this.customFieldsExtendedPropertyGridColumn.Tag = "ep";
		this.customFieldsExtendedPropertyGridColumn.Visible = true;
		this.customFieldsExtendedPropertyGridColumn.VisibleIndex = 2;
		this.extendedPropertyRepositoryItemTextEdit.AutoHeight = false;
		this.extendedPropertyRepositoryItemTextEdit.Name = "extendedPropertyRepositoryItemTextEdit";
		this.customFieldsGridLayoutControlItem.Control = this.customFieldsGridControl;
		this.customFieldsGridLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.customFieldsGridLayoutControlItem.Name = "customFieldsGridLayoutControlItem";
		this.customFieldsGridLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.customFieldsGridLayoutControlItem.Size = new System.Drawing.Size(610, 533);
		this.customFieldsGridLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.customFieldsGridLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.customFieldsLayoutControl);
		base.Name = "ChooseCustomFieldsUserControl";
		base.Size = new System.Drawing.Size(610, 557);
		((System.ComponentModel.ISupportInitialize)this.customFieldsLayoutControl).EndInit();
		this.customFieldsLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.noneCustomFieldsHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allCustomFieldsHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem13).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem14).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.selectedCustomFieldsRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.extendedPropertyRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsGridLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
