using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classification.UserControls.Classes;
using Dataedo.App.Helpers.Controls;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.DataProcessing.Synchronize.Classes;
using Dataedo.Model.Enums;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Classification.UserControls;

public class ClassificationFieldsUserControl : UserControl
{
	public delegate void ClassificationFieldNameChangedHandler();

	private ClassificatorModelRow classificatorModelRow;

	private List<CustomFieldClassRow> customFieldsClasses;

	private IEnumerable<string> takenCustomFieldsTitles;

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private GridControl gridControl;

	private GridView gridView;

	private LayoutControlItem layoutControlItem;

	private GridColumn nameGridColumn;

	private GridColumn classGridColumn;

	private GridColumn fieldTypeGridColumn;

	private GridColumn labelsFromMasksGridColumn;

	private GridColumn additionalLabelsGridColumn;

	private LabelControl maxInfoLabelControl;

	private LayoutControlItem maxInfoLyoutControlItem;

	private RepositoryItemLookUpEdit classRepositoryItemLookUpEdit;

	private RepositoryItemTextEdit additionalLabelsRepositoryItemTextEdit;

	[Browsable(true)]
	public event ClassificationFieldNameChangedHandler ClassificationFieldNameChanged;

	public ClassificationFieldsUserControl()
	{
		InitializeComponent();
	}

	public void SetParameters(ClassificatorModelRow classificatorModelRow, List<CustomFieldClassRow> customFieldsClasses, IEnumerable<string> takenCustomFieldsTitles)
	{
		this.customFieldsClasses = customFieldsClasses;
		this.classificatorModelRow = classificatorModelRow;
		this.takenCustomFieldsTitles = takenCustomFieldsTitles;
		gridControl.DataSource = classificatorModelRow.Fields;
		gridView.ActiveFilter.NonColumnFilter = new BinaryOperator("RowState", ManagingRowsEnum.ManagingRows.Deleted, BinaryOperatorType.NotEqual).ToString();
		classRepositoryItemLookUpEdit.DataSource = this.customFieldsClasses;
		classRepositoryItemLookUpEdit.DropDownRows = this.customFieldsClasses.Count();
	}

	public void FieldAdded(ClassificatorCustomFieldRow newField)
	{
		if (gridView.PostEditor())
		{
			gridView.CloseEditor();
			RefreshDataSource();
			int focusedRowHandle = gridView.FindRow(newField);
			gridView.FocusedRowHandle = focusedRowHandle;
			gridView.FocusedColumn = nameGridColumn;
			gridView.Focus();
			gridView.ShowEditor();
		}
	}

