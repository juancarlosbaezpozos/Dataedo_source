using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classification.UserControls.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Helpers.Controls;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Data.Classificator;
using Dataedo.Model.Enums;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Classification.Forms;

public class EditMask : BaseXtraForm
{
	private ClassificatorModelRow classificatorModelRow;

	private string originalMaskName;

	private int? classificatorId;

	private bool isInitializing;

	private ClassificationMaskRow editedMask;

	private readonly DXErrorProvider errorProvider;

	private IContainer components;

	private RibbonControl ribbonControl;

	private BarButtonItem addPatternBarButtonItem;

	private BarButtonItem removePatternBarButtonItem;

	private RibbonPage ribbonPage;

	private RibbonPageGroup patternsPageGroup;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private SimpleButton cancelButton;

	private LayoutControlItem cancelButtonLayoutControlItem;

	private EmptySpaceItem bottomEmptySpaceItem;

	private SimpleButton saveButton;

	private LayoutControlItem saveButtonLayoutControlItem;

	private SimpleSeparator bottomSeparator;

	private TextEdit maskNameTextEdit;

	private LayoutControlItem maskNameLayoutControlItem;

	private GridControl patternsGridControl;

	private GridView patternsGridView;

	private LayoutControlItem patternsGridLayoutControlItem;

	private CheckedListBoxControl classificationsCheckedListBoxControl;

	private LayoutControlItem classificationsLayoutControlItem;

	private GridColumn patternGridColumn;

	private GridColumn dataTypesGridColumn;

	private GridColumn isColumnGridColumn;

	private GridColumn isTitleGridColumn;

	private GridColumn isDescriptionGridColumn;

	private ToolTipController toolTipController;

	private SimpleButton saveAndCloseButton;

	private LayoutControlItem saveAndCloseButtonLayoutControlItem;

	private RepositoryItemCheckedComboBoxEdit dataTypesRepositoryItemCheckedComboBoxEdit;

