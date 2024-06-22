using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.UserControls.Base;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Dataedo.App.UserControls.WindowControls;

public class AddColumnsUserControl : BaseUserControl
{
	private ColumnBulkHelper bulkHelper;

	private CustomFieldsSupport customFieldsSupport;

	private CustomFieldsCellsTypesSupport customFieldsCellsTypesSupport;

	private IContainer components;

	private GridControl AddColumnsGridControl;

	private CustomGridUserControl addColumnsGridView;

	private GridColumn nameGridColumn;

	private GridColumn titleGridColumn;

	private GridColumn dataTypeGridColumn;

	private GridColumn sizeGridColumn;

	private GridColumn nullableGridColumn;

	private GridColumn defaultValueGridColumn;

	private GridColumn computedFormulaGridColumn;

	private GridColumn identityGridColumn;

	private GridColumn descriptionGridColumn;

	private RepositoryItemPictureEdit repositoryItemPictureEdit1;

	private RepositoryItemTextEdit nameRepositoryItemTextEdit;

	private RepositoryItemTextEdit dataTypeRepositoryItemTextEdit;

	private RepositoryItemTextEdit sizeRepositoryItemTextEdit;

	private RepositoryItemTextEdit titleRepositoryItemTextEdit;

	private SharedObjectTypeEnum.ObjectType objectType { get; set; }

	public int ExistingColumnsCount { get; set; }

	public int TableId { get; set; }

	public bool IsChanged { get; private set; }

	public List<ColumnRow> DataSourceColumns { get; }

	public AddColumnsUserControl()
	{
		InitializeComponent();
		bulkHelper = new BulkColumnsHelper();
		LengthValidation.SetDataSize(sizeRepositoryItemTextEdit);
		LengthValidation.SetDataTypeLength(dataTypeRepositoryItemTextEdit);
		LengthValidation.SetTitleOrNameLengthLimit(nameRepositoryItemTextEdit);
		LengthValidation.SetTitleOrNameLengthLimit(titleRepositoryItemTextEdit);
		customFieldsCellsTypesSupport = new CustomFieldsCellsTypesSupport(isForSummaryTable: false);
		DataSourceColumns = new List<ColumnRow>();
	}

	public void Initialize(SharedObjectTypeEnum.ObjectType objectType, CustomFieldsSupport customFieldsSupport)
	{
		this.objectType = objectType;
		this.customFieldsSupport = customFieldsSupport;
		customFieldsCellsTypesSupport.SetCustomFields(this.customFieldsSupport, new Dictionary<SharedObjectTypeEnum.ObjectType, GridView> { [objectType] = addColumnsGridView });
		if (objectType == SharedObjectTypeEnum.ObjectType.Structure)
		{
			bulkHelper = new BulkObjectColumnsHelper();
			GridColumn gridColumn = defaultValueGridColumn;
			OptionsColumn optionsColumn = defaultValueGridColumn.OptionsColumn;
			GridColumn gridColumn2 = computedFormulaGridColumn;
			OptionsColumn optionsColumn2 = computedFormulaGridColumn.OptionsColumn;
			GridColumn gridColumn3 = identityGridColumn;
			bool flag2 = (identityGridColumn.OptionsColumn.ShowInCustomizationForm = false);
			bool flag4 = (gridColumn3.Visible = flag2);
			bool flag6 = (optionsColumn2.ShowInCustomizationForm = flag4);
			bool flag8 = (gridColumn2.Visible = flag6);
			bool visible = (optionsColumn.ShowInCustomizationForm = flag8);
			gridColumn.Visible = visible;
		}
		else
		{
			bulkHelper = new BulkColumnsHelper();
		}
		bulkHelper.SetTemplate(customFieldsSupport, objectType);
	}

