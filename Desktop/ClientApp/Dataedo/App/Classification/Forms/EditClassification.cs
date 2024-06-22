using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classification.UserControls;
using Dataedo.App.Classification.UserControls.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Helpers.Controls;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.DataProcessing.Synchronize.Classes;
using Dataedo.Model.Data.Classificator;
using Dataedo.Model.Data.CustomFields;
using Dataedo.Model.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Classification.Forms;

public class EditClassification : BaseXtraForm
{
	private ClassificatorModelRow classificatorModelRow;

	private DXErrorProvider errorProvider;

	private bool isInitializing;

	private CustomFieldsSupport customFieldsSupport;

	private List<CustomFieldClassRow> customFieldsClasses;

	private IEnumerable<ClassificatorModel> classificatorModels;

	private IEnumerable<string> takenCustomFieldsTitles;

	private IContainer components;

	private RibbonControl ribbonControl;

	private BarButtonItem addFieldBarButtonItem;

	private BarButtonItem removeFieldBarButtonItem;

	private RibbonPage ribbonPage;

	private RibbonPageGroup fieldsPageGroup;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private RibbonPageGroup classificationPageGroup;

	private BarButtonItem removeClassificationBarButtonItem;

	private SimpleButton cancelButton;

	private LayoutControlItem cancelButtonLayoutControlItem;

	private EmptySpaceItem bottomEmptySpaceItem;

	private SimpleButton saveButton;

	private LayoutControlItem saveButtonLayoutControlItem;

	private MemoEdit descriptionMemoEdit;

	private TextEdit nameTextEdit;

	private ClassificationRulesUserControl classificationRulesUserControl;

	private ClassificationFieldsUserControl classificationFieldsUserControl;

	private LayoutControlItem fieldsControlLayoutControlItem;

	private LayoutControlItem rulesControlLayoutControlItem;

	private LayoutControlItem descriptionLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	private LayoutControlItem titleLayoutControlItem;

	private SimpleSeparator bottomSeparator;

	private SimpleButton saveAndCloseButton;

	private LayoutControlItem saveAndCloseButtonLayoutControlItem;