	public string LastSavedMaskName => originalMaskName;

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams obj = base.CreateParams;
			obj.ExStyle |= 33554432;
			return obj;
		}
	}

	public EditMask()
	{
		InitializeComponent();
		errorProvider = new DXErrorProvider();
		LengthValidation.SetDataTypeLength(maskNameTextEdit);
		dataTypesRepositoryItemCheckedComboBoxEdit.DataSource = new List<string> { "date", "number", "text" };
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.S | Keys.Control:
			if (saveButton.Enabled)
			{
				Save(showInfo: true);
			}
			break;
		case Keys.Escape:
			if (!patternsGridView.IsEditorFocused)
			{
				Close();
			}
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	protected override void OnShown(EventArgs e)
	{
		base.OnShown(e);
		classificationsCheckedListBoxControl.Focus();
	}

	public void SetParameters(string maskName, int? classificatorId, bool addingNewMask = false, ClassificatorModelRow classificatorModelRow = null)
	{
		isInitializing = true;
		this.classificatorId = classificatorId;
		this.classificatorModelRow = classificatorModelRow;
		if (addingNewMask)
		{
			Text = "Add new Mask";
			editedMask = new ClassificationMaskRow
			{
				MaskName = maskName,
				RowState = ManagingRowsEnum.ManagingRows.Added
			};
			IOrderedEnumerable<ClassificatorModel> classificatorModels = from x in DB.Classificator.GetClassificators()
				orderby x.Title
				select x;
			editedMask.SetClassificatorsInPresence(classificatorModels, this.classificatorId);
		}
		else
		{
			originalMaskName = maskName;
			editedMask = DB.Classificator.GetMaskFromRepository(originalMaskName, this.classificatorId);
			if (editedMask == null)
			{
				throw new ArgumentException("The Classification Mask '" + originalMaskName + "' does not exists in repository.");
			}
		}
		classificationsCheckedListBoxControl.DataSource = editedMask.PresenceInClassificators;
		patternsGridControl.DataSource = editedMask.Patterns;
		patternsGridView.ActiveFilter.NonColumnFilter = new BinaryOperator("RowState", ManagingRowsEnum.ManagingRows.Deleted, BinaryOperatorType.NotEqual).ToString();
		maskNameTextEdit.Text = editedMask.MaskName;
		patternGridColumn.SortOrder = ColumnSortOrder.Ascending;
		SetButtonsEnability();
		isInitializing = false;
	}

	private void AddPatternBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (patternsGridView.PostEditor())
		{
			patternsGridView.CloseEditor();
			ClassificationMaskPatternRow row = editedMask.AddNewPattern();
			patternsGridControl.RefreshDataSource();
			int focusedRowHandle = patternsGridView.FindRow(row);
			patternsGridView.FocusedRowHandle = focusedRowHandle;
			patternsGridView.FocusedColumn = patternGridColumn;
			patternsGridView.Focus();
			patternsGridView.ShowEditor();
			SetButtonsEnability();
		}
	}

	private void RemovePatternBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (!patternsGridView.PostEditor())
		{
			return;
		}
		patternsGridView.CloseEditor();
		if (patternsGridView.GetFocusedRow() is ClassificationMaskPatternRow classificationMaskPatternRow)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("Are you sure you want to delete the Pattern <b>" + classificationMaskPatternRow.Mask + "</b>?", "Delete Mask Pattern", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 2, this);
			if (handlingDialogResult != null && handlingDialogResult.DialogResult == DialogResult.Yes)
			{
				editedMask.DeletePattern(classificationMaskPatternRow);
				patternsGridControl.RefreshDataSource();
				SetButtonsEnability();
			}
		}
	}

	private void SaveButton_Click(object sender, EventArgs e)
	{
		Save(showInfo: true);
	}

	private void SaveAndCloseButton_Click(object sender, EventArgs e)
	{
		if (Save())
		{
			Close();
		}
	}

	private void CancelButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void ClassificationsCheckedListBoxControl_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
	{
		if (classificationsCheckedListBoxControl.GetItem(e.Index) is Dataedo.App.Classification.UserControls.Classes.ClassificationMaskPresence classificationMaskPresence)
		{
			classificationMaskPresence.IsChanged = true;
			editedMask.SetUpdatedIfNotAdded();
		}
	}

	private void ClassificationsCheckedListBoxControl_GetItemEnabled(object sender, GetItemEnabledEventArgs e)
	{
		if (!classificatorId.HasValue)
		{
			e.Enabled = true;
			return;
		}
		Dataedo.App.Classification.UserControls.Classes.ClassificationMaskPresence classificationMaskPresence = classificationsCheckedListBoxControl.GetItem(e.Index) as Dataedo.App.Classification.UserControls.Classes.ClassificationMaskPresence;
		e.Enabled = classificationMaskPresence.ClassificatorId == classificatorId.Value;
	}

	private void PatternsGridView_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		GridColumn gridColumn = (sender as GridView)?.FocusedColumn;
		if (gridColumn == patternGridColumn)
		{
			if (e.Value == null || string.IsNullOrWhiteSpace((string)e.Value))
			{
				e.ErrorText = "The pattern cannot be empty.";
				e.Valid = false;
			}
			else if (new Regex("^[\\s%]{1,}$").IsMatch((string)e.Value))
			{
				e.ErrorText = "The pattern cannot consists only of the percent signs and whitespace characters.";
				e.Valid = false;
			}
			else if (editedMask.Patterns.Where((ClassificationMaskPatternRow x) => x.Mask == (string)e.Value && x.RowState != ManagingRowsEnum.ManagingRows.Deleted && x != patternsGridView.GetFocusedRow()).Any())
			{
				e.ErrorText = "This pattern already exists in this mask.";
				e.Valid = false;
			}
		}
		else if (gridColumn == dataTypesGridColumn && (e.Value == null || string.IsNullOrWhiteSpace((string)e.Value)))
		{
			e.ErrorText = "Pattern's data types must not be empty.";
			e.Valid = false;
		}
	}

	private void PatternsGridView_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		e.ExceptionMode = ExceptionMode.DisplayError;
	}

	private void MaskNameTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (!isInitializing)
		{
			errorProvider.SetError(maskNameTextEdit, string.Empty);
			editedMask.MaskName = maskNameTextEdit.Text;
			editedMask.SetUpdatedIfNotAdded();
		}
	}

	private void SetButtonsEnability()
	{
		if (patternsGridView.RowCount == 0)
		{
			removePatternBarButtonItem.Enabled = false;
			saveButton.Enabled = false;
			ButtonsHelpers.AddSuperTip(saveButton, "Classification Mask must have at least one pattern.");
			saveAndCloseButton.Enabled = false;
			ButtonsHelpers.AddSuperTip(saveAndCloseButton, "Classification Mask must have at least one pattern.");
		}
		else
		{
			removePatternBarButtonItem.Enabled = true;
			saveButton.Enabled = true;
			saveButton.SuperTip?.Items?.Clear();
			saveAndCloseButton.Enabled = true;
			saveAndCloseButton.SuperTip?.Items?.Clear();
		}
	}

	private bool Save(bool showInfo = false)
	{
		try
		{
			if (!ValidateEnteredData())
			{
				return false;
			}
			if (!editedMask.AnyChangesMade())
			{
				if (showInfo)
				{
					GeneralMessageBoxesHandling.Show("There are no changes to save.", "No changes made", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, this);
				}
				return true;
			}
			DB.Classificator.SaveClassificationMask(editedMask, originalMaskName);
			if (classificatorModelRow != null && !string.IsNullOrEmpty(originalMaskName))
			{
				classificatorModelRow.Rules.Where((ClassificationRuleRow x) => x.Mask.MaskName == originalMaskName).ForEach(delegate(ClassificationRuleRow x)
				{
					x.Mask.MaskName = editedMask.MaskName;
				});
			}
			originalMaskName = editedMask.MaskName;
			editedMask.Patterns.RemoveAll((ClassificationMaskPatternRow x) => x.RowState == ManagingRowsEnum.ManagingRows.Deleted);
			editedMask.SetUnchanged();
			if (showInfo)
			{
				GeneralMessageBoxesHandling.Show("Changes were successfully saved to the repository.", "Changes saved", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, this);
			}
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while saving classification mask", FindForm());
			return false;
		}
	}

	private bool ValidateEnteredData()
	{
		errorProvider.ClearErrors();
		if (!patternsGridView.PostEditor())
		{
			return false;
		}
		if (patternsGridView.RowCount == 0)
		{
			GeneralMessageBoxesHandling.Show("Classification Mask must have at least one pattern.", "No patterns detected", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, this);
			return false;
		}
		if (string.IsNullOrWhiteSpace(editedMask.MaskName))
		{
			errorProvider.SetError(maskNameTextEdit, "Classification Mask's name cannot be empty", ErrorType.Critical);
			return false;
		}
		if ((editedMask.RowState == ManagingRowsEnum.ManagingRows.Added || originalMaskName != editedMask.MaskName) && DB.Classificator.CheckIfMaskExists(editedMask.MaskName))
		{
			errorProvider.SetError(maskNameTextEdit, "This name is already taken by another mask.", ErrorType.Critical);
			return false;
		}
		return true;
	}

	private void NonCustomizableLayoutControl_MouseMove(object sender, MouseEventArgs e)
	{
		Control childAtPoint = nonCustomizableLayoutControl.GetChildAtPoint(e.Location);
		if (!saveButton.Enabled)
		{
			if (childAtPoint == saveButton)
			{
				ToolTipControllerShowEventArgs eShow = new ToolTipControllerShowEventArgs
				{
					SuperTip = saveButton.SuperTip,
					ToolTipType = ToolTipType.SuperTip
				};
				toolTipController.ShowHint(eShow, Control.MousePosition);
			}
			else if (childAtPoint == saveAndCloseButton)
			{
				ToolTipControllerShowEventArgs eShow2 = new ToolTipControllerShowEventArgs
				{
					SuperTip = saveAndCloseButton.SuperTip,
					ToolTipType = ToolTipType.SuperTip
				};
				toolTipController.ShowHint(eShow2, Control.MousePosition);
			}
			else
			{
				toolTipController.HideHint();
			}
		}
	}

	private void PatternsGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		(patternsGridView.GetRow(e.RowHandle) as ClassificationMaskPatternRow)?.SetUpdatedIfNotAdded();
		editedMask.SetUpdatedIfNotAdded();
	}

	private void EditMask_FormClosing(object sender, FormClosingEventArgs e)
	{
		patternsGridView.PostEditor();
		if (editedMask.AnyChangesMade())
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("There were some changes made. Are you sure you want to cancel?", "Cancel changes", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 2, this);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
			{
				e.Cancel = true;
			}
		}
	}

	private void PatternsGridView_ShownEditor(object sender, EventArgs e)
	{
		if (sender is GridView gridView && gridView.ActiveEditor is TextEdit textEdit)
		{
			GridColumn focusedColumn = gridView.FocusedColumn;
			if (focusedColumn == patternGridColumn || focusedColumn == dataTypesGridColumn)
			{
				textEdit.Properties.MaxLength = 250;
			}
		}
	}

	private void PatternsGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
		_ = e.Menu;
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
		this.components = new System.ComponentModel.Container();
		this.ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
		this.addPatternBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removePatternBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.ribbonPage = new DevExpress.XtraBars.Ribbon.RibbonPage();
		this.patternsPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
		this.nonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.classificationsCheckedListBoxControl = new DevExpress.XtraEditors.CheckedListBoxControl();
		this.patternsGridControl = new DevExpress.XtraGrid.GridControl();
		this.patternsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.patternGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.dataTypesGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.dataTypesRepositoryItemCheckedComboBoxEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
		this.isColumnGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.isTitleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.isDescriptionGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.maskNameTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
		this.saveButton = new DevExpress.XtraEditors.SimpleButton();
		this.saveAndCloseButton = new DevExpress.XtraEditors.SimpleButton();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.cancelButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.bottomEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.saveButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.bottomSeparator = new DevExpress.XtraLayout.SimpleSeparator();
		this.maskNameLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.patternsGridLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.classificationsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.saveAndCloseButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		((System.ComponentModel.ISupportInitialize)this.ribbonControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.classificationsCheckedListBoxControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.patternsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.patternsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataTypesRepositoryItemCheckedComboBoxEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.maskNameTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cancelButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomSeparator).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.maskNameLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.patternsGridLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.classificationsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveAndCloseButtonLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.ribbonControl.AllowMinimizeRibbon = false;
		this.ribbonControl.ExpandCollapseItem.Id = 0;
		this.ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[4]
		{
			this.ribbonControl.ExpandCollapseItem,
			this.ribbonControl.SearchEditItem,
			this.addPatternBarButtonItem,
			this.removePatternBarButtonItem
		});
		this.ribbonControl.Location = new System.Drawing.Point(0, 0);
		this.ribbonControl.Margin = new System.Windows.Forms.Padding(0);
		this.ribbonControl.MaxItemId = 16;
		this.ribbonControl.Name = "ribbonControl";
		this.ribbonControl.OptionsPageCategories.ShowCaptions = false;
		this.ribbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[1] { this.ribbonPage });
		this.ribbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
		this.ribbonControl.ShowToolbarCustomizeItem = false;
		this.ribbonControl.Size = new System.Drawing.Size(800, 100);
		this.ribbonControl.Toolbar.ShowCustomizeItem = false;
		this.ribbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
		this.addPatternBarButtonItem.Caption = "Add";
		this.addPatternBarButtonItem.Id = 12;
		this.addPatternBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.add_16;
		this.addPatternBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.add_32;
		this.addPatternBarButtonItem.Name = "addPatternBarButtonItem";
		this.addPatternBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(AddPatternBarButtonItem_ItemClick);
		this.removePatternBarButtonItem.Caption = "Remove";
		this.removePatternBarButtonItem.Id = 13;
		this.removePatternBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removePatternBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.delete_32;
		this.removePatternBarButtonItem.Name = "removePatternBarButtonItem";
		this.removePatternBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(RemovePatternBarButtonItem_ItemClick);
		this.ribbonPage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[1] { this.patternsPageGroup });
		this.ribbonPage.Name = "ribbonPage";
		this.ribbonPage.Text = "ribbonPage1";
		this.patternsPageGroup.AllowTextClipping = false;
		this.patternsPageGroup.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.patternsPageGroup.ItemLinks.Add(this.addPatternBarButtonItem);
		this.patternsPageGroup.ItemLinks.Add(this.removePatternBarButtonItem);
		this.patternsPageGroup.Name = "patternsPageGroup";
		this.patternsPageGroup.Text = "Patterns";
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.classificationsCheckedListBoxControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.patternsGridControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.maskNameTextEdit);
		this.nonCustomizableLayoutControl.Controls.Add(this.cancelButton);
		this.nonCustomizableLayoutControl.Controls.Add(this.saveButton);
		this.nonCustomizableLayoutControl.Controls.Add(this.saveAndCloseButton);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 100);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1150, 216, 650, 400);
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(800, 350);
		this.nonCustomizableLayoutControl.TabIndex = 2;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl.MouseMove += new System.Windows.Forms.MouseEventHandler(NonCustomizableLayoutControl_MouseMove);
		this.classificationsCheckedListBoxControl.CheckMember = "IsPresent";
		this.classificationsCheckedListBoxControl.DisplayMember = "ClassificatorTitle";
		this.classificationsCheckedListBoxControl.Location = new System.Drawing.Point(117, 35);
		this.classificationsCheckedListBoxControl.Name = "classificationsCheckedListBoxControl";
		this.classificationsCheckedListBoxControl.Size = new System.Drawing.Size(668, 84);
		this.classificationsCheckedListBoxControl.StyleController = this.nonCustomizableLayoutControl;
		this.classificationsCheckedListBoxControl.TabIndex = 12;
		this.classificationsCheckedListBoxControl.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(ClassificationsCheckedListBoxControl_ItemCheck);
		this.classificationsCheckedListBoxControl.GetItemEnabled += new DevExpress.XtraEditors.Controls.GetItemEnabledEventHandler(ClassificationsCheckedListBoxControl_GetItemEnabled);
		this.patternsGridControl.Location = new System.Drawing.Point(15, 130);
		this.patternsGridControl.MainView = this.patternsGridView;
		this.patternsGridControl.MenuManager = this.ribbonControl;
		this.patternsGridControl.Name = "patternsGridControl";
		this.patternsGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.dataTypesRepositoryItemCheckedComboBoxEdit });
		this.patternsGridControl.Size = new System.Drawing.Size(770, 179);
		this.patternsGridControl.TabIndex = 11;
		this.patternsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.patternsGridView });
		this.patternsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.patternGridColumn, this.dataTypesGridColumn, this.isColumnGridColumn, this.isTitleGridColumn, this.isDescriptionGridColumn });
		this.patternsGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
		this.patternsGridView.GridControl = this.patternsGridControl;
		this.patternsGridView.Name = "patternsGridView";
		this.patternsGridView.OptionsCustomization.AllowColumnMoving = false;
		this.patternsGridView.OptionsCustomization.AllowFilter = false;
		this.patternsGridView.OptionsCustomization.AllowGroup = false;
		this.patternsGridView.OptionsCustomization.AllowQuickHideColumns = false;
		this.patternsGridView.OptionsFind.AllowFindPanel = false;
		this.patternsGridView.OptionsMenu.EnableFooterMenu = false;
		this.patternsGridView.OptionsMenu.EnableGroupPanelMenu = false;
		this.patternsGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.patternsGridView.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;
		this.patternsGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.patternsGridView.OptionsView.ShowGroupPanel = false;
		this.patternsGridView.OptionsView.ShowIndicator = false;
		this.patternsGridView.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
		this.patternsGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(PatternsGridView_PopupMenuShowing);
		this.patternsGridView.ShownEditor += new System.EventHandler(PatternsGridView_ShownEditor);
		this.patternsGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(PatternsGridView_CellValueChanged);
		this.patternsGridView.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(PatternsGridView_ValidatingEditor);
		this.patternsGridView.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(PatternsGridView_InvalidValueException);
		this.patternGridColumn.Caption = "Pattern";
		this.patternGridColumn.FieldName = "Mask";
		this.patternGridColumn.Name = "patternGridColumn";
		this.patternGridColumn.Visible = true;
		this.patternGridColumn.VisibleIndex = 0;
		this.patternGridColumn.Width = 192;
		this.dataTypesGridColumn.Caption = "Searched data types";
		this.dataTypesGridColumn.ColumnEdit = this.dataTypesRepositoryItemCheckedComboBoxEdit;
		this.dataTypesGridColumn.FieldName = "DataTypes";
		this.dataTypesGridColumn.Name = "dataTypesGridColumn";
		this.dataTypesGridColumn.OptionsColumn.AllowSize = false;
		this.dataTypesGridColumn.OptionsColumn.FixedWidth = true;
		this.dataTypesGridColumn.Visible = true;
		this.dataTypesGridColumn.VisibleIndex = 1;
		this.dataTypesGridColumn.Width = 120;
		this.dataTypesRepositoryItemCheckedComboBoxEdit.AutoHeight = false;
		this.dataTypesRepositoryItemCheckedComboBoxEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dataTypesRepositoryItemCheckedComboBoxEdit.Name = "dataTypesRepositoryItemCheckedComboBoxEdit";
		this.isColumnGridColumn.Caption = "Search in\nthe column name";
		this.isColumnGridColumn.FieldName = "IsColumn";
		this.isColumnGridColumn.Name = "isColumnGridColumn";
		this.isColumnGridColumn.OptionsColumn.AllowSize = false;
		this.isColumnGridColumn.OptionsColumn.FixedWidth = true;
		this.isColumnGridColumn.Visible = true;
		this.isColumnGridColumn.VisibleIndex = 2;
		this.isColumnGridColumn.Width = 100;
		this.isTitleGridColumn.Caption = "Search in\nthe column title";
		this.isTitleGridColumn.FieldName = "IsTitle";
		this.isTitleGridColumn.Name = "isTitleGridColumn";
		this.isTitleGridColumn.OptionsColumn.AllowSize = false;
		this.isTitleGridColumn.OptionsColumn.FixedWidth = true;
		this.isTitleGridColumn.Visible = true;
		this.isTitleGridColumn.VisibleIndex = 3;
		this.isTitleGridColumn.Width = 90;
		this.isDescriptionGridColumn.Caption = "Search in\nthe column description";
		this.isDescriptionGridColumn.FieldName = "IsDescription";
		this.isDescriptionGridColumn.Name = "isDescriptionGridColumn";
		this.isDescriptionGridColumn.OptionsColumn.AllowSize = false;
		this.isDescriptionGridColumn.OptionsColumn.FixedWidth = true;
		this.isDescriptionGridColumn.Visible = true;
		this.isDescriptionGridColumn.VisibleIndex = 4;
		this.isDescriptionGridColumn.Width = 125;
		this.maskNameTextEdit.Location = new System.Drawing.Point(117, 5);
		this.maskNameTextEdit.MenuManager = this.ribbonControl;
		this.maskNameTextEdit.Name = "maskNameTextEdit";
		this.maskNameTextEdit.Size = new System.Drawing.Size(668, 20);
		this.maskNameTextEdit.StyleController = this.nonCustomizableLayoutControl;
		this.maskNameTextEdit.TabIndex = 9;
		this.maskNameTextEdit.EditValueChanged += new System.EventHandler(MaskNameTextEdit_EditValueChanged);
		this.cancelButton.AllowFocus = false;
		this.cancelButton.Location = new System.Drawing.Point(714, 322);
		this.cancelButton.Margin = new System.Windows.Forms.Padding(10);
		this.cancelButton.Name = "cancelButton";
		this.cancelButton.Size = new System.Drawing.Size(72, 22);
		this.cancelButton.StyleController = this.nonCustomizableLayoutControl;
		this.cancelButton.TabIndex = 8;
		this.cancelButton.Text = "Cancel";
		this.cancelButton.Click += new System.EventHandler(CancelButton_Click);
		this.saveButton.AllowFocus = false;
		this.saveButton.Location = new System.Drawing.Point(498, 322);
		this.saveButton.Margin = new System.Windows.Forms.Padding(10);
		this.saveButton.Name = "saveButton";
		this.saveButton.Size = new System.Drawing.Size(72, 22);
		this.saveButton.StyleController = this.nonCustomizableLayoutControl;
		this.saveButton.TabIndex = 8;
		this.saveButton.Text = "Save";
		this.saveButton.Click += new System.EventHandler(SaveButton_Click);
		this.saveAndCloseButton.AllowFocus = false;
		this.saveAndCloseButton.Location = new System.Drawing.Point(584, 322);
		this.saveAndCloseButton.Margin = new System.Windows.Forms.Padding(10);
		this.saveAndCloseButton.Name = "saveAndCloseButton";
		this.saveAndCloseButton.Size = new System.Drawing.Size(116, 22);
		this.saveAndCloseButton.StyleController = this.nonCustomizableLayoutControl;
		this.saveAndCloseButton.TabIndex = 8;
		this.saveAndCloseButton.Text = "Save and Close";
		this.saveAndCloseButton.Click += new System.EventHandler(SaveAndCloseButton_Click);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[8] { this.cancelButtonLayoutControlItem, this.bottomEmptySpaceItem, this.saveButtonLayoutControlItem, this.bottomSeparator, this.maskNameLayoutControlItem, this.patternsGridLayoutControlItem, this.classificationsLayoutControlItem, this.saveAndCloseButtonLayoutControlItem });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(800, 350);
		this.Root.TextVisible = false;
		this.cancelButtonLayoutControlItem.Control = this.cancelButton;
		this.cancelButtonLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.cancelButtonLayoutControlItem.CustomizationFormText = "runClassificationButtonLayoutControlItem";
		this.cancelButtonLayoutControlItem.Location = new System.Drawing.Point(714, 316);
		this.cancelButtonLayoutControlItem.MaxSize = new System.Drawing.Size(86, 34);
		this.cancelButtonLayoutControlItem.MinSize = new System.Drawing.Size(86, 34);
		this.cancelButtonLayoutControlItem.Name = "cancelButtonLayoutControlItem";
		this.cancelButtonLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.cancelButtonLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.cancelButtonLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.cancelButtonLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.cancelButtonLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.cancelButtonLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.cancelButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 14, 6, 6);
		this.cancelButtonLayoutControlItem.Size = new System.Drawing.Size(86, 34);
		this.cancelButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cancelButtonLayoutControlItem.Text = "runClassificationButtonLayoutControlItem";
		this.cancelButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.cancelButtonLayoutControlItem.TextVisible = false;
		this.bottomEmptySpaceItem.AllowHotTrack = false;
		this.bottomEmptySpaceItem.Location = new System.Drawing.Point(0, 316);
		this.bottomEmptySpaceItem.Name = "bottomEmptySpaceItem";
		this.bottomEmptySpaceItem.Size = new System.Drawing.Size(484, 34);
		this.bottomEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveButtonLayoutControlItem.Control = this.saveButton;
		this.saveButtonLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.saveButtonLayoutControlItem.CustomizationFormText = "runClassificationButtonLayoutControlItem";
		this.saveButtonLayoutControlItem.Location = new System.Drawing.Point(484, 316);
		this.saveButtonLayoutControlItem.MaxSize = new System.Drawing.Size(100, 34);
		this.saveButtonLayoutControlItem.MinSize = new System.Drawing.Size(100, 34);
		this.saveButtonLayoutControlItem.Name = "saveButtonLayoutControlItem";
		this.saveButtonLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.saveButtonLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.saveButtonLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.saveButtonLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.saveButtonLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.saveButtonLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.saveButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(14, 14, 6, 6);
		this.saveButtonLayoutControlItem.Size = new System.Drawing.Size(100, 34);
		this.saveButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.saveButtonLayoutControlItem.Text = "runClassificationButtonLayoutControlItem";
		this.saveButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveButtonLayoutControlItem.TextVisible = false;
		this.bottomSeparator.AllowHotTrack = false;
		this.bottomSeparator.Location = new System.Drawing.Point(0, 315);
		this.bottomSeparator.Name = "bottomSeparator";
		this.bottomSeparator.Size = new System.Drawing.Size(800, 1);
		this.maskNameLayoutControlItem.Control = this.maskNameTextEdit;
		this.maskNameLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.maskNameLayoutControlItem.MaxSize = new System.Drawing.Size(0, 30);
		this.maskNameLayoutControlItem.MinSize = new System.Drawing.Size(158, 30);
		this.maskNameLayoutControlItem.Name = "maskNameLayoutControlItem";
		this.maskNameLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(15, 15, 5, 5);
		this.maskNameLayoutControlItem.Size = new System.Drawing.Size(800, 30);
		this.maskNameLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.maskNameLayoutControlItem.Text = "Mask Name:";
		this.maskNameLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.maskNameLayoutControlItem.TextSize = new System.Drawing.Size(87, 13);
		this.maskNameLayoutControlItem.TextToControlDistance = 15;
		this.patternsGridLayoutControlItem.Control = this.patternsGridControl;
		this.patternsGridLayoutControlItem.Location = new System.Drawing.Point(0, 124);
		this.patternsGridLayoutControlItem.Name = "patternsGridLayoutControlItem";
		this.patternsGridLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(15, 15, 6, 6);
		this.patternsGridLayoutControlItem.Size = new System.Drawing.Size(800, 191);
		this.patternsGridLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.patternsGridLayoutControlItem.TextVisible = false;
		this.classificationsLayoutControlItem.AppearanceItemCaption.Options.UseTextOptions = true;
		this.classificationsLayoutControlItem.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
		this.classificationsLayoutControlItem.Control = this.classificationsCheckedListBoxControl;
		this.classificationsLayoutControlItem.Location = new System.Drawing.Point(0, 30);
		this.classificationsLayoutControlItem.MinSize = new System.Drawing.Size(121, 10);
		this.classificationsLayoutControlItem.Name = "classificationsLayoutControlItem";
		this.classificationsLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(15, 15, 5, 5);
		this.classificationsLayoutControlItem.Size = new System.Drawing.Size(800, 94);
		this.classificationsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.classificationsLayoutControlItem.Text = "Classifications:";
		this.classificationsLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.classificationsLayoutControlItem.TextSize = new System.Drawing.Size(87, 13);
		this.classificationsLayoutControlItem.TextToControlDistance = 15;
		this.saveAndCloseButtonLayoutControlItem.Control = this.saveAndCloseButton;
		this.saveAndCloseButtonLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.saveAndCloseButtonLayoutControlItem.CustomizationFormText = "runClassificationButtonLayoutControlItem";
		this.saveAndCloseButtonLayoutControlItem.Location = new System.Drawing.Point(584, 316);
		this.saveAndCloseButtonLayoutControlItem.MaxSize = new System.Drawing.Size(130, 34);
		this.saveAndCloseButtonLayoutControlItem.MinSize = new System.Drawing.Size(130, 34);
		this.saveAndCloseButtonLayoutControlItem.Name = "saveAndCloseButtonLayoutControlItem";
		this.saveAndCloseButtonLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.saveAndCloseButtonLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.saveAndCloseButtonLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.saveAndCloseButtonLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.saveAndCloseButtonLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.saveAndCloseButtonLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.saveAndCloseButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 14, 6, 6);
		this.saveAndCloseButtonLayoutControlItem.Size = new System.Drawing.Size(130, 34);
		this.saveAndCloseButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.saveAndCloseButtonLayoutControlItem.Text = "runClassificationButtonLayoutControlItem";
		this.saveAndCloseButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveAndCloseButtonLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 450);
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.Controls.Add(this.ribbonControl);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_32;
		base.Name = "EditMask";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Edit Mask";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(EditMask_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.ribbonControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.classificationsCheckedListBoxControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.patternsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.patternsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataTypesRepositoryItemCheckedComboBoxEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.maskNameTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cancelButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomSeparator).EndInit();
		((System.ComponentModel.ISupportInitialize)this.maskNameLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.patternsGridLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.classificationsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveAndCloseButtonLayoutControlItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