	public void DeleteFocusedField()
	{
		if (!gridView.PostEditor())
		{
			return;
		}
		gridView.CloseEditor();
		if (gridView.GetFocusedRow() is ClassificatorCustomFieldRow classificatorCustomFieldRow)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("Are you sure you want to delete the Field <b>" + classificatorCustomFieldRow.Title + "</b>?", "Delete Classification Field", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 2, FindForm());
			if (handlingDialogResult != null && handlingDialogResult.DialogResult == DialogResult.Yes)
			{
				classificatorModelRow.DeleteField(classificatorCustomFieldRow);
				RefreshDataSource();
			}
		}
	}

	public void RefreshDataSource()
	{
		gridControl.RefreshDataSource();
	}

	public bool PostEditor()
	{
		return gridView.PostEditor();
	}

	public void CloseEditor()
	{
		gridView.CloseEditor();
	}

	public bool IsEditorFocused()
	{
		if (!gridView.IsEditorFocused)
		{
			return !gridView.PostEditor();
		}
		return true;
	}

	private void GridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
		_ = e.Menu;
	}

	private void GridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		GridViewHelpers.GrayOutNoneditableColumns(sender as GridView, e);
	}

	private void GridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (e.Column == nameGridColumn)
		{
			this.ClassificationFieldNameChanged?.Invoke();
		}
		(gridView.GetRow(e.RowHandle) as ClassificatorCustomFieldRow)?.SetUpdatedIfNotAdded();
		classificatorModelRow.SetUpdatedIfNotAdded();
	}

	private void AdditionalLabelsRepositoryItemTextEdit_Validating(object sender, CancelEventArgs e)
	{
		TextEdit textEdit = sender as TextEdit;
		string text = textEdit.Text;
		if (!string.IsNullOrEmpty(text))
		{
			IEnumerable<string> values = (from x in text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				select x.Trim() into x
				where !string.IsNullOrWhiteSpace(x)
				select x).Distinct();
			textEdit.Text = string.Join(", ", values);
		}
	}

	private void GridView_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		if ((sender as GridView)?.FocusedColumn == nameGridColumn)
		{
			ClassificatorCustomFieldRow focusedRow = gridView.GetFocusedRow() as ClassificatorCustomFieldRow;
			if (e.Value == null || string.IsNullOrWhiteSpace((string)e.Value))
			{
				e.ErrorText = "The name cannot be empty.";
				e.Valid = false;
			}
			else if (classificatorModelRow.Fields.Where((ClassificatorCustomFieldRow x) => x.Title == (string)e.Value && x != focusedRow).Count() > 0)
			{
				e.ErrorText = "Field with name '" + (string)e.Value + "' already exists in this classification.";
				e.Valid = false;
			}
			else if (takenCustomFieldsTitles.Where((string x) => x == (string)e.Value).Count() > 0)
			{
				e.ErrorText = "Custom Field with title '" + (string)e.Value + "' already exists in the repository.";
				e.Valid = false;
			}
		}
	}

	private void GridView_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		e.ExceptionMode = ExceptionMode.DisplayError;
	}

	private void GridView_ShownEditor(object sender, EventArgs e)
	{
		if (sender is GridView gridView && gridView.ActiveEditor is TextEdit textEdit && gridView.FocusedColumn == nameGridColumn)
		{
			textEdit.Properties.MaxLength = 250;
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
		this.maxInfoLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.gridControl = new DevExpress.XtraGrid.GridControl();
		this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.nameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.classGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.classRepositoryItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.fieldTypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.labelsFromMasksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.additionalLabelsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.additionalLabelsRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.maxInfoLyoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.classRepositoryItemLookUpEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.additionalLabelsRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.maxInfoLyoutControlItem).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.maxInfoLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.gridControl);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(881, 16, 650, 400);
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(532, 281);
		this.nonCustomizableLayoutControl.TabIndex = 0;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.maxInfoLabelControl.Appearance.ForeColor = System.Drawing.Color.Silver;
		this.maxInfoLabelControl.Appearance.Options.UseForeColor = true;
		this.maxInfoLabelControl.Location = new System.Drawing.Point(373, 256);
		this.maxInfoLabelControl.Name = "maxInfoLabelControl";
		this.maxInfoLabelControl.Size = new System.Drawing.Size(147, 13);
		this.maxInfoLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.maxInfoLabelControl.TabIndex = 5;
		this.maxInfoLabelControl.Text = "Maximum 5 Classification Fields";
		this.gridControl.Location = new System.Drawing.Point(12, 28);
		this.gridControl.MainView = this.gridView;
		this.gridControl.Name = "gridControl";
		this.gridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.classRepositoryItemLookUpEdit, this.additionalLabelsRepositoryItemTextEdit });
		this.gridControl.Size = new System.Drawing.Size(508, 220);
		this.gridControl.TabIndex = 4;
		this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView });
		this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.nameGridColumn, this.classGridColumn, this.fieldTypeGridColumn, this.labelsFromMasksGridColumn, this.additionalLabelsGridColumn });
		this.gridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
		this.gridView.GridControl = this.gridControl;
		this.gridView.Name = "gridView";
		this.gridView.OptionsCustomization.AllowColumnMoving = false;
		this.gridView.OptionsCustomization.AllowFilter = false;
		this.gridView.OptionsCustomization.AllowGroup = false;
		this.gridView.OptionsCustomization.AllowQuickHideColumns = false;
		this.gridView.OptionsFind.AllowFindPanel = false;
		this.gridView.OptionsMenu.EnableFooterMenu = false;
		this.gridView.OptionsMenu.EnableGroupPanelMenu = false;
		this.gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gridView.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;
		this.gridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.gridView.OptionsView.ShowGroupPanel = false;
		this.gridView.OptionsView.ShowIndicator = false;
		this.gridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(GridView_CustomDrawCell);
		this.gridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(GridView_PopupMenuShowing);
		this.gridView.ShownEditor += new System.EventHandler(GridView_ShownEditor);
		this.gridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(GridView_CellValueChanged);
		this.gridView.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(GridView_ValidatingEditor);
		this.gridView.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(GridView_InvalidValueException);
		this.nameGridColumn.Caption = "Name";
		this.nameGridColumn.FieldName = "Title";
		this.nameGridColumn.Name = "nameGridColumn";
		this.nameGridColumn.Visible = true;
		this.nameGridColumn.VisibleIndex = 0;
		this.classGridColumn.Caption = "Class";
		this.classGridColumn.ColumnEdit = this.classRepositoryItemLookUpEdit;
		this.classGridColumn.FieldName = "CustomFieldClassId";
		this.classGridColumn.Name = "classGridColumn";
		this.classGridColumn.Visible = true;
		this.classGridColumn.VisibleIndex = 1;
		this.classRepositoryItemLookUpEdit.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
		this.classRepositoryItemLookUpEdit.AutoHeight = false;
		this.classRepositoryItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.classRepositoryItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.classRepositoryItemLookUpEdit.DisplayMember = "Name";
		this.classRepositoryItemLookUpEdit.Name = "classRepositoryItemLookUpEdit";
		this.classRepositoryItemLookUpEdit.NullText = "";
		this.classRepositoryItemLookUpEdit.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.OnlyInPopup;
		this.classRepositoryItemLookUpEdit.ShowFooter = false;
		this.classRepositoryItemLookUpEdit.ShowHeader = false;
		this.classRepositoryItemLookUpEdit.ShowLines = false;
		this.classRepositoryItemLookUpEdit.ValueMember = "CustomFieldClassId";
		this.fieldTypeGridColumn.Caption = "Field Type";
		this.fieldTypeGridColumn.FieldName = "CustomFieldFieldType";
		this.fieldTypeGridColumn.Name = "fieldTypeGridColumn";
		this.fieldTypeGridColumn.OptionsColumn.AllowEdit = false;
		this.fieldTypeGridColumn.Visible = true;
		this.fieldTypeGridColumn.VisibleIndex = 2;
		this.labelsFromMasksGridColumn.Caption = "Labels from Masks";
		this.labelsFromMasksGridColumn.FieldName = "LabelsFromMasks";
		this.labelsFromMasksGridColumn.Name = "labelsFromMasksGridColumn";
		this.labelsFromMasksGridColumn.OptionsColumn.AllowEdit = false;
		this.labelsFromMasksGridColumn.Visible = true;
		this.labelsFromMasksGridColumn.VisibleIndex = 3;
		this.additionalLabelsGridColumn.Caption = "Additional labels";
		this.additionalLabelsGridColumn.ColumnEdit = this.additionalLabelsRepositoryItemTextEdit;
		this.additionalLabelsGridColumn.FieldName = "AdditionalLabels";
		this.additionalLabelsGridColumn.Name = "additionalLabelsGridColumn";
		this.additionalLabelsGridColumn.Visible = true;
		this.additionalLabelsGridColumn.VisibleIndex = 4;
		this.additionalLabelsRepositoryItemTextEdit.AutoHeight = false;
		this.additionalLabelsRepositoryItemTextEdit.Name = "additionalLabelsRepositoryItemTextEdit";
		this.additionalLabelsRepositoryItemTextEdit.Validating += new System.ComponentModel.CancelEventHandler(AdditionalLabelsRepositoryItemTextEdit_Validating);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.layoutControlItem, this.maxInfoLyoutControlItem });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(532, 281);
		this.Root.TextVisible = false;
		this.layoutControlItem.Control = this.gridControl;
		this.layoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem.Name = "layoutControlItem";
		this.layoutControlItem.Size = new System.Drawing.Size(512, 240);
		this.layoutControlItem.Text = "Classification Fields (Custom Fields):";
		this.layoutControlItem.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem.TextSize = new System.Drawing.Size(168, 13);
		this.maxInfoLyoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.maxInfoLyoutControlItem.Control = this.maxInfoLabelControl;
		this.maxInfoLyoutControlItem.Location = new System.Drawing.Point(0, 240);
		this.maxInfoLyoutControlItem.Name = "maxInfoLyoutControlItem";
		this.maxInfoLyoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 6, 2);
		this.maxInfoLyoutControlItem.Size = new System.Drawing.Size(512, 21);
		this.maxInfoLyoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.maxInfoLyoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.Name = "ClassificationFieldsUserControl";
		base.Size = new System.Drawing.Size(532, 281);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.classRepositoryItemLookUpEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.additionalLabelsRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.maxInfoLyoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