	public void AddBulkColumns(string data)
	{
		string[] array = data.Split('\r', '\t');
		if (IsEmptyLine(array) || string.IsNullOrEmpty(data))
		{
			return;
		}
		int num = 0;
		ColumnRow columnRow = new ColumnRow();
		columnRow.CustomFields = new CustomFieldContainer(customFieldsSupport);
		columnRow.CustomFields.RetrieveCustomFields();
		columnRow.Name = ShortenedString(array[num], 80);
		columnRow.DataType = ((++num < array.Length) ? ShortenedString(array[num], 250) : null);
		if (columnRow.DataType != null && columnRow.DataType.Equals(string.Empty))
		{
			columnRow.DataType = null;
		}
		columnRow.DataTypeWithoutLength = columnRow.DataType;
		columnRow.DataLength = ((++num >= array.Length) ? null : (string.IsNullOrEmpty(array[num]) ? null : ShortenedString(array[num], 50)));
		columnRow.Nullable = ++num < array.Length && CheckCheckBoxColumnsValues(array[num]);
		if (objectType != SharedObjectTypeEnum.ObjectType.Structure)
		{
			columnRow.DefaultValue = ((++num < array.Length) ? array[num] : string.Empty);
			columnRow.ComputedFormula = ((++num < array.Length) ? array[num] : string.Empty);
			columnRow.IsIdentity = ++num < array.Length && CheckCheckBoxColumnsValues(array[num]);
		}
		columnRow.Title = ((++num < array.Length) ? ShortenedString(array[num], 80) : string.Empty);
		columnRow.Description = ((++num < array.Length) ? array[num] : string.Empty);
		columnRow.Source = UserTypeEnum.UserType.USER;
		columnRow.TableId = TableId;
		columnRow.RowState = ManagingRowsEnum.ManagingRows.ForAdding;
		foreach (CustomFieldRowExtended field in customFieldsSupport.GetVisibleFields(objectType))
		{
			num++;
			if (field.IsDomainValueType && field.IsValueProperForDomainValuesType((num < array.Length) ? array[num] : string.Empty))
			{
				object obj = ((num < array.Length) ? field.GetPreparedValueForDomainValuesType(array[num]) : string.Empty);
				columnRow.CustomFields.SetCustomFieldValue(field.FieldName, obj?.ToString());
			}
			else
			{
				if (field.IsDomainValueType)
				{
					continue;
				}
				string value = ((num < array.Length) ? array[num] : string.Empty);
				columnRow.CustomFields.SetCustomFieldValue(field.FieldName, value);
				if (field != null && field.IsOpenDefinitionType)
				{
					field.UpdateAddedDefinitionSingleValue(value);
					CustomFieldsRepositoryItems.RefreshEditOpenValues(addColumnsGridView.Columns.FirstOrDefault((GridColumn x) => x.FieldName?.ToLower().Equals(field.FieldName.ToLower()) ?? false).ColumnEdit, field);
				}
			}
		}
		DataSourceColumns.Add(columnRow);
	}

	public void Paste()
	{
		if (string.IsNullOrEmpty(bulkHelper.ClipboardData))
		{
			return;
		}
		List<string> list = bulkHelper.ClipboardData.Split('\n').ToList();
		if (list.FirstOrDefault().Contains(bulkHelper.Template))
		{
			list.Remove(list.FirstOrDefault());
		}
		foreach (string item in list)
		{
			if (DataSourceColumns.Count + ExistingColumnsCount == 1000)
			{
				break;
			}
			AddBulkColumns(item);
		}
		UpdateDataSource();
		IsChanged = DataSourceColumns.Count > 0;
		if (DataSourceColumns.Count + ExistingColumnsCount == 1000)
		{
			string text = SharedObjectTypeEnum.TypeToStringForMenu(objectType);
			GeneralMessageBoxesHandling.Show("Manual " + text + " can store up to 1000 columns. Only columns below that amount were pasted.", "Bulk add columns", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, FindForm());
		}
	}

	public void CopyTemplate()
	{
		bulkHelper.CopyTemplate();
	}