	public int? EditedClassificatorId => classificatorModelRow.Id;

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams obj = base.CreateParams;
			obj.ExStyle |= 33554432;
			return obj;
		}
	}

	public EditClassification()
	{
		InitializeComponent();
		errorProvider = new DXErrorProvider();
		LengthValidation.SetDataTypeLength(nameTextEdit);
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.S | Keys.Control:
			Save(showInfo: true);
			break;
		case Keys.Escape:
			if (!classificationFieldsUserControl.IsEditorFocused() && !classificationRulesUserControl.IsEditorFocused())
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
		classificationFieldsUserControl.Focus();
	}

	public void SetParameters(ClassificatorModel classificatorModel, CustomFieldsSupport customFieldsSupport)
	{
		isInitializing = true;
		this.customFieldsSupport = customFieldsSupport;
		classificatorModels = DB.Classificator.GetClassificators();
		if (classificatorModel.Id == 0)
		{
			Text = "Add new Classification";
		}
		customFieldsClasses = (from x in DB.CustomField.GetCustomFieldClasses()
			where new string[2] { "DOMAIN", "CLASSIFICATION" }.Contains(x.Code)
			select new CustomFieldClassRow(x)).ToList();
		classificatorModelRow = new ClassificatorModelRow(classificatorModel);
		takenCustomFieldsTitles = DB.Classificator.GetFieldsTitlesForClassification().Except(classificatorModelRow?.Fields?.Select((ClassificatorCustomFieldRow x) => x.Title));
		classificationFieldsUserControl.SetParameters(classificatorModelRow, customFieldsClasses, takenCustomFieldsTitles);
		classificationRulesUserControl.SetParameters(classificatorModelRow);
		nameTextEdit.Text = classificatorModelRow.Title;
		descriptionMemoEdit.Text = classificatorModelRow.Description;
		classificationFieldsUserControl.ClassificationFieldNameChanged += ClassificationFieldsUserControl_ClassificationFieldNameChanged;
		classificationRulesUserControl.ValuesFromMasksChanged += ClassificationRulesUserControl_ValuesFromMasksChanged;
		SetButtonsEnability();
		isInitializing = false;
	}

	private void ClassificationRulesUserControl_ValuesFromMasksChanged()
	{
		classificationFieldsUserControl.RefreshDataSource();
	}

	private void ClassificationFieldsUserControl_ClassificationFieldNameChanged()
	{
		classificationRulesUserControl.RefreshColumnsHeaders();
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

	private bool Save(bool showInfo = false)
	{
		try
		{
			if (!classificationFieldsUserControl.PostEditor())
			{
				return false;
			}
			classificationRulesUserControl.CloseEditor();
			if (!ValidateEnteredData())
			{
				return false;
			}
			if (!classificatorModelRow.AnyChangesMade())
			{
				if (showInfo)
				{
					GeneralMessageBoxesHandling.Show("There are no changes to save.", "No changes made", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, this);
				}
				return true;
			}
			DB.Classificator.SaveClassification(classificatorModelRow, customFieldsSupport);
			classificatorModelRow.Fields.RemoveAll((ClassificatorCustomFieldRow x) => x.RowState == ManagingRowsEnum.ManagingRows.Deleted);
			classificatorModelRow.Rules.RemoveAll((ClassificationRuleRow x) => x.RowState == ManagingRowsEnum.ManagingRows.Deleted);
			classificatorModelRow.SetUnchanged();
			classificationRulesUserControl.RefreshManageRulesButton();
			if (showInfo)
			{
				GeneralMessageBoxesHandling.Show("Changes were successfully saved to the repository.", "Changes saved", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, this);
			}
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while saving classification", FindForm());
			return false;
		}
	}

	private bool ValidateEnteredData()
	{
		errorProvider.ClearErrors();
		if (string.IsNullOrWhiteSpace(classificatorModelRow.Title))
		{
			errorProvider.SetError(nameTextEdit, "Classification's name cannot be empty", ErrorType.Critical);
			return false;
		}
		if (classificatorModels.Where((ClassificatorModel x) => x.Id != classificatorModelRow.Id && x.Title == classificatorModelRow.Title).Any())
		{
			errorProvider.SetError(nameTextEdit, "The name '" + classificatorModelRow.Title + "' is already taken by another Classification.", ErrorType.Critical);
			return false;
		}
		return true;
	}

	private void AddFieldBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (classificationFieldsUserControl.PostEditor())
		{
			classificationFieldsUserControl.CloseEditor();
			string newFieldTitle = "New Field";
			int num = 1;
			while (takenCustomFieldsTitles.Where((string x) => x == newFieldTitle).Any() || classificatorModelRow.Fields.Any((ClassificatorCustomFieldRow x) => x.RowState != ManagingRowsEnum.ManagingRows.Deleted && x.Title == newFieldTitle))
			{
				newFieldTitle = $"New Field ({num++})";
			}
			ClassificatorCustomFieldRow classificatorCustomFieldRow = classificatorModelRow.AddNewField(customFieldsClasses.FirstOrDefault()?.CustomFieldClassId, newFieldTitle);
			if (classificatorCustomFieldRow != null)
			{
				classificationFieldsUserControl.FieldAdded(classificatorCustomFieldRow);
				classificationRulesUserControl.RefreshDataSource();
				classificationRulesUserControl.RefreshColumnsHeaders();
				SetButtonsEnability();
			}
		}
	}

	private void RemoveFieldBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		classificationFieldsUserControl.DeleteFocusedField();
		classificationRulesUserControl.RefreshDataSource();
		classificationRulesUserControl.RefreshColumnsHeaders();
		SetButtonsEnability();
	}

	private void SetButtonsEnability()
	{
		if (classificatorModelRow.UsedFields.Count() >= 5)
		{
			addFieldBarButtonItem.Enabled = false;
			ButtonsHelpers.AddSuperTip(addFieldBarButtonItem, "You have reached maximum number of Classification Fields.");
		}
		else
		{
			addFieldBarButtonItem.Enabled = true;
			addFieldBarButtonItem.SuperTip?.Items?.Clear();
		}
		if (classificatorModelRow.UsedFields.Count() == 0)
		{
			removeFieldBarButtonItem.Enabled = false;
			removeClassificationBarButtonItem.Enabled = true;
			removeClassificationBarButtonItem.SuperTip?.Items?.Clear();
		}
		else
		{
			removeFieldBarButtonItem.Enabled = true;
			removeClassificationBarButtonItem.Enabled = false;
			ButtonsHelpers.AddSuperTip(removeClassificationBarButtonItem, "Please remove all Classification Fields first.");
		}
	}

	private void RemoveClassificationBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("Are you sure you want to delete the Classification <b>" + classificatorModelRow.Title + "</b>?", "Delete Classification", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 2, this);
			if (handlingDialogResult != null && handlingDialogResult.DialogResult == DialogResult.Yes)
			{
				if (classificatorModelRow.Id != 0)
				{
					DB.Classificator.DeleteClassification(classificatorModelRow, customFieldsSupport);
				}
				classificatorModelRow.SetUnchanged();
				GeneralMessageBoxesHandling.Show("The <b>" + classificatorModelRow.Title + "</b> Classification was successfully deleted from the repository.", "Classification deleted", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, this);
				Close();
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while removing classification", FindForm());
		}
	}

	private void NameTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (!isInitializing)
		{
			errorProvider.SetError(nameTextEdit, string.Empty);
			classificatorModelRow.Title = nameTextEdit.Text;
			classificatorModelRow.SetUpdatedIfNotAdded();
		}
	}

	private void DescriptionMemoEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (!isInitializing)
		{
			classificatorModelRow.Description = descriptionMemoEdit.Text;
			classificatorModelRow.SetUpdatedIfNotAdded();
		}
	}

	private void EditClassification_FormClosing(object sender, FormClosingEventArgs e)
	{
		classificationFieldsUserControl.PostEditor();
		classificationRulesUserControl.CloseEditor();
		if (classificatorModelRow.AnyChangesMade())
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("There were some changes made. Are you sure you want to cancel?", "Cancel changes", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 2, this);
			if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
			{
				e.Cancel = true;
			}
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
		this.ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
		this.addFieldBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeFieldBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeClassificationBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.ribbonPage = new DevExpress.XtraBars.Ribbon.RibbonPage();
		this.fieldsPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
		this.classificationPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
		this.nonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.descriptionMemoEdit = new DevExpress.XtraEditors.MemoEdit();
		this.nameTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.classificationRulesUserControl = new Dataedo.App.Classification.UserControls.ClassificationRulesUserControl();
		this.classificationFieldsUserControl = new Dataedo.App.Classification.UserControls.ClassificationFieldsUserControl();
		this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
		this.saveButton = new DevExpress.XtraEditors.SimpleButton();
		this.saveAndCloseButton = new DevExpress.XtraEditors.SimpleButton();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.cancelButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.bottomEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.saveButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.fieldsControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.rulesControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.descriptionLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.titleLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.bottomSeparator = new DevExpress.XtraLayout.SimpleSeparator();
		this.saveAndCloseButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.ribbonControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.descriptionMemoEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cancelButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fieldsControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rulesControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomSeparator).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveAndCloseButtonLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.ribbonControl.AllowHtmlText = true;
		this.ribbonControl.AllowMinimizeRibbon = false;
		this.ribbonControl.ExpandCollapseItem.Id = 0;
		this.ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[5]
		{
			this.ribbonControl.ExpandCollapseItem,
			this.ribbonControl.SearchEditItem,
			this.addFieldBarButtonItem,
			this.removeFieldBarButtonItem,
			this.removeClassificationBarButtonItem
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
		this.ribbonControl.Size = new System.Drawing.Size(1013, 102);
		this.ribbonControl.Toolbar.ShowCustomizeItem = false;
		this.ribbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
		this.addFieldBarButtonItem.Caption = "Add";
		this.addFieldBarButtonItem.Id = 12;
		this.addFieldBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.add_16;
		this.addFieldBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.add_32;
		this.addFieldBarButtonItem.Name = "addFieldBarButtonItem";
		this.addFieldBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(AddFieldBarButtonItem_ItemClick);
		this.removeFieldBarButtonItem.Caption = "Remove";
		this.removeFieldBarButtonItem.Id = 13;
		this.removeFieldBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeFieldBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.delete_32;
		this.removeFieldBarButtonItem.Name = "removeFieldBarButtonItem";
		this.removeFieldBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(RemoveFieldBarButtonItem_ItemClick);
		this.removeClassificationBarButtonItem.Caption = "Delete\r\nClassification";
		this.removeClassificationBarButtonItem.Enabled = false;
		this.removeClassificationBarButtonItem.Id = 15;
		this.removeClassificationBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeClassificationBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.delete_32;
		this.removeClassificationBarButtonItem.Name = "removeClassificationBarButtonItem";
		this.removeClassificationBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(RemoveClassificationBarButtonItem_ItemClick);
		this.ribbonPage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[2] { this.fieldsPageGroup, this.classificationPageGroup });
		this.ribbonPage.Name = "ribbonPage";
		this.ribbonPage.Text = "ribbonPage1";
		this.fieldsPageGroup.AllowTextClipping = false;
		this.fieldsPageGroup.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.fieldsPageGroup.ItemLinks.Add(this.addFieldBarButtonItem);
		this.fieldsPageGroup.ItemLinks.Add(this.removeFieldBarButtonItem);
		this.fieldsPageGroup.Name = "fieldsPageGroup";
		this.fieldsPageGroup.Text = "Classification Fields";
		this.classificationPageGroup.ItemLinks.Add(this.removeClassificationBarButtonItem);
		this.classificationPageGroup.Name = "classificationPageGroup";
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.descriptionMemoEdit);
		this.nonCustomizableLayoutControl.Controls.Add(this.nameTextEdit);
		this.nonCustomizableLayoutControl.Controls.Add(this.classificationRulesUserControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.classificationFieldsUserControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.cancelButton);
		this.nonCustomizableLayoutControl.Controls.Add(this.saveButton);
		this.nonCustomizableLayoutControl.Controls.Add(this.saveAndCloseButton);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 102);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1150, 216, 650, 400);
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(1013, 550);
		this.nonCustomizableLayoutControl.TabIndex = 2;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.descriptionMemoEdit.Location = new System.Drawing.Point(137, 32);
		this.descriptionMemoEdit.MenuManager = this.ribbonControl;
		this.descriptionMemoEdit.Name = "descriptionMemoEdit";
		this.descriptionMemoEdit.Size = new System.Drawing.Size(519, 49);
		this.descriptionMemoEdit.StyleController = this.nonCustomizableLayoutControl;
		this.descriptionMemoEdit.TabIndex = 12;
		this.descriptionMemoEdit.EditValueChanged += new System.EventHandler(DescriptionMemoEdit_EditValueChanged);
		this.nameTextEdit.Location = new System.Drawing.Point(136, 5);
		this.nameTextEdit.MenuManager = this.ribbonControl;
		this.nameTextEdit.Name = "nameTextEdit";
		this.nameTextEdit.Size = new System.Drawing.Size(520, 20);
		this.nameTextEdit.StyleController = this.nonCustomizableLayoutControl;
		this.nameTextEdit.TabIndex = 11;
		this.nameTextEdit.EditValueChanged += new System.EventHandler(NameTextEdit_EditValueChanged);
		this.classificationRulesUserControl.Location = new System.Drawing.Point(2, 282);
		this.classificationRulesUserControl.Name = "classificationRulesUserControl";
		this.classificationRulesUserControl.Size = new System.Drawing.Size(1009, 231);
		this.classificationRulesUserControl.TabIndex = 10;
		this.classificationFieldsUserControl.Location = new System.Drawing.Point(2, 85);
		this.classificationFieldsUserControl.Name = "classificationFieldsUserControl";
		this.classificationFieldsUserControl.Size = new System.Drawing.Size(1009, 193);
		this.classificationFieldsUserControl.TabIndex = 9;
		this.cancelButton.AllowFocus = false;
		this.cancelButton.Location = new System.Drawing.Point(927, 522);
		this.cancelButton.Margin = new System.Windows.Forms.Padding(10);
		this.cancelButton.Name = "cancelButton";
		this.cancelButton.Size = new System.Drawing.Size(72, 22);
		this.cancelButton.StyleController = this.nonCustomizableLayoutControl;
		this.cancelButton.TabIndex = 8;
		this.cancelButton.Text = "Cancel";
		this.cancelButton.Click += new System.EventHandler(CancelButton_Click);
		this.saveButton.AllowFocus = false;
		this.saveButton.Location = new System.Drawing.Point(711, 522);
		this.saveButton.Margin = new System.Windows.Forms.Padding(10);
		this.saveButton.Name = "saveButton";
		this.saveButton.Size = new System.Drawing.Size(72, 22);
		this.saveButton.StyleController = this.nonCustomizableLayoutControl;
		this.saveButton.TabIndex = 8;
		this.saveButton.Text = "Save";
		this.saveButton.Click += new System.EventHandler(SaveButton_Click);
		this.saveAndCloseButton.AllowFocus = false;
		this.saveAndCloseButton.Location = new System.Drawing.Point(797, 522);
		this.saveAndCloseButton.Margin = new System.Windows.Forms.Padding(10);
		this.saveAndCloseButton.Name = "saveAndCloseButton";
		this.saveAndCloseButton.Size = new System.Drawing.Size(116, 22);
		this.saveAndCloseButton.StyleController = this.nonCustomizableLayoutControl;
		this.saveAndCloseButton.TabIndex = 8;
		this.saveAndCloseButton.Text = "Save and Close";
		this.saveAndCloseButton.Click += new System.EventHandler(SaveAndCloseButton_Click);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[10] { this.cancelButtonLayoutControlItem, this.bottomEmptySpaceItem, this.saveButtonLayoutControlItem, this.fieldsControlLayoutControlItem, this.rulesControlLayoutControlItem, this.descriptionLayoutControlItem, this.emptySpaceItem1, this.titleLayoutControlItem, this.bottomSeparator, this.saveAndCloseButtonLayoutControlItem });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(1013, 550);
		this.Root.TextVisible = false;
		this.cancelButtonLayoutControlItem.Control = this.cancelButton;
		this.cancelButtonLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.cancelButtonLayoutControlItem.CustomizationFormText = "runClassificationButtonLayoutControlItem";
		this.cancelButtonLayoutControlItem.Location = new System.Drawing.Point(927, 516);
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
		this.bottomEmptySpaceItem.Location = new System.Drawing.Point(0, 516);
		this.bottomEmptySpaceItem.Name = "bottomEmptySpaceItem";
		this.bottomEmptySpaceItem.Size = new System.Drawing.Size(697, 34);
		this.bottomEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveButtonLayoutControlItem.Control = this.saveButton;
		this.saveButtonLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.saveButtonLayoutControlItem.CustomizationFormText = "runClassificationButtonLayoutControlItem";
		this.saveButtonLayoutControlItem.Location = new System.Drawing.Point(697, 516);
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
		this.fieldsControlLayoutControlItem.Control = this.classificationFieldsUserControl;
		this.fieldsControlLayoutControlItem.Location = new System.Drawing.Point(0, 83);
		this.fieldsControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 197);
		this.fieldsControlLayoutControlItem.MinSize = new System.Drawing.Size(104, 197);
		this.fieldsControlLayoutControlItem.Name = "fieldsControlLayoutControlItem";
		this.fieldsControlLayoutControlItem.Size = new System.Drawing.Size(1013, 197);
		this.fieldsControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.fieldsControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.fieldsControlLayoutControlItem.TextVisible = false;
		this.rulesControlLayoutControlItem.Control = this.classificationRulesUserControl;
		this.rulesControlLayoutControlItem.Location = new System.Drawing.Point(0, 280);
		this.rulesControlLayoutControlItem.Name = "rulesControlLayoutControlItem";
		this.rulesControlLayoutControlItem.Size = new System.Drawing.Size(1013, 235);
		this.rulesControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.rulesControlLayoutControlItem.TextVisible = false;
		this.descriptionLayoutControlItem.AppearanceItemCaption.Options.UseTextOptions = true;
		this.descriptionLayoutControlItem.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
		this.descriptionLayoutControlItem.Control = this.descriptionMemoEdit;
		this.descriptionLayoutControlItem.Location = new System.Drawing.Point(0, 30);
		this.descriptionLayoutControlItem.MaxSize = new System.Drawing.Size(658, 53);
		this.descriptionLayoutControlItem.MinSize = new System.Drawing.Size(658, 53);
		this.descriptionLayoutControlItem.Name = "descriptionLayoutControlItem";
		this.descriptionLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(15, 2, 2, 2);
		this.descriptionLayoutControlItem.Size = new System.Drawing.Size(658, 53);
		this.descriptionLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.descriptionLayoutControlItem.Text = "Description:";
		this.descriptionLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.descriptionLayoutControlItem.TextSize = new System.Drawing.Size(107, 13);
		this.descriptionLayoutControlItem.TextToControlDistance = 15;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(658, 0);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(355, 83);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.titleLayoutControlItem.Control = this.nameTextEdit;
		this.titleLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.titleLayoutControlItem.MaxSize = new System.Drawing.Size(658, 30);
		this.titleLayoutControlItem.MinSize = new System.Drawing.Size(658, 30);
		this.titleLayoutControlItem.Name = "titleLayoutControlItem";
		this.titleLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(14, 2, 5, 5);
		this.titleLayoutControlItem.Size = new System.Drawing.Size(658, 30);
		this.titleLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.titleLayoutControlItem.Text = "Classification Name:";
		this.titleLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.titleLayoutControlItem.TextSize = new System.Drawing.Size(107, 13);
		this.titleLayoutControlItem.TextToControlDistance = 15;
		this.bottomSeparator.AllowHotTrack = false;
		this.bottomSeparator.Location = new System.Drawing.Point(0, 515);
		this.bottomSeparator.Name = "bottomSeparator";
		this.bottomSeparator.Size = new System.Drawing.Size(1013, 1);
		this.saveAndCloseButtonLayoutControlItem.Control = this.saveAndCloseButton;
		this.saveAndCloseButtonLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.saveAndCloseButtonLayoutControlItem.CustomizationFormText = "runClassificationButtonLayoutControlItem";
		this.saveAndCloseButtonLayoutControlItem.Location = new System.Drawing.Point(797, 516);
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
		base.ClientSize = new System.Drawing.Size(1013, 652);
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.Controls.Add(this.ribbonControl);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_32;
		base.Name = "EditClassification";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Edit Classification";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(EditClassification_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.ribbonControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.descriptionMemoEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cancelButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fieldsControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rulesControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomSeparator).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveAndCloseButtonLayoutControlItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