	public void Remove()
	{
		if (!addColumnsGridView.IsEditing)
		{
			List<ColumnRow> list = new List<ColumnRow>();
			int[] selectedRows = addColumnsGridView.GetSelectedRows();
			foreach (int rowHandle in selectedRows)
			{
				ColumnRow item = addColumnsGridView.GetRow(rowHandle) as ColumnRow;
				list.Add(item);
			}
			list.ForEach(delegate(ColumnRow x)
			{
				DataSourceColumns.Remove(x);
			});
			if (addColumnsGridView.FocusedRowHandle >= DataSourceColumns.Count - 1)
			{
				addColumnsGridView.MoveLast();
			}
			UpdateDataSource();
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == (Keys.V | Keys.Control) && !addColumnsGridView.IsEditing)
		{
			Paste();
			return true;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private bool IsEmptyLine(string[] data)
	{
		return bulkHelper.IsEmptyLine(data);
	}

	private string ShortenedString(string value, int length)
	{
		return new string(value.Take(length).ToArray());
	}

	private bool CheckCheckBoxColumnsValues(string data)
	{
		return bulkHelper.CheckCheckBoxColumnsValues(data);
	}

	private void UpdateDataSource()
	{
		addColumnsGridView.BeginUpdate();
		AddColumnsGridControl.DataSource = DataSourceColumns;
		addColumnsGridView.EndUpdate();
	}

	private void AddColumnsGridView_MouseDown(object sender, MouseEventArgs e)
	{
		GridHitInfo gridHitInfo = addColumnsGridView.CalcHitInfo(e.Location);
		if (gridHitInfo.InRow && gridHitInfo.Column.RealColumnEdit is RepositoryItemCheckEdit)
		{
			addColumnsGridView.FocusedColumn = gridHitInfo.Column;
			addColumnsGridView.FocusedRowHandle = gridHitInfo.RowHandle;
			addColumnsGridView.ShowEditor();
			if (addColumnsGridView.ActiveEditor is CheckEdit checkEdit)
			{
				checkEdit.Toggle();
			}
		}
	}

	private void addColumnsGridView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
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
		this.AddColumnsGridControl = new DevExpress.XtraGrid.GridControl();
		this.addColumnsGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.nameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.titleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.titleRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.dataTypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.dataTypeRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.sizeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.sizeRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.nullableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.defaultValueGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.computedFormulaGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.identityGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.descriptionGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemPictureEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		((System.ComponentModel.ISupportInitialize)this.AddColumnsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.addColumnsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataTypeRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sizeRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemPictureEdit1).BeginInit();
		base.SuspendLayout();
		this.AddColumnsGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.AddColumnsGridControl.Location = new System.Drawing.Point(0, 0);
		this.AddColumnsGridControl.MainView = this.addColumnsGridView;
		this.AddColumnsGridControl.Name = "AddColumnsGridControl";
		this.AddColumnsGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.repositoryItemPictureEdit1, this.nameRepositoryItemTextEdit, this.dataTypeRepositoryItemTextEdit, this.sizeRepositoryItemTextEdit, this.titleRepositoryItemTextEdit });
		this.AddColumnsGridControl.ShowOnlyPredefinedDetails = true;
		this.AddColumnsGridControl.Size = new System.Drawing.Size(886, 347);
		this.AddColumnsGridControl.TabIndex = 1;
		this.AddColumnsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.addColumnsGridView });
		this.addColumnsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.nameGridColumn, this.titleGridColumn, this.dataTypeGridColumn, this.sizeGridColumn, this.nullableGridColumn, this.defaultValueGridColumn, this.computedFormulaGridColumn, this.identityGridColumn, this.descriptionGridColumn });
		this.addColumnsGridView.GridControl = this.AddColumnsGridControl;
		this.addColumnsGridView.Name = "addColumnsGridView";
		this.addColumnsGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.addColumnsGridView.OptionsFilter.AllowFilterEditor = false;
		this.addColumnsGridView.OptionsSelection.MultiSelect = true;
		this.addColumnsGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.addColumnsGridView.OptionsView.ColumnAutoWidth = false;
		this.addColumnsGridView.OptionsView.RowAutoHeight = true;
		this.addColumnsGridView.OptionsView.ShowGroupPanel = false;
		this.addColumnsGridView.OptionsView.ShowIndicator = false;
		this.addColumnsGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(addColumnsGridView_PopupMenuShowing);
		this.addColumnsGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(AddColumnsGridView_MouseDown);
		this.nameGridColumn.Caption = "Name";
		this.nameGridColumn.ColumnEdit = this.nameRepositoryItemTextEdit;
		this.nameGridColumn.FieldName = "Name";
		this.nameGridColumn.MinWidth = 97;
		this.nameGridColumn.Name = "nameGridColumn";
		this.nameGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.nameGridColumn.Visible = true;
		this.nameGridColumn.VisibleIndex = 0;
		this.nameGridColumn.Width = 97;
		this.nameRepositoryItemTextEdit.AutoHeight = false;
		this.nameRepositoryItemTextEdit.Name = "nameRepositoryItemTextEdit";
		this.titleGridColumn.Caption = "Title";
		this.titleGridColumn.ColumnEdit = this.titleRepositoryItemTextEdit;
		this.titleGridColumn.FieldName = "Title";
		this.titleGridColumn.MinWidth = 64;
		this.titleGridColumn.Name = "titleGridColumn";
		this.titleGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.titleGridColumn.Visible = true;
		this.titleGridColumn.VisibleIndex = 7;
		this.titleGridColumn.Width = 64;
		this.titleRepositoryItemTextEdit.AutoHeight = false;
		this.titleRepositoryItemTextEdit.Name = "titleRepositoryItemTextEdit";
		this.dataTypeGridColumn.Caption = "Data type";
		this.dataTypeGridColumn.ColumnEdit = this.dataTypeRepositoryItemTextEdit;
		this.dataTypeGridColumn.FieldName = "DataTypeWithoutLength";
		this.dataTypeGridColumn.MinWidth = 70;
		this.dataTypeGridColumn.Name = "dataTypeGridColumn";
		this.dataTypeGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.dataTypeGridColumn.Visible = true;
		this.dataTypeGridColumn.VisibleIndex = 1;
		this.dataTypeGridColumn.Width = 70;
		this.dataTypeRepositoryItemTextEdit.AutoHeight = false;
		this.dataTypeRepositoryItemTextEdit.Name = "dataTypeRepositoryItemTextEdit";
		this.sizeGridColumn.Caption = "Size";
		this.sizeGridColumn.ColumnEdit = this.sizeRepositoryItemTextEdit;
		this.sizeGridColumn.FieldName = "DataLength";
		this.sizeGridColumn.MinWidth = 37;
		this.sizeGridColumn.Name = "sizeGridColumn";
		this.sizeGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.sizeGridColumn.Visible = true;
		this.sizeGridColumn.VisibleIndex = 2;
		this.sizeGridColumn.Width = 37;
		this.sizeRepositoryItemTextEdit.AutoHeight = false;
		this.sizeRepositoryItemTextEdit.Name = "sizeRepositoryItemTextEdit";
		this.nullableGridColumn.Caption = "Nullable";
		this.nullableGridColumn.FieldName = "Nullable";
		this.nullableGridColumn.MinWidth = 46;
		this.nullableGridColumn.Name = "nullableGridColumn";
		this.nullableGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.nullableGridColumn.Visible = true;
		this.nullableGridColumn.VisibleIndex = 3;
		this.nullableGridColumn.Width = 46;
		this.defaultValueGridColumn.Caption = "Default value";
		this.defaultValueGridColumn.FieldName = "DefaultValue";
		this.defaultValueGridColumn.MinWidth = 102;
		this.defaultValueGridColumn.Name = "defaultValueGridColumn";
		this.defaultValueGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.defaultValueGridColumn.Visible = true;
		this.defaultValueGridColumn.VisibleIndex = 4;
		this.defaultValueGridColumn.Width = 102;
		this.computedFormulaGridColumn.Caption = "Computed formula";
		this.computedFormulaGridColumn.FieldName = "ComputedFormula";
		this.computedFormulaGridColumn.MinWidth = 107;
		this.computedFormulaGridColumn.Name = "computedFormulaGridColumn";
		this.computedFormulaGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.computedFormulaGridColumn.Visible = true;
		this.computedFormulaGridColumn.VisibleIndex = 5;
		this.computedFormulaGridColumn.Width = 107;
		this.identityGridColumn.Caption = "Identity";
		this.identityGridColumn.FieldName = "IsIdentity";
		this.identityGridColumn.MinWidth = 60;
		this.identityGridColumn.Name = "identityGridColumn";
		this.identityGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.identityGridColumn.Visible = true;
		this.identityGridColumn.VisibleIndex = 6;
		this.identityGridColumn.Width = 60;
		this.descriptionGridColumn.Caption = "Description";
		this.descriptionGridColumn.FieldName = "Description";
		this.descriptionGridColumn.Name = "descriptionGridColumn";
		this.descriptionGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.descriptionGridColumn.Visible = true;
		this.descriptionGridColumn.VisibleIndex = 8;
		this.descriptionGridColumn.Width = 300;
		this.repositoryItemPictureEdit1.Name = "repositoryItemPictureEdit1";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.AddColumnsGridControl);
		base.Name = "AddColumnsUserControl";
		base.Size = new System.Drawing.Size(886, 347);
		((System.ComponentModel.ISupportInitialize)this.AddColumnsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.addColumnsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataTypeRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sizeRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemPictureEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
